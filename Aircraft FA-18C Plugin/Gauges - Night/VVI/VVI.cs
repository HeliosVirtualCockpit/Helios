//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Gauges.A_10.VVI;
    using System;
    using System.Windows;

    [HeliosControl("Helios.FA18C.VVI2", "Vertical Velocity Indicator - Night", "F/A-18C Gauges - Night", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class VVI2 : BaseGauge
    {
        private HeliosValue _verticalVelocity;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _calibrationPoints;

        public VVI2() : base("VVI", new Size(364, 376))
        {
            Components.Add(new GaugeImage("{FA-18C}/Gauges/VVI/N_VVI_Faceplate.png", new Rect(32d, 38d, 300d, 300d)));
            _needle = new GaugeNeedle("{FA-18C}/Gauges/Common/N_needle_a.xaml", new Point(182d, 188d), new Size(22, 165), new Point(11, 130), -90d);
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));
            _verticalVelocity = new HeliosValue(this, new BindingValue(0d), "", "vertical velocity", "Veritcal velocity of the aircraft", "(-6,000 to 6,000)", BindingValueUnits.FeetPerMinute);
            _verticalVelocity.Execute += new HeliosActionHandler(VerticalVelocity_Execute);
            Actions.Add(_verticalVelocity);

            _calibrationPoints = new CalibrationPointCollectionDouble(-6000d, -169d, 6000d, 169d)
            {
                new CalibrationPointDouble(-4000d, -140d),
                new CalibrationPointDouble(-2000d, -102d),
                new CalibrationPointDouble(-1000d, -67d),
                new CalibrationPointDouble(-500d, -37d),
                new CalibrationPointDouble(0d, 0d),
                new CalibrationPointDouble(500d, 37d),
                new CalibrationPointDouble(1000d, 67d),
                new CalibrationPointDouble(2000d, 102d),
                new CalibrationPointDouble(4000d, 140d)
            };
        }
        void VerticalVelocity_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
