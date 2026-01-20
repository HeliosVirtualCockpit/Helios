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

using System.Windows.Controls;

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Xml;

    [HeliosControl("Helios.Base.RotaryEncoderClickable", "Rotary Encoder Clickable - Knob 6", "Rotary Encoders", typeof(RotaryKnobRenderer))]
    public class RotaryEncoderClickable : RotaryEncoder, IConfigurableImageLocation, IRefreshableImage
    {
        private Rect _centreZone;
        private PushButtonType _buttonType;
        private ClickControlType _clickControlType = ClickControlType.PushButton;
        private ToggleSwitchLockPosition _lockPosition = ToggleSwitchLockPosition.None;
        private bool _isSwitch = false;

        private string _pushedImageFile = "{Helios}/Images/Knobs/knob7.png";
        private string _unpushedImageFile = "{Helios}/Images/Knobs/knob6.png";

        private bool _pushed;
        private bool _closed;

        private HeliosAction _pushAction;
        private HeliosAction _releaseAction;

        private HeliosTrigger _openTrigger;
        private HeliosTrigger _closedTrigger;
        private HeliosTrigger _pushedTrigger;
        private HeliosTrigger _releasedTrigger;

        private ToggleSwitchPosition _position = ToggleSwitchPosition.Two;

        private HeliosValue _positionValue;
        private HeliosTrigger _positionOneEnterAction;
        private HeliosTrigger _positionOneExitAction;
        private HeliosTrigger _positionTwoEnterAction;
        private HeliosTrigger _positionTwoExitAction;

        private HeliosValue _value;
        private HeliosValue _pushedValue;
        public RotaryEncoderClickable() : this("Clickable Rotary Encoder"){ }
        public RotaryEncoderClickable(string name) : base(name)
        {
            _centreZone = new Rect(Left + Width / 3, Top + Height / 3, Width / 3, Height / 3);
            _buttonType = PushButtonType.Toggle;
            _unpushedImageFile = KnobImage;
            _pushedTrigger = new HeliosTrigger(this, "", "", "button pushed", "Fired when this button is pushed.", "Always returns true.", BindingValueUnits.Boolean);
            _releasedTrigger = new HeliosTrigger(this, "", "", "button released", "Fired when this button is released.", "Always returns false.", BindingValueUnits.Boolean);
            _closedTrigger = new HeliosTrigger(this, "", "", "button closed", "Fired when this button is in the closed state.", "Always returns true.", BindingValueUnits.Boolean);
            _openTrigger = new HeliosTrigger(this, "", "", "button open", "Fired when this button is in the open state.", "Always returns false.", BindingValueUnits.Boolean);

            _pushAction = new HeliosAction(this, "", "", "push button", "Simulate physically pushing this button.");
            _pushAction.Execute += new HeliosActionHandler(Push_ExecuteAction);
            _releaseAction = new HeliosAction(this, "", "", "release button", "Simulate physically removing pressure from this button.");
            _releaseAction.Execute += new HeliosActionHandler(Release_ExecuteAction);

            _pushedValue = new HeliosValue(this, new BindingValue(false), "", "physical state", "Current state of this button.", "True if the button is currently pushed (either via pressure or toggle), otherwise false.  Setting this value will not fire pushed/released triggers, but will fire on/off triggers.  Directly setting this state to on for a momentary buttons will not auto release, the state must be manually reset to false.", BindingValueUnits.Boolean);
            _pushedValue.Execute += new HeliosActionHandler(PushedValue_Execute);

            _value = new HeliosValue(this, new BindingValue(false), "", "circuit state", "Current open/closed state of this buttons circuit.", "True if the button is currently closed (on), otherwise false.", BindingValueUnits.Boolean);

            _positionOneEnterAction = new HeliosTrigger(this, "", "position one", "entered", "Triggered when position one is entered or depressed.");
            _positionOneExitAction = new HeliosTrigger(this, "", "position one", "exited", "Triggered when position one is exited or released.");
            _positionTwoEnterAction = new HeliosTrigger(this, "", "position two", "entered", "Triggered when position two is entered or depressed.");
            _positionTwoExitAction = new HeliosTrigger(this, "", "position two", "exited", "Triggered when position two is exited or released.");

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the switch.", "Position number 1 or 2.  Positions are numbered from top to bottom.", BindingValueUnits.Numeric);
            _positionValue.Execute += new HeliosActionHandler(SetPositionAction_Execute);

            AddPushButtonActionsTriggers();

        }
        #region properties
        public override PushButtonType ButtonType
        {
            get
            {
                return _buttonType;
            }
            set
            {
                if (!_buttonType.Equals(value))
                {
                    PushButtonType oldValue = _buttonType;
                    _buttonType = value;
                    OnPropertyChanged("ButtonType", oldValue, value, true);
                }
            }
        }
        public ClickControlType ClickControlType
        {
            get
            {
                return _clickControlType;
            }
            set
            {
                if (!_clickControlType.Equals(value))
                {
                    ClickControlType oldValue = _clickControlType;
                    _clickControlType = value;
                    OnPropertyChanged("ClickControlType", oldValue, value, true);
                    _isSwitch = _clickControlType == ClickControlType.Switch;
                    OnPropertyChanged("IsSwitch", !_isSwitch, _isSwitch, true);

                }
            }
        }

        public virtual string PushedImage
        {
            get
            {
                return _pushedImageFile;
            }
            set
            {
                if ((_pushedImageFile == null && value != null)
                    || (_pushedImageFile != null && !_pushedImageFile.Equals(value)))
                {
                    string oldValue = _pushedImageFile;
                    _pushedImageFile = value;
                    if (Pushed && !On)
                    {
                        KnobImage = value;
                    }
                    OnPropertyChanged("PushedImage", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public virtual string UnpushedImage
        {
            get
            {
                return _unpushedImageFile;
            }
            set
            {
                if ((_unpushedImageFile == null && value != null)
                    || (_unpushedImageFile != null && !_unpushedImageFile.Equals(value)))
                {
                    string oldValue = _unpushedImageFile;
                    _unpushedImageFile = value;
                    if (!Pushed && !On)
                    {
                        KnobImage = value;
                    }
                    OnPropertyChanged("UnpushedImage", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public virtual string IndicatorOnNormalImage
        {
            get => "";
            set { }
        }
        public virtual string IndicatorOnClickedImage
        {
            get => "";
            set { }
        }

        public bool Pushed
        {
            get
            {
                return _pushed;
            }
            set
            {
                if (!_pushed.Equals(value))
                {
                    _pushed = value;
                    _pushedValue.SetValue(new BindingValue(_pushed), BypassTriggers);

                    OnPropertyChanged("Pushed", !value, value, false);
                    OnDisplayUpdate();
                }
            }
        }
        public virtual bool On
        {
            get => false;
            set
            {
                // no code
            }
        }
        public bool IsClosed
        {
            get
            {
                return _closed;
            }
            set
            {
                if (!_closed.Equals(value))
                {
                    bool oldValue = _closed;

                    _closed = value;
                    _value.SetValue(new BindingValue(_pushed), BypassTriggers);
                    if (!BypassTriggers)
                    {
                        if (_closed)
                        {
                            _closedTrigger.FireTrigger(_value.Value);
                        }
                        else
                        {
                            _openTrigger.FireTrigger(_value.Value);
                        }
                    }
                    OnPropertyChanged("IsClosed", oldValue, value, false);
                }
            }
        }
        public override bool ClickConfigurable
        {
            get => true;
        }
        public bool IsSwitch
        {
            get => _isSwitch;
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
        public ToggleSwitchLockPosition LockPosition
        {
            get => _lockPosition;
            set
            {
                if (!_lockPosition.Equals(value))
                {
                    ToggleSwitchLockPosition oldValue = _lockPosition;
                    _lockPosition = value;
                    OnPropertyChanged("LockPosition", oldValue, value, false);
                }
            }
        }
        #endregion
        #region Actions
        void PushedValue_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);

            Pushed = e.Value.BoolValue;
            IsClosed = Pushed;

            EndTriggerBypass(e.BypassCascadingTriggers);
        }
        void Push_ExecuteAction(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);

            if (!BypassTriggers)
            {
                _pushedTrigger.FireTrigger(new BindingValue(true));
            }

            if (ButtonType == PushButtonType.Momentary)
            {
                Pushed = true;
                IsClosed = true;
            }
            else
            {
                Pushed = !Pushed;
                IsClosed = Pushed;
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }
        void Release_ExecuteAction(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            if (!BypassTriggers)
            {
                _releasedTrigger.FireTrigger(new BindingValue(false));
            }

            if (ButtonType == PushButtonType.Momentary)
            {
                Pushed = false;
                IsClosed = false;
            }

            EndTriggerBypass(e.BypassCascadingTriggers);
        }

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

        #endregion

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "Pushed":
                    KnobImage = Pushed ? PushedImage : UnpushedImage;
                    OnDisplayUpdate();
                    Refresh();
                    break;
                case "SwitchPosition":
                    KnobImage = _position == ToggleSwitchPosition.Two ? PushedImage : UnpushedImage;
                    OnDisplayUpdate();
                    Refresh();
                    break;
                case "ClickControlType":
                    if((ClickControlType) args.NewValue == ClickControlType.Switch)
                    {
                        AddSwitchActionsTriggers();
                        RemovePushButtonActionsTriggers();
                    } else
                    {
                        AddPushButtonActionsTriggers();
                        RemoveSwitchActionsTriggers();
                    }
                        break;
                default:
                    base.OnPropertyChanged(args);
                    break;
            }
        }

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            Pushed = false;
            IsClosed = false;
            EndTriggerBypass(true);
        }

        public override void ScaleChildren(double scaleX, double scaleY)
        {
            base.ScaleChildren(scaleX, scaleY);
            _centreZone = new Rect(Width / 3, Height / 3, Width / 3, Height / 3);
        }

        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public new void ReplaceImageNames(string oldName, string newName)
        {
            base.ReplaceImageNames(oldName,newName);
            PushedImage = string.IsNullOrEmpty(PushedImage) ? PushedImage : string.IsNullOrEmpty(oldName) ? newName + PushedImage : PushedImage.Replace(oldName, newName);
            UnpushedImage = string.IsNullOrEmpty(UnpushedImage) ? UnpushedImage : string.IsNullOrEmpty(oldName) ? newName + UnpushedImage : UnpushedImage.Replace(oldName, newName);
        }

        public override bool ConditionalImageRefresh(string imageName)
        {
            ImageRefresh = base.ConditionalImageRefresh(imageName);
            if ((PushedImage ?? "").ToLower().Replace("/", @"\") == imageName && PushedImage != KnobImage)
            {
                ImageRefresh = true;
                ReloadImage(imageName);
            }
            if ((UnpushedImage ?? "").ToLower().Replace("/", @"\") == imageName && UnpushedImage != KnobImage)
            {
                ImageRefresh = true;
                ReloadImage(imageName);
            }
            return ImageRefresh;
        }
        public override void MouseDown(Point location)
        {
            if (_centreZone.Contains(location))
            {
                if (!BypassTriggers)
                {
                    _pushedTrigger.FireTrigger(new BindingValue(true));
                }

                if (_clickControlType == ClickControlType.Switch)
                {
                    SwitchPosition = _position == ToggleSwitchPosition.One ? ToggleSwitchPosition.Two : ToggleSwitchPosition.One;
                }
                else
                {
                    switch (ButtonType)
                    {
                        case PushButtonType.Momentary:
                            Pushed = true;
                            IsClosed = true;
                            break;

                        case PushButtonType.Toggle:
                            Pushed = !Pushed;
                            IsClosed = Pushed;
                            break;
                    }
                }
            }
            else
            {
                if (AllowMouseActivity) { base.MouseDown(location); }
            }

        }
        public override void MouseUp(Point location)
        {
            if (_centreZone.Contains(location))
            {

                if (ButtonType == PushButtonType.Momentary)
                {
                    if (!BypassTriggers)
                    {
                        _releasedTrigger.FireTrigger(new BindingValue(false));
                    }
                    Pushed = false;
                    IsClosed = false;
                }
            } else
            {
                if (AllowMouseActivity) { base.MouseUp(location); }
            }
        }
        public override void MouseWheel(int delta)
        {
            if (_clickControlType == ClickControlType.PushButton && AllowMouseActivity) { base.MouseWheel(delta); }
            if (_clickControlType == ClickControlType.Switch && (int)_lockPosition != (int)_position)
            {
                base.MouseWheel(delta);
            }
        }

        public override void MouseDrag(Point location)
        {
            if (_clickControlType == ClickControlType.PushButton && AllowMouseActivity) base.MouseDrag(location);
            if (_clickControlType == ClickControlType.Switch && (int)_lockPosition != (int)_position)
            {
                base.MouseDrag(location);
            }
        }
        private void AddPushButtonActionsTriggers()
        {
            Triggers.Add(_pushedTrigger);
            Triggers.Add(_releasedTrigger);
            Triggers.Add(_closedTrigger);
            Triggers.Add(_openTrigger);
            Actions.Add(_pushAction);
            Actions.Add(_releaseAction);
            Values.Add(_pushedValue);
            Actions.Add(_pushedValue);
            Values.Add(_value);
        }
        private void AddSwitchActionsTriggers()
        {
            Triggers.Add(_positionOneEnterAction);
            Triggers.Add(_positionOneExitAction);
            Triggers.Add(_positionTwoEnterAction);
            Triggers.Add(_positionTwoExitAction);
            Values.Add(_positionValue);
            Actions.Add(_positionValue);
            Triggers.Add(_positionValue);
        }
        private void RemovePushButtonActionsTriggers()
        {
            Triggers.Remove(_pushedTrigger);
            Triggers.Remove(_releasedTrigger);
            Triggers.Remove(_closedTrigger);
            Triggers.Remove(_openTrigger);
            Actions.Remove(_pushAction);
            Actions.Remove(_releaseAction);
            Values.Remove(_pushedValue);
            Actions.Remove(_pushedValue);
            Values.Remove(_value);
        }
        private void RemoveSwitchActionsTriggers()
        {
            Triggers.Remove(_positionOneEnterAction);
            Triggers.Remove(_positionOneExitAction);
            Triggers.Remove(_positionTwoEnterAction);
            Triggers.Remove(_positionTwoExitAction);
            Values.Remove(_positionValue);
            Actions.Remove(_positionValue);
            Triggers.Remove(_positionValue);
        }
        private bool AllowMouseActivity
        {
            get => AllowRotation == RotaryClickAllowRotationType.Both ||
                    (!Pushed && AllowRotation == RotaryClickAllowRotationType.Unclicked) ||
                    (Pushed && AllowRotation == RotaryClickAllowRotationType.Clicked);

        }
        public override void ReadXml(XmlReader reader)
        {
            ButtonType = (PushButtonType)Enum.Parse(typeof(PushButtonType), reader.ReadElementString("ButtonClickType"));
            if (reader.Name.Equals("ClickControlType"))
            {
                ClickControlType = (ClickControlType)Enum.Parse(typeof(ClickControlType), reader.ReadElementString("ClickControlType"));
            } else
            {
                ClickControlType = ClickControlType.PushButton;
            }
            PushedImage = reader.ReadElementString("PushedImage");
            UnpushedImage = KnobImage = reader.ReadElementString("UnpushedImage");
            if (reader.Name.Equals("AllowRotation")) AllowRotation = (RotaryClickAllowRotationType)Enum.Parse(typeof(RotaryClickAllowRotationType), reader.ReadElementString("AllowRotation"));
            LockPosition = reader.Name.Equals("SwitchLockPosition") ? (ToggleSwitchLockPosition)Enum.Parse(typeof(ToggleSwitchLockPosition), reader.ReadElementString("SwitchLockPosition")) : ToggleSwitchLockPosition.None;
            base.ReadXml(reader);
            _centreZone = new Rect(Width / 3, Height / 3, Width / 3, Height / 3);
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter allowRotationConverter = TypeDescriptor.GetConverter(typeof(RotaryClickAllowRotationType));
            TypeConverter clickControlConverter = TypeDescriptor.GetConverter(typeof(ClickControlType));
            TypeConverter SwitchLockPositionConverter = TypeDescriptor.GetConverter(typeof(ToggleSwitchLockPosition));

            writer.WriteElementString("ButtonClickType", ButtonType.ToString());
            if(_clickControlType != ClickControlType.PushButton) writer.WriteElementString("ClickControlType", clickControlConverter.ConvertToInvariantString(ClickControlType));
            writer.WriteElementString("PushedImage", PushedImage);
            writer.WriteElementString("UnpushedImage", UnpushedImage);
            if (AllowRotation != RotaryClickAllowRotationType.Both) writer.WriteElementString("AllowRotation", allowRotationConverter.ConvertToInvariantString(AllowRotation));
            if (LockPosition != ToggleSwitchLockPosition.None) writer.WriteElementString("SwitchLockPosition", SwitchLockPositionConverter.ConvertToInvariantString(LockPosition));
            KnobImage = UnpushedImage;
            base.WriteXml(writer);
        }
    }
}
