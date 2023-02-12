
namespace GadrocsWorkshop.Helios.Interfaces.DCS.F16C
{
    internal enum devices
    {
        FM_PROXY = 1,                   //1
        CONTROL_INTERFACE,      //2
        ELEC_INTERFACE,         //3
        FUEL_INTERFACE,         //4
        HYDRO_INTERFACE,            //5
        ENGINE_INTERFACE,           //6
        GEAR_INTERFACE,         //7
        OXYGEN_INTERFACE,           //8
        HEARING_SENS,               //9
        CPT_MECH,                   //10
        EXTLIGHTS_SYSTEM,           //11
        CPTLIGHTS_SYSTEM,           //12
        ECS_INTERFACE,          //13
        INS,                        //14
        RALT,                       //15
// HOTAS Interface
        HOTAS,                  //16
        UFC,                        //17		// Upfront Controls (UFC) with Integrated Control Panel (ICP)
// Computers ////////////////////////////
        MUX,                        //18		// Multiplex manager, holds channels and manages remote terminals addition/remove
        MMC,                        //19		// Modular Mission Computer (MMC) / Fire Control Computer (FCC)
        CADC,                       //20		// Central Air Data Computer
        FLCC,                       //21		// Flight Control Computer
        SMS,                        //22		// Stores Management Subsystem
// Displays ////////////////////////////-
        HUD,                        //23
        MFD_LEFT,                   //24		// Multifunction Display
        MFD_RIGHT,              //25		// Multifunction Display
        DED,                        //26		// Data Entry Display (DED)
        PFLD,                       //27		// Pilot Fault List Display (PFLD)
        EHSI,                       //28		// Electronic Horizontal Situation Indicator (EHSI)
// Helmet
        HELMET,                 //29
        HMCS,                       //30		// HMCS Interface
// Sensors //////////////////////////////
        FCR,                        //31		// AN/APG-68
// EWS //////////////////////////////////
        CMDS,                       //32		// Counter Measures Dispensing System
        RWR,                        //33		// Radar Warning Receiver (RWR)
// Radio ////////////////////////////////
        IFF,                        //34		// AN/APX-113
        IFF_CONTROL_PANEL,      //35
        UHF_RADIO,              //36		// AN/ARC-164
        UHF_CONTROL_PANEL,      //37
        VHF_RADIO,              //38		// AN/ARC-222
        INTERCOM,                   //39
        MIDS_RT,                    //40
        MIDS,                       //41
        KY58,                       //42		// KY-58 Secure Speech System
        ILS,                        //43
// Instruments //////////////////////////
        AOA_INDICATOR,          //44
        AAU34,                  //45		// Altimeter AAU-34/A
        AMI,                        //46		// Airspeed/Mach Indicator
        SAI,                        //47
        VVI,                        //48		// Vertical Velocity Indicator
        STANDBY_COMPASS,            //49
        ADI,                        //50		// Attitude Director Indicator
        CLOCK,                  //51
        MACROS,                 //52
        AIHelper,                   //53
        KNEEBOARD,              //54
        ARCADE,                 //55
        TACAN_CTRL_PANEL,           //56
// Armament
        SIDEWINDER_INTERFACE,       //57
        TGP_INTERFACE,          //58
        GPS,                        //59
        IDM,                        //60
        MAP,                        //61
// Armament
        MAV_INTERFACE,          //62
        HARM_INTERFACE,         //63
        HTS_INTERFACE,          //64
        DTE,                        //65
// ECM //////////////////////////////////
        ECM_INTERFACE			//66		// Electronic Countermeasures interface
    }
}
