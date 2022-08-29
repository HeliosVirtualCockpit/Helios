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

using System;
using System.Globalization;

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// base class for text displays, does not include any actions or values, just the ability to display text
    /// </summary>
    public abstract class TextDisplayRect : HeliosVisual
    {
        protected bool _useParseDicationary = false;
        protected string _textValue = "";
        protected string _rawValue = "";
        protected string _textValueTest = "O";
        protected string _onImage = "{Helios}/Images/Indicators/anunciator.png";
        protected bool _useBackground = true;    // displaying the background or not
        protected Color _onTextColor = Color.FromArgb(0xff, 0x40, 0xb3, 0x29);
        protected Color _backgroundColor = Color.FromArgb(0xff, 0, 0, 0);
        protected Dictionary<string, string> _parserDictionary = new Dictionary<string, string>(); // the list of input -> output string modifications
        protected TextFormat _textFormat = new TextFormat();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public TextDisplayRect(string name, System.Windows.Size nativeSize) :
            base(name, nativeSize)
        {
            _textFormat.VerticalAlignment = TextVerticalAlignment.Center;
            _textFormat.HorizontalAlignment = TextHorizontalAlignment.Left;
            _textFormat.PropertyChanged += TextFormat_PropertyChanged;
            // _textFormat.FontFamily = FontManager.Instance.GetFontFamilyByName("SF Digital Readout");
            _referenceHeight = Height;
        }

        #region Properties

        public string TextValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                if (_useParseDicationary)
                {
                    if (!_rawValue.Equals(value))
                    {
                        _rawValue = value;
                        // parse the value
                        string parsedValue = value;
                        foreach (KeyValuePair<string, string> entry in _parserDictionary)
                        {
                            parsedValue = parsedValue.Replace(entry.Key, entry.Value);
                        }
                        string oldValue = _textValue;
                        _textValue = parsedValue;
                        OnTextValueChange();
                        OnPropertyChanged("TextValue", oldValue, parsedValue, false);
                        OnDisplayUpdate();
                    }
                }
                else
                {
                    if (!value.Equals(_textValue))
                    {
                        string oldValue = _textValue;
                        _textValue = value;
                        OnTextValueChange();
                        OnPropertyChanged("TextValue", oldValue, value, false);
                        OnDisplayUpdate();
                    }
                }
            }
        }

        public bool UseParseDictionary
        {
            get
            {
                return _useParseDicationary;
            }
            set
            {
                if (!_useParseDicationary.Equals(value))
                {
                    bool oldValue = _useParseDicationary;
                    _useParseDicationary = value;
                    OnPropertyChanged("UseParseDictionary", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public bool UseBackground
        {
            get
            {
                return _useBackground;
            }
            set
            {
                if (!_useBackground.Equals(value))
                {
                    bool oldValue = _useBackground;
                    _useBackground = value;
                    OnPropertyChanged("UseBackground", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public string ParserDictionary
        {
            get
            { /// convert the dictionary to a string
                var stringBuilder = new StringBuilder();
                bool first = true;
                foreach (KeyValuePair<string, string> pair in _parserDictionary)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        stringBuilder.Append(";");
                    }

                    stringBuilder.AppendFormat("{0}={1}", pair.Key, pair.Value);
                }
                return stringBuilder.ToString();
            }
            set /// convert the string to a dictionary
            {
                if (!value.Equals(""))
                {
                    Dictionary<string, string> oldValue = _parserDictionary;
                    _parserDictionary = value.TrimEnd(';').Split(';').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);
                    OnPropertyChanged("ParserDictionary", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public string TextTestValue
        {
            get
            {
                return _textValueTest;
            }
            set
            {
                string oldValue = _textValueTest;
                _textValueTest = value;
                OnPropertyChanged("TextTestValue", oldValue, value, false);
                OnDisplayUpdate();
            }
        }

        public string OnImage
        {
            get
            {
                return _onImage;
            }
            set
            {
                if ((_onImage == null && value != null)
                    || (_onImage != null && !_onImage.Equals(value)))
                {
                    string oldValue = _onImage;
                    _onImage = value;
                    OnPropertyChanged("OnImage", oldValue, value, true);
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

        public Color OnTextColor
        {
            get
            {
                return _onTextColor;
            }
            set
            {
                if (!_onTextColor.Equals(value))
                {
                    Color oldValue = _onTextColor;
                    _onTextColor = value;
                    OnPropertyChanged("OnTextColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public TextFormat TextFormat
        {
            get
            {
                return _textFormat;
            }
            set
            {
                if (_textFormat == value)
                {
                    return;
                }
                TextFormat oldValue = _textFormat;
                if (oldValue != null)
                {
                    oldValue.PropertyChanged -= TextFormat_PropertyChanged;
                }
                _textFormat = value;
                if (_textFormat != null)
                {
                    // NOTE: we indirectly set ConfiguredFontSize by changing text format object
                    _referenceHeight = Height;
                    _textFormat.PropertyChanged += TextFormat_PropertyChanged;
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
                    TextFormat.FontSize = Clamp(TextFormat.ConfiguredFontSize * current.Height / _referenceHeight, 1, 2000);
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

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            TextValue = "";
            EndTriggerBypass(true);
        }

        public override void MouseDown(Point location)
        {
            if (DesignMode)
            {
                TextValue = _textValueTest;
            }
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
            writer.WriteStartElement("Font");
            _textFormat.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteElementString("OnTextColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, OnTextColor));
            writer.WriteElementString("BackgroundColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, BackgroundColor));
            writer.WriteElementString("TextTest", _textValueTest);
            writer.WriteElementString("ParserDictionary", ParserDictionary);
            writer.WriteElementString("UseBackground", boolConverter.ConvertToInvariantString(UseBackground));
            writer.WriteElementString("UseParserDictionary", boolConverter.ConvertToInvariantString(UseParseDictionary));
            if (ScalingMode != TextScalingMode.Legacy)
            {
                writer.WriteElementString("ScalingMode", ScalingMode.ToString());
            }

            WriteAdditionalXml(writer);
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));
            reader.ReadStartElement("Font");
            _textFormat.ReadXml(reader);

            reader.ReadEndElement();
            OnTextColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("OnTextColor"));
            BackgroundColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("BackgroundColor"));
            TextTestValue = reader.ReadElementString("TextTest");
            ParserDictionary = reader.ReadElementString("ParserDictionary");
            UseBackground = (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("UseBackground"));
            UseParseDictionary = (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("UseParserDictionary"));
            if (reader.Name == "ScalingMode" && Enum.TryParse(reader.ReadElementString("ScalingMode"), out TextScalingMode configured))
            {
                ScalingMode = configured;
            }
            else
            {
                ScalingMode = TextScalingMode.Legacy;
            }

            ReadAdditionalXml(reader);
            base.ReadXml(reader);

            // now the auto scaling has messed up our font size, so we restore it
            _textFormat.FontSize = _textFormat.ConfiguredFontSize;
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

        protected abstract void OnTextValueChange();

        /// <summary>
        /// hook for descendants to allow for additional XML after the fixed section
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadAdditionalXml(XmlReader reader)
        {
            // no code in base
        }

        /// <summary>
        /// hook for descendants to allow for additional XML after the fixed section
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteAdditionalXml(XmlWriter writer)
        {
            // no code in base
        }
    }

    [HeliosControl("Helios.Base.TextDisplay", "Text Display", "Text Displays", typeof(TextDisplayRenderer))]
    public class TextDisplay : TextDisplayRect
    {
        private readonly HeliosValue _value;

        public TextDisplay() : this("TextDisplay", new System.Windows.Size(100, 50)) { }
        public TextDisplay(string name, System.Windows.Size nativeSize)
            : base(name,nativeSize)
        {
            _value = new HeliosValue(this, new BindingValue(false), "", "TextDisplay", "Value of this Text Display", "A text string.", BindingValueUnits.Text);
            _value.Execute += On_Execute;
            Values.Add(_value);
            Actions.Add(_value);
        }

        protected override void OnTextValueChange()
        {
            _value.SetValue(new BindingValue(_textValue), BypassTriggers);
        }

        void On_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            TextValue = e.Value.StringValue;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }
    }
}
