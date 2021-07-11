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

namespace GadrocsWorkshop.Helios.Gauges.A_10.IAS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.A10.IAS", "IAS", "A-10 Gauges", typeof(GaugeRenderer))]
    public class IAS : BaseGauge
    {
        private HeliosValue _indicatedAirSpeed;
        private HeliosValue _limitingAirSpeed;
        private GaugeNeedle _needle;
        private GaugeNeedle _tape;
        private GaugeNeedle _limitingAirSpeedNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private CalibrationPointCollectionDouble _tapeCalibration;
        private CalibrationPointCollectionDouble _limitingAirSpeedNeedleCalibration;

        public IAS()
            : base("IAS", new Size(364, 376))
        {
            _tapeCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 346d);
            _tape = new GaugeNeedle("{Helios}/Gauges/A-10/IAS/ias_tape.xaml", new Point(137, 93), new Size(436, 42), new Point(0, 0));
            _tape.HorizontalOffset = -_tapeCalibration.Interpolate(0d);
            _tape.Clip = new RectangleGeometry(new Rect(137d, 93d, 90d, 42d));
            Components.Add(_tape);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/IAS/ias_faceplate.xaml", new Rect(32d, 38d, 300, 300)));

            _limitingAirSpeedNeedleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 550d, 335d);
            _limitingAirSpeedNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/IAS/needle_ias_limit.xaml", new Point(182d, 188d), new Size(22, 165), new Point(11, 130), 10d);
            Components.Add(_limitingAirSpeedNeedle);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 550d, 340d);
            _needleCalibration.Add(new CalibrationPointDouble(100d, 34d));
            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/Common/needle_a.xaml", new Point(182d, 188d), new Size(22, 165), new Point(11, 130), 10d);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _indicatedAirSpeed = new HeliosValue(this, new BindingValue(0d), "", "indicated airspeed", "Current indicated airspeed of the aircraft.", "(0 - 550)", BindingValueUnits.Knots);
            _indicatedAirSpeed.Execute += new HeliosActionHandler(IndicatedAirSpeed_Execute);
            Actions.Add(_indicatedAirSpeed);

            _limitingAirSpeed = new HeliosValue(this, new BindingValue(0d), "", "limiting airspeed", "Current altitude compensated, limiting structural airspeed of the aircraft.", "(0 - 550)", BindingValueUnits.Knots);
            _limitingAirSpeed.Execute += new HeliosActionHandler(LimitingAirSpeed_Execute);
            Actions.Add(_limitingAirSpeed);
        }

        void IndicatedAirSpeed_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
            _tape.HorizontalOffset = -_tapeCalibration.Interpolate(e.Value.DoubleValue % 100d);
        }

        void LimitingAirSpeed_Execute(object action, HeliosActionEventArgs e)
        {
            _limitingAirSpeedNeedle.Rotation = _limitingAirSpeedNeedleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
