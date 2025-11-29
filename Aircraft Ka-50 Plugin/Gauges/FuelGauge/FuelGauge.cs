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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.FuelGauge
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.Fuel", "Fuel Quantity Indicator", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class FuelGauge : BaseGauge
    {
        private GaugeNeedle _needleFwd;
        private GaugeNeedle _needleAft;
        private CalibrationPointCollectionDouble _needleCalibration;

        private GaugeImage _fwdTankLamp;
        private GaugeImage _aftTankLamp;

        private HeliosValue _aftFuelQty;
        private HeliosValue _fwdFuelQty;
        private HeliosValue _fwdTankIndicator;
        private HeliosValue _aftTankIndicator;

        public FuelGauge()
            : base("Fuel Quantity Indicator", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 800d, 300d);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/FuelGauge/fuel_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _fwdTankLamp = new GaugeImage("{Ka-50}/Gauges/FuelGauge/fuel_warning_indicator.xaml", new Rect(146, 242, 15, 15));
            Components.Add(_fwdTankLamp);

            _aftTankLamp = new GaugeImage("{Ka-50}/Gauges/FuelGauge/fuel_warning_indicator.xaml", new Rect(179, 242, 15, 15));
            Components.Add(_aftTankLamp);

            _needleAft = new GaugeNeedle("{Ka-50}/Gauges/FuelGauge/fuel_needle_a.xaml", center, new Size(64, 171), new Point(32, 139), 210d);
            Components.Add(_needleAft);

            _needleFwd = new GaugeNeedle("{Ka-50}/Gauges/FuelGauge/fuel_needle_f.xaml", center, new Size(64, 171), new Point(32, 139), 210d);
            Components.Add(_needleFwd);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/FuelGauge/fuel_bezel.xaml", new Rect(0, 0, 340, 340)));

            _aftFuelQty = new HeliosValue(this, BindingValue.Empty, "", "Aft Fuel Quantity", "Amount of fuel remaining in aft fuel tank.", "", BindingValueUnits.Kilograms);
            _aftFuelQty.Execute += AftQty_Execute;
            Actions.Add(_aftFuelQty);

            _fwdFuelQty = new HeliosValue(this, BindingValue.Empty, "", "Fwd Fuel Quantity", "Amount of fuel remaining in forward fuel tank.", "", BindingValueUnits.Kilograms);
            _fwdFuelQty.Execute += FwdQty_Execute;
            Actions.Add(_fwdFuelQty);

            _fwdTankIndicator = new HeliosValue(this, new BindingValue(false), "", "Fwd Tank Lamp", "Indicates whether the forward tank lamp is lit.", "True if lit.", BindingValueUnits.Boolean);
            _fwdTankIndicator.Execute += new HeliosActionHandler(FwdTankIndicator_Execute);
            Actions.Add(_fwdTankIndicator);

            _aftTankIndicator = new HeliosValue(this, new BindingValue(false), "", "Aft Tank Lamp", "Indicates whether the aft tank lamp is lit.", "True if lit.", BindingValueUnits.Boolean);
            _aftTankIndicator.Execute += new HeliosActionHandler(AftTankIndicator_Execute);
            Actions.Add(_aftTankIndicator);
        }

        void FwdTankIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _fwdTankIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
            _fwdTankLamp.IsHidden = !e.Value.BoolValue;
        }

        void AftTankIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _aftTankIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
            _aftTankLamp.IsHidden = !e.Value.BoolValue;
        }

        private void AftQty_Execute(object action, HeliosActionEventArgs e)
        {
            _aftFuelQty.SetValue(e.Value, e.BypassCascadingTriggers);
            _needleAft.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void FwdQty_Execute(object action, HeliosActionEventArgs e)
        {
            _fwdFuelQty.SetValue(e.Value, e.BypassCascadingTriggers);
            _needleFwd.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
