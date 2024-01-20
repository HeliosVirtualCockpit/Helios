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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.RotorRPM
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.RotorRPM", "Rotor RPM", "KA-50 Gauges", typeof(GaugeRenderer))]
    public class RotorRPM : BaseGauge
    {
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _rpm;

        public RotorRPM()
            : base("Rotor RPM", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 110d, 346.5d);
            _needleCalibration.Add( new CalibrationPointDouble( 33d, 110d ) );  // in gauge: 35%
            _needleCalibration.Add( new CalibrationPointDouble( 55d, 182d ) );  // in gauge: 58%
            _needleCalibration.Add( new CalibrationPointDouble( 88d, 287d ) );  // in gauge: 91%
            _needleCalibration.Add( new CalibrationPointDouble( 99d, 312d ) );  // in gauge: 101%

            Components.Add(new GaugeImage("{Ka-50}/Gauges/RotorRPM/rotor_rpm_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle = new GaugeNeedle("{Ka-50}/Gauges/RotorRPM/rotor_rpm_needle.xaml", center, new Size(25, 152), new Point(12.5, 139.5), 45d);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/RotorRPM/rotor_rpm_bezel.xaml", new Rect(0, 0, 340, 340)));

            _rpm = new HeliosValue(this, BindingValue.Empty, "", "Rotor RPM", "Current RPM of the rotor blades", "", BindingValueUnits.RPMPercent);
            _rpm.Execute += RPM_Execute;
            Actions.Add(_rpm);
        }

        private void RPM_Execute(object action, HeliosActionEventArgs e)
        {
            _rpm.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
