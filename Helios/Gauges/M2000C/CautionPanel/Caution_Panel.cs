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

    [HeliosControl("HELIOS.M2000C.CAUTION_PANEL", "Caution Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_CautionPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 256, 280);
        private string _interfaceDeviceName = "Caution Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_CautionPanel()
            : base("Caution Panel", new Size(256, 280))
        {
            int row0 = 30, row1 = 92, row2 = 112, row3 = 132, row4 = 153, row5 = 173, row6 = 193, row7 = 214, row8 = 234;
            int column1 = 14, column2 = 57, column3 = 100, column4 = 144, column5 = 185;
            //First row
            AddIndicator("BATT", new Point (column1, row1));
            AddIndicator("TR", new Point(column2, row1));
            AddIndicator("ALT1", new Point(column3, row1));
            AddIndicator("ALT2", new Point(column4, row1));
            //Second row
            AddIndicator("HUILE", new Point(column1, row2));
            AddIndicator("T7", new Point(column2, row2));
            AddIndicator("CALC", new Point(column3, row2));
            AddIndicator("SOURIS", new Point(column4, row2));
            AddIndicator("PELLE", new Point(column5, row2));
            //Third row
            AddIndicator("BP", new Point(column1, row3));
            AddIndicator("BPG", new Point(column2, row3));
            AddIndicator("BPD", new Point(column3, row3));
            AddIndicator("TRANSF", new Point(column4, row3));
            AddIndicator("NIVEAU", new Point(column5, row3));
            //Forth row
            AddIndicator("HYD1", new Point(column1, row4));
            AddIndicator("HYD2", new Point(column2, row4));
            AddIndicator("HYDS", new Point(column3, row4));
            AddIndicator("EP", new Point(column4, row4));
            AddIndicator("BINGO", new Point(column5, row4));
            //Fifth row
            AddIndicator("PCAB", new Point(column1, row5));
            AddIndicator("TEMP", new Point(column2, row5));
            AddIndicator("REGO2", new Point(column3, row5));
            AddIndicator("5mnO2", new Point(column4, row5));
            AddIndicator("O2HA", new Point(column5, row5));
            //Sixth row
            AddIndicator("ANEMO", new Point(column1, row6));
            AddIndicator("CC", new Point(column2, row6));
            AddIndicator("DSV", new Point(column3, row6));
            AddIndicator("CONDIT", new Point(column4, row6));
            AddIndicator("CONF", new Point(column5, row6));
            //Seventh row
            AddIndicator("PA", new Point(column1, row7));
            AddIndicator("MAN", new Point(column2, row7));
            AddIndicator("DOM", new Point(column3, row7));
            AddIndicator("BECS", new Point(column4, row7));
            AddIndicator("USEL", new Point(column5, row7));
            //Eighth row
            AddIndicator("ZEICHEN", new Point(column1, row8));
            AddIndicator("GAIN", new Point(column2, row8));
            AddIndicator("RPM", new Point(column3, row8));
            AddIndicator("DECOL", new Point(column4, row8));
            AddIndicator("PARK", new Point(column5, row8));

            AddSwitch("Main Battery Switch", "red-", new Point(15, 25), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            AddSwitch("Electric Power Transfer Switch", "long-black-", new Point(61, row0), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("Alternator 1 Switch", "long-black-", new Point(105, row0), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("Alternator 2 Switch", "long-black-", new Point(147, row0), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            Add3PosnToggle(
                name: "Lights Test Switch",
                posn: new Point(152, 70),
                image: "{M2000C}/Images/Switches/long-black-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Lights Test Switch",
                fromCenter: false
                );
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/CautionPanel/caution-panel.png"; }
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

        private void AddIndicator(string name, Point posn)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: new Size(32,15),
                onImage: "{M2000C}/Images/CautionPanel/" + name + "-on.png",
                offImage: "{M2000C}/Images/CautionPanel/" + name + "-off.png",
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private void AddSwitch(string name, string imagePrefix, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal = false)
        {
            AddToggleSwitch(name: name,
                posn: posn,
                size: new Size(25, 70),
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/Switches/" + imagePrefix + "up.png",
                positionTwoImage: "{M2000C}/Images/SWitches/" + imagePrefix + "mid.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                fromCenter: false);
         }

        private void Add3PosnToggle(string name, Point posn, string image, string interfaceDevice, string interfaceElement, bool fromCenter)
        {
            AddThreeWayToggle(
                name: name,
                pos: posn,
                size: new Size(33, 100),
                positionOneImage: image + "up.png",
                positionTwoImage: image + "mid.png",
                positionThreeImage: image + "down.png",
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                switchType: ThreeWayToggleSwitchType.MomOnMom,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                horizontal: true,
                horizontalRender: true,
                clickType: LinearClickType.Touch,
                fromCenter: true
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
