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

// ReSharper disable once CheckNamespace
namespace GadrocsWorkshop.Helios.Gauges.M2000C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System.Windows;

    [HeliosControl("HELIOS.M2000C.ECMBox", "ECM Box", "M-2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_ECMBox : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 296, 359);
        private string _interfaceDeviceName = "ECM Box";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_ECMBox()
            : base("ECM Box", new Size(296, 359))
        {
            AddSwitch("Dispensing Mode Switch", "long-black-up", "long-black-down", new Point(198, 220), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("Lights Power Switch", "long-black-mid", "long-black-down", new Point(52, 305), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);

            Add3PosnToggle(
                name: "Master Switch",
                posn: new Point(57, 59),
                image: "red-",
                fromCenter: true
                );

            AddPot("Brightness Selector", new Point(73, 271), new Size(45, 45), "brightness-selector", 0d, 0d, 0d, 10d, 0d, 0.1d, true);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/ECMBox/ecm-box.png"; }
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

        private void AddPot(string name, Point posn, Size size, string imagePrefix, double initialRotation, double rotationTravel, double minValue, double maxValue,
            double initialValue, double stepValue, bool fromCenter)
        {
            AddPot(
                name: name,
                posn: posn,
                size: size,
                knobImage: "{M2000C}/Images/ECMBox/" + imagePrefix + ".png",
                initialRotation: initialRotation,
                rotationTravel: rotationTravel,
                minValue: minValue,
                maxValue: maxValue,
                initialValue: initialValue,
                stepValue: stepValue,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true,
                clickType: RotaryClickType.Touch);
        }

        private ToggleSwitch AddSwitch(string name, string imagePrefixPos1, string imagePrefixPos2, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType)
        {
            return AddToggleSwitch(name: name,
                posn: posn,
                size: new Size(37, 112),
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/Switches/" + imagePrefixPos1 + ".png",
                positionTwoImage: "{M2000C}/Images/Switches/" + imagePrefixPos2 + ".png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: false,
                horizontalRender: false,
                nonClickableZones: null,
                fromCenter: false);
        }

        private void Add3PosnToggle(string name, Point posn, string image, bool fromCenter)
        {
            AddThreeWayToggle(
                name: name,
                pos: posn,
                size: new Size(33, 100),
                positionOneImage: "{M2000C}/Images/Switches/" + image + "up.png",
                positionTwoImage: "{M2000C}/Images/Switches/" + image + "mid.png",
                positionThreeImage: "{M2000C}/Images/Switches/" + image + "down.png",
                defaultPosition: ThreeWayToggleSwitchPosition.Three,
                switchType: ThreeWayToggleSwitchType.OnOnOn,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: false,
                horizontalRender: false,
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
