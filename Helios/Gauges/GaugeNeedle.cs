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
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class GaugeNeedle : GaugeComponent
    {
        private string _imageFile;
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

        public GaugeNeedle(string imageFile, Point location, Size size, Point center)
            : this(imageFile, location, size, center, 0d)
        {
        }

        public GaugeNeedle(string imageFile, Point location, Size size, Point center, double baseRotation)
        {
            _imageFile = imageFile;
            _location = location;
            _size = size;
            _center = center;
            _baseRotation = baseRotation;
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
                if (value != _imageFile)
                {
                    _imageFile = value;
                    OnDisplayUpdate();
                }
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
            transform.Children.Add(new TranslateTransform((-_center.X + HorizontalOffset) * _xScale, (-_center.Y + VerticalOffset) * _yScale));
            transform.Children.Add(new RotateTransform(_rotation + _baseRotation));
            transform.Children.Add(new TranslateTransform(_location.X * _xScale, _location.Y * _yScale));

            drawingContext.PushTransform(transform);
            DrawImage(drawingContext, _image, _rectangle);
            drawingContext.Pop();
        }

        protected override void OnRefresh(double xScale, double yScale)
        {
            _xScale = xScale;
            _yScale = yScale;

            _rectangle = new Rect(0d, 0d, Math.Max(1d, _size.Width * xScale),  Math.Max(1d, _size.Height * yScale));
            _image = ConfigManager.ImageManager.LoadImage(_imageFile, (int)_rectangle.Width, (int)_rectangle.Height);
        }
    }
}
