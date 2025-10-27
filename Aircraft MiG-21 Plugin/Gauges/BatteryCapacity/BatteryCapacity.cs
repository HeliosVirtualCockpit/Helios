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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.BatteryCapacity
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.BatteryCapacity", "Battery Capacity", "MiG-21", typeof(GaugeRenderer))]
    public class BatteryCapacity : BaseGauge
    {

        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentBatteryCapacity;

        public BatteryCapacity()
            : base("BatteryCapacity", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 300d);
            _needleCalibration.Add(new CalibrationPointDouble(10d, 36d));
            _needleCalibration.Add(new CalibrationPointDouble(20d, 66d));
            _needleCalibration.Add(new CalibrationPointDouble(30d, 96d));
            _needleCalibration.Add(new CalibrationPointDouble(40d, 126d));
            _needleCalibration.Add(new CalibrationPointDouble(50d, 156d));
            _needleCalibration.Add(new CalibrationPointDouble(60d, 186d));
            _needleCalibration.Add(new CalibrationPointDouble(70d, 216d));
            _needleCalibration.Add(new CalibrationPointDouble(80d, 246d));
            _needleCalibration.Add(new CalibrationPointDouble(90d, 276d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/BatteryCapacity/battery_capacity_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/generic_small_gray_needle.xaml", center, new Size(32, 185), new Point(16, 127), 210d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentBatteryCapacity = new HeliosValue(this, BindingValue.Empty, "", "Current Battery Capacity", "Current Battery Capacity", "", BindingValueUnits.Numeric);
            _currentBatteryCapacity.Execute += CurrentBatteryCapacity_Execute;
            Actions.Add(_currentBatteryCapacity);
        }

        private void CurrentBatteryCapacity_Execute(object action, HeliosActionEventArgs e)
        {
            _currentBatteryCapacity.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
