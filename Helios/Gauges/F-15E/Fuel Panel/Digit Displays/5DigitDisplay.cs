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

    [HeliosControl("Helios.F15E.FuelPanel.FiveDigitDisplay", "Five Digit Display", "F-15E Strike Eagle", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class FiveDigitDisplay : BaseGauge
    {
        private HeliosValue _five_digit_display;
        private GaugeDrumCounter _10kDrum;
        private GaugeDrumCounter _1kDrum;
        private GaugeDrumCounter _hundredsDrum;

        public FiveDigitDisplay(string interfaceElement)
            : base(interfaceElement, new Size(339, 100))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/AV-8B/Fuel Panel/5 Digit Display/digit_faceplate.xaml", new Rect(0d, 0d, 339d, 100d)));

            _10kDrum = new GaugeDrumCounter("{Helios}/Gauges/F-15E/Common/alt_units_drum_tape_reversed.xaml", new Point(13.5d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _10kDrum.Clip = new RectangleGeometry(new Rect(13.5d, 11.5d, 50d, 75d));
            Components.Add(_10kDrum);
            _10kDrum.FastRoll = true;

            _1kDrum = new GaugeDrumCounter("{Helios}/Gauges/F-15E/Common/units_drum_tape_reversed.xaml", new Point(79.5d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _1kDrum.Clip = new RectangleGeometry(new Rect(79.5d, 11.5d, 50d, 75d));
            Components.Add(_1kDrum);

            _hundredsDrum = new GaugeDrumCounter("{Helios}/Gauges/F-15E/Common/hundreds_drum_tape_reversed.xaml", new Point(145.5d, 11.5d), "$", new Size(30d, 15d), new Size(150d, 75d));
            _hundredsDrum.Clip = new RectangleGeometry(new Rect(145.5d, 11.5d, 150d, 75d));
            Components.Add(_hundredsDrum);

            _five_digit_display = new HeliosValue(this, new BindingValue(0d), "", "value", "Five digit display", "Numeric value to be display on a drum", BindingValueUnits.Numeric);
            _five_digit_display.Execute += new HeliosActionHandler(DigitDisplay_Execute);
            Actions.Add(_five_digit_display);

        }

        void DigitDisplay_Execute(object action, HeliosActionEventArgs e)
        {
            double actionValue = e.Value.DoubleValue;
            _10kDrum.Value = actionValue / 10000;
            actionValue -= Math.Truncate(actionValue / 10000d) * 10000d;
            _1kDrum.Value = actionValue / 1000d;
            actionValue -= Math.Truncate(actionValue / 1000d) * 1000d;
            _hundredsDrum.Value = actionValue / 100d;
        }
    }
}
