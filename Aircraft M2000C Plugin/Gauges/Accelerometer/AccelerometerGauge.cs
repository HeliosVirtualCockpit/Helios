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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.Accelerometer
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M2000C.AccelerometerGauge", "Accelerometer Face", "M-2000C", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class AccelerometerGauge : BaseGauge
    {
        private GaugeNeedle _gNeedle;
        private GaugeNeedle _gMinNeedle;
        private GaugeNeedle _gMaxNeedle;

        private HeliosValue _gValue;
        private HeliosValue _gMinValue;
        private HeliosValue _gMaxValue;


        private CalibrationPointCollectionDouble _gCalibration = new CalibrationPointCollectionDouble(-10d, 0d, 10d, 337.5d) { new CalibrationPointDouble (-5.0d, 0d)};

        public AccelerometerGauge(string name, Size size, string device, string[] elements)
            : base(name, size)
        {

            Point center = new Point(size.Width/2, size.Height/2);
            Components.Add(new GaugeImage("{M2000C}/Gauges/Accelerometer/Accelerometer_Dial.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            _gNeedle = new GaugeNeedle("{M2000C}/Gauges/Accelerometer/Needle1.xaml", center, new Size(36.50d, 218.352d), new Point(18.25d, 132.5d), 135d);

            _gValue = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[0], $"{elements[0]} value for Accelerometer.", "(-10 to +10)", BindingValueUnits.Numeric);
            _gValue.Execute += G_Execute;
            Actions.Add(_gValue);

            _gMinNeedle = new GaugeNeedle("{M2000C}/Gauges/Accelerometer/Needle2.xaml", center, new Size(36.5d, 184.161d), new Point(18.25d, 132.5d), 135d);

            _gMinValue = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[1], $"{elements[1]} value for Accelerometer.", "(-10 to +10)", BindingValueUnits.Numeric);
            _gMinValue.Execute += GMin_Execute;
            Actions.Add(_gMinValue);

            _gMaxNeedle = new GaugeNeedle("{M2000C}/Gauges/Accelerometer/Needle2.xaml", center, new Size(36.5d, 184.161d), new Point(18.25d, 132.5d), 135d);
            _gMaxValue = new HeliosValue(this, BindingValue.Empty, $"{device}_{name}", elements[2], $"{elements[2]} value for Accelerometer.", "(-10 to +10)", BindingValueUnits.Numeric);
            _gMaxValue.Execute += GMax_Execute;
            Actions.Add(_gMaxValue);

            Components.Add(_gMinNeedle);
            Components.Add(_gMaxNeedle);
            Components.Add(_gNeedle);
        }

        void G_Execute(object action, HeliosActionEventArgs e)
        {
            _gValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _gNeedle.Rotation = _gCalibration.Interpolate(e.Value.DoubleValue);
        }

        void GMin_Execute(object action, HeliosActionEventArgs e)
        {
            _gMinValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _gMinNeedle.Rotation = _gCalibration.Interpolate(e.Value.DoubleValue);
        }

        void GMax_Execute(object action, HeliosActionEventArgs e)
        {
            _gMaxValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _gMaxNeedle.Rotation = _gCalibration.Interpolate(e.Value.DoubleValue);
        }


    }

}