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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.TACAN
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M2000C.TACANDrumGauge", "TACAN Drum Gauge", "M-2000C Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class TACANDrumGauge : BaseGauge
    {
        private HeliosValue _drumValue;
        private TACANDrumGaugeCounter _drum;

        public TACANDrumGauge()
            : this("TACAN Drum Gauge", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "", "", "#", new Point(0,0), new Size(10d, 15d), new Size(12d, 19d))
        {
        }

        public TACANDrumGauge(string name, string drumWay, string actionIdentifier, string valueDescription, string format, Point posn, Size size, Size renderSize, double drumDigits = 10d)
            : base(name, new Size(renderSize.Width*format.Length,renderSize.Height))
        {
            _drum = new TACANDrumGaugeCounter(drumWay, posn, format, size, renderSize, drumDigits);
            _drum.Clip = new RectangleGeometry(new Rect(posn.X, posn.Y, renderSize.Width*format.Length, renderSize.Height));
            Components.Add(_drum);

            _drumValue = new HeliosValue(this, new BindingValue(0d), "", actionIdentifier, name + " - " + actionIdentifier, valueDescription, BindingValueUnits.Numeric);
            _drumValue.Execute += new HeliosActionHandler(DrumValue_Execute);
            Actions.Add(_drumValue);
        }

        void DrumValue_Execute(object action, HeliosActionEventArgs e)
        {
            _drum.Value = e.Value.DoubleValue * 10d;
        }
    }
}
