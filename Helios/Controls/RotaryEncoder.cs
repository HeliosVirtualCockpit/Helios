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

    [HeliosControl("Helios.Base.RotaryEncoder", "Encoder - Knob 1", "Rotary Encoders", typeof(RotaryKnobRenderer))]
    public class RotaryEncoder : RotaryKnob
    {
        private double _stepValue = 0.1d;
        private double _initialRotation;
        private double _rotationStep = 5d;
        private HeliosValue _heliosValue;

        /// <summary>
        /// the rotation value where we last generated a pulse
        /// </summary>
        private double _lastPulse;

        private readonly HeliosTrigger _incrementTrigger;
        private readonly HeliosTrigger _decrementTrigger;

        public RotaryEncoder()
            : base("Rotary Encoder", new Size(100, 100))
        {
            KnobImage = "{Helios}/Images/Knobs/knob1.png";

            _incrementTrigger = new HeliosTrigger(this, "", "encoder", "incremented", "Triggered when encoder is incremented.", "Encoder step value", BindingValueUnits.Numeric);
            Triggers.Add(_incrementTrigger);
            _decrementTrigger = new HeliosTrigger(this, "", "encoder", "decremented", "Triggered when encoder is decremented.", "Encoder step value (negative)", BindingValueUnits.Numeric);
            Triggers.Add(_decrementTrigger);

            _heliosValue = new HeliosValue(this, new BindingValue(0d), "", "value", "Current value of the rotary encoder.", "", BindingValueUnits.Numeric);
            _heliosValue.Execute += new HeliosActionHandler(SetValue_Execute);
            Values.Add(_heliosValue);
            Actions.Add(_heliosValue);
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
        #region Actions
        void SetValue_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                // NOTE: don't create a Helios object property event
                _heliosValue.SetValue(e.Value, e.BypassCascadingTriggers);
                SetRotation();
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

#endregion

        private void SetRotation()
        {
            KnobRotation = _heliosValue.Value.DoubleValue * 360d;
        }
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

        #region IPulsedControl

        public override void Pulse(int pulses)
        {
            // NOTE: one of these loops will have a false condition already and won't run
            for (int i = pulses; i < 0; i++)
            {
                Decrement();
            }

            // NOTE: one of these loops will have a false condition already and won't run
            for (int i = 0; i < pulses; i++)
            {
                Increment();
            }

            OnDisplayUpdate();
        }

        #endregion

        #region IRotaryControl

        public override double ControlAngle
        {
            get => KnobRotation;

            // translate any rotation into the appropriate pulses, taking care to have "remainder" of rotation available
            // for next pulse
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

                // NOTE: one of these loops will have a false condition already and won't run
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

        public override void Reset()
        {
            base.Reset();
            ResetRotation();
        }

        private void ResetRotation()
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
            WriteOptionalXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            KnobImage = reader.ReadElementString("KnobImage");
            RotationStep = double.Parse(reader.ReadElementString("RotationStep"), CultureInfo.InvariantCulture);
            StepValue = double.Parse(reader.ReadElementString("StepValue"), CultureInfo.InvariantCulture);
            InitialRotation = double.Parse(reader.ReadElementString("InitialRotation"), CultureInfo.InvariantCulture);
            ReadOptionalXml(reader);
            ResetRotation();
        }
    }
}
