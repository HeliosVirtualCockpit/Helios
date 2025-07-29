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

    [HeliosControl("Helios.DH98Mosquito.AirspeedGauge", "Airspeed Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class AirspeedGauge : CompositeBaseGauge
    {
        private static GaugeNeedle _gaugeNeedle;
        private static HeliosValue _gauge;

        private static VehicleSubPosition _side;

        private static CalibrationPointCollectionDouble _scale;

    public AirspeedGauge() : this("Airspeed Gauge", new Size(300, 300), "Flight Instruments", VehicleSubPosition.Port, new string[] { "Airspeed" }) { }
    public AirspeedGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
            _scale = new CalibrationPointCollectionDouble(0d, 0d, 500d, 650d) {
                new CalibrationPointDouble(60, 24.5),
                new CalibrationPointDouble(80, 34.5),
                new CalibrationPointDouble(100, 55.5),
                new CalibrationPointDouble(120, 77),
                new CalibrationPointDouble(140, 104),
                new CalibrationPointDouble(160, 135),
                new CalibrationPointDouble(180, 170),
                new CalibrationPointDouble(200, 206),
                new CalibrationPointDouble(220, 244),
                new CalibrationPointDouble(240, 279),
                new CalibrationPointDouble(260, 309.5),
                new CalibrationPointDouble(280, 339),
                new CalibrationPointDouble(300, 369),
                new CalibrationPointDouble(320, 395),
                new CalibrationPointDouble(340, 423),
                new CalibrationPointDouble(360, 448),
                new CalibrationPointDouble(380, 478),
                new CalibrationPointDouble(400, 505),
                new CalibrationPointDouble(420, 537),
                new CalibrationPointDouble(440, 565),
                new CalibrationPointDouble(460, 593),
                new CalibrationPointDouble(480, 617)
            };

            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };
            _side = side;

            CreateInputBindings();

            double scalingFactor = 1.0d;
            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{DH98Mosquito}/Gauges/Airspeed/Airspeed-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _gaugeNeedle = new GaugeNeedle("{DH98Mosquito}/Gauges/Airspeed/Airspeed-Needle.xaml", center, new Size(14.914d * scalingFactor, 208.979d * scalingFactor), new Point(7.239d * scalingFactor, 76.040d * scalingFactor),0d);
            Components.Add(_gaugeNeedle);

            _gauge = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]}", $"{elements[0]}", $"{_scale.MinimumInputValue} to {_scale.MaximumInputValue}", BindingValueUnits.Knots);
            _gauge.Execute += Gauge_Execute;
            Actions.Add(_gauge);

        }
        void CreateInputBindings()
        {
 
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { $"Flight Instruments.Airspeed.changed", $"Flight Instruments_Airspeed Gauge.set.Airspeed" }
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