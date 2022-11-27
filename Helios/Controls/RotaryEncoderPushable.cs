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
    using System.Globalization;
    using System.Windows;
    using System.Xml;

    // [HeliosControl("Helios.Base.RotaryEncoderPushable", "Pushable Encoder - Knob 1", "Rotary Encoders", typeof(RotaryRenderer))]
    public class RotaryEncoderPushable : Rotary { 
        private double _stepValue = 0.1d;
        private double _initialRotation = 0d;
        private double _rotationStep = 5d;

        private HeliosTrigger _incrementTrigger;
        private HeliosTrigger _decrementTrigger;
        private bool _pushed;
        private HeliosValue _pushedValue;
        private HeliosAction _pushAction;
        private HeliosAction _releaseAction;

        private HeliosTrigger _pushedTrigger;
        private HeliosTrigger _releasedTrigger;

        public RotaryEncoderPushable()
            : base("Pushable Rotary Encoder", new Size(100, 100))
        {
            KnobImage = "{Helios}/Images/Knobs/knob1.png";

            _incrementTrigger = new HeliosTrigger(this, "", "encoder", "incremented", "Triggered when encoder is incremented.", "Current encoder value", BindingValueUnits.Numeric);
            Triggers.Add(_incrementTrigger);
            _decrementTrigger = new HeliosTrigger(this, "", "encoder", "decremented", "Triggered when encoder is decremented.", "Current encoder value", BindingValueUnits.Numeric);
            Triggers.Add(_decrementTrigger);

            _pushedTrigger = new HeliosTrigger(this, "", "", "pushed", "Fired when this button is pushed.", "Always returns true.", BindingValueUnits.Boolean);
            _releasedTrigger = new HeliosTrigger(this, "", "", "released", "Fired when this button is released.", "Always returns false.", BindingValueUnits.Boolean);
            Triggers.Add(_pushedTrigger);
            Triggers.Add(_releasedTrigger);

            _pushAction = new HeliosAction(this, "", "", "push", "Simulate physically pushing this button.");
            _pushAction.Execute += new HeliosActionHandler(Push_ExecuteAction);
            _releaseAction = new HeliosAction(this, "", "", "release", "Simulate physically removing pressure from this button.");
            _releaseAction.Execute += new HeliosActionHandler(Release_ExecuteAction);
            Actions.Add(_pushAction);
            Actions.Add(_releaseAction);

            _pushedValue = new HeliosValue(this, new BindingValue(false), "", "physical state", "Current state of this button.", "True if the button is currently pushed (either via pressure or toggle), otherwise false.  Setting this value will not fire pushed/released triggers, but will fire on/off triggers.  Directly setting this state to on for a momentary buttons will not auto release, the state must be manually reset to false.", BindingValueUnits.Boolean);
            _pushedValue.Execute += new HeliosActionHandler(PushedValue_Execute);
            Values.Add(_pushedValue);
            Actions.Add(_pushedValue);
        }

        #region Properties

        public double StepValue
        {
            get
            {
                return _stepValue;
            }
            set
            {
                if (!_stepValue.Equals(value))
                {
                    double oldValue = _stepValue;
                    _stepValue = value;
                    OnPropertyChanged("StepValue", oldValue, value, true);
                }
            }
        }

        public double RotationStep
        {
            get
            {
                return _rotationStep;
            }
            set
            {
                if (!_rotationStep.Equals(value))
                {
                    double oldValue = _rotationStep;
                    _rotationStep = value;
                    OnPropertyChanged("RotationStep", oldValue, value, true);
                }
            }
        }

        public double InitialRotation
        {
            get
            {
                return _initialRotation;
            }
            set
            {
                if (!_initialRotation.Equals(value))
                {
                    double oldValue = _initialRotation;
                    _initialRotation = value;
                    OnPropertyChanged("InitialRotation", oldValue, value, true);
                    KnobRotation = _initialRotation;
                }
            }
        }

        #endregion

        // XXX refactor this class to RotaryEncoderBase
        // XXX attach the editors to the base class
        // XXX make this work
        // XXX create additional editor for additional functionality

        private double _lastPulse;

        private void Increment()
        {
            KnobRotation += _rotationStep;
            _lastPulse += _rotationStep;
            if (!BypassTriggers)
            {
                _incrementTrigger.FireTrigger(new BindingValue(StepValue));
            }
        }

        private void Decrement()
        {
            KnobRotation -= _rotationStep;
            _lastPulse -= _rotationStep;
            if (!BypassTriggers)
            {
                _decrementTrigger.FireTrigger(new BindingValue(-StepValue));
            }
        }

        #region IRotaryControl

        public override void Pulse(int pulses)
        {
            for (int i = pulses; i < 0; i++)
            {
                Decrement();
            }
            for (int i = 0; i < pulses; i++)
            {
                Increment();
            }
            OnDisplayUpdate();
        }

        public override void ChangeControlAngle(double controlAngleChange)
        {
            // move the control and generate pulses
            ControlAngle = KnobRotation + controlAngleChange;
        }

        public override double ControlAngle
        {
            set
            {
                // guard against infinite loops and degenerate cases
                if (_rotationStep <= 0)
                {
                    return;
                }

                // NOTE: one of these loops will have a false condition already and won't run
                while (_lastPulse >= value + _rotationStep)
                {
                    Decrement();
                }

                while (_lastPulse <= value - _rotationStep)
                {
                    Increment();
                }

                // adjust to final location because of remainder
                KnobRotation = value;
                OnDisplayUpdate();
            }
        }

        #endregion

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

        void Push_ExecuteAction(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);

            if (!BypassTriggers)
            {
                _pushedTrigger.FireTrigger(new BindingValue(true));
            }
                Pushed = true;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        void Release_ExecuteAction(object action, HeliosActionEventArgs e)
        {

            BeginTriggerBypass(e.BypassCascadingTriggers);

            if (!BypassTriggers)
            {
                _releasedTrigger.FireTrigger(new BindingValue(false));
            }

                Pushed = false;
 
            EndTriggerBypass(e.BypassCascadingTriggers);
        }



        public override void MouseDown(Point location)
        {
            if (!BypassTriggers)
            {
                _pushedTrigger.FireTrigger(new BindingValue(true));
            }

                     Pushed = true;
         }
        public override void MouseUp(Point location)
        {
            if (!BypassTriggers)
            {
                _releasedTrigger.FireTrigger(new BindingValue(false));
            }
            Pushed = false;
        }

        void PushedValue_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);

            Pushed = e.Value.BoolValue;

            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        public override void Reset()
        {
            BeginTriggerBypass(true);
            KnobRotation = InitialRotation;
            EndTriggerBypass(true);
        }


        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("KnobImage", KnobImage);
            writer.WriteElementString("RotationStep", RotationStep.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("StepValue", StepValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialRotation", InitialRotation.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("ClickType");
            writer.WriteElementString("Type", ClickType.ToString());
            if (ClickType != ClickType.Touch)
            {
                writer.WriteElementString("Sensitivity", Sensitivity.ToString(CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            KnobImage = reader.ReadElementString("KnobImage");
            RotationStep = double.Parse(reader.ReadElementString("RotationStep"), CultureInfo.InvariantCulture);
            StepValue = double.Parse(reader.ReadElementString("StepValue"), CultureInfo.InvariantCulture);
            InitialRotation = double.Parse(reader.ReadElementString("InitialRotation"), CultureInfo.InvariantCulture);

            if (reader.Name.Equals("ClickType"))
            {
                reader.ReadStartElement("ClickType");
                ClickType = (ClickType)Enum.Parse(typeof(ClickType), reader.ReadElementString("Type"));
                if (reader.Name == "Sensitivity")
                {
                    Sensitivity = double.Parse(reader.ReadElementString("Sensitivity"), CultureInfo.InvariantCulture);
                }
                reader.ReadEndElement();
            }
            else
            {
                ClickType = Controls.ClickType.Swipe;
                Sensitivity = 0d;
            }

            Reset();
        }
    }
}
