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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Instruments.OxygenPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F15E.Instruments.OxygenPressure", "Oxygen pressure gauge", "F-15E Strike Eagle", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class OxygenPressureGauge : BaseGauge
    {
        private HeliosValue _pressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _calibrationPoints;

        public OxygenPressureGauge(string name, Size size, string device)
            : base(name, size)
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/F-15E/Instruments/OxygenPressureDial.xaml", new Rect(0d, 0d, 300d, 300d)));

            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/CabinPressure/cabin_pressure_needle.xaml", new Point(150d, 150d), new Size(53d*0.8d, 158d * 0.8d), new Point(26.5d * 0.8d, 26.5d * 0.8d), 127d);
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _pressure = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", name, $"{name} gauge value.", "(0 to 400)", BindingValueUnits.PoundsPerSquareInch);
            _pressure.Execute += new HeliosActionHandler(pressure_Execute);
            Actions.Add(_pressure);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 1d, 400d);
        }

        void pressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }
    }
}
