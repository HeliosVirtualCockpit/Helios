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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.RadioAltimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.RadioAltimeter", "Radio Altimeter", "MiG-21", typeof(GaugeRenderer))]
    public class RadioAltimeter : BaseGauge
    {

        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentRadAlt;

        public RadioAltimeter()
            : base("RadioAltimeter", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0.0d, 0d, 1000d, 231d);
            _needleCalibration.Add(new CalibrationPointDouble(10d, 22.5d));
            _needleCalibration.Add(new CalibrationPointDouble(20d, 34.6d));
            _needleCalibration.Add(new CalibrationPointDouble(30d, 55.6d));
            _needleCalibration.Add(new CalibrationPointDouble(40d, 73.8d));
            _needleCalibration.Add(new CalibrationPointDouble(50d, 94.2d));
            _needleCalibration.Add(new CalibrationPointDouble(60d, 103.2d));
            _needleCalibration.Add(new CalibrationPointDouble(70d, 114.2d));
            _needleCalibration.Add(new CalibrationPointDouble(80d, 119.6d));
            _needleCalibration.Add(new CalibrationPointDouble(90d, 127d));
            _needleCalibration.Add(new CalibrationPointDouble(100d, 130.5d));
            _needleCalibration.Add(new CalibrationPointDouble(150d, 151.2d));
            _needleCalibration.Add(new CalibrationPointDouble(200d, 160.6d));
            _needleCalibration.Add(new CalibrationPointDouble(250d, 181.9d));
            _needleCalibration.Add(new CalibrationPointDouble(300d, 192.6d));
            _needleCalibration.Add(new CalibrationPointDouble(400d, 210.9d));
            _needleCalibration.Add(new CalibrationPointDouble(500d, 220.6d));
            _needleCalibration.Add(new CalibrationPointDouble(600d, 231d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/RadioAltimeter/radalt_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/RadioAltimeter/radalt_needle.xaml", center, new Size(69d, 178d), new Point(34.5d, 126.5d), -120d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentRadAlt = new HeliosValue(this, BindingValue.Empty, "", "Current RadAlt", "Current RadAlt", "", BindingValueUnits.Numeric);
            _currentRadAlt.Execute += CurrentRadAlt_Execute;
            Actions.Add(_currentRadAlt);
        }

        private void CurrentRadAlt_Execute(object action, HeliosActionEventArgs e)
        {
            _currentRadAlt.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
