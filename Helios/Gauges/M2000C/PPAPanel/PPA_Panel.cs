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

    [HeliosControl("HELIOS.M2000C.PPA_PANEL", "PPA Panel", "M2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_PPAPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 350, 203);
        private string _interfaceDeviceName = "PPA Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _font = "Helios Virtual Cockpit F/A-18C Hornet IFEI";

        public M2000C_PPAPanel()
            : base("PPA Panel", new Size(350, 203))
        {
            Add3PosnToggle(
                name: "Missile Selector Switch",
                posn: new Point(48, 30),
                size: new Size(14, 40),
                image: "{M2000C}/Images/PPAPanel/small-black-",
                switchType: ThreeWayToggleSwitchType.OnOnOn,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Missile Selector Switch",
                fromCenter: true,
                horizontal: true
                );
            Add3PosnToggle(
                name: "Test Switch",
                posn: new Point(284, 17),
                size: new Size(14, 40),
                image: "{M2000C}/Images/PPAPanel/small-black-",
                switchType: ThreeWayToggleSwitchType.MomOnMom,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Test Switch",
                fromCenter: true,
                horizontal: false
                );
            Add3PosnToggle(
                name: "Bomb Fuse Selector",
                posn: new Point(40, 115),
                size: new Size(24, 50),
                image: "{M2000C}/Images/Switches/black-circle-",
                switchType: ThreeWayToggleSwitchType.OnOnOn,
                defaultPosition: ThreeWayToggleSwitchPosition.Three,
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Bomb Fuse Selector",
                fromCenter: true,
                horizontal: false
                );
            Add3PosnToggle(
                name: "Release Quantity Selector",
                posn: new Point(115, 104),
                size: new Size(14, 40),
                image: "{M2000C}/Images/PPAPanel/small-black-",
                switchType: ThreeWayToggleSwitchType.MomOnMom,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Release Quantity Selector",
                fromCenter: true,
                horizontal: true
                );
            Add3PosnToggle(
                name: "Bomb Drop Interval",
                posn: new Point(115, 157),
                size: new Size(14, 40),
                image: "{M2000C}/Images/PPAPanel/small-black-",
                switchType: ThreeWayToggleSwitchType.MomOnMom,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Bomb Drop Interval",
                fromCenter: true,
                horizontal: true
                );

            AddPushButton("S530 Missile Enabler Button", new Point(113, 12), "ppa-p-mis");
            AddPushButton("Missile Fire Mode Selector", new Point(161, 12), "ppa-button");
            AddPushButton("Magic II Missile Enabler Button", new Point(216, 12), "ppa-p-mag");
            AddPushButton("Guns/Rockets/Missiles Firing Mode Selector", new Point(275, 100), "ppa-button");

            int column0 = 130, column1 = 178, column2 = 235, column3 = 293;
            int row0 = 30, row1 = 60, row2 = 118, row3 = 148;
            AddIndicator("S530D P", "p", new Point(column0, row0), new Size(5, 9));
            AddIndicator("S530D MIS", "mis", new Point(column0, row1), new Size(16, 9));
            AddIndicator("Missile AUT Mode", "aut", new Point(column1, row0), new Size(16, 9));
            AddIndicator("Missile MAN Mode", "man", new Point(column1, row1), new Size(16, 9));
            AddIndicator("MAGIC P", "p", new Point(column2, row0), new Size(5, 9));
            AddIndicator("MAGIC MAG", "mag", new Point(column2, row1), new Size(16, 9));
            AddIndicator("TOT Firing Mode", "tot", new Point(column3, row2), new Size(16, 9));
            AddIndicator("PAR Firing Mode", "par", new Point(column3, row3), new Size(16, 9));

            AddTextDisplay("PPA Display Quantity", new Point(196d, 89d), new Size(55d, 50d), _interfaceDeviceName, "PPA Display Quantity", 50, "00", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PPA Dispaly Interval", new Point(196d, 139d), new Size(55d, 50d), _interfaceDeviceName, "PPA Display Interval", 50, "00", TextHorizontalAlignment.Left, "");

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/PPAPanel/ppa-panel.png"; }
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

        private void Add3PosnToggle(string name, Point posn, Size size, string image, ThreeWayToggleSwitchType switchType, ThreeWayToggleSwitchPosition defaultPosition,
            string interfaceDevice, string interfaceElement, bool fromCenter, bool horizontal)
        {
            AddThreeWayToggle(
                name: name,
                pos: posn,
                size: size,
                positionOneImage: image + "up.png",
                positionTwoImage: image + "mid.png",
                positionThreeImage: image + "down.png",
                defaultPosition: defaultPosition,
                switchType: switchType,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                horizontal: horizontal,
                horizontalRender: horizontal,
                clickType: LinearClickType.Touch,
                fromCenter: false
                );
        }

        private void AddPushButton(string name, Point posn, string imagePrefix)
        {
            AddButton(name: name,
                posn: posn,
                size: new Size(36, 61),
                image: "{M2000C}/Images/PPAPanel/" + imagePrefix + ".png",
                pushedImage: "{M2000C}/Images/PPAPanel/" + imagePrefix + ".png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
        }

        private void AddIndicator(string name, string imagePrefix, Point posn, Size size)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: "{M2000C}/Images/PPAPanel/" + imagePrefix + "-on.png",
                offImage: "{M2000C}/Images/PPAPanel/void.png", //empty picture to permit the indicator to work because I’ve nothing to display when off
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }
        private void AddTextDisplay(string name, Point posn, Size size,
    string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
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
