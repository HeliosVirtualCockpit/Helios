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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.IAS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.IAS", "IAS", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class IAS : BaseGauge
    {
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _speed;

        public IAS()
            : base("IAS", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(20d, 5d, 350d, 345d);
            _needleCalibration.Add(new CalibrationPointDouble(50d, 95d));
            _needleCalibration.Add(new CalibrationPointDouble(100d, 145d));
            _needleCalibration.Add(new CalibrationPointDouble(150d, 185d));
            _needleCalibration.Add(new CalibrationPointDouble(200d, 225d));
            _needleCalibration.Add(new CalibrationPointDouble(250d, 265d));
            _needleCalibration.Add(new CalibrationPointDouble(300d, 305d));

            Components.Add(new GaugeImage("{Ka-50}/Gauges/IAS/ias_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle = new GaugeNeedle("{Ka-50}/Gauges/IAS/ias_needle.xaml", center, new Size(40, 159), new Point(20, 139), 5d);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/IAS/ias_bezel.xaml", new Rect(0, 0, 340, 340)));

            _speed = new HeliosValue(this, BindingValue.Empty, "", "Indicated Airspeed", "Current indicated airspeed", "", BindingValueUnits.KilometersPerHour);
            _speed.Execute += Speed_Execute;
            Actions.Add(_speed);
        }

        private void Speed_Execute(object action, HeliosActionEventArgs e)
        {
            _speed.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
