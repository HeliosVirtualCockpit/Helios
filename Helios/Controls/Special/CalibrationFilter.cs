// Copyright 2020 Ammo Goettsch
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
using System.Reflection;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// This invisible control can apply a calibration curve to an input value and
    /// optionally assign a unit to the output.  It can be used to adapt values in a profile
    /// without having to write a calibration function in code or Lua script
    /// </summary>
    [HeliosControl("Helios.Base.CalibrationFilter", "Calibration Filter", "Special Controls",
        typeof(ImageDecorationRenderer))]
    public class CalibrationFilter : ImageDecorationBase
    {
        /// <summary>
        /// list of binding value units sorted in order that is suitable for presentation in a list
        /// </summary>
        internal static readonly SortedSet<BindingValueUnit> StaticUnits;

        /// <summary>
        /// map from serialized unit name to instance of that binding unit
        /// </summary>
        private static readonly Dictionary<string, BindingValueUnit> UnitsByLongName;

        private class CalibrationComparer : IComparer<BindingValueUnit>
        {
            public int Compare(BindingValueUnit left, BindingValueUnit right) => string.Compare(left?.LongName,
                right?.LongName, StringComparison.InvariantCultureIgnoreCase);
        }

        static CalibrationFilter()
        {
            UnitsByLongName = new Dictionary<string, BindingValueUnit>();
            SortedSet<BindingValueUnit> units = new SortedSet<BindingValueUnit>(new CalibrationComparer());
            Type unitsClass = typeof(BindingValueUnits);
            foreach (FieldInfo field in unitsClass.GetFields())
            {
                object value = field.GetValue(null);
                if (value is BindingValueUnit unit)
                {
                    units.Add(unit);
                }
            }

            StaticUnits = units;

            // now add unique mappings to our table
            foreach (BindingValueUnit unit in units)
            {
                UnitsByLongName[unit.LongName] = unit;
            }
        }

        public CalibrationFilter() : base("Calibration Filter")
        {
            DesignTimeOnly = true;
            Image = "{Helios}/Images/General/calibration.png";
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;

            HeliosAction inputValue = new HeliosAction(this, "Input", "Value", "set", 
                "the input value to be transformed by the calibration curve",
                "a numeric value in the range covered by the calibration curve",
                BindingValueUnits.Numeric);
            inputValue.Execute += InputValue_Execute;
            Actions.Add(inputValue);

            // create initial version in case we don't ReadXML
            RecreateOutput();
        }

        private void RecreateOutput()
        {
            if (_outputValue != null)
            {
                // remove any output bindings
                foreach (HeliosBinding outputBinding in OutputBindings)
                {
                    outputBinding.Action.Target.InputBindings.Remove(outputBinding);
                }
                OutputBindings.Clear();

                Values.Remove(_outputValue);
                Triggers.Remove(_outputValue);
                _outputValue = null;
            }

            // now create the new output value
            _outputValue = new HeliosValue(this, _interpolated, "Output", "Value", 
                "the output value after the calibration curve has been applied", 
                "a calibrated value", OutputUnit);
            Values.Add(_outputValue);
            Triggers.Add(_outputValue);
        }

        private void InputValue_Execute(object action, HeliosActionEventArgs e)
        {
            if (e.Value.IsEmptyValue)
            {
                return;
            }
            BindingValue outputValue = new BindingValue(Calibration.Interpolate(e.Value.DoubleValue));
            if (outputValue.IsIdenticalTo(_interpolated))
            {
                return;
            }
            _interpolated = outputValue;
            _outputValue?.SetValue(outputValue, false);
        }

        public CalibrationPointCollectionDouble Calibration { get; } = new CalibrationPointCollectionDouble()
        {
            Preceision = 10
        };

        /// <summary>
        /// backing field for property OutputUnit, contains
        /// binding value unit to use for output
        /// </summary>
        private BindingValueUnit _outputUnit = BindingValueUnits.Numeric;

        /// <summary>
        /// most recently output value
        /// </summary>
        private BindingValue _interpolated = BindingValue.Empty;

        private HeliosValue _outputValue;

        /// <summary>
        /// binding value unit to use for output
        /// </summary>
        public BindingValueUnit OutputUnit
        {
            get => _outputUnit;
            set
            {
                if (_outputUnit.Equals(value))
                {
                    return;
                }

                BindingValueUnit oldValue = _outputUnit;
                _outputUnit = value;
                RecreateOutput();
                OnPropertyChanged("OutputUnit", oldValue, value, true);
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            Calibration.Write(writer);
            if (!OutputUnit.Equals(BindingValueUnits.Numeric))
            {
                writer.WriteElementString("OutputUnit", OutputUnit.LongName);
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name == "PointCount")
            {
                // NOTE: this should always be there
                Calibration.Read(reader);
            }
            if (reader.Name != "OutputUnit")
            {
                return;
            }

            string unitName = reader.ReadElementString("OutputUnit");
            _outputUnit = UnitsByLongName[unitName] ??
                          throw new Exception($"could not decode binding value unit name '{unitName}'");
            RecreateOutput();
        }
    }
}