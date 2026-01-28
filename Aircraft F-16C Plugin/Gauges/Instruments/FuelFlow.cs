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

namespace GadrocsWorkshop.Helios.Gauges.F_16.FuelFlow
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F16.FuelFLow", "Fuel Flow", "F-16", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class FuelFlow : BaseGauge
    {
        private HeliosValue _fuelFlow;
        private HeliosValue _fuelFlowNumeric;
        private GaugeDrumCounter _drum;

        public FuelFlow()
            : base("Fuel Flow", new Size(220, 204))
        {
            Rect drumRect = new Rect(29d, 60d, 162d, 80d);

            Components.Add(new GaugeRectangle(Colors.Black, drumRect));

            _drum = new GaugeDrumCounter("{F-16C}/Gauges/Common/drum_tape.xaml", new Point(30d, 79d), "##%00", new Size(10d, 15d), new Size(32d, 48d));
            _drum.Clip = new RectangleGeometry(drumRect);
            Components.Add(_drum);

            Components.Add(new GaugeImage("{F-16C}/Gauges/Instruments/fuelflow_bezel.png", new Rect(0d, 0d, 220d, 204d)));
            Components.Add(new GaugeImage("{F-16C}/Gauges/Instruments/fuelflow_labels.xaml", new Rect(0d, 0d, 220d, 204d)));

            _fuelFlow = new HeliosValue(this, new BindingValue(0d), "", "fuel flow", "Current rate of consumption of fuel.", "", BindingValueUnits.PoundsPerHour);
            _fuelFlow.Execute += new HeliosActionHandler(FuelFlow_Execute);
            Actions.Add(_fuelFlow);
            _fuelFlowNumeric = new HeliosValue(this, new BindingValue(0d), "", "fuel flow (Numeric)", "Current rate of consumption of fuel.", "Numeric Value between 0 and 1", BindingValueUnits.Numeric);
            _fuelFlowNumeric.Execute += new HeliosActionHandler(FuelFlow_Execute);
            Actions.Add(_fuelFlowNumeric);
        }

        void FuelFlow_Execute(object action, HeliosActionEventArgs e)
        {
            double fuelFlow = 0d;
            HeliosValue hValue = action as HeliosValue;
            switch (hValue.Unit.LongName.ToLower())
            {
                case "numeric":
                    fuelFlow = e.Value.DoubleValue * 5000d;
                    break;
                case "pound per hour":
                    fuelFlow = e.Value.DoubleValue;
                    break;
                default:
                    break;
            }
            _drum.Value = fuelFlow;
        }
    }
}
