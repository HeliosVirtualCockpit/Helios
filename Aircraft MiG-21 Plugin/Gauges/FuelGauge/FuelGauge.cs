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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.FuelGauge
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.FuelGauge", "Fuel Gauge", "MiG-21", typeof(GaugeRenderer))]
    public class FuelGauge : BaseGauge
    {
        private GaugeNeedle _currentNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentFuel;

        public FuelGauge()
            : base("FuelGauge", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 6000d, 326d);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/FuelGauge/fuelgauge_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/FuelGauge/fuelgauge_needle.xaml", center, new Size(69d, 178d), new Point(34.5d, 126.5d), -159d);
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));


            _currentFuel = new HeliosValue(this, BindingValue.Empty, "", "Fuel", "Current quantity", "", BindingValueUnits.Numeric);
            _currentFuel.Execute += CurrentFuel_Execute;
            Actions.Add(_currentFuel);
        }


        private void CurrentFuel_Execute(object action, HeliosActionEventArgs e)
        {
            _currentFuel.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
