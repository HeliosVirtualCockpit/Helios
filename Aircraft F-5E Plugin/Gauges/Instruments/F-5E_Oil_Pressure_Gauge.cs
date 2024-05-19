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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.OilPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.OilPressure", "Oil Pressure Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class OilPressure : BaseGauge
    {
        private readonly HeliosValue _oilPressureLeft;
        private readonly HeliosValue _oilPressureRight;
        private readonly GaugeNeedle _needleLeft;
        private readonly GaugeNeedle _needleRight;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public OilPressure(): base("Oil Pressure Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Oil_Pressure_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needleLeft = new GaugeNeedle($"{_gaugeImagePath}F-5E_Oil_Pressure_Needle_Left.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 156), new Point(34.5, 121.5), 180d);
            Components.Add(_needleLeft); 
            _needleRight = new GaugeNeedle($"{_gaugeImagePath}F-5E_Oil_Pressure_Needle_Right.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 156), new Point(34.5, 121.5), 180d);
            Components.Add(_needleRight);

            _oilPressureLeft = new HeliosValue(this, new BindingValue(0d), "", "Oil Pressure Left", "Percent", "(0 to 100)", BindingValueUnits.Numeric);
            _oilPressureLeft.Execute += new HeliosActionHandler(OilPressure_Execute);
            Actions.Add(_oilPressureLeft);

            _oilPressureRight = new HeliosValue(this, new BindingValue(0d), "", "Oil Pressure Right", "Percent", "(0 to 100)", BindingValueUnits.Numeric);
            _oilPressureRight.Execute += new HeliosActionHandler(OilPressure_Execute);
            Actions.Add(_oilPressureRight);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 100d, 321d)
            {
            };
        }
        void OilPressure_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Oil Pressure Left":
                        _needleLeft.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Oil Pressure Right":
                        _needleRight.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
