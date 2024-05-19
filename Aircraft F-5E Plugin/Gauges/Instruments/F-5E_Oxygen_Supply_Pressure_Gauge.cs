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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.O2SupplyPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F5E.Instruments.OxygenSuppyPressure", "Oxygen Supply Pressure Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class OxygenSuppy : BaseGauge
    {
        private readonly HeliosValue _O2SupplyPressure;
        private readonly GaugeNeedle _needle;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public OxygenSuppy(): base("Oxygen Supply Pressure Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Oxygen_Supply_Pressure_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Oxygen_Supply_Pressure_Needle.xaml", new Point(this.Height/2, this.Width/2), new Size(25, 133), new Point(12.5, 120.5), 270d);
            Components.Add(_needle);
            _O2SupplyPressure = new HeliosValue(this, new BindingValue(0d), "", "Oxygen Supply Pressure", "PSI", "(0 to 500)", BindingValueUnits.PoundsPerSquareInch);
            _O2SupplyPressure.Execute += new HeliosActionHandler(O2SupplyPressure_Execute);
            Actions.Add(_O2SupplyPressure);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 500d, 180d)
            {
                new CalibrationPointDouble(100d, 90d),
                new CalibrationPointDouble(200d, 112.5d),
                new CalibrationPointDouble(300d, 135d),
            };
        }
        void O2SupplyPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
