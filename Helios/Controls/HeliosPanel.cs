//  Copyright 2014 Craig Courtney
//  Copyright 2021 Helios Contributors
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

    [HeliosControl("Helios.Panel", "Generic Bezel", "Panels", typeof(HeliosPanelRenderer))]
    public class HeliosPanel : HeliosVisualContainer
    {
        private bool _drawBorder = true;

        private bool _fillBackground = true;
        private string _backgroundImageFile = "";
        private ImageAlignment _backgroundAlignment = ImageAlignment.Stretched;
        private Color _backgroundColor = Color.FromArgb(255, 30, 30, 30);

        private string _topBorderImageFile = "{Helios}/Images/Panels/panel-top.png";
        private string _rightBorderImageFile = "{Helios}/Images/Panels/panel-right.png";
        private string _bottomBorderImageFile = "{Helios}/Images/Panels/panel-bottom.png";
        private string _leftBorderImageFile = "{Helios}/Images/Panels/panel-left.png";

        private string _topLeftCornerImageFile = "{Helios}/Images/Panels/panel-top-left.png";
        private string _topRigthCornerImageFile = "{Helios}/Images/Panels/panel-top-right.png";
        private string _bottomLeftCornerImageFile = "{Helios}/Images/Panels/panel-bottom-left.png";
        private string _bottomRightCornerImageFile = "{Helios}/Images/Panels/panel-bottom-right.png";
        private double _opacity = 1d; 

        public HeliosPanel()
            : base("Panel", new Size(300,300))
        {
            // no code
        }

        #region Properties

        public bool AllowInteractionFull { get; set; } = false;
        public bool AllowInteractionNone { get; set; } = true;
        public bool AllowInteractionLegacy { get; set; } = false;

        public bool DrawBorder
        {
            get
            {
                return _drawBorder;
            }
            set
            {
                if (!_drawBorder.Equals(value))
                {
                    bool oldValue = _drawBorder;
                    _drawBorder = value;
                    OnPropertyChanged("DrawBorder", oldValue, value, true);
                    OnDisplayUpdate();
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
                    _fillBackground = value;
                    OnPropertyChanged("FillBackground", value, !value, true);
                    OnDisplayUpdate();
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

        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                if (!_backgroundColor.Equals(value))
                {
                    Color oldValue = _backgroundColor;
                    _backgroundColor = value;
                    OnPropertyChanged("BackgroundColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string TopBorderImage
        {
            get
            {
                return _topBorderImageFile;
            }
            set
            {
                if ((_topBorderImageFile == null && value != null)
                    || (_topBorderImageFile != null && !_topBorderImageFile.Equals(value)))
                {
                    string oldValue = _topBorderImageFile;
                    _topBorderImageFile = value;
                    OnPropertyChanged("TopBorderImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string RightBorderImage
        {
            get
            {
                return _rightBorderImageFile;
            }
            set
            {
                if ((_rightBorderImageFile == null && value != null)
                    || (_rightBorderImageFile != null && !_rightBorderImageFile.Equals(value)))
                {
                    string oldValue = _rightBorderImageFile;
                    _rightBorderImageFile = value;
                    OnPropertyChanged("RightBorderImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string BottomBorderImage
        {
            get
            {
                return _bottomBorderImageFile;
            }
            set
            {
                if ((_bottomBorderImageFile == null && value != null)
                    || (_bottomBorderImageFile != null && !_bottomBorderImageFile.Equals(value)))
                {
                    string oldValue = _bottomBorderImageFile;
                    _bottomBorderImageFile = value;
                    OnPropertyChanged("BottomBorderImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string LeftBorderImage
        {
            get
            {
                return _leftBorderImageFile;
            }
            set
            {
                if ((_leftBorderImageFile == null && value != null)
                    || (_leftBorderImageFile != null && !_leftBorderImageFile.Equals(value)))
                {
                    string oldValue = _leftBorderImageFile;
                    _leftBorderImageFile = value;
                    OnPropertyChanged("LeftBorderImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string TopLeftCornerImage
        {
            get
            {
                return _topLeftCornerImageFile;
            }
            set
            {
                if ((_topLeftCornerImageFile == null && value != null)
                    || (_topLeftCornerImageFile != null && !_topLeftCornerImageFile.Equals(value)))
                {
                    string oldValue = _topLeftCornerImageFile;
                    _topLeftCornerImageFile = value;
                    OnPropertyChanged("TopLeftCornerImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string TopRightCornerImage
        {
            get
            {
                return _topRigthCornerImageFile;
            }
            set
            {
                if ((_topRigthCornerImageFile == null && value != null)
                    || (_topRigthCornerImageFile != null && !_topRigthCornerImageFile.Equals(value)))
                {
                    string oldValue = _topRigthCornerImageFile;
                    _topRigthCornerImageFile = value;
                    OnPropertyChanged("TopRightCornerImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string BottomLeftCornerImage
        {
            get
            {
                return _bottomLeftCornerImageFile;
            }
            set
            {
                if ((_bottomLeftCornerImageFile == null && value != null)
                    || (_bottomLeftCornerImageFile != null && !_bottomLeftCornerImageFile.Equals(value)))
                {
                    string oldValue = _bottomLeftCornerImageFile;
                    _bottomLeftCornerImageFile = value;
                    OnPropertyChanged("BottomLeftCornerImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string BottomRightCornerImage
        {
            get
            {
                return _bottomRightCornerImageFile;
            }
            set
            {
                if ((_bottomRightCornerImageFile == null && value != null)
                    || (_bottomRightCornerImageFile != null && !_bottomRightCornerImageFile.Equals(value)))
                {
                    string oldValue = _bottomRightCornerImageFile;
                    _bottomRightCornerImageFile = value;
                    OnPropertyChanged("BottomRightCornerImage", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                double oldValue = _opacity;
                _opacity = value;
                if (value != oldValue)
                {
                    OnPropertyChanged("Opacity", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        #endregion

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));

            writer.WriteStartElement("Background");
            if (FillBackground)
            {
                writer.WriteElementString("Color", cc.ConvertToInvariantString(BackgroundColor));
            }
            if (!string.IsNullOrWhiteSpace(BackgroundImage))
            {
                writer.WriteElementString("Image", BackgroundImage);
                writer.WriteElementString("ImageAlignment", BackgroundAlignment.ToString());
            }
            writer.WriteEndElement();

            if (DrawBorder)
            {
                writer.WriteStartElement("Border");
                writer.WriteElementString("TopLeftCorner", TopLeftCornerImage);
                writer.WriteElementString("TopBorder", TopBorderImage);
                writer.WriteElementString("TopRightCorner", TopRightCornerImage);
                writer.WriteElementString("RightBorder", RightBorderImage);
                writer.WriteElementString("BottomRightCorner", BottomRightCornerImage);
                writer.WriteElementString("BottomBorder", BottomBorderImage);
                writer.WriteElementString("BottomLeftCorner", BottomLeftCornerImage);
                writer.WriteElementString("LeftBorder", LeftBorderImage);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Interaction");
            writer.WriteElementString("AllowInteractionFull", AllowInteractionFull.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("AllowInteractionNone", AllowInteractionNone.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("AllowInteractionLegacy", AllowInteractionLegacy.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));

            reader.ReadStartElement("Background");
            if (reader.Name.Equals("Color"))
            {
                BackgroundColor = (Color)cc.ConvertFromInvariantString(reader.ReadElementString("Color"));
                FillBackground = BackgroundColor.A > 0;
            }
            else
            {
                FillBackground = false;
            }
            if (reader.Name.Equals("Image"))
            {
                BackgroundImage = reader.ReadElementString("Image");
                if (reader.Name.Equals("ImageAlignment"))
                {
                    BackgroundAlignment = (ImageAlignment)Enum.Parse(typeof(ImageAlignment), reader.ReadElementString("ImageAlignment"));
                }
            }
            else
            {
                BackgroundImage = "";
            }
            if (reader.Name.Equals("Background"))
            {
                reader.ReadEndElement();
            }

            if (reader.Name.Equals("Border"))
            {
                reader.ReadStartElement();
                DrawBorder = true;
                TopLeftCornerImage = reader.ReadElementString("TopLeftCorner");
                TopBorderImage = reader.ReadElementString("TopBorder");
                TopRightCornerImage = reader.ReadElementString("TopRightCorner");
                RightBorderImage = reader.ReadElementString("RightBorder");
                BottomRightCornerImage = reader.ReadElementString("BottomRightCorner");
                BottomBorderImage = reader.ReadElementString("BottomBorder");
                BottomLeftCornerImage = reader.ReadElementString("BottomLeftCorner");
                LeftBorderImage = reader.ReadElementString("LeftBorder");
                reader.ReadEndElement();
            }
            else
            {
                DrawBorder = false;
            }

            if (reader.Name.Equals("Interaction"))
            {
                reader.ReadStartElement();
                AllowInteractionFull = bool.Parse(reader.ReadElementString("AllowInteractionFull"));
                AllowInteractionNone = bool.Parse(reader.ReadElementString("AllowInteractionNone"));
                AllowInteractionLegacy = bool.Parse(reader.ReadElementString("AllowInteractionLegacy"));
                reader.ReadEndElement();
            }
            else
            {
                AllowInteractionFull = false;
                AllowInteractionNone = false;
                AllowInteractionLegacy = true;
            }
        }

        public override bool HitTest(Point location)
        {
            bool retVal = FillBackground || DrawBorder;

            if (AllowInteractionFull)
            {
                retVal = false;
            }
            else if (AllowInteractionNone)
            {
                retVal = true;
            }

            return retVal;
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
