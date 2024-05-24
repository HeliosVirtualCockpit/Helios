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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.CabinPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F5E.Instruments.CabinPressure", "Cabin Pressure Indicator", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class CabinPressureGauge : BaseGauge
    {
        private readonly HeliosValue _cabinPressure;
        private readonly GaugeNeedle _needle;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public CabinPressureGauge(): base("Cabin Pressure Indicator", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Cabin_Pressure_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Cabin_Pressure_Needle.xaml", new Point(this.Height/2, this.Width/2), new Size(33d, 207d), new Point(16.5d, 132d), 54.5d);
            Components.Add(_needle);
            _cabinPressure = new HeliosValue(this, new BindingValue(0d), "", "Cabin Altitude Pressure", "Feet x1000", "(0 to 50)", BindingValueUnits.Feet);
            _cabinPressure.Execute += new HeliosActionHandler(CabinPressure_Execute);
            Actions.Add(_cabinPressure);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 50d, 251d);
        }
        void CabinPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
