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

    [HeliosControl("HELIOS.M2000C.FUEL_BURN_BINGO_PANEL", "Fuel Burn / Bingo Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class Fuel_Burn_Bingo : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 140, 172);
        private string _interfaceDeviceName = "Fuel Panel";
        private string _font = "Helios Virtual Cockpit F/A-18C Hornet IFEI";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private bool _useTextualDisplays = true;
        private ImageDecoration _image;

        public Fuel_Burn_Bingo()
            : base("Fuel Burn / Bingo Panel", new Size(140, 172))
        {
            _image = new ImageDecoration();
            _image.Name = "Fuel Burn Display Background";
            _image.Image = "{M2000C}/Images/Miscellaneous/Fuel_Burn_Bingo_Display_Background.png";
            _image.Alignment = ImageAlignment.Stretched;
            _image.Top = 0d;
            _image.Left = 0d;
            _image.Width = Width;
            _image.Height = Height;
            _image.IsHidden = !_useTextualDisplays;
            Children.Add(_image);
            AddDrum("Bingo Fuel 1 000 kg Selector", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Bingo Fuel 1 000 kg Selector", "(0-9)", "#", new Point(24, 104), new Size(10, 15), new Size(12, 19));
            AddDrum("Bingo Fuel 100 kg Selector", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "Bingo Fuel 100 kg Selector", "(0-9)", "#", new Point(65, 104), new Size(10, 15), new Size(12, 19));
            AddTextDisplay("Fuel Burn Rate Display", new Point(36d, 14d), new Size(68d, 41d), _interfaceDeviceName, "Fuel Burn Rate Display", 32, "000", TextHorizontalAlignment.Center, "");

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/Miscellaneous/Fuel_Burn_Bingo.png"; }
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
                fromCenter: false,
                multiplier: 1d,
                offset: -1d);
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
