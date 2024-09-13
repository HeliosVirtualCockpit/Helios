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

    [HeliosControl("Helios.Base.GuardedThreeWayToggle", "Guarded Three Way Toggle Switch", "Three Way Toggle Switches", typeof(GuardedThreeWayToggleRenderer))]
    public class GuardedThreeWayToggle : HeliosVisual, IRefreshableImage
    {
        private static readonly Rect GuardUpRegion = new Rect(0, 0, 65, 123);
        private static readonly Rect SwitchRegion = new Rect(0, 123, 65, 80);
        private static readonly Rect GuardDownRegion = new Rect(0, 75, 65, 174);

        private LinearClickType _clickType = LinearClickType.Swipe;
        private ToggleSwitchOrientation _orientation;

        private ThreeWayToggleSwitchType _switchType = ThreeWayToggleSwitchType.OnOnOn;
        private ThreeWayToggleSwitchPosition _position = ThreeWayToggleSwitchPosition.Two;
        private GuardPosition _guardPosition = GuardPosition.Down;

        private string _positionOneGuardUpImage;
        private string _positionOneGuardDownImage;
        private string _positionTwoGuardUpImage;
        private string _positionTwoGuardDownImage;
        private string _positionThreeGuardUpImage;
        private string _positionThreeGuardDownImage;

        private ThreeWayToggleSwitchPosition _defaultPosition = ThreeWayToggleSwitchPosition.Two;
        private GuardPosition _defaultGuardPosition = GuardPosition.Down;

        private HeliosValue _positionValue;
        private HeliosValue _guardPositionValue;
        private HeliosTrigger _positionOneEnterAction;
        private HeliosTrigger _positionOneExitAction;
        private HeliosTrigger _positionTwoEnterAction;
        private HeliosTrigger _positionTwoExitAction;
        private HeliosTrigger _positionThreeEnterAction;
        private HeliosTrigger _positionThreeExitAction;
        private HeliosTrigger _releaseTrigger;

        private HeliosTrigger _guardUpAction;
        private HeliosTrigger _guardDownAction;

        private Rect _guardUpRegion = GuardedThreeWayToggle.GuardUpRegion;
        private Rect _switchRegion = GuardedThreeWayToggle.SwitchRegion;
        private Rect _guardDownRegion = GuardedThreeWayToggle.GuardDownRegion;

        private Point _mouseDownLocation;
        private bool _mouseAction;

        public GuardedThreeWayToggle()
            : base("Guarded Three Way Toggle Switch", new System.Windows.Size(65, 249))
        {
            _positionOneGuardUpImage = "{Helios}/Images/Toggles/guard-up-on.png";
            _positionOneGuardDownImage = "{Helios}/Images/Toggles/guard-down-on.png";
            _positionTwoGuardUpImage = "{Helios}/Images/Toggles/guard-up-norm.png";
            _positionTwoGuardDownImage = "{Helios}/Images/Toggles/guard-down-norm.png";
            _positionThreeGuardUpImage = "{Helios}/Images/Toggles/guard-up-off.png";
            _positionThreeGuardDownImage = "{Helios}/Images/Toggles/guard-down-off.png";

            _positionOneEnterAction = new HeliosTrigger(this, "", "position one", "entered", "Triggered when position one is entered or depressed.");
            Triggers.Add(_positionOneEnterAction);
            _positionOneExitAction = new HeliosTrigger(this, "", "position one", "exited", "Triggered when posotion one is exited or released.");
            Triggers.Add(_positionOneExitAction);
            _positionTwoEnterAction = new HeliosTrigger(this, "", "position two", "entered", "Triggered when position two is entered or depressed.");
            Triggers.Add(_positionTwoEnterAction);
            _positionTwoExitAction = new HeliosTrigger(this, "", "position two", "exited", "Triggered when position two is exited or released.");
            Triggers.Add(_positionTwoExitAction);
            _positionThreeEnterAction = new HeliosTrigger(this, "", "position three", "entered", "Triggered when position three is entered or depressed.");
            Triggers.Add(_positionThreeEnterAction);
            _positionThreeExitAction = new HeliosTrigger(this, "", "position three", "exited", "Triggered when position three is exited or released.");
            Triggers.Add(_positionThreeExitAction);
            _guardUpAction = new HeliosTrigger(this, "", "guard", "up", "Triggered when guard is moved up.");
            Triggers.Add(_guardUpAction);
            _guardDownAction = new HeliosTrigger(this, "", "guard", "down", "Triggered when guard is moved down.");
            Triggers.Add(_guardDownAction);
            _releaseTrigger = new HeliosTrigger(this, "", "", "released", "This trigger is fired when the user releases pressure on the switch (lifts finger or mouse button.).");
            Triggers.Add(_releaseTrigger);

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the switch.", "Position number 1, 2 or 3.  Positions are numbered from top to bottom.", BindingValueUnits.Numeric);
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

        public ThreeWayToggleSwitchType SwitchType
        {
            get
            {
                return _switchType;
            }
            set
            {
                if (!_switchType.Equals(value))
                {
                    ThreeWayToggleSwitchType oldValue = _switchType;
                    _switchType = value;
                    OnPropertyChanged("SwitchType", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public ThreeWayToggleSwitchPosition DefaultPosition
        {
            get
            {
                return _defaultPosition;
            }
            set
            {
                if (!_defaultPosition.Equals(value))
                {
                    ThreeWayToggleSwitchPosition oldValue = _defaultPosition;
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

        public ThreeWayToggleSwitchPosition SwitchPosition
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    ThreeWayToggleSwitchPosition oldValue = _position;

                    if (!BypassTriggers)
                    {
                        switch (oldValue)
                        {
                            case ThreeWayToggleSwitchPosition.One:
                                _positionOneExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ThreeWayToggleSwitchPosition.Two:
                                _positionTwoExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ThreeWayToggleSwitchPosition.Three:
                                _positionThreeExitAction.FireTrigger(BindingValue.Empty);
                                break;
                        }
                    }

                    _position = value;
                    _positionValue.SetValue(new BindingValue((double)_position), BypassTriggers);

                    if (!BypassTriggers)
                    {
                        switch (value)
                        {
                            case ThreeWayToggleSwitchPosition.One:
                                _positionOneEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ThreeWayToggleSwitchPosition.Two:
                                _positionTwoEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ThreeWayToggleSwitchPosition.Three:
                                _positionThreeEnterAction.FireTrigger(BindingValue.Empty);
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

        public string PositionThreeGuardUpImage
        {
            get
            {
                return _positionThreeGuardUpImage;
            }
            set
            {
                if ((_positionThreeGuardUpImage == null && value != null)
                    || (_positionThreeGuardUpImage != null && !_positionThreeGuardUpImage.Equals(value)))
                {
                    string oldValue = _positionThreeGuardUpImage;
                    _positionThreeGuardUpImage = value;
                    OnPropertyChanged("PositionThreeGuardUpImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionThreeGuardDownImage
        {
            get
            {
                return _positionThreeGuardDownImage;
            }
            set
            {
                if ((_positionThreeGuardDownImage == null && value != null)
                    || (_positionThreeGuardDownImage != null && !_positionThreeGuardDownImage.Equals(value)))
                {
                    string oldValue = _positionThreeGuardDownImage;
                    _positionThreeGuardDownImage = value;
                    OnPropertyChanged("PositionThreeGuardDownImage", oldValue, value, true);
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

                _guardUpRegion = GuardedThreeWayToggle.GuardUpRegion; 
                _guardUpRegion.Scale(scaleX, scaleY);
                _switchRegion = GuardedThreeWayToggle.SwitchRegion;
                _switchRegion.Scale(scaleX, scaleY);
                _guardDownRegion = GuardedThreeWayToggle.GuardDownRegion;
                _guardDownRegion.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
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
        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((PositionOneGuardDownImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionOneGuardUpImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoGuardDownImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoGuardUpImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionThreeGuardUpImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionThreeGuardDownImage ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
            }
            return ImageRefresh;
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
                                case ThreeWayToggleSwitchPosition.One:
                                    if (location.Y - _switchRegion.Top > _switchRegion.Height / 2d)
                                    {
                                        SwitchPosition = ThreeWayToggleSwitchPosition.Two;
                                    }
                                    break;
                                case ThreeWayToggleSwitchPosition.Two:
                                    if (location.Y - _switchRegion.Top <= _switchRegion.Height / 2d)
                                    {
                                        SwitchPosition = ThreeWayToggleSwitchPosition.One;
                                    }
                                    else if (location.Y - _switchRegion.Top >= _switchRegion.Height / 2d)
                                    {
                                        SwitchPosition = ThreeWayToggleSwitchPosition.Three;
                                    }
                                    break;
                                case ThreeWayToggleSwitchPosition.Three:
                                    if (location.Y - _switchRegion.Top < _switchRegion.Height / 2d)
                                    {
                                        SwitchPosition = ThreeWayToggleSwitchPosition.Two;
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
                    switch (SwitchPosition)
                    {
                        case ThreeWayToggleSwitchPosition.One:
                            if (swipeVector.Y > 5)
                            {
                                SwitchPosition = ThreeWayToggleSwitchPosition.Two;
                                _mouseAction = true;
                            }
                            break;
                        case ThreeWayToggleSwitchPosition.Two:
                            if (swipeVector.Y > 5)
                            {
                                SwitchPosition = ThreeWayToggleSwitchPosition.Three;
                                _mouseAction = true;
                            }
                            else if (swipeVector.Y < -5)
                            {
                                SwitchPosition = ThreeWayToggleSwitchPosition.One;
                                _mouseAction = true;
                            }
                            break;
                        case ThreeWayToggleSwitchPosition.Three:
                            if ( swipeVector.Y < -5)
                            {
                                SwitchPosition = ThreeWayToggleSwitchPosition.Two;
                                _mouseAction = true;
                            }
                            break;
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
                case ThreeWayToggleSwitchPosition.One:
                    if (SwitchType == ThreeWayToggleSwitchType.MomOnMom || SwitchType == ThreeWayToggleSwitchType.MomOnOn)
                    {
                        SwitchPosition = ThreeWayToggleSwitchPosition.Two;
                    }
                    break;
                case ThreeWayToggleSwitchPosition.Three:
                    if (SwitchType == ThreeWayToggleSwitchType.MomOnMom || SwitchType == ThreeWayToggleSwitchType.OnOnMom)
                    {
                        SwitchPosition = ThreeWayToggleSwitchPosition.Two;
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
            writer.WriteElementString("PositionThreeImage", PositionThreeGuardUpImage);
            writer.WriteEndElement();
            writer.WriteStartElement("GuardDown");
            writer.WriteElementString("PositionOneImage", PositionOneGuardDownImage);
            writer.WriteElementString("PositionTwoImage", PositionTwoGuardDownImage);
            writer.WriteElementString("PositionThreeImage", PositionThreeGuardDownImage);
            writer.WriteEndElement();
            writer.WriteElementString("DefaultPosition", DefaultPosition.ToString());
            writer.WriteElementString("DefaultGuardPosition", DefaultGuardPosition.ToString());
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            SwitchType = (ThreeWayToggleSwitchType)Enum.Parse(typeof(ThreeWayToggleSwitchType), reader.ReadElementString("SwitchType"));
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
            PositionThreeGuardUpImage = reader.ReadElementString("PositionThreeImage");
            reader.ReadEndElement();

            reader.ReadStartElement("GuardDown");
            PositionOneGuardDownImage = reader.ReadElementString("PositionOneImage");
            PositionTwoGuardDownImage = reader.ReadElementString("PositionTwoImage");
            PositionThreeGuardDownImage = reader.ReadElementString("PositionThreeImage");
            reader.ReadEndElement();

            DefaultPosition = (ThreeWayToggleSwitchPosition)Enum.Parse(typeof(ThreeWayToggleSwitchPosition), reader.ReadElementString("DefaultPosition"));
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
                    if (newPosition > 0 && newPosition < 4)
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
