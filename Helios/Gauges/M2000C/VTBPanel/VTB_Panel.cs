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

    [HeliosControl("HELIOS.M2000C.VTB_PANEL", "VTB Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_VTBPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 531, 586);
        private string _interfaceDeviceName = "HUD/VTB";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_VTBPanel()
            : base("VTB Panel", new Size(531, 586))
        {
            int row1 = 117, row2 = 201, row3 = 284, row4 = 365;
            int column1 = 15, column2 = 500;
            Size VTB_switch_size = new Size(20, 40);

            //Left Switches
            Add3PosnToggle("Target Data Manual Entry Begin/End", new Point(column1, row1), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Data Manual Entry Begin/End", false, false);
            Add3PosnToggle("Bullseye Waypoint Selector",         new Point(column1, row2), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Bullseye Waypoint Selector", false, false);
            Add3PosnToggle("Target Range from Bullseye",         new Point(column1, row3), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Range from Bullseye", false, false);
            Add3PosnToggle("Target Bearing from Bullseye",       new Point(column1, row4), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Bearing from Bullseye", false, false);

            //Right Switches
            Add3PosnToggle("Target Heading",     new Point(column2, row1), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Heading", false, false);
            Add3PosnToggle("Target Altitude",    new Point(column2, row2), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Altitude", false, false);
            Add3PosnToggle("Target Mach Number", new Point(column2, row3), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Mach Number", false, false);
            Add3PosnToggle("Target Age",         new Point(column2, row4), VTB_switch_size, "{M2000C}/Images/switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Age", false, false);

            //Bottom Switches
            AddSwitch("VTB Declutter", new Point(76, 501), ToggleSwitchPosition.Two, ToggleSwitchType.MomOn);
            AddSwitch("VTB Orientation Selector (Inop)", new Point(134, 501), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("VTB Power Switch", new Point(500, 452), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/VTBPanel/VTB-panel.png"; }
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

        private void AddSwitch(string name, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType)
        {
            AddToggleSwitch(name: name,
                posn: posn,
                size: new Size(20, 40),
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/Switches/short-black-up.png",
                positionTwoImage: "{M2000C}/Images/Switches/short-black-down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: false,
                horizontalRender: false,
                nonClickableZones: null,
                fromCenter: false);
        }

        private void Add3PosnToggle(string name, Point posn, Size size, string image, ThreeWayToggleSwitchType switchType, ThreeWayToggleSwitchPosition defaultPosition,
            string interfaceDevice, string interfaceElement, bool fromCenter, bool horizontal)
        {
            AddThreeWayToggle(
                name: name,
                pos: posn,
                size: size,
                positionOneImage:   image + "up.png",
                positionTwoImage:   image + "mid.png",
                positionThreeImage: image + "down.png",
                defaultPosition: defaultPosition,
                switchType: switchType,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                horizontal: horizontal,
                horizontalRender: horizontal,
                clickType: LinearClickType.Swipe,
                fromCenter: false
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
