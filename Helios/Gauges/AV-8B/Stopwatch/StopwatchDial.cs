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

    [HeliosControl("Helios.AV8B.StopwatchDial", "Stopwatch Dial", "AV-8B Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class StopwatchDial : BaseGauge
    {
        private string _imageLocation = "{AV-8B}/Images/";

        private GaugeNeedle _stopWatchMinutesNeedle;
        private GaugeNeedle _stopWatchSecondsNeedle;
        private HeliosValue _stopWatchMinutes;
        private HeliosValue _stopWatchSeconds;

        private CalibrationPointCollectionDouble _stopWatchCalibration = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d);

        public StopwatchDial()
            : base("StopwatchDial", new Size(390, 390))
        {
            Point center = new Point(192d, 193d);
            double needleScaleFactor = 0.65;
            Components.Add(new GaugeImage($"{_imageLocation}Stopwatch Faceplate.png", new Rect(0d, 0d, 390d, 390d)));
            _stopWatchMinutesNeedle = new GaugeNeedle($"{_imageLocation}Stopwatch Minute Hand.png", center, new Size(48d * needleScaleFactor, 144d * needleScaleFactor), new Point(24d * needleScaleFactor, 120d * needleScaleFactor));
            Components.Add(_stopWatchMinutesNeedle);
            _stopWatchSecondsNeedle = new GaugeNeedle($"{_imageLocation}Stopwatch Second Hand.png", center, new Size(50 * needleScaleFactor, 198 * needleScaleFactor), new Point(25 * needleScaleFactor, 173 * needleScaleFactor));
            Components.Add(_stopWatchSecondsNeedle);
            GaugeImage _gauge = new GaugeImage($"{_imageLocation}WQHD/Panel/crystal_reflection_round.png", new Rect(0d, 0d, 390d, 390d));
            _gauge.Opacity = 0.5;
            Components.Add(_gauge);
            Components.Add(new GaugeImage($"{_imageLocation}Stopwatch Bezel.png", new Rect(0d, 0d, 390d, 390d)));

            _stopWatchMinutes = new HeliosValue(this, BindingValue.Empty, "Stopwatch", "Minutes", "Current Minutes on the Stopwatch", "(0-60)", BindingValueUnits.Numeric);
            _stopWatchMinutes.Execute += StopWatchMinutes_Execute;
            Actions.Add(_stopWatchMinutes);

            _stopWatchSeconds = new HeliosValue(this, BindingValue.Empty, "Stopwatch", "Seconds", "Current Seconds on the Stopwatch", "(0-60)", BindingValueUnits.Numeric);
            _stopWatchSeconds.Execute += StopWatchSeconds_Execute;
            Actions.Add(_stopWatchSeconds);
        }

        void StopWatchMinutes_Execute(object action, HeliosActionEventArgs e)
        {
            _stopWatchMinutes.SetValue(e.Value, e.BypassCascadingTriggers);
            _stopWatchMinutesNeedle.Rotation = _stopWatchCalibration.Interpolate(e.Value.DoubleValue);
        }

        void StopWatchSeconds_Execute(object action, HeliosActionEventArgs e)
        {
            _stopWatchSeconds.SetValue(e.Value, e.BypassCascadingTriggers);
            _stopWatchSecondsNeedle.Rotation = _stopWatchCalibration.Interpolate(e.Value.DoubleValue);
        }
        public override bool HitTest(Point location)
        {
            return false;
        }
    }
}