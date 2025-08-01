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

    [HeliosControl("Helios.DH98Mosquito.FlapsIndicator", "Flaps Indicator", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class FlapsIndicator : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeNeedle;
        private static HeliosValue _gauge;


        private static CalibrationPointCollectionDouble _scale;

        public FlapsIndicator() : this("Flaps Indicator", new Size(300, 300), "Flight Instruments", new string[] { "Position" }) { }
    public FlapsIndicator(string name, Size size, string device,  string[] elements)
            : base(name, size)
        {
            _scale = new CalibrationPointCollectionDouble(0d, 0d, 70d, 200d);

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Flaps Indicator/Flaps-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _gaugeNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Flaps Indicator/Flaps-Needle.xaml", center, new Size(31.815d * scalingFactor, 174.459d * scalingFactor), new Point(15.241d * scalingFactor, 122.551d * scalingFactor),-10d);
            Components.Add(_gaugeNeedle);

            _gauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}", $"{elements[0]}", $"0 to 70", BindingValueUnits.Degrees);
            _gauge.Execute += Gauge_Execute;
            Actions.Add(_gauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Systematic Instruments.Flaps.changed", $"Flight Instruments_Flaps Indicator.set.Position" }
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
            _gaugeNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue);
        }
    }
}