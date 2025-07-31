//  Copyright 2020 Ammo Goettsch
//  Copyright 2024 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.DH98Mosquito
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using GadrocsWorkshop.Helios.UDPInterface;

    [HeliosInterface(
        "Helios.DH98Mosquito",                         // Helios internal type ID used in Profile XML, must never change
        "DCS DH98 Mosquito FB Mk VI",                   // human readable UI name for this interface
        typeof(DCSInterfaceEditor),            // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class DH98MosquitoInterface : DCSInterface
    {
#pragma warning disable IDE1006 // Naming Standard issues
#pragma warning disable IDE0051 // Remove unused private members

        #region Devices
        private enum devices
        {
            FM_PROXY = 1,
            CONTROLS,
            ENGINE_CONTROLS,
            SWITCHBOARD,
            WEAPONS,
            TERTIARY_REFLECTS,
            COMPASS,
            REPEATER_COMPASS,
            AH,
            DI,
            TURNSLIP_INDICATOR,
            PORT_SUCTION_PUMP,
            STBD_SUCTION_PUMP,
            OXYGEN,
            PORT_OXYGEN_REGULATOR,
            STBD_OXYGEN_REGULATOR,
            THREE_WAY_SELECTOR,
            FOOTAGE_INDICATOR,
            CLOCK,
            LH_5C1878,
            RH_5C1878,
            FLUORESCENT,
            GLYCOL_PUMP,
            VHF_RADIO,
            RADIO_INTERFACE,
            RADIO_T1154,
            RADIO_R1155,
            RADIO_POWERUNIT,
            INTERCOM,
            SBA,
            AERIAL_WINCH,
            ROCKETS,
            HEARING_SENS,
            HELMET_DEVICE,
            MACROS,
            ARCADE,
            CREWE,
            KNEEBOARD
        };

        #endregion
        #region Command_Defs.lua

        private enum electric_commands
        {
            GEN1_RST_SW = 3001
        };

        #endregion
        #region MainPanel/lamps.lua
        private enum Warning_Lights
        {
            FLAG_MasterWarningPLT = 424,
        };
        #endregion
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE1006 // Naming Standard issues

        public DH98MosquitoInterface(string name)
            : base(name, "MosquitoFBMkVI", "pack://application:,,,/DH98Mosquito;component/Interfaces/ExportFunctions.lua")
        {

            // not setting Vehicles at all results in the module name identifying the only 
            // supported aircraft
            // XXX not yet supported
            // Vehicles = new string[] { ModuleName, "other aircraft", "another aircraft" };

// see if we can restore from JSON
#if (!DEBUG)
                        //if (LoadFunctionsFromJson())
                        //{
                        //    return;
                        //}
#endif
// #define DEV
#if (DEV)

            Dictionary<string, string> dcsBiosArgs = ProcessDCSBios.GetFunctions();
            Dictionary<string, FunctionData> clickableArgs = ProcessClickables.GetFunctions();
#endif

            AddFunction(new PushButton(this, devices.WEAPONS.ToString("d"), "3005", "245", "Stick", "Button A - Gun Trigger", "%1d"));   // "STICK_BTN_A"
            AddFunction(new PushButton(this, devices.WEAPONS.ToString("d"), "3006", "246", "Stick", "Button B1 - Cannon Trigger", "%1d"));   // "STICK_BTN_B1"
            AddFunction(new PushButton(this, devices.WEAPONS.ToString("d"), "3007", "244", "Stick", "Button B2 - Secondary & Drop Ordnance Trigger", "%1d"));   // "STICK_BTN_B2"
            AddFunction(new Axis(this, devices.CONTROLS.ToString("d"), "3001", "248", 0.1d, 0d, 1d, "Stick", "Wheel Brakes Lever", false, "%.1f"));   // "STICK_WH_BRK"
            AddFunction(new PushButton(this, devices.CONTROLS.ToString("d"), "3004", "291", "Stick", "Wheel Brakes Lever Lock", "%1d"));   // "STICK_WH_BRK_LOCK"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3022", "251", "0", "Position 1", "1", "Position 2", "Cockpit", "Side Window Open/Close Left", "%1d"));   // "WINDOW_CONTROL_L"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3063", "253", "0", "Position 1", "1", "Position 2", "Cockpit", "Side Window Open/Close Right", "%1d"));   // "WINDOW_CONTROL_R"
            AddFunction(new PushButton(this, devices.CONTROLS.ToString("d"), "3071", "312", "Cockpit", "Armour Headrest Lock", "%1d"));   // "ARMOR_HEADREST"
            AddFunction(new Axis(this, devices.WEAPONS.ToString("d"), "3045", "107", 0.1d, 0d, 1d, "Reflector Sight", "Setter Rings Range", false, "%.3f"));   // "GUNSIGHT_RANGE"
            AddFunction(new Axis(this, devices.WEAPONS.ToString("d"), "3048", "108", 0.1d, 0d, 1d, "Reflector Sight", "Setter Rings Span", false, "%.3f"));   // "GUNSIGHT_SPAN"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3009", "13", 0.1d, 0d, 1d, "Reflector Sight", "Sight Dimmer", false, "%.3f"));   // "GUNSIGHT_L_DIM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3009", "124", "1", "Position 1", "0", "Position 2", "JBA", "Master Switch Gate", "%1d"));   // "MASTER_SWITCH"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3062", "112", "0", "Position 1", "1", "Position 2", "JBA", "Radiator Flap Switch Port", "%1d"));   // "PORT_RAD_FLAP"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3064", "113", "0", "Position 1", "1", "Position 2", "JBA", "Radiator Flap Switch Starboard", "%1d"));   // "STBD_RAD_FLAP"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3054", "131", "JBA", "Starter Switch Port", "%1d"));   // "PORT_START_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3052", "129", "1", "Position 1", "0", "Position 2", "JBA", "Starter Switch Cover Port", "%1d"));   // "PORT_START_CVR"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3057", "132", "JBA", "Starter Switch Starboard", "%1d"));   // "STBD_START_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3055", "130", "1", "Position 1", "0", "Position 2", "JBA", "Starter Switch Cover Starboard", "%1d"));   // "STBD_START_CVR"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3060", "135", "JBA", "Booster Coil Port", "%1d"));   // "PORT_BOOST_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3058", "133", "1", "Position 1", "0", "Position 2", "JBA", "Booster Switch Cover Port", "%1d"));   // "PORT_BOOST_CVR"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3063", "136", "JBA", "Booster Coil Starboard", "%1d"));   // "STBD_BOOST_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3061", "134", "1", "Position 1", "0", "Position 2", "JBA", "Booster Switch Cover Starboard", "%1d"));   // "STBD_BOOST_CVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3001", "125", "1", "Position 1", "0", "Position 2", "JBA", "Magneto 1 Switch Port", "%1d"));   // "PORT_MAGNETO_1"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3003", "126", "1", "Position 1", "0", "Position 2", "JBA", "Magneto 2 Switch Port", "%1d"));   // "PORT_MAGNETO_2"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3005", "127", "1", "Position 1", "0", "Position 2", "JBA", "Magneto 1 Switch Starboard", "%1d"));   // "STBD_MAGNETO_1"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3007", "128", "1", "Position 1", "0", "Position 2", "JBA", "Magneto 2 Switch Starboard", "%1d"));   // "STBD_MAGNETO_2"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3081", "354", "JBA", "Feathering Switch Port", "%1d"));   // "PORT_AIRSCREW"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3082", "355", "JBA", "Feathering Switch Starboard", "%1d"));   // "STBD_AIRSCREW"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3053", "14", 0.1d, 0d, 1d, "JBA", "Flood Light Dimmer Right", false, "%.1f"));   // "FLOOD_L_DIM_R"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3056", "15", 0.1d, 0d, 1d, "JBA", "J.B.B. Flood Light Dimmer", false, "%.1f"));   // "BOX_B_L_DIM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3066", "114", "0", "Position 1", "1", "Position 2", "JBA", "Tropical Air Filter Switch", "%1d"));   // "AIR_FILTER"
            AddFunction(new FlagValue(this, "281", "JBA", "Fuel Pump Light (Yellow)", "True when indicator is lit"));   // "FUEL_PUMP_L"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3065", "282", "1", "Position 1", "0", "Position 2", "JBA", "Fuel Pump Light Cover", "%1d"));   // "FUEL_PUMP_L_CVR"
            AddFunction(new RotaryEncoder(this, devices.REPEATER_COMPASS.ToString("d"), "3001", "49", 0.1d, "Main Panel", "Repeater Compass Course Set", "%.4f"));   // "REP_COMP"
            AddFunction(new RotaryEncoder(this, devices.CONTROLS.ToString("d"), "3010", "72", 0.1d, "Main Panel", "Altimeter Set", "%.4f"));   // "ALT_SET"
            AddFunction(new RotaryEncoder(this, devices.CONTROLS.ToString("d"), "3013", "74", 0.1d, "Main Panel", "Directional Gyro Set", "%.4f"));   // "DI_SET"
            AddFunction(Switch.CreateToggleSwitch(this, devices.PORT_OXYGEN_REGULATOR.ToString("d"), "3003", "84", "1", "Position 1", "0", "Position 2", "Main Panel", "Oxygen Regulator Port", "%1d"));   // "PORT_OXY_VALVE"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3049", "292", "Main Panel", "Boost Cut-Off T-Handle", "%1d"));   // "BOOST_CUT_OFF"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3018", "62", "0", "Position 1", "1", "Position 2", "Main Panel", "Landing Light Switch Port", "%1d"));   // "PORT_LAND_L_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3020", "63", "0", "Position 1", "1", "Position 2", "Main Panel", "Landing Light Switch Starboard", "%1d"));   // "STBD_LAND_L_SW"

            AddFunction(new Rocker(this, devices.THREE_WAY_SELECTOR.ToString("d"), "3001", "115", "Main Panel", "Bomb Doors Lever", true, "1", "-1", "0", "%1d", true));
            AddFunction(new Rocker(this, devices.THREE_WAY_SELECTOR.ToString("d"), "3002", "116", "Main Panel", "Gear (Chassis) Lever", true, "1", "-1", "0", "%1d", true));
            AddFunction(Switch.CreateToggleSwitch(this, devices.THREE_WAY_SELECTOR.ToString("d"), "3004", "117", "0", "Open", "1", "Closed", "Main Panel", "Gear (Chassis) Lever Gate", "%1d"));   // "CHASSIS_GATE"
            AddFunction(new Rocker(this, devices.THREE_WAY_SELECTOR.ToString("d"), "3003", "118", "Main Panel", "Flaps Lever", true, "1", "-1", "0", "%1d", true));
            AddFunction(Switch.CreateToggleSwitch(this, devices.THREE_WAY_SELECTOR.ToString("d"), "3006", "119", "0", "Open", "1", "Closed", "Main Panel", "Flaps Lever Gate", "%1d"));   // "FLAPS_GATE"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3001", "120", "0", "Safe", "1", "Fire", "Main Panel", "Gun Firing Master Switch Cover", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3003", "121", "0", "Position 1", "1", "Position 2", "Main Panel", "Gun Firing Master Switch", "%1d"));   // "GUN_MASTER"
            AddFunction(new PushButton(this, devices.GLYCOL_PUMP.ToString("d"), "3001", "370", "Main Panel", "De-Ice Glycol Pump Handle", "%1d"));   // "DE_ICE_PUMP"

            AddFunction(new Rocker(this, devices.CONTROLS.ToString("d"), "3053", "111", "Main Panel", "Rudder Trim", true, "1", "-1", "0", "%1d", false));
            AddFunction(new Rocker(this, devices.CONTROLS.ToString("d"), "3051", "280", "Main Panel", "Aileron Trim", true, "-1", "1", "0", "%1d", false));
            AddFunction(new Rocker(this, devices.CONTROLS.ToString("d"), "3016", "279", "Port Wall", "Elevator Trim", true, "1", "-1", "0", "%1d", false));

            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3059", "16", 0.1d, 0d, 1d, "Main Panel", "Bomb Panel Flood Dimmer", false, "%.1f"));   // "BOMB_PANEL_L_DIM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3067", "284", "1", "Open", "0", "Closed", "Main Panel", "Bomb Door Light Cover", "%1d"));   // "BOMB_DOOR_L_CVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3056", "144", "1", "Open", "0", "Closed", "Main Panel", "Container Jettison Cover", "%1d"));   // "JETT_CONT_CVR"
            AddFunction(new PushButton(this, devices.WEAPONS.ToString("d"), "3058", "145", "Main Panel", "Container Jettison Button", "%1d"));   // "JETT_CONT"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3059", "311", "0", "Position 1", "1", "Position 2", "Main Panel", "Bomb Panel Cover", "%1d"));   // "BOMB_PANEL_CVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3061", "143", "0", "Position 1", "1", "Position 2", "Main Panel", "Cine Camera Changeover Switch", "%1d"));   // "CINE_CAM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3063", "148", "0", "Position 1", "1", "Position 2", "Main Panel", "Wing Ordnance 1 Switch", "%1d"));   // "WING_ORD_1"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3065", "149", "0", "Position 1", "1", "Position 2", "Main Panel", "Wing Ordnance 2 Switch", "%1d"));   // "WING_ORD_2"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3067", "150", "0", "Position 1", "1", "Position 2", "Main Panel", "Fuselage Bombs 3 Switch", "%1d"));   // "FUSELAGE_BOMB_3"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3069", "151", "0", "Position 1", "1", "Position 2", "Main Panel", "Fuselage Bombs 4 Switch", "%1d"));   // "FUSELAGE_BOMB_4"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3071", "152", "0", "Position 1", "1", "Position 2", "Main Panel", "All Nose Bombs Switch", "%1d"));   // "NOSE_BOMBS"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3073", "153", "0", "Position 1", "1", "Position 2", "Main Panel", "All Tail Bombs Switch", "%1d"));   // "TAIL_BOMBS"
            AddFunction(new Axis(this, devices.FOOTAGE_INDICATOR.ToString("d"), "3003", "90", 0.01d, 0d, 1d, "Main Panel", "Footage Indicator Scale", false, "%.4f"));   // "FOOTAGE_SCALE"
            AddFunction(Switch.CreateToggleSwitch(this, devices.FOOTAGE_INDICATOR.ToString("d"), "3001", "91", "0", "Position 1", "1", "Position 2", "Main Panel", "Footage Indicator Exposure Switch", "%1d"));   // "FOOTAGE_EXPOSURE"

            /// TODO: check device and command
            AddFunction(new Axis(this, devices.DI.ToString("d"), "3001", "259", 0.01d, 0d, 1d, "Floor", "Drift Recorder", false, "%.3f"));   // "DRIFT_RECORDER_GAUGE"

            AddFunction(new FlagValue(this, "283", "Main Panel", "Bomb Door Light (Yellow)", "True when indicator is lit"));   // "BOMB_DOOR_L"
            AddFunction(new FlagValue(this, "271", "Main Panel", "Center Lamp (White)", "True when indicator is lit"));   // "MAIN_PANEL_C_LAMP_L"
            AddFunction(new FlagValue(this, "270", "Main Panel", "Left Lamp (White)", "True when indicator is lit"));   // "MAIN_PANEL_L_LAMP_L"

            AddFunction(Switch.CreateToggleSwitch(this, devices.ROCKETS.ToString("d"), "3003", "387", "0", "Off", "1", "On", "Rockets", "Rockets Salvo Selector Switch (Main Panel)", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, devices.ROCKETS.ToString("d"), "3001", "385", "0", "Off", "1", "On", "Rockets", "Rockets Master Switch (Port Wall)", "%1d"));
            AddFunction(new PushButton(this, devices.ROCKETS.ToString("d"), "3005", "384", "Rockets", "Rockets Manual Advancement (Port Wall)", "%1d"));
            AddFunction(new PushButton(this, devices.ROCKETS.ToString("d"), "3006", "386", "Rockets", "Rockets Firing Switch (Right Engine Throttle)", "%1d"));

            AddFunction(new FlagValue(this, "277", "Main Panel", "Fuel Pressure Warning Light Left (Red)", "True when indicator is lit"));
            AddFunction(new FlagValue(this, "278", "Main Panel", "Fuel Pressure Warning Light Right (Red)", "True when indicator is lit"));
            /// TODO:  find out the correct device and command codes
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3001", "377", "1", "Open", "0", "Closed", "Main Panel", "Fuel Pressure Warning Light Cover Left", "%1d"));   
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3002", "378", "1", "Open", "0", "Closed", "Main Panel", "Fuel Pressure Warning Light Cover Right", "%1d"));

            AddFunction(new PushButton(this, devices.CLOCK.ToString("d"), "3003", "101", "Clock", "Set (Pull)", "%1d"));   // "CLOCK_PIN_PULL"
            AddFunction(new Axis(this, devices.CLOCK.ToString("d"), "3001", "102", 0.01d, 0d, 1d, "Clock", "Set (Turn)", false, "%.4f"));   // "CLOCK_PIN_TURN"
            AddFunction(new Axis(this, devices.CLOCK.ToString("d"), "3005", "375", 0.01d, 0d, 1d, "Clock", "Reference Hours Knob", true, "%.4f"));   // "CLOCK_REF_H"
            AddFunction(new Axis(this, devices.CLOCK.ToString("d"), "3007", "376", 0.01d, 0d, 1d, "Clock", "Reference Minutes Knob", true, "%.4f"));   // "CLOCK_REF_M"
            AddFunction(new ScaledNetworkValue(this, "98", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 360d), "Clock", "Hours", "", "Value between 0 and 360", BindingValueUnits.Degrees, "%.4f", true));   // "CLOCK_H_G"
            AddFunction(new ScaledNetworkValue(this, "99", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 360d), "Clock", "Minutes", "", "Value between 0 and 360", BindingValueUnits.Degrees, "%.4f", true));   // "CLOCK_M_G"
            AddFunction(new ScaledNetworkValue(this, "100", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 360d), "Clock", "Seconds", "", "Value between 0 and 360", BindingValueUnits.Degrees, "%.4f", true));   // "CLOCK_S_G"
            AddFunction(new RotaryEncoder(this, devices.CONTROLS.ToString("d"), "3007", "28", 0.0333d, "Port Wall", "Magnetic Compass Ring", "%.4f"));   // "COMPASS_RING"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3006", "10", 0.1d, 0d, 1d, "Port Wall", "Emergency Light Rheostat", false, "%.1f"));   // "EMERG_L_DIM"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3012", "11", 0.1d, 0d, 1d, "Port Wall", "Compass Flood Light Dimmer", false, "%.1f"));   // "COMPASS_L_DIM"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3015", "12", 0.1d, 0d, 1d, "Port Wall", "Flood Light Dimmer Left", false, "%.1f"));   // "L_SIDE_L_DIM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3008", "8", "1", "Position 1", "0", "Position 2", "Port Wall", "Wing Tank Jettison Cover", "%1d"));   // "JETT_W_TANK_CVR"
            AddFunction(new PushButton(this, devices.WEAPONS.ToString("d"), "3010", "9", "Port Wall", "Wing Tank Jettison Button", "%1d"));   // "JETT_W_TANK"
            AddFunction(new PushButton(this, devices.SWITCHBOARD.ToString("d"), "3024", "294", "Port Wall", "UV Exciter Button", "%1d"));   // "UV_EXCITER"
            AddFunction(new Axis(this, devices.LH_5C1878.ToString("d"), "3001", "295", 0.1d, 0d, 1d, "Port Wall", "UV Lamp Cap Left", false, "%.1f"));   // "LH_UV_CAP"
            AddFunction(new Axis(this, devices.RH_5C1878.ToString("d"), "3001", "296", 0.1d, 0d, 1d, "Port Wall", "UV Lamp Cap Right", false, "%.1f"));   // "RH_UV_CAP"
            AddFunction(new Axis(this, devices.SBA.ToString("d"), "3003", "7", 0.1d, 0d, 1d, "Port Wall", "Beam Approach Volume Rheostat", false, "%.1f"));   // "BA_VOLUME"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3001", "1", "0", "Position 1", "1", "Position 2", "Port Wall", "R.I. Compass Switch", "%1d"));   // "REP_COMPASS_SW1"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3002", "2", "0", "Position 1", "1", "Position 2", "Port Wall", "R.I. Compass De-ground Switch", "%1d"));   // "REP_COMPASS_SW2"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3004", "3", "0", "Position 1", "1", "Position 2", "Port Wall", "Beam Approach Switch", "%1d"));   // "BEAM_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.VHF_RADIO.ToString("d"), "3099", "4", "1", "Position 1", "0", "Position 2", "Port Wall", "SCR-522 PTT Button", "%1d"));   // "SCR_PTT"
            AddFunction(new FlagValue(this, "325", "Port Wall", "UV Light Left (multi)", "True when indicator is lit"));   // "UV_L_L"
            AddFunction(new FlagValue(this, "326", "Port Wall", "UV Light Right (multi)", "True when indicator is lit"));   // "UV_R_L"
            AddFunction(new PushButton(this, devices.VHF_RADIO.ToString("d"), "3001", "32", "Radio Remote Channel Switcher", "OFF Button", "%1d"));   // "RADIO_OFF"
            AddFunction(new PushButton(this, devices.VHF_RADIO.ToString("d"), "3002", "33", "Radio Remote Channel Switcher", "A Button", "%1d"));   // "RADIO_A"
            AddFunction(new PushButton(this, devices.VHF_RADIO.ToString("d"), "3003", "34", "Radio Remote Channel Switcher", "B Button", "%1d"));   // "RADIO_B"
            AddFunction(new PushButton(this, devices.VHF_RADIO.ToString("d"), "3004", "35", "Radio Remote Channel Switcher", "C Button", "%1d"));   // "RADIO_C"
            AddFunction(new PushButton(this, devices.VHF_RADIO.ToString("d"), "3005", "36", "Radio Remote Channel Switcher", "D Button", "%1d"));   // "RADIO_D"
            AddFunction(Switch.CreateToggleSwitch(this, devices.VHF_RADIO.ToString("d"), "3006", "42", "1", "Position 1", "0", "Position 2", "Radio Remote Channel Switcher", "Dimmer Switch", "%1d"));   // "RADIO_L_DIM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.VHF_RADIO.ToString("d"), "3017", "43", "0", "Position 1", "1", "Position 2", "Radio Remote Channel Switcher", "Transmission Lock", "%1d"));   // "RADIO_TLOCK"
            AddFunction(new Switch(this, devices.VHF_RADIO.ToString("d"), "44", new SwitchPosition[] { new SwitchPosition("1.0", "Position 1", "3008"), new SwitchPosition("0.0", "Position 2", "3008"), new SwitchPosition("-1.0", "Position 3", "3007") }, "Radio Remote Channel Switcher", "Mode Switch", "%.1f"));   // "RADIO_T_MODE"
            AddFunction(new Axis(this, devices.VHF_RADIO.ToString("d"), "3015", "364", 0.1d, 0d, 1d, "Radio Remote Channel Switcher", "Volume Knob", false, "%.1f"));   // "RADIO_VOL"
            AddFunction(new FlagValue(this, "37", "Radio Remote Channel Switcher", "A Light (White)", "True when indicator is lit"));   // "RADIO_A_L"
            AddFunction(new FlagValue(this, "38", "Radio Remote Channel Switcher", "B Light (White)", "True when indicator is lit"));   // "RADIO_B_L"
            AddFunction(new FlagValue(this, "39", "Radio Remote Channel Switcher", "C Light (White)", "True when indicator is lit"));   // "RADIO_C_L"
            AddFunction(new FlagValue(this, "40", "Radio Remote Channel Switcher", "D Light (White)", "True when indicator is lit"));   // "RADIO_D_L"
            AddFunction(new FlagValue(this, "41", "Radio Remote Channel Switcher", "TX Light (White)", "True when indicator is lit"));   // "RADIO_TX_L"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3030", "25", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttle Friction", false, "%.1f"));   // "THROTTLE_FRICTION"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3020", "288", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttles Control Left", false, "%.1f"));   // "THROTTLE_CONTROL_L"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3023", "289", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttles Control Right", false, "%.1f"));   // "THROTTLE_CONTROL_R"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3016", "18", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttles Transit Left", false, "%.1f"));   // "THROTTLE_TRANSIT_L"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3017", "19", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttles Transit Right", false, "%.1f"));   // "THROTTLE_TRANSIT_R"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3014", "20", "1", "Position 1", "0", "Position 2", "Throttle Quadrant", "Throttles Control Left Trigger", "%1d"));   // "THROTTLE_CONTROL_L_TRIG"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3015", "21", "1", "Position 1", "0", "Position 2", "Throttle Quadrant", "Throttles Control Right Trigger", "%1d"));   // "THROTTLE_CONTROL_R_TRIG"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3033", "24", 0.1d, 0d, 1d, "Throttle Quadrant", "Propeller Friction", false, "%.1f"));   // "PROP_FRICTION"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3036", "22", 0.1d, 0d, 1d, "Throttle Quadrant", "Propeller Control Left", false, "%.1f"));   // "PROP_CONTROL_L"
            AddFunction(new Axis(this, devices.ENGINE_CONTROLS.ToString("d"), "3039", "23", 0.1d, 0d, 1d, "Throttle Quadrant", "Propeller Control Right", false, "%.1f"));   // "PROP_CONTROL_R"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3086", "27", "1", "Position 1", "0", "Position 2", "Throttle Quadrant", "Mixture Control", "%1d"));   // "MIXTURE"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3022", "26", "0", "Position 1", "1", "Position 2", "Throttle Quadrant", "Supercharger Gear Change Switch", "%1d"));   // "SUPERCHARGER"
            AddFunction(new PushButton(this, devices.CONTROLS.ToString("d"), "3072", "254", "Starboard Wall", "Door Handle", "%1d"));   // "DOOR_LOCK"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3073", "324", "1", "Position 1", "0", "Position 2", "Starboard Wall", "Door Jettison Handle", "%1d"));   // "DOOR_JETT"
            AddFunction(new Switch(this, devices.SWITCHBOARD.ToString("d"), "160", SwitchPositions.Create(3, 0, 0.5, "3075", "Position ", "%.2f"), "Starboard Wall", "Down Ident Lights Switch", "%.2f"));   // "DN_IDENT_L_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3078", "161", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Camera Gun Switch", "%1d"));   // "CAM_GUN_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3080", "162", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Nav Lights Switch", "%1d"));   // "NAV_L_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3082", "163", "0", "Position 1", "1", "Position 2", "Starboard Wall", "UV Lighting Switch", "%1d"));   // "UV_L_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3084", "164", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Pitot Head Switch", "%1d"));   // "PITOT_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3086", "165", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Fuel Pump Switch", "%1d"));   // "FUEL_PUMP_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3051", "166", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Reflector Sight Switch", "%1d"));   // "REFLECTOR_SIGHT_SW"
            AddFunction(new Switch(this, devices.SWITCHBOARD.ToString("d"), "167", SwitchPositions.Create(3, 0, 0.5, "3048", "Position ", "%.2f"), "Starboard Wall", "Nav Head Lamp Switch", "%.2f"));   // "NAV_HEAD_L_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3088", "168", "0", "Position 1", "1", "Position 2", "Starboard Wall", "IFF Switch", "%1d"));   // "IFF_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3036", "169", "0", "Position 1", "1", "Position 2", "Starboard Wall", "IFF Detonator Cover", "%1d"));   // "IFF_DESRUCT_CVR"
            AddFunction(new PushButton(this, devices.SWITCHBOARD.ToString("d"), "3038", "170", "Starboard Wall", "IFF Detonator Left Switch", "%1d"));   // "IFF_DESRUCT_L_BTN"
            AddFunction(new PushButton(this, devices.SWITCHBOARD.ToString("d"), "3039", "171", "Starboard Wall", "IFF Detonator Right Switch", "%1d"));   // "IFF_DESRUCT_R_BTN"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3040", "172", "1", "Position 1", "0", "Position 2", "Starboard Wall", "Extinguisher Switch Cover Port", "%1d"));   // "PORT_EXTG_CVR"
            AddFunction(new PushButton(this, devices.SWITCHBOARD.ToString("d"), "3042", "173", "Starboard Wall", "Extinguisher Switch Port", "%1d"));   // "PORT_EXTG_BTN"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3043", "174", "1", "Position 1", "0", "Position 2", "Starboard Wall", "Extinguisher Switch Cover Starboard", "%1d"));   // "STBD_EXTG_CVR"
            AddFunction(new PushButton(this, devices.SWITCHBOARD.ToString("d"), "3045", "175", "Starboard Wall", "Extinguisher Switch Starboard", "%1d"));   // "STBD_EXTG_BTN"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3033", "176", 0.1d, 0d, 1d, "Starboard Wall", "Windscreen Wiper Rheostat", false, "%.1f"));   // "WIPER_RT"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3046", "178", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Resin Lights Switch", "%1d"));   // "RESIN_L_SW"
            AddFunction(new PushButton(this, devices.SWITCHBOARD.ToString("d"), "3068", "157", "Starboard Wall", "Morse Key", "%1d"));   // "MORSE_KEY"
            AddFunction(new Switch(this, devices.SWITCHBOARD.ToString("d"), "158", SwitchPositions.Create(3, 0, 0.5, "3069", "Position ", "%.2f"), "Starboard Wall", "Downward Lamp Mode Selector", "%.2f"));   // "DOWNWARD_LVR"
            AddFunction(new Switch(this, devices.SWITCHBOARD.ToString("d"), "159", SwitchPositions.Create(3, 0, 0.5, "3072", "Position ", "%.2f"), "Starboard Wall", "Upward Lamp Mode Selector", "%.2f"));   // "UPWARD_LVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3055", "293", "1", "Position 1", "0", "Position 2", "Starboard Wall", "Oxygen High Pressure Valve", "%1d"));   // "OXY_H_PRESS_VALVE"
            AddFunction(Switch.CreateToggleSwitch(this, devices.STBD_OXYGEN_REGULATOR.ToString("d"), "3003", "187", "1", "Position 1", "0", "Position 2", "Starboard Wall", "Oxygen Regulator Starboard", "%1d"));   // "STBD_OXY_VALVE"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3095", "303", 0.1d, 0d, 1d, "Port Wall", "Chart Table Flood Light Dimmer Type D", false, "%.1f"));   // "CHART_L_DIM"
            AddFunction(Switch.CreateToggleSwitch(this, devices.AERIAL_WINCH.ToString("d"), "3001", "202", "1", "Position 1", "0", "Position 2", "Starboard Wall", "Aerial Winch  Brake Lever", "%1d"));   // "AERIAL_BRAKE"
            AddFunction(new Rocker(this, devices.AERIAL_WINCH.ToString("d"), "3004", "356", "Starboard Wall", "Aerial Winch Rotary Handle", false, "-1", "1", "0", "%1d", false));
            AddFunction(new PushButton(this, devices.AERIAL_WINCH.ToString("d"), "3005", "357", "Starboard Wall", "Aerial Winch Reel Lock", "%1d"));   // "AERIAL_REEL"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3113", "307", "0", "Position 1", "1", "Position 2", "Starboard Wall", "Transmitter Type F Switch", "%1d"));   // "TRANS_TYPF_SW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_POWERUNIT.ToString("d"), "3001", "305", "0", "Position 1", "1", "Position 2", "Starboard Wall", "T.1154 R.1155 L.T. Power Unit Switch", "%1d"));   // "LT_T1154_PW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_POWERUNIT.ToString("d"), "3003", "306", "0", "Position 1", "1", "Position 2", "Starboard Wall", "T.1154 R.1155 H.T. Power Unit Switch", "%1d"));   // "HT_T1154_PW"
            AddFunction(new FlagValue(this, "177", "Starboard Wall", "Generator Warning Light (Red)", "True when indicator is lit"));   // "GEN_WARN_L"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3071", "192", "1", "Position 1", "0", "Position 2", "Port Wall", "Engine Cut-Out Handle Port", "%1d"));   // "PORT_CUT-OUT"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3095", "191", "1", "Position 1", "0", "Position 2", "Port Wall", "Engine Cut-Out Handle Starboard", "%1d"));   // "STBD_CUT-OUT"
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3073", "193", "1", "Position 1", "0", "Position 2", "-1", "Position 3", "Port Wall", "Fuel Cock Port", "%1d"));   // "PORT_FUEL_COCK"
            AddFunction(Switch.CreateThreeWaySwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3076", "194", "1", "Position 1", "0", "Position 2", "-1", "Position 3", "Port Wall", "Fuel Cock Starboard", "%1d"));   // "STBD_FUEL_COCK"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3079", "195", "1", "Position 1", "0", "Position 2", "Port Wall", "Transfer Fuel Cock", "%1d"));   // "TRANSFER_COCK-OUT"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3050", "197", "Port Wall", "Oil Dilution Switch Port", "%1d"));   // "PORT_OIL_DILUTION"
            AddFunction(new PushButton(this, devices.ENGINE_CONTROLS.ToString("d"), "3051", "198", "Port Wall", "Oil Dilution Switch Starboard", "%1d"));   // "STBD_OIL_DILUTION"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3089", "196", "1", "Position 1", "0", "Position 2", "Port Wall", "Tank Pressurizing Lever", "%1d"));   // "TANK_PRESS"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3091", "200", "1", "Position 1", "0", "Position 2", "Port Wall", "Auxiliary Oil Supply Lever Port", "%1d"));   // "PORT_OIL_SUPPLY"
            AddFunction(Switch.CreateToggleSwitch(this, devices.ENGINE_CONTROLS.ToString("d"), "3093", "199", "1", "Position 1", "0", "Position 2", "Port Wall", "Auxiliary Oil Supply Lever Starboard", "%1d"));   // "STBD_OIL_SUPPLY"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3057", "203", "1", "Position 1", "0", "Position 2", "Port Wall", "Cabin Heater Control", "%1d"));   // "CABIN_HEATER_LVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3059", "204", "1", "Position 1", "0", "Position 2", "Port Wall", "Gun Heater Control", "%1d"));   // "GUN_HEATER_LVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3020", "256", "1", "Position 1", "0", "Position 2", "Port Wall", "Arm Rest Port", "%1d"));   // "PORT_ARMREST"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3061", "257", "1", "Position 1", "0", "Position 2", "Port Wall", "Arm Rest Starboard", "%1d"));   // "STBD_ARMREST"
            AddFunction(Switch.CreateToggleSwitch(this, devices.WEAPONS.ToString("d"), "3043", "323", "1", "Position 1", "0", "Position 2", "Port Wall", "Harness Release Lever", "%1d"));   // "HARNESS_LVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3069", "333", "1", "Position 1", "0", "Position 2", "Port Wall", "Hydraulic Hand Pump Emergency Selector", "%1d"));   // "HYDR_SEL"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3101", "302", 0.1d, 0d, 1d, "Quarter", "Roof Dome Light Dimmer", false, "%.1f"));   // "DOME_L_DIM"
            AddFunction(new Axis(this, devices.SWITCHBOARD.ToString("d"), "3092", "17", 0.1d, 0d, 1d, "Quarter", "Loop Antenna Flood Light Dimmer", false, "%.1f"));   // "LOOP_ANT_L_DIM"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "224", SwitchPositions.Create(5, -1, 0.5, "3059", "Position ", "%.2f"), "Quarter", "Aerial Selector Switch Type J", "%.2f"));   // "TYPE_J_SEL"
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_INTERFACE.ToString("d"), "3107", "241", "1", "Position 1", "0", "Position 2", "Quarter", "Loop Antenna Lock", "%1d"));   // "LOOP_ANT_LOCK"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3094", "240", 0.01d, 0d, 1d, "Quarter", "Loop Antenna Scale", false, "%.4f"));   // "LOOP_ANT_SCALE"
            AddFunction(new Switch(this, devices.SWITCHBOARD.ToString("d"), "188", SwitchPositions.Create(6, 0, 0.2, "3104", "Position ", "%.2f"), "Quarter", "IFF Channel Switch", "%.2f"));   // "IFF_CHAN"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3107", "363", "1", "Position 1", "0", "Position 2", "Quarter", "IFF Detonator Switch Cover", "%1d"));   // "IFF_DETONATE_CVR"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3109", "189", "0", "Position 1", "1", "Position 2", "Quarter", "IFF Detonator Switch", "%1d"));   // "IFF_DETONATE"
            AddFunction(Switch.CreateToggleSwitch(this, devices.SWITCHBOARD.ToString("d"), "3111", "190", "0", "Position 1", "1", "Position 2", "Quarter", "IFF Power Switch", "%1d"));   // "IFF_PW"
            AddFunction(new FlagValue(this, "276", "Quarter", "Dome Lamp (White)", "True when indicator is lit"));   // "DOME_LAMP_L"
            AddFunction(new FlagValue(this, "275", "Quarter", "Antenna Lamp (White)", "True when indicator is lit"));   // "LOOP_ANT_LAMP_L"
            AddFunction(new PushButton(this, devices.RADIO_INTERFACE.ToString("d"), "3001", "365", "T.1154", "Key", "%1d"));   // "T1154_KEY"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3002", "358", 0.01d, 0d, 1d, "T.1154", "Master Oscillator Tuning Condenser C2", false, "%.4f"));   // "T1154_C2"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3005", "359", 0.01d, 0d, 1d, "T.1154", "Master Oscillator Tuning Condenser C4", false, "%.4f"));   // "T1154_C4"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3008", "360", 0.01d, 0d, 1d, "T.1154", "Output Tuning Condenser C15", false, "%.4f"));   // "T1154_C15"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3011", "361", 0.01d, 0d, 1d, "T.1154", "Output Tuning Condenser C16", false, "%.4f"));   // "T1154_C16"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3014", "362", 0.01d, 0d, 1d, "T.1154", "Master Oscillator Tuning Condenser C17", false, "%.4f"));   // "T1154_C17"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "212", SwitchPositions.Create(8, 0, 0.1, "3017", "Position ", "%.2f"), "T.1154", "Condenser C2 Presets Knob", "%.2f"));   // "T1154_C2_B"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "211", SwitchPositions.Create(8, 0, 0.1, "3020", "Position ", "%.2f"), "T.1154", "Condenser C4 Presets Knob", "%.2f"));   // "T1154_C4_B"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "213", SwitchPositions.Create(8, 0, 0.1, "3023", "Position ", "%.2f"), "T.1154", "Condenser C15 Presets Knob", "%.2f"));   // "T1154_C15_B"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "214", SwitchPositions.Create(8, 0, 0.1, "3026", "Position ", "%.2f"), "T.1154", "Condenser C16 Presets Knob", "%.2f"));   // "T1154_C16_B"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "210", SwitchPositions.Create(8, 0, 0.1, "3029", "Position ", "%.2f"), "T.1154", "Condenser C17 Presets Knob", "%.2f"));   // "T1154_C17_B"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3032", "223", 0.1d, -1d, 1d, "T.1154", "Master Oscillator C2 Vernier", false, "%.1f"));   // "T1154_C2_V"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3035", "222", 0.1d, -1d, 1d, "T.1154", "Master Oscillator C4 Vernier", false, "%.1f"));   // "T1154_C4_V"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "215", SwitchPositions.Create(3, 0.0, 0.1, "3038", "Position ", "%.2f"), "T.1154", "Frequency Range Switch S1 S2", "%.2f"));   // "T.1154"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "216", SwitchPositions.Create(9, 0.0, 0.1, "3041", "Position ", "%.2f"), "T.1154", "Inductance Tapping Switch S3", "%.2f"));   // "T.1154"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "217", SwitchPositions.Create(9, 0.0, 0.1, "3044", "Position ", "%.2f"), "T.1154", "Inductance Tapping Switch S4", "%.2f"));   // "T.1154"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "218", SwitchPositions.Create(6, 0.0, 0.1, "3047", "Position ", "%.2f"), "T.1154", "Master Switch S5", "%.2f"));   // "T.1154"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "219", SwitchPositions.Create(17, 0.0, 0.05, "3050", "Position ", "%.2f"), "T.1154", "Inductance Tapping Switch S6", "%.2f"));   // "T.1154"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "220", SwitchPositions.Create(17, 0.0, 0.05, "3053", "Position ", "%.2f"), "T.1154", "Inductance Tapping Switch S7", "%.2f"));   // "T.1154"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3056", "221", 0.1d, 0d, 1d, "T.1154", "Output Inductance Control L6", false, "%.1f"));   // "T1154_L6"

            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "238", SwitchPositions.Create(5, 0.0, 0.1, "3062", "Position ", "%.2f"), "R.1155", "Master Selector Switch", "%.2f"));   // "R.1155"
            AddFunction(new Switch(this, devices.RADIO_INTERFACE.ToString("d"), "231", SwitchPositions.Create(5, 0.0, 0.1, "3065", "Position ", "%.2f"), "R.1155", "Frequency Range Switch", "%.2f"));   // "R.1155"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3068", "229", 0.1d, 0d, 1d, "R.1155", "Volume Knob", false, "%.1f"));   // "R1155_VOL"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3071", "233", 0.1d, 0d, 1d, "R.1155", "Tuning (Fine)", false, "%.1f"));   // "R1155_RANGE_HIGH"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3074", "234", 0.1d, 0d, 1d, "R.1155", "Tuning (Coarse)", false, "%.1f"));   // "R1155_RANGE_LOW"
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_INTERFACE.ToString("d"), "3077", "230", "0", "Position 1", "1", "Position 2", "R.1155", "Heterodyne Switch", "%1d"));   // "R1155_HETI"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3079", "225", 0.1d, -1d, 1d, "R.1155", "Meter Balance Knob", false, "%.1f"));   // "R1155_METER_BAL"
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_INTERFACE.ToString("d"), "3082", "226", "0", "Position 1", "1", "Position 2", "R.1155", "Filter Switch", "%1d"));   // "R1155_FILTER"
            AddFunction(new Axis(this, devices.RADIO_INTERFACE.ToString("d"), "3084", "227", 0.1d, 0d, 1d, "R.1155", "Meter Amplitude Knob", false, "%.1f"));   // "R1155_METER_AMP"
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_INTERFACE.ToString("d"), "3087", "235", "0", "Position 1", "1", "Position 2", "R.1155", "Meter Deflection Sensitivity Switch", "%1d"));   // "R1155_METER_DEF"
            AddFunction(new Rocker(this, devices.AERIAL_WINCH.ToString("d"), "3089", "236", "R.1155", "Aural Sense Switch", false, "1", "-1", "0", "%1d", false));
            AddFunction(Switch.CreateToggleSwitch(this, devices.RADIO_INTERFACE.ToString("d"), "3090", "237", "0", "Position 1", "1", "Position 2", "R.1155", "Meter Frequency Switch", "%1d"));   // "R1155_SW_SPEED"

            AddFunction(new NetworkValue(this, "255", "Cockpit", "Cockpit Hatch Position", "", "Numeric Value between 0 and 1", BindingValueUnits.Numeric));   // "COCKPIT_HATCH"
            AddFunction(new FlagValue(this, "308", "Cockpit", "Transmitter Type F (Yellow)", "True when indicator is lit"));   // "TRANS_TYPF_L"
            AddFunction(new FlagValue(this, "379", "Cockpit", "Left Red Lamp (Red)", "True when indicator is lit"));   // "RED_LAMP_L_L"
            AddFunction(new FlagValue(this, "301", "Cockpit", "Right Lamp (White)", "True when indicator is lit"));   // "WH_LAMP_R_L"
            AddFunction(new FlagValue(this, "300", "Cockpit", "Front Lamp (White)", "True when indicator is lit"));   // "WH_LAMP_F_L"
            AddFunction(new FlagValue(this, "299", "Cockpit", "Left Lamp (White)", "True when indicator is lit"));   // "WH_LAMP_L_L"
            AddFunction(new FlagValue(this, "297", "Cockpit", "Gauges Glow (Green)", "True when indicator is lit"));   // "GAUGES_GLOW_L"
            AddFunction(new FlagValue(this, "77", "Main Panel", "Gear Left UP (Red)", "True when indicator is lit"));   // "GEAR_UP_L_L"
            AddFunction(new FlagValue(this, "79", "Main Panel", "Gear Right UP (Red)", "True when indicator is lit"));   // "GEAR_UP_R_L"
            AddFunction(new FlagValue(this, "78", "Main Panel", "Gear Left DOWN (Green)", "True when indicator is lit"));   // "GEAR_DN_L_L"
            AddFunction(new FlagValue(this, "80", "Main Panel", "Gear Right DOWN (Green)", "True when indicator is lit"));   // "GEAR_DN_R_L"
            AddFunction(Switch.CreateToggleSwitch(this, devices.CONTROLS.ToString("d"), "3018", "304", "1", "Position 1", "0", "Position 2", "Main Panel", "Gear Indicator Blind Up/Down", "%1d"));   // "UC_BLIND"


            AddFunction(new ScaledNetworkValue(this, "242", new CalibrationPointCollectionDouble(-1.0d, -100d, 1.0d, 100.0d), "Flight Controls", "Stick Pitch", "Percentage of Travel", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "243", new CalibrationPointCollectionDouble(-1.0d, -100d, 1.0d, 100.0d), "Flight Controls", "Stick Bank", "Percentage of Travel", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "249", new CalibrationPointCollectionDouble(-1.0d, -100d, 1.0d, 100.0d), "Flight Controls", "Rudder Yaw", "Percentage of Travel", BindingValueUnits.Numeric, "%.4f", true));

            #region Flight Instruments
            // AN5730 Repeater Compass
            AddFunction(new ScaledNetworkValue(this, "47", new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d), "Flight Instruments", "Repeater Compass Heading", "Degrees", BindingValueUnits.Degrees, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "48", new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d), "Flight Instruments", "Repeater Compass Course", "Degrees", BindingValueUnits.Degrees, "%.4f", true));

            AddFunction(new ScaledNetworkValue(this, "68", new CalibrationPointCollectionDouble(0d, 0d, 1d, 10d), "Flight Instruments", "Altimeter 100's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "69", new CalibrationPointCollectionDouble(0d, 0d, 1d, 10d), "Flight Instruments", "Altimeter 1000's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "70", new CalibrationPointCollectionDouble(0d, 0d, 1d, 10d), "Flight Instruments", "Altimeter 10000's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "71", new CalibrationPointCollectionDouble(0d, 800d, 1d, 1050d), "Flight Instruments", "Air Pressure", "milliBar", BindingValueUnits.Millibar, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "67", new CalibrationPointCollectionDouble(-1d, -4000d, 1d, 4000d), "Flight Instruments", "Vertical Velocity", "Feet per Minute", BindingValueUnits.FeetPerMinute, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "64", new CalibrationPointCollectionDouble(0d, 0d, 0.5d, 500d), "Flight Instruments", "Airspeed", "Knots", BindingValueUnits.Knots, "%.4f", true));

            AddFunction(new ScaledNetworkValue(this, "65", new CalibrationPointCollectionDouble(-1d, -180d, 1d, 180d), "Flight Instruments", "Artificial Horizon Bank", "Degrees", BindingValueUnits.Degrees, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "66", new CalibrationPointCollectionDouble(-1d, -45d, 1d, 45d), "Flight Instruments", "Artificial Horizon Pitch", "Degrees", BindingValueUnits.Degrees, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "73", new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d), "Flight Instruments", "Direction Indicator", "Degrees", BindingValueUnits.Degrees, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "75", new CalibrationPointCollectionDouble(-1d, -20d, 1d, 20d), "Flight Instruments", "Side Slip", "Degrees", BindingValueUnits.Degrees, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "76", new CalibrationPointCollectionDouble(-1d, -40d, 1d, 40d), "Flight Instruments", "Turn", "Degrees", BindingValueUnits.Degrees, "%.4f", true));

            #endregion Flight Instruments
            #region Engine Instruments
            AddFunction(new ScaledNetworkValue(this, "50", new CalibrationPointCollectionDouble(0d, 0d, 1d, 5000d), "Engine Instruments", "Tacho (Port) 1000's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "51", new CalibrationPointCollectionDouble(0d, 0d, 1d, 1000d), "Engine Instruments", "Tacho (Port) 100's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "52", new CalibrationPointCollectionDouble(0d, 0d, 1d, 5000d), "Engine Instruments", "Tacho (Starboard) 1000's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "53", new CalibrationPointCollectionDouble(0d, 0d, 1d, 1000d), "Engine Instruments", "Tacho (Starboard) 100's", "Number", BindingValueUnits.Numeric, "%.4f", true));
            {
                double[] output = new double[] { -7.0, -6.0, 0.0, 6.0, 10.0, 14.0, 16.0, 18.0, 20.0, 22.0, 24.0, 25.0 };
                double[] input = new double[] { 0.000, 0.035, 0.280, 0.503, 0.629, 0.723, 0.778, 0.834, 0.887, 0.929, 0.980, 1.000 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "54", scale, "Engine Instruments", "Boost Gauge (Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "55", scale, "Engine Instruments", "Boost Gauge (Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }


            AddFunction(new ScaledNetworkValue(this, "56", new CalibrationPointCollectionDouble(-1d, -1d, 1d, 120d), "Engine Instruments", "Oil Temperature (Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "57", new CalibrationPointCollectionDouble(-1d, -1d, 1d, 120d), "Engine Instruments", "Oil Temperature (Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "58", new CalibrationPointCollectionDouble(0d, 0d, 1d, 150d), "Engine Instruments", "Oil Pressure (Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "59", new CalibrationPointCollectionDouble(0d, 0d, 1d, 150d), "Engine Instruments", "Oil Pressure (Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            {
                double[] output = new double[] { 39.0, 40.0, 60   , 80   , 90   , 100  , 110  , 120      , 140.0 };
                double[] input = new double[] { -1.0 , 0.0 , 0.08 , 0.2  , 0.29 , 0.39 , 0.5  , 0.64     , 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "60", scale, "Engine Instruments", "Radiator Temperature (Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "61", scale, "Engine Instruments", "Radiator Temperature (Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            {
                double[] output = new double[] { -1.0, 0.0, 20.0, 60.0, 100.0, 120.0, 146.0, 160.0 };
                double[] input = new double[] { -1.0, 0.0, 0.197, 0.485, 0.741, 0.867, 0.940, 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "92", scale, "Fuel Instruments", "Fuel Gauge (Inner Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "93", scale, "Fuel Instruments", "Fuel Gauge (Inner Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            {
                double[] output = new double[] { -1.0, 0.0, 10.0, 20.0, 30.0, 40.0, 59.0, 70.0 };
                double[] input = new double[] { -1.0, 0.0, 0.197, 0.485, 0.741, 0.867, 0.940, 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "96", scale, "Fuel Instruments", "Fuel Gauge (Outer Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "97", scale, "Fuel Instruments", "Fuel Gauge (Outer Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            {
                double[] output = new double[] { -1.0, 0.0, 10.0, 20.0, 30.0, 40.0, 53.0, 60.0 };
                double[] input = new double[] { -1.0, 0.0, 0.123, 0.306, 0.505, 0.686, 0.887, 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "94", scale, "Fuel Instruments", "Fuel Gauge (Center)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            {
                double[] output = new double[] { -1.0, 0.0, 5.0, 10.0, 20.0, 30.0, 50.0, 70.0 };
                double[] input = new double[] { -1.0, 0.0, 0.08, 0.183, 0.37, 0.524, 0.815, 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "95", scale, "Fuel Instruments", "Fuel Gauge (Long Range)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }

            #endregion Engine Instruments
            #region Systematic Instruments
            AddFunction(new ScaledNetworkValue(this, "82", new CalibrationPointCollectionDouble(0d, 0d, 1d, 10000d), "Systematic Instruments", "Oxygen Delivery (Port)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "155", new CalibrationPointCollectionDouble(0d, 0d, 1d, 10000d), "Systematic Instruments", "Oxygen Delivery (Starboard)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "83", new CalibrationPointCollectionDouble(0d, 0d, 1d, 1d), "Systematic Instruments", "Oxygen Supply (Port)", "Number", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "156", new CalibrationPointCollectionDouble(0d, 0d, 1d, 1d), "Systematic Instruments", "Oxygen Supply (Starboard)", "Number", BindingValueUnits.Numeric, "%.4f"));

            AddFunction(new ScaledNetworkValue(this, "81", new CalibrationPointCollectionDouble(0d, 0d, 0.7d, 70d), "Systematic Instruments", "Flaps", "Degrees", BindingValueUnits.Degrees, "%.4f", true));

            AddFunction(new ScaledNetworkValue(this, "85", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 220d), "Systematic Instruments", "Pneumatic Pressure", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "86", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 130d), "Systematic Instruments", "Wheel Brake Gauge (Left)", "Number", BindingValueUnits.Numeric, "%.4f", true));
            AddFunction(new ScaledNetworkValue(this, "87", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 130d), "Systematic Instruments", "Wheel Brake Gauge (Right)", "Number", BindingValueUnits.Numeric, "%.4f", true));

            {
                double[] output = new double[] { -31, -30.0, 70.0 };
                double[] input = new double[] { -1.0, 0.0, 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "314", scale, "Systematic Instruments", "Air Temperature", "Celsius", BindingValueUnits.Celsius, "%.4f", true));
            }
            AddFunction(new ScaledNetworkValue(this, "103", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 40d), "Systematic Instruments", "Volt Meter", "Number", BindingValueUnits.Numeric, "%.4f", true));

            #endregion Systematic Instruments
            #region Radio Equipment
            {
                double[] output = new double[] { 5.2, 5.3, 5.6, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.4 };
                double[] input = new double[] { 0.000, 0.039, 0.127, 0.218, 0.321, 0.416, 0.502, 0.578, 0.655, 0.734, 0.801, 0.867, 0.937 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "207", scale, "T.1154", "C2", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "208", scale, "T.1154", "C15", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            {
                double[] output = new double[] { 2.9, 3.0, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 4.0, 4.5, 5.0, 5.5, 5.7 };
                double[] input = new double[] { 0.000, 0.057, 0.232, 0.263, 0.299, 0.334, 0.365, 0.400, 0.434, 0.588, 0.736, 0.859, 0.912 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "206", scale, "T.1154", "C4", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "209", scale, "T.1154", "C16", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            {
                double[] output = new double[] { 180, 200, 220, 250, 300, 350, 380, 420, 460, 500, 520 };
                double[] input = new double[] { 0.000, 0.137, 0.241, 0.361, 0.522, 0.643, 0.706, 0.769, 0.827, 0.879, 0.912 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "205", scale, "T.1154", "C17", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }
            AddFunction(new ScaledNetworkValue(this, "104", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 0.3d), "T.1154", "M1", "Number", BindingValueUnits.Numeric, "%.4f", true));
            {
                double[] output = new double[] { 0.000, 0.050, 0.100, 0.150, 0.200, 0.250, 0.300, 0.350 };
                double[] input = new double[] { 0.000, 0.016, 0.074, 0.165, 0.314, 0.505, 0.729, 1.000 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "105", scale, "T.1154", "M2", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }

            {
                double[] output = new double[] { 0.000, 0.050, 0.100, 0.150, 0.200, 0.250, 0.300, 0.350, 0.400 };
                double[] input = new double[] { 0.000, 0.032, 0.069, 0.149, 0.250, 0.388, 0.559, 0.761, 1.000 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "106", scale, "T.1154", "M3 HF Aerial Feed", "Number", BindingValueUnits.Numeric, "%.4f", true));
            }

            AddFunction(new ScaledNetworkValue(this, "232", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 1d), "R.1155", "Tuner", "Number", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "239", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 1d), "R.1155", "Tuning Cathode", "Number", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "228", new CalibrationPointCollectionDouble(0d, 0d, 1.0d, 1d), "R.1155", "Tuning Anode", "Number", BindingValueUnits.Numeric, "%.4f"));
            {
                double[] output = new double[] { -1.0, 0.0, 100.0 };
                double[] input = new double[] { -1.0, 0.0, 1.0 };
                CalibrationPointCollectionDouble scale = new CalibrationPointCollectionDouble(input[0], output[0], input[input.Count() - 1], output[output.Count() - 1]);
                for (int ii = 1; ii < input.Count() - 2; ii++) scale.Add(new CalibrationPointDouble(input[ii], output[ii]));
                AddFunction(new ScaledNetworkValue(this, "88", scale, "R.1155", "Direction Finder (Left)", "Number", BindingValueUnits.Numeric, "%.4f", true));
                AddFunction(new ScaledNetworkValue(this, "89", scale, "R.1155", "Direction Finder (Right)", "Number","", BindingValueUnits.Numeric, "%.4f", true));
            }
            #endregion Radio Equipment

#if (DEV)
            Console.WriteLine("Number of functions:\t{0}",Functions.Count());
            foreach(DCSFunction nf in Functions)
            {
                Console.WriteLine("\tArgument: {2} | {0} | {1} | {3} | {4}", nf.DeviceName , nf.Name, (nf.DataElements.Count() == 0 ? "***" : nf.DataElements[0].ID ), clickableArgs.ContainsKey(nf.DataElements[0].ID)? clickableArgs[nf.DataElements[0].ID].Description : "***", dcsBiosArgs.ContainsKey(nf.DataElements[0].ID) ? dcsBiosArgs[nf.DataElements[0].ID] : "***");
                if (clickableArgs.ContainsKey(nf.DataElements[0].ID)) clickableArgs.Remove(nf.DataElements[0].ID);
                if (dcsBiosArgs.ContainsKey(nf.DataElements[0].ID)) dcsBiosArgs.Remove(nf.DataElements[0].ID);
            }

            foreach (FunctionData fd in clickableArgs.Values)
            {
                Console.WriteLine("Unused Clickable : {0} | {1} | {2} | {3} | {4}", fd.Arg, fd.Var, fd.Device, fd.Command, fd.Description);
            }
            foreach (string argID in dcsBiosArgs.Keys)
            {
                Console.WriteLine("Unused DCS BIOS : {0} | {1}", argID, dcsBiosArgs[argID]);
            }
#endif
        }
    }
}