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
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Text;

    public class GaugeTextDisplay : GaugeComponent
    {
        private bool _useParseDicationary = false;
        private string _textValue = "";
        private string _rawValue = "";
        private string _textValueTest = "O";
        private string _onImage = "{Helios}/Images/Indicators/anunciator.png";
        private bool _useBackground = true;    // displaying the background or not
        private Color _onTextColor = Color.FromArgb(0xff, 0x40, 0xb3, 0x29);
        private Color _backgroundColor = Color.FromArgb(0xff, 0, 0, 0);
        private TextFormat _textFormat = new TextFormat();
        private HeliosValue _value;
        private bool _useBackground = true;    // displaying the background or not
        private Color _onTextColor = Color.FromArgb(0xff, 0x40, 0xb3, 0x29);
        private Color _backgroundColor = Color.FromArgb(0xff, 0, 0, 0);

        public GaugeTextDisplay(string imageFile, Point location, string format, Size digitSize)
            : this("TextDisplay", new System.Windows.Size(100, 50)
            {
                _textFormat.VerticalAlignment = TextVerticalAlignment.Center;
        _textFormat.HorizontalAlignment = TextHorizontalAlignment.Left;
            _textFormat.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TextFormat_PropertyChanged);
            _value = new HeliosValue(this, new BindingValue(false), "", "TextDisplay", "Value of this Text Display", "String Value Unit.", BindingValueUnits.Text);
            _value.Execute += new HeliosActionHandler(On_Execute);
        Values.Add(_value);
            Actions.Add(_value);
        }

    public GaugeTextDisplay(string imageFile, Point location, string format, Size digitSize, Size digitRenderSize)
    {
        _imageFile = imageFile;
        _digitSize = digitSize;
        _baseDigitRenderSize = digitRenderSize;
        _location = location;
        _format = format;
    }

    #region Properties

    public double Value
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

    public double StartRoll
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
        for (int i = _format.Length - 1; i >= 0; i--)
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
            renderValue += 1d; // Push up for the 9
            drawingContext.PushTransform(new TranslateTransform(xOffset, -(renderValue * _digitRenderSize.Height)));
            DrawImage(drawingContext, _image, _imageRect);
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
}
}
