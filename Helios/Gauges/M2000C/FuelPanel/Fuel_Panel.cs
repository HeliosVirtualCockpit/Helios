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

    [HeliosControl("HELIOS.M2000C.FUEL_PANEL", "Fuel Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_FuelPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 223, 396);
        private string _interfaceDeviceName = "Fuel Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_FuelPanel()
            : base("Fuel Panel", new Size(223, 396))
        {
            int row1 = 6, row2 = 178, row3 = 199, row4 = 220, row5 = 163, row6 = 65;
            int column1 = 93, column2 = 81, column3 = 102, column4 = 122;
            //First row
            AddIndicator("Air Refueling", "air-refueling", new Point(column1, row1), new Size(28, 28));
            //Second row
            AddIndicator("left-rl", "rl", new Point(column2, row2), new Size(21, 21));
            AddIndicator("center-rl", "rl", new Point(column3, row2), new Size(21, 21));
            AddIndicator("right-rl", "rl", new Point(column4, row2), new Size(21, 21));
            //Third row
            AddIndicator("left-av", "av", new Point(column2, row3), new Size(21, 21));
            AddIndicator("right-av", "av", new Point(column4, row3), new Size(21, 21));
            //Forth row
            AddIndicator("left-v", "v", new Point(column2, row4), new Size(21, 21));
            AddIndicator("right-v", "v", new Point(column4, row4), new Size(21, 21));

            //            AddPot();
            AddSwitch("Fuel CrossFeed Switch", new Point(112, 360), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);
            Add3PosnToggle("Fuel Detotalizer Switch", new Point(101, 110), new Size(20, 60), "{M2000C}/Images/switches/long-black-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Fuel Detotalizer Switch", false, false);
            Add3PosnToggle("Fuel Panel Transfer Switch", new Point(101, 258), new Size(20, 60), "{M2000C}/Images/switches/long-black-", ThreeWayToggleSwitchType.OnOnOn,
                ThreeWayToggleSwitchPosition.Three, _interfaceDeviceName, "Fuel Panel Transfer Switch", false, false);

            AddRectangleFill("Internal Fuel Quantity Needle", new Point(41, row5));
            AddRectangleFill("Total Fuel Quantity Needle", new Point(192, row5));

            AddDrum("Internal Fuel Quantity (Tens)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "tens quantity", "(0 - 10)", "#",
                new Point(77, row6), new Size(10d, 15d), new Size(12d, 19d));
            AddDrum("Internal Fuel Quantity (Hundreds)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "hundreds quantity", "(0 - 10)", "#",
                new Point(52, row6), new Size(10d, 15d), new Size(12d, 19d));
            AddDrum("Internal Fuel Quantity (Thousands)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "thousands quantity", "(0 - 10)", "#",
                new Point(28, row6), new Size(10d, 15d), new Size(12d, 19d));
            AddDrum("Total Fuel Quantity (Tens)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "tens quantity", "(0 - 10)", "#",
                new Point(167, row6), new Size(10d, 15d), new Size(12d, 19d));
            AddDrum("Total Fuel Quantity (Hundreds)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "hundreds quantity", "(0 - 10)", "#",
                new Point(142, row6), new Size(10d, 15d), new Size(12d, 19d));
            AddDrum("Total Fuel Quantity (Thousands)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "thousands quantity", "(0 - 10)", "#",
                new Point(118, row6), new Size(10d, 15d), new Size(12d, 19d));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/FuelPanel/fuel-panel.png"; }
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

        private void AddIndicator(string name, string imagePrefix, Point posn, Size size)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: "{M2000C}/Images/FuelPanel/" + imagePrefix + "-on.png",
                offImage: "{M2000C}/Images/FuelPanel/" + imagePrefix + "-off.png",
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private void AddSwitch(string name, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType)
        {
            AddToggleSwitch(name: name,
                posn: posn,
                size: new Size(45, 45),
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/FuelPanel/fuel-transfer-knob-on.png",
                positionTwoImage: "{M2000C}/Images/FuelPanel/fuel-transfer-knob-off.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: true,
                horizontalRender: true,
                nonClickableZones: null,
                fromCenter: true);
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
                clickType: LinearClickType.Swipe,
                fromCenter: false
                );
        }

        private void AddPot()
        {
            AddPot("Fuel CrossFeed Switch", new Point(112, 360), new Size(45, 45), "{M2000C}/Images/FuelPanel/fuel-transfer-knob.png", 90d, 180d, 0d, 1d, 0d, 0.1d, _interfaceDeviceName, "Fuel CrossFeed Switch", true);
        }

        private void AddRectangleFill(string name, Point posn)
        {
            AddRectangleFill(name: name,
                posn: posn,
                size: new Size(5,182),
                color: Color.FromArgb(0xff, 0xff, 0xff, 0xff),
                initialValue: 0d,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
        }

        private void AddDrum(string name, string gaugeImage, string actionIdentifier, string valueDescription, string format, Point posn, Size size, Size renderSize)
        {
            AddDrumGauge(name: name,
                gaugeImage: gaugeImage,
                posn: posn,
                size: size,
                renderSize: renderSize,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                actionIdentifier: actionIdentifier,
                valueDescription: valueDescription,
                format: format,
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
