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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Base.RockerSwitch", "Rocker Switch with Label", "Rockers", typeof(RockerSwitchRenderer))]
    public class RockerSwitch : ThreeWayToggleSwitch, ITextControl
    {
        private string _label = "";
        private TextFormat _labelFormat = new TextFormat();
        private Color _labelColor = Color.FromArgb(0xe0,0xff,0xff,0xff);
        private Point _labelPushedOffset = new Point(0, 0);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public RockerSwitch()
            : base("Rocker Switch", new Size(50,100))
        {
            _labelFormat.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Format_PropertyChanged);

            _labelFormat.FontSize = 20;
            _labelFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
            _labelFormat.VerticalAlignment = TextVerticalAlignment.Center;
            _referenceHeight = Height;

            PositionOneImage = "{Helios}/Images/Rockers/arrows-dark-up.png";
            PositionTwoImage = "{Helios}/Images/Rockers/arrows-dark-norm.png";
            PositionThreeImage = "{Helios}/Images/Rockers/arrows-dark-down.png";
            SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            Text = "ABC";
            //Rotation = HeliosVisualRotation.CW;

        }

        #region Properties

        public string Text
        {
            get
            {
                return _label;
            }
            set
            {
                if ((_label == null && value != null)
                    || (_label != null && !_label.Equals(value)))
                {
                    string oldValue = _label;
                    _label = value;
                    OnPropertyChanged("Text", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public Color TextColor
        {
            get
            {
                return _labelColor;
            }
            set
            {
                if (!_labelColor.Equals(value))
                {
                    Color oldValue = _labelColor;
                    _labelColor = value;
                    OnPropertyChanged("TextColor", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public TextFormat TextFormat
        {
            get { return _labelFormat; }
            set { return; }
        }

        public Point TextPushOffset
        {
            get
            {
                return _labelPushedOffset;
            }
            set
            {
                if (!_labelPushedOffset.Equals(value))
                {
                    Point oldValue = _labelPushedOffset;
                    _labelPushedOffset = value;
                    OnPropertyChanged("TextPushOffset", oldValue, value, true);
                    Refresh();
                }
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

        void Format_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyNotificationEventArgs origArgs = e as PropertyNotificationEventArgs;
            if (origArgs != null)
            {
                OnPropertyChanged("TextFormat", origArgs);
            }
            OnDisplayUpdate();
        }


        #region HeliosControl Implementation

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            SwitchPosition = DefaultPosition;
            EndTriggerBypass(true);
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (GlobalOptions.HasScaleAllText)
            {
                _labelFormat.FontSize *= Math.Max(scaleX, scaleY);
            }
            base.ScaleChildren(scaleX, scaleY);
        }

        public override void MouseUp(System.Windows.Point location)
        {
            base.MouseUp(location);

            switch (SwitchPosition)
            {
                case ThreeWayToggleSwitchPosition.One:
                    if (SwitchType == ThreeWayToggleSwitchType.MomOnMom || SwitchType == ThreeWayToggleSwitchType.MomOnOn)
                    {
                        SwitchPosition = ThreeWayToggleSwitchPosition.Two;
                    }
                    break;
                case ThreeWayToggleSwitchPosition.Three:
                    if (SwitchType == ThreeWayToggleSwitchType.OnOnMom || SwitchType == ThreeWayToggleSwitchType.MomOnMom)
                    {
                        SwitchPosition = ThreeWayToggleSwitchPosition.Two;
                    }
                    break;
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
                        _labelFormat.FontSize = _labelFormat.ConfiguredFontSize;
                        break;
                    }
                    // avoid accumulating error from repeated resizing by calculating from a reference point
                    Logger.Debug("scaling font based on new height {Height} versus reference {ReferenceSize} at height {ReferenceHeight}",
                        current.Height, _labelFormat.ConfiguredFontSize, _referenceHeight);
                    _labelFormat.FontSize = Clamp(_labelFormat.ConfiguredFontSize * current.Height / _referenceHeight, 1, 2000);
                    break;
                case TextScalingMode.None:
                    return;
                case TextScalingMode.Legacy:
                    if (previous.Height != 0)
                    {
                        double scale = current.Height / previous.Height;
                        _labelFormat.FontSize = Clamp(scale * _labelFormat.FontSize, 1, 100);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Logger.Debug("Font Size " + _labelFormat.FontSize);
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter pointConverter = TypeDescriptor.GetConverter(typeof(Point));

            base.WriteXml(writer);
            if (Text != null && Text.Length > 0)
            {
                writer.WriteStartElement("Text");

                writer.WriteElementString("Color", colorConverter.ConvertToInvariantString(TextColor));

                writer.WriteStartElement("Font");
                TextFormat.WriteXml(writer);
                writer.WriteEndElement();

                writer.WriteElementString("TextValue", Text);
                if (TextPushOffset != new Point(0, 0))
                {
                    writer.WriteElementString("TextPushOffset", pointConverter.ConvertToString(null, CultureInfo.InvariantCulture, TextPushOffset));
                }
                if (ScalingMode != TextScalingMode.Legacy)
                {
                    writer.WriteElementString("ScalingMode", ScalingMode.ToString());
                }
                writer.WriteEndElement();
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));
            TypeConverter pointConverter = TypeDescriptor.GetConverter(typeof(Point));

            base.ReadXml(reader);

            if (reader.Name.Equals("Text"))
            {
                reader.ReadStartElement("Text");
                if (reader.Name.Equals("Color"))
                {
                    TextColor = (Color)colorConverter.ConvertFromInvariantString(reader.ReadElementString("Color"));
                }
                if (reader.Name.Equals("Font"))
                {
                    reader.ReadStartElement("Font");
                    TextFormat.ReadXml(reader);
                    reader.ReadEndElement();
                }
                Text = reader.ReadElementString("TextValue");
                if (reader.Name.Equals("TextPushOffset"))
                {
                    TextPushOffset = (Point)pointConverter.ConvertFromInvariantString(reader.ReadElementString("TextPushOffset"));
                }
                if (reader.Name.Equals("ScalingMode") && Enum.TryParse(reader.ReadElementString("ScalingMode"), out TextScalingMode configured))
                {
                    ScalingMode = configured;
                }
                else
                {
                    ScalingMode = TextScalingMode.Legacy;
                }
                reader.ReadEndElement();

                // now the auto scaling has messed up our font size, so we restore it
                TextFormat.FontSize = TextFormat.ConfiguredFontSize;
                _referenceHeight = Height;
            } else
            {
                Text = "";
            }

        }

        #endregion

        #region Actions

        void SetPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                BeginTriggerBypass(e.BypassCascadingTriggers);
                int newPosition = 0;
                if (int.TryParse(e.Value.StringValue, out newPosition))
                {
                    if (newPosition > 0 && newPosition <= 3)
                    {
                        SwitchPosition = (ThreeWayToggleSwitchPosition)newPosition;
                    }
                }
                EndTriggerBypass(e.BypassCascadingTriggers);
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        #endregion
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
