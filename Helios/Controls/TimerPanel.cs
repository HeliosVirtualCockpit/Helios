// Copyright 2020 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using System;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace GadrocsWorkshop.Helios.Controls
{
    /// <summary>
    /// This is a subclass of panel with the added behavior to
    /// hide the panel if no click / touch events have occured
    /// within a given time period
    /// </summary>

    [HeliosControl("Helios.Panel.Timer", "Timer Panel", "Panels",
        typeof(HeliosPanelRenderer))]
    public class TimerPanel : HeliosPanel, IWindowsPreviewInput
    {
        private bool _timerEnabled;
        private DispatcherTimer _timer;
        private HeliosValue _timerEnabledValue;
        private HeliosValue _timerIntervalDefaultValue;
        private HeliosValue _timerIntervalOneTimeValue;
//
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public TimerPanel() : base()
        {
            _timerEnabledValue = new HeliosValue(this, new BindingValue(false), "Timer", "Enable", "Indicates whether the timer used to hide the panel should run.", "True if the timer is enabled.", BindingValueUnits.Boolean);
            _timerEnabledValue.Execute += SetTimerEnabledAction_Execute;
            Values.Add(_timerEnabledValue);
            Actions.Add(_timerEnabledValue);

            _timerIntervalDefaultValue = new HeliosValue(this, new BindingValue(false), "Timer", "Default Interval", "Default time before the panel automatically hides.", "Positive numeric value in seconds.", BindingValueUnits.Numeric);
            _timerIntervalDefaultValue.Execute += SetTimerDefaultIntervalAction_Execute;
            Values.Add(_timerIntervalDefaultValue);
            Actions.Add(_timerIntervalDefaultValue);

            _timerIntervalOneTimeValue = new HeliosValue(this, new BindingValue(false), "Timer", "Once Only Interval", "Temporary interval to be used until the panel next hides.  The timer then returns to the deault interval.", "Positive numeric value in seconds.", BindingValueUnits.Numeric);
            _timerIntervalOneTimeValue.Execute += SetTimerOneTimeIntervalAction_Execute;
            Values.Add(_timerIntervalOneTimeValue);
            Actions.Add(_timerIntervalOneTimeValue);
        }
        #region Properties

        public bool TimerEnabled
        {
            get => _timerEnabled;
            set
            {
                if (!_timerEnabled.Equals(value))
                {
                    _timerEnabled = value;
                    OnPropertyChanged("TimerEnabled", value, !value, true);
                    OnDisplayUpdate();
                }
            }
        }

        /// <summary>
        /// backing field for property TimerInterval, contains
        /// time out after which this panel automatically closes if no input is received
        /// </summary>
        private double _timerInterval = 3d;
        
        /// <summary>
        /// Field to hold the time interval configured in Profile Editor and read from XML
        /// Even if the time interval is changed via the Timer Interval action, this value is 
        /// used to set the Timer Interval when the panel is unhidden this providing repeatable
        /// experience every time the panel is seen
        /// </summary>
        private double _configuredTimerInterval = 3d;

        /// <summary>
        /// minimum permissible time out
        /// </summary>
        private const double MINIMUM_TIME_OUT = 1d;

        /// <summary>
        /// time out after which this panel automatically closes if no input is received
        /// </summary>
        public double TimerInterval
        {
            get => _timerInterval;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_timerInterval == value) return;
                double oldValue = _timerInterval;
                _timerInterval = value;
                OnPropertyChanged("TimerInterval", oldValue, value, true);
            }
        }
        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
            if (oldProfile != null)
            {
                oldProfile.ProfileStarted -= Profile_ProfileStarted;
                oldProfile.ProfileStopped -= Profile_ProfileStopped;
            }
            if (Profile != null)
            {
                Profile.ProfileStarted += Profile_ProfileStarted;
                Profile.ProfileStopped += Profile_ProfileStopped;
            }
        }

        private void Profile_ProfileStopped(object sender, EventArgs e)
        {
            if (ConfigManager.Application.ShowDesignTimeControls)
            {
                // don't use this in Profile Editor or other design time tools
                return;
            }

            if (_timer == null)
            {
                // never initialized
                return;
            }

            // shut down
            _timer.Stop();

            // unregister to reduce circularity
            _timer.Tick -= TimerTick;
        }

        private void Profile_ProfileStarted(object sender, EventArgs e)
        {
            if (ConfigManager.Application.ShowDesignTimeControls)
            {
                // don't use this in Profile Editor or other design time tools
                return;
            }

            if (!_timerEnabled)
            {
                // timer functionality not enabled
                return;
            }

            // fixes auto close not working after a stop and start of the profile
            if(_timer != null)
            {
                _timer.Tick += TimerTick;
            }
            else
            {
                _timer = new DispatcherTimer(IntervalTimespan, DispatcherPriority.Input, TimerTick, Dispatcher.CurrentDispatcher);
            }
        }

        private TimeSpan IntervalTimespan => TimeSpan.FromSeconds(Math.Max(_timerInterval, MINIMUM_TIME_OUT));

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (args.PropertyName.Equals("IsHidden"))
            {
                if ((args.NewValue as bool?) == false)
                {
                    // just shown, start time out
                    if (TimerEnabled)
                    {
                        if (_timer == null) _timer = new DispatcherTimer(IntervalTimespan, DispatcherPriority.Input, TimerTick, Dispatcher.CurrentDispatcher);
                        _timer.Interval = IntervalTimespan;
                        _timer.Start();
                    }
                }
                else
                {
                    // hidden, maybe by user or maybe by us
                    _timer?.Stop();
                    // After the panel hides, we always return to the panel's default interval.
                    TimerInterval = _configuredTimerInterval;
                }
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // if the timer ever expires, we timed out, close the panel
            // the timer will be disabled in our handler of the property change
            IsHidden = true;
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("TimerEnabled", _timerEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("TimerInterval", _timerInterval.ToString(CultureInfo.InvariantCulture));
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            _timerEnabled = bool.Parse(reader.ReadElementString("TimerEnabled"));
            _timerInterval = double.Parse(reader.ReadElementString("TimerInterval"), CultureInfo.InvariantCulture);
            _configuredTimerInterval = _timerInterval;
        }

        private void RestartTimer()
        {
            // restart timer, if any (we won't have one at design time)
            if (_timer == null || IsHidden)
            {
                return;
            }
            _timer.Stop();
            _timer.Start();
        }

        #region Actions

        /// <summary>
        /// Set Timer Enabled action on control
        /// </summary>
        /// <param name="action"></param>
        /// <param name="e"></param>
        private void SetTimerEnabledAction_Execute(object action, HeliosActionEventArgs e)
        {
           TimerEnabled = e.Value.BoolValue;
           Logger.Debug($"Timer Panel: {this.Name} Set Timer Enabled Action: {_timerEnabled} {(_timer == null ? "No Timer" : "Timer")}");
            if (_timer == null) _timer = new DispatcherTimer(IntervalTimespan, DispatcherPriority.Input, TimerTick, Dispatcher.CurrentDispatcher);

            if (_timerEnabled)
            {
                RestartTimer();
            }
            else
            {
                _timer?.Stop();
            }
        }
        
        /// <summary>
        /// Set Default Timer Interval action on control
        /// </summary>
        /// <param name="action"></param>
        /// <param name="e"></param>
        private void SetTimerDefaultIntervalAction_Execute(object action, HeliosActionEventArgs e)
        {
            TimerInterval = Math.Abs(e.Value.DoubleValue);
            _configuredTimerInterval = TimerInterval;
            Logger.Debug($"Timer Panel: {this.Name} Set Default Timer Interval Action: {TimerInterval} Enabled: {_timerEnabled} {(_timer==null?"No Timer":"Timer")}");

            if(_timer != null) _timer.Interval = IntervalTimespan;
            if (_timerEnabled)
            {
                RestartTimer();
            }
        }

        /// <summary>
        /// Set One-Time Interval action which reverts to the default once the panel hides on control
        /// </summary>
        /// <param name="action"></param>
        /// <param name="e"></param>
        private void SetTimerOneTimeIntervalAction_Execute(object action, HeliosActionEventArgs e)
        {
            TimerInterval = Math.Abs(e.Value.DoubleValue);
            Logger.Debug($"Timer Panel: {this.Name} Set One-Time Timer Interval Action: {TimerInterval} Enabled: {_timerEnabled} {(_timer == null ? "No Timer" : "Timer")}");

            if (_timer != null) _timer.Interval = IntervalTimespan;
            if (_timerEnabled)
            {
                RestartTimer();
            }
        }
        #endregion


        public void PreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if(_timerEnabled) RestartTimer();
        }

        public void PreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // no code
        }

        public void PreviewTouchDown(object sender, TouchEventArgs touchEventArgs)
        {
            if (_timerEnabled) RestartTimer();
        }

        public void PreviewTouchUp(object sender, TouchEventArgs touchEventArgs)
        {
            // no code
        }
    }
}
