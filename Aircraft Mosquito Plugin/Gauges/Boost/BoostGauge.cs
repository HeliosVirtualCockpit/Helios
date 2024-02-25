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

namespace GadrocsWorkshop.Helios.Gauges.Mosquito.Instrumwents
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    //using System.Drawing;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Linq;

    [HeliosControl("Helios.Mosquito.BoostGaugePort", "Boost Gauge Port", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class BoostGaugePort : BoostGauge
    {
        public BoostGaugePort()
            : base($"Boost Gauge {VehicleSubPosition.Port}", new Size(300, 300), "Instruments", VehicleSubPosition.Port, new string[] { "1" })
        { }
    }

    [HeliosControl("Helios.Mosquito.BoostGaugeStarboard", "Boost Gauge Starboard", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class BoostGaugeStarboard : BoostGauge
    {
        public BoostGaugeStarboard()
            : base($"Boost Gauge {VehicleSubPosition.Starboard}", new Size(300, 300), "Instruments", VehicleSubPosition.Starboard, new string[] { "1" })
        { }
    }


    [HeliosControl("Helios.Mosquito.BoostGauge", "Boost Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BoostGauge : BaseGauge
    {
        private GaugeNeedle _boostNeedle;
        private HeliosValue _boost;

        private CalibrationPointCollectionDouble _boostCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d);

        public BoostGauge() : this("Boost Gauge", new Size(300, 300), "Instruments", VehicleSubPosition.Port, new string[] {"Needle" }) { }
        public BoostGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
            double scalingFactor = 0.5d;
            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{Mosquito}/Gauges/Boost/Boost-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _boostNeedle = new GaugeNeedle("{Mosquito}/Gauges/Boost/Boost-Needle.xaml", center, new Size(52.209d * scalingFactor, 300.962d * scalingFactor), new Point(26d * scalingFactor, 212d * scalingFactor),270d);
            Components.Add(_boostNeedle);

            _boost = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} Boost Pressure {side}.", "(0 - 1)", BindingValueUnits.Numeric);
            _boost.Execute += Boost_Execute;
            Actions.Add(_boost);

        }

        void Boost_Execute(object action, HeliosActionEventArgs e)
        {
            _boost.SetValue(e.Value, e.BypassCascadingTriggers);
            _boostNeedle.Rotation = _boostCalibration.Interpolate(e.Value.DoubleValue);
        }
    }

}