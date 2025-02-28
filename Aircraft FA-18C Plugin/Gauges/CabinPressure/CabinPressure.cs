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

    [HeliosControl("Helios.FA18C.cabinPressure", "Cabin Pressure", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class cabinPressure : AltImageGauge
    {
        private HeliosValue _pressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _calibrationPoints;

        public cabinPressure()
            : base("Cabin Pressure", new Size(300,300), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            Components.Add(new GaugeImage("{FA-18C}/Gauges/CabinPressure/Cabin_Pressure_Faceplate.png", new Rect(0d, 0d, 300d, 300d)));

            _needle = new GaugeNeedle("{FA-18C}/Gauges/CabinPressure/cabin_pressure_needle.xaml", new Point(150d, 150d), new Size(53d, 158d), new Point(26.5d, 26.5d), 0d);
            Components.Add(_needle);

            _pressure = new HeliosValue(this, new BindingValue(0d), "", "cabin pressure", "Cabin pressure of the aircraft.", "(0 to 50,000)", BindingValueUnits.Numeric);
            _pressure.Execute += new HeliosActionHandler(CabinPressure_Execute);
            Actions.Add(_pressure);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 50000d, 315d);

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
                interfaceTriggerName: "System Gauges.Cabin Altitude.changed",
                deviceActionName: "set.cabin pressure"
                );
        }

        void CabinPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }
    }
}
