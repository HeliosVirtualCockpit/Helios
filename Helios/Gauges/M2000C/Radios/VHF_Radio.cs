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
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("HELIOS.M2000C.VHF_RADIO", "VHF Radio", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class VHFRadio : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 600, 211);
        private string _interfaceDeviceName = "VHF Radio Panel";
        private string _font = "Helios Virtual Cockpit A-10C_ALQ_213";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private bool _useTextualDisplays = true;
        private TextDisplay _textDisplay;
        private string _imageAssetLocation = "{M2000C}/Images/V2/";

        public VHFRadio()
            : base("VHF Radio", new Size(600, 211))
        {

            PersistChildren = false;
            AddTextDisplay("VHF Comm Information", new Point(166d, 20d), new Size(222d, 40d), _interfaceDeviceName, "VHF Comm Information", 26, "********", TextHorizontalAlignment.Left, "");
            AddRotarySwitch("VHF MODE", new Point(41d, 104d), new Size(77d, 77d));
            AddEncoder("VHF Channel Sel", new Point(474d, 29d), new Size(90d, 90d), $"{_imageAssetLocation}{Name}/Encoder_Knob.png", 0.1d, 20d, _interfaceDeviceName, "VHF Channel Select", false);
            AddIndicatorPushButton("Key CLR/MEM", new Point(84, 12), new Size(51, 50), "CLR_MEM", "CLR", _interfaceDeviceName, "Key CLR/MEM", "Indicator CLR");
            AddIndicator("Indicator MEM", new Point(84, 12), new Size(51, 50), $"{_imageAssetLocation}{Name}/Key_MEM.png", null, _interfaceDeviceName, "Indicator MEM");
            AddIndicatorPushButton("Key VLD/XFR", new Point(397, 13), new Size(51, 50), "VLD_XFR", "VLD", _interfaceDeviceName, "Key VLD/XFR", "Indicator VLD");
            AddIndicator("Indicator XFR", new Point(397, 13), new Size(51, 50), $"{_imageAssetLocation}{Name}/Key_XFR.png", null, _interfaceDeviceName, "Indicator XFR");
            AddIndicatorPushButton("Key 1", new Point(148, 75), new Size(50, 50), "1", "1", _interfaceDeviceName, "Key 1/READ", "Indicator 1");
            AddIndicator("Indicator READ", new Point(148, 75), new Size(51, 50), $"{_imageAssetLocation}{Name}/Key_READ.png", null, _interfaceDeviceName, "Indicator READ");
            AddIndicatorPushButton("Key 2", new Point(211, 75), new Size(50, 50), "2", "2", _interfaceDeviceName, "Key 2/SQL", "Indicator 2");
            AddIndicator("Indicator 2 Light", new Point(211, 75), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_2_LIGHT.png", null, _interfaceDeviceName, "Indicator 2LIGHT");
            AddIndicator("Indicator SQL", new Point(211, 75), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_SQL.png", null, _interfaceDeviceName, "Indicator SQL");
            AddIndicatorPushButton("Key 3", new Point(274, 75), new Size(50, 50), "3", "3", _interfaceDeviceName, "Key 3/GR", "Indicator 3");
            AddIndicator("Indicator 3 Light", new Point(274, 75), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_3_LIGHT.png", null, _interfaceDeviceName, "Indicator 3LIGHT");
            AddIndicator("Indicator GR", new Point(274, 75), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_GR.png", null, _interfaceDeviceName, "Indicator GR");
            AddIndicatorPushButton("Key 4", new Point(335, 75), new Size(50, 50), "4", "4", _interfaceDeviceName, "Key 4", "Indicator 4");
            AddIndicatorPushButton("Key 5", new Point(398, 75), new Size(50, 50), "5", "5", _interfaceDeviceName, "Key 5/20/LOW", "Indicator 5");
            AddIndicator("Indicator 20", new Point(398, 75), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_20.png", null, _interfaceDeviceName, "Indicator 20");
            AddIndicator("Indicator LOW", new Point(398, 75), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_LOW.png", null, _interfaceDeviceName, "Indicator LOW");
            AddIndicatorPushButton("Key 6", new Point(148, 141), new Size(50, 50), "6", "6", _interfaceDeviceName, "Key 6/TONE", "Indicator 6");
            AddIndicator("Indicator TONE", new Point(148, 141), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_TONE.png", null, _interfaceDeviceName, "Indicator TONE");
            AddIndicatorPushButton("Key 7", new Point(211, 141), new Size(50, 50), "7", "7", _interfaceDeviceName, "Key 7", "Indicator 7");
            AddIndicatorPushButton("Key 8", new Point(274, 141), new Size(50, 50), "8", "8", _interfaceDeviceName, "Key 8/TOD", "Indicator 8");
            AddIndicator("Indicator TOD", new Point(274, 141), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_TOD.png", null, _interfaceDeviceName, "Indicator TOD");
            AddIndicatorPushButton("Key 9", new Point(335, 141), new Size(50, 50), "9", "9", _interfaceDeviceName, "Key 9/ZERO", "Indicator 9");
            AddIndicator("Indicator ZERO", new Point(335, 141), new Size(50, 50), $"{_imageAssetLocation}{Name}/Key_ZERO.png", null, _interfaceDeviceName, "Indicator ZERO");
            AddIndicatorPushButton("Key 0", new Point(398, 141), new Size(50, 50), "0", "0", _interfaceDeviceName, "Key 0", "Indicator 0");
            AddIndicatorPushButton("Key Conf", new Point(464, 141), new Size(66, 47), "CONF", "CONF", _interfaceDeviceName, "Key CONF", "Indicator CONF");

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return $"{_imageAssetLocation}{Name}/M-2000C_VHF_Radio_Background.png"; }
        }

        public bool UseTextualDisplays
        {
            get => _useTextualDisplays;
            set
            {
                if (value != _useTextualDisplays)
                {
                    _useTextualDisplays = value;
                    foreach (HeliosVisual child in this.Children)
                    {
                        if (child is TextDisplay textDisplay)
                        {
                            textDisplay.IsHidden = !_useTextualDisplays;
                        }
                    }
                    _textDisplay.IsHidden = !_useTextualDisplays;
                    Refresh();
                }
            }
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
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name.Equals("UseTextualDisplays"))
            {
                UseTextualDisplays = bool.Parse(reader.ReadElementString("UseTextualDisplays"));
            }
            if (reader.Name.Equals("ImageAssetLocation"))
            {
                ImageAssetLocation = reader.ReadElementString("ImageAssetLocation");
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("UseTextualDisplays", _useTextualDisplays.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ImageAssetLocation", _imageAssetLocation.ToString(CultureInfo.InvariantCulture));
        }

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

        private void AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, double baseFontsize,
                string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            _textDisplay = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xcc, 0x50, 0xc3, 0x39),
                backgroundColor: Color.FromArgb(0xff, 0x04, 0x2a, 0x00),
                useBackground: false,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
            _textDisplay.IsHidden = !_useTextualDisplays;

        }
        private void AddRotarySwitch(string name, Point posn, Size size, NonClickableZone[] nonClickableZones = null)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: $"{_imageAssetLocation}{Name}/Mode_Knob.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Swipe,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                nonClickableZones: nonClickableZones,
                fromCenter: false) ;
            rSwitch.Positions.Clear();
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "O", 325d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "FF", 0d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 3, "HQ", 45d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 4, "SV", 75d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 5, "DL", 115d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 6, "G", 150d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 7, "EN", 190d));
        }
        private IndicatorPushButton AddIndicatorPushButton(string name, Point pos, Size size, string image, string indicatorImage,
            string interfaceDeviceName = "", string interfaceElementName = "", string interfaceElementIndicatorName = "")
        {
            string componentName = $"{Name}_{name}";

            IndicatorPushButton indicator = new Helios.Controls.IndicatorPushButton
            {
                Top = pos.Y,
                Left = pos.X,
                Width = size.Width,
                Height = size.Height,
                Image = $"{_imageAssetLocation}{Name}/Key_{image}_Blank.png",
                PushedImage = $"{_imageAssetLocation}{Name}/Key_{image}_Blank.png",
                IndicatorOnImage = $"{_imageAssetLocation}{Name}/Key_{indicatorImage}.png",
                PushedIndicatorOnImage = $"{_imageAssetLocation}{Name}/Key_{indicatorImage}.png",
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
        protected Indicator AddIndicator(string name, Point posn, Size size,
            string onImage, string offImage, string interfaceDeviceName, string interfaceElementName)
        {
            string componentName = $"{Name}_{name}";
            Indicator indicator = new Helios.Controls.Indicator
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                OnImage = onImage,
                OffImage = offImage,
                Name = componentName
            };
            indicator.Text = "";
            indicator.AllowInteraction = false;

            Children.Add(indicator);
            foreach (IBindingTrigger trigger in indicator.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(indicator.Actions["set.indicator"], componentName);

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.indicator");

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
    }
}
