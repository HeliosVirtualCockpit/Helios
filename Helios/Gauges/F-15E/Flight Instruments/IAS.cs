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
    public class IAS : BaseGauge
    {
        private HeliosValue _indicatedAirSpeed;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        
        public IAS(string name, Size size)
            : base(name, size)
        {

            Components.Add(new GaugeImage("{Helios}/Gauges/F-15E/Flight Instruments/IASDial.xaml", new Rect(0d, 0d, 300, 300)));
            //CalibrationPointCollectionDouble AoAScale = new CalibrationPointCollectionDouble(-0.05d, -5d, 0.20d, 20d) {
            //    new CalibrationPointDouble(0d, 0d)
            //    };
            //_needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 850d, 355d);
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 360d, 360d);
            _needle = new GaugeNeedle("{Helios}/Gauges/AV-8B/Common/needle_a.xaml", new Point(150d, 150d), new Size(30, 128), new Point(15, 113), 0d);
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _indicatedAirSpeed = new HeliosValue(this, new BindingValue(0d), "", "indicated airspeed", "Current indicated airspeed of the aircraft.", "(0 - 850)", BindingValueUnits.Knots);
            _indicatedAirSpeed.Execute += new HeliosActionHandler(IndicatedAirSpeed_Execute);
            Actions.Add(_indicatedAirSpeed);
        }

        void IndicatedAirSpeed_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
