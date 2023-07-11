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

    [HeliosControl("Helios.F15E.FuelGauge", "Fuel Monitor Needles & Flags", "F-15E Strike Eagle", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class Fuel_Gauge : BaseGauge
        {
        private HeliosValue _internalFuel;
        private GaugeNeedle _internalFuelNeedle;
        private HeliosValue _bingoFuelAmount;
        private HeliosValue _offIndicator;
        private GaugeNeedle _bingoNeedle;
        private CalibrationPointCollectionDouble _internalFuelNeedleCalibration;
        private GaugeImage _giOff;

        public Fuel_Gauge()
            : base("Fuel Gauge", new Size(164, 164))
        {
            _giOff = new GaugeImage("{F-15C}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Off_Flag.png", new Rect(179d, 287d, 48d, 107d));
            Components.Add(_giOff);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), "", "Fuel Monitor Panel Off Indicator", "Off Indicator.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);

            _internalFuelNeedleCalibration = new CalibrationPointCollectionDouble(0.0d, 0d, 14000d, 245d);
            _internalFuelNeedle = new GaugeNeedle("{Helios}/Gauges/AV-8B/Common/needle_a.xaml", new Point(82d, 82d), new Size(36d*0.4d, 154d * 0.4d), new Point(18d * 0.4d, 136d * 0.4d), 235d);
            Components.Add(_internalFuelNeedle);
            _bingoNeedle = new GaugeNeedle("{Helios}/Gauges/F-15E/Fuel Panel/Bingo_Needle.xaml", new Point(82d, 82d), new Size(46d * .4d, 205d * .4d), new Point(23d * .4d, 205d * .4d), 235d);
            Components.Add(_bingoNeedle);

            _internalFuel = new HeliosValue(this, new BindingValue(0d), "", "Internal Fuel Amount", "Current Internal Fuel in the aircraft.", "", BindingValueUnits.Pounds);
            _internalFuel.Execute += new HeliosActionHandler(internalFuel_Execute);
            Actions.Add(_internalFuel);
            _bingoFuelAmount = new HeliosValue(this, new BindingValue(0d), "", "Bingo Minimum Fuel", "Minimum Fuel setting for the aircraft.", "", BindingValueUnits.Pounds);
            _bingoFuelAmount.Execute += new HeliosActionHandler(Min_internalFuel_Execute);
            Actions.Add(_bingoFuelAmount);

        }

        void internalFuel_Execute(object action, HeliosActionEventArgs e)
        {
            _internalFuelNeedle.Rotation = _internalFuelNeedleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void Min_internalFuel_Execute(object action, HeliosActionEventArgs e)
        {
            _bingoNeedle.Rotation = _internalFuelNeedleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giOff)].IsHidden = !e.Value.BoolValue;
        }
    }
}
