//  Copyright 2020 Ammo Goettsch
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Template
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Template.Functions;

    /* enabling this attribute will cause Helios to discover this new interface and make it available for use
    [HeliosInterface(
        "Helios.Template",                      // Helios internal type ID used in Profile XML, must never change
        "DCS TemplateHumanName",                // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey="Helios.DCSInterface")]   // all other DCS interfaces exclude this interface
    */
    public class UH60LInterface : DCSInterface
    {
        public UH60LInterface(string name)
            : base(name, "this must the name of primary aircraft in the supported module", "pack://application:,,,/Helios;component/Interfaces/DCS/Template/ExportFunctions.lua")
        {
            // optionally support more than just the base aircraft, or even use a module
            // name that is not a vehicle, by removing it from this list
            //
            // not setting Vehicles at all results in the module name identifying the only 
            // supported aircraft
            // XXX not yet supported
            // Vehicles = new string[] { ModuleName, "other aircraft", "another aircraft" };

            // see if we can restore from JSON
            if (LoadFunctionsFromJson())
            {
                return;
            }

            // add all the functions in code instead
            AddFunction(new ExampleFunction(this, "Fictional Aircraft Device", "Fictional Aircraft Function", "A fictional function on an example aircraft"));
        }
    }
}
