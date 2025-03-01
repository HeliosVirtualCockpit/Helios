﻿//  Copyright 2014 Craig Courtney
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
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.FA18C.IAS", "Indicated Air Speed", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class IAS : AltImageGauge
    {
        private HeliosValue _indicatedAirSpeed;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
 
        public IAS()
            : base("IAS", new Size(300, 300), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            Components.Add(new GaugeImage("{FA-18C}/Gauges/IAS/IAS.png", new Rect(0d, 0d, 300, 300)));

            _needle = new GaugeNeedle("{Helios}/Gauges/Common/needle_a.xaml", new Point(150d, 150d), new Size(30, 128), new Point(15, 113), 0d);
            Components.Add(_needle);


            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 360d, 346d);

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _indicatedAirSpeed = new HeliosValue(this, new BindingValue(0d), "", "indicated airspeed", "Current indicated airspeed of the aircraft.", "(0 - 850)", BindingValueUnits.Knots);
            _indicatedAirSpeed.Execute += new HeliosActionHandler(IndicatedAirSpeed_Execute);
            Actions.Add(_indicatedAirSpeed);
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
                interfaceTriggerName: "Flight Instruments.IAS Airspeed.changed",
                deviceActionName: "set.indicated airspeed"
                );
        }

        void IndicatedAirSpeed_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
