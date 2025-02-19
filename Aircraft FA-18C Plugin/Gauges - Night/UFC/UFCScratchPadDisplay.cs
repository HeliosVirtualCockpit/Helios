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
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;

    [HeliosControl("Helios.FA18C.UFC-Scratchpad2", "UFC Scratchpad Display - Night", "F/A-18C - Night", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class UFC_Scratchpad_FA18C2 : FA18CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "UFC";
        private string _ufcNumbers16 = "`0=«;`1=¬;`2=Ð;`3=®;`4=¯;`5=°;`6=±;`7=²;`8=³;`9=´;~0=µ;0=¡;1=¢;2=£;3=¤;4=¥;5=¦;6=§;7=¨;8=©;9=ª;_=É;!=È"; //Numeric mapping into characters in the UFC font
        private string _ufcNumbers16Tens = "`0=«;`1=¬;`2=Ð;`3=®;`4=¯;`5=°;`6=±;`7=²;`8=³;`9=´;~0=µ;a=Ñ;b=Ñ;c=Ñ;`=Ò;2=Ó;~=Ó;3=Ô;e=Ô;f=Ô;g=Ô;4=Õ;h=Õ;i=Õ;j=Õ;5=Ö;k=Ö;6=×;l=×;7=Ø;m=Ø;n=Ø;o=Ø;8=Ù;q=Ù;s=Ù;9=Ú;t=Ú;u=Ú;v=Ú;_=É;!=È"; //Numeric mapping into characters in the UFC font

        public UFC_Scratchpad_FA18C2()
            : base("UFC Scratch Pad Display", new Size(197, 48))
        {
            /// The following overlaying of textdisplays is needed because double digits up to 99 can be sent to a single 16 segment display element
            /// when precise coordinates are being entered.  The font has numerals which are left aligned and right aligned (handled by different 
            /// textdisplay dictionaries).  The Tens and Units are split out in their respective UFCTextDsplays.
            /// Both UFCTextDisplays are bound to the same network value.
            /// 
            AddUFCTextDisplay("ScratchPadCharacter1", 0, 0, new Size(32, 48), "Scratchpad 1", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16,1,1);
            AddUFCTextDisplay("ScratchPadCharacter1a", 0, 0, new Size(32, 48), "Scratchpad 1", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16Tens,0,1);
            AddUFCTextDisplay("ScratchPadCharacter2", 30, 0, new Size(32, 48), "Scratchpad 2", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16,1,1);
            AddUFCTextDisplay("ScratchPadCharacter2a", 30, 0, new Size(32, 48), "Scratchpad 2", 30, "~", TextHorizontalAlignment.Left, " =;" + _ufcNumbers16Tens,0,1);
            AddTextDisplay("ScratchPadNumbers", 62, 0, new Size(135, 48), "Scratchpad Number", 30, "8888888", TextHorizontalAlignment.Right, " =>");
        }

        public override string DefaultBackgroundImage
        {
            get { return @"{FA-18C}\Images\Scratchpad_Background.png"; }
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
                textColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72),
                backgroundColor: Color.FromArgb(0x00, 0x26, 0x3f, 0x36),
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
            display.OnTextColor = Color.FromArgb(0xff, 0x7e, 0xde, 0x72);
            display.BackgroundColor = Color.FromArgb(0x00, 0x26, 0x3f, 0x36);
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
