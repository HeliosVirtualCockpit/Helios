//  Copyright 2020 Ammo Goettsch
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.CH47F
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.CH47F.Functions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml.Linq;
    using static GadrocsWorkshop.Helios.Interfaces.DCS.CH47F.Commands;
    using System.Net.Sockets;
    using System;

    public enum Cockpit { Pilot, Copilot, TroopCommander, LHGunner, RHGunner, AftEngineer}


    [HeliosInterface(
        "Helios.CH47F",                         // Helios internal type ID used in Profile XML, must never change
        "DCS CH-47F Chinook",                    // human readable UI name for this interface
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
            : base(name, "CH-47Fbl1", "pack://application:,,,/CH-47F;component/Interfaces/ExportFunctions.lua")
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
            #region Cautions
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_26.ToString("d"), "1393", "Canted Console", "Master Caution Switch (Pilot)"));
            AddFunction(new FlagValue(this, "1394", "Canted Console", "Master Caution Indicator (Pilot)", ""));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_106.ToString("d"), "1391", "Canted Console", "Master Caution Switch (Copilot)"));
            AddFunction(new FlagValue(this, "1392", "Canted Console", "Master Caution Indicator (Copilot)", ""));
            #endregion
            #region MFDs and CDUs
            /// RegEx used for this region
            /// elements\[\x22(?'unit'.*?)-(?'position'.*?)-(?'element'.*)-(?'argId'\d{1,4})\x22\]\s*\=\s*(?'function'[a-zA-z0-9_]*)\(\'(?'cockpit'.*)\'\,(?'function_args'.*)\.((KEY)|(LSK))_(?'key'.{1,11})\x22\){0,1}\,.*device_commands\.(?'command'[a-zA-Z0-9_]*)\).*

            #region CDUs
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_1.ToString("d"), "342", "CDU (Left)", "MSN"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_2.ToString("d"), "343", "CDU (Left)", "FPLN"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_3.ToString("d"), "344", "CDU (Left)", "FD"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_4.ToString("d"), "345", "CDU (Left)", "IDX"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_5.ToString("d"), "346", "CDU (Left)", "DIR"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_6.ToString("d"), "347", "CDU (Left)", "SNSR"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_7.ToString("d"), "348", "CDU (Left)", "MFD_DATA"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_8.ToString("d"), "349", "CDU (Left)", "L1"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_9.ToString("d"), "350", "CDU (Left)", "L2"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_10.ToString("d"), "351", "CDU (Left)", "L3"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_11.ToString("d"), "352", "CDU (Left)", "L4"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_12.ToString("d"), "353", "CDU (Left)", "L5"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_13.ToString("d"), "354", "CDU (Left)", "L6"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_14.ToString("d"), "355", "CDU (Left)", "R1"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_15.ToString("d"), "356", "CDU (Left)", "R2"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_16.ToString("d"), "357", "CDU (Left)", "R3"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_17.ToString("d"), "358", "CDU (Left)", "R4"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_18.ToString("d"), "359", "CDU (Left)", "R5"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_19.ToString("d"), "360", "CDU (Left)", "R6"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_20.ToString("d"), "361", "CDU (Left)", "BRT"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_21.ToString("d"), "362", "CDU (Left)", "DIM"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_22.ToString("d"), "363", "CDU (Left)", "CNI"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_23.ToString("d"), "364", "CDU (Left)", "PAD"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_24.ToString("d"), "365", "CDU (Left)", "arrow left"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_25.ToString("d"), "366", "CDU (Left)", "arrow right"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_26.ToString("d"), "367", "CDU (Left)", "arrow up"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_27.ToString("d"), "368", "CDU (Left)", "arrow down"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_28.ToString("d"), "369", "CDU (Left)", "CLR"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_29.ToString("d"), "370", "CDU (Left)", "WPN"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_30.ToString("d"), "371", "CDU (Left)", "1"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_31.ToString("d"), "372", "CDU (Left)", "2"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_32.ToString("d"), "373", "CDU (Left)", "3"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_33.ToString("d"), "374", "CDU (Left)", "4"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_34.ToString("d"), "375", "CDU (Left)", "5"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_35.ToString("d"), "376", "CDU (Left)", "6"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_36.ToString("d"), "377", "CDU (Left)", "7"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_37.ToString("d"), "378", "CDU (Left)", "8"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_38.ToString("d"), "379", "CDU (Left)", "9"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_39.ToString("d"), "380", "CDU (Left)", "0"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_40.ToString("d"), "381", "CDU (Left)", "dot"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_41.ToString("d"), "382", "CDU (Left)", "MARK"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_42.ToString("d"), "383", "CDU (Left)", "slash"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_43.ToString("d"), "384", "CDU (Left)", "A"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_44.ToString("d"), "385", "CDU (Left)", "B"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_45.ToString("d"), "386", "CDU (Left)", "C"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_46.ToString("d"), "387", "CDU (Left)", "D"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_47.ToString("d"), "388", "CDU (Left)", "E"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_48.ToString("d"), "389", "CDU (Left)", "F"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_49.ToString("d"), "390", "CDU (Left)", "G"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_50.ToString("d"), "391", "CDU (Left)", "H"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_51.ToString("d"), "392", "CDU (Left)", "I"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_52.ToString("d"), "393", "CDU (Left)", "J"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_53.ToString("d"), "394", "CDU (Left)", "K"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_54.ToString("d"), "395", "CDU (Left)", "L"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_55.ToString("d"), "396", "CDU (Left)", "M"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_56.ToString("d"), "397", "CDU (Left)", "N"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_57.ToString("d"), "398", "CDU (Left)", "O"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_58.ToString("d"), "399", "CDU (Left)", "P"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_59.ToString("d"), "400", "CDU (Left)", "Q"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_60.ToString("d"), "401", "CDU (Left)", "R"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_61.ToString("d"), "402", "CDU (Left)", "S"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_62.ToString("d"), "403", "CDU (Left)", "T"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_63.ToString("d"), "404", "CDU (Left)", "U"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_64.ToString("d"), "405", "CDU (Left)", "V"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_65.ToString("d"), "406", "CDU (Left)", "W"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_66.ToString("d"), "407", "CDU (Left)", "X"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_67.ToString("d"), "408", "CDU (Left)", "Y"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_68.ToString("d"), "409", "CDU (Left)", "Z"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_69.ToString("d"), "410", "CDU (Left)", "SP"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_70.ToString("d"), "411", "CDU (Left)", "dash"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_71.ToString("d"), "412", "CDU (Left)", "TDL"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_72.ToString("d"), "413", "CDU (Left)", "ASE"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_73.ToString("d"), "414", "CDU (Left)", "empty"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_74.ToString("d"), "415", "CDU (Left)", "DATA"));
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_75.ToString("d"), "416", "CDU (Left)", "STAT"));
            AddFunction(new Axis(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_77.ToString("d"), "982", 0.1d, 0.0d, 1.0d, "CDU (Left)", "CDU Outer Knob"));  // elements["MFK1_KNOB_OUTER"] = axis_limited({0, 1}, _("Cockpit.CH47.MFK_OUTER_LEFT"),    devices.CDU_LEFT, device_commands.Button_77, 982)
            AddFunction(new Axis(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_76.ToString("d"), "983", 0.1d, 0.0d, 1.0d, "CDU (Left)", "CDU Inner Knob"));  // elements["MFK1_KNOB_OUTER"] = axis_limited({0, 1}, _("Cockpit.CH47.MFK_OUTER_LEFT"),    devices.CDU_LEFT, device_commands.Button_77, 982)
            AddFunction(new PushButton(this, devices.CDU_LEFT.ToString("d"), Commands.Button.Button_78.ToString("d"), "984", "CDU (Left)", "CDU Pull Knob", "%.1f"));  // elements["LAMPS_TEST"] =         button({0, 1}, _("Cockpit.CH47.lamps_test_sw"), devices.CANTED_CONSOLE, device_commands.Button_44, 582, {{SOUND_SW07_OFF, SOUND_SW07_ON}})


            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_1.ToString("d"), "417", "CDU (Right)", "MSN"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_2.ToString("d"), "418", "CDU (Right)", "FPLN"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_3.ToString("d"), "419", "CDU (Right)", "FD"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_4.ToString("d"), "420", "CDU (Right)", "IDX"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_5.ToString("d"), "421", "CDU (Right)", "DIR"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_6.ToString("d"), "422", "CDU (Right)", "SNSR"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_7.ToString("d"), "423", "CDU (Right)", "MFD_DATA"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_8.ToString("d"), "424", "CDU (Right)", "L1"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_9.ToString("d"), "425", "CDU (Right)", "L2"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_10.ToString("d"), "426", "CDU (Right)", "L3"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_11.ToString("d"), "427", "CDU (Right)", "L4"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_12.ToString("d"), "428", "CDU (Right)", "L5"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_13.ToString("d"), "429", "CDU (Right)", "L6"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_14.ToString("d"), "430", "CDU (Right)", "R1"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_15.ToString("d"), "431", "CDU (Right)", "R2"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_16.ToString("d"), "432", "CDU (Right)", "R3"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_17.ToString("d"), "433", "CDU (Right)", "R4"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_18.ToString("d"), "434", "CDU (Right)", "R5"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_19.ToString("d"), "435", "CDU (Right)", "R6"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_20.ToString("d"), "436", "CDU (Right)", "BRT"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_21.ToString("d"), "437", "CDU (Right)", "DIM"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_22.ToString("d"), "438", "CDU (Right)", "CNI"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_23.ToString("d"), "439", "CDU (Right)", "PAD"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_24.ToString("d"), "440", "CDU (Right)", "arrow left"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_25.ToString("d"), "441", "CDU (Right)", "arrow right"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_26.ToString("d"), "442", "CDU (Right)", "arrow up"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_27.ToString("d"), "443", "CDU (Right)", "arrow down"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_28.ToString("d"), "444", "CDU (Right)", "CLR"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_29.ToString("d"), "445", "CDU (Right)", "WPN"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_30.ToString("d"), "446", "CDU (Right)", "1"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_31.ToString("d"), "447", "CDU (Right)", "2"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_32.ToString("d"), "448", "CDU (Right)", "3"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_33.ToString("d"), "449", "CDU (Right)", "4"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_34.ToString("d"), "450", "CDU (Right)", "5"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_35.ToString("d"), "451", "CDU (Right)", "6"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_36.ToString("d"), "452", "CDU (Right)", "7"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_37.ToString("d"), "453", "CDU (Right)", "8"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_38.ToString("d"), "454", "CDU (Right)", "9"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_39.ToString("d"), "455", "CDU (Right)", "0"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_40.ToString("d"), "456", "CDU (Right)", "dot"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_41.ToString("d"), "457", "CDU (Right)", "MARK"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_42.ToString("d"), "458", "CDU (Right)", "slash"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_43.ToString("d"), "459", "CDU (Right)", "A"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_44.ToString("d"), "460", "CDU (Right)", "B"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_45.ToString("d"), "461", "CDU (Right)", "C"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_46.ToString("d"), "462", "CDU (Right)", "D"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_47.ToString("d"), "463", "CDU (Right)", "E"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_48.ToString("d"), "464", "CDU (Right)", "F"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_49.ToString("d"), "465", "CDU (Right)", "G"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_50.ToString("d"), "466", "CDU (Right)", "H"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_51.ToString("d"), "467", "CDU (Right)", "I"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_52.ToString("d"), "468", "CDU (Right)", "J"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_53.ToString("d"), "469", "CDU (Right)", "K"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_54.ToString("d"), "470", "CDU (Right)", "L"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_55.ToString("d"), "471", "CDU (Right)", "M"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_56.ToString("d"), "472", "CDU (Right)", "N"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_57.ToString("d"), "473", "CDU (Right)", "O"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_58.ToString("d"), "474", "CDU (Right)", "P"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_59.ToString("d"), "475", "CDU (Right)", "Q"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_60.ToString("d"), "476", "CDU (Right)", "R"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_61.ToString("d"), "477", "CDU (Right)", "S"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_62.ToString("d"), "478", "CDU (Right)", "T"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_63.ToString("d"), "479", "CDU (Right)", "U"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_64.ToString("d"), "480", "CDU (Right)", "V"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_65.ToString("d"), "481", "CDU (Right)", "W"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_66.ToString("d"), "482", "CDU (Right)", "X"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_67.ToString("d"), "483", "CDU (Right)", "Y"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_68.ToString("d"), "484", "CDU (Right)", "Z"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_69.ToString("d"), "485", "CDU (Right)", "SP"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_70.ToString("d"), "486", "CDU (Right)", "dash"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_71.ToString("d"), "487", "CDU (Right)", "TDL"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_72.ToString("d"), "488", "CDU (Right)", "ASE"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_73.ToString("d"), "489", "CDU (Right)", "empty"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_74.ToString("d"), "490", "CDU (Right)", "DATA"));
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_75.ToString("d"), "492", "CDU (Right)", "STAT"));
            AddFunction(new Axis(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_77.ToString("d"), "985", 0.1d, 0.0d, 1.0d, "CDU (Right)", "CDU Outer Knob"));  // elements["MFK2_KNOB_OUTER"] = axis_limited({0, 1}, _("Cockpit.CH47.MFK_OUTER_RIGHT"),    devices.CDU_RIGHT, device_commands.Button_77, 985)
            AddFunction(new Axis(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_76.ToString("d"), "986", 0.1d, 0.0d, 1.0d, "CDU (Right)", "CDU Inner Knob"));  // elements["MFK2_KNOB_OUTER"] = axis_limited({0, 1}, _("Cockpit.CH47.MFK_OUTER_RIGHT"),    devices.CDU_RIGHT, device_commands.Button_77, 985)
            /// TODO:  This looks wrong - it has the same command code as the Left CDU Pull knob
            AddFunction(new PushButton(this, devices.CDU_RIGHT.ToString("d"), Commands.Button.Button_78.ToString("d"), "987", "CDU (Right)", "CDU Pull Knob", "%.1f"));

            #endregion
            #region MFDs
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_1.ToString("d"), "798", "MFD (Copilot Left)", "T1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_2.ToString("d"), "799", "MFD (Copilot Left)", "T2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_3.ToString("d"), "800", "MFD (Copilot Left)", "T3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_4.ToString("d"), "801", "MFD (Copilot Left)", "T4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_5.ToString("d"), "802", "MFD (Copilot Left)", "T5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_6.ToString("d"), "803", "MFD (Copilot Left)", "T6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_7.ToString("d"), "804", "MFD (Copilot Left)", "T7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_8.ToString("d"), "805", "MFD (Copilot Left)", "R1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_9.ToString("d"), "806", "MFD (Copilot Left)", "R2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_10.ToString("d"), "807", "MFD (Copilot Left)", "R3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_11.ToString("d"), "808", "MFD (Copilot Left)", "R4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_12.ToString("d"), "809", "MFD (Copilot Left)", "R5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_13.ToString("d"), "810", "MFD (Copilot Left)", "R6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_14.ToString("d"), "811", "MFD (Copilot Left)", "R7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_15.ToString("d"), "812", "MFD (Copilot Left)", "R8"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_16.ToString("d"), "813", "MFD (Copilot Left)", "R9"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_17.ToString("d"), "814", "MFD (Copilot Left)", "B1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_18.ToString("d"), "815", "MFD (Copilot Left)", "B2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_19.ToString("d"), "816", "MFD (Copilot Left)", "B3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_20.ToString("d"), "817", "MFD (Copilot Left)", "B4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_21.ToString("d"), "818", "MFD (Copilot Left)", "B5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_22.ToString("d"), "819", "MFD (Copilot Left)", "B6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_23.ToString("d"), "820", "MFD (Copilot Left)", "B7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_24.ToString("d"), "821", "MFD (Copilot Left)", "L1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_25.ToString("d"), "822", "MFD (Copilot Left)", "L2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_26.ToString("d"), "823", "MFD (Copilot Left)", "L3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_27.ToString("d"), "824", "MFD (Copilot Left)", "L4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_28.ToString("d"), "825", "MFD (Copilot Left)", "L5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_29.ToString("d"), "826", "MFD (Copilot Left)", "L6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_30.ToString("d"), "827", "MFD (Copilot Left)", "L7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_31.ToString("d"), "828", "MFD (Copilot Left)", "L8"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), Commands.Button.Button_32.ToString("d"), "829", "MFD (Copilot Left)", "L9"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), "794", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_33.ToString("d"), new string[] { "OFF", "NVG", "NORM" }, "%0.1f"), "MFD (Copilot Left)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), "795", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_34.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_35.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0") }, "MFD (Copilot Left)", "Brightness Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), "796", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_37.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_38.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0") }, "MFD (Copilot Left)", "Contrast Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_OUTBOARD.ToString("d"), "797", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_40.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_41.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0") }, "MFD (Copilot Left)", "Backlight Switch", "%0.1f"));

            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_1.ToString("d"), "834", "MFD (Copilot Right)", "T1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_2.ToString("d"), "835", "MFD (Copilot Right)", "T2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_3.ToString("d"), "836", "MFD (Copilot Right)", "T3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_4.ToString("d"), "837", "MFD (Copilot Right)", "T4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_5.ToString("d"), "838", "MFD (Copilot Right)", "T5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_6.ToString("d"), "839", "MFD (Copilot Right)", "T6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_7.ToString("d"), "840", "MFD (Copilot Right)", "T7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_8.ToString("d"), "841", "MFD (Copilot Right)", "R1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_9.ToString("d"), "842", "MFD (Copilot Right)", "R2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_10.ToString("d"), "843", "MFD (Copilot Right)", "R3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_11.ToString("d"), "844", "MFD (Copilot Right)", "R4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_12.ToString("d"), "845", "MFD (Copilot Right)", "R5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_13.ToString("d"), "846", "MFD (Copilot Right)", "R6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_14.ToString("d"), "847", "MFD (Copilot Right)", "R7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_15.ToString("d"), "848", "MFD (Copilot Right)", "R8"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_16.ToString("d"), "849", "MFD (Copilot Right)", "R9"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_17.ToString("d"), "850", "MFD (Copilot Right)", "B1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_18.ToString("d"), "851", "MFD (Copilot Right)", "B2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_19.ToString("d"), "852", "MFD (Copilot Right)", "B3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_20.ToString("d"), "853", "MFD (Copilot Right)", "B4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_21.ToString("d"), "854", "MFD (Copilot Right)", "B5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_22.ToString("d"), "855", "MFD (Copilot Right)", "B6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_23.ToString("d"), "856", "MFD (Copilot Right)", "B7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_24.ToString("d"), "857", "MFD (Copilot Right)", "L1"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_25.ToString("d"), "858", "MFD (Copilot Right)", "L2"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_26.ToString("d"), "859", "MFD (Copilot Right)", "L3"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_27.ToString("d"), "860", "MFD (Copilot Right)", "L4"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_28.ToString("d"), "861", "MFD (Copilot Right)", "L5"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_29.ToString("d"), "862", "MFD (Copilot Right)", "L6"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_30.ToString("d"), "863", "MFD (Copilot Right)", "L7"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_31.ToString("d"), "864", "MFD (Copilot Right)", "L8"));
            AddFunction(new PushButton(this, devices.MFD_COPILOT_INBOARD.ToString("d"), Commands.Button.Button_32.ToString("d"), "865", "MFD (Copilot Right)", "L9"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_INBOARD.ToString("d"), "830", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_33.ToString("d"), new string[] { "OFF", "NVG", "NORM" }, "%0.1f"), "MFD (Copilot Right)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_INBOARD.ToString("d"), "831", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_34.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_35.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0") }, "MFD (Copilot Right)", "Brightness Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_INBOARD.ToString("d"), "832", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_37.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_38.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0") }, "MFD (Copilot Right)", "Contrast Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_COPILOT_INBOARD.ToString("d"), "833", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_40.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_41.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0") }, "MFD (Copilot Right)", "Backlight Switch", "%0.1f"));

            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_1.ToString("d"), "870", "MFD (Center)", "T1"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_2.ToString("d"), "871", "MFD (Center)", "T2"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_3.ToString("d"), "872", "MFD (Center)", "T3"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_4.ToString("d"), "873", "MFD (Center)", "T4"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_5.ToString("d"), "874", "MFD (Center)", "T5"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_6.ToString("d"), "875", "MFD (Center)", "T6"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_7.ToString("d"), "876", "MFD (Center)", "T7"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_8.ToString("d"), "877", "MFD (Center)", "R1"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_9.ToString("d"), "878", "MFD (Center)", "R2"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_10.ToString("d"), "879", "MFD (Center)", "R3"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_11.ToString("d"), "880", "MFD (Center)", "R4"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_12.ToString("d"), "881", "MFD (Center)", "R5"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_13.ToString("d"), "882", "MFD (Center)", "R6"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_14.ToString("d"), "883", "MFD (Center)", "R7"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_15.ToString("d"), "884", "MFD (Center)", "R8"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_16.ToString("d"), "885", "MFD (Center)", "R9"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_17.ToString("d"), "886", "MFD (Center)", "B1"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_18.ToString("d"), "887", "MFD (Center)", "B2"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_19.ToString("d"), "888", "MFD (Center)", "B3"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_20.ToString("d"), "889", "MFD (Center)", "B4"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_21.ToString("d"), "890", "MFD (Center)", "B5"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_22.ToString("d"), "891", "MFD (Center)", "B6"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_23.ToString("d"), "892", "MFD (Center)", "B7"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_24.ToString("d"), "893", "MFD (Center)", "L1"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_25.ToString("d"), "894", "MFD (Center)", "L2"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_26.ToString("d"), "895", "MFD (Center)", "L3"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_27.ToString("d"), "896", "MFD (Center)", "L4"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_28.ToString("d"), "897", "MFD (Center)", "L5"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_29.ToString("d"), "898", "MFD (Center)", "L6"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_30.ToString("d"), "899", "MFD (Center)", "L7"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_31.ToString("d"), "900", "MFD (Center)", "L8"));
            AddFunction(new PushButton(this, devices.MFD_CENTER.ToString("d"), Commands.Button.Button_32.ToString("d"), "901", "MFD (Center)", "L9"));
            AddFunction(new Switch(this, devices.MFD_CENTER.ToString("d"), "866", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_33.ToString("d"), new string[] { "OFF", "NVG", "NORM" }, "%0.1f"), "MFD (Center)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_CENTER.ToString("d"), "867", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_34.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_35.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0") }, "MFD (Center)", "Brightness Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_CENTER.ToString("d"), "868", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_37.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_38.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0") }, "MFD (Center)", "Contrast Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_CENTER.ToString("d"), "869", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_40.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_41.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0") }, "MFD (Center)", "Backlight Switch", "%0.1f"));

            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_1.ToString("d"), "906", "MFD (Pilot Left)", "T1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_2.ToString("d"), "907", "MFD (Pilot Left)", "T2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_3.ToString("d"), "908", "MFD (Pilot Left)", "T3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_4.ToString("d"), "909", "MFD (Pilot Left)", "T4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_5.ToString("d"), "910", "MFD (Pilot Left)", "T5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_6.ToString("d"), "911", "MFD (Pilot Left)", "T6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_7.ToString("d"), "912", "MFD (Pilot Left)", "T7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_8.ToString("d"), "913", "MFD (Pilot Left)", "R1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_9.ToString("d"), "914", "MFD (Pilot Left)", "R2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_10.ToString("d"), "915", "MFD (Pilot Left)", "R3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_11.ToString("d"), "916", "MFD (Pilot Left)", "R4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_12.ToString("d"), "917", "MFD (Pilot Left)", "R5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_13.ToString("d"), "918", "MFD (Pilot Left)", "R6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_14.ToString("d"), "919", "MFD (Pilot Left)", "R7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_15.ToString("d"), "920", "MFD (Pilot Left)", "R8"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_16.ToString("d"), "921", "MFD (Pilot Left)", "R9"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_17.ToString("d"), "922", "MFD (Pilot Left)", "B1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_18.ToString("d"), "923", "MFD (Pilot Left)", "B2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_19.ToString("d"), "924", "MFD (Pilot Left)", "B3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_20.ToString("d"), "925", "MFD (Pilot Left)", "B4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_21.ToString("d"), "926", "MFD (Pilot Left)", "B5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_22.ToString("d"), "927", "MFD (Pilot Left)", "B6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_23.ToString("d"), "928", "MFD (Pilot Left)", "B7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_24.ToString("d"), "929", "MFD (Pilot Left)", "L1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_25.ToString("d"), "930", "MFD (Pilot Left)", "L2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_26.ToString("d"), "931", "MFD (Pilot Left)", "L3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_27.ToString("d"), "932", "MFD (Pilot Left)", "L4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_28.ToString("d"), "933", "MFD (Pilot Left)", "L5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_29.ToString("d"), "934", "MFD (Pilot Left)", "L6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_30.ToString("d"), "935", "MFD (Pilot Left)", "L7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_31.ToString("d"), "936", "MFD (Pilot Left)", "L8"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_INBOARD.ToString("d"), Commands.Button.Button_32.ToString("d"), "937", "MFD (Pilot Left)", "L9"));
            AddFunction(new Switch(this, devices.MFD_PILOT_INBOARD.ToString("d"), "902", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_33.ToString("d"), new string[] { "OFF", "NVG", "NORM" }, "%0.1f"), "MFD (Pilot Left)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_PILOT_INBOARD.ToString("d"), "903", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_34.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_35.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0") }, "MFD (Pilot Left)", "Brightness Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_PILOT_INBOARD.ToString("d"), "904", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_37.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_38.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0") }, "MFD (Pilot Left)", "Contrast Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_PILOT_INBOARD.ToString("d"), "905", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_40.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_41.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0") }, "MFD (Pilot Left)", "Backlight Switch", "%0.1f"));

            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_1.ToString("d"), "942", "MFD (Pilot Right)", "T1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_2.ToString("d"), "943", "MFD (Pilot Right)", "T2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_3.ToString("d"), "944", "MFD (Pilot Right)", "T3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_4.ToString("d"), "945", "MFD (Pilot Right)", "T4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_5.ToString("d"), "946", "MFD (Pilot Right)", "T5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_6.ToString("d"), "947", "MFD (Pilot Right)", "T6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_7.ToString("d"), "948", "MFD (Pilot Right)", "T7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_8.ToString("d"), "949", "MFD (Pilot Right)", "R1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_9.ToString("d"), "950", "MFD (Pilot Right)", "R2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_10.ToString("d"), "951", "MFD (Pilot Right)", "R3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_11.ToString("d"), "952", "MFD (Pilot Right)", "R4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_12.ToString("d"), "953", "MFD (Pilot Right)", "R5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_13.ToString("d"), "954", "MFD (Pilot Right)", "R6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_14.ToString("d"), "955", "MFD (Pilot Right)", "R7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_15.ToString("d"), "956", "MFD (Pilot Right)", "R8"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_16.ToString("d"), "957", "MFD (Pilot Right)", "R9"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_17.ToString("d"), "958", "MFD (Pilot Right)", "B1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_18.ToString("d"), "959", "MFD (Pilot Right)", "B2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_19.ToString("d"), "960", "MFD (Pilot Right)", "B3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_20.ToString("d"), "961", "MFD (Pilot Right)", "B4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_21.ToString("d"), "962", "MFD (Pilot Right)", "B5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_22.ToString("d"), "963", "MFD (Pilot Right)", "B6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_23.ToString("d"), "964", "MFD (Pilot Right)", "B7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_24.ToString("d"), "965", "MFD (Pilot Right)", "L1"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_25.ToString("d"), "966", "MFD (Pilot Right)", "L2"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_26.ToString("d"), "967", "MFD (Pilot Right)", "L3"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_27.ToString("d"), "968", "MFD (Pilot Right)", "L4"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_28.ToString("d"), "969", "MFD (Pilot Right)", "L5"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_29.ToString("d"), "970", "MFD (Pilot Right)", "L6"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_30.ToString("d"), "971", "MFD (Pilot Right)", "L7"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_31.ToString("d"), "972", "MFD (Pilot Right)", "L8"));
            AddFunction(new PushButton(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), Commands.Button.Button_32.ToString("d"), "973", "MFD (Pilot Right)", "L9"));
            AddFunction(new Switch(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), "938", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_33.ToString("d"), new string[] { "OFF", "NVG", "NORM" }, "%0.1f"), "MFD (Pilot Right)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), "939", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_34.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_35.ToString("d"), Commands.Button.Button_36.ToString("d"), "0.0", "0.0") }, "MFD (Pilot Right)", "Brightness Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), "940", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_37.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_38.ToString("d"), Commands.Button.Button_39.ToString("d"), "0.0", "0.0") }, "MFD (Pilot Right)", "Contrast Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_PILOT_OUTBOARD.ToString("d"), "941", new SwitchPosition[] { new SwitchPosition("1.0", "Up", Commands.Button.Button_40.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", Commands.Button.Button_41.ToString("d"), Commands.Button.Button_42.ToString("d"), "0.0", "0.0") }, "MFD (Pilot Right)", "Backlight Switch", "%0.1f"));

            #endregion
            #region Interphones
            int[,] startingArgs = { { 591, 624, 657, 1066, 1099, 690 }, { (int)devices.COMM_PANEL_RIGHT, (int)devices.COMM_PANEL_LEFT, (int)devices.COMM_PANEL_TROOP_COMMANDER, (int)devices.COMM_PANEL_LH_GUNNER, (int)devices.COMM_PANEL_RH_GUNNER, (int)devices.COMM_PANEL_AFT_ENGINEER } };
            for (int i = 0; i < 6; i++)
            {
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_1.ToString("d"), $"{startingArgs[0, i]}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R1 FM1 Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_6.ToString("d"), $"{startingArgs[0, i] + 2}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R2 UHF Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_11.ToString("d"), $"{startingArgs[0, i] + 4}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R3 VHF Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_16.ToString("d"), $"{startingArgs[0, i] + 6}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R4 HF Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_21.ToString("d"), $"{startingArgs[0, i] + 8}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R5 FM2 Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_26.ToString("d"), $"{startingArgs[0, i] + 10}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R6 Spare Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_31.ToString("d"), $"{startingArgs[0, i] + 12}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "R7 RWR Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_36.ToString("d"), $"{startingArgs[0, i] + 14}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "N1 VOR/ILS Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_41.ToString("d"), $"{startingArgs[0, i] + 16}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "N2 TACAN Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_46.ToString("d"), $"{startingArgs[0, i] + 18}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "N3 ADF Pull Button Knob"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_51.ToString("d"), $"{startingArgs[0, i] + 20}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "N4 MB/CWS Pull Button Knob"));
                AddFunction(new Switch(this, $"{startingArgs[1, i]}", $"{startingArgs[0, i] + 22}", SwitchPositions.Create(11, 0.0d, 0.05d, Commands.Button.Button_63.ToString("d"), new string[] { "Tx", "1", "2", "3", "4", "5", "6", "7", "RMT", "BU", "PVT" }, "%0.2f"), $"Interphone ICS{i + 1}", "Tx Switch", "%0.2f"));
                AddFunction(new Axis(this, $"{startingArgs[1, i]}", Commands.Button.Button_56.ToString("d"), $"{startingArgs[0, i] + 23}", 0.1d, 0d, 1d, $"Interphone ICS{i + 1}", "Master Volume Knob"));
                AddFunction(new PushButton(this, $"{startingArgs[1, i]}", Commands.Button.Button_59.ToString("d"), $"{startingArgs[0, i] + 24}", $"Interphone ICS{i + 1}", "ICS Button"));
                AddFunction(new FlagValue(this, $"{startingArgs[0, i] + 29}", $"Interphone ICS{i + 1}", "ICS Indicator", ""));
                AddFunction(new PushButton(this, $"{startingArgs[1, i]}", Commands.Button.Button_60.ToString("d"), $"{startingArgs[0, i] + 25}", $"Interphone ICS{i + 1}", "VOX Button"));
                AddFunction(new FlagValue(this, $"{startingArgs[0, i] + 30}", $"Interphone ICS{i + 1}", "VOX Indicator", ""));
                AddFunction(new PushButton(this, $"{startingArgs[1, i]}", Commands.Button.Button_61.ToString("d"), $"{startingArgs[0, i] + 26}", $"Interphone ICS{i + 1}", "Hot Mic Button"));
                AddFunction(new FlagValue(this, $"{startingArgs[0, i] + 31}", $"Interphone ICS{i + 1}", "Hot Mic Indicator", ""));
                AddFunction(new PushButton(this, $"{startingArgs[1, i]}", Commands.Button.Button_62.ToString("d"), $"{startingArgs[0, i] + 27}", $"Interphone ICS{i + 1}", "Call Button"));
                AddFunction(new FlagValue(this, $"{startingArgs[0, i] + 32}", $"Interphone ICS{i + 1}", "Call Indicator", ""));
                AddFunction(new FlagValue(this, $"{startingArgs[0, i] + 28}", $"Interphone ICS{i + 1}", "ICU Indicator", ""));
            }

            #endregion
            #region Instruments
            #region Free Air Temp
            AddFunction(new ScaledNetworkValue(this, "1211", new CalibrationPointCollectionDouble()
                {
                new CalibrationPointDouble(0.0d, -70d),
                new CalibrationPointDouble(0.152d, -50d),
                new CalibrationPointDouble(0.314d, -30d),
                new CalibrationPointDouble(0.482d, -10d),
                new CalibrationPointDouble(0.567d, 0d),
                new CalibrationPointDouble(0.651d, 10d),
                new CalibrationPointDouble(0.738d, 20d),
                new CalibrationPointDouble(0.825d, 30d),
                new CalibrationPointDouble(1.0d  , 40d),
                }, "Temperature Gauge", "Free Air Temperature", "Current Free Air Temperature.", "", BindingValueUnits.Celsius, "%.4f"));
            #endregion
            #region Rad Alt

            //-- APN-209 RADAR Altimeter
            AddFunction(new Axis(this, devices.APN_209.ToString("d"), Commands.Button.Button_2.ToString("d"), "1193", 0.1d, 0d, 1d, "RADAR Alt", "Low Altitude Set"));
            AddFunction(new Axis(this, devices.APN_209.ToString("d"), Commands.Button.Button_1.ToString("d"), "1194", 0.1d, 0d, 1d, "RADAR Alt", "High Altitude Set"));
            AddFunction(new PushButton(this, devices.APN_209.ToString("d"), Commands.Button.Button_3.ToString("d"), "1195", "RADAR Alt", "Test"));
            AddFunction(new ScaledNetworkValue(this, "1191", 360d, "RADAR Alt", "Altitude Needle", "Position of the altitude needle", "Rotational position of the needle 0-360", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "1196", new CalibrationPointCollectionDouble()
                {
                new CalibrationPointDouble(-0.02d,    0.97d * 360d ),
                new CalibrationPointDouble(-0.01d,    0.99d * 360d ),
                new CalibrationPointDouble(-0.0001d,  1.0d * 360d  ),
                new CalibrationPointDouble(0.0d,      0.0d * 360d  ),
                new CalibrationPointDouble(0.744d,    0.744d * 360d),
                }, "RADAR Alt", "Low Altitude Bug Marker", "Position of the indicator showing the low altitude", "Rotational position of the marker 0-360", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "1197", 360d, "RADAR Alt", "High Altitude Bug Marker", "Position of the indicator showing the high altitude", "Rotational position of the marker 0-360", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "1199", "RADAR Alt", "Low flag", ""));
            AddFunction(new FlagValue(this, "1192", "RADAR Alt", "High flag", ""));
            AddFunction(new FlagValue(this, "1198", "RADAR Alt", "Off flag", ""));
            AddFunction(new RADARAltimeter(this, "2055", "RADAR Alt", "RADAR altitude above ground in feet for digital display."));
            // Digits are 1200 - 1203
            #endregion
            #endregion

            #region Uncategorised
            // RegEx
            // elements\[\x22(?'element'.*)\x22\]\s*\=\s*(?'function'[a-zA-z0-9_]*)\((?'function_args'.*)\){0,1}\,\s*devices\.(?'device'.*)\,.*device_commands\.(?'command'[a-zA-Z0-9_]*)\,\s*(?'argId'\d{1,4})\,{0,1}(?'optional_args'.*)\).*
            // \t\tAddFunction(new ${function}(this, devices.${device}.ToString("d"), Commands.Button.${command}.ToString("d"), \x22${argId}\x22, "${device}", "${element}","%.1f"));  // $&\n
            #region Commented Functions
            //*AddFunction(new multiposition_switch_limited(this, devices.GRIPS.ToString("d"), Commands.Button.Button_16.ToString("d"), "1271", "GRIPS", "C1_XMIT","%.1f"));  // elements["C1_XMIT"] =           multiposition_switch_limited({0}, "", devices.GRIPS,                    device_commands.Button_16,  1271, 3, 0.5, nil, nil, {{SOUND_SW03_OFF, SOUND_SW03_ON}})
            //*AddFunction(new animated_tumbler(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_9.ToString("d"), "1293", "External Cargo Equipment", "C2_HOOKREL_COVER","%.1f"));  // elements["C2_HOOKREL_COVER"] = animated_tumbler({1}, "", devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_9,   1293, 8.0, false, {{SOUND_SW01_OPEN, SOUND_SW01_CLOSE}})
            //*AddFunction(new animated_tumbler(this, devices.GRIPS.ToString("d"), Commands.Button.Button_2.ToString("d"), "1295", "GRIPS", "T1_IRWHT","%.1f"));  // elements["T1_IRWHT"] =            animated_tumbler({0}, "", devices.GRIPS, device_commands.Button_2,  1295)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_35.ToString("d"), "579", "Canted Console", "AFCS_SYSTEM","%.1f"));  // elements["AFCS_SYSTEM"] = animated_multiposition_switch_limited({0, 1}, _("Cockpit.CH47.AFCS.system_kb"),  devices.CANTED_CONSOLE, device_commands.Button_35, 579, nil, 5, 0.1, false, nil, {{SOUND_SW04}})
            //*AddFunction(new MultiFunctionKnob(this, devices.CDU_RIGHT, device_commands.Button_76.ToString("d"), Commands.Button.Button_78.ToString("d"), "986", "CDU_RIGHT, device_commands.Button_76", "MFK2_KNOB_INNER","%.1f"));  // elements["MFK2_KNOB_INNER"]  = MultiFunctionKnob({0, 1}, _("Cockpit.CH47.MFK_INNER_RIGHT"),	devices.CDU_RIGHT, device_commands.Button_76,	device_commands.Button_78,		986, 987)
            //*AddFunction(new MultiFunctionKnob(this, devices.CDU_LEFT, device_commands.Button_76.ToString("d"), Commands.Button.Button_78.ToString("d"), "983", "CDU_LEFT, device_commands.Button_76", "MFK1_KNOB_INNER","%.1f"));  // elements["MFK1_KNOB_INNER"]  = MultiFunctionKnob({0, 1}, _("Cockpit.CH47.MFK_INNER_LEFT"),	devices.CDU_LEFT, device_commands.Button_76,	device_commands.Button_78,		983, 984)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.EMERGENCY_PANEL.ToString("d"), Commands.Button.Button_3.ToString("d"), "585", "EMERGENCY PANEL", "EAUX_EMER","%.1f"));  // elements["EAUX_EMER"] =       animated_multiposition_switch_limited({0, 1}, _("Cockpit.CH47.EAUX.gd_emer_sw"), devices.EMERGENCY_PANEL, device_commands.Button_3, 585, nil, 3, 0.1)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.EMERGENCY_PANEL.ToString("d"), Commands.Button.Button_1.ToString("d"), "583", "EMERGENCY PANEL", "EAUX_RADIO_MODE","%.1f"));  // elements["EAUX_RADIO_MODE"] = animated_multiposition_switch_limited({0, 1}, _("Cockpit.CH47.EAUX.radio_sw"),   devices.EMERGENCY_PANEL, device_commands.Button_1, 583, nil, 3, 0.1)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_5.ToString("d"), "587", "Center Console", "STEER_SWIVEL","%.1f"));  // elements["STEER_SWIVEL"] = animated_multiposition_switch_limited_small({0, 1}, _("Cockpit.CH47.SCP.SWIVEL_SW"), devices.CENTRAL_CONSOLE, device_commands.Button_5, 587, nil, 3, 0.1)
            //*AddFunction(new multiposition_switch_tumb(this, devices.ARC_186.ToString("d"), Commands.Button.Button_3.ToString("d"), "1224", "ARC_186", "ARC186_MODE","%.1f"));  // elements["ARC186_MODE"] 			= multiposition_switch_tumb({0, 1},_("Frequency Mode Dial") , devices.ARC_186, device_commands.Button_3, 1224, 3, 0.1)
            //*AddFunction(new multiposition_switch_limited(this, devices.ARC_186.ToString("d"), Commands.Button.Button_4.ToString("d"), "1221", "ARC_186", "ARC186_FREQEMER","%.1f"));  // elements["ARC186_FREQEMER"] 		= multiposition_switch_limited({0, 1},_("Frequency Selection Dial") , devices.ARC_186, device_commands.Button_4, 1221, 4, 0.1)
            //*AddFunction(new multiposition_switch_limited_1_side(this, devices.ARC_186.ToString("d"), Commands.Button.Button_1.ToString("d"), "1223", "ARC_186", "ARC186_PRESET","%.1f"));  // elements["ARC186_PRESET"] = multiposition_switch_limited_1_side({0, 1},_("Preset Channel Selector"), devices.ARC_186, device_commands.Button_1, 1223, 20, -0.01000001, nil, 0.1,  {-135, 45}, {0, -135} )
            //*AddFunction(new default_tumb_button(this, devices.ARC_186, device_commands.Button_7.ToString("d"), Commands.Button.Button_8.ToString("d"), "1220", "ARC_186,device_commands.Button_7", "ARC186_SQUELCH","%.1f"));  // elements["ARC186_SQUELCH"] = default_tumb_button({0, 1},_("Squelch / TONE"),devices.ARC_186,device_commands.Button_7, device_commands.Button_8, 1220)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_21.ToString("d"), "493", "OVERHEAD CONSOLE", "OCHC_FLT_CONTR","%.1f"));  // elements["OCHC_FLT_CONTR"] =       animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCHC.FLT_CONTR_SW"),    devices.OVERHEAD_CONSOLE, device_commands.Button_21, 493, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_32.ToString("d"), "498", "OVERHEAD CONSOLE", "OCHC_RAMP","%.1f"));  // elements["OCHC_RAMP"] =            animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCHC.RAMP_SW"),         devices.OVERHEAD_CONSOLE, device_commands.Button_32, 498, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_37.ToString("d"), "500", "OVERHEAD CONSOLE", "OCHC_RAMP_EMER","%.1f"));  // elements["OCHC_RAMP_EMER"] = animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCHC.RAMP_EMER_SW"),    devices.OVERHEAD_CONSOLE, device_commands.Button_37, 500, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_58.ToString("d"), "509", "OVERHEAD CONSOLE", "OCLP_EMER","%.1f"));  // elements["OCLP_EMER"] = animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCLP.EMER_SW"),      devices.OVERHEAD_CONSOLE, device_commands.Button_58, 509, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_61.ToString("d"), "510", "OVERHEAD CONSOLE", "OCLP_DOME","%.1f"));  // elements["OCLP_DOME"] =       animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.DOME_SW"),      devices.OVERHEAD_CONSOLE, device_commands.Button_61, 510, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_67.ToString("d"), "512", "OVERHEAD CONSOLE", "OCLP_ANTICOL_MODE","%.1f"));  // elements["OCLP_ANTICOL_MODE"] = animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.ANTICOL_KB"),    devices.OVERHEAD_CONSOLE, device_commands.Button_67, 512, nil, 7, 0.1, false, nil, {{SOUND_SW04}})
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_70.ToString("d"), "513", "OVERHEAD CONSOLE", "OCLP_IR_PATTERN","%.1f"));  // elements["OCLP_IR_PATTERN"] =   animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.IR_PATTERN_KB"), devices.OVERHEAD_CONSOLE, device_commands.Button_70, 513, nil, 5, 0.1, false, nil, {{SOUND_SW04}})
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_76.ToString("d"), "515", "OVERHEAD CONSOLE", "OCLP_FORM","%.1f"));  // elements["OCLP_FORM"] =   animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCLP.FORM_SW"),       devices.OVERHEAD_CONSOLE, device_commands.Button_76, 515, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_84.ToString("d"), "518", "OVERHEAD CONSOLE", "OCLP_POSN","%.1f"));  // elements["OCLP_POSN"] =   animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCLP.POSN_SW"),       devices.OVERHEAD_CONSOLE, device_commands.Button_84, 518, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_91.ToString("d"), "521", "OVERHEAD CONSOLE", "OCLP_MODE","%.1f"));  // elements["OCLP_MODE"] = animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCLP.MODE_SW"),       devices.OVERHEAD_CONSOLE, device_commands.Button_91, 521, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_131.ToString("d"), "541", "OVERHEAD CONSOLE", "OCTW_LT","%.1f"));  // elements["OCTW_LT"] =       animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCTW.LT_SW"),        devices.OVERHEAD_CONSOLE, device_commands.Button_131, 541, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_139.ToString("d"), "544", "OVERHEAD CONSOLE", "OCTW_HTR_MODE","%.1f"));  // elements["OCTW_HTR_MODE"] = animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCTW.HTR_MODE_SW"),  devices.OVERHEAD_CONSOLE, device_commands.Button_139, 544, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_143.ToString("d"), "756", "OVERHEAD CONSOLE", "OCTW_WIPER","%.1f"));  // elements["OCTW_WIPER"] =          animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCTW.WIPER_KB"),     devices.OVERHEAD_CONSOLE, device_commands.Button_143, 756, nil, 5, 0.1, false, nil, {{SOUND_SW04}})
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_156.ToString("d"), "551", "OVERHEAD CONSOLE", "OCCH_HOIST_MASTER","%.1f"));  // elements["OCCH_HOIST_MASTER"] = animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCHH.HOIST_SW"),       devices.OVERHEAD_CONSOLE, device_commands.Button_156, 551, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_159.ToString("d"), "552", "OVERHEAD CONSOLE", "OCCH_HOOK_MASTER","%.1f"));  // elements["OCCH_HOOK_MASTER"] =  animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCHH.HOOK_SW"),        devices.OVERHEAD_CONSOLE, device_commands.Button_159, 552, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_162.ToString("d"), "553", "OVERHEAD CONSOLE", "OCCH_HOOK_SELECTOR","%.1f"));  // elements["OCCH_HOOK_SELECTOR"] =      animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCHH.HOOK_KB"),        devices.OVERHEAD_CONSOLE, device_commands.Button_162, 553, nil, 5, 0.1, false, nil, {{SOUND_SW04}})
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_169.ToString("d"), "556", "OVERHEAD CONSOLE", "OCEP_GEN_1","%.1f"));  // elements["OCEP_GEN_1"] =     animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCEP.GEN_1_SW"),   devices.OVERHEAD_CONSOLE, device_commands.Button_169, 556, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_172.ToString("d"), "557", "OVERHEAD CONSOLE", "OCEP_GEN_2","%.1f"));  // elements["OCEP_GEN_2"] =     animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCEP.GEN_2_SW"),   devices.OVERHEAD_CONSOLE, device_commands.Button_172, 557, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_175.ToString("d"), "558", "OVERHEAD CONSOLE", "OCEP_GEN_APU","%.1f"));  // elements["OCEP_GEN_APU"] =   animated_multiposition_switch_limited({0, 1, 2}, _("Cockpit.CH47.OCEP.GEN_APU_SW"), devices.OVERHEAD_CONSOLE, device_commands.Button_175, 558, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited_small(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_181.ToString("d"), "560", "OVERHEAD CONSOLE", "OCEP_APU","%.1f"));  // elements["OCEP_APU"] = animated_multiposition_switch_limited_small({0, 1, 2}, _("Cockpit.CH47.OCEP.APU_SW"),     devices.OVERHEAD_CONSOLE, device_commands.Button_181, 560, nil, 3, 0.1)
            //*AddFunction(new animated_tumbler(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_13.ToString("d"), "1256", "External Cargo Equipment", "WGRIP_HOOK_COVER","%.1f"));  // elements["WGRIP_HOOK_COVER"] =         animated_tumbler({2}, "",                               devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_13, 1256, 8.0, false, {{SOUND_SW01_OPEN, SOUND_SW01_CLOSE}})
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_17.ToString("d"), "1269", "External Cargo Equipment", "HOP_ARM","%.1f"));  // elements["HOP_ARM"] = animated_multiposition_switch_limited({2}, _("Cockpit.CH47.HOP_ARM"),        devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_17, 1269, nil, 3, 0.1, false)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.MAINTENANCE_PANEL.ToString("d"), Commands.Button.Button_3.ToString("d"), "1035", "Maintenance Panel", "MP_PWR_ASSR","%.1f"));  // elements["MP_PWR_ASSR"] = animated_multiposition_switch_limited({2}, _("Cockpit.CH47.MP.PWR_ASSR"),    devices.MAINTENANCE_PANEL, device_commands.Button_3, 1035, nil, 3, 0.1, false)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.MAINTENANCE_PANEL.ToString("d"), Commands.Button.Button_7.ToString("d"), "1036", "Maintenance Panel", "MP_LTG","%.1f"));  // elements["MP_LTG"] =      animated_multiposition_switch_limited({2}, _("Cockpit.CH47.MP.LTG"),         devices.MAINTENANCE_PANEL, device_commands.Button_7, 1036, nil, 3, 0.1, true)
            //*AddFunction(new animated_multiposition_switch_limited(this, devices.AFT_WORKSTATION.ToString("d"), Commands.Button.Button_1.ToString("d"), "1399", "AFT WORKSTATION", "INTRLTG_CABIN_SW","%.1f"));  // elements["INTRLTG_CABIN_SW"] = animated_multiposition_switch_limited({2}, _("Cockpit.CH47.LCP.INST_SW"), devices.AFT_WORKSTATION, device_commands.Button_1, 1399, nil, 3, 0.1, false)
            //*AddFunction(new multiposition_switch_limited(this, devices.GRIPS.ToString("d"), Commands.Button.Button_96.ToString("d"), "1283", "GRIPS", "C2_XMIT","%.1f"));  // elements["C2_XMIT"] =           multiposition_switch_limited({1}, "", devices.GRIPS,                    device_commands.Button_96,  1283, 3, 0.5, nil, nil, {{SOUND_SW03_OFF, SOUND_SW03_ON}})
            //*AddFunction(new animated_tumbler(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_5.ToString("d"), "1281", "External Cargo Equipment", "C1_HOOKREL_COVER","%.1f"));  // elements["C1_HOOKREL_COVER"] = animated_tumbler({0}, "", devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_5,   1281, 8.0, false, {{SOUND_SW01_OPEN, SOUND_SW01_CLOSE}})
            //*AddFunction(new animated_tumbler(this, devices.GRIPS.ToString("d"), Commands.Button.Button_82.ToString("d"), "1308", "GRIPS", "T2_IRWHT","%.1f"));  // elements["T2_IRWHT"] =            animated_tumbler({1}, "", devices.GRIPS, device_commands.Button_82, 1308)
            //*AddFunction(new rocker_and_axis_limited(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_1.ToString("d"), "731", "Canted Console", "MAIN_ENG1_FIRE_HDL","%.1f"));  // elements["MAIN_ENG1_FIRE_HDL"] = rocker_and_axis_limited({0, 1}, _("Cockpit.CH47.fire_pull_hdl"),     devices.CANTED_CONSOLE, device_commands.Button_1,  731, nil, 0.5)
            //*AddFunction(new rocker_and_axis_limited(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_5.ToString("d"), "735", "Canted Console", "MAIN_ENG2_FIRE_HDL","%.1f"));  // elements["MAIN_ENG2_FIRE_HDL"] = rocker_and_axis_limited({0, 1}, _("Cockpit.CH47.fire_pull_hdl"),     devices.CANTED_CONSOLE, device_commands.Button_5,  735, nil, 0.5)
            //*AddFunction(new button_and_axis_limited(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_9.ToString("d"), "724", "Canted Console", "MAIN_BAT_LOW","%.1f"));  // elements["MAIN_BAT_LOW"] =       button_and_axis_limited({0, 1}, _("Cockpit.Generic.batt_low_light"), devices.CANTED_CONSOLE, device_commands.Button_9,  724, 0.1)
            //*AddFunction(new button_and_axis_limited(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_152.ToString("d"), "549", "OVERHEAD CONSOLE", "OCCH_HOIST_CONTROL_2","%.1f"));  // elements["OCCH_HOIST_CONTROL_2"] =                  button_and_axis_limited({0, 1, 2}, _("Cockpit.CH47.OCHH.HOIST_KB"),       devices.OVERHEAD_CONSOLE, device_commands.Button_152, 549, 0.5)
            //*AddFunction(new animated_pull_handle(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_23.ToString("d"), "1320", "External Cargo Equipment", "HOP_DOOR1","%.1f"));  // elements["HOP_DOOR1"] =                animated_pull_handle({2}, _("Cockpit.CH47.TRAP_DOOR_LOCK"), devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_23, 1320, 0.8, {{SOUND_NOSOUND, SOUND_NOSOUND}})
            //*AddFunction(new multiposition_switch_cyclic(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_253.ToString("d"), "1463", "OVERHEAD CONSOLE", "OCLP_PDP2SL_LOCK","%.1f"));  // elements["OCLP_PDP2SL_LOCK"] = multiposition_switch_cyclic({0},       _("Cockpit.CH47.OCLP.SL_LOCK"), devices.OVERHEAD_CONSOLE, device_commands.Button_253, 1463, 2, 1, nil, 0, {{SOUND_NOSOUND}})
            #endregion
            #region Grips
            AddFunction(new Switch(this, devices.GRIPS.ToString("d"), "1271", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_16.ToString("d"), new string[] { "1", "2", "3" }, "%0.1f"), "Pilot Cyclic",  "XMIT", "%0.1f"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_17.ToString("d"), "1272", "Pilot Cyclic",  "ACT", "%.1f"));  // elements["ACT"] =            PushButton({0}, "", devices.GRIPS,                    device_commands.Button_17,  1272)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_18.ToString("d"), "1273", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Cyclic",  "CRSR H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_19.ToString("d"), "1274", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Cyclic",  "CRSR V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_20.ToString("d"), "1275", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Cyclic",  "TRIM H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_21.ToString("d"), "1276", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Cyclic",  "TRIM V", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_22.ToString("d"), "1277", "Pilot Cyclic",  "FDREL", "%.1f"));  // elements["FDREL"] =          PushButton({0}, "", devices.GRIPS,                    device_commands.Button_22,  1277)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_23.ToString("d"), "1278", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Cyclic",  "CMDS", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_24.ToString("d"), "1279", "Pilot Cyclic",  "ACK", "%.1f"));  // elements["ACK"] =            PushButton({0}, "", devices.GRIPS,                    device_commands.Button_24,  1279)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_25.ToString("d"), "1280", "Pilot Cyclic",  "CDREL", "%.1f"));  // elements["CDREL"] =          PushButton({0}, "", devices.GRIPS,                    device_commands.Button_25,  1280)
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_5.ToString("d"), "1281", "1.0", "Pulled", "0.0", "Norm", "Pilot Cyclic", "Hook Release Cover", "%.1f"));
            AddFunction(new PushButton(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_7.ToString("d"), "1282", "Pilot Cyclic", "Hook Release Switch", "%.1f"));  // elements["HOOKREL"] =        PushButton({0}, "", devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_7,   1282, nil, false)

            AddFunction(new Switch(this, devices.GRIPS.ToString("d"), "1283", SwitchPositions.Create(3, 0.0d, 0.5d, Commands.Button.Button_96.ToString("d"), "Posn", "%0.1f"), "Copilot Cyclic", "XMIT", "%0.1f"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_97.ToString("d"), "1284", "Copilot Cyclic", "ACT", "%.1f"));  // elements["ACT"] =            PushButton({1}, "", devices.GRIPS,                    device_commands.Button_97,  1284)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_98.ToString("d"), "1285", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Cyclic",  "CRSR H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_99.ToString("d"), "1286", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Cyclic",  "CRSR V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_100.ToString("d"), "1287", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Cyclic",  "TRIM H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_101.ToString("d"), "1288", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Cyclic",  "TRIM V", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_102.ToString("d"), "1289", "Copilot Cyclic",  "FDREL", "%.1f"));  // elements["FDREL"] =          PushButton({1}, "", devices.GRIPS,                    device_commands.Button_102, 1289)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_103.ToString("d"), "1290", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Cyclic",  "CMDS", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_104.ToString("d"), "1291", "Copilot Cyclic",  "ACK", "%.1f"));  // elements["ACK"] =            PushButton({1}, "", devices.GRIPS,                    device_commands.Button_104, 1291)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_105.ToString("d"), "1292", "Copilot Cyclic",  "CDREL", "%.1f"));  // elements["CDREL"] =          PushButton({1}, "", devices.GRIPS,                    device_commands.Button_105, 1292)
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_9.ToString("d"), "1293", "1.0", "Pulled", "0.0", "Norm", "Copilot Cyclic", "Hook Release Cover", "%.1f"));
            AddFunction(new PushButton(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_11.ToString("d"), "1294", "Copilot Cyclic", "Hook Release Switch", "%.1f"));  // elements["HOOKREL"] =        PushButton({1}, "", devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_11,  1294, nil, false)

            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_7.ToString("d"), "748", "Pilot Thrust Control Lever",  "BRAKE", "%.1f"));  // elements["BRAKE"] =             PushButton({0}, "", devices.GRIPS, device_commands.Button_7,  748)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_1.ToString("d"), "1299", "Pilot Thrust Control Lever",  "MARK", "%.1f"));  // elements["MARK"] =              PushButton({0}, "", devices.GRIPS, device_commands.Button_1,  1299)
            AddFunction(Switch.CreateToggleSwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_2.ToString("d"), "1295", "1.0", "Pulled", "0.0", "Norm", "Pilot Thrust Control Lever",  "IRWHT", "%.1f"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_4.ToString("d"), "1296", "Pilot Thrust Control Lever",  "SRCH", "%.1f"));  // elements["SRCH"] =              PushButton({0}, "", devices.GRIPS, device_commands.Button_4,  1296)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_5.ToString("d"), "1297", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "SRCH H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_6.ToString("d"), "1298", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "SRCH V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_8.ToString("d"), "1300", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "UPDN", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_9.ToString("d"), "1301", "Pilot Thrust Control Lever",  "GA", "%.1f"));  // elements["GA"] =                PushButton({0}, "", devices.GRIPS, device_commands.Button_9,  1301)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_10.ToString("d"), "1302", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "FRQ H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_11.ToString("d"), "1303", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "FRQ V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_12.ToString("d"), "1304", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "HUDMODE H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_13.ToString("d"), "1305", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "HUDMODE V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_14.ToString("d"), "1306", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "DAFCSMODE H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_15.ToString("d"), "1307", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Thrust Control Lever",  "DAFCSMODE V", "%1d"));

            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_87.ToString("d"), "750", "Copilot Thrust Control Lever",  "BRAKE", "%.1f"));  // elements["BRAKE"] =             PushButton({1}, "", devices.GRIPS, device_commands.Button_87, 750)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_81.ToString("d"), "1312", "Copilot Thrust Control Lever",  "MARK", "%.1f"));  // elements["MARK"] =              PushButton({1}, "", devices.GRIPS, device_commands.Button_81, 1312)
            AddFunction(Switch.CreateToggleSwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_82.ToString("d"), "1308", "1.0", "Pulled", "0.0", "Norm", "Copilot Thrust Control Lever",  "IRWHT", "%.1f"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_84.ToString("d"), "1309", "Copilot Thrust Control Lever",  "SRCH", "%.1f"));  // elements["SRCH"] =              PushButton({1}, "", devices.GRIPS, device_commands.Button_84, 1309)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_85.ToString("d"), "1310", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "SRCH H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_86.ToString("d"), "1311", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "SRCH V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_88.ToString("d"), "1313", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "UPDN", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_89.ToString("d"), "1314", "Copilot Thrust Control Lever",  "GA", "%.1f"));  // elements["GA"] =                PushButton({1}, "", devices.GRIPS, device_commands.Button_89, 1314)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_90.ToString("d"), "1315", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "FRQ H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_91.ToString("d"), "1316", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "FRQ V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_92.ToString("d"), "1317", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "HUDMODE H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_93.ToString("d"), "1318", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "HUDMODE V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_94.ToString("d"), "1319", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "DAFCSMODE H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_95.ToString("d"), "1320", "1", "Up", "0", "Middle", "-1", "Down", "Copilot Thrust Control Lever",  "DAFCSMODE V", "%1d"));

            AddFunction(new Axis(this, devices.GRIPS.ToString("d"), Commands.Button.Button_51.ToString("d"), "1414", 0.1d, 0.0d, 1.0d, "Pilot Multi Function Control Unit",  "S1 H"));  // elements["MFCU1_S1_H"] =          axis_limited({0}, "", devices.GRIPS, device_commands.Button_51,  1414)
            AddFunction(new Axis(this, devices.GRIPS.ToString("d"), Commands.Button.Button_55.ToString("d"), "1415", 0.1d, 0.0d, 1.0d, "Pilot Multi Function Control Unit",  "S1 V"));  // elements["MFCU1_S1_V"] =          axis_limited({0}, "", devices.GRIPS, device_commands.Button_55,  1415)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_59.ToString("d"), "1416", "Pilot Multi Function Control Unit",  "S1 Z", "%.1f"));  // elements["MFCU1_S1_Z"] =       PushButton({0}, "", devices.GRIPS, device_commands.Button_59,  1416)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_60.ToString("d"), "1417", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Multi Function Control Unit",  "S2 H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_61.ToString("d"), "1418", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Multi Function Control Unit",  "S2 V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_62.ToString("d"), "1419", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Multi Function Control Unit",  "S3 H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_63.ToString("d"), "1420", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Multi Function Control Unit",  "S3 V", "%1d")); AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_64.ToString("d"), "1421", "Pilot Multi Function Control Unit",  "MFCU1_S4", "%.1f"));  // elements["MFCU1_S4"] =         PushButton({0}, "", devices.GRIPS, device_commands.Button_64,  1421)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_65.ToString("d"), "1422", "Pilot Multi Function Control Unit",  " S5", "%.1f"));  // elements["MFCU1_S5"] =         PushButton({0}, "", devices.GRIPS, device_commands.Button_65,  1422)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_66.ToString("d"), "1423", "Pilot Multi Function Control Unit",  " S6", "%.1f"));  // elements["MFCU1_S6"] =         Puon({0}, "", devices.GRIPS, device_commands.Button_66,  1423)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_67.ToString("d"), "1425", "1", "Up", "0", "Middle", "-1", "Down", "Pilot Multi Function Control Unit",  "R1", "%1d"));

            AddFunction(new Axis(this, devices.GRIPS.ToString("d"), Commands.Button.Button_131.ToString("d"), "1426", 0.1d, 0.0d, 1.0d, "Coplot Multi Function Control Unit",  "S1 H"));  // elements["MFCU2_S1_H"] =          axis_limited({1}, "", devices.GRIPS, device_commands.Button_131, 1426)
            AddFunction(new Axis(this, devices.GRIPS.ToString("d"), Commands.Button.Button_135.ToString("d"), "1427", 0.1d, 0.0d, 1.0d, "Coplot Multi Function Control Unit",  "S1 V"));  // elements["MFCU2_S1_V"] =          axis_limited({1}, "", devices.GRIPS, device_commands.Button_135, 1427)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_139.ToString("d"), "1428", "Coplot Multi Function Control Unit",  "S1 Z", "%.1f"));  // elements["MFCU2_S1_Z"] =       PushButton({1}, "", devices.GRIPS, device_commands.Button_139, 1428)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_140.ToString("d"), "1429", "1", "Up", "0", "Middle", "-1", "Down", "Coplot Multi Function Control Unit",  "S2 H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_141.ToString("d"), "1430", "1", "Up", "0", "Middle", "-1", "Down", "Coplot Multi Function Control Unit",  "S2 V", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_142.ToString("d"), "1431", "1", "Up", "0", "Middle", "-1", "Down", "Coplot Multi Function Control Unit",  "S3 H", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_143.ToString("d"), "1432", "1", "Up", "0", "Middle", "-1", "Down", "Coplot Multi Function Control Unit",  "S3 V", "%1d"));
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_144.ToString("d"), "1433", "Coplot Multi Function Control Unit",  "S4", "%.1f"));  // elements["MFCU2_S4"] =         PushButton({1}, "", devices.GRIPS, device_commands.Button_144, 1433)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_145.ToString("d"), "1434", "Coplot Multi Function Control Unit",  "S5", "%.1f"));  // elements["MFCU2_S5"] =         PushButton({1}, "", devices.GRIPS, device_commands.Button_145, 1434)
            AddFunction(new PushButton(this, devices.GRIPS.ToString("d"), Commands.Button.Button_146.ToString("d"), "1435", "Coplot Multi Function Control Unit",  "S6", "%.1f"));  // elements["MFCU2_S6"] =         PushButton({1}, "", devices.GRIPS, device_commands.Button_146, 1435)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.GRIPS.ToString("d"), Commands.Button.Button_147.ToString("d"), "1437", "1", "Up", "0", "Middle", "-1", "Down", "Coplot Multi Function Control Unit",  "R1", "%1d"));
            #endregion

            AddFunction(new Axis(this, devices.TERTIARY_REFLECTS.ToString("d"), Commands.Button.Button_6.ToString("d"), "1216", 0.1d, 0.0d, 1.0d, "Compass", "Compass Knob"));  // elements["COMPASS_KNOB"] =                  axis_limited({0, 1}, _("Cockpit.Generic.compass_dimmer"), devices.TERTIARY_REFLECTS, device_commands.Button_6, 1216)
            AddFunction(new ScaledNetworkValue(this, "1213", 90d, "Compass", "Tilt", "Angle of Tilt of the Compass", "-90 to 90 degrees", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "1212", 90d, "Compass", "Rotation", "Angle of Rotation of the Compass", "-90 to 90 degrees", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "1214", 360d, "Compass", "Magnetic Heading", "Magnetic Heading in degrees", "0 to 360 degrees", BindingValueUnits.Degrees));

            #region SFDs
            //  The small button and the adjustment knob for the two SFDs seem to be missing from Clickables and Can't find them in Model Viewer.  Hoeefully they will appear in the module soon.
            AddFunction(new NetworkValue(this, "1217", "Canted Console", "Left SFD Slip Ball", "Aircraft Slip Ball Deflection", "-1.0 to +1.0", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "1218", "Canted Console", "Right SFD Slip Ball", "Aircraft Slip Ball Deflection", "-1.0 to +1.0", BindingValueUnits.Numeric));

            #endregion

            AddFunction(Switch.CreateToggleSwitch(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_1.ToString("d"), "731", "1.0", "Pulled", "0.0", "Norm", "Canted Console", "Main Engine 1 Fire Switch", "%.1f"));
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_2.ToString("d"), "732", 0.1d, 0.0d, 1.0d, "Canted Console", "Main Engine 1 Fire Handle"));
            AddFunction(new FlagValue(this, "737", "Canted Console", "FIRE 1 PULL Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_5.ToString("d"), "735", "1.0", "Pulled", "0.0", "Norm", "Canted Console", "Main Engine 2 Fire Switch", "%.1f"));
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_6.ToString("d"), "736", 0.1d, 0.0d, 1.0d, "Canted Console", "Main Engine 2 Fire Handle"));
            AddFunction(new FlagValue(this, "738", "Canted Console", "FIRE 2 PULL Indicator", ""));
            AddFunction(new PushButton(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_9.ToString("d"), "724", "Canted Console", "Main Battery Low Button", "%.1f"));
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_10.ToString("d"), "725", 0.1d, 0.0d, 1.0d, "Canted Console", "Main Battery Low Knob"));
            AddFunction(new FlagValue(this, "723", "Canted Console", "Battery Low Indicator", ""));
            AddFunction(new NetworkValue(this, "1460", "Canted Console", "Inclinometer Tube", "Aircraft angle of inclination", "-1.0 to +1.0", BindingValueUnits.Numeric));

            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_16.ToString("d"), "739", 0.1d, 0.0d, 1.0d, "Canted Console", "Main RadAlt Dimmer"));  // elements["MAIN_RALT_DIMMER"] =              axis_limited({0, 1}, _("Cockpit.CH47.ralt_dimmer"),       devices.CANTED_CONSOLE, device_commands.Button_16, 739)
            AddFunction(new PushButton(this, devices.TERTIARY_REFLECTS.ToString("d"), Commands.Button.Button_1.ToString("d"), "1209", "M880 Chronometer", "Select Button", "%.1f"));  // elements["M880_SEL"] =       button({0, 1}, _("Cockpit.Generic.clock_select_btn"),  devices.TERTIARY_REFLECTS, device_commands.Button_1, 1209, {{SOUND_SW07_OFF, SOUND_SW07_ON}})
            AddFunction(new PushButton(this, devices.TERTIARY_REFLECTS.ToString("d"), Commands.Button.Button_2.ToString("d"), "1210", "M880 Chronometer", "Control Button", "%.1f"));  // elements["M880_CTL"] =       button({0, 1}, _("Cockpit.Generic.clock_control_btn"), devices.TERTIARY_REFLECTS, device_commands.Button_2, 1210, {{SOUND_SW07_OFF, SOUND_SW07_ON}})
            AddFunction(new Axis(this, devices.TERTIARY_REFLECTS.ToString("d"), Commands.Button.Button_3.ToString("d"), "1208", 0.1d, 0.0d, 1.0d, "M880 Chronometer", "Dim Knob"));  // elements["M880_DIM"] = axis_limited({0, 1}, _("Cockpit.Generic.clock_dimmer"),      devices.TERTIARY_REFLECTS, device_commands.Button_3, 1208)
            AddFunction(Switch.CreateToggleSwitch(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_71.ToString("d"), "1331", "1.0", "Pulled", "0.0", "Norm", "Canted Console", "RWR Day/Night Switch", "%.1f"));
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_73.ToString("d"), "1332", 0.1d, 0.0d, 1.0d, "Canted Console", "RWR Brightness Knob"));  // elements["RWR_BRIL_KNOB"] =        axis_limited({0, 1}, _("Cockpit.CH47.RWR.dimmer"),       devices.CANTED_CONSOLE, device_commands.Button_73, 1332)
            AddFunction(new PushButton(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_100.ToString("d"), "741", "Center Console", "Main Parking Brake", "%.1f"));  // elements["MAIN_PARKING_BRAKE"] = button({0}, _("Cockpit.CH47.MAIN.PARKING_LV"), devices.CENTRAL_CONSOLE, device_commands.Button_100, 741, {{SOUND_SW13_PUSH, SOUND_SW13_PULL}})
            AddFunction(new PushButton(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_30.ToString("d"), "574", "AFCS Panel (Canted Console)", "Flight Director Button", "%.1f"));  // elements["AFCS_FLT_DIR"] =                               button({0, 1}, _("Cockpit.CH47.AFCS.flt_dir_sw"), devices.CANTED_CONSOLE, device_commands.Button_30, 574)
            AddFunction(new FlagValue(this, "575", "AFCS Panel (Canted Console)", "Flight Director Indicator", ""));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_31.ToString("d"), "576", "1", "Ext", "0", "Middle", "-1", "Ret", "AFCS Panel (Canted Console)", "AFCS Fwd", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_32.ToString("d"), "577", "1", "Ext", "0", "Middle", "-1", "Ret", "AFCS Panel (Canted Console)", "AFCS Aft", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_33.ToString("d"), "578", "1.0", "Auto", "0.0", "Manual", "AFCS Panel (Canted Console)", "Mode Switch", "%.1f"));
            AddFunction(new Switch(this, devices.CANTED_CONSOLE.ToString("d"), "579", SwitchPositions.Create(5, 0.0d, 0.1d, Commands.Button.Button_35.ToString("d"), new string[] { "OFF", "1", "Both", "2", "OFF" }, "%0.1f"), "AFCS Panel (Canted Console)", "System Select Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_38.ToString("d"), "580", 0.1d, 0.0d, 1.0d, "Canted Console", "CDU 1 Dimmer Knob"));  // elements["CDU_1_DIMMER"] = axis_limited({0, 1}, _("Cockpit.CH47.cdu_dimmer"),    devices.CANTED_CONSOLE, device_commands.Button_38, 580)
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_41.ToString("d"), "581", 0.1d, 0.0d, 1.0d, "Canted Console", "CDU 2 Dimmer Knob"));  // elements["CDU_2_DIMMER"] = axis_limited({0, 1}, _("Cockpit.CH47.cdu_dimmer"),    devices.CANTED_CONSOLE, device_commands.Button_41, 581)
            AddFunction(new PushButton(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_44.ToString("d"), "582", "Canted Console", "Lamps Test Button", "%.1f"));  // elements["LAMPS_TEST"] =         button({0, 1}, _("Cockpit.CH47.lamps_test_sw"), devices.CANTED_CONSOLE, device_commands.Button_44, 582, {{SOUND_SW07_OFF, SOUND_SW07_ON}})


            AddFunction(new Switch(this, devices.EMERGENCY_PANEL.ToString("d"), "583", SwitchPositions.Create(3, 0.2d, -0.1d, Commands.Button.Button_1.ToString("d"), new string[] {"GUARD", "NORM", "MAN" }, "%0.1f"), "Emergency Panel (Canted Console)", "EAUX Radio Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EMERGENCY_PANEL.ToString("d"), Commands.Button.Button_2.ToString("d"), "584", "1.0", "Pulled", "0.0", "Norm", "Emergency Panel (Canted Console)", "Ident Switch", " %.1f"));
            AddFunction(new Switch(this, devices.EMERGENCY_PANEL.ToString("d"), "585", SwitchPositions.Create(3, 0.2d, -0.1d, Commands.Button.Button_3.ToString("d"), new string[] { "EMER", "OFF", "HOLD"}, "%0.1f"), "Emergency Panel (Canted Console)", "EAUX Ground Emergency Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EMERGENCY_PANEL.ToString("d"), Commands.Button.Button_4.ToString("d"), "586", "1.0", "Off", "0.0", "Zero", "Emergency Panel (Canted Console)", "Zeroize Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_25.ToString("d"), "1465", "1", "Up", "0", "Middle", "-1", "Down", "Center Console", "CGI Test Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_26.ToString("d"), "1466", "1.0", "Pulled", "0.0", "Norm", "Center Console", "Backup Radio Select Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_28.ToString("d"), "1467", "1.0", "Pulled", "0.0", "Norm", "Center Console", "Antenna Select Switch", "%.1f"));
            AddFunction(new FlagValue(this, "1396", "Center Console", "Antenna VHF Top/FM1 Bottom Indicator", ""));
            AddFunction(new FlagValue(this, "1395", "Center Console", "Antenna FM1 Top/VHF Bottom Indicator", ""));
            AddFunction(new Switch(this, devices.CENTRAL_CONSOLE.ToString("d"), "587", SwitchPositions.Create(3, 0.2d, -0.1d, Commands.Button.Button_5.ToString("d"), "Posn", "%0.1f"), "Center Console", "Steer / Swivel", "%0.1f"));
            AddFunction(new Axis(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_1.ToString("d"), "589", 0.1d, -1.0d, 1.0d, "Center Console", "Steering Knob"));
            AddFunction(new PushButton(this, devices.CENTRAL_CONSOLE.ToString("d"), Commands.Button.Button_4.ToString("d"), "590", "Center Console", "Steering Button", "%.1f"));

            AddFunction(Switch.CreateToggleSwitch(this, devices.AN_ALE47.ToString("d"), Commands.Button.Button_1.ToString("d"), "1444", "1.0", "Pulled", "0.0", "Norm", "AN_ALE47", "ASE Jettison Cover", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.AN_ALE47.ToString("d"), Commands.Button.Button_3.ToString("d"), "1445", "1.0", "Pulled", "0.0", "Norm", "AN_ALE47", "ASE Jettison Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.AN_ALE47.ToString("d"), Commands.Button.Button_5.ToString("d"), "1446", "1.0", "Pulled", "0.0", "Norm", "AN_ALE47", "ASE Arm Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.AN_ALE47.ToString("d"), Commands.Button.Button_7.ToString("d"), "1447", "1.0", "Pulled", "0.0", "Norm", "AN_ALE47", "ASE Bypass Switch", "%.1f"));
            AddFunction(new Axis(this, devices.AN_ALE47.ToString("d"), Commands.Button.Button_9.ToString("d"), "1448", 0.1d, 0.0d, 1.0d, "AN_ALE47", "ASE Volume"));  // elements["ASE_VOL"] =            axis_limited({0, 1, 2}, _("Cockpit.CH47.ASE.VOLUME_KB"),      devices.AN_ALE47, device_commands.Button_9, 1448)
            AddFunction(new FlagValue(this, "1449", "AN_ALE47", "ASE ARM Indicator", ""));

            ///TODO: Needs to have values checked.
            AddFunction(new Switch(this, devices.ARC_186.ToString("d"), "1223", SwitchPositions.Create(20, 1.0d, -0.01000001d, Commands.Button.Button_1.ToString("d"), "Click", "%0.1f"), "ARC_186", "Preset Channel Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC_186.ToString("d"), "1224", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_3.ToString("d"), "Posn", "%0.1f"), "ARC_186", "Frequency Mode Dial", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC_186.ToString("d"), "1221", SwitchPositions.Create(4, 0.0d, 0.1d, Commands.Button.Button_4.ToString("d"), "Posn", "%0.1f"), "ARC_186", "Frequency Selection Dial", "%0.1f"));
            AddFunction(new Axis(this, devices.ARC_186.ToString("d"), Commands.Button.Button_5.ToString("d"), "1219", 0.1d, 0.0d, 1.0d, "ARC_186", "Volume"));  // elements["ARC186_VOLUME"] 			= axis({0, 1},_("Volume"), devices.ARC_186, device_commands.Button_5, 1219)
            AddFunction(new PushButton(this, devices.ARC_186.ToString("d"), Commands.Button.Button_6.ToString("d"), "1222", "ARC_186", "Load", "%.1f"));  // elements["ARC186_LOAD"] 			= default_button({0, 1},_("Load"), devices.ARC_186, device_commands.Button_6, 1222, 1, {0, 1})
            AddFunction(new Switch(this, devices.ARC_186.ToString("d"), "1220", new SwitchPosition[] { new SwitchPosition("1.0", "Menu", Commands.Button.Button_7.ToString("d"), Commands.Button.Button_7.ToString("d"), "0.0"), new SwitchPosition("0.0", "On", Commands.Button.Button_8.ToString("d")), new SwitchPosition("-1.0", "OFF", Commands.Button.Button_8.ToString("d"), Commands.Button.Button_8.ToString("d"), "0.0", null) }, "ARC_186", "Squelch / Tone", "%0.1f"));

            //*AddFunction(new vhf_radio_wheel(this, devices.ARC_186, device_commands.Button_9.ToString("d"), Commands.Button.Button_10.ToString("d"), "1225", "ARC_186, device_commands.Button_9", "ARC186_FREQ1","%.1f"));  // elements["ARC186_FREQ1"] = vhf_radio_wheel({0, 1},_("Frequency Selector the 1nd"), devices.ARC_186, device_commands.Button_9,  device_commands.Button_10, 1225, {-0.1, 0.1}, {1229,{0.125,0.775},0.5})
            //*AddFunction(new vhf_radio_wheel(this, devices.ARC_186, device_commands.Button_11.ToString("d"), Commands.Button.Button_12.ToString("d"), "1226", "ARC_186, device_commands.Button_11", "ARC186_FREQ2","%.1f"));  // elements["ARC186_FREQ2"] = vhf_radio_wheel({0, 1},_("Frequency Selector the 2nd"), devices.ARC_186, device_commands.Button_11, device_commands.Button_12, 1226, {-0.1, 0.1}, {1230,{0.0,1.0},1.0})
            //*AddFunction(new vhf_radio_wheel(this, devices.ARC_186, device_commands.Button_13.ToString("d"), Commands.Button.Button_14.ToString("d"), "1227", "ARC_186, device_commands.Button_13", "ARC186_FREQ3","%.1f"));  // elements["ARC186_FREQ3"] = vhf_radio_wheel({0, 1},_("Frequency Selector the 3rd"), devices.ARC_186, device_commands.Button_13, device_commands.Button_14, 1227, {-0.1, 0.1}, {1231,{0.0,1.0},1.0})
            //*AddFunction(new vhf_radio_wheel(this, devices.ARC_186, device_commands.Button_15.ToString("d"), Commands.Button.Button_16.ToString("d"), "1228", "ARC_186, device_commands.Button_15", "ARC186_FREQ4","%.1f"));  // elements["ARC186_FREQ4"] = vhf_radio_wheel({0, 1},_("Frequency Selector the 4th"), devices.ARC_186, device_commands.Button_15, device_commands.Button_16, 1228, {-0.25, 0.25}, {1232,{0.0,1.0},1.0})
            AddFunction(new AbsoluteEncoder(this, devices.ARC_186.ToString("d"), Commands.Button.Button_15.ToString("d"), Commands.Button.Button_16.ToString("d"), "1228", 0.25d, 0.0d, 1.0d, "ARC_186", "Frequency Selector 4th Digit", true, "%.3f"));
            AddFunction(new NetworkValue(this, "1232", "ARC_186", "Frequency Selector 4th Value", "Value of the 4th digit drum", "0 to 0.775 in steps of 0.125", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(new AbsoluteEncoder(this, devices.ARC_186.ToString("d"), Commands.Button.Button_13.ToString("d"), Commands.Button.Button_14.ToString("d"), "1227", 0.1d, 0.0d, 1.0d, "ARC_186", "Frequency Selector 3rd Digit", true, "%.3f"));
            AddFunction(new NetworkValue(this, "1231", "ARC_186", "Frequency Selector 3rd Value", "Value of the 3rd digit drum", "0 to 0.1", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(new AbsoluteEncoder(this, devices.ARC_186.ToString("d"), Commands.Button.Button_11.ToString("d"), Commands.Button.Button_12.ToString("d"), "1226", 0.1d, 0.0d, 1.0d, "ARC_186", "Frequency Selector 2nd Digit", true, "%.3f"));
            AddFunction(new NetworkValue(this, "1230", "ARC_186", "Frequency Selector 2nd Value", "Value of the 2nd digit drum", "0 to 0.1", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(new AbsoluteEncoder(this, devices.ARC_186.ToString("d"), Commands.Button.Button_9.ToString("d"), Commands.Button.Button_10.ToString("d"), "1225", 0.1d, 0.125d, 0.775d, "ARC_186", "Frequency Selector 1st Digit", true, "%.3f"));
            AddFunction(new NetworkValue(this, "1229", "ARC_186", "Frequency Selector 1st Value", "Value of the 1st digit drum", "0 to 1 in steps of 0.25", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_1.ToString("d"), "1", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "LH Aux Forward Pump Switch", "%0 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_3.ToString("d"), "2", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "LH Main Fwd Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_5.ToString("d"), "3", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "LH Main Aft Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_7.ToString("d"), "4", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "LH Aux Aft Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_9.ToString("d"), "5", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "RH Aux Fwd Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_11.ToString("d"), "6", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "RH Main Fwd Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_13.ToString("d"), "7", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "RH Main Aft Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_15.ToString("d"), "8", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "RH Aux Aft Pump Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_17.ToString("d"), "9", "0.0", "Off", "1.0", "On", "Fuel Control (Overhead Console)", "Refuel Sta Switch", "% 1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_19.ToString("d"), "10", "0.0", "Close", "1.0", "Open", "Fuel Control (Overhead Console)", "Cross-Feed Switch", "% 1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "493", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_21.ToString("d"), new string[] { "2 ON", "BOTH", "1 ON" }, "%0.1f"), "Hydraulic Panel (Overhead Console)", "Flight Controls Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_24.ToString("d"), "494", "0.0", "Off", "1.0", "On", "Hydraulic Panel (Overhead Console)", "Power Xfer 1 Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_26.ToString("d"), "495", "0.0", "Off", "1.0", "On", "Hydraulic Panel (Overhead Console)", "Power Xfer 2 Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_28.ToString("d"), "496", "1.0", "Pulled", "0.0", "Norm", "Hydraulic Panel (Overhead Console)", "Power Steer Cover", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_30.ToString("d"), "497", "1.0", "Pulled", "0.0", "Norm", "Hydraulic Panel (Overhead Console)", "Power Steer Switch", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "498", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_32.ToString("d"), new string[] { "OFF", "ON", "EMER" }, "%0.1f"), "Hydraulic Panel (Overhead Console)", "Ramp Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_35.ToString("d"), "499", "0.0", "Off", "1.0", "On", "Hydraulic Panel (Overhead Console)", "Ramp Emergency Cover", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "500", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_37.ToString("d"), new string[]{"UP", "HOLD", "Down" }, "%0.1f"), "Hydraulic Panel (Overhead Console)", "Ramp Emergency Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_40.ToString("d"), "501", "0.0", "Off", "1.0", "On", "EAPS (Overhead Console)", "Engine 1 Fan Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_42.ToString("d"), "502", "0.0", "Off", "1.0", "On", "EAPS (Overhead Console)", "Engine 1 Door Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_44.ToString("d"), "503", "0.0", "Off", "1.0", "On", "EAPS (Overhead Console)", "Engine 2 Fan Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_46.ToString("d"), "504", "0.0", "Off", "1.0", "On", "EAPS (Overhead Console)", "Engine 2 Door Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_48.ToString("d"), "505", 0.1d, 0.0d, 1.0d, "Lighting Panel (Overhead Console)", "Center Console Lighting Knob"));  // elements["OCLP_CONSOLE_DIMMER"] = axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.CONSOLE_DIMMER"), devices.OVERHEAD_CONSOLE, device_commands.Button_48, 505)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_51.ToString("d"), "506", 0.1d, 0.0d, 1.0d, "Lighting Panel (Overhead Console)", "Stick Position Lighting Knob"));  // elements["OCLP_STICK_DIMMER"] =   axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.STICK_DIMMER"),   devices.OVERHEAD_CONSOLE, device_commands.Button_51, 506)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_54.ToString("d"), "507", "0.0", "Off", "1.0", "On", "Internal Lighting Panel (Overhead Console)", "Instrument Flood Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_56.ToString("d"), "508", "0.0", "Off", "1.0", "On", "Internal Lighting Panel (Overhead Console)", "Overhead Flood Switch", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "509", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_58.ToString("d"), new string[] { "Disarm", "Test","Arm"}, "%0.1f"), "Internal Lighting Panel (Overhead Console)", "Emergency Exit Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "510", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_61.ToString("d"), new string[] { "White", "Off", "NVG"}, "%0.1f"), "Internal Lighting Panel (Overhead Console)", "Dome Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_64.ToString("d"), "511", 0.1d, 0.0d, 1.0d, "Internal Lighting Panel (Overhead Console)", "Flood Dimmer Knob"));  // elements["OCLP_FLOOD_DIMMER"] =                        axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.FLOOD_DIMMER"), devices.OVERHEAD_CONSOLE, device_commands.Button_64, 511)
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "512", SwitchPositions.Create(7, 0.0d, 0.1d, Commands.Button.Button_67.ToString("d"), new string[] { "Off", "Top IR", "Lower IR", "Both IR", "Top FAA", "Lower FAA", "Both FAA" }, "%0.1f"), "External Lighting Panel (Overhead Console)", "Anti-Collision IR Mode Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "513", SwitchPositions.Create(5, 0.0d, 0.1d, Commands.Button.Button_70.ToString("d"), new string[] { "1", "2", "3", "4", "5"}, "%0.1f"), "External Lighting Panel (Overhead Console)", "Anti-Collision IR Flash Pattern Select Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_73.ToString("d"), "514", 0.1d, 0.0d, 1.0d, "External Lighting Panel (Overhead Console)", "Anti-Collision IR Intensity Knob"));  // elements["OCLP_IR_DIMMER"] =                             axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.IR_DIMMER"),     devices.OVERHEAD_CONSOLE, device_commands.Button_73, 514)
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "515", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_76.ToString("d"), new string[] {"NVG", "Off", "Norm" }, "%0.1f"), "External Lighting Panel (Overhead Console)", "Formation Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_79.ToString("d"), "516", "0.0", "Flash", "1.0", "Steady", "External Lighting Panel (Overhead Console)", "Formation Mode Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_81.ToString("d"), "517", 0.1d, 0.0d, 1.0d, "External Lighting Panel (Overhead Console)", "Formation Brightness Knob"));  // elements["OCLP_FORM_DIMMER"] =                           axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.FORM_DIMMER"),   devices.OVERHEAD_CONSOLE, device_commands.Button_81, 517, true)
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "518", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_84.ToString("d"), new string[] {"Dim", "Off", "Bright" }, "%0.1f"), "External Lighting Panel (Overhead Console)", "Position Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_87.ToString("d"), "519", "0.0", "Flash", "1.0", "Steady", "External Lighting Panel (Overhead Console)", "Position Mode Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_89.ToString("d"), "520", "0.0", "Ret", "1.0", "On", "Pilot Lighting Panel (Overhead Console)", "Pilot Search Light Switch", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "521", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_91.ToString("d"), new string[] { "DAY", "NIGHT", "NVG"}, "%0.1f"), "Pilot Lighting Panel (Overhead Console)", "Lighting Mode Select Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_94.ToString("d"), "522", 0.1d, 0.0d, 1.0d, "Pilot Lighting Panel (Overhead Console)", "Center Instrument Dimmer Knob"));  // elements["OCLP_CTR_DIMMER"] =                          axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.CTR_DIMMER"),    devices.OVERHEAD_CONSOLE, device_commands.Button_94, 522)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_97.ToString("d"), "523", 0.1d, 0.0d, 1.0d, "Pilot Lighting Panel (Overhead Console)", "Pilot Instrument Dimmer Knob"));  // elements["OCLP_PLT_DIMMER"] =                          axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.PLT_DIMMER"),    devices.OVERHEAD_CONSOLE, device_commands.Button_97, 523)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_100.ToString("d"), "524", "0.0", "Ret", "1.0", "On", "Copilot Lighting Panel (Overhead Console)", "Search Light Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_102.ToString("d"), "525", 0.1d, 0.0d, 1.0d, "Copilot Lighting Panel (Overhead Console)", "Instrument Dimmer Knob"));  // elements["OCLP_CPLT_DIMMER"] =           axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.CPLT_DIMMER"),    devices.OVERHEAD_CONSOLE, device_commands.Button_102, 525)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_105.ToString("d"), "526", 0.1d, 0.0d, 1.0d, "Copilot Lighting Panel (Overhead Console)", "Overhead Console Instrument Knob"));  // elements["OCLP_OVHD_DIMMER"] =           axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.OVHD_DIMMER"),    devices.OVERHEAD_CONSOLE, device_commands.Button_105, 526)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_108.ToString("d"), "527", "0.0", "Off", "1.0", "On", "Anti-Ice (Overhead Console)", "Copilot Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_110.ToString("d"), "528", "0.0", "Off", "1.0", "On", "Anti-Ice (Overhead Console)", "Center Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_112.ToString("d"), "529", "0.0", "Off", "1.0", "On", "Anti-Ice (Overhead Console)", "Pilot Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_114.ToString("d"), "530", "0.0", "Off", "1.0", "On", "Anti-Ice (Overhead Console)", "Pitot Switch", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "541", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_131.ToString("d"), "Posn", "%0.1f"), "Troop Warning (Overhead Console)", " Light Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "540", "Troop Warning (Overhead Console)", " Jump Indicator Red", ""));
            AddFunction(new FlagValue(this, "539", "Troop Warning (Overhead Console)", " Jump Indicator Green", ""));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_134.ToString("d"), "542", "0.0", "Off", "1.0", "On", "Troop Warning (Overhead Console)", " Alarm Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_136.ToString("d"), "543", 0.1d, 0.0d, 1.0d, "Heating Panel (Overhead Console)", "Heater Temp Knob"));  // elements["OCTW_HTR_TEMP"] =                                axis_limited({0, 1, 2}, _("Cockpit.CH47.OCTW.HTR_RHEOSTAT"), devices.OVERHEAD_CONSOLE, device_commands.Button_136, 543)
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "544", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_139.ToString("d"), new string[] {"Blower Only", "Off", "Heater Oo" }, "%0.1f"), "Heating Panel (Overhead Console)", "Heater Mode Switch", "%0.1f"));
            AddFunction(new PushButton(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_142.ToString("d"), "545", "Heating Panel (Overhead Console)", "Heater Start Button", "%.1f"));  // elements["OCTW_HTR_START"] =                                     button({0, 1, 2}, _("Cockpit.CH47.OCTW.HTR_START_SW"), devices.OVERHEAD_CONSOLE, device_commands.Button_142, 545, {{SOUND_SW07_OFF, SOUND_SW07_ON}})
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "756", SwitchPositions.Create(5, 0.0d, 0.1d, Commands.Button.Button_143.ToString("d"), new string[] {"Park", "Off", "Slow", "Medium", "High" }, "%0.1f"), "Wipers Panel (Overhead Console)", "Wiper Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_147.ToString("d"), "546", "0.0", "Closed", "1.0", "Open", "Hoist Panel (Overhead Console)", "Cable Cut Cover", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_149.ToString("d"), "547", "0.0", "Off", "1.0", "On", "Hoist Panel (Overhead Console)", "Cable Cut Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_151.ToString("d"), "548", 0.1d, -1.0d, 1.0d, "Hoist Panel (Overhead Console)", "Hoist Out/In Knob"));  // elements["OCCH_HOIST_CONTROL"] =                               axis_limited({0, 1, 2}, _("Cockpit.CH47.OCHH.HOIST_KB"),       devices.OVERHEAD_CONSOLE, device_commands.Button_151, 548, nil, nil, nil, nil, {-1.0, 1.0}
            AddFunction(new PushButton(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_152.ToString("d"), "549", "Hoist Panel (Overhead Console)", "Hoist Control 2 Button", "%.1f"));
            AddFunction(new Axis(this, devices.CANTED_CONSOLE.ToString("d"), Commands.Button.Button_153.ToString("d"), "550", 0.1d, -1.0d, 1.0d, "Hoist Panel (Overhead Console)", "Hoist Control 2 Knob"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "551", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_156.ToString("d"), "Posn", "%0.1f"), "Hoist Panel (Overhead Console)", "Hoist Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "552", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_159.ToString("d"), new string[] {"RESET", "OFF", "ARM" }, "%0.1f"), "Cargo Hook Panel (Overhead Console)", "Hook Master Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "553", SwitchPositions.Create(5, 0.0d, 0.1d, Commands.Button.Button_162.ToString("d"), new string[] { "FWD", "MID", "AFT", "TANDEM", "ALL" }, "%0.1f"), "Cargo Hook Panel (Overhead Console)", "Hook Selector Knob", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_165.ToString("d"), "554", "0.0", "Closed", "1.0", "Open", "Cargo Hook Panel (Overhead Console)", "Emergency Cover", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_167.ToString("d"), "555", "0.0", "Off", "1.0", "On", "Cargo Hook Panel (Overhead Console)", "Emergency Switch", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "556", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_169.ToString("d"), new string[]{"Test", "Off/Reset", "On"}, "%0.1f"), "Electrical Panel (Overhead Console)", "GEN 1", "%0.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "557", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_172.ToString("d"), new string[]{"Test", "Off/Reset", "On"}, "%0.1f"), "Electrical Panel (Overhead Console)", "GEN 2", "%0.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "558", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_175.ToString("d"), new string[]{"Test", "Off/Reset", "On"}, "%0.1f"), "Electrical Panel (Overhead Console)", "GEN APU", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_178.ToString("d"), "559", "0.0", "Off", "1.0", "On", "Electrical Panel (Overhead Console)", "Battery Switch", "%.1f"));
            AddFunction(new Switch(this, devices.OVERHEAD_CONSOLE.ToString("d"), "560", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_181.ToString("d"), new string[] { "Off", "Run", "Start" }, "%0.1f"), "Electrical Panel (Overhead Console)", "APU Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "563", "Electrical Panel (Overhead Console)", "APU READY Indicator", ""));
            AddFunction(new FlagValue(this, "562", "Electrical Panel (Overhead Console)", "UTIL PRESSURE Indicator", ""));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_180.ToString("d"), "561", "1", "Up", "0", "Middle", "-1", "Down", "Electrical Panel (Overhead Console)", "APU_2", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_184.ToString("d"), "564", "0.0", "Rev", "1.0", "Pri", "FADEC (Overhead Console)", "Mode 1 Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_186.ToString("d"), "565", "0.0", "Rev", "1.0", "Pri", "FADEC (Overhead Console)", "Mode 2 Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_188.ToString("d"), "566", "-1", "Dec", "0", "Middle", "1", "Inc", "FADEC (Overhead Console)", "NR 1", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_189.ToString("d"), "567", "-1", "Dec", "0", "Middle", "1", "Inc", "FADEC (Overhead Console)", "NR 2", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_190.ToString("d"), "568", "0.0", "Off", "1.0", "On", "FADEC (Overhead Console)", "Backup Power Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_192.ToString("d"), "569", "1", "Up", "0", "Middle", "-1", "2", "FADEC (Overhead Console)", "Overspeed Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_193.ToString("d"), "570", "0.0", "PTIT", "1.0", "TRO", "FADEC (Overhead Console)", "Load Share Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_195.ToString("d"), "571", "-1", "1", "0", "Off", "1", "2", "FADEC (Overhead Console)", "Engine Start Switch", "%1d"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_196.ToString("d"), "572", 0.1d, -1.0d, 1.0d, "FADEC (Overhead Console)", "NR % Knob"));  // elements["OCFC_NR"] =               axis_limited({0, 1, 2}, _("Cockpit.CH47.OCFC.NR_KB"),        devices.OVERHEAD_CONSOLE, device_commands.Button_196, 572, 0.5, 0.02)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_207.ToString("d"), "1348", 0.1d, 0.0d, 1.0d, "Pilot Utility Light", "Brightness Knob"));  // elements["OCLP_PUL_KNOB"] =             axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.PUL_KNOB"),  devices.OVERHEAD_CONSOLE, device_commands.Button_207, 1348)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_216.ToString("d"), "1351", "1.0", "Pulled", "0.0", "Norm", "Pilot Utility Light", "Color Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_222.ToString("d"), "1356", 0.1d, 0.0d, 1.0d, "Copilot Utility Light", "Brightness Knob"));  // elements["OCLP_CPUL_KNOB"] =            axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.PUL_KNOB"),  devices.OVERHEAD_CONSOLE, device_commands.Button_222, 1356)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_230.ToString("d"), "1359", "1.0", "Pulled", "0.0", "Norm", "Copilot Utility Light", "Color Switch", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_236.ToString("d"), "1380", 0.1d, 0.0d, 1.0d, "Stalk Lighting", "Copilot Brightness Knob"));  // elements["OCLP_CPSL_KNOB"] =                  axis_limited({0, 1},    _("Cockpit.CH47.OCLP.SL_KNOB"), devices.OVERHEAD_CONSOLE, device_commands.Button_236, 1380)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_239.ToString("d"), "1382", 0.1d, 0.0d, 1.0d, "Stalk Lighting", "Pilot Brightness Knob"));  // elements["OCLP_PSL_KNOB"] =                   axis_limited({0, 1},    _("Cockpit.CH47.OCLP.SL_KNOB"), devices.OVERHEAD_CONSOLE, device_commands.Button_239, 1382)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_242.ToString("d"), "1384", 0.1d, 0.0d, 1.0d, "Stalk Lighting", "TC Brightness Knob"));  // elements["OCLP_TCSL_KNOB"] =                  axis_limited({0, 1, 2}, _("Cockpit.CH47.OCLP.SL_KNOB"), devices.OVERHEAD_CONSOLE, device_commands.Button_242, 1384)
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_245.ToString("d"), "1386", 0.1d, 0.0d, 1.0d, "Stalk Lighting", "PDP1  Brightness Knob"));  // elements["OCLP_PDP1SL_KNOB"] =                axis_limited({1},       _("Cockpit.CH47.OCLP.SL_KNOB"), devices.OVERHEAD_CONSOLE, device_commands.Button_245, 1386)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_251.ToString("d"), "1461", "1.0", "Pulled", "0.0", "Norm", "Stalk Lighting", "PDP1 Lamp Lock", "%.1f"));
            AddFunction(new Axis(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_248.ToString("d"), "1388", 0.1d, 0.0d, 1.0d, "Stalk Lighting", "PDP2 Brightness Knob"));  // elements["OCLP_PDP2SL_KNOB"] =                axis_limited({0},       _("Cockpit.CH47.OCLP.SL_KNOB"), devices.OVERHEAD_CONSOLE, device_commands.Button_248, 1388)
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_253.ToString("d"), "1463", "1.0", "Pulled", "0.0", "Norm", "Stalk Lighting", "PDP2 Lamp Lock", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.OVERHEAD_CONSOLE.ToString("d"), Commands.Button.Button_205.ToString("d"), "1413", "1.0", "Pulled", "0.0", "Norm", "Cockpit Rear Panel", "RB Oil Level Check Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_13.ToString("d"), "1256", "1.0", "Pulled", "0.0", "Norm", "External Cargo Equipment", "WGRIP Hook Cover", " %.1f"));
            AddFunction(new PushButton(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_15.ToString("d"), "1257", "External Cargo Equipment", "WGRIP_HOOK", "%.1f"));  // elements["WGRIP_HOOK"] =                PushButton({2}, "",                               devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_15, 1257, nil, false)
            AddFunction(new Switch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), "1269", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_17.ToString("d"), "Posn", "%0.1f"), "External Cargo Equipment", "HOP ARM", "%0.1f"));
            /// TODO:  Not convinced this 1320 arg is correct!  Check it out.
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_23.ToString("d"), "1320", "1.0", "Pulled", "0.0", "Norm", "External Cargo Equipment", "Trap Door Lever", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_71.ToString("d"), "1247", "1.0", "Pulled", "0.0", "Norm", "External Cargo Equipment", "LP Fwd Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_73.ToString("d"), "1248", "1.0", "Pulled", "0.0", "Norm", "External Cargo Equipment", "LP Center Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_75.ToString("d"), "1249", "1.0", "Pulled", "0.0", "Norm", "External Cargo Equipment", "LP Aft Switch", "%.1f"));
            AddFunction(new PushButton(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_77.ToString("d"), "1250", "External Cargo Equipment", "LP FWD", "%.1f"));  // elements["CHLP_FWD"] =                  PushButton({2}, _("Cockpit.CH47.CHLP_FWD"),       devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_77, 1250)
            AddFunction(new PushButton(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_78.ToString("d"), "1251", "External Cargo Equipment", "LP CTR", "%.1f"));  // elements["CHLP_CTR"] =                  PushButton({2}, _("Cockpit.CH47.CHLP_CTR"),       devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_78, 1251)
            AddFunction(new PushButton(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_79.ToString("d"), "1252", "External Cargo Equipment", "LP AFT", "%.1f"));  // elements["CHLP_AFT"] =                  PushButton({2}, _("Cockpit.CH47.CHLP_AFT"),       devices.EXTERNAL_CARGO_EQUIPMENT, device_commands.Button_79, 1252)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.EXTERNAL_CARGO_EQUIPMENT.ToString("d"), Commands.Button.Button_80.ToString("d"), "1167", "1", "Up", "0", "Middle", "-1", "Down", "External Cargo Equipment", "HOP_EMER", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.MAINTENANCE_PANEL.ToString("d"), Commands.Button.Button_1.ToString("d"), "1034", "1.0", "Pulled", "0.0", "Norm", "Maintenance Panel", "Flight Cont Swtich", "%.1f"));
            AddFunction(new Switch(this, devices.MAINTENANCE_PANEL.ToString("d"), "1035", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_3.ToString("d"), "Posn", "%0.1f"), "Maintenance Panel", "Power ASSR", "%0.1f"));
            AddFunction(new Switch(this, devices.MAINTENANCE_PANEL.ToString("d"), "1036", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_7.ToString("d"), "Posn", "%0.1f"), "Maintenance Panel", "Lighting", "%0.1f"));
            AddFunction(new PushButton(this, devices.MAINTENANCE_PANEL.ToString("d"), Commands.Button.Button_10.ToString("d"), "1037", "Maintenance Panel", "Level Check", "%.1f"));  // elements["MP_LEVEL_CHECK"] =                    PushButton({2}, _("Cockpit.CH47.MP.LEVEL_CHECK"), devices.MAINTENANCE_PANEL, device_commands.Button_10, 1037)
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.MAINTENANCE_PANEL.ToString("d"), Commands.Button.Button_11.ToString("d"), "1038", "1", "Up", "0", "Middle", "-1", "Down", "Maintenance Panel", "Fault Indicator Switch", "%1d"));
            AddFunction(new Switch(this, devices.AFT_WORKSTATION.ToString("d"), "1399", SwitchPositions.Create(3, 0.0d, 0.1d, Commands.Button.Button_1.ToString("d"), "Posn", "%0.1f"), "Aft Workstation", "Interior Cabin Lighting Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.AFT_WORKSTATION.ToString("d"), Commands.Button.Button_4.ToString("d"), "1400", 0.1d, 0.0d, 1.0d, "Aft Workstation", "Interior Cabin Lighting Knob"));  // elements["INTRLTG_CABIN_KNOB"] =                        axis_limited({2}, _("Cockpit.CH47.LCP.DIMMER"),  devices.AFT_WORKSTATION, device_commands.Button_4, 1400)

            #endregion

            #endregion
            #region Circuit Breakers
            /// RegEx used for this region
            /// Circuit Breaker
            /// elements\[\x22(?'panel'.{ 1})(? 'row'.{ 1})(? 'column'.{ 2})\x22\]\s *\=\s* pdp_special\(.* _\(\x22Cockpit\.CH47\.CB.(?'name'.*)\x22\)\)
            /// \t\tAddFunction(new PushButton(this, devices.PDP${panel}.ToString("d"), CbToCommand(\'${row}\', ${column}), CbToArg(${panel}, \'${row}\', ${column}), "Circuit Breaker Panel ${panel}", "${name} (${row},${column})"));\n

            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 06), CbToArg(1, 'A', 06), "Power Distribution Panel 1", "FUEL REFUEL (A,06)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 07), CbToArg(1, 'A', 07), "Power Distribution Panel 1", "FUEL XFEED CONTR (A,07)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 08), CbToArg(1, 'A', 08), "Power Distribution Panel 1", "FUEL R QTY LO LVL (A,08)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 09), CbToArg(1, 'A', 09), "Power Distribution Panel 1", "FUEL L QTY (A,09)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 10), CbToArg(1, 'A', 10), "Power Distribution Panel 1", "L FUEL PUMP CONTR AUX AFT (A,10)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 11), CbToArg(1, 'A', 11), "Power Distribution Panel 1", "L FUEL PUMP CONTR MAIN AFT (A,11)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 12), CbToArg(1, 'A', 12), "Power Distribution Panel 1", "L FUEL PUMP CONTR MAIN FWD (A,12)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 13), CbToArg(1, 'A', 13), "Power Distribution Panel 1", "L FUEL PUMP CONTR AUX FWD (A,13)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 14), CbToArg(1, 'A', 14), "Power Distribution Panel 1", "FADEC NO 1 PRI PWR (A,14)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 15), CbToArg(1, 'A', 15), "Power Distribution Panel 1", "FADEC NO 1 REV PWR (A,15)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 16), CbToArg(1, 'A', 16), "Power Distribution Panel 1", "FADEC NO 1 START AND IGN (A,16)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 17), CbToArg(1, 'A', 17), "Power Distribution Panel 1", "ENGINE NO 1 FUEL SHUT OFF (A,17)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 18), CbToArg(1, 'A', 18), "Power Distribution Panel 1", "ENGINE NO 1 FIRE EXT (A,18)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 19), CbToArg(1, 'A', 19), "Power Distribution Panel 1", "ENGINE NO 1 FIRE DET (A,19)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 20), CbToArg(1, 'A', 20), "Power Distribution Panel 1", "ENGINE NO 1 TORQUE (A,20)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 21), CbToArg(1, 'A', 21), "Power Distribution Panel 1", "ENGINE NO 1 FUEL FLOW (A,21)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 22), CbToArg(1, 'A', 22), "Power Distribution Panel 1", "ENGINE NO 1 OIL PRESS (A,22)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 23), CbToArg(1, 'A', 23), "Power Distribution Panel 1", "DCU 1 26VAC (A,23)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 24), CbToArg(1, 'A', 24), "Power Distribution Panel 1", "COLL PONS (A,24)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 26), CbToArg(1, 'A', 26), "Power Distribution Panel 1", "L FUEL PUMP MAIN AFT (A,26)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 29), CbToArg(1, 'A', 29), "Power Distribution Panel 1", "L FUEL PUMP MAIN FWD (A,29)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('A', 31), CbToArg(1, 'A', 31), "Power Distribution Panel 1", "NO 1 EAPS FAN (A,31)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 01), CbToArg(1, 'B', 01), "Power Distribution Panel 1", "ELECT BATT TEST LOW (B,01)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 02), CbToArg(1, 'B', 02), "Power Distribution Panel 1", "ELECT CKPT UTIL RCPT 1 (B,02)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 03), CbToArg(1, 'B', 03), "Power Distribution Panel 1", "ELECT DC 1 BUS CONTR (B,03)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 04), CbToArg(1, 'B', 04), "Power Distribution Panel 1", "ELECT DC 1 BUS CURR XDCR (B,04)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 05), CbToArg(1, 'B', 05), "Power Distribution Panel 1", "ELECT AC 1 BUS CURR XDCR (B,05)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 06), CbToArg(1, 'B', 06), "Power Distribution Panel 1", "ELECT BATT CHRG 1 RCCO CONTR (B,06)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 07), CbToArg(1, 'B', 07), "Power Distribution Panel 1", "ELECT BATT CHRG 1 CONTR (B,07)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 08), CbToArg(1, 'B', 08), "Power Distribution Panel 1", "ELECT DC 1 ESS BUS CONTR (B,08)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 09), CbToArg(1, 'B', 09), "Power Distribution Panel 1", "ELECT NO 1 RVS CUR CUTOUT (B,09)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 10), CbToArg(1, 'B', 10), "Power Distribution Panel 1", "ELECT EXT PWR CONTR (B,10)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 11), CbToArg(1, 'B', 11), "Power Distribution Panel 1", "HOIST CONTR (B,11)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 12), CbToArg(1, 'B', 12), "Power Distribution Panel 1", "HOIST CABLE CUT (B,12)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 13), CbToArg(1, 'B', 13), "Power Distribution Panel 1", "CARGO HOOK EMER REL PWR (B,13)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 14), CbToArg(1, 'B', 14), "Power Distribution Panel 1", "CARGO HOOK EMER REL CONTR (B,14)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 15), CbToArg(1, 'B', 15), "Power Distribution Panel 1", "AFCS THRUST BRAKE (B,15)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 16), CbToArg(1, 'B', 16), "Power Distribution Panel 1", "AFCS CONTR CTR (B,16)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 17), CbToArg(1, 'B', 17), "Power Distribution Panel 1", "AFCS CYC TRIM FWD ACTR (B,17)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 18), CbToArg(1, 'B', 18), "Power Distribution Panel 1", "AFCS CYC TRIM MAN (B,18)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 19), CbToArg(1, 'B', 19), "Power Distribution Panel 1", "AFCS CLTV DRIVER ACTR (B,19)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 20), CbToArg(1, 'B', 20), "Power Distribution Panel 1", "AFCS FCC 1 (B,20)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 21), CbToArg(1, 'B', 21), "Power Distribution Panel 1", "AFCS FCC 1 (B,21)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 22), CbToArg(1, 'B', 22), "Power Distribution Panel 1", "AFCS CLTV DRIVER ACTR (B,22)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 26), CbToArg(1, 'B', 26), "Power Distribution Panel 1", "L FUEL PUMP AUX AFT (B,26)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 29), CbToArg(1, 'B', 29), "Power Distribution Panel 1", "L FUEL PUMP AUX FWD (B,29)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 31), CbToArg(1, 'B', 31), "Power Distribution Panel 1", "NO 1 XFMR RECT (B,31)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 32), CbToArg(1, 'B', 32), "Power Distribution Panel 1", "DC ESS 1 BUS FEED (B,32)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('B', 33), CbToArg(1, 'B', 33), "Power Distribution Panel 1", "BATT BUS FEED (B,33)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 08), CbToArg(1, 'C', 08), "Power Distribution Panel 1", "AIR WAR MASK BLWR (C,08)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 09), CbToArg(1, 'C', 09), "Power Distribution Panel 1", "AIR WAR MCU 1 CPLT (C,09)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 10), CbToArg(1, 'C', 10), "Power Distribution Panel 1", "CAAS MFCU 1 (C,10)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 11), CbToArg(1, 'C', 11), "Power Distribution Panel 1", "CAAS CDU 1 (C,11)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 12), CbToArg(1, 'C', 12), "Power Distribution Panel 1", "CAAS ZEROIZE (C,12)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 13), CbToArg(1, 'C', 13), "Power Distribution Panel 1", "NAV EGI 1 BACKUP PWR (C,13)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 14), CbToArg(1, 'C', 14), "Power Distribution Panel 1", "NAV SFD 1 (C,14)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 15), CbToArg(1, 'C', 15), "Power Distribution Panel 1", "NAV ADC 1 (C,15)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 16), CbToArg(1, 'C', 16), "Power Distribution Panel 1", "NAV ADF (C,16)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 17), CbToArg(1, 'C', 17), "Power Distribution Panel 1", "NAV EGI 1 (C,17)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 18), CbToArg(1, 'C', 18), "Power Distribution Panel 1", "NAV DR-200 (C,18)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 19), CbToArg(1, 'C', 19), "Power Distribution Panel 1", "NAV ASG HUD (C,19)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 20), CbToArg(1, 'C', 20), "Power Distribution Panel 1", "ASE CMDS CONTR (C,20)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 21), CbToArg(1, 'C', 21), "Power Distribution Panel 1", "NAV MMS (C,21)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 22), CbToArg(1, 'C', 22), "Power Distribution Panel 1", "XMSN OIL PRESS (C,22)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 26), CbToArg(1, 'C', 26), "Power Distribution Panel 1", "CAAS GPPU 1 (C,26)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 29), CbToArg(1, 'C', 29), "Power Distribution Panel 1", "CAAS DCU 1 (C,29)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 31), CbToArg(1, 'C', 31), "Power Distribution Panel 1", "UTIL HYD COOLING BLOWER (C,31)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('C', 32), CbToArg(1, 'C', 32), "Power Distribution Panel 1", "NO 1 DC CROSS TIE (C,32)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 04), CbToArg(1, 'D', 04), "Power Distribution Panel 1", "HYDRAULICS ACC PUMP (D,04)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 05), CbToArg(1, 'D', 05), "Power Distribution Panel 1", "HYDRAULICS OIL LVL (D,05)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 06), CbToArg(1, 'D', 06), "Power Distribution Panel 1", "HYDRAULICS MAINT PNL LTG (D,06)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 07), CbToArg(1, 'D', 07), "Power Distribution Panel 1", "HYDRAULICS NO 1 BLO CONTR (D,07)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 08), CbToArg(1, 'D', 08), "Power Distribution Panel 1", "HYDRAULICS UTIL BLO CONTR (D,08)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 09), CbToArg(1, 'D', 09), "Power Distribution Panel 1", "HYDRAULICS SYS CONTR (D,09)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 10), CbToArg(1, 'D', 10), "Power Distribution Panel 1", "HYDRAULICS BRAKE STEER (D,10)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 11), CbToArg(1, 'D', 11), "Power Distribution Panel 1", "HYDRAULICS RAMP EMER CONTR (D,11)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 12), CbToArg(1, 'D', 12), "Power Distribution Panel 1", "COMM ICU (D,12)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 13), CbToArg(1, 'D', 13), "Power Distribution Panel 1", "COMM L ICS (D,13)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 14), CbToArg(1, 'D', 14), "Power Distribution Panel 1", "COMM UHF RT (D,14)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 15), CbToArg(1, 'D', 15), "Power Distribution Panel 1", "COMM UHF SCTY SET (D,15)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 16), CbToArg(1, 'D', 16), "Power Distribution Panel 1", "COMM SINCGARS 1 RT (D,16)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 17), CbToArg(1, 'D', 17), "Power Distribution Panel 1", "COMM SINCGARS 1 PWR AMP (D,17)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 18), CbToArg(1, 'D', 18), "Power Distribution Panel 1", "COMM IDM (D,18)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 19), CbToArg(1, 'D', 19), "Power Distribution Panel 1", "COMM BFT (D,19)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 20), CbToArg(1, 'D', 20), "Power Distribution Panel 1", "ANTI-ICE WSHLD CPLT CONTR (D,20)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 21), CbToArg(1, 'D', 21), "Power Distribution Panel 1", "ANTI-ICE WSHLD CPLT HTR (D,21)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 22), CbToArg(1, 'D', 22), "Power Distribution Panel 1", "ANTI-ICE PITOT 1 HTR (D,22)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 23), CbToArg(1, 'D', 23), "Power Distribution Panel 1", "NO 1 INSTR XFMR (D,23)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 24), CbToArg(1, 'D', 24), "Power Distribution Panel 1", "NO 2 INSTR XFMR (D,24)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 26), CbToArg(1, 'D', 26), "Power Distribution Panel 1", "CAAS MFD 2 (D,26)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('D', 29), CbToArg(1, 'D', 29), "Power Distribution Panel 1", "CAAS MFD 1 (D,29)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 04), CbToArg(1, 'E', 04), "Power Distribution Panel 1", "L PROX SW (E,04)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 05), CbToArg(1, 'E', 05), "Power Distribution Panel 1", "APU CONTR NORM (E,05)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 06), CbToArg(1, 'E', 06), "Power Distribution Panel 1", "APU CONTR EMER (E,06)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 07), CbToArg(1, 'E', 07), "Power Distribution Panel 1", "NO 1 EAPS BYPASS DOORS (E,07)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 08), CbToArg(1, 'E', 08), "Power Distribution Panel 1", "NO 1 EAPS FAN CONTR (E,08)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 14), CbToArg(1, 'E', 14), "Power Distribution Panel 1", "LIGHTING CARGO HOOK (E,14)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 15), CbToArg(1, 'E', 15), "Power Distribution Panel 1", "LIGHTING EMER EXIT (E,15)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 16), CbToArg(1, 'E', 16), "Power Distribution Panel 1", "LIGHTING OIL LVL CHK (E,16)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 17), CbToArg(1, 'E', 17), "Power Distribution Panel 1", "LIGHTING CABIN AND RAMP (E,17)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 18), CbToArg(1, 'E', 18), "Power Distribution Panel 1", "LIGHTING NVG FORM (E,18)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 19), CbToArg(1, 'E', 19), "Power Distribution Panel 1", "LIGHTING CPLT SLT CONTR (E,19)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 20), CbToArg(1, 'E', 20), "Power Distribution Panel 1", "LIGHTING CPLT SLT FIL (E,20)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 22), CbToArg(1, 'E', 22), "Power Distribution Panel 1", "LIGHTING CPLT INSTR (E,22)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 23), CbToArg(1, 'E', 23), "Power Distribution Panel 1", "LIGHTING CONSOLE (E,23)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 24), CbToArg(1, 'E', 24), "Power Distribution Panel 1", "LIGHTING OVHD PNL (E,24)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 25), CbToArg(1, 'E', 25), "Power Distribution Panel 1", "LIGHTING ILL SW (E,25)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 26), CbToArg(1, 'E', 26), "Power Distribution Panel 1", "LIGHTING FORM (E,26)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 27), CbToArg(1, 'E', 27), "Power Distribution Panel 1", "LIGHTING SEC CKPT LTG CONTR (E,27)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 29), CbToArg(1, 'E', 29), "Power Distribution Panel 1", "HYDRAULICS NO 1 COOLING BLOWER (E,29)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 32), CbToArg(1, 'E', 32), "Power Distribution Panel 1", "BATT CHRG 1 RCCO (E,32)"));
            AddFunction(new PushButton(this, devices.PDP1.ToString("d"), CbToCommand('E', 33), CbToArg(1, 'E', 33), "Power Distribution Panel 1", "NO 1 DC AUX PDP FEED (E,33)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 08), CbToArg(2, 'A', 08), "Power Distribution Panel 2", "FUEL L QTY LO LVL (A,08)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 09), CbToArg(2, 'A', 09), "Power Distribution Panel 2", "FUEL R QTY (A,09)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 10), CbToArg(2, 'A', 10), "Power Distribution Panel 2", "R FUEL PUMP CONTR AUX AFT (A,10)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 11), CbToArg(2, 'A', 11), "Power Distribution Panel 2", "R FUEL PUMP CONTR MAIN AFT (A,11)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 12), CbToArg(2, 'A', 12), "Power Distribution Panel 2", "R FUEL PUMP CONTR MAIN FWD (A,12)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 13), CbToArg(2, 'A', 13), "Power Distribution Panel 2", "R FUEL PUMP CONTR AUX FWD (A,13)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 14), CbToArg(2, 'A', 14), "Power Distribution Panel 2", "FADEC NO 2 PRI PWR (A,14)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 15), CbToArg(2, 'A', 15), "Power Distribution Panel 2", "FADEC NO 2 REV PWR (A,15)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 16), CbToArg(2, 'A', 16), "Power Distribution Panel 2", "FADEC NO 2 START AND IGN (A,16)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 17), CbToArg(2, 'A', 17), "Power Distribution Panel 2", "ENGINE NO 2 FUEL SHUT OFF (A,17)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 18), CbToArg(2, 'A', 18), "Power Distribution Panel 2", "ENGINE NO 2 FIRE EXT (A,18)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 19), CbToArg(2, 'A', 19), "Power Distribution Panel 2", "ENGINE NO 2 FIRE DET (A,19)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 20), CbToArg(2, 'A', 20), "Power Distribution Panel 2", "ENGINE NO 2 TORQUE (A,20)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 21), CbToArg(2, 'A', 21), "Power Distribution Panel 2", "ENGINE NO 2 FUEL FLOW (A,21)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 22), CbToArg(2, 'A', 22), "Power Distribution Panel 2", "ENGINE NO 2 OIL PRESS (A,22)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 23), CbToArg(2, 'A', 23), "Power Distribution Panel 2", "DCU 2 26VAC (A,23)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 26), CbToArg(2, 'A', 26), "Power Distribution Panel 2", "R FUEL PUMP MAIN AFT (A,26)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 29), CbToArg(2, 'A', 29), "Power Distribution Panel 2", "R FUEL PUMP MAIN FWD (A,29)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('A', 31), CbToArg(2, 'A', 31), "Power Distribution Panel 2", "NO 2 XFMR RECT (A,31)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 04), CbToArg(2, 'B', 04), "Power Distribution Panel 2", "ELECT CKPT UTIL RCPT 2 (B,04)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 05), CbToArg(2, 'B', 05), "Power Distribution Panel 2", "ELECT DC 2 BUS CONTR (B,05)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 06), CbToArg(2, 'B', 06), "Power Distribution Panel 2", "ELECT DC 2 BUS CURR XDCR (B,06)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 07), CbToArg(2, 'B', 07), "Power Distribution Panel 2", "ELECT AC 2 BUS CURR XDCR (B,07)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 08), CbToArg(2, 'B', 08), "Power Distribution Panel 2", "ELECT BATT CHRG 2 RCCO CONTR (B,08)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 09), CbToArg(2, 'B', 09), "Power Distribution Panel 2", "ELECT BATT CHRG 2 CONTR (B,09)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 10), CbToArg(2, 'B', 10), "Power Distribution Panel 2", "ELECT DC 2 ESS BUS CONTR (B,10)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 11), CbToArg(2, 'B', 11), "Power Distribution Panel 2", "ELECT NO 2 RVS CUR CUTOUT (B,11)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 13), CbToArg(2, 'B', 13), "Power Distribution Panel 2", "AIR WAR MCU 3 PLT (B,13)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 14), CbToArg(2, 'B', 14), "Power Distribution Panel 2", "CARGO HOOK NORM REL PWR (B,14)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 15), CbToArg(2, 'B', 15), "Power Distribution Panel 2", "CARGO HOOK NORM REL CONTR (B,15)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 16), CbToArg(2, 'B', 16), "Power Distribution Panel 2", "BLADE TRACK (B,16)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 17), CbToArg(2, 'B', 17), "Power Distribution Panel 2", "CRUISE GUIDE (B,17)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 18), CbToArg(2, 'B', 18), "Power Distribution Panel 2", "AFCS CYC TRIM AFT ACTR (B,18)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 19), CbToArg(2, 'B', 19), "Power Distribution Panel 2", "AFCS LONG DRVR ACTR (B,19)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 20), CbToArg(2, 'B', 20), "Power Distribution Panel 2", "AFCS FCC 2 (B,20)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 21), CbToArg(2, 'B', 21), "Power Distribution Panel 2", "AFCS FCC 2 (B,21)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 22), CbToArg(2, 'B', 22), "Power Distribution Panel 2", "AFCS LONG DRVR ACTR (B,22)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 23), CbToArg(2, 'B', 23), "Power Distribution Panel 2", "VIB ABSORB R (B,23)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 24), CbToArg(2, 'B', 24), "Power Distribution Panel 2", "VIB ABSORB L (B,24)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 26), CbToArg(2, 'B', 26), "Power Distribution Panel 2", "R FUEL PUMP AUX AFT (B,26)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 29), CbToArg(2, 'B', 29), "Power Distribution Panel 2", "R FUEL PUMP AUX FWD (B,29)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 31), CbToArg(2, 'B', 31), "Power Distribution Panel 2", "NO 2 EAPS FAN (B,31)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 32), CbToArg(2, 'B', 32), "Power Distribution Panel 2", "NO 2 DC AUX PDP FEED (B,32)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('B', 33), CbToArg(2, 'B', 33), "Power Distribution Panel 2", "NO 2 DC CROSS TIE (B,33)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 08), CbToArg(2, 'C', 08), "Power Distribution Panel 2", "CAAS MFCU 2 (C,08)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 09), CbToArg(2, 'C', 09), "Power Distribution Panel 2", "CAAS CDU 2 (C,09)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 10), CbToArg(2, 'C', 10), "Power Distribution Panel 2", "NAV CVR FDR (C,10)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 11), CbToArg(2, 'C', 11), "Power Distribution Panel 2", "NAV EGI 2 BACKUP PWR (C,11)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 12), CbToArg(2, 'C', 12), "Power Distribution Panel 2", "NAV SFD 2 (C,12)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 13), CbToArg(2, 'C', 13), "Power Distribution Panel 2", "NAV RDR ALT (C,13)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 14), CbToArg(2, 'C', 14), "Power Distribution Panel 2", "NAV VOR (C,14)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 15), CbToArg(2, 'C', 15), "Power Distribution Panel 2", "NAV TACAN (C,15)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 16), CbToArg(2, 'C', 16), "Power Distribution Panel 2", "NAV STORM SCOPE (C,16)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 17), CbToArg(2, 'C', 17), "Power Distribution Panel 2", "NAV EGI 2 (C,17)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 18), CbToArg(2, 'C', 18), "Power Distribution Panel 2", "NAV ADC 2 (C,18)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 19), CbToArg(2, 'C', 19), "Power Distribution Panel 2", "ASE RDR WARN (C,19)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 20), CbToArg(2, 'C', 20), "Power Distribution Panel 2", "ASE MWS CONTR (C,20)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 22), CbToArg(2, 'C', 22), "Power Distribution Panel 2", "ASE MWS PWR (C,22)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 26), CbToArg(2, 'C', 26), "Power Distribution Panel 2", "CAAS GPPU 2 (C,26)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 29), CbToArg(2, 'C', 29), "Power Distribution Panel 2", "CAAS DCU 2 (C,29)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('C', 31), CbToArg(2, 'C', 31), "Power Distribution Panel 2", "CABIN HEATER BLOWER (C,31)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 02), CbToArg(2, 'D', 02), "Power Distribution Panel 2", "HYDRAULICS NO 2 BLO CONTR (D,02)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 03), CbToArg(2, 'D', 03), "Power Distribution Panel 2", "HYDRAULICS PWR XFER (D,03)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 04), CbToArg(2, 'D', 04), "Power Distribution Panel 2", "HYDRAULICS PRESS IND (D,04)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 05), CbToArg(2, 'D', 05), "Power Distribution Panel 2", "HYDRAULICS FLUID TEMP (D,05)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 06), CbToArg(2, 'D', 06), "Power Distribution Panel 2", "HYDRAULICS FLT CONTR (D,06)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 07), CbToArg(2, 'D', 07), "Power Distribution Panel 2", "HYDRAULICS MAINT PNL LTS (D,07)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 08), CbToArg(2, 'D', 08), "Power Distribution Panel 2", "COMM R ICS (D,08)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 09), CbToArg(2, 'D', 09), "Power Distribution Panel 2", "COMM VHF ANT SW (D,09)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 10), CbToArg(2, 'D', 10), "Power Distribution Panel 2", "COMM VHF RT (D,10)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 11), CbToArg(2, 'D', 11), "Power Distribution Panel 2", "COMM VHF SCTY SET (D,11)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 12), CbToArg(2, 'D', 12), "Power Distribution Panel 2", "COMM IFF (D,12)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 13), CbToArg(2, 'D', 13), "Power Distribution Panel 2", "COMM HF PA CLR (D,13)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 14), CbToArg(2, 'D', 14), "Power Distribution Panel 2", "COMM HF KY-100 (D,14)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 15), CbToArg(2, 'D', 15), "Power Distribution Panel 2", "COMM SINCGARS 2 RT (D,15)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 16), CbToArg(2, 'D', 16), "Power Distribution Panel 2", "COMM SINCGARS 2 PWR AMP (D,16)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 17), CbToArg(2, 'D', 17), "Power Distribution Panel 2", "WSHLD WIPER (D,17)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 18), CbToArg(2, 'D', 18), "Power Distribution Panel 2", "ANTI-ICE PITOT 3 HTR (D,18)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 19), CbToArg(2, 'D', 19), "Power Distribution Panel 2", "ANTI-ICE WSHLD PLT CONTR (D,19)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 20), CbToArg(2, 'D', 20), "Power Distribution Panel 2", "ANTI-ICE WSHLD CTR CONTR (D,20)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 21), CbToArg(2, 'D', 21), "Power Distribution Panel 2", "ANTI-ICE WSHLD PLT HTR (D,21)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 22), CbToArg(2, 'D', 22), "Power Distribution Panel 2", "ANTI-ICE WSHLD CTR HTR (D,22)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 23), CbToArg(2, 'D', 23), "Power Distribution Panel 2", "ANTI-ICE PITOT 2 HTR (D,23)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 24), CbToArg(2, 'D', 24), "Power Distribution Panel 2", "ANTI-ICE STATIC PORT HTR (D,24)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 26), CbToArg(2, 'D', 26), "Power Distribution Panel 2", "CAAS MFD 3 (D,26)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('D', 29), CbToArg(2, 'D', 29), "Power Distribution Panel 2", "CAAS MFD 4 (D,29)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 02), CbToArg(2, 'E', 02), "Power Distribution Panel 2", "R PROX SW (E,02)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 03), CbToArg(2, 'E', 03), "Power Distribution Panel 2", "TROOP ALARM JUMP LT (E,03)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 04), CbToArg(2, 'E', 04), "Power Distribution Panel 2", "TROOP ALARM BELL (E,04)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 05), CbToArg(2, 'E', 05), "Power Distribution Panel 2", "CLOCK (E,05)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 06), CbToArg(2, 'E', 06), "Power Distribution Panel 2", "CABIN HTR CONTR (E,06)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 07), CbToArg(2, 'E', 07), "Power Distribution Panel 2", "NO 2 EAPS BYPASS DOORS (E,07)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 08), CbToArg(2, 'E', 08), "Power Distribution Panel 2", "NO 2 EAPS FAN CONTR (E,08)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 10), CbToArg(2, 'E', 10), "Power Distribution Panel 2", "LIGHTING PLT SLT CONTR (E,10)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 11), CbToArg(2, 'E', 11), "Power Distribution Panel 2", "LIGHTING PLT SLT FIL (E,11)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 12), CbToArg(2, 'E', 12), "Power Distribution Panel 2", "LIGHTING PDP MAP (E,12)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 13), CbToArg(2, 'E', 13), "Power Distribution Panel 2", "LIGHTING LAMP TEST (E,13)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 14), CbToArg(2, 'E', 14), "Power Distribution Panel 2", "LIGHTING INSTR FLOOD (E,14)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 15), CbToArg(2, 'E', 15), "Power Distribution Panel 2", "LIGHTING STBY CMPS (E,15)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 16), CbToArg(2, 'E', 16), "Power Distribution Panel 2", "LIGHTING MODE SEL (E,16)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 17), CbToArg(2, 'E', 17), "Power Distribution Panel 2", "LIGHTING CKPT DOME (E,17)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 18), CbToArg(2, 'E', 18), "Power Distribution Panel 2", "LIGHTING POSN (E,18)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 19), CbToArg(2, 'E', 19), "Power Distribution Panel 2", "LIGHTING BOT ANTI COL (E,19)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 20), CbToArg(2, 'E', 20), "Power Distribution Panel 2", "LIGHTING TOP ANTI COL (E,20)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 22), CbToArg(2, 'E', 22), "Power Distribution Panel 2", "LIGHTING PLT INSTR (E,22)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 23), CbToArg(2, 'E', 23), "Power Distribution Panel 2", "LIGHTING CTR INSTR (E,23)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 24), CbToArg(2, 'E', 24), "Power Distribution Panel 2", "NO 2 INSTR XMFR (E,24)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 26), CbToArg(2, 'E', 26), "Power Distribution Panel 2", "CAAS MFD 5 (E,26)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 29), CbToArg(2, 'E', 29), "Power Distribution Panel 2", "HYDRAULICS NO 2 COOLING BLOWER (E,29)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 32), CbToArg(2, 'E', 32), "Power Distribution Panel 2", "BATT CHRG 2 RCCO (E,32)"));
            AddFunction(new PushButton(this, devices.PDP2.ToString("d"), CbToCommand('E', 33), CbToArg(2, 'E', 33), "Power Distribution Panel 2", "DC ESS 2 BUS FEED (E,33)"));

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
