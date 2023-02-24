//  Copyright 2020 Ammo Goettsch
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
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.H60.Functions;
    using GadrocsWorkshop.Helios.Gauges.H60;
    using static System.Net.Mime.MediaTypeNames;
    using System.Security.Policy;
    using static GadrocsWorkshop.Helios.Interfaces.DCS.H60.Functions.Altimeter;
    using System;

    //using GadrocsWorkshop.Helios.Controls;

    public class H60Interface : DCSInterface
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _dcsPath = $@"{Environment.GetEnvironmentVariable("userprofile")}\Saved Games\DCS World.openbeta\Mods\Aircraft";
        protected H60Interface(string heliosName, string dcsVehicleName, string exportFunctionsUri)
            : base(heliosName, dcsVehicleName, exportFunctionsUri)
        {

            // PILOT BARO ALTIMETER
            AddFunction(new Altimeter(this, FLYER.Pilot));
            AddFunction(new FlagValue(this, mainpanel.pilotBaroAltEncoderFlag.ToString("d"), "Indicators/Lamps/Flags", "PILOT BAROALT ENCODER FLAG", ""));

            // COPILOT ALTIMETER
            AddFunction(new Altimeter(this, FLYER.Copilot));
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
            AddFunction(new FlagValue(this, mainpanel.pduCpltOverspeed1.ToString("d"), "Indicators/Lamps/Flags", "PDU CPLT OVERSPEED1", ""));
            AddFunction(new FlagValue(this, mainpanel.pduCpltOverspeed2.ToString("d"), "Indicators/Lamps/Flags", "PDU CPLT OVERSPEED2", ""));
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
            AddFunction(new SegmentedMeter(this, "2068", 30, "Engine Management", "Transmission Pressure", "Pressure in transmission in PSI"));
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
