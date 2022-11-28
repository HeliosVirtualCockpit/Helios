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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using NLog;
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Base.IndicatorPushButton", "Push Caution Indicator", "Push Button Indicators", typeof(IndicatorPushButtonRenderer))]
    public class IndicatorPushButton : PushButton, IConfigurableImageLocation
    {
        private string _imageOnFile = "{Helios}/Images/Indicators/caution-indicator-on.png";
        private string _pushedOnImageFile = "{Helios}/Images/Indicators/caution-indicator-on-pushed.png";

        private bool _on;

        private Color _onTextColor = Color.FromRgb(179, 162, 41);
        private Color _onGlyphColor = Color.FromRgb(179, 162, 41);

        private HeliosAction _toggleAction;
        private HeliosValue _value;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public IndicatorPushButton() : this("Indicator Pushbutton", new Size(100, 50)) { }
        public IndicatorPushButton(string name, Size size)
            : base(name, size)
        {
            Image = "{Helios}/Images/Indicators/caution-indicator-off.png";
            PushedImage = "{Helios}/Images/Indicators/caution-indicator-off-pushed.png";
            TextColor = Color.FromRgb(28, 28, 28);
            GlyphColor = Color.FromRgb(28, 28, 28);

            _value = new HeliosValue(this, new BindingValue(false), "", "indicator", "Current On/Off State for this buttons indicator.", "True if the indicator is on, otherwise false.", BindingValueUnits.Boolean);
            _value.Execute += new HeliosActionHandler(Indicator_Execute);
            Values.Add(_value);
            Actions.Add(_value);

            _toggleAction = new HeliosAction(this, "", "", "toggle indicator", "Toggles this indicator between on and off.");
            _toggleAction.Execute += new HeliosActionHandler(ToggleAction_Execute);
            Actions.Add(_toggleAction);
        }

        #region Properties

        public bool Indicator
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

                    OnPropertyChanged("Indicator", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public string IndicatorOnImage
        {
            get
            {
                return _imageOnFile;
            }
            set
            {
                if ((_imageOnFile == null && value != null)
                    || (_imageOnFile != null && !_imageOnFile.Equals(value)))
                {
                    string oldValue = _imageOnFile;
                    _imageOnFile = value;
                    OnPropertyChanged("IndicatorOnImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PushedIndicatorOnImage
        {
            get
            {
                return _pushedOnImageFile;
            }
            set
            {
                if ((_pushedOnImageFile == null && value != null)
                    || (_pushedOnImageFile != null && !_pushedOnImageFile.Equals(value)))
                {
                    string oldValue = _pushedOnImageFile;
                    _pushedOnImageFile = value;
                    OnPropertyChanged("PushedIndicatorOnImage", oldValue, value, true);
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

        public Color OnGlyphColor
        {
            get
            {
                return _onGlyphColor;
            }
            set
            {
                if (!_onGlyphColor.Equals(value))
                {
                    Color oldValue = _onGlyphColor;
                    _onGlyphColor = value;
                    OnPropertyChanged("OnGlypColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        #endregion

        void ToggleAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            Indicator= !Indicator;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        void Indicator_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            Indicator = e.Value.BoolValue;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        public override void ScaleChildren(double scaleX, double scaleY)
        {
            double scale = scaleX > scaleY ? scaleX : scaleY;
            TextFormat.FontSize *= scale;
        }

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            Pushed = false;
            IsClosed = false;
            Indicator = false;
            EndTriggerBypass(true);
        }
        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public new void ReplaceImageNames(string oldName, string newName)
        {
            base.ReplaceImageNames(oldName, newName);
            IndicatorOnImage = !string.IsNullOrEmpty(IndicatorOnImage) ? IndicatorOnImage : string.IsNullOrEmpty(oldName) ? newName + IndicatorOnImage : IndicatorOnImage.Replace(oldName, newName);
            PushedIndicatorOnImage = !string.IsNullOrEmpty(PushedIndicatorOnImage) ? PushedIndicatorOnImage : string.IsNullOrEmpty(oldName) ? newName + PushedIndicatorOnImage : PushedIndicatorOnImage.Replace(oldName, newName);
        }

        public override void MouseDown(Point location)
        {
            if (DesignMode)
            {
                Indicator = !Indicator;
            }
            base.MouseDown(location);
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            writer.WriteElementString("IndicatorOnImage", IndicatorOnImage);
            writer.WriteElementString("PushedIndicatorOnImage", PushedIndicatorOnImage);
            writer.WriteElementString("OnTextColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, OnTextColor));
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            IndicatorOnImage = reader.ReadElementString("IndicatorOnImage");
            PushedIndicatorOnImage = reader.ReadElementString("PushedIndicatorOnImage");

            if (reader.Name.Equals("Font"))
            {
                reader.ReadStartElement("Font");
                TextFormat.ReadXml(reader);
                reader.ReadEndElement();
                Text = reader.ReadElementString("Text");
            }

            OnTextColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("OnTextColor"));

            if (reader.Name.Equals("OffTextColor"))
            {
                TextColor = (Color)colorConverter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, reader.ReadElementString("OffTextColor"));
            }
            if (reader.Name == "ScalingMode" && Enum.TryParse(reader.ReadElementString("ScalingMode"), out TextScalingMode configured))
            {
                Logger.Info("Profile load encountered historical \"ScalingMode\" parameter.");
            }
            base.ReadXml(reader);
        }
    }
}
