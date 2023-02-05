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

#pragma warning disable IDE0051 // Remove unused private member: interfaces contain definitions we don't implement
namespace GadrocsWorkshop.Helios.Interfaces.DCS.FA18C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.FA18C.Functions;
    using static System.Net.Mime.MediaTypeNames;

    [HeliosInterface("Helios.FA18C", "DCS F/A-18C", typeof(DCSInterfaceEditor), typeof(UniqueHeliosInterfaceFactory), UniquenessKey = "Helios.DCSInterface")]
    public class FA18CInterface : DCSInterface
    {
        #region Devices
        //  From devices.lua - DCS seem to want this to remain constant which is great 
        private const string FM_PROXY = "1";
        private const string CONTROL_INTERFACE = "2";
        private const string ELEC_INTERFACE = "3";
        private const string HYDRAULIC_INTERFACE = "4";
        private const string GEAR_INTERFACE = "5";
        private const string FUEL_INTERFACE = "6";
        private const string CPT_MECHANICS = "7";               // Integral DEVICE for various mechanical systems like canopy, etc
        private const string EXT_LIGHTS = "8";                  // External Lighting
        private const string CPT_LIGTHS = "9";                  // Cockpit Lighting
        private const string OXYGEN_INTERFACE = "10";           // Cockpit interface to OBOGS
        private const string ECS_INTERFACE = "11";              //  Environmental Control System
        private const string ENGINES_INTERFACE = "12";          //  Cockpit interface to aircraft engines
        //  HOTAS interface
        private const string HOTAS = "13";                      //  Is this a duplicate of 38 
        // Computers
        private const string MUX = "14";                        // Multiplex manager, holds channels and manages remote terminals addition/remove
        private const string SDC = "15";                        // Signal Data Computer (part of FIRAMS)
        private const string MISSION_COMPUTER_NO_1 = "16";      // AN/AYK-14, Mission Computer No 1
        private const string MISSION_COMPUTER_NO_2 = "17";      // AN/AYK-14, Mission Computer No 2
        private const string FCCA = "18";                       //  Flight Control Computer A
        private const string FCCB = "19";                       //  Flight Control Computer B
        private const string ADC = "20";                        //  Air Data Computer - CPO-1334/A
        private const string ARMAMENT_COMPUTER = "21";          //  Armament Computer - AYK-22
        private const string CONTROL_CONVERTER = "22";          // Control-Converter - C-10382/A
        private const string SMS = "23";                        // Stores Management System, Armament Computer - CP-2218/AYK-22(V)
        private const string DIGITAL_MAP_COMPUTER = "24";       //  Digital Map Computer - CP-1802/ASQ-196
        private const string UFC = "25";                        // Electronic Equipment Control (UFC) - C-10380/ASQ
        // Instruments
        private const string AAU52 = "26";                   //  Standby Pressure Altimeter - AAU-52/A
        private const string AVU35 = "27";                   //  Indicated Airspeed Indicator - AVU-35/A
        private const string AVU53 = "28";                   //  Vertical Speed Indicator - AVU-53/A
        private const string STANDBY_COMPASS = "29";            // Standby Compass - AQU-3/A
        private const string ID2163A = "30";                    // Radar Altimeter Height Indicator - ID-2163/A
        private const string RADAR_ALTIMETER = "31";            // Radar Altimeter - APN-194(V)
        private const string SAI = "32";                        // Standby Attitude Indicator - ARU-48
        // FIRAMS
        private const string IFEI = "33";                       // Integrated Fuel/Engine Indicator (IFEI)
        //  Multipurpose Display Group
        private const string HUD = "34";                        // Head-Up Display - AVQ-32
        private const string MDI_LEFT = "35";                   // Left Multipurpose Display Indicator (DDI) - IP-1556/A
        private const string MDI_RIGHT = "36";                  // Right Multipurpose Display Indicator (DDI) - IP-1556/A
        private const string AMPCD = "37";                      // Advanced Multipurpose Color Display - ???
        //  Radio & Comm
        private const string UHF1 = "38";                       // VHF/UHF Receiver-Transmitter - ARC 210
        private const string UHF2 = "39";                       // VHF/UHF Receiver-Transmitter - ARC 210 DCS
        private const string INTERCOM = "40";                   // Intercommunication Amplifier-Control - AM-7360/A
        private const string KY58 = "41";                       // KY-58 Secure Speech System
        //  Sensors
        private const string RADAR = "42";                      // Radar - AN/APG-73, interfaced to the rest of avionic system via the Radar Data Processor CP-2062/APG-73
        private const string FLIR = "43";                       // Forward Looking Infrared Pod interface
        //  INS/GPS
        private const string INS = "44";                        // INS, AN/ASN-139
        private const string GPS = "45";                        // GPS, AN/ASN-163
        private const string MAD = "46";                        // Magnetic Azimuth Detector, DT-604/A
        //  Armament
        private const string SIDEWINDER_INTERFACE = "47";
        private const string MAVERICK_INTERFACE = "48";
        //  RNAV
        private const string ADF = "49";                        // Direction Finder OA-8697/ARD
        private const string ANTENNA_SELECTOR = "50";
        private const string MIDS = "51";                       // MIDS-LVT (implements Link 16 and TACAN)
        private const string ILS = "52";                        // AN/ARA-63D, airborne segment of US NAVY ACLS, and US Marines MRAALS
        //  TEWS
        private const string RWR = "53";                        // AN/ALR-67(V)
        private const string CMDS = "54";                       // Countermeasures dispenser System

        private const string MACROS = "55";
        private const string IFF = "56";                        // IFF, AN/APX-111(V) CIT
        //Helmet
        private const string NVG = "57";
        private const string HMD_INTERFACE = "58";
        // MIDS Parts
        private const string MIDS_RT = "59";
        private const string CLC = "60";                        // Command Launch Computer
        private const string HARM_INTERFACE = "61";
        private const string TGP_INTERFACE = "62";
        private const string WALLEYE_INTERFACE = "63";
        private const string DATALINK_INTERFACE = "64";
        private const string SLAM_INTERFACE = "65";
        private const string ASPJ = "66";
        private const string BCN = "67";
        private const string KNEEBOARD = "68";
        private const string LINK4 = "69";

        private enum gear_commands
        {
            GearHandle = 3001,
            EmergDown = 3002,
            DownLockOverrideBtn = 3003,
            AntiSkidSw = 3004,
            EmergParkHandleOnOff = 3005,
            EmergParkHandleSelectPark = 3006,
            EmergParkHandleSelectEmerg = 3007,
            LaunchBarSw = 3008,
            HookHandle = 3009,
            AntiSkidSw_EXT = 3010,
            EmergParkHandleOnOff_EXT = 3011,
            EmergParkHandleSelect_EXT = 3012,
            HookHandle_EXT = 3013,
            LaunchBarSw_EXT = 3014,
        };
        #endregion

        public FA18CInterface(string name)
            : base(name, "FA-18C_hornet", "pack://application:,,,/Helios;component/Interfaces/DCS/FA18C/ExportFunctions.lua")
        {
#if (!DEBUG)
                if (LoadFunctionsFromJson())
                {
                    return;
                }
#endif
            #region Caution Indicators
            // Caution Light Indicator Panel
            AddFunction(new FlagValue(this, "298", "Caution Indicators", "CK_SEAT", ""));
            AddFunction(new FlagValue(this, "299", "Caution Indicators", "APU_ACC", ""));
            AddFunction(new FlagValue(this, "300", "Caution Indicators", "BATT_SW", ""));
            AddFunction(new FlagValue(this, "301", "Caution Indicators", "FCS_HOT", ""));
            AddFunction(new FlagValue(this, "302", "Caution Indicators", "GEN_TIE", ""));
            AddFunction(new FlagValue(this, "303", "Caution Indicators", "SPARE_CTN1", ""));
            AddFunction(new FlagValue(this, "304", "Caution Indicators", "FUEL_LO", ""));
            AddFunction(new FlagValue(this, "305", "Caution Indicators", "FCES", ""));
            AddFunction(new FlagValue(this, "306", "Caution Indicators", "SPARE_CTN2", ""));
            AddFunction(new FlagValue(this, "307", "Caution Indicators", "L_GEN", ""));
            AddFunction(new FlagValue(this, "308", "Caution Indicators", "R_GEN", ""));
            AddFunction(new FlagValue(this, "309", "Caution Indicators", "SPARE_CTN3", ""));
            // LH Advisory and Threat Warning Indicator Panel
            AddFunction(new FlagValue(this, "13", "Caution Indicators", "MASTER_CAUTION", ""));
            AddFunction(new FlagValue(this, "10", "Caution Indicators", "FIRE_LEFT", ""));
            AddFunction(new FlagValue(this, "15", "Caution Indicators", "GO", ""));
            AddFunction(new FlagValue(this, "16", "Caution Indicators", "NO_GO", ""));
            AddFunction(new FlagValue(this, "17", "Caution Indicators", "L_BLEED", ""));
            AddFunction(new FlagValue(this, "18", "Caution Indicators", "R_BLEED", ""));
            AddFunction(new FlagValue(this, "19", "Caution Indicators", "SPD_BRK", ""));
            AddFunction(new FlagValue(this, "20", "Caution Indicators", "STBY", ""));
            AddFunction(new FlagValue(this, "21", "Caution Indicators", "L_BAR_RED", ""));
            AddFunction(new FlagValue(this, "22", "Caution Indicators", "REC", ""));
            AddFunction(new FlagValue(this, "23", "Caution Indicators", "L_BAR_GREEN", ""));
            AddFunction(new FlagValue(this, "24", "Caution Indicators", "XMIT", ""));
            AddFunction(new FlagValue(this, "25", "Caution Indicators", "ASPJ_OH", ""));
            // RH Advisory and Threat Warning Indicator Panel
            AddFunction(new FlagValue(this, "29", "Caution Indicators", "FIRE_APU", ""));
            AddFunction(new FlagValue(this, "26", "Caution Indicators", "FIRE_RIGHT", ""));
            AddFunction(new FlagValue(this, "31", "Caution Indicators", "RCDR_ON", ""));
            AddFunction(new FlagValue(this, "32", "Caution Indicators", "DISP", ""));
            AddFunction(new FlagValue(this, "38", "Caution Indicators", "SAM", ""));
            AddFunction(new FlagValue(this, "39", "Caution Indicators", "AI", ""));
            AddFunction(new FlagValue(this, "40", "Caution Indicators", "AAA", ""));
            AddFunction(new FlagValue(this, "41", "Caution Indicators", "CW", ""));
            AddFunction(new FlagValue(this, "33", "Caution Indicators", "SPARE_RH1", ""));
            AddFunction(new FlagValue(this, "34", "Caution Indicators", "SPARE_RH2", ""));
            AddFunction(new FlagValue(this, "35", "Caution Indicators", "SPARE_RH3", ""));
            AddFunction(new FlagValue(this, "36", "Caution Indicators", "SPARE_RH4", ""));
            AddFunction(new FlagValue(this, "37", "Caution Indicators", "SPARE_RH5", ""));
            // Flaps Landing Gear and Stores Indicator Panel
            AddFunction(new FlagValue(this, "152", "Caution Indicators", "CTR", "Centre Stores Indicator for Jettison"));
            AddFunction(new FlagValue(this, "154", "Caution Indicators", "LI", "Left Inner Stores Indicator for Jettison"));
            AddFunction(new FlagValue(this, "156", "Caution Indicators", "LO", "Left Outer Stores Indicator for Jettison"));
            AddFunction(new FlagValue(this, "158", "Caution Indicators", "RI", "Right Inner Stores Indicator for Jettison"));
            AddFunction(new FlagValue(this, "160", "Caution Indicators", "RO", "Right Outer Stores Indicator for Jettison"));
            AddFunction(new FlagValue(this, "166", "Caution Indicators", "NOSE_GEAR", ""));
            AddFunction(new FlagValue(this, "165", "Caution Indicators", "LEFT_GEAR", ""));
            AddFunction(new FlagValue(this, "167", "Caution Indicators", "RIGHT_GEAR", ""));
            AddFunction(new FlagValue(this, "163", "Caution Indicators", "HALF_FLAPS", ""));
            AddFunction(new FlagValue(this, "164", "Caution Indicators", "FULL_FLAPS", ""));
            AddFunction(new FlagValue(this, "162", "Caution Indicators", "FLAPS", ""));
            // Lock/Shoot Light Assy
            AddFunction(new FlagValue(this, "1", "Caution Indicators", "LOCK", ""));
            AddFunction(new FlagValue(this, "2", "Caution Indicators", "SHOOT", ""));
            AddFunction(new FlagValue(this, "3", "Caution Indicators", "SHOOT_STROBE", ""));
            // Master Arm Control
            AddFunction(new FlagValue(this, "47", "Caution Indicators", "AA", "Air to Air Master Mode Indicator"));
            AddFunction(new FlagValue(this, "48", "Caution Indicators", "AG", "Air to Ground Master Mode Indicator"));
            AddFunction(new FlagValue(this, "45", "Caution Indicators", "DISCH", ""));
            AddFunction(new FlagValue(this, "44", "Caution Indicators", "READY", ""));
            // Arresting Hook Control Handle
            AddFunction(new FlagValue(this, "294", "Caution Indicators", "HOOK", ""));

            AddFunction(new FlagValue(this, "227", "Caution Indicators", "Landing Gear Handle", ""));
            AddFunction(new FlagValue(this, "376", "Caution Indicators", "APU_READY", ""));



            AddFunction(new FlagValue(this, "137", "Caution Indicators", "SPN", ""));
            // AoA Indexer Lights
            AddFunction(new IndicatorDimmable(this, "4", "Caution Indicators", "AOA_HIGH", "AoA Indexer Light (High)", "%0.2f"));
            AddFunction(new IndicatorDimmable(this, "5", "Caution Indicators", "AOA_CENTER", "AoA Indexer Light (Center)", "%0.2f"));
            AddFunction(new IndicatorDimmable(this, "6", "Caution Indicators", "AOA_LOW", "AoA Indexer Light (Low)", "%0.2f"));
            // Declarations for Caution Lights numbers from Lamps.lua
            #endregion
            #region Internal Lights
            AddFunction(new FlagValue(this, "460", "Internal Lights", "Console", ""));
            AddFunction(new FlagValue(this, "461", "Internal Lights", "Flood", ""));
            AddFunction(new FlagValue(this, "462", "Internal Lights", "NVG Flood", ""));
            AddFunction(new FlagValue(this, "464", "Internal Lights", "Emergancy Instruments", ""));
            AddFunction(new FlagValue(this, "465", "Internal Lights", "Engine Instrument Flood", ""));
            AddFunction(new FlagValue(this, "466", "Internal Lights", "Instrument", ""));
            AddFunction(new FlagValue(this, "467", "Internal Lights", "Standby Compass", ""));
            AddFunction(new FlagValue(this, "810", "Internal Lights", "Utility", ""));
            AddFunction(new FlagValue(this, "463", "Internal Lights", "Chart", ""));

            #endregion
            #region RWR
            AddFunction(new FlagValue(this, "273", "RWR", "Limit", ""));
            AddFunction(new FlagValue(this, "274", "RWR", "Display", ""));
            AddFunction(new FlagValue(this, "270", "RWR", "Special Enable", ""));
            AddFunction(new FlagValue(this, "271", "RWR", "Special", ""));
            AddFunction(new FlagValue(this, "267", "RWR", "Enable", ""));
            AddFunction(new FlagValue(this, "268", "RWR", "Offset", ""));
            AddFunction(new FlagValue(this, "264", "RWR", "Fail", ""));
            AddFunction(new FlagValue(this, "265", "RWR", "BIT", ""));
            AddFunction(new FlagValue(this, "520", "RWR", "RWR Lights Brightness ", ""));
            #endregion
            #region CMDS
            AddFunction(new FlagValue(this, "516", "CMDS", "ECM Jettison", ""));
            #endregion
            #region  Control System
            AddFunction(new Axis(this, CONTROL_INTERFACE, "3001", "345", 0.05d, -1.00d, 1.00d, "Control system", "RUD TRIM Control"));
            AddFunction(new PushButton(this, CONTROL_INTERFACE, "3002", "346", "Control system", "T/O TRIM Button", "1", "0", "%1d"));
            AddFunction(new PushButton(this, CONTROL_INTERFACE, "3003", "349", "Control system", "FCS RESET Button", "1", "0", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, CONTROL_INTERFACE, "3005", "348", "1.0", "OPEN", "0.0", "CLOSE", "Control system", "GAIN Switch Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CONTROL_INTERFACE, "3006", "347", "1.0", "NORM", "0.0", "ORIDE", "Control system", "GAIN Switch", "%0.1f"));
            AddFunction(new Switch(this, CONTROL_INTERFACE, "234", new SwitchPosition[] { new SwitchPosition("1.0", "AUTO", "3007"), new SwitchPosition("0.0", "HALF", "3007"), new SwitchPosition("-1.0", "FULL", "3007") }, "Control system", "FLAP Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CONTROL_INTERFACE, "3008", "139", "1.0", "OPEN", "0.0", "CLOSE", "Control system", "Spin Recovery Switch Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CONTROL_INTERFACE, "3009", "138", "1.0", "RCVY", "0.0", "NORM", "Control system", "Spin Recovery Switch", "%0.1f"));
            AddFunction(new PushButton(this, CONTROL_INTERFACE, "3004", "470", "Control system", "FCS BIT Switch", "1", "0", "%1d"));
            AddFunction(new Axis(this, CONTROL_INTERFACE, "3012", "504", 0.15d, 0d, 1d, "Control system", "Throttles Friction Adjusting Lever"));
            AddFunction(new Switch(this, CONTROL_INTERFACE, "295", new SwitchPosition[] { new SwitchPosition("-1.0", "Fold", "3011"), new SwitchPosition("0.0", "Hold", "3011"), new SwitchPosition("1.0", "Spread", "3011") }, "Control system", "Wing Fold Lever", "%0.1f"));
            AddFunction(new PushButton(this, CONTROL_INTERFACE, "3010", "296", "Control system", "Wing Fold Lever Stow/Pull", "1", "0", "%1d"));
            #endregion
            #region  Electric system
            AddFunction(new Switch(this, ELEC_INTERFACE, "404", new SwitchPosition[] { new SwitchPosition("1.0", "ON", "3001"), new SwitchPosition("0.0", "OFF", "3001"), new SwitchPosition("-1.0", "ORIDE", "3001") }, "Electric system", "Battery Switch", "%0.1f"));    // elements["pnt_404"]     = default_3_position_tumb(_("Battery Switch, ON/OFF/ORIDE"),                devices.ELEC_INTERFACE, elec_commands.BattSw,               404)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3002", "402", "1.0", "NORM", "0.0", "OFF", "Electric system", "Left Generator Control Switch", "%0.1f"));    // elements["pnt_402"]     = default_2_position_tumb(_("Left Generator Control Switch, NORM/OFF"),     devices.ELEC_INTERFACE, elec_commands.LGenSw,               402)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3003", "403", "1.0", "NORM", "0.0", "OFF", "Electric system", "Right Generator Control Switch", "%0.1f"));    // elements["pnt_403"]     = default_2_position_tumb(_("Right Generator Control Switch, NORM/OFF"),    devices.ELEC_INTERFACE, elec_commands.RGenSw,               403)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3007", "379", "1.0", "OPEN", "0.0", "CLOSE", "Electric system", "Generator TIE Control Switch Cover", "%0.1f"));    // elements["pnt_379"]     = default_red_cover(_("Generator TIE Control Switch Cover, OPEN/CLOSE"),    devices.ELEC_INTERFACE, elec_commands.GenTieControlSwCover, 379)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3006", "378", "1.0", "NORM", "0.0", "RESET", "Electric system", "Generator TIE Control Switch", "%0.1f"));    // elements["pnt_378"]     = default_2_position_tumb(_("Generator TIE Control Switch, NORM/RESET"),    devices.ELEC_INTERFACE, elec_commands.GenTieControlSw,      378)
            AddFunction(new Switch(this, ELEC_INTERFACE, "336", new SwitchPosition[] { new SwitchPosition("1", "Reset", "3004", "3004", "0"), new SwitchPosition("0", "Norm", "3005"), new SwitchPosition("-1", "Off", "3005", "3005", "0") }, "Electric system", "External Power Switch", "%1d"));     // elements["pnt_336"] = default_button_tumb_v2(_("External Power Switch, RESET/NORM/OFF"), devices.ELEC_INTERFACE, elec_commands.ExtPwrSw, elec_commands.ExtPwrReset,  336)
            AddFunction(new Switch(this, ELEC_INTERFACE, "332", new SwitchPosition[] { new SwitchPosition("1", "A On", "3008", "3008", "0"), new SwitchPosition("0", "Auto", "3009"), new SwitchPosition("-1", "B On", "3009", "3009", "0") }, "Electric system", "Ground Power Switch 1", "%1d")); // elements["pnt_332"] = springloaded_3_pos_tumb2(_("Ground Power Switch 1, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr1SwB, elec_commands.GndPwr1SwA, 332)
            AddFunction(new Switch(this, ELEC_INTERFACE, "333", new SwitchPosition[] { new SwitchPosition("1", "A On", "3010", "3010", "0"), new SwitchPosition("0", "Auto", "3011"), new SwitchPosition("-1", "B On", "3011", "3011", "0") }, "Electric system", "Ground Power Switch 2", "%1d")); // elements["pnt_333"] = springloaded_3_pos_tumb2(_("Ground Power Switch 2, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr2SwB, elec_commands.GndPwr2SwA, 333)
            AddFunction(new Switch(this, ELEC_INTERFACE, "334", new SwitchPosition[] { new SwitchPosition("1", "A On", "3012", "3012", "0"), new SwitchPosition("0", "Auto", "3013"), new SwitchPosition("-1", "B On", "3013", "3013", "0") }, "Electric system", "Ground Power Switch 3", "%1d")); // elements["pnt_334"] = springloaded_3_pos_tumb2(_("Ground Power Switch 3, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr3SwB, elec_commands.GndPwr3SwA, 334)
            AddFunction(new Switch(this, ELEC_INTERFACE, "335", new SwitchPosition[] { new SwitchPosition("1", "A On", "3014", "3014", "0"), new SwitchPosition("0", "Auto", "3015"), new SwitchPosition("-1", "B On", "3015", "3015", "0") }, "Electric system", "Ground Power Switch 4", "%1d")); // elements["pnt_335"] = springloaded_3_pos_tumb2(_("Ground Power Switch 4, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr4SwB, elec_commands.GndPwr4SwA, 335)
            #region incorrect switches
            //AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, "3004", "336", "1.0", "RESET", "0.5", "NORM", "0.0", "OFF", "Electric system", "External Power Switch", "%0.1f"));    // elements["pnt_336"] = default_button_tumb_v2(_("External Power Switch, RESET/NORM/OFF"), devices.ELEC_INTERFACE, elec_commands.ExtPwrSw, elec_commands.ExtPwrReset,  336)
            //AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, "3009", "332", "1.0", "A ON", "0.5", "AUTO", "0.0", "B ON", "Electric system", "Ground Power Switch 1", "%0.1f"));    // elements["pnt_332"] = springloaded_3_pos_tumb2(_("Ground Power Switch 1, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr1SwB, elec_commands.GndPwr1SwA, 332)
            //AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, "3011", "333", "1.0", "A ON", "0.5", "AUTO", "0.0", "B ON", "Electric system", "Ground Power Switch 2", "%0.1f"));    // elements["pnt_333"] = springloaded_3_pos_tumb2(_("Ground Power Switch 2, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr2SwB, elec_commands.GndPwr2SwA, 333)
            //AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, "3013", "334", "1.0", "A ON", "0.5", "AUTO", "0.0", "B ON", "Electric system", "Ground Power Switch 3", "%0.1f"));    // elements["pnt_334"] = springloaded_3_pos_tumb2(_("Ground Power Switch 3, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr3SwB, elec_commands.GndPwr3SwA, 334)
            //AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, "3015", "335", "1.0", "A ON", "0.5", "AUTO", "0.0", "B ON", "Electric system", "Ground Power Switch 4", "%0.1f"));    // elements["pnt_335"] = springloaded_3_pos_tumb2(_("Ground Power Switch 4, A ON/AUTO/B ON"),devices.ELEC_INTERFACE, elec_commands.GndPwr4SwB, elec_commands.GndPwr4SwA, 335)
            #endregion
            #endregion
            #region  Anti-Ice

            AddFunction(new Switch(this, ELEC_INTERFACE, "409", new SwitchPosition[] { new SwitchPosition("1.0", "ON", "3016", "3016", null, "1.0"), new SwitchPosition("0.0", "AUTO", "3016", "3016", null, "0.0") }, "Anti-Ice", "Pitot Heater Switch", "%0.1f"));
            AddFunction(new Switch(this, ENGINES_INTERFACE, "410", new SwitchPosition[] { new SwitchPosition("1.0", "ON", "3014"), new SwitchPosition("0.0", "OFF", "3014"), new SwitchPosition("-1.0", "TEST", "3014") }, "Anti-Ice", "Engine Anti-Ice Switch", "%0.1f"));
            #endregion
            #region  CB
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3017", "381", "1.0", "ON", "0.0", "OFF", "CB", "CB FCS CHAN 1", "%0.1f"));    // elements["pnt_381"]     = default_CB_button(_("CB FCS CHAN 1, ON/OFF"),     devices.ELEC_INTERFACE, elec_commands.CB_FCS_CHAN1,     381)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3018", "382", "1.0", "ON", "0.0", "OFF", "CB", "CB FCS CHAN 2", "%0.1f"));    // elements["pnt_382"]     = default_CB_button(_("CB FCS CHAN 2, ON/OFF"),     devices.ELEC_INTERFACE, elec_commands.CB_FCS_CHAN2,     382)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3019", "383", "1.0", "ON", "0.0", "OFF", "CB", "CB SPD BRK", "%0.1f"));    // elements["pnt_383"]     = default_CB_button(_("CB SPD BRK, ON/OFF"),        devices.ELEC_INTERFACE, elec_commands.CB_SPD_BRK,       383)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3020", "384", "1.0", "ON", "0.0", "OFF", "CB", "CB LAUNCH BAR", "%0.1f"));    // elements["pnt_384"]     = default_CB_button(_("CB LAUNCH BAR, ON/OFF"),     devices.ELEC_INTERFACE, elec_commands.CB_LAUNCH_BAR,    384)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3021", "454", "1.0", "ON", "0.0", "OFF", "CB", "CB FCS CHAN 3", "%0.1f"));    // elements["pnt_454"]     = default_CB_button(_("CB FCS CHAN 3, ON/OFF"),     devices.ELEC_INTERFACE, elec_commands.CB_FCS_CHAN3,     454)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3022", "455", "1.0", "ON", "0.0", "OFF", "CB", "CB FCS CHAN 4", "%0.1f"));    // elements["pnt_455"]     = default_CB_button(_("CB FCS CHAN 4, ON/OFF"),     devices.ELEC_INTERFACE, elec_commands.CB_FCS_CHAN4,     455)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3023", "456", "1.0", "ON", "0.0", "OFF", "CB", "CB HOOK", "%0.1f"));    // elements["pnt_456"]     = default_CB_button(_("CB HOOK, ON/OFF"),           devices.ELEC_INTERFACE, elec_commands.CB_HOOK,          456)
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, "3024", "457", "1.0", "ON", "0.0", "OFF", "CB", "CB LG", "%0.1f"));    // elements["pnt_457"]     = default_CB_button(_("CB LG, ON/OFF"),             devices.ELEC_INTERFACE, elec_commands.CB_LG,            457)
            AddFunction(new Switch(this, ELEC_INTERFACE, "368", new SwitchPosition[] { new SwitchPosition("1", "1 Off", "3026", "3026", "0"), new SwitchPosition("0", "Norm", "3027"), new SwitchPosition("-1", "2 Off", "3027", "3027", "0") }, "CB", "MC Switch", "%1d")); // elements["pnt_368"] = springloaded_3_pos_tumb2(_("MC Switch, 1 OFF/NORM/2 OFF"), devices.ELEC_INTERFACE, elec_commands.MC2OffSw, elec_commands.MC1OffSw, 368)
            //AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, "3026", "368", "1.0", "1 OFF", "0.5", "NORM", "0.0", "2 OFF", "CB", "MC Switch", "%0.1f"));    
            #endregion
            #region  Power Plant
            AddFunction(Switch.CreateToggleSwitch(this, ENGINES_INTERFACE, "3001", "375", "1.0", "ON", "0.0", "OFF", "Power Plant", "APU Control Switch", "%0.1f"));
            AddFunction(new Switch(this, ENGINES_INTERFACE, "377", new SwitchPosition[] { new SwitchPosition("1", "Right", "3003", "3003", "0"), new SwitchPosition("0", "Off", "3002"), new SwitchPosition("-1", "Left", "3002", "3002", "0") }, "Power Plant", "Engine Crank Switch", "%1d"));
            AddFunction(new Switch(this, ENGINES_INTERFACE, "331", new SwitchPosition[] { new SwitchPosition("1", "Test A", "3006", "3006", "0"), new SwitchPosition("0", "Off", "3007"), new SwitchPosition("-1", "Test B", "3007", "3007", "0") }, "Power Plant", "Fire and Bleed Air Test Switch", "%1d"));
            #endregion
            #region  Hydraulic system
            AddFunction(Switch.CreateToggleSwitch(this, HYDRAULIC_INTERFACE, "3001", "369", "1.0", "NORM", "0.0", "ORIDE", "Hydraulic system", "Hydraulic Isolate Override Switch", "%0.1f"));    // elements["pnt_369"]     = default_2_position_tumb(_("Hydraulic Isolate Override Switch, NORM/ORIDE"),   devices.HYDRAULIC_INTERFACE, hydro_commands.HydIsolSw,  369)
            #endregion
            #region  Gear system
            AddFunction(Switch.CreateToggleSwitch(this, GEAR_INTERFACE, "3001", "226", "1.0", "UP", "0.0", "DOWN", "Gear system", "Landing Gear Control Handle", "%0.1f"));
            AddFunction(new PushButton(this, GEAR_INTERFACE, "3002", "228", "Gear system", "Emergency Down - Push to unlock", "1", "0", "%1d"));
            AddFunction(new PushButton(this, GEAR_INTERFACE, "3003", "229", "Gear system", "Down Lock Override Button - Push to unlock", "1", "0", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, GEAR_INTERFACE, "3004", "238", "1.0", "ON", "0.0", "OFF", "Gear system", "Anti Skid Switch", "%0.1f"));
            AddFunction(new Switch(this, GEAR_INTERFACE, "233", new SwitchPosition[] { new SwitchPosition("0.0", "Retract", "3008", "3008", null, "0.0"), new SwitchPosition("1.0", "Extend", "3008", "3008", null, "1.0") }, "Gear system", "Launch Bar Control Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, GEAR_INTERFACE, "3009", "293", "1.0", "UP", "0.0", "DOWN", "Gear system", "Arresting Hook Handle", "%0.1f"));
            AddFunction(new Switch(this, GEAR_INTERFACE, "240", new SwitchPosition[] { new SwitchPosition("0.0", "Brake Off", gear_commands.EmergParkHandleOnOff.ToString("d"), gear_commands.EmergParkHandleOnOff.ToString("d"), null, "0.0"), new SwitchPosition("1.0", "Parking Brake On", gear_commands.EmergParkHandleOnOff.ToString("d"), gear_commands.EmergParkHandleOnOff.ToString("d"), null, "1.0") }, "Gear system", "Emergency / Parking Brake Handle On/Off", "%0.1f"));
            AddFunction(new Switch(this, GEAR_INTERFACE, "241", new SwitchPosition[] { new SwitchPosition("0.999", "Release", gear_commands.EmergParkHandleSelectPark.ToString("d"), gear_commands.EmergParkHandleSelectPark.ToString("d"), "0.000", "0.000"), new SwitchPosition("0.666", "Park", gear_commands.EmergParkHandleSelectEmerg.ToString("d"), gear_commands.EmergParkHandleSelectEmerg.ToString("d"), "0.666", "0.666"), new SwitchPosition("0.000", "Emergency", gear_commands.EmergParkHandleSelectEmerg.ToString("d"), gear_commands.EmergParkHandleSelectEmerg.ToString("d"), "0.000", "0.000") }, "Gear system", "Emergency / Parking Brake Handle Park/Emergency", "%0.3f"));
            AddFunction(new Switch(this, CPT_LIGTHS, "239", new SwitchPosition[] { new SwitchPosition("1.0", "Field", "3009", "3009", null, "1.0"), new SwitchPosition("0.0", "Carrier", "3009", "3009", null, "0.0") }, "Gear system", "Hook Bypass Switch", "%0.1f"));

            #endregion
            #region  Fuel system
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_INTERFACE, "3001", "340", "1.0", "INHIBIT", "0.0", "NORM", "Fuel system", "Internal Wing Tank Fuel Control Switch", "%0.1f"));    // elements["pnt_340"]     = default_2_position_tumb(_("Internal Wing Tank Fuel Control Switch, INHIBIT/NORM"),            devices.FUEL_INTERFACE, fuel_commands.IntrWingInhibitSw,    340)
            AddFunction(new Switch(this, FUEL_INTERFACE, "341", new SwitchPosition[] { new SwitchPosition("1.0", "EXTEND", "3002"), new SwitchPosition("0.0", "RETRACT", "3002"), new SwitchPosition("-1.0", "EMERG EXTD", "3002") }, "Fuel system", "Probe Control Switch", "%0.1f"));
            AddFunction(new Switch(this, FUEL_INTERFACE, "344", new SwitchPosition[] { new SwitchPosition("1.0", "ON", "3003", "3003", null, "1.0"), new SwitchPosition("0.0", "NORM", "3003", "3003", null, "0.0") }, "Fuel system", "Fuel Dump Switch", "%0.1f"));    // elements["pnt_344"]     = default_button2(_("Fuel Dump Switch, ON/OFF"),                                                    devices.FUEL_INTERFACE, fuel_commands.DumpSw,               344, anim_speed_default)
            AddFunction(new Switch(this, FUEL_INTERFACE, "343", new SwitchPosition[] { new SwitchPosition("1.0", "STOP", "3004"), new SwitchPosition("0.0", "NORM", "3004"), new SwitchPosition("-1.0", "ORIDE", "3004") }, "Fuel system", "External Centerline Tank Fuel Control Switch", "%0.1f"));    // elements["pnt_343"]     = default_3_position_tumb(_("External Centerline Tank Fuel Control Switch, STOP/NORM/ORIDE"),   devices.FUEL_INTERFACE, fuel_commands.ExtTanksCtrSw,        343)
            AddFunction(new Switch(this, FUEL_INTERFACE, "342", new SwitchPosition[] { new SwitchPosition("1.0", "STOP", "3005"), new SwitchPosition("0.0", "NORM", "3005"), new SwitchPosition("-1.0", "ORIDE", "3005") }, "Fuel system", "External Wing Tanks Fuel Control Switch", "%0.1f"));    // elements["pnt_342"]     = default_3_position_tumb(_("External Wing Tanks Fuel Control Switch, STOP/NORM/ORIDE"),        devices.FUEL_INTERFACE, fuel_commands.ExtTanksWingSw,       342)
            #endregion
            #region  Cockpit Mechanics
            AddFunction(new Switch(this, CPT_MECHANICS, "453", new SwitchPosition[] { new SwitchPosition("1", "Open", "3001", "3001", "0"), new SwitchPosition("0", "Hold", "3002"), new SwitchPosition("-1", "Close", "3002", "3002", "0") }, "Cockpit Mechanics", "Canopy Control Switch", "%1d"));
            AddFunction(new PushButton(this, CPT_MECHANICS, "3004", "43", "Cockpit Mechanics", "Canopy Jettison Handle Unlock Button - Press to unlock", "1", "0", "%1d"));    // elements["pnt_43"]      = default_button(_("Canopy Jettison Handle Unlock Button - Press to unlock"),   devices.CPT_MECHANICS,  cpt_commands.CanopyJettLeverButton, 43)
            AddFunction(new PushButton(this, CPT_MECHANICS, "3003", "42", "Cockpit Mechanics", "Canopy Jettison Handle - Pull to jettison", "1", "0", "%1d"));    // elements["pnt_42"]      = default_2_position_tumb(_("Canopy Jettison Handle - Pull to jettison"),       devices.CPT_MECHANICS,  cpt_commands.CanopyJettLever,       42)
            AddFunction(new PushButton(this, CPT_MECHANICS, "3008", "510", "Cockpit Mechanics", "Ejection Control Handle (3 times)", "1", "0", "%1d"));    // elements["pnt_510"]     = default_button(_("Ejection Control Handle (3 times)"),                        devices.CPT_MECHANICS,  cpt_commands.SeatEjectionControlHandle,         510)
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECHANICS, "3006", "511", "1.0", "SAFE", "0.0", "ARMED", "Cockpit Mechanics", "Ejection Seat SAFE/ARMED Handle", "%0.1f"));    // elements["pnt_511"]     = default_2_position_tumb(_("Ejection Seat SAFE/ARMED Handle, SAFE/ARMED"),     devices.CPT_MECHANICS,  cpt_commands.EjectionSeatSafeArmedHandle,       511)
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECHANICS, "3009", "513", "1.0", "LOCK", "0.0", "UNLOCK", "Cockpit Mechanics", "Shoulder Harness Control Handle", "%0.1f"));    // elements["pnt_513"]     = default_2_position_tumb(_("Shoulder Harness Control Handle, LOCK/UNLOCK"),    devices.CPT_MECHANICS,  cpt_commands.ShoulderHarnessControlHandle,      513)
            AddFunction(new Switch(this, ENGINES_INTERFACE, "514", new SwitchPosition[] { new SwitchPosition("1", "Up", "3010", "3010", "0"), new SwitchPosition("0", "Hold", "3011"), new SwitchPosition("-1", "Down", "3011", "3011", "0") }, "Cockpit Mechanics", "Seat Height Adjustment Switch", "%1d")); // elements["pnt_514"]     = springloaded_3_pos_tumb(_("Seat Height Adjustment Switch, UP/HOLD/DOWN"),     devices.CPT_MECHANICS,  cpt_commands.SeatHeightAdjustmentSwitchUp, cpt_commands.SeatHeightAdjustmentSwitchDn, 514)
            //AddFunction(Switch.CreateThreeWaySwitch(this, CPT_MECHANICS, "3010", "514", "1.0", "UP", "0.5", "HOLD", "0.0", "DOWN", "Cockpit Mechanics", "Seat Height Adjustment Switch", "%0.1f"));    
            AddFunction(new PushButton(this, CPT_MECHANICS, "3012", "260", "Cockpit Mechanics", "Rudder Pedal Adjust Lever", "1", "0", "%1d"));    // elements["pnt_260"]     = default_button(_("Rudder Pedal Adjust Lever"),                                devices.CPT_MECHANICS,  cpt_commands.RudderPedalAdjustLever,    260)
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECHANICS, "3013", "575", "1", "Hide", "0", "Show", "Cockpit Mechanics", "Hide Stick toggle", "%1d"));    // elements["pnt_575"]     = default_2_position_tumb(_("Hide Stick toggle"),                               devices.CPT_MECHANICS,  cpt_commands.StickHide,                 575)
            #endregion
            #region  Mirrors
            //AddFunction(new PushButton(this,, "", "497", "Mirrors","Toggle Mirrors"));    // elements["pnt_497"]             = default_2_position_tumb(_("Toggle Mirrors"), 0, 3002, 0)
            //AddFunction(new PushButton(this,, "", "498", "Mirrors","Toggle Mirrors"));    // elements["pnt_498"]             = default_2_position_tumb(_("Toggle Mirrors"), 0, 3003, 0)
            //AddFunction(new PushButton(this,, "", "499", "Mirrors","Toggle Mirrors"));    // elements["pnt_499"]             = default_2_position_tumb(_("Toggle Mirrors"), 0, 3004, 0)
            #endregion
            #region  Exterior Lights
            AddFunction(new Axis(this, EXT_LIGHTS, "3001", "338", 0.15d, 0d, 1d, "Exterior Lights", "Position Lights Dimmer Control"));
            AddFunction(new Axis(this, EXT_LIGHTS, "3002", "337", 0.10d, 0d, 1d, "Exterior Lights", "Formation Lights Dimmer Control"));
            AddFunction(new Switch(this, EXT_LIGHTS, "339", new SwitchPosition[] { new SwitchPosition("1.0", "BRT", "3003"), new SwitchPosition("0.0", "OFF", "3003"), new SwitchPosition("-1.0", "DIM", "3003") }, "Exterior Lights", "STROBE Lights Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, EXT_LIGHTS, "3004", "237", "1.0", "ON", "0.0", "OFF", "Exterior Lights", "LDG/TAXI LIGHT Switch", "%0.1f"));
            #endregion
            #region  Cockpit Lights
            AddFunction(new Axis(this, CPT_LIGTHS, "3001", "413", 0.15d, 0d, 1d, "Cockpit Lights", "CONSOLES Lights Dimmer Control"));    // elements["pnt_413"]     = default_axis_limited(_("CONSOLES Lights Dimmer Control"),         devices.CPT_LIGTHS,     cptlights_commands.Consoles,    413, 0, 0.15)
            AddFunction(new Axis(this, CPT_LIGTHS, "3002", "414", 0.15d, 0d, 1d, "Cockpit Lights", "INST PNL Dimmer Control"));    // elements["pnt_414"]     = default_axis_limited(_("INST PNL Dimmer Control"),                devices.CPT_LIGTHS,     cptlights_commands.InstPnl,     414, 0, 0.15)
            AddFunction(new Axis(this, CPT_LIGTHS, "3003", "415", 0.15d, 0d, 1d, "Cockpit Lights", "FLOOD Light Dimmer Control"));    // elements["pnt_415"]     = default_axis_limited(_("FLOOD Light Dimmer Control"),             devices.CPT_LIGTHS,     cptlights_commands.Flood,       415, 0, 0.15)
            AddFunction(new Switch(this, CPT_LIGTHS, "419", new SwitchPosition[] { new SwitchPosition("1.0", "NVG", "3004"), new SwitchPosition("0.0", "NITE", "3004"), new SwitchPosition("-1.0", "DAY", "3004") }, "Cockpit Lights", "MODE Switch", "%0.1f"));
            AddFunction(new Axis(this, CPT_LIGTHS, "3005", "418", 0.15d, 0d, 1d, "Cockpit Lights", "CHART Light Dimmer Control"));    // elements["pnt_418"]     = default_axis_limited(_("CHART Light Dimmer Control"),             devices.CPT_LIGTHS,     cptlights_commands.Chart,       418, 0, 0.15)
            AddFunction(new Axis(this, CPT_LIGTHS, "3006", "417", 0.15d, 0d, 1d, "Cockpit Lights", "WARN/CAUTION Dimmer Control"));    // elements["pnt_417"]     = default_axis_limited(_("WARN/CAUTION Dimmer Control"),            devices.CPT_LIGTHS,     cptlights_commands.WarnCaution, 417, 0, 0.15)
            AddFunction(Switch.CreateToggleSwitch(this, CPT_LIGTHS, "3007", "416", "1.0", "TEST", "0.0", "OFF", "Cockpit Lights", "Lights Test Switch", "%0.1f"));    // elements["pnt_416"]     = springloaded_2_pos_tumb2(_("Lights Test Switch, TEST/OFF"),       devices.CPT_LIGTHS,     cptlights_commands.LtTestSw,    416)
            AddFunction(new PushButton(this, CPT_LIGTHS, "3008", "14", "Cockpit Lights", "MASTER CAUTION Reset Button - Press to reset", "1", "0", "%1d"));    // elements["pnt_14"]      = default_button(_("MASTER CAUTION Reset Button - Press to reset"), devices.CPT_LIGTHS,     cptlights_commands.MasterCaution,   14, anim_speed_def_buttons * 1.2)
            #endregion
            #region  Oxygen System
            AddFunction(Switch.CreateToggleSwitch(this, OXYGEN_INTERFACE, "3001", "365", "1.0", "ON", "0.0", "OFF", "Oxygen system", "OBOGS Control Switch", "%0.1f"));    // elements["pnt_365"]     = default_2_position_tumb(_("OBOGS Control Switch, ON/OFF"),        devices.OXYGEN_INTERFACE,   oxygen_commands.OBOGS_ControlSw,        365)
            AddFunction(new Axis(this, OXYGEN_INTERFACE, "3002", "366", 0.15d, 0d, 1d, "Oxygen system", "OXY Flow Knob"));    // elements["pnt_366"]     = default_axis_limited(_("OXY Flow Knob"),                          devices.OXYGEN_INTERFACE,   oxygen_commands.OxyFlowControlValve,    366, 1.0, 0.5, false, false, {0,1})
            #endregion
            #region  ECS
            AddFunction(new Switch(this, ECS_INTERFACE, "411", new SwitchPosition[] { new SwitchPosition("0.0", "OFF", "3001"), new SwitchPosition("0.1", "L OFF", "3001"), new SwitchPosition("0.2", "NORM", "3001"), new SwitchPosition("0.3", "R OFF", "3001") }, "ECS", "Bleed Air Knob", "%0.1f"));
            AddFunction(new PushButton(this, ECS_INTERFACE, "3002", "412", "ECS", "Bleed Air Knob AUG PULL", "1", "0", "%1d"));    // elements["pnt_412"]     = default_button(_("Bleed Air Knob, AUG PULL"),                                     devices.ECS_INTERFACE, ECS_commands.BleedAirSwAugPull, 412)
            AddFunction(new Switch(this, ECS_INTERFACE, "405", new SwitchPosition[] { new SwitchPosition("1.0", "AUTO", "3003"), new SwitchPosition("0.7", "MAN", "3003"), new SwitchPosition("0.3", "OFF", "3003"), new SwitchPosition("0.0", "RAM", "3003") }, "ECS", "ECS Mode Switch", "%0.1f"));    // elements["pnt_405"]     = default_3_position_tumb(_("ECS Mode Switch, AUTO/MAN/ OFF/RAM"),                  devices.ECS_INTERFACE, ECS_commands.ECSModeSw, 405)
            AddFunction(new Switch(this, ECS_INTERFACE, "408", new SwitchPosition[] { new SwitchPosition("1.0", "NORM", "3004"), new SwitchPosition("0.7", "DUMP", "3004"), new SwitchPosition("0.3", "RAM", "3004"), new SwitchPosition("0.0", "DUMP", "3004") }, "ECS", "Cabin Pressure Switch", "%0.1f"));    // elements["pnt_408"]     = default_3_position_tumb(_("Cabin Pressure Switch, NORM/DUMP/ RAM/DUMP"),          devices.ECS_INTERFACE, ECS_commands.CabinPressSw, 408)
            AddFunction(new Axis(this, ECS_INTERFACE, "3005", "451", 0.50d, 0d, 1d, "ECS", "Defog Handle"));    // elements["pnt_451"]     = default_axis_limited(_("Defog Handle"),                                           devices.ECS_INTERFACE, ECS_commands.DefogHandle, 451, 0.0, 0.1, true, false, {-1,1})
            AddFunction(new Axis(this, ECS_INTERFACE, "3006", "407", 0.15d, 0d, 1d, "ECS", "Cabin Temperature Knob"));    // elements["pnt_407"]     = default_axis_limited(_("Cabin Temperature Knob"),                                 devices.ECS_INTERFACE, ECS_commands.CabinTemperatureRst, 407, 0.0, 0.1, false, false, {0,1})
            AddFunction(new Axis(this, ECS_INTERFACE, "3007", "406", 0.15d, 0d, 1d, "ECS", "Suit Temperature Knob"));    // elements["pnt_406"]     = default_axis_limited(_("Suit Temperature Knob"),                                  devices.ECS_INTERFACE, ECS_commands.SuitTemperatureRst, 406, 0.0, 0.1, false, false, {0,1})
            AddFunction(Switch.CreateToggleSwitch(this, ECS_INTERFACE, "3008", "297", "1.0", "NORM", "0.0", "EMERG", "ECS", "AV COOL Switch", "%0.1f"));    // elements["pnt_297"]     = default_2_position_tumb(_("AV COOL Switch, NORM/EMERG"),                          devices.ECS_INTERFACE, ECS_commands.AV_COOL_Sw, 297)
            AddFunction(new Switch(this, ECS_INTERFACE, "452", new SwitchPosition[] { new SwitchPosition("1.0", "ANTI ICE", "3009"), new SwitchPosition("0.5", "OFF", "3009"), new SwitchPosition("0.0", "RAIN", "3009") }, "ECS", "Windshield Anti-Ice/Rain Switch", "%0.1f"));    // elements["pnt_452"]     = default_3_position_tumb(_("Windshield Anti-Ice/Rain Switch, ANTI ICE/OFF/RAIN"),  devices.ECS_INTERFACE, ECS_commands.WindshieldSw, 452)
            AddFunction(new Axis(this, ECS_INTERFACE, "3010", "505", 0.15d, 0d, 1d, "ECS", "Left Louver"));    // elements["pnt_505"]     = default_axis_limited(_("Left Louver"),                                            devices.ECS_INTERFACE, ECS_commands.LeftLouver, 505, 0.0, 0.1, false, false, {0,1})
            AddFunction(new Axis(this, ECS_INTERFACE, "3011", "506", 0.15d, 0d, 1d, "ECS", "Right Louver"));    // elements["pnt_506"]     = default_axis_limited(_("Right Louver"),                                           devices.ECS_INTERFACE, ECS_commands.RightLouver, 506, 0.0, 0.1, false, false, {0,1})
            #endregion
            #region  HOTAS
            AddFunction(Switch.CreateToggleSwitch(this, HOTAS, "3041", "494", "1.0", "ON", "0.0", "OFF", "HOTAS", "Exterior Lights Switch", "%0.1f"));    // elements["pnt_494"]         = default_2_position_tumb(_("Exterior Lights Switch, ON/OFF"),          devices.HOTAS,  hotas_commands.THROTTLE_EXTERIOR_LIGTHS,    494)
            #endregion
            #region  Master Arm Panel
            AddFunction(new PushButton(this, SMS, "3001", "458", "Master Arm Panel", "Master Mode Button A/A", "1", "0", "%1d"));
            AddFunction(new PushButton(this, SMS, "3002", "459", "Master Arm Panel", "Master Mode Button A/G", "1", "0", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, SMS, "3003", "49", "1.0", "ARM", "0.0", "SAFE", "Master Arm Panel", "Master Arm Switch", "%0.1f"));    // elements["pnt_49"]      = default_2_position_tumb(_("Master Arm Switch, ARM/SAFE"),             devices.SMS, SMS_commands.MasterArmSw, 49)
            AddFunction(new PushButton(this, SMS, "3004", "50", "Master Arm Panel", "Emergency Jettison Button", "1", "0", "%1d"));    // elements["pnt_50"]      = default_button(_("Emergency Jettison Button"),                        devices.SMS, SMS_commands.EmerJettSw, 50)
            AddFunction(Switch.CreateToggleSwitch(this, SMS, "3012", "258", "1.0", "ENABLE", "0.0", "NORM", "Master Arm Panel", "Auxiliary Release Switch", "%0.1f"));    // elements["pnt_258"]     = default_2_position_tumb(_("Auxiliary Release Switch, ENABLE/NORM"),   devices.SMS, SMS_commands.AuxRelSw, 258)
            AddFunction(new PushButton(this, SMS, "3005", "153", "Master Arm Panel", "Station Jettison Select Button CENTER", "1", "0", "%1d"));
            AddFunction(new PushButton(this, SMS, "3006", "155", "Master Arm Panel", "Station Jettison Select Button LEFT IN", "1", "0", "%1d"));
            AddFunction(new PushButton(this, SMS, "3007", "157", "Master Arm Panel", "Station Jettison Select Button LEFT OUT", "1", "0", "%1d"));
            AddFunction(new PushButton(this, SMS, "3008", "159", "Master Arm Panel", "Station Jettison Select Button RIGHT IN", "1", "0", "%1d"));
            AddFunction(new PushButton(this, SMS, "3009", "161", "Master Arm Panel", "Station Jettison Select Button RIGHT OUT", "1", "0", "%1d"));
            AddFunction(new PushButton(this, SMS, "3010", "235", "Master Arm Panel", "Selective Jettison Pushbutton"));    // elements["pnt_235"]     = default_button(_("Selective Jettison Pushbutton"),                    devices.SMS, SMS_commands.SelJettBtn, 235)
            AddFunction(new Switch(this, SMS, "236", new SwitchPosition[] { new SwitchPosition("-0.1", "L FUS MSL", "3011"), new SwitchPosition("0.0", "SAFE", "3011"), new SwitchPosition("0.1", "R FUS MSL", "3011"), new SwitchPosition("0.2", "Rack / LCHR", "3011"), new SwitchPosition("0.3", "STORES", "3011") }, "Master Arm Panel", "Selective Jettison Knob", "%0.1f"));    // elements["pnt_236"]     = multiposition_switch(_("Selective Jettison Knob, L FUS MSL/SAFE/R FUS MSL/ RACK/LCHR /STORES"),   devices.SMS, SMS_commands.SelJettLvr, 236, 5, 0.1, false, -0.1, 1.5)
            AddFunction(new Switch(this, SMS, "135", new SwitchPosition[] { new SwitchPosition("0.2", "ORIDE", "3013"), new SwitchPosition("0.1", "NORM", "3013"), new SwitchPosition("0.0", "OFF", "3013") }, "Master Arm Panel", "IR Cooling Switch", "%0.1f"));    // elements["pnt_135"]     = default_3_position_tumb(_("IR Cooling Switch, ORIDE/NORM/OFF"),           devices.SMS, SMS_commands.IRCoolingSw, 135, false, anim_speed_default, false, 0.1, {0, 0.2})
            #endregion
            #region  Fire Systems
            AddFunction(new PushButton(this, ENGINES_INTERFACE, "3008", "46", "Fire Systems", "Fire Extinguisher Pushbutton", "1", "0", "%1d"));    // elements["pnt_46"]      = default_button(_("Fire Extinguisher Pushbutton"),                     devices.ENGINES_INTERFACE, engines_commands.FireExtghDischSw, 46)
            AddFunction(new PushButton(this, ENGINES_INTERFACE, "3009", "30", "Fire Systems", "APU Fire Warning/Extinguisher Light", "1", "0", "%1d"));    // elements["pnt_30"]      = default_2_position_tumb(_("APU Fire Warning/Extinguisher Light"),     devices.ENGINES_INTERFACE, engines_commands.APU_FireSw, 30)
            AddFunction(new PushButton(this, ENGINES_INTERFACE, "3010", "11", "Fire Systems", "Left Engine/AMAD Fire Warning/Extinguisher Light - (LMB) depress/(RMB) cover control", "1", "0", "%1d"));    // elements["pnt_11"]      = default_double_tumb(_("Left Engine/AMAD Fire Warning/Extinguisher Light - (LMB) depress/(RMB) cover control"),    devices.ENGINES_INTERFACE, engines_commands.LENG_FireSw, engines_commands.LENG_FireSwCover, 11, 12)
            AddFunction(new PushButton(this, ENGINES_INTERFACE, "3011", "27", "Fire Systems", "Right Engine/AMAD Fire Warning/Extinguisher Light - (LMB) depress/(RMB) cover control", "1", "0", "%1d"));    // elements["pnt_27"]      = default_double_tumb(_("Right Engine/AMAD Fire Warning/Extinguisher Light - (LMB) depress/(RMB) cover control"),   devices.ENGINES_INTERFACE, engines_commands.RENG_FireSw, engines_commands.RENG_FireSwCover, 27, 28)
            #endregion
            #region  Multipurpose Display Group
            #endregion
            #region  Head-Up Display
            AddFunction(new Switch(this, HUD, "140", new SwitchPosition[] { new SwitchPosition("0.2", "NORM", "3001"), new SwitchPosition("0.1", "REJ 1", "3001"), new SwitchPosition("0.0", "REJ 2", "3001") }, "Head-Up Display", "HUD Symbology Reject Switch", "%0.1f"));
            AddFunction(new Axis(this, HUD, "3002", "141", 0.15d, 0d, 1d, "Head-Up Display", "HUD Symbology Brightness Control Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, HUD, "3003", "142", "1.0", "DAY", "0.0", "NIGHT", "Head-Up Display", "HUD Symbology Brightness Selector Switch", "%0.1f"));
            AddFunction(new Axis(this, HUD, "3004", "143", 0.15d, 0d, 1d, "Head-Up Display", "Black Level Control Knob"));
            AddFunction(new Switch(this, HUD, "144", new SwitchPosition[] { new SwitchPosition("0.2", "W/B", "3005"), new SwitchPosition("0.1", "VID", "3005"), new SwitchPosition("0.0", "OFF", "3005") }, "Head-Up Display", "HUD Video Control Switch", "%0.1f"));
            AddFunction(new Axis(this, HUD, "3006", "145", 0.15d, 0d, 1d, "Head-Up Display", "Balance Control Knob"));
            AddFunction(new Axis(this, HUD, "3007", "146", 0.15d, 0d, 1d, "Head-Up Display", "AOA Indexer Control Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, HUD, "3008", "147", "1.0", "BARO", "0.0", "RDR", "Head-Up Display", "Altitude Switch", "%0.1f"));
            AddFunction(new Switch(this, HUD, "148", new SwitchPosition[] { new SwitchPosition("1.0", "INS", "3009"), new SwitchPosition("0.0", "AUTO", "3009"), new SwitchPosition("-1.0", "STBY", "3009") }, "Head-Up Display", "Attitude Selector Switch", "%0.1f"));
            #endregion
            #region  Left MDI
            AddFunction(new Switch(this, MDI_LEFT, "51", new SwitchPosition[] { new SwitchPosition("0.0", "OFF", "3001"), new SwitchPosition("0.1", "NIGHT", "3001"), new SwitchPosition("0.2", "DAY", "3001") }, "Left MDI", "Left MDI Brightness Selector Knob", "%0.1f"));    // elements["pnt_51"]      = default_3_position_tumb(_("Left MDI Brightness Selector Knob, OFF/NIGHT/DAY"),    devices.MDI_LEFT, MDI_commands.MDI_off_night_day, 51, false, anim_speed_default, false, 0.1, {0, 0.2})
            AddFunction(new Axis(this, MDI_LEFT, "3002", "52", 0.15d, 0d, 1d, "Left MDI", "Left MDI Brightness Control Knob"));    // elements["pnt_52"]      = default_axis_limited(_("Left MDI Brightness Control Knob"),                       devices.MDI_LEFT, MDI_commands.MDI_brightness, 52, 0.0, 0.1, false, false, {0, 1})
            AddFunction(new Axis(this, MDI_LEFT, "3003", "53", 0.15d, 0d, 1d, "Left MDI", "Left MDI Contrast Control Knob"));    // elements["pnt_53"]      = default_axis_limited(_("Left MDI Contrast Control Knob"),                         devices.MDI_LEFT, MDI_commands.MDI_contrast, 53, 0.0, 0.1, false, false, {0, 1})
            AddFunction(new PushButton(this, MDI_LEFT, "3011", "54", "Left MDI", "OSB 01", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3012", "55", "Left MDI", "OSB 02", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3013", "56", "Left MDI", "OSB 03", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3014", "57", "Left MDI", "OSB 04", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3015", "58", "Left MDI", "OSB 05", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3016", "59", "Left MDI", "OSB 06", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3017", "60", "Left MDI", "OSB 07", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3018", "61", "Left MDI", "OSB 08", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3019", "62", "Left MDI", "OSB 09", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3020", "63", "Left MDI", "OSB 10", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3021", "64", "Left MDI", "OSB 11", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3022", "65", "Left MDI", "OSB 12", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3023", "66", "Left MDI", "OSB 13", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3024", "67", "Left MDI", "OSB 14", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3025", "68", "Left MDI", "OSB 15", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3026", "69", "Left MDI", "OSB 16", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3027", "70", "Left MDI", "OSB 17", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3028", "72", "Left MDI", "OSB 18", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3029", "73", "Left MDI", "OSB 19", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_LEFT, "3030", "75", "Left MDI", "OSB 20", "1", "0", "%1d"));
            #endregion
            #region  Right MDI
            AddFunction(new Switch(this, MDI_RIGHT, "76", new SwitchPosition[] { new SwitchPosition("0.0", "OFF", "3001"), new SwitchPosition("0.1", "NIGHT", "3001"), new SwitchPosition("0.2", "DAY", "3001") }, "Right MDI", "Right MDI Brightness Selector Knob", "%0.1f"));    // elements["pnt_76"]      = default_3_position_tumb(_("Right MDI Brightness Selector Knob, OFF/NIGHT/DAY"),   devices.MDI_RIGHT, MDI_commands.MDI_off_night_day, 76, false, anim_speed_default, false, 0.1, {0, 0.2})
            AddFunction(new Axis(this, MDI_RIGHT, "3002", "77", 0.15d, 0d, 1d, "Right MDI", "Right MDI Brightness Control Knob"));    // elements["pnt_77"]      = default_axis_limited(_("Right MDI Brightness Control Knob"),                      devices.MDI_RIGHT, MDI_commands.MDI_brightness, 77, 0.0, 0.1, false, false, {0, 1})
            AddFunction(new Axis(this, MDI_RIGHT, "3003", "78", 0.15d, 0d, 1d, "Right MDI", "Right MDI Contrast Control Knob"));    // elements["pnt_78"]      = default_axis_limited(_("Right MDI Contrast Control Knob"),                        devices.MDI_RIGHT, MDI_commands.MDI_contrast, 78, 0.0, 0.1, false, false, {0, 1})
            AddFunction(new PushButton(this, MDI_RIGHT, "3011", "79", "Right MDI", "OSB 01", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3012", "80", "Right MDI", "OSB 02", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3013", "81", "Right MDI", "OSB 03", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3014", "82", "Right MDI", "OSB 04", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3015", "83", "Right MDI", "OSB 05", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3016", "84", "Right MDI", "OSB 06", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3017", "85", "Right MDI", "OSB 07", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3018", "86", "Right MDI", "OSB 08", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3019", "87", "Right MDI", "OSB 09", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3020", "88", "Right MDI", "OSB 10", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3021", "89", "Right MDI", "OSB 11", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3022", "90", "Right MDI", "OSB 12", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3023", "91", "Right MDI", "OSB 13", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3024", "92", "Right MDI", "OSB 14", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3025", "93", "Right MDI", "OSB 15", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3026", "94", "Right MDI", "OSB 16", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3027", "95", "Right MDI", "OSB 17", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3028", "96", "Right MDI", "OSB 18", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3029", "97", "Right MDI", "OSB 19", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MDI_RIGHT, "3030", "98", "Right MDI", "OSB 20", "1", "0", "%1d"));
            #endregion
            #region  AMPCD
            AddFunction(new Axis(this, AMPCD, "3001", "203", 0.15d, 0d, 1d, "AMPCD", "AMPCD Off/Brightness Control Knob"));    // elements["pnt_203"]     = default_axis_limited(_("AMPCD Off/Brightness Control Knob"),          devices.AMPCD, AMPCD_commands.AMPCD_off_brightness, 203, 0.0, 0.1, false, false, {0, 1})
            AddFunction(new Switch(this, AMPCD, "177", new SwitchPosition[] { new SwitchPosition("1", "Day", "3002", "3002", "0"), new SwitchPosition("0", "Off", "3002"), new SwitchPosition("-1", "Night", "3003", "3003", "0") }, "AMPCD", "AMPCD Night/Day Brightness Selector DAY", "%1d"));
            AddFunction(new Switch(this, AMPCD, "179", new SwitchPosition[] { new SwitchPosition("1", "Up", "3004", "3004", "0"), new SwitchPosition("0", "Off", "3004"), new SwitchPosition("-1", "Down", "3005", "3005", "0") }, "AMPCD", "AMPCD Symbology Control Switch UP", "%1d"));
            AddFunction(new Switch(this, AMPCD, "180", new SwitchPosition[] { new SwitchPosition("1", "Up", "3008", "3008", "0"), new SwitchPosition("0", "Off", "3008"), new SwitchPosition("-1", "Down", "3009", "3009", "0") }, "AMPCD", "AMPCD Gain Control Switch UP", "%1d"));
            AddFunction(new Switch(this, AMPCD, "182", new SwitchPosition[] { new SwitchPosition("1", "Up", "3006", "3006", "0"), new SwitchPosition("0", "Off", "3006"), new SwitchPosition("-1", "Down", "3007", "3007", "0") }, "AMPCD", "AMPCD Contrast Control Switch UP", "%1d"));
            //AddFunction(Switch.CreateThreeWaySwitch(this, AMPCD, "3002", "177", "1.0", "Day", "0.0", "NORM", "-1.0", "Night", "AMPCD", "AMPCD Night/Day Brightness Selector DAY", "%0.1f"));    // elements["pnt_177_2"]   = AMPCD_switch_positive(_("AMPCD Night/Day Brightness Selector, DAY"),  devices.AMPCD, AMPCD_commands.AMPCD_nite_day_DAY, 177)
            //AddFunction(Switch.CreateThreeWaySwitch(this, AMPCD, "3004", "179", "1.0", "Up", "0.0", "Off", "-1.0", "Down", "AMPCD", "AMPCD Symbology Control Switch UP", "%0.1f"));    // elements["pnt_179_2"]   = AMPCD_switch_positive(_("AMPCD Symbology Control Switch, UP"),        devices.AMPCD, AMPCD_commands.AMPCD_symbology_UP, 179)
            //AddFunction(Switch.CreateThreeWaySwitch(this, AMPCD, "3006", "182", "1.0", "up", "0.0", "Off", "-1.0", "Down", "AMPCD", "AMPCD Contrast Control Switch UP", "%0.1f"));    // elements["pnt_182_2"]   = AMPCD_switch_positive(_("AMPCD Contrast Control Switch, UP"),         devices.AMPCD, AMPCD_commands.AMPCD_contrast_UP, 182)
            //AddFunction(Switch.CreateThreeWaySwitch(this, AMPCD, "3008", "180", "1.0", "Up", "0.0", "Off", "-1.0", "Down", "AMPCD", "AMPCD Gain Control Switch UP", "%0.1f"));    // elements["pnt_180_2"]   = AMPCD_switch_positive(_("AMPCD Gain Control Switch, UP"),             devices.AMPCD, AMPCD_commands.AMPCD_gain_UP, 180)
            AddFunction(new PushButton(this, AMPCD, "3011", "183", "AMPCD", "OSB 01", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3012", "184", "AMPCD", "OSB 02", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3013", "185", "AMPCD", "OSB 03", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3014", "186", "AMPCD", "OSB 04", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3015", "187", "AMPCD", "OSB 05", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3016", "188", "AMPCD", "OSB 06", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3017", "189", "AMPCD", "OSB 07", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3018", "190", "AMPCD", "OSB 08", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3019", "191", "AMPCD", "OSB 09", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3020", "192", "AMPCD", "OSB 10", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3021", "193", "AMPCD", "OSB 11", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3022", "194", "AMPCD", "OSB 12", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3023", "195", "AMPCD", "OSB 13", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3024", "196", "AMPCD", "OSB 14", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3025", "197", "AMPCD", "OSB 15", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3026", "198", "AMPCD", "OSB 16", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3027", "199", "AMPCD", "OSB 17", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3028", "200", "AMPCD", "OSB 18", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3029", "201", "AMPCD", "OSB 19", "1", "0", "%1d"));
            AddFunction(new PushButton(this, AMPCD, "3030", "202", "AMPCD", "OSB 20", "1", "0", "%1d"));
            AddFunction(new Switch(this, MDI_LEFT, "312", new SwitchPosition[] { new SwitchPosition("1", "+ve", "3004", "3004", "0"), new SwitchPosition("0", "Off", "3005"), new SwitchPosition("-1", "-ve", "3005", "3005", "0") }, "AMPCD", "Heading Set Switch", "%1d")); // elements["pnt_312"]     = springloaded_3_pos_tumb2(_("Heading Set Switch"),devices.MDI_LEFT, MDI_commands.MDI_Left_HDG_Negative, MDI_commands.MDI_Left_HDG_Positive, 312)
            AddFunction(new Switch(this, MDI_LEFT, "313", new SwitchPosition[] { new SwitchPosition("1", "+ve", "3006", "3006", "0"), new SwitchPosition("0", "Off", "3007"), new SwitchPosition("-1", "-ve", "3007", "3007", "0") }, "AMPCD", "Course Set Switch", "%1d"));  // elements["pnt_313"]= springloaded_3_pos_tumb(_("Course Set Switch"),devices.MDI_LEFT, MDI_commands.MDI_Left_CRS_Negative, MDI_commands.MDI_Left_CRS_Positive, 313)
            #endregion
            #region  Integrated Fuel/Engine Indicator (IFEI)
            AddFunction(new PushButton(this, IFEI, "3001", "168", "IFEI", "IFEI Mode Button", "1", "0", "%1d"));
            AddFunction(new PushButton(this, IFEI, "3002", "169", "IFEI", "IFEI QTY Button", "1", "0", "%1d"));
            AddFunction(new PushButton(this, IFEI, "3003", "170", "IFEI", "IFEI Up Arrow Button", "1", "0", "%1d"));
            AddFunction(new PushButton(this, IFEI, "3004", "171", "IFEI", "IFEI Down Arrow Button", "1", "0", "%1d"));
            AddFunction(new PushButton(this, IFEI, "3005", "172", "IFEI", "IFEI ZONE Button", "1", "0", "%1d"));
            AddFunction(new PushButton(this, IFEI, "3006", "173", "IFEI", "IFEI ET Button", "1", "0", "%1d"));
            AddFunction(new Axis(this, IFEI, "3007", "174", 0.1d, 0.5d, 1d, "IFEI", "IFEI Brightness Control Knob"));
            AddFunction(new FlagValue(this, "468", "IFEI", "IFEI", ""));
            AddFunction(new FlagValue(this, "469", "IFEI", "IFEI buttons", ""));

            AddFunction(new Common.Text(this, "2052", "IFEI", "Bingo Value", "Value of the BINGO fuel state"));
            AddFunction(new Common.Text(this, "2053", "IFEI", "Clock hours", "Value of the clock HH"));
            AddFunction(new Common.Text(this, "2054", "IFEI", "Clock minutes", "Value of the clock MM"));
            AddFunction(new Common.Text(this, "2055", "IFEI", "Clock seconds", "Value of the clock SS"));
            AddFunction(new FlagValue(this, "2056", "IFEI", "Clock HH MM separator", "Flag to display colon HH:MM on IFEI clock display", null));
            AddFunction(new FlagValue(this, "2057", "IFEI", "Clock MM SS separator", "Flag to display colon MM:SS on IFEI clock display", null));
            AddFunction(new Common.Text(this, "2061", "IFEI", "Left Fuel Flow Value", "Value of the Fuel Flow for the Left Engine"));
            AddFunction(new Common.Text(this, "2062", "IFEI", "Right Fuel Flow Value", "Value of the Fuel Flow for the Right Engine"));
            AddFunction(new Common.Text(this, "2063", "IFEI", "Internal Fuel Amount", "Internel Fuel Value"));
            AddFunction(new Common.Text(this, "2064", "IFEI", "Total Fuel Amount", "Total Fuel Value"));
            AddFunction(new Common.Text(this, "2065", "IFEI", "Left Oil Pressure", "Value of the Left Engine Oil Pressure"));
            AddFunction(new Common.Text(this, "2066", "IFEI", "Right Oil Pressure", "Value of the Right Engine Oil Pressure"));
            AddFunction(new Common.Text(this, "2067", "IFEI", "Left RPM Value", "Left Engine RPM"));
            AddFunction(new Common.Text(this, "2068", "IFEI", "Right RPM Value", "Right Engine RPM"));
            AddFunction(new Common.Text(this, "2069", "IFEI", "Left Temperature Value", "Left Engine Temperature"));
            AddFunction(new Common.Text(this, "2070", "IFEI", "Right Temperature Value", "Right Engine Temperature"));
            AddFunction(new Common.Text(this, "2073", "IFEI", "Timer hours", "Value of the Timer Hours"));
            AddFunction(new Common.Text(this, "2072", "IFEI", "Timer minutes", "Value of the Timer Minutes"));
            AddFunction(new Common.Text(this, "2071", "IFEI", "Timer seconds", "Value of the Timer Seconds"));
            AddFunction(new FlagValue(this, "2058", "IFEI", "Timer H MM separator", "Flag to display colon HH:MM on IFEI timer display", null));
            // 2059 is used for the altimeter 
            AddFunction(new FlagValue(this, "2060", "IFEI", "Timer MM SS separator", "Flag to display colon MM:SS on IFEI timer display", null));
            AddFunction(new Common.Text(this, "2074", "IFEI", "SP Code", "Value of the code before the SP"));
            AddFunction(new Common.Text(this, "2075", "IFEI", "SP", "Value of SP"));
            AddFunction(new Common.Text(this, "2076", "IFEI", "Draw Character", "Draw Character Not sure what this is"));
            AddFunction(new Common.Text(this, "2077", "IFEI", "T Value", "T Value"));
            AddFunction(new Common.Text(this, "2078", "IFEI", "Time Set Mode", "Alter / Set Clock Mode"));

            // These are described as textures, but currently unclear what these actually represent.
            uint commandCode = 4000;
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "RPM Flag", "Show RPM on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Temp Flag", "Show Temp on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "FF Flag", "Show FF on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Noz Flag", "Show Noz on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Oil Flag", "Show Oil on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Bingo Flag", "Show Bingo on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Left Scale Flag", "Show Left Scale on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Right Scale Flag", "Show Right Scale on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Left Scale 0 Flag", "Show Left Scale 0 value on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Right Scale 0 Flag", "Show Right Scale 0 value on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Left Scale 50 Flag", "Show Left Scale 50 value on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Right Scale 50 Flag", "Show Right Scale 50 value on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Left Scale 100 Flag", "Show Left Scale 10 value on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Right Scale 100 Flag", "Show Right Scale 10 value on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Left Nozzle Needle Flag", "Left nozzle needle on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Right Nozzle Needle Flag", "Right nozzle needle on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Zulu Time Flag", "Z flag indicating Zulu time on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Left Fuel Flag", "L flag indicating Left fuel quantity on IFEI", null));
            AddFunction(new FlagValue(this, commandCode++.ToString(), "IFEI", "Right Fuel Flag", "R flag indicating Right fuel quantity on IFEI", null));
            AddFunction(new Axis(this, IFEI, commandCode.ToString(), commandCode++.ToString(), 1d, 0d, 400d, "IFEI", "Left Nozzle Position", false, null));
            AddFunction(new Axis(this, IFEI, commandCode.ToString(), commandCode++.ToString(), 1d, 0d, 400d, "IFEI", "Right Nozzle Position", false, null));

            #endregion
            #region  Sensor panel
            AddFunction(new Switch(this, RADAR, "440", new SwitchPosition[] { new SwitchPosition("0.0", "OFF", "3001"), new SwitchPosition("0.1", "STBY", "3001"), new SwitchPosition("0.2", "OPR", "3001"), new SwitchPosition("0.3", "EMERG(PULL)", "3002") }, "Sensor panel", "RADAR Switch (MW to pull)", "%0.1f"));    // elements["pnt_440"] = multiposition_switch_with_pull(_("RADAR Switch (MW to pull), OFF/STBY/OPR/EMERG(PULL)"),
            #endregion
            #region  INS
            AddFunction(new Switch(this, INS, "443", SwitchPositions.Create(8, 0.0, 0.1, "3001", new string[] { "OFF", "CV", "GND", "NAV", "IFA", "GYRO", "GB", "TEST" }), "INS", "Inertial Navigation System Switch", "%0.1f"));
            #endregion

            #region  UFC
            AddFunction(new PushButton(this, UFC, "3001", "128", "UFC", "UFC Function Selector Pushbutton A/P", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3002", "129", "UFC", "UFC Function Selector Pushbutton IFF", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3003", "130", "UFC", "UFC Function Selector Pushbutton TCN", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3004", "131", "UFC", "UFC Function Selector Pushbutton ILS", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3005", "132", "UFC", "UFC Function Selector Pushbutton D/L", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3006", "133", "UFC", "UFC Function Selector Pushbutton BCN", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3007", "134", "UFC", "UFC Function Selector Pushbutton ON/OFF", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3010", "100", "UFC", "UFC Option Select Pushbutton 1", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3011", "101", "UFC", "UFC Option Select Pushbutton 2", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3012", "102", "UFC", "UFC Option Select Pushbutton 3", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3013", "103", "UFC", "UFC Option Select Pushbutton 4", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3014", "106", "UFC", "UFC Option Select Pushbutton 5", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3019", "111", "UFC", "UFC Keyboard Pushbutton 1", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3020", "112", "UFC", "UFC Keyboard Pushbutton 2", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3021", "113", "UFC", "UFC Keyboard Pushbutton 3", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3022", "114", "UFC", "UFC Keyboard Pushbutton 4", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3023", "115", "UFC", "UFC Keyboard Pushbutton 5", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3024", "116", "UFC", "UFC Keyboard Pushbutton 6", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3025", "117", "UFC", "UFC Keyboard Pushbutton 7", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3026", "118", "UFC", "UFC Keyboard Pushbutton 8", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3027", "119", "UFC", "UFC Keyboard Pushbutton 9", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3018", "120", "UFC", "UFC Keyboard Pushbutton 0", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3028", "121", "UFC", "UFC Keyboard Pushbutton CLR", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3029", "122", "UFC", "UFC Keyboard Pushbutton ENT", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3015", "99", "UFC", "UFC I/P Pushbutton", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3017", "110", "UFC", "UFC Emission Control Pushbutton", "1", "0", "%1d"));
            AddFunction(new Switch(this, UFC, "107", new SwitchPosition[] { new SwitchPosition("1.0", "1", "3016"), new SwitchPosition("0.5", "OFF", "3016"), new SwitchPosition("0.0", "2", "3016") }, "UFC", "UFC ADF Function Select Switch", "%0.1f"));
            AddFunction(new Axis(this, UFC, "3030", "108", 0.1d, 0d, 1d, "UFC", "UFC COMM 1 Volume Control Knob"));
            AddFunction(new Axis(this, UFC, "3031", "123", 0.1d, 0d, 1d, "UFC", "UFC COMM 2 Volume Control Knob"));
            AddFunction(new Axis(this, UFC, "3032", "109", 0.1d, 0d, 1d, "UFC", "UFC Brightness Control Knob"));
            AddFunction(new RotaryEncoder(this, UFC, "3033", "124", 0.1d, "UFC", "UFC COMM 1 Channel Selector Knob"));
            AddFunction(new RotaryEncoder(this, UFC, "3034", "126", 0.1d, "UFC", "UFC COMM 2 Channel Selector Knob"));
            AddFunction(new PushButton(this, UFC, "3008", "125", "UFC", "UFC COMM 1 Channel Selector Pull", "1", "0", "%1d"));
            AddFunction(new PushButton(this, UFC, "3009", "127", "UFC", "UFC COMM 2 Channel Selector Pull", "1", "0", "%1d"));

            AddFunction(new Common.Text(this, "2080", "UFC", "UFC Unsure 1", "Value of the (unsure!)"));
            AddFunction(new Common.Text(this, "2081", "UFC", "UFC Mask", "Value of the UFC Mask"));
            AddFunction(new Common.Text(this, "2082", "UFC", "Option Display 1", "Text Value of the Option Display #1"));
            AddFunction(new Common.Text(this, "2083", "UFC", "Option Display 2", "Text Value of the Option Display #2"));
            AddFunction(new Common.Text(this, "2084", "UFC", "Option Display 3", "Text Value of the Option Display #3"));
            AddFunction(new Common.Text(this, "2085", "UFC", "Option Display 4", "Text Value of the Option Display #4"));
            AddFunction(new Common.Text(this, "2086", "UFC", "Option Display 5", "Text Value of the Option Display #5"));
            AddFunction(new Common.Text(this, "2087", "UFC", "Option Display 1 Selected", "Option Display #1 Selected"));
            AddFunction(new Common.Text(this, "2088", "UFC", "Option Display 2 Selected", "Option Display #2 Selected"));
            AddFunction(new Common.Text(this, "2089", "UFC", "Option Display 3 Selected", "Option Display #3 Selected"));
            AddFunction(new Common.Text(this, "2090", "UFC", "Option Display 4 Selected", "Option Display #4 Selected"));
            AddFunction(new Common.Text(this, "2091", "UFC", "Option Display 5 Selected", "Option Display #5 Selected"));
            AddFunction(new Common.Text(this, "2092", "UFC", "Scratchpad 1", "Value of the first scratchpad display"));
            AddFunction(new Common.Text(this, "2093", "UFC", "Scratchpad 2", "Value of the second scratchpad display"));
            AddFunction(new Common.Text(this, "2094", "UFC", "Scratchpad Number", "Value of the scratchpad number display"));
            AddFunction(new Common.Text(this, "2095", "UFC", "Comm Channel 1", "Value of Communications Channel 1 display"));
            AddFunction(new Common.Text(this, "2096", "UFC", "Comm Channel 2", "Value of Communications Channel 2 display"));

            #endregion
            #region  Intercom
            AddFunction(new Axis(this, INTERCOM, "3002", "357", 0.1d, 0d, 1d, "Intercom", "VOX Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3003", "358", 0.1d, 0d, 1d, "Intercom", "ICS Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3004", "359", 0.1d, 0d, 1d, "Intercom", "RWR Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3005", "360", 0.1d, 0d, 1d, "Intercom", "WPN Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3007", "361", 0.1d, 0d, 1d, "Intercom", "MIDS B Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3006", "362", 0.1d, 0d, 1d, "Intercom", "MIDS A Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3008", "363", 0.1d, 0d, 1d, "Intercom", "TACAN Volume Control Knob"));
            AddFunction(new Axis(this, INTERCOM, "3009", "364", 0.1d, 0d, 1d, "Intercom", "AUX Volume Control Knob"));
            AddFunction(new Switch(this, INTERCOM, "350", new SwitchPosition[] { new SwitchPosition("1.0", "CIPHER", "3010"), new SwitchPosition("0.5", "OFF", "3010"), new SwitchPosition("0.0", "PLAIN", "3010") }, "Intercom", "Comm Relay Switch", "%0.1f"));    // elements["pnt_350"]     = default_3_position_tumb(_("Comm Relay Switch, CIPHER/OFF/PLAIN"),             devices.INTERCOM, Intercom_commands.COMM_RLY_Sw,    350,    false, anim_speed_default, false)
            AddFunction(new Switch(this, INTERCOM, "351", new SwitchPosition[] { new SwitchPosition("1.0", "COMM 1", "3011"), new SwitchPosition("0.5", "OFF", "3011"), new SwitchPosition("0.0", "COMM 2", "3011") }, "Intercom", "COMM G XMT Switch", "%0.1f"));    // elements["pnt_351"]     = default_3_position_tumb(_("COMM G XMT Switch, COMM 1/OFF/COMM 2"),            devices.INTERCOM, Intercom_commands.G_XMT_Sw,       351,    false, anim_speed_default, false)
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, "3012", "356", "1.0", "EMER", "0.0", "NORM", "Intercom", "IFF Master Switch", "%0.1f"));    // elements["pnt_356"]     = default_2_position_tumb(_("IFF Master Switch, EMER/NORM"),                    devices.INTERCOM, Intercom_commands.IFF_MasterSw,   356)
            AddFunction(new Switch(this, INTERCOM, "355", new SwitchPosition[] { new SwitchPosition("1.0", "DIS", "3013"), new SwitchPosition("0.7", "AUD", "3013"), new SwitchPosition("0.3", "DIS", "3013"), new SwitchPosition("0.0", "OFF", "3013") }, "Intercom", "IFF Mode 4 Switch", "%0.1f"));    // elements["pnt_355"]     = default_3_position_tumb(_("IFF Mode 4 Switch, DIS/AUD /DIS/OFF"),             devices.INTERCOM, Intercom_commands.IFF_Mode4Sw,    355,    false, anim_speed_default, false)
            AddFunction(new Switch(this, INTERCOM, "354", new SwitchPosition[] { new SwitchPosition("1", "Hold", "3014", "3014", "0"), new SwitchPosition("0", "Norm", "3015"), new SwitchPosition("-1", "Zero", "3015", "3015", "0") }, "Intercom", "CRYPTO Switch", "%1d"));  // elements["pnt_354"]     = springloaded_3_pos_tumb2(_("CRYPTO Switch, HOLD/NORM/ZERO"),                  devices.INTERCOM, Intercom_commands.IFF_CryptoSw_Zero, Intercom_commands.IFF_CryptoSw_Hold, 354,    anim_speed_default)
            //AddFunction(Switch.CreateThreeWaySwitch(this, INTERCOM, "3014", "354", "1.0", "HOLD", "0.5", "NORM", "0.0", "ZERO", "Intercom", "CRYPTO Switch", "%0.1f"));    
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, "3016", "353", "1.0", "UFC", "0.0", "MAN", "Intercom", "ILS UFC/MAN Switch", "%0.1f"));    // elements["pnt_353"]     = default_2_position_tumb(_("ILS UFC/MAN Switch, UFC/MAN"),                     devices.INTERCOM, Intercom_commands.ILS_UFC_MAN_Sw, 353)
            AddFunction(new Switch(this, INTERCOM, "352", new SwitchPosition[] { }, "Intercom", "ILS Channel Selector Switch", "%0.1f"));    // elements["pnt_352"]     = multiposition_switch(_("ILS Channel Selector Switch"),                        devices.INTERCOM, Intercom_commands.ILS_ChannelSelector,    352, 20, 0.05, false, 0.0, anim_speed_default * 0.05, false)
            AddFunction(new Switch(this, KY58, "444", new SwitchPosition[] { new SwitchPosition("1.0", "P", "3001"), new SwitchPosition("0.7", "C", "3001"), new SwitchPosition("0.3", "LD", "3001"), new SwitchPosition("0.0", "RV", "3001") }, "Intercom", "KY-58 Mode Select Knob", "%0.1f"));    // elements["pnt_444"]     = multiposition_switch(_("KY-58 Mode Select Knob, P/C/LD/RV"),                  devices.KY58, ky58_commands.KY58_ModeSw,            444, 4, 0.1, false, 0.0, anim_speed_default * 0.1, false)
            AddFunction(new Axis(this, KY58, "3005", "445", 0.15d, 0d, 1d, "Intercom", "KY-58 Volume Control Knob"));    // elements["pnt_445"]     = default_axis_limited(_("KY-58 Volume Control Knob"),                          devices.KY58, ky58_commands.KY58_Volume,            445, 0.0, 0.1, false, false, {0,1})
            AddFunction(new Switch(this, KY58, "446", new SwitchPosition[] { new SwitchPosition("1.0", "Z 1-5", "3002"), new SwitchPosition("0.9", "1", "3002"), new SwitchPosition("0.7", "2", "3002"), new SwitchPosition("0.6", "3", "3002"), new SwitchPosition("0.4", "4", "3002"), new SwitchPosition("0.3", "5", "3002"), new SwitchPosition("0.1", "6", "3002"), new SwitchPosition("0.0", "Z ALL", "3002") }, "Intercom", "KY-58 Fill Select Knob", "%0.1f"));    // elements["pnt_446"]     = multiposition_switch(_("KY-58 Fill Select Knob, Z 1-5/1/2/3/4/5/6/Z ALL"),    devices.KY58, ky58_commands.KY58_FillSw,            446, 8, 0.1, false, 0.0, anim_speed_default * 0.1, false)
            AddFunction(new Switch(this, KY58, "447", new SwitchPosition[] { new SwitchPosition("1.0", "OFF", "3004"), new SwitchPosition("0.5", "ON", "3004"), new SwitchPosition("0.0", "TD", "3004") }, "Intercom", "KY-58 Power Select Knob", "%0.1f"));    // elements["pnt_447"]     = multiposition_switch(_("KY-58 Power Select Knob, OFF/ON/TD"),                 devices.KY58, ky58_commands.KY58_PowerSw,           447, 3, 0.1, false, 0.0, anim_speed_default * 0.1, false)
            AddFunction(new PushButton(this, INTERCOM, "3018", "230", "Intercom", "Warning Tone Silence Button - Push to silence", "1", "0", "%1d"));    // elements["pnt_230"]     = default_button(_("Warning Tone Silence Button - Push to silence"),            devices.INTERCOM, Intercom_commands.WarnToneSilBtn, 230)
            #endregion
            #region  antenna selector
            AddFunction(new Switch(this, ANTENNA_SELECTOR, "373", new SwitchPosition[] { new SwitchPosition("1.0", "UPPER", "3001"), new SwitchPosition("0.5", "AUTO", "3001"), new SwitchPosition("0.0", "LOWER", "3001") }, "Antenna Selector", "COMM 1 Antenna Selector Switch", "%0.1f"));    // elements["pnt_373"]     = default_3_position_tumb(_("COMM 1 Antenna Selector Switch, UPPER/AUTO/LOWER"),    devices.ANTENNA_SELECTOR, antsel_commands.Comm1AntSelSw,    373,    false, anim_speed_default, false)
            AddFunction(new Switch(this, ANTENNA_SELECTOR, "374", new SwitchPosition[] { new SwitchPosition("1.0", "UPPER", "3002"), new SwitchPosition("0.5", "BOTH", "3002"), new SwitchPosition("0.0", "LOWER", "3002") }, "Antenna Selector", "IFF Antenna Selector Switch", "%0.1f"));    // elements["pnt_374"]     = default_3_position_tumb(_("IFF Antenna Selector Switch, UPPER/BOTH/LOWER"),       devices.ANTENNA_SELECTOR, antsel_commands.AntSelIFFSw,      374,    false, anim_speed_default, false)
            #endregion
            #region  RWR
            AddFunction(new PushButton(this, RWR, "3001", "277", "RWR", "ALR-67 POWER Pushbutton", "1", "0", "%1d"));
            AddFunction(new PushButton(this, RWR, "3002", "275", "RWR", "ALR-67 DISPLAY Pushbutton", "1", "0", "%1d"));
            AddFunction(new PushButton(this, RWR, "3003", "272", "RWR", "ALR-67 SPECIAL Pushbutton", "1", "0", "%1d"));
            AddFunction(new PushButton(this, RWR, "3004", "269", "RWR", "ALR-67 OFFSET Pushbutton", "1", "0", "%1d"));
            AddFunction(new PushButton(this, RWR, "3005", "266", "RWR", "ALR-67 BIT Pushbutton", "1", "0", "%1d"));
            AddFunction(new Axis(this, RWR, "3006", "263", 0.15d, 0d, 1d, "RWR", "ALR-67 DMR Control Knob"));    // elements["pnt_263"]     = default_axis_limited(_("ALR-67 DMR Control Knob"),                            devices.RWR, rwr_commands.DmrControl, 263, 0.0, 0.1, false, false, {0,1})
            AddFunction(new Axis(this, RWR, "3009", "262", 0.15d, 0d, 1d, "RWR", "ALR-67 AUDIO Control Knob (no function)"));    // elements["pnt_262"]     = default_axis_limited(_("ALR-67 AUDIO Control Knob (no function)"),            0, 3130, 262, 0.0, 0.1, false, false, {0,1})
            AddFunction(new Switch(this, RWR, "261", new SwitchPosition[] { new SwitchPosition("0.0", "F", "3007"), new SwitchPosition("0.1", "U", "3007"), new SwitchPosition("0.2", "A", "3007"), new SwitchPosition("0.3", "I", "3007"), new SwitchPosition("0.4", "N", "3007") }, "RWR", "ALR-67 DIS TYPE Switch", "%0.1f"));
            AddFunction(new Axis(this, RWR, "3008", "216", 0.15d, 0d, 1d, "RWR", "RWR Intensity Knob"));    // elements["pnt_216"]     = default_axis_limited(_("RWR Intensity Knob"),                                 devices.RWR, rwr_commands.IntControl, 216, 0.0, 0.1, false, false, {0,1})
            #endregion

            #region  CMDS
            AddFunction(new PushButton(this, CMDS, "3002", "380", "CMDS", "Dispense Button - Push to dispense flares and chaff", "1", "0", "%1d"));    // elements["pnt_380"]     = default_button(_("Dispense Button - Push to dispense flares and chaff"),  devices.CMDS, cmds_commands.EcmDisp, 380)
            AddFunction(new Switch(this, CMDS, "517", new SwitchPosition[] { new SwitchPosition("0.2", "BYPASS", "3001"), new SwitchPosition("0.1", "ON", "3001"), new SwitchPosition("0.0", "OFF", "3001") }, "CMDS", "DISPENSER Switch", "%0.1f"));
            AddFunction(new PushButton(this, CMDS, "3003", "515", "CMDS", "ECM JETT JETT SEL Button - Push to jettison", "1", "0", "%1d"));    // elements["pnt_515"]     = default_2_position_tumb(_("ECM JETT JETT SEL Button - Push to jettison"), devices.CMDS, cmds_commands.EcmJett, 515)
            #endregion
            #region  ASPJ
            AddFunction(new Switch(this, ASPJ, "248", new SwitchPosition[] { new SwitchPosition("0.0", "OFF", "3001"), new SwitchPosition("0.1", "STBY", "3001"), new SwitchPosition("0.2", "BIT", "3001"), new SwitchPosition("0.3", "REC", "3001"), new SwitchPosition("0.4", "XMIT", "3001") }, "ASPJ", "ECM Mode Switch", "%0.1f"));    // elements["pnt_107"]     = default_3_position_tumb(_("UFC ADF Function Select Switch, 1/OFF/2"), devices.UFC, UFC_commands.SwADF,        107,    false, anim_speed_default, false)
                                                                                                                                                                                                                                                                                                                                            //AddFunction(new PushButton(this,, "", "507","1.0","ENABLE","0.0","DISABLE (no function)", "ICMCP","NUC WPN Switch"));    // elements["pnt_507"]     = default_2_position_tumb(_("NUC WPN Switch, ENABLE/DISABLE (no function)"),        0, 3100, 507)
            #endregion
            #region  TODO list
            //AddFunction(new Switch(this, "0", "175", new SwitchPosition[] { new SwitchPosition("1.0", "HMD", "3104"), new SwitchPosition("0.5", "LDDI", "3104"), new SwitchPosition("0.0", "RDDI", "3104") }, "IFEI", "Video Record Selector Switch HMD/LDDI/RDDI", "%0.1f"));
            //AddFunction(new Switch(this, "0", "176", new SwitchPosition[] { new SwitchPosition("1.0", "HUD", "3105"), new SwitchPosition("0.5", "LDIR", "3105"), new SwitchPosition("0.0", "RDDI", "3105") }, "IFEI", "Video Record Selector Switch, HUD/LDIR/RDDI", "%0.1f"));
            //AddFunction(new Switch(this, "0", "314", new SwitchPosition[] { new SwitchPosition("1.0", "MAN", "3106"), new SwitchPosition("0.5", "OFF", "3106"), new SwitchPosition("0.0", "AUTO", "3106") }, "IFEI", "Video Record Mode Selector Switch, MAN/OFF/AUTO", "%0.1f"));
            //AddFunction(new PushButton(this,, "", "315", "TODO list","Left Video Sensor BIT Initiate Pushbutton - Push to initiate BIT"));    // elements["pnt_315"]     = default_button(_("Left Video Sensor BIT Initiate Pushbutton - Push to initiate BIT"),     0, 3127, 315)
            //AddFunction(new PushButton(this,, "", "318", "TODO list","Right Video Sensor BIT Initiate Pushbutton - Push to initiate BIT"));    // elements["pnt_318"]     = default_button(_("Right Video Sensor BIT Initiate Pushbutton - Push to initiate BIT"),    0, 3128, 318)
            //AddFunction(new PushButton(this,, "", "7", "TODO list","HUD Video BIT Initiate Pushbutton - Push to initiate BIT"));    // elements["pnt_07"]      = default_button(_("HUD Video BIT Initiate Pushbutton - Push to initiate BIT"), 0, 3107, 7)
            #endregion
            #region HMD
            AddFunction(new Axis(this, HMD_INTERFACE, "3001", "136", 0.15d, 0d, 1d, "HMD", "HMD OFF/BRT Knob", false, "%0.2f"));
            #endregion
            #region Targetting Pod FLIR
            AddFunction(new Switch(this, TGP_INTERFACE, "439", new SwitchPosition[] { new SwitchPosition("1.0", "ON", "3001"), new SwitchPosition("0.0", "STBY", "3001"), new SwitchPosition("-1.0", "OFF", "3001") }, "TGP/FLIR", "FLIR Switch", "%0.1f"));
            AddFunction(new Switch(this, TGP_INTERFACE, "441", new SwitchPosition[] { new SwitchPosition("1.0", "ARM", "3002"), new SwitchPosition("0.0", "SAFE", "3002") }, "TGP/FLIR", "LTD/R Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, TGP_INTERFACE, "3003", "442", "1.0", "ON", "0.0", "OFF", "TGP/FLIR", "LST/NFLR Switch", "%0.1f"));
            #endregion


            #region  Instruments
            #region  Standby Baro Altimeter AAU-52/A
            AddFunction(new Axis(this, AAU52, "3001", "224", 0.01d, 0d, 1d, "Standby Baro Altimeter AAU-52/A", "Pressure Setting Knob", true, "%.3f"));    // elements["pnt_224"]     = default_axis(_("AAU-52 Altimeter Pressure Setting Knob"), devices.AAU52, aau52_commands.AAU52_ClkCmd_ZeroSetting, 224, 0.04, 0.1, false, true)
            AddFunction(new Altimeter(this, "Standby Baro Altimeter AAU-52/A", "2051", "Altitude", "Barometric altitude above sea level of the aircraft.", "Value is adjusted per altimeter pressure setting.", "2059", "Pressure", "Manually set barometric altitude.", ""));
            //AddFunction(new Axis(this, ADC, "3653", "653", 0.01d, 0d, 1d, "Altimeter", "Barometric pressure calibration adjust", true, "%.3f"));  // not sure what this is
            #endregion
            #region  Radar Altimeter Height Indicator
            AddFunction(new PushButton(this, ID2163A, "3001", "292", "Radar Altimeter ID2163A", "Push to Test Button"));
            AddFunction(new Axis(this, ID2163A, "3002", "291", 0.001d, 0d, 1d, "Radar Altimeter ID2163A", "Set Minimum Altitude", true, "%.3f"));
            AddFunction(new NetworkValue(this, "287", "Radar Altimeter ID2163A", "Minimum Height Indicator", "Minimum Altitude in Feet.", "", BindingValueUnits.Feet));
            AddFunction(new NetworkValue(this, "286", "Radar Altimeter ID2163A", "RADAR Altitude", "Altitude in feet measured by RADAR.", "", BindingValueUnits.Feet));
            AddFunction(new FlagValue(this, "288", "Radar Altimeter ID2163A", "Off Flag", ""));
            AddFunction(new FlagValue(this, "289", "Radar Altimeter ID2163A", "Green Lamp", ""));
            AddFunction(new FlagValue(this, "290", "Radar Altimeter ID2163A", "Red Lamp", ""));
            #endregion
            #region  Standby Attitude Indicator
            AddFunction(new PushButton(this, SAI, "3001", "215", "SAI", "SAI Test Button - Push to test", "1", "0", "%1d"));
            AddFunction(new ScaledNetworkValue(this, "205", 90d, "SAI", "Pitch", "Current pitch displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "206", -180d, "SAI", "Bank", "Current bank displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "209", "SAI", "Warning Flag", "Displayed when SAI is caged or non-functional."));
            AddFunction(new NetworkValue(this, "207", "SAI", "Slip Ball", "Current position of the slip ball relative to the center of the tube.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "208", "SAI", "Rate of Turn", "Turn indicator position", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new RotaryEncoder(this, SAI, "3003", "214", 0.1d, "SAI", "Pitch Adjust Knob"));
            AddFunction(new NetworkValue(this, "210", "SAI", "Pitch Adjustment", "Position of the pitch adjust.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new PushButton(this, SAI, "3002", "213", "SAI", "Cage Pull Switch", "1.0", "0.0", "%0.1f"));
            AddFunction(new NetworkValue(this, "212", "SAI", "Pitch Steering Bar", "Current position of the steering bar.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "211", "SAI", "Bank Steering Bar", "Current position of the steering bar.", "(-1 to 1)", BindingValueUnits.Numeric));
            #endregion
            //  VVI
            CalibrationPointCollectionDouble vviScale = new CalibrationPointCollectionDouble(-1.0d, -6000d, 1.0d, 6000d);
            vviScale.Add(new CalibrationPointDouble(0d, 0d));
            AddFunction(new ScaledNetworkValue(this, "225", vviScale, "Flight Instruments", "VVI", "Vertical velocity indicator -6000 to +6000.", "", BindingValueUnits.FeetPerMinute));

            //  IAS
            CalibrationPointCollectionDouble airspeedScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 360d);
            AddFunction(new ScaledNetworkValue(this, "217", airspeedScale, "Flight Instruments", "IAS Airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.Knots));

            //Cabin Altitude Pressure 
            //CalibrationPointCollectionDouble cabinScale = new CalibrationPointCollectionDouble(-0.003d, -300d, 0.5000d, 50000d);
            CalibrationPointCollectionDouble cabinScale = new CalibrationPointCollectionDouble(-0.003d, -300d, 1.0d, 50000d);
            cabinScale.Add(new CalibrationPointDouble(0d, 0d));
            AddFunction(new ScaledNetworkValue(this, "285", cabinScale, "System Gauges", "Cabin Altitude", "Cabin altitude pressue in feet 0 to +50000.", "", BindingValueUnits.Numeric));

            //  Hydraulic Pressure
            AddFunction(new NetworkValue(this, "242", "System Gauges", "Brake pressure", "Brake pressure in psi", "", BindingValueUnits.PoundsPerSquareInch, "%0.2f"));
            CalibrationPointCollectionDouble hydpressScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 5000.0d);
            AddFunction(new ScaledNetworkValue(this, "310", hydpressScale, "System Gauges", "Left Hyd pressure display", "Left Hydraulic system pressure in psi", "", BindingValueUnits.PoundsPerSquareInch));
            AddFunction(new ScaledNetworkValue(this, "311", hydpressScale, "System Gauges", "Right Hyd pressure display", "Right Hydraulic system pressure in psi", "", BindingValueUnits.PoundsPerSquareInch));

            //  Battery Voltage
            CalibrationPointCollectionDouble voltageScale = new CalibrationPointCollectionDouble(0.0d, 15.0d, 1.0d, 30d);
            AddFunction(new ScaledNetworkValue(this, "400", voltageScale, "System Gauges", "Voltage U", "Battery Voltage U", "", BindingValueUnits.Volts));
            AddFunction(new ScaledNetworkValue(this, "401", voltageScale, "System Gauges", "Voltage E", "Battery Voltage E", "", BindingValueUnits.Volts));

            #endregion
        }
    }
}
