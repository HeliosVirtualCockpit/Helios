//  Copyright 2023 Helios Virtual Cockpit Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.Instruments.VVI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.M2000C.Instruments.VVI.Gauge", "Vertical Velocity Indicator", "M-2000C", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class VVIGauge : BaseGauge
    {
        private readonly HeliosValue _verticalVelocity;
        private readonly GaugeNeedle _needle;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;

        public VVIGauge(string name, Size size, string device = "Flight Instruments"): base(name, size)
        {
            Components.Add(new GaugeImage("{M2000C}/Gauges/VVI/VVI_Dial.xaml", new Rect(0d, 0d, 300d, 300d)));

            _needle = new GaugeNeedle("{M2000C}/Gauges/VVI/VVI_Needle.xaml", new Point(Width/2d, Height/2d), new Size(175, 175), new Point(87.5, 87.5), 0d);
            Components.Add(_needle);

            _verticalVelocity = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", name, "Veritcal velocity of the aircraft", "(-6,000 to 6,000)", BindingValueUnits.FeetPerMinute);
            _verticalVelocity.Execute += new HeliosActionHandler(VerticalVelocity_Execute);
            Actions.Add(_verticalVelocity);
            _calibrationPoints = new CalibrationPointCollectionDouble(-6000d, -169d, 6000d, 169d)
            {
                new CalibrationPointDouble(-5000d, -158d),
                new CalibrationPointDouble(-4000d, -141d),
                new CalibrationPointDouble(-3000d, -115d),
                new CalibrationPointDouble(-2000d, -90d),
                new CalibrationPointDouble(-1500d, -75d),
                new CalibrationPointDouble(-1000d, -52d),
                new CalibrationPointDouble(-500d, -30d),
                new CalibrationPointDouble(0d, 0d),
                new CalibrationPointDouble(500d, 30d),
                new CalibrationPointDouble(1000d, 52d),
                new CalibrationPointDouble(1500d, 75d),
                new CalibrationPointDouble(2000d, 90d),
                new CalibrationPointDouble(3000d, 115d),
                new CalibrationPointDouble(4000d, 141d),
                new CalibrationPointDouble(5000d, 158d)
            };
        }
        void VerticalVelocity_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
