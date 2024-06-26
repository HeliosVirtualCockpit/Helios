﻿//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.Gauges.AV8B;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;

    [HeliosControl("Helios.AV8B.ThreatIndicatorPanel", "Threat Indicator Panel", "AV-8B Harrier", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class ThreatIndicatorPanel: AV8BDevice
    {
        private string _interfaceDeviceName = "Threat Indicators";
        private string _imageLocation = "{AV-8B}/Images/WQHD/";
        private String _font = "MS 33558";

        public ThreatIndicatorPanel()
            : base("Threat Indicator Panel", new Size(251, 361))
        {
            AddIndicator("SAM", 24, 9, new Size(45, 49), "SAM");
            AddIndicator("CW", 67, 74, new Size(45, 49), "CW");
            AddIndicator("AI", 108  , 139, new Size(45, 49), "AI");
            AddIndicator("AAA", 154, 203, new Size(45, 49), "AAA");
        }
        public override string DefaultBackgroundImage
        {
            get { return _imageLocation + "Panel/Threat Indicator Panel.png"; }
        }
            
        private void AddIndicator(string name, double x, double y, Size size, string interfaceElementName) { AddIndicator(name, x, y, size, false, interfaceElementName); }
        private void AddIndicator(string name, double x, double y, Size size, bool _vertical, string interfaceElementName)
        {
            Indicator indicator = AddIndicator(
                name: name,
                posn: new Point(x, y),
                size: size,
                onImage: _imageLocation + "indicator/" + name + " On.png",
                offImage: "{av-8b}/Images/_transparent.png",
                onTextColor: System.Windows.Media.Color.FromArgb(0x00, 0xff, 0xff, 0xff),
                offTextColor: System.Windows.Media.Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                font: _font,
                vertical: _vertical,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
                indicator.Text = "";
                indicator.Name = "Threat Indicator Panel_" + name;
        }

        public override bool HitTest(Point location)
        {
            //if (_scaledScreenRect.Contains(location))
            //{
            //    return false;
            //}

            return false;
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
