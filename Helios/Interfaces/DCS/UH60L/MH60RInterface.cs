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
//#define CREATEINTERFACE
namespace GadrocsWorkshop.Helios.Interfaces.DCS.UH60L
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Functions;
    using GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Tools;
    using GadrocsWorkshop.Helios.Gauges.UH60L;
    using static System.Net.Mime.MediaTypeNames;
    using System.Security.Policy;
    using static GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Functions.Altimeter;
    using GadrocsWorkshop.Helios.UDPInterface;
    using NLog;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using GadrocsWorkshop.Helios.Interfaces.DCS.H60;

    //using GadrocsWorkshop.Helios.Controls;

    /* enabling this attribute will cause Helios to discover this new interface and make it available for use    */
    [HeliosInterface(
        "Helios.MH60R",                         // Helios internal type ID used in Profile XML, must never change
        "DCS MH-60R Seahawk (Helios)",          // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class MH60RInterface : H60Interface
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _dcsPath = $@"{Environment.GetEnvironmentVariable("userprofile")}\DCS World.openbeta\Mods\Aircraft";

        public MH60RInterface(string name)
            : base(name, "MH-60R", "pack://application:,,,/Helios;component/Interfaces/DCS/UH60L/ExportFunctionsRomeo.lua")
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
#if (CREATEINTERFACE && DEBUG)
            DcsPath = Path.Combine(Environment.GetEnvironmentVariable("userprofile"), "Saved Games","DCS World.openbeta","mods","Aircraft");
            H60InterfaceCreation ic = new H60InterfaceCreation();
            //ic.CrerateDevicesEnum(Path.Combine(DcsPath,"MH-60R"), "GadrocsWorkshop.Helios.Interfaces.DCS.MH60R");
            //ic.CreateCommandsEnum(Path.Combine(DcsPath, "MH-60R"));
            AddFunctionsFromDCSModule();
            return;
#endif
            #region Electric system
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "017", new SwitchPosition[] { new SwitchPosition("1.0", "ON", H60Commands.EFM_commands.batterySwitch.ToString("d")), new SwitchPosition("0.0", "OFF", H60Commands.EFM_commands.batterySwitch.ToString("d")) }, "Electric system", "Battery Switch, ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "018", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.EFM_commands.extPwrSwitch.ToString("d"), H60Commands.EFM_commands.extPwrSwitch.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", H60Commands.EFM_commands.extPwrSwitch2.ToString("d"), H60Commands.EFM_commands.extPwrSwitch2.ToString("d"), "0.0", "0.0") }, "Electric system", "External Power Switch, ON/OFF/RESET", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "019", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.EFM_commands.apuGenSwitch.ToString("d"), H60Commands.EFM_commands.apuGenSwitch.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", H60Commands.EFM_commands.apuGenSwitch2.ToString("d"), H60Commands.EFM_commands.apuGenSwitch2.ToString("d"), "0.0", "0.0") }, "Electric system", "APU GEN Switch, ON/OFF/TEST", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "020", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.EFM_commands.gen1Switch.ToString("d"), H60Commands.EFM_commands.gen1Switch.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", H60Commands.EFM_commands.gen1Switch2.ToString("d"), H60Commands.EFM_commands.gen1Switch2.ToString("d"), "0.0", "0.0") }, "Electric system", "GEN 1 Switch, ON/OFF/TEST", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "021", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.EFM_commands.gen2Switch.ToString("d"), H60Commands.EFM_commands.gen2Switch.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", H60Commands.EFM_commands.gen2Switch2.ToString("d"), H60Commands.EFM_commands.gen2Switch2.ToString("d"), "0.0", "0.0") }, "Electric system", "GEN 2 Switch, ON/OFF/TEST", "%0.1f"));
            #endregion Electric system
            #region Fuel and Engines
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "022", new SwitchPosition[] { new SwitchPosition("-1.0", "FUEL PRIME", H60Commands.EFM_commands.switchFuelPump.ToString("d")), new SwitchPosition("0.0", "OFF", H60Commands.EFM_commands.switchFuelPump.ToString("d")), new SwitchPosition("1.0", "APU BOOST", H60Commands.EFM_commands.switchFuelPump.ToString("d")) }, "Fuel and Engines", "Fuel Pump Switch, FUEL PRIME/OFF/APU BOOST", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "023", new SwitchPosition[] { new SwitchPosition("-1.0", "APU", H60Commands.EFM_commands.switchAirSource.ToString("d")), new SwitchPosition("0.0", "OFF", H60Commands.EFM_commands.switchAirSource.ToString("d")), new SwitchPosition("1.0", "ENG", H60Commands.EFM_commands.switchAirSource.ToString("d")) }, "Fuel and Engines", "Air Source Switch, APU/OFF/ENG", "%0.1f"));
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "024", new SwitchPosition[] { new SwitchPosition("1.0", "ON", H60Commands.EFM_commands.switchAPU.ToString("d")), new SwitchPosition("0.0", "OFF", H60Commands.EFM_commands.switchAPU.ToString("d")) }, "Fuel and Engines", "APU CONTROL, ON/OFF", "%0.1f"));
            #endregion Fuel and Engines
            #region PNT-025 APU EXTINGUISH
            #endregion PNT-025 APU EXTINGUISH
            #region STAB PANEL
            #endregion STAB PANEL
            #region FUEL PUMPS
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "040", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.EFM_commands.fuelPumpL.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.EFM_commands.fuelPumpL.ToString("d")) }, "FUEL PUMPS", "No. 1 Fuel Boost Pump ON/OFF", "%0.1f"));
            #endregion FUEL PUMPS
            #region Engine Control Locks
            #endregion Engine Control Locks
            #region PILOT BARO ALTIMETER
            AddFunction(new Axis(this, devices.PLTAAU32A.ToString("d"), H60Commands.device_commands.pilotBarometricScaleSet.ToString("d"), "063", 0.1d, 0.0d, 0d, "PILOT BARO ALTIMETER", "Barometric Scale Set", false, "%0.1f"));
            #endregion PILOT BARO ALTIMETER
            #region COPILOT BARO ALTIMETER
            AddFunction(new Axis(this, devices.CPLTAAU32A.ToString("d"), H60Commands.device_commands.copilotBarometricScaleSet.ToString("d"), "073", 0.1d, 0.0d, 0d, "COPILOT BARO ALTIMETER", "Barometric Scale Set", false, "%0.1f"));
            #endregion COPILOT BARO ALTIMETER
            #region PARKING BRAKE
            AddFunction(new Switch(this, devices.EFM_HELPER.ToString("d"), "080", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.parkingBrake.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.parkingBrake.ToString("d")) }, "PARKING BRAKE", "Parking Brake ON/OFF", "%0.1f"));
            #endregion PARKING BRAKE
            #region AHRU
            #endregion AHRU
            #region PILOT HSI
            #endregion PILOT HSI
            #region COPILOT HSI
            #endregion COPILOT HSI
            #region MISC
            AddFunction(new PushButton(this, devices.MISC.ToString("d"), H60Commands.device_commands.miscTailWheelLock.ToString("d"), "291", "MISC", "Tail Wheel Lock", "%1d"));
            #endregion MISC
            #region CAUTION/DISPLAY PANELS
            AddFunction(new Switch(this, devices.VIDS.ToString("d"), "301", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.cduLampTest.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.cduLampTest.ToString("d")) }, "CAUTION/DISPLAY PANELS", "CDU Lamp Test", "%0.1f"));
            AddFunction(new Switch(this, devices.VIDS.ToString("d"), "302", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.pilotPDUTest.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.pilotPDUTest.ToString("d")) }, "CAUTION/DISPLAY PANELS", "Pilot Lamp Test", "%0.1f"));
            AddFunction(new Switch(this, devices.VIDS.ToString("d"), "303", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.copilotPDUTest.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.copilotPDUTest.ToString("d")) }, "CAUTION/DISPLAY PANELS", "Copilot Lamp Test", "%0.1f"));
            #endregion CAUTION/DISPLAY PANELS
            #region CIS/MODE SEL
            AddFunction(new PushButton(this, devices.CISP.ToString("d"), H60Commands.device_commands.PilotCISHdgToggle.ToString("d"), "930", "CIS/MODE SEL", "CIS Heading Mode ON/OFF", "%1d"));
            AddFunction(new PushButton(this, devices.CISP.ToString("d"), H60Commands.device_commands.PilotCISNavToggle.ToString("d"), "931", "CIS/MODE SEL", "CIS Nav Mode ON/OFF", "%1d"));
            AddFunction(new PushButton(this, devices.CISP.ToString("d"), H60Commands.device_commands.PilotCISAltToggle.ToString("d"), "932", "CIS/MODE SEL", "CIS Altitude Hold Mode ON/OFF", "%1d"));
            #endregion CIS/MODE SEL
            #region AN/AVS-7 PANEL
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1100", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.setAVS7Power.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.setAVS7Power.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.setAVS7Power.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 OFF/ON/ADJUST", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1101", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Program Pilot/Copilot (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1102", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Pilot MODE 1-4/DCLT (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1103", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Copilot MODE 1-4/DCLT (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1104", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 BIT/ACK (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1105", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 ALT/P/R DEC/INC PGM NXT/SEL (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1106", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.incAVS7Brightness.ToString("d"), H60Commands.device_commands.incAVS7Brightness.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", H60Commands.device_commands.decAVS7Brightness.ToString("d"), H60Commands.device_commands.decAVS7Brightness.ToString("d"), "0.0", "0.0") }, "AN/AVS-7 PANEL", "AN/AVS-7 Pilot BRT/DIM", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1107", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Pilot DSPL POS D/U (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1108", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Pilot DSPL POS L/R (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1109", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Copilot BRT/DIM (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1110", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Copilot DSPL POS D/U (Inop)", "%0.1f"));
            AddFunction(new Switch(this, devices.AVS7.ToString("d"), "1111", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.foo.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.foo.ToString("d")) }, "AN/AVS-7 PANEL", "AN/AVS-7 Copilot DSPL POS L/R (Inop)", "%0.1f"));
            #endregion AN/AVS-7 PANEL
            #region AN/ARC-164
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "050", SwitchPositions.Create(4, 0d, 0.01d, H60Commands.device_commands.arc164_mode.ToString("d"), "Posn", "%0.2f"), "AN/ARC-164", "AN/ARC-164 Mode", "%0.2f"));
            AddFunction(new Axis(this, devices.ARC164.ToString("d"), H60Commands.device_commands.arc164_volume.ToString("d"), "051", 0.1d, 0.0d, 1.0d, "AN/ARC-164", "AN/ARC-164 Volume", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "052", SwitchPositions.Create(4, 0d, 0.01d, H60Commands.device_commands.arc164_xmitmode.ToString("d"), "Posn", "%0.2f"), "AN/ARC-164", "AN/ARC-164 Manual/Preset/Guard", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "053", SwitchPositions.Create(2, 0d, 0.1d, H60Commands.device_commands.arc164_freq_Xooooo.ToString("d"), new string[] { "AN", "" }, "%0.1f"), "AN/ARC-164", "AN/ARC-164 100s", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "054", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arc164_freq_oXoooo.ToString("d"), "Posn", "%0.1f"), "AN/ARC-164", "AN/ARC-164 10s", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "055", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arc164_freq_ooXooo.ToString("d"), "Posn", "%0.1f"), "AN/ARC-164", "AN/ARC-164 1s", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "056", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arc164_freq_oooXoo.ToString("d"), "Posn", "%0.1f"), "AN/ARC-164", "AN/ARC-164 .1s", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "057", SwitchPositions.Create(4, 0d, 0.1d, H60Commands.device_commands.arc164_freq_ooooXX.ToString("d"), "Posn", "%0.1f"), "AN/ARC-164", "AN/ARC-164 .010s", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC164.ToString("d"), "058", SwitchPositions.Create(20, 0d, 0.05d, H60Commands.device_commands.arc164_preset.ToString("d"), "Posn", "%0.2f"), "AN/ARC-164", "AN/ARC-164 Preset", "%0.2f"));
            #endregion AN/ARC-164
            #region Pilot APN-209 Radar Altimeter
            #endregion Pilot APN-209 Radar Altimeter
            #region Lighting
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.glareshieldLights.ToString("d"), "251", 0.1d, 0.0d, 1.0d, "Lighting", "Glareshield Lights OFF/BRT", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "252", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.posLightIntensity.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.posLightIntensity.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.posLightIntensity.ToString("d")) }, "Lighting", "Position Lights DIM/OFF/BRT", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "253", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.posLightMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.posLightMode.ToString("d")) }, "Lighting", "Position Lights STEADY/FLASH", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "254", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.antiLightGrp.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.antiLightGrp.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.antiLightGrp.ToString("d")) }, "Lighting", "Anticollision Lights UPPER/BOTH/LOWER", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "255", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.antiLightMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.antiLightMode.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.antiLightMode.ToString("d")) }, "Lighting", "Anticollision Lights DAY/OFF/NIGHT", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "256", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.navLightMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.navLightMode.ToString("d")) }, "Lighting", "Nav Lights NORM/IR", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "257", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.cabinLightMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.cabinLightMode.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.cabinLightMode.ToString("d")) }, "Lighting", "Cabin Lights BLUE/OFF/WHITE", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.cpltInstrLights.ToString("d"), "259", 0.1d, 0.0d, 1.0d, "Lighting", "Copilot Flight Instrument Lights OFF/BRT", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.lightedSwitches.ToString("d"), "260", 0.1d, 0.0d, 1.0d, "Lighting", "Lighted Switches OFF/BRT", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "261", SwitchPositions.Create(6, 0d, 0.2d, H60Commands.device_commands.formationLights.ToString("d"), "Posn", "%0.1f"), "Lighting", "Formation Lights OFF/1/2/3/4/5", "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.upperConsoleLights.ToString("d"), "262", 0.1d, 0.0d, 1.0d, "Lighting", "Upper Console Lights OFF/BRT", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.lowerConsoleLights.ToString("d"), "263", 0.1d, 0.0d, 1.0d, "Lighting", "Lower Console Lights OFF/BRT", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.pltInstrLights.ToString("d"), "264", 0.1d, 0.0d, 1.0d, "Lighting", "Pilot Flight Instrument Lights OFF/BRT", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.nonFltInstrLights.ToString("d"), "265", 0.1d, 0.0d, 1.0d, "Lighting", "Non Flight Instrument Lights OFF/BRT", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.pltRdrAltLights.ToString("d"), "266", 0.1d, 0.0d, 1.0d, "Lighting", "Radar Altimeter Dimmer", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EXTLIGHTS.ToString("d"), H60Commands.device_commands.cpltRdrAltLights.ToString("d"), "267", 0.1d, 0.0d, 1.0d, "Lighting", "Copilot Radar Altimeter Dimmer", false, "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "268", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.magCompassLights.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.magCompassLights.ToString("d")) }, "Lighting", "Magnetic Compass Light ON/OFF", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLIGHTS.ToString("d"), "269", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.cockpitLightMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.cockpitLightMode.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.cockpitLightMode.ToString("d")) }, "Lighting", "Cockpit Lights BLUE/OFF/WHITE", "%0.1f"));
            #endregion Lighting
            #region AN/APR-39
            AddFunction(new Switch(this, devices.APR39.ToString("d"), "270", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.apr39Power.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.apr39Power.ToString("d")) }, "AN/APR-39", "AN/APR-39 Power ON/OFF", "%0.1f"));
            AddFunction(new Axis(this, devices.APR39.ToString("d"), H60Commands.device_commands.apr39Volume.ToString("d"), "273", 0.1d, 0.0d, 1.0d, "AN/APR-39", "AN/APR-39 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.APR39.ToString("d"), H60Commands.device_commands.apr39Brightness.ToString("d"), "274", 0.1d, 0.0d, 1.0d, "AN/APR-39", "AN/APR-39 Brilliance", false, "%0.1f"));
            #endregion AN/APR-39
            #region PILOT LC6 CHRONOMETER
            AddFunction(new PushButton(this, devices.PLTLC6.ToString("d"), H60Commands.device_commands.resetSetBtn.ToString("d"), "280", "PILOT LC6 CHRONOMETER", "Pilot's Chronometer RESET/SET Button", "%1d"));
            AddFunction(new PushButton(this, devices.PLTLC6.ToString("d"), H60Commands.device_commands.modeBtn.ToString("d"), "281", "PILOT LC6 CHRONOMETER", "Pilot's Chronometer MODE Button", "%1d"));
            AddFunction(new PushButton(this, devices.PLTLC6.ToString("d"), H60Commands.device_commands.startStopAdvBtn.ToString("d"), "282", "PILOT LC6 CHRONOMETER", "Pilot's Chronometer START/STOP/ADVANCE Button", "%1d"));
            #endregion PILOT LC6 CHRONOMETER
            #region COPILOT LC6 CHRONOMETER
            AddFunction(new PushButton(this, devices.CPLTLC6.ToString("d"), H60Commands.device_commands.resetSetBtn.ToString("d"), "283", "COPILOT LC6 CHRONOMETER", "Copilot's Chronometer RESET/SET Button", "%1d"));
            AddFunction(new PushButton(this, devices.CPLTLC6.ToString("d"), H60Commands.device_commands.modeBtn.ToString("d"), "284", "COPILOT LC6 CHRONOMETER", "Copilot's Chronometer MODE Button", "%1d"));
            AddFunction(new PushButton(this, devices.CPLTLC6.ToString("d"), H60Commands.device_commands.startStopAdvBtn.ToString("d"), "285", "COPILOT LC6 CHRONOMETER", "Copilot's Chronometer START/STOP/ADVANCE Button", "%1d"));
            #endregion COPILOT LC6 CHRONOMETER
            #region PILOT ICS PANEL
            #endregion PILOT ICS PANEL
            #region ARC-186 VHF
            AddFunction(new Axis(this, devices.ARC186.ToString("d"), H60Commands.device_commands.arc186Volume.ToString("d"), "410", 0.1d, 0.0d, 1.0d, "ARC-186 VHF", "AN/ARC-186 Volume", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "412", SwitchPositions.Create(13, 0d, 0.083d, H60Commands.device_commands.arc186Selector10MHz.ToString("d"), "Posn", "%0.3f"), "ARC-186 VHF", "AN/ARC-186 10MHz Selector", "%0.3f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "413", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arc186Selector1MHz.ToString("d"), "Posn", "%0.1f"), "ARC-186 VHF", "AN/ARC-186 1MHz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "414", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arc186Selector100KHz.ToString("d"), "Posn", "%0.1f"), "ARC-186 VHF", "AN/ARC-186 100KHz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "415", SwitchPositions.Create(4, 0d, 0.25d, H60Commands.device_commands.arc186Selector25KHz.ToString("d"), "Posn", "%0.2f"), "ARC-186 VHF", "AN/ARC-186 25KHz Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "416", SwitchPositions.Create(4, 0d, 0.333d, H60Commands.device_commands.arc186FreqSelector.ToString("d"), "Posn", "%0.3f"), "ARC-186 VHF", "AN/ARC-186 Frequency Control Selector", "%0.3f"));
            AddFunction(new PushButton(this, devices.ARC186.ToString("d"), H60Commands.device_commands.arc186Load.ToString("d"), "417", "ARC-186 VHF", "AN/ARC-186 Load Pushbutton", "%1d"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "418", SwitchPositions.Create(20, 0d, 0.05d, H60Commands.device_commands.arc186PresetSelector.ToString("d"), "Posn", "%0.2f"), "ARC-186 VHF", "AN/ARC-186 Preset Channel Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC186.ToString("d"), "419", SwitchPositions.Create(3, 0d, 0.5d, H60Commands.device_commands.arc186ModeSelector.ToString("d"), "Posn", "%0.1f"), "ARC-186 VHF", "AN/ARC-186 Mode Selector", "%0.1f"));
            #endregion ARC-186 VHF
            #region AFMS
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "460", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.afmcpXferMode.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.afmcpXferMode.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.afmcpXferMode.ToString("d")) }, "AFMS", "Aux Fuel Transfer Mode MAN/OFF/AUTO", "%0.1f"));
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "461", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.afmcpManXfer.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.afmcpManXfer.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.afmcpManXfer.ToString("d")) }, "AFMS", "Aux Fuel Manual Transfer RIGHT/BOTH/LEFT", "%0.1f"));
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "462", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.afmcpXferFrom.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.afmcpXferFrom.ToString("d")) }, "AFMS", "Aux Fuel Transfer From OUTBD/INBD", "%0.1f"));
            AddFunction(new Switch(this, devices.AFMS.ToString("d"), "463", SwitchPositions.Create(4, 0d, 0.333d, H60Commands.device_commands.afmcpPress.ToString("d"), "Posn", "%0.3f"), "AFMS", "Aux Fuel Pressurization Selector", "%0.3f"));
            #endregion AFMS
            #region DOORS
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "470", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.doorCplt.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.doorCplt.ToString("d")) }, "DOORS", "Copilot Door", "%0.1f"));
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "471", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.doorPlt.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.doorPlt.ToString("d")) }, "DOORS", "Pilot Door", "%0.1f"));
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "472", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.doorLGnr.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.doorLGnr.ToString("d")) }, "DOORS", "Left Gunner Window", "%0.1f"));
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "473", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.doorRGnr.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.doorRGnr.ToString("d")) }, "DOORS", "Right Gunner Window", "%0.1f"));
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "474", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.doorLCargo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.doorLCargo.ToString("d")) }, "DOORS", "Left Cargo Door", "%0.1f"));
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "475", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.doorRCargo.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.doorRCargo.ToString("d")) }, "DOORS", "Right Cargo Door", "%0.1f"));
            #endregion DOORS
            #region M130 CM System
            AddFunction(new Switch(this, devices.M130.ToString("d"), "550", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.cmFlareDispenseModeCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.cmFlareDispenseModeCover.ToString("d")) }, "M130 CM System", "Flare Dispenser Mode Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.M130.ToString("d"), "551", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.cmFlareDispenseModeSwitch.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.cmFlareDispenseModeSwitch.ToString("d")) }, "M130 CM System", "Flare Dispenser Switch", "%0.1f"));
            #endregion M130 CM System
            #region cmFlareDispenseMode
            AddFunction(new Switch(this, devices.M130.ToString("d"), "552", SwitchPositions.Create(10, 0d, 0.111d, H60Commands.device_commands.cmFlareCounterDial.ToString("d"), "Posn", "%0.3f"), "cmFlareDispenseMode", "Flare Counter", "%0.3f"));
            AddFunction(new Switch(this, devices.M130.ToString("d"), "553", SwitchPositions.Create(10, 0d, 0.111d, H60Commands.device_commands.cmChaffCounterDial.ToString("d"), "Posn", "%0.3f"), "cmFlareDispenseMode", "Chaff Counter", "%0.3f"));
            AddFunction(new Switch(this, devices.M130.ToString("d"), "559", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.cmArmSwitch.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.cmArmSwitch.ToString("d")) }, "cmFlareDispenseMode", "Countermeasures Arming Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.M130.ToString("d"), "560", SwitchPositions.Create(3, 0d, 0.5d, H60Commands.device_commands.cmProgramDial.ToString("d"), "Posn", "%0.3f"), "cmFlareDispenseMode", "Chaff Dispenser Mode Selector", "%0.3f"));
            AddFunction(new PushButton(this, devices.M130.ToString("d"), H60Commands.device_commands.cmChaffDispense.ToString("d"), "561", "cmFlareDispenseMode", "Chaff Dispense", "%1d"));
            #endregion cmFlareDispenseMode
            #region ARC-201 FM1
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "600", SwitchPositions.Create(8, 0d, 0.01d, H60Commands.device_commands.fm1PresetSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM1", "AN/ARC-201 (FM1) PRESET Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "601", SwitchPositions.Create(9, 0d, 0.01d, H60Commands.device_commands.fm1FunctionSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM1", "AN/ARC-201 (FM1) FUNCTION Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "602", SwitchPositions.Create(4, 0d, 0.01d, H60Commands.device_commands.fm1PwrSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM1", "AN/ARC-201 (FM1) PWR Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM1.ToString("d"), "603", SwitchPositions.Create(4, 0d, 0.01d, H60Commands.device_commands.fm1ModeSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM1", "AN/ARC-201 (FM1) MODE Selector", "%0.2f"));
            AddFunction(new Axis(this, devices.ARC201_FM1.ToString("d"), H60Commands.device_commands.fm1Volume.ToString("d"), "604", 0.1d, 0.0d, 1.0d, "ARC-201 FM1", "AN/ARC-201 (FM1) Volume", false, "%0.1f"));
            #endregion ARC-201 FM1
            #region AN/ARN-149
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "620", SwitchPositions.Create(3, 0d, 0.5d, H60Commands.device_commands.arn149Preset.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 PRESET Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "621", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.arn149ToneTest.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.arn149ToneTest.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.arn149ToneTest.ToString("d")) }, "AN/ARN-149", "AN/ARN-149 TONE/OFF/TEST", "%0.1f"));
            AddFunction(new Axis(this, devices.ARN149.ToString("d"), H60Commands.device_commands.arn149Volume.ToString("d"), "622", 0.1d, 0.0d, 1.0d, "AN/ARN-149", "AN/ARN-149 Volume", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "624", SwitchPositions.Create(3, 0d, 0.5d, H60Commands.device_commands.arn149Power.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 POWER Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "625", SwitchPositions.Create(3, 0d, 0.5d, H60Commands.device_commands.arn149thousands.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 1000s Khz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "626", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arn149hundreds.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 100s Khz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "627", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arn149tens.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 10s Khz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "628", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arn149ones.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 1s Khz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN149.ToString("d"), "629", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arn149tenths.ToString("d"), "Posn", "%0.1f"), "AN/ARN-149", "AN/ARN-149 .1s Khz Selector", "%0.1f"));
            #endregion AN/ARN-149
            #region AN/ARN-147
            AddFunction(new Switch(this, devices.ARN147.ToString("d"), "650", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arn147MHz.ToString("d"), "Posn", "%0.1f"), "AN/ARN-147", "AN/ARN-147 MHz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN147.ToString("d"), "651", SwitchPositions.Create(10, 0d, 0.1d, H60Commands.device_commands.arn147KHz.ToString("d"), "Posn", "%0.1f"), "AN/ARN-147", "AN/ARN-147 KHz Selector", "%0.1f"));
            AddFunction(new Switch(this, devices.ARN147.ToString("d"), "653", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.arn147Power.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.arn147Power.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.arn147Power.ToString("d")) }, "AN/ARN-147", "AN/ARN-147 Power Selector OFF/ON/TEST", "%0.1f"));
            #endregion AN/ARN-147
            #region WIPERS
            AddFunction(new Switch(this, devices.MISC.ToString("d"), "631", SwitchPositions.Create(4, -0.5d, 0.5d, H60Commands.device_commands.wiperSelector.ToString("d"), new string[] { "PARK", "OFF", "LOW", "HI" }, "%0.1f"), "WIPERS", "Wipers PARK/OFF/LOW/HI", "%0.1f"));
            #endregion WIPERS
            #region ARC-201 FM2
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "700", SwitchPositions.Create(8, 0d, 0.01d, H60Commands.device_commands.fm2PresetSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM2", "AN/ARC-201 (FM2) PRESET Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "701", SwitchPositions.Create(9, 0d, 0.01d, H60Commands.device_commands.fm2FunctionSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM2", "AN/ARC-201 (FM2) FUNCTION Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "702", SwitchPositions.Create(4, 0d, 0.01d, H60Commands.device_commands.fm2PwrSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM2", "AN/ARC-201 (FM2) PWR Selector", "%0.2f"));
            AddFunction(new Switch(this, devices.ARC201_FM2.ToString("d"), "703", SwitchPositions.Create(4, 0d, 0.01d, H60Commands.device_commands.fm2ModeSelector.ToString("d"), "Posn", "%0.2f"), "ARC-201 FM2", "AN/ARC-201 (FM2) MODE Selector", "%0.2f"));
            AddFunction(new Axis(this, devices.ARC201_FM2.ToString("d"), H60Commands.device_commands.fm2Volume.ToString("d"), "704", 0.1d, 0.0d, 1.0d, "ARC-201 FM2", "AN/ARC-201 (FM2) Volume", false, "%0.1f"));
            #endregion ARC-201 FM2
            #region CPLT ICP
            #endregion CPLT ICP
            #region AUX SYSTEM CONTROL PANEL
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "1998", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.masterSonarCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.masterSonarCover.ToString("d")) }, "AUX SYSTEM CONTROL PANEL", "Master Sonar Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "1999", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.masterSonarArm.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.masterSonarArm.ToString("d")) }, "AUX SYSTEM CONTROL PANEL", "Master Arm Sonar", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2000", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.masterSonoArm.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.masterSonoArm.ToString("d")) }, "AUX SYSTEM CONTROL PANEL", "Master Arm Sonobuoys", "%0.1f"));
            AddFunction(new PushButton(this, devices.WEAPONS.ToString("d"), H60Commands.device_commands.buoysDispense.ToString("d"), "2001", "AUX SYSTEM CONTROL PANEL", "Sonobuoy Dispense", "%1d"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2002", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", H60Commands.device_commands.setSonarLift.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.setSonarLift.ToString("d")), new SwitchPosition("1.0", "Posn 3", H60Commands.device_commands.setSonarLift.ToString("d")) }, "AUX SYSTEM CONTROL PANEL", "Sonar DOWN/STOP/UP", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2003", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.masterSonoCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.masterSonoCover.ToString("d")) }, "AUX SYSTEM CONTROL PANEL", "Master Sonobuoys Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2003", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.masterSonoCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.masterSonoCover.ToString("d")) }, "AUX SYSTEM CONTROL PANEL", "Master Sonobuoys Cover", "%0.1f"));
            #endregion AUX SYSTEM CONTROL PANEL
            #region WEAPONS SYSTEM CONTROL PANEL
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2004", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.wpnsetMasterArm.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.wpnsetMasterArm.ToString("d")) }, "WEAPONS SYSTEM CONTROL PANEL", "Weapons Master Arm", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2005", SwitchPositions.Create(7, 0d, 0.1d, H60Commands.device_commands.wpnSalveSelector.ToString("d"), "Posn", "%0.1f"), "WEAPONS SYSTEM CONTROL PANEL", "Select Salve", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2006", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.wpnMasterArmCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.wpnMasterArmCover.ToString("d")) }, "WEAPONS SYSTEM CONTROL PANEL", "Master Arm Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2007", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.wpnStationLO.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.wpnStationLO.ToString("d")) }, "WEAPONS SYSTEM CONTROL PANEL", "Stat. Left Pyl.", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2008", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.wpnStationLI.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.wpnStationLI.ToString("d")) }, "WEAPONS SYSTEM CONTROL PANEL", "Stat. Left Side", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2009", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.wpnStationRI.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.wpnStationRI.ToString("d")) }, "WEAPONS SYSTEM CONTROL PANEL", "Stat. Right Side", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2010", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.wpnStationRO.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.wpnStationRO.ToString("d")) }, "WEAPONS SYSTEM CONTROL PANEL", "Stat. Right Pyl.", "%0.1f"));
            #endregion WEAPONS SYSTEM CONTROL PANEL
            #region JETTISON
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2011", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.jettSelectSwitch.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.jettSelectSwitch.ToString("d")) }, "JETTISON", "Select Jettison Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2012", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.jettSelectCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.jettSelectCover.ToString("d")) }, "JETTISON", "Select Jettison Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2013", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", H60Commands.device_commands.jettAllCover.ToString("d")), new SwitchPosition("1.0", "Posn 2", H60Commands.device_commands.jettAllCover.ToString("d")) }, "JETTISON", "All Jettison Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2014", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.jettAllSwitch.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.jettAllSwitch.ToString("d")) }, "JETTISON", "All Jettison Switch", "%0.1f"));
            #endregion JETTISON
            #region Don't use arg 2015 ...
            AddFunction(new Switch(this, devices.WEAPONS.ToString("d"), "2016", SwitchPositions.Create(8, 0d, 0.1d, H60Commands.device_commands.jettStationSel.ToString("d"), "Posn", "%0.1f"), "Don't use arg 2015 ...", "Select Stations", "%0.1f"));
            #endregion Don't use arg 2015 ...
            #region DEBUG
            AddFunction(new Switch(this, devices.DEBUG.ToString("d"), "3000", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", H60Commands.device_commands.visualisationToggle.ToString("d")), new SwitchPosition("0.0", "Posn 2", H60Commands.device_commands.visualisationToggle.ToString("d")) }, "DEBUG", "Debug Visualisation ON/OFF", "%0.1f"));
            #endregion DEBUG

        }
        virtual protected void AddFunctionsFromDCSModule()
        {
            Dictionary<string, string> idValidator = new Dictionary<string, string>();
            foreach (NetworkFunction nf in MakeFunctionsFromDcsModule())
            {
                if (!idValidator.ContainsKey(nf.DataElements[0].ID))
                {
                    idValidator.Add(nf.DataElements[0].ID, nf.LocalKey);
                    AddFunction(nf);
                }
                else
                {
                    Logger.Warn($"Duplicate Function Found for ID {nf.DataElements[0].ID}: {nf.LocalKey} and {idValidator[nf.DataElements[0].ID]}");
                }
            }
        }
        virtual protected NetworkFunctionCollection MakeFunctionsFromDcsModule()
        {
            NetworkFunctionCollection functions = new NetworkFunctionCollection();
            H60InterfaceCreation ic = new H60InterfaceCreation();
            foreach (string path in new string[] { Path.Combine(DcsPath, "MH-60R", "Cockpit", "Scripts", "clickabledata.lua") })
            {
                functions.AddRange(ic.CreateFunctionsFromDcsModule(this, path));
            }
            return functions;
        }
        virtual protected string DcsPath { get => _dcsPath; set => _dcsPath = value; }


    }
}
