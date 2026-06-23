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
    using System.Windows;

    [HeliosControl("Helios.Base.GuardedSwitch", "Guard Empty", "Miscellaneous", typeof(GuardedSwitchRenderer))]
    class GuardedSwitch : HeliosVisual
    {
        private static readonly Rect GuardUpRegion = new Rect(0, 0, 65, 123);
        private static readonly Rect SwitchRegion = new Rect(0, 0, 0, 0);
        private static readonly Rect GuardDownRegion = new Rect(0, 75, 65, 174);

        private LinearClickType _clickType = LinearClickType.Swipe;
        private ToggleSwitchOrientation _orientation;

        private ToggleSwitchType _switchType = ToggleSwitchType.OnOn;
        private GuardPosition _guardPosition = GuardPosition.Down;

        private string _positionOneGuardUpImage;
        private string _positionTwoGuardDownImage;

        private ToggleSwitchPosition _defaultPosition = ToggleSwitchPosition.Two;
        private GuardPosition _defaultGuardPosition = GuardPosition.Down;

        private HeliosValue _guardPositionValue;
        private HeliosTrigger _releaseTrigger;

        private HeliosTrigger _guardUpAction;
        private HeliosTrigger _guardDownAction;

        private Rect _guardUpRegion = GuardedSwitch.GuardUpRegion;
        private Rect _switchRegion = GuardedSwitch.SwitchRegion;
        private Rect _guardDownRegion = GuardedSwitch.GuardDownRegion;

        private Point _mouseDownLocation;
        private bool _mouseAction;

        public GuardedSwitch()
            : base("Guard Empty", new System.Windows.Size(65, 249))
        {
            _positionOneGuardUpImage = "{Helios}/Images/Toggles/guard-up-empty.png";
            _positionTwoGuardDownImage = "{Helios}/Images/Toggles/guard-down-off.png";

            _guardPositionValue = new HeliosValue(this, new BindingValue((double)GuardPosition), "", "guard position", "Current position of the switch guard.", "1 = Up, 2 = Down.", BindingValueUnits.Numeric);
            _guardPositionValue.Execute += new HeliosActionHandler(SetGuardPositionAction_Execute);
            Values.Add(_guardPositionValue);
            Actions.Add(_guardPositionValue);
            Triggers.Add(_guardPositionValue);

            _guardUpAction = new HeliosTrigger(this, "", "guard", "up", "Triggered when guard is moved up.");
            Triggers.Add(_guardUpAction);
            _guardDownAction = new HeliosTrigger(this, "", "guard", "down", "Triggered when guard is moved down.");
            Triggers.Add(_guardDownAction);
            _releaseTrigger = new HeliosTrigger(this, "", "", "released", "This trigger is fired when the user releases pressure on the switch (lifts finger or mouse button.).");
            Triggers.Add(_releaseTrigger);

        }

        #region Properties

        public LinearClickType ClickType
        {
            get
            {
                return _clickType;
            }
            set
            {
                if (!_clickType.Equals(value))
                {
                    LinearClickType oldValue = _clickType;
                    _clickType = value;
                    OnPropertyChanged("ClickType", oldValue, value, true);
                }
            }
        }

        public ToggleSwitchOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (!_orientation.Equals(value))
                {
                    ToggleSwitchOrientation oldValue = _orientation;
                    _orientation = value;
                    OnPropertyChanged("Orientation", oldValue, value, true);
                }
            }
        }

        public GuardPosition GuardPosition
        {
            get
            {
                return _guardPosition;
            }
            set
            {
                if (!_guardPosition.Equals(value))
                {
                    GuardPosition oldValue = _guardPosition;
                    _guardPosition = value;

                    _guardPositionValue.SetValue(new BindingValue((double)_guardPosition), BypassTriggers);
                    if (!BypassTriggers)
                    {
                        switch (value)
                        {
                            case GuardPosition.Up:
                                _guardUpAction.FireTrigger(BindingValue.Empty);
                                break;
                            case GuardPosition.Down:
                                _guardDownAction.FireTrigger(BindingValue.Empty);
                                break;
                        }

                    }

                    OnPropertyChanged("GuardPosition", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public ToggleSwitchType SwitchType
        {
            get
            {
                return _switchType;
            }
            set
            {
                if (!_switchType.Equals(value))
                {
                    ToggleSwitchType oldValue = _switchType;
                    _switchType = value;
                    OnPropertyChanged("SwitchType", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

      public ToggleSwitchPosition DefaultPosition
      {
          get
          {
              return _defaultPosition;
          }
          set
          {
              if (!_defaultPosition.Equals(value))
              {
                  ToggleSwitchPosition oldValue = _defaultPosition;
                  _defaultPosition = value;
                  OnPropertyChanged("DefaultPosition", oldValue, value, true);
              }
          }
      }

        public GuardPosition DefaultGuardPosition
        {
            get
            {
                return _defaultGuardPosition;
            }
            set
            {
                if (!_defaultGuardPosition.Equals(value))
                {
                    GuardPosition oldValue = _defaultGuardPosition;
                    _defaultGuardPosition = value;
                    OnPropertyChanged("DefaultGuardPosition", oldValue, value, true);
                }
            }
        }


        public string PositionOneGuardUpImage
        {
            get
            {
                return _positionOneGuardUpImage;
            }
            set
            {
                if ((_positionOneGuardUpImage == null && value != null)
                    || (_positionOneGuardUpImage != null && !_positionOneGuardUpImage.Equals(value)))
                {
                    string oldValue = _positionOneGuardUpImage;
                    _positionOneGuardUpImage = value;
                    OnPropertyChanged("PositionOneGuardUpImage", oldValue, value, true);
                    Refresh();
                }
            }
        }



        public string PositionTwoGuardDownImage
        {
            get
            {
                return _positionTwoGuardDownImage;
            }
            set
            {
                if ((_positionTwoGuardDownImage == null && value != null)
                    || (_positionTwoGuardDownImage != null && !_positionTwoGuardDownImage.Equals(value)))
                {
                    string oldValue = _positionTwoGuardDownImage;
                    _positionTwoGuardDownImage = value;
                    OnPropertyChanged("PositionTwoGuardDownImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        #endregion

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                double scaleX = Width / NativeSize.Width;
                double scaleY = Height / NativeSize.Height;

                _guardUpRegion = GuardedSwitch.GuardUpRegion; 
                _guardUpRegion.Scale(scaleX, scaleY);
                _switchRegion = GuardedSwitch.SwitchRegion;
                _switchRegion.Scale(scaleX, scaleY);
                _guardDownRegion = GuardedSwitch.GuardDownRegion;
                _guardDownRegion.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }

        #region HeliosControl Implementation

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            GuardPosition = DefaultGuardPosition;
            EndTriggerBypass(true);
        }

        public override bool HitTest(Point location)
        {
            

            switch (GuardPosition)
            {
                case GuardPosition.Up:
                    return _guardUpRegion.Contains(location) || _switchRegion.Contains(location);
                case GuardPosition.Down:
                    return _guardDownRegion.Contains(location);
            }
            return false;
        }

        public override void MouseDown(Point location)
        {
            if (ClickType == LinearClickType.Swipe)
            {
                _mouseDownLocation = location;
                _mouseAction = false;
            }
            else if (ClickType == LinearClickType.Touch || ClickType == LinearClickType.OneTouch)
            {
                switch (GuardPosition)
                {
                    case GuardPosition.Up:
                        if (_guardUpRegion.Contains(location))
                        {
                            GuardPosition = Controls.GuardPosition.Down;
                        }
                        break;
                    case GuardPosition.Down:
                        if (_guardDownRegion.Contains(location))
                        {
                            GuardPosition = Controls.GuardPosition.Up;
                        }
                        break;
                }
            }
        }

        public override void MouseDrag(Point location)
        {
            if (!_mouseAction)
            {
                Vector swipeVector = location - _mouseDownLocation;
                if (GuardPosition == Controls.GuardPosition.Down && _guardDownRegion.Contains(_mouseDownLocation))
                {
                    if (swipeVector.Y < -5)
                    {
                        GuardPosition = GuardPosition.Up;
                        _mouseAction = true;
                    }
                }
                else if (GuardPosition == Controls.GuardPosition.Up && _guardUpRegion.Contains(_mouseDownLocation))
                {
                    if (swipeVector.Y > 5)
                    {
                        GuardPosition = GuardPosition.Down;
                        _mouseAction = true;
                    }
                }
            
            }
        }

        public override void MouseUp(Point location)
        {
            if (_mouseAction)
            {
                _releaseTrigger.FireTrigger(BindingValue.Empty);
                _mouseAction = false;
            }

        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SwitchType", SwitchType.ToString());
            writer.WriteElementString("Orientation", Orientation.ToString());
            writer.WriteElementString("ClickType", ClickType.ToString());
            writer.WriteStartElement("GuardUp");
            writer.WriteElementString("PositionOneImage", PositionOneGuardUpImage);
            writer.WriteElementString("PositionTwoImage", PositionTwoGuardDownImage);// writing this value just for compatibility with old profiles
			writer.WriteEndElement();
            writer.WriteStartElement("GuardDown");
            writer.WriteElementString("PositionOneImage", PositionOneGuardUpImage);// writing this value just for compatibility with old profiles
			writer.WriteElementString("PositionTwoImage", PositionTwoGuardDownImage);
            writer.WriteEndElement();
            writer.WriteElementString("DefaultPosition", "Two"); // writing this value just for compatibility with old profiles
			writer.WriteElementString("DefaultGuardPosition", DefaultGuardPosition.ToString());
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
			string trash;
			SwitchType = (ToggleSwitchType)Enum.Parse(typeof(ToggleSwitchType), reader.ReadElementString("SwitchType"));
            Orientation = (ToggleSwitchOrientation)Enum.Parse(typeof(ToggleSwitchOrientation), reader.ReadElementString("Orientation"));
            if (reader.Name.Equals("ClickType"))
            {
                ClickType = (LinearClickType)Enum.Parse(typeof(LinearClickType), reader.ReadElementString("ClickType"));
            }
            else
            {
                ClickType = LinearClickType.Swipe;
            }

            reader.ReadStartElement("GuardUp");
            PositionOneGuardUpImage = reader.ReadElementString("PositionOneImage");
			trash = reader.ReadElementString("PositionTwoImage"); // reading this value just for compatibility with old profiles
			reader.ReadEndElement();

            reader.ReadStartElement("GuardDown"); 
			trash = reader.ReadElementString("PositionOneImage"); // reading this value just for compatibility with old profiles
            PositionTwoGuardDownImage = reader.ReadElementString("PositionTwoImage");
            reader.ReadEndElement();

			trash = reader.ReadElementString("DefaultPosition"); // reading this value just for compatibility with old profiles
			DefaultGuardPosition = (GuardPosition)Enum.Parse(typeof(GuardPosition), reader.ReadElementString("DefaultGuardPosition"));

            BeginTriggerBypass(true);

            GuardPosition = DefaultGuardPosition;
            EndTriggerBypass(true);
        }

        #endregion

        #region Actions

   
        void SetGuardPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                BeginTriggerBypass(e.BypassCascadingTriggers);
                int newPosition = 0;
                if (int.TryParse(e.Value.StringValue, out newPosition))
                {
                    if (newPosition > 0 && newPosition < 3)
                    {
                        GuardPosition = (GuardPosition)newPosition;
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
