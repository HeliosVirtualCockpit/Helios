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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.Tachometer
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.Tachometer", "Tachometer", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class Tachometer : BaseGauge
    {
        private GaugeNeedle _needle1;
        private GaugeNeedle _needle2;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _rightRpm;
        private HeliosValue _leftRpm;

        public Tachometer()
            : base("Tachometer", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 110d, 346.5d);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/Tachometer/tachometer_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle2 = new GaugeNeedle("{Ka-50}/Gauges/Tachometer/tachometer_needle_2.xaml", center, new Size(25, 152), new Point(12.5, 139.5), 45d);
            Components.Add(_needle2);

            _needle1 = new GaugeNeedle("{Ka-50}/Gauges/Tachometer/tachometer_needle_1.xaml", center, new Size(25, 152), new Point(12.5, 139.5), 45d);
            Components.Add(_needle1);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/Tachometer/tachometer_bezel.xaml", new Rect(0, 0, 340, 340)));

            _rightRpm = new HeliosValue(this, BindingValue.Empty, "", "Right Engine RPM", "Current RPM of the rotor blades", "", BindingValueUnits.RPMPercent);
            _rightRpm.Execute += RightRPM_Execute;
            Actions.Add(_rightRpm);

            _leftRpm = new HeliosValue(this, BindingValue.Empty, "", "Left Engine RPM", "Current RPM of the rotor blades", "", BindingValueUnits.RPMPercent);
            _leftRpm.Execute += LeftRPM_Execute;
            Actions.Add(_leftRpm);


        }

        private void RightRPM_Execute(object action, HeliosActionEventArgs e)
        {
            _rightRpm.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle2.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void LeftRPM_Execute(object action, HeliosActionEventArgs e)
        {
            _leftRpm.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle1.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
