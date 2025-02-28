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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.FA18C.Instruments", "RADAR Altimeter", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class RAltimeter : AltImageGauge
    {
        private readonly HeliosValue _altitude;
        private readonly GaugeNeedle _needle;
        private readonly HeliosValue _min_altitude;
        private readonly HeliosValue _redIndicator;
        private readonly HeliosValue _greenIndicator;
        private readonly HeliosValue _offIndicator;
        private readonly GaugeNeedle _minimum_needle;
        private readonly CalibrationPointCollectionDouble _needleCalibration;
        private readonly GaugeImage _giRed;
        private readonly GaugeImage _giGreen;
        private readonly GaugeImage _giOff;

        public RAltimeter()
            : base("RADAR Altimeter", new Size(420, 420), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{FA-18C}/Images/indicator_off.png", new Rect(108d, 177d, 50d, 50d)));
            Components.Add(new GaugeImage("{FA-18C}/Images/indicator_off.png", new Rect(260d, 177d, 50d, 50d)));
            Components.Add(new GaugeImage("{FA-18C}/Images/Radar Altimeter Blank.png", new Rect(179d, 288d, 56d, 22d)));

            bool hidden = _giRed == null || _giRed.IsHidden;
            _giRed = new GaugeImage("{FA-18C}/Images/indicator_red.png", new Rect(108d, 177d, 50d, 50d)) {
                IsHidden = hidden
             };
            Components.Add(_giRed);

            hidden = _giGreen == null || _giGreen.IsHidden;
            _giGreen = new GaugeImage("{FA-18C}/Images/indicator_green.png", new Rect(260d, 177d, 50d, 50d))
            {
                IsHidden = hidden
            };
            Components.Add(_giGreen);

            hidden = _giOff == null || _giOff.IsHidden;
            _giOff = new GaugeImage("{FA-18C}/Images/Radar Altimeter Off Flag.png", new Rect(179d, 287d, 56d, 24d))
            {
                IsHidden = hidden
            };
            Components.Add(_giOff);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/Altimeter/RADAR_Altimeter_Faceplate.png", new Rect(0d, 0d, 420d, 420d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0.048d, 0d, 1d, 330d)
            {
                new CalibrationPointDouble(0.000d, -12d)
            };
            double needle = _needle != null ? _needle.Rotation : 0d;
            _needle = new GaugeNeedle("{FA-18C}/Gauges/Altimeter/altimeter_needle.xaml", new Point(210d, 210d), new Size(16d, 250d), new Point(8d, 200d), 0d) {
                Rotation = needle
            };
            Components.Add(_needle);

            needle = _minimum_needle != null ? _minimum_needle.Rotation : 0d;
            _minimum_needle = new GaugeNeedle("{FA-18C}/Gauges/Altimeter/RADAR_Altimeter_Min_Needle.xaml", new Point(210d, 210d), new Size(46d, 205d), new Point(23d, 205d), 0d) {
                Rotation = needle
            };
            Components.Add(_minimum_needle);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/Altimeter/RADAR_Altimeter_Cover.png", new Rect(94d, 11d, 89d, 88d)));  // this is the needle cover

            //Components.Add(new GaugeImage("{FA-18C}/Gauges/Common/engine_bezel.png", new Rect(0d, 0d, 400d, 400d)));

            _redIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Red", "Red Indicator.", "", BindingValueUnits.Boolean);
            _redIndicator.Execute += new HeliosActionHandler(RedIndicator_Execute);
            Values.Add(_redIndicator);
            Actions.Add(_redIndicator);
            _greenIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Green", "Green Indicator.", "", BindingValueUnits.Boolean);
            _greenIndicator.Execute += new HeliosActionHandler(GreenIndicator_Execute);
            Values.Add(_greenIndicator);
            Actions.Add(_greenIndicator);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Off", "Off Indicator.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);
            _altitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR altitude", "Current RADAR altitude of the aircraft.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);
            _min_altitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Minimum", "Minimum Altitude setting for the aircraft in feet.", "", BindingValueUnits.Feet);
            _min_altitude.Execute += new HeliosActionHandler(Min_Altitude_Execute);
            Actions.Add(_min_altitude);

        }

        void CreateInputBindings()
        {
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "Cockpit Lights.MODE Switch.changed",
                deviceActionName: "set.Enable Alternate Image Set",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue<3"),
                triggerBindingSource: BindingValueSources.LuaScript
                );

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "Radar Altimeter ID2163A.RADAR Altitude.changed", "set.RADAR altitude" },
                { "Radar Altimeter ID2163A.Minimum Height Indicator.changed", "set.RADAR Altimeter Minimum" },
                { "Radar Altimeter ID2163A.Green Lamp.changed", "set.RADAR Altimeter Green" },
                { "Radar Altimeter ID2163A.Off Flag.changed", "set.RADAR Altimeter Off" },
                { "Radar Altimeter ID2163A.Red Lamp.changed", "set.RADAR Altimeter Red" }
            };

            foreach (string t in bindings.Keys)
            {
                AddDefaultInputBinding(
                    childName: "",
                    interfaceTriggerName: t,
                    deviceActionName: bindings[t]
                    );
            }
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
