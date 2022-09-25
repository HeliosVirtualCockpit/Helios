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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.TACANChannel
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M2000C.TACANChannel", "TACAN Channel", "M2000C Gauges", typeof(GaugeRenderer))]
    public class TACANChannel : BaseGauge
    {
        private HeliosValue _channel;
        private HeliosValue _xyMode;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _onesDrum;
        private GaugeImage _xModeImage;
        private GaugeImage _yModeImage;

        public TACANChannel()
            : base("TACAN Channel", new Size(243, 100))
        {
            Components.Add(new GaugeImage("{Helios}/Gauges/M2000C/TACANChannel/tacan_channel_faceplate.xaml", new Rect(0d, 0d, 243d, 100d)));

            _tensDrum = new GaugeDrumCounter("{Helios}/Gauges/M2000C/TACANChannel/tacan_channel_hundreds_tape.xaml", new Point(84d, 11.5d), "##", new Size(10d, 15d), new Size(50d, 75d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(84d, 11.5d, 100d, 75d));
            Components.Add(_tensDrum);

            _onesDrum = new GaugeDrumCounter("{Helios}/Gauges/M2000C/Common/drum_tape.xaml", new Point(177d, 11.5d), "#", new Size(10d, 15d), new Size(50d, 75d));
            _onesDrum.Clip = new RectangleGeometry(new Rect(177d, 11.5d, 50d, 75d));
            Components.Add(_onesDrum);

            _xModeImage = new GaugeImage("{Helios}/Gauges/M2000C/TACANChannel/tacan_channel_x_mode.xaml", new Rect(20d, 20d, 44d, 59d));
            Components.Add(_xModeImage);

            _yModeImage = new GaugeImage("{Helios}/Gauges/M2000C/TACANChannel/tacan_channel_y_mode.xaml", new Rect(20d, 20d, 44d, 59d));
            _yModeImage.IsHidden = true;
            Components.Add(_yModeImage);

            _channel = new HeliosValue(this, new BindingValue(0d), "", "channel", "TACAN channel", "Does not include X/Y mode", BindingValueUnits.Numeric);
            _channel.Execute += new HeliosActionHandler(Channel_Execute);
            Actions.Add(_channel);

            _xyMode = new HeliosValue(this, new BindingValue(0d), "", "mode", "TACAN X/Y Mode", "1=X, 2=Y", BindingValueUnits.Numeric);
            _xyMode.Execute += new HeliosActionHandler(Mode_Execute);
            Actions.Add(_xyMode);
        }

        void Channel_Execute(object action, HeliosActionEventArgs e)
        {
            _onesDrum.Value = e.Value.DoubleValue;
            _tensDrum.Value = _onesDrum.Value / 10d;
        }
       
        void Mode_Execute(object action, HeliosActionEventArgs e)
        {
            _xModeImage.IsHidden = e.Value.StringValue.Equals("2");
            _yModeImage.IsHidden = e.Value.StringValue.Equals("1");
        }
    }
}
