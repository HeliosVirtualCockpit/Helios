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

using GadrocsWorkshop.Helios.Controls;

namespace GadrocsWorkshop.Helios.Gauges.M2000C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("HELIOS.M2000C.PCI_PANEL", "Post Combustion Indicator Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_PCIPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 66, 66);
        private string _interfaceDeviceName = "Post Combustion Indicator Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_PCIPanel()
            : base("Post Combustion Indicator Panel", new Size(66, 66))
        {

            AddIndicator("pc", new Point(6, 6));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/Miscellaneous/pc-panel.png"; }
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
                size: new Size(54, 54),
                onImage: "{M2000C}/Images/Miscellaneous/" + name + "-on.png",
                offImage: "{M2000C}/Images/Miscellaneous/" + name + "-off.png",
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
