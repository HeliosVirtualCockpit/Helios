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
    using GadrocsWorkshop.Helios.Controls;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;


    public class GaugeImage : GaugeComponent
    {
        private string _imageFile;
        private ImageSource _image;
        private Rect _rectangle;
        private Rect _imageRectangle;
        private double _opacity;
		private double _rotation;

        public GaugeImage(string imageFile, Rect location, double opacity = 1, double rotation = 0)
        {
            _imageFile = imageFile;
            _rectangle = location;
            _opacity = opacity;
			_rotation = rotation;
        }

        #region Properties

        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                if (value != _opacity)
                {
                    _opacity = value;
                    OnDisplayUpdate();
                }
            }
        }

		public string Image
		{
			get
			{
				return _imageFile;
			}
			set
			{
				if (value != _imageFile)
				{
					_imageFile = value;
					OnDisplayUpdate();
				}
			}
		}

		public double Width
		{
			get
			{
				return _rectangle.Width;
			}
			set
			{
				if (value != _rectangle.Width)
				{
					_rectangle.Width = value;
					OnDisplayUpdate();
				}
			}
		}

		public double Height
		{
			get
			{
				return _rectangle.Height;
			}
			set
			{
				if (value != _rectangle.Height)
				{
					_rectangle.Height = value;
					OnDisplayUpdate();
				}
			}
		}

		public double PosX
		{
			get
			{
				return _rectangle.X;
			}
			set
			{
				if (value != _rectangle.X)
				{
					_rectangle.X = value;
					OnDisplayUpdate();
				}
			}
		}

		public double PosY
		{
			get
			{
				return _rectangle.Y;
			}
			set
			{
				if (value != _rectangle.Y)
				{
					_rectangle.Y = value;
					OnDisplayUpdate();
				}
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
                if (value != _rotation)
                {
                    _rotation = value;
                    OnDisplayUpdate();
                }
            }
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_opacity >= 1.0 && _rotation == 0)
            {
                DrawImage(drawingContext, _image, _imageRectangle);
                return;
            }
            drawingContext.PushTransform(new RotateTransform(_rotation, _imageRectangle.X + (_imageRectangle.Width/2), _imageRectangle.Y + (_imageRectangle.Height / 2)));
            drawingContext.PushOpacity(_opacity);
            DrawImage(drawingContext, _image, _imageRectangle);
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected override void OnRefresh(double xScale, double yScale)
        {
            _imageRectangle = new Rect(_rectangle.X * xScale, _rectangle.Y * yScale, _rectangle.Width * xScale, _rectangle.Height * yScale);
            IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
            LoadImageOptions loadOptions = ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None; 
			_image = refreshCapableImage.LoadImage(_imageFile, (int)_imageRectangle.Width, (int)_imageRectangle.Height, loadOptions);
        }
    }
}
