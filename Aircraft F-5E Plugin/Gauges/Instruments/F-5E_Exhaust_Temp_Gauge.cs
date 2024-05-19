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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.ExhaustTemperature
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F5E.Instruments.ExhaustTemperature", "Exhaust Temperature Indicator", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class ExhaustTemperatureGauge : BaseGauge
    {
        private readonly HeliosValue _exhaustTemperature;
        private readonly GaugeNeedle _needle;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public ExhaustTemperatureGauge(): base("Exhaust Temperature Indicator", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Exhaust_Temp_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Exhaust_Temp_Needle.xaml", new Point(this.Height/2, this.Width/2), new Size(51d, 154d), new Point(25.5d, 154d-25.5d), 90d);
            Components.Add(_needle);
            _exhaustTemperature = new HeliosValue(this, new BindingValue(0d), "", "Exhaust Temperature", "°c x100", "(0 to 12)", BindingValueUnits.Celsius);
            _exhaustTemperature.Execute += new HeliosActionHandler(ExhaustTemperature_Execute);
            Actions.Add(_exhaustTemperature);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 12d, 338.3d)
            {
                new CalibrationPointDouble(2d, 16.5d),
                new CalibrationPointDouble(3d, 36.7d),
                new CalibrationPointDouble(4d, 57d),
                new CalibrationPointDouble(5d, 77.5d),
                new CalibrationPointDouble(5.5d, 107.5d),
                new CalibrationPointDouble(6d, 137.5d),
                new CalibrationPointDouble(6.5d, 167.5d),
                new CalibrationPointDouble(7d, 197.5d),
                new CalibrationPointDouble(7.5d, 227.5d),
                new CalibrationPointDouble(8d, 257.5d),
                new CalibrationPointDouble(9d, 277.8d),
                new CalibrationPointDouble(10d, 298d),
                new CalibrationPointDouble(11d, 318d)
            };
        }
        void ExhaustTemperature_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
