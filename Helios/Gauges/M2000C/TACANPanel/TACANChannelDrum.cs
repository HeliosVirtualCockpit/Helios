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

    [HeliosControl("Helios.M2000C.TacanChannelDrum", "Mk2C Drum Tacan Channel", "M2000C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class TACANChannelDrum : BaseGauge
    {
        private HeliosValue _xyMode;
        private GaugeImage _xModeImage;
        private GaugeImage _yModeImage;

        public TACANChannelDrum()
            : this("Mk2C Drum Tacan Channel", new Point(0,0), new Size(10d, 15d))
        {
        }

        public TACANChannelDrum(string name, Point posn, Size size)
            : base(name, size)
        {
            _xModeImage = new GaugeImage("{Helios}/Gauges/M2000C/TACANPanel/tacan_channel_x_mode.xaml", new Rect(posn.X, posn.Y, size.Width, size.Height));
            Components.Add(_xModeImage);

            _yModeImage = new GaugeImage("{Helios}/Gauges/M2000C/TACANPanel/tacan_channel_y_mode.xaml", new Rect(posn.X, posn.Y, size.Width, size.Height));
            _yModeImage.IsHidden = true;
            Components.Add(_yModeImage);

            _xyMode = new HeliosValue(this, new BindingValue(0d), "", name, "TACAN X/Y Mode", "1=X, 0=Y", BindingValueUnits.Numeric);
            _xyMode.Execute += new HeliosActionHandler(Mode_Execute);
            Actions.Add(_xyMode);
        }

        void Mode_Execute(object action, HeliosActionEventArgs e)
        {
            _xModeImage.IsHidden = e.Value.StringValue.Equals("1");
            _yModeImage.IsHidden = e.Value.StringValue.Equals("0");
        }
    }
}
