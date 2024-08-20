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
        private string ValueDescription { get; set; }
        public bool ShouldSerializeValueDescription()
        {
            return !string.IsNullOrEmpty(ValueDescription);
        }

        [JsonProperty("unit")]
        protected BindingValueUnit Unit { get; set; }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, string valueDescription, BindingValueUnit unit)
           : this(sourceInterface, id, device, name, description, valueDescription, unit, "%.4f", calibration, 0d, 0d)
        {
        }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, calibration, 0d, 0d)
        {
        }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, "%.4f", null, 0d, scale)
        {
        }
        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, null, 0d, scale)
        {
        }

        public ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit, double baseValue, string exportFormat)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, null, baseValue, scale)
        {
        }

        protected ScaledNetworkValue(BaseUDPInterface sourceInterface, string id, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat, CalibrationPointCollectionDouble calibration, double baseValue, double scale)
            : base(sourceInterface, device, name, description)
        {
            _id = id;
            _format = exportFormat;
            ValueDescription = valueDescription;
            Unit = unit;
            CalibratedScale = calibration;
            BaseValue = baseValue;
            Scale = scale;
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
            DoBuild();
        }

        protected void DoBuild()
        {
            _value = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, ValueDescription, Unit);
            Values.Add(_value);
            Triggers.Add(_value);
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
        }

        protected override ExportDataElement[] DefaultDataElements =>
            new ExportDataElement[] { new DCSDataElement(_id, _format, true) };

        public override void Reset()
        {
            _value.SetValue(BindingValue.Empty, true);
        }
    }
}
