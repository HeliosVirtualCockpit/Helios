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

    [HeliosControl("HELIOS.M2000C.LG_PANEL", "Landing Gear Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_LGPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 300, 390);
        private string _interfaceDeviceName = "Landing Gear Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        
        public M2000C_LGPanel()
            : base("Landing Gear Panel", new Size(300, 390))
        {
            int row1 = 173, row2 = 191, row3 = 210, row4 = 228, row5 = 232, row6 = 242;
            int column1 = 202, column2 = 217, column3 = 232, column4 = 248, column5 = 262;
            int sizeIndicX = 23, sizeIndicY = 14;
           PushButton emergencyJettisonButton =  AddPushButton("Emergency Jettison Lever");

            //First row
            AddIndicator("A", new Point (column2, row1), new Size(sizeIndicX, sizeIndicY));
            AddIndicator("F", new Point(column4, row1), new Size(sizeIndicX, sizeIndicY));
            //Second row
            AddIndicator("DIRAV", new Point(column1, row2), new Size(sizeIndicX, sizeIndicY));
            AddIndicator("FREIN", new Point(column5, row2), new Size(sizeIndicX, sizeIndicY));
            //Third row
            AddIndicator("CROSS", new Point(column1, row3), new Size(sizeIndicX, sizeIndicY));
            AddIndicator("SPAD", new Point(column5, row3), new Size(sizeIndicX, sizeIndicY));
            //Forth row
            AddIndicator("BIP", new Point(column3, row4), new Size(24, 10));
            //Fifth row
            AddIndicator("left-gear", new Point(219, row5), new Size(7,20));
            AddIndicator("right-gear", new Point(265, row5), new Size(7, 20));
            //Sixth row
            AddIndicator("nose-gear", new Point(242, row6), new Size(7, 17));

            AddSwitch("Gun Arming Switch", "{M2000C}/Images/LGPanel/gun-arming-guard-", new Point(146, 173), new Size(47, 63), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            ToggleSwitch fbwgSwitch = AddSwitch("Fly by Wire Gain Mode Switch", "{M2000C}/Images/Switches/black-circle-", new Point(163, 257), new Size(25, 70), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("Fly by Wire G Limiter Switch", "{M2000C}/Images/Switches/black-circle-", new Point(241, 255), new Size(25, 70), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("Landing Gear Lever", "{M2000C}/Images/LGPanel/landing-gear-", new Point(70, 140), new Size(50, 170), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);

            /*            AddGuard("Fly By Wire Gain Switch Guard", "fbwg-guard-", new Point(152, 242), new Size(43, 90), ToggleSwitchPosition.One, ToggleSwitchType.OnOn,
                            new NonClickableZone[] { new NonClickableZone(new Rect(0, 0, 43, 70), ToggleSwitchPosition.Two, fbwgSwitch, ToggleSwitchPosition.One) },
                            false, false);
            */
            
            AddRotarySwitch("Emergency Landing Gear Lever", new NonClickableZone[] {
                    new NonClickableZone(new Rect(123, 81, 37, 70), true, emergencyJettisonButton)});
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/LGPanel/lg-panel.png"; }
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

        private void AddIndicator(string name, Point posn, Size size)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: "{M2000C}/Images/LGPanel/" + name + "-on.png",
                offImage: "{M2000C}/Images/LGPanel/" + name + "-off.png",
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private ToggleSwitch AddSwitch(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal = false)
        {
            return AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: imagePrefix + "up.png",
                positionTwoImage: imagePrefix + "down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                fromCenter: false);
         }

        private void AddGuard(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition,
            ToggleSwitchType defaultType, NonClickableZone[] nonClickableZones, bool horizontal = true, bool horizontalRender = true)
        {
            AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/LGPanel/" + imagePrefix + "down.png",
                positionTwoImage: "{M2000C}/Images/LGPanel/" + imagePrefix + "up.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                horizontalRender: horizontalRender,
                nonClickableZones: nonClickableZones,
                fromCenter: false);
        }

        private PushButton AddPushButton(string name)
        {
            return AddButton(name: name,
                posn: new Point(212, 81),
                size: new Size(70, 70),
                image: "{M2000C}/Images/LGPanel/emergency-jettison-not-pushed.png",
                pushedImage: "{M2000C}/Images/LGPanel/emergency-jettison-pushed.png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
        }

        private void AddRotarySwitch(string name, NonClickableZone[] nonClickableZones)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: new Point(169, 83),
                size: new Size(160, 160),
                knobImage: "{M2000C}/Images/LGPanel/emergency-landing-gear-lever.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name, 
                nonClickableZones: nonClickableZones,
                fromCenter:true);
            rSwitch.Positions.Clear();
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "OFF", 0d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "ON", 90d));
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
