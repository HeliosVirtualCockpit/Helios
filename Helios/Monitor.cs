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

using System.Diagnostics;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    [DebuggerDisplay("Monitor at {" + nameof(DisplayRectangle) + "}")]
    public class Monitor : HeliosVisualContainer
    {
        private MonitorRenderer _renderer;

        private bool _fillBackground;
        private Color _backgroundColor = Colors.DarkGray;
        private string _backgroundImageFile = "";
        private ImageAlignment _backgroundAlignment = ImageAlignment.Stretched;
        private bool _alwaysOnTop = true;

        /// <summary>
        /// backing field for property Orientation, contains
        /// the orientation of this monitor object
        /// </summary>
        private DisplayOrientation _orientation;

        public Monitor()
            : this(0, 0, 1024, 768, DisplayOrientation.DMDO_DEFAULT)
        {
        }

        public Monitor(double left, double top, double width, double height, DisplayOrientation orientation)
            : base("Monitor", new Size(width, height))
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
            Orientation = orientation;
        }

        public Monitor(Monitor display)
            : this(display.Left, display.Top, display.Width, display.Height, display.Orientation)
        {
        }

        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((BackgroundImage?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                OnPropertyChanged("BackgroundImage", BackgroundImage, BackgroundImage, true);
                Refresh();
            }
            return ImageRefresh;
        }
        #region Properties

        public override string TypeIdentifier => "Helios.Monitor";

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        /// <summary>
        /// the orientation of this monitor object
        /// </summary>
        public DisplayOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation == value) return;
                DisplayOrientation oldValue = _orientation;
                _orientation = value;
                OnPropertyChanged("Orientation", oldValue, value, true);
            }
        }

        public override HeliosVisualRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = new MonitorRenderer();
                    _renderer.Visual = this;
                }
                return _renderer;
            }
        }

        public bool FillBackground
        {
            get
            {
                return _fillBackground;
            }
            set
            {
                if (!_fillBackground.Equals(value))
                {
                    bool oldValue = _fillBackground;
                    _fillBackground = value;
                    OnPropertyChanged("FillBackground", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                if ((_backgroundColor == null && value != null)
                    || (_backgroundColor != null && !_backgroundColor.Equals(value)))
                {
                    Color oldValue = _backgroundColor;
                    _backgroundColor = value;
                    OnPropertyChanged("BackgroundColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string BackgroundImage
        {
            get
            {
                return _backgroundImageFile;
            }
            set
            {
                if ((_backgroundImageFile == null && value != null)
                    || (_backgroundImageFile != null && !_backgroundImageFile.Equals(value)))
                {
                    string oldValue = _backgroundImageFile;
                    _backgroundImageFile = value;
                    OnPropertyChanged("BackgroundImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public ImageAlignment BackgroundAlignment
        {
            get
            {
                return _backgroundAlignment;
            }
            set
            {
                if (!_backgroundAlignment.Equals(value))
                {
                    ImageAlignment oldValue = _backgroundAlignment;
                    _backgroundAlignment = value;
                    OnPropertyChanged("BackgroundAlignment", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public bool AlwaysOnTop
        {
            get
            {
                return _alwaysOnTop;
            }
            set
            {
                if (!_alwaysOnTop.Equals(value))
                {
                    _alwaysOnTop = value;
                    OnPropertyChanged("AlwaysOnTop", !value, value, true);
                }
            }
        }

        public int SuppressMouseAfterTouchDuration { get; set; }

        public bool IsPrimaryDisplay => (Math.Abs(Top) < 0.00001 && Math.Abs(Left) < 0.00001);

        #endregion


        public void CopyGeometry(Monitor display)
        {
            using (new HeliosUndoBatch())
            {
                Rectangle = display.Rectangle;
                Orientation = display.Orientation;
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));

            base.ReadXml(reader);

            Orientation = (DisplayOrientation)Enum.Parse(typeof(DisplayOrientation), reader.ReadElementString("Orientation"));

            // REVISIT: this assumes the order of XML elements and also assumes that there are no foreign elements present
            // and that all properties are located just ahead of the "Children" element

            if (reader.Name.Equals("AlwaysOnTop"))
            {
                _alwaysOnTop = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("AlwaysOnTop"));
            }

            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("Background");
                if (reader.Name.Equals("Image"))
                {
                    BackgroundImage = reader.ReadElementString("Image");
                    BackgroundAlignment = (ImageAlignment)Enum.Parse(typeof(ImageAlignment), reader.ReadElementString("ImageAlignment"));
                }
                else
                {
                    BackgroundImage = "";
                }
                if (reader.Name.Equals("Color"))
                {
                    BackgroundColor = (Color)cc.ConvertFromInvariantString(reader.ReadElementString("Color"));
                    FillBackground = true;
                }
                else
                {
                    FillBackground = false;
                }

                reader.ReadEndElement();
            }
            else
            {
                FillBackground = false;
                reader.Read();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            TypeConverter ic = TypeDescriptor.GetConverter(typeof(int));

            base.WriteXml(writer);

            writer.WriteElementString("Orientation", Orientation.ToString());
            writer.WriteElementString("AlwaysOnTop", bc.ConvertToInvariantString(AlwaysOnTop));

            writer.WriteStartElement("Background");
            if (!string.IsNullOrWhiteSpace(BackgroundImage))
            {
                writer.WriteElementString("Image", BackgroundImage);
                writer.WriteElementString("ImageAlignment", BackgroundAlignment.ToString());
            }

            if (FillBackground)
            {
                writer.WriteElementString("Color", cc.ConvertToInvariantString(BackgroundColor));
            }

            writer.WriteEndElement();
        }

        public override bool HitTest(Point location)
        {
            return FillBackground || !String.IsNullOrWhiteSpace(BackgroundImage);
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
