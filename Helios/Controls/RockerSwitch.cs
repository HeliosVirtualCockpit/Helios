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
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Base.RockerSwitch", "Rocker Switch", "Rocker Switches", typeof(RockerSwitchRenderer))]
    public class RockerSwitch : ThreeWayToggleSwitch
    {
        private string _label = "";
        private TextFormat _labelFormat = new TextFormat();
        private Color _labelColor = Color.FromArgb(0xe0,0xff,0xff,0xff);
        private Point _labelPushedOffset = new Point(0, 0);

        public RockerSwitch()
            : base("Rocker Switch", new Size(50,100))
        {
            _labelFormat.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Format_PropertyChanged);

            _labelFormat.FontSize = 20;
            _labelFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
            _labelFormat.VerticalAlignment = TextVerticalAlignment.Center;

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

                writer.WriteElementString("Text", Text);
                if (TextPushOffset != new Point(0, 0))
                {
                    writer.WriteElementString("TextPushOffset", pointConverter.ConvertToString(null, CultureInfo.InvariantCulture, TextPushOffset));
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

                TextColor = (Color)colorConverter.ConvertFromInvariantString(reader.ReadElementString("Color"));
                reader.ReadStartElement("Font");
                TextFormat.ReadXml(reader);
                reader.ReadEndElement();

                Text = reader.ReadElementString("Text");
                if (reader.Name.Equals("TextPushOffset"))
                {
                    TextPushOffset = (Point)pointConverter.ConvertFromInvariantString(reader.ReadElementString("TextPushOffset"));
                }
                reader.ReadEndElement();
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
    }
}
