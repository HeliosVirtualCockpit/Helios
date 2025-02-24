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

namespace GadrocsWorkshop.Helios.Gauges.FA18C.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.FA18C.Instruments.O2Gauge", "Liquid Oxygen", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class O2Gauge : AltImageGauge
    {
        private HeliosValue _LOx;
        private HeliosValue _airPressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _airPressureDrum;

        public O2Gauge()
            : base("O2 Gauge", new Size(300, 300), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            _airPressureDrum = new GaugeDrumCounter("{FA-18C}/Gauges/Common/drum_tape.xaml", new Point(128d, 258d), "###%", new Size(10d, 15d), new Size(12d, 28d));
            _airPressureDrum.Value = 2992d;
            _airPressureDrum.Clip = new RectangleGeometry(new Rect(128d, 258d, 48d, 28d));
            Components.Add(_airPressureDrum);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/LOx/LOx_Faceplate.png", new Rect(0d, 0d, 300d, 300d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, -90d, 1d, 90d);
            _needle = new GaugeNeedle("{Helios}/Gauges/Common/needle_a.xaml", new Point(150d, 150d), new Size(30, 128), new Point(15, 113), -90d);
            Components.Add(_needle);

            _airPressure = new HeliosValue(this, new BindingValue(0d), "", "air pressure", "Current air pressure calibaration setting for the altimeter.", "", BindingValueUnits.InchesOfMercury);
            _airPressure.SetValue(new BindingValue(29.92), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);

            _LOx = new HeliosValue(this, new BindingValue(0d), "", "Oxygen Flow", "Current Liquid Oxygen in litres.", "", BindingValueUnits.Numeric);
            _LOx.Execute += new HeliosActionHandler(LOx_Execute);
            Actions.Add(_LOx);
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

            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "Standby Baro Altimeter AAU-52/A.Pressure.changed",
                deviceActionName: "set.air pressure"
                );
        }

        void LOx_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        void AirPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _airPressureDrum.Value = e.Value.DoubleValue * 100d;
        }
    }
}
