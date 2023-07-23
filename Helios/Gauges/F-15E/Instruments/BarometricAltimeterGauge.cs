//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Instruments.BarometricAltimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.Instruments.BAltimeter", "Altimeter", "F-15E Strike Eagle", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BAltimeterGauge : BaseGauge
    {
        private HeliosValue _altitude;
        private HeliosValue _airPressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _drum;
        private GaugeDrumCounter _airPressureDrum;

        public BAltimeterGauge(string name, Size size, string device)
            : base(name, size)
        {
            _tensDrum = new GaugeDrumCounter("{Helios}/Gauges/FA-18C/Altimeter/alt_drum_tape.xaml", new Point(55d, 89d), "#", new Size(10d, 15d), new Size(30d, 40d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(55d, 89d, 30d, 40d));
            Components.Add(_tensDrum);

            _drum = new GaugeDrumCounter("{Helios}/Gauges/FA-18C/Common/drum_tape.xaml", new Point(85d, 89d), "#%00", new Size(10d, 15d), new Size(30d, 40d));
            _drum.Clip = new RectangleGeometry(new Rect(85d, 89d, 60d, 40d));
            Components.Add(_drum);

            _airPressureDrum = new GaugeDrumCounter("{Helios}/Gauges/FA-18C/Common/drum_tape.xaml", new Point(108d, 221d), "###%", new Size(10d, 15d), new Size(20d, 30d));
            _airPressureDrum.Value = 2992d;
            _airPressureDrum.Clip = new RectangleGeometry(new Rect(108d, 221d, 84d, 30d));
            Components.Add(_airPressureDrum);

            Components.Add(new GaugeImage("{Helios}/Gauges/F-15E/Instruments/BarometricAltimeterDial.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _needle = new GaugeNeedle("{Helios}/Gauges/F-15E/Instruments/NeedleC.xaml", new Point(150d, 150d), new Size(30d, 200d), new Point(15d, 128.9d));
            Components.Add(_needle);

            _altitude = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Altitude", "Current altitude of the aircraft in feet.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);

            _airPressure = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Air Pressure", "Current air pressure calibaration setting for the altimeter.", "", BindingValueUnits.InchesOfMercury);
            _airPressure.SetValue(new BindingValue(29.92d), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);
        }

        void Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue % 1000d);
            _tensDrum.Value = e.Value.DoubleValue / 10000d;

            // Setup the thousands drum to roll with the rest
            double tenThousands = (e.Value.DoubleValue / 100d) % 100d;
            if (tenThousands >= 99)
            {
                _tensDrum.StartRoll = tenThousands % 1d;
            }
            else
            {
                _tensDrum.StartRoll = -1d;
            }
            double thousands = (e.Value.DoubleValue % 10000d);
            _drum.Value = thousands;
        }

        void AirPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _airPressureDrum.Value = e.Value.DoubleValue * 100d;
        }
    }
}
