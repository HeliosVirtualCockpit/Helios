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
    using System.IO;
    using System.Collections.Generic;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Interfaces.DCS.F16C.Functions;
    using NLog;
    using System.Windows.Controls;

    //[HeliosInterface(
    //"Helios.F-16C_50",                          // Helios internal type ID used in Profile XML, must never change
    //    "DCS F-16C Viper (Helios Version)",     // human readable UI name for this interface
    //    typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
    //    typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
    //    UniquenessKey = "Helios.DCSInterface")] // all other DCS interfaces exclude this interface

    public class F16CInterface : DCSInterface
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _dcsPath = $@"{Environment.GetEnvironmentVariable("ProgramFiles")}\Eagle Dynamics\DCS World.openbeta\Mods\Aircraft";

        public F16CInterface(string name)
            : base(name, "F-16C_50", "pack://application:,,,/F-16C;component/Interfaces/ExportFunctions.lua")
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
                AddFunction(new FlagValue(this, lamp.ToString(), "Caution Lights", Enum.GetName(typeof(CautionLights),lamp), "True if indicator lit"));
            }
            foreach (int lamp in Enum.GetValues(typeof(RWRLights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "RWR Lights", Enum.GetName(typeof(RWRLights), lamp), "True if indicator lit"));
            }
            foreach (int lamp in Enum.GetValues(typeof(CmdsLights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "CMDS Lights", Enum.GetName(typeof(CmdsLights), lamp), "True if indicator lit"));
            }
            foreach (int lamp in Enum.GetValues(typeof(InternalLights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "Internal Lights", Enum.GetName(typeof(InternalLights), lamp), "True if indicator lit"));
            }
            foreach (int lamp in Enum.GetValues(typeof(ECM_Button_Lights)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "ECM Lights", Enum.GetName(typeof(ECM_Button_Lights), lamp), "True if indicator lit"));
            }
            foreach (int lamp in Enum.GetValues(typeof(controllers)))
            {
                AddFunction(new FlagValue(this, lamp.ToString(), "Controller Lights", Enum.GetName(typeof(controllers), lamp), "True if indicator lit"));
            }
#if (CREATEINTERFACE && DEBUG)
            DcsPath = Path.Combine(Environment.GetEnvironmentVariable("userprofile"), "Desktop","DCSLua");
            AddFunctionsFromDCSModule(new F16InterfaceCreator());
            return;
#else
            #region Control Interface
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "566", new SwitchPosition[] { new SwitchPosition("1.0", "OFF", F16CCommands.controlCommands.DigitalBackup.ToString("d")), new SwitchPosition("0.0", "BACKUP", F16CCommands.controlCommands.DigitalBackup.ToString("d")) }, "Control Interface", "DIGITAL BACKUP Switch, OFF/BACKUP", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "567", new SwitchPosition[] { new SwitchPosition("1.0", "NORM", F16CCommands.controlCommands.AltFlaps.ToString("d")), new SwitchPosition("0.0", "EXTEND", F16CCommands.controlCommands.AltFlaps.ToString("d")) }, "Control Interface", "ALT FLAPS Switch, NORM/EXTEND", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "574", new SwitchPosition[] { new SwitchPosition("1.0", "OFF", F16CCommands.controlCommands.BitSw.ToString("d")), new SwitchPosition("0.0", "BIT", F16CCommands.controlCommands.BitSw.ToString("d")) }, "Control Interface", "BIT Switch, OFF/BIT", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "573", new SwitchPosition[] { new SwitchPosition("1.0", "OFF", F16CCommands.controlCommands.FlcsReset.ToString("d")), new SwitchPosition("0.0", "RESET", F16CCommands.controlCommands.FlcsReset.ToString("d")) }, "Control Interface", "FLCS RESET Switch, OFF/RESET", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "572", new SwitchPosition[] { new SwitchPosition("1.0", "AUTO", F16CCommands.controlCommands.LeFlaps.ToString("d")), new SwitchPosition("0.0", "LOCK", F16CCommands.controlCommands.LeFlaps.ToString("d")) }, "Control Interface", "LE FLAPS Switch, AUTO/LOCK", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "564", new SwitchPosition[] { new SwitchPosition("1.0", "DISC", F16CCommands.controlCommands.TrimApDisc.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.controlCommands.TrimApDisc.ToString("d")) }, "Control Interface", "TRIM/AP DISC Switch, DISC/NORM", "%0.1f"));
            AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.RollTrim.ToString("d"), "560", 0.1d, 0.0d, 1.0d, "Control Interface", "ROLL TRIM Wheel", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.PitchTrim.ToString("d"), "562", 0.1d, 0.0d, 1.0d, "Control Interface", "PITCH TRIM Wheel", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.YawTrim.ToString("d"), "565", 0.1d, 0.0d, 1.0d, "Control Interface", "YAW TRIM Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "425", new SwitchPosition[] { new SwitchPosition("1.0", "OVRD", F16CCommands.controlCommands.ManualPitchOverride.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.controlCommands.ManualPitchOverride.ToString("d")) }, "Control Interface", "MANUAL PITCH Override Switch, OVRD/NORM", "%0.1f"));
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "358", new SwitchPosition[] { new SwitchPosition("1.0", "CAT III", F16CCommands.controlCommands.StoresConfig.ToString("d")), new SwitchPosition("0.0", "CAT I", F16CCommands.controlCommands.StoresConfig.ToString("d")) }, "Control Interface", "STORES CONFIG Switch, CAT III/CAT I", "%0.1f"));
            AddFunction(new Rocker(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.ApPitchAtt.ToString("d"), F16CCommands.controlCommands.ApPitchAlt.ToString("d"), F16CCommands.controlCommands.ApPitchAtt.ToString("d"), F16CCommands.controlCommands.ApPitchAlt.ToString("d"), "109", "Control Interface", "Autopilot PITCH Switch, ATT HOLD/ A/P OFF/ ALT HOLD", true)); AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "108", new SwitchPosition[] { new SwitchPosition("-1.0", "STRG SEL", F16CCommands.controlCommands.ApRoll.ToString("d")), new SwitchPosition("0.0", "ATT HOLD", F16CCommands.controlCommands.ApRoll.ToString("d")), new SwitchPosition("1.0", "HDG SEL", F16CCommands.controlCommands.ApRoll.ToString("d")) }, "Control Interface", "Autopilot ROLL Switch, STRG SEL/ATT HOLD/HDG SEL", "%0.1f"));
            AddFunction(new PushButton(this, devices.CONTROL_INTERFACE.ToString("d"), F16CCommands.controlCommands.AdvMode.ToString("d"), "97", "Control Interface", "ADV MODE Switch", "%1d"));            
            AddFunction(new Switch(this, devices.CONTROL_INTERFACE.ToString("d"), "568", new SwitchPosition[] { new SwitchPosition("1.0", "ENABLE", F16CCommands.controlCommands.ManualTfFlyup.ToString("d")), new SwitchPosition("0.0", "DISABLE", F16CCommands.controlCommands.ManualTfFlyup.ToString("d")) }, "Control Interface", "MANUAL TF FLYUP Switch, ENABLE/DISABLE", "%0.1f"));
            #endregion Control Interface
            #region External Lights
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "531", SwitchPositions.Create(8, 0d, 0.1d, F16CCommands.extlightsCommands.AntiCollKn.ToString("d"), new string[] { "OFF", "1", "2", "3", "4", "A", "B", "C" }, "%0.1f"), "External Lights", "ANTI-COLL Knob, OFF/1/2/3/4/A/B/C", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "532", new SwitchPosition[] { new SwitchPosition("1.0", "FLASH", F16CCommands.extlightsCommands.PosFlash.ToString("d")), new SwitchPosition("0.0", "STEADY", F16CCommands.extlightsCommands.PosFlash.ToString("d")) }, "External Lights", "FLASH STEADY Switch, FLASH/STEADY", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "533", new SwitchPosition[] { new SwitchPosition("-1.0", "BRT", F16CCommands.extlightsCommands.PosWingTail.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.extlightsCommands.PosWingTail.ToString("d")), new SwitchPosition("1.0", "DIM", F16CCommands.extlightsCommands.PosWingTail.ToString("d")) }, "External Lights", "WING/TAIL Switch, BRT/OFF/DIM", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "534", new SwitchPosition[] { new SwitchPosition("-1.0", "BRT", F16CCommands.extlightsCommands.PosFus.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.extlightsCommands.PosFus.ToString("d")), new SwitchPosition("1.0", "DIM", F16CCommands.extlightsCommands.PosFus.ToString("d")) }, "External Lights", "FUSELAGE Switch, BRT/OFF/DIM", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), F16CCommands.extlightsCommands.FormKn.ToString("d"), "535", 0.1d, 0.0d, 1.0d, "External Lights", "FORM Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "536", SwitchPositions.Create(5, 0d, 0.1d, F16CCommands.extlightsCommands.Master.ToString("d"), new string[] { "OFF", "ALL", "A-C", "FORM", "NORM" }, "%0.1f"), "External Lights", "MASTER Switch, OFF/ALL/A-C/FORM/NORM", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), F16CCommands.extlightsCommands.AerialRefuel.ToString("d"), "537", 0.1d, 0.0d, 1.0d, "External Lights", "AERIAL REFUELING Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS_SYSTEM.ToString("d"), "360", new SwitchPosition[] { new SwitchPosition("-1.0", "LANDING", F16CCommands.extlightsCommands.LandingTaxi.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.extlightsCommands.LandingTaxi.ToString("d")), new SwitchPosition("1.0", "TAXI", F16CCommands.extlightsCommands.LandingTaxi.ToString("d")) }, "External Lights", "LANDING TAXI LIGHTS Switch, LANDING/OFF/TAXI", "%0.1f"));
            #endregion External Lights
            #region Interior Lights
            AddFunction(new PushButton(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.MasterCaution.ToString("d"), "116", "Interior Lights", "Master Caution Button - Push to reset", "%1d"));
            AddFunction(new PushButton(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsTest.ToString("d"), "577", "Interior Lights", "MAL and IND LTS Test Button - Push to test", "%1d"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.Consoles.ToString("d"), "685", 0.1d, 0.0d, 1.0d, "Interior Lights", "PRIMARY CONSOLES BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.IntsPnl.ToString("d"), "686", 0.1d, 0.0d, 1.0d, "Interior Lights", "PRIMARY INST PNL BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.DataEntryDisplay.ToString("d"), "687", 0.1d, 0.0d, 1.0d, "Interior Lights", "PRIMARY DATA ENTRY DISPLAY BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.ConsolesFlood.ToString("d"), "688", 0.1d, 0.0d, 1.0d, "Interior Lights", "FLOOD CONSOLES BRT Knob", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.InstPnlFlood.ToString("d"), "690", 0.1d, 0.0d, 1.0d, "Interior Lights", "FLOOD INST PNL BRT Knob", false, "%0.1f"));
            AddFunction(new Rocker(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsDim.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsBrt.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsDim.ToString("d"), F16CCommands.cptlightsCommands.MalIndLtsBrt.ToString("d"), "691", "Interior Lights", "MAL and IND LTS Switch, BRT/Center/DIM", true));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.IndBrtAoA.ToString("d"), "794", 0.1d, 0.0d, 1.0d, "Interior Lights", "AOA Indexer Dimming Lever", false, "%0.1f"));
            AddFunction(new Axis(this, devices.CPTLIGHTS_SYSTEM.ToString("d"), F16CCommands.cptlightsCommands.IndBrtAR.ToString("d"), "795", 0.1d, 0.0d, 1.0d, "Interior Lights", "AR Status Indicator Dimming Lever", false, "%0.1f"));
            #endregion Interior Lights
            #region Electric System
            AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString("d"), "510", new SwitchPosition[] { new SwitchPosition("-1.0", "MAIN PWR", F16CCommands.elecCommands.MainPwrSw.ToString("d")), new SwitchPosition("0.0", "BATT", F16CCommands.elecCommands.MainPwrSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.elecCommands.MainPwrSw.ToString("d")) }, "Electric System", "MAIN PWR Switch, MAIN PWR/BATT/OFF", "%0.1f"));
            AddFunction(new PushButton(this, devices.ELEC_INTERFACE.ToString("d"), F16CCommands.elecCommands.CautionResetBtn.ToString("d"), "511", "Electric System", "ELEC CAUTION RESET Button - Push to reset", "%1d"));
            AddFunction(new Switch(this, devices.ELEC_INTERFACE.ToString("d"), "579", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.elecCommands.EPU_GEN_TestSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.elecCommands.EPU_GEN_TestSw.ToString("d")) }, "Electric System", "EPU/GEN Test Switch, EPU/GEN /OFF", "%0.1f"));
            AddFunction(new Rocker(this, devices.ELEC_INTERFACE.ToString("d"), F16CCommands.elecCommands.ProbeHeatSwTEST.ToString("d"), F16CCommands.elecCommands.ProbeHeatSw.ToString("d"), F16CCommands.elecCommands.ProbeHeatSwTEST.ToString("d"), F16CCommands.elecCommands.ProbeHeatSw.ToString("d"), "578", "Electric System", "PROBE HEAT Switch, PROBE HEAT/OFF/TEST(momentarily)", true));
            AddFunction(new Rocker(this, devices.ELEC_INTERFACE.ToString("d"), F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), F16CCommands.elecCommands.FlcsPwrTestSwMAINT.ToString("d"), "585", "Electric System", "FLCS PWR TEST Switch, MAINT/NORM/TEST(momentarily)", true));
            #endregion Electric System
            #region Fuel System
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "559", new SwitchPosition[] { new SwitchPosition("1.0", "MASTER", F16CCommands.fuelCommands.FuelMasterSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.fuelCommands.FuelMasterSw.ToString("d")) }, "Fuel System", "FUEL MASTER Switch, MASTER/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "558", new SwitchPosition[] { new SwitchPosition("0.0", "OPEN", F16CCommands.fuelCommands.FuelMasterSwCvr.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.fuelCommands.FuelMasterSwCvr.ToString("d")) }, "Fuel System", "FUEL MASTER Switch Cover, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "557", new SwitchPosition[] { new SwitchPosition("1.0", "TANK INERTING ", F16CCommands.fuelCommands.TankInertingSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.fuelCommands.TankInertingSw.ToString("d")) }, "Fuel System", "TANK INERTING Switch, TANK INERTING /OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "556", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.fuelCommands.EngineFeedSw.ToString("d"), new string[] { "OFF", "NORM", "AFT", "FWD" }, "%0.1f"), "Fuel System", "ENGINE FEED Knob, OFF/NORM/AFT/FWD", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "555", new SwitchPosition[] { new SwitchPosition("1.0", "OPEN", F16CCommands.fuelCommands.AirRefuelSw.ToString("d")), new SwitchPosition("0.0", "CLOSE", F16CCommands.fuelCommands.AirRefuelSw.ToString("d")) }, "Fuel System", "AIR REFUEL Switch, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "159", new SwitchPosition[] { new SwitchPosition("1.0", "NORM", F16CCommands.fuelCommands.ExtFuelTransferSw.ToString("d")), new SwitchPosition("0.0", "WING FIRST", F16CCommands.fuelCommands.ExtFuelTransferSw.ToString("d")) }, "Fuel System", "External Fuel Transfer Switch, NORM/ WING FIRST", "%0.1f"));

            // elements["PTR-CDCP-TMB-FMODE-158"] =
            // {
            // 	class			= {class_type.BTN,					class_type.TUMB},
            // 	hint			= _("FUEL QTY SEL Knob, TEST(momentarily)/NORM/RSVR/INT WING/EXT WING/EXT CTR"),
            // 	device			= devices.FUEL_INTERFACE,
            // 	action			= {fuel_commands.FuelQtySelSwTEST,	fuel_commands.FuelQtySelSw,	fuel_commands.FuelQtySelSwTEST},
            // 	stop_action		= {fuel_commands.FuelQtySelSwTEST,	0,							fuel_commands.FuelQtySelSwTEST},
            // 	arg				= {158,								158,						158},
            // 	arg_value		= {-0.1,							0.1,						-1},
            // 	arg_lim			= {{0.0, 0.5},						{0.1, 0.5},					{-0.1, 0.5}},
            // 	updatable		= true,
            // 	use_OBB			= true,
            // 	cycle			= false,

            SwitchPosition[] fuelSelectPositions = new SwitchPosition[]
            {
                new SwitchPosition("0.0","TEST",F16CCommands.fuelCommands.FuelQtySelSwTEST.ToString("d")),
                new SwitchPosition("0.1","NORM",F16CCommands.fuelCommands.FuelQtySelSwTEST.ToString("d")),
                new SwitchPosition("0.2","RSVR",F16CCommands.fuelCommands.FuelQtySelSwTEST.ToString("d")),
                new SwitchPosition("0.3","INT WING",F16CCommands.fuelCommands.FuelQtySelSwTEST.ToString("d")),
                new SwitchPosition("0.4","EXT WING",F16CCommands.fuelCommands.FuelQtySelSwTEST.ToString("d")),
                new SwitchPosition("0.5","EXT CTR",F16CCommands.fuelCommands.FuelQtySelSwTEST.ToString("d")),
            };
            AddFunction(new Switch(this, devices.FUEL_INTERFACE.ToString("d"), "158", fuelSelectPositions, "Fuel System", "FUEL QTY SEL Knob", "%0.1f"));
            #endregion Fuel System
            #region Gear System
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "362", new SwitchPosition[] { new SwitchPosition("1.0", "UP", F16CCommands.gearCommands.LGHandle.ToString("d")), new SwitchPosition("0.0", "DN", F16CCommands.gearCommands.LGHandle.ToString("d")) }, "Gear System", "LG Handle, UP/DN", "%0.1f"));
            AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.DownLockRelBtn.ToString("d"), "361", "Gear System", "DN LOCK REL Button - Push to reset", "%1d"));
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "354", new SwitchPosition[] { new SwitchPosition("1.0", "UP", F16CCommands.gearCommands.HookSw.ToString("d")), new SwitchPosition("0.0", "DN", F16CCommands.gearCommands.HookSw.ToString("d")) }, "Gear System", "HOOK Switch, UP/DN", "%0.1f"));
            AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.HornSilencerBtn.ToString("d"), "359", "Gear System", "HORN SILENCER Button - Push to reset", "%1d"));
            AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "356", new SwitchPosition[] { new SwitchPosition("1.0", "CHAN 1", F16CCommands.gearCommands.BrakesChannelSw.ToString("d")), new SwitchPosition("0.0", "CHAN 2", F16CCommands.gearCommands.BrakesChannelSw.ToString("d")) }, "Gear System", "BRAKES Channel Switch, CHAN 1/CHAN 2", "%0.1f"));
            AddFunction(new Rocker(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.AntiSkidSw.ToString("d"), F16CCommands.gearCommands.AntiSkidSw.ToString("d"), F16CCommands.gearCommands.AntiSkidSw.ToString("d"), F16CCommands.gearCommands.AntiSkidSw.ToString("d"), "357", "Gear System", "ANTI-SKID Switch, PARKING BRAKE/ANTI-SKID/OFF", true)); AddFunction(new Switch(this, devices.GEAR_INTERFACE.ToString("d"), "380", new SwitchPosition[] { new SwitchPosition("1.0", "PULL", F16CCommands.gearCommands.AltGearHandle.ToString("d")), new SwitchPosition("0.0", "STOW", F16CCommands.gearCommands.AltGearHandle.ToString("d")) }, "Gear System", "ALT GEAR Handle, PULL/STOW", "%0.1f"));
            AddFunction(new PushButton(this, devices.GEAR_INTERFACE.ToString("d"), F16CCommands.gearCommands.AltGearResetBtn.ToString("d"), "381", "Gear System", "ALT GEAR Handle - Push to reset", "%1d"));
            #endregion Gear System
            #region ECS
            AddFunction(new Axis(this, devices.ECS_INTERFACE.ToString("d"), F16CCommands.ecsCommands.TempKnob.ToString("d"), "692", 0.1d, 0.0d, 1.0d, "ECS", "TEMP Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ECS_INTERFACE.ToString("d"), "693", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.ecsCommands.AirSourceKnob.ToString("d"), new string[] { "OFF", "NORM", "DUMP", "RAM" }, "%0.1f"), "ECS", "AIR SOURCE Knob, OFF/NORM/DUMP/RAM", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS_INTERFACE.ToString("d"), "602", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", F16CCommands.ecsCommands.DefogLever.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.ecsCommands.DefogLever.ToString("d")) }, "ECS", "DEFOG Lever", "%0.1f"));
            #endregion ECS
            #region EPU
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "527", new SwitchPosition[] { new SwitchPosition("0.0", "OPEN", F16CCommands.engineCommands.EpuSwCvrOn.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.engineCommands.EpuSwCvrOn.ToString("d")) }, "EPU", "EPU Switch Cover for ON, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "529", new SwitchPosition[] { new SwitchPosition("0.0", "OPEN", F16CCommands.engineCommands.EpuSwCvrOff.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.engineCommands.EpuSwCvrOff.ToString("d")) }, "EPU", "EPU Switch Cover for OFF, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "528", new SwitchPosition[] { new SwitchPosition("-1.0", "ON", F16CCommands.engineCommands.EpuSw.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.engineCommands.EpuSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.engineCommands.EpuSw.ToString("d")) }, "EPU", "EPU Switch, ON/NORM/OFF", "%0.1f"));
            #endregion EPU
            #region engine
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "710", new SwitchPosition[] { new SwitchPosition("-1.0", "ON", F16CCommands.engineCommands.EngAntiIceSw.ToString("d")), new SwitchPosition("0.0", "AUTO", F16CCommands.engineCommands.EngAntiIceSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.engineCommands.EngAntiIceSw.ToString("d")) }, "engine", "Engine ANTI ICE Switch, ON/AUTO/OFF", "%0.1f"));
            AddFunction(new Rocker(this, devices.ENGINE_INTERFACE.ToString("d"), F16CCommands.engineCommands.JfsSwStart2.ToString("d"), F16CCommands.engineCommands.JfsSwStart1.ToString("d"), F16CCommands.engineCommands.JfsSwStart2.ToString("d"), F16CCommands.engineCommands.JfsSwStart1.ToString("d"), "447", "engine", "JFS Switch, START 1/OFF/START 2", true)); AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "448", new SwitchPosition[] { new SwitchPosition("0.0", "OPEN", F16CCommands.engineCommands.EngContSwCvr.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.engineCommands.EngContSwCvr.ToString("d")) }, "engine", "ENG CONT Switch Cover, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "449", new SwitchPosition[] { new SwitchPosition("1.0", "PRI", F16CCommands.engineCommands.EngContSw.ToString("d")), new SwitchPosition("0.0", "SEC", F16CCommands.engineCommands.EngContSw.ToString("d")) }, "engine", "ENG CONT Switch, PRI/SEC", "%0.1f"));
            AddFunction(new Switch(this, devices.ENGINE_INTERFACE.ToString("d"), "451", new SwitchPosition[] { new SwitchPosition("1.0", "MAX POWER", F16CCommands.engineCommands.MaxPowerSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.engineCommands.MaxPowerSw.ToString("d")) }, "engine", "MAX POWER Switch (is inoperative), MAX POWER/OFF", "%0.1f"));
            AddFunction(new Rocker(this, devices.ENGINE_INTERFACE.ToString("d"), F16CCommands.engineCommands.ABResetSwEngData.ToString("d"), F16CCommands.engineCommands.ABResetSwReset.ToString("d"), F16CCommands.engineCommands.ABResetSwEngData.ToString("d"), F16CCommands.engineCommands.ABResetSwReset.ToString("d"), "450", "engine", "AB RESET Switch, AB RESET/NORM/ENG DATA", true)); AddFunction(new PushButton(this, devices.ENGINE_INTERFACE.ToString("d"), F16CCommands.engineCommands.FireOheatTestBtn.ToString("d"), "575", "engine", "FIRE and HEAT DETECT Test Button - Push to test", "%1d"));
            #endregion engine
            #region Oxygen System
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "728", SwitchPositions.Create(3, 0.0d, 0.5d, F16CCommands.oxygenCommands.SupplyLever.ToString("d"), new string[] { "PBG", "ON", "OFF" }, "%0.1f"), "Oxygen", "Supply Lever", "%0.1f"));  //unused
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "727", new SwitchPosition[] { new SwitchPosition("1.0", "100 percent", F16CCommands.oxygenCommands.DiluterLever.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.oxygenCommands.DiluterLever.ToString("d")) }, "Oxygen System", "Diluter Lever, 100 percent/NORM", "%0.1f"));
            AddFunction(new Rocker(this, devices.OXYGEN_INTERFACE.ToString("d"), F16CCommands.oxygenCommands.EmergencyLeverTestMask.ToString("d"), F16CCommands.oxygenCommands.EmergencyLever.ToString("d"), F16CCommands.oxygenCommands.EmergencyLeverTestMask.ToString("d"), F16CCommands.oxygenCommands.EmergencyLever.ToString("d"), "726", "Oxygen System", "Emergency Lever, EMERGENCY/NORMAL/TEST MASK(momentarily)", true)); 
            AddFunction(new Switch(this, devices.OXYGEN_INTERFACE.ToString("d"), "576", new SwitchPosition[] { new SwitchPosition("1.0", "BIT", F16CCommands.oxygenCommands.ObogsBitSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.oxygenCommands.ObogsBitSw.ToString("d")) }, "Oxygen System", "OBOGS BIT Switch, BIT/OFF", "%0.1f"));

            #endregion Oxygen System
            #region Sensor Power Control Panel
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "670", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.smsCommands.LeftHDPT.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.LeftHDPT.ToString("d")) }, "Sensor Power Control Panel", "LEFT HDPT Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "671", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.smsCommands.RightHDPT.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.RightHDPT.ToString("d")) }, "Sensor Power Control Panel", "RIGHT HDPT Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.FCR.ToString("d"), "672", new SwitchPosition[] { new SwitchPosition("1.0", "FCR", F16CCommands.fcrCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.fcrCommands.PwrSw.ToString("d")) }, "Sensor Power Control Panel", "FCR Switch, FCR/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.RALT.ToString("d"), "673", new SwitchPosition[] { new SwitchPosition("-1.0", "RDR ALT", F16CCommands.raltCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "STBY", F16CCommands.raltCommands.PwrSw.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.raltCommands.PwrSw.ToString("d")) }, "Sensor Power Control Panel", "RDR ALT Switch, RDR ALT/STBY/OFF", "%0.1f"));
            #endregion Sensor Power Control Panel
            #region Avionic Power panel
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "715", new SwitchPosition[] { new SwitchPosition("1.0", "MMC", F16CCommands.mmcCommands.MmcPwr.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.MmcPwr.ToString("d")) }, "Avionic Power panel", "MMC Switch, MMC/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "716", new SwitchPosition[] { new SwitchPosition("1.0", "ST STA", F16CCommands.smsCommands.StStaSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.StStaSw.ToString("d")) }, "Avionic Power panel", "ST STA Switch, ST STA/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "717", new SwitchPosition[] { new SwitchPosition("1.0", "MFD", F16CCommands.mmcCommands.MFD.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.MFD.ToString("d")) }, "Avionic Power panel", "MFD Switch, MFD/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "718", new SwitchPosition[] { new SwitchPosition("1.0", "UFC", F16CCommands.ufcCommands.UFC_Sw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.ufcCommands.UFC_Sw.ToString("d")) }, "Avionic Power panel", "UFC Switch, UFC/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.GPS.ToString("d"), "720", new SwitchPosition[] { new SwitchPosition("1.0", "GPS", F16CCommands.gpsCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.gpsCommands.PwrSw.ToString("d")) }, "Avionic Power panel", "GPS Switch, GPS/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MIDS.ToString("d"), "723", SwitchPositions.Create(3, 0d, 0.1d, F16CCommands.midsCommands.PwrSw.ToString("d"), new string[] { "ZERO", "OFF", "ON" }, "%0.1f"), "Avionic Power panel", "MIDS LVT Knob, ZERO/OFF/ON", "%0.1f"));
            AddFunction(new Switch(this, devices.INS.ToString("d"), "719", SwitchPositions.Create(7, 0d, 0.1d, F16CCommands.insCommands.ModeKnob.ToString("d"), new string[] { "OFF", "STOR HDG", "NORM", "NAV", "CAL", "INFLT ALIGN", "ATT" }, "%0.1f"), "Avionic Power panel", "INS Knob, OFF/STOR HDG/NORM/NAV/CAL/INFLT ALIGN/ATT", "%0.1f"));
            AddFunction(new Switch(this, devices.MAP.ToString("d"), "722", new SwitchPosition[] { new SwitchPosition("1.0", "MAP", F16CCommands.mapCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mapCommands.PwrSw.ToString("d")) }, "Avionic Power panel", "MAP Switch, MAP/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.IDM.ToString("d"), "721", new SwitchPosition[] { new SwitchPosition("1.0", "DL", F16CCommands.idmCommands.PwrSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.idmCommands.PwrSw.ToString("d")) }, "Avionic Power panel", "DL Switch, DL/OFF", "%0.1f"));
            #endregion Avionic Power panel
            #region Modular Mission Computer (MMC)
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "105", new SwitchPosition[] { new SwitchPosition("-1.0", "MASTER ARM", F16CCommands.mmcCommands.MasterArmSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.MasterArmSw.ToString("d")), new SwitchPosition("1.0", "SIMULATE", F16CCommands.mmcCommands.MasterArmSw.ToString("d")) }, "Modular Mission Computer (MMC)", "MASTER ARM Switch, MASTER ARM/OFF/SIMULATE", "%0.1f"));
            AddFunction(new PushButton(this, devices.MMC.ToString("d"), F16CCommands.mmcCommands.EmerStoresJett.ToString("d"), "353", "Modular Mission Computer (MMC)", "EMER STORES JETTISON Button - Push to jettison", "%1d"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "355", new SwitchPosition[] { new SwitchPosition("1.0", "ENABLE", F16CCommands.mmcCommands.GroundJett.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.mmcCommands.GroundJett.ToString("d")) }, "Modular Mission Computer (MMC)", "GND JETT ENABLE Switch, ENABLE/OFF", "%0.1f"));
            AddFunction(new PushButton(this, devices.MMC.ToString("d"), F16CCommands.mmcCommands.AltRel.ToString("d"), "104", "Modular Mission Computer (MMC)", "ALT REL Button - Push to release", "%1d"));
            AddFunction(new Switch(this, devices.SMS.ToString("d"), "103", new SwitchPosition[] { new SwitchPosition("1.0", "ARM", F16CCommands.smsCommands.LaserSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.smsCommands.LaserSw.ToString("d")) }, "Modular Mission Computer (MMC)", "LASER ARM Switch, ARM/OFF", "%0.1f"));
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
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "189", new SwitchPosition[] { new SwitchPosition("-1.0", "GAIN", F16CCommands.ufcCommands.FLIR_GAIN_Sw.ToString("d")), new SwitchPosition("0.0", "LVL", F16CCommands.ufcCommands.FLIR_GAIN_Sw.ToString("d")), new SwitchPosition("1.0", "AUTO", F16CCommands.ufcCommands.FLIR_GAIN_Sw.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP FLIR GAIN/LEVEL Switch, GAIN/LVL/AUTO", "%0.1f"));
            AddFunction(new Rocker(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DED_INC.ToString("d"), F16CCommands.ufcCommands.DED_DEC.ToString("d"), F16CCommands.ufcCommands.DED_INC.ToString("d"), F16CCommands.ufcCommands.DED_DEC.ToString("d"), "183", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP DED Increment/Decrement Switch", true));
            AddFunction(new Rocker(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.FLIR_INC.ToString("d"), F16CCommands.ufcCommands.FLIR_DEC.ToString("d"), F16CCommands.ufcCommands.FLIR_INC.ToString("d"), F16CCommands.ufcCommands.FLIR_DEC.ToString("d"), "188", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP FLIR Increment/Decrement Switch", true));
            AddFunction(new Rocker(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.WARN_RESET.ToString("d"), F16CCommands.ufcCommands.DRIFT_CUTOUT.ToString("d"), F16CCommands.ufcCommands.WARN_RESET.ToString("d"), F16CCommands.ufcCommands.DRIFT_CUTOUT.ToString("d"), "186", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP DRIFT CUTOUT/WARN RESET Switch, DRIFT C/O /NORM/WARN RESET", true));
            AddFunction(new Rocker(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DCS_RTN.ToString("d"), F16CCommands.ufcCommands.DCS_SEQ.ToString("d"), F16CCommands.ufcCommands.DCS_RTN.ToString("d"), F16CCommands.ufcCommands.DCS_SEQ.ToString("d"), "184", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Data Control Switch, RTN/SEQ", true));
            AddFunction(new Rocker(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.DCS_UP.ToString("d"), F16CCommands.ufcCommands.DCS_DOWN.ToString("d"), F16CCommands.ufcCommands.DCS_UP.ToString("d"), F16CCommands.ufcCommands.DCS_DOWN.ToString("d"), "185", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "ICP Data Control Switch, UP/DN", true));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.F_ACK.ToString("d"), "122", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "F-ACK Button", "%1d"));
            AddFunction(new PushButton(this, devices.UFC.ToString("d"), F16CCommands.ufcCommands.IFF_IDENT.ToString("d"), "125", "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "IFF IDENT Button", "%1d"));
            AddFunction(new Switch(this, devices.UFC.ToString("d"), "100", new SwitchPosition[] { new SwitchPosition("-1.0", "SILENT", F16CCommands.ufcCommands.RF_Sw.ToString("d")), new SwitchPosition("0.0", "QUIET", F16CCommands.ufcCommands.RF_Sw.ToString("d")), new SwitchPosition("1.0", "NORM", F16CCommands.ufcCommands.RF_Sw.ToString("d")) }, "Integrated Control Panel (ICP) of Upfront Controls (UFC)", "RF Switch, SILENT/QUIET/NORM", "%0.1f"));
            #endregion Integrated Control Panel (ICP) of Upfront Controls (UFC)
            #region HUD Remote Control Panel
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "675", new SwitchPosition[] { new SwitchPosition("-1.0", "VV/VAH ", F16CCommands.mmcCommands.VvVah.ToString("d")), new SwitchPosition("0.0", "VAH ", F16CCommands.mmcCommands.VvVah.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.VvVah.ToString("d")) }, "HUD Remote Control Panel", "HUD Scales Switch, VV/VAH / VAH / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "676", new SwitchPosition[] { new SwitchPosition("-1.0", "ATT/FPM ", F16CCommands.mmcCommands.AttFpm.ToString("d")), new SwitchPosition("0.0", "FPM ", F16CCommands.mmcCommands.AttFpm.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.AttFpm.ToString("d")) }, "HUD Remote Control Panel", "HUD Flightpath Marker Switch, ATT/FPM / FPM / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "677", new SwitchPosition[] { new SwitchPosition("-1.0", "DED ", F16CCommands.mmcCommands.DedData.ToString("d")), new SwitchPosition("0.0", "PFL ", F16CCommands.mmcCommands.DedData.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.DedData.ToString("d")) }, "HUD Remote Control Panel", "HUD DED/PFLD Data Switch, DED / PFL / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "678", new SwitchPosition[] { new SwitchPosition("-1.0", "STBY ", F16CCommands.mmcCommands.DeprRet.ToString("d")), new SwitchPosition("0.0", "PRI ", F16CCommands.mmcCommands.DeprRet.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.DeprRet.ToString("d")) }, "HUD Remote Control Panel", "HUD Depressible Reticle Switch, STBY / PRI / OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "679", new SwitchPosition[] { new SwitchPosition("-1.0", "CAS ", F16CCommands.mmcCommands.Spd.ToString("d")), new SwitchPosition("0.0", "TAS ", F16CCommands.mmcCommands.Spd.ToString("d")), new SwitchPosition("1.0", "GND SPD", F16CCommands.mmcCommands.Spd.ToString("d")) }, "HUD Remote Control Panel", "HUD Velocity Switch, CAS / TAS / GND SPD", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "680", new SwitchPosition[] { new SwitchPosition("-1.0", "RADAR ", F16CCommands.mmcCommands.Alt.ToString("d")), new SwitchPosition("0.0", "BARO ", F16CCommands.mmcCommands.Alt.ToString("d")), new SwitchPosition("1.0", "AUTO", F16CCommands.mmcCommands.Alt.ToString("d")) }, "HUD Remote Control Panel", "HUD Altitude Switch, RADAR / BARO / AUTO", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "681", new SwitchPosition[] { new SwitchPosition("-1.0", "DAY ", F16CCommands.mmcCommands.Brt.ToString("d")), new SwitchPosition("0.0", "AUTO BRT ", F16CCommands.mmcCommands.Brt.ToString("d")), new SwitchPosition("1.0", "NIGHT", F16CCommands.mmcCommands.Brt.ToString("d")) }, "HUD Remote Control Panel", "HUD Brightness Control Switch, DAY / AUTO BRT / NIGHT", "%0.1f"));
            AddFunction(new Switch(this, devices.MMC.ToString("d"), "682", new SwitchPosition[] { new SwitchPosition("-1.0", "STEP ", F16CCommands.mmcCommands.Test.ToString("d")), new SwitchPosition("0.0", "ON ", F16CCommands.mmcCommands.Test.ToString("d")), new SwitchPosition("1.0", "OFF", F16CCommands.mmcCommands.Test.ToString("d")) }, "HUD Remote Control Panel", "HUD TEST Switch, STEP / ON / OFF", "%0.1f"));
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
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "443", new SwitchPosition[] { new SwitchPosition("-1.0", "HOT MIC ", F16CCommands.intercomCommands.HotMicCipherSw.ToString("d")), new SwitchPosition("0.0", "OFF ", F16CCommands.intercomCommands.HotMicCipherSw.ToString("d")), new SwitchPosition("1.0", "CIPHER", F16CCommands.intercomCommands.HotMicCipherSw.ToString("d")) }, "Audio Control Panels", "HOT MIC CIPHER Switch, HOT MIC / OFF / CIPHER", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "696", new SwitchPosition[] { new SwitchPosition("1.0", "VOICE MESSAGE", F16CCommands.intercomCommands.VMS_InhibitSw.ToString("d")), new SwitchPosition("0.0", "INHIBIT", F16CCommands.intercomCommands.VMS_InhibitSw.ToString("d")) }, "Audio Control Panels", "Voice Message Inhibit Switch, VOICE MESSAGE/INHIBIT", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "711", new SwitchPosition[] { new SwitchPosition("-1.0", "LOWER", F16CCommands.intercomCommands.IFF_AntSelSw.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.intercomCommands.IFF_AntSelSw.ToString("d")), new SwitchPosition("1.0", "UPPER", F16CCommands.intercomCommands.IFF_AntSelSw.ToString("d")) }, "Audio Control Panels", "IFF ANT SEL Switch, LOWER/NORM/UPPER", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "712", new SwitchPosition[] { new SwitchPosition("-1.0", "LOWER", F16CCommands.intercomCommands.UHF_AntSelSw.ToString("d")), new SwitchPosition("0.0", "NORM", F16CCommands.intercomCommands.UHF_AntSelSw.ToString("d")), new SwitchPosition("1.0", "UPPER", F16CCommands.intercomCommands.UHF_AntSelSw.ToString("d")) }, "Audio Control Panels", "UHF ANT SEL Switch, LOWER/NORM/UPPER", "%0.1f"));
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
            AddFunction(new Switch(this, devices.UHF_CONTROL_PANEL.ToString("d"), "734", new SwitchPosition[] { new SwitchPosition("1.0", "Open", F16CCommands.uhfCommands.AccessDoor.ToString("d")), new SwitchPosition("0.0", "Closed", F16CCommands.uhfCommands.AccessDoor.ToString("d")) }, "UHF Backup Control Panel", "Access Door, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Text(this, "2004", "UHF Displays", "Preset Channel", "Text value")); 
            AddFunction(new Text(this, "2005", "UHF Displays", "Frequency Display", "Text value"));
            #endregion UHF Backup Control Panel
            #region IFF Control Panel
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "542", SwitchPositions.Create(2, 0d, 1d, F16CCommands.iffCommands.CNI_Knob.ToString("d"), new string[] { "UFC", "BACKUP" }, "%0.-1f"), "IFF Control Panel", "C and I Knob, UFC/BACKUP", "%0.-1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "540", SwitchPositions.Create(5, 0d, 0.1d, F16CCommands.iffCommands.MasterKnob.ToString("d"), new string[] { "OFF", "STBY", "LOW", "NORM", "EMER" }, "%0.1f"), "IFF Control Panel", "IFF MASTER Knob, OFF/STBY/LOW/NORM/EMER", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "541", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.iffCommands.M4CodeSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.iffCommands.M4CodeSw.ToString("d")), new SwitchPosition("1.0", "Posn 3", F16CCommands.iffCommands.M4CodeSw.ToString("d")) }, "IFF Control Panel", "IFF M-4 CODE Switch, HOLD/ A/B /ZERO", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "543", new SwitchPosition[] { new SwitchPosition("-1.0", "OUT", F16CCommands.iffCommands.M4ReplySw.ToString("d")), new SwitchPosition("0.0", "A", F16CCommands.iffCommands.M4ReplySw.ToString("d")), new SwitchPosition("1.0", "B", F16CCommands.iffCommands.M4ReplySw.ToString("d")) }, "IFF Control Panel", "IFF MODE 4 REPLY Switch, OUT/A/B", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "544", new SwitchPosition[] { new SwitchPosition("1.0", "OUT", F16CCommands.iffCommands.M4MonitorSw.ToString("d")), new SwitchPosition("0.0", "AUDIO", F16CCommands.iffCommands.M4MonitorSw.ToString("d")) }, "IFF Control Panel", "IFF MODE 4 MONITOR Switch, OUT/AUDIO", "%0.1f"));
            AddFunction(new Switch(this, devices.IFF_CONTROL_PANEL.ToString("d"), "553", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", F16CCommands.iffCommands.EnableSw.ToString("d")), new SwitchPosition("0.0", "Posn 2", F16CCommands.iffCommands.EnableSw.ToString("d")), new SwitchPosition("1.0", "Posn 3", F16CCommands.iffCommands.EnableSw.ToString("d")) }, "IFF Control Panel", "IFF ENABLE Switch, M1/M3 /OFF/ M3/MS", "%0.1f"));
            AddFunction(new Rocker(this, devices.IFF_CONTROL_PANEL.ToString("d"), F16CCommands.iffCommands.M1M3Selector1_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector1_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector1_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector1_Inc.ToString("d"), "545", "IFF Control Panel", "IFF MODE 1 Selector Lever, DIGIT 1", true));
            AddFunction(new Rocker(this, devices.IFF_CONTROL_PANEL.ToString("d"), F16CCommands.iffCommands.M1M3Selector2_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector2_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector2_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector2_Inc.ToString("d"), "547", "IFF Control Panel", "IFF MODE 1 Selector Lever, DIGIT 2", true));
            AddFunction(new Rocker(this, devices.IFF_CONTROL_PANEL.ToString("d"), F16CCommands.iffCommands.M1M3Selector3_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector3_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector3_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector3_Inc.ToString("d"), "549", "IFF Control Panel", "IFF MODE 3 Selector Lever, DIGIT 1", true));
            AddFunction(new Rocker(this, devices.IFF_CONTROL_PANEL.ToString("d"), F16CCommands.iffCommands.M1M3Selector4_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector4_Inc.ToString("d"), F16CCommands.iffCommands.M1M3Selector4_Dec.ToString("d"), F16CCommands.iffCommands.M1M3Selector4_Inc.ToString("d"), "551", "IFF Control Panel", "IFF MODE 3 Selector Lever, DIGIT 2", true));
            #endregion IFF Control Panel
            #region KY-58
            AddFunction(new Switch(this, devices.KY58.ToString("d"), "705", SwitchPositions.Create(4, 0d, 0.1d, F16CCommands.ky58Commands.KY58_ModeSw.ToString("d"), new string[] { "P", "C", "LD", "RV" }, "%0.1f"), "KY-58", "KY-58 MODE Knob, P/C/LD/RV", "%0.1f"));
            AddFunction(new Axis(this, devices.KY58.ToString("d"), F16CCommands.ky58Commands.KY58_Volume.ToString("d"), "708", 0.1d, 0.0d, 1.0d, "KY-58", "KY-58 VOLUME Knob", false, "%0.1f"));
            AddFunction(new Switch(this, devices.KY58.ToString("d"), "706", SwitchPositions.Create(8, 0d, 0.1d, F16CCommands.ky58Commands.KY58_FillSw.ToString("d"), new string[] { "Z 1-5", "1", "2", "3", "4", "5", "6", "Z ALL" }, "%0.1f"), "KY-58", "KY-58 FILL Knob, Z 1-5/1/2/3/4/5/6/Z ALL", "%0.1f"));
            AddFunction(new Switch(this, devices.KY58.ToString("d"), "707", SwitchPositions.Create(3, 0d, 0.5d, F16CCommands.ky58Commands.KY58_PowerSw.ToString("d"), new string[] { "OFF", "ON", "TD" }, "%0.1f"), "KY-58", "KY-58 Power Knob, OFF/ON/TD", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "701", new SwitchPosition[] { new SwitchPosition("-1.0", "CRAD 1", F16CCommands.intercomCommands.PlainCipherSw.ToString("d")), new SwitchPosition("0.0", "PLAIN", F16CCommands.intercomCommands.PlainCipherSw.ToString("d")), new SwitchPosition("1.0", "CRAD 2", F16CCommands.intercomCommands.PlainCipherSw.ToString("d")) }, "KY-58", "PLAIN Cipher Switch, CRAD 1/PLAIN/CRAD 2", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "694", new SwitchPosition[] { new SwitchPosition("0.0", "OPEN", F16CCommands.intercomCommands.ZeroizeSwCvr.ToString("d")), new SwitchPosition("1.0", "CLOSE", F16CCommands.intercomCommands.ZeroizeSwCvr.ToString("d")) }, "KY-58", "ZEROIZE Switch Cover, OPEN/CLOSE", "%0.1f"));
            AddFunction(new Switch(this, devices.INTERCOM.ToString("d"), "695", new SwitchPosition[] { new SwitchPosition("-1.0", "OFP", F16CCommands.intercomCommands.ZeroizeSw.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.intercomCommands.ZeroizeSw.ToString("d")), new SwitchPosition("1.0", "DATA", F16CCommands.intercomCommands.ZeroizeSw.ToString("d")) }, "KY-58", "ZEROIZE Switch, OFP/OFF/DATA", "%0.1f"));
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
            AddFunction(new Axis(this, devices.RWR.ToString("d"), "3017", "403", 0.1d, 0.0d, 1.0d, "RWR", "RWR 3 Amp Knob", false, "%0.1f"));  // Command Code for this is unknown
            #endregion RWR
            #region CMDS
            AddFunction(new PushButton(this, devices.CMDS.ToString("d"), F16CCommands.cmdsCommands.DispBtn.ToString("d"), "604", "CMDS", "CHAFF/FLARE Dispense Button - Push to dispense", "%1d"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "375", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.RwrSrc.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.RwrSrc.ToString("d")) }, "CMDS", "RWR 555 Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "374", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.JmrSrc.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.JmrSrc.ToString("d")) }, "CMDS", "JMR Source Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "373", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.MwsSrc.ToString("d")), new SwitchPosition("0.0", "OFF (no function)", F16CCommands.cmdsCommands.MwsSrc.ToString("d")) }, "CMDS", "MWS Source Switch, ON/OFF (no function)", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "371", new SwitchPosition[] { new SwitchPosition("1.0", "JETT", F16CCommands.cmdsCommands.Jett.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.Jett.ToString("d")) }, "CMDS", "Jettison Switch, JETT/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "365", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.O1Exp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.O1Exp.ToString("d")) }, "CMDS", "O1 Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "366", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.O2Exp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.O2Exp.ToString("d")) }, "CMDS", "O2 Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "367", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.ChExp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.ChExp.ToString("d")) }, "CMDS", "CH Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "368", new SwitchPosition[] { new SwitchPosition("1.0", "ON", F16CCommands.cmdsCommands.FlExp.ToString("d")), new SwitchPosition("0.0", "OFF", F16CCommands.cmdsCommands.FlExp.ToString("d")) }, "CMDS", "FL Expendable Category Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "377", SwitchPositions.Create(5, 0d, 0.1d, F16CCommands.cmdsCommands.Prgm.ToString("d"), new string[] { "BIT", "1", "2", "3", "4" }, "%0.1f"), "CMDS", "PROGRAM Knob, BIT/1/2/3/4", "%0.1f"));
            AddFunction(new Switch(this, devices.CMDS.ToString("d"), "378", SwitchPositions.Create(6, 0d, 0.1d, F16CCommands.cmdsCommands.Mode.ToString("d"), new string[] { "OFF", "STBY", "MAN", "SEMI", "AUTO", "BYP" }, "%0.1f"), "CMDS", "MODE Knob, OFF/STBY/MAN/SEMI/AUTO/BYP", "%0.1f"));
            AddFunction(new Text(this, "2000", "CMDS", "O1 Display", "Text value"));
            AddFunction(new Text(this, "2001", "CMDS", "O2 Display", "Text value"));
            AddFunction(new Text(this, "2002", "CMDS", "Chaff Count", "Text value"));
            AddFunction(new Text(this, "2003", "CMDS", "Flare Count", "Text value"));
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
            AddFunction(new Rocker(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), "320", "MFD Left", "Left MFD GAIN AddFunction(new Rocker Switch", true));
            AddFunction(new Rocker(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), "321", "MFD Left", "Left MFD SYM AddFunction(new Rocker Switch", true));
            AddFunction(new Rocker(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), "322", "MFD Left", "Left MFD CON AddFunction(new Rocker Switch", true));
            AddFunction(new Rocker(this, devices.MFD_LEFT.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), "323", "MFD Left", "Left MFD BRT AddFunction(new Rocker Switch", true));
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
            AddFunction(new Rocker(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.GAIN_Rocker_DOWN.ToString("d"), "346", "MFD Right", "Left MFD GAIN AddFunction(new Rocker Switch", true));
            AddFunction(new Rocker(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.SYM_Rocker_DOWN.ToString("d"), "347", "MFD Right", "Left MFD SYM AddFunction(new Rocker Switch", true));
            AddFunction(new Rocker(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.CON_Rocker_DOWN.ToString("d"), "348", "MFD Right", "Left MFD CON AddFunction(new Rocker Switch", true));
            AddFunction(new Rocker(this, devices.MFD_RIGHT.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_UP.ToString("d"), F16CCommands.mfdCommands.BRT_Rocker_DOWN.ToString("d"), "349", "MFD Right", "Left MFD BRT AddFunction(new Rocker Switch", true));
            #endregion MFD Right
            #region Instruments
            #endregion Instruments
            #region Airspeed/Mach Indicator
            AddFunction(new Axis(this, devices.AMI.ToString("d"), F16CCommands.amiCommands.SettingKnob.ToString("d"), "71", 0.05d, 0.0d, 0d, "Airspeed/Mach Indicator", "SET INDEX Knob", false, "%0.1f"));
            #endregion Airspeed/Mach Indicator
            #region Altimeter
            AddFunction(new Axis(this, devices.AAU34.ToString("d"), F16CCommands.altCommands.ZERO.ToString("d"), "62", 0.2d, 0.0d, 0.4d, "Altimeter", "Barometric Setting Knob", false, "%0.1f"));
            AddFunction(new Rocker(this, devices.AAU34.ToString("d"), F16CCommands.altCommands.ELEC.ToString("d"), F16CCommands.altCommands.PNEU.ToString("d"), F16CCommands.altCommands.ELEC.ToString("d"), F16CCommands.altCommands.PNEU.ToString("d"), "60", "Altimeter", "Mode Lever, ELEC/OFF/PNEU", true));

            #endregion Altimeter
            #region SAI ARU-42/A-2
            AddFunction(new PushButton(this, devices.SAI.ToString("d"), F16CCommands.saiCommands.cage.ToString("d"), "67", "SAI", "Cage Button", "%1d"));
            AddFunction(new Axis(this, devices.SAI.ToString("d"), F16CCommands.saiCommands.reference.ToString("d"), "66", 0.1d, 0.0d, 1.0d, "SAI", "Reference Knob", false, "%0.1f"));

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
            AddFunction(new PushButton(this, devices.CLOCK.ToString("d"), F16CCommands.clockCommands.CLOCK_left_lev_up.ToString("d"), "626", "Clock", "Clock Setting Knob button", "%1d"));
            AddFunction(new Axis(this, devices.CLOCK.ToString("d"), F16CCommands.clockCommands.CLOCK_left_lev_rotate.ToString("d"), "625", 0.1d, 0.0d, 1.0d, "Clock", "Clock Winding Knob", true, "%0.1f"));
            AddFunction(new PushButton(this, devices.CLOCK.ToString("d"), F16CCommands.clockCommands.CLOCK_right_lev_down.ToString("d"), "628", "Clock", "Clock Elapsed Time button", "%1d"));
            #endregion Clock
            #region Cockpit Mechanics
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "600", new SwitchPosition[] { new SwitchPosition("1.0", "UP", F16CCommands.cptCommands.CanopyHandle.ToString("d")), new SwitchPosition("0.0", "DOWN", F16CCommands.cptCommands.CanopyHandle.ToString("d")) }, "Cockpit Mechanics", "Canopy Handle, UP/DOWN", "%0.1f"));
            AddFunction(new Rocker(this, devices.CPT_MECH.ToString("d"), F16CCommands.cptCommands.SeatAdjSwitchDown.ToString("d"), F16CCommands.cptCommands.SeatAdjSwitchUp.ToString("d"), F16CCommands.cptCommands.SeatAdjSwitchDown.ToString("d"), F16CCommands.cptCommands.SeatAdjSwitchUp.ToString("d"), "786", "Cockpit Mechanics", "SEAT ADJ Switch, UP/OFF/DOWN", true));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "601", new SwitchPosition[] { new SwitchPosition("1.0", "PULL", F16CCommands.cptCommands.CanopyTHandle.ToString("d")), new SwitchPosition("0.0", "STOW", F16CCommands.cptCommands.CanopyTHandle.ToString("d")) }, "Cockpit Mechanics", "CANOPY JETTISON T-Handle, PULL/STOW", "%0.1f"));
            AddFunction(new Switch(this, devices.CPT_MECH.ToString("d"), "785", new SwitchPosition[] { new SwitchPosition("1.0", "ARMED", F16CCommands.cptCommands.EjectionSafetyLever.ToString("d")), new SwitchPosition("0.0", "LOCKED", F16CCommands.cptCommands.EjectionSafetyLever.ToString("d")) }, "Cockpit Mechanics", "Ejection Safety Lever, ARMED/LOCKED", "%0.1f"));
            AddFunction(new Rocker(this, devices.CPT_MECH.ToString("d"), F16CCommands.cptCommands.CanopySwitchClose.ToString("d"), F16CCommands.cptCommands.CanopySwitchOpen.ToString("d"), F16CCommands.cptCommands.CanopySwitchClose.ToString("d"), F16CCommands.cptCommands.CanopySwitchOpen.ToString("d"), "606", "Cockpit Mechanics", "Canopy Switch, OPEN/HOLD/CLOSE(momentarily)", true));
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

            AddFunction(new Switch(this, "0", "699", new SwitchPosition[] { new SwitchPosition("1.0", "Open", "0000"), new SwitchPosition("0.0", "Closed", "0000") }, "Nuclear", "Nuclear Consent Cover", "%0.1f"));  //unused
            AddFunction(new Switch(this, "0", "700", SwitchPositions.Create(3, 1.0d, -1.0d, "0000", "Posn", "%0.1f"), "Nuclear", "Nuclear Consent switch", "%0.1f"));  //unused

            AddFunction(new NetworkValue(this, "13", "Network Value", "ADI_LOC_flag", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "14", "Network Value", "ADI_AUX_flag", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new ScaledNetworkValue(this, "15", 
                new CalibrationPointCollectionDouble(-43.0, -1.0, 35.0, 1){
                    new CalibrationPointDouble(-43.0, -1.0),
                    new CalibrationPointDouble(-35.0, -0.807),
                    new CalibrationPointDouble(0.0, 0.0),
                    new CalibrationPointDouble(35.0, 1.0)
                }, "Network Value", "AOA", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new ScaledNetworkValue(this, "16", 
                new CalibrationPointCollectionDouble(-7750.0, -1.0, 6000.0, 1.0){
                    new CalibrationPointDouble(-6000.0, -0.77),
                    new CalibrationPointDouble(0.0, 0.0)
                }, "Network Value", "VVI", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "17", "Network Value", "ADI_Pitch", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "18", "Network Value", "ADI_Bank", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "20", "Network Value", "ADI_LOC_Bar", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "21", "Network Value", "ADI_GS_Bar", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "23", "Network Value", "ADI_TurnRate", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "24", "Network Value", "ADI_Bubble", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "25", "Network Value", "ADI_OFF_flag", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "26", "Network Value", "ADI_GS_flag", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "27", "Network Value", "ADI_GS_Pointer", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new ScaledNetworkValue(this, "47", 
                new CalibrationPointCollectionDouble(0d, 0d, 0.85, 0.850){
                    new CalibrationPointDouble(0.06,    0.042),
                    new CalibrationPointDouble(0.085,   0.095),
                    new CalibrationPointDouble(0.22,    0.152),
                    new CalibrationPointDouble(0.27,    0.182),
                    new CalibrationPointDouble(0.34,    0.199),
                    new CalibrationPointDouble(0.41,    0.255),
                    new CalibrationPointDouble(0.465,   0.303),
                    new CalibrationPointDouble(0.52,    0.355),
                    new CalibrationPointDouble(0.565,   0.402),
                    new CalibrationPointDouble(0.605,   0.455),
                    new CalibrationPointDouble(0.645,   0.500),
                    new CalibrationPointDouble(0.68,    0.552),
                    new CalibrationPointDouble(0.715,   0.600),
                    new CalibrationPointDouble(0.74,    0.653),
                    new CalibrationPointDouble(0.77,    0.698),
                    new CalibrationPointDouble(0.8,     0.750),
                    new CalibrationPointDouble(0.83,    0.797)
                }, "Network Value", "MaxAirspeed", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "51", "Network Value", "Altimeter_100_footPtr", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "52", "Network Value", "Altimeter_10000_footCount", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "53", "Network Value", "Altimeter_1000_footCount", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "54", "Network Value", "Altimeter_100_footCount", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "56", "Network Value", "pressure_setting_3", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "57", "Network Value", "pressure_setting_2", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "58", "Network Value", "pressure_setting_1", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "59", "Network Value", "pressure_setting_0", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "61", "Network Value", "AAU34_PNEU_flag", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "63", "Network Value", "SAI_Pitch", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "64", "Network Value", "SAI_Bank", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "65", "Network Value", "SAI_OFF_flag", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "68", "Network Value", "SAI_AircraftReferenceSymbol", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "69", "Network Value", "SAI_knob_arrow", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "70", "Network Value", "SetAirspeed", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "88", "Network Value", "FuelFlowCounter_10k", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "89", "Network Value", "FuelFlowCounter_1k", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "90", "Network Value", "FuelFlowCounter_100", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            //AddFunction(new FuelFlow(this, "2091", "Fuel System", "Fuel Flow", "Fuel Flow Rate in Pounds per Hour."));
            AddFunction(new NetworkValue(this, "93", "Network Value", "EngineOilPressure", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "94", "Network Value", "EngineNozzlePos", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "95", "Network Value", "EngineTachometer", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "96", "Network Value", "EngineFTIT", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "97", "Network Value", "AlterReleaseRods", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "109", "Network Value", "Autopilot PITCH Switch, ATT HOLD/ A/P OFF/ ALT HOLD  electrically held", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "111", "Network Value", "AuxIntakeDoors", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "357", "Network Value", "ANTI-SKID Switch, PARKING BRAKE/ANTI-SKID/OFF electrically held", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "363", "Network Value", "SpeedBrake_Indicator", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "447", "Network Value", "JFS Switch, START 1/OFF/START 2 electrically held", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "546", "Network Value", "IffCodeDrumDigit1", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "548", "Network Value", "IffCodeDrumDigit2", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "550", "Network Value", "IffCodeDrumDigit3", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "552", "Network Value", "IffCodeDrumDigit4", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "561", "Network Value", "RollTrimInd", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "563", "Network Value", "PitchTrimInd", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "610", "Network Value", "compass H", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "611", "Network Value", "compass V", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "612", "Network Value", "compass rot", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "613", "Network Value", "FuelAL", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "614", "Network Value", "FuelFR", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "615", "Network Value", "SysA_Pressure", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "616", "Network Value", "SysB_Pressure", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "617", "Network Value", "EPU fuel", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "618", "Network Value", "CockpitAltitude", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "621", "Network Value", "CLOCK_currtime_hours", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "622", "Network Value", "CLOCK_currtime_minutes", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "623", "Network Value", "CLOCK_elapsed_time_minutes", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "624", "Network Value", "CLOCK_elapsed_time_seconds", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new ScaledNetworkValue(this, "692", 
                new CalibrationPointCollectionDouble(0.0, -0.3, 1.0, 1.0){
                    new CalibrationPointDouble(0.23,    -0.2),
                    new CalibrationPointDouble(0.455,   -0.1),
                    new CalibrationPointDouble(0.5,    0.0),
                    new CalibrationPointDouble(0.6,    0.1),
                    new CalibrationPointDouble(0.76,    0.2),
                    new CalibrationPointDouble(1.0,    0.3)
                }, "Network Value", "air cond axis", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "712", "Network Value", "CanopyHandle", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "725", "Network Value", "FlowIndicator", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "729", "Network Value", "OxygenPressure", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "730", "Network Value", "FuelTotalizer_10k", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "731", "Network Value", "FuelTotalizer_1k", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "732", "Network Value", "FuelTotalizer_100", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            //AddFunction(new TotalFuel(this, "2090", "Fuel System", "Total Fuel", "Fuel amount shown on the totalizer."));
            AddFunction(new NetworkValue(this, "736", "Network Value", "StickPitch", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "737", "Network Value", "StickRoll", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "755", "Network Value", "Throttle", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "780", "Network Value", "Rudder", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "781", "Network Value", "LeftWheelBrake", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "782", "Network Value", "RightWheelBrake", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "783", "Network Value", "SeatHeight", "Float values from DCS", "", BindingValueUnits.Numeric, null));
            //NetWork Values.NetWorkValue_20.changed" not found at path "Interface; ; Helios.F - 16C_50; DCS F-16C Viper(Helios Version)"   Arg: airspeed Description: Airspeed
            //NetWork Values.NetWorkValue_23.changed" not found at path "Interface; ; Helios.F - 16C_50; DCS F-16C Viper(Helios Version)"   Arg: airspeed Description: MachIndicator
            //NetWork Values.NetWorkValue_74.changed" not found at path "Interface; ; Helios.F - 16C_50; DCS F-16C Viper(Helios Version)"   Arg: 447 Description: JFS Switch, START 1/OFF/START 2    electrically held
            //NetWork Values.NetWorkValue_75.changed" not found at path "Interface; ; Helios.F - 16C_50; DCS F-16C Viper(Helios Version)"   Arg: 357 Description: ANTI-SKID Switch, PARKING BRAKE/ANTI-SKID/OFF  electrically held
            //NetWork Values.NetWorkValue_76.changed" not found at path "Interface; ; Helios.F - 16C_50; DCS F-16C Viper(Helios Version)"   Arg: 692 Description: air cond axis
            //NetWork Values.NetWorkValue_77.changed" not found at path "Interface; ; Helios.F - 16C_50; DCS F-16C Viper(Helios Version)"   Arg: 109 Description: Autopilot PITCH Switch, ATT HOLD/ A/P OFF/ ALT HOLD  electrically held


#endif
            // string profile = ReadProfile(new F16CProfileAnalyzer());

        }
        // virtual internal void AddFunctionsFromDCSModule(IDCSInterfaceCreator ic)
        // {
            // Dictionary<string, string> idValidator = new Dictionary<string, string>();
            // foreach (NetworkFunction nf in MakeFunctionsFromDcsModule(ic))
            // {
                // if (!idValidator.ContainsKey(nf.DataElements[0].ID))
                // {
                    // idValidator.Add(nf.DataElements[0].ID, nf.LocalKey);
                    // AddFunction(nf);
                // }
                // else
                // {
                    // Logger.Warn($"Duplicate Function Found for ID {nf.DataElements[0].ID}: {nf.LocalKey} and {idValidator[nf.DataElements[0].ID]}");
                // }
            // }
        // }
        // virtual internal NetworkFunctionCollection MakeFunctionsFromDcsModule(IDCSInterfaceCreator ic)
        // {
            // NetworkFunctionCollection functions = new NetworkFunctionCollection();
            // foreach (string path in new string[] { Path.Combine(DcsPath, "F-16C", "Cockpit", "Scripts", "clickabledata.lua") })
            // {
                // functions.AddRange(ic.CreateFunctionsFromDcsModule(this, path));
            // }
            // return functions;
        // }
        // virtual protected string DcsPath { get => _dcsPath; set => _dcsPath = value; }

        // virtual internal string ReadProfile(F16CProfileAnalyzer ip)
        // {
            // return ip.ReadProfile(Path.Combine(Environment.GetEnvironmentVariable("UserProfile"), "desktop", "F16C_v1.1b.hpf"), Functions);
        // }

    }
}
