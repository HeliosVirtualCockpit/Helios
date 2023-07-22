//  Copyright 2014 Craig Courtney
//  Copyright 2023 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E.CautionPanel
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.F15E;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.CAUTIONS.PILOT", "Caution Panel (Pilot)", "F-15E Strike Eagle", typeof(BackgroundImageRenderer), HeliosControlFlags.None)]
    public class Cautions_Pilot : CompositeVisualWithBackgroundImage
    {
        //private HeliosPanel _frameBezelPanel;
        //private const string Panel_Image = "Caution_Panel_";
        private const string _imageLocation = "{F-15E}/Images/";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "Caution Panel (Pilot)";
        private String _font = "MS 33558";
        private int _spareCounter = 1;

        public Cautions_Pilot()
            : base("Caution Panel (Pilot)", new Size(248, 535))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.F15E.F15EInterface) };

            //  +-----------------------+-----------------------+
            //  | PROGRAM (GR)     411? | MINIMUM         412?  |
            //  +-----------------------+-----------------------+
            //  | CHAFF            413? | FLARE           414?  |
            //  +-----------------------+-----------------------+
            //  | EMER BST ON      415  | BST SYS MAL     416   |
            //  +-----------------------+-----------------------+
            //  | NUCLEAR          420? | FUEL LOW        417?  |
            //  +-----------------------+-----------------------+
            //  | L GEN            419  | R GEN           418   |
            //  +-----------------------+-----------------------+
            //  | Engine           421  | FLT CONTR       422   |
            //  +-----------------------+-----------------------+
            //  | HYD              423? | AV BIT          424?  |
            //  +-----------------------+-----------------------+
            //  | DSPFLOLO         425? | OXYGEN          426   |
            //  +-----------------------+-----------------------+
            //  | SPARE              432| SPARE           432   |
            //  +-----------------------+-----------------------+
            //  | SPARE             432 | SPARE           432   |
            //  +-----------------------+-----------------------+
            string[][] labels = { new string[] {"PROGRAM", "CHAFF", "EMER BST ON", "NUCLEAR", "L GEN", "ENGINE", "HYD", "DSPFLOLO", "SPARE", "SPARE" },
                new string[] { "MINIMUM", "FLARE", "BST SYS MAL", "FUEL LOW", "R GEN", "FLT CONTR", "AV BIT", "OXYGEN", "SPARE", "SPARE"} };

            string[] interfaceElements = new string[] { "Program",
            "Chaff",
            "Emergency BST On",
            "Nuclear", 
            "Left Generator Warning",
            "Engine Warning",
            "Hydraulics Warning",
            "DSPFLOLO",
            "SPARE x4", 
            "SPARE x4",
            "Minimum Warning",
            "Flare",
            "BST System Malfunction",
            "FUEL LOW",
            "Right Generator Warning",
            "Flight Control",
            "AV BIT",
            "Oxygen",
            "SPARE x4",
            "SPARE x4"
            };
            for (int y = 0; y <= 9; y++)
            {
                for (int x = 0; x <= 1; x++)
                {
                    AddIndicator(labels[x][y], new Point(x * 120 + 13, y * 42 + 60), new Size(100, 31), $"{interfaceElements[x*10+y]} Indicator");
                }
            }
        }

        private void AddIndicator(string name, Point posn, Size size, string interfaceElementName) { AddIndicator(name, posn, size, false, interfaceElementName); }
        private void AddIndicator(string name, Point posn, Size size, bool _vertical, string interfaceElementName)
        {
            Indicator indicator = AddIndicator(
                name: name == "SPARE" ? name + _spareCounter.ToString() : name,
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
            indicator.Text = name;
            indicator.Name = $"{_interfaceDeviceName}_{(name == "SPARE"? name + _spareCounter++.ToString(): name)}";
            indicator.OnTextColor = name == "PROGRAM"? Color.FromArgb(0xff, 0x94, 0xEB, 0xA6): Color.FromArgb(0xff, 0xff, 0x7e, 0x00);
            indicator.OffTextColor = Color.FromArgb(0xff, 0x1C, 0x1C, 0x1C);
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
                indicator.TextFormat.FontSize = 12;
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



        public override string DefaultBackgroundImage => $"{_imageLocation}Bezels_And_Faces/Caution_Indicator_Panel.png";

        private string ComponentName(string name)
        {
            return $"{Name}_{name}";
        }

        protected override void OnBackgroundImageChange()
        {
            BackgroundImage = BackgroundImageIsCustomized ? null : $"{_imageLocation}/Bezels_And_Faces/Caution_Indicator_Panel.png";
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
