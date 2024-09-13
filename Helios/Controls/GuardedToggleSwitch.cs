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
    using System.Windows;

    [HeliosControl("Helios.Base.GuardedToggleSwitch", "Guarded Toggle Switch", "Toggle Switches", typeof(GuardedToggleSwitchRenderer))]
    public class GuardedToggleSwitch : HeliosVisual, IRefreshableImage
    {
        private static readonly Rect GuardUpRegion = new Rect(0, 0, 65, 123);
        private static readonly Rect SwitchRegion = new Rect(0, 123, 65, 80);
        private static readonly Rect GuardDownRegion = new Rect(0, 75, 65, 174);

        private LinearClickType _clickType = LinearClickType.Swipe;
        private ToggleSwitchOrientation _orientation;

        private ToggleSwitchType _switchType = ToggleSwitchType.OnOn;
        private ToggleSwitchPosition _position = ToggleSwitchPosition.Two;
        private GuardPosition _guardPosition = GuardPosition.Down;

        private string _positionOneGuardUpImage;
        private string _positionOneGuardDownImage;

        private string _positionTwoGuardUpImage;
        private string _positionTwoGuardDownImage;

        private ToggleSwitchPosition _defaultPosition = ToggleSwitchPosition.Two;
        private GuardPosition _defaultGuardPosition = GuardPosition.Down;

        private HeliosValue _positionValue;
        private HeliosValue _guardPositionValue;
        private HeliosTrigger _positionOneEnterAction;
        private HeliosTrigger _positionOneExitAction;
        private HeliosTrigger _positionTwoEnterAction;
        private HeliosTrigger _positionTwoExitAction;
        private HeliosTrigger _releaseTrigger;

        private HeliosTrigger _guardUpAction;
        private HeliosTrigger _guardDownAction;

        private Rect _guardUpRegion = GuardedToggleSwitch.GuardUpRegion;
        private Rect _switchRegion = GuardedToggleSwitch.SwitchRegion;
        private Rect _guardDownRegion = GuardedToggleSwitch.GuardDownRegion;

        private Point _mouseDownLocation;
        private bool _mouseAction;

        public GuardedToggleSwitch()
            : base("Guarded Toggle Switch", new System.Windows.Size(65, 249))
        {
            _positionOneGuardUpImage = "{Helios}/Images/Toggles/guard-up-on.png";
            _positionOneGuardDownImage = "{Helios}/Images/Toggles/guard-down-on.png";
            _positionTwoGuardUpImage = "{Helios}/Images/Toggles/guard-up-off.png";
            _positionTwoGuardDownImage = "{Helios}/Images/Toggles/guard-down-off.png";

            _positionOneEnterAction = new HeliosTrigger(this, "", "position one", "entered", "Triggered when position one is entered or depressed.");
            Triggers.Add(_positionOneEnterAction);
            _positionOneExitAction = new HeliosTrigger(this, "", "position one", "exited", "Triggered when posotion one is exited or released.");
            Triggers.Add(_positionOneExitAction);
            _positionTwoEnterAction = new HeliosTrigger(this, "", "position two", "entered", "Triggered when position two is entered or depressed.");
            Triggers.Add(_positionTwoEnterAction);
            _positionTwoExitAction = new HeliosTrigger(this, "", "position two", "exited", "Triggered when position two is exited or released.");
            Triggers.Add(_positionTwoExitAction);
            _guardUpAction = new HeliosTrigger(this, "", "guard", "up", "Triggered when guard is moved up.");
            Triggers.Add(_guardUpAction);
            _guardDownAction = new HeliosTrigger(this, "", "guard", "down", "Triggered when guard is moved down.");
            Triggers.Add(_guardDownAction);
            _releaseTrigger = new HeliosTrigger(this, "", "", "released", "This trigger is fired when the user releases pressure on the switch (lifts finger or mouse button.).");
            Triggers.Add(_releaseTrigger);

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the switch.", "Position number 1 or 2.  Positions are numbered from top to bottom.", BindingValueUnits.Numeric);
            _positionValue.Execute += new HeliosActionHandler(SetPositionAction_Execute);
            Values.Add(_positionValue);
            Actions.Add(_positionValue);
            Triggers.Add(_positionValue);

            _guardPositionValue = new HeliosValue(this, new BindingValue((double)GuardPosition), "", "guard position", "Current position of the switch guard.", "1 = Up, 2 = Down.", BindingValueUnits.Numeric);
            _guardPositionValue.Execute += new HeliosActionHandler(SetGuardPositionAction_Execute);
            Values.Add(_guardPositionValue);
            Actions.Add(_guardPositionValue);
            Triggers.Add(_guardPositionValue);
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

        public ToggleSwitchPosition SwitchPosition
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    ToggleSwitchPosition oldValue = _position;

                    _position = value;
                    _positionValue.SetValue(new BindingValue((double)_position), BypassTriggers);

                    if (!BypassTriggers)
                    {
                        switch (oldValue)
                        {
                            case ToggleSwitchPosition.One:
                                _positionOneExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ToggleSwitchPosition.Two:
                                _positionTwoExitAction.FireTrigger(BindingValue.Empty);
                                break;
                        }

                        switch (value)
                        {
                            case ToggleSwitchPosition.One:
                                _positionOneEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ToggleSwitchPosition.Two:
                                _positionTwoEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                        }
                    }

                    OnPropertyChanged("SwitchPosition", oldValue, value, false);
                    OnDisplayUpdate();
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

        public string PositionOneGuardDownImage
        {
            get
            {
                return _positionOneGuardDownImage;
            }
            set
            {
                if ((_positionOneGuardDownImage == null && value != null)
                    || (_positionOneGuardDownImage != null && !_positionOneGuardDownImage.Equals(value)))
                {
                    string oldValue = _positionOneGuardDownImage;
                    _positionOneGuardDownImage = value;
                    OnPropertyChanged("PositionOneGuardDownImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionTwoGuardUpImage
        {
            get
            {
                return _positionTwoGuardUpImage;
            }
            set
            {
                if ((_positionTwoGuardUpImage == null && value != null)
                    || (_positionTwoGuardUpImage != null && !_positionTwoGuardUpImage.Equals(value)))
                {
                    string oldValue = _positionTwoGuardUpImage;
                    _positionTwoGuardUpImage = value;
                    OnPropertyChanged("PositionTwoGuardUpImage", oldValue, value, true);
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

                _guardUpRegion = GuardedToggleSwitch.GuardUpRegion; 
                _guardUpRegion.Scale(scaleX, scaleY);
                _switchRegion = GuardedToggleSwitch.SwitchRegion;
                _switchRegion.Scale(scaleX, scaleY);
                _guardDownRegion = GuardedToggleSwitch.GuardDownRegion;
                _guardDownRegion.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }
        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((PositionOneGuardDownImage ?? "")   .ToLower().Replace("/", @"\") == imageName ||
                    (PositionOneGuardUpImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoGuardDownImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoGuardUpImage ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
            }
            return ImageRefresh;
        }
        #region HeliosControl Implementation

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            SwitchPosition = DefaultPosition;
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
            else if (ClickType == LinearClickType.Touch)
            {
                switch (GuardPosition)
                {
                    case GuardPosition.Up:
                        if (_guardUpRegion.Contains(location))
                        {
                            GuardPosition = Controls.GuardPosition.Down;
                        }
                        else if (_switchRegion.Contains(location))
                        {
                            switch (SwitchPosition)
                            {
                                case ToggleSwitchPosition.One:
                                    if (location.Y - _switchRegion.Top > _switchRegion.Height / 2d)
                                    {
                                        SwitchPosition = ToggleSwitchPosition.Two;
                                    }
                                    break;
                                case ToggleSwitchPosition.Two:
                                    if (location.Y - _switchRegion.Top < _switchRegion.Height / 2d)
                                    {
                                        SwitchPosition = ToggleSwitchPosition.One;
                                    }
                                    break;
                            }
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
                else if (GuardPosition == Controls.GuardPosition.Up && _switchRegion.Contains(_mouseDownLocation))
                {
                    if (SwitchPosition == ToggleSwitchPosition.One && swipeVector.Y > 5)
                    {
                        SwitchPosition = ToggleSwitchPosition.Two;
                        _mouseAction = true;
                    }
                    else if (SwitchPosition == ToggleSwitchPosition.Two && swipeVector.Y < -5)
                    {
                        SwitchPosition = ToggleSwitchPosition.One;
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

            switch (SwitchPosition)
            {
                case ToggleSwitchPosition.One:
                    if (SwitchType == ToggleSwitchType.MomOn)
                    {
                        SwitchPosition = ToggleSwitchPosition.Two;
                    }
                    break;
                case ToggleSwitchPosition.Two:
                    if (SwitchType == ToggleSwitchType.OnMom)
                    {
                        SwitchPosition = ToggleSwitchPosition.One;
                    }
                    break;
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
            writer.WriteElementString("PositionTwoImage", PositionTwoGuardUpImage);
            writer.WriteEndElement();
            writer.WriteStartElement("GuardDown");
            writer.WriteElementString("PositionOneImage", PositionOneGuardDownImage);
            writer.WriteElementString("PositionTwoImage", PositionTwoGuardDownImage);
            writer.WriteEndElement();
            writer.WriteElementString("DefaultPosition", DefaultPosition.ToString());
            writer.WriteElementString("DefaultGuardPosition", DefaultGuardPosition.ToString());
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
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
            PositionTwoGuardUpImage = reader.ReadElementString("PositionTwoImage");
            reader.ReadEndElement();

            reader.ReadStartElement("GuardDown");
            PositionOneGuardDownImage = reader.ReadElementString("PositionOneImage");
            PositionTwoGuardDownImage = reader.ReadElementString("PositionTwoImage");
            reader.ReadEndElement();

            DefaultPosition = (ToggleSwitchPosition)Enum.Parse(typeof(ToggleSwitchPosition), reader.ReadElementString("DefaultPosition"));
            DefaultGuardPosition = (GuardPosition)Enum.Parse(typeof(GuardPosition), reader.ReadElementString("DefaultGuardPosition"));

            BeginTriggerBypass(true);
            SwitchPosition = DefaultPosition;
            GuardPosition = DefaultGuardPosition;
            EndTriggerBypass(true);
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
                    if (newPosition > 0 && newPosition < 3)
                    {
                        SwitchPosition = (ToggleSwitchPosition)newPosition;
                    }
                }

                EndTriggerBypass(e.BypassCascadingTriggers);
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

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
