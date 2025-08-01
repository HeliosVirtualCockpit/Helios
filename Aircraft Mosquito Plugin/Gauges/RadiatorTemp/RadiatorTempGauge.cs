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

    [HeliosControl("Helios.DH98Mosquito.RadiatorTempGaugePort", "Radiator Temperature Gauge Port", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class RadiatorTempGaugePort : RadiatorTempGauge
    {
        public RadiatorTempGaugePort()
            : base($"Radiator Temperature ({VehicleSubPosition.Port})", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port, new string[] { "Temperature" })
        { }
    }

    [HeliosControl("Helios.DH98Mosquito.RadiatorTempGaugeStarboard", "Radiator Temperature Gauge Starboard", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class RadiatorTempGaugeStarboard : RadiatorTempGauge
    {
        public RadiatorTempGaugeStarboard()
            : base($"Radiator Temperature ({VehicleSubPosition.Starboard})", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Starboard, new string[] { "Temperature" })
        { }
    }


    [HeliosControl("Helios.DH98Mosquito.RadiatorTempGauge", "Radiator Temperature Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class RadiatorTempGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeNeedle;
        private static HeliosValue _gauge;

        private static VehicleSubPosition _side;

        private static CalibrationPointCollectionDouble _scale;

    public RadiatorTempGauge() : this("Radiator Temp Gauge", new Size(300, 300), "Engine Instruments", VehicleSubPosition.Port, new string[] {"Needle" }) { }
    public RadiatorTempGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
            //double[] input = new double[] { 39.0, 40.0, 60, 80, 90, 100, 110, 120, 140.0 };
            //double[] output = new double[] { -1.0, 0.0, 0.08, 0.2, 0.29, 0.39, 0.5, 0.64, 1.0 };
            double[] input = new double[] { 40.0, 60, 80, 90, 100, 110, 120, 140.0 };
            double[] output = new double[] { 0.0, 0.08, 0.2, 0.29, 0.39, 0.5, 0.64, 1.0 };

            _scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
            for (int ii = 1; ii < input.Count() - 2; ii++) _scale.Add(new CalibrationPointDouble(input[ii], output[ii]));

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };
            _side = side;

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/RadiatorTemp/Rad-Temp-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _gaugeNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/RadiatorTemp/Rad-Temp-Needle.xaml", center, new Size(31.825d * scalingFactor, 178.491d * scalingFactor), new Point(15.913d * scalingFactor, 130.041d * scalingFactor),208d);
            Components.Add(_gaugeNeedle);

            _gauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} {_side}.", $"({output[0]} - {output[output.Count() - 1]}", BindingValueUnits.Numeric);
            _gauge.Execute += Gauge_Execute;
            Actions.Add(_gauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Engine Instruments.Radiator Temperature ({_side}) (Unscaled).changed", $"Engine Instruments_Radiator Temperature ({_side}).set.Temperature" }
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
            //_gaugeNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 315d;
            _gaugeNeedle.Rotation = e.Value.DoubleValue * 315d;
        }
    }
}