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

    [HeliosControl("HELIOS.M2000C.IFF_MODE_SETTING", "IFF Mode Setting Display", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class IFFMode : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 600, 190);
        private string _interfaceDeviceName = "IFF";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _imageAssetLocation = "Helios Assets/M-2000C/";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public IFFMode()
            : base("IFF Mode Setting Display", new Size(600, 190))
        {

            Children.Add(AddImage($"{Name}_ThumbWheel1", new Point(109d, 64d)));
            Children.Add(AddImage($"{Name}_ThumbWheel2", new Point(184d, 64d)));
            Children.Add(AddImage($"{Name}_ThumbWheel3", new Point(290d, 64d)));
            Children.Add(AddImage($"{Name}_ThumbWheel4", new Point(366d, 64d)));
            Children.Add(AddImage($"{Name}_ThumbWheel5", new Point(443d, 64d)));
            Children.Add(AddImage($"{Name}_ThumbWheel6", new Point(519d, 64d)));

            AddRotarySwitch("Mode-1 Tens Selector", new Point(109, 64), new Size(46, 81), 10);
            AddDrum("Mode-1 Tens Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Mode-1 Tens Drum", "(0-9)", "#", new Point(81, 90), new Size(10, 15), new Size(26, 39));
            AddRotarySwitch("Mode-1 Ones Selector", new Point(184, 64), new Size(46, 81), 10);
            AddDrum("Mode-1 Ones Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Mode-1 Ones Drum", "(0-9)", "#", new Point(157, 90), new Size(10, 15), new Size(26, 39));

            AddRotarySwitch("Mode-3A Thousands Selector", new Point(290, 64), new Size(46, 81), 10);
            AddDrum("Mode-3A Thousands Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Mode-3A Thousands Drum", "(0-9)", "#", new Point(263, 90), new Size(10, 15), new Size(26, 39));
            AddRotarySwitch("Mode-3A Hundreds Selector", new Point(366, 64), new Size(46, 81), 10);
            AddDrum("Mode-3A Hundreds Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Mode-3A Hundreds Drum", "(0-9)", "#", new Point(341, 90), new Size(10, 15), new Size(26, 39));
            AddRotarySwitch("Mode-3A Tens Selector", new Point(443, 64), new Size(46, 81), 10);
            AddDrum("Mode-3A Tens Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Mode-3A Tens Drum", "(0-9)", "#", new Point(419, 90), new Size(10, 15), new Size(26, 39));
            AddRotarySwitch("Mode-3A Ones Selector", new Point(519, 64), new Size(46, 81), 10);
            AddDrum("Mode-3A Ones Drum", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Mode-3A Ones Drum", "(0-9)", "#", new Point(495, 90), new Size(10, 15), new Size(26, 39));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return $"{_imageAssetLocation}{Name}/IFF_Code_Settings.png"; }
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
                interfaceTriggerName: $"{_interfaceDeviceName}.{name.Replace("Drum", "Selector")}.changed",
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
