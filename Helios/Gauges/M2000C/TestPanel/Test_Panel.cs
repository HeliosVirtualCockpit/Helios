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

    [HeliosControl("HELIOS.M2000C.TEST_PANEL", "Test Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_TESTPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 690, 680);
        private string _interfaceDeviceName = "Test Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        
        public M2000C_TESTPanel()
            : base("Test Panel", new Size(690, 680))
        {
            int row0 = 51, row1 = 214, row2 = 277;
            int column0 = 106, column1 = 220, column2 = 424;

            //Pannes en vol
            AddIndicator("HYD", "hyd", new Point(column0, row0), new Size(65, 160));
            AddIndicator("ELEC", "elec", new Point(column0, row2), new Size(65, 160));
            //Test lights
            AddIndicator("Test Fail", "test-fail", new Point(column1, row1), new Size(156, 65));
            AddIndicator("Test Ok", "test-ok", new Point(column2, row1), new Size(156, 65));

            ThreeWayToggleSwitch fbwTestSwitch = Add3PosnToggle(
                name: "FBW Test Switch",
                posn: new Point(330, 65),
                image: "long-black-",
                fromCenter: true
                );
            AddGuard("FBW Test Guard", "fbw-test-", new Point(286, 5), new Size(200, 170), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn,
                new NonClickableZone[] { new NonClickableZone(new Rect(0, 30, 200, 140), ToggleSwitchPosition.Two, fbwTestSwitch, ToggleSwitchPosition.Two) }, false, false);

            ToggleSwitch autopilotTestSwitch = AddSwitch("Autopilot Test Switch", "long-black-down", "long-black-up", new Point(365, 428), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, true);
            AddGuard("Autopilot Test Guard", "autopilot-test-", new Point(285, 355), new Size(180, 80), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn,
                new NonClickableZone[] { new NonClickableZone(new Rect(30, 0, 180, 80), ToggleSwitchPosition.Two, autopilotTestSwitch, ToggleSwitchPosition.Two) }, false, false);
            ToggleSwitch cdve5Switch = AddSwitch("FBW Channel 5 Switch", "long-black-up", "long-black-down", new Point(187, 563), ToggleSwitchPosition.One, ToggleSwitchType.OnOn, false);
            AddGuard("FBW Channel 5 Guard", "cdve5-", new Point(160, 490), new Size(65, 180), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn,
                new NonClickableZone[] { new NonClickableZone(new Rect(0, 30, 65, 150), ToggleSwitchPosition.Two, cdve5Switch, ToggleSwitchPosition.One) }, false, false);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/TestPanel/test-panel.png"; }
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

        private ThreeWayToggleSwitch Add3PosnToggle(string name, Point posn, string image, bool fromCenter)
        {
            return AddThreeWayToggle(
                name: name,
                pos: posn,
                size: new Size(45, 112),
                positionOneImage: "{M2000C}/Images/Switches/" + image + "down.png",
                positionTwoImage: "{M2000C}/Images/Switches/" + image + "mid.png",
                positionThreeImage: "{M2000C}/Images/Switches/" + image + "up.png",
                defaultPosition: ThreeWayToggleSwitchPosition.Three,
                switchType: ThreeWayToggleSwitchType.OnOnOn,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: true,
                horizontalRender: true,
                clickType: LinearClickType.Touch,
                fromCenter: true
                );
        }

        private void AddIndicator(string name, string imagePrefix, Point posn, Size size)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: "{M2000C}/Images/TestPanel/" + imagePrefix + "-on.png",
                offImage: "{M2000C}/Images/TestPanel/" + imagePrefix + "-off.png",
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private ToggleSwitch AddSwitch(string name, string imagePrefix1, string imagePrefix2, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal)
        {
            return AddToggleSwitch(name: name,
                posn: posn,
                size: new Size(45, 112),
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/Switches/" + imagePrefix1 + ".png",
                positionTwoImage: "{M2000C}/Images/Switches/" + imagePrefix2 + ".png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                horizontalRender: horizontal,
                nonClickableZones: null,
                fromCenter: true);
        }

        private void AddGuard(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition,
            ToggleSwitchType defaultType, NonClickableZone[] nonClickableZones, bool horizontal = true, bool horizontalRender = true)
        {
            AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/TestPanel/" + imagePrefix + "down.png",
                positionTwoImage: "{M2000C}/Images/TestPanel/" + imagePrefix + "up.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                horizontalRender: horizontalRender,
                nonClickableZones: nonClickableZones,
                fromCenter: false);
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
