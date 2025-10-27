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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.BladeAngle
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.BladeAngle", "Blade Angle", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class BladeAngle : BaseGauge
    {
        private GaugeNeedle _needle;
        private HeliosValue _angle;
        private CalibrationPointCollectionDouble _callibration;

        public BladeAngle()
            : base("Blade Angle", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/BladeAngle/blade_angle_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle = new GaugeNeedle("{Ka-50}/Gauges/BladeAngle/blade_angle_needle.xaml", center, new Size(40, 159), new Point(20, 139), -105d);
            Components.Add(_needle);

            _callibration = new CalibrationPointCollectionDouble(0d, 0d, 15d, 210d);
            _callibration.Add( new CalibrationPointDouble( 0.68d, 7d ) ); // 1.5 degree
            _callibration.Add( new CalibrationPointDouble( 1.35d, 15d ) ); // 2 degrees 
            _callibration.Add( new CalibrationPointDouble( 2.51d, 30d ) ); // 3 degrees 
            _callibration.Add( new CalibrationPointDouble( 4.66d, 60d ) ); // 5 degrees 
            _callibration.Add( new CalibrationPointDouble( 6.61d, 89d ) ); // 7 degrees 
            _callibration.Add( new CalibrationPointDouble( 8.39d, 119d ) ); // 9 degrees 
            _callibration.Add( new CalibrationPointDouble( 10.27d, 149d ) );   // 11 degrees 
            _callibration.Add( new CalibrationPointDouble( 12.45d, 178d ) );  // 13 degrees
            _callibration.Add( new CalibrationPointDouble( 14.95d, 210d ) );  // 15 degrees

            _angle = new HeliosValue(this, BindingValue.Empty, "", "Blade Angle", "Angle of the blades", "", BindingValueUnits.Degrees);
            _angle.Execute += Angle_Execute;
            Actions.Add(_angle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/BladeAngle/blade_angle_bezel.xaml", new Rect(0, 0, 340, 340)));
        }

        private void Angle_Execute(object action, HeliosActionEventArgs e)
        {
            _angle.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _callibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
