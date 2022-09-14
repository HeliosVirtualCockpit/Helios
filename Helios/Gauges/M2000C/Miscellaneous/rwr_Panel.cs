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

    [HeliosControl("HELIOS.M2000C.RWR_PANEL", "RWR Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_RWRPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 138, 15);
        private string _interfaceDeviceName = "RWR Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_RWRPanel()
            : base("RWR Panel", new Size(138, 15))
        {
            int row0 = 5;

            AddIndicator("RWR V", new Point(10, row0));
            AddIndicator("RWR BR", new Point(33, row0));
            AddIndicator("RWR DA", new Point(60, row0));
            AddIndicator("RWR D2M", new Point(90, row0));
            AddIndicator("RWR LL", new Point(120, row0));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return null; }
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
                size: new Size(8, 8),
                onImage: "{M2000C}/Images/Miscellaneous/rwr-light-on.png",
                offImage: "{M2000C}/Images/Miscellaneous/rwr-light-off.png",
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text,
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text,
                font: "", //don’t need it because not using text,
                vertical: true,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
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
