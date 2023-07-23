//  Copyright 2014 Craig Courtney
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.Gauges.F15E.Clock
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.ClockGauge", "Clock Face", "F-15E Strike Eagle", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ClockGauge : BaseGauge
    {
        private GaugeNeedle _clockHoursNeedle;
        private GaugeNeedle _clockMinutesNeedle;
        private GaugeNeedle _clockSecondsNeedle;
        private HeliosValue _clockHours;
        private HeliosValue _clockMinutes;
        private HeliosValue _clockSeconds;

        private CalibrationPointCollectionDouble _clockCalibration = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d);
        private CalibrationPointCollectionDouble _clockHoursCalibration = new CalibrationPointCollectionDouble(0d, 0d, 12d, 360d);

        public ClockGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {

            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{Helios}/Gauges/F-15E/Clock/Clock_Dial.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _clockHoursNeedle = new GaugeNeedle("{Helios}/Gauges/F-15E/Instruments/NeedleHours.xaml", center, new Size(32d, 91d), new Point(16d, 75d));
            Components.Add(_clockHoursNeedle);

            _clockMinutesNeedle = new GaugeNeedle("{Helios}/Gauges/F-15E/Instruments/NeedleMinutes.xaml", center, new Size(32d, 131d), new Point(16d, 115d));
            Components.Add(_clockMinutesNeedle);

            _clockSecondsNeedle = new GaugeNeedle("{Helios}/Gauges/F-15E/Instruments/NeedleSeconds.xaml", center, new Size(32d, 206d), new Point(16d, 133.4d));
            Components.Add(_clockSecondsNeedle);

            _clockHours = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} value for Clock.", "(0-12)", BindingValueUnits.Hours);
            _clockHours.Execute += ClockHours_Execute;
            Actions.Add(_clockHours);

            _clockMinutes = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]} value for Clock.", "(0-60)", BindingValueUnits.Minutes);
            _clockMinutes.Execute += ClockMinutes_Execute;
            Actions.Add(_clockMinutes);

            _clockSeconds = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[2], $"{elements[2]} value for Clock.", "(0-60)", BindingValueUnits.Seconds);
            _clockSeconds.Execute += ClockSeconds_Execute;
            Actions.Add(_clockSeconds);
        }

        void ClockHours_Execute(object action, HeliosActionEventArgs e)
        {
            _clockHours.SetValue(e.Value, e.BypassCascadingTriggers);
            _clockHoursNeedle.Rotation = _clockHoursCalibration.Interpolate(e.Value.DoubleValue);
        }

        void ClockMinutes_Execute(object action, HeliosActionEventArgs e)
        {
            _clockMinutes.SetValue(e.Value, e.BypassCascadingTriggers);
            _clockMinutesNeedle.Rotation = _clockCalibration.Interpolate(e.Value.DoubleValue);
        }

        void ClockSeconds_Execute(object action, HeliosActionEventArgs e)
        {
            _clockSeconds.SetValue(e.Value, e.BypassCascadingTriggers);
            _clockSecondsNeedle.Rotation = _clockCalibration.Interpolate(e.Value.DoubleValue);
        }
    }

}