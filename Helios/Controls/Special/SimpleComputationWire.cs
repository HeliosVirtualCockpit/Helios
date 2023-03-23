// Copyright 2023 Helios Contributors
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
using NLog;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// a piece of wire with memory which can have simple arithmetic operations applied to it in a cumulative manner.  The wire also 
    /// has the ability to detect threshold values and trigger on these.
    /// </summary>
    [HeliosControl("Helios.Base.SimpleComputationWire", "Simple Computation Wire", "Special Controls", typeof(ImageDecorationRenderer))]
    public class SimpleComputationWire : MemoryWire, IPropertyNotification
    {
        private HeliosValue _booleanXorSignal;
        private HeliosValue _numericAdditionSignal;
        private HeliosValue _numericSubtractionSignal;
        private HeliosValue _numericMultiplicationSignal;
        private HeliosValue _numericDivisionSignal;
        private HeliosValue _numericThresholdReached;

        private double _upperThresholdValue;
        private double _lowerThresholdValue;
        private double _initialValue;
        private bool _useThresholdValues = false;

        public SimpleComputationWire(): base("SimpleComputationalWire")
        {
            DesignTimeOnly = true;
            Image = "{Helios}/Images/General/simple_computation_wire.png";
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;

            NumericValue = new BindingValue(0d);
            NumericValueInitialized = true;

            _booleanXorSignal = new HeliosValue(this, BindingValue.Empty, "", "Input is Exclusively OR'ed (XOR) with previous boolean result value", "Current boolean signal on this wire.", "Input Value XOR'ed and result sent to output.", BindingValueUnits.Boolean);
            _booleanXorSignal.Execute += new HeliosActionHandler(BooleanXorSignal_Execute);
            Actions.Add(_booleanXorSignal);
            Values.Add(_booleanXorSignal);

            _numericAdditionSignal = new HeliosValue(this, BindingValue.Empty, "", "Numeric addition Value", "Numeric Input is added to the previous numeric result value.", "Input Value Added and result sent to output.", BindingValueUnits.Numeric);
            _numericAdditionSignal.Execute += new HeliosActionHandler(NumericSignalComputation_Execute);
            Actions.Add(_numericAdditionSignal);
            Values.Add(_numericAdditionSignal);

            _numericSubtractionSignal = new HeliosValue(this, BindingValue.Empty, "", "Numeric subtraction value", "Numeric Input is subtracted from the previous numeric result value.", "Input Value Subtracted and result sent to output.", BindingValueUnits.Numeric);
            _numericSubtractionSignal.Execute += new HeliosActionHandler(NumericSignalComputation_Execute);
            Actions.Add(_numericSubtractionSignal);
            Values.Add(_numericSubtractionSignal);

            _numericMultiplicationSignal = new HeliosValue(this, BindingValue.Empty, "", "Numeric multiplication value", "Numeric Input is multiplied by the previous numeric result value.", "Input Value Multiplied and result sent to output.", BindingValueUnits.Numeric);
            _numericMultiplicationSignal.Execute += new HeliosActionHandler(NumericSignalComputation_Execute);
            Actions.Add(_numericMultiplicationSignal);
            Values.Add(_numericMultiplicationSignal);

            _numericDivisionSignal = new HeliosValue(this, BindingValue.Empty, "", "Numeric divisor value", "Previous numeric result value is divided by the numeric input.", "Result of previous numeric input, divided by the current numeric input is sent to output.", BindingValueUnits.Numeric);
            _numericDivisionSignal.Execute += new HeliosActionHandler(NumericSignalComputation_Execute);
            Actions.Add(_numericDivisionSignal);
            Values.Add(_numericDivisionSignal);

            _numericThresholdReached = new HeliosValue(this, BindingValue.Empty, "", "Upper/Lower threshold breach", "The resultant numeric value is outside the Upper/Lower Threshold.", "Result of previous numeric input, divided by the current numeric input is sent to output.", BindingValueUnits.Numeric);
            Triggers.Add(_numericThresholdReached);
            Values.Add(_numericThresholdReached);

        }

        private void BooleanXorSignal_Execute(object action, HeliosActionEventArgs e)
        {
            if (!BooleanValueInitialized)
                BooleanValue = new BindingValue(false);    
            BooleanSignal.SetValue(new BindingValue(BooleanValue.BoolValue ^ e.Value.BoolValue), false);
            BooleanValue = new BindingValue(BooleanValue.BoolValue ^ e.Value.BoolValue);
            BooleanValueInitialized = true;
        }
        private void NumericSignalComputation_Execute(object action, HeliosActionEventArgs e)
        {
            BindingValue tempValue;
            if(action is HeliosValue hAction){
                if (hAction.Name.Contains("addition"))
                {
                    tempValue = new BindingValue(NumericValue.DoubleValue + e.Value.DoubleValue);
                } else if (hAction.Name.Contains("subtraction"))
                {
                    tempValue = new BindingValue(NumericValue.DoubleValue - e.Value.DoubleValue);
                }
                else if (hAction.Name.Contains("multiplication"))
                {
                    tempValue = new BindingValue(NumericValue.DoubleValue * e.Value.DoubleValue);
                }
                else if (hAction.Name.Contains("divisor"))
                {
                    if (e.Value.DoubleValue != 0)
                    {
                        tempValue = new BindingValue(NumericValue.DoubleValue / e.Value.DoubleValue);
                    } else
                    {
                        tempValue = new BindingValue(NumericValue.DoubleValue);
                    }
                }
                else
                {
                    tempValue = new BindingValue(0d);
                }

                NumericValue = tempValue;
                NumericSignal.SetValue(tempValue, false);
                NumericValueInitialized = true;
                if (_useThresholdValues && ((tempValue.DoubleValue >= _upperThresholdValue && _upperThresholdValue != _initialValue) || (tempValue.DoubleValue <= _lowerThresholdValue && _lowerThresholdValue != _initialValue)))
                {
                    NumericValue = new BindingValue(_initialValue);
                    _numericThresholdReached.SetValue(tempValue, false);
                }
            }
        }
        public double LowerThresholdValue
        {
            get => _lowerThresholdValue;
            set
            {
                double oldValue;
                _lowerThresholdValue = value;
                if(value > _initialValue)
                {
                    oldValue = _initialValue;
                    InitialValue = value;
                    OnPropertyChanged("InitialValue", oldValue, value, true);
                }
            }
        }
        public double UpperThresholdValue
        {
            get => _upperThresholdValue;
            set
            {
                double oldValue;
                _upperThresholdValue = value;
                if (value < _initialValue)
                {
                    oldValue = _initialValue;
                    InitialValue = value;
                    OnPropertyChanged("InitialValue", oldValue, value, true);
                }
            }
        }
        public double InitialValue
        {
            get => _initialValue;
            set
            {
                double oldValue;
                _initialValue = value;
                if(value < _lowerThresholdValue)
                {
                    oldValue = _lowerThresholdValue;
                    LowerThresholdValue = value;
                    OnPropertyChanged("LowerThresholdValue", oldValue, value, true);
                } else if(value > _upperThresholdValue)
                {
                    oldValue = _upperThresholdValue;
                    UpperThresholdValue = value;
                    OnPropertyChanged("UpperThresholdValue", oldValue, value, true);
                }
            }
        }
        public bool UseThresholdValues
        {
            get => _useThresholdValues;
            set
            {
                if (!_useThresholdValues.Equals(value))
                {
                    bool oldValue = _useThresholdValues;
                    _useThresholdValues = value;
                    OnPropertyChanged("UseThresholdValues",oldValue,value,true);
                }
            }
        }
        #region overrides
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (reader.Name.Equals("Thresholds"))
            {
                reader.ReadStartElement("Thresholds");
                UseThresholdValues = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("UseThresholdValues"));
                InitialValue = double.Parse(reader.ReadElementString("InitialValue"), CultureInfo.InvariantCulture);
                _lowerThresholdValue = double.Parse(reader.ReadElementString("MinimumValue"), CultureInfo.InvariantCulture);
                _upperThresholdValue = double.Parse(reader.ReadElementString("MaximumValue"), CultureInfo.InvariantCulture);
                reader.ReadEndElement();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (_useThresholdValues)
            {
                writer.WriteStartElement("Thresholds");
                writer.WriteElementString("UseThresholdValues", bc.ConvertToInvariantString(_useThresholdValues));
                writer.WriteElementString("InitialValue", _initialValue.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("MinimumValue", _lowerThresholdValue.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("MaximumValue", _upperThresholdValue.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }

        }
        #endregion
    }
}
