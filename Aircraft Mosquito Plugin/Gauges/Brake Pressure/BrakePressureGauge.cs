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

    [HeliosControl("Helios.DH98Mosquito.BrakePressureGauge", "Brake Pressure Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BrakePressureGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeLeftNeedle;
        private static GaugeNeedle _gaugeRightNeedle;
        private static GaugeNeedle _gaugePressureNeedle;
        private static HeliosValue _leftGauge;
        private static HeliosValue _rightGauge;
        private static HeliosValue _pressureGauge;
        private static string[] _elements;

        private static CalibrationPointCollectionDouble _pressureScale;
        private static CalibrationPointCollectionDouble _leftScale;
        private static CalibrationPointCollectionDouble _rightScale;

        public BrakePressureGauge() : this("Brake Pressure Gauge", new Size(300, 300), "Systematic Instruments", new string[] { "Pneumatic Pressure", "Wheel Brake Gauge (Left)", "Wheel Brake Gauge (Right)" }) { }
    public BrakePressureGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {
            _elements = elements;
            _leftScale = _rightScale = new CalibrationPointCollectionDouble(0d, 00d, 130d, 138d);
            _pressureScale = new CalibrationPointCollectionDouble(0d, 0d, 220d, 180d);

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);

            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Brake Pressure/Brake-Pressure-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _gaugePressureNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Brake Pressure/Brake-Pressure-Needle-1.xaml", center, new Size(34.253d * scalingFactor, 152.034d * scalingFactor), new Point(16.407d * scalingFactor, 134.908d * scalingFactor), -90d);
            Components.Add(_gaugePressureNeedle);

            _pressureGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}", $"{elements[0]}", $"{_pressureScale.MinimumInputValue} to {_pressureScale.MaximumInputValue}", BindingValueUnits.Numeric);
            _pressureGauge.Execute += Pressure_Execute;
            Actions.Add(_pressureGauge);

            _gaugeLeftNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Brake Pressure/Brake-Pressure-Needle-2.xaml", new Point(131.773d, 181.916d), new Size(32.175d * scalingFactor, 123.438d * scalingFactor), new Point(15.368d * scalingFactor, 108.265d * scalingFactor), -169d);
            Components.Add(_gaugeLeftNeedle);

            _leftGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]}", $"{elements[1]}", $"{_leftScale.MinimumInputValue} to {_leftScale.MaximumInputValue}", BindingValueUnits.Numeric);
            _leftGauge.Execute += Left_Execute;
            Actions.Add(_leftGauge);
 
            _gaugeRightNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Brake Pressure/Brake-Pressure-Needle-2.xaml", new Point(164.867d, 181.916d), new Size(32.175d * scalingFactor, 123.438d * scalingFactor), new Point(15.368d * scalingFactor, 108.265d * scalingFactor), 169d);
            Components.Add(_gaugeRightNeedle);

            _rightGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[2], $"{elements[2]}", $"{elements[2]}", $"{_rightScale.MinimumInputValue} to {_rightScale.MaximumInputValue}", BindingValueUnits.Numeric);
            _rightGauge.Execute += Right_Execute;
            Actions.Add(_rightGauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Systematic Instruments.{_elements[0]}.changed", $"Systematic Instruments_{Name}.set.{_elements[0]}" },
                { $"Systematic Instruments.{_elements[1]}.changed", $"Systematic Instruments_{Name}.set.{_elements[1]}" },
                { $"Systematic Instruments.{_elements[2]}.changed", $"Systematic Instruments_{Name}.set.{_elements[2]}" }
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
        void Left_Execute(object action, HeliosActionEventArgs e)
        {
            _leftGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaugeLeftNeedle.Rotation = _leftScale.Interpolate(e.Value.DoubleValue);
        }
        void Right_Execute(object action, HeliosActionEventArgs e)
        {
            _rightGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaugeRightNeedle.Rotation = -_rightScale.Interpolate(e.Value.DoubleValue);
        }
        void Pressure_Execute(object action, HeliosActionEventArgs e)
        {
            _pressureGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaugePressureNeedle.Rotation = _pressureScale.Interpolate(e.Value.DoubleValue);
        }
    }
}