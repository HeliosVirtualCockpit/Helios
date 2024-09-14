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
    using System.ComponentModel;
    using System.Globalization;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

    [HeliosControl("Helios.Base.PotentiometerClickable", "Clickable Potentiometer - Knob 1", "Potentiometers", typeof(RotaryKnobRenderer))]
    public class PotentiometerClickable : PotentiometerKnob, IConfigurableImageLocation, IRefreshableImage
    {
        private Rect _centreZone;
        private PushButtonType _buttonType;
        private string _pushedImageFile = "{Helios}/Images/Knobs/knob6.png";
        private string _knobImageFile = "{Helios}/Images/Knobs/knob1.png";

        private bool _pushed;
        private bool _closed;

        private HeliosAction _pushAction;
        private HeliosAction _releaseAction;

        private HeliosTrigger _openTrigger;
        private HeliosTrigger _closedTrigger;
        private HeliosTrigger _pushedTrigger;
        private HeliosTrigger _releasedTrigger;

        private HeliosValue _value;
        private HeliosValue _pushedValue;
        public PotentiometerClickable() : base("Clickable Potentiometer")
        {
            _centreZone = new Rect(Left + Width / 3, Top + Height / 3, Width / 3, Height / 3);
            _buttonType = PushButtonType.Toggle;
            _knobImageFile = KnobImage;
            _pushedTrigger = new HeliosTrigger(this, "", "", "button pushed", "Fired when this button is pushed.", "Always returns true.", BindingValueUnits.Boolean);
            _releasedTrigger = new HeliosTrigger(this, "", "", "button released", "Fired when this button is released.", "Always returns false.", BindingValueUnits.Boolean);
            _closedTrigger = new HeliosTrigger(this, "", "", "button closed", "Fired when this button is in the closed state.", "Always returns true.", BindingValueUnits.Boolean);
            _openTrigger = new HeliosTrigger(this, "", "", "button open", "Fired when this button is in the open state.", "Always returns false.", BindingValueUnits.Boolean);
            Triggers.Add(_pushedTrigger);
            Triggers.Add(_releasedTrigger);
            Triggers.Add(_closedTrigger);
            Triggers.Add(_openTrigger);

            _pushAction = new HeliosAction(this, "", "", "push button", "Simulate physically pushing this button.");
            _pushAction.Execute += new HeliosActionHandler(Push_ExecuteAction);
            _releaseAction = new HeliosAction(this, "", "", "release button", "Simulate physically removing pressure from this button.");
            _releaseAction.Execute += new HeliosActionHandler(Release_ExecuteAction);
            Actions.Add(_pushAction);
            Actions.Add(_releaseAction);

            _pushedValue = new HeliosValue(this, new BindingValue(false), "", "physical state", "Current state of this button.", "True if the button is currently pushed (either via pressure or toggle), otherwise false.  Setting this value will not fire pushed/released triggers, but will fire on/off triggers.  Directly setting this state to on for a momentary buttons will not auto release, the state must be manually reset to false.", BindingValueUnits.Boolean);
            _pushedValue.Execute += new HeliosActionHandler(PushedValue_Execute);
            Values.Add(_pushedValue);
            Actions.Add(_pushedValue);

            _value = new HeliosValue(this, new BindingValue(false), "", "circuit state", "Current open/closed state of this buttons circuit.", "True if the button is currently closed (on), otherwise false.", BindingValueUnits.Boolean);
            Values.Add(_value);
        }
        public PushButtonType ButtonType
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

        public string PushedImage
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
                    OnPropertyChanged("PushedImage", oldValue, value, true);
                    Refresh();
                }
            }
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

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if(args.PropertyName == "Pushed")
            {
                if (Pushed)
                {
                    _knobImageFile = KnobImage;
                    KnobImage = _pushedImageFile;
                } else
                {
                    KnobImage = _knobImageFile;
                }
            }
            OnDisplayUpdate();
            base.OnPropertyChanged(args);
        }

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            Pushed = false;
            IsClosed = false;
            EndTriggerBypass(true);
        }
        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public new void ReplaceImageNames(string oldName, string newName)
        {
            PushedImage = string.IsNullOrEmpty(PushedImage) ? PushedImage : string.IsNullOrEmpty(oldName) ? newName + PushedImage : PushedImage.Replace(oldName, newName);
            KnobImage = string.IsNullOrEmpty(KnobImage) ? KnobImage : string.IsNullOrEmpty(oldName) ? newName + KnobImage : KnobImage.Replace(oldName, newName);
            _knobImageFile = string.IsNullOrEmpty(_knobImageFile) ? _knobImageFile : string.IsNullOrEmpty(oldName) ? newName + _knobImageFile : _knobImageFile.Replace(oldName, newName);
        }

        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((PushedImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (KnobImage ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
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
            } else
            {
                base.MouseDown(location);
            }

        }
        public override void MouseUp(Point location)
        {
            if (_centreZone.Contains(location))
            {

                if (ButtonType == PushButtonType.Momentary)
                {
                    Pushed = false;
                    IsClosed = false;
                }
            } else
            {
                base.MouseUp(location);
            }
        }
        public override void ReadXml(XmlReader reader)
        {
            ButtonType = (PushButtonType)Enum.Parse(typeof(PushButtonType), reader.ReadElementString("ButtonClickType"));
            PushedImage = reader.ReadElementString("PushedImage");
            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("ButtonClickType", ButtonType.ToString());
            writer.WriteElementString("PushedImage", PushedImage);
            base.WriteXml(writer);
        }


    }
}
