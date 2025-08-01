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

    [HeliosControl("Helios.DH98Mosquito.SlipTurnGauge", "Side-Slip / Turn Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class SlipTurnGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeTurnNeedle;
        private static GaugeNeedle _gaugeSlipNeedle;
        private static HeliosValue _turnGauge;
        private static HeliosValue _slipGauge;
        private static string[] _elements;

        private static CalibrationPointCollectionDouble _slipScale;
        private static CalibrationPointCollectionDouble _turnScale;

        public SlipTurnGauge() : this("Side-Slip / Turn Gauge", new Size(300, 300), "Flight Instruments", new string[] { "Turn", "Slip" }) { }
    public SlipTurnGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {
            _elements = elements;
            _turnScale = new CalibrationPointCollectionDouble(-40d, 44d, 40d, -44d);
            _slipScale = new CalibrationPointCollectionDouble(-20d, -35d, 20d, 35d);

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);

            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Slip Turn/Slip-Turn-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _gaugeSlipNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Slip Turn/Slip-Turn-Slip-Needle.xaml", center, new Size(87.086d * scalingFactor, 146.746d * scalingFactor), new Point(43.093d * scalingFactor, 103.203d * scalingFactor), 0d);
            Components.Add(_gaugeSlipNeedle);

            _slipGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]}", $"{elements[1]}", $"{_slipScale.MinimumInputValue} to {_slipScale.MaximumInputValue}", BindingValueUnits.Degrees);
            _slipGauge.Execute += Slip_Execute;
            Actions.Add(_slipGauge);


            _gaugeTurnNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Slip Turn/Slip-Turn-Turn-Needle.xaml", center, new Size(87.086d * scalingFactor, 158.208d * scalingFactor), new Point(43.093d * scalingFactor, 43.543d * scalingFactor), 0d);
            Components.Add(_gaugeTurnNeedle);

            _turnGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}", $"{elements[0]}", $"{_turnScale.MinimumInputValue} to {_turnScale.MaximumInputValue}", BindingValueUnits.Degrees);
            _turnGauge.Execute += Turn_Execute;
            Actions.Add(_turnGauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Flight Instruments.Turn.changed", $"Flight Instruments_{Name}.set.{_elements[0]}" },
                { $"Flight Instruments.Side Slip.changed", $"Flight Instruments_{Name}.set.{_elements[1]}" }
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
        void Turn_Execute(object action, HeliosActionEventArgs e)
        {
            _turnGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaugeTurnNeedle.Rotation = _turnScale.Interpolate(e.Value.DoubleValue);
        }
        void Slip_Execute(object action, HeliosActionEventArgs e)
        {
            _slipGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaugeSlipNeedle.Rotation = _slipScale.Interpolate(e.Value.DoubleValue);
        }
    }
}