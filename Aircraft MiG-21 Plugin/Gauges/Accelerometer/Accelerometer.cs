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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.Accelerometer
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.Accelerometer", "Accelerometer", "MiG-21", typeof(GaugeRenderer))]
    public class Accelerometer : BaseGauge
    {
        private GaugeNeedle _lowNeedle;
        private GaugeNeedle _highNeedle;
        private GaugeNeedle _currentNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;

        private HeliosValue _lowG;
        private HeliosValue _highG;
        private HeliosValue _currentG;

        public Accelerometer()
            : base("Accelerometer", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            //accelerometer input min, deg, max, deg where i want the indicator to point
            _needleCalibration = new CalibrationPointCollectionDouble(-5d, -110d, 10d, 220d);
            _needleCalibration.Add(new CalibrationPointDouble(0d, 0d));
            _needleCalibration.Add(new CalibrationPointDouble(1d, 22d));
            _needleCalibration.Add(new CalibrationPointDouble(5d, 110d));
            _needleCalibration.Add(new CalibrationPointDouble(8d, 176d));
            //ACCELEROMETER.input = { -5.0, 1, 5, 8, 10.0 } 
            //ACCELEROMETER.output = { -0.41, 0.096, 0.5, 0.81, 1 }

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Accelerometer/accelerometer_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _lowNeedle = new GaugeNeedle("{MiG-21}/Gauges/Accelerometer/accelerometer_limit_needleLow.xaml", center, new Size(32, 189), new Point(16, 161));
            Components.Add(_lowNeedle);

            _highNeedle = new GaugeNeedle("{MiG-21}/Gauges/Accelerometer/accelerometer_limit_needle.xaml", center, new Size(32, 189), new Point(16, 161));
            Components.Add(_highNeedle);

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Accelerometer/accelerometer_needle.xaml", center, new Size(32, 185), new Point(16, 127));
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Accelerometer/accelerometer_bezel.xaml", new Rect(0, 0, 340, 340)));

            _lowG = new HeliosValue(this, BindingValue.Empty, "", "Low G", "Lowest G attained", "", BindingValueUnits.Numeric);
            _lowG.Execute += LowG_Execute;
            Actions.Add(_lowG);

            _highG = new HeliosValue(this, BindingValue.Empty, "", "High G", "Highest G attained", "", BindingValueUnits.Numeric);
            _highG.Execute += HighG_Execute;
            Actions.Add(_highG);

            _currentG = new HeliosValue(this, BindingValue.Empty, "", "Current G", "Current G", "", BindingValueUnits.Numeric);
            _currentG.Execute += CurrentG_Execute;
            Actions.Add(_currentG);
        }

        private void LowG_Execute(object action, HeliosActionEventArgs e)
        {
            _lowG.SetValue(e.Value, e.BypassCascadingTriggers);
            _lowNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void HighG_Execute(object action, HeliosActionEventArgs e)
        {
            _highG.SetValue(e.Value, e.BypassCascadingTriggers);
            _highNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void CurrentG_Execute(object action, HeliosActionEventArgs e)
        {
            _currentG.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
