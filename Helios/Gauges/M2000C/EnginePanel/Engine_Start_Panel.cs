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
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("HELIOS.M2000C.ENGINE_PANEL", "Engine Start Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_EnginePanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 700, 251);
        private string _interfaceDeviceName = "Engine Start Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_EnginePanel()
            : base("Engine Start Panel", new Size(700, 251))
        {
            int row0 = 44, row1 = 166;

            ToggleSwitch starterFuelPumpSwitch = AddSwitch("Starter Fuel Pump Switch", "long-black-", new Point(208, row0), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            AddSwitch("Left Fuel Pump Switch", "long-black-", new Point(343, row0), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            AddSwitch("Right Fuel Pump Switch", "long-black-", new Point(484, row0), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            ToggleSwitch fuelShutOffSwitch = AddSwitch("Fuel Shut-Off Switch", "long-black-", new Point(440, 165), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            PushButton engineStartButton = AddPushButton("Engine Start Button");
            AddGuard("Engine Start Switch Guard", "engine-start-", new Point(82, 37), new Size(206, 169), ToggleSwitchPosition.One, ToggleSwitchType.OnOn,
                new NonClickableZone[] {
                    new NonClickableZone(new Rect(0, 50, 90, 119), ToggleSwitchPosition.One, engineStartButton, ToggleSwitchPosition.Two),
                    new NonClickableZone(new Rect(125, 0, 81, 50), ToggleSwitchPosition.Two, starterFuelPumpSwitch, ToggleSwitchPosition.Two) },
                false, false, true);
            AddGuard("Fuel Shut-Off Switch Guard", "coupe-feu-", new Point(385, 160), new Size(150,54), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, 
                new NonClickableZone[] { new NonClickableZone(new Rect(30,0,140,50), ToggleSwitchPosition.One, fuelShutOffSwitch, ToggleSwitchPosition.Two) }, false, false);

            Add3PosnToggle(
                name: "Ignition Ventilation Selector Switch",
                posn: new Point(233, row1),
                image: "{M2000C}/Images/Switches/long-black-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Ignition Ventilation Selector Switch",
                fromCenter: false
                );

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/EnginePanel/engine-start-panel.png"; }
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

        private ToggleSwitch AddSwitch(string name, string imagePrefix, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType)
        {
            return AddToggleSwitch(name: name,
                posn: posn,
                size: new Size(37, 75),
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/Switches/" + imagePrefix + "down.png",
                positionTwoImage: "{M2000C}/Images/Switches/" + imagePrefix + "up.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: true,
                horizontalRender: true,
                nonClickableZones: null,
                fromCenter: false);
        }

        private void AddGuard(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition, 
            ToggleSwitchType defaultType, NonClickableZone[] nonClickableZones, bool horizontal = true, bool horizontalRender = true,bool horizontalReversed = false)
        {
            ToggleSwitch cover = AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/EnginePanel/" + imagePrefix + "up.png",
                positionTwoImage: "{M2000C}/Images/EnginePanel/" + imagePrefix + "down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                horizontalRender: horizontalRender,
                nonClickableZones: nonClickableZones,
                fromCenter: false);
            cover.Orientation = horizontalReversed?ToggleSwitchOrientation.HorizontalReversed:ToggleSwitchOrientation.Horizontal;
            cover.ClickType = LinearClickType.Swipe;
        }

        private void Add3PosnToggle(string name, Point posn, string image, string interfaceDevice, string interfaceElement, bool fromCenter)
        {
            AddThreeWayToggle(
                name: name,
                pos: posn,
                size: new Size(33, 100),
                positionOneImage: image + "down.png",
                positionTwoImage: image + "mid.png",
                positionThreeImage: image + "up.png",
                defaultPosition: ThreeWayToggleSwitchPosition.One,
                switchType: ThreeWayToggleSwitchType.OnOnOn,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                horizontal: true,
                horizontalRender: true,
                clickType: LinearClickType.Touch,
                fromCenter: true
                );
        }

        private PushButton AddPushButton(string name)
        {
            return AddButton(name: name,
                posn: new Point(98,92),
                size: new Size(102,102), 
                image: "{M2000C}/Images/EnginePanel/start-engine-not-pushed.png", 
                pushedImage: "{M2000C}/Images/EnginePanel/start-engine-pushed.png",
                buttonText: "", 
                interfaceDeviceName: _interfaceDeviceName, 
                interfaceElementName: name, 
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
