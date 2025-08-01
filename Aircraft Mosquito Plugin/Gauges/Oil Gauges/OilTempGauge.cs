//  Copyright 2014 Craig Courtney
//  Copyright 2025 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.DH98Mosquito
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Gauges.DH98Mosquito.Instruments;
    using GadrocsWorkshop.Helios.Interfaces.DCS.DH98Mosquito;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Linq;

    [HeliosControl("Helios.DH98Mosquito.OilTempGaugePort", "Oil Temperature Gauge Port", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class OilTempGaugePort : OilTempGauge
    {
        public OilTempGaugePort()
            : base($"Oil Temp Gauge {VehicleSubPosition.Port}", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port, new string[] { "Temperature" })
        { }
    }

    [HeliosControl("Helios.DH98Mosquito.OilTempGaugeStarboard", "Oil Temperature Gauge Starboard", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class OilTempGaugeStarboard : OilTempGauge
    {
        public OilTempGaugeStarboard()
            : base($"Oil Temp Gauge {VehicleSubPosition.Starboard}", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Starboard, new string[] { "Temperature" })
        { }
    }


    [HeliosControl("Helios.DH98Mosquito.OilTempGauge", "Oil Temperature Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class OilTempGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeNeedle;
        private static HeliosValue _gauge;

        private static VehicleSubPosition _side;

        private static CalibrationPointCollectionDouble _scale;

    public OilTempGauge() : this("Oil Temp Gauge", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port, new string[] {"Needle" }) { }
    public OilTempGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
            _scale = new CalibrationPointCollectionDouble(0d, 0d, 1d, 120d);
    
            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };
            _side = side;

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Oil Gauges/Oil-Temp-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _gaugeNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Oil Gauges/Oil-Temp-Needle.xaml", center, new Size(26.688d * scalingFactor, 200.786d * scalingFactor), new Point(12.659d * scalingFactor, 128.875d * scalingFactor),210d);
            Components.Add(_gaugeNeedle);

            _gauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} {_side}", $"({elements[0]} {_side}", $"{_scale.MinimumInputValue} to {_scale.MaximumInputValue}", BindingValueUnits.Numeric);
            _gauge.Execute += Gauge_Execute;
            Actions.Add(_gauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Engine Instruments.Oil Temperature ({_side}) (Unscaled).changed", $"Engine Instruments_Oil Temp Gauge {_side}.set.Temperature" }
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
        void Gauge_Execute(object action, HeliosActionEventArgs e)
        {
            _gauge.SetValue(e.Value, e.BypassCascadingTriggers);
//            _gaugeNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 360d;
            _gaugeNeedle.Rotation = e.Value.DoubleValue * 300d;
        }
    }
}