//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

using System;

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Schema;

    [HeliosControl("Helios.Base.Text", "Label", "Panel Decorations", typeof(TextDecorationRenderer))]
    public class TextDecoration : HeliosVisual, ITextControl
    {
        private readonly HeliosValue _textValue;
        private string _defaultText = "";
        private TextFormat _format = new TextFormat();
        private Color _fontColor = Colors.White;
        private Color _backgroundColor = Color.FromArgb(0xff, 30, 30, 30);
        private Color _fontColorDefault = Colors.White;
        private Color _backgroundColorDefault = Color.FromArgb(0xff, 30, 30, 30);
        private bool _fillBackground = false;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly HeliosValue _brightnessValue;
        private readonly HeliosAction _incrementBrightnessAction;
        private readonly HeliosAction _decrementBrightnessAction;
        private double _displayBrightness = 1.0;
        private HeliosValue _opacityValue;
        private HeliosAction _incrementOpacityAction;
        private HeliosAction _decrementOpacityAction;
        private HeliosTrigger _incrementOpacityTrigger;
        private HeliosTrigger _decrementOpacityTrigger;

        private readonly CalibrationPointCollectionDouble _calBrightness = new CalibrationPointCollectionDouble(0, 0, 1, 1) {
                new CalibrationPointDouble(0.10,0.6),
                new CalibrationPointDouble(0.30,0.80),
            };
        private CalibrationPointCollectionDouble _calOpacity = new CalibrationPointCollectionDouble(0, 0, 1, 255) { };


        public TextDecoration()
            : this("Label")
        {
            // all code in protected constructor shared with descendants
        }

        protected TextDecoration(string name)
        : base(name, new Size(60, 20))
        {
            _format.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TextFormat_PropertyChanged);
            _textValue = new HeliosValue(this, new BindingValue("Label"), "", "Text", "Text for this label.", "", BindingValueUnits.Text);
            _textValue.Execute += new HeliosActionHandler(TextValue_Execute);
            _format.VerticalAlignment = TextVerticalAlignment.Center;
            _format.HorizontalAlignment = TextHorizontalAlignment.Left;
            _referenceHeight = Height;
            Actions.Add(_textValue);
            Values.Add(_textValue);

            _brightnessValue = new HeliosValue(this, new BindingValue(false), "", "text display brightness value", "Number", "0.0 to 1.0", BindingValueUnits.Numeric);
            _brightnessValue.Execute += new HeliosActionHandler(DisplayBrightness_Execute);
            Actions.Add(_brightnessValue);
            Values.Add(_brightnessValue);

            _incrementBrightnessAction = new HeliosAction(this, "", "text display brightness", "increment", "Increments the display brightness.");
            _incrementBrightnessAction.Execute += new HeliosActionHandler(IncrementBrightnessAction_Execute);
            Actions.Add(_incrementBrightnessAction);

            _decrementBrightnessAction = new HeliosAction(this, "", "text display brightness", "decrement", "decrements the display brightness.");
            _decrementBrightnessAction.Execute += new HeliosActionHandler(DecrementBrightnessAction_Execute);
            Actions.Add(_decrementBrightnessAction);

            _opacityValue = new HeliosValue(this, new BindingValue(false), "", "text display opacity value", "number", "0.0 to 1.0", BindingValueUnits.Numeric);
            _opacityValue.Execute += new HeliosActionHandler(DisplayOpacity_Execute);
            Actions.Add(_opacityValue);
            Values.Add(_opacityValue);

            _incrementOpacityAction = new HeliosAction(this, "", "text display opacity", "increment", "Increments the display opacity.");
            _incrementOpacityAction.Execute += new HeliosActionHandler(IncrementOpacityAction_Execute);
            Actions.Add(_incrementOpacityAction);

            _decrementOpacityAction = new HeliosAction(this, "", "text display opacity", "decrement", "decrements the display opacity.");
            _decrementOpacityAction.Execute += new HeliosActionHandler(DecrementOpacityAction_Execute);
            Actions.Add(_decrementOpacityAction);

            _incrementOpacityTrigger = new HeliosTrigger(this, "", "", "opacity maximum", "Fired when the opacity value has reached the maximum.", "returns true.", BindingValueUnits.Boolean);
            Triggers.Add(_incrementOpacityTrigger);
            _decrementOpacityTrigger = new HeliosTrigger(this, "", "", "opacity minimum", "Fired when the opacity value has reached the minimum.", "returns true.", BindingValueUnits.Boolean);
            Triggers.Add(_decrementOpacityTrigger);

        }
        public double Brightness
        {
            get => _displayBrightness;
            set
            {
                if (value != _displayBrightness)
                {
                    double oldValue = _displayBrightness;
                    _displayBrightness = Clamp(value, 0, 1);
                    double brightness = _calBrightness.Interpolate(_displayBrightness);
                    _fontColor = Color.FromArgb(_fontColor.A, Convert.ToByte((double)_fontColor.R * brightness), Convert.ToByte((double)_fontColor.G * brightness), Convert.ToByte((double)_fontColor.B * brightness));
                    OnPropertyChanged("Brightness", oldValue, value, true);
                }
            }
        }
        private void DisplayBrightness_Execute(object action, HeliosActionEventArgs e)
        {
            double brightness = _calBrightness.Interpolate(e.Value.DoubleValue);
            _fontColor = Color.FromArgb(_fontColor.A, Convert.ToByte((double)_fontColor.R * brightness), Convert.ToByte((double)_fontColor.G * brightness), Convert.ToByte((double)_fontColor.B * brightness));
        }
        private void IncrementBrightnessAction_Execute(object action, HeliosActionEventArgs e)
        {
            _displayBrightness = _displayBrightness >= 10 ? 10 : ++_displayBrightness;
            double brightness = _calBrightness.Interpolate(_displayBrightness / 10d);
            _fontColor = Color.FromArgb(_fontColor.A, Convert.ToByte((double)_fontColor.R * brightness), Convert.ToByte((double)_fontColor.G * brightness), Convert.ToByte((double)_fontColor.B * brightness));
        }
        private void DecrementBrightnessAction_Execute(object action, HeliosActionEventArgs e)
        {
            _displayBrightness = _displayBrightness <= 0 ? 0 : --_displayBrightness;
            double brightness = _calBrightness.Interpolate(_displayBrightness / 10d);
            _fontColor = Color.FromArgb(_fontColor.A, Convert.ToByte((double)_fontColor.R * brightness), Convert.ToByte((double)_fontColor.G * brightness), Convert.ToByte((double)_fontColor.B * brightness));
        }
        private void DisplayOpacity_Execute(object action, HeliosActionEventArgs e)
        {
            byte opacity = Convert.ToByte(_calOpacity.Interpolate(e.Value.DoubleValue));
            FontColor = Color.FromArgb(opacity, FontColor.R, FontColor.G, FontColor.B);
            if (FillBackground)
            {
                BackgroundColor = Color.FromArgb(opacity, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            }


            if (!BypassTriggers)
            {
                if (opacity > 0xfc)
                {
                    _incrementOpacityTrigger.FireTrigger(new BindingValue(true));
                }
                else if (opacity < 0x04)
                {
                    _decrementOpacityTrigger.FireTrigger(new BindingValue(true));
                }
            }
        }
        private void IncrementOpacityAction_Execute(object action, HeliosActionEventArgs e)
        {
            byte opacity = FontColor.A;
            opacity = (byte)(opacity > 0xfc ? opacity : opacity + 0x04);
            FontColor = Color.FromArgb(opacity, FontColor.R, FontColor.G, FontColor.B);

            if (FillBackground)
            {
                byte bOpacity = BackgroundColor.A;
                bOpacity = (byte)(bOpacity < 0xfc ? bOpacity : bOpacity + 0x04);
                BackgroundColor = Color.FromArgb(bOpacity, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            }

            if (!BypassTriggers && opacity > 0xfc)
            {
                _incrementOpacityTrigger.FireTrigger(new BindingValue(true));
            }
        }
        private void DecrementOpacityAction_Execute(object action, HeliosActionEventArgs e)
        {
            byte opacity = FontColor.A;
            opacity = (byte)(opacity < 0x04 ? opacity : opacity - 0x04);
            FontColor = Color.FromArgb(opacity, FontColor.R, FontColor.G, FontColor.B);

            if (FillBackground)
            {
                byte bOpacity = BackgroundColor.A;
                bOpacity = (byte)(bOpacity < 0x04 ? bOpacity : bOpacity - 0x04);
                BackgroundColor = Color.FromArgb(bOpacity, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            }

            if (!BypassTriggers && opacity < 0x04)
            {
                _decrementOpacityTrigger.FireTrigger(new BindingValue(true));
            }
        }

        #region Properties

        public TextFormat Format
        {
            get
            {
                return _format;
            }
            set
            {
                if ((_format == null && value != null)
                    || (_format != null && !_format.Equals(value)))
                {
                    TextFormat oldValue = _format;
                    _format = value;
                    OnPropertyChanged("Format", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string Text
        {
            get
            {
                return _textValue.Value.StringValue;
            }
            set
            {
                if ((_textValue.Value.StringValue == null && value != null)
                    || (_textValue.Value.StringValue != null && !_textValue.Value.StringValue.Equals(value)))
                {
                    string oldValue = _textValue.Value.StringValue;
                    _textValue.SetValue(new BindingValue(value), BypassTriggers);
                    OnPropertyChanged("Text", oldValue, value, true);
                    OnDisplayUpdate();
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
                    bool oldValue = _fillBackground;
                    _fillBackground = value;
                    OnPropertyChanged("FillBackground", oldValue, value, true);
                    OnDisplayUpdate();
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

        public Color FontColor
        {
            get
            {
                return _fontColor;
            }
            set
            {
                if ((_fontColor == null && value != null)
                    || (_fontColor != null && !_fontColor.Equals(value)))
                {
                    Color oldValue = _fontColor;
                    _fontColor = value;
                    OnPropertyChanged("FontColor", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public TextFormat TextFormat
        {
            get
            {
                return _format;
            }
            set
            {
                if (_format == value)
                {
                    return;
                }
                TextFormat oldValue = _format;
                if (oldValue != null)
                {
                    oldValue.PropertyChanged -= TextFormat_PropertyChanged;
                }
                _format = value;
                if (_format != null)
                {
                    // NOTE: we indirectly set ConfiguredFontSize by changing text format object
                    _referenceHeight = Height;
                    _format.PropertyChanged += TextFormat_PropertyChanged;
                }
                OnPropertyChanged("TextFormat", oldValue, value, true);
                Refresh();
            }
        }
        /// <summary>
        /// backing field for property ScalingMode, contains
        /// the selected automatic font size scaling mode
        /// </summary>
        private TextScalingMode _scalingMode;

        /// <summary>
        /// the height this display had when the font size was configured
        /// </summary>
        private double _referenceHeight;

        /// <summary>
        /// the selected automatic font size scaling mode
        /// </summary>
        public TextScalingMode ScalingMode
        {
            get => _scalingMode;
            set
            {
                if (_scalingMode == value) return;
                TextScalingMode oldValue = _scalingMode;
                _scalingMode = value;
                OnPropertyChanged("ScalingMode", oldValue, value, true);
            }
        }
        // WARNING: this virtual method is called from the base constructor (indirectly)
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            switch (ScalingMode)
            {
                case TextScalingMode.Height:
                    if (_referenceHeight < 0.001)
                    {
                        TextFormat.FontSize = TextFormat.ConfiguredFontSize;
                        break;
                    }
                    // avoid accumulating error from repeated resizing by calculating from a reference point
                    Logger.Debug("scaling font based on new height {Height} versus reference {ReferenceSize} at height {ReferenceHeight}",
                        current.Height, TextFormat.ConfiguredFontSize, _referenceHeight);
                    Format.FontSize = Clamp(TextFormat.ConfiguredFontSize * current.Height / _referenceHeight, 1, 2000);
                    break;
                case TextScalingMode.None:
                    return;
                case TextScalingMode.Legacy:
                    if (previous.Height != 0)
                    {
                        double scale = current.Height / previous.Height;
                        TextFormat.FontSize = Clamp(scale * TextFormat.FontSize, 1, 100);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Logger.Debug("Font Size " + TextFormat.FontSize);
        }

        #endregion

        void TextFormat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TextFormat.ConfiguredFontSize))
            {
                // track these for recalculation of automatic scaling
                _referenceHeight = Height;
            }

            // invalidate entire TextFormat
            OnPropertyChanged("TextFormat", (PropertyNotificationEventArgs)e);

            // recalculate rendering
            OnDisplayUpdate();
        }


        void TextValue_Execute(object action, HeliosActionEventArgs e)
        {
            _textValue.SetValue(e.Value, e.BypassCascadingTriggers);
            OnDisplayUpdate();
        }

        public override void ConfigureIconInstance()
        {
            FontColor = Color.FromRgb(30, 30, 30);
        }

        //public override void ScaleChildren(double scaleX, double scaleY)
        //{
        //    double scale = scaleX > scaleY ? scaleX : scaleY;
        //    Format.FontSize *= scale;
        //}

        public override bool HitTest(Point location)
        {
            return FillBackground;
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

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            writer.WriteElementString("FillBackground", boolConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FillBackground));
            writer.WriteElementString("BackgroundColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, BackgroundColor));
            writer.WriteElementString("FontColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FontColor));

            writer.WriteStartElement("Font");
            _format.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteElementString("Text", Text);
            if (ScalingMode != TextScalingMode.Legacy)
            {
                writer.WriteElementString("ScalingMode", ScalingMode.ToString());
            }
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            FillBackground = (bool)boolConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("FillBackground"));
            _backgroundColorDefault = BackgroundColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("BackgroundColor"));
            _fontColorDefault = FontColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("FontColor"));

            reader.ReadStartElement("Font");
            _format.ReadXml(reader);
            reader.ReadEndElement();

            _defaultText = Text = reader.ReadElementString("Text");
            if (reader.Name == "ScalingMode" && Enum.TryParse(reader.ReadElementString("ScalingMode"), out TextScalingMode configured))
            {
                ScalingMode = configured;
            }
            else
            {
                ScalingMode = TextScalingMode.Legacy;
            }
            base.ReadXml(reader);

            // now the auto scaling has messed up our font size, so we restore it
            _format.FontSize = _format.ConfiguredFontSize;
            _referenceHeight = Height;
        }

        private double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
        public override void Reset()
        {
            _fontColor = _fontColorDefault;
            BackgroundColor = _backgroundColorDefault;
            Text = _defaultText;
            Brightness = 1.0d;
            base.Reset();
        }
    }
}
