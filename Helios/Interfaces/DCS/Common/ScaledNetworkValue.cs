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
// 

using System.Globalization;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class ScaledNetworkValue : DCSFunction
    {
        private string _id;
        private string _format;
        private HeliosValue _value;
        private HeliosValue _valueUnscaled;
        private bool _exposeUnscaledValue = false;

        [JsonProperty("calibration", NullValueHandling = NullValueHandling.Ignore)]
        protected CalibrationPointCollectionDouble CalibratedScale { get; set; }

        [JsonProperty("baseValue")]
        protected double BaseValue { get; set; }
        public bool ShouldSerializeBaseValue()
        {
            return CalibratedScale == null;
        }

        [JsonProperty("scale")]
        protected double Scale { get; set; }
        public bool ShouldSerializeScale()
        {
            return CalibratedScale == null;
        }

        [JsonProperty("valueDescription")]
        protected string ValueDescription { get; set; }
        public bool ShouldSerializeValueDescription()
        {
            return !string.IsNullOrEmpty(ValueDescription);
        }

        [JsonProperty("unit")]
        protected BindingValueUnit Unit { get; set; }

        [JsonProperty("exposeunscaledvalue", NullValueHandling = NullValueHandling.Ignore)]
        protected bool? ExposeUnscaledValue { get; set; }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, BindingValueUnit unit, bool exposeUnscaledValue = false)
           : this(sourceInterface, id, device, name, description, "", unit, "%.4f", calibration, 0d, 0d, exposeUnscaledValue)
        {
        }
        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, BindingValueUnit unit, string exportFormat, bool exposeUnscaledValue = false)
            : this(sourceInterface, id, device, name, description, "", unit, exportFormat, calibration, 0d, 0d, exposeUnscaledValue)
        {
        }
        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, string valueDescription, BindingValueUnit unit, bool exposeUnscaledValue = false)
           : this(sourceInterface, id, device, name, description, valueDescription, unit, "%.4f", calibration, 0d, 0d, exposeUnscaledValue)
        {
        }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat, bool exposeUnscaledValue = false)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, calibration, 0d, 0d, exposeUnscaledValue)
        {
        }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit, bool exposeUnscaledValue = false)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, "%.4f", null, 0d, scale, exposeUnscaledValue)
        {
        }
        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat, bool exposeUnscaledValue = false)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, null, 0d, scale, exposeUnscaledValue)
        {
        }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit, double baseValue, string exportFormat, bool exposeUnscaledValue = false)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, null, baseValue, scale, exposeUnscaledValue)
        {
        }

        protected ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat, CalibrationPointCollectionDouble calibration, double baseValue, double scale, bool exposeUnscaledValue = false)
            : base(sourceInterface, device, name, description)
        {
            _id = id;
            _format = exportFormat;
            _exposeUnscaledValue = exposeUnscaledValue;
            Unit = unit;
            CalibratedScale = calibration;
            BaseValue = baseValue;
            Scale = scale;
            if (Scale == 0 && calibration != null && string.IsNullOrEmpty(valueDescription))
            {
                ValueDescription = $"Value between {calibration.MinimumOutputValue} and {calibration.MaximumOutputValue}";
            }
            else
            {
                ValueDescription = valueDescription;
            }

            if (_exposeUnscaledValue)
            {
                ExposeUnscaledValue = true;
            }
            else
            {
                ExposeUnscaledValue = null;
            }
            DoBuild();
        }

        // deserialization constructor
        public ScaledNetworkValue(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            _exposeUnscaledValue = ExposeUnscaledValue?? false;
            DoBuild();
        }

        protected void DoBuild()
        {
            _value = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, ValueDescription, Unit);
            Values.Add(_value);
            Triggers.Add(_value);

            if(_exposeUnscaledValue)
            {
                _valueUnscaled = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} (Unscaled)",
                    $"{CalibratedScale.MinimumInputValue} to {CalibratedScale.MaximumInputValue}", "unscaled value as sent from DCS", BindingValueUnits.Numeric);
                Values.Add(_valueUnscaled);
                Triggers.Add(_valueUnscaled);
            }
        }

        public override void ProcessNetworkData(string id, string value)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat,
                out double scaledValue))
            {
                return;
            }

            if (CalibratedScale != null)
            {
                _value.SetValue(new BindingValue(CalibratedScale.Interpolate(scaledValue)), false);
            }
            else
            {
                _value.SetValue(new BindingValue((scaledValue * Scale) + BaseValue), false);
            }
            if (_exposeUnscaledValue)
            {
                _valueUnscaled.SetValue(new BindingValue(scaledValue), false);
            }
        }

        protected override ExportDataElement[] DefaultDataElements =>
            new ExportDataElement[] { new DCSDataElement(_id, _format, true) };

        public override void Reset()
        {
            _value.SetValue(BindingValue.Empty, true);
            if (_exposeUnscaledValue)
            {
                _valueUnscaled.SetValue(BindingValue.Empty, true);
            }
        }
    }
}
