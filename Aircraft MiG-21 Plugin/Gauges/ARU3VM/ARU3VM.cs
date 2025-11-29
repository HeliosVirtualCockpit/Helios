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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.ARU3VM
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.ARU3VM", "ARU", "MiG-21", typeof(GaugeRenderer))]
    public class ARU3VM : BaseGauge
    {

        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentARU3VM;

        public ARU3VM()
            : base("ARU3VM", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 204d);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/ARU3VM/ARU3VM_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/ARU3VM/aru3vm_needle.xaml", center, new Size(100, 185), new Point(50, 127), 246d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentARU3VM = new HeliosValue(this, BindingValue.Empty, "", "Current ARU3VM", "Current ARU3VM", "", BindingValueUnits.Numeric);
            _currentARU3VM.Execute += CurrentARU3VM_Execute;
            Actions.Add(_currentARU3VM);
        }

        private void CurrentARU3VM_Execute(object action, HeliosActionEventArgs e)
        {
            _currentARU3VM.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
