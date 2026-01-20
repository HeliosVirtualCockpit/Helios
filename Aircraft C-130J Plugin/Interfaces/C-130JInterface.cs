//  Copyright 2020 Ammo Goettsch
//  Copyright 2025 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.C130J
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J.Functions;


    public enum Cockpit { Pilot, Copilot, Augpilot }


    [HeliosInterface(
        "Helios.C130J",                         // Helios internal type ID used in Profile XML, must never change
        "DCS C-130J Hercules (ASC)",            // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class C130JInterface : DCSInterface
    {
        public C130JInterface(string name)
            : base(name, "C-130J-30", "pack://application:,,,/C-130J;component/Interfaces/ExportC130JFunctions.lua")
        {

            // see if we can restore from JSON
#if (!DEBUG)
            if (LoadFunctionsFromJson())
            {
                return;
            }
#endif
            if (Functions.Count <= 17)
            {
                foreach (NetworkFunction nf in ProcessInterfaceFunctions.Process(this))
                {
                    AddFunction(nf);
                }
            }

            //ProcessClickables.Analyze();
            //ProcessClickables.CreateFunctionSwitcher();


        }
    }
}
