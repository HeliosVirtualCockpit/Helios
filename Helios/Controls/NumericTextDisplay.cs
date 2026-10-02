// Copyright 2021 Helios Contributors
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
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Windows.Media;
using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls
{
    [HeliosControl(TYPE_ID, "Numeric Text Display", "Text Displays", typeof(TextDisplayRenderer))]
    public class NumericTextDisplay : TextDisplayRect
    {
        public const string TYPE_ID = "Helios.Base.NumericTextDisplay";
        protected const int DEFAULT_PRECISION = 0;

        private HeliosValue _value;
        private HeliosValue _brightnessValue;
        private HeliosAction _incrementBrightnessAction;
        private HeliosAction _decrementBrightnessAction;
        private double _displayBrightness = 10;

        private HeliosValue _opacityValue;
        private HeliosAction _incrementOpacityAction;
        private HeliosAction _decrementOpacityAction;
        private HeliosTrigger _incrementOpacityTrigger;
        private HeliosTrigger _decrementOpacityTrigger;

        private CalibrationPointCollectionDouble _calBrightness = new CalibrationPointCollectionDouble(0, 0, 1, 1) {
                new CalibrationPointDouble(0.25,0.5),
                new CalibrationPointDouble(0.5,0.75),
            };
        private CalibrationPointCollectionDouble _calOpacity = new CalibrationPointCollectionDouble(0, 0, 1, 255) { };

        /// <summary>
        /// backing field for property Unit, contains
        /// the binding value unit used for this numeric display, used to convert inputs
        /// </summary>
        private BindingValueUnit _unit = BindingValueUnits.Numeric;

        /// <summary>
        /// backing field for property Precision, contains
        /// number of digits after the decimal point to display, or 0
        /// </summary>
        private int _precision = DEFAULT_PRECISION;

        public NumericTextDisplay()
            : base("NumericTextDisplay", new System.Windows.Size(100, 25))
        {
            BuildValue();
        }

        /// <summary>
        /// rebuild the Helios value to use the configured unit type
        /// </summary>
        private void BuildValue()
        {
            HeliosValue oldValue = _value;
            if (oldValue != null)
            {
                Values.Remove(oldValue);
                Actions.Remove(oldValue);
                oldValue.Execute -= Value_Execute;
            }

            _value = new HeliosValue(this, new BindingValue(0.0), "", "Number", "The number to display",
                "The value to display.", Unit);
            _value.Execute += Value_Execute;
            Values.Add(_value);
            Actions.Add(_value);

            if (oldValue != null)
            {
                // update any bindings to it, since we cannot change the target of a binding
                List<HeliosBinding> redirect = InputBindings.Where(binding => ReferenceEquals(binding.Action, oldValue)).ToList();
                redirect.ForEach(binding =>
                {
                    binding.Action = _value;
                });
            }

            _brightnessValue = new HeliosValue(this, new BindingValue(false), "", "number display brightness value", "number", "0.0 to 1.0", BindingValueUnits.Numeric);
            _brightnessValue.Execute += new HeliosActionHandler(DisplayBrightness_Execute);
            Actions.Add(_brightnessValue);
            Values.Add(_brightnessValue);
            _incrementBrightnessAction = new HeliosAction(this, "", "number display brightness", "increment", "Increments the display brightness.");
            _incrementBrightnessAction.Execute += new HeliosActionHandler(IncrementBrightnessAction_Execute);
            Actions.Add(_incrementBrightnessAction);

            _decrementBrightnessAction = new HeliosAction(this, "", "number display brightness", "decrement", "decrements the display brightness.");
            _decrementBrightnessAction.Execute += new HeliosActionHandler(DecrementBrightnessAction_Execute);
            Actions.Add(_decrementBrightnessAction);

            _opacityValue = new HeliosValue(this, new BindingValue(false), "", "number display opacity value", "number", "0.0 to 1.0", BindingValueUnits.Numeric);
            _opacityValue.Execute += new HeliosActionHandler(DisplayOpacity_Execute);
            Actions.Add(_opacityValue);
            Values.Add(_opacityValue);

            _incrementOpacityAction = new HeliosAction(this, "", "number display opacity", "increment", "Increments the display opacity.");
            _incrementOpacityAction.Execute += new HeliosActionHandler(IncrementOpacityAction_Execute);
            Actions.Add(_incrementOpacityAction);

            _decrementOpacityAction = new HeliosAction(this, "", "number display opacity", "decrement", "decrements the display opacity.");
            _decrementOpacityAction.Execute += new HeliosActionHandler(DecrementOpacityAction_Execute);
            Actions.Add(_decrementOpacityAction);

            _incrementOpacityTrigger = new HeliosTrigger(this, "", "", "opacity maximum", "Fired when the opacity value has reached the maximum.", "returns true.", BindingValueUnits.Boolean);
            Triggers.Add(_incrementOpacityTrigger);
            _decrementOpacityTrigger = new HeliosTrigger(this, "", "", "opacity minimum", "Fired when the opacity value has reached the minimum.", "returns true.", BindingValueUnits.Boolean);
            Triggers.Add(_decrementOpacityTrigger);

        }

        #region Event Handlers

        private void Value_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            TextValue = e.Value.DoubleValue.ToString($"F{Math.Max(0, Precision)}", CultureInfo.InvariantCulture);
            EndTriggerBypass(e.BypassCascadingTriggers);
        }
        private void DisplayBrightness_Execute(object action, HeliosActionEventArgs e)
        {
            double brightness = _calBrightness.Interpolate(e.Value.DoubleValue);
            OnTextColor = Color.FromArgb(OnTextColor.A, Convert.ToByte((double)OnTextColorDefault.R * brightness), Convert.ToByte((double)OnTextColorDefault.G * brightness), Convert.ToByte((double)OnTextColorDefault.B * brightness));
        }
        private void IncrementBrightnessAction_Execute(object action, HeliosActionEventArgs e)
        {
            _displayBrightness = _displayBrightness >= 10 ? 10 : ++_displayBrightness;
            double brightness = _calBrightness.Interpolate(_displayBrightness / 10d);
            OnTextColor = Color.FromArgb(OnTextColor.A, Convert.ToByte((double)OnTextColorDefault.R * brightness), Convert.ToByte((double)OnTextColorDefault.G * brightness), Convert.ToByte((double)OnTextColorDefault.B * brightness));
        }
        private void DecrementBrightnessAction_Execute(object action, HeliosActionEventArgs e)
        {
            _displayBrightness = _displayBrightness <= 0 ? 0 : --_displayBrightness;
            double brightness = _calBrightness.Interpolate(_displayBrightness / 10d);
            OnTextColor = Color.FromArgb(OnTextColor.A, Convert.ToByte((double)OnTextColorDefault.R * brightness), Convert.ToByte((double)OnTextColorDefault.G * brightness), Convert.ToByte((double)OnTextColorDefault.B * brightness));
        }

        private void DisplayOpacity_Execute(object action, HeliosActionEventArgs e)
        {
            byte opacity = Convert.ToByte(_calOpacity.Interpolate(e.Value.DoubleValue));
            OnTextColor = Color.FromArgb(opacity, OnTextColor.R, OnTextColor.G, OnTextColor.B);

            if (UseBackground)
            {
                BackgroundColor = Color.FromArgb(Convert.ToByte(_calOpacity.Interpolate(e.Value.DoubleValue)), BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            }

            if (!BypassTriggers)
            {
                if (opacity > 0xfc)
                {
                    _incrementOpacityTrigger.FireTrigger(new BindingValue(true));
                }
                else if (opacity < 0x04)
                {
                    _decrementOpacityTrigger.FireTrigger(new BindingValue(true));
                }
            }
        }
        private void IncrementOpacityAction_Execute(object action, HeliosActionEventArgs e)
        {
            byte opacity = OnTextColor.A;
            opacity = (byte)(opacity > 0xfc ? opacity : opacity + 0x04);
            OnTextColor = Color.FromArgb(opacity, OnTextColor.R, OnTextColor.G, OnTextColor.B);

            if (UseBackground)
            {
                byte bOpacity = BackgroundColor.A;
                bOpacity = (byte)(bOpacity > 0xfc ? bOpacity : bOpacity + 0x04);
                BackgroundColor = Color.FromArgb(bOpacity, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            }

            if (!BypassTriggers && opacity > 0xfc)
            {
                _incrementOpacityTrigger.FireTrigger(new BindingValue(true));
            }
        }
        private void DecrementOpacityAction_Execute(object action, HeliosActionEventArgs e)
        {
            byte opacity = OnTextColor.A;
            opacity = (byte)(opacity < 0x04 ? opacity : opacity - 0x04);
            OnTextColor = Color.FromArgb(opacity, OnTextColor.R, OnTextColor.G, OnTextColor.B);

            if (UseBackground)
            {
                byte bOpacity = BackgroundColor.A;
                bOpacity = (byte)(bOpacity < 0x04 ? bOpacity : bOpacity - 0x04);
                BackgroundColor = Color.FromArgb(bOpacity, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            }

            if (!BypassTriggers && opacity < 0x04)
            {
                _decrementOpacityTrigger.FireTrigger(new BindingValue(true));
            }
        }

        #endregion

        #region Overrides

        protected override void OnTextValueChange()
        {
            if (!double.TryParse(_textValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                value = 0d;
            }

            _value.SetValue(new BindingValue(value), BypassTriggers);
        }

        protected override void ReadAdditionalXml(XmlReader reader)
        {
            base.ReadAdditionalXml(reader);

            if (reader.Name == "Unit")
            {
                Unit = BindingValueUnits.FetchUnitByName(reader.ReadElementString("Unit")) ??
                       BindingValueUnits.Numeric;
            }

            if (reader.Name == "Precision")
            {
                int.TryParse(reader.ReadElementString("Precision"), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out _precision);
            }
        }

        protected override void WriteAdditionalXml(XmlWriter writer)
        {
            base.WriteAdditionalXml(writer);

            if (!ReferenceEquals(Unit, BindingValueUnits.Numeric))
            {
                string className = BindingValueUnits.FetchUnitName(Unit);
                if (className != null)
                {
                    writer.WriteElementString("Unit", className);
                }
            }

            if (Precision != DEFAULT_PRECISION)
            {
                writer.WriteElementString("Precision", Precision.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// the binding value unit used for this numeric display, used to convert inputs
        /// </summary>
        public BindingValueUnit Unit
        {
            get => _unit;
            set
            {
                if (ReferenceEquals(_unit, value))
                {
                    return;
                }

                BindingValueUnit oldValue = _unit;
                _unit = value;

                // now actually change unit
                BuildValue();
                OnPropertyChanged(nameof(Unit), oldValue, value, true);
            }
        }
        public override void Reset()
        {
            _displayBrightness = 10d;
            base.Reset();
        }

        /// <summary>
        /// number of digits after the decimal point to display, or 0
        /// </summary>
        public int Precision
        {
            get => _precision;
            set
            {
                if (_precision == value)
                {
                    return;
                }

                int oldValue = _precision;
                _precision = value;
                OnPropertyChanged(nameof(Precision), oldValue, value, true);
            }
        }

        #endregion
    }
}