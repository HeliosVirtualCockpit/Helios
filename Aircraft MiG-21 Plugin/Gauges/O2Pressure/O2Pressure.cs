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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.O2Pressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.O2Pressure", "O2 Pressure", "MiG-21", typeof(GaugeRenderer))]
    public class O2Pressure : BaseGauge
    {

        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentO2Pressure;

        public O2Pressure()
            : base("O2Pressure", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0, 0d, 20d, 277d);
            _needleCalibration.Add(new CalibrationPointDouble(2d, 30d));
            _needleCalibration.Add(new CalibrationPointDouble(4d, 58d));
            _needleCalibration.Add(new CalibrationPointDouble(6d, 86d));
            _needleCalibration.Add(new CalibrationPointDouble(8d, 115d));
            _needleCalibration.Add(new CalibrationPointDouble(10d, 137.9d));
            _needleCalibration.Add(new CalibrationPointDouble(12d, 160.7d));
            _needleCalibration.Add(new CalibrationPointDouble(14d, 189d));
            _needleCalibration.Add(new CalibrationPointDouble(16d, 210.1d));
            _needleCalibration.Add(new CalibrationPointDouble(18d, 245d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/O2Pressure/o2_pressure_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/small_needle.xaml", center, new Size(23, 164), new Point(11.5, 127), 222d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentO2Pressure = new HeliosValue(this, BindingValue.Empty, "", "Current O2 Pressure", "Current O2 Pressure", "", BindingValueUnits.Numeric);
            _currentO2Pressure.Execute += CurrentO2Pressure_Execute;
            Actions.Add(_currentO2Pressure);
        }

        private void CurrentO2Pressure_Execute(object action, HeliosActionEventArgs e)
        {
            _currentO2Pressure.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
