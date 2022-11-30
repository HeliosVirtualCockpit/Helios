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
    using System;
    using System.Globalization;
    using System.Runtime.Remoting;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("HELIOS.M2000C.LANDING_GEAR_PANEL_V2", "Landing Gear Panel V2", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class LandingGearPanelV2 : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 587, 800);
        private string _interfaceDeviceName = "Landing Gear Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _imageAssetLocation = "Helios Assets/M-2000C_V2/";

        public LandingGearPanelV2()
            : base("Landing Gear Panel", new Size(587, 800))
        {
            PersistChildren = false;
            PushButton emergencyJettisonButton = AddPushButton("Emergency Jettison Lever");
            AddIndicator("A", new Point(409, 340), new Size(51, 28));
            AddIndicator("F", new Point(472, 340), new Size(51, 28));
            AddIndicator("DIRAV", new Point(377, 375), new Size(52, 30));
            AddIndicator("FREIN", new Point(502, 376), new Size(52, 30));
            AddIndicator("CROSS", new Point(377, 414), new Size(51, 30));
            AddIndicator("SPAD", new Point(501, 414), new Size(52, 31));
            AddIndicator("BIP", new Point(440, 451), new Size(53, 21));
            AddIndicator("left-gear", new Point(404, 457), new Size(33, 54));
            AddIndicator("right-gear", new Point(499, 457), new Size(33, 54));
            AddIndicator("nose-gear", new Point(451, 477), new Size(33, 44));

            AddSwitch("Gun Arm/Safe Switch", $"{Name}/gun-arming-guard-", new Point(280, 352), new Size(92, 107), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, false, false);
            ToggleSwitch fbwgSwitch = AddSwitch("Fly by Wire Gain Mode Switch", $"{Name}/FBW-Gain-Switch-", new Point(308, 522), new Size(48, 97), ToggleSwitchPosition.One, ToggleSwitchType.OnOn, false, true);
            fbwgSwitch.ClickType = LinearClickType.Touch;
            AddGuard("FBW Gain Mode Switch Cover", "fbwg-guard-", new Point(293, 502), new Size(68, 152), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn,
                new NonClickableZone[] { new NonClickableZone(new Rect(0, 0, 68, 117), ToggleSwitchPosition.One, fbwgSwitch, ToggleSwitchPosition.One) },
                false, false, true);
            AddSwitch("Fly by Wire G Limiter Switch", $"{Name}/FBW-G-Limit-Switch-", new Point(459, 518), new Size(47, 106), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, false, false, false);
            AddButton("FBW Reset Button", new Point(461, 627), new Size(44, 45), $"{_imageAssetLocation}{Name}/FBW-Reset-Button-Up.png", $"{_imageAssetLocation}{Name}/FBW-Reset-Button-Up.png", "", _interfaceDeviceName, "FBW Reset Button", false);
            AddButton("Landing Gear Tone", new Point(451, 407), new Size(30, 29), $"{_imageAssetLocation}{Name}/BIP-Button-Up.png", $"{_imageAssetLocation}{Name}/BIP-Button-Down.png", "", _interfaceDeviceName, "Landing Gear Tone", false);
            AddSwitch("Landing Gear Lever", $"{Name}/landing-gear-", new Point(122, 267), new Size(88, 328), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, false, false, true, "Landing Gear Handle Indicator");
            AddRotarySwitch("Emergency Landing Gear Lever", new Point(162, 7), new Size(290, 294), new NonClickableZone[] {
                    new NonClickableZone(new Rect(221, 124, 69, 170), true, emergencyJettisonButton)});
            AddIndicatorDrum("Outer Left Indicator", new Point(160, 706));
            AddIndicatorDrum("Inner Left Indicator", new Point(239, 706));
            AddIndicatorDrum("Rudder Indicator", new Point(315, 709), new Size(52,51),true);
            AddIndicatorDrum("Inner Right Indicator", new Point(393, 706));
            AddIndicatorDrum("Outer Right Indicator", new Point(474, 706));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return $"{_imageAssetLocation}{Name}/lg-panel.png"; }
        }

        public string ImageAssetLocation
        {
            get => _imageAssetLocation;
            set
            {
                if ( value!=null && !_imageAssetLocation.Equals(value))
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

        private void AddIndicator(string name, Point posn, Size size)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: $"{_imageAssetLocation}{Name}/{ name}-on.png",
                offImage: $"{_imageAssetLocation}{Name}/{name}-off.png",
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private ToggleSwitch AddSwitch(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal = false, bool verticalReversed = false)
        {
            return AddSwitch(name, imagePrefix, posn, size, defaultPosition, defaultType, horizontal, verticalReversed, false);
        }
        private ToggleSwitch AddSwitch(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal = false, bool verticalReversed = false, bool indicator = false, string interfaceElementNameIndicator = "")
        {

            ToggleSwitch togSwitch = AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: $"{_imageAssetLocation}{imagePrefix}" + (indicator ? "lit-" : "") + "up.png",
                positionTwoImage: $"{_imageAssetLocation}{imagePrefix}" + (indicator ? "lit-" : "") + "down.png",
                positionOneIndicatorImage: $"{_imageAssetLocation}{imagePrefix}" + (indicator ? "" : "" ) + "up.png",
                positionTwoIndicatorImage: $"{_imageAssetLocation}{imagePrefix}" + (indicator ? "" : "") + "down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementNameIndicator: interfaceElementNameIndicator,
                interfaceElementName: name,
                horizontal: horizontal,
                indicator: indicator,
                fromCenter: false);
            togSwitch.Orientation = verticalReversed ? ToggleSwitchOrientation.VerticalReversed : ToggleSwitchOrientation.Vertical;
            togSwitch.ClickType = LinearClickType.Swipe;
            return togSwitch;
        }

        private void AddGuard(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition,
            ToggleSwitchType defaultType, NonClickableZone[] nonClickableZones, bool horizontal = true, bool horizontalRender = true, bool verticalReversed = false)
        {
            ToggleSwitch cover = AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: $"{_imageAssetLocation}{Name}/{imagePrefix}up.png",
                positionTwoImage: $"{_imageAssetLocation}{Name}/{imagePrefix}down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                horizontalRender: horizontalRender,
                nonClickableZones: nonClickableZones,
                fromCenter: false);
            cover.Orientation = verticalReversed ? ToggleSwitchOrientation.VerticalReversed : ToggleSwitchOrientation.Vertical;
            cover.ClickType = LinearClickType.Swipe;
        }

        private PushButton AddPushButton(string name)
        {
            return AddButton(name: name,
                posn: new Point(385, 144),
                size: new Size(166, 167),
                image: $"{_imageAssetLocation}{Name}/emergency-jettison-not-pushed.png",
                pushedImage: $"{_imageAssetLocation}{Name}/emergency-jettison-pushed.png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
        }

        private void AddRotarySwitch(string name, Point posn, Size size, NonClickableZone[] nonClickableZones)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: $"{_imageAssetLocation}{Name}/emergency-landing-gear-lever.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                nonClickableZones: nonClickableZones,
                fromCenter: false);
            rSwitch.Positions.Clear();
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "OFF", 0d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "ON", 90d));
        }

        private void AddIndicatorDrum(string name, Point posn)
        {
            AddIndicatorDrum(name, posn, new Size(46, 51), false);
        }
        private void AddIndicatorDrum(string name, Point posn, Size size , bool vertical = false)
        {
            CustomDrum customDrum = new CustomDrum( $"{Name}_{name}", size);
            customDrum.Left = posn.X;
            customDrum.Top = posn.Y;
            customDrum.DrumImage = $"{_imageAssetLocation}{Name}/Bar-Indicator-Horizontal.xaml";
            customDrum.Drum_PosX = 0;
            customDrum.Drum_PosY = -28;
            customDrum.Drum_Width = 46;
            customDrum.Drum_Height = 110;
            customDrum.MinVertical = 20;
            customDrum.VerticalTravel = -20;
            customDrum.InitialVertical = -20;
            customDrum.MinInputVertical = -1;
            customDrum.MaxInputVertical = 1;
            if (vertical) { customDrum.Rotation = HeliosVisualRotation.CW; }
            Children.Add(customDrum);
            foreach (IBindingAction action in customDrum.Actions)
            {
                AddAction(action, $"{Name}_{name}");
            }

            AddDefaultInputBinding(
                childName: $"{Name}_{name}",
                interfaceTriggerName: _interfaceDeviceName + "." + name + ".changed",
                deviceActionName: "set." + "Drum tape offset");
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
