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

/// <summary>
/// This is the revised version of the Up Front Controller which is larger and uses text displays to avoid cutouts for the exported viewport.
/// It has a slightly different name because the old version is retained to help with backward compatability
/// </summary>
/// 
namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Threading;


    [HeliosControl("Helios.AV8B.UFC1", "Up Front Controller", "AV-8B Gauges", typeof(BackgroundImageRenderer))]
    class UFC_1: AV8BDevice
    {
        // these three sections are the dead space in the UFC image.
        private static readonly Rect SCREEN_RECT_L = new Rect(0, 135, 38, 415);
        private Rect _scaledScreenRectL = SCREEN_RECT_L;
        private static readonly Rect SCREEN_RECT_LB = new Rect(38, 476, 103, 74);
        private Rect _scaledScreenRectLB = SCREEN_RECT_LB;
        private static readonly Rect SCREEN_RECT_R = new Rect(743, 102, 65, 448);
        private Rect _scaledScreenRectR = SCREEN_RECT_R;
        private static readonly Rect SCREEN_RECT_BL = new Rect(0, 792, 150, 177);
        private Rect _scaledScreenRectBL = SCREEN_RECT_BL;
        private static readonly Rect SCREEN_RECT_BR = new Rect(818, 792, 150, 177);
        private Rect _scaledScreenRectBR = SCREEN_RECT_BR;
        private static readonly Rect SCREEN_RECT_BM = new Rect(236, 889, 500, 81);
        private Rect _scaledScreenRectBM = SCREEN_RECT_BM;
        private String _font = "MS 33558";
        private string _interfaceDeviceName = "UFC";
        private string _ufcNumbers16 = "`0=«;`1=¬;`2=­;`3=®;`4=¯;`5=°;`6=±;`7=²;`8=³;`9=´;~0=µ;0=¡;1=¢;2=£;3=¤;4=¥;5=¦;6=§;7=¨;8=©;9=ª;_=É"; //Numeric mapping into characters in the UFC font
        private string _ufcNumbers7 = "_=É;.=<"; //Numeric mapping into characters in the UFC font
        private string _imageLocation = "{AV-8B}/Images/";

        public UFC_1()
            : base("UFC", new Size(968, 970))
        {
            AddButton("TMR", 228, 76, new Size(64, 64), "UFC Function Selector Pushbutton TMR");
            AddButton("TOO", 228, 168, new Size(64, 64), "UFC Function Selector Pushbutton TOO");
            AddButton("1", 321, 168, new Size(64, 64), "UFC Keypad Pushbutton 1");
            AddButton("2", 410, 168, new Size(64, 64), "UFC Keypad Pushbutton 2");
            AddButton("3", 497, 168, new Size(64, 64), "UFC Keypad Pushbutton 3");
            AddButton("CLR", 588, 168, new Size(64, 64), "UFC Keypad Pushbutton CLR");
            AddButton("4", 321, 253, new Size(64, 64), "UFC Keypad Pushbutton 4");
            AddButton("5", 410, 253, new Size(64, 64), "UFC Keypad Pushbutton 5");
            AddButton("6", 497, 253, new Size(64, 64), "UFC Keypad Pushbutton 6");
            AddButton("SVE", 588, 253, new Size(64, 64), "UFC Keypad Pushbutton SVE");
            AddButton("7", 321, 339, new Size(64, 64), "UFC Keypad Pushbutton 7");
            AddButton("8", 410, 339, new Size(64, 64), "UFC Keypad Pushbutton 8");
            AddButton("9", 497, 339, new Size(64, 64), "UFC Keypad Pushbutton 9");
            AddButton("Dash", 588, 339, new Size(64, 64), "UFC Keypad Pushbutton -");
            AddButton("ENT", 321, 426, new Size(64, 64), "UFC Keypad Pushbutton ENT");
            AddButton("0", 410, 426, new Size(64, 64), "UFC Keypad Pushbutton 0");
            AddButton("Dot", 497, 426, new Size(64, 64), "UFC Keypad Pushbutton .");
            AddButton("ONOFF", 588, 426, new Size(64, 64), "UFC Function Selector Pushbutton ON/OFF");
            AddButton("IFF", 321, 512, new Size(64, 64), "UFC Function Selector Pushbutton IFF");
            AddButton("TCN", 410, 512, new Size(64, 64), "UFC Function Selector Pushbutton TCN");
            AddButton("AWL", 497, 512, new Size(64, 64), "UFC Function Selector Pushbutton AWL");
            AddButton("WPN", 588, 512, new Size(64, 64), "UFC Function Selector Pushbutton WPN");
            AddButton("WOF", 321, 598, new Size(64, 64), "UFC Function Selector Pushbutton WOF");
            AddButton("BCN", 410, 598, new Size(64, 64), "UFC Function Selector Pushbutton BCN");
            AddButton("ALT", 497, 598, new Size(64, 64), "UFC Function Selector Pushbutton ALT");
            AddButton("EMCON", 588, 598, new Size(64, 64), "UFC Emission Control Pushbutton");
            AddButtonIP("IP", 685, 172, new Size(54, 54), "UFC I/P Pushbutton");

            _interfaceDeviceName = "Caution Indicators";
            AddIndicator("15 SEC", 40, 302, new Size(68, 36), "Caution 15 Sec Indicator");
            AddIndicator("MFS", 40, 350, new Size(68, 36), "Caution MFS");
            AddIndicator("BINGO", 40, 401, new Size(68, 36), "Caution Bingo");
            AddIndicator("H2O", 40, 451, new Size(68, 36), "Caution H2O");
            AddIndicator("Unknown 1", 40, 503, new Size(68, 36), "Caution Unknown 1");
            AddIndicator("Unknown 2", 40, 554, new Size(68, 36), "Caution Unknown 2");
            AddIndicator("L FUEL", 42, 199, new Size(36, 88), "Caution L Fuel Indicator");
            AddIndicator("R FUEL", 100, 199, new Size(36, 88), "Caution R fuel Indicator");

            _interfaceDeviceName = "Warning Indicators";
            AddIndicator("FIRE", 831, 149, new Size(68, 36), "Warning Fire");
            AddIndicator("LAW", 831, 199, new Size(68, 36), "Warning LAW");
            AddIndicator("FLAPS", 831, 252, new Size(68, 36), "Warning Flaps");
            AddIndicator("L TANK", 831, 302, new Size(68, 36), true, "Warning L Tank");  
            AddIndicator("R TANK", 831, 350, new Size(68, 36), true, "Warning R Tank");  
            AddIndicator("HYD", 831, 401, new Size(68, 36), "Warning HYD");
            AddIndicator("GEAR", 831, 451, new Size(68, 36), "Warning Gear");
            AddIndicator("OT", 831, 503, new Size(68, 36), "Warning OT");
            AddIndicator("JPTL", 831, 554, new Size(68, 36), "Warning JPTL");
            AddIndicator("EFC", 831, 606, new Size(68, 36), "Warning EFC");
            AddIndicator("GEN", 831, 658, new Size(68, 36), "Warning GEN");

            _interfaceDeviceName = "Caution/Warning";
            AddCautionWarningIndicator("MASTER CAUTION", 9, 20, new Size(128, 98), "Master Caution Indicator");
            AddCautionWarningIndicator("MASTER WARNING", 831, 20, new Size(128, 98), "Master Warning Indicator");
            AddButton("MASTER CAUTION button", 9, 20, new Size(128, 98), "Caution/Warning", "Master Caution Button");
            AddButton("MASTER WARNING button", 831, 20, new Size(128, 98), "Caution/Warning", "Master Warning Button");

            _interfaceDeviceName = "UFC";
            AddPot("UFC Display Brightness", new Point(675, 67), new Size(64, 64), "UFC Brightness Control Knob");
            AddPot("Radio Volume 1", new Point(214, 270), new Size(64, 64), "UFC COMM 1 Volume Control Knob");
            AddPot("Radio Volume 2", new Point(699, 270), new Size(64, 64), "UFC COMM 2 Volume Control Knob");
            AddEncoder("Radio 1", new Point(179, 490), new Size(132, 132), "UFC COMM 1 Channel Selector Knob");
            AddEncoder("Radio 2", new Point(661, 490), new Size(132, 132), "UFC COMM 2 Channel Selector Knob");
            AddButton("Comm 1", 215, 526, new Size(60, 60), "UFC", "UFC COMM 1 Channel Selector Pull");
            AddButton("Comm 2", 697, 526, new Size(60, 60), "UFC", "UFC COMM 2 Channel Selector Pull");

            
            AddTextDisplay("Left Scratchpad", 329, 53, new Size(120, 60), "Left Scratchpad Data", 40, "XX", TextHorizontalAlignment.Left, _ufcNumbers16);
            AddTextDisplay("Scratchpad", 350, 53, new Size(280,60), "Scratchpad Number", 40, "1234567", TextHorizontalAlignment.Right, _ufcNumbers7);
            AddTextDisplay("Comm 1 Display", 187,367, new Size(120, 60), "Comm Channel 1", 40, "~~", TextHorizontalAlignment.Center, _ufcNumbers16);
            AddTextDisplay("Comm 2 Display", 669,367, new Size(120, 60), "Comm Channel 2", 40, "~~", TextHorizontalAlignment.Center, _ufcNumbers16);

            _interfaceDeviceName = "HUD Control";
            AddPot("HUD Brightness", new Point(297, 779), new Size(64, 64), "Off/Brightness control");
            AddPot("Video Brightness", new Point(516, 779), new Size(64, 64), "Video Brightness");
            AddPot("Video Contrast", new Point(625, 779), new Size(64, 64), "Video Contrast");
            AddThreeWayToggle("Declutter", 194, 761, new Size(50, 100), "Declutter switch");
            AddThreeWayToggle("Display Mode", 413, 761, new Size(50, 100), "Display Mode switch");
            AddTwoWayToggle("Altimeter Switch", 719, 761, new Size(50, 100), "Altimeter Mode Switch");
        }

        public override string DefaultBackgroundImage
        {
            get { return "{AV-8B}/Images/WQHD/Panel/UFC Large.png"; }
        }

        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            AddPot(name: name,
                posn: posn,
                size: size,
                knobImage: "{AV-8B}/Images/Common Knob.png",
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
                knobImage: "{AV-8B}/Images/AV8BNA_Rotary5.png",
                stepValue: 0.005,
                rotationStep: 10,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        //private void AddTextDisplay(string name, double x, double y, Size size, string testDisp)
        //{
        //    AddTextDisplay(name, x, y, size, 32, testDisp, TextHorizontalAlignment.Left);
        //}

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
                backgroundColor: Color.FromArgb(0xff, 0x26, 0x3f, 0x36),
                useBackground: false,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: ufcDictionary
                );
        }

        private void AddButton(string name, double x, double y, Size size, string interfaceElementName)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: "{AV-8B}/Images/UFC Button Up " + name + ".png",
                pushedImage: "{AV-8B}/Images/UFC Button Dn " + name + ".png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }
        private void AddButton(string name, double x, double y, Size size, string interfaceDeviceName, string interfaceElementName)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: "{AV-8B}/Images/_transparent.png",
                pushedImage: "{AV-8B}/Images/_transparent.png",
                buttonText: "",
                interfaceDeviceName: interfaceDeviceName,
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
        private void AddIndicator(string name, double x, double y, Size size, string interfaceElementName) { AddIndicator(name, x, y, size, false, interfaceElementName); }
        private void AddIndicator(string name, double x, double y, Size size, bool _vertical, string interfaceElementName)
        {
            Indicator indicator = AddIndicator(
                name: name,
                posn: new Point(x, y),
                size: size,
                onImage: _imageLocation + "Caution " + name + ".png",
                offImage: _imageLocation + "_transparent.png",
                onTextColor: Color.FromArgb(0x00, 0xff, 0xff, 0xff),
                offTextColor: Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                font: _font,
                vertical: _vertical,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
                indicator.Text = "";
                indicator.Name = "UFC_" + name;
        }

        private void AddCautionWarningIndicator(string name, double x, double y, Size size, string interfaceElementName)
        {
            Color onTextColor;
            if (name == "MASTER WARNING")
            {
                onTextColor = Color.FromArgb(0x00, 0xc7, 0x1e, 0x1e);
            }
            else
            {
                onTextColor = Color.FromArgb(0x00, 0xb3, 0xa2, 0x29);
            }

            Indicator indicator = AddIndicator(
                name: name,
                posn: new Point(x, y),
                size: size,
                onImage: "{AV-8B}/Images/Caution " + name + ".png",
                offImage: "{AV-8B}/Images/_transparent.png",
                offTextColor: Color.FromArgb(0x00, 0x1C, 0x1C, 0x1C),
                onTextColor: onTextColor,
                font: _font,
                vertical: false,
                interfaceDeviceName: "Caution/Warning",
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            indicator.Text = name;
            indicator.Name = "UFC_" + name;
            if (name == "MASTER WARNING")
            {
                indicator.OnTextColor = Color.FromArgb(0x00, 0xc7, 0x1e, 0x1e);
            }
            else
            {
                indicator.OnTextColor = Color.FromArgb(0x00, 0xb3, 0xa2, 0x29);
            }
            indicator.TextFormat.FontStyle = FontStyles.Normal;
            indicator.TextFormat.FontWeight = FontWeights.Normal;
            indicator.TextFormat.FontSize = 18;
            indicator.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName(_font);
            indicator.TextFormat.PaddingLeft = 0;
            indicator.TextFormat.PaddingRight = 0;
            indicator.TextFormat.PaddingTop = 0;
            indicator.TextFormat.PaddingBottom = 0;
            indicator.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
            indicator.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
        }


        private void AddTwoWayToggle(string name, double x, double y, Size size, string interfaceElementName)
        {
            ToggleSwitch toggle = AddToggleSwitch(
                name: name,
                posn: new Point(x, y),
                size: size,
                defaultPosition: ToggleSwitchPosition.One,
                defaultType: ToggleSwitchType.OnOn,
                positionOneImage: "{Helios}/Images/Toggles/round-up.png",
                positionTwoImage: "{Helios}/Images/Toggles/round-down.png",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                clickType: LinearClickType.Swipe,
                fromCenter: false
                );
            toggle.Name = "UFC_" + name;
        }

        private void AddThreeWayToggle(string name, double x, double y, Size size, string interfaceElementName)
        {
            ThreeWayToggleSwitch toggle = AddThreeWayToggle(
                name: name,
                posn: new Point(x, y),
                size: size,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.OnOnOn,
                positionOneImage: "{Helios}/Images/Toggles/round-up.png",
                positionTwoImage: "{Helios}/Images/Toggles/round-norm.png",
                positionThreeImage: "{Helios}/Images/Toggles/round-down.png",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            toggle.Name = "UFC_" + name;
        }


        public override bool HitTest(Point location)
        {
            if (_scaledScreenRectL.Contains(location) || _scaledScreenRectLB.Contains(location) || _scaledScreenRectR.Contains(location) || _scaledScreenRectBL.Contains(location) || _scaledScreenRectBR.Contains(location) || _scaledScreenRectBM.Contains(location))
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