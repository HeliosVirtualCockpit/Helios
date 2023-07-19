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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Instruments.PitchRatio
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    [HeliosControl("Helios.F15E.Instruments.PitchRatio.Gauge", "Pitch Ratio Gauge", "F-15E Strike Eagle", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class PitchRatioGauge : BaseGauge
    {
        private HeliosValue _pitchRatio;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _calibrationPoints;

        public PitchRatioGauge()
            : base("Pitch Ratio Gauge", new Size(300d,300d))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/F-15E/Instruments/PitchRatioDial.xaml", new Rect(0d, 0d, 300d, 300d)));

            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/CabinPressure/cabin_pressure_needle.xaml", new Point(150d, 150d), new Size(53d*0.8d, 158d * 0.8d), new Point(26.5d * 0.8d, 26.5d * 0.8d), 58d);
            Components.Add(_needle);
            string elementName = "Pitch Ratio";
            _pitchRatio = new HeliosValue(this, new BindingValue(0d), $"{Name}_{elementName}", elementName, "Pitch ratio value.", "(0 to 1)", BindingValueUnits.Numeric);
            _pitchRatio.Execute += new HeliosActionHandler(pressure_Execute);
            Actions.Add(_pitchRatio);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 1d, 245d);
        }

        void pressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }
    }
}
