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

namespace GadrocsWorkshop.Helios.Gauges.Mosquito.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Gauges.Mosquito.Instrumwents;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.Mosquito.RPMGaugePort", "RPM Gauge Port", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class RPMGaugePort : RPMGauge
    {
        public RPMGaugePort()
            : base($"RPM Gauge {VehicleSubPosition.Port}", new Size(300, 300), "Instruments", VehicleSubPosition.Port)
        { }
    }

    [HeliosControl("Helios.Mosquito.RPMGaugeStarboard", "RPM Gauge Starboard", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class RPMGaugeStarboard : RPMGauge
    {
        public RPMGaugeStarboard()
            : base($"RPM Gauge {VehicleSubPosition.Starboard}", new Size(300, 300), "Instruments", VehicleSubPosition.Starboard)
        { }
    }
    [HeliosControl("Helios.Mosquito.RPMGauge", "RPM Gauge", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class RPMGauge : BaseGauge
    {
        private GaugeNeedle _rpmThousandsNeedle;
        private GaugeNeedle _rpmHundredsNeedle;
        private HeliosValue _rpmThousands;
        private HeliosValue _rpmHundreds;

        private CalibrationPointCollectionDouble _rpmHundredsCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d);
        private CalibrationPointCollectionDouble _rpmThousandsCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 180d);

        public RPMGauge() : this("RPM Gauge", new Size(300, 300), "Instruments", VehicleSubPosition.Port) { }
        public RPMGauge(string name, Size size, string device, VehicleSubPosition side) : this(name, size, device, side, new string[] { "1000's Needle", "100's Needle" }) { }
        public RPMGauge(string name, Size size, string device, VehicleSubPosition side,  string[] elements)
            : base(name, size)
        {
            double scalingFactor = 0.5d;
            Point center = new Point(size.Width / 2, size.Height / 2);
            Components.Add(new GaugeImage("{Mosquito}/Gauges/RPM/RPM-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _rpmThousandsNeedle = new GaugeNeedle("{Mosquito}/Gauges/RPM/RPM-Small-Needle.xaml", center, new Size(55.817d * scalingFactor, 189.885d * scalingFactor), new Point(28.4090003967285d * scalingFactor, 118.467002868652d * scalingFactor));
            Components.Add(_rpmThousandsNeedle);

            _rpmHundredsNeedle = new GaugeNeedle("{Mosquito}/Gauges/RPM/RPM-Large-Needle.xaml", center, new Size(26d * scalingFactor, 311.337d * scalingFactor), new Point(13.5d * scalingFactor, 209.893005371094d * scalingFactor));
            Components.Add(_rpmHundredsNeedle);

            _rpmThousands = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} value for RPM Gauge {side}.", "(0 - 1)", BindingValueUnits.Numeric );
            _rpmThousands.Execute += Thousands_Execute;
            Actions.Add(_rpmThousands);

            _rpmHundreds = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]} value for RPM Gauge {side}.", "(0 - 1)", BindingValueUnits.Numeric );
            _rpmHundreds.Execute += Hundreds_Execute;
            Actions.Add(_rpmHundreds);
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