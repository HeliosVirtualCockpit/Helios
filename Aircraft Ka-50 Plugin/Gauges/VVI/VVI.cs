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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.VVI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.VVI", "VVI", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class VVI : BaseGauge
    {
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _verticalVelocity;

        public VVI()
            : base("VVI", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(-30d, -180d, 30d, 180d);
            _needleCalibration.Add(new CalibrationPointDouble(-10d, -80d));
            _needleCalibration.Add(new CalibrationPointDouble(10d, 80d));
            _needleCalibration.Add(new CalibrationPointDouble(20d, 140d));
            _needleCalibration.Add(new CalibrationPointDouble(-20d, -140d));

            Components.Add(new GaugeImage("{Ka-50}/Gauges/VVI/vvi_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle = new GaugeNeedle("{Ka-50}/Gauges/VVI/vvi_needle.xaml", center, new Size(19, 188), new Point(9.5, 139.25), -90d);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/VVI/vvi_bezel.xaml", new Rect(0, 0, 340, 340)));

            _verticalVelocity = new HeliosValue(this, BindingValue.Empty, "", "Vertical Velocity", "Current climb/descent velocity", "", BindingValueUnits.MetersPerSecond);
            _verticalVelocity.Execute += VerticalVelocity_Execute;
            Actions.Add(_verticalVelocity);
        }

        private void VerticalVelocity_Execute(object action, HeliosActionEventArgs e)
        {
            _verticalVelocity.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
