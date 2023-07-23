//  Copyright 2023 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E
{
    using GadrocsWorkshop.Helios.Gauges;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;

    [HeliosControl("Helios.F15E.FuelGauge", "Internal Fuel Gauge (WSO)", "F-15E Strike Eagle", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class FuelGaugeWSO : BaseGauge
        {
        private HeliosValue _internalFuel;
        private GaugeNeedle _internalFuelNeedle;
        private CalibrationPointCollectionDouble _internalFuelNeedleCalibration;
        private GaugeImage _giDial;

        public FuelGaugeWSO()
            : base("Fuel Gauge", new Size(300, 300))
        {
            _giDial = new GaugeImage("{Helios}/Gauges/F-15E/Fuel Panel/InternalDialWSO.xaml", new Rect(0d, 0d, 300d, 300d));
            Components.Add(_giDial);

            _internalFuelNeedleCalibration = new CalibrationPointCollectionDouble(0.0d, 0d, 14000d, 248d);
            _internalFuelNeedle = new GaugeNeedle("{Helios}/Gauges/F-15E/Instruments/NeedleA.xaml", new Point(150d, 150d), new Size(34, 214), new Point(17, 130), -126d);
            Components.Add(_internalFuelNeedle);

            _internalFuel = new HeliosValue(this, new BindingValue(0d), $"Fuel Gauge (WSO)_{Name}", "Internal Fuel Value", "Current Internal Fuel in the aircraft.", "", BindingValueUnits.Pounds);
            _internalFuel.Execute += new HeliosActionHandler(InternalFuelExecute);
            Actions.Add(_internalFuel);

        }

        void InternalFuelExecute(object action, HeliosActionEventArgs e)
        {
            _internalFuelNeedle.Rotation = _internalFuelNeedleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
