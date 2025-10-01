// Copyright 2024 Helios Contributors
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
using System.ComponentModel;
using System.Globalization;
using System.Net.Cache;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// This is a subclass of panel with the added behavior to
    /// hide the panel if no click / touch events have occured
    /// within a given time period
    /// </summary>

    [HeliosControl("Helios.Special.Metronome", "Metronome", "Special Controls",typeof(ImageDecorationRenderer))]
    public class Metronome : ImageDecorationBase, IWindowsPreviewInput
    {
        private bool _metronomeEnabled = true;
        private DispatcherTimer _tick;
        private HeliosValue _tickIntervalDefaultValue;
        private HeliosValue _metronomeEnabledValue;
        private HeliosTrigger _tickTrigger;
        private HeliosTrigger _tickTockTrigger;
        private HeliosTrigger _tockTickTrigger;
        private bool _tickTock = false;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Metronome() : base("Metronome")
        {
            DesignTimeOnly = true;
            Image = "{Helios}/Images/General/metronome_wire.png";
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;

            _metronomeEnabledValue = new HeliosValue(this, new BindingValue(false), "Metronome", "Enable", "Indicates whether the metronome should run.", "True if the metronome is enabled.", BindingValueUnits.Boolean);
            _metronomeEnabledValue.Execute += SetTimerEnabledAction_Execute;
            Values.Add(_metronomeEnabledValue);
            Actions.Add(_metronomeEnabledValue);

            _tickIntervalDefaultValue = new HeliosValue(this, new BindingValue(1d), "Metronome", "Default Interval", "Default time for each tick.", "Positive numeric value in seconds.", BindingValueUnits.Numeric);
            _tickIntervalDefaultValue.Execute += SetTimerDefaultIntervalAction_Execute;
            Values.Add(_tickIntervalDefaultValue);
            Actions.Add(_tickIntervalDefaultValue);

            _tickTrigger = new HeliosTrigger(this, "", "", "Metronome Tick", "Fired when the time period interval expires.", "Always returns true.", BindingValueUnits.Boolean);
            Triggers.Add(_tickTrigger);

            _tickTockTrigger = new HeliosTrigger(this, "", "", "Metronome Tick Tock", "Fired when the time period interval expires.", "Returns alternating true and false. Returns false on disable.", BindingValueUnits.Boolean);
            Triggers.Add(_tickTockTrigger);

            _tockTickTrigger = new HeliosTrigger(this, "", "", "Metronome Tock Tick", "Fired when the time period interval expires.", "Returns alternating false and true. Returns true on disable.", BindingValueUnits.Boolean);
            Triggers.Add(_tockTickTrigger);


        }
        #region Properties

        /// <summary>
        /// backing field for property TickInterval, contains
        /// time out after which this panel automatically closes if no input is received
        /// </summary>
        private double _tickInterval = 1d;

        /// <summary>
        /// Field to hold the time interval configured in Profile Editor and read from XML
        /// Even if the time interval is changed via the Timer Interval action, this value is 
        /// used to set the Timer Interval when the panel is unhidden this providing repeatable
        /// experience every time the panel is seen
        /// </summary>
        private double _configuredTickInterval = 1d;

        /// <summary>
        /// minimum permissible time out
        /// </summary>
        private const double MINIMUM_TIME_OUT = 0.01d;

        /// <summary>
        /// time out after which this panel automatically closes if no input is received
        /// </summary>
        public double TickInterval
        {
            get => _tickInterval;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_tickInterval == value) return;
                double oldValue = _tickInterval;
                _tickInterval = value;
                OnPropertyChanged("TickInterval", oldValue, value, true);
                OnDisplayUpdate();
            }
        }
        public bool MetronomeEnabled
        {
            get => _metronomeEnabled;
            set
            {
                if (!_metronomeEnabled.Equals(value))
                {
                    _metronomeEnabled = value;
                    OnPropertyChanged("MetronomeEnabled", value, !value, true);
                    OnDisplayUpdate();
                }
            }
        }

        #endregion
        public override bool EffectsExclusion
        {
            get => true;
            set { }
        }
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

        private void Profile_ProfileStarted(object sender, EventArgs e)
        {
            if (ConfigManager.Application.ShowDesignTimeControls)
            {
                // don't use this in Profile Editor or other design time tools
                return;
            }

            if (!_metronomeEnabled)
            {
                // timer functionality not enabled
                return;
            }
            if(_tick is null)
            {
                _tick = new DispatcherTimer(IntervalTimespan, DispatcherPriority.Input, TimerTick, Dispatcher.CurrentDispatcher);

            } else
            {
                _tick.Tick += TimerTick;
            }
            _tick.IsEnabled = _metronomeEnabled;
            _tick.Start();
        }
        private void Profile_ProfileStopped(object sender, EventArgs e)
        {
            if (ConfigManager.Application.ShowDesignTimeControls)
            {
                // don't use this in Profile Editor or other design time tools
                return;
            }

            if (_tick == null)
            {
                // never initialized
                return;
            }

            // shut down
            _tick.Stop();
            // unregister to reduce circularity
            _tick.Tick -= TimerTick;
        }


        private TimeSpan IntervalTimespan => TimeSpan.FromSeconds(Math.Max(_tickInterval, MINIMUM_TIME_OUT));

        private void TimerTick(object sender, EventArgs e)
        {
            TickInterval = _configuredTickInterval;
            _tickTrigger.FireTrigger(new BindingValue(true));
            _tickTock = !_tickTock;
            _tickTockTrigger.FireTrigger(new BindingValue(_tickTock));
            _tockTickTrigger.FireTrigger(new BindingValue(!_tickTock));
        }

        private void RestartTimer()
        {
            // restart timer, if any (we won't have one at design time)
            if (_tick == null)
            {
                return;
            }
            _tick.Stop();
            _tick.Start();
        }

        #region Actions

        /// <summary>
        /// Set Default Timer Interval action on control
        /// </summary>
        /// <param name="action"></param>
        /// <param name="e"></param>
        private void SetTimerDefaultIntervalAction_Execute(object action, HeliosActionEventArgs e)
        {
            TickInterval = Math.Abs(e.Value.DoubleValue);
            _configuredTickInterval = TickInterval;
            Logger.Debug($"Metronome: {this.Name} Set Default Metronome Interval Action: {TickInterval} Enabled: {_metronomeEnabled} {(_tick == null ? "No Timer" : "Timer")}");

            if (_tick != null) _tick.Interval = IntervalTimespan;
            if (_metronomeEnabled)
            {
                RestartTimer();
            }
        }

        /// <summary>
        /// Set Timer Enabled action on control
        /// </summary>
        /// <param name="action"></param>
        /// <param name="e"></param>
        private void SetTimerEnabledAction_Execute(object action, HeliosActionEventArgs e)
        {
            _metronomeEnabled = e.Value.BoolValue;
            Logger.Debug($"Metronome: {this.Name} Set Metronome Enabled Action: {_metronomeEnabled} {(_tick == null ? "No Metronome" : "Metronome")}");

            if (_metronomeEnabled)
            {
                if (_tick == null)
                {
                    _tick = new DispatcherTimer(IntervalTimespan, DispatcherPriority.Input, TimerTick, Dispatcher.CurrentDispatcher);
                } else
                {
                    _tick.Tick += TimerTick;
                }
                _tick.IsEnabled = _metronomeEnabled;
                RestartTimer();
            }
            else
            {
                _tick?.Stop();
                if (_tick != null)
                {
                    _tick.Tick -= TimerTick;
                }
                _tickTockTrigger.FireTrigger(new BindingValue(false));
                _tockTickTrigger.FireTrigger(new BindingValue(true));
            }
        }

        #endregion
        #region overrides
        public void PreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_metronomeEnabled) RestartTimer();
        }

        public void PreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // no code
        }

        public void PreviewTouchDown(object sender, TouchEventArgs touchEventArgs)
        {
            if (_metronomeEnabled) RestartTimer();
        }

        public void PreviewTouchUp(object sender, TouchEventArgs touchEventArgs)
        {
            // no code
        }
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (!_metronomeEnabled)
            {
                writer.WriteElementString("MetronomeEnabled", _metronomeEnabled.ToString(CultureInfo.InvariantCulture));
            }
            writer.WriteElementString("TickInterval", _tickInterval.ToString(CultureInfo.InvariantCulture));
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));
            base.ReadXml(reader); 
            MetronomeEnabled = reader.Name.Equals("MetronomeEnabled") ? (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("MetronomeEnabled")) : true;
            _tickInterval = double.Parse(reader.ReadElementString("TickInterval"), CultureInfo.InvariantCulture);
            _configuredTickInterval = _tickInterval;
        }
        #endregion
    }
}
