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

    [HeliosControl("Helios.DH98Mosquito.ArtificialHorizonGauge", "Artificial Horizon Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class ArtificialHorizonGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeBankNeedle;
        private static GaugeNeedle _gaugePitchNeedle;
        private static HeliosValue _bankGauge;
        private static HeliosValue _pitchGauge;
        private static string[] _elements;

        private static CalibrationPointCollectionDouble _pitchScale;
        private static CalibrationPointCollectionDouble _bankScale;

        public ArtificialHorizonGauge() : this("Artificial Horizon Gauge", new Size(300, 300), "Flight Instruments", new string[] { "Bank", "Pitch" }) { }
    public ArtificialHorizonGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {
            _elements = elements;
            _bankScale = new CalibrationPointCollectionDouble(-180d, 1d, 180d, -1d);
            _pitchScale = new CalibrationPointCollectionDouble(-45d, -1d, 45d, 1d);

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);
            _gaugePitchNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Artificial Horizon/Artificial-Horizon-Wings-Marker.xaml", center, new Size(223.872d * scalingFactor, 445.744d * scalingFactor), new Point(111.436d * scalingFactor, 222.872d * scalingFactor), 0d);
            _gaugePitchNeedle.Clip = new EllipseGeometry(center, 223.872d / 2d, 223.872d / 2d);
            Components.Add(_gaugePitchNeedle);

            _pitchGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]}", $"{elements[1]}", $"{_pitchScale.MinimumInputValue} to {_pitchScale.MaximumInputValue}", BindingValueUnits.Degrees);
            _pitchGauge.Execute += Pitch_Execute;
            Actions.Add(_pitchGauge);

            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Artificial Horizon/Artificial-Horizon-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _gaugeBankNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Artificial Horizon/Artificial-Horizon-Bank-Marker.xaml", center, new Size(228.692d * scalingFactor, 228.822d * scalingFactor), new Point(113.885d * scalingFactor, 114.346d * scalingFactor), 0d);
            Components.Add(_gaugeBankNeedle);

            _bankGauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}", $"{elements[0]}", $"{_bankScale.MinimumInputValue} to {_bankScale.MaximumInputValue}", BindingValueUnits.Degrees);
            _bankGauge.Execute += Bank_Execute;
            Actions.Add(_bankGauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Flight Instruments.Artificial Horizon Bank.changed", $"Flight Instruments_{Name}.set.{_elements[0]}" },
                { $"Flight Instruments.Artificial Horizon Pitch.changed", $"Flight Instruments_{Name}.set.{_elements[1]}" }
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
        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _bankGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaugeBankNeedle.Rotation = _bankScale.Interpolate(e.Value.DoubleValue) * 180d;
            _gaugePitchNeedle.Rotation = _bankScale.Interpolate(e.Value.DoubleValue) * 180d;
            //_gaugeBankNeedle.Rotation = e.Value.DoubleValue * 360d;
        }
        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchGauge.SetValue(e.Value, e.BypassCascadingTriggers);
            // _gaugePitchNeedle = _pitchScale.Interpolate(e.Value.DoubleValue) * 360d;
            _gaugePitchNeedle.VerticalOffset = _pitchScale.Interpolate(e.Value.DoubleValue) * 223.872d / 2d;
        }
    }
}