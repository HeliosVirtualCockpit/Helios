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

namespace GadrocsWorkshop.Helios.Gauges.FA18C.Hydraulic
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;

    [HeliosControl("Helios.FA18C.Hydraulic", "Hydraulic Pressure", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class Hydraulic : AltImageGauge
    {
        private HeliosValue _pressure1;
        private HeliosValue _pressure2;
        private GaugeNeedle _needle1;
        private GaugeNeedle _needle2;
        private CalibrationPointCollectionDouble _needleCalibration;

        public Hydraulic()
            : base("Hydraulic Pressure", new Size(300, 300),"Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            Components.Add(new GaugeImage("{FA-18C}/Gauges/HydraulicPressure/Hyd_Pressure_Faceplate.png", new Rect(0d, 0d, 300d, 300d)));

            _needle1 = new GaugeNeedle("{FA-18C}/Gauges/HydraulicPressure/PRESS_1.png", new Point(150d, 150d), new Size(40, 146), new Point(20, 126), 80d);
            Components.Add(_needle1);

            _needle2 = new GaugeNeedle("{FA-18C}/Gauges/HydraulicPressure/PRESS_2.png", new Point(150d, 150d), new Size(40, 146), new Point(20, 126), 80d);
            Components.Add(_needle2);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 5000d, 315d);
            _pressure1 = new HeliosValue(this, new BindingValue(0d), "", "Hydraulic pressure left", "Current pressure for the left hydraulic system.", "", BindingValueUnits.PoundsPerSquareInch);
            _pressure1.Execute += new HeliosActionHandler(Pressure1_Execute);
            Actions.Add(_pressure1);

            _pressure2 = new HeliosValue(this, new BindingValue(0d), "", "Hydraulic pressure right", "Current pressure for the right hydraulic system.", "", BindingValueUnits.PoundsPerSquareInch);
            _pressure2.Execute += new HeliosActionHandler(Pressure2_Execute);
            Actions.Add(_pressure2);

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
                interfaceTriggerName: "System Gauges.Left Hyd pressure display.changed",
                deviceActionName: "set.Hydraulic pressure left"
                );
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "System Gauges.Right Hyd pressure display.changed",
                deviceActionName: "set.Hydraulic pressure right"
                );
        }

        void Pressure1_Execute(object action, HeliosActionEventArgs e)
        {
            _needle1.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void Pressure2_Execute(object action, HeliosActionEventArgs e)
        {
            _needle2.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
     }
}
