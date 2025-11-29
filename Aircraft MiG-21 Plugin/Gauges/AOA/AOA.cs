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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.AOA
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.AOA", "AOA", "MiG-21", typeof(GaugeRenderer))]
    public class AOA : BaseGauge
    {

        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentAoa;

        public AOA()
            : base("AOA", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(-0.1745d, -53d, 0.6108d, 195d);
            _needleCalibration.Add(new CalibrationPointDouble(0d, 0d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/AOA/aoa_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/generic_needle.xaml", center, new Size(32, 185), new Point(16, 127), -154d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentAoa = new HeliosValue(this, BindingValue.Empty, "", "Current AOA", "Current AOA", "", BindingValueUnits.Numeric);
            _currentAoa.Execute += CurrentAoa_Execute;
            Actions.Add(_currentAoa);
        }

        private void CurrentAoa_Execute(object action, HeliosActionEventArgs e)
        {
            _currentAoa.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
