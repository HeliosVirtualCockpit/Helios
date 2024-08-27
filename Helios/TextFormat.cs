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

namespace GadrocsWorkshop.Helios
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;

    public class TextFormat : NotificationObject
    {
        private string _displayName = "Franklin Gothic";
        private FontFamily _family = ConfigManager.FontManager.GetFontFamilyByName("Franklin Gothic");
        private FontStyle _style = FontStyles.Normal;
        private FontWeight _weight = FontWeights.Normal;
        private FontStretch _stretch = FontStretches.Medium;
        private double _size = 12f;
        private TextHorizontalAlignment _horizontalAlignment = TextHorizontalAlignment.Center;
        private TextVerticalAlignment _verticalAlignment = TextVerticalAlignment.Top;
        private TextDecorations _decorations;
        private TextDecorationCollection _formattedDecorations;

        private double _textPaddingLeft;
        private double _textPaddingTop;
        private double _textPaddingRight;
        private double _textPaddingBottom;

        /// <summary>
        /// backing field for property ConfiguredFontSize, contains
        /// the font size we actually configured via the UI
        /// </summary>
        private double _configuredFontSize = 12f;

        public TextFormat()
        {
        }

        public FontFamily FontFamily
        {
            get
            {
                return _family;
            }
            set
            {
                if ((_family == null && value != null)
                    || (_family != null && !_family.Equals(value)))
                {
                    FontFamily oldValue = _family;
                    string oldName = _displayName;
                    _family = value;
                    _displayName = GetFontDisplayName(_family);
                    OnPropertyChanged("DisplayName", oldName, _displayName, false);
                    OnPropertyChanged("FontFamily", oldValue, value, true);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return _style;
            }
            set
            {
                if ((_style == null && value != null)
                    || (_style != null && !_style.Equals(value)))
                {
                    FontStyle oldValue = _style;
                    _style = value;
                    OnPropertyChanged("FontStyle", oldValue, value, true);
                }
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                return _weight;
            }
            set
            {
                if ((_weight == null && value != null)
                    || (_weight != null && !_weight.Equals(value)))
                {
                    FontWeight oldValue = _weight;
                    _weight = value;
                    OnPropertyChanged("FontWeight", oldValue, value, true);
                }
            }
        }
        public FontStretch FontStretch
        {
            get
            {
                return _stretch;
            }
            set
            {
                if ((_stretch == null && value != null)
                    || (_stretch != null && !_stretch.Equals(value)))
                {
                    FontStretch oldValue = _stretch;
                    _stretch = value;
                    OnPropertyChanged("FontStretch", oldValue, value, true);
                }
            }
        }

        public double FontSize
        {
            get
            {
                return _size;
            }
            set
            {
                if (!_size.Equals(value))
                {
                    double oldValue = _size;
                    _size = value;
                    OnPropertyChanged("FontSize", oldValue, value, true);
                }
            }
        }

        /// <summary>
        /// the font size we actually configured via the UI
        /// </summary>
        public double ConfiguredFontSize
        {
            get
            {
                return _configuredFontSize;
            }
            set
            {
                if (!_configuredFontSize.Equals(value))
                {
                    double oldValue = _configuredFontSize;
                    _configuredFontSize = value;
                    OnPropertyChanged("ConfiguredFontSize", oldValue, value, true);
                }
            }
        }

        public TextHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return _horizontalAlignment;
            }
            set
            {
                if (!_horizontalAlignment.Equals(value))
                {
                    TextHorizontalAlignment oldValue = _horizontalAlignment;
                    _horizontalAlignment = value;
                    OnPropertyChanged("HorizontalAlignment", oldValue, value, true);
                }
            }
        }

        public TextVerticalAlignment VerticalAlignment
        {
            get
            {
                return _verticalAlignment;
            }
            set
            {
                if (!_verticalAlignment.Equals(value))
                {
                    TextVerticalAlignment oldValue = _verticalAlignment;
                    _verticalAlignment = value;
                    OnPropertyChanged("VerticalAlignment", oldValue, value, true);
                }
            }
        }

        public TextDecorations Decorations
        {
            get
            {
                return _decorations;
            }
            set
            {
                if (!_decorations.Equals(value))
                {
                    TextDecorations oldValue = _decorations;
                    _decorations = value;
                    _formattedDecorations = null;
                    OnPropertyChanged("Decorations", oldValue, value, true);
                    OnPropertyChanged("TextDeocrations", null, null, false);
                }
            }
        }

        public TextDecorationCollection TextDecorations
        {
            get
            {
                if (_formattedDecorations == null)
                {
                    _formattedDecorations = new TextDecorationCollection();
                    if (Decorations.HasFlag(Helios.TextDecorations.Underline))
                    {
                        _formattedDecorations.Add(System.Windows.TextDecorations.Underline[0]);
                    }
                    if (Decorations.HasFlag(Helios.TextDecorations.Strikethrough))
                    {
                        _formattedDecorations.Add(System.Windows.TextDecorations.Strikethrough[0]);
                    }
                    if (Decorations.HasFlag(Helios.TextDecorations.Baseline))
                    {
                        _formattedDecorations.Add(System.Windows.TextDecorations.Baseline[0]);
                    }
                    if (Decorations.HasFlag(Helios.TextDecorations.OverLine))
                    {
                        _formattedDecorations.Add(System.Windows.TextDecorations.OverLine[0]);
                    }
                    _formattedDecorations.Freeze();
                }

                return _formattedDecorations;
            }
        }


        public double PaddingRight
        {
            get
            {
                return _textPaddingRight;
            }
            set
            {
                if (!_textPaddingRight.Equals(value))
                {
                    double oldValue = _textPaddingRight;
                    _textPaddingRight = value;
                    OnPropertyChanged("PaddingRight", oldValue, value, true);
                }
            }
        }

        public double PaddingLeft
        {
            get
            {
                return _textPaddingLeft;
            }
            set
            {
                if (!_textPaddingLeft.Equals(value))
                {
                    double oldValue = _textPaddingLeft;
                    _textPaddingLeft = value;
                    OnPropertyChanged("PaddingLeft", oldValue, value, true);
                }
            }
        }

        public double PaddingTop
        {
            get
            {
                return _textPaddingTop;
            }
            set
            {
                if (!_textPaddingTop.Equals(value))
                {
                    double oldValue = _textPaddingTop;
                    _textPaddingTop = value;
                    OnPropertyChanged("PaddingTop", oldValue, value, true);
                }
            }
        }

        public double PaddingBottom
        {
            get
            {
                return _textPaddingBottom;
            }
            set
            {
                if (!_textPaddingBottom.Equals(value))
                {
                    double oldValue = _textPaddingBottom;
                    _textPaddingBottom = value;
                    OnPropertyChanged("PaddingBottom", oldValue, value, true);
                }
            }
        }

        public FormattedText GetFormattedText(Brush textBrush, string text)
        {
            Typeface type = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            FormattedText formatedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, type, FontSize, textBrush, ConfigManager.DisplayManager.PixelsPerDip);

            formatedText.SetTextDecorations(TextDecorations);

            formatedText.Trimming = TextTrimming.CharacterEllipsis;

            switch (_horizontalAlignment)
            {
                case TextHorizontalAlignment.Left:
                    formatedText.TextAlignment = TextAlignment.Left;
                    break;
                case TextHorizontalAlignment.Center:
                    formatedText.TextAlignment = TextAlignment.Center;
                    break;
                case TextHorizontalAlignment.Right:
                    formatedText.TextAlignment = TextAlignment.Right;
                    break;
            }

            return formatedText;
        }

        public void RenderText(DrawingContext drawingContext, Brush textBrush, string text, Rect rectangle)
        {
            FormattedText formatedText = GetFormattedText(textBrush, text);

            Rect adjustedRect = new Rect(rectangle.X + (PaddingLeft * rectangle.Width), rectangle.Y + (PaddingTop * rectangle.Height),
                                            Math.Max(0.1d, rectangle.Width - ((PaddingLeft + PaddingRight) * rectangle.Width)),
                                            Math.Max(0.1d, rectangle.Height - ((PaddingTop + PaddingBottom) * rectangle.Height)));

            formatedText.MaxTextHeight = adjustedRect.Height;
            formatedText.MaxTextWidth = adjustedRect.Width;

            double yOffset = 0;
            if (_verticalAlignment != TextVerticalAlignment.Top)
            {
                yOffset = Math.Max(0d, adjustedRect.Height - formatedText.Height);
                if (_verticalAlignment == TextVerticalAlignment.Center)
                {
                    yOffset = yOffset / 2d;
                }
            }

            drawingContext.DrawText(formatedText, new Point(adjustedRect.X, adjustedRect.Y + yOffset));
        }

        public void ReadXml(XmlReader reader)
        {
            TypeConverter ffc = TypeDescriptor.GetConverter(typeof(FontFamily));
            TypeConverter fsc = TypeDescriptor.GetConverter(typeof(FontStyle));
            TypeConverter fwc = TypeDescriptor.GetConverter(typeof(FontWeight));
            TypeConverter fstc = TypeDescriptor.GetConverter(typeof(FontStretch));
            TypeConverter rc = TypeDescriptor.GetConverter(typeof(Rect));

            FontFamily = ConfigManager.FontManager.GetFontFamilyByName(reader.ReadElementString("FontFamily"));
            _style = (FontStyle)fsc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("FontStyle"));
            _weight = (FontWeight)fwc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("FontWeight"));
            _stretch = reader.Name.Equals("FontStretch") ? (FontStretch)fstc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("FontStretch")) : FontStretches.Medium;

            _size = double.Parse(reader.ReadElementString("FontSize"), CultureInfo.InvariantCulture);
            _configuredFontSize = _size;

            if (reader.Name.Equals("HorizontalAlignment"))
            {
                _horizontalAlignment = (TextHorizontalAlignment)Enum.Parse(typeof(TextHorizontalAlignment), reader.ReadElementString("HorizontalAlignment"));
                _verticalAlignment = (TextVerticalAlignment)Enum.Parse(typeof(TextVerticalAlignment), reader.ReadElementString("VerticalAlignment"));
            }

            if (reader.Name.Equals("Padding"))
            {
                reader.ReadStartElement("Padding");
                PaddingLeft = double.Parse(reader.ReadElementString("Left"), CultureInfo.InvariantCulture);
                PaddingTop = double.Parse(reader.ReadElementString("Top"), CultureInfo.InvariantCulture);
                PaddingRight = double.Parse(reader.ReadElementString("Right"), CultureInfo.InvariantCulture);
                PaddingBottom = double.Parse(reader.ReadElementString("Bottom"), CultureInfo.InvariantCulture);
                reader.ReadEndElement();
            }
            else
            {
                PaddingLeft = 0d;
                PaddingRight = 0d;
                PaddingTop = 0d;
                PaddingBottom = 0d;
            }
            
            if (reader.Name.Equals("Underline"))
            {
                _decorations |= Helios.TextDecorations.Underline;
                reader.Skip();
            }
            if (reader.Name.Equals("Strikethrough"))
            {
                _decorations |= Helios.TextDecorations.Strikethrough;
                reader.Skip();
            }
            if (reader.Name.Equals("Baseline"))
            {
                _decorations |= Helios.TextDecorations.Baseline;
                reader.Skip();
            }
            if (reader.Name.Equals("OverLine"))
            {
                _decorations |= Helios.TextDecorations.OverLine;
                reader.Skip();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            TypeConverter ffc = TypeDescriptor.GetConverter(typeof(FontFamily));
            TypeConverter fsc = TypeDescriptor.GetConverter(typeof(FontStyle));
            TypeConverter fwc = TypeDescriptor.GetConverter(typeof(FontWeight));
            TypeConverter fstc = TypeDescriptor.GetConverter(typeof(FontStretch));
            TypeConverter rc = TypeDescriptor.GetConverter(typeof(Rect));

            writer.WriteElementString("FontFamily", (FontFamily.Source.Contains("#") ? FontFamily.Source.Split('#')[1] : ffc.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FontFamily)));
            writer.WriteElementString("FontStyle", fsc.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FontStyle));
            writer.WriteElementString("FontWeight", fwc.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FontWeight));
            if( _stretch != FontStretches.Medium)
            {
                writer.WriteElementString("FontStretch", fstc.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, FontStretch));
            }
            writer.WriteElementString("FontSize", FontSize.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("HorizontalAlignment", HorizontalAlignment.ToString());
            writer.WriteElementString("VerticalAlignment", VerticalAlignment.ToString());

            writer.WriteStartElement("Padding");
            writer.WriteElementString("Left", PaddingLeft.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Top", PaddingTop.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Right", PaddingRight.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Bottom", PaddingBottom.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            if (Decorations.HasFlag(Helios.TextDecorations.Underline))
            {
                writer.WriteStartElement("Underline");
                writer.WriteEndElement();
            }

            if (Decorations.HasFlag(Helios.TextDecorations.Strikethrough))
            {
                writer.WriteStartElement("Strikethrough");
                writer.WriteEndElement();
            }

            if (Decorations.HasFlag(Helios.TextDecorations.Baseline))
            {
                writer.WriteStartElement("Baseline");
                writer.WriteEndElement();
            }

            if (Decorations.HasFlag(Helios.TextDecorations.OverLine))
            {
                writer.WriteStartElement("OverLine");
                writer.WriteEndElement();
            }
       }

        public static string GetFontDisplayName(FontFamily fontFamily)
        {
            return GetDisplayName(fontFamily.FamilyNames);
        }

        public static string GetTypefaceDisplayName(Typeface typeface)
        {
            return GetDisplayName(typeface.FaceNames);
        }

        private static string GetDisplayName(LanguageSpecificStringDictionary nameDictionary)
        {
            // Look up the display name based on the UI culture, which is the same culture
            // used for resource loading.
            XmlLanguage userLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            // Look for an exact match.
            string name;
            if (nameDictionary.TryGetValue(userLanguage, out name))
            {
                return name;
            }

            // No exact match; return the name for the most closely related language.
            int bestRelatedness = -1;
            string bestName = string.Empty;

            foreach (KeyValuePair<XmlLanguage, string> pair in nameDictionary)
            {
                int relatedness = GetRelatedness(pair.Key, userLanguage);
                if (relatedness > bestRelatedness)
                {
                    bestRelatedness = relatedness;
                    bestName = pair.Value;
                }
            }

            return bestName;
        }

        private static int GetRelatedness(XmlLanguage keyLang, XmlLanguage userLang)
        {
            try
            {
                // Get equivalent cultures.
                CultureInfo keyCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(keyLang.IetfLanguageTag);
                CultureInfo userCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(userLang.IetfLanguageTag);
                if (!userCulture.IsNeutralCulture)
                {
                    userCulture = userCulture.Parent;
                }

                // If the key is a prefix or parent of the user language it's a good match.
                if (IsPrefixOf(keyLang.IetfLanguageTag, userLang.IetfLanguageTag) || userCulture.Equals(keyCulture))
                {
                    return 2;
                }

                // If the key and user language share a common prefix or parent neutral culture, it's a reasonable match.
                if (IsPrefixOf(TrimSuffix(userLang.IetfLanguageTag), keyLang.IetfLanguageTag) || userCulture.Equals(keyCulture.Parent))
                {
                    return 1;
                }
            }
            catch (ArgumentException)
            {
                // Language tag with no corresponding CultureInfo.
            }

            // They're unrelated languages.
            return 0;
        }

        private static string TrimSuffix(string tag)
        {
            int i = tag.LastIndexOf('-');
            if (i > 0)
            {
                return tag.Substring(0, i);
            }
            else
            {
                return tag;
            }
        }

        private static bool IsPrefixOf(string prefix, string tag)
        {
            return prefix.Length < tag.Length &&
                tag[prefix.Length] == '-' &&
                string.CompareOrdinal(prefix, 0, tag, 0, prefix.Length) == 0;
        }
    }
}
