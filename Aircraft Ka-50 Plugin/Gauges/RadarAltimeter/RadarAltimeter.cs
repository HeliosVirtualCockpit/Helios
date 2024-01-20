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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.RadarAltimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.RadarAltimeter", "Radar Altimeter", "KA-50 Gauges", typeof(GaugeRenderer))]
    public class RadarAltimeter : BaseGauge
    {
        private GaugeNeedle _needle;
        private GaugeNeedle _safeNeedle;

        private GaugeImage _flagImage;

        private CalibrationPointCollectionDouble _needleCalibration;

        private HeliosValue _altitude;
        private HeliosValue _safeAltitude;
        private HeliosValue _flag;

        public RadarAltimeter()
            : base("Radar Altimeter", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 350d, 330d);
            _needleCalibration.Add(new CalibrationPointDouble(20d, 66d));
            _needleCalibration.Add(new CalibrationPointDouble(30d, 115d));
            _needleCalibration.Add(new CalibrationPointDouble(50d, 165d));
            _needleCalibration.Add(new CalibrationPointDouble(150d, 271d));
            _needleCalibration.Add(new CalibrationPointDouble(200, 300d));

            Components.Add(new GaugeImage("{Ka-50}/Gauges/RadarAltimeter/radar_alt_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _needle = new GaugeNeedle("{Ka-50}/Gauges/RadarAltimeter/radar_alt_needle.xaml", center, new Size(32, 143), new Point(16, 127));
            Components.Add(_needle);

            _safeNeedle = new GaugeNeedle("{Ka-50}/Gauges/RadarAltimeter/radar_alt_safe_needle.xaml", center, new Size(13, 16), new Point(6.5, 151));
            Components.Add(_safeNeedle);

            _flagImage = new GaugeImage("{Ka-50}/Gauges/RadarAltimeter/radar_alt_flag.xaml", new Rect(280, 50, 32, 89));
            Components.Add(_flagImage);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/RadarAltimeter/radar_alt_bezel.xaml", new Rect(0, 0, 340, 340)));

            _altitude = new HeliosValue(this, BindingValue.Empty, "", "Altitude", "Current altitude AGL", "", BindingValueUnits.Meters);
            _altitude.Execute += Altitude_Execute;
            Actions.Add(_altitude);

            _safeAltitude = new HeliosValue(this, BindingValue.Empty, "", "Safe Altitude", "Current safe altitude AGL", "", BindingValueUnits.Meters);
            _safeAltitude.Execute += SafeAltitude_Execute;
            Actions.Add(_safeAltitude);

            _flag = new HeliosValue(this, new BindingValue(false), "", "Failure flag", "Indicates altimeter is not functional failure.", "True if displayed.", BindingValueUnits.Boolean);
            _flag.Execute += Flag_Execute;
            Actions.Add(_flag);
        }

        private void Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _altitude.SetValue(e.Value, e.BypassCascadingTriggers);
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void SafeAltitude_Execute(object action, HeliosActionEventArgs e)
        {
            _safeAltitude.SetValue(e.Value, e.BypassCascadingTriggers);
            _safeNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void Flag_Execute(object action, HeliosActionEventArgs e)
        {
            _flag.SetValue(e.Value, e.BypassCascadingTriggers);
            _flagImage.IsHidden = !e.Value.BoolValue;
        }
    }
}
