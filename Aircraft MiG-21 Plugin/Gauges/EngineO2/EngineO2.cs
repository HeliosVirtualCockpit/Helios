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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.EngineO2
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.EngineO2", "Enginge O2", "MiG-21", typeof(GaugeRenderer))]
    public class EngineO2 : BaseGauge
    {
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _needleE;

        public EngineO2()
            : base("EngineO2", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 40d, 256.8d);
            _needleCalibration.Add(new CalibrationPointDouble(10d, 64.8d));
            _needleCalibration.Add(new CalibrationPointDouble(20d, 129d));
            _needleCalibration.Add(new CalibrationPointDouble(30d, 193d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/EngineO2/engine_o2_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle = new GaugeNeedle("{MiG-21}/Gauges/Common/generic_small_gray_needle.xaml", center, new Size(32, 185), new Point(16, 127), 231.3);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _needleE = new HeliosValue(this, BindingValue.Empty, "", "Engine O2", "Engine O2", "", BindingValueUnits.Numeric);
            _needleE.Execute += Eng_Execute;
            Actions.Add(_needleE);
        }

        private void Eng_Execute(object action, HeliosActionEventArgs e)
        {
            _needleE.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
