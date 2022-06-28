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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.AH64D
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.AH64D.Functions;


    [HeliosInterface(
        "Helios.AH64D",                         // Helios internal type ID used in Profile XML, must never change
        "DCS AH-64D Apache",                    // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class AH64DInterface : DCSInterface
    {
        #region Devices
        private const string FM_PROXY = "1";
        private const string CONTROL_INTERFACE = "2";
        private const string ELEC_INTERFACE = "3";
        private const string FUEL_INTERFACE = "4";
        private const string HYDRO_INTERFACE = "5";
        private const string ENGINE_INTERFACE = "6";
        private const string GEAR_INTERFACE = "7";
        private const string OXYGEN_INTERFACE = "8";
        private const string CPT_MECH = "9";
        private const string EXTLIGHTS_SYSTEM = "10";
        private const string CPTLIGHTS_SYSTEM = "11";
        private const string ECS_INTERFACE = "12";
        // Instruments
        private const string SAI = "13";      // Standby Attitude Indicator
        private const string IAS = "14";      // Standby Airspeed Indicator
        private const string BARO_ALTIMETER = "15";       // Standby Altimeter
        private const string STANDBY_COMPASS = "16";      // Standby Magnetic Compass
        private const string FATgage = "17";      // Free Air Temperature Gage
        private const string GPS1 = "18";
        private const string GPS2 = "19";
        private const string EGI1 = "20";
        private const string EGI2 = "21";

        private const string MUX = "22";      // Multiplex manager, holds channels and manages remote terminals addition/remove

        // HOTAS Interface
        private const string HOTAS_PLT = "23";
        private const string HOTAS_CPG = "24";
        private const string HOTAS_INPUT = "25";

        // Helmet
        private const string IHADSS = "26";

        // Computers
        private const string SP_PLT = "27";       // System Processor
        private const string SP_CPG = "28";       // System Processor
        private const string KU_PLT = "29";       // Keyboard Unit
        private const string KU_CPG = "30";       // Keyboard Unit
        private const string KU_INPUT = "31";     // for input and routing
        private const string ELC1 = "32";
        private const string ELC2 = "33";
        private const string HIADC = "34";        // Highly Integrated Air Data Computer
        private const string FMC = "35";          // Flight Management Computer
        private const string WP1 = "36";          // Weapon Processor
        private const string WP2 = "37";          // Weapon Processor
        private const string DP_PLT = "38";       // Display Processor, DP2
        private const string DP_CPG = "39";       // Display Processor, DP1

        // Displays
        private const string HMD = "40";
        private const string HMD_INPUT = "41";
        private const string MFD_PLT_LEFT = "42";     // Multifunction Display
        private const string MFD_PLT_RIGHT = "43";    // Multifunction Display
        private const string MFD_CPG_LEFT = "44";     // Multifunction Display
        private const string MFD_CPG_RIGHT = "45";    // Multifunction Display
        private const string MFD_INPUT_LEFT = "46";
        private const string MFD_INPUT_RIGHT = "47";
        private const string EUFD_PLT = "48";     // Enhanced Up-Front Display Pilot
        private const string EUFD_CPG = "49";     // Enhanced Up-Front Display CPG
        private const string EUFD_INPUT = "50";
        private const string TEDAC = "51";        // TADS Electronic Display and Control
        private const string TEDAC_INPUT = "52";

        // Radio
        private const string DRVS_ASN157 = "53";
        private const string RADALT = "54";
        private const string ADF_ARN_149 = "55";
        private const string EMERGENCY_PANEL = "56";
        private const string UHF_RADIO = "57";        // UHF-AM ARC-164(V)
        private const string VHF_AM_RADIO = "58";     // VHF-AM ARC-186(V)
        private const string FM1 = "59";      // VHF-FM ARC-210D 
        private const string FM2 = "60";      // VHF-FM ARC-210D 
        private const string HF_RADIO = "61";     // HF ARC-220
        private const string COMM_PANEL_CPG = "62";       // Communications Panel (COMM - front)
        private const string COMM_PANEL_PLT = "63";       // Communications Panel (COMM - rear)
        private const string COMM_PANEL_INPUT = "64";
        private const string CIU = "65";      // Communications Interface Unit ( Voice messages )
        private const string IFF = "66";
        private const string INTERCOM = "67";     // must be last in radio group

        // Sights
        private const string TADS = "68";     // Target Acquisition Designation Sight
        private const string PNVS = "69";     // Pilot Night Vision Sensor
        private const string PNVS_TURRET_CONTROL = "70";      // PNVS control system
        private const string FCR = "71";      // Fire Control Radar
        private const string RFI = "72";      // Radar Frequency Interferometer

        // Weapon
        private const string GUN_TURRET_CONTROL = "73";       // gun control system
        private const string PYLONS_INPUT = "74";     // pylons dispatcher
        private const string JETT_PANEL_PLT = "75";       // left console jettison panel
        private const string JETT_PANEL_CPG = "76";       // left console jettison panel
        private const string JETT_PANEL_INPUT = "77";
        private const string HELLFIRE_INTERFACE = "78";       // for emulate HF missiles while on board

        // ASE
        private const string AN_APR39 = "79";     // AN/APR-39 Radar Warning Receiver (RWR)
        private const string CMWS = "80";

        //
        private const string BRU_PLT = "81";
        private const string BRU_CPG = "82";

        //
        private const string MACROS = "83";
        private const string KNEEBOARD = "84";
        private const string ARCADE = "85";

        // NVG
        private const string NVG = "86";

        // AI
        private const string PrestonAI = "87";

        // 
        private const string HEAD_WRAPPER = "88";
        private const string DATA_TRANSFER_UNIT = "89";
        #endregion
        #region Command_Defs.lua

        private enum electric_commands
        {
            GEN1_RST_SW = 3001,
            GEN2_RST_SW = 3002,
            MIK = 3003,
            SP_SELECT_SW = 3004,
            NVS_MODE_PLT_KNOB = 3005,
            NVS_MODE_CPG_KNOB = 3006,
            VCP_FLIR_GAIN_KNOB = 3007,
            VCP_FLIR_LEV_KNOB = 3008,
            VCP_ACM_SW = 3009,
            VCP_IHADSS_CON_KNOB = 3010,
            VCP_IHADSS_BRT_KNOB = 3011,
            VCP_SYM_BRT_KNOB = 3012,
            GND_ORIDE_PLT_BTN = 3013,
            ARM_SAFE_PLT_BTN = 3014,
            GND_ORIDE_CPG_BTN = 3015,
            ARM_SAFE_CPG_BTN = 3016,
            GEN1_RST_SW_EXT = 3017,
            GEN2_RST_SW_EXT = 3018,
            MIK_EXT = 3019,
            MIK_ITER = 3020,
            SP_SELECT_SW_EXT = 3021,
            SP_SELECT_SW_ITER = 3022,
            NVS_MODE_KNOB_EXT = 3023,
            NVS_MODE_KNOB_ITER = 3024,
            VCP_FLIR_GAIN_KNOB_ITER = 3025,
            VCP_FLIR_LEV_KNOB_ITER = 3026,
            VCP_ACM_SW_ITER = 3027,
            VCP_IHADSS_CON_KNOB_ITER = 3028,
            VCP_IHADSS_BRT_KNOB_ITER = 3029,
            VCP_SYM_BRT_KNOB_ITER = 3030,
            VCP_FLIR_LEV_KNOB_AXIS = 3031,
            VCP_FLIR_GAIN_KNOB_AXIS = 3032,
            VCP_IHADSS_CON_KNOB_AXIS = 3033,
            VCP_IHADSS_BRT_KNOB_AXIS = 3034,
            VCP_SYM_BRT_KNOB_AXIS = 3035,
            VCP_FLIR_GAIN_KNOB_EXT = 3036,
            VCP_FLIR_LEV_KNOB_EXT = 3037,
            VCP_ACM_SW_EXT = 3038,
            VCP_IHADSS_CON_KNOB_EXT = 3039,
            VCP_IHADSS_BRT_KNOB_EXT = 3040,
            VCP_SYM_BRT_KNOB_EXT = 3041,
            GND_ORIDE_BTN_EXT = 3042,
            ARM_SAFE_BTN_EXT = 3043
        };

        private enum CMWS_commands
        {
            CMWS_PWR = 3001,
            CMWS_PWR_TEST = 3002,
            CMWS_AUDIO_KNOB = 3003,
            CMWS_LAMP_KNOB = 3004,
            CMWS_ARM_SAFE_SW = 3005,
            CMWS_CMWS_NAV_SW = 3006,
            CMWS_BYPASS_AUTO_SW = 3007,
            CMWS_JETT_COVER = 3008,
            CMWS_JETT_SW = 3009,
            CMWS_PWR_ITER = 3010,
            CMWS_PWR_EXT = 3011,
            CMWS_AUDIO_KNOB_AXIS = 3012,
            CMWS_AUDIO_KNOB_ITER = 3013,
            CMWS_LAMP_KNOB_AXIS = 3014,
            CMWS_LAMP_KNOB_ITER = 3015,
            CMWS_ARM_SAFE_SW_EXT = 3016,
            CMWS_ARM_SAFE_SW_ITER = 3017,
            CMWS_CMWS_NAV_SW_EXT = 3018,
            CMWS_CMWS_NAV_SW_ITER = 3019,
            CMWS_BYPASS_AUTO_SW_EXT = 3020,
            CMWS_BYPASS_AUTO_SW_ITER = 3021,
            CMWS_JETT_COVER_EXT = 3022,
            CMWS_JETT_COVER_ITER = 3023,
            CMWS_JETT_SW_EXT = 3024
        };

        private enum hydraulic_commands
        {
            Rotor_Brake = 3001,
            Emergency_HYD_PLT = 3002,
            TailWheelUnLock_PLT = 3003,
            Emergency_HYD_CPG = 3004,
            TailWheelUnLock_CPG = 3005,
            Rotor_Brake_Sw_EXT = 3006,
            Rotor_Brake_Sw_ITER = 3007,
            Emergency_HYD_EXT = 3008,
            TailWheelUnLock_EXT = 3009
        };

        private enum gear_commands
        {
            AH64_ParkingBrake = 3001,
            AH64_ParkingBrake_EXT = 3002
        };

        private enum ctrl_commands
        {
            FrictionLever = 3001,
            FingerLiftLeft_PLT = 3002,
            FingerLiftRight_PLT = 3003,
            FingerLiftBoth_PLT = 3004,
            FingerLiftLeft_CPG = 3005,
            FingerLiftRight_CPG = 3006,
            FingerLiftBoth_CPG = 3007,
            FrictionLever_EXT = 3008,
            FrictionLever_ITER = 3009,
            FrictionLever_AXIS = 3010,
            FingerLiftLeft_EXT = 3011,
            FingerLiftRight_EXT = 3012,
            FingerLiftBoth_EXT = 3013,
            LockoutDetent = 3014
        };

        private enum engine_commands
        {
            APU_StartBtn = 3001,
            APU_StartBtnCover = 3002,
            Eng1StartSw = 3003,
            Eng1IgnOrideSw = 3004,
            Eng2StartSw = 3005,
            Eng2IgnOrideSw = 3006,
            ChkOvspTestSwENG1A = 3007,
            ChkOvspTestSwENG2A = 3008,
            ChkOvspTestSwENG1B = 3009,
            ChkOvspTestSwENG2B = 3010,
            PLT_Eng1FireBtn = 3011,
            PLT_Eng1FireBtnCover = 3012,
            PLT_Eng2FireBtn = 3013,
            PLT_Eng2FireBtnCover = 3014,
            PLT_ApuFireBtn = 3015,
            PLT_ApuFireBtnCover = 3016,
            PLT_PrimaryDischBtn = 3017,
            PLT_ReserveDischBtn = 3018,
            PLT_FireDetTestSw1 = 3019,
            PLT_FireDetTestSw2 = 3020,
            CPG_Eng1FireBtn = 3021,
            CPG_Eng1FireBtnCover = 3022,
            CPG_Eng2FireBtn = 3023,
            CPG_Eng2FireBtnCover = 3024,
            CPG_ApuFireBtn = 3025,
            CPG_ApuFireBtnCover = 3026,
            CPG_PrimaryDischBtn = 3027,
            CPG_ReserveDischBtn = 3028,
            CPG_FireDetTestSw1 = 3029,
            CPG_FireDetTestSw2 = 3030,
            PLT_L_PowerLever = 3031,
            PLT_R_PowerLever = 3032,
            CPG_L_PowerLever = 3033,
            CPG_R_PowerLever = 3034,
            APU_StartBtn_EXT = 3035,
            APU_StartBtnCover_EXT = 3036,
            APU_StartBtnCover_ITER = 3037,
            Eng1StartSw_EXT = 3038,
            Eng1IgnOrideSw_EXT = 3039,
            Eng2StartSw_EXT = 3040,
            Eng2IgnOrideSw_EXT = 3041,
            ChkOvspTestSwENG1A_EXT = 3042,
            ChkOvspTestSwENG2A_EXT = 3043,
            ChkOvspTestSwENG1B_EXT = 3044,
            ChkOvspTestSwENG2B_EXT = 3045,
            Eng1FireBtn_EXT = 3046,
            Eng1FireBtnCover_EXT = 3047,
            Eng1FireBtnCover_ITER = 3048,
            Eng2FireBtn_EXT = 3049,
            Eng2FireBtnCover_EXT = 3050,
            Eng2FireBtnCover_ITER = 3051,
            ApuFireBtn_EXT = 3052,
            ApuFireBtnCover_EXT = 3053,
            ApuFireBtnCover_ITER = 3054,
            PrimaryDischBtn_EXT = 3055,
            ReserveDischBtn_EXT = 3056,
            FireDetTestSw1_EXT = 3057,
            FireDetTestSw2_EXT = 3058,
            PLT_BothPowerLevers_EXT = 3059,
            CPG_BothPowerLevers_EXT = 3060
        };

        private enum extlights_commands
        {
            FormationLights = 3001,
            NavLights = 3002,
            AntiCollLights = 3003,
            FormationLights_ITER = 3004,
            FormationLights_AXIS = 3005,
            NavLights_EXT = 3006,
            NavLights_ITER = 3007,
            AntiCollLights_EXT = 3008,
            AntiCollLights_ITER = 3009
        };

        private enum intlights_commands
        {
            MasterCautionPLT = 3001,
            MasterWarningPLT = 3002,
            MasterCautionCPG = 3003,
            MasterWarningCPG = 3004,
            TestLightsPLT = 3005,
            SignalPLT = 3006,
            PrimaryPLT = 3007,
            FloodPLT = 3008,
            StbyInstPLT = 3009,
            UtilityPLT = 3010,
            UtilityButtonPLT = 3011,
            TestLightsCPG = 3012,
            SignalCPG = 3013,
            PrimaryCPG = 3014,
            FloodCPG = 3015,
            UtilityCPG = 3016,
            UtilityButtonCPG = 3017,
            MasterCaution_EXT = 3018,
            MasterWarning_EXT = 3019,
            TestLights_EXT = 3020,
            UtilityButton_EXT = 3021,
            Signal_ITER = 3022,
            Primary_ITER = 3023,
            Flood_ITER = 3024,
            StbyInst_ITER = 3025,
            Utility_ITER = 3026,
            Signal_AXIS = 3027,
            Primary_AXIS = 3028,
            Flood_AXIS = 3029,
            StbyInst_AXIS = 3030,
            Utility_AXIS = 3031
        };

        private enum sai_commands
        {
            CageKnobPull = 3001,
            CageKnobRotate = 3002,
            CageKnobPull_EXT = 3003,
            CageKnobRotate_ITER = 3004,
            CageKnobRotate_AXIS = 3005
        };

        private enum baro_alt_commands
        {
            PressureSet = 3001,
            PressureSet_ITER = 3002,
            PressureSet_AXIS = 3003
        };

        private enum mpd_commands
        {
            T1 = 3001,
            T2 = 3002,
            T3 = 3003,
            T4 = 3004,
            T5 = 3005,
            T6 = 3006,
            R1 = 3007,
            R2 = 3008,
            R3 = 3009,
            R4 = 3010,
            R5 = 3011,
            R6 = 3012,
            B6 = 3013,
            B5 = 3014,
            B4 = 3015,
            B3 = 3016,
            B2 = 3017,
            B1 = 3018,
            L6 = 3019,
            L5 = 3020,
            L4 = 3021,
            L3 = 3022,
            L2 = 3023,
            L1 = 3024,
            Asterisk = 3025,
            VID = 3026,
            COM = 3027,
            AC = 3028,
            TSD = 3029,
            WPN = 3030,
            FCR = 3031,
            BRT_KNOB = 3032,
            VID_KNOB = 3033,
            MODE_KNOB = 3034,
            BRT_KNOB_ITER = 3035,
            BRT_KNOB_AXIS = 3036,
            VID_KNOB_ITER = 3037,
            VID_KNOB_AXIS = 3038,
            MODE_KNOB_ITER = 3039
        };

        private enum KU_commands
        {
            keyCLR = 3001,
            keyBKS = 3002,
            keySPC = 3003,
            keyLeft = 3004,
            keyRight = 3005,
            keyEnter = 3006,
            keyA = 3007,
            keyB = 3008,
            keyC = 3009,
            keyD = 3010,
            keyE = 3011,
            keyF = 3012,
            keyG = 3013,
            keyH = 3014,
            keyI = 3015,
            keyJ = 3016,
            keyK = 3017,
            keyL = 3018,
            keyM = 3019,
            keyN = 3020,
            keyO = 3021,
            keyP = 3022,
            keyQ = 3023,
            keyR = 3024,
            keyS = 3025,
            keyT = 3026,
            keyU = 3027,
            keyV = 3028,
            keyW = 3029,
            keyX = 3030,
            keyY = 3031,
            keyZ = 3032,
            key1 = 3033,
            key2 = 3034,
            key3 = 3035,
            key4 = 3036,
            key5 = 3037,
            key6 = 3038,
            key7 = 3039,
            key8 = 3040,
            key9 = 3041,
            keyDot = 3042,
            key0 = 3043,
            keySign = 3044,
            keySlash = 3045,
            keyPlus = 3046,
            keyMinus = 3047,
            keyDivide = 3048,
            keyMultiply = 3049,
            BrightnessKnob = 3050,
            BrightnessKnob_KB = 3051,
            BrightnessKnob_AXIS = 3052
        };

        private enum eufd_commands
        {
            WCA_UP = 3001,
            WCA_DOWN = 3002,
            IDM_UP = 3003,
            IDM_DOWN = 3004,
            RTS_UP = 3005,
            RTS_DOWN = 3006,
            Preset = 3007,
            Enter = 3008,
            Stopwatch = 3009,
            Swap = 3010,
            BRT = 3011,
            BRT_ITER = 3012,
            BRT_AXIS = 3013
        };

        private enum hotas_commands
        {
            CYCLIC_TRIGGER_GUARD = 3001,
            CYCLIC_TRIGGER_1ST_DETENT = 3002,
            CYCLIC_TRIGGER_2ND_DETENT = 3003,
            CYCLIC_TRIM_HOLD_SW_UP = 3004, //- (R) FORCE TRIM RELEASE
            CYCLIC_TRIM_HOLD_SW_DOWN = 3005, //- (D) HOLD MODES DISENGAGE
            CYCLIC_TRIM_HOLD_SW_LEFT = 3006, //- (AT) ATTITUDE/HOVER HOLD
            CYCLIC_TRIM_HOLD_SW_RIGHT = 3007, //- (AL) ALTITUDE HOLD
            CYCLIC_WEAPONS_ACTION_SW_UP = 3008, //- (G) GUN
            CYCLIC_WEAPONS_ACTION_SW_DOWN = 3009, //- (A) AIR-TO-AIR
            CYCLIC_WEAPONS_ACTION_SW_LEFT = 3010, //- (R) ROCKET
            CYCLIC_WEAPONS_ACTION_SW_RIGHT = 3011, //- (M) MISSILE
            CYCLIC_SYMBOLOGY_SELECT_SW_UP = 3012, //- (T),(C)
            CYCLIC_SYMBOLOGY_SELECT_SW_DOWN = 3013, //- (B),(H)
            CYCLIC_SYMBOLOGY_SELECT_SW_DEPRESS = 3014,
            CYCLIC_CMDS_SW_FWD = 3015,
            CYCLIC_CMDS_SW_AFT = 3016,
            CYCLIC_RTS_SW_LEFT = 3017, //- RADIO
            CYCLIC_RTS_SW_RIGHT = 3018, //- ICS
            CYCLIC_RTS_SW_DEPRESS = 3019, //- RTS
            CYCLIC_FMC_RELEASE_SW = 3020,
            CYCLIC_CHAFF_DISPENCE_BTN = 3021,
            CYCLIC_FLARE_DISPENCE_BTN = 3022,
            CYCLIC_ATA_CAGE_UNCAGE_BTN = 3023,
            MISSION_FCR_SCAN_SIZE_SW_UP = 3024, //- (Z) ZOOM
            MISSION_FCR_SCAN_SIZE_SW_DOWN = 3025, //- (M) MEDIUM
            MISSION_FCR_SCAN_SIZE_SW_LEFT = 3026, //- (N) NARROW
            MISSION_FCR_SCAN_SIZE_SW_RIGHT = 3027, //- (W) WIDE
            MISSION_SIGHT_SELECT_SW_UP = 3028, //- (HMD)
            MISSION_SIGHT_SELECT_SW_DOWN = 3029, //- (LINK)
            MISSION_SIGHT_SELECT_SW_LEFT = 3030, //- (FCR)
            MISSION_SIGHT_SELECT_SW_RIGHT = 3031, //- (TADS)
            MISSION_FCR_MODE_SW_UP = 3032, //- (GTM) Ground Targeting Mode
            MISSION_FCR_MODE_SW_DOWN = 3033, //- (ATM) Air Targeting Mode
            MISSION_FCR_MODE_SW_LEFT = 3034, //- (TPM) Terrain Profile Mode
            MISSION_FCR_MODE_SW_RIGHT = 3035, //- (RMAP) Radar MAP
            MISSION_CURSOR_UP = 3036,
            MISSION_CURSOR_DOWN = 3037,
            MISSION_CURSOR_LEFT = 3038,
            MISSION_CURSOR_RIGHT = 3039,
            MISSION_CURSOR_ENTER = 3040,
            MISSION_ALTERNATE_CURSOR_ENTER = 3041,
            MISSION_CURSOR_AXIS_X = 3042,
            MISSION_CURSOR_AXIS_Y = 3043,
            MISSION_CURSOR_DISPLAY_SELECT_BTN = 3044,
            MISSION_FCR_SCAN_SW_SINGLE = 3045,
            MISSION_FCR_SCAN_SW_CONTINUOUS = 3046,
            MISSION_CUED_SEARCH_SW = 3047,
            MISSION_MISSILE_ADVANCE_SW = 3048,
            FLIGHT_EMERGENCY_JETTISON_SW_GUARD = 3049,
            FLIGHT_EMERGENCY_JETTISON_SW = 3050,
            FLIGHT_NVS_SELECT_SW_TADS = 3051,
            FLIGHT_NVS_SELECT_SW_PNVS = 3052,
            FLIGHT_BORESIGHT_POLARITY_SW_BS = 3053,
            FLIGHT_BORESIGHT_POLARITY_SW_PLRT = 3054,
            FLIGHT_STABILATOR_CONTROL_SW_NU = 3055, //- NOSE UP
            FLIGHT_STABILATOR_CONTROL_SW_ND = 3056, //- NOSE DOWN
            FLIGHT_STABILATOR_CONTROL_SW_DEPRESS = 3057, //- STABILATOR AUTO MODE
            FLIGHT_SEARCHLIGHT_SW_UP = 3058, //- ON/OFF
            FLIGHT_SEARCHLIGHT_SW_DOWN = 3059, //- STOW/OFF
            FLIGHT_SEARCHLIGHT_POSITION_SW_UP = 3060, //- EXT
            FLIGHT_SEARCHLIGHT_POSITION_SW_DOWN = 3061, //- RET
            FLIGHT_SEARCHLIGHT_POSITION_SW_LEFT = 3062, //- L
            FLIGHT_SEARCHLIGHT_POSITION_SW_RIGHT = 3063, //- R
            FLIGHT_CHOP_BTN_GUARD = 3064,
            FLIGHT_CHOP_BTN = 3065,
            FLIGHT_TAIL_WHEEL_BTN = 3066, //- LOCK/UNLOCK
            FLIGHT_BUCS_TRIGGER_GUARD = 3067, //- only in CPG
            FLIGHT_BUCS_TRIGGER = 3068, //- only in CPG
            CYCLIC_TRIGGER_GUARD_ITER = 3069,
            FLIGHT_EMERGENCY_JETTISON_SW_GUARD_ITER = 3070,
            FLIGHT_SEARCHLIGHT_SW_ITER = 3071,
            FLIGHT_CHOP_BTN_GUARD_ITER = 3072,
            FLIGHT_BUCS_TRIGGER_GUARD_ITER = 3073
        };

        private enum tedac_commands
        {
            TDU_MODE_KNOB = 3001,
            TDU_GAIN_KNOB = 3002,
            TDU_LEV_KNOB = 3003,
            TDU_ASTERISK_BTN = 3004,
            TDU_VIDEO_SELECT_TAD_BTN = 3005,
            TDU_VIDEO_SELECT_FCR_BTN = 3006,
            TDU_VIDEO_SELECT_PNV_BTN = 3007,
            TDU_VIDEO_SELECT_GS_BTN = 3008,
            TDU_SYM_INC = 3009,
            TDU_SYM_DEC = 3010,
            TDU_BRT_INC = 3011,
            TDU_BRT_DEC = 3012,
            TDU_CON_INC = 3013,
            TDU_CON_DEC = 3014,
            TDU_AZ_LEFT = 3015,
            TDU_AZ_RIGHT = 3016,
            TDU_EL_UP = 3017,
            TDU_EL_DOWN = 3018,
            TDU_RF_UP = 3019,
            TDU_RF_DOWN = 3020,
            TDU_B1 = 3021, //- AZ/EL
            TDU_B2 = 3022, //- ACM
            TDU_B3 = 3023, //- FREEZE
            TDU_B4 = 3024, //- FILTER
            TDU_MODE_KNOB_ITER = 3025,
            TDU_GAIN_KNOB_ITER = 3026,
            TDU_GAIN_KNOB_AXIS = 3027,
            TDU_LEV_KNOB_ITER = 3028,
            TDU_LEV_KNOB_AXIS = 3029,
            LHG_IAT_OFS_SW_IAT = 3030, //- Image Auto Track / Offset
            LHG_IAT_OFS_SW_OFS = 3031, //- Image Auto Track / Offset
            LHG_TADS_FOV_SW_Z = 3032, //- Zoom
            LHG_TADS_FOV_SW_M = 3033, //- Medium
            LHG_TADS_FOV_SW_N = 3034, //- Narrow
            LHG_TADS_FOV_SW_W = 3035, //- Wide
            LHG_TADS_SENSOR_SELECT_SW_FLIR = 3036,
            LHG_TADS_SENSOR_SELECT_SW_TV = 3037,
            LHG_TADS_SENSOR_SELECT_SW_DVO = 3038, //- ORT only
            LHG_STORE_UPDT_SW_STORE = 3039,
            LHG_STORE_UPDT_SW_UPDT = 3040,
            LHG_FCR_SCAN_SW_S = 3041, //- single (as in collective mission grip)
            LHG_FCR_SCAN_SW_C = 3042, //- continuous (as in collective mission grip)
            LHG_CUED_SEARCH_BTN = 3043, //- (as in collective mission grip)
            LHG_LMC_BTN = 3044, //- Linear Motion Compensation
            LHG_FCR_MODE_SW_UP = 3045, //- (GTM) Ground Targeting Mode(as in collective mission grip)
            LHG_FCR_MODE_SW_DOWN = 3046, //- (ATM) Air Targeting Mode(as in collective mission grip)
            LHG_FCR_MODE_SW_LEFT = 3047, //- (TPM) Terrain Profile Mode(as in collective mission grip)
            LHG_FCR_MODE_SW_RIGHT = 3048, //- (RMAP) Radar MAP(as in collective mission grip)
            LHG_WEAPONS_ACTION_SW_UP = 3049, //- (G) GUN(as in collective mission grip)
            LHG_WEAPONS_ACTION_SW_DOWN = 3050, //- (A) AIR-TO-AIR(as in collective mission grip)
            LHG_WEAPONS_ACTION_SW_LEFT = 3051, //- (R) ROCKET(as in collective mission grip)
            LHG_WEAPONS_ACTION_SW_RIGHT = 3052, //- (M) MISSILE(as in collective mission grip)
            LHG_CURSOR_UP = 3053,
            LHG_CURSOR_DOWN = 3054,
            LHG_CURSOR_LEFT = 3055,
            LHG_CURSOR_RIGHT = 3056,
            LHG_CURSOR_ENTER = 3057,
            LHG_CURSOR_AXIS_X = 3058,
            LHG_CURSOR_AXIS_Y = 3059,
            LHG_LR_BTN = 3060, //- (L/R Switch) Cursor Display Select Btn
            LHG_WEAPON_TRIGGER_1ST_DETENT = 3061,
            LHG_WEAPON_TRIGGER_2ND_DETENT = 3062,
            RHG_SIGHT_SELECT_SW_UP = 3063, //- (HMD)(as in collective mission grip)
            RHG_SIGHT_SELECT_SW_DOWN = 3064, //- (LINK)(as in collective mission grip)
            RHG_SIGHT_SELECT_SW_LEFT = 3065, //- (FCR)(as in collective mission grip)
            RHG_SIGHT_SELECT_SW_RIGHT = 3066, //- (TADS)(as in collective mission grip)
            RHG_LT_SW_A = 3067, //- AutomaticLaser Tracker Switch
            RHG_LT_SW_O = 3068, //- Off
            RHG_LT_SW_M = 3069, //- Manual
            RHG_FCR_SCAN_SIZE_SW_UP = 3070, //- (Z) ZOOM
            RHG_FCR_SCAN_SIZE_SW_DOWN = 3071, //- (M) MEDIUM
            RHG_FCR_SCAN_SIZE_SW_LEFT = 3072, //- (N) NARROW
            RHG_FCR_SCAN_SIZE_SW_RIGHT = 3073, //- (W) WIDE
            RHG_C_SCOPE_SW = 3074,
            RHG_FLIR_PLRT_BTN = 3075,
            RHG_SIGHT_SLAVE_BTN = 3076,
            RHG_DISPLAY_ZOOM_BTN = 3077,
            RHG_LRFD_TRIGGER = 3078,
            RHG_SPARE_SW_FWD = 3079, //- MTADS only, MTT Switch (Multi Target Tracker, 3-pos, rocker)
            RHG_SPARE_SW_AFT = 3080, //- MTADS only, MTT Switch (Multi Target Tracker, 3-pos, rocker)
            RHG_HDD_SW = 3081, //- HDD/HOD
            RHG_ENTER_BTN = 3082,
            RHG_MAN_TRK_UP = 3083, //- Sensor (Sight) Manual Tracker
            RHG_MAN_TRK_DOWN = 3084,
            RHG_MAN_TRK_LEFT = 3085,
            RHG_MAN_TRK_RIGHT = 3086,
            RHG_MAN_TRK_AXIS_X = 3087,
            RHG_MAN_TRK_AXIS_Y = 3088,
            RHG_IAT_POLARITY_W = 3089, //- White
            RHG_IAT_POLARITY_A = 3090, //- Auto
            RHG_IAT_POLARITY_B = 3091, //- Black
            LHG_TADS_SENSOR_SELECT_SW = 3092, //- for clickability
            RHG_LT_SW = 3093, //- for clickability
            RHG_IAT_POLARITY_SW = 3094, //- for clickability

        };

        private enum hmd_commands
        {
            DAP_PLT_SIZE_KNOB = 3001,
            DAP_PLT_H_CTRG_KNOB = 3002,
            DAP_PLT_V_CTRG_KNOB = 3003,
            DAP_CPG_SIZE_KNOB = 3004,
            DAP_CPG_H_CTRG_KNOB = 3005,
            DAP_CPG_V_CTRG_KNOB = 3006,
            DAP_SIZE_ITER = 3007,
            DAP_H_CTRG_ITER = 3008,
            DAP_V_CTRG_ITER = 3009,
            HMD_SHOW = 3010
        };

        private enum BRU_commands
        {
            BRT_KNOB = 3001,
            BRT_KNOB_ITER = 3002,
            BRT_KNOB_AXIS = 3003
        };

        private enum JETT_commands
        {
            STORE_LO_JETTISON_ARMED = 3001,
            STORE_LI_JETTISON_ARMED = 3002,
            STORE_RI_JETTISON_ARMED = 3003,
            STORE_RO_JETTISON_ARMED = 3004,
            STORE_JETTISON_LEFT_WINGTIP = 3005,
            STORE_JETTISON_RIGHT_WINGTIP = 3006,
            STORES_JETT_PUSHBUTTON = 3007
        };

        private enum preston_commands
        {
            ControlRequest = 3001,
            ShowHideMenu = 3002,
            HatUp = 3003,
            HatDown = 3004,
            HatLeft = 3005,
            HatRight = 3006,
            StickFolding = 3007,
            ConsentToFire = 3008
        };

        private enum comm_commands
        {
            VHF_volume = 3001,
            UHF_volume = 3002,
            FM1_volume = 3003,
            FM2_volume = 3004,
            HF_volume = 3005,
            IFF_volume = 3006,
            RLWR_volume = 3007,
            ATA_volume = 3008,
            VCR_volume = 3009,
            ADF_volume = 3010,
            MASTER_volume = 3011,
            SensControl = 3012,
            VHF_SQL = 3013,
            UHF_SQL = 3014,
            FM1_SQL = 3015,
            FM2_SQL = 3016,
            HF_SQL = 3017,
            ICS_MODE = 3018,
            IDENT = 3019,
            VHF_disable = 3020,
            UHF_disable = 3021,
            FM1_disable = 3022,
            FM2_disable = 3023,
            HF_disable = 3024,
            IFF_disable = 3025,
            RLWR_disable = 3026,
            ATA_disable = 3027,
            VCR_disable = 3028,
            ADF_disable = 3029,
            VHF_volume_ITER = 3030,
            VHF_volume_AXIS = 3031,
            UHF_volume_ITER = 3032,
            UHF_volume_AXIS = 3033,
            FM1_volume_ITER = 3034,
            FM1_volume_AXIS = 3035,
            FM2_volume_ITER = 3036,
            FM2_volume_AXIS = 3037,
            HF_volume_ITER = 3038,
            HF_volume_AXIS = 3039,
            IFF_volume_ITER = 3040,
            IFF_volume_AXIS = 3041,
            RLWR_volume_ITER = 3042,
            RLWR_volume_AXIS = 3043,
            ATA_volume_ITER = 3044,
            ATA_volume_AXIS = 3045,
            VCR_volume_ITER = 3046,
            VCR_volume_AXIS = 3047,
            ADF_volume_ITER = 3048,
            ADF_volume_AXIS = 3049,
            MASTER_volume_ITER = 3050,
            MASTER_volume_AXIS = 3051,
            SensControl_ITER = 3052,
            SensControl_AXIS = 3053,
            VHF_SQL_ITER = 3054,
            UHF_SQL_ITER = 3055,
            FM1_SQL_ITER = 3056,
            FM2_SQL_ITER = 3057,
            HF_SQL_ITER = 3058,
            ICS_MODE_ITER = 3059,
            VHF_disable_ITER = 3060,
            UHF_disable_ITER = 3061,
            FM1_disable_ITER = 3062,
            FM2_disable_ITER = 3063,
            HF_disable_ITER = 3064,
            IFF_disable_ITER = 3065,
            RLWR_disable_ITER = 3066,
            ATA_disable_ITER = 3067,
            VCR_disable_ITER = 3068,
            ADF_disable_ITER = 3069
        };

        private enum intercom_commands
        {
            PLT_UHF_GUARD_Btn = 3001,
            CPG_UHF_GUARD_Btn = 3002,
            PLT_XPNDR_Btn = 3003,
            CPG_XPNDR_Btn = 3004,
            PLT_ZEROIZE_Sw = 3005,
            CPG_ZEROIZE_Sw = 3006,
            PLT_MasterZeroizeSw = 3007,
            PLT_MasterZeroizeSwCover = 3008,
            CPG_MasterZeroizeSw = 3009,
            CPG_MasterZeroizeSwCover = 3010,
            UHF_GUARD_Btn_EXT = 3011,
            XPNDR_Btn_EXT = 3012,
            ZEROIZE_Sw_EXT = 3013,
            ZEROIZE_Sw_ITER = 3014,
            MasterZeroizeSw_EXT = 3015,
            MasterZeroizeSw_ITER = 3016,
            MasterZeroizeSwCover_EXT = 3017,
            MasterZeroizeSwCover_ITER = 3018
        };

        private enum cpt_mech_commands
        {
            PLT_DefogBtn = 3001,
            PLT_WiperSw = 3002,
            CPG_DefogBtn = 3003,
            CPG_WiperSw = 3004,
            PLT_Door_Lock = 3005,
            CPG_Door_Lock = 3006,
            PLT_M4_Safety = 3007,
            CPG_M4_Safety = 3008,
            PLT_M4_Trigger = 3009,
            CPG_M4_Trigger = 3010,
            DefogBtn_EXT = 3011,
            WiperSw_EXT = 3012,
            WiperSw_ITER = 3013,
            PLT_Door_Lock_EXT = 3014,
            CPG_Door_Lock_EXT = 3015,
            MECH_Door_EXT = 3016
        };

        private enum head_wrapper_commands
        {
            OccupySeatPLT = 3001,
            OccupySeatCPG = 3002
        };

        private enum DTU_commands
        {
            KB_FLARE_IterBurstCount = 3001,
            KB_FLARE_IterBurstInterval = 3002,
            KB_FLARE_IterSalvoCount = 3003,
            KB_FLARE_IterSalvoInterval = 3004,
            KB_FLARE_IterDelayTime = 3005
        };

        #endregion
        #region MainPanel/lamps.lua
        private enum Warning_Lights
        {
            FLAG_MasterWarningPLT = 424,
            FLAG_MasterCautionPLT = 425,
            FLAG_FireEng1PLT = 416,
            FLAG_FireApuPLT = 418,
            FLAG_FireEng2PLT = 420,
            FLAG_RdyEng1PLT = 417,
            FLAG_RdyApuPLT = 419,
            FLAG_RdyEng2PLT = 421,
            FLAG_DischPriPLT = 422,
            FLAG_DischResPLT = 423,
            FLAG_EmergencyGuardPLT = 403,
            FLAG_EmergencyXpndrPLT = 404,
            FLAG_EmergencyEmergHydPLT = 405,
            FLAG_TailWheelUnlockPLT = 402,
            FLAG_ArmLTipPLT = 411,
            FLAG_ArmLOutbdPLT = 407,
            FLAG_ArmLInbdPLT = 408,
            FLAG_ArmRInbdPLT = 409,
            FLAG_ArmROutbdPLT = 410,
            FLAG_ArmRTipPLT = 412,
            FLAG_ArmamentASArmPLT = 413,
            FLAG_ArmamentASSafePLT = 414,
            FLAG_ArmamentGndOrideOnPLT = 415,
            FLAG_APUPLT = 406,
            FLAG_MasterWarningCPG = 806,
            FLAG_MasterCautionCPG = 808,
            FLAG_FireEng1CPG = 441,
            FLAG_FireApuCPG = 443,
            FLAG_FireEng2CPG = 445,
            FLAG_RdyEng1CPG = 442,
            FLAG_RdyApuCPG = 444,
            FLAG_RdyEng2CPG = 446,
            FLAG_DischPriCPG = 447,
            FLAG_DischResCPG = 448,
            FLAG_EmergencyGuardCPG = 427,
            FLAG_EmergencyXpndrCPG = 428,
            FLAG_EmergencyEmergHydCPG = 429,
            FLAG_TailWheelUnlockCPG = 426,
            FLAG_ArmLTipCPG = 434,
            FLAG_ArmLOutbdCPG = 430,
            FLAG_ArmLInbdCPG = 431,
            FLAG_ArmRInbdCPG = 432,
            FLAG_ArmROutbdCPG = 433,
            FLAG_ArmRTipCPG = 435,
            FLAG_ArmamentASArmCPG = 438,
            FLAG_ArmamentASSafeCPG = 439,
            FLAG_ArmamentGndOrideOnCPG = 440,
            FLAG_ProcessorSelectSp1CPG = 436,
            FLAG_ProcessorSelectSp2CPG = 437
        };
        #endregion
        public AH64DInterface(string name)
            : base(name, "AH-64D_BLK_II", "pack://application:,,,/Helios;component/Interfaces/DCS/AH64D/ExportFunctions.lua")
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
#region MPDs
            #region Pilot Left
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.T1.ToString("d"), "20", "MFD Left (Pilot)", "Button T1"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.T2.ToString("d"), "21", "MFD Left (Pilot)", "Button T2"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.T3.ToString("d"), "22", "MFD Left (Pilot)", "Button T3"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.T4.ToString("d"), "23", "MFD Left (Pilot)", "Button T4"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.T5.ToString("d"), "24", "MFD Left (Pilot)", "Button T5"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.T6.ToString("d"), "25", "MFD Left (Pilot)", "Button T6"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.Asterisk.ToString("d"), "27", "MFD Left (Pilot)", "Button Asterisk"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.R1.ToString("d"), "28", "MFD Left (Pilot)", "Button R1"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.R2.ToString("d"), "29", "MFD Left (Pilot)", "Button R2"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.R3.ToString("d"), "30", "MFD Left (Pilot)", "Button R3"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.R4.ToString("d"), "31", "MFD Left (Pilot)", "Button R4"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.R5.ToString("d"), "32", "MFD Left (Pilot)", "Button R5"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.R6.ToString("d"), "33", "MFD Left (Pilot)", "Button R6"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.VID.ToString("d"), "34", "MFD Left (Pilot)", "Button VID"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.COM.ToString("d"), "35", "MFD Left (Pilot)", "Button COM"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.TSD.ToString("d"), "43", "MFD Left (Pilot)", "Button TSD"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.B1.ToString("d"), "42", "MFD Left (Pilot)", "Button B1/M(Menu)"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.B2.ToString("d"), "41", "MFD Left (Pilot)", "Button B2"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.B3.ToString("d"), "40", "MFD Left (Pilot)", "Button B3"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.B4.ToString("d"), "39", "MFD Left (Pilot)", "Button B4"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.B5.ToString("d"), "38", "MFD Left (Pilot)", "Button B5"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.B6.ToString("d"), "37", "MFD Left (Pilot)", "Button B6"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.AC.ToString("d"), "36", "MFD Left (Pilot)", "Button A/C"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.FCR.ToString("d"), "11", "MFD Left (Pilot)", "Button FCR"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.WPN.ToString("d"), "10", "MFD Left (Pilot)", "Button WPN"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.L1.ToString("d"), "17", "MFD Left (Pilot)", "Button L1"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.L2.ToString("d"), "16", "MFD Left (Pilot)", "Button L2"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.L3.ToString("d"), "15", "MFD Left (Pilot)", "Button L3"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.L4.ToString("d"), "14", "MFD Left (Pilot)", "Button L4"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.L5.ToString("d"), "13", "MFD Left (Pilot)", "Button L5"));
            AddFunction(new PushButton(this, MFD_PLT_LEFT, mpd_commands.L6.ToString("d"), "12", "MFD Left (Pilot)", "Button L6"));
            AddFunction(new Axis(this, MFD_PLT_LEFT, mpd_commands.BRT_KNOB.ToString("d"), "18", 0.1d, 0d, 1d, "MFD Left (Pilot)", "Brightness Control Knob"));
            AddFunction(new Axis(this, MFD_PLT_LEFT, mpd_commands.VID_KNOB.ToString("d"), "19", 0.1d, 0d, 1d, "MFD Left (Pilot)", "Video Control Knob"));
            AddFunction(Switch.CreateThreeWaySwitch(this, MFD_PLT_LEFT, mpd_commands.MODE_KNOB.ToString("d"), "26", "1.0", "Day", "0.5", "Night", "0.0", "Mono", "MFD Left (Pilot)", "Mode Knob", "%0.1f"));

#endregion
#region Pilot Right
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.T1.ToString("d"), "54", "MFD Right (Pilot)", "Button T1"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.T2.ToString("d"), "55", "MFD Right (Pilot)", "Button T2"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.T3.ToString("d"), "56", "MFD Right (Pilot)", "Button T3"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.T4.ToString("d"), "57", "MFD Right (Pilot)", "Button T4"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.T5.ToString("d"), "58", "MFD Right (Pilot)", "Button T5"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.T6.ToString("d"), "59", "MFD Right (Pilot)", "Button T6"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.Asterisk.ToString("d"), "61", "MFD Right (Pilot)", "Button Asterisk"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.R1.ToString("d"), "62", "MFD Right (Pilot)", "Button R1"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.R2.ToString("d"), "63", "MFD Right (Pilot)", "Button R2"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.R3.ToString("d"), "64", "MFD Right (Pilot)", "Button R3"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.R4.ToString("d"), "65", "MFD Right (Pilot)", "Button R4"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.R5.ToString("d"), "66", "MFD Right (Pilot)", "Button R5"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.R6.ToString("d"), "67", "MFD Right (Pilot)", "Button R6"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.VID.ToString("d"), "68", "MFD Right (Pilot)", "Button VID"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.COM.ToString("d"), "69", "MFD Right (Pilot)", "Button COM"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.TSD.ToString("d"), "77", "MFD Right (Pilot)", "Button TSD"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.B1.ToString("d"), "76", "MFD Right (Pilot)", "Button B1/M(Menu)"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.B2.ToString("d"), "75", "MFD Right (Pilot)", "Button B2"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.B3.ToString("d"), "74", "MFD Right (Pilot)", "Button B3"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.B4.ToString("d"), "73", "MFD Right (Pilot)", "Button B4"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.B5.ToString("d"), "72", "MFD Right (Pilot)", "Button B5"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.B6.ToString("d"), "71", "MFD Right (Pilot)", "Button B6"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.AC.ToString("d"), "70", "MFD Right (Pilot)", "Button A/C"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.FCR.ToString("d"), "45", "MFD Right (Pilot)", "Button FCR"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.WPN.ToString("d"), "44", "MFD Right (Pilot)", "Button WPN"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.L1.ToString("d"), "51", "MFD Right (Pilot)", "Button L1"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.L2.ToString("d"), "50", "MFD Right (Pilot)", "Button L2"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.L3.ToString("d"), "49", "MFD Right (Pilot)", "Button L3"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.L4.ToString("d"), "48", "MFD Right (Pilot)", "Button L4"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.L5.ToString("d"), "47", "MFD Right (Pilot)", "Button L5"));
            AddFunction(new PushButton(this, MFD_PLT_RIGHT, mpd_commands.L6.ToString("d"), "46", "MFD Right (Pilot)", "Button L6"));
            AddFunction(new Axis(this, MFD_PLT_RIGHT, mpd_commands.BRT_KNOB.ToString("d"), "52", 0.1d, 0d, 1d, "MFD Right (Pilot)", "Brightness Control Knob"));
            AddFunction(new Axis(this, MFD_PLT_RIGHT, mpd_commands.VID_KNOB.ToString("d"), "53", 0.1d, 0d, 1d, "MFD Right (Pilot)", "Video Control Knob"));
            AddFunction(Switch.CreateThreeWaySwitch(this, MFD_PLT_RIGHT, mpd_commands.MODE_KNOB.ToString("d"), "60", "1.0", "Day", "0.5", "Night", "0.0", "Mono", "MFD Right (Pilot)", "Mode Knob", "%0.1f"));

#endregion
#region CP/G Left
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.T1.ToString("d"), "88", "MFD Left (CP/G)", "Button T1"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.T2.ToString("d"), "89", "MFD Left (CP/G)", "Button T2"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.T3.ToString("d"), "90", "MFD Left (CP/G)", "Button T3"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.T4.ToString("d"), "91", "MFD Left (CP/G)", "Button T4"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.T5.ToString("d"), "92", "MFD Left (CP/G)", "Button T5"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.T6.ToString("d"), "93", "MFD Left (CP/G)", "Button T6"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.Asterisk.ToString("d"), "95", "MFD Left (CP/G)", "Button Asterisk"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.R1.ToString("d"), "96", "MFD Left (CP/G)", "Button R1"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.R2.ToString("d"), "97", "MFD Left (CP/G)", "Button R2"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.R3.ToString("d"), "98", "MFD Left (CP/G)", "Button R3"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.R4.ToString("d"), "99", "MFD Left (CP/G)", "Button R4"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.R5.ToString("d"), "100", "MFD Left (CP/G)", "Button R5"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.R6.ToString("d"), "101", "MFD Left (CP/G)", "Button R6"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.VID.ToString("d"), "102", "MFD Left (CP/G)", "Button VID"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.COM.ToString("d"), "103", "MFD Left (CP/G)", "Button COM"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.TSD.ToString("d"), "111", "MFD Left (CP/G)", "Button TSD"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.B1.ToString("d"), "110", "MFD Left (CP/G)", "Button B1/M(Menu)"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.B2.ToString("d"), "109", "MFD Left (CP/G)", "Button B2"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.B3.ToString("d"), "108", "MFD Left (CP/G)", "Button B3"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.B4.ToString("d"), "107", "MFD Left (CP/G)", "Button B4"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.B5.ToString("d"), "106", "MFD Left (CP/G)", "Button B5"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.B6.ToString("d"), "105", "MFD Left (CP/G)", "Button B6"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.AC.ToString("d"), "104", "MFD Left (CP/G)", "Button A/C"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.FCR.ToString("d"), "79", "MFD Left (CP/G)", "Button FCR"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.WPN.ToString("d"), "78", "MFD Left (CP/G)", "Button WPN"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.L1.ToString("d"), "85", "MFD Left (CP/G)", "Button L1"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.L2.ToString("d"), "84", "MFD Left (CP/G)", "Button L2"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.L3.ToString("d"), "83", "MFD Left (CP/G)", "Button L3"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.L4.ToString("d"), "82", "MFD Left (CP/G)", "Button L4"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.L5.ToString("d"), "81", "MFD Left (CP/G)", "Button L5"));
            AddFunction(new PushButton(this, MFD_CPG_LEFT, mpd_commands.L6.ToString("d"), "80", "MFD Left (CP/G)", "Button L6"));
            AddFunction(new Axis(this, MFD_CPG_LEFT, mpd_commands.BRT_KNOB.ToString("d"), "86", 0.1d, 0d, 1d, "MFD Left (CP/G)", "Brightness Control Knob"));
            AddFunction(new Axis(this, MFD_CPG_LEFT, mpd_commands.VID_KNOB.ToString("d"), "87", 0.1d, 0d, 1d, "MFD Left (CP/G)", "Video Control Knob"));
            AddFunction(Switch.CreateThreeWaySwitch(this, MFD_CPG_LEFT, mpd_commands.MODE_KNOB.ToString("d"), "94", "1.0", "Day", "0.5", "Night", "0.0", "Mono", "MFD Left (CP/G)", "Mode Knob", "%0.1f"));

#endregion
#region CP/G Right
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.T1.ToString("d"), "122", "MFD Right (CP/G)", "Button T1"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.T2.ToString("d"), "123", "MFD Right (CP/G)", "Button T2"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.T3.ToString("d"), "124", "MFD Right (CP/G)", "Button T3"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.T4.ToString("d"), "125", "MFD Right (CP/G)", "Button T4"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.T5.ToString("d"), "126", "MFD Right (CP/G)", "Button T5"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.T6.ToString("d"), "127", "MFD Right (CP/G)", "Button T6"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.Asterisk.ToString("d"), "129", "MFD Right (CP/G)", "Button Asterisk"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.R1.ToString("d"), "130", "MFD Right (CP/G)", "Button R1"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.R2.ToString("d"), "131", "MFD Right (CP/G)", "Button R2"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.R3.ToString("d"), "132", "MFD Right (CP/G)", "Button R3"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.R4.ToString("d"), "133", "MFD Right (CP/G)", "Button R4"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.R5.ToString("d"), "134", "MFD Right (CP/G)", "Button R5"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.R6.ToString("d"), "135", "MFD Right (CP/G)", "Button R6"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.VID.ToString("d"), "136", "MFD Right (CP/G)", "Button VID"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.COM.ToString("d"), "137", "MFD Right (CP/G)", "Button COM"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.TSD.ToString("d"), "145", "MFD Right (CP/G)", "Button TSD"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.B1.ToString("d"), "144", "MFD Right (CP/G)", "Button B1/M(Menu)"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.B2.ToString("d"), "143", "MFD Right (CP/G)", "Button B2"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.B3.ToString("d"), "142", "MFD Right (CP/G)", "Button B3"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.B4.ToString("d"), "141", "MFD Right (CP/G)", "Button B4"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.B5.ToString("d"), "140", "MFD Right (CP/G)", "Button B5"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.B6.ToString("d"), "139", "MFD Right (CP/G)", "Button B6"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.AC.ToString("d"), "138", "MFD Right (CP/G)", "Button A/C"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.FCR.ToString("d"), "113", "MFD Right (CP/G)", "Button FCR"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.WPN.ToString("d"), "112", "MFD Right (CP/G)", "Button WPN"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.L1.ToString("d"), "119", "MFD Right (CP/G)", "Button L1"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.L2.ToString("d"), "118", "MFD Right (CP/G)", "Button L2"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.L3.ToString("d"), "117", "MFD Right (CP/G)", "Button L3"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.L4.ToString("d"), "116", "MFD Right (CP/G)", "Button L4"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.L5.ToString("d"), "115", "MFD Right (CP/G)", "Button L5"));
            AddFunction(new PushButton(this, MFD_CPG_RIGHT, mpd_commands.L6.ToString("d"), "114", "MFD Right (CP/G)", "Button L6"));
            AddFunction(new Axis(this, MFD_CPG_RIGHT, mpd_commands.BRT_KNOB.ToString("d"), "120", 0.1d, 0d, 1d, "MFD Right (CP/G)", "Brightness Control Knob"));
            AddFunction(new Axis(this, MFD_CPG_RIGHT, mpd_commands.VID_KNOB.ToString("d"), "121", 0.1d, 0d, 1d, "MFD Right (CP/G)", "Video Control Knob"));
            AddFunction(Switch.CreateThreeWaySwitch(this, MFD_CPG_RIGHT, mpd_commands.MODE_KNOB.ToString("d"), "128", "1.0", "Day", "0.5", "Night", "0.0", "Mono", "MFD Right (CP/G)", "Mode Knob", "%0.1f"));
#endregion
#endregion
#region Keyboard Unit
#region Pilot
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyA.ToString("d"), "213", "Keyboard Unit (Pilot)", "Key A"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyB.ToString("d"), "214", "Keyboard Unit (Pilot)", "Key B"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyC.ToString("d"), "215", "Keyboard Unit (Pilot)", "Key C"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyD.ToString("d"), "216", "Keyboard Unit (Pilot)", "Key D"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyE.ToString("d"), "217", "Keyboard Unit (Pilot)", "Key E"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyF.ToString("d"), "218", "Keyboard Unit (Pilot)", "Key F"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyG.ToString("d"), "222", "Keyboard Unit (Pilot)", "Key G"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyH.ToString("d"), "223", "Keyboard Unit (Pilot)", "Key H"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyI.ToString("d"), "224", "Keyboard Unit (Pilot)", "Key I"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyJ.ToString("d"), "225", "Keyboard Unit (Pilot)", "Key J"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyK.ToString("d"), "226", "Keyboard Unit (Pilot)", "Key K"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyL.ToString("d"), "227", "Keyboard Unit (Pilot)", "Key L"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyM.ToString("d"), "231", "Keyboard Unit (Pilot)", "Key M"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyN.ToString("d"), "232", "Keyboard Unit (Pilot)", "Key N"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyO.ToString("d"), "233", "Keyboard Unit (Pilot)", "Key O"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyP.ToString("d"), "234", "Keyboard Unit (Pilot)", "Key P"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyQ.ToString("d"), "235", "Keyboard Unit (Pilot)", "Key Q"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyR.ToString("d"), "236", "Keyboard Unit (Pilot)", "Key R"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyS.ToString("d"), "240", "Keyboard Unit (Pilot)", "Key S"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyT.ToString("d"), "241", "Keyboard Unit (Pilot)", "Key T"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyU.ToString("d"), "242", "Keyboard Unit (Pilot)", "Key U"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyV.ToString("d"), "243", "Keyboard Unit (Pilot)", "Key V"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyW.ToString("d"), "244", "Keyboard Unit (Pilot)", "Key W"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyX.ToString("d"), "245", "Keyboard Unit (Pilot)", "Key X"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyY.ToString("d"), "249", "Keyboard Unit (Pilot)", "Key Y"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyZ.ToString("d"), "250", "Keyboard Unit (Pilot)", "Key Z"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keySlash.ToString("d"), "251", "Keyboard Unit (Pilot)", "Key /"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key0.ToString("d"), "247", "Keyboard Unit (Pilot)", "Key 0"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key1.ToString("d"), "219", "Keyboard Unit (Pilot)", "Key 1"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key2.ToString("d"), "220", "Keyboard Unit (Pilot)", "Key 2"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key3.ToString("d"), "221", "Keyboard Unit (Pilot)", "Key 3"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key4.ToString("d"), "228", "Keyboard Unit (Pilot)", "Key 4"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key5.ToString("d"), "229", "Keyboard Unit (Pilot)", "Key 5"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key6.ToString("d"), "230", "Keyboard Unit (Pilot)", "Key 6"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key7.ToString("d"), "237", "Keyboard Unit (Pilot)", "Key 7"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key8.ToString("d"), "238", "Keyboard Unit (Pilot)", "Key 8"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.key9.ToString("d"), "239", "Keyboard Unit (Pilot)", "Key 9"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyDot.ToString("d"), "246", "Keyboard Unit (Pilot)", "Key Decimal"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keySign.ToString("d"), "248", "Keyboard Unit (Pilot)", "Key +/-"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyBKS.ToString("d"), "252", "Keyboard Unit (Pilot)", "Key BKS"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keySPC.ToString("d"), "253", "Keyboard Unit (Pilot)", "Key SPC"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyMultiply.ToString("d"), "254", "Keyboard Unit (Pilot)", "Key *"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyDivide.ToString("d"), "255", "Keyboard Unit (Pilot)", "Key DIV"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyPlus.ToString("d"), "256", "Keyboard Unit (Pilot)", "Key +"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyMinus.ToString("d"), "257", "Keyboard Unit (Pilot)", "Key -"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyCLR.ToString("d"), "258", "Keyboard Unit (Pilot)", "Key CLR"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyLeft.ToString("d"), "259", "Keyboard Unit (Pilot)", "Key <"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyRight.ToString("d"), "260", "Keyboard Unit (Pilot)", "Key >"));
            AddFunction(new PushButton(this, KU_PLT, KU_commands.keyEnter.ToString("d"), "261", "Keyboard Unit (Pilot)", "Key ENTER"));
            AddFunction(new Axis(this, KU_PLT, KU_commands.BrightnessKnob.ToString("d"), "316", 0.1d, 0d, 1d, "Keyboard Unit (Pilot)", "Brightness Control Knob"));
            AddFunction(new Text(this, "2080", "Keyboard Unit (Pilot)", "Scratchpad", "Keyboard Unit Display (Pilot)"));  // 22 Characters

#endregion
#region CP/G
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyA.ToString("d"), "164", "Keyboard Unit (CP/G)", "Key A"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyB.ToString("d"), "165", "Keyboard Unit (CP/G)", "Key B"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyC.ToString("d"), "166", "Keyboard Unit (CP/G)", "Key C"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyD.ToString("d"), "167", "Keyboard Unit (CP/G)", "Key D"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyE.ToString("d"), "168", "Keyboard Unit (CP/G)", "Key E"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyF.ToString("d"), "169", "Keyboard Unit (CP/G)", "Key F"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyG.ToString("d"), "173", "Keyboard Unit (CP/G)", "Key G"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyH.ToString("d"), "174", "Keyboard Unit (CP/G)", "Key H"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyI.ToString("d"), "175", "Keyboard Unit (CP/G)", "Key I"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyJ.ToString("d"), "176", "Keyboard Unit (CP/G)", "Key J"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyK.ToString("d"), "177", "Keyboard Unit (CP/G)", "Key K"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyL.ToString("d"), "178", "Keyboard Unit (CP/G)", "Key L"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyM.ToString("d"), "182", "Keyboard Unit (CP/G)", "Key M"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyN.ToString("d"), "183", "Keyboard Unit (CP/G)", "Key N"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyO.ToString("d"), "184", "Keyboard Unit (CP/G)", "Key O"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyP.ToString("d"), "185", "Keyboard Unit (CP/G)", "Key P"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyQ.ToString("d"), "186", "Keyboard Unit (CP/G)", "Key Q"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyR.ToString("d"), "187", "Keyboard Unit (CP/G)", "Key R"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyS.ToString("d"), "191", "Keyboard Unit (CP/G)", "Key S"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyT.ToString("d"), "192", "Keyboard Unit (CP/G)", "Key T"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyU.ToString("d"), "193", "Keyboard Unit (CP/G)", "Key U"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyV.ToString("d"), "194", "Keyboard Unit (CP/G)", "Key V"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyW.ToString("d"), "195", "Keyboard Unit (CP/G)", "Key W"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyX.ToString("d"), "196", "Keyboard Unit (CP/G)", "Key X"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyY.ToString("d"), "200", "Keyboard Unit (CP/G)", "Key Y"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyZ.ToString("d"), "201", "Keyboard Unit (CP/G)", "Key Z"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keySlash.ToString("d"), "202", "Keyboard Unit (CP/G)", "Key /"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key0.ToString("d"), "198", "Keyboard Unit (CP/G)", "Key 0"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key1.ToString("d"), "170", "Keyboard Unit (CP/G)", "Key 1"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key2.ToString("d"), "171", "Keyboard Unit (CP/G)", "Key 2"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key3.ToString("d"), "172", "Keyboard Unit (CP/G)", "Key 3"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key4.ToString("d"), "179", "Keyboard Unit (CP/G)", "Key 4"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key5.ToString("d"), "180", "Keyboard Unit (CP/G)", "Key 5"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key6.ToString("d"), "181", "Keyboard Unit (CP/G)", "Key 6"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key7.ToString("d"), "188", "Keyboard Unit (CP/G)", "Key 7"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key8.ToString("d"), "189", "Keyboard Unit (CP/G)", "Key 8"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.key9.ToString("d"), "190", "Keyboard Unit (CP/G)", "Key 9"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyDot.ToString("d"), "197", "Keyboard Unit (CP/G)", "Key Decimal"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keySign.ToString("d"), "199", "Keyboard Unit (CP/G)", "Key +/-"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyBKS.ToString("d"), "203", "Keyboard Unit (CP/G)", "Key BKS"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keySPC.ToString("d"), "204", "Keyboard Unit (CP/G)", "Key SPC"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyMultiply.ToString("d"), "205", "Keyboard Unit (CP/G)", "Key *"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyDivide.ToString("d"), "206", "Keyboard Unit (CP/G)", "Key DIV"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyPlus.ToString("d"), "207", "Keyboard Unit (CP/G)", "Key +"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyMinus.ToString("d"), "208", "Keyboard Unit (CP/G)", "Key -"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyCLR.ToString("d"), "209", "Keyboard Unit (CP/G)", "Key CLR"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyLeft.ToString("d"), "210", "Keyboard Unit (CP/G)", "Key <"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyRight.ToString("d"), "211", "Keyboard Unit (CP/G)", "Key >"));
            AddFunction(new PushButton(this, KU_CPG, KU_commands.keyEnter.ToString("d"), "212", "Keyboard Unit (CP/G)", "Key ENTER"));
            AddFunction(new Axis(this, KU_CPG, KU_commands.BrightnessKnob.ToString("d"), "621", 0.1d, 0d, 1d, "Keyboard Unit (CP/G)", "Brightness Control Knob"));
            AddFunction(new Text(this, "2081", "Keyboard Unit (CP/G)", "Scratchpad", "Keyboard Unit Display (CP/G)")); // 22 characters
#endregion
#endregion
#region External Lighting
            AddFunction(Switch.CreateThreeWaySwitch(this, EXTLIGHTS_SYSTEM, extlights_commands.NavLights.ToString("d"), "326", "1.0", "BRT", "0.0", "Off", "-1.0", "DIM", "External Lights", "Navigation Lights Switch", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, EXTLIGHTS_SYSTEM, extlights_commands.AntiCollLights.ToString("d"), "332", "1.0", "White", "0.0", "Off", "-1.0", "Red", "External Lights", "Anti-Collision Lights Switch", "%0.1f"));
            AddFunction(new Axis(this, EXTLIGHTS_SYSTEM, extlights_commands.FormationLights.ToString("d"), "329", 0.1d, 0d, 1d, "External Lights", "Formation Lights Control Knob"));

#endregion
#region Cockpit Lighting
#region Pilot
            AddFunction(new PushButton(this, CPTLIGHTS_SYSTEM, intlights_commands.MasterCautionPLT.ToString("d"), "305", "Cockpit Lights (Pilot)", "Master Caution Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_MasterCautionPLT.ToString("d"), "Cockpit Lights (Pilot)", "Master Caution Indicator", ""));
            AddFunction(new PushButton(this, CPTLIGHTS_SYSTEM, intlights_commands.MasterWarningPLT.ToString("d"), "304", "Cockpit Lights (Pilot)", "Master Warning Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_MasterWarningPLT.ToString("d"), "Cockpit Lights (Pilot)", "Master Warning Indicator", ""));

            AddFunction(new PushButton(this, CPTLIGHTS_SYSTEM, intlights_commands.TestLightsPLT.ToString("d"), "333", "Cockpit Lights (Pilot)", "Test Lights"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.SignalPLT.ToString("d"), "327", 0.1d, 0d, 1d, "Cockpit Lights (Pilot)", "Signal Lights Control Knob"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.PrimaryPLT.ToString("d"), "328", 0.1d, 0d, 1d, "Cockpit Lights (Pilot)", "Primary Lights Control Knob"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.FloodPLT.ToString("d"), "330", 0.1d, 0d, 1d, "Cockpit Lights (Pilot)", "Flood Lights Control Knob"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.StbyInstPLT.ToString("d"), "331", 0.1d, 0d, 1d, "Cockpit Lights (Pilot)", "Standby Instrument Control Knob"));
            //--elements[""] = default_rheostat(CREW.PLT, _("Utility Lights Rheostat Control"), devices.CPTLIGHTS_SYSTEM, intlights_commands.UtilityPLT,          )
            //--elements[""] = default_button(CREW.PLT, _("Press To Hold Brt Button"), devices.CPTLIGHTS_SYSTEM, intlights_commands.UtilityButtonPLT,    )

#endregion
#region CP/G
            AddFunction(new PushButton(this, CPTLIGHTS_SYSTEM, intlights_commands.MasterCautionCPG.ToString("d"), "807", "Cockpit Lights (CP/G)", "Master Caution"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_MasterCautionCPG.ToString("d"), "Cockpit Lights (CP/G)", "Master Caution Indicator", ""));
            AddFunction(new PushButton(this, CPTLIGHTS_SYSTEM, intlights_commands.MasterWarningCPG.ToString("d"), "805", "Cockpit Lights (CP/G)", "Master Warning"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_MasterWarningCPG.ToString("d"), "Cockpit Lights (CP/G)", "Master Warning Indicator", ""));
            AddFunction(new PushButton(this, CPTLIGHTS_SYSTEM, intlights_commands.TestLightsCPG.ToString("d"), "367", "Cockpit Lights (CP/G)", "Test Lights"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.SignalCPG.ToString("d"), "364", 0.1d, 0d, 1d, "Cockpit Lights (CP/G)", "Signal Lights Control Knob"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.PrimaryCPG.ToString("d"), "365", 0.1d, 0d, 1d, "Cockpit Lights (CP/G)", "Primary Lights Control Knob"));
            AddFunction(new Axis(this, CPTLIGHTS_SYSTEM, intlights_commands.FloodCPG.ToString("d"), "366", 0.1d, 0d, 1d, "Cockpit Lights (CP/G)", "Flood Lights Control Knob"));
            //--elements[""] = default_rheostat(CREW.CPG, _("Utility Lights Rheostat Control"), devices.CPTLIGHTS_SYSTEM, intlights_commands.UtilityCPG,          )
            //--elements[""] = default_button(CREW.CPG, _("Press To Hold Brt Button"), devices.CPTLIGHTS_SYSTEM, intlights_commands.UtilityButtonCPG,    )
#endregion
#endregion
#region Armament Panel
#region Pilot
            AddFunction(new PushButton(this, ELEC_INTERFACE, electric_commands.ARM_SAFE_PLT_BTN.ToString("d"), "306", "Armament Panel (Pilot)", "Master Arm"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmamentASArmPLT.ToString("d"), "Armament Panel (Pilot)", "Armed Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmamentASSafePLT.ToString("d"), "Armament Panel (Pilot)", "Safe Indicator", ""));
            AddFunction(new PushButton(this, ELEC_INTERFACE, electric_commands.GND_ORIDE_PLT_BTN.ToString("d"), "307", "Armament Panel (Pilot)", "Ground Override"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmamentGndOrideOnPLT.ToString("d"), "Armament Panel (Pilot)", "Ground Override On Indicator", ""));
#endregion
#region CP/G
            AddFunction(new PushButton(this, ELEC_INTERFACE, electric_commands.ARM_SAFE_CPG_BTN.ToString("d"), "293", "Armament Panel (CP/G)", "Master Arm"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmamentASArmCPG.ToString("d"), "Armament Panel (CP/G)", "Armed Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmamentASSafeCPG.ToString("d"), "Armament Panel (CP/G)", "Safe Indicator", ""));
            AddFunction(new PushButton(this, ELEC_INTERFACE, electric_commands.GND_ORIDE_CPG_BTN.ToString("d"), "294", "Armament Panel (CP/G)", "Ground Override"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmamentGndOrideOnCPG.ToString("d"), "Armament Panel (CP/G)", "Ground Override On Indicator", ""));
#endregion
#endregion
#region Emergency Panel
#region Pilot
            AddFunction(new PushButton(this, EMERGENCY_PANEL, intercom_commands.PLT_UHF_GUARD_Btn.ToString("d"), "310", "Emergency Panel (Pilot)", "UHF Guard Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_EmergencyGuardPLT.ToString("d"), "Emergency Panel (Pilot)", "Emergency UHF Guard Indicator", ""));
            AddFunction(new PushButton(this, EMERGENCY_PANEL, intercom_commands.PLT_XPNDR_Btn.ToString("d"), "311", "Emergency Panel (Pilot)", "XPNDR Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_EmergencyXpndrPLT.ToString("d"), "Emergency Panel (Pilot)", "Emergency XPNDR Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, EMERGENCY_PANEL, intercom_commands.PLT_ZEROIZE_Sw.ToString("d"), "312", "1.0", "On", "0.0", "Off", "Emergency Panel (Pilot)", "Zeroize Switch", "%0.1f"));
#endregion
#region CP/G            
            AddFunction(new PushButton(this, EMERGENCY_PANEL, intercom_commands.CPG_UHF_GUARD_Btn.ToString("d"), "358", "Emergency Panel (CP/G)", "UHF Guard Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_EmergencyGuardCPG.ToString("d"), "Emergency Panel (CP/G)", "Emergency UHF Guard Indicator", ""));
            AddFunction(new PushButton(this, EMERGENCY_PANEL, intercom_commands.CPG_XPNDR_Btn.ToString("d"), "359", "Emergency Panel (CP/G)", "XPNDR Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_EmergencyXpndrCPG.ToString("d"), "Emergency Panel (CP/G)", "Emergency XPNDR Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, EMERGENCY_PANEL, intercom_commands.CPG_ZEROIZE_Sw.ToString("d"), "360", "1.0", "On", "0.0", "Off", "Emergency Panel (CP/G)", "Zeroize Switch", "%0.1f"));
#endregion
#endregion
#region Instrument Panel
#region Pilot
            AddFunction(Switch.CreateToggleSwitch(this, EMERGENCY_PANEL, intercom_commands.PLT_MasterZeroizeSw.ToString("d"), "804", "1.0", "On", "0.0", "Off", "Emergency Panel (Pilot)", "Master Zeroize Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, EMERGENCY_PANEL, intercom_commands.PLT_MasterZeroizeSwCover.ToString("d"), "803", "1.0", "Open", "0.0", "Closed", "Emergency Panel (Pilot)", "Master Zeroize Switch Cover", "%0.1f"));
#endregion
#region CP/G
            AddFunction(Switch.CreateToggleSwitch(this, EMERGENCY_PANEL, intercom_commands.CPG_MasterZeroizeSw.ToString("d"), "802", "1.0", "On", "0.0", "Off", "Emergency Panel (CP/G)", "Master Zeroize Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, EMERGENCY_PANEL, intercom_commands.CPG_MasterZeroizeSwCover.ToString("d"), "801", "1.0", "Open", "0.0", "Closed", "Emergency Panel (CP/G)", "Master Zeroize Switch Cover", "%0.1f"));
#endregion
#endregion
#region Enhanced Up - Front Display
#region Pilot
            AddFunction(new Switch(this, EUFD_PLT, "271", new SwitchPosition[] { new SwitchPosition("1.0", "Up", eufd_commands.WCA_UP.ToString("d"), eufd_commands.WCA_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", eufd_commands.WCA_DOWN.ToString("d"), eufd_commands.WCA_DOWN.ToString("d"), "0.0", "0.0") }, "Up Front Display (Pilot)", "WCA Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, EUFD_PLT, "270", new SwitchPosition[] { new SwitchPosition("1.0", "Up", eufd_commands.IDM_UP.ToString("d"), eufd_commands.IDM_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", eufd_commands.IDM_DOWN.ToString("d"), eufd_commands.IDM_DOWN.ToString("d"), "0.0", "0.0") }, "Up Front Display (Pilot)", "IDM Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, EUFD_PLT, "272", new SwitchPosition[] { new SwitchPosition("1.0", "Up", eufd_commands.RTS_UP.ToString("d"), eufd_commands.RTS_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", eufd_commands.RTS_DOWN.ToString("d"), eufd_commands.RTS_DOWN.ToString("d"), "0.0", "0.0") }, "Up Front Display (Pilot)", "RTS Rocker Switch", "%0.1f"));
            AddFunction(new Axis(this, EUFD_PLT, eufd_commands.BRT.ToString("d"), "273", 0.1d, 0d, 1d, "Up Front Display (Pilot)", "Brightness Control Knob"));
            AddFunction(new PushButton(this, EUFD_PLT, eufd_commands.Enter.ToString("d"), "275", "Up Front Display (Pilot)", "Enter Button"));
            AddFunction(new PushButton(this, EUFD_PLT, eufd_commands.Swap.ToString("d"), "277", "Up Front Display (Pilot)", "Swap Button"));
            AddFunction(new PushButton(this, EUFD_PLT, eufd_commands.Preset.ToString("d"), "274", "Up Front Display (Pilot)", "Preset Button"));
            AddFunction(new PushButton(this, EUFD_PLT, eufd_commands.Stopwatch.ToString("d"), "276", "Up Front Display (Pilot)", "Stopwatch Button"));

#endregion
#region CP/G
            AddFunction(new Switch(this, EUFD_CPG, "263", new SwitchPosition[] { new SwitchPosition("1.0", "Up", eufd_commands.WCA_UP.ToString("d"), eufd_commands.WCA_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", eufd_commands.WCA_DOWN.ToString("d"), eufd_commands.WCA_DOWN.ToString("d"), "0.0", "0.0") }, "Up Front Display (CP/G)", "WCA Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, EUFD_CPG, "262", new SwitchPosition[] { new SwitchPosition("1.0", "Up", eufd_commands.IDM_UP.ToString("d"), eufd_commands.IDM_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", eufd_commands.IDM_DOWN.ToString("d"), eufd_commands.IDM_DOWN.ToString("d"), "0.0", "0.0") }, "Up Front Display (CP/G)", "IDM Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, EUFD_CPG, "264", new SwitchPosition[] { new SwitchPosition("1.0", "Up", eufd_commands.RTS_UP.ToString("d"), eufd_commands.RTS_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", eufd_commands.RTS_DOWN.ToString("d"), eufd_commands.RTS_DOWN.ToString("d"), "0.0", "0.0") }, "Up Front Display (CP/G)", "RTS Rocker Switch", "%0.1f"));
            AddFunction(new Axis(this, EUFD_CPG, eufd_commands.BRT.ToString("d"), "265", 0.1d, 0d, 1d, "Up Front Display (CP/G)", "Brightness Control Knob"));
            AddFunction(new PushButton(this, EUFD_CPG, eufd_commands.Enter.ToString("d"), "267", "Up Front Display (CP/G)", "Enter Button"));
            AddFunction(new PushButton(this, EUFD_CPG, eufd_commands.Swap.ToString("d"), "269", "Up Front Display (CP/G)", "Swap Button"));
            AddFunction(new PushButton(this, EUFD_CPG, eufd_commands.Preset.ToString("d"), "266", "Up Front Display (CP/G)", "Preset Button"));
            AddFunction(new PushButton(this, EUFD_CPG, eufd_commands.Stopwatch.ToString("d"), "268", "Up Front Display (CP/G)", "Stopwatch Button"));

#endregion
#endregion
#region TEDAC Display
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_VIDEO_SELECT_TAD_BTN.ToString("d"), "150", "TEDAC", "TAD Video Select Button"));  // Press to select TADS as the video source
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_VIDEO_SELECT_FCR_BTN.ToString("d"), "151", "TEDAC", "FCR Video Select Button"));  // Press to select FCR targeting format
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_VIDEO_SELECT_PNV_BTN.ToString("d"), "152", "TEDAC", "PNV Video Select Button"));  // Press to select PNVS as the video source
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_VIDEO_SELECT_GS_BTN.ToString("d"), "153", "TEDAC", "G/S Video Select Button"));   // Press to activate grayscale for the video display

            AddFunction(new Switch(this, TEDAC, "155", new SwitchPosition[] { new SwitchPosition("1.0", "Inc", tedac_commands.TDU_SYM_INC.ToString("d"), tedac_commands.TDU_SYM_INC.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Mid", null), new SwitchPosition("-1.0", "Dec", tedac_commands.TDU_SYM_DEC.ToString("d"), tedac_commands.TDU_SYM_DEC.ToString("d"), "0.0", "0.0") }, "TEDAC", "SYM Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, TEDAC, "156", new SwitchPosition[] { new SwitchPosition("1.0", "Inc", tedac_commands.TDU_BRT_INC.ToString("d"), tedac_commands.TDU_BRT_INC.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Mid", null), new SwitchPosition("-1.0", "Dec", tedac_commands.TDU_BRT_DEC.ToString("d"), tedac_commands.TDU_BRT_DEC.ToString("d"), "0.0", "0.0") }, "TEDAC", "BRT Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, TEDAC, "157", new SwitchPosition[] { new SwitchPosition("1.0", "Inc", tedac_commands.TDU_CON_INC.ToString("d"), tedac_commands.TDU_CON_INC.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Mid", null), new SwitchPosition("-1.0", "Dec", tedac_commands.TDU_CON_DEC.ToString("d"), tedac_commands.TDU_CON_DEC.ToString("d"), "0.0", "0.0") }, "TEDAC", "CON Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, TEDAC, "147", new SwitchPosition[] { new SwitchPosition("1.0", "Up", tedac_commands.TDU_RF_UP.ToString("d"), tedac_commands.TDU_RF_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Mid", null), new SwitchPosition("-1.0", "Down", tedac_commands.TDU_RF_DOWN.ToString("d"), tedac_commands.TDU_RF_DOWN.ToString("d"), "0.0", "0.0") }, "TEDAC", "R/F Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, TEDAC, "146", new SwitchPosition[] { new SwitchPosition("1.0", "Up", tedac_commands.TDU_EL_UP.ToString("d"), tedac_commands.TDU_EL_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Mid", null), new SwitchPosition("-1.0", "Down", tedac_commands.TDU_EL_DOWN.ToString("d"), tedac_commands.TDU_EL_DOWN.ToString("d"), "0.0", "0.0") }, "TEDAC", "EL Adjust Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, TEDAC, "163", new SwitchPosition[] { new SwitchPosition("1.0", "Left", tedac_commands.TDU_AZ_LEFT.ToString("d"), tedac_commands.TDU_AZ_LEFT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Mid", null), new SwitchPosition("-1.0", "Right", tedac_commands.TDU_AZ_RIGHT.ToString("d"), tedac_commands.TDU_AZ_RIGHT.ToString("d"), "0.0", "0.0") }, "TEDAC", "AZ Adjust Rocker Switch", "%0.1f"));

            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_ASTERISK_BTN.ToString("d"), "158", "TEDAC", "Asterisk (*) Button"));  //  Press to adjust the brightness and contrast to nominal settings
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_B1.ToString("d"), "162", "TEDAC", "AZ/EL Boresight Enable Button"));  // Press to enable boresight controls
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_B2.ToString("d"), "161", "TEDAC", "ACM Button"));
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_B3.ToString("d"), "160", "TEDAC", "FREEZE Button"));  // Press to freeze the video imaging on the TDU
            AddFunction(new PushButton(this, TEDAC, tedac_commands.TDU_B4.ToString("d"), "159", "TEDAC", "FILTER Button"));  // Press to select filter in the TADS FLIR sensor
            AddFunction(new Axis(this, TEDAC, tedac_commands.TDU_GAIN_KNOB.ToString("d"), "148", 0.1d, 0d, 1d, "TEDAC", "FLIR GAIN Control"));
            AddFunction(new Axis(this, TEDAC, tedac_commands.TDU_LEV_KNOB.ToString("d"), "149", 0.1d, 0d, 1d, "TEDAC", "FLIR LEV Control"));
            AddFunction(Switch.CreateThreeWaySwitch(this, TEDAC, tedac_commands.TDU_MODE_KNOB.ToString("d"), "154", "1.0", "Day", "0.5", "Night", "0.0", "Off", "TEDAC", "Display Mode", "%0.1f"));
#endregion
#region Video Control Panel,
            AddFunction(new Axis(this, ELEC_INTERFACE, electric_commands.VCP_IHADSS_BRT_KNOB.ToString("d"), "278", 0.1d, 0d, 1d, "Video Control Panel", "IHADSS BRT Control Knob"));
            AddFunction(new Axis(this, ELEC_INTERFACE, electric_commands.VCP_IHADSS_CON_KNOB.ToString("d"), "279", 0.1d, 0d, 1d, "Video Control Panel", "IHADSS CON Control Knob"));
            AddFunction(new Axis(this, ELEC_INTERFACE, electric_commands.VCP_SYM_BRT_KNOB.ToString("d"), "280", 0.1d, 0d, 1d, "Video Control Panel", "SYM BRT Control Knob"));
            AddFunction(new Axis(this, ELEC_INTERFACE, electric_commands.VCP_FLIR_LEV_KNOB.ToString("d"), "282", 0.1d, 0d, 1d, "Video Control Panel", "FLIR LVL Control Knob"));
            AddFunction(new Axis(this, ELEC_INTERFACE, electric_commands.VCP_FLIR_GAIN_KNOB.ToString("d"), "283", 0.1d, 0d, 1d, "Video Control Panel", "FLIR GAIN Control Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, electric_commands.VCP_ACM_SW.ToString("d"), "281", "1.0", "On", "0.0", "Off", "Video Control Panel", "Automatic Contrast Mode Switch", "%0.1f"));
#endregion
#region NVS MODE
#region Pilot
            AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, electric_commands.NVS_MODE_PLT_KNOB.ToString("d"), "309", "1.0", "Fixed", "0.0", "Norm", "-1.0", "Off", "NVS (Pilot)", "Mode Switch", "%0.1f"));
#endregion
#region CP/G
            AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, electric_commands.NVS_MODE_CPG_KNOB.ToString("d"), "363", "1.0", "Fixed", "0.0", "Norm", "-1.0", "Off", "NVS (CP/G)", "Mode Switch", "%0.1f"));
#endregion
#endregion
#region Left Console
#region Pilot
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORE_LO_JETTISON_ARMED.ToString("d"), "319", "Left Console (Pilot)", "L OUTBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmLOutbdPLT.ToString("d"), "Left Console (Pilot)", "Arm Left Outbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORE_LI_JETTISON_ARMED.ToString("d"), "320", "Left Console (Pilot)", "L INBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmLInbdPLT.ToString("d"), "Left Console (Pilot)", "Arm Left Inbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORE_RI_JETTISON_ARMED.ToString("d"), "321", "Left Console (Pilot)", "R INBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmRInbdPLT.ToString("d"), "Left Console (Pilot)", "Arm Right Inbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORE_RO_JETTISON_ARMED.ToString("d"), "322", "Left Console (Pilot)", "R OUTBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmROutbdPLT.ToString("d"), "Left Console (Pilot)", "Arm Right Outbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORE_JETTISON_LEFT_WINGTIP.ToString("d"), "323", "Left Console (Pilot)", "L TIP Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmLTipPLT.ToString("d"), "Left Console (Pilot)", "Arm Left Tip Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORE_JETTISON_RIGHT_WINGTIP.ToString("d"), "325", "Left Console (Pilot)", "R TIP Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmRTipPLT.ToString("d"), "Left Console (Pilot)", "Arm Right Tip Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_PLT, JETT_commands.STORES_JETT_PUSHBUTTON.ToString("d"), "324", "Left Console (Pilot)", "JETT Pushbutton"));  // Press to jettison stores from all armed stations
            AddFunction(new PushButton(this, HYDRO_INTERFACE, hydraulic_commands.Emergency_HYD_PLT.ToString("d"), "313", "Left Console (Pilot)", "EMERG HYD Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_EmergencyEmergHydPLT.ToString("d"), "Left Console (Pilot)", "Emergency Emerg Hyd Indicator", ""));
            AddFunction(new PushButton(this, HYDRO_INTERFACE, hydraulic_commands.TailWheelUnLock_PLT.ToString("d"), "308", "Left Console (Pilot)", "Tail Wheel Unlocked Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_TailWheelUnlockPLT.ToString("d"), "Left Console (Pilot)", "Tail Wheel Unlock Indicator", ""));
            AddFunction(Switch.CreateThreeWaySwitch(this, HYDRO_INTERFACE, hydraulic_commands.Rotor_Brake.ToString("d"), "314", "1.0", "Off", "0.0", "Brake", "-1.0", "Lock", "Left Console (Pilot)", "Rotor Brake Switch", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.APU_StartBtn.ToString("d"), "400", "Left Console (Pilot)", "APU Start Button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_APUPLT.ToString("d"), "Left Console (Pilot)", "APU Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.APU_StartBtnCover.ToString("d"), "401", "1.0", "Closed", "0.0", "Open", "Left Console (Pilot)", "APU Start Button Cover", "%0.1f"));

            AddFunction(new Switch(this, ENGINE_INTERFACE, "317", new SwitchPosition[] { new SwitchPosition("1.0", "Start", engine_commands.Eng1StartSw.ToString("d"), engine_commands.Eng1StartSw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "IGN ORIDE", engine_commands.Eng1IgnOrideSw.ToString("d"), engine_commands.Eng1IgnOrideSw.ToString("d"), "0.0", "0.0") }, "Left Console (Pilot)", "No.1 Engine Start Switch", "%0.1f"));
            AddFunction(new Switch(this, ENGINE_INTERFACE, "318", new SwitchPosition[] { new SwitchPosition("1.0", "Start", engine_commands.Eng2StartSw.ToString("d"), engine_commands.Eng2StartSw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "IGN ORIDE", engine_commands.Eng2IgnOrideSw.ToString("d"), engine_commands.Eng2IgnOrideSw.ToString("d"), "0.0", "0.0") }, "Left Console (Pilot)", "No.2 Engine Start Switch", "%0.1f"));
#endregion
#region CP/G
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORE_LO_JETTISON_ARMED.ToString("d"), "368", "Left Console (CP/G)", "L OUTBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmLOutbdCPG.ToString("d"), "Left Console (CP/G)", "Arm Left Outbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORE_LI_JETTISON_ARMED.ToString("d"), "369", "Left Console (CP/G)", "L INBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmLInbdCPG.ToString("d"), "Left Console (CP/G)", "Arm Left Inbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORE_RI_JETTISON_ARMED.ToString("d"), "370", "Left Console (CP/G)", "R INBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmRInbdCPG.ToString("d"), "Left Console (CP/G)", "Arm Right Inbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORE_RO_JETTISON_ARMED.ToString("d"), "371", "Left Console (CP/G)", "R OUTBD Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmROutbdCPG.ToString("d"), "Left Console (CP/G)", "Arm Right Outbd Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORE_JETTISON_LEFT_WINGTIP.ToString("d"), "372", "Left Console (CP/G)", "L TIP Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmLTipCPG.ToString("d"), "Left Console (CP/G)", "Arm Left Tip Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORE_JETTISON_RIGHT_WINGTIP.ToString("d"), "374", "Left Console (CP/G)", "R TIP Station Select Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ArmRTipCPG.ToString("d"), "Left Console (CP/G)", "Arm Right Tip Indicator", ""));
            AddFunction(new PushButton(this, JETT_PANEL_CPG, JETT_commands.STORES_JETT_PUSHBUTTON.ToString("d"), "373", "Left Console (CP/G)", "JETT Pushbutton"));  // Press to jettison stores from all armed stations
            AddFunction(new PushButton(this, HYDRO_INTERFACE, hydraulic_commands.Emergency_HYD_CPG.ToString("d"), "361", "Left Console (CP/G)", "EMERG HYD Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_EmergencyEmergHydCPG.ToString("d"), "Left Console (CP/G)", "Emergency Emerg Hyd Indicator", ""));
            AddFunction(new PushButton(this, HYDRO_INTERFACE, hydraulic_commands.TailWheelUnLock_CPG.ToString("d"), "362", "Left Console (CP/G)", "Tail Wheel Unlocked Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_TailWheelUnlockCPG.ToString("d"), "Left Console (CP/G)", "Tail Wheel Unlock Indicator", ""));

#endregion
#endregion
#region Canopy
#region Pilot
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, cpt_mech_commands.PLT_Door_Lock.ToString("d"), "796", "1.0", "Open", "0.0", "Closed", "Canopy", "Pilot Handle", "%0.1f"));
#endregion
#region CP/G
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, cpt_mech_commands.CPG_Door_Lock.ToString("d"), "799", "1", "Open", "0", "Closed", "Canopy", "CP/G Handle", "%0.1f"));
#endregion
#endregion
#region FIRE DET / EXTG
#region Pilot
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.PLT_Eng1FireBtnCover.ToString("d"), "296", "1.0", "Closed", "0.0", "Open", "Fire Panel (Pilot)", "ENG 1 Button Cover", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.PLT_Eng1FireBtn.ToString("d"), "295", "Fire Panel (Pilot)", "ENG 1 Fire Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_FireEng1PLT.ToString("d"), "Fire Panel (Pilot)", "Eng1 Fire Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_RdyEng1PLT.ToString("d"), "Fire Panel (Pilot)", "Eng1 Ready Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.PLT_ApuFireBtnCover.ToString("d"), "298", "1.0", "Closed", "0.0", "Open", "Fire Panel (Pilot)", "APU Button Cover", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.PLT_ApuFireBtn.ToString("d"), "297", "Fire Panel (Pilot)", "APU Fire Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_FireApuPLT.ToString("d"), "Fire Panel (Pilot)", "APU Fire Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_RdyApuPLT.ToString("d"), "Fire Panel (Pilot)", "APU Ready Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.PLT_Eng2FireBtnCover.ToString("d"), "300", "1.0", "Closed", "0.0", "Open", "Fire Panel (Pilot)", "ENG 2 Button Cover", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.PLT_Eng2FireBtn.ToString("d"), "299", "Fire Panel (Pilot)", "ENG 2 Fire Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_FireEng2PLT.ToString("d"), "Fire Panel (Pilot)", "Eng2 Fire Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_RdyEng2PLT.ToString("d"), "Fire Panel (Pilot)", "Eng2 Ready Indicator", ""));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.PLT_PrimaryDischBtn.ToString("d"), "301", "Fire Panel (Pilot)", "Primary Extinguisher button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_DischPriPLT.ToString("d"), "Fire Panel (Pilot)", "Primary Discharge Indicator", ""));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.PLT_ReserveDischBtn.ToString("d"), "303", "Fire Panel (Pilot)", "Reserve Extinguisher button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_DischResPLT.ToString("d"), "Fire Panel (Pilot)", "Reserve Discharge Indicator", ""));
            AddFunction(new Switch(this, ENGINE_INTERFACE, "302", new SwitchPosition[] { new SwitchPosition("-1.0", "1", engine_commands.PLT_FireDetTestSw1.ToString("d")), new SwitchPosition("0.0", "OFF", engine_commands.PLT_FireDetTestSw1.ToString("d")), new SwitchPosition("1.0", "2", engine_commands.PLT_FireDetTestSw2.ToString("d")) }, "Fire Panel (Pilot)", "Fire Test Switch", "%0.1f"));
#endregion
#region CP/G
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.CPG_Eng1FireBtnCover.ToString("d"), "285", "1.0", "Closed", "0.0", "Open", "Fire Panel (CP/G)", "ENG 1 Button Cover", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.CPG_Eng1FireBtn.ToString("d"), "284", "Fire Panel (CP/G)", "ENG 1 Fire Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_FireEng1CPG.ToString("d"), "Fire Panel (CP/G)", "Eng1 Fire Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_RdyEng1CPG.ToString("d"), "Fire Panel (CP/G)", "Eng1 Ready Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.CPG_ApuFireBtnCover.ToString("d"), "287", "1.0", "Closed", "0.0", "Open", "Fire Panel (CP/G)", "APU Button Cover", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.CPG_ApuFireBtn.ToString("d"), "286", "Fire Panel (CP/G)", "APU Fire Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_FireApuCPG.ToString("d"), "Fire Panel (CP/G)", "APU Fire Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_RdyApuCPG.ToString("d"), "Fire Panel (CP/G)", "APU Ready Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, engine_commands.CPG_Eng2FireBtnCover.ToString("d"), "289", "1.0", "Closed", "0.0", "Open", "Fire Panel (CP/G)", "ENG 2 Button Cover", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.CPG_Eng2FireBtn.ToString("d"), "288", "Fire Panel (CP/G)", "ENG 2 Fire Pushbutton"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_FireEng2CPG.ToString("d"), "Fire Panel (CP/G)", "Eng2 Fire Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_RdyEng2CPG.ToString("d"), "Fire Panel (CP/G)", "Eng2 Ready Indicator", ""));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.CPG_PrimaryDischBtn.ToString("d"), "290", "Fire Panel (CP/G)", "Primary Extinguisher button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_DischPriCPG.ToString("d"), "Fire Panel (CP/G)", "Primary Discharge Indicator", ""));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, engine_commands.CPG_ReserveDischBtn.ToString("d"), "292", "Fire Panel (CP/G)", "Reserve Extinguisher button"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_DischResCPG.ToString("d"), "Fire Panel (CP/G)", "Reserve Discharge Indicator", ""));
            AddFunction(new Switch(this, ENGINE_INTERFACE, "291", new SwitchPosition[] { new SwitchPosition("-1.0", "1", engine_commands.CPG_FireDetTestSw1.ToString("d")), new SwitchPosition("0.0", "OFF", engine_commands.CPG_FireDetTestSw1.ToString("d")), new SwitchPosition("1.0", "2", engine_commands.CPG_FireDetTestSw2.ToString("d")) }, "Fire Panel (CP/G)", "Fire Test Switch", "%0.1f"));
#endregion
#endregion
#region Very essential cockpit elements
#region Pilot
            //elements["pnt_827"] = default_button(CREW.PLT, _('PLT M4 Trigger'), devices.CPT_MECH, cpt_mech_commands.PLT_M4_Trigger, 827)
            //elements["pnt_828"] = default_3_position_tumb(CREW.PLT, _("PLT M4 Safety"), devices.CPT_MECH, cpt_mech_commands.PLT_M4_Safety, 828)
#endregion
#region CP/G
            //elements["pnt_825"] = default_button(CREW.CPG, _('CPG M4 Trigger'), devices.CPT_MECH, cpt_mech_commands.CPG_M4_Trigger, 825)
            //elements["pnt_826"] = default_3_position_tumb(CREW.CPG, _("CPG M4 Safety"), devices.CPT_MECH, cpt_mech_commands.CPG_M4_Safety, 826)
#endregion
#endregion
#region Windshield Panels
#region Pilot
            AddFunction(new PushButton(this, CPT_MECH, cpt_mech_commands.PLT_DefogBtn.ToString("d"), "356", "Windshield Panel (Pilot)", "Defog Button"));
            AddFunction(new Switch(this, CPT_MECH, "357", new SwitchPosition[] { new SwitchPosition("0.0", "PARK", cpt_mech_commands.PLT_WiperSw.ToString("d")), new SwitchPosition("0.1", "OFF", cpt_mech_commands.PLT_WiperSw.ToString("d")), new SwitchPosition("0.2", "LO", cpt_mech_commands.PLT_WiperSw.ToString("d")), new SwitchPosition("0.3", "HI", cpt_mech_commands.PLT_WiperSw.ToString("d")) }, "Windshield Panel (Pilot)", "Wiper Control Switch, PARK/OFF/LO/HI", "%0.1f"));
#endregion
#region CP/G
            AddFunction(new PushButton(this, CPT_MECH, cpt_mech_commands.CPG_DefogBtn.ToString("d"), "394", "Windshield Panel (CP/G)", "Defog Button"));
            AddFunction(new Switch(this, CPT_MECH, "395", new SwitchPosition[] { new SwitchPosition("0.0", "PARK", cpt_mech_commands.CPG_WiperSw.ToString("d")), new SwitchPosition("0.1", "OFF", cpt_mech_commands.CPG_WiperSw.ToString("d")), new SwitchPosition("0.2", "LO", cpt_mech_commands.CPG_WiperSw.ToString("d")), new SwitchPosition("0.3", "HI", cpt_mech_commands.CPG_WiperSw.ToString("d")) }, "Windshield Panel (CP/G)", "Wiper Control Switch, PARK/OFF/LO/HI", "%0.1f"));
#endregion
#endregion
#region Gear System
            AddFunction(Switch.CreateToggleSwitch(this, GEAR_INTERFACE, gear_commands.AH64_ParkingBrake.ToString("d"), "634", "1", "Pull", "0", "Stow", "Gear", "Parking Brake Handle", "%0.1f"));
#endregion
#region Power Lever Quadrant
#region Pilot
            AddFunction(new Switch(this, ELEC_INTERFACE, "315", new SwitchPosition[] { new SwitchPosition("0.0", "Off", electric_commands.MIK.ToString("d")), new SwitchPosition("0.5", "Batt", electric_commands.MIK.ToString("d")), new SwitchPosition("1.0", "External Power", electric_commands.MIK.ToString("d")) }, "Left Console (Pilot)", "Master Ignition Switch", "%0.1f"));
            AddFunction(new Axis(this, CONTROL_INTERFACE, ctrl_commands.FrictionLever.ToString("d"), "633", 0.1d, 0d, 1d, "Left Console (Pilot)", "Lever Friction Adjustment", false, "%0.3f"));
            AddFunction(new Axis(this, ENGINE_INTERFACE, engine_commands.PLT_L_PowerLever.ToString("d"), "398", 0.05d, 0d, 1d, "Left Console (Pilot)", "Left Power Lever", false, "%0.3f"));
            AddFunction(new Axis(this, ENGINE_INTERFACE, engine_commands.PLT_R_PowerLever.ToString("d"), "399", 0.05d, 0d, 1d, "Left Console (Pilot)", "Right Power Lever", false, "%0.3f"));
#endregion
#region CP/G
            AddFunction(new Axis(this, ENGINE_INTERFACE, engine_commands.CPG_L_PowerLever.ToString("d"), "628", 0.05d, 0d, 1d, "Left Console (CP/G)", "Left Power Lever", false, "%0.3f"));
            AddFunction(new Axis(this, ENGINE_INTERFACE, engine_commands.CPG_R_PowerLever.ToString("d"), "629", 0.05d, 0d, 1d, "Left Console (CP/G)", "Right Power Lever", false, "%0.3f"));
#endregion
#endregion
#region Processor Select Panel
#region CP/G
            AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, electric_commands.SP_SELECT_SW.ToString("d"), "397", "-1.0", "SP 1", "0.0", "Auto", "1.0", "SP 2", "Processor Panel", "Select Switch", "%0.1f"));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ProcessorSelectSp1CPG.ToString("d"), "Processor Panel", "Processor Select Sp1 Indicator", ""));
            AddFunction(new FlagValue(this, Warning_Lights.FLAG_ProcessorSelectSp2CPG.ToString("d"), "Processor Panel", "Processor Select Sp2 Indicator", ""));
#endregion
#endregion
#region Generator Reset Panel
#region Pilot
            AddFunction(new Switch(this, ELEC_INTERFACE, "355", new SwitchPosition[] { new SwitchPosition("1.0", "Gen 1", electric_commands.GEN1_RST_SW.ToString("d"), electric_commands.GEN1_RST_SW.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Gen 2", electric_commands.GEN2_RST_SW.ToString("d"), electric_commands.GEN2_RST_SW.ToString("d"), "0.0", "0.0") }, "Generator Reset", "Generator Reset Switch", "%0.1f"));
            AddFunction(new Switch(this, ELEC_INTERFACE, "353", new SwitchPosition[] { new SwitchPosition("1.0", "Eng 2", engine_commands.ChkOvspTestSwENG2A.ToString("d"), engine_commands.ChkOvspTestSwENG2A.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Start", engine_commands.ChkOvspTestSwENG1A.ToString("d"), engine_commands.ChkOvspTestSwENG1A.ToString("d"), "0.0", "0.0") }, "Generator Reset", "CKT A Check Overspeed Test", "%0.1f"));
            AddFunction(new Switch(this, ELEC_INTERFACE, "354", new SwitchPosition[] { new SwitchPosition("1.0", "Eng 2", engine_commands.ChkOvspTestSwENG2B.ToString("d"), engine_commands.ChkOvspTestSwENG2B.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Start", engine_commands.ChkOvspTestSwENG1B.ToString("d"), engine_commands.ChkOvspTestSwENG1B.ToString("d"), "0.0", "0.0") }, "Generator Reset", "CKT B Check Overspeed Test", "%0.1f"));
#endregion
#endregion
#region Handgrip
#region CP/G
#region Left
            //            elements["pnt_491"] = springloaded_3_pos_tumb(CREW.CPG, _("Image AutoTrack/Offset Switch, OFS(LMB)/IAT(RMB)"), devices.TEDAC, tedac_commands.LHG_IAT_OFS_SW_OFS, tedac_commands.LHG_IAT_OFS_SW_IAT, 491)
            //elements["pnt_491"].side = { }
            //            elements["pnt_492-1"] = knuppel_button(CREW.CPG, _("TADS FOV Select Switch, Z (Zoom)"), devices.TEDAC, tedac_commands.LHG_TADS_FOV_SW_Z, 492, 1.0)
            //elements["pnt_492-1"].side = { }
            //            elements["pnt_492-2"] = knuppel_button(CREW.CPG, _("TADS FOV Select Switch, M (Medium)"), devices.TEDAC, tedac_commands.LHG_TADS_FOV_SW_M, 492, -1.0)
            //elements["pnt_492-2"].side = { }
            //            elements["pnt_493-1"] = knuppel_button(CREW.CPG, _("TADS FOV Select Switch, N (Narrow)"), devices.TEDAC, tedac_commands.LHG_TADS_FOV_SW_N, 493, -1.0)
            //elements["pnt_493-1"].side = { }
            //            elements["pnt_493-2"] = knuppel_button(CREW.CPG, _("TADS FOV Select Switch, W (Wide)"), devices.TEDAC, tedac_commands.LHG_TADS_FOV_SW_W, 493, 1.0)
            //elements["pnt_493-2"].side = { }
            //            elements["pnt_494"] = default_3_position_tumb(CREW.CPG, _("TADS Sensor Select Switch, FLIR/TV/DVO"), devices.TEDAC, tedac_commands.LHG_TADS_SENSOR_SELECT_SW, 494, NOT_CYCLED, anim_speed_default, NOT_INVERSED)
            //elements["pnt_494"].side = { }
            //            elements["pnt_495"] = springloaded_3_pos_tumb(CREW.CPG, _("STORE/Update Switch, UPDT(LMB)/STORE(RMB)"), devices.TEDAC, tedac_commands.LHG_STORE_UPDT_SW_UPDT, tedac_commands.LHG_STORE_UPDT_SW_STORE, 495)
            //elements["pnt_495"].side = { }
            //            elements["pnt_500"] = springloaded_3_pos_tumb(CREW.CPG, _("FCR Scan Switch, C (Continuous)(LMB)/S (Single)(RMB)"), devices.TEDAC, tedac_commands.LHG_FCR_SCAN_SW_C, tedac_commands.LHG_FCR_SCAN_SW_S, 500)
            //elements["pnt_500"].side = { }
            //            elements["pnt_501"] = default_button(CREW.CPG, _("CUED Search Button - Press to orient the FCR centerline"), devices.TEDAC, tedac_commands.LHG_CUED_SEARCH_BTN, 501)
            //elements["pnt_501"].side = { }
            //            elements["pnt_496"] = default_button(CREW.CPG, _("Linear Motion Compensation (LMC) Button - Press to toggle LMC mode"), devices.TEDAC, tedac_commands.LHG_LMC_BTN, 496)
            //elements["pnt_496"].side = { }
            //            elements["pnt_498-1"] = knuppel_button(CREW.CPG, _("FCR Mode Switch, GTM (Ground Targeting Mode)"), devices.TEDAC, tedac_commands.LHG_FCR_MODE_SW_UP, 498, 1.0)
            //elements["pnt_498-1"].side = { }
            //            elements["pnt_498-2"] = knuppel_button(CREW.CPG, _("FCR Mode Switch, ATM (Air Targeting Mode)"), devices.TEDAC, tedac_commands.LHG_FCR_MODE_SW_DOWN, 498, -1.0)
            //elements["pnt_498-2"].side = { }
            //            elements["pnt_499-1"] = knuppel_button(CREW.CPG, _("FCR Mode Switch, TPM (Terrain Profile Mode)"), devices.TEDAC, tedac_commands.LHG_FCR_MODE_SW_LEFT, 499, -1.0)
            //elements["pnt_499-1"].side = { }
            //            elements["pnt_499-2"] = knuppel_button(CREW.CPG, _("FCR Mode Switch, RMAP (Radar MAP)"), devices.TEDAC, tedac_commands.LHG_FCR_MODE_SW_RIGHT, 499, 1.0)
            //elements["pnt_499-2"].side = { }
            //            elements["pnt_502-1"] = knuppel_button(CREW.CPG, _("Weapons Action (WAS) Switch, GUN"), devices.TEDAC, tedac_commands.LHG_WEAPONS_ACTION_SW_UP, 502, 1.0)
            //elements["pnt_502-1"].side = { }
            //            elements["pnt_502-2"] = knuppel_button(CREW.CPG, _("Weapons Action (WAS) Switch, ATA"), devices.TEDAC, tedac_commands.LHG_WEAPONS_ACTION_SW_DOWN, 502, -1.0)
            //elements["pnt_502-2"].side = { }
            //            elements["pnt_503-1"] = knuppel_button(CREW.CPG, _("Weapons Action (WAS) Switch, RKT"), devices.TEDAC, tedac_commands.LHG_WEAPONS_ACTION_SW_LEFT, 503, -1.0)
            //elements["pnt_503-1"].side = { }
            //            elements["pnt_503-2"] = knuppel_button(CREW.CPG, _("Weapons Action (WAS) Switch, MSL"), devices.TEDAC, tedac_commands.LHG_WEAPONS_ACTION_SW_RIGHT, 503, 1.0)
            //elements["pnt_503-2"].side = { }
            //            elements["pnt_487-1"] = knuppel_button(CREW.CPG, _("Cursor Controller, Up"), devices.TEDAC, tedac_commands.LHG_CURSOR_UP, 487, 1.0)
            //elements["pnt_487-1"].side = { }
            //            elements["pnt_487-2"] = knuppel_button(CREW.CPG, _("Cursor Controller, Down"), devices.TEDAC, tedac_commands.LHG_CURSOR_DOWN, 487, -1.0)
            //elements["pnt_487-2"].side = { }
            //            elements["pnt_488-1"] = knuppel_button(CREW.CPG, _("Cursor Controller, Left"), devices.TEDAC, tedac_commands.LHG_CURSOR_LEFT, 488, -1.0)
            //elements["pnt_488-1"].side = { }
            //            elements["pnt_488-2"] = knuppel_button(CREW.CPG, _("Cursor Controller, Right"), devices.TEDAC, tedac_commands.LHG_CURSOR_RIGHT, 488, 1.0)
            //elements["pnt_488-2"].side = { }
            //            elements["pnt_489"] = knuppel_button(CREW.CPG, _("Cursor Controller, Enter"), devices.TEDAC, tedac_commands.LHG_CURSOR_ENTER, 489, 1.0)
            //elements["pnt_489"].side = { }
            //            elements["pnt_490"] = default_button(CREW.CPG, _("Cursor Display Select (L/R) Button - Press to move the cursor to the center of the opposite MPD"), devices.TEDAC, tedac_commands.LHG_LR_BTN, 490)
            //elements["pnt_490"].side = { }
#endregion
#region Right
            //elements["pnt_508-1"] = knuppel_button(CREW.CPG, _("Sight Select Switch, HMD"), devices.TEDAC, tedac_commands.RHG_SIGHT_SELECT_SW_UP, 508, 1.0)
            //elements["pnt_508-1"].side = { }
            //            elements["pnt_508-2"] = knuppel_button(CREW.CPG, _("Sight Select Switch, LINK"), devices.TEDAC, tedac_commands.RHG_SIGHT_SELECT_SW_DOWN, 508, -1.0)
            //elements["pnt_508-2"].side = { }
            //            elements["pnt_509-1"] = knuppel_button(CREW.CPG, _("Sight Select Switch, FCR"), devices.TEDAC, tedac_commands.RHG_SIGHT_SELECT_SW_LEFT, 509, -1.0)
            //elements["pnt_509-1"].side = { }
            //            elements["pnt_509-2"] = knuppel_button(CREW.CPG, _("Sight Select Switch, TADS"), devices.TEDAC, tedac_commands.RHG_SIGHT_SELECT_SW_RIGHT, 509, 1.0)
            //elements["pnt_509-2"].side = { }
            //            elements["pnt_510"] = default_3_position_tumb(CREW.CPG, _("Laser Tracker Mode (LT) Switch, A (Automatic)/O (Off)/M (Manual)"), devices.TEDAC, tedac_commands.RHG_LT_SW, 510, NOT_CYCLED, anim_speed_default, NOT_INVERSED)
            //elements["pnt_510"].side = { }
            //            elements["pnt_511-1"] = knuppel_button(CREW.CPG, _("FCR Scan Size Switch, Z (Zoom)"), devices.TEDAC, tedac_commands.RHG_FCR_SCAN_SIZE_SW_UP, 511, 1.0)
            //elements["pnt_511-1"].side = { }
            //            elements["pnt_511-2"] = knuppel_button(CREW.CPG, _("FCR Scan Size Switch, M (Medium)"), devices.TEDAC, tedac_commands.RHG_FCR_SCAN_SIZE_SW_DOWN, 511, -1.0)
            //elements["pnt_511-2"].side = { }
            //            elements["pnt_512-1"] = knuppel_button(CREW.CPG, _("FCR Scan Size Switch, N (Narrow)"), devices.TEDAC, tedac_commands.RHG_FCR_SCAN_SIZE_SW_LEFT, 512, -1.0)
            //elements["pnt_512-1"].side = { }
            //            elements["pnt_512-2"] = knuppel_button(CREW.CPG, _("FCR Scan Size Switch, W (Wide)"), devices.TEDAC, tedac_commands.RHG_FCR_SCAN_SIZE_SW_RIGHT, 512, 1.0)
            //elements["pnt_512-2"].side = { }
            //            elements["pnt_513"] = default_button(CREW.CPG, _("C-Scope Button"), devices.TEDAC, tedac_commands.RHG_C_SCOPE_SW, 513)
            //elements["pnt_513"].side = { }
            //            elements["pnt_504"] = default_button(CREW.CPG, _("FLIR Polarity Button - Press to change polarity"), devices.TEDAC, tedac_commands.RHG_FLIR_PLRT_BTN, 504)
            //elements["pnt_504"].side = { }
            //            elements["pnt_514"] = default_button(CREW.CPG, _("Sight Slave Button - Press to slave TADS or FCR to the selected acquisition source"), devices.TEDAC, tedac_commands.RHG_SIGHT_SLAVE_BTN, 514)
            //elements["pnt_514"].side = { }
            //            elements["pnt_517"] = default_button(CREW.CPG, _("Display Zoom Button - Press to view FCR targeting information on the NTS target"), devices.TEDAC, tedac_commands.RHG_DISPLAY_ZOOM_BTN, 517)
            //elements["pnt_517"].side = { }
            //            elements["pnt_519"] = springloaded_3_pos_tumb(CREW.CPG, _("Spare Switch, PREVIOUS(LMB)/NEXT(RMB)"), devices.TEDAC, tedac_commands.RHG_SPARE_SW_AFT, tedac_commands.RHG_SPARE_SW_FWD, 519)
            //elements["pnt_519"].side = { }
            //            elements["pnt_505"] = default_button(CREW.CPG, _("HDD/HOD Select Button - currently not used"), devices.TEDAC, tedac_commands.RHG_HDD_SW, 505)
            //elements["pnt_505"].side = { }
            //            elements["pnt_518"] = default_button(CREW.CPG, _("Cursor Enter Button - Press to enter"), devices.TEDAC, tedac_commands.RHG_ENTER_BTN, 518)
            //elements["pnt_518"].side = { }
            //            elements["pnt_515-1"] = knuppel_button(CREW.CPG, _("Sight Manual Tracker (MAN TRK) Controller, Up"), devices.TEDAC, tedac_commands.RHG_MAN_TRK_UP, 515, 1.0)
            //elements["pnt_515-1"].side = { }
            //            elements["pnt_515-2"] = knuppel_button(CREW.CPG, _("Sight Manual Tracker (MAN TRK) Controller, Down"), devices.TEDAC, tedac_commands.RHG_MAN_TRK_DOWN, 515, -1.0)
            //elements["pnt_515-2"].side = { }
            //            elements["pnt_516-1"] = knuppel_button(CREW.CPG, _("Sight Manual Tracker (MAN TRK) Controller, Left"), devices.TEDAC, tedac_commands.RHG_MAN_TRK_LEFT, 516, -1.0)
            //elements["pnt_516-1"].side = { }
            //            elements["pnt_516-2"] = knuppel_button(CREW.CPG, _("Sight Manual Tracker (MAN TRK) Controller, Right"), devices.TEDAC, tedac_commands.RHG_MAN_TRK_RIGHT, 516, 1.0)
            //elements["pnt_516-2"].side = { }
            //            elements["pnt_507"] = default_3_position_tumb(CREW.CPG, _("Image Auto Tracker (IAT) Polarity Switch, W (White)/A (Auto)/B (Black)"), devices.TEDAC, tedac_commands.RHG_IAT_POLARITY_SW, 507, NOT_CYCLED, anim_speed_default, NOT_INVERSED)
            //elements["pnt_507"].side = { }
#endregion
#endregion
#endregion
#region Comm Panel
#region Pilot
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.VHF_volume.ToString("d"), "334", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "VHF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.VHF_disable.ToString("d"), "449", "Communications Panel (Pilot)", "VHF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.UHF_volume.ToString("d"), "335", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "UHF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.UHF_disable.ToString("d"), "450", "Communications Panel (Pilot)", "UHF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.FM1_volume.ToString("d"), "336", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "FM1 Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.FM1_disable.ToString("d"), "451", "Communications Panel (Pilot)", "FM1 Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.FM2_volume.ToString("d"), "337", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "FM2 Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.FM2_disable.ToString("d"), "452", "Communications Panel (Pilot)", "FM2 Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.HF_volume.ToString("d"), "338", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "HF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.HF_disable.ToString("d"), "453", "Communications Panel (Pilot)", "HF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.IFF_volume.ToString("d"), "348", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "IFF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.IFF_disable.ToString("d"), "454", "Communications Panel (Pilot)", "IFF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.RLWR_volume.ToString("d"), "349", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "RLWR Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.RLWR_disable.ToString("d"), "455", "Communications Panel (Pilot)", "RLWR Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.ATA_volume.ToString("d"), "350", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "ATA Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.ATA_disable.ToString("d"), "456", "Communications Panel (Pilot)", "ATA Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.VCR_volume.ToString("d"), "351", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "VCR Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.VCR_disable.ToString("d"), "457", "Communications Panel (Pilot)", "VCR Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.ADF_volume.ToString("d"), "352", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "ADF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.ADF_disable.ToString("d"), "458", "Communications Panel (Pilot)", "ADF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.MASTER_volume.ToString("d"), "344", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "MASTER Volume Control Knob"));
            AddFunction(new Axis(this, COMM_PANEL_PLT, comm_commands.SensControl.ToString("d"), "345", 0.1d, 0d, 1d, "Communications Panel (Pilot)", "SENS Control Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_PLT, comm_commands.VHF_SQL.ToString("d"), "339", "1.0", "On", "0.0", "Off", "Communications Panel (Pilot)", "VHF Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_PLT, comm_commands.UHF_SQL.ToString("d"), "340", "1.0", "On", "0.0", "Off", "Communications Panel (Pilot)", "UHF Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_PLT, comm_commands.FM1_SQL.ToString("d"), "341", "1.0", "On", "0.0", "Off", "Communications Panel (Pilot)", "FM1 Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_PLT, comm_commands.FM2_SQL.ToString("d"), "342", "1.0", "On", "0.0", "Off", "Communications Panel (Pilot)", "FM2 Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_PLT, comm_commands.HF_SQL.ToString("d"), "343", "1.0", "On", "0.0", "Off", "Communications Panel (Pilot)", "HF Squelch Switch", "%0.1f"));
            AddFunction(new Switch(this, COMM_PANEL_PLT, "346", new SwitchPosition[] { new SwitchPosition("1.0", "Hot Mic", comm_commands.ICS_MODE.ToString("d")), new SwitchPosition("0.5", "Vox", comm_commands.ICS_MODE.ToString("d")), new SwitchPosition("0.0", "PTT", comm_commands.ICS_MODE.ToString("d")) }, "Communications Panel (Pilot)", "ICS Mode Switch", "%0.1f"));
            AddFunction(new PushButton(this, COMM_PANEL_PLT, comm_commands.IDENT.ToString("d"), "347", "Communications Panel (Pilot)", "IDENT Button"));

#endregion
#region CP/G
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.VHF_volume.ToString("d"), "375", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "VHF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.VHF_disable.ToString("d"), "459", "Communications Panel (CP/G)", "VHF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.UHF_volume.ToString("d"), "376", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "UHF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.UHF_disable.ToString("d"), "460", "Communications Panel (CP/G)", "UHF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.FM1_volume.ToString("d"), "377", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "FM1 Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.FM1_disable.ToString("d"), "461", "Communications Panel (CP/G)", "FM1 Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.FM2_volume.ToString("d"), "378", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "FM2 Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.FM2_disable.ToString("d"), "462", "Communications Panel (CP/G)", "FM2 Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.HF_volume.ToString("d"), "379", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "HF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.HF_disable.ToString("d"), "463", "Communications Panel (CP/G)", "HF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.IFF_volume.ToString("d"), "389", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "IFF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.IFF_disable.ToString("d"), "464", "Communications Panel (CP/G)", "IFF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.RLWR_volume.ToString("d"), "390", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "RLWR Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.RLWR_disable.ToString("d"), "465", "Communications Panel (CP/G)", "RLWR Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.ATA_volume.ToString("d"), "391", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "ATA Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.ATA_disable.ToString("d"), "466", "Communications Panel (CP/G)", "ATA Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.VCR_volume.ToString("d"), "392", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "VCR Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.VCR_disable.ToString("d"), "467", "Communications Panel (CP/G)", "VCR Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.ADF_volume.ToString("d"), "393", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "ADF Volume Control Knob"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.ADF_disable.ToString("d"), "468", "Communications Panel (CP/G)", "ADF Volume Control Knob Pull"));  //  (LMB) Pull to disable / (MW) Rotate to adjust volume
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.MASTER_volume.ToString("d"), "385", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "MASTER Volume Control Knob"));
            AddFunction(new Axis(this, COMM_PANEL_CPG, comm_commands.SensControl.ToString("d"), "386", 0.1d, 0d, 1d, "Communications Panel (CP/G)", "SENS Control Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_CPG, comm_commands.VHF_SQL.ToString("d"), "380", "1.0", "On", "0.0", "Off", "Communications Panel (CP/G)", "VHF Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_CPG, comm_commands.UHF_SQL.ToString("d"), "381", "1.0", "On", "0.0", "Off", "Communications Panel (CP/G)", "UHF Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_CPG, comm_commands.FM1_SQL.ToString("d"), "382", "1.0", "On", "0.0", "Off", "Communications Panel (CP/G)", "FM1 Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_CPG, comm_commands.FM2_SQL.ToString("d"), "383", "1.0", "On", "0.0", "Off", "Communications Panel (CP/G)", "FM2 Squelch Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, COMM_PANEL_CPG, comm_commands.HF_SQL.ToString("d"), "384", "1.0", "On", "0.0", "Off", "Communications Panel (CP/G)", "HF Squelch Switch", "%0.1f"));
            AddFunction(new Switch(this, COMM_PANEL_CPG, "387", new SwitchPosition[] { new SwitchPosition("1.0", "Hot Mic", comm_commands.ICS_MODE.ToString("d")), new SwitchPosition("0.5", "Vox", comm_commands.ICS_MODE.ToString("d")), new SwitchPosition("0.0", "PTT", comm_commands.ICS_MODE.ToString("d")) }, "Communications Panel (CP/G)", "ICS Mode Switch", "%0.1f"));
            AddFunction(new PushButton(this, COMM_PANEL_CPG, comm_commands.IDENT.ToString("d"), "388", "Communications Panel (CP/G)", "IDENT Button"));

#endregion
#endregion
#region CMWS
#region Pilot
            AddFunction(new Switch(this, CMWS, "610", new SwitchPosition[] { new SwitchPosition("-1.0", "Off", CMWS_commands.CMWS_PWR.ToString("d")), new SwitchPosition("0.0", "On", CMWS_commands.CMWS_PWR.ToString("d")), new SwitchPosition("1.0", "Test", CMWS_commands.CMWS_PWR_TEST.ToString("d")) }, "CMWS", "Power Switch", "%0.1f"));
            AddFunction(new Axis(this, CMWS, CMWS_commands.CMWS_AUDIO_KNOB.ToString("d"), "611", 0.1d, 0d, 1d, "CMWS", "Audio Volume Knob"));
            AddFunction(new Axis(this, CMWS, CMWS_commands.CMWS_LAMP_KNOB.ToString("d"), "612", 0.1d, 0d, 1d, "CMWS", "Brightness Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, CMWS, CMWS_commands.CMWS_ARM_SAFE_SW.ToString("d"), "614", "1.0", "Arm", "0.0", "Safe", "CMWS", "Arm Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CMWS, CMWS_commands.CMWS_CMWS_NAV_SW.ToString("d"), "615", "1.0", "CMWS", "0.0", "Nav", "CMWS", "Mode Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CMWS, CMWS_commands.CMWS_BYPASS_AUTO_SW.ToString("d"), "616", "1.0", "Bypass", "0.0", "Auto", "CMWS", "Operation Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CMWS, CMWS_commands.CMWS_JETT_COVER.ToString("d"), "617", "1.0", "Open", "0.0", "Closed", "CMWS", "Jettison Switch Cover", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CMWS, CMWS_commands.CMWS_JETT_SW.ToString("d"), "618", "1.0", "Arm", "0.0", "Safe", "CMWS", "Flare Jettison Switch", "%.1f"));

            AddFunction(new NetworkValue(this, "2084", "CMWS", "Ready Flag", "Displayed when CMWS is ready.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2085", "CMWS", "Dispensing Flag", "Displayed when CMWS is dispensing countermeasures.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2086", "CMWS", "Threat Direction Front Left", "Displayed when there is a threat front and left.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2087", "CMWS", "Threat Direction Front Right", "Displayed when there is a threat front and right.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2088", "CMWS", "Threat Direction Aft Left", "Displayed when there is a threat rear and left.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2089", "CMWS", "Threat Direction Aft Right", "Displayed when there is a threat rear and right.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2090", "CMWS", "Threat Zone Front", "Displayed when both front detectors are active.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2091", "CMWS", "Threat Zone Left", "Displayed when both left detectors are active.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2092", "CMWS", "Threat Zone Aft", "Displayed when both rear detectors are active.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2093", "CMWS", "Threat Zone Right", "Displayed when both right detectors are active.", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new Text(this, "2082", "CMWS", "Line 1", "Display Line 1"));
            AddFunction(new Text(this, "2083", "CMWS", "Line 2", "Display Line 2"));

#endregion
#endregion
#region Standby Instruments
#region Altimeter
            AddFunction(new Functions.Altimeter(this));
            AddFunction(new Axis(this, BARO_ALTIMETER, baro_alt_commands.PressureSet.ToString("d"), "477", 0.002d, 0d, 1d, "Standby Altimeter", "Pressure Setting Knob", true, "%.4f"));
#endregion
#region Standby Airspeed Indicator
            double[] IASoutput = { 0.0d, 20.0d, 30.0d, 40.0d, 50.0d, 60.0d, 70.0d, 80.0d, 90.0d, 100.0d, 110.0d, 120.0d, 130.0d, 140.0d, 150.0d, 200.0d, 230.0d, 240.0d, 250.0d };
            double[] IASinput = { 0.0d, 0.018d, 0.05d, 0.09d, 0.142d, 0.2d, 0.27d, 0.345d, 0.394d, 0.432d, 0.465d, 0.505d, 0.543d, 0.581d, 0.623d, 0.789d, 0.896d, 0.934d, 0.967d };
            CalibrationPointCollectionDouble airspeedScale = new CalibrationPointCollectionDouble(IASinput[0], IASoutput[0], IASinput[1], IASoutput[1]);
            for (int ii = 2; ii < 19; ii++) airspeedScale.Add(new CalibrationPointDouble(IASinput[ii], IASoutput[ii]));
            AddFunction(new ScaledNetworkValue(this, "469", airspeedScale, "Standby IAS", "IAS Airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.Knots, "%.4f"));
#endregion
#region Free Air Temperature Guage
            double[] FAToutput = { -70.0d, -60.0d, -50.0d, -40.0d, -30.0d, 50.0d };
            double[] FATinput = { 0.0d, 0.0674d, 0.1405d, 0.22d, 0.305d, 1.0d };
            CalibrationPointCollectionDouble FATScale = new CalibrationPointCollectionDouble(FATinput[0], FAToutput[0], FATinput[1], FAToutput[1]);
            for (int ii = 2; ii < 6; ii++) FATScale.Add(new CalibrationPointDouble(FATinput[ii], FAToutput[ii]));
            AddFunction(new ScaledNetworkValue(this, "636", FATScale, "Temperature Gauge", "Free Air Temperature", "Current Free Air Temperature.", "", BindingValueUnits.Celsius, "%.4f"));
#endregion
#region  Standby Attitude Indicator
            AddFunction(new ScaledNetworkValue(this, "622", 90d, "Standby Attitude Indicator", "Pitch", "Current pitch displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "623", -180d, "Standby Attitude Indicator", "Bank", "Current bank displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new NetworkValue(this, "625", "Standby Attitude Indicator", "Arrow Pointer", "Unclear what this is", "(-0.85 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "626", "Standby Attitude Indicator", "Slip Ball", "Current position of the slip ball relative to the center of the tube.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "627", "Standby Attitude Indicator", "Turn Marker", "Position of the turn marker.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new Axis(this, SAI, sai_commands.CageKnobRotate.ToString("d"), "619", 0.05d, 0.0d, 1.00d, "Standby Attitude Indicator", "Pitch Adjustment Knob", true, "%.3f"));
            AddFunction(new PushButton(this, SAI, sai_commands.CageKnobPull.ToString("d"), "620", "Standby Attitude Indicator", "Cage Pull"));
            AddFunction(new FlagValue(this, "624", "Standby Attitude Indicator", "Warning Flag", "Displayed when SAI is caged or non-functional."));
#endregion

#endregion


        }
    }
}
