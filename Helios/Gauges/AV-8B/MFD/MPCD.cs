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

using GadrocsWorkshop.Helios.Interfaces.DCS.AV8B;

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Globalization;
    using System.Windows;

    class MPCD : MFD
    {
        private static readonly Rect SCREEN_RECT = new Rect(80, 204, 754, 703);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName;
        private string _side;

        public MPCD( string name ,string interfaceDeviceName)
            : base(name, new Size(924, 1078))
        {
            _interfaceDeviceName = interfaceDeviceName;
            _side = name;
            SupportedInterfaces = new[] { typeof(AV8BInterface) };

            AddButton("OSB1", 5, 740, new Size(56,56), true, "OSB01");
            AddButton("OSB2", 5, 643, new Size(56, 56), true, "OSB02");
            AddButton("OSB3", 5, 534, new Size(56, 56), true, "OSB03");
            AddButton("OSB4", 5, 427, new Size(56, 56), true, "OSB04");
            AddButton("OSB5", 5, 325, new Size(56, 56), true, "OSB05");

            AddButton("OSB6", 204, 135, new Size(56, 56), false, "OSB06");
            AddButton("OSB7", 319, 135, new Size(56, 56), false, "OSB07");
            AddButton("OSB8", 430, 135, new Size(56, 56), false, "OSB08");
            AddButton("OSB9", 544, 135, new Size(56, 56), false, "OSB09");
            AddButton("OSB10", 655, 135, new Size(56, 56), false, "OSB10");

            AddButton("OSB11", 855, 325, new Size(56, 56), true, "OSB11");
            AddButton("OSB12", 855, 427, new Size(56, 56), true, "OSB12");
            AddButton("OSB13", 855, 534, new Size(56, 56), true, "OSB13");
            AddButton("OSB14", 855, 643, new Size(56, 56), true, "OSB14");
            AddButton("OSB15", 855, 740, new Size(56, 56), true, "OSB15");

            AddButton("OSB16", 655, 929, new Size(56, 56), false, "OSB16");
            AddButton("OSB17", 544, 929, new Size(56, 56), false, "OSB17");
            AddButton("OSB18", 430, 929, new Size(56, 56), false, "OSB18");
            AddButton("OSB19", 319, 929, new Size(56, 56), false, "OSB19");
            AddButton("OSB20", 204, 929, new Size(56, 56), false, "OSB20");

            AddRocker("Day / Night", "MFD Rocker", "L", 76, 73, "DAY/NIGHT Mode");
            AddRocker("Symbols", "MFD Rocker", "R", 720, 74, "Symbology");
            AddRocker("Gain", "MFD Rocker", "V", 6, 854, "Gain");
            AddRocker("Contrast", "MFD Rocker", "V", 857, 854, "Contrast");

            AddPot("Brightness Knob", new Point(420,47), new Size(70,70), "Off/Brightness Control");
        }
        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{AV-8B}/Images/MPCD Bezel 2.png"; }
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
        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            Potentiometer knob = AddPot(
                name: name,
                posn: posn,
                size: size,
                knobImage: "{AV-8B}/Images/Common Knob.png",
                initialRotation: 219,
                rotationTravel: 291,
                minValue: 0,
                maxValue: 1,
                initialValue: 1,
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            knob.Name = _side + "_" + name;
        }
        private void AddButton(string name, double x, double y, Size size, bool horizontal, string interfaceElementName)
        {
            PushButton button = AddButton(
                name: name,
                posn: new Point(x, y),
                size: size,
                image: "{AV-8B}/Images/MFD Button 1 UpV.png",
                pushedImage: "{AV-8B}/Images/MFD Button 1 DnV.png",
                buttonText: "",
                interfaceDeviceName:  _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            if (horizontal)
            {
                button.Image = "{AV-8B}/Images/MFD Button 1 UpH.png";
                button.PushedImage = "{AV-8B}/Images/MFD Button 1 DnH.png";
            }
            button.Name = _side + "_" + name;
        }

        private void AddRocker(string name, string imagePrefix, string imageOrientation, double x, double y, string interfaceElementName)
        {
            Size rockerSize = new Size(0,0);
            switch (imageOrientation)
            {
                case ("V"):
                    rockerSize = new Size(54, 114);
                    break;
                case ("L"):
                    rockerSize = new Size(120, 110);
                    break;
                case ("R"):
                    rockerSize = new Size(120, 110);
                    break;
            }


            ThreeWayToggleSwitch rocker = AddThreeWayToggle(
                name: name,
                posn: new Point(x,y),
                size: rockerSize,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.MomOnMom,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                positionTwoImage: "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Mid.png",
                clickType: LinearClickType.Touch,
                fromCenter: false
                );
            rocker.Name = _side + "_" + name;
            switch (imageOrientation)
            {
                case ("V"):
                    rocker.PositionOneImage = "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                     break;
                case ("L"):
                    rocker.PositionOneImage = "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    break;
                case ("R"):
                    rocker.PositionOneImage = "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{AV-8B}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                     break;
                default:
                    break;
            }
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
