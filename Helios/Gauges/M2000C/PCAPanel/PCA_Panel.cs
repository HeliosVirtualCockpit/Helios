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

    [HeliosControl("HELIOS.M2000C.PCA_PANEL", "PCA Panel", "M2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_PCAPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 690, 300);
        private string _interfaceDeviceName = "PCA Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _font = "Helios Virtual Cockpit F/A-18C_Hornet-Up_Front_Controller";

        public M2000C_PCAPanel()
            : base("PCA Panel", new Size(690, 300))
        {
            int row0 = 104, row1 = 236;
            int column0 = 37, column1 = 132, column2 = 245, column3 = 355, column4 = 466, column5 = 575;
            int offsetXg = 20, offsetXc = 32, offsetXd = 47, offsetY = 20;
            AddPushButton("Targeting Mode Selection", new Point(column1, row0));
            AddPushButton("Master Mode Selection", new Point(column2, row0));
            AddPushButton("Approach Mode Selection", new Point(column3, row0));
            AddPushButton("Fligt Plan Route Selection", new Point(column4, row0));
            AddPushButton("INS Calibration", new Point(column5, row0));
            AddPushButton("Gun Mode Selector", new Point(column0, row1));
            AddPushButton("Weapon Store Selector 1", new Point(column1, row1));
            AddPushButton("Weapon Store Selector 2", new Point(column2, row1));
            AddPushButton("Weapon Store Selector 3", new Point(column3, row1));
            AddPushButton("Weapon Store Selector 4", new Point(column4, row1));
            AddPushButton("Weapon Store Selector 5", new Point(column5, row1));

            AddIndicator("TMS S", "s", new Point(column1 + offsetXc, row0 + offsetY), new Size(5, 9));
            AddIndicator("MMS", "s", new Point(column2 + offsetXc, row0 + offsetY), new Size(5, 9));
            AddIndicator("AMS", "s", new Point(column3 + offsetXc, row0 + offsetY), new Size(5, 9));
            AddIndicator("FPRS", "s", new Point(column4 + offsetXc, row0 + offsetY), new Size(5, 9));
            AddIndicator("INS C", "s", new Point(column5 + offsetXc, row0 + offsetY), new Size(5, 9));
            AddIndicator("KL1", "kl1", new Point(column0 + offsetXg, row1 + offsetY), new Size(16, 9));
            AddIndicator("KL2", "kl2", new Point(column0 + offsetXd, row1 + offsetY), new Size(16, 9));
            AddIndicator("WSS1 S", "s", new Point(column1 + offsetXg, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS2 S", "s", new Point(column2 + offsetXg, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS3 S", "s", new Point(column3 + offsetXg, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS4 S", "s", new Point(column4 + offsetXg, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS5 S", "s", new Point(column5 + offsetXg, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS1 P", "p", new Point(column1 + offsetXd, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS2 P", "p", new Point(column2 + offsetXd, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS3 P", "p", new Point(column3 + offsetXd, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS4 P", "P", new Point(column4 + offsetXd, row1 + offsetY), new Size(5, 9));
            AddIndicator("WSS5 P", "p", new Point(column5 + offsetXd, row1 + offsetY), new Size(5, 9));

            AddSwitch("Master Arm Switch", "red-", new Point(32, 28), new Size(30, 90), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, false);
            ToggleSwitch selectiveJettisonSwitch = AddSwitch("Selective Jettison Switch", "long-black-", new Point(36, 170), new Size(29, 60), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn, true);
            AddGuard("Selective Jettison Switch Guard", "guard-", new Point(5, 160), new Size(100, 50), ToggleSwitchPosition.One, ToggleSwitchType.OnOn,
                new NonClickableZone[] { new NonClickableZone(new Rect(30, 0, 120, 63), ToggleSwitchPosition.Two, selectiveJettisonSwitch, ToggleSwitchPosition.One) });

            AddTextDisplay("PCA Upper Display", new Point(110d, 35d), new Size(551d, 52d), _interfaceDeviceName, "PCA Upper Display", 36.5, "MMMMMMMMMMMMMMM", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PCA Lower Display", new Point(110d, 169d), new Size(551d, 52d), _interfaceDeviceName, "PCA Lower Display", 36.5, "MMMMMMMMMMMMMMM", TextHorizontalAlignment.Left, "");

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/PCAPanel/pca-panel.png"; }
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

        private void AddPushButton(string name, Point posn)
        {
            AddButton(name: name,
                posn: posn,
                size: new Size(65, 40),
                image: "{M2000C}/Images/PCAPanel/pca-button.png",
                pushedImage: "{M2000C}/Images/PCAPanel/pca-button.png",
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
                onImage: "{M2000C}/Images/PCAPanel/" + imagePrefix + "-on.png",
                offImage: "{M2000C}/Images/PCAPanel/void.png", //empty picture to permit the indicator to work because I’ve nothing to display when off
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private ToggleSwitch AddSwitch(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType, bool horizontal)
        {
            return AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/Switches/" + imagePrefix + "up.png",
                positionTwoImage: "{M2000C}/Images/Switches/" + imagePrefix + "down.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: horizontal,
                horizontalRender: horizontal,
                nonClickableZones: null,
                fromCenter: false);
        }

        private void AddGuard(string name, string imagePrefix, Point posn, Size size, ToggleSwitchPosition defaultPosition,
            ToggleSwitchType defaultType, NonClickableZone[] nonClickableZones)
        {
            AddToggleSwitch(name: name,
                posn: posn,
                size: size,
                defaultPosition: defaultPosition,
                positionOneImage: "{M2000C}/Images/PCAPanel/" + imagePrefix + "down.png",
                positionTwoImage: "{M2000C}/Images/PCAPanel/" + imagePrefix + "up.png",
                defaultType: defaultType,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                horizontal: false,
                horizontalRender: false,
                nonClickableZones: nonClickableZones,
                fromCenter: false);
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
