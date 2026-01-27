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

using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class DrawingValue : DCSFunction
    {
        private string _id;
        private string _format;
        private HeliosValue _value;

        [JsonProperty("valueDescription")]
        private string _valueDescription;

        [JsonProperty("unit")]
        private BindingValueUnit _unit;

        [JsonProperty("aircraftDrawArgument")]
        private string _aircraftDrawArgument;

        /// <summary>
        /// Defines a value which is obtained from a LoGetAircraftDrawArgumentValue() call
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="id"></param>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="valueDescription"></param>
        /// <param name="unit"></param>
        public DrawingValue(BaseUDPInterface sourceInterface, string id, string device, string name, string description, string valueDescription, BindingValueUnit unit)
            : this(sourceInterface, id, device, name, description, valueDescription, unit, "%.2f")
        {
        }
        /// <summary>
        /// Defines a value which is obtained from a LoGetAircraftDrawArgumentValue() call
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="id"></param>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="valueDescription"></param>
        /// <param name="unit"></param>
        /// <param name="exportFormat"></param>
        public DrawingValue(BaseUDPInterface sourceInterface, string id, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat)
            : base(sourceInterface, device, name, description)
        {
            _id = id;
            _aircraftDrawArgument = id;
            _format = exportFormat;
            _valueDescription = valueDescription;
            _unit = unit;
            DoBuild();
        }

        // deserialization constructor
        public DrawingValue(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            DoBuild();
        }

        private void DoBuild()
        {
            _value = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, _valueDescription, _unit);
            Values.Add(_value);
            Triggers.Add(_value);
        }

        public override void ProcessNetworkData(string id, string value)
        {
            _value.SetValue(new BindingValue(value), false);
        }

        protected override ExportDataElement[] DefaultDataElements =>
            new ExportDataElement[] { new DCSDataElement("D" + _id, _format, true) };

        public override void Reset()
        {
            _value.SetValue(BindingValue.Empty, true);
        }
    }
}
