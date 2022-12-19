//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.Altimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M-2000C.Altimeter", "Altimeter", "M-2000C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class Altimeter : BaseGauge
    {
        private HeliosValue _altitdue;
        private HeliosValue _airPressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _tenThousandsDrum;
        private GaugeDrumCounter _hundredsDrum;
        private GaugeDrumCounter _thousandDrum;
        private GaugeDrumCounter _airPressureDrum;

        public Altimeter()
            : base("Altimeter", new Size(364, 376))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/M2000C/Altimeter/altimeter_backplate.xaml", new Rect(32d, 38d, 300d, 300d)));

            _tenThousandsDrum = new GaugeDrumCounter("{Helios}/Gauges/M2000C/Altimeter/alt_drum_tape.xaml", new Point(98d, 126d), "#", new Size(13d, 15d), new Size(30d, 44d));
            _tenThousandsDrum.Clip = new RectangleGeometry(new Rect(98d, 126d, 30d, 44d));
            Components.Add(_tenThousandsDrum);

            _hundredsDrum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Common/drum_tape.xaml", new Point(182d, 126d), "#", new Size(10d, 15d), new Size(28d, 44d));
            _hundredsDrum.Clip = new RectangleGeometry(new Rect(182d, 126d, 28d, 60d));
            Components.Add(_hundredsDrum);

            _thousandDrum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Common/drum_tape.xaml", new Point(134d, 126d), "#", new Size(10d, 15d), new Size(42d, 60d));
            _thousandDrum.Clip = new RectangleGeometry(new Rect(134d, 126d, 42d, 84d));
            Components.Add(_thousandDrum);

            _airPressureDrum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Common/drum_tape.xaml", new Point(150d, 268d), "###%", new Size(10d, 15d), new Size(16d, 20d));
            _airPressureDrum.Value = 1013d;
            _airPressureDrum.Clip = new RectangleGeometry(new Rect(150d, 268d, 70d, 24d));
            Components.Add(_airPressureDrum);

            Components.Add(new GaugeImage("{Helios}/Gauges/M2000C/Altimeter/altimeter_faceplate.xaml", new Rect(32d, 38d, 300d, 300d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _needleCalibration.Add(new CalibrationPointDouble(-1000d, -360d));

            _needle = new GaugeNeedle("{Helios}/Gauges/M2000C/Altimeter/altimeter_needle.xaml", new Point(182d, 188d), new Size(14d, 178d), new Point(7d, 115d));
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _airPressure = new HeliosValue(this, new BindingValue(0d), "", "air pressure", "Current air pressure calibaration setting for the altimeter.", "", BindingValueUnits.Millibar);
            _airPressure.SetValue(new BindingValue(1013), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);

            _altitdue = new HeliosValue(this, new BindingValue(0d), "", "altitude", "Current altitude of the aircraft.", "", BindingValueUnits.Feet);
            _altitdue.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitdue);
        }

        void Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue % 1000d);
            double clampedAltitude = e.Value.DoubleValue < 0 ? 0d : e.Value.DoubleValue;
            _tenThousandsDrum.Value = clampedAltitude / 10000d;

            // Setup then thousands drum to roll with the rest
            double thousands = (clampedAltitude / 100d) % 100d;
            if (thousands >= 99)
            {
                _tenThousandsDrum.StartRoll = thousands % 1d;
            }
            else
            {
                _tenThousandsDrum.StartRoll = -1d;
            }
            double lowestTwo = (clampedAltitude / 100d) % 100d;
            _hundredsDrum.Value = lowestTwo - lowestTwo % 2;
            _thousandDrum.Value = (clampedAltitude / 1000d) % 1000d;
        }

        void AirPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _airPressureDrum.Value = e.Value.DoubleValue * 100d;
        }
    }
}
