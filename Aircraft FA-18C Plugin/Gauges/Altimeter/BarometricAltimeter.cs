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

    [HeliosControl("Helios.FA18C.Instruments.BAltimeter", "Altimeter", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class BAltimeter : AltImageGauge
    {
        private HeliosValue _altitude;
        private HeliosValue _airPressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _drum;
        private GaugeDrumCounter _airPressureDrum;

        public BAltimeter()
            : base("Barometric Altimeter", new Size(376, 376), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            _tensDrum = new GaugeDrumCounter("{FA-18C}/Gauges/Altimeter/alt_drum_tape.xaml", new Point(73d, 129d), "#", new Size(10d, 15d), new Size(31d, 38d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(71d, 144d, 31d, 38d));
            Components.Add(_tensDrum);

            _drum = new GaugeDrumCounter("{FA-18C}/Gauges/Common/drum_tape.xaml", new Point(123d, 128d), "#%00", new Size(10d, 15d), new Size(31d, 38d));
            _drum.Clip = new RectangleGeometry(new Rect(123d, 130d, 31d, 38d));
            Components.Add(_drum);

            _airPressureDrum = new GaugeDrumCounter("{FA-18C}/Gauges/Common/drum_tape.xaml", new Point(142d, 276d), "###%", new Size(10d, 15d), new Size(24d, 32d));
            _airPressureDrum.Value = 2992d;
            _airPressureDrum.Clip = new RectangleGeometry(new Rect(142d, 276d, 96d, 32d));
            Components.Add(_airPressureDrum);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/Altimeter/Altimeter_Faceplate.png", new Rect(0d, 0d, 376d, 376d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _needle = new GaugeNeedle("{FA-18C}/Gauges/Altimeter/altimeter_needle.xaml", new Point(188d, 188d), new Size(16d, 160d), new Point(8d, 160d));
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{FA-18C}/Gauges/Common/engine_bezel.png", new Rect(0d, 0d, 376d, 376d)));
            _altitude = new HeliosValue(this, new BindingValue(0d), "", "altitude", "Current altitude of the aircraft in feet.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);

            _airPressure = new HeliosValue(this, new BindingValue(0d), "", "air pressure", "Current air pressure calibaration setting for the altimeter.", "", BindingValueUnits.InchesOfMercury);
            _airPressure.SetValue(new BindingValue(29.92d), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);
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
                { "Standby Baro Altimeter AAU-52/A.Pressure.changed", "set.air pressure" },
                { "Standby Baro Altimeter AAU-52/A.Altitude.changed", "set.altitude" }
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
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue % 1000d);
            _tensDrum.Value = e.Value.DoubleValue / 10000d;

            // Setup then thousands drum to roll with the rest
            double thousands = (e.Value.DoubleValue / 100d) % 100d;
            if (thousands >= 99)
            {
                _tensDrum.StartRoll = thousands % 1d;
            }
            else
            {
                _tensDrum.StartRoll = -1d;
            }
            _drum.Value = e.Value.DoubleValue;
        }

        void AirPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _airPressureDrum.Value = e.Value.DoubleValue * 100d;
        }
    }
}
