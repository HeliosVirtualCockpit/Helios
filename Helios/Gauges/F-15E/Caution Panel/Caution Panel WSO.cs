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

    [HeliosControl("Helios.F15E.CAUTIONS.WSO", "Caution Panel (WSO)", "F-15E Strike Eagle", typeof(BackgroundImageRenderer), HeliosControlFlags.None)]
    public class CautionsWSO : CompositeVisualWithBackgroundImage
    {
        private const string _imageLocation = "{F-15E}/Images/";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "";
        private String _font = "MS 33558";

        public CautionsWSO()
            : base("Caution and Warning Panel (WSO)", new Size(1698, 106))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.F15E.F15EInterface) };
            string[][] labels = { new string[] { "EngineFireLeft", "EngineFireRight", "CanopyUnlocked", "LowAlt", "Engine", "Hyd", "FltContr", "AVBIT", "MasterArm", "AutoPilot", "Program","Minimum","DSPFLOLO"},
                new string[] { "AI", "SAM", "OBST", "TFFail", "LGen", "RGen", "EmisLmt", "FuelLow", "Nuclear", "Unarmed", "Chaff", "Flare", "Oxygen" } };

            string[] interfaceCautionElements = new string[] {
                "Engine Indicator",
                "Hydraulics Indicator",
                "Flight Control Indicator",
                "AV-BIT Indicator",
                "Master Arm Indicator",
                "A/P Indicator",
                "PROGRAM Indicator",
                "MINIMUM Indicator",
                "Display Flow Low Indicator",
                "Left Generator Indicator",
                "Right Generator Indicator",
                "EMIS Limit Indicator",
                "Fuel Low Indicator",
                "Nuclear Indicator",
                "Unarmed Indicator",
                "Chaff Indicator",
                "Flare Indicator",
                "Oxygen Indicator"
            };
            string[] interfaceWarningElements = new string[]{"Engine Fire Left Indicator",
            "Engine Fire Right Indicator",
            "Canopy Unlocked Indicator (Rear)",
            "Low Altitude Indicator",
            "AI Threat Indicator",
            "SAM Threat Indicator",
            "OBST Indicator",
            "TF FAIL Indicator"};
            _interfaceDeviceName = "Warning Indicators (WSO)";
            for (int y = 0; y <= 1; y++)
            {
                for (int x = 0; x <= 3; x++)
                {
                    AddIndicator(labels[y][x], new Point(x * 110 + 51, y * 45 + 18), new Size(89, 31), interfaceWarningElements[y*4+x]);
                }
            }
            _interfaceDeviceName = "Caution Panel (WSO)";
            for (int y = 0; y <= 1; y++)
            {
                for (int x = 0; x <= 8; x++)
                {
                    AddIndicator(labels[y][x + 4], new Point(x * 110 + 678, y * 45 + 18), new Size(89, 31), interfaceCautionElements[y * 8 + x]);
                }
            }

        }

        private void AddIndicator(string name, Point posn, Size size, string interfaceElementName) { AddIndicator(name, posn, size, false, interfaceElementName); }
        private void AddIndicator(string name, Point posn, Size size, bool _vertical, string interfaceElementName)
        {
            Indicator indicator = AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: $"{_imageLocation}CautionPanels/CautionLightsWSO_{name}.png",
                offImage: "{Helios}/Images/Indicators/anunciator.png",
                onTextColor: Color.FromArgb(0x00, 0x24, 0x8D, 0x22),
                offTextColor: Color.FromArgb(0x00, 0x1C, 0x1C, 0x1C),
                font: _font,
                vertical: _vertical,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false,
                withText: false
                ) ;
            indicator.Name = $"{Name}_{name}";
        }



        public override string DefaultBackgroundImage => $"{_imageLocation}CautionPanels/CautionPanelWSO.png";

        private string ComponentName(string name)
        {
            return $"{Name}_{name}";
        }

        protected override void OnBackgroundImageChange()
        {
            BackgroundImage = BackgroundImageIsCustomized ? null : $"{_imageLocation}CautionPanels/CautionPanelWSO.png";
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
