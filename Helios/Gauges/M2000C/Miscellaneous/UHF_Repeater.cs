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
    using System.Xml;

    [HeliosControl("HELIOS.M2000C.UHF_REPEATER_DISPLAY", "UHF Repeater Display", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class UHF_Repeater : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 130, 148);
        private string _interfaceDeviceName = "U/VHF";
        private string _font = "Helios Virtual Cockpit F/A-18C Hornet IFEI";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private bool _useTextualDisplays = true;
        private ImageDecoration _image;

        public UHF_Repeater()
            : base("UHF Repeater Display", new Size(130, 148))
        {
            _image = new ImageDecoration();
            _image.Name = "UHF Repeater Display Background";
            _image.Image = "{M2000C}/Images/Miscellaneous/UHF_Repeater_Display_Background.png";
            _image.Alignment = ImageAlignment.Stretched;
            _image.Top = 0d;
            _image.Left = 0d;
            _image.Width = Width;
            _image.Height = Height;
            _image.IsHidden = !_useTextualDisplays;
            Children.Add(_image);

            AddTextDisplay("UHF Upper Comm Information", new Point(5d, 38d), new Size(122d, 36d), _interfaceDeviceName, "UHF Upper Comm Information", 32, "********", TextHorizontalAlignment.Left, "");
            AddTextDisplay("UHF Lower Comm Information", new Point(5d, 75d), new Size(122d, 36d), _interfaceDeviceName, "UHF Lower Comm Information", 32, "********", TextHorizontalAlignment.Left, "");

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/Miscellaneous/UHF_Repeater.png"; }
        }

        public bool UseTextualDisplays
        {
            get => _useTextualDisplays;
            set
            {
                if (value != _useTextualDisplays)
                {
                    _useTextualDisplays = value;
                    foreach (HeliosVisual child in this.Children)
                    {
                        if (child is TextDisplay textDisplay)
                        {
                            textDisplay.IsHidden = !_useTextualDisplays;
                        }
                    }
                    _image.IsHidden = !_useTextualDisplays;
                    Refresh();
                }
            }
        }

        #endregion
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name.Equals("UseTextualDisplays"))
            {
                UseTextualDisplays = bool.Parse(reader.ReadElementString("UseTextualDisplays"));
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("UseTextualDisplays", _useTextualDisplays.ToString(CultureInfo.InvariantCulture));
        }

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

        private void AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, double baseFontsize,
                string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
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
            display.IsHidden = !_useTextualDisplays;
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
