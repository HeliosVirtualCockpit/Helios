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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Instruments.IAS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.Instruments.IAS", "Indicated Air Speed", "F-15E Strike Eagle", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class IASGauge : BaseGauge
    {
        private HeliosValue _indicatedAirSpeed;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        
        public IASGauge(string name, Size size, string device)
            : base(name, size)
        {

            Components.Add(new GaugeImage("{Helios}/Gauges/F-15E/Instruments/IASDial.xaml", new Rect(0d, 0d, 300, 300)));
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 850d, 345d)
            {
                new CalibrationPointDouble(60d, 16d),
                new CalibrationPointDouble(100d, 85d),
                new CalibrationPointDouble(150d, 146d),
                new CalibrationPointDouble(200d, 205d),
                new CalibrationPointDouble(300d, 239d),
                new CalibrationPointDouble(400d, 270d),
                new CalibrationPointDouble(500d, 288d),
                new CalibrationPointDouble(600d, 304d),
                new CalibrationPointDouble(700d, 320d),
                new CalibrationPointDouble(800d, 338d)
            };
            _needle = new GaugeNeedle("{Helios}/Gauges/F-15E/Instruments/NeedleB.xaml", new Point(150d, 150d), new Size(14d, 140d), new Point(7d, 135d), 0d);
            Components.Add(_needle);

            _indicatedAirSpeed = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", name, "Current indicated airspeed of the aircraft.", "(0 - 850)", BindingValueUnits.Knots);
            _indicatedAirSpeed.Execute += new HeliosActionHandler(IndicatedAirSpeed_Execute);
            Actions.Add(_indicatedAirSpeed);
        }

        void IndicatedAirSpeed_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
