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
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Linq;

    [HeliosControl("Helios.DH98Mosquito.BoostGaugePort", "Boost Gauge Port", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BoostGaugePort : BoostGauge
    {
        public BoostGaugePort()
            : base($"Boost Gauge {VehicleSubPosition.Port}", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port, new string[] { "Boost Pressure" })
        { }
    }

    [HeliosControl("Helios.DH98Mosquito.BoostGaugeStarboard", "Boost Gauge Starboard", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BoostGaugeStarboard : BoostGauge
    {
        public BoostGaugeStarboard()
            : base($"Boost Gauge {VehicleSubPosition.Starboard}", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Starboard, new string[] { "Boost Pressure" })
        { }
    }


    [HeliosControl("Helios.DH98Mosquito.BoostGauge", "Boost Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BoostGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _boostNeedle;
        private static HeliosValue _boost;

        private static VehicleSubPosition _side;

        private static CalibrationPointCollectionDouble _scale;

    public BoostGauge() : this("Boost Gauge", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port, new string[] {"Needle" }) { }
    public BoostGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
            double[] input = new double[] { -7.0, -6.0, 0.0, 6.0, 10.0, 14.0, 16.0, 18.0, 20.0, 22.0, 24.0, 25.0 };
            double[] output = new double[] { 0.000, 0.035, 0.280, 0.503, 0.629, 0.723, 0.778, 0.834, 0.887, 0.929, 0.980, 1.000 };
            _scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
            for (int ii = 1; ii < input.Count() - 2; ii++) _scale.Add(new CalibrationPointDouble(input[ii], output[ii]));

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };
            _side = side;

            CreateInputBindings();

            double scalingFactor = 0.5d;
            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Boost/Boost-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _boostNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Boost/Boost-Needle.xaml", center, new Size(52.209d * scalingFactor, 300.962d * scalingFactor), new Point(26d * scalingFactor, 212d * scalingFactor),270d);
            Components.Add(_boostNeedle);

            _boost = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} {_side}.", "(0 - 1)", BindingValueUnits.PoundsPerSquareInch);
            _boost.Execute += Boost_Execute;
            Actions.Add(_boost);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Engine Instruments.Boost Gauge ({_side}).changed", $"Engine Instruments_Boost Gauge {_side}.set.Boost Pressure" }
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
        void Boost_Execute(object action, HeliosActionEventArgs e)
        {
            _boost.SetValue(e.Value, e.BypassCascadingTriggers);
            _boostNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 360d;
        }
    }

}