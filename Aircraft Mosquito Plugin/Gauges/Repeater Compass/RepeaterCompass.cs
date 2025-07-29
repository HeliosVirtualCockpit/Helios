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

    [HeliosControl("Helios.DH98Mosquito.RepeaterCompassGauge", "Repeater Compass", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class RepeaterCompassGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeCourseNeedle;
        private static GaugeNeedle _gaugeHeadingNeedle;
        private static HeliosValue _headingGauge;
        private static HeliosValue _courseGauge;
        private static string[] _elements;

        private static CalibrationPointCollectionDouble _headingScale;
        private static CalibrationPointCollectionDouble _courseScale;

        public RepeaterCompassGauge() : this("Repeater Compass", new Size(300, 300), "Flight Instruments", new string[] { "heading", "course" }) { }
    public RepeaterCompassGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {
            _elements = elements;
            //_headingScale = new CalibrationPointCollectionDouble(0d, 0d, 360d, 360d);
            //_courseScale = new CalibrationPointCollectionDouble(0d, 0d, 360d, 360d);

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);


            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Repeater Compass/Repeater-Compass-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _gaugeCourseNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Repeater Compass/Repeater-Compass-Course-Needle.xaml", center, new Size(80.712d * scalingFactor, 258.215d * scalingFactor), new Point(39.911d * scalingFactor, 144.294d * scalingFactor), 0d);
            Components.Add(_gaugeCourseNeedle);

            _courseGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]}", $"{elements[1]}", "0 to 360", BindingValueUnits.Degrees);
            _courseGauge.Execute += Course_Execute;
            Actions.Add(_courseGauge);

            _gaugeHeadingNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Repeater Compass/Repeater-Compass-Heading-Needle.xaml", center, new Size(33.366d * scalingFactor, 261.770d * scalingFactor), new Point(16.683d * scalingFactor, 144.294d * scalingFactor), 0d);
            Components.Add(_gaugeHeadingNeedle);

            _headingGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}", $"{elements[0]}", "0 to 360", BindingValueUnits.Degrees);
            _headingGauge.Execute += Heading_Execute;
            Actions.Add(_headingGauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Flight Instruments.Repeater Compass Heading.changed", $"Flight Instruments_{Name}.set.{_elements[0]}" },
                { $"Flight Instruments.Repeater Compass Course.changed", $"Flight Instruments_{Name}.set.{_elements[1]}" }
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
        void Heading_Execute(object action, HeliosActionEventArgs e)
        {
            _headingGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            //_gaugeHeadingNeedle.Rotation = _headingScale.Interpolate(e.Value.DoubleValue);
            _gaugeHeadingNeedle.Rotation = e.Value.DoubleValue;
        }
        void Course_Execute(object action, HeliosActionEventArgs e)
        {
            _courseGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            //_gaugeCourseNeedle.Rotation = _headingScale.Interpolate(e.Value.DoubleValue);
            _gaugeCourseNeedle.Rotation = e.Value.DoubleValue;
        }
    }
}