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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Functions
{
    public class UH60LFunction : DCSFunction
    {
        // We can statically declare our exports because they are not specified in arguments to our
        // constructor.  If we have configurable id numbers, we need to persist those to JSON, so the 
        // code becomes more complicated.  See the GuardedSwitch class for a complicated example.
        private static readonly ExportDataElement[] Elements =
        {
            new DCSDataElement("2055", null), // no format means we export it ourselves in our ExportFunctions.lua 
            new DCSDataElement("2056", null)  // no format means we export it ourselves in our ExportFunctions.lua 
            //new DCSDataElement("12",
            //    "%s") // this will export mainPanelDevice argument number 12 as a string and send it to this function
        };

        public UH60LFunction(BaseUDPInterface sourceInterface, string device, string name, string description)
            : base(sourceInterface, device, name, description)
        {
            DoBuild();
        }

        // deserialization constructor
        public UH60LFunction(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public void DoBuild()
        {
            // construct any values or triggers from data that was potentially deserialized from JSON
        }

        #region Overrides

        protected override ExportDataElement[] DefaultDataElements => Elements;

        public override void ProcessNetworkData(string id, string value)
        {
            // handle the received data for our id values
            switch (id)
            {
                case "2055":
                case "2056":
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