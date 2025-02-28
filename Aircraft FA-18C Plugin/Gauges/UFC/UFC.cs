//  Copyright 2019 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.Gauges.FA18C;
    using GadrocsWorkshop.Helios.Gauges;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using System.IO;
    using System.Xml;
    using System.Globalization;
    using System.ComponentModel;

    [HeliosControl("Helios.FA18C.UFC", "Up Front Controller", "F/A-18C", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class UFC_FA18C : FA18CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "UFC";
        private string _font = "MS 33558";
        private string _ufcNumbers16 = "`0=«;`1=¬;`2=Ð;`3=®;`4=¯;`5=°;`6=±;`7=²;`8=³;`9=´;~0=µ;0=¡;1=¢;2=£;3=¤;4=¥;5=¦;6=§;7=¨;8=©;9=ª;_=É;!=È"; //Numeric mapping into characters in the UFC font
        private string _ufcNumbers16Tens = "`0=«;`1=¬;`2=Ð;`3=®;`4=¯;`5=°;`6=±;`7=²;`8=³;`9=´;~0=µ;a=Ñ;b=Ñ;c=Ñ;`=Ò;2=Ó;~=Ó;3=Ô;e=Ô;f=Ô;g=Ô;4=Õ;h=Õ;i=Õ;j=Õ;5=Ö;k=Ö;6=×;l=×;7=Ø;m=Ø;n=Ø;o=Ø;8=Ù;q=Ù;s=Ù;9=Ú;t=Ú;u=Ú;v=Ú;_=É;!=È"; //Numeric mapping into characters in the UFC font
        private string _ufcCueing = "!=È;|=È";
        private HeliosValue _alternateImages;
        private string _altImageLocation = "Alt";
        private bool _enableAlternateImageSet = false;
        private string _defaultBackgroundImage = "{FA-18C}/Images/UFC Faceplate.png";

        private Color _onTextColor = Color.FromArgb(0xff, 0x24, 0x8D, 0x22);
        private Color _offTextColor = Color.FromArgb(0xff, 0x1C, 0x1C, 0x1C);
        private Color _onTextDisplayColor = Color.FromArgb(0xff, 0x7e, 0xde, 0x72);
        private Color _offTextDisplayColor = Color.FromArgb(0x00, 0x26, 0x3f, 0x36);

        public UFC_FA18C()
            : base("UFC", new Size(602, 470))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };

            _alternateImages = new HeliosValue(this, new BindingValue(false), "", "Enable Alternate Image Set", "Indicates whether the alternate image set is to be used", "True or False", BindingValueUnits.Boolean);
            _alternateImages.Execute += new HeliosActionHandler(EnableAltImages_Execute);
            Actions.Add(_alternateImages);

            AddDefaultInputBinding(
                    childName: "",
                    deviceActionName: "set.Enable Alternate Image Set",
                    interfaceTriggerName: "Cockpit Lights.MODE Switch.changed",
                    deviceTriggerName: "",
                    triggerBindingValue: new BindingValue("return TriggerValue<3"),
                    triggerBindingSource: BindingValueSources.LuaScript
                    );

            AddButton("EMCON", 527, 129, new Size(48, 48), "UFC Emission Control Pushbutton");
            AddButton("1", 105, 116, new Size(48, 48), "UFC Keyboard Pushbutton 1");
            AddButton("2", 167, 116, new Size(48, 48), "UFC Keyboard Pushbutton 2");
            AddButton("3", 229, 116, new Size(48, 48), "UFC Keyboard Pushbutton 3");
            AddButton("4", 105, 179, new Size(48, 48), "UFC Keyboard Pushbutton 4");
            AddButton("5", 167, 179, new Size(48, 48), "UFC Keyboard Pushbutton 5");
            AddButton("6", 229, 179, new Size(48, 48), "UFC Keyboard Pushbutton 6");
            AddButton("7", 105, 240, new Size(48, 48), "UFC Keyboard Pushbutton 7");
            AddButton("8", 167, 240, new Size(48, 48), "UFC Keyboard Pushbutton 8");
            AddButton("9", 229, 240, new Size(48, 48), "UFC Keyboard Pushbutton 9");
            AddButton("CLR", 105, 303, new Size(48, 48), "UFC Keyboard Pushbutton CLR");
            AddButton("0", 167, 303, new Size(48, 48), "UFC Keyboard Pushbutton 0");
            AddButton("ENT", 229, 303, new Size(48, 48), "UFC Keyboard Pushbutton ENT");
            AddButton("AP", 125, 400, new Size(40, 40), "UFC Function Selector Pushbutton A/P");
            AddButton("IFF", 176, 400, new Size(40, 40), "UFC Function Selector Pushbutton IFF");
            AddButton("TCN", 229, 400, new Size(40, 40), "UFC Function Selector Pushbutton TCN");
            AddButton("ILS", 284, 400, new Size(40, 40), "UFC Function Selector Pushbutton ILS");
            AddButton("DL", 337, 400, new Size(40, 40), "UFC Function Selector Pushbutton D/L");
            AddButton("BCN", 393, 400, new Size(40, 40), "UFC Function Selector Pushbutton BCN");
            AddButton("ONOFF", 447, 400, new Size(40, 40), "UFC Function Selector Pushbutton ON/OFF");
            AddButtonIP("IP", 28, 60, new Size(40, 40), "UFC I/P Pushbutton");
            AddButtonIP("ODU 1", 302, 42, new Size(40, 40), "UFC Option Select Pushbutton 1");
            AddButtonIP("ODU 2", 302, 107, new Size(40, 40), "UFC Option Select Pushbutton 2");
            AddButtonIP("ODU 3", 302, 175, new Size(40, 40), "UFC Option Select Pushbutton 3");
            AddButtonIP("ODU 4", 302, 241, new Size(40, 40), "UFC Option Select Pushbutton 4");
            AddButtonIP("ODU 5", 302, 310, new Size(40, 40), "UFC Option Select Pushbutton 5");
            AddThreeWayToggle("ADF", 33, 122, new Size(30, 60), "UFC ADF Function Select Switch");

            AddPot("Display Brightness", new Point(528, 66), new Size(48, 48), "UFC Brightness Control Knob");
            AddPot("Radio Volume 1", new Point(25, 213), new Size(48, 48), "UFC COMM 1 Volume Control Knob");
            AddPot("Radio Volume 2", new Point(528, 213), new Size(48, 48), "UFC COMM 2 Volume Control Knob");
            AddEncoder("Radio 1", new Point(29, 383), new Size(75, 75), "UFC COMM 1 Channel Selector Knob");
            AddButtonIP("Radio 1 Pull", 52, 408, new Size(28, 28), "UFC COMM 1 Channel Selector Pull", false);
            AddEncoder("Radio 2", new Point(500, 383), new Size(75, 75), "UFC COMM 2 Channel Selector Knob");
            AddButtonIP("Radio 2 Pull", 523, 408, new Size(28, 28), "UFC COMM 2 Channel Selector Pull", false);

            /// adding the displays
            AddTextDisplay("OptionCueing1", 358, 45, new Size(40, 42), "Option Display 1 Selected", 32, "~", TextHorizontalAlignment.Left, _ufcCueing);
            AddTextDisplay("OptionDisplay1", 381, 45, new Size(129, 42), "Option Display 1", 32, "~", TextHorizontalAlignment.Left, _ufcNumbers16);
            AddTextDisplay("OptionCueing2", 358, 111, new Size(40, 42), "Option Display 2 Selected", 32, "~", TextHorizontalAlignment.Left, _ufcCueing);
            AddTextDisplay("OptionDisplay2", 381, 111, new Size(129, 42), "Option Display 2",32, "~", TextHorizontalAlignment.Left, _ufcNumbers16);
            AddTextDisplay("OptionCueing3", 358, 177, new Size(40, 42), "Option Display 3 Selected", 32, "~", TextHorizontalAlignment.Left, _ufcCueing);
            AddTextDisplay("OptionDisplay3", 381, 177, new Size(129, 42), "Option Display 3", 32, "~", TextHorizontalAlignment.Left, _ufcNumbers16);
            AddTextDisplay("OptionCueing4", 358, 244, new Size(40, 42), "Option Display 4 Selected", 32, "~", TextHorizontalAlignment.Left, _ufcCueing);
            AddTextDisplay("OptionDisplay4", 381, 244, new Size(129,42), "Option Display 4", 32, "~", TextHorizontalAlignment.Left, _ufcNumbers16);
            AddTextDisplay("OptionCueing5", 358, 310, new Size(40, 42), "Option Display 5 Selected", 32, "~", TextHorizontalAlignment.Left, _ufcCueing);
            AddTextDisplay("OptionDisplay5", 381, 310, new Size(129,42), "Option Display 5", 32, "~", TextHorizontalAlignment.Left, _ufcNumbers16);
            ///
            /// The following overlaying of textdisplays is needed because double digits up to 99 can be sent to a single 16 segment display element
            /// when precise coordinates are being entered.  The font has numerals which are left aligned and right aligned (handled by different 
            /// textdisplay dictionaries).  The Tens and Units are split out in their respective UFCTextDsplays.
            /// Both UFCTextDisplays are bound to the same network value.
            /// 
            AddUFCTextDisplay("ScratchPadCharacter1", 92, 35, new Size(32, 48), "Scratchpad 1", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16,1,1);
            AddUFCTextDisplay("ScratchPadCharacter1a", 92, 35, new Size(32, 48), "Scratchpad 1", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16Tens,0,1);
            AddUFCTextDisplay("ScratchPadCharacter2", 122, 35, new Size(32, 48), "Scratchpad 2", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16,1,1);
            AddUFCTextDisplay("ScratchPadCharacter2a", 122, 35, new Size(32, 48), "Scratchpad 2", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16Tens,0,1);
            AddTextDisplay("ScratchPadNumbers", 152, 35, new Size(135, 48), "Scratchpad Number", 30, "~", TextHorizontalAlignment.Right, " =>");
            AddTextDisplay("Comm1", 26, 314, new Size(41, 42), "Comm Channel 1",32, "~", TextHorizontalAlignment.Center, " =;" + _ufcNumbers16);
            AddTextDisplay("Comm2", 538, 309, new Size(40, 42), "Comm Channel 2",32, "~", TextHorizontalAlignment.Center, " =;" + _ufcNumbers16);
        }

        public override string DefaultBackgroundImage
        {
            get => _defaultBackgroundImage;
        }

        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            AddPot(name: name, 
                posn: posn, 
                size: size, 
                knobImage: "{FA-18C}/Images/Common Knob.png", 
                initialRotation: 219, 
                rotationTravel: 291, 
                minValue: 0, 
                maxValue: 1, 
                initialValue: 0, 
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false);
        }

        private void AddEncoder(string name, Point posn, Size size, string interfaceElementName)
        {
            AddEncoder(
                name: name,
                size: size,
                posn: posn,
                knobImage: "{FA-18C}/Images/UFC Rotator_U.png",
                stepValue: 0.1,
                rotationStep: 5,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        private void AddTextDisplay(string name, double x, double y, Size size,
            string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string ufcDictionary)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: new Point(x, y),
                size: size,
                font: "Helios Virtual Cockpit F/A-18C_Hornet-Up_Front_Controller",
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: _onTextDisplayColor,
                backgroundColor: _offTextDisplayColor,
                useBackground: false,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: ufcDictionary
                );
        }
        private void AddUFCTextDisplay(string name, double x, double y, Size size,
            string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string ufcDictionary,int textIndex,int textLength)
        {
            string componentName = GetComponentName(name);
            UFCTextDisplay display = new UFCTextDisplay
            {
                TextIndex = textIndex,
                TextLength = textLength,
                Top = y,
                Left = x,
                Width = size.Width,
                Height = size.Height,
                Name = componentName
            };
            TextFormat textFormat = new TextFormat
            {
                FontFamily = ConfigManager.FontManager.GetFontFamilyByName("Helios Virtual Cockpit F/A-18C_Hornet-Up_Front_Controller"),
                HorizontalAlignment = hTextAlign,
                VerticalAlignment = TextVerticalAlignment.Center,
                FontSize = baseFontsize,
                ConfiguredFontSize = baseFontsize,
                PaddingRight = 0,
                PaddingLeft = 0,
                PaddingTop = 0,
                PaddingBottom = 0
            };

            // NOTE: for scaling purposes, we commit to the reference height at the time we set TextFormat, since that indirectly sets ConfiguredFontSize 
            display.TextFormat = textFormat;
            display.OnTextColor = _onTextDisplayColor;
            display.BackgroundColor = _offTextDisplayColor;
            display.UseBackground = false;

            if (ufcDictionary.Equals(""))
            {
                display.ParserDictionary = "";
            }
            else
            {
                display.ParserDictionary = ufcDictionary;
                display.UseParseDictionary = true;
            }
            display.TextTestValue = testDisp;
            Children.Add(display);
            AddAction(display.Actions["set.TextDisplay"], componentName);

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: _interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.TextDisplay");
        }


        private void AddButton(string name, double x, double y, Size size, string interfaceElementName)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: "{FA-18C}/Images/UFC Button Up " + name + ".png",
                pushedImage: "{FA-18C}/Images/UFC Button Dn " + name + ".png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        private void AddButtonIP(string name, double x, double y, Size size, string interfaceElementName)
        { AddButtonIP(name, x, y, size, interfaceElementName, true); }
        private void AddButtonIP(string name, double x, double y, Size size, string interfaceElementName, Boolean glyph)
        {
            Point pos = new Point(x, y);
            PushButton button = AddButton(
                name: name,
                posn: pos,
                size: size,
                image: "{Helios}/Images/Buttons/tactile-dark-round.png",
                pushedImage: "{Helios}/Images/Buttons/tactile-dark-round-in.png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );

            if (glyph)
            {
                button.Glyph = PushButtonGlyph.Circle;
                button.GlyphThickness = 3;
                button.GlyphColor = Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0);
            }

        }
        private void AddIndicator(string name, double x, double y, Size size) { AddIndicator(name, x, y, size, false); }
        private void AddIndicator(string name, double x, double y, Size size, bool _vertical)
        {
            Helios.Controls.Indicator indicator = AddIndicator(
                name: name,
                posn: new Point(x, y),
                size: size,
                onImage: "{Helios}/Images/Indicators/anunciator.png",
                offImage: "{Helios}/Images/Indicators/anunciator.png",
                onTextColor: _onTextColor,
                offTextColor: _offTextColor,
                font: _font,
                vertical: _vertical,
                interfaceDeviceName: "",
                interfaceElementName: "",
                fromCenter: false
                );
        }


        private void AddThreeWayToggle(string name, double x, double y, Size size, string interfaceElementName)
        {

            AddThreeWayToggle(
                name: name,
                posn: new Point(x, y),
                size: size,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.OnOnOn,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        void EnableAltImages_Execute(object sender, HeliosActionEventArgs e)
        {
            EnableAlternateImageSet = e.Value.BoolValue;
            _alternateImages.SetValue(e.Value, e.BypassCascadingTriggers);
        }

        public bool EnableAlternateImageSet
        {
            get => _enableAlternateImageSet;
            set
            {
                bool newValue = value;
                bool oldValue = _enableAlternateImageSet;

                if (newValue != oldValue)
                {
                    _enableAlternateImageSet = newValue;

                    foreach (HeliosVisual hv in this.Children)
                    {
                        if (hv is TextDisplay txtDisplay)
                        {
                            //txtDisplay.OnTextColor = newValue ? Color.FromArgb(0xff, 0x00, 0xc0, 0x00) : Color.FromArgb(0xff, 0xc0, 0xc0, 0xc0);
                            continue;
                        }
                        if (hv is PushButton pb)
                        {
                            pb.Image = ImageSwitchName(pb.Image);
                            pb.PushedImage = ImageSwitchName(pb.PushedImage);
                            pb.GlyphColor = newValue ? Color.FromArgb(0xf0, 0x00, 0xc0, 0x00) : Color.FromArgb(0xf0, 0xc0, 0xc0, 0xc0);
                            continue;
                        }
                        if (hv is ThreeWayToggleSwitch sw)
                        {
                            sw.PositionOneImage = ImageSwitchName(sw.PositionOneImage);
                            sw.PositionTwoImage = ImageSwitchName(sw.PositionTwoImage);
                            sw.PositionThreeImage = ImageSwitchName(sw.PositionThreeImage);
                            continue;
                        }
                        if (hv is Indicator ind)
                        {
                            ind.OnImage = ImageSwitchName(ind.OnImage);
                            ind.OffImage = ImageSwitchName(ind.OffImage);
                            continue;
                        }
                        if (hv is Potentiometer pot)
                        {
                            pot.KnobImage = ImageSwitchName(pot.KnobImage);
                            continue;
                        }
                        if (hv is RotaryEncoder enc)
                        {
                            enc.KnobImage = ImageSwitchName(enc.KnobImage);
                            continue;
                        }
                    }

                    BackgroundImage = ImageSwitchName(BackgroundImage);
                    // notify change after change is made
                    OnPropertyChanged("EnableAlternateImageSet", oldValue, newValue, true);
                }
            }
        }

        private string ImageSwitchName(string imageName)
        {
            string imageSubfolder = _enableAlternateImageSet ? $"/{_altImageLocation}" : "";

            string dir = Path.GetDirectoryName(imageName);
            if (new DirectoryInfo(dir).Name == _altImageLocation)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return $"{dir}{imageSubfolder}/{Path.GetFileName(imageName)}";
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
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            //if (_IFEI_gauges.GlassReflectionOpacity != IFEI_Gauges.GLASS_REFLECTION_OPACITY_DEFAULT)
            //{
            //    writer.WriteElementString("GlassReflectionOpacity", GlassReflectionOpacity.ToString(CultureInfo.InvariantCulture));
            //}
            if (EnableAlternateImageSet) writer.WriteElementString("EnableAlternateImageSet", EnableAlternateImageSet.ToString(CultureInfo.InvariantCulture));

        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            base.ReadXml(reader);
            //if (reader.Name.Equals("GlassReflectionOpacity"))
            //{
            //    GlassReflectionOpacity = double.Parse(reader.ReadElementString("GlassReflectionOpacity"), CultureInfo.InvariantCulture);
            //}
            if (reader.Name.Equals("EnableAlternateImageSet"))
            {
                bool enableAlternateImageSet = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("EnableAlternateImageSet"));
                //_textColor = enableAlternateImageSet ? Color.FromArgb(0xff, 0, 220, 0) : Color.FromArgb(0xff, 220, 220, 220);
                EnableAlternateImageSet = enableAlternateImageSet;
            }
            else
            {
                //_textColor = Color.FromArgb(0xff, 220, 220, 220);
                EnableAlternateImageSet = false;
            }
        }

    }
}
