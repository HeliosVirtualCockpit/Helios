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
        //#pragma warning disable IDE1006 // Naming Standard issues
        //#pragma warning disable IDE0051 // Remove unused private members

        //#pragma warning restore IDE0051 // Remove unused private members
        //#pragma warning restore IDE1006 // Naming Standard issues

        public C130JInterface(string name)
            : base(name, "C-130J-30", "pack://application:,,,/C-130J;component/Interfaces/ExportC130JFunctions.lua")
        {

            // not setting Vehicles at all results in the module name identifying the only 
            // supported aircraft
            // XXX not yet supported
            // Vehicles = new string[] { ModuleName, "other aircraft", "another aircraft" };

            // see if we can restore from JSON
#if (!DEBUG)
            if (LoadFunctionsFromJson())
            {
                return;
            }
#endif
            //#endregion

            //#endregion
            if (Functions.Count <= 17)
            {
                foreach (NetworkFunction nf in ProcessInterfaceFunctions.Process(this))
                {
                    AddFunction(nf);
                }
            }

            //ProcessClickables.Analyze();
            //ProcessClickables.CreateFunctionSwitcher();
            #region VHF AM Radio (ARC-210)
            //
            // Below are the switch and button functions for the A-10C II.  The C-130J has it's own functions with different names.
            //
            //AddFunction(new PushButton(this, UFC, BUTTON_33, "534", "UFC", "Toggle ARC-210 RT2 Status (long press)", "%0.1f"));
            //AddFunction(new PushButton(this, UFC, BUTTON_34, "535", "UFC", "Toggle ECCM", "%0.1f"));
            //AddFunction(new PushButton(this, UFC, BUTTON_35, "536", "UFC", "Toggle IDM R/T", "%0.1f"));

            //AddFunction(new Switch(this, VHF_AM_RADIO, "551", SwitchPositions.Create(7, 0d, 0.1d, BUTTON_43, new string[] { "OFF", "TR G", "TR", "ADF", "CHG PRST", "TEST", "ZERO (PULL)" }, "%0.1f"), "ARC-210", "Master switch", "%0.1f"));
            //AddFunction(new RotaryEncoder(this, VHF_AM_RADIO, BUTTON_27, "552", 0.1d, "ARC-210", "Channel select knob"));
            //AddFunction(new Switch(this, VHF_AM_RADIO, "553", SwitchPositions.Create(7, 0d, 0.1d, BUTTON_44, new string[] { "ECCM MASTER", "ECCM", "PRST", "MAN", "MAR", "243", "121 (PULL)" }, "%0.1f"), "ARC-210", "Secondary switch", "%0.1f"));

            //AddFunction(new Switch(this, VHF_AM_RADIO, "554", SwitchPositions.Create(4, 0d, 0.1d, BUTTON_25, "Posn", "%0.1f"), "ARC-210", "100 MHz Selector", "%0.1f"));
            //AddFunction(new Switch(this, VHF_AM_RADIO, "555", SwitchPositions.Create(10, 0d, 0.1d, BUTTON_23, "Posn", "%0.1f"), "ARC-210", "10 MHz Selector", "%0.1f"));
            //AddFunction(new Switch(this, VHF_AM_RADIO, "556", SwitchPositions.Create(10, 0d, 0.1d, BUTTON_21, "Posn", "%0.1f"), "ARC-210", "1 MHz Selector", "%0.1f"));
            //AddFunction(new Switch(this, VHF_AM_RADIO, "557", SwitchPositions.Create(10, 0d, 0.1d, BUTTON_19, "Posn", "%0.1f"), "ARC-210", "100 KHz Selector", "%0.1f"));
            //AddFunction(new Switch(this, VHF_AM_RADIO, "558", SwitchPositions.Create(4, 0d, 0.1d, BUTTON_17, "Posn", "%0.1f"), "ARC-210", "25 KHz Selector", "%0.1f"));

            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_14, "573", "ARC-210", "Enter", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_13, "572", "ARC-210", "Offset frequency", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_12, "571", "ARC-210", "Transmit / receive function toggle", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_11, "570", "ARC-210", "Amplitude modulation / frequency modulation select", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_10, "569", "ARC-210", "Menu pages", "%0.1f"));
            //AddFunction(new Switch(this, VHF_AM_RADIO, "568", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_15), new SwitchPosition("1.0", "On", BUTTON_15) }, "ARC-210", "Squelch on/off", "%0.1f"));

            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_4, "567", "ARC-210", "Select receiver - transmitter", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_3, "566", "ARC-210", "Global positioning system", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_2, "565", "ARC-210", "Time of day receive", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_1, "564", "ARC-210", "Time of day send", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_5, "563", "ARC-210", "Upper FSK", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_6, "562", "ARC-210", "Middle FSK", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_7, "561", "ARC-210", "Lower FSK", "%0.1f"));

            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_8, "560", "ARC-210", "Brightness increase", "%0.1f"));
            //AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_9, "559", "ARC-210", "Brightness decrease", "%0.1f"));

            AddFunction(new ARC210Display(this)); // 2927
            #endregion


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
