﻿// Copyright 2020 Helios Contributors
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
                    if (_timer != null)
                    {
                        _timer.Interval = IntervalTimespan;
                        _timer.Start();
                    }
                }
                else
                {
                    // hidden, maybe by user or maybe by us
                    _timer?.Stop();
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

        public void PreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            RestartTimer();
        }

        public void PreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // no code
        }

        public void PreviewTouchDown(object sender, TouchEventArgs touchEventArgs)
        {
            RestartTimer();
        }

        public void PreviewTouchUp(object sender, TouchEventArgs touchEventArgs)
        {
            // no code
        }
    }
}
