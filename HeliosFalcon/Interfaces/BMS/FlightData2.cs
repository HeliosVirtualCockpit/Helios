//  Copyright 2014 Craig Courtney
//  Copyright 2023 Helios Contributors
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

using System;
using System.Runtime.InteropServices;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.BMS
{
    enum TacanSources : int
    {
        UFC = 0,
        AUX = 1,
        NUMBER_OF_SOURCES = 2
    }

    [Flags]
    enum TacanBits : byte
    {
        band = 0x01,   // true in this bit position if band is X
        mode = 0x02    // true in this bit position if domain is air to air
    }

    [Flags]
    public enum AltBits : uint
    {
        CalType = 0x01, // true if calibration in inches of Mercury (Hg), false if in hectoPascal (hPa)
        PneuFlag = 0x02	// true if PNEU flag is visible
    }

    [Flags]
    public enum PowerBits : uint
    {
        BusPowerBattery = 0x01, // true if at least the battery bus is powered
        BusPowerEmergency = 0x02,   // true if at least the emergency bus is powered
        BusPowerEssential = 0x04,   // true if at least the essential bus is powered
        BusPowerNonEssential = 0x08,    // true if at least the non-essential bus is powered
        MainGenerator = 0x10,   // true if the main generator is online
        StandbyGenerator = 0x20,    // true if the standby generator is online
        JetFuelStarter = 0x40	// true if JFS is running, can be used for magswitch
    }

    [Flags]
    public enum BlinkBits : uint
    {
        // currently working
        OuterMarker = 0x01, // defined in HsiBits    - slow flashing for outer marker
        MiddleMarker = 0x02,    // defined in HsiBits    - fast flashing for middle marker
        PROBEHEAT = 0x04,   // defined in LightBits2 - probeheat system is tested
        AuxSrch = 0x08, // defined in LightBits2 - search function in NOT activated and a search radar is painting ownship
        Launch = 0x10,  // defined in LightBits2 - missile is fired at ownship
        PriMode = 0x20, // defined in LightBits2 - priority mode is enabled but more than 5 threat emitters are detected
        Unk = 0x40, // defined in LightBits2 - unknown is not active but EWS detects unknown radar

        // not working yet, defined for future use
        Elec_Fault = 0x80,  // defined in LightBits3 - non-resetting fault
        OXY_BROW = 0x100,   // defined in LightBits  - monitor fault during Obogs
        EPUOn = 0x200,  // defined in LightBits3 - abnormal EPU operation

        // working
        JFSOn_Slow = 0x400, // defined in LightBits3 - slow blinking: non-critical failure
        JFSOn_Fast = 0x800,	// defined in LightBits3 - fast blinking: critical failure

        // VERSION 19
        ECM_Oper = 0x1000  // defined in EcmOperStates - system warming up
    }

    public enum CmdsModes : int
    {
        CmdsOFF = 0,
        CmdsSTBY = 1,
        CmdsMAN = 2,
        CmdsSEMI = 3,
        CmdsAUTO = 4,
        CmdsBYP = 5
    }

    public enum NavModes : byte
    {
        ILS_TACAN = 0,
        TACAN = 1,
        NAV = 2,
        ILS_NAV = 3
    }

    public enum FlyStates : int
    {
        IN_UI = 0, // UI      - in the UI
        LOADING = 1, // UI>3D   - loading the sim data
        WAITING = 2, // UI>3D   - waiting for other players
        FLYING = 3, // 3D      - flying
        DEAD = 4, // 3D>Dead - dead, waiting to respawn
        UNKNOWN = 5 // ???
    }

    // RTT area indices
    public enum RTT_areas : byte
    {
        RTT_HUD = 0,
        RTT_PFL,
        RTT_DED,
        RTT_RWR,
        RTT_MFDLEFT,
        RTT_MFDRIGHT,
        RTT_HMS,
        RTT_noOfAreas
    };

    // instrument backlight brightness
    enum InstrLight : byte
    {
        INSTR_LIGHT_OFF = 0,
        INSTR_LIGHT_DIM = 1,
        INSTR_LIGHT_BRT = 2
    };

    // flood console brightness
    enum FloodConsole : byte
    {
        FLOOD_CONSOLE_OFF = 0,
        FLOOD_CONSOLE_1   = 1,
        FLOOD_CONSOLE_2   = 2,
        FLOOD_CONSOLE_3   = 3,
        FLOOD_CONSOLE_4   = 4,
        FLOOD_CONSOLE_5   = 5,
        FLOOD_CONSOLE_6   = 6
    };

    [Flags]
    enum MiscBits : uint
	{
        RALT_Valid = 0x01, // indicates weather the RALT reading is valid/reliable
        Flcs_Flcc_A = 0x02,
        Flcs_Flcc_B = 0x04,
        Flcs_Flcc_C = 0x08,
        Flcs_Flcc_D = 0x10,
        SolenoidStatus = 0x20, // 0 not powered or failed or WOW, 1 is working OK

        AllLampBitsFlccOn = 0x1e // Not bit by itself! This is the check mask for ALL the Flcs bits
    };

    // ECM indicator states
    [Flags]
    enum EcmBits : uint  // Note: these are currently not combinable bits, but mutually exclusive states
    {
        ECM_UNPRESSED_NO_LIT  = 0x01,
        ECM_UNPRESSED_ALL_LIT = 0x02,
        ECM_PRESSED_NO_LIT    = 0x04,
        ECM_PRESSED_STANDBY   = 0x08,
        ECM_PRESSED_ACTIVE    = 0x10,
        ECM_PRESSED_TRANSMIT  = 0x20,
        ECM_PRESSED_FAIL      = 0x40,
        ECM_PRESSED_ALL_LIT   = 0x80
    };

    // IDIAS operating states
    [Flags]
    enum EcmOperStates : byte  // Note: these are currently not combinable bits, but mutually exclusive states
    {
        ECM_OPER_NO_LIT = 0,
        ECM_OPER_STDBY = 1,
        ECM_OPER_ACTIVE = 2,
        ECM_OPER_ALL_LIT = 3
    };

    // RWR jamming states
    enum JammingStates : byte
    {
        JAMMED_NO     = 0,
        JAMMED_YES    = 1,
        JAMMED_SHOULD = 2
    };

[Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    struct FlightData2
    {
        // changelog:
        // 1: initial BMS 4.33 version
        // 2: added AltCalReading, altBits, BupUhfPreset, powerBits, blinkBits, cmdsMode
        // 3: added VersionNum, hydPressureA/B, cabinAlt, BupUhfFreq, currentTime, vehicleACD
        // 4: added fuelflow2
        // 5: added RwrInfo, lefPos, tefPos
        // 6: added vtolPos
        // 7: bit fields are now unsigned instead of signed
        // 8: increased RwrInfo size to 512
        // 9: added human pilot names and their status in a session
        // 10: added bump intensity while taxiing/rolling
        // 11: added latitude/longitude
        // 12: added RTT info
        // 13: added IFF panel backup digits
        // 14: added instrument backlight brightness
        // 15: added MiscBits, BettyBits, radar altitude, bingo fuel, cara alow, bullseye, BMS version information, string area size/time
        // 16: added turn rate
        // 17: added Flcs_Flcc, SolenoidStatus to MiscBits
        // 18. added console floodlight brightness
        // 19: added ECM_M1-5, ECM oper + blinkbit, magnetic deviation, RWR jamming status


        public const int RWRINFO_SIZE = 512;
        public const int MAX_CALLSIGNS = 32;
        public const int CALLSIGN_LEN = 12;
        public const int MAX_ECM_PROGRAMS = 5;
        public const int MAX_RWR_OBJECTS = 40;


        // VERSION 1
        public float nozzlePos2;   // Ownship engine nozzle2 percent open (0-100)
        public float rpm2;         // Ownship engine rpm2 (Percent 0-103)
        public float ftit2;        // Ownship Forward Turbine Inlet Temp2 (Degrees C)
        public float oilPressure2; // Ownship Oil Pressure2 (Percent 0-100)
        public NavModes navMode;  // current mode selected for HSI/eHSI (added in BMS4)
        public float aauz; // Ownship barometric altitude given by AAU (depends on calibration)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)TacanSources.NUMBER_OF_SOURCES)]
        public TacanBits[] tacanInfo;      // Tacan band/mode settings for UFC and AUX COMM

        //VERSION 2/7
        public int AltCalReading;  // barometric altitude calibration (depends on CalType)
        public AltBits altBits;       // various altimeter bits, see AltBits enum for details
        public PowerBits powerBits;     // Ownship power bus / generator states, see PowerBits enum for details
        public BlinkBits blinkBits;     // Cockpit indicator lights blink status, see BlinkBits enum for details
        // NOTE: these bits indicate only *if* a lamp is blinking, in addition to the
        // existing on/off bits. It's up to the external program to implement the
        // *actual* blinking.
        public CmdsModes cmdsMode;		// Ownship CMDS mode state, see CmdsModes enum for details
        public int BupUhfPreset;	// BUP UHF channel preset

        // VERSION 3
        public int BupUhfFreq;		// BUP UHF channel frequency
        public float cabinAlt;		// Ownship cabin altitude
        public float hydPressureA;	// Ownship Hydraulic Pressure A
        public float hydPressureB;	// Ownship Hydraulic Pressure B
        public int currentTime;	// Current time in seconds (max 60 * 60 * 24)
        public short vehicleACD;	// Ownship ACD index number, i.e. which aircraft type are we flying.
        public int VersionNum;		// Version of FlightData2 mem area

        // VERSION 4
        public float fuelFlow2;    // Ownship fuel flow2 (Lbs/Hour)

        // VERSION 5 / 8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RWRINFO_SIZE)]
        public byte[] RwrInfo;     //[512] New RWR Info
        public float lefPos;       // Ownship LEF position
        public float tefPos;       // Ownship TEF position

        // VERSION 6
        public float vtolPos;      // Ownship VTOL exhaust angle

        // VERSION 9
        public byte pilotsOnline;                  // Number of pilots in an MP session

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CALLSIGNS * CALLSIGN_LEN)]
        public byte[] pilotsCallsign;        // [MAX_CALLSIGNS][CALLSIGN_LEN] List of pilots callsign connected to an MP session


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CALLSIGNS)]
        public byte[] pilotsStatus;                // [MAX_CALLSIGNS] Status of the MP pilots, see enum FlyStates


        //VERSION 10
        public float bumpIntensity; // Intensity of a "bump" while taxiing/rolling, 0..1

        //VERSION 11
        public float latitude;      // Ownship latitude in degrees (as known by avionics)
        public float longitude;     // Ownship longitude in degrees (as known by avionics)

        //VERSION 12
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort[] RTT_size;                 // RTT overall width and height
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)RTT_areas.RTT_noOfAreas * 4)]
        public ushort[] RTT_area;  // For each area: left/top/right/bottom

        // VERSION 13
        public byte iffBackupMode1Digit1;                     // IFF panel backup Mode1 digit 1
        public byte iffBackupMode1Digit2;                     // IFF panel backup Mode1 digit 2
        public byte iffBackupMode3ADigit1;                    // IFF panel backup Mode3A digit 1
        public byte iffBackupMode3ADigit2;                    // IFF panel backup Mode3A digit 2

        // VERSION 14
        public InstrLight instrLight;       // (unsigned char) current instrument backlight brightness setting, see InstrLight enum for details

        // VERSION 15
        public uint bettyBits;              // see BettyBits enum for details
        public MiscBits miscBits;           // see MiscBits enum for details
        public float RALT;                  // radar altitude (only valid/ reliable if MiscBit "RALT_Valid" is set)
        public float bingoFuel;             // bingo fuel level
        public float caraAlow;              // cara alow setting
        public float bullseyeX;             // bullseye X in sim coordinates (same as ownship, i.e. North (Ft))
        public float bullseyeY;             // bullseye Y in sim coordinates (same as ownship, i.e. East (Ft))
        public int BMSVersionMajor;         // E.g.  4.
        public int BMSVersionMinor;         //         34.
        public int BMSVersionMicro;         //            1
        public int BMSBuildNumber;          //              build 20050
        public uint StringAreaSize;         // the overall size of the StringData/FalconSharedMemoryAreaString shared memory area
        public uint StringAreaTime;         // last time the StringData/FalconSharedMemoryAreaString shared memory area has been changed - you only need to re-read the string shared mem if this changes
        public uint DrawingAreaSize;        // the overall size of the DrawingData/FalconSharedMemoryAreaDrawing area

        // VERSION 16
        public float turnRate;                     // actual turn rate (no delay or dampening) in degrees/second

        // VERSION 18
        public FloodConsole floodConsole;   // (unsigned char) current floodconsole brightness setting, see FloodConsole enum for details

        // VERSION 19
        public float magDeviationSystem;    // current mag deviation of the system
        public float magDeviationReal;      // current mag deviation of the system

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ECM_PROGRAMS)]
        public EcmBits[] ecmBits;           // see EcmBits enum for details - Note: these are currently not combinable bits, but mutually exclusive states!

        public EcmOperStates ecmOperState;  // (unsigned char) see enum EcmOperStates for details

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_RWR_OBJECTS)]
        public JammingStates[] RWRjammingStatus;  // (unsigned) char see enum JammingStates for details
    }
}
