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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.UH60L
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Functions;
    using static System.Net.Mime.MediaTypeNames;
    using System.Security.Policy;
    using static GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Functions.Altimeter;

    //using GadrocsWorkshop.Helios.Controls;

    /* enabling this attribute will cause Helios to discover this new interface and make it available for use    */
    [HeliosInterface(
        "Helios.UH60L",                        // Helios internal type ID used in Profile XML, must never change
        "DCS UH-60L Blackhawk",                // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey="Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class UH60LInterface : DCSInterface
    {
        private enum devices
        {
            PLT_ICP = 1,
            ARC201_FM1,
            ARC164,
            INTERCOM,
            UHF_RADIO,
            FM1_RADIO,
            ARC186,
            VHF_RADIO,
            ARC201_FM2,
            FM2_RADIO,
            BASERADIO,
            HF_RADIO,
            ADF_RADIO,
            VORILS_RADIO,
            ELECTRIC_SYSTEM,
            ECQ,
            AFCS,
            AHRU,
            APR39,
            EXTLIGHTS,
            HELMET_DEVICE,
            AVIONICS,
            ASN128B,
            CAUTION_ADVISORY_PANEL,
            VIDS,
            KNEEBOARD,
            EFM_HELPER,
            SOUNDSYSTEM,
            DEBUG,
            AVS7,
            CREWE,
            M130,
            SYNC_CONTROLLER,
            POSITION,
            PLTLC6,
            CPLTLC6,
            PLTCISP,
            CPLTCISP,
            CISP,
            ARN149,
            ARN147,
            PLTAAU32A,
            CPLTAAU32A,
            PLTAPN209,
            CPLTAPN209,
            MISC,
            AFMS,
            MACROS
        }

        private enum keys
        {
            iCommandEnginesStart = 309,
            BattSwitch = 10001,
            ExtPwrSwitch = 10002,
            ThrottleCutoff = 10003,
            ThrottleIncrease = 10004,
            ThrottleDecrease = 10005,
            ThrottleStop = 10006,
            LandingLight = 10007,
            TriggerFireOn = 10008,
            TriggerFireOff = 10009,
            PickleOn = 10010,
            PickleOff = 10011,
            MasterArm = 10012,
            e1PCL = 10013,
            e2PCL = 10014,
            bothPCLs = 10015,
            slewStabUp = 10016,
            slewStabDown = 10017,
            showControlInd = 10018,
            toggleDoors = 10019,
            toggleDoorsClose = 10020,
            toggleDoorsOpen = 10021,
            landingLightToggle = 10022,
            landingLightExtend = 10023,
            landingLightRetract = 10024,
            searchLightToggle = 10025,
            searchLightExtend = 10026,
            searchLightRetract = 10027,
            searchLightLeft = 10028,
            searchLightRight = 10029,
            searchLightBrighten = 10030,
            searchLightDim = 10031,
            debugMoveElementUp = 10032,
            debugMoveElementDn = 10033,
            debugMoveElementLeft = 10034,
            debugMoveElementRight = 10035,
            debugMoveElementForward = 10036,
            debugMoveElementBack = 10037,
            ptt = 10038,
            pilotICPXmitSelectorInc = 10039,
            pilotICPXmitSelectorDec = 10040,
            avs7Toggle = 10041,
            avs7Brighten = 10042,
            avs7Dim = 10043,
            dispenseChaffDown = 10044,
            dispenseChaffUp = 10045,
            dispenseFlareDown = 10046,
            dispenseFlareUp = 10047,
            toggleCopilotDoor = 10048,
            toggleLeftCargoDoor = 10049,
            toggleRightCargoDoor = 10050,
            toggleLeftGunnerDoor = 10051,
            toggleRightGunnerDoor = 10052,
            toggleProbe = 10053,
            radioPTT = 10054,
            cycleposLightIntensity = 10055,
            cyclecabinLightMode = 10056,
            cyclecockpitLightMode = 10057,
            navLightModeCycle = 10058,
            posLightModeCycle = 10059,
            antiLightGrpCycle = 10060,
            antiLightModeCycle = 10061,
            magCompassLightsCycle = 10062,
            arc164_presetInc = 10063,
            arc164_presetDec = 10064,
            arc164_freq_XoooooInc = 10065,
            arc164_freq_XoooooDec = 10066,
            arc164_freq_oXooooInc = 10067,
            arc164_freq_oXooooDec = 10068,
            arc164_freq_ooXoooInc = 10069,
            arc164_freq_ooXoooDec = 10070,
            arc164_freq_oooXooInc = 10071,
            arc164_freq_oooXooDec = 10072,
            arc164_freq_ooooXXInc = 10073,
            arc164_freq_ooooXXDec = 10074,
            arc164_modeInc = 10075,
            arc164_modeDec = 10076,
            arc164_xmitmodeInc = 10077,
            arc164_xmitmodeDec = 10078,
            arc164_modeCycle = 10079,
            arc164_xmitmodeCycle = 10080,
            apn209PilotLoSetInc = 10081,
            apn209PilotLoSetDec = 10082,
            apn209PilotHiSetInc = 10083,
            apn209PilotHiSetDec = 10084,
            apn209CopilotLoSetInc = 10085,
            apn209CopilotLoSetDec = 10086,
            apn209CopilotHiSetInc = 10087,
            apn209CopilotHiSetDec = 10088,
            SelectDisplayInc = 10089,
            SelectDisplayDec = 10090,
            SelectModeInc = 10091,
            SelectModeDec = 10092,
            arn147MHzInc = 10093,
            arn147MHzDec = 10094,
            arn147KHzInc = 10095,
            arn147KHzDec = 10096,
            arn147PowerCycle = 10097,
            fm1PresetSelectorInc = 10098,
            fm1PresetSelectorDec = 10099,
            fm1PresetSelectorCycle = 10100,
            fm1FunctionSelectorInc = 10101,
            fm1FunctionSelectorDec = 10102,
            fm1FunctionSelectorCycle = 10103,
            fm2PresetSelectorInc = 10104,
            fm2PresetSelectorDec = 10105,
            fm2PresetSelectorCycle = 10106,
            fm2FunctionSelectorInc = 10107,
            fm2FunctionSelectorDec = 10108,
            fm2FunctionSelectorCycle = 10109,
            fm1PwrSelectorInc = 10110,
            fm1PwrSelectorDec = 10111,
            fm1PwrSelectorCycle = 10112,
            fm1ModeSelectorInc = 10113,
            fm1ModeSelectorDec = 10114,
            fm1ModeSelectorCycle = 10115,
            fm2PwrSelectorInc = 10116,
            fm2PwrSelectorDec = 10117,
            fm2PwrSelectorCycle = 10118,
            fm2ModeSelectorInc = 10119,
            fm2ModeSelectorDec = 10120,
            fm2ModeSelectorCycle = 10121,
            pilotBarometricScaleSetInc = 10122,
            pilotBarometricScaleSetDec = 10123,
            copilotBarometricScaleSetInc = 10124,
            copilotBarometricScaleSetDec = 10125,
            arc186Selector10MHzInc = 10126,
            arc186Selector1MHzInc = 10127,
            arc186Selector100KHzInc = 10128,
            arc186Selector25KHzInc = 10129,
            arc186FreqSelectorInc = 10130,
            arc186PresetSelectorInc = 10131,
            arc186ModeSelectorInc = 10132,
            arc186Selector10MHzDec = 10133,
            arc186Selector1MHzDec = 10134,
            arc186Selector100KHzDec = 10135,
            arc186Selector25KHzDec = 10136,
            arc186FreqSelectorDec = 10137,
            arc186PresetSelectorDec = 10138,
            arc186ModeSelectorDec = 10139,
            BattSwitchOn = 10140,
            BattSwitchOff = 10141,
            gen1SwitchOn = 10142,
            gen1SwitchOff = 10143,
            gen1SwitchTest = 10144,
            gen2SwitchOn = 10145,
            gen2SwitchOff = 10146,
            gen2SwitchTest = 10147,
            extPwrSwitchOn = 10148,
            extPwrSwitchOff = 10149,
            extPwrSwitchReset = 10150,
            apuGenSwitchOn = 10151,
            apuGenSwitchOff = 10152,
            apuGenSwitchTest = 10153,
            switchFuelPumpPrime = 10154,
            switchFuelPumpOff = 10155,
            switchFuelPumpApuBoost = 10156,
            switchAirSourceApu = 10157,
            switchAirSourceOff = 10158,
            switchAirSourceEngine = 10159,
            switchAPUOn = 10160,
            switchAPUOff = 10161,
            glareshieldLightsInc = 10162,
            glareshieldLightsDec = 10163,
            cpltInstrLightsInc = 10164,
            cpltInstrLightsDec = 10165,
            lightedSwitchesInc = 10166,
            lightedSwitchesDec = 10167,
            upperConsoleLightsInc = 10168,
            upperConsoleLightsDec = 10169,
            lowerConsoleLightsInc = 10170,
            lowerConsoleLightsDec = 10171,
            pltInstrLightsInc = 10172,
            pltInstrLightsDec = 10173,
            nonFltInstrLightsInc = 10174,
            nonFltInstrLightsDec = 10175,
            formationLights_AXIS = 10176,
            formationLightsInc = 10177,
            formationLightsDec = 10178,
            wiperSelectorInc = 10179,
            wiperSelectorDec = 10180,
            wiperSelectorCycle = 10181,
            pltRdrAltLights_AXIS = 10182,
            cpltRdrAltLights_AXIS = 10183,
            apn209PilotLoSet_AXIS = 10184,
            apn209PilotHiSet_AXIS = 10185,
            apn209CopilotLoSet_AXIS = 10186,
            apn209CopilotHiSet_AXIS = 10187,
            pilotBarometricScaleSet_AXIS = 10188,
            copilotBarometricScaleSet_AXIS = 10189,
            lowerConsoleLights_AXIS = 10190,
            glareshieldLights_AXIS = 10191,
            cpltInstrLights_AXIS = 10192,
            lightedSwitches_AXIS = 10193,
            upperConsoleLights_AXIS = 10194,
            pltInstrLights_AXIS = 10195,
            nonFltInstrLights_AXIS = 10196,
            engFSSBoth = 10197,
            setEngControlBoth = 10198,
            eng1FSS_AXIS = 10199,
            eng2FSS_AXIS = 10200,
            engFSSBoth_AXIS = 10201,
            PilotCISHdgCycle = 10202,
            PilotCISNavCycle = 10203,
            PilotCISAltCycle = 10204,
            PilotNavGPSCycle = 10205,
            PilotNavVORILSCycle = 10206,
            PilotNavBACKCRSCycle = 10207,
            PilotNavFMHOMECycle = 10208,
            PilotTURNRATECycle = 10209,
            PilotCRSHDGCycle = 10210,
            PilotVERTGYROCycle = 10211,
            PilotBRG2Cycle = 10212,
            CopilotNavGPSCycle = 10213,
            CopilotNavVORILSCycle = 10214,
            CopilotNavBACKCRSCycle = 10215,
            CopilotNavFMHOMECycle = 10216,
            CopilotTURNRATECycle = 10217,
            CopilotCRSHDGCycle = 10218,
            CopilotVERTGYROCycle = 10219,
            CopilotBRG2Cycle = 10220,
            arn149PresetCycle = 10221,
            arn149ToneTestCycle = 10222,
            arn149VolumeCycle = 10223,
            arn149PowerCycle = 10224,
            arn149thousandsCycle = 10225,
            arn149hundredsCycle = 10226,
            arn149tensCycle = 10227,
            arn149onesCycle = 10228,
            arn149tenthsCycle = 10229,
            afcsStabAutoToggle = 10230,
            afcsFPSToggle = 10231,
            afcsBoostToggle = 10232,
            afcsSAS1Toggle = 10233,
            afcsSAS2Toggle = 10234,
            afcsTrimToggle = 10235,
            miscTailWheelLockToggle = 10236,
            apr39PowerCycle = 10237,
            apr39BrightnessIncDec = 10238,
            apr39VolumeIncDec = 10239,
            apr39Volume_AXIS = 10240,
            apr39Brightness_AXIS = 10241,
            afmcpXferModeCycle = 10242,
            afmcpManXferCycle = 10243,
            afmcpXferFromCycle = 10244,
            afmcpPressCycle = 10245,
            afmcpPress_AXIS = 10246,
            afmcpPressInc = 10247,
            afmcpPressDec = 10248,
            cmProgramDial_AXIS = 10249,
            cmProgramDialInc = 10250,
            cmProgramDialDec = 10251,
            cmProgramDialCycle = 10252
        };

        private enum device_commands
        {
            AuxPowerSw = 3201,
            FuelShutoffSw = 3202,
            FuelPumpSw = 3203,
            eng1ControlDetent = 3204,
            eng2ControlDetent = 3205,
            setEng1Control = 3206,
            setEng2Control = 3207,
            eng1FSS = 3208,
            eng2FSS = 3209,
            eng1Starter = 3210,
            eng2Starter = 3211,

            // AHRU
            ahruMode = 3212,
            ahruFunc = 3213,
            ahruUp = 3214,
            ahruRight = 3215,
            ahruEnter = 3216,
            RWRpower = 3217,
            RWRBrightness = 3218,
            CAPLampTest = 3219,
            CAPLampBrightness = 3220,
            CAPMasterCautionReset = 3221,

            // AFCS
            afcsStabAuto = 3222,
            afcsSAS1 = 3223,
            afcsSAS2 = 3224,
            afcsTrim = 3225,
            afcsFPS = 3226,
            afcsBoost = 3227,
            slewStabUp = 3228,
            slewStabDown = 3229,

            // VIDS
            cduLampTest = 3230,
            pilotPDUTest = 3231,
            copilotPDUTest = 3232,

            //AAU32A
            pilotBarometricScaleSet = 3233,
            copilotBarometricScaleSet = 3234,

            //ASN128B
            SelectMode = 3235,
            SelectDisplay = 3236,
            SelectBtnKybd = 3237,
            SelectBtnLtrLeft = 3238,
            SelectBtnLtrMid = 3239,
            SelectBtnLtrRight = 3240,
            SelectBtnF1 = 3241,
            SelectBtn1 = 3242,
            SelectBtn2 = 3243,
            SelectBtn3 = 3244,
            SelectBtnTgtStr = 3245,
            SelectBtn4 = 3246,
            SelectBtn5 = 3247,
            SelectBtn6 = 3248,
            SelectBtnInc = 3249,
            SelectBtn7 = 3250,
            SelectBtn8 = 3251,
            SelectBtn9 = 3252,
            SelectBtnDec = 3253,
            SelectBtnClr = 3254,
            SelectBtn0 = 3255,
            SelectBtnEnt = 3256,

            //AVS7
            setAVS7Power = 3257,
            incAVS7Brightness = 3258,
            decAVS7Brightness = 3259,

            // Generic
            foo = 3260,

            //Radio
            arc164_mode = 3261,
            arc164_xmitmode = 3262,
            arc164_volume = 3263,
            arc164_squelch = 3264,
            arc164_freq_preset = 3265,
            arc164_freq_Xooooo = 3266,
            arc164_freq_oXoooo = 3267,
            arc164_freq_ooXooo = 3268,
            arc164_freq_oooXoo = 3269,
            arc164_freq_ooooXX = 3270,
            arc164_preset = 3271,

            //Lighting Panel
            glareshieldLights = 3272,
            cpltInstrLights = 3273,
            lightedSwitches = 3274,
            formationLights = 3275,
            upperConsoleLights = 3276,
            lowerConsoleLights = 3277,
            pltInstrLights = 3278,
            nonFltInstrLights = 3279,
            magCompassLights = 3280,
            posLightIntensity = 3281,
            posLightMode = 3282,
            antiLightGrp = 3283,
            antiLightMode = 3284,
            navLightMode = 3285,
            cabinLightMode = 3286,
            cockpitLightMode = 3287,
            wiperSelector = 3288,
            pltRdrAltLights = 3289,
            cpltRdrAltLights = 3290,

            // APR-39
            apr39Power = 3291,
            apr39SelfTest = 3292,
            apr39Altitude = 3293,
            apr39Volume = 3294,
            apr39Brightness = 3295,

            // LC6
            resetSetBtn = 3296,
            modeBtn = 3297,
            startStopAdvBtn = 3298,

            // Pilot ICP
            pilotICPXmitSelector = 3299,
            pilotICPSetVolume = 3300,
            pilotICPToggleFM1 = 3301,
            pilotICPToggleUHF = 3302,
            pilotICPToggleVHF = 3303,
            pilotICPToggleFM2 = 3304,
            pilotICPToggleHF = 3305,
            pilotICPToggleVOR = 3306,
            pilotICPToggleADF = 3307,

            // CM System
            cmFlareDispenseModeCover = 3308,
            cmFlareCounterDial = 3309,
            cmChaffCounterDial = 3310,
            cmArmSwitch = 3311,
            cmProgramDial = 3312,
            cmChaffDispense = 3313,
            cmFlareDispense = 3314,

            // ARC201 FM1
            fm1PresetSelector = 3315,
            fm1FunctionSelector = 3316,
            fm1PwrSelector = 3317,
            fm1ModeSelector = 3318,
            fm1Volume = 3319,
            fm1Btn1 = 3320,
            fm1Btn2 = 3321,
            fm1Btn3 = 3322,
            fm1Btn4 = 3323,
            fm1Btn5 = 3324,
            fm1Btn6 = 3325,
            fm1Btn7 = 3326,
            fm1Btn8 = 3327,
            fm1Btn9 = 3328,
            fm1Btn0 = 3329,
            fm1BtnClr = 3330,
            fm1BtnEnt = 3331,
            fm1BtnFreq = 3332,
            fm1BtnErfOfst = 3333,
            fm1BtnTime = 3334,

            // ARC201 FM2
            fm2PresetSelector = 3335,
            fm2FunctionSelector = 3336,
            fm2PwrSelector = 3337,
            fm2ModeSelector = 3338,
            fm2Volume = 3339,
            fm2Btn1 = 3340,
            fm2Btn2 = 3341,
            fm2Btn3 = 3342,
            fm2Btn4 = 3343,
            fm2Btn5 = 3344,
            fm2Btn6 = 3345,
            fm2Btn7 = 3346,
            fm2Btn8 = 3347,
            fm2Btn9 = 3348,
            fm2Btn0 = 3349,
            fm2BtnClr = 3350,
            fm2BtnEnt = 3351,
            fm2BtnFreq = 3352,
            fm2BtnErfOfst = 3353,
            fm2BtnTime = 3354,

            // ARC186
            arc186Volume = 3355,
            arc186Tone = 3356,
            arc186Selector10MHz = 3357,
            arc186Selector1MHz = 3358,
            arc186Selector100KHz = 3359,
            arc186Selector25KHz = 3360,
            arc186FreqSelector = 3361,
            arc186Load = 3362,
            arc186PresetSelector = 3363,
            arc186ModeSelector = 3364,

            // CISP
            pilotHSIHdgSet = 3365,
            pilotHSICrsSet = 3366,
            copilotHSIHdgSet = 3367,
            copilotHSICrsSet = 3368,
            PilotCISHdgToggle = 3369,
            PilotCISNavToggle = 3370,
            PilotCISAltToggle = 3371,
            PilotNavGPSToggle = 3372,
            PilotNavVORILSToggle = 3373,
            PilotNavBACKCRSToggle = 3374,
            PilotNavFMHOMEToggle = 3375,
            PilotTURNRATEToggle = 3376,
            PilotCRSHDGToggle = 3377,
            PilotVERTGYROToggle = 3378,
            PilotBRG2Toggle = 3379,
            CopilotNavGPSToggle = 3380,
            CopilotNavVORILSToggle = 3381,
            CopilotNavBACKCRSToggle = 3382,
            CopilotNavFMHOMEToggle = 3383,
            CopilotTURNRATEToggle = 3384,
            CopilotCRSHDGToggle = 3385,
            CopilotVERTGYROToggle = 3386,
            CopilotBRG2Toggle = 3387,

            // DEBUG
            visualisationToggle = 3388,

            // AN/ARN-149
            arn149Preset = 3389,
            arn149ToneTest = 3390,
            arn149Volume = 3391,
            arn149Power = 3392,
            arn149thousands = 3393,
            arn149hundreds = 3394,
            arn149tens = 3395,
            arn149ones = 3396,
            arn149tenths = 3397,

            // AN/ARN-147
            arn147MHz = 3398,
            arn147KHz = 3399,
            arn147Power = 3400,

            // MISC
            fuelProbe = 3401,
            parkingBrake = 3402,
            miscTailWheelLock = 3403,
            doorCplt = 3404,
            doorPlt = 3405,
            doorLGnr = 3406,
            doorRGnr = 3407,
            doorLCargo = 3408,
            doorRCargo = 3409,

            // MISC PANEL
            miscFuelIndTest = 3410,
            miscTailWheelLockPanel = 3411,   // this was a duplicate name so changed from miscTailWheelLock
            miscGyroEffect = 3412,
            miscTailServo = 3413,

            // APN209
            apn209PilotLoSet = 3414,
            apn209PilotHiSet = 3415,
            apn209CopilotLoSet = 3416,
            apn209CopilotHiSet = 3417,
 
            // AFMS
            afmcpXferMode = 3418,
            afmcpManXfer = 3419,
            afmcpXferFrom = 3420,
            afmcpPress = 3421
        }

        private enum EFM_commands
        {
            batterySwitch = 3013,
            extPwrSwitch = 3014,
            apuGenSwitch = 3015,
            gen1Switch = 3016,
            gen2Switch = 3017,
            apuGenSwitch2 = 3018,
            gen1Switch2 = 3019,
            gen2Switch2 = 3020,
            extPwrSwitch2 = 3021,
            switchFuelPump = 3022,
            switchAirSource = 3023,
            switchAPU = 3024,
            setEng1Control = 3028,
            setEng2Control = 3029,
            eng1FSS = 3030,
            eng2FSS = 3031,
            eng1Starter = 3034,
            eng2Starter = 3035,
            stabManualSlew = 3036,
            stabPwrReset = 3043,
            fuelPumpL = 3044,
            fuelPumpR = 3045,
            setRefuelProbeState = 3046,
            slewStabUp = 3050,
            slewStabDown = 3051,
            lockTailWheel = 3052,
            afmcpXferMode = 3053,
            afmcpManXfer = 3054,
            afmcpXferFrom = 3055,
            trimUp = 4017,
            trimDown = 4018,
            trimLeft = 4019,
            trimRight = 4020,
            trimRelease = 4025,
            trimSet = 4026,
            trimReset = 4027,
            intercomPTT = 4029,
            nonFltLighting = 4040,
            setPilotHSIHeading = 5000,
            wheelbrake = 3100,
            wheelbrakeToggle = 3101,
            wheelbrakeLeft = 3102,
            wheelbrakeRight = 3103,
            setParkingBrake = 3104,
            collectiveIncrease = 3105,
            collectiveDecrease = 3106,
            dampenValue = 5001,
            startServer = 5016,
            connectServer = 5017
        }

        private enum mainpanel
        {
            // PILOT BARO ALTIMETER
            pilotBaroAlt100s = 60,
            pilotBaroAlt1000s = 61,             // correction pilotBaroAlt100s = 61,
            pilotBaroAlt10000s = 62,            // correction pilotBaroAlt100s = 62,
            pilotPressureScale1 = 64,           // correction pilotBaroAlt100s = 64,
            pilotPressureScale2 = 65,           // correction pilotBaroAlt100s = 65,
            pilotPressureScale3 = 66,           // correction pilotBaroAlt100s = 66,
            pilotPressureScale4 = 67,           // correction pilotBaroAlt100s = 67,
            pilotBaroAltEncoderFlag = 68,       // correction pilotBaroAlt100s = 68,


            // COPILOT ALTIMETER
            copilotBaroAlt100s = 70,
            copilotBaroAlt1000s = 71,
            copilotBaroAlt10000s = 72,
            copilotPressureScale1 = 74,
            copilotPressureScale2 = 75,
            copilotPressureScale3 = 76,
            copilotPressureScale4 = 77,
            copilotBaroAltEncoderFlag = 78,

            // MISC
            parkingBrakeHandle = 81,
            pilotPTT = 82,

            // AHRU
            ahruIndSlave = 95,
            ahruIndDG = 96,
            ahruIndCcal = 97,
            ahruIndFail = 98,
            ahruIndAln = 99,

            // CIS MODE LIGHTS
            cisHdgOnLight = 212,
            cisNavOnLight = 213,
            cisAltOnLight = 214,
            pltCisDplrLight = 215,
            pltCisVorLight = 216,
            pltCisILSLight = 217,
            pltCisBackCrsLight = 218,
            pltCisFMHomeLight = 219,
            pltCisTRNorm = 220,
            pltCisTRAlt = 221,
            pltCisCrsHdgPlt = 222,
            pltCisCrsHdgCplt = 223,
            pltCisGyroNorm = 224,
            pltCisGyroAlt = 225,
            pltCisBrg2ADF = 226,
            pltCisVBrg2VOR = 227,
            cpltCisDplrLight = 228,
            cpltCisVorLight = 229,
            cpltCisILSLight = 230,
            cpltCisBackCrsLight = 231,
            cpltCisFMHomeLight = 232,
            cpltCisTRNorm = 233,
            cpltCisTRAlt = 234,
            cpltCisCrsHdgPlt = 235,
            cpltCisCrsHdgCplt = 236,
            cpltCisGyroNorm = 237,
            cpltCisGyroAlt = 238,
            cpltCisBrg2ADF = 239,
            cpltCisVBrg2VOR = 240,

            // AFCS LIGHTS
            afcBoostLight = 241,
            afcSAS1Light = 242,
            afcSAS2Light = 243,
            afcTrimLight = 244,
            afcFPSLight = 245,
            afcStabLight = 246,

            // COCKPIT DOME LTS
            domeLightBlue = 275,
            domeLightWhite = 276,

            // MISC PANEL LIGHTS
            //miscFuelIndTestLight = 246,
            miscTailWheelLockLight = 294,
            //miscGyroEffectLight = 246,

            // CAP & MCP LAMPS
            capBrightness = 309,

            mcpEng1Out = 310,
            mcpEng2Out = 311,
            mcpFire = 312,
            mcpMasterCaution = 313,
            mcpLowRPM = 314,
            capFuel1Low = 315,
            capGen1 = 316,
            capGen2 = 317,
            capFuel2Low = 318,
            capFuelPress1 = 319,
            capGenBrg1 = 320,
            capGenBrg2 = 321,
            capFuelPress2 = 322,
            capEngOilPress1 = 323,
            capConv1 = 324,
            capConv2 = 325,
            capEngOilPress2 = 326,
            capEngOilTemp1 = 327,
            capAcEssBus = 328,
            capDcEssBus = 329,
            capEngOilTemp2 = 330,
            capEng1Chip = 331,
            capBattLow = 332,
            capBattFault = 333,
            capEng2Chip = 334,
            capFuelFltr1 = 335,
            capGustLock = 336,
            capPitchBias = 337,
            capFuelFltr2 = 338,
            capEng1Starter = 339,
            capOilFltr1 = 340,
            capOilFltr2 = 341,
            capEng2Starter = 342,
            capPriServo1 = 343,
            capHydPump1 = 344,
            capHydPump2 = 345,
            capPriServo2 = 346,
            capTailRtrQuad = 347,
            capIrcmInop = 348,
            capAuxFuel = 349,
            capTailRtrServo1 = 350,
            capMainXmsnOilTemp = 351,
            capIntXmsnOilTemp = 352,
            capTailXmsnOilTemp = 353,
            capApuOilTemp = 354,
            capBoostServo = 355,
            capStab = 356,
            capSAS = 357,
            capTrim = 358,
            capLftPitot = 359,
            capFPS = 360,
            capIFF = 361,
            capRtPitot = 362,
            capLftChipInput = 363,
            capChipIntXmsn = 364,
            capChipTailXmsn = 365,
            capRtChipInput = 366,
            capLftChipAccess = 367,
            capChipMainSump = 368,
            capApuFail = 369,
            capRtChipAccess = 370,
            capMrDeIceFault = 371,
            capMrDeIceFail = 372,
            capTRDeIceFail = 373,
            capIce = 374,
            capXmsnOilPress = 375,
            capRsvr1Low = 376,
            capRsvr2Low = 377,
            capBackupRsvrLow = 378,
            capEng1AntiIce = 379,
            capEng1InletAntiIce = 380,
            capEng2InletAntiIce = 381,
            capEng2AntiIce = 382,
            capApuOn = 383,
            capApuGen = 384,
            capBoostPumpOn = 385,
            capBackUpPumpOn = 386,
            capApuAccum = 387,
            capSearchLt = 388,
            capLdgLt = 389,
            capTailRtrServo2 = 390,
            capHookOpen = 391,
            capHookArmed = 392,
            capGPS = 393,
            capParkingBrake = 394,
            capExtPwr = 395,
            capBlank = 396,

            pduPltOverspeed1 = 450,
            pduPltOverspeed2 = 451,
            pduPltOverspeed3 = 452,
            pduCpltOverspeed1 = 453,  // correction - was pduCpltOverspeed2?
            pduCpltOverspeed2 = 454,  // correction - was pduCpltOverspeed3?
            pduCpltOverspeed3 = 455,

            // M130 CM System
            cmFlareCounterTens = 554,
            cmFlareCounterOnes = 555,
            cmChaffCounterTens = 556,
            cmChaffCounterOnes = 557,
            cmArmedLight = 558,

            pltDoorGlass = 1201,
            cpltDoorGlass = 1202,
            lGnrDoorGlass = 1203,
            rGnrDoorGlass = 1204,
            lCargoDoorGlass = 1205,
            rCargoDoorGlass = 1206,

        }

        public UH60LInterface(string name)
            : base(name, "UH-60L", "pack://application:,,,/Helios;component/Interfaces/DCS/UH60L/ExportFunctions.lua")
        {
            // optionally support more than just the base aircraft, or even use a module
            // name that is not a vehicle, by removing it from this list
            //
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

            //-- Electric system
            AddFunction(Switch.CreateToggleSwitch(this, devices.EFM_HELPER.ToString("d"), EFM_commands.batterySwitch.ToString("d"), "17", "1.0", "On", "0.0", "Off", "EFM", "Battery Switch, ON/OFF", "%.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "18", new SwitchPosition[] { new SwitchPosition("-1.0", "Off", EFM_commands.extPwrSwitch.ToString("d")), new SwitchPosition("0.0", "On", EFM_commands.extPwrSwitch.ToString("d")), new SwitchPosition("1.0", "Reset", EFM_commands.extPwrSwitch2.ToString("d"), null, null, "0.0") }, "EFM", "External Power Switch, ON/OFF/RESET", "%.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "19", new SwitchPosition[] { new SwitchPosition("-1.0", "Off", EFM_commands.apuGenSwitch.ToString("d")), new SwitchPosition("0.0", "On", EFM_commands.apuGenSwitch.ToString("d")), new SwitchPosition("1.0", "Test", EFM_commands.apuGenSwitch2.ToString("d"), null, null, "0.0") }, "EFM", "APU GEN Switch, ON/OFF/TEST", "%.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "20", new SwitchPosition[] { new SwitchPosition("-1.0", "Off", EFM_commands.gen1Switch.ToString("d")), new SwitchPosition("0.0", "On", EFM_commands.gen1Switch.ToString("d")), new SwitchPosition("1.0", "Test", EFM_commands.gen1Switch2.ToString("d"), null, null, "0.0") }, "EFM", "GEN 1 Switch, ON/OFF/TEST", "%.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "21", new SwitchPosition[] { new SwitchPosition("-1.0", "Off", EFM_commands.gen2Switch.ToString("d")), new SwitchPosition("0.0", "On", EFM_commands.gen2Switch.ToString("d")), new SwitchPosition("1.0", "Test", EFM_commands.gen2Switch2.ToString("d"), null, null, "0.0") }, "EFM", "GEN 2 Switch, ON/OFF/TEST", "%.1f"));

            //-- Fuel and Engines
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "22", new SwitchPosition[] { new SwitchPosition("-1.0", "Fuel Prime", EFM_commands.switchFuelPump.ToString("d")), new SwitchPosition("0.0", "Off", EFM_commands.switchFuelPump.ToString("d")), new SwitchPosition("1.0", "APU Boost", EFM_commands.switchFuelPump.ToString("d"), null, null, "0.0") }, "EFM", "Fuel Pump Switch, FUEL PRIME/OFF/APU BOOST", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "23", new SwitchPosition[] { new SwitchPosition("-1.0", "APU", EFM_commands.switchAirSource.ToString("d")), new SwitchPosition("0.0", "On", EFM_commands.switchAirSource.ToString("d")), new SwitchPosition("1.0", "Eng", EFM_commands.switchAirSource.ToString("d"), null, null, "0.0") }, "EFM", "Air Source Switch, APU/OFF/ENG", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EFM_HELPER.ToString("d"), EFM_commands.switchAPU.ToString("d"), "24", "1.0", "On", "0.0", "Off", "EFM", "APU CONTROL, ON/OFF", "%.1f"));

            //-- PNT-025 APU EXTINGUISH
            AddFunction(new Axis(this, devices.ECQ.ToString("d"), device_commands.setEng1Control.ToString("d"), "26", 0.1d, 0d, 1d, "Engine Control", "Engine 1 Control"));
            AddFunction(new Axis(this, devices.ECQ.ToString("d"), device_commands.setEng2Control.ToString("d"), "27", 0.1d, 0d, 1d, "Engine Control", "Engine 2 Control"));

            AddFunction(new Switch(this, devices.ECQ.ToString("d"), "28", new SwitchPosition[] { new SwitchPosition("0.0", "Off", device_commands.eng1FSS.ToString("d")), new SwitchPosition("0.5", "Direct", device_commands.eng1FSS.ToString("d")), new SwitchPosition("1.0", "Cross Feed", device_commands.eng1FSS.ToString("d"), null, null, "0.0") }, "Engine Control", "Engine 1 FSS, OFF/DIR/XFD", "%0.1f"));
            AddFunction(new Switch(this, devices.ECQ.ToString("d"), "29", new SwitchPosition[] { new SwitchPosition("0.0", "Off", device_commands.eng2FSS.ToString("d")), new SwitchPosition("0.5", "Direct", device_commands.eng2FSS.ToString("d")), new SwitchPosition("1.0", "Cross Feed", device_commands.eng2FSS.ToString("d"), null, null, "0.0") }, "Engine Control", "Engine 2 FSS, OFF/DIR/XFD", "%0.1f"));

            AddFunction(new PushButton(this, devices.ECQ.ToString("d"), device_commands.eng1Starter.ToString("d"), "30", "Engine Control", "Engine 1 Starter"));
            AddFunction(new PushButton(this, devices.ECQ.ToString("d"), device_commands.eng2Starter.ToString("d"), "31", "Engine Control", "Engine 2 Starter"));
            //
            //-- STAB PANEL
            AddFunction(new Switch(this, devices.ECQ.ToString("d"), "32", new SwitchPosition[] { new SwitchPosition("0.0", "Up", device_commands.slewStabUp.ToString("d")), new SwitchPosition("0.5", "Off", device_commands.slewStabUp.ToString("d")), new SwitchPosition("1.0", "Down", device_commands.slewStabDown.ToString("d"), null, null, "0.0") }, "Stabilator Control", "Stabilator Manual Slew UP/DOWN", "%0.1f"));
            AddFunction(new PushButton(this, devices.AFCS.ToString("d"), device_commands.afcsStabAuto.ToString("d"), "33", "Stabilator Control", "Stabilator Auto ON/OFF"));
            AddFunction(new PushButton(this, devices.AFCS.ToString("d"), device_commands.afcsSAS1.ToString("d"), "34", "Stabilator Control", "SAS 1 ON/OFF"));
            AddFunction(new PushButton(this, devices.AFCS.ToString("d"), device_commands.afcsSAS2.ToString("d"), "35", "Stabilator Control", "SAS 2 ON/OFF"));
            AddFunction(new PushButton(this, devices.AFCS.ToString("d"), device_commands.afcsTrim.ToString("d"), "36", "Stabilator Control", "Trim ON/OFF"));
            AddFunction(new PushButton(this, devices.AFCS.ToString("d"), device_commands.afcsFPS.ToString("d"), "37", "Stabilator Control", "FPS ON/OFF"));
            AddFunction(new PushButton(this, devices.AFCS.ToString("d"), device_commands.afcsBoost.ToString("d"), "38", "Stabilator Control", "SAS Boost ON/OFF"));
            AddFunction(new PushButton(this, devices.EFM_HELPER.ToString("d"), EFM_commands.stabPwrReset.ToString("d"), "39", "Stabilator Control", "SAS Power On Reset"));

            //
            //--FUEL PUMPS
            AddFunction(Switch.CreateToggleSwitch(this, devices.EFM_HELPER.ToString("d"), EFM_commands.fuelPumpL.ToString("d"), "40", "1.0", "On", "0.0", "Off", "EFM", "No. 1 Fuel Boost Pump ON/OFF", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EFM_HELPER.ToString("d"), EFM_commands.fuelPumpR.ToString("d"), "41", "1.0", "On", "0.0", "Off", "EFM", "No. 2 Fuel Boost Pump ON/OFF", "%.1f"));

            //-- Engine Control Locks
            AddFunction(Switch.CreateToggleSwitch(this, devices.ECQ.ToString("d"), device_commands.eng1ControlDetent.ToString("d"), "42", "-1.0", "Off", "0.0", "Idle", "Engine Control", "Engine 1 Control Level OFF/IDLE", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.ECQ.ToString("d"), device_commands.eng2ControlDetent.ToString("d"), "43", "-1.0", "Off", "0.0", "Idle", "Engine Control", "Engine 2 Control Level OFF/IDLE", "%.1f"));

            //
            //-- PILOT BARO ALTIMETER
            AddFunction(new Axis(this, devices.PLTAAU32A.ToString("d"), device_commands.pilotBarometricScaleSet.ToString("d"), "63", 0.1d, 0d, 1d, "Altimeter Encoder (Pilot)", "Barometric Scale Set"));
            //-- COPILOT BARO ALTIMETER
            AddFunction(new Axis(this, devices.CPLTAAU32A.ToString("d"), device_commands.copilotBarometricScaleSet.ToString("d"), "73", 0.1d, 0d, 1d, "Altimeter Encoder (Copilot)", "Barometric Scale Set"));
            //
            //-- PARKING BRAKE
            AddFunction(Switch.CreateToggleSwitch(this, devices.EFM_HELPER.ToString("d"), device_commands.parkingBrake.ToString("d"), "80", "1.0", "On", "0.0", "Off", "EFM", "Parking Brake ON/OFF", "%.1f"));

            //
            //-- AHRU
            AddFunction(new PushButton(this, devices.AHRU.ToString("d"), device_commands.ahruMode.ToString("d"), "90", "Attitude and Heading Ref Unit", "Mode Selector")); // Inop
            AddFunction(new PushButton(this, devices.AHRU.ToString("d"), device_commands.ahruUp.ToString("d"), "91", "Attitude and Heading Ref Unit", "AHRU Display Cursor Movement UP")); // Inop
            AddFunction(new PushButton(this, devices.AHRU.ToString("d"), device_commands.ahruRight.ToString("d"), "92", "Attitude and Heading Ref Unit", "Display Cursor Movement RIGHT")); // Inop
            AddFunction(new PushButton(this, devices.AHRU.ToString("d"), device_commands.ahruEnter.ToString("d"), "93", "Attitude and Heading Ref Unit", "Enter Selection")); // Inop
            AddFunction(new NetworkValue(this, "2106", "Attitude and Heading Ref Unit", "Display Text", "Text line displayed in  AHRU", "Text", BindingValueUnits.Text, null));

            AddFunction(new FlagValue(this, mainpanel.ahruIndSlave.ToString("d"), "Indicators/Lamps/Flags", "AHRU IND SLAVE", ""));
            AddFunction(new FlagValue(this, mainpanel.ahruIndDG.ToString("d"), "Indicators/Lamps/Flags", "AHRU IND DG", ""));
            AddFunction(new FlagValue(this, mainpanel.ahruIndCcal.ToString("d"), "Indicators/Lamps/Flags", "AHRU IND CCAL", ""));
            AddFunction(new FlagValue(this, mainpanel.ahruIndFail.ToString("d"), "Indicators/Lamps/Flags", "AHRU IND FAIL", ""));
            AddFunction(new FlagValue(this, mainpanel.ahruIndAln.ToString("d"), "Indicators/Lamps/Flags", "AHRU IND ALIGN", ""));

            //
            //-- PILOT HSI
            AddFunction(new Axis(this, devices.PLTCISP.ToString("d"), device_commands.pilotHSIHdgSet.ToString("d"), "130", 0.1d, 0d, 1d, "HSI (Pilot)", "Heading Set"));
            AddFunction(new Axis(this, devices.PLTCISP.ToString("d"), device_commands.pilotHSICrsSet.ToString("d"), "131", 0.1d, 0d, 1d, "HSI (Pilot)", "Course Set"));
            //-- COPILOT HSI
            AddFunction(new Axis(this, devices.CPLTCISP.ToString("d"), device_commands.copilotHSIHdgSet.ToString("d"), "150", 0.1d, 0d, 1d, "HSI (Copilot)", "Heading Set"));
            AddFunction(new Axis(this, devices.CPLTCISP.ToString("d"), device_commands.copilotHSICrsSet.ToString("d"), "151", 0.1d, 0d, 1d, "HSI (Copilot)", "Course Set"));

            //
            //-- MISC
            AddFunction(new PushButton(this, devices.MISC.ToString("d"), device_commands.miscFuelIndTest.ToString("d"), "290", "Misc Panel", "Fuel Indicator Test")); // Inop
            AddFunction(new PushButton(this, devices.MISC.ToString("d"), device_commands.miscTailWheelLock.ToString("d"), "291", "Misc Panel", "Wheel Lock"));
            AddFunction(new PushButton(this, devices.MISC.ToString("d"), device_commands.miscGyroEffect.ToString("d"), "292", "Misc Panel", "Gyro Select"));  // Inop
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.miscTailServo.ToString("d"), "296", "1.0", "Normal", "0.0", "Backup", "Misc Panel", "Servo Select NORMAL/BACKUP", "%.1f")); // Inop

            //
            //-- CAUTION/DISPLAY PANELS
            AddFunction(Switch.CreateToggleSwitch(this, devices.VIDS.ToString("d"), device_commands.cduLampTest.ToString("d"), "301", "1.0", "On", "0.0", "Off", "Lamp Test", "CDU Test", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.VIDS.ToString("d"), device_commands.pilotPDUTest.ToString("d"), "302", "1.0", "On", "0.0", "Off", "Lamp Test", "Pilot PDU Test", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.VIDS.ToString("d"), device_commands.copilotPDUTest.ToString("d"), "303", "1.0", "On", "0.0", "Off", "Lamp Test", "Copilot PDU Test", "%.1f"));

            //
            AddFunction(new Switch(this, devices.CAUTION_ADVISORY_PANEL.ToString("d"), "304", new SwitchPosition[] { new SwitchPosition("0.0", "Test", device_commands.CAPLampTest.ToString("d")), new SwitchPosition("0.5", "Off", device_commands.CAPLampTest.ToString("d")), new SwitchPosition("1.0", "Brightness", device_commands.CAPLampBrightness.ToString("d"), null, null, "0.0") }, "Caution Panel", "Lamp Test", "%0.1f"));
            AddFunction(new PushButton(this, devices.CAUTION_ADVISORY_PANEL.ToString("d"), device_commands.CAPMasterCautionReset.ToString("d"), "305", "Caution Panel", "Master Caution Reset"));
            AddFunction(new PushButton(this, devices.CAUTION_ADVISORY_PANEL.ToString("d"), device_commands.CAPMasterCautionReset.ToString("d"), "306", "Caution Panel", "Master Caution Reset (306)"));  // not sure why this has two codes CPLT/Pilot maybe

            //
            AddFunction(new Switch(this, devices.ASN128B.ToString("d"), "500", CreateSwitchPositions(7, 0.01, device_commands.SelectDisplay.ToString("d")), "Doppler Nav", "Display Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ASN128B.ToString("d"), "501", CreateSwitchPositions(6, 0.01, device_commands.SelectMode.ToString("d")), "Doppler Nav", "Mode Selector", "%0.2f"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnKybd.ToString("d"), "502", "Doppler Nav", "Button Keyboard"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnLtrLeft.ToString("d"), "503", "Doppler Nav", "Button Letter Left"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnLtrMid.ToString("d"), "504", "Doppler Nav", "Button Letter Middle"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnLtrRight.ToString("d"), "505", "Doppler Nav", "Button Letter Right"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnF1.ToString("d"), "506", "Doppler Nav", "Button F1"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnTgtStr.ToString("d"), "510", "Doppler Nav", "Button TGT STR"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnInc.ToString("d"), "514", "Doppler Nav", "Button Increment"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnDec.ToString("d"), "518", "Doppler Nav", "Button Decrement"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn1.ToString("d"), "507", "Doppler Nav", "Button 1"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn2.ToString("d"), "508", "Doppler Nav", "Button 2"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn3.ToString("d"), "509", "Doppler Nav", "Button 3"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn4.ToString("d"), "511", "Doppler Nav", "Button 4"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn5.ToString("d"), "512", "Doppler Nav", "Button 5"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn6.ToString("d"), "513", "Doppler Nav", "Button 6"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn7.ToString("d"), "515", "Doppler Nav", "Button 7"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn8.ToString("d"), "516", "Doppler Nav", "Button 8"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn9.ToString("d"), "517", "Doppler Nav", "Button 9"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtn0.ToString("d"), "520", "Doppler Nav", "Button 0"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnClr.ToString("d"), "519", "Doppler Nav", "Button Clear"));
            AddFunction(new PushButton(this, devices.ASN128B.ToString("d"), device_commands.SelectBtnEnt.ToString("d"), "521", "Doppler Nav", "Button Enter"));
            AddFunction(new NetworkValue(this, "2091", "Doppler Nav", "Display Line 1", "Nav Computer first display line", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2092", "Doppler Nav", "Display Line 2", "Nav Computer second display line", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2093", "Doppler Nav", "Display Line 3", "Nav Computer third display line", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2094", "Doppler Nav", "Display Line 4", "Nav Computer fourth display line", "Text", BindingValueUnits.Text, null));

            //
            //-- CIS/MODE SEL
            AddFunction(new PushButton(this, devices.CISP.ToString("d"), device_commands.PilotCISHdgToggle.ToString("d"), "930", "CIS", "Heading Mode ON/OFF"));
            AddFunction(new PushButton(this, devices.CISP.ToString("d"), device_commands.PilotCISNavToggle.ToString("d"), "931", "CIS", "Nav Mode ON/OFF"));
            AddFunction(new PushButton(this, devices.CISP.ToString("d"), device_commands.PilotCISAltToggle.ToString("d"), "932", "CIS", "Altitude Hold Mode ON/OFF"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotNavGPSToggle.ToString("d"), "933", "Mode Select (Pilot)", "NAV Mode: Doppler/GPS ON/OFF"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotNavVORILSToggle.ToString("d"), "934", "Mode Select (Pilot)", "NAV Mode: VOR/ILS ON/OFF"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotNavBACKCRSToggle.ToString("d"), "935", "Mode Select (Pilot)", "NAV Mode: Back Course ON/OFF"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotNavFMHOMEToggle.ToString("d"), "936", "Mode Select (Pilot)", "NAV Mode: FM Homing ON/OFF"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotTURNRATEToggle.ToString("d"), "937", "Mode Select (Pilot)", "Turn Rate Selector NORM/ALTR"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotCRSHDGToggle.ToString("d"), "938", "Mode Select (Pilot)", "Course Heading Selector PLT/CPLT"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotVERTGYROToggle.ToString("d"), "939", "Mode Select (Pilot)", "Vertical Gyro Selector NORM/ALTR"));
            AddFunction(new PushButton(this, devices.PLTCISP.ToString("d"), device_commands.PilotBRG2Toggle.ToString("d"), "940", "Mode Select (Pilot)", "No. 2 Bearing Selector ADF/VOR"));

            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotNavGPSToggle.ToString("d"), "941", "Mode Select (Copilot)", "NAV Mode: Doppler/GPS ON/OFF"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotNavVORILSToggle.ToString("d"), "942", "Mode Select (Copilot)", "NAV Mode: VOR/ILS ON/OFF"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotNavBACKCRSToggle.ToString("d"), "943", "Mode Select (Copilot)", "NAV Mode: Back Course ON/OFF"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotNavFMHOMEToggle.ToString("d"), "944", "Mode Select (Copilot)", "NAV Mode: FM Homing ON/OFF"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotTURNRATEToggle.ToString("d"), "945", "Mode Select (Copilot)", "Turn Rate Selector NORM/ALTR"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotCRSHDGToggle.ToString("d"), "946", "Mode Select (Copilot)", "Course Heading Selector PLT/CPLT"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotVERTGYROToggle.ToString("d"), "947", "Mode Select (Copilot)", "Vertical Gyro Selector NORM/ALTR"));
            AddFunction(new PushButton(this, devices.CPLTCISP.ToString("d"), device_commands.CopilotBRG2Toggle.ToString("d"), "948", "Mode Select (Copilot)", "No. 2 Bearing Selector ADF/VOR"));

            //
            //-- AN/AVS-7 PANEL
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1100", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Off", device_commands.setAVS7Power.ToString("d")),
                new SwitchPosition("0.0", "On", device_commands.setAVS7Power.ToString("d")),
                new SwitchPosition("1.0", "Adjust", device_commands.setAVS7Power.ToString("d"))
                }, "NVG / HUD", "Power OFF/ON/ADJUST", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1101", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Pilot", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Copilot", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Program Pilot/Copilot", "%0.1f"));  // Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1102", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Mode 1-4", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Declutter", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Pilot MODE 1-4/DCLT", "%0.1f"));  // Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1103", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Mode 1-4", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Declutter", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Copilot MODE 1-4/DCLT", "%0.1f"));  // Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1104", new SwitchPosition[] {
                new SwitchPosition("-1.0", "BIT", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Ack", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "BIT/ACK", "%0.1f"));  // Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1105", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Decrement", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Increment", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "ALT/P/R DEC/INC PGM NXT/SEL", "%0.1f"));  // Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1106", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Bright", device_commands.incAVS7Brightness.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.incAVS7Brightness.ToString("d")),
                new SwitchPosition("1.0", "Dim", device_commands.decAVS7Brightness.ToString("d"))
                }, "NVG / HUD", "Pilot BRT/DIM", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1107", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Up", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Down", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Pilot DSPL POS Down/Up", "%0.1f"));  //Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1108", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Left", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Right", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Pilot DSPL POS Left/Right", "%0.1f"));  //Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1109", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Bright", device_commands.incAVS7Brightness.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.incAVS7Brightness.ToString("d")),
                new SwitchPosition("1.0", "Dim", device_commands.decAVS7Brightness.ToString("d"))
                }, "NVG / HUD", "Copilot BRT/DIM", "%0.1f"));  // Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1110", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Up", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Down", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Copilot DSPL POS Down/Up", "%0.1f"));  //Inop
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1111", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Left", device_commands.foo.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.foo.ToString("d")),
                new SwitchPosition("1.0", "Right", device_commands.foo.ToString("d"))
                }, "NVG / HUD", "Copilot DSPL POS Left/Right", "%0.1f"));  //Inop

            //-- AN/ARC-164
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "50", CreateSwitchPositions(4 ,0.01, device_commands.arc164_mode.ToString("d")), "UHF Radio", "Mode", "%0.2f"));
            AddFunction(new Axis(this, devices.ARC164.ToString("d"), device_commands.arc164_volume.ToString("d"), "51", 0.1d, 0d, 1d, "UHF Radio", "Volume"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "52", CreateSwitchPositions(4, 0.01, device_commands.arc164_freq_Xooooo.ToString("d")), "UHF Radio", "Manual/Preset/Guard", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "53", CreateSwitchPositions(2, 0.1, device_commands.arc164_freq_Xooooo.ToString("d")), "UHF Radio", "Digit 100", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "54", CreateSwitchPositions(10, 0.1, device_commands.arc164_freq_oXoooo.ToString("d")), "UHF Radio", "Digit 10", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "55", CreateSwitchPositions(10, 0.1, device_commands.arc164_freq_ooXooo.ToString("d")), "UHF Radio", "Digit 1", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "56", CreateSwitchPositions(10, 0.1, device_commands.arc164_freq_oooXoo.ToString("d")), "UHF Radio", "Digit 0.1", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "57", CreateSwitchPositions(4, 0.1, device_commands.arc164_freq_ooooXX.ToString("d")), "UHF Radio", "Digit 0.010", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "58", CreateSwitchPositions(20, 0.05, device_commands.arc164_preset.ToString("d")), "UHF Radio", "Preset", "%0.2f"));

            //
            //-- Pilot APN-209 Radar Altimeter
            AddFunction(new Axis(this, devices.PLTAPN209.ToString("d"), device_commands.apn209PilotLoSet.ToString("d"), "170", 0.1d, 0d, 1d, "RADAR Alt (Pilot)", "Low Altitude Set"));
            AddFunction(new Axis(this, devices.PLTAPN209.ToString("d"), device_commands.apn209PilotHiSet.ToString("d"), "171", 0.1d, 0d, 1d, "RADAR Alt (Pilot)", "High Altitude Set"));
            AddFunction(new Axis(this, devices.CPLTAPN209.ToString("d"), device_commands.apn209CopilotLoSet.ToString("d"), "183", 0.1d, 0d, 1d, "RADAR Alt (Copilot)", "Low Altitude Set"));
            AddFunction(new Axis(this, devices.CPLTAPN209.ToString("d"), device_commands.apn209CopilotHiSet.ToString("d"), "184", 0.1d, 0d, 1d, "RADAR Alt (Copilot)", "High Altitude Set"));

            //
            //-- Lighting
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.glareshieldLights.ToString("d"), "251", 0.1d, 0d, 1d, "Lighting", "Glareshield Lights OFF/BRT"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "252", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Dim", device_commands.posLightIntensity.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.posLightIntensity.ToString("d")),
                new SwitchPosition("1.0", "Brt", device_commands.posLightIntensity.ToString("d"))
                }, "Lighting", "Position Lights DIM/OFF/BRT", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTLIGHTS.ToString("d"), device_commands.posLightMode.ToString("d"), "253", "1.0", "On", "0.0", "Off", "Lighting", "Position Lights STEADY/FLASH", "%.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "254", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Upper", device_commands.antiLightGrp.ToString("d")),
                new SwitchPosition("0.0", "Both", device_commands.antiLightGrp.ToString("d")),
                new SwitchPosition("1.0", "Lower", device_commands.antiLightGrp.ToString("d"))
                }, "Lighting", "Anticollision Lights UPPER/BOTH/LOWER", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "255", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Day", device_commands.antiLightMode.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.antiLightMode.ToString("d")),
                new SwitchPosition("1.0", "Night", device_commands.antiLightMode.ToString("d"))
                }, "Lighting", "Anticollision Lights DAY/OFF/NIGHT", "%0.1f")); 
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTLIGHTS.ToString("d"), device_commands.navLightMode.ToString("d"), "256", "1.0", "Norm", "0.0", "IR", "Lighting", "Nav Lights NORM/IR", "%.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "257", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Blue", device_commands.cabinLightMode.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.cabinLightMode.ToString("d")),
                new SwitchPosition("1.0", "White", device_commands.cabinLightMode.ToString("d"))
                }, "Lighting", "Cabin Lights BLUE/OFF/WHITE", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.cpltInstrLights.ToString("d"), "259", 0.1d, 0d, 1d, "Lighting", "Copilot Flight Instrument Lights OFF/BRT"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.lightedSwitches.ToString("d"), "260", 0.1d, 0d, 1d, "Lighting", "Lighted Switches OFF/BRT"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "261", new SwitchPosition[] {
                new SwitchPosition("0.0", "Off", device_commands.formationLights.ToString("d")),
                new SwitchPosition("0.2", "1", device_commands.formationLights.ToString("d")),
                new SwitchPosition("0.4", "2", device_commands.formationLights.ToString("d")),
                new SwitchPosition("0.6", "3", device_commands.formationLights.ToString("d")),
                new SwitchPosition("0.8", "4", device_commands.formationLights.ToString("d")),
                new SwitchPosition("1.0", "5", device_commands.formationLights.ToString("d"))
                }, "Lighting", "Formation Lights OFF/1/2/3/4/5", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.upperConsoleLights.ToString("d"), "262", 0.1d, 0d, 1d, "Lighting", "Upper Console Lights OFF/BRT"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.lowerConsoleLights.ToString("d"), "263", 0.1d, 0d, 1d, "Lighting", "Lower Console Lights OFF/BRT"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.pltInstrLights.ToString("d"), "264", 0.1d, 0d, 1d, "Lighting", "Pilot Flight Instrument Lights OFF/BRT"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.nonFltInstrLights.ToString("d"), "265", 0.1d, 0d, 1d, "Lighting", "Non Flight Instrument Lights OFF/BRT"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.pltRdrAltLights.ToString("d"), "266", 0.1d, 0d, 1d, "Lighting", "Radar Altimeter Dimmer Pilot"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), device_commands.cpltRdrAltLights.ToString("d"), "267", 0.1d, 0d, 1d, "Lighting", "Radar Altimeter Dimmer Copilot"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.EXTLIGHTS.ToString("d"), device_commands.magCompassLights.ToString("d"), "268", "1.0", "On", "0.0", "Off", "Lighting", "Magnetic Compass Light ON/OFF", "%.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "269", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Blue", device_commands.cockpitLightMode.ToString("d")),
                new SwitchPosition("0.0", "Off", device_commands.cockpitLightMode.ToString("d")),
                new SwitchPosition("1.0", "White", device_commands.cockpitLightMode.ToString("d"))
                }, "Lighting", "Cockpit Lights BLUE/OFF/WHITE", "%0.1f"));
            //
            //-- AN/APR-39
            AddFunction(Switch.CreateToggleSwitch(this, devices.APR39.ToString("d"), device_commands.apr39Power.ToString("d"), "270", "1.0", "On", "0.0", "Off", "RWR", "Power ON/OFF", "%.1f"));
            AddFunction(new PushButton(this, devices.APR39.ToString("d"), device_commands.apr39SelfTest.ToString("d"), "271", "RWR", "Self Test")); // Inop
            AddFunction(Switch.CreateToggleSwitch(this, devices.APR39.ToString("d"), device_commands.apr39Power.ToString("d"), "272", "1.0", "High", "0.0", "Low", "RWR", "Altitude HIGH/LOW", "%.1f"));
            AddFunction(new Axis(this, devices.APR39.ToString("d"), device_commands.apr39Volume.ToString("d"), "273", 0.1d, 0d, 1d, "RWR", "Volume"));
            AddFunction(new Axis(this, devices.APR39.ToString("d"), device_commands.apr39Brightness.ToString("d"), "274", 0.1d, 0d, 1d, "RWR", "Brightness"));

            //-- PILOT LC6 CHRONOMETER
            AddFunction(new PushButton(this, devices.PLTLC6.ToString("d"), device_commands.resetSetBtn.ToString("d"), "280", "Chronometer (Pilot)", "RESET/SET Button"));
            AddFunction(new PushButton(this, devices.PLTLC6.ToString("d"), device_commands.modeBtn.ToString("d"), "281", "Chronometer (Pilot)", "MODE Button"));
            AddFunction(new PushButton(this, devices.PLTLC6.ToString("d"), device_commands.startStopAdvBtn.ToString("d"), "282", "Chronometer (Pilot)", "START/STOP/ADVANCE Button"));
            AddFunction(new NetworkValue(this, "2096", "Chronometer (Pilot)", "Time Display", "Display of the Chronometer Time", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2097", "Chronometer (Pilot)", "Mode Display", "Display of the Chronometer Mode", "Text", BindingValueUnits.Text, null));

            //-- COPILOT LC6 CHRONOMETER
            AddFunction(new PushButton(this, devices.CPLTLC6.ToString("d"), device_commands.resetSetBtn.ToString("d"), "283", "Chronometer (Copilot)", "RESET/SET Button"));
            AddFunction(new PushButton(this, devices.CPLTLC6.ToString("d"), device_commands.modeBtn.ToString("d"), "284", "Chronometer (Copilot)", "MODE Button"));
            AddFunction(new PushButton(this, devices.CPLTLC6.ToString("d"), device_commands.startStopAdvBtn.ToString("d"), "285", "Chronometer (Copilot)", "START/STOP/ADVANCE Button"));
            AddFunction(new NetworkValue(this, "2098", "Chronometer (Copilot)", "Time Display", "Display of the Chronometer Time", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2099", "Chronometer (Copilot)", "Mode Display", "Display of the Chronometer Mode", "Text", BindingValueUnits.Text, null));

            //
            //-- PILOT ICS PANEL
            AddFunction(new Switch(this, devices.BASERADIO.ToString("d"), "400", new SwitchPosition[] {
                new SwitchPosition("0.0", "Off", device_commands.pilotICPXmitSelector.ToString("d")),
                new SwitchPosition("0.2", "1", device_commands.pilotICPXmitSelector.ToString("d")),
                new SwitchPosition("0.4", "2", device_commands.pilotICPXmitSelector.ToString("d")),
                new SwitchPosition("0.6", "3", device_commands.pilotICPXmitSelector.ToString("d")),
                new SwitchPosition("0.8", "4", device_commands.pilotICPXmitSelector.ToString("d")),
                new SwitchPosition("1.0", "5", device_commands.pilotICPXmitSelector.ToString("d"))
                }, "IC Panel", "Pilot ICP XMIT Selector", "%0.1f"));
            AddFunction(new Axis(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPSetVolume.ToString("d"), "401", 0.1d, 0d, 1d, "ICS Panel", "Pilot RCV Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.foo.ToString("d"), "402", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot ICP Hot Mike", "%.1f")); // Inop
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleFM1.ToString("d"), "403", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV FM1", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleUHF.ToString("d"), "404", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV UHF", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleVHF.ToString("d"), "405", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV VHF", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleFM2.ToString("d"), "406", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV FM2", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleHF.ToString("d"), "407", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV HF", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleVOR.ToString("d"), "408", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV VOR/LOC", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.PLT_ICP.ToString("d"), device_commands.pilotICPToggleADF.ToString("d"), "409", "1.0", "On", "0.0", "Off", "IC Panel", "Pilot RCV ADF", "%.1f"));
            //
            //-- TODO OTHER ICS PANELS?
            //
            //-- ARC-186 VHF
            AddFunction(new Axis(this, devices.ARC186.ToString("d"), device_commands.arc186Volume.ToString("d"), "410", 0.1d, 0d, 1d, "VHF Radio", "Volume"));
            AddFunction(new PushButton(this, devices.ARC186.ToString("d"), device_commands.arc186Tone.ToString("d"), "411", "VHF Radio", "Tone")); // Inop
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "412", new SwitchPosition[] {
                new SwitchPosition("0.000", "0",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.083", "1",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.167", "2",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.250", "3",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.333", "4",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.416", "5",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.500", "6",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.583", "7",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.667", "8",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.750", "9",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.833", "10",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("0.916", "11",device_commands.arc186Selector10MHz.ToString("d")),
                new SwitchPosition("1.000", "12",device_commands.arc186Selector10MHz.ToString("d"))
                }, "VHF Radio", "10MHz Selector", "%0.3f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "413", new SwitchPosition[] {
                new SwitchPosition("0.0", "0",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.1", "1",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.2", "2",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.3", "3",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.4", "4",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.5", "5",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.6", "6",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.7", "7",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.8", "8",device_commands.arc186Selector1MHz.ToString("d")),
                new SwitchPosition("0.9", "9",device_commands.arc186Selector1MHz.ToString("d"))
                }, "VHF Radio", "1MHz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "414", new SwitchPosition[] {
                new SwitchPosition("0.0", "0",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.1", "1",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.2", "2",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.3", "3",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.4", "4",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.5", "5",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.6", "6",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.7", "7",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.8", "8",device_commands.arc186Selector100KHz.ToString("d")),
                new SwitchPosition("0.9", "9",device_commands.arc186Selector100KHz.ToString("d"))
                }, "VHF Radio", "100KHz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "415", new SwitchPosition[] {
                new SwitchPosition("0.00", "0",device_commands.arc186Selector25KHz.ToString("d")),
                new SwitchPosition("0.25", "1",device_commands.arc186Selector25KHz.ToString("d")),
                new SwitchPosition("0.50", "2",device_commands.arc186Selector25KHz.ToString("d")),
                new SwitchPosition("0.75", "3",device_commands.arc186Selector25KHz.ToString("d"))
                }, "VHF Radio", "25KHz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "416", new SwitchPosition[] {
                new SwitchPosition("0.00", "0",device_commands.arc186FreqSelector.ToString("d")),
                new SwitchPosition("0.33", "1",device_commands.arc186FreqSelector.ToString("d")),
                new SwitchPosition("0.67", "2",device_commands.arc186FreqSelector.ToString("d")),
                new SwitchPosition("1.00", "3",device_commands.arc186FreqSelector.ToString("d"))
                }, "VHF Radio", "Frequency Control Selector", "%0.2f"));
            AddFunction(new PushButton(this, devices.ARC186.ToString("d"), device_commands.arc186Load.ToString("d"), "417", "VHF Radio", "Load Pushbutton"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "418", CreateSwitchPositions(20,0.05, device_commands.arc186PresetSelector.ToString("d")), "VHF Radio", "Preset Channel Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "419", new SwitchPosition[] {
                new SwitchPosition("0.0", "0",device_commands.arc186ModeSelector.ToString("d")),
                new SwitchPosition("0.5", "1",device_commands.arc186ModeSelector.ToString("d")),
                new SwitchPosition("1.0", "2",device_commands.arc186ModeSelector.ToString("d"))
                }, "VHF Radio", "Mode Selector", "%0.1f"));
            //
            //-- AFMS
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "460", new SwitchPosition[] {
                new SwitchPosition("0.0", "Man", device_commands.afmcpXferMode.ToString("d")),
                new SwitchPosition("0.5", "Off", device_commands.afmcpXferMode.ToString("d")),
                new SwitchPosition("1.0", "Auto", device_commands.afmcpXferMode.ToString("d"))
                }, "Aux Fuel", "Transfer Mode MAN/OFF/AUTO", "%0.1f"));
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "461", new SwitchPosition[] {
                new SwitchPosition("0.0", "Right", device_commands.afmcpXferFrom.ToString("d")),
                new SwitchPosition("0.5", "Both", device_commands.afmcpXferFrom.ToString("d")),
                new SwitchPosition("1.0", "Left", device_commands.afmcpXferFrom.ToString("d"))
                }, "Aux Fuel", "Manual Transfer RIGHT/BOTH/LEFT", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.AFMS.ToString("d"), device_commands.afmcpXferFrom.ToString("d"), "462", "1.0", "OUTBD", "0.0", "INBD", "Aux Fuel", "Transfer From OUTBD/INBD", "%.1f"));
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "463", new SwitchPosition[] {
                new SwitchPosition("0.0", "1", device_commands.afmcpPress.ToString("d")),
                new SwitchPosition("0.33", "2", device_commands.afmcpPress.ToString("d")),
                new SwitchPosition("0.67", "3", device_commands.afmcpPress.ToString("d")),
                new SwitchPosition("1.0", "4", device_commands.afmcpPress.ToString("d"))
                }, "Aux Fuel", "Pressurization Selector", "%0.2f"));
            AddFunction(new NetworkValue(this, "2107", "Aux Fuel", "Left Outboard Fuel Quantity", "Display of the fuel quantity", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2108", "Aux Fuel", "Left Inboard Fuel Quantity", "Display of the fuel quantity", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2109", "Aux Fuel", "Right Outboard Fuel Quantity", "Display of the fuel quantity", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2110", "Aux Fuel", "Right Inboard Fuel Quantity", "Display of the fuel quantity", "Text", BindingValueUnits.Text, null));


            //
            //-- DOORS
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.doorCplt.ToString("d"), "470", "1.0", "On", "0.0", "Off", "Doors", "Copilot Door", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.doorPlt.ToString("d"), "471", "1.0", "On", "0.0", "Off", "Doors", "Pilot Door", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.doorLGnr.ToString("d"), "472", "1.0", "On", "0.0", "Off", "Doors", "Left Gunner Window", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.doorRGnr.ToString("d"), "473", "1.0", "On", "0.0", "Off", "Doors", "Right Gunner Window", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.doorLCargo.ToString("d"), "474", "1.0", "On", "0.0", "Off", "Doors", "Left Cargo Door", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.MISC.ToString("d"), device_commands.doorRCargo.ToString("d"), "475", "1.0", "On", "0.0", "Off", "Doors", "Right Cargo Door", "%.1f"));

            //
            //-- M130 CM System
            AddFunction(Switch.CreateToggleSwitch(this, devices.M130.ToString("d"), device_commands.cmFlareDispenseModeCover.ToString("d"), "550", "1.0", "Open", "0.0", "Closed", "Counter Measures", "Flare Dispenser Mode Cover", "%.1f"));
            //--cmFlareDispenseMode
            AddFunction(new Switch(this, devices.M130.ToString("d"), "552", CreateSwitchPositions(10, 1 / 9, device_commands.cmFlareCounterDial.ToString("d")), "Counter Measures", "Flare Counter Encoder", "%0.2f"));
            AddFunction(new Switch(this, devices.M130.ToString("d"), "553", CreateSwitchPositions(10, 1 / 9, device_commands.cmChaffCounterDial.ToString("d")), "Counter Measures", "Chaff Counter Encoder", "%0.2f"));
            AddFunction(new NetworkValue(this, mainpanel.cmFlareCounterTens.ToString("d"), "Counter Measures", "M130 Flare Counter Tens Digit", "Number", "", BindingValueUnits.Numeric, "%0.2f"));
            AddFunction(new NetworkValue(this, mainpanel.cmFlareCounterOnes.ToString("d"), "Counter Measures", "M130 Flare Counter Ones Digit", "Number", "", BindingValueUnits.Numeric, "%0.2f"));
            AddFunction(new NetworkValue(this, mainpanel.cmChaffCounterTens.ToString("d"), "Counter Measures", "M130 Chaff Counter Tens Digit", "Number", "", BindingValueUnits.Numeric, "%0.2f"));
            AddFunction(new NetworkValue(this, mainpanel.cmChaffCounterOnes.ToString("d"), "Counter Measures", "M130 Chaff Counter Ones Digit", "Number", "", BindingValueUnits.Numeric, "%0.2f"));
            AddFunction(new FlagValue(this, mainpanel.cmArmedLight.ToString("d"), "Counter Measures", "M130 Armed Indicator", ""));
            AddFunction(Switch.CreateToggleSwitch(this, devices.M130.ToString("d"), device_commands.cmArmSwitch.ToString("d"), "559", "1.0", "Arm", "0.0", "Safe", "Counter Measures", "Arming Switch", "%.1f"));
            AddFunction(new Switch(this, devices.M130.ToString("d"), "560", new SwitchPosition[] {
                new SwitchPosition("0.0", "0",device_commands.cmProgramDial.ToString("d")),
                new SwitchPosition("0.5", "1",device_commands.cmProgramDial.ToString("d")),
                new SwitchPosition("1.0", "2",device_commands.cmProgramDial.ToString("d"))
                }, "Counter Measures", "Chaff Mode Selector", "%0.1f"));
            AddFunction(new PushButton(this, devices.M130.ToString("d"), device_commands.cmChaffDispense.ToString("d"), "561", "Counter Measures", "Chaff Dispense"));

            //-- ARC-201 FM1
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "600", CreateSwitchPositions(8, 0.01, device_commands.fm1PresetSelector.ToString("d")), "Tactical Radio FM1", "Preset Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "601", CreateSwitchPositions(9, 0.01, device_commands.fm1FunctionSelector.ToString("d")), "Tactical Radio FM1", "Function Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "602", CreateSwitchPositions(4, 0.01, device_commands.fm1PwrSelector.ToString("d")), "Tactical Radio FM1", "PWR Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "603", CreateSwitchPositions(4, 0.01, device_commands.fm1ModeSelector.ToString("d")), "Tactical Radio FM1", "Mode Selector", "%0.2f"));
            AddFunction(new Axis(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Volume.ToString("d"), "604", 0.1d, 0d, 1d, "Tactical Radio FM1", "Volume"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn1.ToString("d"), "605", "Tactical Radio FM1", "Button 1"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn2.ToString("d"), "606", "Tactical Radio FM1", "Button 2"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn3.ToString("d"), "607", "Tactical Radio FM1", "Button 3"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn4.ToString("d"), "608", "Tactical Radio FM1", "Button 4"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn5.ToString("d"), "609", "Tactical Radio FM1", "Button 5"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn6.ToString("d"), "610", "Tactical Radio FM1", "Button 6"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn7.ToString("d"), "611", "Tactical Radio FM1", "Button 7"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn8.ToString("d"), "612", "Tactical Radio FM1", "Button 8"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn9.ToString("d"), "613", "Tactical Radio FM1", "Button 9"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1Btn0.ToString("d"), "614", "Tactical Radio FM1", "Button 0"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1BtnClr.ToString("d"), "615", "Tactical Radio FM1", "Button Clear"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1BtnEnt.ToString("d"), "616", "Tactical Radio FM1", "Button Enter"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1BtnFreq.ToString("d"), "617", "Tactical Radio FM1", "Button Frequency"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1BtnErfOfst.ToString("d"), "618", "Tactical Radio FM1", "Button EFF/OFST"));
            AddFunction(new PushButton(this, devices.ARC201_FM1.ToString("d"), device_commands.fm1BtnTime.ToString("d"), "619", "Tactical Radio FM1", "Button Time"));
            AddFunction(new NetworkValue(this, "2104", "Tactical Radio FM1", "Display Text", "FM Radio Display Text", "Text", BindingValueUnits.Text, null));

            //
            //-- AN/ARN-149
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "620", CreateSwitchPositions(3, 0.5, device_commands.arn149Preset.ToString("d")), "ADF", "Preset Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "621", new SwitchPosition[] { new SwitchPosition("-1.0", "Tone", device_commands.arn149ToneTest.ToString("d")), new SwitchPosition("0.0", "Off", device_commands.arn149ToneTest.ToString("d")), new SwitchPosition("1.0", "Test", device_commands.arn149ToneTest.ToString("d"), null, null, "0.0") }, "ADF", "TONE/OFF/TEST", "%0.1f"));
            AddFunction(new Axis(this, devices.ARN149.ToString("d"), device_commands.fm2Volume.ToString("d"), "622", 0.1d, 0d, 1d, "ADF", "Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.ARN149.ToString("d"), device_commands.foo.ToString("d"), "623", "1.0", "On", "0.0", "Off", "ADF", "Take CMD", "%.1f"));  // Inop
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "624", CreateSwitchPositions(3, 0.5, device_commands.arn149Power.ToString("d")), "ADF", "Power Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "625", CreateSwitchPositions(3, 0.5, device_commands.arn149thousands.ToString("d")), "ADF", "1000s Khz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "626", CreateSwitchPositions(10, 0.1, device_commands.arn149hundreds.ToString("d")), "ADF", "100s Khz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "627", CreateSwitchPositions(10, 0.1, device_commands.arn149tens.ToString("d")), "ADF", "10s Khz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "628", CreateSwitchPositions(10, 0.1, device_commands.arn149ones.ToString("d")), "ADF", "1s Khz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "629", CreateSwitchPositions(10, 0.1, device_commands.arn149tenths.ToString("d")), "ADF", ".1s Khz Selector", "%0.2f"));

            //
            //-- AN/ARN-147
            AddFunction(new Switch(this, devices.ARN147.ToString("d"), "650", CreateSwitchPositions(10, 0.1, device_commands.arn147MHz.ToString("d")), "VOR ILS Receiver", "MHz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARN147.ToString("d"), "651", CreateSwitchPositions(10, 0.1, device_commands.arn147KHz.ToString("d")), "VOR ILS Receiver", "KHz Selector", "%0.2f"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.ARN147.ToString("d"), device_commands.foo.ToString("d"), "652", "1.0", "High", "0.0", "Low", "VOR ILS Receiver", "Marker Beacon HI/LO", "%.1f"));  // Inop
            AddFunction(new Switch(this, devices.ARN147.ToString("d"), "653", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Off", device_commands.arn147Power.ToString("d")),
                new SwitchPosition("0.0", "On", device_commands.arn147Power.ToString("d")),
                new SwitchPosition("1.0", "Test", device_commands.arn147Power.ToString("d"))
                }, "VOR ILS Receiver", "Power Selector OFF/ON/TEST", "%0.1f"));

            //
            //-- WIPERS
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "631", new SwitchPosition[] {
                new SwitchPosition("-0.50", "Park",device_commands.wiperSelector.ToString("d")),
                new SwitchPosition("0.00", "Off",device_commands.wiperSelector.ToString("d")),
                new SwitchPosition("0.50", "Low",device_commands.wiperSelector.ToString("d")),
                new SwitchPosition("1.00", "High",device_commands.wiperSelector.ToString("d"))
                }, "Wipers", "Wipers PARK/OFF/LOW/HI", "%0.2f"));
            //
            //-- ARC-201 FM2
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "700", CreateSwitchPositions(8, 0.01, device_commands.fm2PresetSelector.ToString("d")), "Tactical Radio FM2", "Preset Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "701", CreateSwitchPositions(9, 0.01, device_commands.fm2FunctionSelector.ToString("d")), "Tactical Radio FM2", "Function Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "702", CreateSwitchPositions(4, 0.01, device_commands.fm2PwrSelector.ToString("d")), "Tactical Radio FM2", "PWR Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "703", CreateSwitchPositions(4, 0.01, device_commands.fm2ModeSelector.ToString("d")), "Tactical Radio FM2", "Mode Selector", "%0.2f"));
            AddFunction(new Axis(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Volume.ToString("d"), "704", 0.1d, 0d, 1d, "Tactical Radio FM2", "Volume"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn1.ToString("d"), "705", "Tactical Radio FM2", "Button 1"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn2.ToString("d"), "706", "Tactical Radio FM2", "Button 2"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn3.ToString("d"), "707", "Tactical Radio FM2", "Button 3"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn4.ToString("d"), "708", "Tactical Radio FM2", "Button 4"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn5.ToString("d"), "709", "Tactical Radio FM2", "Button 5"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn6.ToString("d"), "710", "Tactical Radio FM2", "Button 6"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn7.ToString("d"), "711", "Tactical Radio FM2", "Button 7"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn8.ToString("d"), "712", "Tactical Radio FM2", "Button 8"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn9.ToString("d"), "713", "Tactical Radio FM2", "Button 9"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2Btn0.ToString("d"), "714", "Tactical Radio FM2", "Button 0"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2BtnClr.ToString("d"), "715", "Tactical Radio FM2", "Button Clear"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2BtnEnt.ToString("d"), "716", "Tactical Radio FM2", "Button Enter"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2BtnFreq.ToString("d"), "717", "Tactical Radio FM2", "Button Frequency"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2BtnErfOfst.ToString("d"), "718", "Tactical Radio FM2", "Button ERF/OFST"));
            AddFunction(new PushButton(this, devices.ARC201_FM2.ToString("d"), device_commands.fm2BtnTime.ToString("d"), "719", "Tactical Radio FM2", "Button Time"));
            AddFunction(new NetworkValue(this, "2105", "Tactical Radio FM2", "Display Text", "FM Radio Display Text", "Text", BindingValueUnits.Text, null));

            //
            //-- CPLT ICP
            // * * * CPLT_ICP not currently defined.
            //elements["PNT-800"]	= multiposition_switch(_("Copilot ICP XMIT Selector (Inop.)"),            devices.CPLT_ICP, device_commands.copilotICPXmitSelector, 800, 6,  1/5,  false, 0, 16, false)
            //AddFunction(new Axis(this, devices.CPLT_ICP.ToString("d"), device_commands.copilotICPSetVolume.ToString("d"), "801", 0.1d, 0d, 1d, "ICP (Copilot)", "Volume"));
            //elements["PNT-802"]	= default_2_position_tumb(_("Copilot ICP Hot Mike (Inop.)"),              devices.CPLT_ICP, device_commands.foo, 802, 8)
            //elements["PNT-803"]	= default_2_position_tumb(_("Copilot ICP RCV FM1 (Inop.)"),               devices.CPLT_ICP, device_commands.copilotICPToggleFM1, 803, 8)
            //elements["PNT-804"]	= default_2_position_tumb(_("Copilot ICP RCV UHF (Inop.)"),               devices.CPLT_ICP, device_commands.copilotICPToggleUHF, 804, 8)
            //elements["PNT-805"]	= default_2_position_tumb(_("Copilot ICP RCV VHF (Inop.)"),               devices.CPLT_ICP, device_commands.copilotICPToggleVHF, 805, 8)
            //elements["PNT-806"]	= default_2_position_tumb(_("Copilot ICP RCV FM2 (Inop.)"),               devices.CPLT_ICP, device_commands.copilotICPToggleFM2, 806, 8)
            //elements["PNT-807"]	= default_2_position_tumb(_("Copilot ICP RCV HF (Inop.)"),                devices.CPLT_ICP, device_commands.copilotICPToggleHF, 807, 8)
            //elements["PNT-808"]	= default_2_position_tumb(_("Copilot ICP RCV VOR/LOC (Inop.)"),           devices.CPLT_ICP, device_commands.copilotICPToggleVOR, 808, 8)
            //elements["PNT-809"]	= default_2_position_tumb(_("Copilot ICP RCV ADF (Inop.)"),               devices.CPLT_ICP, device_commands.copilotICPToggleADF, 809, 8)
            //
            //-- DEBUG
            //elements["PNT-3000"]	= default_2_position_tumb(_("Debug Visualisation ON/OFF"), devices.DEBUG, device_commands.visualisationToggle, 3000, 8)

            // Indicators / Lamps / Flags

            // PILOT BARO ALTIMETER
            AddFunction(new Altimeter(this, Cockpit.Pilot));
            AddFunction(new FlagValue(this, mainpanel.pilotBaroAltEncoderFlag.ToString("d"), "Indicators/Lamps/Flags", "PILOT BAROALT ENCODER FLAG", ""));

            // COPILOT ALTIMETER
            AddFunction(new Altimeter(this, Cockpit.Copilot));
            AddFunction(new FlagValue(this, mainpanel.copilotBaroAltEncoderFlag.ToString("d"), "Indicators/Lamps/Flags", "COPILOT BAROALT ENCODER FLAG", ""));

            // MISC
            AddFunction(new FlagValue(this, mainpanel.parkingBrakeHandle.ToString("d"), "Indicators/Lamps/Flags", "PARKING BRAKE HANDLE", ""));
            AddFunction(new FlagValue(this, mainpanel.pilotPTT.ToString("d"), "Indicators/Lamps/Flags", "PILOT PTT", ""));

            // CIS MODE LIGHTS
            AddFunction(new FlagValue(this, mainpanel.cisHdgOnLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS HDG ON", ""));
            AddFunction(new FlagValue(this, mainpanel.cisNavOnLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS NAV ON", ""));
            AddFunction(new FlagValue(this, mainpanel.cisAltOnLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS ALT ON", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisDplrLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT DPLRGPS", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisVorLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT VOR", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisILSLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT ILS", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisBackCrsLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT BACKCRS", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisFMHomeLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT FMHOME", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisTRNorm.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT TRNORM", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisTRAlt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT TRALT", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisCrsHdgPlt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT CRSHDGPLT", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisCrsHdgCplt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT CRSHDGCPLT", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisGyroNorm.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT GYRONORM", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisGyroAlt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT GYROALT", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisBrg2ADF.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT BRG2ADF", ""));
            AddFunction(new FlagValue(this, mainpanel.pltCisVBrg2VOR.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS PLT BRG2VOR", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisDplrLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT DPLRGPS", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisVorLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT VOR", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisILSLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT ILS", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisBackCrsLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT BACKCRS", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisFMHomeLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT FMHOME", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisTRNorm.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT TRNORM", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisTRAlt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT TRALT", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisCrsHdgPlt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT CRSHDGPLT", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisCrsHdgCplt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT CRSHDGCPLT", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisGyroNorm.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT GYRONORM", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisGyroAlt.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT GYROALT", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisBrg2ADF.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT BRG2ADF", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltCisVBrg2VOR.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING CIS CPLT BRG2VOR", ""));

            // AFCS LIGHTS
            AddFunction(new FlagValue(this, mainpanel.afcBoostLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING AFCS BOOST", ""));
            AddFunction(new FlagValue(this, mainpanel.afcSAS1Light.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING AFCS SAS1", ""));
            AddFunction(new FlagValue(this, mainpanel.afcSAS2Light.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING AFCS SAS2", ""));
            AddFunction(new FlagValue(this, mainpanel.afcTrimLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING AFCS TRIM", ""));
            AddFunction(new FlagValue(this, mainpanel.afcFPSLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING AFCS FPS", ""));
            AddFunction(new FlagValue(this, mainpanel.afcStabLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING AFCS STABAUTO", ""));

            // COCKPIT DOME LTS
            AddFunction(new FlagValue(this, mainpanel.domeLightBlue.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING DOME BLUE", ""));
            AddFunction(new FlagValue(this, mainpanel.domeLightWhite.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING DOME WHITE", ""));

            // MISC PANEL LIGHTS
            //AddFunction(new FlagValue(this, mainpanel.miscFuelIndTestLight.ToString("d"), "Indicators/Lamps/Flags","LIGHTING AFCS STABAUTO"  ""));// NOT YET IMPLEMENTED
            AddFunction(new FlagValue(this, mainpanel.miscTailWheelLockLight.ToString("d"), "Indicators/Lamps/Flags", "LIGHTING MISC TAILWHEELLOCK", ""));
            //AddFunction(new FlagValue(this, mainpanel.//miscGyroEffectLight.ToString("d"), "Indicators/Lamps/Flags","LIGHTING AFCS STABAUTO" , ""));// NOT YET IMPLEMENTED

            // CAP & MCP LAMPS
            AddFunction(new FlagValue(this, mainpanel.capBrightness.ToString("d"), "Indicators/Lamps/Flags", "CAP BRIGHTNESS", ""));

            AddFunction(new FlagValue(this, mainpanel.mcpEng1Out.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY MCP 1ENGOUT", ""));
            AddFunction(new FlagValue(this, mainpanel.mcpEng2Out.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY MCP 2ENGOUT", ""));
            AddFunction(new FlagValue(this, mainpanel.mcpFire.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY MCP FIRE", ""));
            AddFunction(new FlagValue(this, mainpanel.mcpMasterCaution.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY MCP MC", ""));
            AddFunction(new FlagValue(this, mainpanel.mcpLowRPM.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY MCP LOWROTORRPM", ""));
            AddFunction(new FlagValue(this, mainpanel.capFuel1Low.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1FUELLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capGen1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1GEN", ""));
            AddFunction(new FlagValue(this, mainpanel.capGen2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2GEN", ""));
            AddFunction(new FlagValue(this, mainpanel.capFuel2Low.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2FUELLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capFuelPress1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1FUELPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capGenBrg1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1GENBRG", ""));
            AddFunction(new FlagValue(this, mainpanel.capGenBrg2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2GENBRG", ""));
            AddFunction(new FlagValue(this, mainpanel.capFuelPress2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2FUELPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capEngOilPress1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1ENGOILPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capConv1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1CONV", ""));
            AddFunction(new FlagValue(this, mainpanel.capConv2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2CONV", ""));
            AddFunction(new FlagValue(this, mainpanel.capEngOilPress2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2ENGOILPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capEngOilTemp1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1ENGOILTEMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capAcEssBus.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP ACESSBUSOFF", ""));
            AddFunction(new FlagValue(this, mainpanel.capDcEssBus.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP DCESSBUSOFF", ""));
            AddFunction(new FlagValue(this, mainpanel.capEngOilTemp2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2ENGOILTEMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng1Chip.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1ENGCHIP", ""));
            AddFunction(new FlagValue(this, mainpanel.capBattLow.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP BATTLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capBattFault.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP BATTFAULT", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng2Chip.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2ENGCHIP", ""));
            AddFunction(new FlagValue(this, mainpanel.capFuelFltr1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1FUELFILTERBYPASS", ""));
            AddFunction(new FlagValue(this, mainpanel.capGustLock.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP GUSTLOCK", ""));
            AddFunction(new FlagValue(this, mainpanel.capPitchBias.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP PITCHBIASFAIL", ""));
            AddFunction(new FlagValue(this, mainpanel.capFuelFltr2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2FUELFILTERBYPASS", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng1Starter.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1ENGSTARTER", ""));
            AddFunction(new FlagValue(this, mainpanel.capOilFltr1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1OILFILTERBYPASS", ""));
            AddFunction(new FlagValue(this, mainpanel.capOilFltr2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2OILFILTERBYPASS", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng2Starter.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2ENGSTARTER", ""));
            AddFunction(new FlagValue(this, mainpanel.capPriServo1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1PRISERVOPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capHydPump1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1HYDPUMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capHydPump2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2HYDPUMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capPriServo2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2PRISERVOPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capTailRtrQuad.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP TAILROTORQUADRANT", ""));
            AddFunction(new FlagValue(this, mainpanel.capIrcmInop.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP IRCMINOP", ""));
            AddFunction(new FlagValue(this, mainpanel.capAuxFuel.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP AUXFUEL", ""));
            AddFunction(new FlagValue(this, mainpanel.capTailRtrServo1.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1TAILRTRSERVO", ""));
            AddFunction(new FlagValue(this, mainpanel.capMainXmsnOilTemp.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP MAINXMSNOILTEMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capIntXmsnOilTemp.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP INTXMSNOILTEMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capTailXmsnOilTemp.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP TAILXMSNOILTEMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capApuOilTemp.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP APUOILTEMPHI", ""));
            AddFunction(new FlagValue(this, mainpanel.capBoostServo.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP BOOSTSERVOOFF", ""));
            AddFunction(new FlagValue(this, mainpanel.capStab.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP STABILATOR", ""));
            AddFunction(new FlagValue(this, mainpanel.capSAS.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP SASOFF", ""));
            AddFunction(new FlagValue(this, mainpanel.capTrim.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP TRIMFAIL", ""));
            AddFunction(new FlagValue(this, mainpanel.capLftPitot.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP LEFTPITOTHEAT", ""));
            AddFunction(new FlagValue(this, mainpanel.capFPS.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP FLTPATHSTAB", ""));
            AddFunction(new FlagValue(this, mainpanel.capIFF.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP IFF", ""));
            AddFunction(new FlagValue(this, mainpanel.capRtPitot.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP RIGHTPITOTHEAT", ""));
            AddFunction(new FlagValue(this, mainpanel.capLftChipInput.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CHIPINPUTMDLLH", ""));
            AddFunction(new FlagValue(this, mainpanel.capChipIntXmsn.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CHIPINTXMSN", ""));
            AddFunction(new FlagValue(this, mainpanel.capChipTailXmsn.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CHIPTAILXMSN", ""));
            AddFunction(new FlagValue(this, mainpanel.capRtChipInput.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CHIPINPUTMDLRH", ""));
            AddFunction(new FlagValue(this, mainpanel.capLftChipAccess.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CHIPACCESSMDLLH", ""));
            AddFunction(new FlagValue(this, mainpanel.capChipMainSump.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP MAINMDLSUMP", ""));
            AddFunction(new FlagValue(this, mainpanel.capApuFail.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP APUFAIL", ""));
            AddFunction(new FlagValue(this, mainpanel.capRtChipAccess.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CHIPACCESSMDLRH", ""));
            AddFunction(new FlagValue(this, mainpanel.capMrDeIceFault.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP MRDEICEFAIL", ""));
            AddFunction(new FlagValue(this, mainpanel.capMrDeIceFail.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP MRDEICEFAULT", ""));
            AddFunction(new FlagValue(this, mainpanel.capTRDeIceFail.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP TRDEICEFAIL", ""));
            AddFunction(new FlagValue(this, mainpanel.capIce.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP ICEDETECTED", ""));
            AddFunction(new FlagValue(this, mainpanel.capXmsnOilPress.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP MAINXMSNOILPRESS", ""));
            AddFunction(new FlagValue(this, mainpanel.capRsvr1Low.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1RSVRLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capRsvr2Low.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2RSVRLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capBackupRsvrLow.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP BACKUPRSVRLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng1AntiIce.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1ENGANTIICEON", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng1InletAntiIce.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 1ENGINLETANTIICEON", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng2InletAntiIce.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2ENGINLETANTIICEON", ""));
            AddFunction(new FlagValue(this, mainpanel.capEng2AntiIce.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2ENGANTIICEON", ""));
            AddFunction(new FlagValue(this, mainpanel.capApuOn.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP APU", ""));
            AddFunction(new FlagValue(this, mainpanel.capApuGen.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP APUGENON", ""));
            AddFunction(new FlagValue(this, mainpanel.capBoostPumpOn.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP PRIMEBOOSTPUMPON", ""));
            AddFunction(new FlagValue(this, mainpanel.capBackUpPumpOn.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP BACKUPPUMPON", ""));
            AddFunction(new FlagValue(this, mainpanel.capApuAccum.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP APUACCUMLOW", ""));
            AddFunction(new FlagValue(this, mainpanel.capSearchLt.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP SEARCHLIGHTON", ""));
            AddFunction(new FlagValue(this, mainpanel.capLdgLt.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP LANDINGLIGHTON", ""));
            AddFunction(new FlagValue(this, mainpanel.capTailRtrServo2.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP 2TAILRTRSERVOON", ""));
            AddFunction(new FlagValue(this, mainpanel.capHookOpen.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP CARGOHOOKOPEN", ""));
            AddFunction(new FlagValue(this, mainpanel.capHookArmed.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP HOOKARMED", ""));
            AddFunction(new FlagValue(this, mainpanel.capGPS.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP GPSPOSALERT", ""));
            AddFunction(new FlagValue(this, mainpanel.capParkingBrake.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP PARKINGBRAKEON", ""));
            AddFunction(new FlagValue(this, mainpanel.capExtPwr.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP EXTPWRCONNECTED", ""));
            AddFunction(new FlagValue(this, mainpanel.capBlank.ToString("d"), "Indicators/Lamps/Flags", "DISPLAY CAP BLANK", ""));

            AddFunction(new FlagValue(this, mainpanel.pduPltOverspeed1.ToString("d"), "Indicators/Lamps/Flags", "PDU PLT OVERSPEED1", ""));
            AddFunction(new FlagValue(this, mainpanel.pduPltOverspeed2.ToString("d"), "Indicators/Lamps/Flags", "PDU PLT OVERSPEED2", ""));
            AddFunction(new FlagValue(this, mainpanel.pduPltOverspeed3.ToString("d"), "Indicators/Lamps/Flags", "PDU PLT OVERSPEED3", ""));
            AddFunction(new FlagValue(this, mainpanel.pduCpltOverspeed2.ToString("d"), "Indicators/Lamps/Flags", "PDU CPLT OVERSPEED1", ""));
            AddFunction(new FlagValue(this, mainpanel.pduCpltOverspeed3.ToString("d"), "Indicators/Lamps/Flags", "PDU CPLT OVERSPEED2", ""));
            AddFunction(new FlagValue(this, mainpanel.pduCpltOverspeed3.ToString("d"), "Indicators/Lamps/Flags", "PDU CPLT OVERSPEED3", ""));

            // M130 CM System
            //AddFunction(new FlagValue(this, mainpanel.cmFlareCounterTens.ToString("d"), "Indicators/Lamps/Flags", "M130 FLARECOUNTER TENS", ""));
            //AddFunction(new FlagValue(this, mainpanel.cmFlareCounterOnes.ToString("d"), "Indicators/Lamps/Flags", "M130 FLARECOUNTER ONES", ""));
            //AddFunction(new FlagValue(this, mainpanel.cmChaffCounterTens.ToString("d"), "Indicators/Lamps/Flags", "M130 CHAFFCOUNTER TENS", ""));
            //AddFunction(new FlagValue(this, mainpanel.cmChaffCounterOnes.ToString("d"), "Indicators/Lamps/Flags", "M130 CHAFFCOUNTER ONES", ""));

            AddFunction(new FlagValue(this, mainpanel.pltDoorGlass.ToString("d"), "Indicators/Lamps/Flags", "Pilot Door Glass", ""));
            AddFunction(new FlagValue(this, mainpanel.cpltDoorGlass.ToString("d"), "Indicators/Lamps/Flags", "Co-Pilot Door Glass", ""));
            AddFunction(new FlagValue(this, mainpanel.lGnrDoorGlass.ToString("d"), "Indicators/Lamps/Flags", "Left Gunner Door Glass", ""));
            AddFunction(new FlagValue(this, mainpanel.rGnrDoorGlass.ToString("d"), "Indicators/Lamps/Flags", "Right Gunner Door Glass", ""));
            AddFunction(new FlagValue(this, mainpanel.lCargoDoorGlass.ToString("d"), "Indicators/Lamps/Flags", "Left Cargo Door Glass", ""));
            AddFunction(new FlagValue(this, mainpanel.rCargoDoorGlass.ToString("d"), "Indicators/Lamps/Flags", "Right Cargo Door Glass", ""));


            #region Network Values

            AddFunction(new SegmentedMeter(this, "2065", 30, "Engine Management", "Fuel Quantity Left", "Bar display of the left fuel quantity"));
            AddFunction(new SegmentedMeter(this, "2066", 30, "Engine Management", "Fuel Quantity Right", "Bar display of the right fuel quantity"));
            AddFunction(new NetworkValue(this, "2060", "Engine Management", "Total Fuel Quantity", "Display of the total fuel amount.", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2067", 30, "Engine Management", "Transmission Temperature", "Bar display of the transmission temp in celsius"));
            AddFunction(new SegmentedMeter(this, "2068", 30, "Engine Management", "Transmission pressure", "Pressure in transmission in PSI"));
            AddFunction(new SegmentedMeter(this, "2069", 29, "Engine Management", "Engine 1 Oil Temperature", "Bar display of the oil temperature in celsius"));
            AddFunction(new SegmentedMeter(this, "2070", 29, "Engine Management", "Engine 2 Oil Temperature", "Bar display of the oil temperature in celsius"));
            AddFunction(new SegmentedMeter(this, "2071", 30, "Engine Management", "Engine 1 Oil Pressure", "Bar display of the oil pressure in PSI"));
            AddFunction(new SegmentedMeter(this, "2072", 30, "Engine Management", "Engine 2 Oil Pressure", "Bar display of the oil pressure in PSI"));
            AddFunction(new SegmentedMeter(this, "2073", 30, "Engine Management", "Engine 1 TGT", "Bar display of the Turbine Gas Temperature in celsius"));
            AddFunction(new NetworkValue(this, "2061", "Engine Management", "Engine 1 TGT Text", "Display of the Turbine Gas Temperature", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2074", 30, "Engine Management", "Engine 2 TGT", "Bar display of the Turbine Gas Temperature in celsius"));
            AddFunction(new NetworkValue(this, "2062", "Engine Management", "Engine 2 TGT Text", "Display of the Turbine Gas Temperature", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2075", 30, "Engine Management", "Engine 1 Ng", "Bar display of the Gas Generator Speed in RPM"));
            AddFunction(new NetworkValue(this, "2063", "Engine Management", "Engine 1 Ng Text", "Display of the Gas Generator Speed", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2076", 30, "Engine Management", "Engine 2 Ng", "Bar display of the Gas Generator Speed in RPM"));
            AddFunction(new NetworkValue(this, "2064", "Engine Management", "Engine 2 Ng Text", "Display of the Gas Generator Speed", "Text", BindingValueUnits.Text, null));

            AddFunction(new SegmentedMeter(this, "2079", 41, "Engine Management (Pilot)", "Engine 1 RPM", "Bar display of Engine 1 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2080", 41, "Engine Management (Pilot)", "Rotor RPM", "Bar display of Rotor RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2081", 41, "Engine Management (Pilot)", "Engine 2 RPM", "Bar display of Engine 2 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2082", 30, "Engine Management (Pilot)", "Engine 1 Torque", "Bar display of Engine 1 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2077", "Engine Management (Pilot)", "Engine 1 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2083", 30, "Engine Management (Pilot)", "Engine 2 Torque", "Bar display of Engine 2 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2078", "Engine Management (Pilot)", "Engine 2 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));

            AddFunction(new SegmentedMeter(this, "2086", 41, "Engine Management (Copilot)", "Engine 1 RPM", "Bar display of Engine 1 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2087", 41, "Engine Management (Copilot)", "Rotor RPM", "Bar display of Rotor RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2088", 41, "Engine Management (Copilot)", "Engine 2 RPM", "Bar display of Engine 2 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2089", 30, "Engine Management (Copilot)", "Engine 1 Torque", "Bar display of Engine 1 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2084", "Engine Management (Copilot)", "Engine 1 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2090", 30, "Engine Management (Copilot)", "Engine 2 Torque", "Bar display of Engine 2 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2085", "Engine Management (Copilot)", "Engine 2 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));



            #endregion

        }

        private SwitchPosition[] CreateSwitchPositions(int numPositions, double incrementalValue, string command)
        {
            SwitchPosition[] switchPositions = new SwitchPosition[numPositions];
            for(int i = 1; i<= numPositions; i++)
            {
                switchPositions[i - 1] = new SwitchPosition( ((i-1)*incrementalValue).ToString("0.##"), i.ToString(), command);
            }
            return switchPositions;
        }
    }
}
