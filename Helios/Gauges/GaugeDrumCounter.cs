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
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    public class GaugeDrumCounter : GaugeComponent, IConfigurableImageLocation
    {
        private string _imageFile;
        private ImageSource _image;
        private Rect _imageRect;
        private string _format;
        private Size _digitSize;
        private Size _baseDigitRenderSize;
        private Size _digitRenderSize;
        private Point _location;
        private Point _scaledLocation;
        private double _startRoll = 0d;
        private double _digitsOnTape = 10;

        private double _value;

        public GaugeDrumCounter(string imageFile, Point location, string format, Size digitSize)
            : this(imageFile, location, format, digitSize, digitSize)
        {
        }
        public GaugeDrumCounter(string imageFile, Point location, string format, Size digitSize, Size digitRenderSize) : this(imageFile, location, format, digitSize, digitRenderSize, 10) { }

        public GaugeDrumCounter(string imageFile, Point location, string format, Size digitSize, Size digitRenderSize, double digitsOnTape)
        {
            _imageFile = imageFile;
            _digitSize = digitSize;
            _baseDigitRenderSize = digitRenderSize;
            _location = location;
            _format = format;
            _digitsOnTape = digitsOnTape;
        }

        #region Properties

        public virtual string Image
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

        protected ImageSource ImageSource
        {
            get
            {
                return _image;
            }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    OnDisplayUpdate();
                }
            }
        }

        protected Rect ImageRect
        {
            get
            {
                return _imageRect;
            }
            set
            {
                if (value != _imageRect)
                {
                    _imageRect = value;
                    OnDisplayUpdate();
                }
            }
        }

        protected Point ScaledLocation
        {
            get => _scaledLocation;
            set
            {
                if (!_scaledLocation.Equals(value))
                {
                    _scaledLocation = value;
                }
            }
        }
        protected Size DigitRenderSize
        {
            get => _digitRenderSize;
            set
            {
                if (!_digitRenderSize.Equals(value))
                {
                    _digitRenderSize = value;
                }
            }
        }

        public virtual double Value
        {
            get
            {
                return _value;
            }
            set
            {
                double newValue = Math.Round(value, 2);
                if (!_value.Equals(newValue))
                {
                    _value = value;
                    OnDisplayUpdate();
                }
            }
        }

        public virtual double StartRoll
        {
            get
            {
                return _startRoll;
            }
            set
            {
                _startRoll = value;
            }
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            bool rolling = (_startRoll >= 0);
            double rollingValue = Value;
            double previousDigitValue = (rollingValue % 1d) * 10d;
            double roll = (_startRoll > 0) ? _startRoll : previousDigitValue % 1d;
            double xOffset = 0;
            int digit = 0;

            drawingContext.PushTransform(new TranslateTransform(_scaledLocation.X + ((_format.Length - 1) * _digitRenderSize.Width), _scaledLocation.Y));
            for (int i = _format.Length-1; i >= 0; i--)
            {
                digit++;
                double digitValue = rollingValue % 10d;
                char formatDigit = _format[i];

                double renderValue = digitValue;

                if ("0123456789".Contains(formatDigit))
                {
                    renderValue = double.Parse(formatDigit.ToString());
                    formatDigit = '#';
                }
                else if (formatDigit.Equals('#'))
                {
                    renderValue = Math.Truncate(digitValue);

                    if (rolling && previousDigitValue >= 9)
                    {
                        renderValue += roll;
                    }
                    else
                    {
                        rolling = false;
                    }
                }

                roll = renderValue % 1d;
                renderValue += 1d; // Push up for the last digit
                drawingContext.PushTransform(new TranslateTransform(xOffset, -(renderValue * _digitRenderSize.Height)));
                drawingContext.DrawImage(_image, _imageRect);
                drawingContext.Pop();

                previousDigitValue = digitValue;
                rollingValue = rollingValue / 10d;
                xOffset -= _digitRenderSize.Width;
            }
            drawingContext.Pop();
        }

        protected override void OnRefresh(double xScale, double yScale)
        {
            _scaledLocation = new Point(_location.X * xScale, _location.Y * yScale);
            _digitRenderSize = new Size(_baseDigitRenderSize.Width * xScale, _baseDigitRenderSize.Height * yScale);

            double scaleX = _digitRenderSize.Width / _digitSize.Width;
            double scaleY = _digitRenderSize.Height / _digitSize.Height;
            ImageSource originalImage = ConfigManager.ImageManager.LoadImage(_imageFile);
            if (originalImage != null)
            {
                _imageRect = new Rect(0, 0, (originalImage.Width * scaleX), (originalImage.Height * scaleY));
                _image = ConfigManager.ImageManager.LoadImage(_imageFile, (int)_imageRect.Width, (int)_imageRect.Height);                
            }
        }
        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceImageNames(string oldName, string newName)
        {
            Image = string.IsNullOrEmpty(Image) ? Image : string.IsNullOrEmpty(oldName) ? newName + Image : Image.Replace(oldName, newName);
        }
    }
}
