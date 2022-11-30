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

    [HeliosControl("HELIOS.M2000C.LOW_ALTITUDE_SETTING", "Low Altitude Setting Display", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class LowAltitudeSetting : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 400, 205);
        private string _interfaceDeviceName = "AFCS";
        private string _imageAssetLocation = "Helios Assets/M-2000C_V2/";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public LowAltitudeSetting()
            : base("Low Altitude Setting Display", new Size(400, 205))
        {
            Children.Add(AddImage($"{Name}_ThumbWheel1", new Point(90d, 46d)));
            Children.Add(AddImage($"{Name}_ThumbWheel2", new Point(168d, 46d)));
            Children.Add(AddImage($"{Name}_ThumbWheel3", new Point(249d, 46d)));
            AddRotarySwitch("Altitude 10 000 ft Selector", new Point(90, 46), new Size(68, 111), 6);
            AddDrum("Altitude 10 000 ft Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Altitude 10 000 ft Drum", "(0-3)", "#", new Point(115,79), new Size(10, 15), new Size(24,44));
            AddRotarySwitch("Altitude 1 000 ft Selector", new Point(168, 46), new Size(68, 111), 10);
            AddDrum("Altitude 1 000 ft Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Altitude 1 000 ft Drum", "(0-9)", "#", new Point(193, 79), new Size(10, 15), new Size(24, 44));
            AddRotarySwitch("Altitude 100 ft Selector", new Point(240, 46), new Size(68, 111), 10);
            AddDrum("Altitude 100 ft Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Altitude 100 ft Drum", "(0-9)", "#", new Point(274, 79), new Size(10, 15), new Size(24, 44));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return $"{_imageAssetLocation}{Name}/Low_Altitude_Setting.png"; }
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

            AddDefaultInputBinding(
                childName: $"{Name}_{name}",
                interfaceTriggerName: $"{_interfaceDeviceName}.{name.Replace("Drum","Selector")}.changed",
                deviceActionName: "set." + name);

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
                fromCenter: false
                );
            rSwitch.IsContinuous = false;
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


        private ImageDecoration AddImage(string name, Point posn)
        {
            return (new ImageDecoration()
            {
                Name = name,
                Image = $"{_imageAssetLocation}{Name}/Altitude_ThumbWheel_1.png",
                Alignment = ImageAlignment.Stretched,
                Left = posn.X,
                Top = posn.Y,
                Width = 22,
                Height = 110,
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
