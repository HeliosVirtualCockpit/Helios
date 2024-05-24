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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.FuelQuantity
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.FuelQuantity", "Fuel Quantity Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class FuelQuantity : BaseGauge
    {
        private readonly HeliosValue _fuelQuantityLeft;
        private readonly HeliosValue _fuelQuantityRight;
        private readonly GaugeNeedle _needleLeft;
        private readonly GaugeNeedle _needleRight;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public FuelQuantity(): base("Fuel Quantity Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Fuel_Quantity_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needleLeft = new GaugeNeedle($"{_gaugeImagePath}F-5E_Fuel_Quantity_Needle_Left.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 163), new Point(34.5, 163 - 34.5), 206d);
            Components.Add(_needleLeft); 
            _needleRight = new GaugeNeedle($"{_gaugeImagePath}F-5E_Fuel_Quantity_Needle_Right.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 163), new Point(34.5, 163 - 34.5), 206d);
            Components.Add(_needleRight);

            _fuelQuantityLeft = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Left", "Pounds x100", "(0 to 25)", BindingValueUnits.Pounds);
            _fuelQuantityLeft.Execute += new HeliosActionHandler(FuelQuantity_Execute);
            Actions.Add(_fuelQuantityLeft);

            _fuelQuantityRight = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Right", "Pounds x100", "(0 to 25)", BindingValueUnits.Pounds);
            _fuelQuantityRight.Execute += new HeliosActionHandler(FuelQuantity_Execute);
            Actions.Add(_fuelQuantityRight);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 25d, 322d)
            {
            };
        }
        void FuelQuantity_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Fuel Quantity Left":
                        _needleLeft.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Fuel Quantity Right":
                        _needleRight.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
