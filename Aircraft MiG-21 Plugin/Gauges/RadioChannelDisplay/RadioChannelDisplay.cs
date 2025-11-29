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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.ChannelDisplay
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.MiG-21.ChannelDisplay", "Radio/RSBN Land & Nav Channel Displays", "MiG-21", typeof(GaugeRenderer))]
    public class ChannelDisplay : BaseGauge
    {
        private HeliosValue _val;
        private GaugeDrumCounter _drum;

        public ChannelDisplay()
            : base("Channel Display", new Size(100, 75))
        {
            _drum = new GaugeDrumCounter("{Helios}/Gauges/A-10/Common/drum_tape.xaml", new Point(0d, 0d), "##", new Size(10d, 15d), new Size(50d, 75d));
            _drum.Clip = new RectangleGeometry(new Rect(0d, 0d, 100d, 75d));
            Components.Add(_drum);

            _val = new HeliosValue(this, new BindingValue(0d), "", "Channel Display for Radio, RSBN Nav or RSBN Land", "Use Correct Channel Display as input", "Current Channel", BindingValueUnits.Numeric);
            _val.Execute += new HeliosActionHandler(Drum_Execute);
            Actions.Add(_val);
        }

        void Drum_Execute(object action, HeliosActionEventArgs e)
        {
            _drum.Value = (e.Value.DoubleValue);
        }
    }
}
