//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.FA18C.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.FA18C.Instruments", "RADAR Altimeter", "F/A-18C Gauges", typeof(GaugeRenderer))]
    public class RAltimeter : BaseGauge
    {
        private HeliosValue _altitude;
        private GaugeNeedle _needle;
        private HeliosValue _min_altitude;
        private HeliosValue _redIndicator;
        private HeliosValue _greenIndicator;
        private HeliosValue _offIndicator;
        private GaugeNeedle _minimum_needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeImage _giRed;
        private GaugeImage _giGreen;
        private GaugeImage _giOff;

        public RAltimeter()
            : base("RADAR Altimeter", new Size(420, 420))
        {
            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{FA-18C}/Images/indicator_off.png", new Rect(108d, 177d, 50d, 50d)));
            Components.Add(new GaugeImage("{FA-18C}/Images/indicator_off.png", new Rect(260d, 177d, 50d, 50d)));
            Components.Add(new GaugeImage("{FA-18C}/Images/Radar Altimeter Blank.png", new Rect(179d, 288d, 56d, 22d)));

            _giRed = new GaugeImage("{FA-18C}/Images/indicator_red.png", new Rect(108d, 177d, 50d, 50d));
            Components.Add(_giRed);
            _redIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Red", "Red Indicator.", "", BindingValueUnits.Boolean);
            _redIndicator.Execute += new HeliosActionHandler(RedIndicator_Execute);
            Values.Add(_redIndicator);
            Actions.Add(_redIndicator);

            _giGreen = new GaugeImage("{FA-18C}/Images/indicator_green.png", new Rect(260d, 177d, 50d, 50d));
            Components.Add(_giGreen);
            _greenIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Green", "Green Indicator.", "", BindingValueUnits.Boolean);
            _greenIndicator.Execute += new HeliosActionHandler(GreenIndicator_Execute);
            Values.Add(_greenIndicator);
            Actions.Add(_greenIndicator);

            _giOff = new GaugeImage("{FA-18C}/Images/Radar Altimeter Off Flag.png", new Rect(179d, 287d, 56d, 24d));
            Components.Add(_giOff);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Off", "Off Indicator.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);

            Components.Add(new GaugeImage("{Helios}/Gauges/FA-18C/Altimeter/RADAR_Altimeter_Faceplate.png", new Rect(0d, 0d, 420d, 420d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0.048d, 0d, 1d, 330d);
            _needleCalibration.Add(new CalibrationPointDouble(0.000d, -12d));
            _needle = new GaugeNeedle("{Helios}/Gauges/FA-18C/Altimeter/altimeter_needle.xaml", new Point(210d, 210d), new Size(16d, 250d), new Point(8d, 200d),0d);
            Components.Add(_needle);
            _minimum_needle = new GaugeNeedle("{Helios}/Gauges/FA-18C/Altimeter/RADAR_Altimeter_Min_Needle.xaml", new Point(210d, 210d), new Size(46d, 205d), new Point(23d, 205d),0d);
            Components.Add(_minimum_needle);

            Components.Add(new GaugeImage("{Helios}/Gauges/FA-18C/Altimeter/RADAR_Altimeter_Cover.png", new Rect(94d, 11d, 89d, 88d)));  // this is the needle cover

            //Components.Add(new GaugeImage("{Helios}/Gauges/FA-18C/Common/engine_bezel.png", new Rect(0d, 0d, 400d, 400d)));

            _altitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR altitude", "Current RADAR altitude of the aircraft.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);
            _min_altitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Minimum", "Minimum Altitude setting for the aircraft in feet.", "", BindingValueUnits.Feet);
            _min_altitude.Execute += new HeliosActionHandler(Min_Altitude_Execute);
            Actions.Add(_min_altitude);

        }

        void Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void Min_Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _minimum_needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void RedIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giRed)].IsHidden = !e.Value.BoolValue;
        }
        void GreenIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giGreen)].IsHidden = !e.Value.BoolValue;
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giOff)].IsHidden = !e.Value.BoolValue;
        }
    }
}
