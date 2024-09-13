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
    using System;
    using System.Xml;

    [HeliosControl("Helios.Base.KneeboardSwitch", "Kneeboard Switch", "Rockers", typeof(KneeboardSwitchRenderer))]
    public class KneeboardSwitch : ToggleSwitchBase, IRefreshableImage
    {
        private double _value = 0.0d;
        private double _value_inc=1;
        private double _value_dec=0;

        private KneeboardSwitchType _switchType = KneeboardSwitchType.MomOnMom;
        private KneeboardSwitchPosition _position = KneeboardSwitchPosition.Two;
        private string _positionOneImage;
        private string _positionOneImageIndicatorOn;
        private string _positionTwoImage;
        private string _positionTwoImageIndicatorOn;
        private string _positionThreeImage;
        private string _positionThreeImageIndicatorOn;

        private KneeboardSwitchPosition _defaultPosition = KneeboardSwitchPosition.Two;

        private HeliosValue _positionValue;
        private HeliosValue _KneeboardPosition;

        public KneeboardSwitch()
            : base("Kneeboard Switch", new System.Windows.Size(50, 100))
        {
            _positionOneImage = "{Helios}/Images/Rockers/triangles-light-up.png";
            _positionTwoImage = "{Helios}/Images/Rockers/triangles-light-norm.png";
            _positionThreeImage = "{Helios}/Images/Rockers/triangles-light-down.png";

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the switch.", "1,2,3 1 is top position", BindingValueUnits.Numeric);
            _positionValue.Execute += new HeliosActionHandler(SetPositionAction_Execute);

     
            _KneeboardPosition = new HeliosValue(this, new BindingValue(4), "", "Kneeboard position", "Current position of the KneeBoard.", "", BindingValueUnits.Numeric);
          
            _KneeboardPosition.Execute += new HeliosActionHandler(SetValue_Execute);
            _value = 0;
            _KneeboardPosition.SetValue(new BindingValue(4), BypassTriggers);

            Values.Add(_positionValue);
            Values.Add(_KneeboardPosition);
            Actions.Add(_positionValue);
            Actions.Add(_KneeboardPosition);
            Triggers.Add(_positionValue);
            Triggers.Add(_KneeboardPosition);
        }

        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((PositionOneImage?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionOneIndicatorOnImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoIndicatorOnImage ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
            }
            return ImageRefresh;
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
                if (!_value.Equals(value))
                {
                    double oldValue = _value;
                    _value = value;
                    _KneeboardPosition.SetValue(new BindingValue(_value), BypassTriggers);
                    _value_inc = _value + 1;
                    _value_dec = _value -1;
                   if (_value_dec < 0)
                       {
                       _value_dec = 0;
                       }
                    OnPropertyChanged("Value", oldValue, value, true);
                }
            }
        }

        public KneeboardSwitchPosition DefaultPosition
        {
            get
            {
                return _defaultPosition;
            }
            set
            {
                if (!_defaultPosition.Equals(value))
                {
                    KneeboardSwitchPosition oldValue = _defaultPosition;
                    _defaultPosition = value;
                    OnPropertyChanged("DefaultPosition", oldValue, value, true);
                }
            }
        }

        public KneeboardSwitchType SwitchType
        {
            get
            {
                return _switchType;
            }
            set
            {
                if (!_switchType.Equals(value))
                {
                    KneeboardSwitchType oldValue = _switchType;
                    _switchType = value;
                    OnPropertyChanged("SwitchType", oldValue, value, true);
                }
            }
        }

        public KneeboardSwitchPosition SwitchPosition
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    KneeboardSwitchPosition oldValue = _position;

                    _position = value;
                    _positionValue.SetValue(new BindingValue((double)_position), BypassTriggers);

                    if (!BypassTriggers)
                    {
                        switch (value)
                        {
                            case KneeboardSwitchPosition.One:
                                _KneeboardPosition.SetValue(new BindingValue(_value_inc), BypassTriggers); // increment kneeboard position
                                 break;
                            case KneeboardSwitchPosition.Two:
                               
                                break;
                            case KneeboardSwitchPosition.Three:
                                _KneeboardPosition.SetValue(new BindingValue(_value_dec), BypassTriggers);  // decrement kneeboard position
                                break;
                        }
                    }

                    OnPropertyChanged("SwitchPosition", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public string PositionOneImage
        {
            get
            {
                return _positionOneImage;
            }
            set
            {
                if ((_positionOneImage == null && value != null)
                    || (_positionOneImage != null && !_positionOneImage.Equals(value)))
                {
                    string oldValue = _positionOneImage;
                    _positionOneImage = value;
                    OnPropertyChanged("PositionOneImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionOneIndicatorOnImage
        {
          get
          {
              return _positionOneImageIndicatorOn;
          }
          set
          {
              if ((_positionOneImageIndicatorOn == null && value != null)
                  || (_positionOneImageIndicatorOn != null && !_positionOneImageIndicatorOn.Equals(value)))
              {
                  string oldValue = _positionOneImageIndicatorOn;
                  _positionOneImageIndicatorOn = value;
                  OnPropertyChanged("PositionOneIndicatorOnImage", oldValue, value, true);
                  Refresh();
              }
          }
        }

        public string PositionThreeImage
        {
            get
            {
                return _positionThreeImage;
            }
            set
            {
                if ((_positionThreeImage == null && value != null)
                    || (_positionThreeImage != null && !_positionThreeImage.Equals(value)))
                {
                    string oldValue = _positionThreeImage;
                    _positionThreeImage = value;
                    OnPropertyChanged("PositionThreeImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

           public string PositionThreeIndicatorOnImage
           {
               get
            {
                return _positionThreeImageIndicatorOn;
            }
            set
            {
                if ((_positionThreeImageIndicatorOn == null && value != null)
                    || (_positionThreeImageIndicatorOn != null && !_positionThreeImageIndicatorOn.Equals(value)))
                {
                       string oldValue = _positionThreeImageIndicatorOn;
                       _positionThreeImageIndicatorOn = value;
                       OnPropertyChanged("PositionThreeIndicatorOnImage", oldValue, value, true);
                       Refresh();
                   }
               }
           }


        public string PositionTwoImage
        {
            get
            {
                return _positionTwoImage;
            }
            set
            {
                if ((_positionTwoImage == null && value != null)
                    || (_positionTwoImage != null && !_positionTwoImage.Equals(value)))
                {
                    string oldValue = _positionTwoImage;
                    _positionTwoImage = value;
                    OnPropertyChanged("PositionTwoImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

      public string PositionTwoIndicatorOnImage
      {
          get
          {
              return _positionTwoImageIndicatorOn;
          }
          set
          {
              if ((_positionTwoImageIndicatorOn == null && value != null)
                  || (_positionTwoImageIndicatorOn != null && !_positionTwoImageIndicatorOn.Equals(value)))
              {
                  string oldValue = _positionTwoImageIndicatorOn;
                  _positionTwoImageIndicatorOn = value;
                  OnPropertyChanged("PositionTwoIndicatorOnImage", oldValue, value, true);
                  Refresh();
              }
          }
      }

        #endregion

        #region HeliosControl Implementation

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            SwitchPosition = DefaultPosition;
            EndTriggerBypass(true);
        }

        protected override void ThrowSwitch(ToggleSwitchBase.SwitchAction action)
        {
            if (action == SwitchAction.Increment)
            {
                switch (SwitchPosition)
                {
                    case KneeboardSwitchPosition.One:
                        SwitchPosition = KneeboardSwitchPosition.Two;
                        break;
                    case KneeboardSwitchPosition.Two:
                        SwitchPosition = KneeboardSwitchPosition.Three;
                        break;
                }                
            }
            else if (action == SwitchAction.Decrement)
            {
                switch (SwitchPosition)
                {
                    case KneeboardSwitchPosition.Two:
                        SwitchPosition = KneeboardSwitchPosition.One;
                        break;
                    case KneeboardSwitchPosition.Three:
                        SwitchPosition = KneeboardSwitchPosition.Two;
                        break;
                }
            }
        }

        public override void MouseUp(System.Windows.Point location)
        {
            base.MouseUp(location);

            switch (SwitchPosition)
            {
                case KneeboardSwitchPosition.One:
                    if (SwitchType == KneeboardSwitchType.MomOnMom || SwitchType == KneeboardSwitchType.MomOnOn)
                    {
                        SwitchPosition = KneeboardSwitchPosition.Two;
                    }
                    break;
                case KneeboardSwitchPosition.Three:
                    if (SwitchType == KneeboardSwitchType.OnOnMom || SwitchType == KneeboardSwitchType.MomOnMom)
                    {
                        SwitchPosition = KneeboardSwitchPosition.Two;
                    }
                    break;
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SwitchType", SwitchType.ToString());
            writer.WriteElementString("Orientation", Orientation.ToString());
            writer.WriteElementString("ClickType", ClickType.ToString());
            writer.WriteElementString("PositionOneImage", PositionOneImage);
            writer.WriteElementString("PositionTwoImage", PositionTwoImage);
            writer.WriteElementString("PositionThreeImage", PositionThreeImage);
            if (HasIndicator)
            {
                writer.WriteStartElement("Indicator");
                writer.WriteElementString("PositionOneImage", PositionOneIndicatorOnImage);
                writer.WriteElementString("PositionTwoImage", PositionTwoIndicatorOnImage);
                writer.WriteElementString("PositionThreeImage", PositionThreeIndicatorOnImage);
                writer.WriteEndElement();
            }
            writer.WriteElementString("DefaultPosition", DefaultPosition.ToString());
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            SwitchType = (KneeboardSwitchType)Enum.Parse(typeof(KneeboardSwitchType), reader.ReadElementString("SwitchType"));
            Orientation = (ToggleSwitchOrientation)Enum.Parse(typeof(ToggleSwitchOrientation), reader.ReadElementString("Orientation"));
            if (reader.Name.Equals("ClickType"))
            {
                ClickType = (LinearClickType)Enum.Parse(typeof(LinearClickType), reader.ReadElementString("ClickType"));
            }
            else
            {
                ClickType = LinearClickType.Swipe;
            }
            PositionOneImage = reader.ReadElementString("PositionOneImage");
            PositionTwoImage = reader.ReadElementString("PositionTwoImage");
            PositionThreeImage = reader.ReadElementString("PositionThreeImage");
            if (reader.Name == "Indicator")
            {
                HasIndicator = true;
                reader.ReadStartElement("Indicator");
                PositionOneIndicatorOnImage = reader.ReadElementString("PositionOneImage");
                PositionTwoIndicatorOnImage = reader.ReadElementString("PositionTwoImage");
                PositionThreeIndicatorOnImage = reader.ReadElementString("PositionThreeImage");
                reader.ReadEndElement();
            }
            else
            {
                HasIndicator = false;
            }
            if (reader.Name == "DefaultPosition")
            {
                DefaultPosition = (KneeboardSwitchPosition)Enum.Parse(typeof(KneeboardSwitchPosition), reader.ReadElementString("DefaultPosition"));
                BeginTriggerBypass(true);
                SwitchPosition = DefaultPosition;
                EndTriggerBypass(true);
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
                        SwitchPosition = (KneeboardSwitchPosition)newPosition;
                    }
                }
                EndTriggerBypass(e.BypassCascadingTriggers);
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        void SetValue_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                BeginTriggerBypass(e.BypassCascadingTriggers);
                Value = e.Value.DoubleValue;
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
