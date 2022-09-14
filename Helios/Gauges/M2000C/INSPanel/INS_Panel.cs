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

    [HeliosControl("HELIOS.M2000C.INS_PANEL", "INS Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_INSPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 700, 198);
        private string _interfaceDeviceName = "INS Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_INSPanel()
            : base("INS Panel", new Size(700, 198))
        {

            AddSwitch("Mode Selector", "{M2000C}/Images/INSPanel/mode-selector.png", new Point(157, 85), new Size(100, 100), true);
            AddSwitch("Operation Selector", "{M2000C}/Images/INSPanel/operation-selector.png", new Point(589, 117), new Size(84, 84), true);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/INSPanel/ins-panel.png"; }
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

        private void AddSwitch(string name, string knobImage, Point posn, Size size, bool fromCenter)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: knobImage,
                defaultPosition: 0, 
                clickType: RotaryClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: fromCenter);
            rSwitch.IsContinuous = true;
            rSwitch.Positions.Clear();
            switch(name)
            {
                case "Mode Selector":
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 0, "AR", 2d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "VEI", 27d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "CAL", 61d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 3, "TST", 93d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 4, "ALN", 120d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 5, "ALCM", 148d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 6, "NAV", 178d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 7, "SEC", 202d));
                    break;
                case "Operation Selector":
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 0, "N", 357d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "STS", 322d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "DCI", 290d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 3, "CRV", 254d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 4, "MAIN", 216d));
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
