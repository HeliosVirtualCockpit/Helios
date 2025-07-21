//  Copyright 2014 Craig Courtney
//  Copyright 2024 Helios Contributors
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
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.DH98Mosquito.RPMGaugePort", "RPM Gauge Port", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class RPMGaugePort : RPMGauge
    {
        public RPMGaugePort()
            : base($"RPM Gauge {VehicleSubPosition.Port}", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port)
        { }
    }

    [HeliosControl("Helios.DH98Mosquito.RPMGaugeStarboard", "RPM Gauge Starboard", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class RPMGaugeStarboard : RPMGauge
    {
        public RPMGaugeStarboard()
            : base($"RPM Gauge {VehicleSubPosition.Starboard}", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Starboard)
        { }
    }
    [HeliosControl("Helios.DH98Mosquito.RPMGauge", "RPM Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class RPMGauge : CompositeBaseGauge 
    {

        private static GaugeNeedle _rpmThousandsNeedle;
        private static GaugeNeedle _rpmHundredsNeedle;
        private static HeliosValue _rpmThousands;
        private static HeliosValue _rpmHundreds;

        private static string[] _elements;
        private static VehicleSubPosition _side;

        private static CalibrationPointCollectionDouble _rpmHundredsCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d);
        private static CalibrationPointCollectionDouble _rpmThousandsCalibration = new CalibrationPointCollectionDouble(0d, 0d, 5d, 180d);

        public RPMGauge() : this("RPM Gauge", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port) { }
        public RPMGauge(string name, Size size, string device, VehicleSubPosition side) : this(name, size, device, side, new string[] { "1000's Needle", "100's Needle" }) { }
        public RPMGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
        SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };
            _elements = elements;
            _side = side;

            CreateInputBindings();

            double scalingFactor = 0.5d;
            Point center = new Point(size.Width / 2, size.Height / 2);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/RPM/RPM-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _rpmThousandsNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/RPM/RPM-Small-Needle.xaml", center, new Size(55.817d * scalingFactor, 189.885d * scalingFactor), new Point(28.4090003967285d * scalingFactor, 118.467002868652d * scalingFactor));
            Components.Add(_rpmThousandsNeedle);

            _rpmHundredsNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/RPM/RPM-Large-Needle.xaml", center, new Size(26d * scalingFactor, 311.337d * scalingFactor), new Point(13.5d * scalingFactor, 209.893005371094d * scalingFactor));
            Components.Add(_rpmHundredsNeedle);

            _rpmThousands = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{_elements[0]} value for RPM Gauge {_side}.", "(0 - 1)", BindingValueUnits.RPM(5000) );
            _rpmThousands.Execute += Thousands_Execute;
            Actions.Add(_rpmThousands);

            _rpmHundreds = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{_elements[1]} value for RPM Gauge {_side}.", "(0 - 1)", BindingValueUnits.RPM(1000) );
            _rpmHundreds.Execute += Hundreds_Execute;
            Actions.Add(_rpmHundreds);
        }
        void CreateInputBindings()
        {

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Engine Instruments.Tacho ({_side}) 100's.changed", $"Engine Instruments_RPM Gauge {_side}.set.100's Needle" },
                { $"Engine Instruments.Tacho ({_side}) 1000's.changed", $"Engine Instruments_RPM Gauge {_side}.set.1000's Needle" }
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
        void Thousands_Execute(object action, HeliosActionEventArgs e)
        {
            _rpmThousands.SetValue(e.Value, e.BypassCascadingTriggers);
            _rpmThousandsNeedle.Rotation = _rpmThousandsCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Hundreds_Execute(object action, HeliosActionEventArgs e)
        {
            _rpmHundreds.SetValue(e.Value, e.BypassCascadingTriggers);
            _rpmHundredsNeedle.Rotation = _rpmHundredsCalibration.Interpolate(e.Value.DoubleValue);
        }
    }

}