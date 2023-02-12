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

using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.UDPInterface;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.F16C.Functions
{
    public class F16CFunction : DCSFunction
    {
        private HeliosValue _value;
        // We can statically declare our exports because they are not specified in arguments to our
        // constructor.  If we have configurable id numbers, we need to persist those to JSON, so the 
        // code becomes more complicated.  See the GuardedSwitch class for a complicated example.
        private static readonly ExportDataElement[] Elements =
        {
            new DCSDataElement("4000", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("4001", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("4002", null), // no format means we export it ourselves in our ExportFunctions.lua 
            //new DCSDataElement("2051", null), // no format means we export it ourselves in our ExportFunctions.lua 
            //new DCSDataElement("2059", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2060", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2080", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2081", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2082", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2083", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2084", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2085", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2086", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2087", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2088", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2089", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("4002", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("12", "%s")    // this will export mainPanelDevice argument number 12 as a string and send it to this function
        };

        public F16CFunction(BaseUDPInterface sourceInterface, string device, string name, string description)
            : base(sourceInterface, device, name, description)
        {
            DoBuild();
        }

        // deserialization constructor
        public F16CFunction(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public void DoBuild()
        {
            // construct any values or triggers from data that was potentially deserialized from JSON
            _value = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                    SerializedDescription, "", BindingValueUnits.Boolean);
            Values.Add(_value);
            Triggers.Add(_value);
        }

        #region Overrides

        protected override ExportDataElement[] DefaultDataElements => Elements;

        public override void ProcessNetworkData(string id, string value)
        {
            // handle the received data for our id values
            switch (id)
            {
                case "12":
                    // do stuff with received valued here
                    break;
                case "4000":
                    // do stuff with received valued here
                    break;
                case "4001":
                    // do stuff with received valued here
                    break;
                case "4002":
                    // do stuff with received valued here
                    break;
                case "2051":
                    // do stuff with received valued here
                    break;
                case "2059":
                    // do stuff with received valued here
                    break;
                case "2060":
                    // do stuff with received valued here
                    break;
                case "2080":
                    // do stuff with received valued here
                    break;
                case "2081":
                    // do stuff with received valued here
                    break;
                case "2082":
                    // do stuff with received valued here
                    break;
                case "2083":
                    // do stuff with received valued here
                    break;
                case "2084":
                case "2085":
                case "2086":
                case "2087":
                case "2088":
                case "2089":
                    // do stuff with received valued here
                    break;

            }
        }

        public override void Reset()
        {
            // reset to clean state if profile is reset
        }

        public override void BuildAfterDeserialization()
        {
            // this does the same initialization as our non-default constructor, but is used
            // when we are deserialized from JSON
            DoBuild();
        }

        #endregion
    }
}