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


namespace GadrocsWorkshop.Helios.Gauges.M2000C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using NLog;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("HELIOS.M2000C.RADAR_IFF_MODE_PANEL", "RADAR IFF Mode Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class RADARIFFMode : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 600, 186);
        private string _interfaceDeviceName = "RADAR IFF";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _imageAssetLocation = "Helios Assets/M-2000C/";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public RADARIFFMode()
            : base("RADAR IFF Mode Panel", new Size(600, 186))
        {

            AddRotarySwitch("Radar IFF Code-4 Selector", new Point(206, 47), new Size(40, 84), 10);
            AddDrum("Radar IFF Code-4 Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Radar IFF Code-4 Drum", "(0-9)", "#", new Point(212, 75), new Size(10, 15), new Size(18, 23));
            AddRotarySwitch("Radar IFF Code-3 Selector", new Point(259, 47), new Size(40, 84), 10);
            AddDrum("Radar IFF Code-3 Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Radar IFF Code-3 Drum", "(0-9)", "#", new Point(264, 75), new Size(10, 15), new Size(18, 23));
            AddRotarySwitch("Radar IFF Code-2 Selector", new Point(315, 47), new Size(40, 84), 10);
            AddDrum("Radar IFF Code-2 Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Radar IFF Code-2 Drum", "(0-9)", "#", new Point(319, 75), new Size(10, 15), new Size(18, 23));
            AddRotarySwitch("Radar IFF Code-1 Selector", new Point(367, 47), new Size(40, 84), 10);
            AddDrum("Radar IFF Code-1 Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Radar IFF Code-1 Drum", "(0-9)", "#", new Point(370, 75), new Size(10, 15), new Size(18, 23));
            AddRotarySwitch1("Radar IFF Mode Switch", new Point(47, -5), new Size(137, 136), 6, new String[] { "1", "4", "3/2", "3/3", "3/4", "2" }, new Double[] { 280, 305, 330, 0, 28, 53 });
            AddRotarySwitch1("Radar IFF Power Switch", new Point(414, -5), new Size(137, 136), 3, new String[] { "Off", "Sect", "blank" }, new Double[] { 280, 0, 85 });
            AddToggleSwitch("Radar IFF L/R Selector", new Point(78, 136), new Size(32, 77), ToggleSwitchPosition.One, $"{_imageAssetLocation}{Name}/black-circle-up.png", $"{_imageAssetLocation}{Name}/black-circle-down.png", ToggleSwitchType.OnOn, LinearClickType.Swipe,_interfaceDeviceName, "Radar IFF L/R Selector", false, true);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return $"{_imageAssetLocation}{Name}/RADAR_IFF_Panel.png"; }
        }

        public string ImageAssetLocation
        {
            get => _imageAssetLocation;
            set
            {
                if (value != null && !_imageAssetLocation.Equals(value))
                {
                    string oldValue = _imageAssetLocation;
                    _imageAssetLocation = value;
                    OnPropertyChanged("ImageAssetLocation", oldValue, value, false);
                    Refresh();
                }
            }
        }
        #endregion
        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                double scaleX = Width / NativeSize.Width;
                double scaleY = Height / NativeSize.Height;
                _scaledScreenRect.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }

        private void AddDrum(string name, string gaugeImage, string actionIdentifier, string valueDescription, string format, Point posn, Size size, Size renderSize)
        {
            Mk2CDrumGauge.Mk2CDrumGauge drum = AddDrumGauge(name: name,
                gaugeImage: gaugeImage,
                posn: posn,
                size: size,
                renderSize: renderSize,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: actionIdentifier,
                actionIdentifier: actionIdentifier,
                valueDescription: valueDescription,
                format: format,
                fromCenter: false,
                multiplier: 1d,
                offset: -1d);

            try
            {
                /// This is an internal binding within the gauge as opposed to a binding to the default interface
                /// and it is required because the data for the drum is not passed explicity over tbe interface.
                InputBindings.Add(CreateNewBinding(Children[$"{Name}_{actionIdentifier.Replace("Drum", "Selector")}"].Triggers["position.changed"], drum.Actions[$"set.{name}"]));
            }
            catch
            {
                Logger.Error($"Unable to create self-binding for gauge {Name}_{actionIdentifier.Replace("Drum", "Selector")} trigger: {actionIdentifier.Replace("Drum", "Selector")} \"position.changed\" action: {drum.Name} \"set.Drum tape offset\" ");
            }

        }

        private void AddRotarySwitch(string name, Point posn, Size size, int positions)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: "{M2000C}/Images/Miscellaneous/void.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Swipe,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
            rSwitch.Positions.Clear();
            for (int i = 0; i < positions; i++)
            {
                rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, i, i.ToString(), i));
            }
            string drum = name.Replace("Selector", "Drum");
            AddDefaultInputBinding(
                 childName: $"{Name}_{drum}",
                 interfaceTriggerName: $"{_interfaceDeviceName}.{name}.changed",
                 deviceActionName: $"set.{drum}");
        }
        private void AddRotarySwitch1(string name, Point posn, Size size, int positions, String[] switchLabels, Double[] switchAngles)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: $"{_imageAssetLocation}{Name}/RADAR_IFF_Knob.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Swipe,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
            rSwitch.Positions.Clear();
            for (int i = 0; i < positions; i++)
            {
                rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, i, switchLabels[i], switchAngles[i]));
            }
        }
            private ImageDecoration AddImage(string name, Point posn)
        {
            return(new ImageDecoration()
            {
                Name = name,
                Image = $"{_imageAssetLocation}{Name}/IFF_Thumbwheel_1.png",
                Alignment = ImageAlignment.Stretched,
                Left = posn.X,
                Top = posn.Y,
                Width = 18,
                Height = 81,
                IsHidden = false
            });

        }
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name.Equals("ImageAssetLocation"))
            {
                ImageAssetLocation = reader.ReadElementString("ImageAssetLocation");
            }
        }
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("ImageAssetLocation", _imageAssetLocation.ToString(CultureInfo.InvariantCulture));
        }
        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return true;
        }

        public override void MouseDown(Point location)
        {
            // No-Op
        }

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }
    }
}
