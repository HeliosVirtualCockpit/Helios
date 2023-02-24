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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.H60
{
    internal enum mainpanel
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

        apn209PilotAltNeedle = 173,
        apn209PilotAltDigit1 = 174,
        apn209PilotAltDigit2 = 175,
        apn209PilotAltDigit3 = 176,
        apn209PilotAltDigit4 = 177,
        apn209PilotLoBug = 178,
        apn209PilotHiBug = 179,
        apn209PilotLoLight = 180,
        apn209PilotHiLight = 181,
        apn209PilotFlag = 182,   // off
        apn209CopilotAltNeedle = 186,
        apn209CopilotAltDigit1 = 187,
        apn209CopilotAltDigit2 = 188,
        apn209CopilotAltDigit3 = 189,
        apn209CopilotAltDigit4 = 190,
        apn209CopilotLoBug = 191,
        apn209CopilotHiBug = 192,
        apn209CopilotLoLight = 193,
        apn209CopilotHiLight = 194,
        apn209CopilotFlag = 195,   // off

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

        StabInd = 3406,  // -1 to 1
        StabIndFlag = 3407,

    }
}
