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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.EngExhaustTempGauge
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.EngExhaustTempGauge", "Engine Temp Gauge", "MiG-21", typeof(GaugeRenderer))]
    public class EngExhaustTempGauge : BaseGauge
    {
        private GaugeNeedle _currentNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentTemp;

        public EngExhaustTempGauge()
            : base("EngExhaustTempGauge", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(300d, 0d, 900d, 231d);
            _needleCalibration.Add(new CalibrationPointDouble(400d, 29d));
            _needleCalibration.Add(new CalibrationPointDouble(500d, 59));
            _needleCalibration.Add(new CalibrationPointDouble(600d, 88d));
            _needleCalibration.Add(new CalibrationPointDouble(650d, 118d));
            _needleCalibration.Add(new CalibrationPointDouble(700d, 148d));
            _needleCalibration.Add(new CalibrationPointDouble(750d, 177d));
            _needleCalibration.Add(new CalibrationPointDouble(800d, 206d));
            _needleCalibration.Add(new CalibrationPointDouble(850d, 218d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/EngExhaustTemp/EngExhaustTemp_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/EngExhaustTemp/EngExhaustTemp_needle.xaml", center, new Size(69d, 178d), new Point(34.5d, 126.5d), 242d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/EngExhaustTemp/EngExhaustTemp_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentTemp = new HeliosValue(this, BindingValue.Empty, "", "Engine Exhaust", "Current Temp", "", BindingValueUnits.Numeric);
            _currentTemp.Execute += CurrentFuel_Execute;
            Actions.Add(_currentTemp);
        }


        private void CurrentFuel_Execute(object action, HeliosActionEventArgs e)
        {
            _currentTemp.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
