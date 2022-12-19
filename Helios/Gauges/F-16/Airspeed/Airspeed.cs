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

namespace GadrocsWorkshop.Helios.Gauges.F_16.Airspeed
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F16.Airspeed", "Airspeed", "F-16", typeof(GaugeRenderer))]
    public class Airspeed : BaseGauge
    {
        private HeliosValue _airspeed;
        private HeliosValue _mach;

        private GaugeNeedle _machRing;
        private GaugeNeedle _needle;

        private CalibrationPointCollectionDouble _needleCalibration;
        private CalibrationPointCollectionDouble _machCalibration;

        public Airspeed()
            : base("Airspeed", new Size(364, 376))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/F-16/Airspeed/asi_faceplate.xaml", new Rect(32d, 38d, 300d, 300d)));

            _machCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1.9d, 270d);
            _machCalibration.Add(new CalibrationPointDouble(0.5d, 60d));

            _machRing = new GaugeNeedle("{Helios}/Gauges/F-16/Airspeed/asi_inner_faceplate.xaml", new Point(182d, 188d), new Size(188d, 188d), new Point(94d, 94d), -90d);
            Components.Add(_machRing);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 850d, 350d);
            _needleCalibration.Add(new CalibrationPointDouble(200d, 135d));
            _needleCalibration.Add(new CalibrationPointDouble(300d, 195d));
            _needleCalibration.Add(new CalibrationPointDouble(400d, 235d));
            _needleCalibration.Add(new CalibrationPointDouble(500d, 267d));

            _needle = new GaugeNeedle("{Helios}/Gauges/F-16/Airspeed/asi_needle.xaml", new Point(182d, 188d), new Size(300d, 300d), new Point(150d, 150d), -90d);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Helios}/Gauges/F-16/Common/f16_gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _airspeed = new HeliosValue(this, new BindingValue(0d), "", "indicated air speed", "Current airspeed of the aircraft.", "", BindingValueUnits.Knots);
            _airspeed.Execute += new HeliosActionHandler(Airspeed_Execute);
            Actions.Add(_airspeed);

            _mach = new HeliosValue(this, new BindingValue(0d), "", "mach", "Current airspeed of the aircraft.", "", BindingValueUnits.Numeric);
            _mach.Execute += new HeliosActionHandler(Mach_Execute);
            Actions.Add(_mach);
        }

        void Airspeed_Execute(object action, HeliosActionEventArgs e)
        {
            _airspeed.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _needleCalibration.Interpolate(_airspeed.Value.DoubleValue);
            _machRing.Rotation = _needle.Rotation - _machCalibration.Interpolate(_mach.Value.DoubleValue);
        }

        void Mach_Execute(object action, HeliosActionEventArgs e)
        {
            _mach.SetValue(e.Value, e.BypassCascadingTriggers);
            _machRing.Rotation = _needle.Rotation - _machCalibration.Interpolate(_mach.Value.DoubleValue);
        }
    }
}
