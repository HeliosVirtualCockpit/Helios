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

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AV8B.ClockFace", "Clock Face", "AV-8B Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ClockFace : BaseGauge
    {
        private string _imageLocation = "{AV-8B}/Images/";
        private GaugeNeedle _clockHoursNeedle;
        private GaugeNeedle _clockMinutesNeedle;
        private GaugeNeedle _clockSecondsNeedle;
        private HeliosValue _clockHours;
        private HeliosValue _clockMinutes;
        private HeliosValue _clockSeconds;

        private CalibrationPointCollectionDouble _clockCalibration = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d);
        private CalibrationPointCollectionDouble _clockHoursCalibration = new CalibrationPointCollectionDouble(0d, 0d, 12d, 360d);

        public ClockFace()
            : base("Clock", new Size(390, 390))
        {

            Point center = new Point(195d, 195d);
            double needleScaleFactor = 1.1;
            Components.Add(new GaugeImage("{Helios}/Gauges/AV-8B/Clock/clock_faceplate.xaml", new Rect(0d, 0d, 394d, 394d)));
            _clockHoursNeedle = new GaugeNeedle("{Helios}/Gauges/AV-8B/Clock/hours_needle.xaml", center, new Size(25d, 125d), new Point(12.5d, 112.5d));
            Components.Add(_clockHoursNeedle);

            _clockMinutesNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/Common/needle_a.xaml", center, new Size(22 * needleScaleFactor, 165 * needleScaleFactor), new Point(11 * needleScaleFactor, 130 * needleScaleFactor));
            //_clockMinutesNeedle = new GaugeNeedle("{Helios}/Gauges/KA-50/Clock/clock_min_needle.xaml", center, new Size(20, 135), new Point(10, 125));
            Components.Add(_clockMinutesNeedle);

            _clockSecondsNeedle = new GaugeNeedle("{Helios}/Gauges/KA-50/Clock/clock_seconds_needle.xaml", center, new Size(13 * needleScaleFactor, 166 * needleScaleFactor), new Point(6.5 * needleScaleFactor, 141 * needleScaleFactor));
            Components.Add(_clockSecondsNeedle);
            GaugeImage _gauge = new GaugeImage($"{_imageLocation}WQHD/Panel/crystal_reflection_round.png", new Rect(0d, 0d, 390d, 390d));
            _gauge.Opacity = 0.5;
            Components.Add(_gauge);
            // Components.Add(new GaugeImage($"{_imageLocation}Clock Bezel.png", new Rect(0d, 0d, 390d, 390d)));

            _clockHours = new HeliosValue(this, BindingValue.Empty, "Clock", "Hours", "Hours value for Clock.", "(0-12)", BindingValueUnits.Numeric);
            _clockHours.Execute += ClockHours_Execute;
            Actions.Add(_clockHours);

            _clockMinutes = new HeliosValue(this, BindingValue.Empty, "Clock", "Minutes", "Minute value for Clock.", "(0-60)", BindingValueUnits.Numeric);
            _clockMinutes.Execute += ClockMinutes_Execute;
            Actions.Add(_clockMinutes);

            _clockSeconds = new HeliosValue(this, BindingValue.Empty, "Clock", "Seconds", "Seconds value for Clock.", "(0-60)", BindingValueUnits.Numeric);
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