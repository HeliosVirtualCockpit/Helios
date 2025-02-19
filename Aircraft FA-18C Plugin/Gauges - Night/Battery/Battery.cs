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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.FA18C.Battery2", "Battery - Night", "F/A-18C Gauges - Night", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class Battery2 : BaseGauge
    {
        private HeliosValue _voltageU;
        private GaugeNeedle _needleU;
        private HeliosValue _voltageE;
        private GaugeNeedle _needleE;
        private CalibrationPointCollectionDouble _calibrationPointsU;
        private CalibrationPointCollectionDouble _calibrationPointsE;

        public Battery2()
            : base("voltage", new Size(300, 300))
        {
            Components.Add(new GaugeImage("{FA-18C}/Gauges/Battery/N Battery_Faceplate.png", new Rect(0d, 0d, 300d, 300d)));
            _needleU = new GaugeNeedle("{FA-18C}/Gauges/Common/N_needle_a.xaml", new Point(150d, 150d), new Size(36, 154), new Point(18, 136), -180d);
            _needleE = new GaugeNeedle("{FA-18C}/Gauges/Common/N_needle_a.xaml", new Point(150d, 150d), new Size(36, 154), new Point(18, 136), 180d);
            Components.Add(_needleU);
            Components.Add(_needleE);
            Components.Add(new GaugeImage("{FA-18C}/Gauges/Battery/N Battery_Needle_Cover.png", new Rect(118d, 115d, 62d, 162d)));

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _voltageU = new HeliosValue(this, new BindingValue(0d), "", "Battery Voltage U", "This is the voltage of the battery in volts", "(16 to 30)", BindingValueUnits.Volts);
            _voltageU.Execute += new HeliosActionHandler(voltageU_Execute);
            Actions.Add(_voltageU);
            _voltageE = new HeliosValue(this, new BindingValue(0d), "", "Battery Voltage E", "This is the voltage of the battery in volts", "(16 to 30)", BindingValueUnits.Volts);
            _voltageE.Execute += new HeliosActionHandler(voltageE_Execute);
            Actions.Add(_voltageE);

            _calibrationPointsU = new CalibrationPointCollectionDouble(16d, 30d, 30d, 150d);
            _calibrationPointsU.Add(new CalibrationPointDouble(15d, 30d));  // used to set an end stop at 16v
            _calibrationPointsE = new CalibrationPointCollectionDouble(16d, -30d, 30d, -150d);
            _calibrationPointsE.Add(new CalibrationPointDouble(15d, -30d));  // used to set an end stop at 16v

        }

        void voltageU_Execute(object action, HeliosActionEventArgs e)
        {
            _needleU.Rotation = _calibrationPointsU.Interpolate(e.Value.DoubleValue);
        }
        void voltageE_Execute(object action, HeliosActionEventArgs e)
        {
            _needleE.Rotation = _calibrationPointsE.Interpolate(e.Value.DoubleValue);
        }
    }
}
