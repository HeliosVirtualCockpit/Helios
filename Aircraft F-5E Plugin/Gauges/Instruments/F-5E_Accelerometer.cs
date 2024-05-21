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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.Accelerometer
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.Accelerometer", "Accelerometer Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class Accelerometer : BaseGauge
    {
        private readonly HeliosValue _accelerometerCurrent;
        private readonly HeliosValue _accelerometerHWM;
        private readonly HeliosValue _accelerometerLWM;
        private readonly GaugeNeedle _needleCurrent;
        private readonly GaugeNeedle _needleHWM;
        private readonly GaugeNeedle _needleLWM;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public Accelerometer(): base("Accelerometer Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Accelerometer_FacePlate.xaml", new Rect(0d, 0d, this.Width, this.Height)));

            _needleLWM = new GaugeNeedle($"{_gaugeImagePath}F-5E_Accelerometer_Needle_Min_Max.xaml", new Point(this.Width / 2, this.Height / 2), new Size(34d, 221d), new Point(17, 132), 135d);
            Components.Add(_needleLWM);
            _needleHWM = new GaugeNeedle($"{_gaugeImagePath}F-5E_Accelerometer_Needle_Min_Max.xaml", new Point(this.Width / 2, this.Height / 2), new Size(34d, 221d), new Point(17, 132), 135d);
            Components.Add(_needleHWM);
            _needleCurrent = new GaugeNeedle($"{_gaugeImagePath}F-5E_Accelerometer_Needle_Current.xaml", new Point(this.Width / 2, this.Height / 2), new Size(34d, 221d), new Point(17, 132), 135d);
            Components.Add(_needleCurrent);

            _accelerometerCurrent = new HeliosValue(this, new BindingValue(0d), "", "Accelerometer Current G", "Number", "(-5 to 10)", BindingValueUnits.Numeric);
            _accelerometerCurrent.Execute += new HeliosActionHandler(Accelerometer_Execute);
            Actions.Add(_accelerometerCurrent);

            _accelerometerHWM = new HeliosValue(this, new BindingValue(0d), "", "Accelerometer Maximum G", "Number", "(-5 to 10)", BindingValueUnits. Numeric);
            _accelerometerHWM.Execute += new HeliosActionHandler(Accelerometer_Execute);
            Actions.Add(_accelerometerHWM);

            _accelerometerLWM = new HeliosValue(this, new BindingValue(0d), "", "Accelerometer Minimum G", "Number", "(-5 to 10)", BindingValueUnits.Numeric);
            _accelerometerLWM.Execute += new HeliosActionHandler(Accelerometer_Execute);
            Actions.Add(_accelerometerLWM);

            _calibrationPoints = new CalibrationPointCollectionDouble(-5d, 0d, 10d, 337.5d)
            {
            };
        }
        void Accelerometer_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Accelerometer Current G":
                        _needleCurrent.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Accelerometer Maximum G":
                        _needleHWM.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Accelerometer Minimum G":
                        _needleLWM.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
