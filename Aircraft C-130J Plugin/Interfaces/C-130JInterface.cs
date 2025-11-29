//  Copyright 2020 Ammo Goettsch
//  Copyright 2024 Helios Contributors
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
     using System.Windows;

    public enum Cockpit { Pilot, Copilot, TroopCommander, LHGunner, RHGunner, AftEngineer}


    [HeliosInterface(
        "Helios.C130J",                         // Helios internal type ID used in Profile XML, must never change
        "DCS C-130J Hercules",                  // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class CH47FInterface : DCSInterface
    {
        //#pragma warning disable IDE1006 // Naming Standard issues
        //#pragma warning disable IDE0051 // Remove unused private members

        //#pragma warning restore IDE0051 // Remove unused private members
        //#pragma warning restore IDE1006 // Naming Standard issues


        public CH47FInterface(string name)
            : base(name, "C-130J-30", "pack://application:,,,/C-130J;component/Interfaces/ExportFunctions.lua")
        {

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
            //#endregion

            //#endregion

        }
        /// <summary>
        /// Converts a circuit breaker Row/Col to the arg code
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>Arg ID in a string</returns>
        private string CbToArg(int panel, char row, int column)
        {
            return (((((int)row - 65) * 33) + column - 1) + (panel == 1 ? 12 : 177)).ToString();
        }

        /// <summary>
        /// Converts a circuit breaker Row/Col to the command code
        /// </summary>
        private string CbToCommand(char row, int column)
        {
            return ((((int)row - 65) * 33) + column - 1 + Commands.Button.Button_1).ToString();
        }
    }
}
