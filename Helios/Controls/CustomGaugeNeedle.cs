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

namespace GadrocsWorkshop.Helios.Gauges

{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class CustomGaugeNeedle : GaugeComponent
    {
	
        private ImageSource _image;
        private Point _location;
        private Size _size;
        private Point _center;
        private double _baseRotation;
        private double _rotation;
        private Rect _rectangle;
        private double _horizontalOffset;
        private double _verticalOffset;
        private double _xScale = 1.0;
        private double _yScale = 1.0;

        public CustomGaugeNeedle(string imageFile, Point location, Size size, Point center)
            : this(imageFile, location, size, center, 0d)
        {
        }

        public CustomGaugeNeedle(string imageFile, Point location, Size size, Point center, double baseRotation)
        {
			Image = imageFile;
			_location = location;
            _size = size;
            _center = center;
            _baseRotation = baseRotation;
        }

        #region Properties

        public string Image { get; set; }

		public double Tape_Width
		{
			get
			{
				return _size.Width;
			}
			set
			{
				_size.Width = value;
			}
		}

		public double Tape_Height
		{
			get
			{
				return _size.Height;
			}
			set
			{
				_size.Height = value;
			}
		}

		public double TapePosX
		{
			get
			{
				return _location.X;
			}
			set
			{
				_location.X = value;
			}
		}

		public double TapePosY
		{
			get
			{
				return _location.Y;
			}
			set
			{
				_location.Y = value;
			}
		}

		public double Tape_CenterX
		{
			get
			{
				return _center.X;
			}
			set
			{
				_center.X = value;
			}
		}

		public double Tape_CenterY
		{
			get
			{
				return _center.Y;
			}
			set
			{
				_center.Y = value;
			}
		}






		public double BaseRotation
        {
            get
            {
                return _baseRotation;
            }
            set
            {
                _baseRotation = value;
                OnDisplayUpdate();
            }
        }

        public double Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                double newValue = Math.Round(value, 1);
                if (!_rotation.Equals(newValue))
                {
                    _rotation = value;
                    OnDisplayUpdate();
                }
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return _horizontalOffset;
            }
            set
            {
                double newValue = Math.Round(value, 1);
                if (!_horizontalOffset.Equals(newValue))
                {
                    _horizontalOffset = value;
                    OnDisplayUpdate();
                }
            }
        }
        
        public double VerticalOffset
        {
            get
            {
                return _verticalOffset;
            }
            set
            {
                double newValue = Math.Round(value, 1);
                if (!_verticalOffset.Equals(newValue))
                {
                    _verticalOffset = value;
                    OnDisplayUpdate();
                }
            }
        }
        
        #endregion

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform((_location.X  + HorizontalOffset) * _xScale, (_location.Y  + VerticalOffset) * _yScale));
            transform.Children.Add(new RotateTransform(_rotation + _baseRotation, _center.X * _xScale, _center.Y * _yScale));
          

            drawingContext.PushTransform(transform);
            RenderEffect(drawingContext, _image, _rectangle);
            drawingContext.Pop();
        }

        protected override void OnRefresh(double xScale, double yScale)
        {
            _xScale = xScale;
            _yScale = yScale;

            _rectangle = new Rect(0d, 0d, Math.Max(1d, _size.Width * xScale),  Math.Max(1d, _size.Height * yScale));
            
            if (ImageRefresh is true && ConfigManager.ImageManager is IImageManager3 refreshCapable)
            {
                _image = refreshCapable.LoadImage(Image, (int)_rectangle.Width, (int)_rectangle.Height, LoadImageOptions.ReloadIfChangedExternally);
            }
            else
            {
                _image = ConfigManager.ImageManager.LoadImage(Image, (int)_rectangle.Width, (int)_rectangle.Height);
            }

            if (_image == null)
			{
				_image = ConfigManager.ImageManager.LoadImage("{Helios}/Images/General/missing_image.png", (int)_rectangle.Width, (int)_rectangle.Height);
			}
        }
    }
}
