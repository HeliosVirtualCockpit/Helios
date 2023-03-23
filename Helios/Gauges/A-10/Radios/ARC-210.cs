//  Copyright 2020 Helios Contributors
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

// ReSharper disable once CheckNamespace
namespace GadrocsWorkshop.Helios.Gauges.A10C.ARC210
{
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Globalization;
    using System.Xml;
    using System.Collections.Generic;

    /// <summary>
    /// This is an A-10C UHF Radio that uses text displays instead of an exported viewport.
    /// </summary>
    [HeliosControl("Helios.A10C.ARC210Radio", "ARC-210 Radio", "A-10C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class ARC210Radio : A10CDevice
    {
        //private const double SCREENRES = 1.0;
        private readonly string _interfaceDeviceName = "ARC-210";
        private readonly string _imageLocation = "{A-10C}/Images/A-10CII/";
        private bool _useTextualDisplays = false;
        private ImageDecoration _displayBackground;
        private string _vpName = "";
        private string _font = "LED Counter 7";
        private List<TextDisplay> _textDisplayList = new List<TextDisplay>();

        public ARC210Radio()
            : base("ARC-210 Radio", new Size(640, 523))
        {

            _displayBackground = AddImage($"{_imageLocation}ARC-210_Display.png", new Point(148d, 91d), new Size(297d,193d), $"{_imageLocation}ARC-210_Display.png");
            _textDisplayList.Add(AddTextDisplay("Frequency Display", new Point(185, 223), new Size(259, 60), _interfaceDeviceName, "Frequency Display", 42, "133.100", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Modulation Mode", new Point(351, 194), new Size(72, 42), _interfaceDeviceName, "Modulation Mode", 32, "AM", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Communications Security Mode", new Point(150, 150), new Size(291, 42), _interfaceDeviceName, "Communications Security Mode", 32, "Comm Sec", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Communications Security Submode", new Point(150, 175), new Size(291, 48), _interfaceDeviceName, "Communications Security Submode", 32, "PT", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Prev Label Display", new Point(149, 83), new Size(106, 42), _interfaceDeviceName, "Prev Label Display", 32, "PREV", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Display of Previous Manual Frequency", new Point(220, 83), new Size(180, 42), _interfaceDeviceName, "Display of Previous Manual Frequency", 32, "133.100", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("RT Label", new Point(368, 83), new Size(80, 42), _interfaceDeviceName, "RT Label", 32, "RT1", TextHorizontalAlignment.Left, ""));

            RotarySwitchPositionCollection positions = new RotarySwitchPositionCollection();
            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "OFF", 225d));
            positions.Add(new RotarySwitchPosition(this, 2, "TR G", 270d));
            positions.Add(new RotarySwitchPosition(this, 3, "TR", 315d));
            positions.Add(new RotarySwitchPosition(this, 4, "ADF", 360d));
            positions.Add(new RotarySwitchPosition(this, 5, "CHG PRST", 45d));
            positions.Add(new RotarySwitchPosition(this, 6, "TEST", 90d));
            positions.Add(new RotarySwitchPosition(this, 7, "ZERO (PULL)", 135d));
            AddRotarySwitch("Master switch", new Point(119, 418), new Size(90, 90), $"{_imageLocation}ARC-210_Rotary_Switch.png", 1, positions, "Master switch");
            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "ECCM MASTER", 225d));
            positions.Add(new RotarySwitchPosition(this, 2, "ECCM", 270d));
            positions.Add(new RotarySwitchPosition(this, 3, "PRST", 315d));
            positions.Add(new RotarySwitchPosition(this, 4, "MAN", 360d));
            positions.Add(new RotarySwitchPosition(this, 5, "MAR", 45d));
            positions.Add(new RotarySwitchPosition(this, 6, "243", 90d));
            positions.Add(new RotarySwitchPosition(this, 7, "121 (PULL)", 135d));
            AddRotarySwitch("Secondary switch", new Point(430, 418), new Size(90, 90), $"{_imageLocation}ARC-210_Rotary_Switch.png", 1, positions, "Secondary switch");

            RotaryEncoder enc;
            enc = AddEncoder("Channel select knob", new Point(276, 420), new Size(82,82),  $"{_imageLocation}ARC-210_Channel_Knob.png", 0.1d, 30d, _interfaceDeviceName, "Channel select knob", false);
            enc.InitialRotation = 15;

            AddPushButton("Upper FSK", 44, 91, new Size(44, 30), $"{_imageLocation}ARC-210_Button_FSK_Up.png", "Upper FSK");
            AddPushButton("Middle FSK", 44, 171, new Size(44, 30), $"{_imageLocation}ARC-210_Button_FSK_Up.png", "Middle FSK");
            AddPushButton("Lower FSK", 44, 251, new Size(44, 30), $"{_imageLocation}ARC-210_Button_FSK_Up.png", "Lower FSK");

            AddPushButton("Brightness increase", 16, 304, new Size(35, 43), $"{_imageLocation}ARC-210_Button_UpArrow_Up.png", "Brightness increase");
            AddPushButton("Brightness decrease", 16, 383, new Size(35, 43), $"{_imageLocation}ARC-210_Button_DownArrow_Up.png", "Brightness decrease");

            AddPushButton("Time of day send", 121, 7, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Time of day send");
            AddPushButton("Time of day receive", 243, 7, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Time of day receive");
            AddPushButton("Global positioning system", 331, 7, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Global positioning system");
            AddPushButton("Select receiver - transmitter", 417, 7, new Size(70, 33), $"{_imageLocation}ARC-210_WideButton_Up.png", "Select receiver - transmitter");

            AddPushButton("Menu pages", 494, 177, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Menu pages");
            AddPushButton("Amplitude modulation / frequency modulation select", 579, 177, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Amplitude modulation / frequency modulation select");
            AddPushButton("Transmit / receive function toggle", 494, 254, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Transmit / receive function toggle");
            AddPushButton("Offset frequency", 579, 254, new Size(45, 33), $"{_imageLocation}ARC-210_Button_Up.png", "Offset frequency");
            AddPushButton("Enter", 585, 323, new Size(45, 101), $"{_imageLocation}ARC-210_Button_Enter_Up.png", "Enter");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 MHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 2, "100 MHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 3, "200 MHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 4, "300 MHz", 30d));
            AddRotarySwitch("100 MHz Selector", new Point(77, 329), new Size(70, 70), $"{_imageLocation}ARC-210_Frequency_Knob.png", 1, positions, "100 MHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 MHz",  270d));
            positions.Add(new RotarySwitchPosition(this, 2, "10 MHz", 290d));
            positions.Add(new RotarySwitchPosition(this, 3, "20 MHz", 310d));
            positions.Add(new RotarySwitchPosition(this, 4, "30 MHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 5, "40 MHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 6, "50 MHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 7, "60 MHz", 30d));
            positions.Add(new RotarySwitchPosition(this, 8, "70 MHz", 50d));
            positions.Add(new RotarySwitchPosition(this, 9, "80 MHz", 70d));
            positions.Add(new RotarySwitchPosition(this, 10, "90 MHz", 90d));
            AddRotarySwitch("10 MHz Selector", new Point(182, 329), new Size(70, 70), $"{_imageLocation}ARC-210_Frequency_Knob.png", 1, positions, "10 MHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 MHz", 270d));
            positions.Add(new RotarySwitchPosition(this, 2, "1 MHz", 290d));
            positions.Add(new RotarySwitchPosition(this, 3, "2 MHz", 310d));
            positions.Add(new RotarySwitchPosition(this, 4, "3 MHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 5, "4 MHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 6, "5 MHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 7, "6 MHz", 30d));
            positions.Add(new RotarySwitchPosition(this, 8, "7 MHz", 50d));
            positions.Add(new RotarySwitchPosition(this, 9, "8 MHz", 70d));
            positions.Add(new RotarySwitchPosition(this, 10, "9 MHz", 90d));
            AddRotarySwitch("1 MHz Selector", new Point(288, 329), new Size(70, 70), $"{_imageLocation}ARC-210_Frequency_Knob.png", 1, positions, "1 MHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 KHz",   270d));
            positions.Add(new RotarySwitchPosition(this, 2, "100 KHz", 290d));
            positions.Add(new RotarySwitchPosition(this, 3, "200 KHz", 310d));
            positions.Add(new RotarySwitchPosition(this, 4, "300 KHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 5, "400 KHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 6, "500 KHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 7, "600 KHz", 30d));
            positions.Add(new RotarySwitchPosition(this, 8, "700 KHz", 50d));
            positions.Add(new RotarySwitchPosition(this, 9, "800 KHz", 70d));
            positions.Add(new RotarySwitchPosition(this, 10, "900 KHz", 90d));
            AddRotarySwitch("100 KHz Selector", new Point(388, 329), new Size(70, 70), $"{_imageLocation}ARC-210_Frequency_Knob.png", 1, positions, "100 KHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 KHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 2, "25 KHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 3, "50 KHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 4, "75 KHz", 30d));
            AddRotarySwitch("25 KHz Selector", new Point(494, 329), new Size(70, 70), $"{_imageLocation}ARC-210_Frequency_Knob.png", 1, positions, "25 KHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "OFF", 330d));
            positions.Add(new RotarySwitchPosition(this, 2, "ON", 30d));
            AddRotarySwitch("Squelch on/off", new Point(506, 74), new Size(70, 70), $"{_imageLocation}ARC-210_Squelch_Knob.png", 1, positions, "Squelch on/off");
            UseTextualDisplays = false;
        }

        public override string DefaultBackgroundImage => _imageLocation + "ARC-210_Faceplate.png";

        public bool UseTextualDisplays
        {
            get => _useTextualDisplays;
            set
            {
                if (value != _useTextualDisplays)
                {
                    _useTextualDisplays = value;
                    _displayBackground.IsHidden = !_useTextualDisplays;
                    foreach(TextDisplay td in _textDisplayList)
                    {
                        td.IsHidden = !_useTextualDisplays;
                    }
                    ViewportName = _useTextualDisplays ? "" : "A_10C_2_ARC210_SCREEN";
                    Refresh();
                }
            }
        }

        private void AddPushButton(string name, double x, double y, Size size, string buttonImage, string interfaceElement)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: buttonImage,
                pushedImage: buttonImage.Replace("_Up.", "_Down."),
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElement,
                fromCenter: false
                );
        }
        private RotarySwitch AddRotarySwitch(string name, Point posn, Size size, string knobImage, int defaultPosition, RotarySwitchPositionCollection positions, string interfaceElementName)
        {
            RotarySwitch newSwitch = new RotarySwitch
            {
                Name = Name + "_" + name,
                KnobImage = knobImage,
                DrawLabels = false,
                DrawLines = false,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = defaultPosition,
                IsContinuous = false
            };
            newSwitch.Positions.Clear();
            foreach (RotarySwitchPosition swPosn in positions)
            {
                newSwitch.Positions.Add(swPosn);
            }

            AddRotarySwitchBindings(name, posn, size, newSwitch, _interfaceDeviceName, interfaceElementName);
            return newSwitch;
        }
        private TextDisplay AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, 
                TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            TextDisplay display = AddTextDisplay(
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
            display.IsHidden = !_useTextualDisplays;
            return display; 
        }

        private ImageDecoration AddImage(string name, Point posn, Size size, string imageName)
        {
            ImageDecoration image = new ImageDecoration()
            {
                Name = name,
                Left = posn.X,
                Top = posn.Y,
                Width = size.Width,
                Height = size.Height,
                Alignment = ImageAlignment.Stretched,
                Image = imageName,
                IsHidden = !_useTextualDisplays

            };
            Children.Add(image);
            return image;
        }

        public string ViewportName
        {
            get => _vpName;
            set
            {
                if (_vpName != value)
                {
                    if (_vpName == "")
                    {
                        AddViewport(value);
                        OnDisplayUpdate();
                    }
                    else if (value != "")
                    {
                        foreach (HeliosVisual visual in this.Children)
                        {
                            if (visual.TypeIdentifier == "Helios.Base.ViewportExtent")
                            {
                                Controls.Special.ViewportExtent viewportExtent = visual as Controls.Special.ViewportExtent;
                                viewportExtent.ViewportName = value;
                                break;
                            }
                        }
                    }
                    else
                    {
                        RemoveViewport(value);
                    }
                    OnPropertyChanged("ViewportName", _vpName, value, false);
                    _vpName = value;
                }
            }
        }

        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(147, 90, 299, 195);
            vpRect.Scale(Width / NativeSize.Width, Height / NativeSize.Height);
            TextFormat tf = new TextFormat()
            {
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Normal,
                FontSize = 2,
                FontFamily = ConfigManager.FontManager.GetFontFamilyByName("Franklin Gothic"),
                ConfiguredFontSize = 2,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                VerticalAlignment = TextVerticalAlignment.Center
            };

            Children.Add(new Helios.Controls.Special.ViewportExtent
            {
                FillBackground = true,
                BackgroundColor = Color.FromArgb(0x80,0x20,0xBA, 0xA3),
                FontColor = Color.FromArgb(255, 255, 255, 255),
                TextFormat = tf, 
                ViewportName = name,
                Left = vpRect.Left,
                Top = vpRect.Top,
                Width = vpRect.Width,
                Height = vpRect.Height,
                RequiresPatches = true,
            });
        }

        private void RemoveViewport(string name)
        {
            foreach (HeliosVisual visual in this.Children)
            {
                if (visual.TypeIdentifier == "Helios.Base.ViewportExtent")
                {
                    Children.Remove(visual);
                    break;
                }
            }
        }
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name.Equals("UseTextualDisplays"))
            {
                UseTextualDisplays = bool.Parse(reader.ReadElementString("UseTextualDisplays"));
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("UseTextualDisplays", _useTextualDisplays.ToString(CultureInfo.InvariantCulture));
        }

        public override bool HitTest(Point location) => false;
    }
}