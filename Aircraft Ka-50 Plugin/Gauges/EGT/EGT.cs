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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.EGT
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.EGT", "Exhaust Gas Temperature", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class EGT : BaseGauge
    {
        private GaugeNeedle _leftLargeNeedle;
        private GaugeNeedle _leftSmallNeedle;
        private GaugeNeedle _rightLargeNeedle;
        private GaugeNeedle _rightSmallNeedle;

        private CalibrationPointCollectionDouble _largeNeedleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1200d, 270d);
        private CalibrationPointCollectionDouble _smallNeedleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 360d);

        private HeliosValue _leftTemperature;
        private HeliosValue _rigthTemperature;

        public EGT()
            : base("Exhast Gas Temperature", new Size(340, 340))
        {
            Components.Add(new GaugeImage("{Ka-50}/Gauges/EGT/egt_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _leftSmallNeedle = new GaugeNeedle("{Ka-50}/Gauges/EGT/egt_small_needle.xaml", new Point(97, 227), new Size(15, 45), new Point(7.5, 37.5), 180d);
            Components.Add(_leftSmallNeedle);

            _leftLargeNeedle = new GaugeNeedle("{Ka-50}/Gauges/EGT/egt_large_needle.xaml", new Point(97, 158), new Size(19, 76), new Point(9.5, 66.5), -135d);
            Components.Add(_leftLargeNeedle);

            _rightSmallNeedle = new GaugeNeedle("{Ka-50}/Gauges/EGT/egt_small_needle.xaml", new Point(238, 227), new Size(15, 45), new Point(7.5, 37.5), 180d);
            Components.Add(_rightSmallNeedle);

            _rightLargeNeedle = new GaugeNeedle("{Ka-50}/Gauges/EGT/egt_large_needle.xaml", new Point(238, 158), new Size(19, 76), new Point(9.5, 66.5), -135d);
            Components.Add(_rightLargeNeedle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/EGT/egt_bezel.xaml", new Rect(0, 0, 340, 340)));

            _leftTemperature = new HeliosValue(this, BindingValue.Empty, "", "Left Exhaust Temperature", "Exhaust Gas Temperature of left engine.", "", BindingValueUnits.Celsius);
            _leftTemperature.Execute += LeftTemperature_Execute;
            Actions.Add(_leftTemperature);

            _rigthTemperature = new HeliosValue(this, BindingValue.Empty, "", "Right Exhaust Temperature", "Exhaust Gas Temperature of right engine.", "", BindingValueUnits.Celsius);
            _rigthTemperature.Execute += RightTemperature_Execute;
            Actions.Add(_rigthTemperature);
        }

        private void LeftTemperature_Execute(object action, HeliosActionEventArgs e)
        {
            _leftTemperature.SetValue(e.Value, e.BypassCascadingTriggers);
            _leftLargeNeedle.Rotation = _largeNeedleCalibration.Interpolate(e.Value.DoubleValue);
            _leftSmallNeedle.Rotation = _smallNeedleCalibration.Interpolate(e.Value.DoubleValue % 100d);
        }

        private void RightTemperature_Execute(object action, HeliosActionEventArgs e)
        {
            _rigthTemperature.SetValue(e.Value, e.BypassCascadingTriggers);
            _rightLargeNeedle.Rotation = _largeNeedleCalibration.Interpolate(e.Value.DoubleValue);
            _rightSmallNeedle.Rotation = _smallNeedleCalibration.Interpolate(e.Value.DoubleValue % 100d);
        }
    }
}
