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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.OilPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.OilPressure", "Oil Pressure", "MiG-21", typeof(GaugeRenderer))]
    public class OilPressure : BaseGauge
    {
        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentPressure;

        public OilPressure()
            : base("OilPressure", new Size(340, 340))
        {
            Point center = new Point(170, 170);
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 4d, 89d);
            _needleCalibration.Add(new CalibrationPointDouble(0.8d, 17d));
            _needleCalibration.Add(new CalibrationPointDouble(1.6d, 35d));
            _needleCalibration.Add(new CalibrationPointDouble(2.4d, 53d));
            _needleCalibration.Add(new CalibrationPointDouble(3.2d, 71d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/OilPressure/oilpressure_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/generic_gray_needle.xaml", center, new Size(69d, 178d), new Point(34.5d, 126.5d), 287.3d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/OilPressure/oilpressure_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentPressure = new HeliosValue(this, BindingValue.Empty, "", "Oil Pressure", "Current pressure", "", BindingValueUnits.Numeric);
            _currentPressure.Execute += CurrentOil_Execute;
            Actions.Add(_currentPressure);
        }

        private void CurrentOil_Execute(object action, HeliosActionEventArgs e)
        {
            _currentPressure.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
