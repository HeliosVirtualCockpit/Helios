//  Copyright 2014 Craig Courtney
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.Interfaces.DCS.FC2
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;

    // WARNING: the type ID must forever stay FC2 so we don't break profiles
    [HeliosInterface("Helios.FC2", "Flaming Cliffs 3", typeof(DCSInterfaceEditor), typeof(UniqueHeliosInterfaceFactory), UniquenessKey = "Helios.DCSInterface")]
    public class FC3Interface : DCSInterface
    {
        public FC3Interface()
            : base("Flaming Cliffs 3", "FC3", "pack://application:,,,/Helios;component/Interfaces/DCS/FC3/ExportFunctions.lua")
        {
            // copy received traffic in both directions (we need to update both models regardless of received code)
            DuplicateReceivedNetworkData("1", "T1");
            DuplicateReceivedNetworkData("2", "T2");
            DuplicateReceivedNetworkData("3", "T3");
            DuplicateReceivedNetworkData("4", "T4");
            DuplicateReceivedNetworkData("5", "T5");
            DuplicateReceivedNetworkData("13", "T13");
            DuplicateReceivedNetworkData("14", "T14");
            DuplicateReceivedNetworkData("16", "T16");
            DuplicateReceivedNetworkData("17", "T17");
            DuplicateReceivedNetworkData("18", "T18");
            DuplicateReceivedNetworkData("19", "T19");
            DuplicateReceivedNetworkData("20", "T20");

            // not setting Vehicles at all results in the module name identifying the only 
            // supported aircraft
            // XXX not yet supported
            // Vehicles = new string[] { ModuleName, "other aircraft", "another aircraft" };

            // see if we can restore from JSON
            //#if (!DEBUG)
            if (LoadFunctionsFromJson())
            {
                return;
            }
            //#endif


            Functions.Add(new NetworkValue(this, "6", "HSI", "ADF bearing", "Current ADF heading.", "(0 - 360)", BindingValueUnits.Degrees, null));
            Functions.Add(new NetworkValue(this, "7", "HSI", "RMI bearing", "Current RMI heading.", "(0 - 360)", BindingValueUnits.Degrees, null));
            Functions.Add(new NetworkValue(this, "8", "HSI", "heading", "Current compass heading.", "(0 - 360)", BindingValueUnits.Degrees, null));
            Functions.Add(new NetworkValue(this, "9", "left engine", "RPM", "Current RPM of the left engine.", "", BindingValueUnits.RPMPercent, null));
            Functions.Add(new NetworkValue(this, "10", "right engine", "RPM", "Current RPM of the right engine.", "", BindingValueUnits.RPMPercent, null));
            Functions.Add(new NetworkValue(this, "11", "left engine", "temperature", "Current temperature of the left engine.", "", BindingValueUnits.Celsius, null));
            Functions.Add(new NetworkValue(this, "12", "right engine", "temperature", "Current temperature of the right engine.", "", BindingValueUnits.Celsius, null));
            Functions.Add(new NetworkValue(this, "15", "HSI", "distance to waypoint", "Number of meters till the next waypoint.", "", BindingValueUnits.Meters, null));

            // for backwards compatibility with modules targeting FC2 interface, we need to also understand these codes that are now provided with "T..." id's by our base class
            // and present them at their original locations in the tree
            Functions.Add(new NetworkValue(this, "1", "ADI", "pitch", "Current pitch of the aircraft.", "(-180 to 180)", BindingValueUnits.Degrees, null));
            Functions.Add(new NetworkValue(this, "2", "ADI", "bank", "Current bank of the aircraft.", "(-180 to 180)", BindingValueUnits.Degrees, null));
            Functions.Add(new NetworkValue(this, "3", "ADI", "side slip", "Current yaw of the aircraft.", "(-180 to 180)", BindingValueUnits.Degrees, null));
            // WARNING: do not correct spelling in value names, because it will break bindings
            Functions.Add(new NetworkValue(this, "4", "", "barametric altitude", "Current barometric altitude the aircraft.", "", BindingValueUnits.Meters, null));
            // WARNING: do not correct spelling in value names, because it will break bindings
            Functions.Add(new NetworkValue(this, "5", "", "rardar altitude", "Current radar altitude of the aircraft.", "", BindingValueUnits.Meters, null));
            Functions.Add(new NetworkValue(this, "13", "", "vertical velocity", "Current vertical velocity of the aircraft.", "", BindingValueUnits.MetersPerSecond, null));
            Functions.Add(new NetworkValue(this, "14", "", "indicated airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.MetersPerSecond, null));
            Functions.Add(new NetworkValue(this, "16", "", "angle of attack", "Current angle of attack for the aircraft.", "", BindingValueUnits.Degrees, null));
            Functions.Add(new NetworkValue(this, "17", "ADI", "glide deviation", "ILS Glide Deviation", "-1 to 1", BindingValueUnits.Numeric, null));
            Functions.Add(new NetworkValue(this, "18", "ADI", "side deviation", "ILS Side Deiviation", "-1 to 1", BindingValueUnits.Numeric, null));
            Functions.Add(new NetworkValue(this, "19", "", "Mach", "Current Mach number", "number in M", BindingValueUnits.Numeric, null));
            Functions.Add(new NetworkValue(this, "20", "", "G", "Current G load", "number in g", BindingValueUnits.Numeric, null));

        }

        public override string StatusName =>
            ImpersonatedVehicleName != null ? $"Flaming Cliffs 3 {ImpersonatedVehicleName}" : Name;
    }
}
