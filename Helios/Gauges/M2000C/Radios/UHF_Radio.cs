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
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("HELIOS.M2000C.UHF_RADIO", "UHF Radio", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class UHFRadio : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 600, 189);
        private string _interfaceDeviceName = "UHF Radio Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _imageAssetLocation = "Helios Assets/M-2000C/";
        private string _xamlLocation = "{helios}/Images/M-2000C/";
        private Potentiometer _potentiometer;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public UHFRadio()
            : base("UHF Radio", new Size(600, 189))
        {
            AddSwitch("UHF Power 5W/25W Switch", "{M2000C}/Images/Switches/short-black-", new Point(106, 12), new Size(40, 80), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("UHF SIL Switch", "{M2000C}/Images/Switches/long-black-", new Point(180, 8), new Size(30, 80), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            AddThreeWayToggle("UHF E+A2 Switch", new Point(250, 20), new Size(30, 80), "UHF E+A2 Switch", "{M2000C}/Images/Switches/long-black-");
            AddIndicatorPushButton("UHF CDE Switch", new Point(82, 118), new Size(32, 30), "Green_UHF_Button", _interfaceDeviceName, "UHF CDE Switch", "UHF CDE Indicator");
            AddRotarySwitch("UHF Mode Switch", new Point(140d, 98d), new Size(80d, 80d));
            AddIndicatorPushButton("UHF Test Switch", new Point(250, 118), new Size(31,30), "Orange_UHF_Button", _interfaceDeviceName, "UHF TEST Switch", "UHF TEST Indicator");
            _potentiometer = AddPot("UHF Channel Sel", new Point(320d, 6d), new Size(140d, 140d), $"{_imageAssetLocation}{Name}/UHF_Channel_Knob.png", 0d, 360d, 0.0d, 1.0d, 0.0d, 0.05d, _interfaceDeviceName, "UHF Channel Select", false, RotaryClickType.Swipe, true);
            AddIndicatorDrum("UHF Channel Display", new Point(471, 44));
         }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return $"{_imageAssetLocation}{Name}/UHF_Radio_Panel.png"; }
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

        private void AddRotarySwitch(string name, Point posn, Size size, NonClickableZone[] nonClickableZones = null)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: $"{_imageAssetLocation}{Name}/UHF_Mode_Knob.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Swipe,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                nonClickableZones: nonClickableZones,
                fromCenter: false) ;
            rSwitch.Positions.Clear();
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "AR", 225d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "M", 315d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 3, "FI", 45d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 4, "H", 135d));
        }

        private void AddIndicatorDrum(string name, Point posn)
        {
            AddIndicatorDrum(name, posn, new Size(42, 54), false);
        }
        private void AddIndicatorDrum(string name, Point posn, Size size, bool vertical = false)
        {
            CustomDrum customDrum = new CustomDrum($"{Name}_{name}", size);
            customDrum.Left = posn.X;
            customDrum.Top = posn.Y;
            customDrum.DrumImage = $"{_xamlLocation}{Name}/Channel_Drum_Tape.xaml";
            customDrum.Drum_PosX = 4;
            customDrum.Drum_PosY = 4;
            customDrum.Drum_Width = 46;
            customDrum.Drum_Height = 1188;
            customDrum.MinVertical = -54;
            customDrum.VerticalTravel = -1134;
            customDrum.InitialVertical = -54;
            customDrum.MinInputVertical = 0;
            customDrum.MaxInputVertical = 1;
            if (vertical) { customDrum.Rotation = HeliosVisualRotation.CW; }
            Children.Add(customDrum);
            foreach (IBindingAction action in customDrum.Actions)
            {
                AddAction(action, $"{Name}_{name}");
            }

            AddDefaultInputBinding(
                childName: $"{Name}_{name}",
                interfaceTriggerName: _interfaceDeviceName + "." + "UHF Channel Select" + ".changed",
                deviceActionName: "set." + "Drum tape offset");
            try
            {
                /// This is an internal binding within the gauge as opposed to a binding to the default interface
                InputBindings.Add(CreateNewBinding(_potentiometer.Triggers["value.changed"], customDrum.Actions["set.Drum tape offset"]));
            }
            catch
            {
                Logger.Error($"Unable to create self-binding for gauge {Name} control {name} trigger: {_potentiometer.Name} \"value.changed\" action: {customDrum.Name} \"set.Drum tape offset\" ");
            }
        }

        private ToggleSwitch AddSwitch(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal = false, bool verticalReversed = false, bool indicator = false)
        {

            ToggleSwitch togSwitch = AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: imagePrefix + (indicator ? "lit-" : "") + "up.png",
                positionTwoImage: imagePrefix + (indicator ? "lit-" : "") + "down.png",
                positionOneIndicatorImage: imagePrefix + (indicator ? "" : "") + "up.png",
                positionTwoIndicatorImage: imagePrefix + (indicator ? "" : "") + "down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                indicator: indicator,
                fromCenter: false);
            togSwitch.Orientation = verticalReversed ? ToggleSwitchOrientation.VerticalReversed : ToggleSwitchOrientation.Vertical;
            togSwitch.ClickType = LinearClickType.Swipe;
            return togSwitch;
        }

        private void AddThreeWayToggle(string name, Point posn, Size size, string interfaceElementName) =>
            AddThreeWayToggle(name, posn, size, interfaceElementName, "Toggles/round-");
        private void AddThreeWayToggle(string name, Point posn, Size size, string interfaceElementName, string imageStem)
        {
            ThreeWayToggleSwitch toggle = AddThreeWayToggle(
                name: name,
                posn: posn,
                size: size,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.MomOnMom,
                positionOneImage: imageStem + "Up.png",
                positionTwoImage: imageStem + "Mid.png",
                positionThreeImage: imageStem + "Down.png",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        private IndicatorPushButton AddIndicatorPushButton(string name, Point pos, Size size, string image,
            string interfaceDeviceName = "", string interfaceElementName = "", string interfaceElementIndicatorName = "")
        {
            string componentName = $"{Name}_{name}";

            IndicatorPushButton indicator = new Helios.Controls.IndicatorPushButton
            {
                Top = pos.Y,
                Left = pos.X,
                Width = size.Width,
                Height = size.Height,
                Image = $"{_imageAssetLocation}{Name}/{image}_Up.png",
                PushedImage = $"{_imageAssetLocation}{Name}/{image}_Down.png",
                IndicatorOnImage = $"{_imageAssetLocation}{Name}/{image}_Lit_Up.png",
                PushedIndicatorOnImage = $"{_imageAssetLocation}{Name}/{image}_Lit_Down.png",
                Name = componentName,
                OnTextColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                TextColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00)
            };
            indicator.Text = "";

            Children.Add(indicator);
            foreach (IBindingTrigger trigger in indicator.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in indicator.Actions)
            {
                AddAction(action, componentName);
            }

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementIndicatorName + ".changed",
                deviceActionName: "set.indicator");
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.physical state");
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "pushed",
                interfaceActionName: interfaceDeviceName + ".push." + interfaceElementName);
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "released",
                interfaceActionName: interfaceDeviceName + ".release." + interfaceElementName);

            return indicator;
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
    }
}
