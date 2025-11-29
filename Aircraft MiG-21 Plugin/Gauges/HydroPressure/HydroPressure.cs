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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.HydroPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.HydroPressure", "Hydraulic Pressure", "MiG-21", typeof(GaugeRenderer))]
    public class HydroPressure : BaseGauge
    {
        private GaugeNeedle _oneNeedle;
        private GaugeNeedle _twoNeedle;

        private CalibrationPointCollectionDouble _needleCalibration1;
        private CalibrationPointCollectionDouble _needleCalibration2;

        private HeliosValue _oneE;
        private HeliosValue _twoE;

        public HydroPressure()
            : base("HydroPressure", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration1 = new CalibrationPointCollectionDouble(0d, 0d, 300d, 131d);

            _needleCalibration2 = new CalibrationPointCollectionDouble(0d, 0d, 300d, -131);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/HydroPressure/hydropressure_faceplate.xaml", new Rect(15, 15, 310, 310)));

            Point mainneedle = new Point(266, 266);
            Point secondneedle = new Point(71, 73);

            _oneNeedle = new GaugeNeedle("{MiG-21}/Gauges/HydroPressure/hydropressure_needle.xaml", mainneedle, new Size(20, 130), new Point(10, 120), 248);
            Components.Add(_oneNeedle);

            _twoNeedle = new GaugeNeedle("{MiG-21}/Gauges/HydroPressure/hydropressure_needle.xaml", secondneedle, new Size(20, 130), new Point(10, 120), 201);
            Components.Add(_twoNeedle);
 
            Components.Add(new GaugeImage("{MiG-21}/Gauges/HydroPressure/hydropressure_needle_cover.xaml", new Rect(235, 235, 70, 70)));
            Components.Add(new GaugeImage("{MiG-21}/Gauges/HydroPressure/hydropressure_needle_cover_second.xaml", new Rect(35, 35, 70, 70)));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _oneE = new HeliosValue(this, BindingValue.Empty, "", "Pressure Main", "Pressure Main", "", BindingValueUnits.Numeric);
            _oneE.Execute += OneEng_Execute;
            Actions.Add(_oneE);

            _twoE = new HeliosValue(this, BindingValue.Empty, "", "Pressure Secondary", "Pressure Secondary", "", BindingValueUnits.Numeric);
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
            _twoNeedle.Rotation = _needleCalibration2.Interpolate(e.Value.DoubleValue);
        }

    }
}
