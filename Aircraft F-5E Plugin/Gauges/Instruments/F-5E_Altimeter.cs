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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.Altimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F5E.Instruments.Altimeter", "Altimeter", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class Altimeter : BaseGauge
    {
        private HeliosValue _altitude;
        private HeliosValue _airPressure;
        private HeliosValue _pneuFlag;
        private GaugeNeedle _needle;
        private GaugeNeedle _pneuFlagNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _drum;
        private GaugeDrumCounter _airPressureDrum;

        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";


        public Altimeter() : this("Altimeter", new Size (300, 300), "") {}
        public Altimeter(string name, Size size, string device)
            : base(name, size)
        {
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Altimeter_Background.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _tensDrum = new GaugeDrumCounter($"{_gaugeImagePath}F-5E_Altimeter_Atlitude_Tape_10K.xaml", new Point(49d, 130d), "#", new Size(25.243d, 467d / 12d), new Size(28d, 40d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(49d, 130d, 28d, 40d));
            Components.Add(_tensDrum);

            _drum = new GaugeDrumCounter($"{_gaugeImagePath}F-5E_Altimeter_Atlitude_Tape_1K_and_Hundreds.xaml", new Point(77d, 130d), "#%00", new Size(25.243d, 467d / 12d), new Size(28d, 40d));
            _drum.Clip = new RectangleGeometry(new Rect(77d, 116d, 56d, 68d));
            Components.Add(_drum);

            _airPressureDrum = new GaugeDrumCounter($"{_gaugeImagePath}F-5E_Altimeter_Pressure_Tape.xaml", new Point(180d, 192d), "###%", new Size(13.461d, 249.022d / 12d), new Size(58d / 4d, 19d));
            _airPressureDrum.Value = 2992d;
            _airPressureDrum.Clip = new RectangleGeometry(new Rect(180d, 192d, 58d, 19d));
            Components.Add(_airPressureDrum);

            _pneuFlagNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Altimeter_Flag_Pneu.xaml", new Point(68d, 94d), new Size(45d, 20d), new Point(0,0), -90d);
            Components.Add(_pneuFlagNeedle);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Altimeter_FacePlate.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Altimeter_Needle.xaml", new Point(150d, 150d), new Size(14.135d, 236.233d), new Point(14.135d / 2d, 137d));
            Components.Add(_needle);

            _altitude = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Altitude", "Current altitude of the aircraft in feet.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);

            _airPressure = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Air Pressure", "Current air pressure calibaration setting for the altimeter.", "", BindingValueUnits.InchesOfMercury);
            _airPressure.SetValue(new BindingValue(29.92d), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);

            _pneuFlag = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "PNEU Flag", "Number representing position of the flag", "(0 to 1)", BindingValueUnits.Numeric);
            _pneuFlag.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_pneuFlag);

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

        void Flag_Execute(object action, HeliosActionEventArgs e)
        {
            _pneuFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _pneuFlagNeedle.Rotation = (e.Value.DoubleValue) * 90d;
        }
    }
}
