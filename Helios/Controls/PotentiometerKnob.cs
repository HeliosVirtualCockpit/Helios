// Copyright 2014 Craig Courtney
// Copyright 2020 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml;

namespace GadrocsWorkshop.Helios.Controls
{
    public class PotentiometerKnob : RotaryKnob
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private double _initialValue = 0.0d;
        private double _stepValue = 0.1d;
        private double _minValue = 0d;
        private double _maxValue = 1d;

        private double _initialRotation = 225d;
        private double _rotationTravel = 270d;

        private HeliosValue _heliosValue;
        private HeliosTrigger _releasedTrigger;

        private bool _isContinuous;
        private bool _continuousConfigurable = false;

        public PotentiometerKnob(string name)
            : base(name, new Size(100, 100))
        {
            KnobImage = "{Helios}/Images/Knobs/knob1.png";

            _releasedTrigger = new HeliosTrigger(this, "", "", "released", "Fires when the user's pressure on the control is released");
            this.Triggers.Add(_releasedTrigger);
            _heliosValue = new HeliosValue(this, new BindingValue(0d), "", "value", "Current value of the potentiometer.", "", BindingValueUnits.Numeric);
            _heliosValue.Execute += new HeliosActionHandler(SetValue_Execute);
            Values.Add(_heliosValue);
            Actions.Add(_heliosValue);
            Triggers.Add(_heliosValue);
        }

        #region Properties

        public bool IsContinuous
        {
            get
            {
                return _isContinuous;
            }
            set
            {
                if (!_isContinuous.Equals(value))
                {
                    bool oldValue = _isContinuous;
                    _isContinuous = value;
                    OnPropertyChanged("IsContinuous", oldValue, value, true);
                    if (value)
                    {
                        // implementation limitation
                        RotationTravel = 360d;
                    }
                    Refresh();
                }
            }
        }

        public bool ContinuousConfigurable { get => _continuousConfigurable;
            set => _continuousConfigurable = value; 
        }

        public double InitialValue
        {
            get
            {
                return _initialValue;
            }
            set
            {
                if (!_initialValue.Equals(value))
                {
                    double oldValue = _initialValue;
                    _initialValue = value;
                    OnPropertyChanged("InitialValue", oldValue, value, true);
                }
            }
        }

        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                if (!_minValue.Equals(value))
                {
                    double oldValue = _minValue;
                    _minValue = value;
                    OnPropertyChanged("MinValue", oldValue, value, true);
                    SetRotation();
                }
            }
        }

        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (!_maxValue.Equals(value))
                {
                    double oldValue = _maxValue;
                    _maxValue = value;
                    OnPropertyChanged("MaxValue", oldValue, value, true);
                    SetRotation();
                }
            }
        }

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

        /// <summary>
        /// UI access to current value, backed by Helios value of the potentiometer
        ///
        /// writes to this property do not create Undo events
        /// </summary>
        public double Value
        {
            get
            {
                return _heliosValue.Value.DoubleValue;
            }
            set
            {
                if (_heliosValue.Value.DoubleValue.Equals(value))
                {
                    return;
                }

                double oldValue = _heliosValue.Value.DoubleValue;
                if (IsContinuous)
                {
                    if (value < MinValue)
                    {
                        value += MaxValue - MinValue;
                    }
                    else if (value > MaxValue)
                    {
                        value -= MaxValue - MinValue;
                    }
                }
                value = Math.Round(value, 5);
                _heliosValue.SetValue(new BindingValue(value), BypassTriggers);
                OnPropertyChanged("Value", oldValue, value, false);
                SetRotation();
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
                    SetRotation();
                }
            }
        }

        public double RotationTravel
        {
            get
            {
                return _rotationTravel;
            }
            set
            {
                if (!_rotationTravel.Equals(value))
                {
                    double oldValue = _rotationTravel;
                    _rotationTravel = value;
                    OnPropertyChanged("RotationTravel", oldValue, value, true);
                    SetRotation();
                }
            }
        }

        internal double MaxRotation => InitialRotation + RotationTravel;

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
            KnobRotation = ControlAngle;
        }

        #region IPulsedControl

        public override void Pulse(int pulses)
        {
            // we don't have any special implementation for pulses, so just convert to a change in value
            if (IsContinuous)
            {
                // unbounded
                Value += _stepValue * pulses;
            }
            else
            {
                // clamp
                Value = Math.Max(Math.Min(Value + _stepValue * pulses, MaxValue), MinValue);
            }
        }

        #endregion

        #region IRotaryControl

        public override double ControlAngle
        {
            get => InitialRotation + (((_heliosValue.Value.DoubleValue - MinValue) / ValueSpan) * RotationTravel);
            set
            {
                // division by zero guard
                if (_maxValue <= _minValue)
                {
                    return;
                }
                
                // check for unsupported configuration that UI should have prevented
                // ReSharper disable once CompareOfFloatsByEqualityOperator checking for default value
                if (IsContinuous && RotationTravel != 360d)
                {
                    Logger.Warn("Potentiometer-style control {Name} is configured to be continuous but has a other than one full revolution; this is unsupported", Name);
                    return;
                }

                // calculate what angle we are actually allowed to set
                double effectiveAngle = value;
                if (IsContinuous)
                {
                    effectiveAngle = value;
                }
                else
                {
                    if (value < InitialRotation)
                    {
                        effectiveAngle = InitialRotation;
                    }
                    else if (value > InitialRotation + RotationTravel)
                    {
                        effectiveAngle = InitialRotation + RotationTravel;
                    }
                }

                // calculate value based on where we had to set the control angle
                double valueScale = (effectiveAngle - InitialRotation) / RotationTravel;
                Value = ValueSpan * valueScale + _minValue;
            }
        }

        private double ValueSpan => (MaxValue - MinValue);

        #endregion

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            Value = InitialValue;
            EndTriggerBypass(true);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("KnobImage", KnobImage);
            writer.WriteElementString("InitialValue", InitialValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("StepValue", StepValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MaxValue", MaxValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MinValue", MinValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialRotation", InitialRotation.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("RotationTravel", RotationTravel.ToString(CultureInfo.InvariantCulture));
            if (IsContinuous)
            {
                writer.WriteElementString("Continuous", true.ToString(CultureInfo.InvariantCulture));
            }
            WriteOptionalXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            KnobImage = reader.ReadElementString("KnobImage");
            InitialValue = double.Parse(reader.ReadElementString("InitialValue"), CultureInfo.InvariantCulture);
            StepValue = double.Parse(reader.ReadElementString("StepValue"), CultureInfo.InvariantCulture);
            MaxValue = double.Parse(reader.ReadElementString("MaxValue"), CultureInfo.InvariantCulture);
            MinValue = double.Parse(reader.ReadElementString("MinValue"), CultureInfo.InvariantCulture);
            InitialRotation = double.Parse(reader.ReadElementString("InitialRotation"), CultureInfo.InvariantCulture);
            RotationTravel = double.Parse(reader.ReadElementString("RotationTravel"), CultureInfo.InvariantCulture);
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (reader.Name.Equals("Continuous"))
            {
                IsContinuous = (bool)bc.ConvertFromInvariantString(reader.ReadElementString());
            }
            ReadOptionalXml(reader);

            BeginTriggerBypass(true);
            Value = InitialValue;
            SetRotation();
            EndTriggerBypass(true);
        }
        public override void MouseUp(Point location)
        {
            _releasedTrigger.FireTrigger(BindingValue.Empty);
            base.MouseUp(location);
        }

    }
}