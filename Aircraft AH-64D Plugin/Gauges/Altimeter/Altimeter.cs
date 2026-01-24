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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.Altimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.Altimeter", "Standby Altimeter", "AH-64D", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class Altimeter : BaseGauge
    {
        private HeliosValue _altitude;
        private HeliosValue _airPressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _drum;
        private GaugeDrumCounter _airPressureDrum;

        public Altimeter()
            : base("Altimeter", new Size(364, 376))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Altimeter/altimeter_backplate.xaml", new Rect(32d, 38d, 300d, 300d)));

            _tensDrum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Altimeter/alt_drum_tape.xaml", new Point(79d, 168d), "#", new Size(13d, 15d), new Size(39d, 45d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(71d, 144d, 150d, 81d));
            Components.Add(_tensDrum);

            _drum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Common/drum_tape.xaml", new Point(110d, 168d), "%000", new Size(13d, 15d), new Size(39d, 45d));
            _drum.Clip = new RectangleGeometry(new Rect(101d, 144d, 150d, 81d));
            Components.Add(_drum);

            _airPressureDrum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Common/drum_tape.xaml", new Point(182d, 256d), "###%", new Size(10d, 15d), new Size(15d, 20d));
            _airPressureDrum.Value = 2992d;
            _airPressureDrum.Clip = new RectangleGeometry(new Rect(182d, 256d, 60d, 20d));
            Components.Add(_airPressureDrum);

            Components.Add(new GaugeImage("{AH-64D}/Images/Altimeter/altimeter_faceplate.xaml", new Rect(32d, 38d, 300d, 300d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/Altimeter/altimeter_needle.xaml", new Point(182d, 188d), new Size(16d, 257d), new Point(8d, 138.5d));
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _airPressure = new HeliosValue(this, new BindingValue(0d), "", "air pressure", "Current air pressure calibaration setting for the altimeter.", "", BindingValueUnits.InchesOfMercury);
            _airPressure.SetValue(new BindingValue(29.92), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);

            _altitude = new HeliosValue(this, new BindingValue(0d), "", "altitude", "Current altitude of the aircraft.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);
        }

        void Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue % 1000d);
            bool increasing = _drum.Value < e.Value.DoubleValue;  
            _tensDrum.Value = e.Value.DoubleValue / 10000d;
            // Setup then thousands drum to roll with the rest
            double tenThousands = (e.Value.DoubleValue / 100d) % 100d;
            if (tenThousands >= 99.1 && increasing)
            {
                _tensDrum.StartRoll = tenThousands % 1d;
            }
            else
            {
                _tensDrum.StartRoll = -1d;
            }

            _drum.Value = e.Value.DoubleValue;

        }

        void AirPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _airPressureDrum.Value = e.Value.DoubleValue * 100d;
        }
    }
}
