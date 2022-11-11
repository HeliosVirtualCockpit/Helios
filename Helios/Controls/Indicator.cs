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
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using NLog;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    
    [HeliosControl("Helios.Base.Indicator", "Caution Indicator", "Indicators", typeof(IndicatorRenderer))]
    public class Indicator : HeliosVisual, IConfigurableImageLocation
    {
        private bool _on;

        private bool _allowInteraction;

        private string _onImage = "{Helios}/Images/Indicators/caution-indicator-on.png";
        private string _offImage = "{Helios}/Images/Indicators/caution-indicator-off.png";

        private string _indicatorText = "Fault";
        private Color _onTextColor = Color.FromRgb(179, 162, 41);
        private Color _offTextColor = Color.FromRgb(28, 28, 28);
        private TextFormat _textFormat = new TextFormat();

        private HeliosAction _toggleAction;
        private HeliosValue _value;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

         public Indicator()
            : base("Indicator", new System.Windows.Size(100, 50))
        {
            _referenceHeight = Height;

            _allowInteraction = true;

            _textFormat.VerticalAlignment = TextVerticalAlignment.Center;
            _textFormat.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TextFormat_PropertyChanged);

            _value = new HeliosValue(this, new BindingValue(false), "", "indicator", "Current On/Off State for this indicator.", "True if the indicator is on, otherwise false.", BindingValueUnits.Boolean);
            _value.Execute += new HeliosActionHandler(On_Execute);
            Values.Add(_value);
            Actions.Add(_value);

            _toggleAction = new HeliosAction(this, "", "", "toggle indicator", "Toggles this indicator between on and off.");
            _toggleAction.Execute += new HeliosActionHandler(ToggleAction_Execute);
            Actions.Add(_toggleAction);
        }

        #region Properties

        public bool On
        {
            get
            {
                return _on;
            }
            set
            {
                if (!_on.Equals(value))
                {
                    bool oldValue = _on;

                    _on = value;
                    _value.SetValue(new BindingValue(_on), BypassTriggers);

                    OnPropertyChanged("On", oldValue, value, false);
                    OnDisplayUpdate();
                }
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

        public string OffImage
        {
            get
            {
                return _offImage;
            }
            set
            {
                if ((_offImage == null && value != null)
                    || (_offImage != null && !_offImage.Equals(value)))
                {
                    string oldValue = _offImage;
                    _offImage = value;
                    OnPropertyChanged("OffImage", oldValue, value, true);
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

        public Color OffTextColor
        {
            get
            {
                return _offTextColor;
            }
            set
            {
                if (!_offTextColor.Equals(value))
                {
                    Color oldValue = _offTextColor;
                    _offTextColor = value;
                    OnPropertyChanged("OffTextColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string Text
        {
            get
            {
                return _indicatorText;
            }
            set
            {
                if ((_indicatorText == null && value != null)
                    || (_indicatorText != null && !_indicatorText.Equals(value)))
                {
                    string oldValue = _indicatorText;
                    _indicatorText = value;
                    OnPropertyChanged("Text", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public TextFormat TextFormat
        {
            get
            {
                return _textFormat;
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

        public bool AllowInteraction
        {
            get => _allowInteraction;
            set => _allowInteraction = value;
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


        void ToggleAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            On = !On;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        void On_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            On = e.Value.BoolValue;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            On = false;
            EndTriggerBypass(true);
        }

        public override void MouseDown(Point location)
        {
            if (DesignMode)
            {
                On = !On;
            }
        }

        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceImageNames(string oldName, string newName)
        {
            OffImage = string.IsNullOrEmpty(OffImage) ? OffImage : string.IsNullOrEmpty(oldName) ? newName + OffImage : OffImage.Replace(oldName, newName);
            OnImage = string.IsNullOrEmpty(OnImage) ? OnImage : string.IsNullOrEmpty(oldName) ? newName + OnImage : OnImage.Replace(oldName, newName);
        }

        #region Overrides of HeliosVisual

        public override void ConfigureIconInstance()
        {
            On = true;
            base.ConfigureIconInstance();
        }

        public override void ConfigureTemplateIconInstance()
        {
            On = true;
            base.ConfigureTemplateIconInstance();
        }

        #endregion

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }

        public override bool HitTest(Point location)
        {
            return _allowInteraction;
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            writer.WriteElementString("OnImage", OnImage);
            writer.WriteElementString("OffImage", OffImage);
            writer.WriteStartElement("Font");
            _textFormat.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteElementString("Text", Text);
            writer.WriteElementString("OnTextColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, OnTextColor));
            writer.WriteElementString("OffTextColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, OffTextColor));
            if (ScalingMode != TextScalingMode.Legacy)
            {
                writer.WriteElementString("ScalingMode", ScalingMode.ToString());
            }

            writer.WriteStartElement("Interaction");
            writer.WriteElementString("AllowInteraction", AllowInteraction.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            OnImage = reader.ReadElementString("OnImage");
            OffImage = reader.ReadElementString("OffImage");
            reader.ReadStartElement("Font");
            _textFormat.ReadXml(reader);
            reader.ReadEndElement();
            Text = reader.ReadElementString("Text");
            OnTextColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("OnTextColor"));
            OffTextColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("OffTextColor"));
            if (reader.Name == "ScalingMode" && Enum.TryParse(reader.ReadElementString("ScalingMode"), out TextScalingMode configured))
            {
                ScalingMode = configured;
            }
            else
            {
                ScalingMode = TextScalingMode.Legacy;
            }

            if (reader.Name.Equals("Interaction"))
            {
                reader.ReadStartElement("Interaction");
                AllowInteraction = bool.Parse(reader.ReadElementString("AllowInteraction"));
                reader.ReadEndElement();
            }
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
    }
}
