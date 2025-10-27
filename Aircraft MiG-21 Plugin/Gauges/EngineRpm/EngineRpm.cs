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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.EngingeRpm
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.EngingeRpm", "Enginge RPM", "MiG-21", typeof(GaugeRenderer))]
    public class EngingeRpm : BaseGauge
    {
        private GaugeNeedle _oneNeedle;
        private GaugeNeedle _twoNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;
        private CalibrationPointCollectionDouble _needleCalibration1;

        private HeliosValue _oneE;
        private HeliosValue _twoE;

        public EngingeRpm()
            : base("EngingeRpm", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 110d, 344.5d);
            _needleCalibration.Add(new CalibrationPointDouble(100d, 315d));

            _needleCalibration1 = new CalibrationPointCollectionDouble(0d, 0d, 110d, 344.5d);
            _needleCalibration1.Add(new CalibrationPointDouble(25d, 6d));  
            _needleCalibration1.Add(new CalibrationPointDouble(65d, 158d));  
            _needleCalibration1.Add(new CalibrationPointDouble(92d, 281d));   
            _needleCalibration1.Add(new CalibrationPointDouble(100d, 318d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/EngineRpm/enginerpm_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _twoNeedle = new GaugeNeedle("{MiG-21}/Gauges/EngineRpm/enginerpm_needle2.xaml", center, new Size(32, 185), new Point(16, 127), 44);
            Components.Add(_twoNeedle);

            _oneNeedle = new GaugeNeedle("{MiG-21}/Gauges/EngineRpm/enginerpm_needle1.xaml", center, new Size(32, 185), new Point(16, 127), 44);
            Components.Add(_oneNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/EngineRpm/enginerpm_bezel.xaml", new Rect(0, 0, 340, 340)));

            _oneE = new HeliosValue(this, BindingValue.Empty, "", "rpm1", "Engine rpm 1", "", BindingValueUnits.Numeric);
            _oneE.Execute += OneEng_Execute;
            Actions.Add(_oneE);

            _twoE = new HeliosValue(this, BindingValue.Empty, "", "rpm2", "Engine rpm 2", "", BindingValueUnits.Numeric);
            _twoE.Execute += TwoEng_Execute;
            Actions.Add(_twoE);

        }

        private void OneEng_Execute(object action, HeliosActionEventArgs e)
        {
            _oneE.SetValue(e.Value, e.BypassCascadingTriggers);
            _oneNeedle.Rotation = _needleCalibration1.Interpolate(e.Value.DoubleValue);
        }

        private void TwoEng_Execute(object action, HeliosActionEventArgs e)
        {
            _twoE.SetValue(e.Value, e.BypassCascadingTriggers);
            _twoNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
