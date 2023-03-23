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

using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// a piece of wire with memory, simply sends any value on its input side to its output side
    /// and on a resend request sends any last values entered on its input side to its output side
    /// </summary>
    [HeliosControl("Helios.Base.MemoryWire", "Memory Wire", "Special Controls", typeof(ImageDecorationRenderer))]
    public class MemoryWire : ImageDecorationBase
    {
        private HeliosValue _numericSignal;
        private HeliosValue _booleanSignal;
        private HeliosValue _textSignal;
        private HeliosValue _resendValues;

        private BindingValue _numericValue;
        private BindingValue _booleanValue;
        private BindingValue _textValue;

        private bool _numericValueInitialized = false;
        private bool _booleanValueInitialized = false;
        private bool _textValueInitialized = false;


        public MemoryWire() : this("MemoryWire") { } 
        public MemoryWire (string ControlName) : base(ControlName){
            DesignTimeOnly = true;
            Image = "{Helios}/Images/General/memory_wire.png";
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;

            _numericSignal = new HeliosValue(this, BindingValue.Empty, "", "Numeric Signal", "Current numeric signal on this wire.", "Value copied from input to output.", BindingValueUnits.Numeric);
            _numericSignal.Execute += new HeliosActionHandler(NumericSignal_Execute);
            Actions.Add(_numericSignal);
            Triggers.Add(_numericSignal);
            Values.Add(_numericSignal);

            _booleanSignal = new HeliosValue(this, BindingValue.Empty, "", "Boolean Signal", "Current boolean signal on this wire.", "Value copied from input to output.", BindingValueUnits.Boolean);
            _booleanSignal.Execute += new HeliosActionHandler(BooleanSignal_Execute);
            Actions.Add(_booleanSignal);
            Triggers.Add(_booleanSignal);
            Values.Add(_booleanSignal);

            _textSignal = new HeliosValue(this, BindingValue.Empty, "", "Text Signal", "Current text signal on this wire.", "Value copied from input to output.", BindingValueUnits.Text);
            _textSignal.Execute += new HeliosActionHandler(TextSignal_Execute);
            Actions.Add(_textSignal);
            Triggers.Add(_textSignal);
            Values.Add(_textSignal);

            _resendValues = new HeliosValue(this, new BindingValue(false), "", "Resend Values", "Resend last input values to output.", "Set true to resend last input values to output.", BindingValueUnits.Boolean);
            _resendValues.Execute += new HeliosActionHandler(ResendValues_Execute);
            Actions.Add(_resendValues);
            Values.Add(_resendValues);
        }

        private void NumericSignal_Execute(object action, HeliosActionEventArgs e)
        {
            _numericValue = e.Value;
            _numericSignal.SetValue(e.Value, false);
            _numericValueInitialized = true;
        }

        private void BooleanSignal_Execute(object action, HeliosActionEventArgs e)
        {
            _booleanValue = e.Value;
            _booleanSignal.SetValue(e.Value, false);
            _booleanValueInitialized = true;
        }

        private void TextSignal_Execute(object action, HeliosActionEventArgs e)
        {
            _textValue = e.Value;
            _textSignal.SetValue(e.Value, false);
            _textValueInitialized = true;
        }

        private void ResendValues_Execute(object action, HeliosActionEventArgs e)
        {
            _resendValues.SetValue(e.Value, e.BypassCascadingTriggers);

            if (_resendValues.Value.BoolValue)
            {
                ResendValues();
            }
        }

        virtual protected void ResendValues()
        {
            if (_numericValueInitialized)
            {
                _numericSignal.SetValue(BindingValue.Empty, true);
                _numericSignal.SetValue(_numericValue, false);
            }

            if (_booleanValueInitialized)
            {
                _booleanSignal.SetValue(BindingValue.Empty, true);
                _booleanSignal.SetValue(_booleanValue, false);
            }

            if (_textValueInitialized)
            {
                _textSignal.SetValue(BindingValue.Empty, true);
                _textSignal.SetValue(_textValue, false);
            }
        }
        #region properties
        virtual protected HeliosValue BooleanSignal
        {
            get => _booleanSignal;
            set => _booleanSignal = value;
        }
        virtual protected BindingValue BooleanValue
        {
            get => _booleanValue;
            set => _booleanValue = value;
        }
        virtual protected bool BooleanValueInitialized
        {
            get => _booleanValueInitialized;
            set => _booleanValueInitialized = value;
        }
        virtual protected HeliosValue NumericSignal
        {
            get => _numericSignal;
            set => _numericSignal = value;
        }
        virtual protected BindingValue NumericValue
        {
            get => _numericValue;
            set => _numericValue = value;
        }
        virtual protected bool NumericValueInitialized
        {
            get => _numericValueInitialized;
            set => _numericValueInitialized = value;
        }
        #endregion properties
    }
}
