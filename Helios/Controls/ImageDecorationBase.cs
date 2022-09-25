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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Base.Image", "User Image", "Panel Decorations", typeof(ImageDecorationRenderer))]
    public class ImageDecoration : ImageDecorationBase
    {
        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            Image = reader.ReadElementString("Image");
            if (reader.Name.Equals("Alignment"))
            {
                Alignment = (ImageAlignment)Enum.Parse(typeof(ImageAlignment), reader.ReadElementString("Alignment"));
            }
            if (reader.Name.Equals("CornerRadius"))
            {
                CornerRadius = Double.Parse(reader.ReadElementString("CornerRadius"), CultureInfo.InvariantCulture);
            }
            if (reader.Name.Equals("Border"))
            {
                reader.ReadStartElement("Border");
                BorderThickness = Double.Parse(reader.ReadElementString("Thickness"), CultureInfo.InvariantCulture);
                BorderColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("Color"));
                reader.ReadEndElement();
            }
            else
            {
                BorderThickness = 0d;
            }
            // Load base after image so size is properly persisted.
            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            writer.WriteElementString("Image", Image);
            writer.WriteElementString("Alignment", Alignment.ToString());
            writer.WriteElementString("CornerRadius", CornerRadius.ToString(CultureInfo.InvariantCulture));
            if (BorderThickness != 0d)
            {
                writer.WriteStartElement("Border");
                writer.WriteElementString("Thickness", BorderThickness.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Color", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, BorderColor));
                writer.WriteEndElement();
            }
            // Save base after image so size is properly persisted.
            base.WriteXml(writer);
        }
    }

    public class ImageDecorationBase : HeliosVisual
    {
        private string _imageFile = "";
        private ImageAlignment _alignment = ImageAlignment.Centered;
        private double _borderThickness = 0d;
        private double _cornerRadius = 0d;
        private Color _borderColor = Colors.Black;

        /// <summary>
        /// backing field for property DesignTimeOnly, contains
        /// true if this decoration is only shown at design time and hidden
        /// at run time
        /// </summary>
        private bool _designTimeOnly;

        public ImageDecorationBase()
            : base("Image", new Size(100, 100))
        {
            IsSnapTarget = false;
        }

        protected ImageDecorationBase(string name)
            : base(name, new Size(100, 100))
        {
            IsSnapTarget = false;
        }

        internal ImageDecorationBase(string name, Size size)
            : base(name, size)
        {
            IsSnapTarget = false;
        }

        #region Properties

        public string Image
        {
            get
            {
                return _imageFile;
            }
            set
            {
                if ((_imageFile == null && value != null)
                    || (_imageFile != null && !_imageFile.Equals(value)))
                {
                    string oldValue = _imageFile;
                    _imageFile = value;

                    ImageSource image = ConfigManager.ImageManager.LoadImage(_imageFile);
                    if (image != null)
                    {
                        Width = image.Width;
                        Height = image.Height;
                    }
                    OnPropertyChanged("Image", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public ImageAlignment Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                if (!_alignment.Equals(value))
                {
                    ImageAlignment oldValue = _alignment;
                    _alignment = value;
                    OnPropertyChanged("Alignment", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                if ((_borderColor == null && value != null)
                    || (_borderColor != null && !_borderColor.Equals(value)))
                {
                    Color oldValue = _borderColor;
                    _borderColor = value;
                    OnPropertyChanged("BorderColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double BorderThickness
        {
            get
            {
                return _borderThickness;
            }
            set
            {
                if (!_borderThickness.Equals(value))
                {
                    double oldValue = _borderThickness;
                    _borderThickness = value;
                    OnPropertyChanged("BorderThickness", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double CornerRadius
        {
            get
            {
                return _cornerRadius;
            }
            set
            {
                if (!_cornerRadius.Equals(value))
                {
                    double oldValue = _cornerRadius;
                    _cornerRadius = value;
                    OnPropertyChanged("CornerRadius", oldValue, value, true);
                    Refresh();
                }
            }
        }

        /// <summary>
        /// true if this decoration is only shown at design time and hidden
        /// at run time
        /// </summary>
        public bool DesignTimeOnly
        {
            get => _designTimeOnly;
            set
            {
                if (_designTimeOnly == value) return;
                bool oldValue = _designTimeOnly;
                _designTimeOnly = value;
                OnPropertyChanged("DesignTimeOnly", oldValue, value, true);
            }
        }

        #endregion

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
