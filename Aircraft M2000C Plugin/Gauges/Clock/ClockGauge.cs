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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.Clock
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M2000C.ClockGauge", "Clock Face", "M-2000C", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ClockGauge : BaseGauge
    {
        private GaugeNeedle _clockHoursNeedle;
        private GaugeNeedle _clockMinutesNeedle;
        private GaugeNeedle _stopWatchSecondsNeedle;
        private GaugeNeedle _stopWatchMinutesNeedle;

        private HeliosValue _clockHours;
        private HeliosValue _clockMinutes;
        private HeliosValue _stopWatchSeconds;


        private CalibrationPointCollectionDouble _stopWatchCalibration = new CalibrationPointCollectionDouble(0d, 0d, 15d, 360d);
        private CalibrationPointCollectionDouble _clockCalibration = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d);
        private CalibrationPointCollectionDouble _clockHoursCalibration = new CalibrationPointCollectionDouble(0d, 0d, 12d, 360d);

        public ClockGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {

            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{M2000C}/Gauges/Clock/Clock_Dial.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _clockHoursNeedle = new GaugeNeedle("{M2000C}/Gauges/Clock/NeedleHours.xaml", center, new Size(28.089d, 127.748d), new Point(14.045d, 113.619d));
            Components.Add(_clockHoursNeedle);

            _clockMinutesNeedle = new GaugeNeedle("{M2000C}/Gauges/Clock/NeedleMinutes.xaml", center, new Size(27.883d, 158.059d), new Point(13.941d, 143.9194d));
            Components.Add(_clockMinutesNeedle);

            _stopWatchSecondsNeedle = new GaugeNeedle("{M2000C}/Gauges/Clock/NeedleSeconds.xaml", center, new Size(28.231d, 209.156d), new Point(14.115d, 144.1556d));
            Components.Add(_stopWatchSecondsNeedle);

            _stopWatchMinutesNeedle = new GaugeNeedle("{M2000C}/Gauges/Clock/NeedleStopWatchMinutes.xaml", new Point(size.Width / 2, 263d), new Size(12.749d, 57.624d), new Point(6.375d, 51.4065d));
            Components.Add(_stopWatchMinutesNeedle);


            _clockHours = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} value for Clock.", "(0-12)", BindingValueUnits.Hours);
            _clockHours.Execute += ClockHours_Execute;
            Actions.Add(_clockHours);

            _clockMinutes = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]} value for Clock.", "(0-60)", BindingValueUnits.Minutes);
            _clockMinutes.Execute += ClockMinutes_Execute;
            Actions.Add(_clockMinutes);

            _stopWatchSeconds = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[2], $"{elements[2]} value for Clock.", "(0-60)", BindingValueUnits.Seconds);
            _stopWatchSeconds.Execute += stopWatchSeconds_Execute;
            Actions.Add(_stopWatchSeconds);

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

        void stopWatchSeconds_Execute(object action, HeliosActionEventArgs e)
        {
            double seconds = Math.Round(e.Value.DoubleValue % 60d);
            double minutes = e.Value.DoubleValue / 60d;
            _stopWatchSeconds.SetValue(e.Value, e.BypassCascadingTriggers);
            _stopWatchSecondsNeedle.Rotation = _clockCalibration.Interpolate(seconds);
            _stopWatchMinutesNeedle.Rotation = _stopWatchCalibration.Interpolate(minutes);

        }
    }

}