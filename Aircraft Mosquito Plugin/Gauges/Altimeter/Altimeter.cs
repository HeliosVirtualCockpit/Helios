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

    [HeliosControl("Helios.DH98Mosquito.Altimeter", "Altimeter", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class Altimeter : CompositeBaseGauge
    {
        private static GaugeNeedle _hundredsNeedle;
        private static GaugeNeedle _thousandsNeedle;
        private static GaugeNeedle _tenThousandsNeedle;
        private static GaugeNeedle _pressureScaleNeedle;
        private static HeliosValue _hundreds;
        private static HeliosValue _thousands;
        private static HeliosValue _tenThousands;
        private static HeliosValue _pressureScale;

        private static CalibrationPointCollectionDouble _scale;

    public Altimeter() : this("Altimeter", new Size(300, 300), "Flight Instruments", new string[] { "Altimeter 100's", "Altimeter 1000's", "Altimeter 10000's", "Air Pressure" }) { }
    public Altimeter(string name, Size size, string device, string[] elements)
            : base(name, size)
        {
            _scale = new CalibrationPointCollectionDouble(0d, 800d, 1d, 1050d);

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };
            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);
            _pressureScaleNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Altimeter/Altimeter-Pressure-Drum.xaml", center, new Size(233.947d * scalingFactor, 234.235d * scalingFactor), new Point(116.641d * scalingFactor, 117.118d * scalingFactor), 13.85d);
            Components.Add(_pressureScaleNeedle);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Altimeter/Altimeter-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _thousandsNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Altimeter/Altimeter-Needle-Fat.xaml", center, new Size(34.625d * scalingFactor, 117.802d * scalingFactor), new Point(16.852d * scalingFactor, 72.201d * scalingFactor), 0d);
            Components.Add(_thousandsNeedle);
            _tenThousandsNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Altimeter/Altimeter-Needle-Small.xaml", center, new Size(13.222d * scalingFactor, 60.988d * scalingFactor), new Point(6.111d * scalingFactor, 54.377d * scalingFactor), 0d);
            Components.Add(_tenThousandsNeedle);
            _hundredsNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Altimeter/Altimeter-Needle-Long.xaml", center, new Size(17.667d * scalingFactor, 194.868d * scalingFactor), new Point(8.333 * scalingFactor, 132.034d * scalingFactor), 0d);
            Components.Add(_hundredsNeedle);

            _hundreds = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}.", "(0 - 1)", BindingValueUnits.Numeric);
            _hundreds.Execute += Hundreds_Execute;
            Actions.Add(_hundreds);
            _thousands = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]}.", "(0 - 1)", BindingValueUnits.Numeric);
            _thousands.Execute += Thousands_Execute;
            Actions.Add(_thousands);
            _tenThousands = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[2], $"{elements[2]}.", "(0 - 1)", BindingValueUnits.Numeric);
            _tenThousands.Execute += TenThousands_Execute;
            Actions.Add(_tenThousands);
            _pressureScale = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[3], $"{elements[3]}.", "(800 - 1050)", BindingValueUnits.Millibar);
            _pressureScale.Execute += PressureScale_Execute;
            Actions.Add(_pressureScale);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Flight Instruments.Altimeter 100's (Unscaled).changed", $"Flight Instruments_Altimeter.set.Altimeter 100's" },
                { $"Flight Instruments.Altimeter 1000's (Unscaled).changed", $"Flight Instruments_Altimeter.set.Altimeter 1000's" },
                { $"Flight Instruments.Altimeter 10000's (Unscaled).changed", $"Flight Instruments_Altimeter.set.Altimeter 10000's" },
                { $"Flight Instruments.Air Pressure (Unscaled).changed", $"Flight Instruments_Altimeter.set.Air Pressure" }
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
        void Hundreds_Execute(object action, HeliosActionEventArgs e)
        {
            _hundreds.SetValue(e.Value, e.BypassCascadingTriggers);
            //_hundredsNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 360d;
            _hundredsNeedle.Rotation = e.Value.DoubleValue * 360d;
        }
        void Thousands_Execute(object action, HeliosActionEventArgs e)
        {
            _thousands.SetValue(e.Value, e.BypassCascadingTriggers);
            //_thousandsNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 360d;
            _thousandsNeedle.Rotation = e.Value.DoubleValue * 360d;
        }
        void TenThousands_Execute(object action, HeliosActionEventArgs e)
        {
            _tenThousands.SetValue(e.Value, e.BypassCascadingTriggers);
            //_tenThousandsNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 360d;
            _tenThousandsNeedle.Rotation = e.Value.DoubleValue * 360d;
        }
        void PressureScale_Execute(object action, HeliosActionEventArgs e)
        {
            _pressureScale.SetValue(e.Value, e.BypassCascadingTriggers);
            //_pressureScaleNeedle.Rotation = _scale.Interpolate(e.Value.DoubleValue) * 360d;
            _pressureScaleNeedle.Rotation = e.Value.DoubleValue * 346d;
        }

    }

}