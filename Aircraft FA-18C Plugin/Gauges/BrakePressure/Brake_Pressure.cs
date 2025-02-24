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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.FA18C.Instruments.BrakePressure", "Brake Pressure", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class BrakePressure : AltImageGauge
    {
        private HeliosValue _brakePressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;

        public BrakePressure()
            : base("Brake Pressue Gauge", new Size(280, 280), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            Components.Add(new GaugeImage("{FA-18C}/Gauges/BrakePressure/Brake_Pressure_Faceplate.png", new Rect(0d, 0d, 280d, 280d)));

            double needle = _needle != null ? _needle.Rotation : 0d;
            _needle = new GaugeNeedle("{Helios}/Gauges/Common/needle_a.xaml", new Point(140d, 220d), new Size(36, 154), new Point(18, 136), -40d);
            _needle.Rotation = needle;
            Components.Add(_needle);
            Components.Add(new GaugeImage("{FA-18C}/Gauges/BrakePressure/Brake_Pressure_Cover.png", new Rect(29d, 182d, 226d, 98d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 80d) {
                new CalibrationPointDouble(0.036d, 10d),
                new CalibrationPointDouble(0.338d, 30d),
                new CalibrationPointDouble(0.636d, 50d),
                new CalibrationPointDouble(0.924d, 70d)
            };

            _brakePressure = new HeliosValue(this, new BindingValue(0d), "", "Brake Pressue", "Brake Pressure in PSI.", "", BindingValueUnits.PoundsPerSquareInch);
            _brakePressure.Execute += new HeliosActionHandler(brakePressure_Execute);
            Actions.Add(_brakePressure);
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
                    interfaceTriggerName: "System Gauges.Brake pressure.changed",
                    deviceActionName: "set.Brake Pressue"
                    );
        }


        void brakePressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
