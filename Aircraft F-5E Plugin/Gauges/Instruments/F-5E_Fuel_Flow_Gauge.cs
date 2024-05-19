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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.FuelFlow
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.FuelFlow", "Fuel Flow Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class FuelFlow : BaseGauge
    {
        private readonly HeliosValue _fuelFlowLeft;
        private readonly HeliosValue _fuelFlowRight;
        private readonly GaugeNeedle _needleLeft;
        private readonly GaugeNeedle _needleRight;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public FuelFlow(): base("Fuel Flow Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Fuel_Flow_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needleLeft = new GaugeNeedle($"{_gaugeImagePath}F-5E_Fuel_Flow_Needle_Left.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 163), new Point(34.5, 128.5), 26d);
            Components.Add(_needleLeft); 
            _needleRight = new GaugeNeedle($"{_gaugeImagePath}F-5E_Fuel_Flow_Needle_Right.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 163), new Point(34.5, 128.5), 26d);
            Components.Add(_needleRight);

            _fuelFlowLeft = new HeliosValue(this, new BindingValue(0d), "", "Fuel Flow Left", "Pounds x1000 per Hour", "(0 to 15)", BindingValueUnits.PoundsPerHour);
            _fuelFlowLeft.Execute += new HeliosActionHandler(FuelFlow_Execute);
            Actions.Add(_fuelFlowLeft);

            _fuelFlowRight = new HeliosValue(this, new BindingValue(0d), "", "Fuel Flow Right", "Pounds x1000 per Hour", "(0 to 15)", BindingValueUnits.PoundsPerHour);
            _fuelFlowRight.Execute += new HeliosActionHandler(FuelFlow_Execute);
            Actions.Add(_fuelFlowRight);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 15d, 308d)
            {
                new CalibrationPointDouble(1d, 51d),
                new CalibrationPointDouble(2d, 102.5d),
                new CalibrationPointDouble(3d, 154d),
                new CalibrationPointDouble(4d, 205.5d),
                new CalibrationPointDouble(7d, 233.5d),
                new CalibrationPointDouble(10d, 261.5d)
            };
        }
        void FuelFlow_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Fuel Flow Left":
                        _needleLeft.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Fuel Flow Right":
                        _needleRight.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
