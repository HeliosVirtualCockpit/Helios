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

//#define CREATEINTERFACE

namespace GadrocsWorkshop.Helios.Interfaces.DCS.F16C
{
    using System;
    using System.Windows.Media.Media3D;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.F16C.Functions;
    using GadrocsWorkshop.Helios.Interfaces.DCS.F16C.Tools;
    using GadrocsWorkshop.Helios.UDPInterface;
    [HeliosInterface(
    "Helios.F16C",                         // Helios internal type ID used in Profile XML, must never change
        "DCS F-16C Viper (Helios)",                    // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class F16CInterface : DCSInterface
    {
        public F16CInterface(string name)
            : base(name, "F-16C_50", "pack://application:,,,/Helios;component/Interfaces/DCS/F16C/ExportFunctions.lua")
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
            foreach (int lamp in Enum.GetValues(typeof(CautionLights))){
                AddFunction(new FlagValue(this, lamp.ToString(), "Caution Lights", Enum.GetName(typeof(CautionLights),lamp), ""));
            }
            foreach (int lamp in Enum.GetValues(typeof(RWRLights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "RWR Lights", Enum.GetName(typeof(RWRLights), lamp), ""));
            }
            foreach (int lamp in Enum.GetValues(typeof(CmdsLights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "CMDS Lights", Enum.GetName(typeof(CmdsLights), lamp), ""));
            }
            foreach (int lamp in Enum.GetValues(typeof(InternalLights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "Internal Lights", Enum.GetName(typeof(InternalLights), lamp), ""));
            }
            foreach (int lamp in Enum.GetValues(typeof(ECM_Button_Lights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "ECM Lights", Enum.GetName(typeof(ECM_Button_Lights), lamp), ""));
            }
            foreach (int lamp in Enum.GetValues(typeof(controllers)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "Controller Lights", Enum.GetName(typeof(controllers), lamp), ""));
            }
#if (CREATEINTERFACE && DEBUG)
            string DCSAircraft = $@"{Environment.GetEnvironmentVariable("ProgramFiles")}\Eagle Dynamics\DCS World.openbeta\Mods\Aircraft";
            DCSAircraft = $@"{Environment.GetEnvironmentVariable("userprofile")}\Desktop\DCSLua";
            InterfaceCreation ic = new InterfaceCreation();
            foreach (string path in new string[] { $@"{DCSAircraft}\F-16C\Cockpit\Scripts\clickabledata.lua" })
            {
                foreach (NetworkFunction nf in ic.CreateFunctionsFromClickable(this, path))
                {
                    AddFunction(nf);
                }
            }
            return;
#endif
            #region Control Interface
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "566", new SwitchPosition[] { new SwitchPosition("1.0", " OFF", F16CCommands.controlCommands.DigitalBackup.ToString("d")), new SwitchPosition("0.0", "BACKUP", F16CCommands.controlCommands.DigitalBackup.ToString("d")) }, "Control Interface", "DIGITAL BACKUP Switch, OFF/BACKUP", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "567", new SwitchPosition[] { new SwitchPosition("1.0", " NORM", F16CCommands.controlCommands.AltFlaps.ToString("d")), new SwitchPosition("0.0", "EXTEND", F16CCommands.controlCommands.AltFlaps.ToString("d")) }, "Control Interface", "ALT FLAPS Switch, NORM/EXTEND", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "574", new SwitchPosition[] { new SwitchPosition("1.0", " OFF", F16CCommands.controlCommands.BitSw.ToString("d")), new SwitchPosition("0.0", "BIT", F16CCommands.controlCommands.BitSw.ToString("d")) }, "Control Interface", "BIT Switch, OFF/BIT", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "573", new SwitchPosition[] { new SwitchPosition("1.0", " OFF", F16CCommands.controlCommands.FlcsReset.ToString("d")), new SwitchPosition("0.0", "RESET", F16CCommands.controlCommands.FlcsReset.ToString("d")) }, "Control Interface", "FLCS RESET Switch, OFF/RESET", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "572", new SwitchPosition[] { new SwitchPosition("1.0", " AUTO", F16CCommands.controlCommands.LeFlaps.ToString("d")), new SwitchPosition("0.0", "LOCK", F16CCommands.controlCommands.LeFlaps.ToString("d")) }, "Control Interface", "LE FLAPS Switch, AUTO/LOCK", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "564", new SwitchPosition[] { new SwitchPosition("1.0", " DISC", F16CCommands.controlCommands.TrimApDisc.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.controlCommands.TrimApDisc.ToString("d")) }, "Control Interface", "TRIM/AP DISC Switch, DISC/NORM", "%0.1f"));
            AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.RollTrim.ToString("d"), "560", 0.1d, 0.0d, 1.0d, "Control Interface", "ROLL TRIM Wheel", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.PitchTrim.ToString("d"), "562", 0.1d, 0.0d, 1.0d, "Control Interface", "PITCH TRIM Wheel", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.YawTrim.ToString("d"), "565", 0.1d, 0.0d, 1.0d, "Control Interface", "YAW TRIM Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "425", new SwitchPosition[] { new SwitchPosition("1.0", " OVRD", F16CCommands.controlCommands.ManualPitchOverride.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.controlCommands.ManualPitchOverride.ToString("d")) }, "Control Interface", "MANUAL PITCH Override Switch, OVRD/NORM", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "358", new SwitchPosition[] { new SwitchPosition("1.0", " CAT III", F16CCommands.controlCommands.StoresConfig.ToString("d")), new SwitchPosition("0.0", "CAT I", F16CCommands.controlCommands.StoresConfig.ToString("d")) }, "Control Interface", "STORES CONFIG Switch, CAT III/CAT I", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "109", new SwitchPosition[] { new SwitchPosition("1.0", " ATT HOLD", F16CCommands.controlCommands.ApPitchAtt.ToString("d"), F16CCommands.controlCommands.ApPitchAtt.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "A/P OFF", null), new SwitchPosition("-1.0", "ALT HOLD", F16CCommands.controlCommands.ApPitchAlt.ToString("d"), F16CCommands.controlCommands.ApPitchAlt.ToString("d"), "0.0", "0.0") }, "Control Interface", "Autopilot PITCH Switch, ATT HOLD/ A/P OFF/ ALT HOLD", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "108", new SwitchPosition[] { new SwitchPosition("-1.0", " STRG SEL", F16CCommands.controlCommands.ApRoll.ToString("d")), new SwitchPosition("0.0", "ATT HOLD", F16CCommands.controlCommands.ApRoll.ToString("d")), new SwitchPosition("1.0", "HDG SEL", F16CCommands.controlCommands.ApRoll.ToString("d")) }, "Control Interface", "Autopilot ROLL Switch, STRG SEL/ATT HOLD/HDG SEL", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "97", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.controlCommands.AdvMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.controlCommands.AdvMode.ToString("d")) }, "Control Interface", "ADV MODE Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "568", new SwitchPosition[] { new SwitchPosition("1.0", " ENABLE", F16CCommands.controlCommands.ManualTfFlyup.ToString("d")), new SwitchPosition("0.0", "DISABLE", F16CCommands.controlCommands.ManualTfFlyup.ToString("d")) }, "Control Interface", "MANUAL TF FLYUP Switch, ENABLE/DISABLE", "%0.1f"));
            #endregion Control Interface
            #region External Lights
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "531", SwitchPositions.Create(8, 0d, 0.1d, F16CCommands.extlightsCommands.AntiCollKn.ToString("d"), new string[] { "OFF", "1", "2", "3", "4", "A", "B", "C" }, "%0.1f"), "External Lights", "ANTI-COLL Knob, OFF/1/2/3/4/A/B/C", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "532", new SwitchPosition[] { new SwitchPosition("1.0", " FLASH", F16CCommands.extlightsCommands.PosFlash.ToString("d")), new SwitchPosition("0.0", "STEADY", F16CCommands.extlightsCommands.PosFlash.ToString("d")) }, "External Lights", "FLASH STEADY Switch, FLASH/STEADY", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "533", new SwitchPosition[] { new SwitchPosition("-1.0", " BRT", F16CCommands.extlightsCommands.PosWingTail.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.extlightsCommands.PosWingTail.ToString("d")), new SwitchPosition("1.0", "DIM", F16CCommands.extlightsCommands.PosWingTail.ToString("d")) }, "External Lights", "WING/TAIL Switch, BRT/OFF/DIM", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "534", new SwitchPosition[] { new SwitchPosition("-1.0", " BRT", F16CCommands.extlightsCommands.PosFus.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.extlightsCommands.PosFus.ToString("d")), new SwitchPosition("1.0", "DIM", F16CCommands.extlightsCommands.PosFus.ToString("d")) }, "External Lights", "FUSELAGE Switch, BRT/OFF/DIM", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), F16CCommands.extlightsCommands.FormKn.ToString("d"), "535", 0.1d, 0.0d, 1.0d, "External Lights", "FORM Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "536", SwitchPositions.Create(5, 0d, 0.1d, F16CCommands.extlightsCommands.Master.ToString("d"), new string[] { "OFF", "ALL", "A-C", "FORM", "NORM" }, "%0.1f"), "External Lights", "MASTER Switch, OFF/ALL/A-C/FORM/NORM", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), F16CCommands.extlightsCommands.AerialRefuel.ToString("d"), "537", 0.1d, 0.0d, 1.0d, "External Lights", "AERIAL REFUELING Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "360", new SwitchPosition[] { new SwitchPosition("-1.0", " LANDING", F16CCommands.extlightsCommands.LandingTaxi.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.extlightsCommands.LandingTaxi.ToString("d")), new SwitchPosition("1.0", "TAXI", F16CCommands.extlightsCommands.LandingTaxi.ToString("d")) }, "External Lights", "LANDING TAXI LIGHTS Switch, LANDING/OFF/TAXI", "%0.1f"));
            #endregion External Lights
            #region Interior Lights
            AddFunction(new PushButton(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.MasterCaution.ToString("d"), "116", "Interior Lights", "Master Caution Button - Push to reset", "%1d"));
            AddFunction(new PushButton(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsTest.ToString("d"), "577", "Interior Lights", "MAL & IND LTS Test Button - Push to test", "%1d"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.Consoles.ToString("d"), "685", 0.1d, 0.0d, 1.0d, "Interior Lights", "PRIMARY CONSOLES BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.IntsPnl.ToString("d"), "686", 0.1d, 0.0d, 1.0d, "Interior Lights", "PRIMARY INST PNL BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.DataEntryDisplay.ToString("d"), "687", 0.1d, 0.0d, 1.0d, "Interior Lights", "PRIMARY DATA ENTRY DISPLAY BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.ConsolesFlood.ToString("d"), "688", 0.1d, 0.0d, 1.0d, "Interior Lights", "FLOOD CONSOLES BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.InstPnlFlood.ToString("d"), "690", 0.1d, 0.0d, 1.0d, "Interior Lights", "FLOOD INST PNL BRT Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), "691", new SwitchPosition[] { new SwitchPosition("1.0", " BRT", F16CCommands.cptlightsCommands.MalIndLtsDim.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsDim.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Center", null), new SwitchPosition("-1.0", "DIM", F16CCommands.cptlightsCommands.MalIndLtsBrt.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsBrt.ToString("d"), "0.0", "0.0") }, "Interior Lights", "MAL & IND LTS Switch, BRT/Center/DIM", "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.IndBrtAoA.ToString("d"), "794", 0.1d, 0.0d, 1.0d, "Interior Lights", "AOA Indexer Dimming Lever", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.IndBrtAR.ToString("d"), "795", 0.1d, 0.0d, 1.0d, "Interior Lights", "AR Status Indicator Dimming Lever", false, "%0.1f"));
            #endregion Interior Lights
            #region Electric System
            AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString("d"), "510", new SwitchPosition[] { new SwitchPosition("-1.0", " MAIN PWR", F16CCommands.elecCommands.MainPwrSw.ToString("d")), new SwitchPosition("0.0", "BATT", F16CCommands.elecCommands.MainPwrSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.elecCommands.MainPwrSw.ToString("d")) }, "Electric System", "MAIN PWR Switch, MAIN PWR/BATT/OFF", "%0.1f"));
            AddFunction(new PushButton(this, devices.ELEC_INTERFACE.ToString("d"), F16CCommands.elecCommands.CautionResetBtn.ToString("d"), "511", "Electric System", "ELEC CAUTION RESET Button - Push to reset", "%1d"));
            AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString("d"), "579", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.elecCommands.EPU_GEN_TestSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.elecCommands.EPU_GEN_TestSw.ToString("d")) }, "Electric System", "EPU/GEN Test Switch, EPU/GEN /OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString("d"), "578", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.elecCommands.ProbeHeatSwTEST.ToString("d"), F16CCommands.elecCommands.ProbeHeatSwTEST.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.elecCommands.ProbeHeatSw.ToString("d"), F16CCommands.elecCommands.ProbeHeatSw.ToString("d"), "0.0", "0.0") }, "Electric System", "PROBE HEAT Switch, PROBE HEAT/OFF/TEST(momentarily)", "%0.1f"));
            AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString("d"), "585", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), "0.0"), new SwitchPosition("-1.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), "0.0") }, "Electric System", "FLCS PWR TEST Switch, MAINT/NORM/TEST(momentarily)", "%0.1f"));
            #endregion Electric System
            #region Fuel System
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "559", new SwitchPosition[] { new SwitchPosition("1.0", " MASTER", F16CCommands.fuelCommands.FuelMasterSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.fuelCommands.FuelMasterSw.ToString("d")) }, "Fuel System", "FUEL MASTER Switch, MASTER/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "558", new SwitchPosition[] { new SwitchPosition("0.0", " OPEN", F16CCommands.fuelCommands.FuelMasterSwCvr.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.fuelCommands.FuelMasterSwCvr.ToString("d")) }, "Fuel System", "FUEL MASTER Switch Cover, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "557", new SwitchPosition[] { new SwitchPosition("1.0", " TANK INERTING ", F16CCommands.fuelCommands.TankInertingSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.fuelCommands.TankInertingSw.ToString("d")) }, "Fuel System", "TANK INERTING Switch, TANK INERTING /OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "556", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.fuelCommands.EngineFeedSw.ToString("d"), new string[] { "OFF", "NORM", "AFT", "FWD" }, "%0.1f"), "Fuel System", "ENGINE FEED Knob, OFF/NORM/AFT/FWD", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "555", new SwitchPosition[] { new SwitchPosition("1.0", " OPEN", F16CCommands.fuelCommands.AirRefuelSw.ToString("d")), new SwitchPosition("0.0", "CLOSE", F16CCommands.fuelCommands.AirRefuelSw.ToString("d")) }, "Fuel System", "AIR REFUEL Switch, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "159", new SwitchPosition[] { new SwitchPosition("1.0", " NORM", F16CCommands.fuelCommands.ExtFuelTransferSw.ToString("d")), new SwitchPosition("0.0", "WING FIRST", F16CCommands.fuelCommands.ExtFuelTransferSw.ToString("d")) }, "Fuel System", "External Fuel Transfer Switch, NORM/ WING FIRST", "%0.1f"));
            #endregion Fuel System
            #region Gear System
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "362", new SwitchPosition[] { new SwitchPosition("1.0", " UP", F16CCommands.gearCommands.LGHandle.ToString("d")), new SwitchPosition("0.0", "DN", F16CCommands.gearCommands.LGHandle.ToString("d")) }, "Gear System", "LG Handle, UP/DN", "%0.1f"));
            AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.DownLockRelBtn.ToString("d"), "361", "Gear System", "DN LOCK REL Button - Push to reset", "%1d"));
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "354", new SwitchPosition[] { new SwitchPosition("1.0", " UP", F16CCommands.gearCommands.HookSw.ToString("d")), new SwitchPosition("0.0", "DN", F16CCommands.gearCommands.HookSw.ToString("d")) }, "Gear System", "HOOK Switch, UP/DN", "%0.1f"));
            AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.HornSilencerBtn.ToString("d"), "359", "Gear System", "HORN SILENCER Button - Push to reset", "%1d"));
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "356", new SwitchPosition[] { new SwitchPosition("1.0", " CHAN 1", F16CCommands.gearCommands.BrakesChannelSw.ToString("d")), new SwitchPosition("0.0", "CHAN 2", F16CCommands.gearCommands.BrakesChannelSw.ToString("d")) }, "Gear System", "BRAKES Channel Switch, CHAN 1/CHAN 2", "%0.1f"));
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "357", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.gearCommands.AntiSkidSw.ToString("d"), F16CCommands.gearCommands.AntiSkidSw.ToString("d"), "0.0"), new SwitchPosition("-1.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", F16CCommands.gearCommands.AntiSkidSw.ToString("d"), F16CCommands.gearCommands.AntiSkidSw.ToString("d"), "0.0") }, "Gear System", "ANTI-SKID Switch, PARKING BRAKE/ANTI-SKID/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "380", new SwitchPosition[] { new SwitchPosition("1.0", " PULL", F16CCommands.gearCommands.AltGearHandle.ToString("d")), new SwitchPosition("0.0", "STOW", F16CCommands.gearCommands.AltGearHandle.ToString("d")) }, "Gear System", "ALT GEAR Handle, PULL/STOW", "%0.1f"));
            AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.AltGearResetBtn.ToString("d"), "381", "Gear System", "ALT GEAR Handle - Push to reset", "%1d"));
            #endregion Gear System
            #region ECS
            AddFunction(new Axis(this, devices.ECS_INTERFACE.ToString("d"), F16CCommands.ecsCommands.TempKnob.ToString("d"), "692", 0.1d, 0.0d, 1.0d, "ECS", "TEMP Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ECS_INTERFACE.ToString("d"), "693", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.ecsCommands.AirSourceKnob.ToString("d"), new string[] { "OFF", "NORM", "DUMP", "RAM" }, "%0.1f"), "ECS", "AIR SOURCE Knob, OFF/NORM/DUMP/RAM", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS_INTERFACE.ToString("d"), "602", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecsCommands.DefogLever.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecsCommands.DefogLever.ToString("d")) }, "ECS", "DEFOG Lever", "%0.1f"));
            #endregion ECS
            #region EPU
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "527", new SwitchPosition[] { new SwitchPosition("0.0", " OPEN", F16CCommands.engineCommands.EpuSwCvrOn.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.engineCommands.EpuSwCvrOn.ToString("d")) }, "EPU", "EPU Switch Cover for ON, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "529", new SwitchPosition[] { new SwitchPosition("0.0", " OPEN", F16CCommands.engineCommands.EpuSwCvrOff.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.engineCommands.EpuSwCvrOff.ToString("d")) }, "EPU", "EPU Switch Cover for OFF, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "528", new SwitchPosition[] { new SwitchPosition("-1.0", " ON", F16CCommands.engineCommands.EpuSw.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.engineCommands.EpuSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.engineCommands.EpuSw.ToString("d")) }, "EPU", "EPU Switch, ON/NORM/OFF", "%0.1f"));
            #endregion EPU
            #region engine
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "710", new SwitchPosition[] { new SwitchPosition("-1.0", " ON", F16CCommands.engineCommands.EngAntiIceSw.ToString("d")), new SwitchPosition("0.0", "AUTO", F16CCommands.engineCommands.EngAntiIceSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.engineCommands.EngAntiIceSw.ToString("d")) }, "engine", "Engine ANTI ICE Switch, ON/AUTO/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "447", new SwitchPosition[] { new SwitchPosition("1.0", " START 1", F16CCommands.engineCommands.JfsSwStart2.ToString("d"), F16CCommands.engineCommands.JfsSwStart2.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "OFF", null), new SwitchPosition("-1.0", "START 2", F16CCommands.engineCommands.JfsSwStart1.ToString("d"), F16CCommands.engineCommands.JfsSwStart1.ToString("d"), "0.0", "0.0") }, "engine", "JFS Switch, START 1/OFF/START 2", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "448", new SwitchPosition[] { new SwitchPosition("0.0", " OPEN", F16CCommands.engineCommands.EngContSwCvr.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.engineCommands.EngContSwCvr.ToString("d")) }, "engine", "ENG CONT Switch Cover, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "449", new SwitchPosition[] { new SwitchPosition("1.0", " PRI", F16CCommands.engineCommands.EngContSw.ToString("d")), new SwitchPosition("0.0", "SEC", F16CCommands.engineCommands.EngContSw.ToString("d")) }, "engine", "ENG CONT Switch, PRI/SEC", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "451", new SwitchPosition[] { new SwitchPosition("1.0", " MAX POWER", F16CCommands.engineCommands.MaxPowerSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.engineCommands.MaxPowerSw.ToString("d")) }, "engine", "MAX POWER Switch (is inoperative), MAX POWER/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "450", new SwitchPosition[] { new SwitchPosition("1.0", " AB RESET", F16CCommands.engineCommands.ABResetSwEngData.ToString("d"), F16CCommands.engineCommands.ABResetSwEngData.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "NORM", null), new SwitchPosition("-1.0", "ENG DATA", F16CCommands.engineCommands.ABResetSwReset.ToString("d"), F16CCommands.engineCommands.ABResetSwReset.ToString("d"), "0.0", "0.0") }, "engine", "AB RESET Switch, AB RESET/NORM/ENG DATA", "%0.1f"));
            AddFunction(new PushButton(this, devices.ENGINE_INTERFACE.ToString("d"), F16CCommands.engineCommands.FireOheatTestBtn.ToString("d"), "575", "engine", "FIRE & OHEAT DETECT Test Button - Push to test", "%1d"));
            #endregion engine
            #region Oxygen System
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "728", new SwitchPosition[] { new SwitchPosition("-1.0", " PBG", F16CCommands.oxygenCommands.SupplyLever.ToString("d")), new SwitchPosition("0.0", "ON", F16CCommands.oxygenCommands.SupplyLever.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.oxygenCommands.SupplyLever.ToString("d")) }, "Oxygen System", "Supply Lever, PBG/ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "727", new SwitchPosition[] { new SwitchPosition("1.0", " 100 percent", F16CCommands.oxygenCommands.DiluterLever.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.oxygenCommands.DiluterLever.ToString("d")) }, "Oxygen System", "Diluter Lever, 100 percent/NORM", "%0.1f"));
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "726", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.oxygenCommands.EmergencyLeverTestMask.ToString("d"), F16CCommands.oxygenCommands.EmergencyLeverTestMask.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.oxygenCommands.EmergencyLever.ToString("d"), F16CCommands.oxygenCommands.EmergencyLever.ToString("d"), "0.0", "0.0") }, "Oxygen System", "Emergency Lever, EMERGENCY/NORMAL/TEST MASK(momentarily)", "%0.1f"));
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "576", new SwitchPosition[] { new SwitchPosition("1.0", " BIT", F16CCommands.oxygenCommands.ObogsBitSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.oxygenCommands.ObogsBitSw.ToString("d")) }, "Oxygen System", "OBOGS BIT Switch, BIT/OFF", "%0.1f"));
            #endregion Oxygen System
            #region Sensor Power Control Panel
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "670", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.smsCommands.LeftHDPT.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.LeftHDPT.ToString("d")) }, "Sensor Power Control Panel", "LEFT HDPT Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "671", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.smsCommands.RightHDPT.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.RightHDPT.ToString("d")) }, "Sensor Power Control Panel", "RIGHT HDPT Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.FCR.ToString("d"), "672", new SwitchPosition[] { new SwitchPosition("1.0", " FCR", F16CCommands.fcrCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.fcrCommands.PwrSw.ToString("d")) }, "Sensor Power Control Panel", "FCR Switch, FCR/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.RALT.ToString("d"), "673", new SwitchPosition[] { new SwitchPosition("-1.0", " RDR ALT", F16CCommands.raltCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "STBY", F16CCommands.raltCommands.PwrSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.raltCommands.PwrSw.ToString("d")) }, "Sensor Power Control Panel", "RDR ALT Switch, RDR ALT/STBY/OFF", "%0.1f"));
            #endregion Sensor Power Control Panel
            #region Avionic Power panel
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "715", new SwitchPosition[] { new SwitchPosition("1.0", " MMC", F16CCommands.mmcCommands.MmcPwr.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.MmcPwr.ToString("d")) }, "Avionic Power panel", "MMC Switch, MMC/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "716", new SwitchPosition[] { new SwitchPosition("1.0", " ST STA", F16CCommands.smsCommands.StStaSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.StStaSw.ToString("d")) }, "Avionic Power panel", "ST STA Switch, ST STA/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "717", new SwitchPosition[] { new SwitchPosition("1.0", " MFD", F16CCommands.mmcCommands.MFD.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.MFD.ToString("d")) }, "Avionic Power panel", "MFD Switch, MFD/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "718", new SwitchPosition[] { new SwitchPosition("1.0", " UFC", F16CCommands.ufcCommands.UFC_Sw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.ufcCommands.UFC_Sw.ToString("d")) }, "Avionic Power panel", "UFC Switch, UFC/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.GPS.ToString("d"), "720", new SwitchPosition[] { new SwitchPosition("1.0", " GPS", F16CCommands.gpsCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.gpsCommands.PwrSw.ToString("d")) }, "Avionic Power panel", "GPS Switch, GPS/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MIDS.ToString("d"), "723", SwitchPositions.Create(3, 0d, 0.1d, F16CCommands.midsCommands.PwrSw.ToString("d"), new string[] { "ZERO", "OFF", "ON" }, "%0.1f"), "Avionic Power panel", "MIDS LVT Knob, ZERO/OFF/ON", "%0.1f"));
            AddFunction(new Switch(this, devices.INS.ToString("d"), "719", SwitchPositions.Create(7, 0d, 0.1d, F16CCommands.insCommands.ModeKnob.ToString("d"), new string[] { "OFF", "STOR HDG", "NORM", "NAV", "CAL", "INFLT ALIGN", "ATT" }, "%0.1f"), "Avionic Power panel", "INS Knob, OFF/STOR HDG/NORM/NAV/CAL/INFLT ALIGN/ATT", "%0.1f"));
            AddFunction(new Switch(this, devices.MAP.ToString("d"), "722", new SwitchPosition[] { new SwitchPosition("1.0", " MAP", F16CCommands.mapCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mapCommands.PwrSw.ToString("d")) }, "Avionic Power panel", "MAP Switch, MAP/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.IDM.ToString("d"), "721", new SwitchPosition[] { new SwitchPosition("1.0", " DL", F16CCommands.idmCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.idmCommands.PwrSw.ToString("d")) }, "Avionic Power panel", "DL Switch, DL/OFF", "%0.1f"));
            #endregion Avionic Power panel
            #region Modular Mission Computer (MMC)
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "105", new SwitchPosition[] { new SwitchPosition("-1.0", " MASTER ARM", F16CCommands.mmcCommands.MasterArmSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.MasterArmSw.ToString("d")), new SwitchPosition("1.0", "SIMULATE", F16CCommands.mmcCommands.MasterArmSw.ToString("d")) }, "Modular Mission Computer (MMC)", "MASTER ARM Switch, MASTER ARM/OFF/SIMULATE", "%0.1f"));
            AddFunction(new PushButton(this, devices.MMC.ToString("d"), F16CCommands.mmcCommands.EmerStoresJett.ToString("d"), "353", "Modular Mission Computer (MMC)", "EMER STORES JETTISON Button - Push to jettison", "%1d"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "355", new SwitchPosition[] { new SwitchPosition("1.0", " ENABLE", F16CCommands.mmcCommands.GroundJett.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.GroundJett.ToString("d")) }, "Modular Mission Computer (MMC)", "GND JETT ENABLE Switch, ENABLE/OFF", "%0.1f"));
            AddFunction(new PushButton(this, devices.MMC.ToString("d"), F16CCommands.mmcCommands.AltRel.ToString("d"), "104", "Modular Mission Computer (MMC)", "ALT REL Button - Push to release", "%1d"));
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "103", new SwitchPosition[] { new SwitchPosition("1.0", " ARM", F16CCommands.smsCommands.LaserSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.LaserSw.ToString("d")) }, "Modular Mission Computer (MMC)", "LASER ARM Switch, ARM/OFF", "%0.1f"));
            #endregion Modular Mission Computer (MMC)
            #region Integrated Control Panel (ICP) of Upfront Controls (UFC)
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG1_T_ILS.ToString("d"), "171", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 1(T-ILS)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG2_ALOW.ToString("d"), "172", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 2/N(ALOW)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG3.ToString("d"), "173", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 3", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG4_STPT.ToString("d"), "175", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 4/W(STPT)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG5_CRUS.ToString("d"), "176", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 5(CRUS)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG6_TIME.ToString("d"), "177", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 6/E(TIME)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG7_MARK.ToString("d"), "179", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 7(MARK)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG8_FIX.ToString("d"), "180", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 8/S(FIX)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG9_A_CAL.ToString("d"), "181", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 9(A-CAL)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DIG0_M_SEL.ToString("d"), "182", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Priority Function Button, 0(M-SEL)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.COM1.ToString("d"), "165", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP COM Override Button, COM1(UHF)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.COM2.ToString("d"), "166", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP COM Override Button, COM2(VHF)", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.IFF.ToString("d"), "167", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP IFF Override Button, IFF", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.LIST.ToString("d"), "168", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP LIST Override Button, LIST", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.AA.ToString("d"), "169", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Master Mode Button, A-A", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.AG.ToString("d"), "170", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Master Mode Button, A-G", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.RCL.ToString("d"), "174", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Recall Button, RCL", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.ENTR.ToString("d"), "178", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Enter Button, ENTR", "%1d"));
            AddFunction(new Axis(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.RET_DEPR_Knob.ToString("d"), "192", 0.1d, 0.0d, 1.0d, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Reticle Depression Control Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.CONT_Knob.ToString("d"), "193", 0.1d, 0.0d, 1.0d, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Raster Contrast Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.BRT_Knob.ToString("d"), "191", 0.1d, 0.0d, 1.0d, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Raster Intensity Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.SYM_Knob.ToString("d"), "190", 0.1d, 0.0d, 1.0d, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP HUD Symbology Intensity Knob", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.Wx.ToString("d"), "187", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP FLIR Polarity Button, Wx", "%1d"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "189", new SwitchPosition[] { new SwitchPosition("-1.0", " GAIN", F16CCommands.ufcCommands.FLIR_GAIN_Sw.ToString("d")), new SwitchPosition("0.0", "LVL", F16CCommands.ufcCommands.FLIR_GAIN_Sw.ToString("d")), new SwitchPosition("1.0", "AUTO", F16CCommands.ufcCommands.FLIR_GAIN_Sw.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP FLIR GAIN/LEVEL Switch, GAIN/LVL/AUTO", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "183", new SwitchPosition[] { new SwitchPosition("1.0", "Up", F16CCommands.ufcCommands.DED_INC.ToString("d"), F16CCommands.ufcCommands.DED_INC.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", F16CCommands.ufcCommands.DED_DEC.ToString("d"), F16CCommands.ufcCommands.DED_DEC.ToString("d"), "0.0", "0.0") }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP DED Increment/Decrement Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "188", new SwitchPosition[] { new SwitchPosition("1.0", "Up", F16CCommands.ufcCommands.FLIR_INC.ToString("d"), F16CCommands.ufcCommands.FLIR_INC.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down", F16CCommands.ufcCommands.FLIR_DEC.ToString("d"), F16CCommands.ufcCommands.FLIR_DEC.ToString("d"), "0.0", "0.0") }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP FLIR Increment/Decrement Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "186", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ufcCommands.WARN_RESET.ToString("d"), F16CCommands.ufcCommands.WARN_RESET.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.ufcCommands.DRIFT_CUTOUT.ToString("d"), F16CCommands.ufcCommands.DRIFT_CUTOUT.ToString("d"), "0.0", "0.0") }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP DRIFT CUTOUT/WARN RESET Switch, DRIFT C/O /NORM/WARN RESET", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "184", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ufcCommands.DCS_RTN.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ufcCommands.DCS_RTN.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Data Control Switch, RTN", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "184", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ufcCommands.DCS_SEQ.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ufcCommands.DCS_SEQ.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Data Control Switch, SEQ", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "185", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ufcCommands.DCS_UP.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ufcCommands.DCS_UP.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Data Control Switch, UP", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "185", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ufcCommands.DCS_DOWN.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ufcCommands.DCS_DOWN.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Data Control Switch, DN", "%0.1f"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.F_ACK.ToString("d"), "122", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "F-ACK Button", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.IFF_IDENT.ToString("d"), "125", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "IFF IDENT Button", "%1d"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "100", new SwitchPosition[] { new SwitchPosition("-1.0", " SILENT", F16CCommands.ufcCommands.RF_Sw.ToString("d")), new SwitchPosition("0.0", "QUIET", F16CCommands.ufcCommands.RF_Sw.ToString("d")), new SwitchPosition("1.0", "NORM", F16CCommands.ufcCommands.RF_Sw.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "RF Switch, SILENT/QUIET/NORM", "%0.1f"));
            #endregion Integrated Control Panel (ICP) of Upfront Controls (UFC)
            #region HUD Remote Control Panel
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "675", new SwitchPosition[] { new SwitchPosition("-1.0", " VV/VAH ", F16CCommands.mmcCommands.VvVah.ToString("d")), new SwitchPosition("0.0", "VAH ", F16CCommands.mmcCommands.VvVah.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.VvVah.ToString("d")) }, "HUD Remote Control Panel", "HUD Scales Switch, VV/VAH / VAH / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "676", new SwitchPosition[] { new SwitchPosition("-1.0", " ATT/FPM ", F16CCommands.mmcCommands.AttFpm.ToString("d")), new SwitchPosition("0.0", "FPM ", F16CCommands.mmcCommands.AttFpm.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.AttFpm.ToString("d")) }, "HUD Remote Control Panel", "HUD Flightpath Marker Switch, ATT/FPM / FPM / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "677", new SwitchPosition[] { new SwitchPosition("-1.0", " DED ", F16CCommands.mmcCommands.DedData.ToString("d")), new SwitchPosition("0.0", "PFL ", F16CCommands.mmcCommands.DedData.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.DedData.ToString("d")) }, "HUD Remote Control Panel", "HUD DED/PFLD Data Switch, DED / PFL / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "678", new SwitchPosition[] { new SwitchPosition("-1.0", " STBY ", F16CCommands.mmcCommands.DeprRet.ToString("d")), new SwitchPosition("0.0", "PRI ", F16CCommands.mmcCommands.DeprRet.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.DeprRet.ToString("d")) }, "HUD Remote Control Panel", "HUD Depressible Reticle Switch, STBY / PRI / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "679", new SwitchPosition[] { new SwitchPosition("-1.0", " CAS ", F16CCommands.mmcCommands.Spd.ToString("d")), new SwitchPosition("0.0", "TAS ", F16CCommands.mmcCommands.Spd.ToString("d")), new SwitchPosition("1.0", "GND SPD", F16CCommands.mmcCommands.Spd.ToString("d")) }, "HUD Remote Control Panel", "HUD Velocity Switch, CAS / TAS / GND SPD", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "680", new SwitchPosition[] { new SwitchPosition("-1.0", " RADAR ", F16CCommands.mmcCommands.Alt.ToString("d")), new SwitchPosition("0.0", "BARO ", F16CCommands.mmcCommands.Alt.ToString("d")), new SwitchPosition("1.0", "AUTO", F16CCommands.mmcCommands.Alt.ToString("d")) }, "HUD Remote Control Panel", "HUD Altitude Switch, RADAR / BARO / AUTO", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "681", new SwitchPosition[] { new SwitchPosition("-1.0", " DAY ", F16CCommands.mmcCommands.Brt.ToString("d")), new SwitchPosition("0.0", "AUTO BRT ", F16CCommands.mmcCommands.Brt.ToString("d")), new SwitchPosition("1.0", "NIGHT", F16CCommands.mmcCommands.Brt.ToString("d")) }, "HUD Remote Control Panel", "HUD Brightness Control Switch, DAY / AUTO BRT / NIGHT", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "682", new SwitchPosition[] { new SwitchPosition("-1.0", " STEP ", F16CCommands.mmcCommands.Test.ToString("d")), new SwitchPosition("0.0", "ON ", F16CCommands.mmcCommands.Test.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.Test.ToString("d")) }, "HUD Remote Control Panel", "HUD TEST Switch, STEP / ON / OFF", "%0.1f"));
            #endregion HUD Remote Control Panel
            #region Audio Control Panels
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "434", SwitchPositions.Create(3, 0d, 0.5d, F16CCommands.intercomCommands.COM1_ModeKnob.ToString("d"), "Posn", "%0.1f"), "Audio Control Panels", "COMM 1 (UHF) Mode Knob", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "435", SwitchPositions.Create(3, 0d, 0.5d, F16CCommands.intercomCommands.COM2_ModeKnob.ToString("d"), "Posn", "%0.1f"), "Audio Control Panels", "COMM 2 (VHF) Mode Knob", "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.COM1_PowerKnob.ToString("d"), "430", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "COMM 1 Power Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.COM2_PowerKnob.ToString("d"), "431", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "COMM 2 Power Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.SecureVoiceKnob.ToString("d"), "432", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "SECURE VOICE Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.MSL_ToneKnob.ToString("d"), "433", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "MSL Tone Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.TF_ToneKnob.ToString("d"), "436", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "TF Tone Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.THREAT_ToneKnob.ToString("d"), "437", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "THREAT Tone Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.INTERCOM_Knob.ToString("d"), "440", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "INTERCOM Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.TACAN_Knob.ToString("d"), "441", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "TACAN Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTERCOM.ToString("d"), F16CCommands.intercomCommands.ILS_PowerKnob.ToString("d"), "442", 0.1d, 0.0d, 1.0d, "Audio Control Panels", "ILS Power Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "443", new SwitchPosition[] { new SwitchPosition("-1.0", " HOT MIC ", F16CCommands.intercomCommands.HotMicCipherSw.ToString("d")), new SwitchPosition("0.0", "OFF ", F16CCommands.intercomCommands.HotMicCipherSw.ToString("d")), new SwitchPosition("1.0", "CIPHER", F16CCommands.intercomCommands.HotMicCipherSw.ToString("d")) }, "Audio Control Panels", "HOT MIC CIPHER Switch, HOT MIC / OFF / CIPHER", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "696", new SwitchPosition[] { new SwitchPosition("1.0", " VOICE MESSAGE", F16CCommands.intercomCommands.VMS_InhibitSw.ToString("d")), new SwitchPosition("0.0", "INHIBIT", F16CCommands.intercomCommands.VMS_InhibitSw.ToString("d")) }, "Audio Control Panels", "Voice Message Inhibit Switch, VOICE MESSAGE/INHIBIT", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "711", new SwitchPosition[] { new SwitchPosition("-1.0", " LOWER", F16CCommands.intercomCommands.IFF_AntSelSw.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.intercomCommands.IFF_AntSelSw.ToString("d")), new SwitchPosition("1.0", "UPPER", F16CCommands.intercomCommands.IFF_AntSelSw.ToString("d")) }, "Audio Control Panels", "IFF ANT SEL Switch, LOWER/NORM/UPPER", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "712", new SwitchPosition[] { new SwitchPosition("-1.0", " LOWER", F16CCommands.intercomCommands.UHF_AntSelSw.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.intercomCommands.UHF_AntSelSw.ToString("d")), new SwitchPosition("1.0", "UPPER", F16CCommands.intercomCommands.UHF_AntSelSw.ToString("d")) }, "Audio Control Panels", "UHF ANT SEL Switch, LOWER/NORM/UPPER", "%0.1f"));
            #endregion Audio Control Panels
            #region UHF Backup Control Panel
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "410", SwitchPositions.Create(20, 0d, 0.05d, F16CCommands.uhfCommands.ChannelKnob.ToString("d"), "Posn", "%0.2f"), "UHF Backup Control Panel", "UHF CHAN Knob", "%0.2f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "411", SwitchPositions.Create(3, 0.1d, 0.1d, F16CCommands.uhfCommands.FreqSelector100Mhz.ToString("d"), "Posn", "%0.1f"), "UHF Backup Control Panel", "UHF Manual Frequency Knob 100 MHz", "%0.1f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "412", SwitchPositions.Create(10, 0d, 0.1d, F16CCommands.uhfCommands.FreqSelector10Mhz.ToString("d"), "Posn", "%0.1f"), "UHF Backup Control Panel", "UHF Manual Frequency Knob 10 MHz", "%0.1f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "413", SwitchPositions.Create(10, 0d, 0.1d, F16CCommands.uhfCommands.FreqSelector1Mhz.ToString("d"), "Posn", "%0.1f"), "UHF Backup Control Panel", "UHF Manual Frequency Knob 1 MHz", "%0.1f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "414", SwitchPositions.Create(10, 0d, 0.1d, F16CCommands.uhfCommands.FreqSelector01Mhz.ToString("d"), "Posn", "%0.1f"), "UHF Backup Control Panel", "UHF Manual Frequency Knob 0.1 MHz", "%0.1f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "415", SwitchPositions.Create(4, 0d, 0.25d, F16CCommands.uhfCommands.FreqSelector0025Mhz.ToString("d"), "Posn", "%0.2f"), "UHF Backup Control Panel", "UHF Manual Frequency Knob 0.025 MHz", "%0.2f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "417", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.uhfCommands.FunctionKnob.ToString("d"), "Posn", "%0.1f"), "UHF Backup Control Panel", "UHF Function Knob", "%0.1f"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "416", SwitchPositions.Create(3, 0d, 0.1d, F16CCommands.uhfCommands.FreqModeKnob.ToString("d"), "Posn", "%0.1f"), "UHF Backup Control Panel", "UHF Mode Knob", "%0.1f"));
            AddFunction(new PushButton(this, devices.UHF_CONTROL_PANEL.ToString("d"), F16CCommands.uhfCommands.TToneSw.ToString("d"), "418", "UHF Backup Control Panel", "UHF Tone Button", "%1d"));
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "419", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.uhfCommands.SquelchSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.uhfCommands.SquelchSw.ToString("d")) }, "UHF Backup Control Panel", "UHF SQUELCH Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.UHF_CONTROL_PANEL.ToString("d"), F16CCommands.uhfCommands.VolumeKnob.ToString("d"), "420", 0.1d, 0.0d, 1.0d, "UHF Backup Control Panel", "UHF VOL Knob", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.UHF_CONTROL_PANEL.ToString("d"), F16CCommands.uhfCommands.TestDisplayBtn.ToString("d"), "421", "UHF Backup Control Panel", "UHF TEST DISPLAY Button", "%1d"));
            AddFunction(new PushButton(this, devices.UHF_CONTROL_PANEL.ToString("d"), F16CCommands.uhfCommands.StatusBtn.ToString("d"), "422", "UHF Backup Control Panel", "UHF STATUS Button", "%1d"));
            #endregion UHF Backup Control Panel
            #region IFF Control Panel
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "542", SwitchPositions.Create(2, 0d, 1d, F16CCommands.iffCommands.CNI_Knob.ToString("d"), new string[] { "UFC", "BACKUP" }, "%0.-1f"), "IFF Control Panel", "C & I Knob, UFC/BACKUP", "%0.-1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "540", SwitchPositions.Create(5, 0d, 0.1d, F16CCommands.iffCommands.MasterKnob.ToString("d"), new string[] { "OFF", "STBY", "LOW", "NORM", "EMER" }, "%0.1f"), "IFF Control Panel", "IFF MASTER Knob, OFF/STBY/LOW/NORM/EMER", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "541", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.iffCommands.M4CodeSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.iffCommands.M4CodeSw.ToString("d")), new SwitchPosition("1.0", "Posn 3", F16CCommands.iffCommands.M4CodeSw.ToString("d")) }, "IFF Control Panel", "IFF M-4 CODE Switch, HOLD/ A/B /ZERO", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "543", new SwitchPosition[] { new SwitchPosition("-1.0", " OUT", F16CCommands.iffCommands.M4ReplySw.ToString("d")), new SwitchPosition("0.0", "A", F16CCommands.iffCommands.M4ReplySw.ToString("d")), new SwitchPosition("1.0", "B", F16CCommands.iffCommands.M4ReplySw.ToString("d")) }, "IFF Control Panel", "IFF MODE 4 REPLY Switch, OUT/A/B", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "544", new SwitchPosition[] { new SwitchPosition("1.0", " OUT", F16CCommands.iffCommands.M4MonitorSw.ToString("d")), new SwitchPosition("0.0", "AUDIO", F16CCommands.iffCommands.M4MonitorSw.ToString("d")) }, "IFF Control Panel", "IFF MODE 4 MONITOR Switch, OUT/AUDIO", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "553", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.iffCommands.EnableSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.iffCommands.EnableSw.ToString("d")), new SwitchPosition("1.0", "Posn 3", F16CCommands.iffCommands.EnableSw.ToString("d")) }, "IFF Control Panel", "IFF ENABLE Switch, M1/M3 /OFF/ M3/MS", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "545", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.iffCommands.M1M3Selector1_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector1_Dec.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.iffCommands.M1M3Selector1_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector1_Inc.ToString("d"), "0.0", "0.0") }, "IFF Control Panel", "IFF MODE 1 Selector Lever, DIGIT 1", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "547", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.iffCommands.M1M3Selector2_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector2_Dec.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.iffCommands.M1M3Selector2_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector2_Inc.ToString("d"), "0.0", "0.0") }, "IFF Control Panel", "IFF MODE 1 Selector Lever, DIGIT 2", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "549", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.iffCommands.M1M3Selector3_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector3_Dec.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.iffCommands.M1M3Selector3_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector3_Inc.ToString("d"), "0.0", "0.0") }, "IFF Control Panel", "IFF MODE 3 Selector Lever, DIGIT 1", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "551", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.iffCommands.M1M3Selector4_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector4_Dec.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.iffCommands.M1M3Selector4_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector4_Inc.ToString("d"), "0.0", "0.0") }, "IFF Control Panel", "IFF MODE 3 Selector Lever, DIGIT 2", "%0.1f"));
            #endregion IFF Control Panel
            #region KY-58
            AddFunction(new Switch(this, devices.KY58.ToString("d"), "705", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.ky58Commands.KY58_ModeSw.ToString("d"), new string[] { "P", "C", "LD", "RV" }, "%0.1f"), "KY-58", "KY-58 MODE Knob, P/C/LD/RV", "%0.1f"));
            AddFunction(new Axis(this, devices.KY58.ToString("d"), F16CCommands.ky58Commands.KY58_Volume.ToString("d"), "708", 0.1d, 0.0d, 1.0d, "KY-58", "KY-58 VOLUME Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.KY58.ToString("d"), "706", SwitchPositions.Create(8, 0d, 0.1d, F16CCommands.ky58Commands.KY58_FillSw.ToString("d"), new string[] { "Z 1-5", "1", "2", "3", "4", "5", "6", "Z ALL" }, "%0.1f"), "KY-58", "KY-58 FILL Knob, Z 1-5/1/2/3/4/5/6/Z ALL", "%0.1f"));
            AddFunction(new Switch(this, devices.KY58.ToString("d"), "707", SwitchPositions.Create(3, 0d, 0.5d, F16CCommands.ky58Commands.KY58_PowerSw.ToString("d"), new string[] { "OFF", "ON", "TD" }, "%0.1f"), "KY-58", "KY-58 Power Knob, OFF/ON/TD", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "701", new SwitchPosition[] { new SwitchPosition("-1.0", " CRAD 1", F16CCommands.intercomCommands.PlainCipherSw.ToString("d")), new SwitchPosition("0.0", "PLAIN", F16CCommands.intercomCommands.PlainCipherSw.ToString("d")), new SwitchPosition("1.0", "CRAD 2", F16CCommands.intercomCommands.PlainCipherSw.ToString("d")) }, "KY-58", "PLAIN Cipher Switch, CRAD 1/PLAIN/CRAD 2", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "694", new SwitchPosition[] { new SwitchPosition("0.0", " OPEN", F16CCommands.intercomCommands.ZeroizeSwCvr.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.intercomCommands.ZeroizeSwCvr.ToString("d")) }, "KY-58", "ZEROIZE Switch Cover, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "695", new SwitchPosition[] { new SwitchPosition("-1.0", " OFP", F16CCommands.intercomCommands.ZeroizeSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.intercomCommands.ZeroizeSw.ToString("d")), new SwitchPosition("1.0", "DATA", F16CCommands.intercomCommands.ZeroizeSw.ToString("d")) }, "KY-58", "ZEROIZE Switch, OFP/OFF/DATA", "%0.1f"));
            #endregion KY-58
            #region HMCS
            AddFunction(new Axis(this, devices.HMCS.ToString("d"), F16CCommands.hmcsCommands.IntKnob.ToString("d"), "392", 0.1d, 0.0d, 1.0d, "HMCS", "HMCS SYMBOLOGY INT Knob", false, "%0.1f"));
            #endregion HMCS
            #region RWR
            AddFunction(new Axis(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.IntKnob.ToString("d"), "140", 0.1d, 0.0d, 1.0d, "RWR", "RWR Intensity Knob - Rotate to adjust brightness", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.Handoff.ToString("d"), "141", "RWR", "RWR Indicator Control HANDOFF Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.Launch.ToString("d"), "143", "RWR", "RWR Indicator Control LAUNCH Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.Mode.ToString("d"), "145", "RWR", "RWR Indicator Control MODE Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.UnknownShip.ToString("d"), "147", "RWR", "RWR Indicator Control UNKNOWN SHIP Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.SysTest.ToString("d"), "149", "RWR", "RWR Indicator Control SYS TEST Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.TgtSep.ToString("d"), "151", "RWR", "RWR Indicator Control T Button", "%1d"));
            AddFunction(new Axis(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.BrtKnob.ToString("d"), "404", 0.1d, 0.0d, 1.0d, "RWR", "RWR Indicator Control DIM Knob - Rotate to adjust brightness", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.Search.ToString("d"), "395", "RWR", "RWR Indicator Control SEARCH Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.ActPwr.ToString("d"), "397", "RWR", "RWR Indicator Control ACT/PWR Button", "%1d"));
            AddFunction(new PushButton(this, devices.RWR.ToString("d"), F16CCommands.rwrCommands.Altitude.ToString("d"), "399", "RWR", "RWR Indicator Control ALTITUDE Button", "%1d"));
            AddFunction(new Switch(this, devices.RWR.ToString("d"), "401", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.rwrCommands.Power.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.rwrCommands.Power.ToString("d")) }, "RWR", "RWR Indicator Control POWER Button", "%0.1f"));
            #endregion RWR
            #region CMDS
            AddFunction(new PushButton(this, devices.CMDS.ToString("d"), F16CCommands.cmdsCommands.DispBtn.ToString("d"), "604", "CMDS", "CHAFF/FLARE Dispense Button - Push to dispense", "%1d"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "375", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.RwrSrc.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.RwrSrc.ToString("d")) }, "CMDS", "RWR 555 Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "374", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.JmrSrc.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.JmrSrc.ToString("d")) }, "CMDS", "JMR Source Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "373", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.MwsSrc.ToString("d")), new SwitchPosition("0.0", "OFF (no function)", F16CCommands.cmdsCommands.MwsSrc.ToString("d")) }, "CMDS", "MWS Source Switch, ON/OFF (no function)", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "371", new SwitchPosition[] { new SwitchPosition("1.0", " JETT", F16CCommands.cmdsCommands.Jett.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.Jett.ToString("d")) }, "CMDS", "Jettison Switch, JETT/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "365", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.O1Exp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.O1Exp.ToString("d")) }, "CMDS", "O1 Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "366", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.O2Exp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.O2Exp.ToString("d")) }, "CMDS", "O2 Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "367", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.ChExp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.ChExp.ToString("d")) }, "CMDS", "CH Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "368", new SwitchPosition[] { new SwitchPosition("1.0", " ON", F16CCommands.cmdsCommands.FlExp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.FlExp.ToString("d")) }, "CMDS", "FL Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "377", SwitchPositions.Create(5, 0d, 0.1d, F16CCommands.cmdsCommands.Prgm.ToString("d"), new string[] { "BIT", "1", "2", "3", "4" }, "%0.1f"), "CMDS", "PROGRAM Knob, BIT/1/2/3/4", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "378", SwitchPositions.Create(6, 0d, 0.1d, F16CCommands.cmdsCommands.Mode.ToString("d"), new string[] { "OFF", "STBY", "MAN", "SEMI", "AUTO", "BYP" }, "%0.1f"), "CMDS", "MODE Knob, OFF/STBY/MAN/SEMI/AUTO/BYP", "%0.1f"));
            #endregion CMDS
            #region MFD Left
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_1.ToString("d"), "300", "MFD Left", "Left MFD OSB 1", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_2.ToString("d"), "301", "MFD Left", "Left MFD OSB 2", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_3.ToString("d"), "302", "MFD Left", "Left MFD OSB 3", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_4.ToString("d"), "303", "MFD Left", "Left MFD OSB 4", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_5.ToString("d"), "304", "MFD Left", "Left MFD OSB 5", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_6.ToString("d"), "305", "MFD Left", "Left MFD OSB 6", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_7.ToString("d"), "306", "MFD Left", "Left MFD OSB 7", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_8.ToString("d"), "307", "MFD Left", "Left MFD OSB 8", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_9.ToString("d"), "308", "MFD Left", "Left MFD OSB 9", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_10.ToString("d"), "309", "MFD Left", "Left MFD OSB 10", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_11.ToString("d"), "310", "MFD Left", "Left MFD OSB 11", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_12.ToString("d"), "311", "MFD Left", "Left MFD OSB 12", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_13.ToString("d"), "312", "MFD Left", "Left MFD OSB 13", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_14.ToString("d"), "313", "MFD Left", "Left MFD OSB 14", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_15.ToString("d"), "314", "MFD Left", "Left MFD OSB 15", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_16.ToString("d"), "315", "MFD Left", "Left MFD OSB 16", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_17.ToString("d"), "316", "MFD Left", "Left MFD OSB 17", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_18.ToString("d"), "317", "MFD Left", "Left MFD OSB 18", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_19.ToString("d"), "318", "MFD Left", "Left MFD OSB 19", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.OSB_20.ToString("d"), "319", "MFD Left", "Left MFD OSB 20", "%1d"));
            AddFunction(new Switch(this, devices.MFD_LEFT.ToString("d"), "320", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Left", "Left MFD GAIN Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_LEFT.ToString("d"), "321", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Left", "Left MFD SYM Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_LEFT.ToString("d"), "322", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Left", "Left MFD CON Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_LEFT.ToString("d"), "323", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Left", "Left MFD BRT Rocker Switch", "%0.1f"));
            #endregion MFD Left
            #region MFD Right
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_1.ToString("d"), "326", "MFD Right", "Right MFD OSB 1", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_2.ToString("d"), "327", "MFD Right", "Right MFD OSB 2", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_3.ToString("d"), "328", "MFD Right", "Right MFD OSB 3", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_4.ToString("d"), "329", "MFD Right", "Right MFD OSB 4", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_5.ToString("d"), "330", "MFD Right", "Right MFD OSB 5", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_6.ToString("d"), "331", "MFD Right", "Right MFD OSB 6", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_7.ToString("d"), "332", "MFD Right", "Right MFD OSB 7", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_8.ToString("d"), "333", "MFD Right", "Right MFD OSB 8", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_9.ToString("d"), "334", "MFD Right", "Right MFD OSB 9", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_10.ToString("d"), "335", "MFD Right", "Right MFD OSB 10", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_11.ToString("d"), "336", "MFD Right", "Right MFD OSB 11", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_12.ToString("d"), "337", "MFD Right", "Right MFD OSB 12", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_13.ToString("d"), "338", "MFD Right", "Right MFD OSB 13", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_14.ToString("d"), "339", "MFD Right", "Right MFD OSB 14", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_15.ToString("d"), "340", "MFD Right", "Right MFD OSB 15", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_16.ToString("d"), "341", "MFD Right", "Right MFD OSB 16", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_17.ToString("d"), "342", "MFD Right", "Right MFD OSB 17", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_18.ToString("d"), "343", "MFD Right", "Right MFD OSB 18", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_19.ToString("d"), "344", "MFD Right", "Right MFD OSB 19", "%1d"));
            AddFunction(new PushButton(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.OSB_20.ToString("d"), "345", "MFD Right", "Right MFD OSB 20", "%1d"));
            AddFunction(new Switch(this, devices.MFD_RIGHT.ToString("d"), "346", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Right", "Left MFD GAIN Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_RIGHT.ToString("d"), "347", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Right", "Left MFD SYM Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_RIGHT.ToString("d"), "348", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Right", "Left MFD CON Rocker Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MFD_RIGHT.ToString("d"), "349", new SwitchPosition[] { new SwitchPosition("1.0", "Up/Increase", F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Down/Decrease", F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), "0.0", "0.0") }, "MFD Right", "Left MFD BRT Rocker Switch", "%0.1f"));
            #endregion MFD Right
            #region Instruments
            #endregion Instruments
            #region Airspeed/Mach Indicator
            AddFunction(new Axis(this, devices.AMI.ToString("d"), F16CCommands.amiCommands.SettingKnob.ToString("d"), "71", 0.05d, 0.0d, 0d, "Airspeed/Mach Indicator", "SET INDEX Knob", false, "%0.1f"));
            #endregion Airspeed/Mach Indicator
            #region Altimeter
            AddFunction(new Axis(this, devices.AAU34.ToString("d"), F16CCommands.altCommands.ZERO.ToString("d"), "62", 0.2d, 0.0d, 0.4d, "Altimeter", "Barometric Setting Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.AAU34.ToString("d"), "60", new SwitchPosition[] { new SwitchPosition("1.0", " ELEC", F16CCommands.altCommands.ELEC.ToString("d"), F16CCommands.altCommands.ELEC.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "OFF", null), new SwitchPosition("-1.0", "PNEU", F16CCommands.altCommands.PNEU.ToString("d"), F16CCommands.altCommands.PNEU.ToString("d"), "0.0", "0.0") }, "Altimeter", "Mode Lever, ELEC/OFF/PNEU", "%0.1f"));
            #endregion Altimeter
            #region SAI ARU-42/A-2
            #endregion SAI ARU-42/A-2
            #region ADI
            AddFunction(new Axis(this, devices.ADI.ToString("d"), F16CCommands.deviceCommands.Button_1.ToString("d"), "22", 0.1d, 0.0d, 1.0d, "ADI", "Pitch Trim Knob", false, "%0.1f"));
            #endregion ADI
            #region EHSI
            AddFunction(new PushButton(this, devices.EHSI.ToString("d"), F16CCommands.ehsiCommands.RightKnobBtn.ToString("d"), "43", "EHSI", "Button CRS Set / Brightness Control Knob", "%1d"));
            AddFunction(new Axis(this, devices.EHSI.ToString("d"), F16CCommands.ehsiCommands.RightKnob.ToString("d"), "44", 0.5d, 0.0d, 1.0d, "EHSI", "Lamp CRS Set / Brightness Control Knob", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.EHSI.ToString("d"), F16CCommands.ehsiCommands.LeftKnobBtn.ToString("d"), "42", "EHSI", "Button HDG Set Knob", "%1d"));
            AddFunction(new Axis(this, devices.EHSI.ToString("d"), F16CCommands.ehsiCommands.LeftKnob.ToString("d"), "45", 0.5d, 0.0d, 1.0d, "EHSI", "Lamp HDG Set Knob", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.EHSI.ToString("d"), F16CCommands.ehsiCommands.ModeBtn.ToString("d"), "46", "EHSI", "Mode (M) Button", "%1d"));
            #endregion EHSI
            #region Clock
            AddFunction(new PushButton(this, devices.CLOCK.ToString("d"), F16CCommands.clockCommands.CLOCK_right_lev_down.ToString("d"), "627", "Clock", "Clock Elapsed Time Knob", "%1d"));
            #endregion Clock
            #region Cockpit Mechanics
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "600", new SwitchPosition[] { new SwitchPosition("1.0", " UP", F16CCommands.cptCommands.CanopyHandle.ToString("d")), new SwitchPosition("0.0", "DOWN", F16CCommands.cptCommands.CanopyHandle.ToString("d")) }, "Cockpit Mechanics", "Canopy Handle, UP/DOWN", "%0.1f"));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "786", new SwitchPosition[] { new SwitchPosition("1.0", " UP", F16CCommands.cptCommands.SeatAdjSwitchDown.ToString("d"), F16CCommands.cptCommands.SeatAdjSwitchDown.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "OFF", null), new SwitchPosition("-1.0", "DOWN", F16CCommands.cptCommands.SeatAdjSwitchUp.ToString("d"), F16CCommands.cptCommands.SeatAdjSwitchUp.ToString("d"), "0.0", "0.0") }, "Cockpit Mechanics", "SEAT ADJ Switch, UP/OFF/DOWN", "%0.1f"));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "601", new SwitchPosition[] { new SwitchPosition("1.0", " PULL", F16CCommands.cptCommands.CanopyTHandle.ToString("d")), new SwitchPosition("0.0", "STOW", F16CCommands.cptCommands.CanopyTHandle.ToString("d")) }, "Cockpit Mechanics", "CANOPY JETTISON T-Handle, PULL/STOW", "%0.1f"));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "785", new SwitchPosition[] { new SwitchPosition("1.0", " ARMED", F16CCommands.cptCommands.EjectionSafetyLever.ToString("d")), new SwitchPosition("0.0", "LOCKED", F16CCommands.cptCommands.EjectionSafetyLever.ToString("d")) }, "Cockpit Mechanics", "Ejection Safety Lever, ARMED/LOCKED", "%0.1f"));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "606", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.cptCommands.CanopySwitchClose.ToString("d"), F16CCommands.cptCommands.CanopySwitchClose.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", F16CCommands.cptCommands.CanopySwitchOpen.ToString("d"), F16CCommands.cptCommands.CanopySwitchOpen.ToString("d"), "0.0", "0.0") }, "Cockpit Mechanics", "Canopy Switch, OPEN/HOLD/CLOSE(momentarily)", "%0.1f"));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "796", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.cptCommands.StickHide.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.cptCommands.StickHide.ToString("d")) }, "Cockpit Mechanics", "Hide Stick toggle", "%0.1f"));
            #endregion Cockpit Mechanics
            #region ECM
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "455", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.ecmCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.PwrSw.ToString("d")), new SwitchPosition("1.0", "Posn 3", F16CCommands.ecmCommands.PwrSw.ToString("d")) }, "ECM", "ECM Power Switch", "%0.1f"));
            AddFunction(new Axis(this, devices.ECM_INTERFACE.ToString("d"), F16CCommands.ecmCommands.DimRotary.ToString("d"), "456", 0.1d, 0.0d, 1.0d, "ECM", "ECM DIM Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "457", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.ecmCommands.XmitSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.XmitSw.ToString("d")), new SwitchPosition("1.0", "Posn 3", F16CCommands.ecmCommands.XmitSw.ToString("d")) }, "ECM", "ECM XMIT Switch", "%0.1f"));
            AddFunction(new PushButton(this, devices.ECM_INTERFACE.ToString("d"), F16CCommands.ecmCommands.ResetBtn.ToString("d"), "458", "ECM", "ECM RESET Button", "%1d"));
            AddFunction(new PushButton(this, devices.ECM_INTERFACE.ToString("d"), F16CCommands.ecmCommands.BitBtn.ToString("d"), "459", "ECM", "ECM BIT Button", "%1d"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "460", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.OneBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.OneBtn.ToString("d")) }, "ECM", "ECM 1 Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "465", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.TwoBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.TwoBtn.ToString("d")) }, "ECM", "ECM 2 Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "470", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.ThreeBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.ThreeBtn.ToString("d")) }, "ECM", "ECM 3 Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "475", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.FourBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.FourBtn.ToString("d")) }, "ECM", "ECM 4 Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "480", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.FiveBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.FiveBtn.ToString("d")) }, "ECM", "ECM 5 Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "485", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.SixBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.SixBtn.ToString("d")) }, "ECM", "ECM 6 Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "490", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.FrmBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.FrmBtn.ToString("d")) }, "ECM", "ECM FRM Button", "%0.1f"));
            AddFunction(new Switch(this, devices.ECM_INTERFACE.ToString("d"), "495", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecmCommands.SplBtn.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecmCommands.SplBtn.ToString("d")) }, "ECM", "ECM SPL Button", "%0.1f"));
            #endregion ECM
            //#region Control Interface
            //AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.RollTrim.ToString("d"), "560", 0.05d, -1.0d, 1.0d, "Control Interface", "ROLL TRIM Wheel", false, "%.5f"));
            //AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.PitchTrim.ToString("d"), "562", 0.05d, -1.0d, 1.0d, "Control Interface", "PITCH TRIM Wheel", false, "%.5f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.TrimApDisc.ToString("d"), "564", "1.0", "Disc", "0.0", "Norm", "Control Interface", "TRIM/AP DISC Switch, DISC/NORM", "%0.1f"));
            //AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.YawTrim.ToString("d"), "565", 0.05d, -1.0d, 1.0d, "Control Interface", "YAW TRIM Knob", false, "%.5f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.DigitalBackup.ToString("d"), "566", "1.0", "Off", "0.0", "Backup", "Control Interface", "DIGITAL BACKUP Switch, OFF/BACKUP", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.AltFlaps.ToString("d"), "567", "1.0", "Norm", "0.0", "Extend", "Control Interface", "ALT FLAPS Switch, NORM/EXTEND", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.ManualTfFlyup.ToString("d"), "568", "1.0", "Enable", "0.0", "Disable", "Control Interface", "MANUAL TF FLYUP Switch, ENABLE/DISABLE", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.LeFlaps.ToString("d"), "572", "1.0", "Auto", "0.0", "Lock", "Control Interface", "LE FLAPS Switch, AUTO/LOCK", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.FlcsReset.ToString("d"), "573", "1.0", "Off", "0.0", "Reset", "Control Interface", "FLCS RESET Switch, OFF/RESET", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.BitSw.ToString("d"), "574", "1.0", "Off", "0.0", "BIT", "Control Interface", "BIT Switch, OFF/BIT", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.ManualPitchOverride.ToString("d"), "425", "1.0", "Override", "0.0", "Norm", "Control Interface", "MANUAL PITCH Override Switch, OVRD/NORM", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.StoresConfig.ToString("d"), "358", "1.0", "CAT III", "0.0", "CAT I", "Control Interface", "STORES CONFIG Switch, CAT III/CAT I", "%0.1f"));
            //AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString(), "109", new SwitchPosition[] { new SwitchPosition("1.0", "ATT HOLD", controlCommands.ApPitchAtt.ToString("d"), controlCommands.ApPitchAtt.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "A/P Off", null), new SwitchPosition("-1.0", "ALT HOLD", controlCommands.ApPitchAlt.ToString("d"), controlCommands.ApPitchAlt.ToString("d"), "0.0", "0.0") }, "Control Interface", "Autopilot PITCH Switch, ATT HOLD/ A/P OFF/ ALT HOLD", "%0.1f"));
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.ApRoll.ToString("d"), "108", "1.0", "HDG SEL", "0.0", "ATT HOLD", "-1.0", "STRG SEL", "Control Interface", "Autopilot ROLL Switch, STRG SEL/ATT HOLD/HDG SEL", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROL_INTERFACE.ToString(), controlCommands.AdvMode.ToString("d"), "97", "1.0", "Position1", "0.0", "Position2", "Control Interface", "ADV MODE Switch", "%0.1f"));
            //#endregion        
            //#region External Lights
            //AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString(), "531", new SwitchPosition[] {
            //    new SwitchPosition("0.0", "Off", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.1", "1", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.2", "2", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.3", "3", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.4", "4", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.5", "A", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.6", "B", extlightsCommands.AntiCollKn.ToString("d")),
            //    new SwitchPosition("0.7", "C", extlightsCommands.AntiCollKn.ToString("d"))
            //    }, "External Lights", "ANTI-COLL Knob, OFF/1/2/3/4/A/B/C", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.EXTLIGHTS_SYSTEM.ToString(), extlightsCommands.PosFlash.ToString("d"), "532", "1.0", "Flash", "0.0", "Steady", "External Lights", "FLASH STEADY Switch, FLASH/STEADY", "%0.1f"));
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.EXTLIGHTS_SYSTEM.ToString(), extlightsCommands.PosWingTail.ToString("d"), "533", "1.0", "BRT", "0.0", "OFF", "-1.0", "DIM", "External Lights", "WING/TAIL Switch, BRT/OFF/DIM", "%0.1f"));
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.EXTLIGHTS_SYSTEM.ToString(), extlightsCommands.PosFus.ToString("d"), "534", "1.0", "BRT", "0.0", "OFF", "-1.0", "DIM", "External Lights", "FUSELAGE Switch, BRT/OFF/DIM", "%0.1f"));
            //AddFunction(new Axis(this, devices.EXTLIGHTS_SYSTEM.ToString(), extlightsCommands.FormKn.ToString("d"), "535", 0.025d, 0.0d, 1.0d, "External Lights", "FORM Knob", false, "%.5f"));
            //AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString(), "536", new SwitchPosition[] {
            //    new SwitchPosition("0.0", "Off", extlightsCommands.Master.ToString("d")),
            //    new SwitchPosition("0.1", "ALL", extlightsCommands.Master.ToString("d")),
            //    new SwitchPosition("0.2", "A-C", extlightsCommands.Master.ToString("d")),
            //    new SwitchPosition("0.3", "FORM", extlightsCommands.Master.ToString("d")),
            //    new SwitchPosition("0.4", "NORM", extlightsCommands.Master.ToString("d"))
            //    }, "External Lights", "MASTER Switch, OFF/ALL/A-C/FORM/NORM", "%0.1f"));
            //AddFunction(new Axis(this, devices.EXTLIGHTS_SYSTEM.ToString(), extlightsCommands.AerialRefuel.ToString("d"), "537", 0.025d, 0.0d, 1.0d, "External Lights", "AERIAL REFUELING Knob",false, "%.5f"));
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.EXTLIGHTS_SYSTEM.ToString(), extlightsCommands.PosFus.ToString("d"), "360", "1.0", "TAXI", "0.0", "OFF", "-1.0", "LANDING", "External Lights", "LANDING TAXI LIGHTS Switch, LANDING/OFF/TAXI", "%0.1f"));
            //#endregion
            //#region Interior Lights
            //AddFunction(new PushButton(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.MasterCaution.ToString("d"), "116", "Cockpit Lights", "Master Caution Button - Push to reset"));
            //AddFunction(new PushButton(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.MalIndLtsTest.ToString("d"), "577", "Cockpit Lights", "MAL & IND LTS Test Button - Push to test"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.Consoles.ToString("d"), "685", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "PRIMARY CONSOLES BRT Knob", false, "%.5f"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.IntsPnl.ToString("d"), "686", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "PRIMARY INST PNL BRT Knob", false, "%.5f"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.DataEntryDisplay.ToString("d"), "687", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "PRIMARY DATA ENTRY DISPLAY BRT Knob", false, "%.5f"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.ConsolesFlood.ToString("d"), "688", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "FLOOD CONSOLES BRT Knob", false, "%.5f"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.InstPnlFlood.ToString("d"), "690", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "FLOOD INST PNL BRT Knob", false, "%.5f"));
            //AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString(), "691", new SwitchPosition[] { new SwitchPosition("1.0", "Dim", cptlightsCommands.MalIndLtsDim.ToString("d"), cptlightsCommands.MalIndLtsDim.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Center", null), new SwitchPosition("-1.0", "Bright", cptlightsCommands.MalIndLtsBrt.ToString("d"), cptlightsCommands.MalIndLtsBrt.ToString("d"), "0.0", "0.0") }, "Control Interface", "MAL & IND LTS Switch, BRT/Center/DIM", "%0.1f"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.IndBrtAoA.ToString("d"), "794", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "AOA Indexer Dimming Lever", false, "%.5f"));
            //AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString(), cptlightsCommands.IndBrtAR.ToString("d"), "795", 0.05d, 0.0d, 1.0d, "Cockpit Lights", "AR Status Indicator Dimming Lever", false, "%.5f"));
            //#endregion
            //#region Electric System
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.ELEC_INTERFACE.ToString(), elecCommands.MainPwrSw.ToString("d"), "510", "1.0", "Main Power", "0.0", "Battery", "-1.0", "Off", "Electrical Interface", "MAIN PWR Switch, MAIN PWR/BATT/OFF", "%0.1f"));
            //AddFunction(new PushButton(this, devices.ELEC_INTERFACE.ToString(), elecCommands.CautionResetBtn.ToString("d"), "511", "Electrical Interface", "ELEC CAUTION RESET Button - Push to reset"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ELEC_INTERFACE.ToString(), elecCommands.EPU_GEN_TestSw.ToString("d"), "579", "1.0", "EPU/Gen", "0.0", "Off", "Electrical Interface", "EPU/GEN Test Switch, EPU/GEN / OFF", "%0.1f"));
            //AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString(), "578", new SwitchPosition[] { new SwitchPosition("1.0", "Probe Heat", elecCommands.ProbeHeatSw.ToString("d"), elecCommands.ProbeHeatSw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Test", elecCommands.ProbeHeatSwTEST.ToString("d"), elecCommands.ProbeHeatSwTEST.ToString("d"), "0.0", "0.0") }, "Electrical Interface", "PROBE HEAT Switch, PROBE HEAT/OFF/TEST(momentarily)", "%0.1f"));
            //AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString(), "585", new SwitchPosition[] { new SwitchPosition("1.0", "Maint", elecCommands.FlcsPwrTestSwMAINT.ToString("d"), elecCommands.FlcsPwrTestSwMAINT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Norm", null), new SwitchPosition("-1.0", "Test", elecCommands.FlcsPwrTestSwTEST.ToString("d"), elecCommands.FlcsPwrTestSwTEST.ToString("d"), "0.0", "0.0") }, "Electrical Interface", "FLCS PWR TEST Switch, MAINT/NORM/TEST(momentarily)", "%0.1f"));
            //#endregion
            //#region Fuel System
            //AddFunction(Switch.CreateToggleSwitch(this, devices.FUEL_INTERFACE.ToString(), fuelCommands.FuelMasterSw.ToString("d"), "559", "1.0", "Master", "0.0", "Off", "Fuel Interface", "FUEL MASTER Switch, MASTER/OFF", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.FUEL_INTERFACE.ToString(), fuelCommands.FuelMasterSwCvr.ToString("d"), "558", "1.0", "Open", "0.0", "Close", "Fuel Interface", "FUEL MASTER Switch Cover, OPEN/CLOSE", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.FUEL_INTERFACE.ToString(), fuelCommands.TankInertingSw.ToString("d"), "557", "1.0", "Tank Inerting", "0.0", "Off", "Fuel Interface", "TANK INERTING Switch, TANK INERTING /OFF", "%0.1f"));
            //AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString(), "536", new SwitchPosition[] {
            //    new SwitchPosition("0.0", "Off", fuelCommands.EngineFeedSw.ToString("d")),
            //    new SwitchPosition("0.1", "Norm", fuelCommands.EngineFeedSw.ToString("d")),
            //    new SwitchPosition("0.2", "Aft", fuelCommands.EngineFeedSw.ToString("d")),
            //    new SwitchPosition("0.3", "Fwd", fuelCommands.EngineFeedSw.ToString("d"))
            //    }, "Fuel Interface", "ENGINE FEED Knob, OFF/NORM/AFT/FWD", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.FUEL_INTERFACE.ToString(), fuelCommands.AirRefuelSw.ToString("d"), "555", "1.0", "Open", "0.0", "Close", "Fuel Interface", "AIR REFUEL Switch, OPEN/CLOSE", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.FUEL_INTERFACE.ToString(), fuelCommands.ExtFuelTransferSw.ToString("d"), "159", "1.0", "Norm", "0.0", "Wing First", "Fuel Interface", "External Fuel Transfer Switch, NORM/ WING FIRST", "%0.1f"));
            //AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString(), "158", new SwitchPosition[] {
            //    new SwitchPosition("0.0", "Test", fuelCommands.FuelQtySelSw.ToString("d")),
            //    new SwitchPosition("0.1", "Norm", fuelCommands.FuelQtySelSw.ToString("d")),
            //    new SwitchPosition("0.2", "Int Wing", fuelCommands.FuelQtySelSw.ToString("d")),
            //    new SwitchPosition("0.3", "Ext Wing", fuelCommands.FuelQtySelSw.ToString("d")),
            //    new SwitchPosition("0.4", "Ext Center", fuelCommands.FuelQtySelSw.ToString("d"))
            //    }, "Fuel Interface", "FUEL QTY SEL Knob, TEST(momentarily)/NORM/RSVR/INT WING/EXT WING/EXT CTR", "%0.1f"));
            //#endregion
            //#region Gear System
            //AddFunction(Switch.CreateToggleSwitch(this, devices.GEAR_INTERFACE.ToString(), gearCommands.LGHandle.ToString("d"), "362", "1.0", "Up", "0.0", "Down", "Gear Interface", "Landing Gear Handle, UP/DN", "%0.1f"));
            //AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString(), gearCommands.DownLockRelBtn.ToString("d"), "361", "Gear Interface", "DN LOCK REL Button - Push to reset"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.GEAR_INTERFACE.ToString(), gearCommands.HookSw.ToString("d"), "354", "1.0", "Up", "0.0", "Down", "Gear Interface", "HOOK Switch, UP/DN", "%0.1f"));
            //AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString(), gearCommands.HornSilencerBtn.ToString("d"), "359", "Gear Interface", "HORN SILENCER Button - Push to reset"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.GEAR_INTERFACE.ToString(), gearCommands.BrakesChannelSw.ToString("d"), "356", "1.0", "Channel 1", "0.0", "Channel 2", "Gear Interface", "BRAKES Channel Switch, CHAN 1/CHAN 2", "%0.1f"));
            //AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString(), "357", new SwitchPosition[] { new SwitchPosition("1.0", "Park", gearCommands.ParkingSw.ToString("d"), gearCommands.ParkingSw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Anti Skid", null), new SwitchPosition("-1.0", "Off", gearCommands.AntiSkidSw.ToString("d"), gearCommands.AntiSkidSw.ToString("d"), "0.0", "0.0") }, "Gear Interface", "ANTI-SKID Switch, PARKING BRAKE/ANTI-SKID/OFF", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.GEAR_INTERFACE.ToString(), gearCommands.AltGearHandle.ToString("d"), "380", "1.0", "Pull", "0.0", "Stow", "Gear Interface", "ALT GEAR Handle, PULL/STOW", "%0.1f"));
            //AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString(), gearCommands.AltGearResetBtn.ToString("d"), "381", "Gear Interface", "ALT GEAR Handle - Push to reset"));
            //#endregion
            //#region ECS
            //AddFunction(new Axis(this, devices.ECS_INTERFACE.ToString(), ecsCommands.TempKnob.ToString("d"), "692", 0.05d, 0.0d, 1.0d, "Environmental Control", "Temperature Knob", false, "%.5f"));
            //AddFunction(new Switch(this, devices.ECS_INTERFACE.ToString(), "693", new SwitchPosition[] {
            //    new SwitchPosition("0.0", "Off", ecsCommands.AirSourceKnob.ToString("d")),
            //    new SwitchPosition("0.1", "Norm", ecsCommands.AirSourceKnob.ToString("d")),
            //    new SwitchPosition("0.2", "Dump", ecsCommands.AirSourceKnob.ToString("d")),
            //    new SwitchPosition("0.3", "Ram", ecsCommands.AirSourceKnob.ToString("d"))
            //    }, "Environmental Control", "AIR SOURCE Knob, OFF/NORM/DUMP/RAM", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ECS_INTERFACE.ToString(), ecsCommands.DefogLever.ToString("d"), "682", "1.0", "On", "0.0", "Off", "Environmental Control", "DEFOG Lever", "%0.1f"));
            //#endregion
            //#region EPU
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.EpuSwCvrOn.ToString("d"), "527", "1.0", "Open", "0.0", "Close", "Engine Interface", "EPU Switch Cover for ON, OPEN/CLOSE", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.EpuSwCvrOff.ToString("d"), "529", "1.0", "Open", "0.0", "Close", "Engine Interface", "EPU Switch Cover for OFF, OPEN/CLOSE", "%0.1f"));
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.EpuSw.ToString("d"), "528", "1.0", "On", "0.0", "Norm", "-1.0", "Off", "Engine Interface", "EPU Switch, ON/NORM/OFF", "%0.1f"));
            //#endregion
            //#region engine
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.EngAntiIceSw.ToString("d"), "710", "1.0", "On", "0.0", "Auto", "-1.0", "Off", "Engine Interface", "Engine ANTI ICE Switch, ON/AUTO/OFF", "%0.1f"));
            //AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString(), "447", new SwitchPosition[] { 
            //    new SwitchPosition("1.0", "Start 1", engineCommands.JfsSwStart1.ToString("d"), engineCommands.JfsSwStart1.ToString("d"), "0.0", "0.0"), 
            //    new SwitchPosition("0.0", "Off", null), 
            //    new SwitchPosition("-1.0", "Start 2", engineCommands.JfsSwStart2.ToString("d"), engineCommands.JfsSwStart2.ToString("d"), "0.0", "0.0") }, 
            //    "Engine Interface", "JFS Switch, START 1/OFF/START 2", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.EngContSwCvr.ToString("d"), "448", "1.0", "Open", "0.0", "Close", "Engine Interface", "ENG CONT Switch Cover, OPEN/CLOSE", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.EngContSw.ToString("d"), "449", "1.0", "Pri", "0.0", "Sec", "Engine Interface", "ENG CONT Switch, PRI/SEC", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.MaxPowerSw.ToString("d"), "451", "1.0", "Max Power", "0.0", "Off", "Engine Interface", "MAX POWER Switch, MAX POWER/OFF", "%0.1f"));
            //AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString(), "450", new SwitchPosition[] {
            //    new SwitchPosition("1.0", "After Burner Reset", engineCommands.ABResetSwReset.ToString("d"), engineCommands.ABResetSwReset.ToString("d"), "0.0", "0.0"),
            //    new SwitchPosition("0.0", "Norm", null),
            //    new SwitchPosition("-1.0", "Engine Data", engineCommands.ABResetSwEngData.ToString("d"), engineCommands.ABResetSwEngData.ToString("d"), "0.0", "0.0") },
            //    "Engine Interface", "AB RESET Switch, AB RESET/NORM/ENG DATA", "%0.1f"));
            //AddFunction(new PushButton(this, devices.ENGINE_INTERFACE.ToString(), engineCommands.FireOheatTestBtn.ToString("d"), "575", "Engine Interface", "FIRE & OHEAT DETECT Test Button - Push to test"));
            //#endregion
            //#region Oxygen System
            //AddFunction(Switch.CreateThreeWaySwitch(this, devices.OXYGEN_INTERFACE.ToString(), oxygenCommands.SupplyLever.ToString("d"), "728", "1.0", "PBG", "0.0", "On", "-1.0", "Off", "Oxygen Interface", "Supply Lever, PBG/ON/OFF", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.OXYGEN_INTERFACE.ToString(), oxygenCommands.DiluterLever.ToString("d"), "727", "1.0", "100 Percent", "0.0", "Norm", "Oxygen Interface", "Diluter Lever, 100 percent/NORM", "%0.1f"));
            //AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString(), "726", new SwitchPosition[] {
            //    new SwitchPosition("1.0", "Emergency", oxygenCommands.EmergencyLever.ToString("d"), oxygenCommands.EmergencyLever.ToString("d"), "0.0", "0.0"),
            //    new SwitchPosition("0.0", "Normal", null),
            //    new SwitchPosition("-1.0", "Test Mask", oxygenCommands.EmergencyLeverTestMask.ToString("d"), oxygenCommands.EmergencyLeverTestMask.ToString("d"), "0.0", "0.0") },
            //    "Oxygen Interface", "Emergency Lever, EMERGENCY/NORMAL/TEST MASK(momentarily)", "%0.1f"));
            //AddFunction(Switch.CreateToggleSwitch(this, devices.OXYGEN_INTERFACE.ToString(), oxygenCommands.ObogsBitSw.ToString("d"), "576", "1.0", "BIT", "0.0", "Off", "Oxygen Interface", "OBOGS BIT Switch, BIT/OFF", "%0.1f"));
            //#endregion
            //#region Sensor Power Control Panel
            ////"PTR-SNSR-TMB-LEFT-670"]		= default_2_position_tumb(_("LEFT HDPT Switch, ON/OFF"),			devices.SMS, sms_commands.LeftHDPT,  670)
            ////"PTR-SNSR-TMB-RIGHT-671"]		= default_2_position_tumb(_("RIGHT HDPT Switch, ON/OFF"),			devices.SMS, sms_commands.RightHDPT, 671)
            ////"PTR-SNSR-TMB-FCR-672"]		= default_2_position_tumb(_("FCR Switch, FCR/OFF"),					devices.FCR, fcr_commands.PwrSw,     672)
            ////"PTR-SNSR-TMB-RDR-673"]		= default_3_position_tumb(_("RDR ALT Switch, RDR ALT/STBY/OFF"),	devices.RALT, ralt_commands.PwrSw,   673)
            //#endregion
            //#region Avionic Power panel
            ////"PTR-AVIPWR-TMB-MMC-715"]		= default_2_position_tumb(_("MMC Switch, MMC/OFF"),				devices.MMC, mmc_commands.MmcPwr,	715)
            ////"PTR-AVIPWR-TMB-STSTA-716"]	= default_2_position_tumb(_("ST STA Switch, ST STA/OFF"),		devices.SMS, sms_commands.StStaSw,	716)
            ////"PTR-AVIPWR-TMB-MFD-717"]		= default_2_position_tumb(_("MFD Switch, MFD/OFF"),				devices.MMC, mmc_commands.MFD,		717)
            ////"PTR-AVIPWR-TMB-UFC-718"]		= default_2_position_tumb(_("UFC Switch, UFC/OFF"),				devices.UFC, ufc_commands.UFC_Sw,	718)
            ////"PTR-AVIPWR-TMB-GPS-720"]		= default_2_position_tumb(_("GPS Switch, GPS/OFF"),				devices.GPS, gps_commands.PwrSw,	720)
            ////"PTR-AVIPWR-LVR-MIDS-723"]		= multiposition_switch(_("MIDS LVT Knob, ZERO/OFF/ON"),			devices.MIDS, mids_commands.PwrSw, 723, 3, 0.1, NOT_INVERSED, 0.0, anim_speed_default * 0.1, NOT_CYCLED)
            ////"PTR-AVIPWR-LVR-INS-719"]		= multiposition_switch(_("INS Knob, OFF/STOR HDG/NORM/NAV/CAL/INFLT ALIGN/ATT"),	devices.INS, ins_commands.ModeKnob, 719, 7, 0.1, NOT_INVERSED, 0.0, anim_speed_default * 0.1, NOT_CYCLED)
            ////"PTR-AVIPWR-TMB-MAP-722"]		= default_2_position_tumb(_("MAP Switch, MAP/OFF"),				devices.MAP, map_commands.PwrSw,	722)
            ////"PTR-AVIPWR-TMB-DL-721"]		= default_2_position_tumb(_("DL Switch, DL/OFF"),				devices.IDM, idm_commands.PwrSw,	721)
            //#endregion
            //#region Modular Mission Computer (MMC)
            ////"PTR-CLCP-TMB-MASTER-105"]		= default_3_position_tumb(_("MASTER ARM Switch, MASTER ARM/OFF/SIMULATE"),	devices.MMC, mmc_commands.MasterArmSw,		105)
            ////"PTR-LGCP-BTN-ESJETT-353"]		= default_button(_("EMER STORES JETTISON Button - Push to jettison"),		devices.MMC, mmc_commands.EmerStoresJett,	353)
            ////"PTR-LGCP-TMB-GNDJETT-355"]	= default_2_position_tumb(_("GND JETT ENABLE Switch, ENABLE/OFF"),			devices.MMC, mmc_commands.GroundJett,		355)
            ////"PTR-CLCP-TMB-ALT-104"]		= default_button(_("ALT REL Button - Push to release"),						devices.MMC, mmc_commands.AltRel,			104)

            ////"PTR-CLCP-TMB-LASER-103"]		= default_2_position_tumb(_("LASER ARM Switch, ARM/OFF"),		devices.SMS, sms_commands.LaserSw, 103)
            //#endregion
            //#region  Integrated Control Panel (ICP) of Upfront Controls (UFC)
            ////"PTR-ICP-BTN-NMB1-171"]	= short_way_button(_("ICP Priority Function Button, 1(T-ILS)"),									devices.UFC, ufc_commands.DIG1_T_ILS,		171)
            ////"PTR-ICP-BTN-NMB2-172"]	= short_way_button(_("ICP Priority Function Button, 2/N(ALOW)"),								devices.UFC, ufc_commands.DIG2_ALOW,		172)
            ////"PTR-ICP-BTN-NMB3-173"]	= short_way_button(_("ICP Priority Function Button, 3"),										devices.UFC, ufc_commands.DIG3,				173)
            ////"PTR-ICP-BTN-NMB4-175"]	= short_way_button(_("ICP Priority Function Button, 4/W(STPT)"),								devices.UFC, ufc_commands.DIG4_STPT,		175)
            ////"PTR-ICP-BTN-NMB5-176"]	= short_way_button(_("ICP Priority Function Button, 5(CRUS)"),									devices.UFC, ufc_commands.DIG5_CRUS,		176)
            ////"PTR-ICP-BTN-NMB6-177"]	= short_way_button(_("ICP Priority Function Button, 6/E(TIME)"),								devices.UFC, ufc_commands.DIG6_TIME,		177)
            ////"PTR-ICP-BTN-NMB7-179"]	= short_way_button(_("ICP Priority Function Button, 7(MARK)"),									devices.UFC, ufc_commands.DIG7_MARK,		179)
            ////"PTR-ICP-BTN-NMB8-180"]	= short_way_button(_("ICP Priority Function Button, 8/S(FIX)"),									devices.UFC, ufc_commands.DIG8_FIX,			180)
            ////"PTR-ICP-BTN-NMB9-181"]	= short_way_button(_("ICP Priority Function Button, 9(A-CAL)"),									devices.UFC, ufc_commands.DIG9_A_CAL,		181)
            ////"PTR-ICP-BTN-NMB0-182"]	= short_way_button(_("ICP Priority Function Button, 0(M-SEL)"),									devices.UFC, ufc_commands.DIG0_M_SEL,		182)
            ////"PTR-ICP-BTN-COM1-165"]	= short_way_button(_("ICP COM Override Button, COM1(UHF)"),										devices.UFC, ufc_commands.COM1,				165)
            ////"PTR-ICP-BTN-COM2-166"]	= short_way_button(_("ICP COM Override Button, COM2(VHF)"),										devices.UFC, ufc_commands.COM2,				166)
            ////"PTR-ICP-BTN-IFF-167"]		= short_way_button(_("ICP IFF Override Button, IFF"),											devices.UFC, ufc_commands.IFF,				167)
            ////"PTR-ICP-BTN-LIST-168"]	= short_way_button(_("ICP LIST Override Button, LIST"),											devices.UFC, ufc_commands.LIST,				168)
            ////"PTR-ICP-BTN-AA-169"]		= short_way_button(_("ICP Master Mode Button, A-A"),											devices.UFC, ufc_commands.AA,				169)
            ////"PTR-ICP-BTN-AG-170"]		= short_way_button(_("ICP Master Mode Button, A-G"),											devices.UFC, ufc_commands.AG,				170)
            ////"PTR-ICP-BTN-RCL-174"]		= short_way_button(_("ICP Recall Button, RCL"),													devices.UFC, ufc_commands.RCL,				174)
            ////"PTR-ICP-BTN-ENTR-178"]	= short_way_button(_("ICP Enter Button, ENTR"),													devices.UFC, ufc_commands.ENTR,				178)
            ////"PTR-ICP-LVR-RET-192"]		= default_axis_limited_1_side(_("ICP Reticle Depression Control Knob"),							devices.UFC, ufc_commands.RET_DEPR_Knob,	192,nil,nil,nil,nil,nil, {0,135},{0,-90})
            ////"PTR-ICP-LVR-CONT-193"]	= default_axis_limited_1_side(_("ICP Raster Contrast Knob"),									devices.UFC, ufc_commands.CONT_Knob,		193,nil,nil,nil,nil,nil, {0,135},{0,-90})
            ////"PTR-ICP-LVR-BRT-191"]		= default_axis_limited_1_side(_("ICP Raster Intensity Knob"),									devices.UFC, ufc_commands.BRT_Knob,			191,nil,nil,nil,nil,nil, {0,90},{0,-135})
            ////"PTR-ICP-LVR-SYM-190"]		= default_axis_limited_1_side(_("ICP HUD Symbology Intensity Knob"),							devices.UFC, ufc_commands.SYM_Knob,			190,nil,nil,nil,nil,nil, {0,90},{0,-135})
            ////"PTR-ICP-BTN-WX-187"]		= short_way_button(_("ICP FLIR Polarity Button, Wx"),											devices.UFC, ufc_commands.Wx,				187)
            ////"PTR-ICP-TMB-GAIN-189"]	= default_3_position_tumb_small(_("ICP FLIR GAIN/LEVEL Switch, GAIN/LVL/AUTO"),					devices.UFC, ufc_commands.FLIR_GAIN_Sw,		189)
            ////"PTR-ICP-RS-OFF-UP-183"]	= Rocker_switch_positive(_("ICP DED Increment/Decrement Switch, Up"),							devices.UFC, ufc_commands.DED_INC,			183)
            ////"PTR-ICP-RS-OFF-DN-183"]	= Rocker_switch_negative(_("ICP DED Increment/Decrement Switch, Down"),							devices.UFC, ufc_commands.DED_DEC,			183)
            ////"PTR-ICP-RS-FLIR-UP-188"]	= Rocker_switch_positive(_("ICP FLIR Increment/Decrement Switch, Up"),							devices.UFC, ufc_commands.FLIR_INC,			188)
            ////"PTR-ICP-RS-FLIR-DN-188"]	= Rocker_switch_negative(_("ICP FLIR Increment/Decrement Switch, Down"),						devices.UFC, ufc_commands.FLIR_DEC,			188)
            ////"PTR-ICP-TMB-DRIFT-186"]	= default_button_tumb(_("ICP DRIFT CUTOUT/WARN RESET Switch, DRIFT C/O /NORM/WARN RESET"),	    devices.UFC, ufc_commands.WARN_RESET, ufc_commands.DRIFT_CUTOUT, 186)

            ////"PTR-ICP-TMB-RTN-184"]			= springloaded_2_pos_tumb_small(_("ICP Data Control Switch, RTN"),	devices.UFC, ufc_commands.DCS_RTN,	184)
            ////"PTR-ICP-TMB-RTN-184"].arg_value	= {-1, -1} 
            ////"PTR-ICP-TMB-RTN-184"].arg_lim		= {{-1,0},{-1,0}}
            ////"PTR-ICP-TMB-RTN-184"].side	    = {{BOX_SIDE_Y_bottom},{BOX_SIDE_Y_bottom}}
            ////"PTR-ICP-TMB-RTN-184"].sound = {{SOUND_SW4_OFF, SOUND_SW4_ON}}
            ////"PTR-ICP-TMB-SEQ-184"]			= springloaded_2_pos_tumb_small(_("ICP Data Control Switch, SEQ"),	devices.UFC, ufc_commands.DCS_SEQ,	184)
            ////"PTR-ICP-TMB-SEQ-184"].side	    = {{BOX_SIDE_Y_bottom},{BOX_SIDE_Y_bottom}}
            ////"PTR-ICP-TMB-SEQ-184"].sound = {{SOUND_SW4_ON, SOUND_SW4_OFF}}
            ////"PTR-ICP-TMB-RTNSEQ-UP-185"]	= springloaded_2_pos_tumb_small(_("ICP Data Control Switch, UP"),	devices.UFC, ufc_commands.DCS_UP,	185)
            ////"PTR-ICP-TMB-RTNSEQ-UP-185"].side	    = {{BOX_SIDE_Y_bottom},{BOX_SIDE_Y_bottom}}
            ////"PTR-ICP-TMB-RTNSEQ-UP-185"].sound = {{SOUND_SW4_ON, SOUND_SW4_OFF}}
            ////"PTR-ICP-TMB-RTNSEQ-DN-185"]	= springloaded_2_pos_tumb_small(_("ICP Data Control Switch, DN"),	devices.UFC, ufc_commands.DCS_DOWN,	185)
            ////"PTR-ICP-TMB-RTNSEQ-DN-185"].arg_value	= {-1, -1}
            ////"PTR-ICP-TMB-RTNSEQ-DN-185"].arg_lim	= {{-1,0},{-1,0}}
            ////"PTR-ICP-TMB-RTNSEQ-DN-185"].side	    = {{BOX_SIDE_Y_bottom},{BOX_SIDE_Y_bottom}}
            ////"PTR-ICP-TMB-RTNSEQ-DN-185"].sound = {{SOUND_SW4_OFF, SOUND_SW4_ON}}

            ////"PTR-CPBC-BTN-FACK-122"]		= short_way_button(_("F-ACK Button"),							devices.UFC, ufc_commands.F_ACK,		122)
            ////"PTR-CPBC-BTN-IFF-125"]		= short_way_button(_("IFF IDENT Button"),						devices.UFC, ufc_commands.IFF_IDENT,	125)
            ////"PTR-CLCP-TMB-RF-100"]			= default_3_position_tumb(_("RF Switch, SILENT/QUIET/NORM"),	devices.UFC, ufc_commands.RF_Sw,		100)
            //#endregion
            //#region  HUD Remote Control Panel
            ////"PTR-RHUD-TMB-WVAH-675"]	= default_3_position_tumb_small(_("HUD Scales Switch, VV/VAH / VAH / OFF"),						devices.MMC, mmc_commands.VvVah,	675)
            ////"PTR-RHUD-TMB-ATT-676"]	= default_3_position_tumb_small(_("HUD Flightpath Marker Switch, ATT/FPM / FPM / OFF"),			devices.MMC, mmc_commands.AttFpm,	676)
            ////"PTR-RHUD-TMB-DED-677"]	= default_3_position_tumb_small(_("HUD DED/PFLD Data Switch, DED / PFL / OFF"),				devices.MMC, mmc_commands.DedData,	677)
            ////"PTR-RHUD-TMB-DEPR-678"]	= default_3_position_tumb(_("HUD Depressible Reticle Switch, STBY / PRI / OFF"),			devices.MMC, mmc_commands.DeprRet,	678)
            ////"PTR-RHUD-TMB-CAS-679"]	= default_3_position_tumb_small(_("HUD Velocity Switch, CAS / TAS / GND SPD"),				devices.MMC, mmc_commands.Spd,		679)
            ////"PTR-RHUD-TMB-ALT-680"]	= default_3_position_tumb_small(_("HUD Altitude Switch, RADAR / BARO / AUTO"),				devices.MMC, mmc_commands.Alt,		680)
            ////"PTR-RHUD-TMB-DAYNGT-681"]	= default_3_position_tumb_small(_("HUD Brightness Control Switch, DAY / AUTO BRT / NIGHT"),	devices.MMC, mmc_commands.Brt,		681)
            ////"PTR-RHUD-TMB-TEST-682"]	= default_3_position_tumb_small(_("HUD TEST Switch, STEP / ON / OFF"),						devices.MMC, mmc_commands.Test,		682)
            //#endregion
            //#region  Audio Control Panels
            ////"PTR-AUDIO1-TMB-COMM1-434"]	= multiposition_switch(_("COMM 1 (UHF) Mode Knob"),											devices.INTERCOM, intercom_commands.COM1_ModeKnob,		434, 3, 0.5, NOT_INVERSED, 0.0, anim_speed_default, NOT_CYCLED, {100,-30}, {-70,-45})
            ////"PTR-AUDIO1-TMB-COMM2-435"]	= multiposition_switch(_("COMM 2 (VHF) Mode Knob"),											devices.INTERCOM, intercom_commands.COM2_ModeKnob,		435, 3, 0.5, NOT_INVERSED, 0.0, anim_speed_default, NOT_CYCLED, {100,-30}, {-70,-45})
            ////"PTR-AUDIO1-LVR-COMM1-430"]	= default_axis_limited(_("COMM 1 Power Knob"),												devices.INTERCOM, intercom_commands.COM1_PowerKnob,		430,nil,nil,nil,nil,nil, {100,-30}, {-70,-45})
            ////"PTR-AUDIO1-LVR-COMM2-431"]	= default_axis_limited(_("COMM 2 Power Knob"),												devices.INTERCOM, intercom_commands.COM2_PowerKnob,		431,nil,nil,nil,nil,nil, {100,-30}, {-70,-45})
            ////"PTR-AUDIO1-LVR-SV-432"]		= default_axis_limited(_("SECURE VOICE Knob"),												devices.INTERCOM, intercom_commands.SecureVoiceKnob,	432,nil,nil,nil,nil,nil, {100,-30}, {-100,-45})
            ////"PTR-AUDIO1-LVR-MSL-433"]		= default_axis_limited(_("MSL Tone Knob"),													devices.INTERCOM, intercom_commands.MSL_ToneKnob,		433,nil,nil,nil,nil,nil, {110,-30}, {-110,-45})
            ////"PTR-AUDIO1-LVR-TF-436"]		= default_axis_limited(_("TF Tone Knob"),													devices.INTERCOM, intercom_commands.TF_ToneKnob,		436,nil,nil,nil,nil,nil, {100,-30}, {-100,-45})
            ////"PTR-AUDIO1-LVR-THREAT-437"]	= default_axis_limited(_("THREAT Tone Knob"),												devices.INTERCOM, intercom_commands.THREAT_ToneKnob,	437,nil,nil,nil,nil,nil, {110,-30}, {-110,-45})
            ////"PTR-AUDIO2-LVR-INTERCOM-440"]	= default_axis_limited(_("INTERCOM Knob"),													devices.INTERCOM, intercom_commands.INTERCOM_Knob,		440,nil,nil,nil,nil,nil, {100,-30}, {-70,-45})
            ////"PTR-AUDIO2-LVR-TACAN-441"]	= default_axis_limited(_("TACAN Knob"),														devices.INTERCOM, intercom_commands.TACAN_Knob,			441,nil,nil,nil,nil,nil, {100,-30}, {-70,-45})
            ////"PTR-AUDIO2-LVR-ILS-442"]		= default_axis_limited(_("ILS Power Knob"),													devices.INTERCOM, intercom_commands.ILS_PowerKnob,		442,nil,nil,nil,nil,nil, {100,-30}, {-100,-45})
            ////"PTR-AUDIO2-TMB-MODE-443"]		= default_3_position_tumb_small(_("HOT MIC CIPHER Switch, HOT MIC / OFF / CIPHER"),			devices.INTERCOM, intercom_commands.HotMicCipherSw,		443)
            ////"PTR-ZROIZE-TMB-VOICE-696"]	= default_2_position_tumb_small(_("Voice Message Inhibit Switch, VOICE MESSAGE/INHIBIT"),	devices.INTERCOM, intercom_commands.VMS_InhibitSw,		696)
            ////"PTR-ANTICE-TMB-IFF-711"]		= default_3_position_tumb_small(_("IFF ANT SEL Switch, LOWER/NORM/UPPER"),					devices.INTERCOM, intercom_commands.IFF_AntSelSw,		711)
            ////"PTR-ANTICE-TMB-UHF-712"]		= default_3_position_tumb_small(_("UHF ANT SEL Switch, LOWER/NORM/UPPER"),					devices.INTERCOM, intercom_commands.UHF_AntSelSw,		712)
            //#endregion
            //#region  UHF Backup Control Panel
            ////"PTR-ANARC164-CHNL-SELECTOR-410"]		= multiposition_switch_tumb(_("UHF CHAN Knob"),							devices.UHF_CONTROL_PANEL, uhf_commands.ChannelKnob,			410, 20, 0.05, NOT_INVERSED, 0.0, anim_speed_default * 0.05, NOT_CYCLED)
            ////"PTR-ANARC164-100MHZ-SEL-411"]			= multiposition_switch_tumb(_("UHF Manual Frequency Knob 100 MHz"),		devices.UHF_CONTROL_PANEL, uhf_commands.FreqSelector100Mhz,		411,  3,  0.1, NOT_INVERSED, 0.1, anim_speed_default *  0.1, NOT_CYCLED)
            ////"PTR-ANARC164-10MHZ-SEL-412"]			= multiposition_switch_tumb(_("UHF Manual Frequency Knob 10 MHz"),		devices.UHF_CONTROL_PANEL, uhf_commands.FreqSelector10Mhz,		412, 10,  0.1, NOT_INVERSED, 0.0, anim_speed_default *  0.1, NOT_CYCLED)
            ////"PTR-ANARC164-1MHZ-SEL-413"]			= multiposition_switch_tumb(_("UHF Manual Frequency Knob 1 MHz"),		devices.UHF_CONTROL_PANEL, uhf_commands.FreqSelector1Mhz,		413, 10,  0.1, NOT_INVERSED, 0.0, anim_speed_default *  0.1, NOT_CYCLED)
            ////"PTR-ANARC164-0.1MHZ-SEL-414"]			= multiposition_switch_tumb(_("UHF Manual Frequency Knob 0.1 MHz"),		devices.UHF_CONTROL_PANEL, uhf_commands.FreqSelector01Mhz,		414, 10,  0.1, NOT_INVERSED, 0.0, anim_speed_default *  0.1, NOT_CYCLED)
            ////"PTR-ANARC164-0.025MHZ-SEL-415"]		= multiposition_switch_tumb(_("UHF Manual Frequency Knob 0.025 MHz"),	devices.UHF_CONTROL_PANEL, uhf_commands.FreqSelector0025Mhz,	415,  4, 0.25, NOT_INVERSED, 0.0, anim_speed_default *  0.1, NOT_CYCLED)
            ////"PTR-ANARC164-FUNCTION-417"]			= multiposition_switch(_("UHF Function Knob"),						devices.UHF_CONTROL_PANEL, uhf_commands.FunctionKnob,			417,  4,  0.1, NOT_INVERSED, 0.0, anim_speed_default *  0.1, NOT_CYCLED, {100,-30}, {-70,-45})
            ////"PTR-ANARC164-FREQMODE-416"]			= multiposition_switch(_("UHF Mode Knob"),							devices.UHF_CONTROL_PANEL, uhf_commands.FreqModeKnob,			416,  3,  0.1, NOT_INVERSED, 0.0, anim_speed_default *  0.1, NOT_CYCLED, {110,-30}, {-100,-45})
            ////"PTR-ANARC164-T-TONE-418"]				= default_button(_("UHF Tone Button"),								devices.UHF_CONTROL_PANEL, uhf_commands.TToneSw,				418)
            ////"PTR-ANARC164-T-TONE-418"].sound 	= {{SOUND_SW3, SOUND_SW3_OFF}, {SOUND_SW3, SOUND_SW3_OFF}}
            ////"PTR-ANARC164-SQUELCH-419"]			= default_2_position_tumb_small(_("UHF SQUELCH Switch"),			devices.UHF_CONTROL_PANEL, uhf_commands.SquelchSw,				419)
            ////"PTR-ANARC164-VOLUME-420"]				= default_axis_limited(_("UHF VOL Knob"),							devices.UHF_CONTROL_PANEL, uhf_commands.VolumeKnob,				420,nil,nil,nil,nil,nil, {100,-30}, {-90,-45})
            ////"PTR-ANARC164-TEST-DISPLAY-421"]		= default_button(_("UHF TEST DISPLAY Button"),						devices.UHF_CONTROL_PANEL, uhf_commands.TestDisplayBtn,			421)
            ////"PTR-ANARC164-STATUS-422"]				= default_button(_("UHF STATUS Button"),							devices.UHF_CONTROL_PANEL, uhf_commands.StatusBtn,				422)
            ////"PTR_ANARC164-CHNL-SELECTOR01-734"]	= default_2_position_tumb(_("Access Door, OPEN/CLOSE"),				devices.UHF_CONTROL_PANEL, uhf_commands.AccessDoor,				734, anim_speed_default * 0.5)
            ////"PTR_ANARC164-CHNL-SELECTOR01-734"].side = {{BOX_SIDE_Y_top},{BOX_SIDE_Y_bottom}}
            //#endregion
            //#region IFF Control Panel
            ////"PTR-AUXCOM-TMB-CNI-542"]		= multiposition_switch(_("C & I Knob, UFC/BACKUP"),							devices.IFF_CONTROL_PANEL, iff_commands.CNI_Knob,		542, 2, 1, NOT_INVERSED, 0.0, anim_speed_default, NOT_CYCLED, nil, {45,-45})
            ////"PTR-AUXCOM-LVR-MODE-540"]		= multiposition_switch(_("IFF MASTER Knob, OFF/STBY/LOW/NORM/EMER"),		devices.IFF_CONTROL_PANEL, iff_commands.MasterKnob,		540, 5, 0.1, NOT_INVERSED, 0.0, anim_speed_default * 0.1, NOT_CYCLED, nil, {45,-45})
            ////"PTR-AUXCOM-TMB-M4-541"]		= default_3_position_tumb(_("IFF M-4 CODE Switch, HOLD/ A/B /ZERO"),		devices.IFF_CONTROL_PANEL, iff_commands.M4CodeSw,		541)
            ////"PTR-AUXCOM-TMB-REPLY-543"]	= default_3_position_tumb(_("IFF MODE 4 REPLY Switch, OUT/A/B"),			devices.IFF_CONTROL_PANEL, iff_commands.M4ReplySw,		543)
            ////"PTR-AUXCOM-TMB-MONITOR-544"]	= default_2_position_tumb_small(_("IFF MODE 4 MONITOR Switch, OUT/AUDIO"),	devices.IFF_CONTROL_PANEL, iff_commands.M4MonitorSw,	544)
            ////"PTR-AUXCOM-TMB-TACAN-553"]	= default_3_position_tumb_small(_("IFF ENABLE Switch, M1/M3 /OFF/ M3/MS"),	devices.IFF_CONTROL_PANEL, iff_commands.EnableSw,		553)

            ////"PTR-AUXCOM-TMB-CHNL1-545"]	= springloaded_3_pos_tumb(_("IFF MODE 1 Selector Lever, DIGIT 1"),		devices.IFF_CONTROL_PANEL, iff_commands.M1M3Selector1_Dec, iff_commands.M1M3Selector1_Inc, 545)
            ////"PTR-AUXCOM-TMB-CHNL1-545"].sound = {{SOUND_SW5_OFF, SOUND_SW7}, {SOUND_SW7, SOUND_SW5_OFF}}
            ////"PTR-AUXCOM-TMB-CHNL2-547"]	= springloaded_3_pos_tumb(_("IFF MODE 1 Selector Lever, DIGIT 2"),		devices.IFF_CONTROL_PANEL, iff_commands.M1M3Selector2_Dec, iff_commands.M1M3Selector2_Inc, 547)
            ////"PTR-AUXCOM-TMB-CHNL2-547"].sound = {{SOUND_SW5_OFF, SOUND_SW7}, {SOUND_SW7, SOUND_SW5_OFF}}
            ////"PTR-AUXCOM-TMB-CHNL3-549"]	= springloaded_3_pos_tumb(_("IFF MODE 3 Selector Lever, DIGIT 1"),		devices.IFF_CONTROL_PANEL, iff_commands.M1M3Selector3_Dec, iff_commands.M1M3Selector3_Inc, 549)
            ////"PTR-AUXCOM-TMB-CHNL3-549"].sound = {{SOUND_SW5_OFF, SOUND_SW7}, {SOUND_SW7, SOUND_SW5_OFF}}
            ////"PTR-AUXCOM-TMB-CHNL4-551"]	= springloaded_3_pos_tumb(_("IFF MODE 3 Selector Lever, DIGIT 2"),		devices.IFF_CONTROL_PANEL, iff_commands.M1M3Selector4_Dec, iff_commands.M1M3Selector4_Inc, 551)
            ////"PTR-AUXCOM-TMB-CHNL4-551"].sound = {{SOUND_SW5_OFF, SOUND_SW7}, {SOUND_SW7, SOUND_SW5_OFF}}
            //#endregion
            //#region KY-58
            ////"PTR-KY58-LVR-MODE1-705"]		= multiposition_switch(_("KY-58 MODE Knob, P/C/LD/RV"),						devices.KY58, ky58_commands.KY58_ModeSw,			705, 4, 0.1, NOT_INVERSED, 0.0, anim_speed_default * 0.1, NOT_CYCLED, {135,-30}, {45,-30})
            ////"PTR-KY58-LVR-VOL-708"]		= default_axis_limited(_("KY-58 VOLUME Knob"),								devices.KY58, ky58_commands.KY58_Volume,			708, 0.0, 0.1, false, false, {0,1}, {135,-30}, {45,-30})
            ////"PTR-KY58-LVR-MODE2-706"]		= multiposition_switch(_("KY-58 FILL Knob, Z 1-5/1/2/3/4/5/6/Z ALL"),		devices.KY58, ky58_commands.KY58_FillSw,			706, 8, 0.1, NOT_INVERSED, 0.0, anim_speed_default * 0.1, NOT_CYCLED, {135,-30}, {45,-30})
            ////"PTR-KY58-LVR-MODE3-707"]		= multiposition_switch(_("KY-58 Power Knob, OFF/ON/TD"),					devices.KY58, ky58_commands.KY58_PowerSw,			707, 3, 0.5, NOT_INVERSED, 0.0, anim_speed_default * 0.5, NOT_CYCLED, {135,-30}, {45,-30})

            ////"PTR-NUCLR-TMB-PLAIN-701"]		= default_3_position_tumb_small(_("PLAIN Cipher Switch, CRAD 1/PLAIN/CRAD 2"),	devices.INTERCOM, intercom_commands.PlainCipherSw,	701)
            ////"PTR-ZROIZE-CVR-ZERO-694"]		= default_red_cover(_("ZEROIZE Switch Cover, OPEN/CLOSE"),					devices.INTERCOM, intercom_commands.ZeroizeSwCvr,	    694)
            ////"PTR-ZROIZE-TMB-ZERO-695"]		= default_3_position_tumb_small(_("ZEROIZE Switch, OFP/OFF/DATA"),				devices.INTERCOM, intercom_commands.ZeroizeSw,		695)
            //#endregion
            //#region HMCS
            ////"PTR-HMDP-LVR-SMBINT-392"]		= default_axis_limited(_("HMCS SYMBOLOGY INT Knob"),	devices.HMCS, hmcs_commands.IntKnob,	392,nil,nil,nil,nil,nil,{100, -60}, {-45,-55})
            //#endregion
            //#region RWR
            ////"PTR-CMSC-LVR-BRT-140"]	= default_axis_limited(_("RWR Intensity Knob - Rotate to adjust brightness"),	devices.RWR, rwr_commands.IntKnob,		140, 0, 0.1, NOT_UPDATABLE, NOT_RELATIVE, {0, 0.8},{95, -35},{80, -35})
            ////"PTR-CMSC-BTN-HNDOFF-141"]	= short_way_button(_("RWR Indicator Control HANDOFF Button"),					devices.RWR, rwr_commands.Handoff,		141)
            ////"PTR-CMSC-BTN-LNCH-143"]	= short_way_button(_("RWR Indicator Control LAUNCH Button"),					devices.RWR, rwr_commands.Launch,		143)
            ////"PTR-CMSC-BTN-MODE-145"]	= short_way_button(_("RWR Indicator Control MODE Button"),						devices.RWR, rwr_commands.Mode,			145)
            ////"PTR-CMSC-BTN-OBJ-147"]	= short_way_button(_("RWR Indicator Control UNKNOWN SHIP Button"),				devices.RWR, rwr_commands.UnknownShip,	147)
            ////"PTR-CMSC-BTN-STEST-149"]	= short_way_button(_("RWR Indicator Control SYS TEST Button"),					devices.RWR, rwr_commands.SysTest,		149)
            ////"PTR-CMSC-BTN-T-151"]		= short_way_button(_("RWR Indicator Control T Button"),							devices.RWR, rwr_commands.TgtSep,		151)

            ////"PTR-TWAP-LVR-DIM-404"]	= default_axis_limited(_("RWR Indicator Control DIM Knob - Rotate to adjust brightness"),	devices.RWR, rwr_commands.BrtKnob,		404,nil,nil,nil,nil,nil,{100, -60})
            ////"PTR-TWAP-BTN-SRCH-395"]	= short_way_button(_("RWR Indicator Control SEARCH Button"),								devices.RWR, rwr_commands.Search,		395)
            ////"PTR-TWAP-BTN-ACTPWR-397"]	= short_way_button(_("RWR Indicator Control ACT/PWR Button"),								devices.RWR, rwr_commands.ActPwr,		397)
            ////"PTR-TWAP-BTN-ALT-399"]	= short_way_button(_("RWR Indicator Control ALTITUDE Button"),								devices.RWR, rwr_commands.Altitude,		399)
            ////"PTR-TWAP-BTN-PWR-401"]	= default_2_position_tumb(_("RWR Indicator Control POWER Button"),							devices.RWR, rwr_commands.Power,		401)
            ////"PTR-TWAP-BTN-PWR-401"].sound    = {{SOUND_SW4_ON, SOUND_SW4_OFF}}
            ////"PTR-TWAP-BTN-PWR-401"].class_vr = {class_type.BTN_FIX}
            ////"PTR-TWAP-BTN-PWR-401"].side     = {{BOX_SIDE_Y_bottom}}
            //#endregion
            //#region CMDS
            ////"PTR-LSIDE-BTN-CHAFF-604"]		= default_button(_("CHAFF/FLARE Dispense Button - Push to dispense"),			devices.CMDS, cmds_commands.DispBtn,	604)
            ////"PTR-LPAN-CMDS-TMB-RWR-375"]	= default_2_position_tumb_small(_("RWR 555 Switch, ON/OFF"),					devices.CMDS, cmds_commands.RwrSrc,		375)
            ////"PTR-LPAN-CMDS-TMB-JMR-374"]	= default_2_position_tumb_small(_("JMR Source Switch, ON/OFF"),					devices.CMDS, cmds_commands.JmrSrc,		374)
            ////"PTR-LPAN-CMDS-TMB-MWS-373"]	= default_2_position_tumb_small(_("MWS Source Switch, ON/OFF (no function)"),	devices.CMDS, cmds_commands.MwsSrc,		373)
            ////"PTR-LPAN-CMDS-TMB-JTSN-371"]	= default_2_position_tumb(_("Jettison Switch, JETT/OFF"),						devices.CMDS, cmds_commands.Jett,		371)
            ////"PTR-LPAN-CMDS-BTN-OSB1-365"]	= default_2_position_tumb_small(_("O1 Expendable Category Switch, ON/OFF"),		devices.CMDS, cmds_commands.O1Exp,		365)
            ////"PTR-LPAN-CMDS-BTN-OSB2-366"]	= default_2_position_tumb_small(_("O2 Expendable Category Switch, ON/OFF"),		devices.CMDS, cmds_commands.O2Exp,		366)
            ////"PTR-LPAN-CMDS-BTN-OSB3-367"]	= default_2_position_tumb_small(_("CH Expendable Category Switch, ON/OFF"),		devices.CMDS, cmds_commands.ChExp,		367)
            ////"PTR-LPAN-CMDS-BTN-OSB4-368"]	= default_2_position_tumb_small(_("FL Expendable Category Switch, ON/OFF"),		devices.CMDS, cmds_commands.FlExp,		368)
            ////"PTR-LPAN-CMDS-LVR-PRGM-377"]	= multiposition_switch(_("PROGRAM Knob, BIT/1/2/3/4"),					devices.CMDS, cmds_commands.Prgm,	377, 5, 0.1, NOT_INVERSED, 0.0, anim_speed_default, NOT_CYCLED, {100, -60}, {-45,-55})
            ////"PTR-LPAN-CMDS-LVR-MODE-378"]	= multiposition_switch(_("MODE Knob, OFF/STBY/MAN/SEMI/AUTO/BYP"),		devices.CMDS, cmds_commands.Mode,	378, 6, 0.1, NOT_INVERSED, 0.0, anim_speed_default, NOT_CYCLED, {120, -60}, {-45,-55})
            //#endregion
            //#region MFD Left
            ////"PTR-LMFCD-BTN-OSB1-300"]		= mfd_button(_("Left MFD OSB 1"),											devices.MFD_LEFT, mfd_commands.OSB_1,				300)
            ////"PTR-LMFCD-BTN-OSB2-301"]		= mfd_button(_("Left MFD OSB 2"),											devices.MFD_LEFT, mfd_commands.OSB_2,				301)
            ////"PTR-LMFCD-BTN-OSB3-302"]		= mfd_button(_("Left MFD OSB 3"),											devices.MFD_LEFT, mfd_commands.OSB_3,				302)
            ////"PTR-LMFCD-BTN-OSB4-303"]		= mfd_button(_("Left MFD OSB 4"),											devices.MFD_LEFT, mfd_commands.OSB_4,				303)
            ////"PTR-LMFCD-BTN-OSB5-304"]		= mfd_button(_("Left MFD OSB 5"),											devices.MFD_LEFT, mfd_commands.OSB_5,				304)
            ////"PTR-LMFCD-BTN-OSB6-305"]		= mfd_button(_("Left MFD OSB 6"),											devices.MFD_LEFT, mfd_commands.OSB_6,				305)
            ////"PTR-LMFCD-BTN-OSB7-306"]		= mfd_button(_("Left MFD OSB 7"),											devices.MFD_LEFT, mfd_commands.OSB_7,				306)
            ////"PTR-LMFCD-BTN-OSB8-307"]		= mfd_button(_("Left MFD OSB 8"),											devices.MFD_LEFT, mfd_commands.OSB_8,				307)
            ////"PTR-LMFCD-BTN-OSB9-308"]		= mfd_button(_("Left MFD OSB 9"),											devices.MFD_LEFT, mfd_commands.OSB_9,				308)
            ////"PTR-LMFCD-BTN-OSB10-309"]		= mfd_button(_("Left MFD OSB 10"),											devices.MFD_LEFT, mfd_commands.OSB_10,				309)
            ////"PTR-LMFCD-BTN-OSB11-310"]		= mfd_button(_("Left MFD OSB 11"),											devices.MFD_LEFT, mfd_commands.OSB_11,				310)
            ////"PTR-LMFCD-BTN-OSB12-311"]		= mfd_button(_("Left MFD OSB 12"),											devices.MFD_LEFT, mfd_commands.OSB_12,				311)
            ////"PTR-LMFCD-BTN-OSB13-312"]		= mfd_button(_("Left MFD OSB 13"),											devices.MFD_LEFT, mfd_commands.OSB_13,				312)
            ////"PTR-LMFCD-BTN-OSB14-313"]		= mfd_button(_("Left MFD OSB 14"),											devices.MFD_LEFT, mfd_commands.OSB_14,				313)
            ////"PTR-LMFCD-BTN-OSB15-314"]		= mfd_button(_("Left MFD OSB 15"),											devices.MFD_LEFT, mfd_commands.OSB_15,				314)
            ////"PTR-LMFCD-BTN-OSB16-315"]		= mfd_button(_("Left MFD OSB 16"),											devices.MFD_LEFT, mfd_commands.OSB_16,				315)
            ////"PTR-LMFCD-BTN-OSB17-316"]		= mfd_button(_("Left MFD OSB 17"),											devices.MFD_LEFT, mfd_commands.OSB_17,				316)
            ////"PTR-LMFCD-BTN-OSB18-317"]		= mfd_button(_("Left MFD OSB 18"),											devices.MFD_LEFT, mfd_commands.OSB_18,				317)
            ////"PTR-LMFCD-BTN-OSB19-318"]		= mfd_button(_("Left MFD OSB 19"),											devices.MFD_LEFT, mfd_commands.OSB_19,				318)
            ////"PTR-LMFCD-BTN-OSB20-319"]		= mfd_button(_("Left MFD OSB 20"),											devices.MFD_LEFT, mfd_commands.OSB_20,				319)
            ////"PTR-LMFCD-RS-GAIN-UP-320"]	= Rocker_switch_positive(_("Left MFD GAIN Rocker Switch, Up/Increase"),		devices.MFD_LEFT, mfd_commands.GAIN_Rocker_UP, 		320)
            ////"PTR-LMFCD-RS-GAIN-DN-320"]	= Rocker_switch_negative(_("Left MFD GAIN Rocker Switch, Down/Decrease"),	devices.MFD_LEFT, mfd_commands.GAIN_Rocker_DOWN, 	320)
            ////"PTR-LMFCD-RS-SYM-UP-321"]		= Rocker_switch_positive(_("Left MFD SYM Rocker Switch, Up/Increase"),		devices.MFD_LEFT, mfd_commands.SYM_Rocker_UP, 		321)
            ////"PTR-LMFCD-RS-SYM-DN-321"]		= Rocker_switch_negative(_("Left MFD SYM Rocker Switch, Down/Decrease"),	devices.MFD_LEFT, mfd_commands.SYM_Rocker_DOWN, 	321)
            ////"PTR-LMFCD-RS-CON-UP-322"]		= Rocker_switch_positive(_("Left MFD CON Rocker Switch, Up/Increase"),		devices.MFD_LEFT, mfd_commands.CON_Rocker_UP, 		322)
            ////"PTR-LMFCD-RS-CON-DN-322"]		= Rocker_switch_negative(_("Left MFD CON Rocker Switch, Down/Decrease"),	devices.MFD_LEFT, mfd_commands.CON_Rocker_DOWN, 	322)
            ////"PTR-LMFCD-RS-BRT-UP-323"]		= Rocker_switch_positive(_("Left MFD BRT Rocker Switch, Up/Increase"),		devices.MFD_LEFT, mfd_commands.BRT_Rocker_UP, 		323)
            ////"PTR-LMFCD-RS-BRT-DN-323"]		= Rocker_switch_negative(_("Left MFD BRT Rocker Switch, Down/Decrease"),	devices.MFD_LEFT, mfd_commands.BRT_Rocker_DOWN, 	323)
            //#endregion
            //#region MFD Right
            ////"PTR-RMFCD-BTN-OSB1-326"]		= mfd_button(_("Right MFD OSB 1"),											devices.MFD_RIGHT, mfd_commands.OSB_1,				326)
            ////"PTR-RMFCD-BTN-OSB2-327"]		= mfd_button(_("Right MFD OSB 2"),											devices.MFD_RIGHT, mfd_commands.OSB_2,				327)
            ////"PTR-RMFCD-BTN-OSB3-328"]		= mfd_button(_("Right MFD OSB 3"),											devices.MFD_RIGHT, mfd_commands.OSB_3,				328)
            ////"PTR-RMFCD-BTN-OSB4-329"]		= mfd_button(_("Right MFD OSB 4"),											devices.MFD_RIGHT, mfd_commands.OSB_4,				329)
            ////"PTR-RMFCD-BTN-OSB5-330"]		= mfd_button(_("Right MFD OSB 5"),											devices.MFD_RIGHT, mfd_commands.OSB_5,				330)
            ////"PTR-RMFCD-BTN-OSB6-331"]		= mfd_button(_("Right MFD OSB 6"),											devices.MFD_RIGHT, mfd_commands.OSB_6,				331)
            ////"PTR-RMFCD-BTN-OSB7-332"]		= mfd_button(_("Right MFD OSB 7"),											devices.MFD_RIGHT, mfd_commands.OSB_7,				332)
            ////"PTR-RMFCD-BTN-OSB8-333"]		= mfd_button(_("Right MFD OSB 8"),											devices.MFD_RIGHT, mfd_commands.OSB_8,				333)
            ////"PTR-RMFCD-BTN-OSB9-334"]		= mfd_button(_("Right MFD OSB 9"),											devices.MFD_RIGHT, mfd_commands.OSB_9,				334)
            ////"PTR-RMFCD-BTN-OSB10-335"]		= mfd_button(_("Right MFD OSB 10"),											devices.MFD_RIGHT, mfd_commands.OSB_10,				335)
            ////"PTR-RMFCD-BTN-OSB11-336"]		= mfd_button(_("Right MFD OSB 11"),											devices.MFD_RIGHT, mfd_commands.OSB_11,				336)
            ////"PTR-RMFCD-BTN-OSB12-337"]		= mfd_button(_("Right MFD OSB 12"),											devices.MFD_RIGHT, mfd_commands.OSB_12,				337)
            ////"PTR-RMFCD-BTN-OSB13-338"]		= mfd_button(_("Right MFD OSB 13"),											devices.MFD_RIGHT, mfd_commands.OSB_13,				338)
            ////"PTR-RMFCD-BTN-OSB14-339"]		= mfd_button(_("Right MFD OSB 14"),											devices.MFD_RIGHT, mfd_commands.OSB_14,				339)
            ////"PTR-RMFCD-BTN-OSB15-340"]		= mfd_button(_("Right MFD OSB 15"),											devices.MFD_RIGHT, mfd_commands.OSB_15,				340)
            ////"PTR-RMFCD-BTN-OSB16-341"]		= mfd_button(_("Right MFD OSB 16"),											devices.MFD_RIGHT, mfd_commands.OSB_16,				341)
            ////"PTR-RMFCD-BTN-OSB17-342"]		= mfd_button(_("Right MFD OSB 17"),											devices.MFD_RIGHT, mfd_commands.OSB_17,				342)
            ////"PTR-RMFCD-BTN-OSB18-343"]		= mfd_button(_("Right MFD OSB 18"),											devices.MFD_RIGHT, mfd_commands.OSB_18,				343)
            ////"PTR-RMFCD-BTN-OSB19-344"]		= mfd_button(_("Right MFD OSB 19"),											devices.MFD_RIGHT, mfd_commands.OSB_19,				344)
            ////"PTR-RMFCD-BTN-OSB20-345"]		= mfd_button(_("Right MFD OSB 20"),											devices.MFD_RIGHT, mfd_commands.OSB_20,				345)
            ////"PTR-RMFCD-RS-GAIN-UP-346"]	= Rocker_switch_positive(_("Left MFD GAIN Rocker Switch, Up/Increase"),		devices.MFD_RIGHT, mfd_commands.GAIN_Rocker_UP, 	346)
            ////"PTR-RMFCD-RS-GAIN-DN-346"]	= Rocker_switch_negative(_("Left MFD GAIN Rocker Switch, Down/Decrease"),	devices.MFD_RIGHT, mfd_commands.GAIN_Rocker_DOWN, 	346)
            ////"PTR-RMFCD-RS-SYM-UP-347"]		= Rocker_switch_positive(_("Left MFD SYM Rocker Switch, Up/Increase"),		devices.MFD_RIGHT, mfd_commands.SYM_Rocker_UP, 		347)
            ////"PTR-RMFCD-RS-SYM-DN-347"]		= Rocker_switch_negative(_("Left MFD SYM Rocker Switch, Down/Decrease"),	devices.MFD_RIGHT, mfd_commands.SYM_Rocker_DOWN, 	347)
            ////"PTR-RMFCD-RS-CON-UP-348"]		= Rocker_switch_positive(_("Left MFD CON Rocker Switch, Up/Increase"),		devices.MFD_RIGHT, mfd_commands.CON_Rocker_UP, 		348)
            ////"PTR-RMFCD-RS-CON-DN-348"]		= Rocker_switch_negative(_("Left MFD CON Rocker Switch, Down/Decrease"),	devices.MFD_RIGHT, mfd_commands.CON_Rocker_DOWN, 	348)
            ////"PTR-RMFCD-RS-BRT-UP-349"]		= Rocker_switch_positive(_("Left MFD BRT Rocker Switch, Up/Increase"),		devices.MFD_RIGHT, mfd_commands.BRT_Rocker_UP, 		349)
            ////"PTR-RMFCD-RS-BRT-DN-349"]		= Rocker_switch_negative(_("Left MFD BRT Rocker Switch, Down/Decrease"),	devices.MFD_RIGHT, mfd_commands.BRT_Rocker_DOWN, 	349)
            //#endregion
            //// Instruments
            //#region  Airspeed/Mach Indicator
            ////"PTR-SPD-LVR-SET-71"]			= default_axis(_("SET INDEX Knob"), devices.AMI, ami_commands.SettingKnob, 71, 0, 0.05, true, true, true, {135,-45},{45,-45})
            //#endregion
            //#region Altimeter
            ////"PTR-ALT-LVR-SET-62"]			= default_axis(_("Barometric Setting Knob"),				devices.AAU34, alt_commands.ZERO, 62, 0.4, 0.2, true, true, true,{135,-45},{45,-45})
            ////"PTR-ALT-TMB-MODE-60"]			= springloaded_3_pos_tumb(_("Mode Lever, ELEC/OFF/PNEU"),	devices.AAU34, alt_commands.ELEC, alt_commands.PNEU, 60)
            ////"PTR-ALT-TMB-MODE-60"].sound 	= {{SOUND_SW3, SOUND_SW3_OFF}, {SOUND_SW3, SOUND_SW3_OFF}}
            //#endregion
            //#region SAI ARU-42/A-2
            ////"PTR-SAI-LVR-CAGE-P-66"] =
            ////            {

            ////    class               = {class_type.BTN, class_type.LEV
            ////    },
            ////	hint				= _("SAI Cage Knob, (LMB) Pull to cage /(MW) Adjust aircraft reference symbol"),
            ////	device				= devices.SAI,
            ////	action				= {sai_commands.cage, sai_commands.reference
            ////},
            ////	stop_action = { sai_commands.cage, 0},
            ////	is_repeatable = { },
            ////	arg = { 67, 66},
            ////	arg_value = { 1.0, 1.0},
            ////	arg_lim = { { 0, 1}, { 0.375, 0.625} },
            ////	relative = { false, false},
            ////	cycle = false,
            ////	gain = { 1.0, 0.02},
            ////	use_release_message = { true, false},
            ////	sound = { { SOUND_SW12} },
            ////	use_OBB = true,
            ////	side = { { BOX_SIDE_Y_bottom},{ BOX_SIDE_X_top, BOX_SIDE_X_bottom, BOX_SIDE_Z_top, BOX_SIDE_Z_bottom} },
            ////	attach_left = { 90, -45},
            ////	attach_right = { 45, -45}
            ////}
            //#endregion
            //#region ADI
            ////"PTR-ADI-LVR-KNOB-22"]		= default_axis_limited(_("Pitch Trim Knob"),			devices.ADI, device_commands.Button_1, 22)
            //#endregion
            //#region EHSI
            ////"PTR-EHSI-LVR-CRS-44"]	= default_button_knob(_("CRS Set / Brightness Control Knob"),	devices.EHSI, ehsi_commands.RightKnobBtn, ehsi_commands.RightKnob, 43, 44)
            ////"PTR-EHSI-LVR-CRS-44"].sound = {{SOUND_SW4_ON, SOUND_SW4_OFF}}
            ////"PTR-EHSI-LVR-HDG-45"]	= default_button_knob(_("HDG Set Knob"),						devices.EHSI, ehsi_commands.LeftKnobBtn, ehsi_commands.LeftKnob, 42, 45)
            ////"PTR-EHSI-LVR-HDG-45"].sound = {{SOUND_SW4_ON, SOUND_SW4_OFF}}
            ////"PTR-EHSI-BTN-M-46"]	= default_button(_("Mode (M) Button"),							devices.EHSI, ehsi_commands.ModeBtn, 46)
            //#endregion
            //#region Clock
            ////"PTR-SNSR-TMB-RDR-674"] =
            ////{

            ////    class               = { class_type.BTN, class_type.LEV},
            ////	hint = _("Clock Winding and Setting Knob"), 
            ////	device = devices.CLOCK, 
            ////	action = { clock_commands.CLOCK_left_lev_up, clock_commands.CLOCK_left_lev_rotate}, 
            ////	stop_action = { clock_commands.CLOCK_left_lev_up, 0},
            ////	is_repeatable = { },
            ////	arg = { 626, 625}, 
            ////	arg_value = { 1.0, 0.04}, 
            ////	arg_lim = { { 0, 1}, { 0, 1} }, 
            ////	relative = { false,true}, 
            ////	gain = { 1.0, 0.6}, 
            ////	animated = { true, false},
            ////	animation_speed = { anim_speed_default, 0.0},
            ////	use_release_message = { true, false}, 
            ////	use_OBB = true,
            ////	sound = { { SOUND_SW11} }
            ////}

            ////"PTR-RPAN-WATCH-BTN-CTRL-627"] = default_button(_("Clock Elapsed Time Knob"), devices.CLOCK, clock_commands.CLOCK_right_lev_down, 628)
            //#endregion
            //#region Cockpit Mechanics
            ////"PTR-LARMS-CNPOPN-600"]		= default_2_position_tumb(_("Canopy Handle, UP/DOWN"),					devices.CPT_MECH, cpt_commands.CanopyHandle, 600)
            ////"PTR-LARMS-CNPOPN-600"].sound = {{SOUND_SW9_UP, SOUND_SW9_DOWN}}
            ////"PTR-SEAT-TMB-ADJ-786"]		= springloaded_3_pos_tumb_small(_("SEAT ADJ Switch, UP/OFF/DOWN"),		devices.CPT_MECH, cpt_commands.SeatAdjSwitchDown, cpt_commands.SeatAdjSwitchUp, 786)
            ////"PTR-LARMS-CNPJETT-601"]		= default_2_position_tumb(_("CANOPY JETTISON T-Handle, PULL/STOW"),		devices.CPT_MECH, cpt_commands.CanopyTHandle, 601)
            ////"PTR-LARMS-CNPJETT-601"].sound = {{SOUND_SW11_UP, SOUND_SW11_DOWN}}
            ////"PTR-SEAT-ARM-LOCK-785"]		= default_2_position_tumb(_("Ejection Safety Lever, ARMED/LOCKED"),		devices.CPT_MECH, cpt_commands.EjectionSafetyLever, 785)
            ////"PTR-SEAT-ARM-LOCK-785"].sound = {{SOUND_SW10_DOWN,SOUND_SW10_UP}}
            ////"PTR-LARMS-TMB-OPN-606"]		= default_button_tumb(_("Canopy Switch, OPEN/HOLD/CLOSE(momentarily)"),	devices.CPT_MECH, cpt_commands.CanopySwitchClose, cpt_commands.CanopySwitchOpen, 606)

            ////"PTR-STICK-HIDE-796"]			= default_2_position_tumb(_("Hide Stick toggle"),						devices.CPT_MECH, cpt_commands.StickHide, 796)
            //#endregion
            //#region ECM
            ////"PTR-ECM-TMB-OPR-455"]			= default_3_position_tumb(_("ECM Power Switch"),						devices.ECM_INTERFACE, ecm_commands.PwrSw, 455, false, anim_speed_default, false)
            ////"PTR-ECM-LVR-DIM-456"]			= default_axis_limited_1_side(_("ECM DIM Knob"),						devices.ECM_INTERFACE, ecm_commands.DimRotary, 456)
            ////"PTR-ECM-TMB-XMIT-457"]		= default_3_position_tumb(_("ECM XMIT Switch"),							devices.ECM_INTERFACE, ecm_commands.XmitSw,	457, false, anim_speed_default, false)
            ////"PTR-ECM-BTN-RESET-458"]		= default_button(_("ECM RESET Button"),									devices.ECM_INTERFACE, ecm_commands.ResetBtn, 458)
            ////"PTR-ECM-BTN-BIT-459"]			= default_button(_("ECM BIT Button"),									devices.ECM_INTERFACE, ecm_commands.BitBtn, 459)
            ////"PTR-ECM-BTN-1-460"]			= default_2_position_tumb(_("ECM 1 Button"),							devices.ECM_INTERFACE, ecm_commands.OneBtn, 460)
            ////"PTR-ECM-BTN-2-465"]			= default_2_position_tumb(_("ECM 2 Button"),							devices.ECM_INTERFACE, ecm_commands.TwoBtn, 465)
            ////"PTR-ECM-BTN-3-470"]			= default_2_position_tumb(_("ECM 3 Button"),							devices.ECM_INTERFACE, ecm_commands.ThreeBtn, 470)
            ////"PTR-ECM-BTN-4-475"]			= default_2_position_tumb(_("ECM 4 Button"),							devices.ECM_INTERFACE, ecm_commands.FourBtn, 475)
            ////"PTR-ECM-BTN-5-480"]			= default_2_position_tumb(_("ECM 5 Button"),							devices.ECM_INTERFACE, ecm_commands.FiveBtn, 480)
            ////"PTR-ECM-BTN-6-485"]			= default_2_position_tumb(_("ECM 6 Button"),							devices.ECM_INTERFACE, ecm_commands.SixBtn, 485)
            ////"PTR-ECM-BTN-FRM-490"]			= default_2_position_tumb(_("ECM FRM Button"),							devices.ECM_INTERFACE, ecm_commands.FrmBtn, 490)
            ////"PTR-ECM-BTN-SPL-495"]			= default_2_position_tumb(_("ECM SPL Button"),							devices.ECM_INTERFACE, ecm_commands.SplBtn, 495)
            //#endregion
        }
    }
}
