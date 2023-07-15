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

namespace GadrocsWorkshop.Helios.Gauges.F15E.FuelPanel
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.FuelPanel.FourDigitDisplay", "F-15E Strike Eagle", "AV-8B Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class FourDigitDisplay : BaseGauge
    {
        private HeliosValue _four_digit_display;
        private GaugeDrumCounter _thousandsDrum;
        private GaugeDrumCounter _hundredsDrum;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _onesDrum;

        public FourDigitDisplay(string interfaceElement)
            : base(interfaceElement, new Size(275, 100))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/AV-8B/Fuel Panel/4 Digit Display/digit_faceplate.xaml", new Rect(0d, 0d, 275d, 100d)));

            _thousandsDrum = new GaugeDrumCounter("{Helios}/Gauges/AV-8B/Common/drum_tape.xaml", new Point(13.5d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _thousandsDrum.Clip = new RectangleGeometry(new Rect(13.5d, 11.5d, 50d, 75d));
            Components.Add(_thousandsDrum);

            _hundredsDrum = new GaugeDrumCounter("{Helios}/Gauges/AV-8B/Common/drum_tape.xaml", new Point(79.5d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _hundredsDrum.Clip = new RectangleGeometry(new Rect(79.5d, 11.5d, 50d, 75d));
            Components.Add(_hundredsDrum);

            _tensDrum = new GaugeDrumCounter("{Helios}/Gauges/AV-8B/Common/drum_tape.xaml", new Point(145.5d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(145.5d, 11.5d, 50d, 75d));
            Components.Add(_tensDrum);

            _onesDrum = new GaugeDrumCounter("{Helios}/Gauges/AV-8B/Common/drum_tape.xaml", new Point(211.5d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _onesDrum.Clip = new RectangleGeometry(new Rect(211.5d, 11.5d, 50d, 75d));
            Components.Add(_onesDrum);

            _four_digit_display = new HeliosValue(this, new BindingValue(0d), "", "value", "Four digit display", "Numeric value to be display on a drum", BindingValueUnits.Numeric) ;
            _four_digit_display.Execute += new HeliosActionHandler(DigitDisplay_Execute);
            Actions.Add(_four_digit_display);

        }

        void DigitDisplay_Execute(object action, HeliosActionEventArgs e)
        {
            _onesDrum.Value = e.Value.DoubleValue;
            _tensDrum.Value = _onesDrum.Value / 10d;
            _hundredsDrum.Value = _tensDrum.Value / 10d;
            _thousandsDrum.Value = _hundredsDrum.Value / 10d;
        }
    }
}
