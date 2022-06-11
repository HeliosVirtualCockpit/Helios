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

    [HeliosControl("Helios.Base.ImageTranslucent", "Translucent Image", "Panel Decorations", typeof(ImageTranslucentRenderer))]
    public class ImageTranslucent : HeliosVisual
    {
		private double _opacity= 0.5d;
		private double _default_opacity = 0.5d;
		private string _imageFile = "";
        private ImageAlignment _alignment = ImageAlignment.Centered;
        private double _borderThickness = 0d;
        private double _cornerRadius = 0d;
        private Color _borderColor = Colors.Black;

		private double _value = 0.5d;
		private HeliosValue _opacityValue;


		public ImageTranslucent()
            : base("Translucent Image", new Size(100, 100))
        {
            IsSnapTarget = false;
			_opacityValue = new HeliosValue(this, new BindingValue(0d), "", "opacity", "Current opacity of the Image.", "", BindingValueUnits.Numeric);
			_opacityValue.Execute += new HeliosActionHandler(SetValue_Execute);
			Values.Add(_opacityValue);
			Actions.Add(_opacityValue);
		}

        #region Properties

        public bool AllowInteraction { get; set; } = false;

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
					else
					{
						Width =100d;
						Height = 100d;
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


		public double Default_Opacity
		{
			get
			{
				return _default_opacity;
			}
			set
			{
				if (!_default_opacity.Equals(value))
				{
					double oldValue = _default_opacity;
					_default_opacity = value;
					OnPropertyChanged("Default_Opacity", oldValue, value, true);
					SetDefaultOpacity();
					Refresh();
				}
			}
		}

		public double Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (!_value.Equals(value))
				{
					double oldValue = _value;
					_value = value;
					_opacityValue.SetValue(new BindingValue(_value), BypassTriggers);
					OnPropertyChanged("Value", oldValue, value, true);
					SetOpacity();
				}
			}
		}


		#endregion

		#region Actions

		void SetValue_Execute(object action, HeliosActionEventArgs e)
		{
			try
			{
				BeginTriggerBypass(e.BypassCascadingTriggers);
				Value = e.Value.DoubleValue;
				EndTriggerBypass(e.BypassCascadingTriggers);
			}
			catch
			{
				// No-op if the parse fails we won't set the position.
			}
		}

		#endregion

		public double ImageOpacity
		{
			get
			{
				return _opacity;
			}
			protected set
			{
				if (!_opacity.Equals(value))
				{
					double oldValue = _opacity;
					_opacity = value;
					OnPropertyChanged("ImageOpacity", oldValue, value, false);
					OnDisplayUpdate();
				}
			}
		}

		private void SetOpacity()
		{
			ImageOpacity = Value;
		}

		private void SetDefaultOpacity()
		{
			ImageOpacity = _default_opacity;
		}

		public override void Reset()
		{
            base.Reset();

            BeginTriggerBypass(true);
			Value = _default_opacity;
			EndTriggerBypass(true);
		}

        public override bool HitTest(Point location)
        {
            // return false to allow pass through interaction with underlying controls
            return !AllowInteraction;
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
			Default_Opacity = double.Parse(reader.ReadElementString("DefaultOpacity"), CultureInfo.InvariantCulture);

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

            if (reader.Name.Equals("Interaction"))
            {
                reader.ReadStartElement("Interaction");
                AllowInteraction = bool.Parse(reader.ReadElementString("AllowInteraction"));
                reader.ReadEndElement();
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
			writer.WriteElementString("DefaultOpacity", Default_Opacity.ToString(CultureInfo.InvariantCulture));
			if (BorderThickness != 0d)
            {
                writer.WriteStartElement("Border");
                writer.WriteElementString("Thickness", BorderThickness.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Color", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, BorderColor));
                writer.WriteEndElement();
            }
            writer.WriteStartElement("Interaction");
            writer.WriteElementString("AllowInteraction", AllowInteraction.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Save base after image so size is properly persisted.
            base.WriteXml(writer);
        }
    }
}
