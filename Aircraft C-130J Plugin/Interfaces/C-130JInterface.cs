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
    using System.Windows.Input;
    using System.Xml.Linq;

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
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_1.ToString("d"), "1100", "Pilot CNI", "MU LSK L1"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_2.ToString("d"), "1101", "Pilot CNI", "MU LSK L2"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_3.ToString("d"), "1102", "Pilot CNI", "MU LSK L3"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_4.ToString("d"), "1103", "Pilot CNI", "MU LSK L4"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_5.ToString("d"), "1104", "Pilot CNI", "MU LSK L5"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_6.ToString("d"), "1105", "Pilot CNI", "MU LSK L6"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_7.ToString("d"), "1106", "Pilot CNI", "MU LSK R1"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_8.ToString("d"), "1107", "Pilot CNI", "MU LSK R2"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_9.ToString("d"), "1108", "Pilot CNI", "MU LSK R3"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_10.ToString("d"), "1109", "Pilot CNI", "MU LSK R4"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_11.ToString("d"), "1110", "Pilot CNI", "MU LSK R5"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.SELECT_12.ToString("d"), "1111", "Pilot CNI", "MU LSK R6"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.COMM_TUNE.ToString("d"), "1112", "Pilot CNI", "MU COMM TUNE Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.TOLD.ToString("d"), "1118", "Pilot CNI", "MU TOLD Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.NAV_TUNE.ToString("d"), "1113", "Pilot CNI", "MU NAV TUNE Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.INDEX.ToString("d"), "1119", "Pilot CNI", "MU INDX Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.MC_INDX.ToString("d"), "1120", "Pilot CNI", "MU MC INDX Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.EXEC.ToString("d"), "1122", "Pilot CNI", "MU EXEC Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.DIR.ToString("d"), "1117", "Pilot CNI", "MU DIR INTC Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.LEGS.ToString("d"), "1123", "Pilot CNI", "MU LEGS Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.NEXT.ToString("d"), "1126", "Pilot CNI", "MU NEXT PAGE Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.PREV.ToString("d"), "1125", "Pilot CNI", "MU PREV PAGE Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.MARK.ToString("d"), "1124", "Pilot CNI", "MU MARK Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.IFF.ToString("d"), "1114", "Pilot CNI", "MU IFF Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.NAV_CTRL.ToString("d"), "1115", "Pilot CNI", "MU NAV CTRL Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.MSN.ToString("d"), "1116", "Pilot CNI", "MU MSN Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.CAPS.ToString("d"), "1121", "Pilot CNI", "MU CAPS Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_0.ToString("d"), "1137", "Pilot CNI", "MU 0 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_1.ToString("d"), "1128", "Pilot CNI", "MU 1 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_2.ToString("d"), "1129", "Pilot CNI", "MU 2 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_3.ToString("d"), "1130", "Pilot CNI", "MU 3 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_4.ToString("d"), "1131", "Pilot CNI", "MU 4 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_5.ToString("d"), "1132", "Pilot CNI", "MU 5 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_6.ToString("d"), "1133", "Pilot CNI", "MU 6 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_7.ToString("d"), "1134", "Pilot CNI", "MU 7 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_8.ToString("d"), "1135", "Pilot CNI", "MU 8 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_9.ToString("d"), "1136", "Pilot CNI", "MU 9 Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_DOT.ToString("d"), "1138", "Pilot CNI", "MU Decimal Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_PLUSMINUS.ToString("d"), "1139", "Pilot CNI", "MU Minus Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_A.ToString("d"), "1140", "Pilot CNI", "MU A Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_B.ToString("d"), "1141", "Pilot CNI", "MU B Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_C.ToString("d"), "1142", "Pilot CNI", "MU C Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_D.ToString("d"), "1143", "Pilot CNI", "MU D Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_E.ToString("d"), "1144", "Pilot CNI", "MU E Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_F.ToString("d"), "1145", "Pilot CNI", "MU F Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_G.ToString("d"), "1146", "Pilot CNI", "MU G Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_H.ToString("d"), "1147", "Pilot CNI", "MU H Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_I.ToString("d"), "1148", "Pilot CNI", "MU I Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_J.ToString("d"), "1149", "Pilot CNI", "MU J Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_K.ToString("d"), "1150", "Pilot CNI", "MU K Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_L.ToString("d"), "1151", "Pilot CNI", "MU L Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_M.ToString("d"), "1152", "Pilot CNI", "MU M Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_N.ToString("d"), "1153", "Pilot CNI", "MU N Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_O.ToString("d"), "1154", "Pilot CNI", "MU O Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_P.ToString("d"), "1155", "Pilot CNI", "MU P Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_Q.ToString("d"), "1156", "Pilot CNI", "MU Q Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_R.ToString("d"), "1157", "Pilot CNI", "MU R Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_S.ToString("d"), "1158", "Pilot CNI", "MU S Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_T.ToString("d"), "1159", "Pilot CNI", "MU T Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_U.ToString("d"), "1160", "Pilot CNI", "MU U Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_V.ToString("d"), "1161", "Pilot CNI", "MU V Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_W.ToString("d"), "1162", "Pilot CNI", "MU W Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_X.ToString("d"), "1163", "Pilot CNI", "MU X Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_Y.ToString("d"), "1164", "Pilot CNI", "MU Y Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_Z.ToString("d"), "1165", "Pilot CNI", "MU Z Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.DEL.ToString("d"), "1167", "Pilot CNI", "MU DEL Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.CLR.ToString("d"), "1169", "Pilot CNI", "MU CLR Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_SPACE.ToString("d"), "1166", "Pilot CNI", "MU Unused Key"));
            AddFunction(new PushButton(this, devices.P_CNI.ToString("d"), Commands.P_CNI.KBD_SLASH.ToString("d"), "1168", "Pilot CNI", "MU Slash Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_1.ToString("d"), "1170", "Copilot CNI", "MU LSK L1"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_2.ToString("d"), "1171", "Copilot CNI", "MU LSK L2"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_3.ToString("d"), "1172", "Copilot CNI", "MU LSK L3"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_4.ToString("d"), "1173", "Copilot CNI", "MU LSK L4"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_5.ToString("d"), "1174", "Copilot CNI", "MU LSK L5"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_6.ToString("d"), "1175", "Copilot CNI", "MU LSK L6"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_7.ToString("d"), "1176", "Copilot CNI", "MU LSK R1"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_8.ToString("d"), "1177", "Copilot CNI", "MU LSK R2"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_9.ToString("d"), "1178", "Copilot CNI", "MU LSK R3"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_10.ToString("d"), "1179", "Copilot CNI", "MU LSK R4"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_11.ToString("d"), "1180", "Copilot CNI", "MU LSK R5"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.SELECT_12.ToString("d"), "1181", "Copilot CNI", "MU LSK R6"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.COMM_TUNE.ToString("d"), "1182", "Copilot CNI", "MU COMM TUNE Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.NAV_TUNE.ToString("d"), "1183", "Copilot CNI", "MU NAV TUNE Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.IFF.ToString("d"), "1184", "Copilot CNI", "MU IFF Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.NAV_CTRL.ToString("d"), "1185", "Copilot CNI", "MU NAV CTRL Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.MSN.ToString("d"), "1186", "Copilot CNI", "MU MSN Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.EXEC.ToString("d"), "1187", "Copilot CNI", "MU EXEC Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.CAPS.ToString("d"), "1188", "Copilot CNI", "MU CAPS Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.MC_INDX.ToString("d"), "1189", "Copilot CNI", "MU MC INDX Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.INDEX.ToString("d"), "1191", "Copilot CNI", "MU INDX Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.DIR.ToString("d"), "1192", "Copilot CNI", "MU DIR INTC Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.TOLD.ToString("d"), "1193", "Copilot CNI", "MU TOLD Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.LEGS.ToString("d"), "1194", "Copilot CNI", "MU LEGS Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.MARK.ToString("d"), "1195", "Copilot CNI", "MU MARK Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.PREV.ToString("d"), "1196", "Copilot CNI", "MU PREV PAGE Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.NEXT.ToString("d"), "1197", "Copilot CNI", "MU NEXT PAGE Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_1.ToString("d"), "1198", "Copilot CNI", "MU 1 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_2.ToString("d"), "1199", "Copilot CNI", "MU 2 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_3.ToString("d"), "1200", "Copilot CNI", "MU 3 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_4.ToString("d"), "1201", "Copilot CNI", "MU 4 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_5.ToString("d"), "1202", "Copilot CNI", "MU 5 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_6.ToString("d"), "1203", "Copilot CNI", "MU 6 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_7.ToString("d"), "1204", "Copilot CNI", "MU 7 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_8.ToString("d"), "1205", "Copilot CNI", "MU 8 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_9.ToString("d"), "1206", "Copilot CNI", "MU 9 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_DOT.ToString("d"), "1207", "Copilot CNI", "MU Decimal Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_0.ToString("d"), "1208", "Copilot CNI", "MU 0 Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_PLUSMINUS.ToString("d"), "1209", "Copilot CNI", "MU Minus Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_A.ToString("d"), "1210", "Copilot CNI", "MU A Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_B.ToString("d"), "1211", "Copilot CNI", "MU B Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_C.ToString("d"), "1212", "Copilot CNI", "MU C Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_D.ToString("d"), "1213", "Copilot CNI", "MU D Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_E.ToString("d"), "1214", "Copilot CNI", "MU E Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_F.ToString("d"), "1215", "Copilot CNI", "MU F Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_G.ToString("d"), "1216", "Copilot CNI", "MU G Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_H.ToString("d"), "1217", "Copilot CNI", "MU H Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_I.ToString("d"), "1218", "Copilot CNI", "MU I Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_J.ToString("d"), "1219", "Copilot CNI", "MU J Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_K.ToString("d"), "1220", "Copilot CNI", "MU K Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_L.ToString("d"), "1221", "Copilot CNI", "MU L Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_M.ToString("d"), "1222", "Copilot CNI", "MU M Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_N.ToString("d"), "1223", "Copilot CNI", "MU N Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_O.ToString("d"), "1224", "Copilot CNI", "MU O Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_P.ToString("d"), "1225", "Copilot CNI", "MU P Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_Q.ToString("d"), "1226", "Copilot CNI", "MU Q Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_R.ToString("d"), "1227", "Copilot CNI", "MU R Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_S.ToString("d"), "1228", "Copilot CNI", "MU S Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_T.ToString("d"), "1229", "Copilot CNI", "MU T Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_U.ToString("d"), "1230", "Copilot CNI", "MU U Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_V.ToString("d"), "1231", "Copilot CNI", "MU V Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_W.ToString("d"), "1232", "Copilot CNI", "MU W Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_X.ToString("d"), "1233", "Copilot CNI", "MU X Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_Y.ToString("d"), "1234", "Copilot CNI", "MU Y Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_Z.ToString("d"), "1235", "Copilot CNI", "MU Z Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.DEL.ToString("d"), "1237", "Copilot CNI", "MU DEL Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_SPACE.ToString("d"), "1236", "Copilot CNI", "MU Unused Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.CLR.ToString("d"), "1239", "Copilot CNI", "MU CLR Key"));
            AddFunction(new PushButton(this, devices.C_CNI.ToString("d"), Commands.C_CNI.KBD_SLASH.ToString("d"), "1238", "Copilot CNI", "MU Slash Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_1.ToString("d"), "1240", "Aug Crew CNI", "MU LSK L1"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_2.ToString("d"), "1241", "Aug Crew CNI", "MU LSK L2"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_3.ToString("d"), "1242", "Aug Crew CNI", "MU LSK L3"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_4.ToString("d"), "1243", "Aug Crew CNI", "MU LSK L4"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_5.ToString("d"), "1244", "Aug Crew CNI", "MU LSK L5"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_6.ToString("d"), "1245", "Aug Crew CNI", "MU LSK L6"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_7.ToString("d"), "1246", "Aug Crew CNI", "MU LSK R1"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_8.ToString("d"), "1247", "Aug Crew CNI", "MU LSK R2"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_9.ToString("d"), "1248", "Aug Crew CNI", "MU LSK R3"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_10.ToString("d"), "1249", "Aug Crew CNI", "MU LSK R4"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_11.ToString("d"), "1250", "Aug Crew CNI", "MU LSK R5"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.SELECT_12.ToString("d"), "1251", "Aug Crew CNI", "MU LSK R6"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.COMM_TUNE.ToString("d"), "1252", "Aug Crew CNI", "MU COMM TUNE Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.NAV_TUNE.ToString("d"), "1253", "Aug Crew CNI", "MU NAV TUNE Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.IFF.ToString("d"), "1254", "Aug Crew CNI", "MU IFF Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.NAV_CTRL.ToString("d"), "1255", "Aug Crew CNI", "MU NAV CTRL Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.MSN.ToString("d"), "1256", "Aug Crew CNI", "MU MSN Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.DIR.ToString("d"), "1257", "Aug Crew CNI", "MU DIR INTC Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.TOLD.ToString("d"), "1258", "Aug Crew CNI", "MU TOLD Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.INDEX.ToString("d"), "1259", "Aug Crew CNI", "MU INDX Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.MC_INDX.ToString("d"), "1260", "Aug Crew CNI", "MU MC INDX Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.CAPS.ToString("d"), "1261", "Aug Crew CNI", "MU CAPS Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.EXEC.ToString("d"), "1262", "Aug Crew CNI", "MU EXEC Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.LEGS.ToString("d"), "1264", "Aug Crew CNI", "MU LEGS Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.MARK.ToString("d"), "1265", "Aug Crew CNI", "MU MARK Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.PREV.ToString("d"), "1266", "Aug Crew CNI", "MU PREV PAGE Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.NEXT.ToString("d"), "1267", "Aug Crew CNI", "MU NEXT PAGE Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_1.ToString("d"), "1268", "Aug Crew CNI", "MU 1 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_2.ToString("d"), "1269", "Aug Crew CNI", "MU 2 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_3.ToString("d"), "1270", "Aug Crew CNI", "MU 3 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_4.ToString("d"), "1271", "Aug Crew CNI", "MU 4 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_5.ToString("d"), "1272", "Aug Crew CNI", "MU 5 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_6.ToString("d"), "1273", "Aug Crew CNI", "MU 6 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_7.ToString("d"), "1274", "Aug Crew CNI", "MU 7 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_8.ToString("d"), "1275", "Aug Crew CNI", "MU 8 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_9.ToString("d"), "1276", "Aug Crew CNI", "MU 9 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_DOT.ToString("d"), "1277", "Aug Crew CNI", "MU Decimal Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_0.ToString("d"), "1278", "Aug Crew CNI", "MU 0 Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_PLUSMINUS.ToString("d"), "1279", "Aug Crew CNI", "MU Minus Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_A.ToString("d"), "1280", "Aug Crew CNI", "MU A Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_B.ToString("d"), "1281", "Aug Crew CNI", "MU B Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_C.ToString("d"), "1282", "Aug Crew CNI", "MU C Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_D.ToString("d"), "1283", "Aug Crew CNI", "MU D Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_E.ToString("d"), "1284", "Aug Crew CNI", "MU E Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_F.ToString("d"), "1285", "Aug Crew CNI", "MU F Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_G.ToString("d"), "1286", "Aug Crew CNI", "MU G Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_H.ToString("d"), "1287", "Aug Crew CNI", "MU H Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_I.ToString("d"), "1288", "Aug Crew CNI", "MU I Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_J.ToString("d"), "1289", "Aug Crew CNI", "MU J Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_K.ToString("d"), "1290", "Aug Crew CNI", "MU K Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_L.ToString("d"), "1291", "Aug Crew CNI", "MU L Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_M.ToString("d"), "1292", "Aug Crew CNI", "MU M Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_N.ToString("d"), "1293", "Aug Crew CNI", "MU N Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_O.ToString("d"), "1294", "Aug Crew CNI", "MU O Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_P.ToString("d"), "1295", "Aug Crew CNI", "MU P Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_Q.ToString("d"), "1296", "Aug Crew CNI", "MU Q Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_R.ToString("d"), "1297", "Aug Crew CNI", "MU R Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_S.ToString("d"), "1298", "Aug Crew CNI", "MU S Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_T.ToString("d"), "1299", "Aug Crew CNI", "MU T Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_U.ToString("d"), "1300", "Aug Crew CNI", "MU U Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_V.ToString("d"), "1301", "Aug Crew CNI", "MU V Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_W.ToString("d"), "1302", "Aug Crew CNI", "MU W Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_X.ToString("d"), "1303", "Aug Crew CNI", "MU X Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_Y.ToString("d"), "1304", "Aug Crew CNI", "MU Y Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_Z.ToString("d"), "1305", "Aug Crew CNI", "MU Z Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_SPACE.ToString("d"), "1306", "Aug Crew CNI", "MU Unused Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.DEL.ToString("d"), "1307", "Aug Crew CNI", "MU DEL Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.KBD_SLASH.ToString("d"), "1308", "Aug Crew CNI", "MU Slash Key"));
            AddFunction(new PushButton(this, devices.AC_CNI.ToString("d"), Commands.AC_CNI.CLR.ToString("d"), "1309", "Aug Crew CNI", "MU CLR Key"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_start.ToString("d"), "1651", "Microwave ", " Start"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_clear.ToString("d"), "1652", "Microwave ", " Clear"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_1.ToString("d"), "1653", "Microwave ", " 1"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_2.ToString("d"), "1654", "Microwave ", " 2"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_3.ToString("d"), "1655", "Microwave ", " 3"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_4.ToString("d"), "1656", "Microwave ", " 4"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_5.ToString("d"), "1657", "Microwave ", " 5"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_6.ToString("d"), "1658", "Microwave ", " 6"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_7.ToString("d"), "1659", "Microwave ", " 7"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_8.ToString("d"), "1660", "Microwave ", " 8"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_9.ToString("d"), "1661", "Microwave ", " 9"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.popcorn.ToString("d"), "1662", "Microwave ", " Popcorn"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.micro_0.ToString("d"), "1663", "Microwave ", " 0"));
            AddFunction(new PushButton(this, devices.GALLEY.ToString("d"), Commands.GALLEY.min.ToString("d"), "1664", "Microwave ", " Min"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_l1.ToString("d"), "550", "ARC-210", "LSK 1"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_l2.ToString("d"), "549", "ARC-210", "LSK 2"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_l3.ToString("d"), "548", "ARC-210", "LSK 3"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_tod_snd.ToString("d"), "551", "ARC-210", "TOD SND Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_tod_rcv.ToString("d"), "552", "ARC-210", "TOD RCV Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_gps.ToString("d"), "553", "ARC-210", "GPS Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_rt_select.ToString("d"), "554", "ARC-210", "RT SELECT Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_menu.ToString("d"), "533", "ARC-210", "MENU/TIME Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_am_fm.ToString("d"), "534", "ARC-210", "AM/FM Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_xmt_rec_send.ToString("d"), "535", "ARC-210", "XMT/REC or SEND Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_rcu_offset.ToString("d"), "536", "ARC-210", "OFFSET or RCV Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_enter.ToString("d"), "537", "ARC-210", "ENTER Key"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_brt_up.ToString("d"), "547", "ARC-210", "Brightness Increase"));
            AddFunction(new PushButton(this, devices.VOLUME_MANAGER.ToString("d"), Commands.VOL_CTRL.arc_brt_dn.ToString("d"), "546", "ARC-210", "Brightness Decrease"));

            // ^elements\[""[^""] *? _(?< arg1 >\d{ 2,4})(?: _(?< arg2 >\d{ 1,4}))?""\].*=\s * (? 'function'.* _key.*)\(""(?'name'.*)(?:-(? 'name2'.*))""\s *,\s * (? 'command'.*)\s *,\s * (? 'arg0'\d{ 2,4})\s *,\s * (?: devices\.(? 'device'.*))\).*$
            // AddFunction(new PushButton(this, devices.${ device }.ToString("d"), Commands.${ command}.ToString("d"),"${arg1}","${name}","${name2}"));\n

            // ^elements\[""[^""]*?_(?<arg1>\d{2,4})(?:_(?<arg2>\d{1,4}))?""\].*=\s*(?'function'.*)\(""(?'name'.*)""\s*,\s*(?:devices\.(?'device'.*))\s*,\s*(?'command'.*)\s*,\s*.*$

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
