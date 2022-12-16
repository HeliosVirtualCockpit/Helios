//  Copyright 2019 Helios Contributors//    
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

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.Gauges.AV8B;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Threading;


    [HeliosControl("Helios.AV8B.Advisory", "Advisory Indicators", "AV-8B Gauges", typeof(BackgroundImageRenderer))]
    class AdvisoryPanel : AV8BDevice
    {
        // these three sections are the dead space in the UFC image.
        private static readonly Rect SCREEN_RECT1 = new Rect(265, 0, 290, 320);
        private static readonly Rect SCREEN_RECT2 = new Rect(401, 290, 154, 320);
        private Rect _scaledScreenRect1 = SCREEN_RECT1;
        private Rect _scaledScreenRect2 = SCREEN_RECT2;

        private String _font = "MS 33558";
        private string _interfaceDeviceName = "Advisory Indicators";

        public AdvisoryPanel()
            : base("Advisory Indicators", new Size(555, 1024))
        {
            AddIndicator("OXY", new Point(22,86), new Size(104,40), "Advisory OXY");
            AddIndicator("HYD 1", new Point(22, 146), new Size(104, 40), "Advisory HYD 1");
            AddIndicator("L PUMP", new Point(22, 204), new Size(104, 40), "Advisory L PUMP");
            AddIndicator("L TRANS", new Point(22, 262), new Size(104, 40), "Advisory L TRANS");
            AddIndicator("FLAPS 1", new Point(22, 323), new Size(104, 40), "Advisory FLAPS 1");
            AddIndicator("PROP", new Point(22, 382), new Size(104, 40), "Advisory PROP");
            AddIndicator("APU GEN", new Point(22, 441), new Size(104, 40), "Advisory APU GEN");
            AddIndicator("DEP RES", new Point(22, 499), new Size(104, 40), "Advisory DEP RES");
            AddIndicator("CS COOL", new Point(22, 557), new Size(104, 40), "Advisory CS COOL");
            AddIndicator("INS", new Point(22, 616), new Size(104, 40), "Advisory INS");
            AddIndicator("AFC", new Point(22, 676), new Size(104, 40), "Advisory AFC");
            AddIndicator("PITCH", new Point(22, 734), new Size(104, 40), "Advisory PITCH");
            AddIndicator("ROLL", new Point(22, 792), new Size(104, 40), "Advisory ROLL");
            AddIndicator("YAW", new Point(22, 850), new Size(104, 40), "Advisory YAW");
            AddIndicator("ENG EXC", new Point(22, 909), new Size(104, 40), "Advisory ENG EXC");
            AddIndicator("WSHLD", new Point(154, 86), new Size(104, 40), "Advisory WSHLD");
            AddIndicator("HYD 2", new Point(154, 146), new Size(104, 40), "Advisory HYD 2");
            AddIndicator("R PUMP", new Point(154, 204), new Size(104, 40), "Advisory R PUMP");
            AddIndicator("R TRANS", new Point(154, 262), new Size(104, 40), "Advisory R TRANS");
            AddIndicator("FLAPS 2", new Point(154, 323), new Size(104, 40), "Advisory FLAPS 2");
            AddIndicator("LIDS", new Point(154, 382), new Size(104, 40), "Advisory LIDS");
            AddIndicator("Blank 1", new Point(154, 441), new Size(104, 40), "Advisory Blank 1");
            AddIndicator("DC", new Point(154, 499), new Size(104, 40), "Advisory DC");
            AddIndicator("LOAD", new Point(154, 557), new Size(104, 40), "Advisory LOAD");
            AddIndicator("SKID", new Point(154, 616), new Size(104, 40), "Advisory SKID");
            AddIndicator("C*AUT", new Point(154, 676), new Size(104, 40), "Advisory C*AUT");
            AddIndicator("IFF", new Point(154, 734), new Size(104, 40), "Advisory IFF");
            AddIndicator("AFT BAY", new Point(154, 792), new Size(104, 40), "Advisory AFT BAY");
            AddIndicator("CW NOGO", new Point(154, 850), new Size(104, 40), "Advisory CW NOGO");
            AddIndicator("P NOGO", new Point(154, 909), new Size(104, 40), "Advisory P NOGO");
            AddIndicator("AUT FLP", new Point(294, 323), new Size(104, 40), "Advisory AUT FLP");
            AddIndicator("OIL", new Point(294, 382), new Size(104, 40), "Advisory OIL");
            AddIndicator("GPS", new Point(294, 441), new Size(104, 40), "Advisory GPS");
            AddIndicator("STBY TR", new Point(294, 499), new Size(104, 40), "Advisory STBY TRU");
            AddIndicator("CANOPY", new Point(294, 557), new Size(104, 40), "Advisory CANOPY");
            AddIndicator("EFC", new Point(294, 616), new Size(104, 40), "Advisory EFC");
            AddIndicator("H2O SEL", new Point(294, 676), new Size(104, 40), "Advisory H2O SEL");
            AddIndicator("SPD BRK", new Point(294, 734), new Size(104, 40), "Advisory SPD BRK");
            AddIndicator("AV BIT", new Point(294, 792), new Size(104, 40), "Advisory AV BIT");
            AddIndicator("P JAM", new Point(294, 850), new Size(104, 40), "Advisory P JAM");
            AddIndicator("CW JAM", new Point(294, 909), new Size(104, 40), "Advisory CW JAM");
            AddIndicator("NWS", new Point(426, 616), new Size(104, 40), "Advisory NWS");
            AddIndicator("APU", new Point(426, 676), new Size(104, 40), "Advisory APU");
            AddIndicator("DROOP", new Point(426, 734), new Size(104, 40), "Advisory DROOP");
            AddIndicator("Blank 2", new Point(426, 792), new Size(104, 40), "Advisory Blank 2");
            AddIndicator("JMR HOT", new Point(426, 850), new Size(104, 40), "Advisory JMR HOT");
            AddIndicator("REPLY", new Point(426, 909), new Size(104, 40), "Advisory REPLY");
        }

        public override string DefaultBackgroundImage
        {
            get { return "{AV-8B}/Images/Advisory Panel Frame.png"; }
        }

            private void AddIndicator(string name, Point posn, Size size, string interfaceElementName) { AddIndicator(name, posn, size, false, interfaceElementName); }
            private void AddIndicator(string name, Point posn, Size size, bool _vertical, string interfaceElementName)
            {
                Indicator indicator = AddIndicator(
                    name: name,
                    posn: posn,
                    size: size,
                    onImage: "{Helios}/Images/Indicators/anunciator.png",
                    offImage: "{Helios}/Images/Indicators/anunciator.png",
                    onTextColor: Color.FromArgb(0xff, 0x24, 0x8D, 0x22),
                    offTextColor: Color.FromArgb(0xff, 0x1C, 0x1C, 0x1C),
                    font: _font,
                    vertical: _vertical,
                    interfaceDeviceName: _interfaceDeviceName,
                    interfaceElementName: interfaceElementName,
                    fromCenter: false
                    );
                if (name == "Unknown 1" || name == "Unknown 2")
                {
                    indicator.Text = ".";
                }
                else
                {
                    indicator.Text = name;
                }
                indicator.Name = "Advisory Indicators_" + name;
                indicator.OnTextColor = Color.FromArgb(0xff, 0x94, 0xEB, 0xA6);
                indicator.OffTextColor = Color.FromArgb(0xff, 0x10, 0x10, 0x10);
                indicator.TextFormat.FontStyle = FontStyles.Normal;
                indicator.TextFormat.FontWeight = FontWeights.Normal;
                if (_vertical)
                {
                    if (_font == "MS 33558")
                    {
                        indicator.TextFormat.FontSize = 8;
                    }
                    else
                    {
                        indicator.TextFormat.FontSize = 11;
                    }
                }
                else
                {
                    indicator.TextFormat.FontSize = 15;
                }
                indicator.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName(_font);  // this probably needs to change before release
                indicator.TextFormat.PaddingLeft = 0;
                indicator.TextFormat.PaddingRight = 0;
                indicator.TextFormat.PaddingTop = 0;
                indicator.TextFormat.PaddingBottom = 0;
                indicator.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
                indicator.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                indicator.ScalingMode = TextScalingMode.Height;
            }


        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect1.Contains(location) || _scaledScreenRect2.Contains(location))
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