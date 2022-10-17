//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Gauges.BarGauge", "Bar Gauge", "Custom Controls", typeof(GaugeRenderer), HeliosControlFlags.None)]
    internal class BarGauge : BaseGauge
    {

        private string _imageFile = "";
        private ImageAlignment _alignment = ImageAlignment.Centered;
        private double _borderThickness = 0d;
        private double _cornerRadius = 0d;
        private Color _borderColor = Colors.Black;
        private bool _designTimeOnly;

        private HeliosValue _barSegmentStart;
        private HeliosValue _barSegmentEnd;

        private double _barSegmentStartValue;
        private double _barSegmentEndValue;
        private double _segmentCount;
        private double _segmentHeight;
        private Size _size;

        private GaugeImage _barImage;

        public BarGauge() : this("Segmented Gauge", new Size(40,540), "{Helios}/Images/Custom/SegmentBarDisplay30.xaml", 30) 
        { }
        public BarGauge(string name, Size size, string image, double segmentCount)
            : base(name, size)
        {
            _imageFile = image;
            SegmentCount = segmentCount;
            _segmentHeight = size.Height / segmentCount;
            _size = size;
            double barDisplayX = 0; double barDisplayY = 0;
            _barImage = new GaugeImage(image, new Rect(barDisplayX, barDisplayY,size.Width, size.Height));
            _barImage.Clip = new RectangleGeometry(new Rect(0, 0, size.Width, size.Height));
            _barImage.IsHidden = false;
            Components.Add(_barImage);

            _barSegmentStart = new HeliosValue(this, new BindingValue(false), "Bar Segment Display", "Start Segment", "The first segment of a run.", "Number", BindingValueUnits.Numeric);
            _barSegmentStart.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_barSegmentStart);
            _barSegmentEnd = new HeliosValue(this, new BindingValue(false), "Bar Segment Display", "Finish Segment", "The last segment of a run.", "Number", BindingValueUnits.Numeric);
            _barSegmentEnd.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_barSegmentEnd);
        }

        protected void Flag_Execute(object action, HeliosActionEventArgs e)
        {
            HeliosValue hAction = (HeliosValue)action;
            Boolean hActionVal = !(e.Value.DoubleValue > 0d ? true : false);
            switch (hAction.Name)
            {
                case "Start Segment":
                    _barSegmentStartValue = e.Value.DoubleValue;                                                   
                    _barSegmentStart.SetValue(e.Value, e.BypassCascadingTriggers);
                    break;
                case "Finish Segment":
                    _barSegmentEndValue = e.Value.DoubleValue;
                    _barSegmentEnd.SetValue(e.Value, e.BypassCascadingTriggers);
                    _barImage.Clip = new RectangleGeometry(new Rect(0, (_size.Height - (_barSegmentEndValue + 0 ) * _segmentHeight), _size.Width, (_barSegmentEndValue  - _barSegmentStartValue +1 ) * _segmentHeight));
                    Refresh();
                    break;
                default:
                    break;
            }
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
                        _barImage.Clip = new RectangleGeometry(new Rect(0, 0, image.Width, image.Height));
                        _barImage.Image = _imageFile;
                        _segmentHeight = image.Height / _segmentCount;
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

        public double SegmentCount
        {
            get
            {
                return _segmentCount;
            }
            set
            {
                if (!_segmentCount.Equals(value))
                {
                    double oldValue = _segmentCount;
                    _segmentCount = value;
                    _segmentHeight = _size.Height / _segmentCount;
                    OnPropertyChanged("SegmentCount", oldValue, value, true);
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
            if (reader.Name.Equals("SegmentCount"))
            {
                SegmentCount = Double.Parse(reader.ReadElementString("SegmentCount"), CultureInfo.InvariantCulture);
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
            writer.WriteElementString("SegmentCount", SegmentCount.ToString(CultureInfo.InvariantCulture));

            // Save base after image so size is properly persisted.
            base.WriteXml(writer);
        }
    }
}

