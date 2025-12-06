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

using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Xml.Linq;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.C130J
{
    internal static class ProcessClickables
    {
        private static string _pattern;
        private static string _input;
        private static Dictionary<string, FunctionData> _functions = new Dictionary<string, FunctionData>();
        private static NetworkFunctionCollection _functionList = new NetworkFunctionCollection();
        private static BaseUDPInterface _baseUDPInterface;
        private static readonly Dictionary <string, string>_categorySubstitutions;
        static ProcessClickables() {
            _categorySubstitutions = CategoryInit();
        }
        internal static NetworkFunctionCollection Process(BaseUDPInterface udpInterface)
        {
            _baseUDPInterface = udpInterface;
            SetClickables();
            RegexOptions options = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            int i = 0;
            foreach (Match m in Regex.Matches(_input, _pattern, options))
            {
                i++;
                var argArray = m.Groups["arg"].Captures.OfType<Capture>().Select(c => c.Value).ToArray();
                bool duplicateArg = false;
                string argKey;
                if (!argArray.Any())
                {
                    if (m.Groups["value"].Captures.Count >= 1 && Int32.TryParse(m.Groups["value"].Captures[0].Value, out int newArg))
                    {
                        argKey = newArg.ToString();
                        argArray = new string[] { argKey };
                    }
                    else
                    {
                        argKey = (i + 9000).ToString();
                    }
                }
                else
                {
                    duplicateArg = _functions.ContainsKey(argArray[0]);
                    argKey = duplicateArg ? "D_" + argArray[0] : argArray[0];  // this obviously will only allow one duplicate
                }
                _functions.Add(argKey, new FunctionData()
                {
                    Fn = m.Groups["function"].Value,
                    Name = m.Groups["name"].Captures.OfType<Capture>().Select(c => c.Value).ToArray(),
                    Val = m.Groups["value"].Captures.OfType<Capture>().Select(c => c.Value).ToArray(),
                    Arg = argArray,
                    Device = m.Groups["device"].Value,
                    Command = m.Groups["command"].Captures.OfType<Capture>().Select(c => c.Value).ToArray(),
                    Head = "",
                    Tail = "",
                    Description = "",
                    Duplicate = duplicateArg
                });
            }
            string[,] lamps = LampsToArray();
            for (int j = 0; j < lamps.GetLength(0); j++)
            {
                if (string.IsNullOrEmpty(lamps[j, 2]))
                {
                    continue;
                }
                _functions.Add(lamps[j, 0], new FunctionData()
                {
                    Fn = "Lamp",
                    Name = new string[] { lamps[j, 1], $"{lamps[j, 2]} Indicator" },
                    Val = null,
                    Arg = new string[] { lamps[j, 0] },
                    Device = null,
                    Command = null,
                    Head = "",
                    Tail = "",
                    Description = "",
                    Duplicate = false
                });
            }


            foreach (FunctionData fd in _functions.Values)
            {
                if (!fd.Duplicate)
                {
                    FunctionBuilder(fd);
                }
                else
                {
                    Console.WriteLine("Not creating function for duplicate \"{0}\"", fd.Name);
                }

            }
            return _functionList;
        }
        internal static void Analyze()
        {
            foreach (string fn in _functions.Values.Select(f => f.Fn).Distinct())
            {
                //Console.WriteLine("\t\t\t case \"{0}\":\n\t\t\tbreak;", fn);
            }
        }
        internal static void CreateFunctionSwitcher()
        {
            Console.WriteLine("\tswitch(fd.Fn){");
            foreach (string fn in _functions.Values.Select(f => f.Fn).Distinct())
            {
                Console.WriteLine("\t\t\t case \"{0}\":\n\t\t\tbreak;", fn);
            }
            Console.WriteLine("\t\tdefault:\n\t\t\tbreak;\n\t}");
        }

        internal static void FunctionBuilder(FunctionData fd)
        {
            switch (fd.Fn)
            {
                case "Lamp":
                    Console.WriteLine("\t\t{0}", BuildFnLamp(fd));
                    break;
                case "display_rocker_hdd":
                case "master_warning":
                case "master_caution":
                case "ref_btn":
                case "push_button":
                case "base_btn":
                case "microwave_key":
                case "amu_key":
                case "cni_key_no_stop":
                    Console.WriteLine("\t\t{0}", BuildFnKey(fd));
                    break;
                case "boost_guard":
                case "fuel_xfeed":
                case "parking_brake":
                case "air_deflector":
                case "landing_lights":
                case "two_pos_switch_ap":
                case "two_pos_switch_spring":
                case "two_pos_switch":
                    Console.WriteLine("\t\t{0}", BuildFnToggle(fd));
                    break;
                case "generator_switch":
                case "two_pos_switch_rev":
                    break;
                case "fire_pull":
                    break;
                case "multiswitch_stop":
                    break;
                case "lsgi_btn":
                    break;
                case "at_disconnect":
                    break;
                case "gear_handle":
                    break;
                case "landing_lights_motor":
                    break;
                case "base_btn_cycle3":
                    break;
                case "cni_brt":
                    break;
                case "multiswitch":
                    break;

                case "wiper":
                    break;
                case "knob_360_press":
                    break;
                case "rotary":
                    break;
                case "knob_360_0_1":
                    break;
                case "knob_fixed":
                    break;
                case "knob_rot":
                    break;
                case "ics_knob":
                    break;
                case "one_way_rocker":
                    break;
                case "rocker_centering":
                    break;
                case "stby_altim":
                    break;
                case "adi_cage":
                    break;
                case "flap_switch":
                    break;
                case "hud_btn":
                    break;
                case "hud_latch":
                    break;
                case "hud_brt_knob":
                    break;
                case "guard":
                    break;
                case "three_pos_switch_spring":
                    break;
                case "knob_rot_rel":
                    break;
                case "fuel_transfer":
                    break;
                case "three_pos_spring_load_on":
                    break;
                case "three_pos_spring_load_on_inv":
                    break;
                case "emg_ext_light":
                    break;
                case "rudder_trim":
                    break;
                case "oil_flap_switch":
                    break;
                case "oil_flap_switch_open_close":
                    break;
                case "oxygen_switch":
                    break;
                case "glare_activate":
                    break;
                case "scroll_point_axis":
                    break;
                case "base_btn_cycle":
                    break;
                case "base_btn_cycle2":
                    break;
                case "base_btn_cycle_release":
                    break;
                case "cable_interaction":
                    break;
                case "knob_360":
                    break;
                case "emergency_trim":
                    break;
                case "tab":
                    break;
                default:
                    break;
            }

        }

        private static string BuildFnLamp(FunctionData fd)
        {
            (string category, string name) = AdjustName(fd.Name, fd.Device);
            _functionList.Add(new FlagValue(_baseUDPInterface, fd.Arg[0], category, name,""));
            return $"AddFunction(new FlagValue(this, \"{fd.Arg[0]}\", \"{category}\", \"{name}\", \"\"));";
        }
        private static string BuildFnKey(FunctionData fd)
        {
            //devices d = (devices)Enum.Parse(typeof(devices),fd.Device);
            //string di = ((devices)Enum.Parse(typeof(devices), fd.Device)).ToString("d");
            //var x = fd.Command[0].Split('.');
            //Type type = Type.GetType($"GadrocsWorkshop.Helios.Interfaces.DCS.C130J.Commands+{fd.Command[0].Split('.')[0]}");
            //string o = ((int)Enum.Parse(Type.GetType($"GadrocsWorkshop.Helios.Interfaces.DCS.C130J.Commands+{fd.Command[0].Split('.')[0]}"), fd.Command[0].Split('.')[1])).ToString();
            ////string os = o.ToString("d");
            ////string s = fd.Command[0];
            ////string dj = c.ToString("d");

            (string category, string name) = AdjustName(fd.Name, fd.Device);
            _functionList.Add(new PushButton(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], category, name));
            return $"AddFunction(new PushButton(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", \"{category}\", \"{name}\"));";
        }
        private static string BuildFnToggle(FunctionData fd)
        {
            (string category, string name) = AdjustName(fd.Name, fd.Device);
            _functionList.Add(Switch.CreateToggleSwitch(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], "1.0", "OPEN", "0.0", "CLOSE", category, name, "%0.1f"));
            return $"AddFunction(Switch.CreateToggleSwitch(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", \"1.0\", \"OPEN\", \"0.0\", \"CLOSE\", \"{category}\", \"{name}\", \"%0.1f\"));";
        }
        private static string CommandEnumToString(string commandEnum)
        {
            return ((int)Enum.Parse(Type.GetType($"GadrocsWorkshop.Helios.Interfaces.DCS.C130J.Commands+{commandEnum.Split('.')[0]}"), commandEnum.Split('.')[1])).ToString();
        }
        private static string DeviceEnumToString(string deviceEnum)
        {
            return ((devices)Enum.Parse(typeof(devices), deviceEnum)).ToString("d");
        }
        private static (string, string) AdjustName(string[] origName, string origDevice)
        {
            string category;
            string name;
            if (origName.Length > 1)
            {
                //Pilot Master Warning Button -Push to Reset
                if (origName[0] == "ARC")
                {
                    category = "ARC-210";
                    name = origName[1].Substring(origName[1].IndexOf("210") + 3).Trim();
                }
                else if (origName[1].Contains("Push to Reset"))
                {
                    category = "Ref Panel";
                    name = origName[0].Trim();
                }
                else if (origName[0].Contains("Microwave") || origName[0].Contains("Btm drawer") || origName[0].Contains("Top shelf") || origName[0].Contains("Galley"))
                {
                    category = "Galley";
                    name = ($"{origName[0].Trim()}-{origName[1]}").Replace("Btm", "Bottom").Replace("Galley ", "");
                }
                else if (origName[0].Contains("Anti"))
                {
                    category = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(origDevice.ToLower().Replace("_", " "));
                    name = $"{origName[0].Trim()}-{origName[1]}";
                }
                else
                {
                    category = origName[0].Trim();
                    name = origName[1].Trim();
                }
            }
            else
            {
                if (origName[0].Contains("LSK"))
                {
                    category = origName[0].Substring(0, origName[0].IndexOf("LSK")).Trim();
                    name = origName[0].Substring(origName[0].IndexOf("LSK")).Trim();
                }
                else if (origName[0].Contains("FLCV"))
                {
                    category = "FLCV";
                    name = origName[0].Substring(origName[0].IndexOf("FLCV") + 4).Trim();
                }
                else if (origName[0].Contains("CNBP"))
                {
                    category = "CNBP";
                    name = origName[0].Substring(origName[0].IndexOf("CNBP") + 4).Trim();
                }
                else if (origName[0].Contains("Microwave") || origName[0].Contains("Galley"))
                {
                    category = "Galley";
                    name = origName[0].Replace("Galley ", "");
                }
                else if (origName[0].Contains("Radar"))
                {
                    category = "RADAR";
                    name = origName[0].Substring(origName[0].IndexOf("Radar") + 5).Trim();
                }
                else if (origName[0].Contains("RWR"))
                {
                    category = "RWR";
                    name = origName[0].Substring(origName[0].IndexOf("RWR") + 3).Trim();
                }
                else if (origName[0].Contains("Landing Gear"))
                {
                    category = "Landing Gear";
                    name = origName[0].Substring(origName[0].IndexOf("Landing Gear") + 12).Trim();
                }
                else
                {
                    category = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(origDevice.ToLower().Replace("_", " "));
                    name = origName[0].Trim();
                }
            }

            foreach (var pair in _categorySubstitutions)
            {
                category = category.Replace(pair.Key, pair.Value);
            }

            foreach (var swap in new List<string>{ " AMU", " Displays", " CNI" })
            {
                if (category.Contains(swap))
                {
                    category = $"{swap.Trim()} {category.Replace(swap, "")}";
                    break;
                }
            }
            return (category, name);
        }
        private static Dictionary<string, string> CategoryInit()
        {
            return new Dictionary<string, string> {
                { "C ", "Copilot " },
                { "P ", "Pilot " },
                { "Ap Interface", "Autopilot" },
                { "Cms Mgr", "Counter Measure System" },
                { "Atm", "Atmospheric" },
                { " Apu Ctrl", "" },
                { "Copilot Ref Mode Panel", "Ref Mode Panel Copilot" },
                { "Ref Panel", "Ref Mode Panel Pilot" },
                };
        }
        private static void SetClickables()
        {
            #region Process Clickables
            _pattern = @"^elements\[""[^""]*?(?:_(?<arg>\d{2,4}))*""\].*=\s*(?'function'.*)\((?:""(?'name'[^-,]*)(?:\s*-\s*(?'name'[^-,]*))*"")(((?:\s*,\s*devices\.(?'device'[^,\s\)]*))(?:\s*,\s*(?'command'[^,\s\)]*))(?:\s*,\s*((?'value'[^,\s]*)))*)|((?:\s*,\s*(?'command'[^,\s\)]*))(?:\s*,\s*(?'value'[^,\s\)]*))*)(?:\s*,\s*devices\.(?'device'[^,\s\)]*)))(?:\s*,\s*(?'value'[^,\s\)]*))*\s*\)\s*";
            _input = @"show_element_boxes            = false
show_element_parent_boxes     = false
show_tree_boxes               = false
show_other_pointers           = false
show_indicator_borders        = false

dofile(LockOn_Options.script_path..""clickable_defs.lua"")
dofile(LockOn_Options.script_path..""command_defs.lua"")
dofile(LockOn_Options.script_path..""devices.lua"")
dofile(LockOn_Options.script_path..""sounds.lua"")
dofile(LockOn_Options.script_path..""config.lua"")

local gettext = require(""i_18n"")
_ = gettext.translate

cursor_mode =
{
    CUMODE_CLICKABLE = 0,
    CUMODE_CLICKABLE_AND_CAMERA  = 1,
    CUMODE_CAMERA = 2,
};

clickable_mode_initial_status  = cursor_mode.CUMODE_CLICKABLE
elements = {}
-------------------------------------------------------------------------- FIRE_HANDLES
elements[""PNT_ENGFIRE_HANDLE_1_314_315""] = fire_pull(""Engine 1 Fire Handle"",  devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.engine_1_fire_pull, ENGINE_ELEC_APU_CTRL.engine_1_fire, 314, 315, 1)
elements[""PNT_ENGFIRE_HANDLE_2_316_317""] = fire_pull(""Engine 2 Fire Handle"",  devices.ENGINE_APU_CTRL,  ENGINE_ELEC_APU_CTRL.engine_2_fire_pull, ENGINE_ELEC_APU_CTRL.engine_2_fire,  316, 317, 1)
elements[""PNT_ENGFIRE_HANDLE_3_318_319""] = fire_pull(""Engine 3 Fire Handle"",  devices.ENGINE_APU_CTRL,  ENGINE_ELEC_APU_CTRL.engine_3_fire_pull, ENGINE_ELEC_APU_CTRL.engine_3_fire, 318, 319, 1)
elements[""PNT_ENGFIRE_HANDLE_4_320_321""] = fire_pull(""Engine 4 Fire Handle"",  devices.ENGINE_APU_CTRL,  ENGINE_ELEC_APU_CTRL.engine_4_fire_pull, ENGINE_ELEC_APU_CTRL.engine_4_fire,  320, 321, 1)

-- ------------------------------------------------------------------------ ENGINE__PANEL
elements[""PNT_ENGSTART_1_310""] = multiswitch_stop(""Engine 1 Start Switch"", devices.ENGINE_APU_CTRL,   ENGINE_ELEC_APU_CTRL.engine_1_start_select, 310, {-0.33, 1}, 0.5, ENGINE_ELEC_APU_CTRL.engine_1_stop)
elements[""PNT_ENGSTART_2_311""] = multiswitch_stop(""Engine 2 Start Switch"", devices.ENGINE_APU_CTRL,   ENGINE_ELEC_APU_CTRL.engine_2_start_select, 311, {-0.33, 1}, 0.5, ENGINE_ELEC_APU_CTRL.engine_2_stop)
elements[""PNT_ENGSTART_3_312""] = multiswitch_stop(""Engine 3 Start Switch"", devices.ENGINE_APU_CTRL,   ENGINE_ELEC_APU_CTRL.engine_3_start_select, 312, {-0.33, 1}, 0.5, ENGINE_ELEC_APU_CTRL.engine_3_stop)
elements[""PNT_ENGSTART_4_313""] = multiswitch_stop(""Engine 4 Start Switch"", devices.ENGINE_APU_CTRL,   ENGINE_ELEC_APU_CTRL.engine_4_start_select, 313, {-0.33, 1}, 0.5, ENGINE_ELEC_APU_CTRL.engine_4_stop)

elements[""PNT_GND_IDL_1_46""] = lsgi_btn(""Engine 1 LSGI Select Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.lsgi_1, 46)
elements[""PNT_GND_IDL_2_47""] = lsgi_btn(""Engine 2 LSGI Select Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.lsgi_2, 47)
elements[""PNT_GND_IDL_3_48""] = lsgi_btn(""Engine 3 LSGI Select Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.lsgi_3, 48)
elements[""PNT_GND_IDL_4_49""] = lsgi_btn(""Engine 4 LSGI Select Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.lsgi_4, 49)

elements[""PNT_L_ATHROTTLE_DISC_19""] = at_disconnect(""Left Autothrottle Disconnect Button"", devices.AP_INTERFACE, PILOT_AP.at_disconnect_l, 19)
elements[""PNT_R_ATHROTTLE_DISC_20""] = at_disconnect(""Right Autothrottle Disconnect Button"", devices.AP_INTERFACE, PILOT_AP.at_disconnect_r, 20)

-------------------------------------------------------------------------- LANDING_GEAR_LIGHTS_PANEL
elements[""PNT_GEAR_LVR_126""] = gear_handle(""Landing Gear Handle"", devices.HYDRAULICS, HYD_SYSTEM.gear_lever, 126, 4)
elements[""PNT_DWN_LOCK_RELEASE_36""] = base_btn(""Landing Gear Downlock Release Button"", devices.HYDRAULICS, HYD_SYSTEM.gear_lock_release, 36)
elements[""PNT_LDG_LIGHT_L_32""] = landing_lights(""Left Landing Light Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.landing_left, 32, 18)
elements[""PNT_LDG_LIGHT_R_33""] = landing_lights(""Right Landing Light Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.landing_right, 33, 18)
elements[""PNT_MOTORS_L_30""] = landing_lights_motor(""Left Landing Light Motor Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.landing_left_position, 30, {-1, 1}, 1, 12)
elements[""PNT_MOTORS_R_31""] = landing_lights_motor(""Right Landing Light Motor Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.landing_right_position, 31, {-1, 1}, 1, 12)
elements[""PNT_TXI_LTE_L_34""] = landing_lights(""Taxi Lights Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.taxi_light_wheels, 34, 18)
elements[""PNT_TXI_LTE_R_35""] = landing_lights(""Wingtip Taxi Lights Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.taxi_light_wings, 35, 18)
elements[""PNT_TXI_LTE_R_35""] = landing_lights(""Wingtip Taxi Lights Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.taxi_light_wings, 35, 18)
elements[""PNT_MOTORS_ALL_30_31""] = base_btn_cycle3(""Toggle Both Landing Light Motors"", devices.LIGHTING_PANELS, LIGHTING_PANELS.landing_motor_all)
elements[""PNT_LDG_LIGHT_ALL_32_33""] = base_btn_cycle3(""Toggle Both Landing Lights"", devices.LIGHTING_PANELS, LIGHTING_PANELS.landing_light_all)
elements[""PNT_TXI_ALL_34_35""] =  base_btn_cycle3(""Toggle Both Taxi Lights"", devices.LIGHTING_PANELS, LIGHTING_PANELS.taxi_light_all)
-------------------------------------------------------------------------- SWING_WINDOWS
-- elements[""PNT_L_SWING_WIND_LATCH_10""] = multiswitch(""Pilot Window Open/Close"", devices.MECH_INTERFACE, MECH_INTERFACE.pilot_window, 10, {-0.21, 1},  1.21,  2.5)
-- elements[""PNT_L_SWING_WIND_LATCH_10""] = window_latch(""Pilot Window Open/Close"", devices.MECH_INTERFACE, MECH_INTERFACE.pilot_window, 10, 1.5)
-- elements[""PNT_L_WINDOW_CATCH_08""] = window_latch_top(""Pilot Window Top Latch"", devices.MECH_INTERFACE, MECH_INTERFACE.pilot_top_latch, 8, 3.5)
-- elements[""PNT_L_SWING_OPEN_CATCH_9""] = window_latch_bottom(""Pilot Window Bottom Latch"", devices.MECH_INTERFACE, MECH_INTERFACE.pilot_bottom_latch, 9, 14)

-- elements[""PNT_R_SWING_WIND_LATCH_11""] = window_latch(""Copilot Window Open/Close"", devices.MECH_INTERFACE, MECH_INTERFACE.copilot_window, 11, 1.5)
-- -- elements[""PNT_R_WINDOW_CATCH_17""] = window_latch_top(""Copilot Window Top Latch"", devices.MECH_INTERFACE, MECH_INTERFACE.copilot_top_latch, 17, 3.5)
-- elements[""PNT_R_SWING_OPEN_CATCH_15""] = window_latch_bottom(""Copilot Window Bottom Latch"", devices.MECH_INTERFACE, MECH_INTERFACE.copilot_bottom_latch, 15, 14)

-------------------------------------------------------------------------- AMU_PANEL
elements[""PNT_CMDU_1_BRT_116""] = display_rocker_hdd(""HDD 1 Brighness Increase"", devices.P_DISPLAYS, P_AMU.l_hdd_brightness, 116, 1.0)
elements[""PNT_CMDU_1_DIM_117""] = display_rocker_hdd(""HDD 1 Brighness Decrease"", devices.P_DISPLAYS, P_AMU.l_hdd_brightness_down, 116,-1.0)

elements[""PNT_CMDU_2_BRT_117""] = display_rocker_hdd(""HDD 2 Brighness Increase"", devices.P_DISPLAYS, P_AMU.r_hdd_brightness, 117, 1.0)
elements[""PNT_CMDU_2_DIM_118""] = display_rocker_hdd(""HDD 2 Brighness Decrease"", devices.P_DISPLAYS, P_AMU.r_hdd_brightness_down, 117, -1.0)

elements[""PNT_CMDU_3_BRT_118""] = display_rocker_hdd(""HDD 3 Brighness Increase"", devices.C_DISPLAYS, C_AMU.r_hdd_brightness, 118, 1.0)
elements[""PNT_CMDU_3_DIM_119""] = display_rocker_hdd(""HDD 3 Brighness Decrease"", devices.C_DISPLAYS, C_AMU.r_hdd_brightness_down, 118, -1.0)

elements[""PNT_CMDU_4_BRT_119""] = display_rocker_hdd(""HDD 4 Brighness Increase"", devices.C_DISPLAYS, C_AMU.l_hdd_brightness, 119, 1.0)
elements[""PNT_CMDU_4_DIM_120""] = display_rocker_hdd(""HDD 4 Brighness Decrease"", devices.C_DISPLAYS, C_AMU.l_hdd_brightness_down, 119, -1.0)

elements[""PNT_AMU_L_BRT_201""] = display_rocker_hdd(""Pilot AMU Brightness Increase"", devices.P_DISPLAYS, P_AMU.amu_brightness, 200, 1.0)
elements[""PNT_AMU_L_DIM_200""] = display_rocker_hdd(""Pilot AMU Brightness Decrease"", devices.P_DISPLAYS, P_AMU.amu_brightness_down, 200, -1.0)

elements[""PNT_AMU_R_BRT_203""] = display_rocker_hdd(""Copilot AMU Brightness Increase"", devices.C_DISPLAYS, C_AMU.amu_brightness, 202, 1.0)
elements[""PNT_AMU_R_DIM_202""] = display_rocker_hdd(""Copilot AMU Brightness Decrease"", devices.C_DISPLAYS, C_AMU.amu_brightness_down, 202, -1.0)

elements[""PNT_L_AMU_L1_BTN1_133""] = amu_key(""Pilot Left AMU LSK L1"", P_AMU.r_key_1,133, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN2_134""] = amu_key(""Pilot Left AMU LSK L2"", P_AMU.r_key_2,134, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN3_135""] = amu_key(""Pilot Left AMU LSK L3"", P_AMU.r_key_3,135, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN4_136""] = amu_key(""Pilot Left AMU LSK L4"", P_AMU.r_key_4,136, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN5_137""] = amu_key(""Pilot Left AMU LSK R1"", P_AMU.r_key_5,137, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN6_138""] = amu_key(""Pilot Left AMU LSK R2"", P_AMU.r_key_6,138, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN7_139""] = amu_key(""Pilot Left AMU LSK R3"", P_AMU.r_key_7,139, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L1_BTN8_140""] = amu_key(""Pilot Left AMU LSK R4"", P_AMU.r_key_8,140, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN1_141""] = amu_key(""Pilot Right AMU LSK L1"", P_AMU.l_key_1,141, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN2_142""] = amu_key(""Pilot Right AMU LSK L2"", P_AMU.l_key_2,142, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN3_143""] = amu_key(""Pilot Right AMU LSK L3"", P_AMU.l_key_3,143, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN4_144""] = amu_key(""Pilot Right AMU LSK L4"", P_AMU.l_key_4,144, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN5_145""] = amu_key(""Pilot Right AMU LSK R1"", P_AMU.l_key_5,145, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN6_146""] = amu_key(""Pilot Right AMU LSK R2"", P_AMU.l_key_6,146, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN7_147""] = amu_key(""Pilot Right AMU LSK R3"", P_AMU.l_key_7,147, devices.P_DISPLAYS)
elements[""PNT_L_AMU_L2_BTN8_148""] = amu_key(""Pilot Right AMU LSK R4"", P_AMU.l_key_8,148, devices.P_DISPLAYS)
--------------------------------------------------------------
elements[""PNT_AMU_R1_BTN1_174""] = amu_key(""Copilot Left AMU LSK L1"", C_AMU.l_key_1, 174, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN2_177""] = amu_key(""Copilot Left AMU LSK L2"", C_AMU.l_key_2, 175, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN3_175""] = amu_key(""Copilot Left AMU LSK L3"", C_AMU.l_key_3, 176, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN4_176""] = amu_key(""Copilot Left AMU LSK L4"", C_AMU.l_key_4, 177, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN5_178""] = amu_key(""Copilot Left AMU LSK R1"", C_AMU.l_key_5, 178, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN6_179""] = amu_key(""Copilot Left AMU LSK R2"", C_AMU.l_key_6, 179, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN7_180""] = amu_key(""Copilot Left AMU LSK R3"", C_AMU.l_key_7, 180, devices.C_DISPLAYS)
elements[""PNT_AMU_R1_BTN8_181""] = amu_key(""Copilot Left AMU LSK R4"", C_AMU.l_key_8, 181, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN1_182""] = amu_key(""Copilot Right AMU LSK L1"", C_AMU.r_key_1, 182, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN2_183""] = amu_key(""Copilot Right AMU LSK L2"", C_AMU.r_key_2, 183, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN3_185""] = amu_key(""Copilot Right AMU LSK L3"", C_AMU.r_key_3, 184, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN4_184""] = amu_key(""Copilot Right AMU LSK L4"", C_AMU.r_key_4, 185, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN5_186""] = amu_key(""Copilot Right AMU LSK R1"", C_AMU.r_key_5, 186, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN6_187""] = amu_key(""Copilot Right AMU LSK R2"", C_AMU.r_key_6, 187, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN7_188""] = amu_key(""Copilot Right AMU LSK R3"", C_AMU.r_key_7, 188, devices.C_DISPLAYS)
elements[""PNT_AMU_R2_BTN8_189""] = amu_key(""Copilot Right AMU LSK R4"", C_AMU.r_key_8, 189, devices.C_DISPLAYS)

-------------------------------------------------------------------------- COMM_NAV_ECB_PANEL
elements[""PNT_CNBP_COMM_159""] = amu_key(""CNBP COMM Key"", CNE.comm, 159, devices.CNBP)
elements[""PNT_CNBP_NAV_160""] =  amu_key(""CNBP NAV Key"", CNE.nav, 160, devices.CNBP)
elements[""PNT_CNBP_ECB_161""] =  amu_key(""CNBP ECB Key"", CNE.ecb, 161, devices.CNBP)
elements[""PNT_CNBP_BTN1_151""] = amu_key(""CNBP LSK L1"", CNE.selkey1, 151, devices.CNBP)
elements[""PNT_CNBP_BTN2_153""] = amu_key(""CNBP LSK L2"", CNE.selkey2, 152, devices.CNBP)
elements[""PNT_CNBP_BTN3_152""] = amu_key(""CNBP LSK L3"", CNE.selkey3, 153, devices.CNBP)
elements[""PNT_CNBP_BTN4_154""] = amu_key(""CNBP LSK L4"", CNE.selkey4, 154, devices.CNBP)
elements[""PNT_CNBP_BTN5_155""] = amu_key(""CNBP LSK R1"", CNE.selkey5, 155, devices.CNBP)
elements[""PNT_CNBP_BTN6_156""] = amu_key(""CNBP LSK R2"", CNE.selkey6, 156, devices.CNBP)
elements[""PNT_CNBP_BTN7_157""] = amu_key(""CNBP LSK R3"", CNE.selkey7, 157, devices.CNBP)
elements[""PNT_CNBP_BTN8_158""] = amu_key(""CNBP LSK R4"", CNE.selkey8, 158, devices.CNBP)
elements[""PNT_CNBP_NUM1_162""] = amu_key(""CNBP 1 Key"", CNE.num_1, 162, devices.CNBP)
elements[""PNT_CNBP_NUM2_163""] = amu_key(""CNBP 2 Key"", CNE.num_2, 163, devices.CNBP)
elements[""PNT_CNBP_NUM3_164""] = amu_key(""CNBP 3 Key"", CNE.num_3, 164, devices.CNBP)
elements[""PNT_CNBP_NUM4_165""] = amu_key(""CNBP 4 Key"", CNE.num_4, 165, devices.CNBP)
elements[""PNT_CNBP_NUM5_166""] = amu_key(""CNBP 5 Key"", CNE.num_5,  166, devices.CNBP)
elements[""PNT_CNBP_NUM6_167""] = amu_key(""CNBP 6 Key"", CNE.num_6, 167, devices.CNBP)
elements[""PNT_CNBP_NUM7_167""] = amu_key(""CNBP 7 Key"", CNE.num_7, 168, devices.CNBP)
elements[""PNT_CNBP_NUM8_168""] = amu_key(""CNBP 8 Key"", CNE.num_8, 169, devices.CNBP)
elements[""PNT_CNBP_NUM9_170""] = amu_key(""CNBP 9 Key"", CNE.num_9, 170, devices.CNBP)
elements[""PNT_CNBP_NUM_DOT_171""] = amu_key(""CNBP Decimal Key"", CNE.num_dot, 171, devices.CNBP)
elements[""PNT_CNBP_NUM0_172""] = amu_key(""CNBP 0 Key"", CNE.num_0, 172, devices.CNBP)
elements[""PNT_CNBP_NUM_CLR_173""] = amu_key(""CNBP CLR Key"", CNE.clr, 173, devices.CNBP)
elements[""PNT_CNBP_BRT_202""] = display_rocker_hdd(""CNBP Brightness Increase"", devices.CNBP, CNE.brightness, 201, 1.0)
elements[""PNT_CNBP_DIM_201""] = display_rocker_hdd(""CNBP Brightness Decrease"", devices.CNBP, CNE.brightness_down, 201, -1.0)

----------------------------------------------------------------------- CNI_MU_PANEL
elements[""PNT_P_CNI_DISPL1_1100""] = cni_key_no_stop(""Pilot CNI-MU LSK L1"", P_CNI.SELECT_1, 1100, devices.P_CNI)
elements[""PNT_P_CNI_DISPL2_1101""] = cni_key_no_stop(""Pilot CNI-MU LSK L2"", P_CNI.SELECT_2, 1101, devices.P_CNI)
elements[""PNT_P_CNI_DISPL3_1102""] = cni_key_no_stop(""Pilot CNI-MU LSK L3"", P_CNI.SELECT_3, 1102, devices.P_CNI)
elements[""PNT_P_CNI_DISPL4_1103""] = cni_key_no_stop(""Pilot CNI-MU LSK L4"", P_CNI.SELECT_4, 1103, devices.P_CNI)
elements[""PNT_P_CNI_DISPL5_1104""] = cni_key_no_stop(""Pilot CNI-MU LSK L5"", P_CNI.SELECT_5, 1104, devices.P_CNI)
elements[""PNT_P_CNI_DISPL6_1105""] = cni_key_no_stop(""Pilot CNI-MU LSK L6"", P_CNI.SELECT_6, 1105, devices.P_CNI)
elements[""PNT_P_CNI_DISPR1_1106""] = cni_key_no_stop(""Pilot CNI-MU LSK R1"", P_CNI.SELECT_7, 1106, devices.P_CNI)
elements[""PNT_P_CNI_DISPR2_1107""] = cni_key_no_stop(""Pilot CNI-MU LSK R2"", P_CNI.SELECT_8, 1107, devices.P_CNI)
elements[""PNT_P_CNI_DISPR3_1108""] = cni_key_no_stop(""Pilot CNI-MU LSK R3"", P_CNI.SELECT_9, 1108, devices.P_CNI)
elements[""PNT_P_CNI_DISPR4_1109""] = cni_key_no_stop(""Pilot CNI-MU LSK R4"", P_CNI.SELECT_10, 1109, devices.P_CNI)
elements[""PNT_P_CNI_DISPR5_1110""] = cni_key_no_stop(""Pilot CNI-MU LSK R5"", P_CNI.SELECT_11, 1110, devices.P_CNI)
elements[""PNT_P_CNI_DISPR6_1111""] = cni_key_no_stop(""Pilot CNI-MU LSK R6"", P_CNI.SELECT_12, 1111, devices.P_CNI)
elements[""PNT_P_CNI_COMM_1112""] = cni_key_no_stop(""Pilot CNI-MU COMM TUNE Key"", P_CNI.COMM_TUNE, 1112, devices.P_CNI)
elements[""PNT_P_CNI_TOLD_1118""] = cni_key_no_stop(""Pilot CNI-MU TOLD Key"", P_CNI.TOLD, 1118, devices.P_CNI)
elements[""PNT_P_CNI_NAV_1113""] = cni_key_no_stop(""Pilot CNI-MU NAV TUNE Key"", P_CNI.NAV_TUNE, 1113, devices.P_CNI)
elements[""PNT_P_CNI_INDX_1119""] = cni_key_no_stop(""Pilot CNI-MU INDX Key"", P_CNI.INDEX, 1119, devices.P_CNI)
elements[""PNT_P_CNI_MC_INDEX_1120""] = cni_key_no_stop(""Pilot CNI-MU MC INDX Key"", P_CNI.MC_INDX, 1120, devices.P_CNI)

elements[""PNT_P_CNI_EXEC_1122""] = cni_key_no_stop(""Pilot CNI-MU EXEC Key"", P_CNI.EXEC, 1122, devices.P_CNI)

elements[""PNT_P_CNI_BRTUP_1127""] = cni_brt(""Pilot CNI-MU Brightness Increase"", P_CNI.BRT_UP, 1127, devices.P_CNI, 1)
elements[""PNT_P_CNI_BRTDWN_1127""] = cni_brt(""Pilot CNI-MU Brightness Decrease"", P_CNI.BRT_DN, 1127, devices.P_CNI, 0)

elements[""PNT_P_CNI_DIR_1117""] = cni_key_no_stop(""Pilot CNI-MU DIR INTC Key"", P_CNI.DIR, 1117, devices.P_CNI)
elements[""PNT_P_CNI_LEGS_1123""] = cni_key_no_stop(""Pilot CNI-MU LEGS Key"", P_CNI.LEGS, 1123, devices.P_CNI)
elements[""PNT_P_CNI_NEXT_1126""] = cni_key_no_stop(""Pilot CNI-MU NEXT PAGE Key"", P_CNI.NEXT, 1126, devices.P_CNI)
elements[""PNT_P_CNI_PREV_1125""] = cni_key_no_stop(""Pilot CNI-MU PREV PAGE Key"", P_CNI.PREV, 1125, devices.P_CNI)
elements[""PNT_P_CNI_MARK_1124""] = cni_key_no_stop(""Pilot CNI-MU MARK Key"", P_CNI.MARK, 1124, devices.P_CNI)
elements[""PNT_P_CNI_IFF_1114""] = cni_key_no_stop(""Pilot CNI-MU IFF Key"", P_CNI.IFF, 1114, devices.P_CNI)
elements[""PNT_P_CNI_NAVCTRL_1115""] = cni_key_no_stop(""Pilot CNI-MU NAV CTRL Key"", P_CNI.NAV_CTRL, 1115, devices.P_CNI)
elements[""PNT_P_CNI_MSN_1116""] = cni_key_no_stop(""Pilot CNI-MU MSN Key"", P_CNI.MSN, 1116, devices.P_CNI)
elements[""PNT_P_CNI_CAPS_1121""] = cni_key_no_stop(""Pilot CNI-MU CAPS Key"", P_CNI.CAPS, 1121, devices.P_CNI)
elements[""PNT_P_CNI_0_1137""] = cni_key_no_stop(""Pilot CNI-MU 0 Key"", P_CNI.KBD_0, 1137, devices.P_CNI)
elements[""PNT_P_CNI_1_1128""] = cni_key_no_stop(""Pilot CNI-MU 1 Key"", P_CNI.KBD_1, 1128, devices.P_CNI)
elements[""PNT_P_CNI_2_1129""] = cni_key_no_stop(""Pilot CNI-MU 2 Key"", P_CNI.KBD_2, 1129, devices.P_CNI)
elements[""PNT_P_CNI_3_1130""] = cni_key_no_stop(""Pilot CNI-MU 3 Key"", P_CNI.KBD_3, 1130, devices.P_CNI)
elements[""PNT_P_CNI_4_1131""] = cni_key_no_stop(""Pilot CNI-MU 4 Key"", P_CNI.KBD_4, 1131, devices.P_CNI)
elements[""PNT_P_CNI_5_1132""] = cni_key_no_stop(""Pilot CNI-MU 5 Key"", P_CNI.KBD_5, 1132, devices.P_CNI)
elements[""PNT_P_CNI_6_1133""] = cni_key_no_stop(""Pilot CNI-MU 6 Key"", P_CNI.KBD_6, 1133, devices.P_CNI)
elements[""PNT_P_CNI_7_1134""] = cni_key_no_stop(""Pilot CNI-MU 7 Key"", P_CNI.KBD_7, 1134, devices.P_CNI)
elements[""PNT_P_CNI_8_1135""] = cni_key_no_stop(""Pilot CNI-MU 8 Key"", P_CNI.KBD_8, 1135, devices.P_CNI)
elements[""PNT_P_CNI_9_1136""] = cni_key_no_stop(""Pilot CNI-MU 9 Key"", P_CNI.KBD_9, 1136, devices.P_CNI)
elements[""PNT_P_CNI_DOT_1138""] = cni_key_no_stop(""Pilot CNI-MU Decimal Key"", P_CNI.KBD_DOT, 1138, devices.P_CNI)
elements[""PNT_P_CNI_PLUSMINUS_1139""] = cni_key_no_stop(""Pilot CNI-MU Minus Key"", P_CNI.KBD_PLUSMINUS, 1139, devices.P_CNI)
elements[""PNT_P_CNI_A_1140""] = cni_key_no_stop(""Pilot CNI-MU A Key"", P_CNI.KBD_A, 1140, devices.P_CNI)
elements[""PNT_P_CNI_B_1141""] = cni_key_no_stop(""Pilot CNI-MU B Key"", P_CNI.KBD_B, 1141, devices.P_CNI)
elements[""PNT_P_CNI_C_1142""] = cni_key_no_stop(""Pilot CNI-MU C Key"", P_CNI.KBD_C, 1142, devices.P_CNI)
elements[""PNT_P_CNI_D_1143""] = cni_key_no_stop(""Pilot CNI-MU D Key"", P_CNI.KBD_D, 1143, devices.P_CNI)
elements[""PNT_P_CNI_E_1144""] = cni_key_no_stop(""Pilot CNI-MU E Key"", P_CNI.KBD_E, 1144, devices.P_CNI)
elements[""PNT_P_CNI_F_1145""] = cni_key_no_stop(""Pilot CNI-MU F Key"", P_CNI.KBD_F, 1145, devices.P_CNI)
elements[""PNT_P_CNI_G_1146""] = cni_key_no_stop(""Pilot CNI-MU G Key"", P_CNI.KBD_G, 1146, devices.P_CNI)
elements[""PNT_P_CNI_H_1147""] = cni_key_no_stop(""Pilot CNI-MU H Key"", P_CNI.KBD_H, 1147, devices.P_CNI)
elements[""PNT_P_CNI_I_1148""] = cni_key_no_stop(""Pilot CNI-MU I Key"", P_CNI.KBD_I, 1148, devices.P_CNI)
elements[""PNT_P_CNI_J_1149""] = cni_key_no_stop(""Pilot CNI-MU J Key"", P_CNI.KBD_J, 1149, devices.P_CNI)
elements[""PNT_P_CNI_K_1150""] = cni_key_no_stop(""Pilot CNI-MU K Key"", P_CNI.KBD_K, 1150, devices.P_CNI)
elements[""PNT_P_CNI_L_1151""] = cni_key_no_stop(""Pilot CNI-MU L Key"", P_CNI.KBD_L, 1151, devices.P_CNI)
elements[""PNT_P_CNI_M_1152""] = cni_key_no_stop(""Pilot CNI-MU M Key"", P_CNI.KBD_M, 1152, devices.P_CNI)
elements[""PNT_P_CNI_N_1153""] = cni_key_no_stop(""Pilot CNI-MU N Key"", P_CNI.KBD_N, 1153, devices.P_CNI)
elements[""PNT_P_CNI_O_1154""] = cni_key_no_stop(""Pilot CNI-MU O Key"", P_CNI.KBD_O, 1154, devices.P_CNI)
elements[""PNT_P_CNI_P_1155""] = cni_key_no_stop(""Pilot CNI-MU P Key"", P_CNI.KBD_P, 1155, devices.P_CNI)
elements[""PNT_P_CNI_Q_1156""] = cni_key_no_stop(""Pilot CNI-MU Q Key"", P_CNI.KBD_Q, 1156, devices.P_CNI)
elements[""PNT_P_CNI_R_1157""] = cni_key_no_stop(""Pilot CNI-MU R Key"", P_CNI.KBD_R, 1157, devices.P_CNI)
elements[""PNT_P_CNI_S_1158""] = cni_key_no_stop(""Pilot CNI-MU S Key"", P_CNI.KBD_S, 1158, devices.P_CNI)
elements[""PNT_P_CNI_T_1159""] = cni_key_no_stop(""Pilot CNI-MU T Key"", P_CNI.KBD_T, 1159, devices.P_CNI)
elements[""PNT_P_CNI_U_1160""] = cni_key_no_stop(""Pilot CNI-MU U Key"", P_CNI.KBD_U, 1160, devices.P_CNI)
elements[""PNT_P_CNI_V_1161""] = cni_key_no_stop(""Pilot CNI-MU V Key"", P_CNI.KBD_V, 1161, devices.P_CNI)
elements[""PNT_P_CNI_W_1162""] = cni_key_no_stop(""Pilot CNI-MU W Key"", P_CNI.KBD_W, 1162, devices.P_CNI)
elements[""PNT_P_CNI_X_1163""] = cni_key_no_stop(""Pilot CNI-MU X Key"", P_CNI.KBD_X, 1163, devices.P_CNI)
elements[""PNT_P_CNI_Y_1164""] = cni_key_no_stop(""Pilot CNI-MU Y Key"", P_CNI.KBD_Y, 1164, devices.P_CNI)
elements[""PNT_P_CNI_Z_1165""] = cni_key_no_stop(""Pilot CNI-MU Z Key"", P_CNI.KBD_Z, 1165, devices.P_CNI)
elements[""PNT_P_CNI_DEL_1167""] = cni_key_no_stop(""Pilot CNI-MU DEL Key"", P_CNI.DEL, 1167, devices.P_CNI)
elements[""PNT_P_CNI_CLR_1169""] = cni_key_no_stop(""Pilot CNI-MU CLR Key"", P_CNI.CLR, 1169, devices.P_CNI)
elements[""PNT_P_CNI_MINUS_1166""] = cni_key_no_stop(""Pilot CNI-MU Unused Key"", P_CNI.KBD_SPACE, 1166, devices.P_CNI)
elements[""PNT_P_CNI_SLASH_1168""] = cni_key_no_stop(""Pilot CNI-MU Slash Key"", P_CNI.KBD_SLASH, 1168, devices.P_CNI)

elements[""PNT_BLEED_AIR_APU_355""] = push_button(""APU Bleed Air Button"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.bleed_apu, 355)
elements[""PNT_BLEEDAIR_L_ISO_394""] = multiswitch(""Left Wing Isolation Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.iso_left, 394, {-1, 1}, 1)
elements[""PNT_BLEEDAIR_DIVIDER_395""] = multiswitch(""Divider Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.divider, 395, {-1, 1}, 1)
elements[""PNT_BLEEDAIR_R_ISO_396""] = multiswitch(""Right Wing Isolation Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.iso_right, 396, {-1, 1}, 1)
elements[""PNT_BLEEDAIR_ENG_1_390""] = multiswitch(""Engine 1 Nacelle Shutoff Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.bleed_eng_1, 390, {-1, 1}, 1)
elements[""PNT_BLEEDAIR_ENG_2_391""] = multiswitch(""Engine 2 Nacelle Shutoff Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.bleed_eng_2, 391, {-1, 1}, 1)
elements[""PNT_BLEEDAIR_ENG_3_392""] = multiswitch(""Engine 3 Nacelle Shutoff Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.bleed_eng_3, 392, {-1, 1}, 1)
elements[""PNT_BLEEDAIR_ENG_4_393""] = multiswitch(""Engine 4 Nacelle Shutoff Valve Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.bleed_eng_4, 393, {-1, 1}, 1)

elements[""PNT_ICE_PROP_1_386""] = multiswitch(""Propeller 1 Ice Protection Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_prop_1, 386, {-1, 1}, 1, 12)
elements[""PNT_ICE_PROP_2_387""] = multiswitch(""Propeller 2 Ice Protection Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_prop_2, 387, {-1, 1}, 1, 12)
elements[""PNT_ICE_PROP_3_388""] = multiswitch(""Propeller 3 Ice Protection Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_prop_3, 388, {-1, 1}, 1, 12)
elements[""PNT_ICE_PROP_4_389""] = multiswitch(""Propeller 4 Ice Protection Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_prop_4, 389, {-1, 1}, 1, 12)
elements[""PNT_ICE_ENGINE_385""] = multiswitch(""Engine Ice Protection Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_engine, 385, {-1, 1}, 1)
elements[""PNT_ICE_WING_EMP_384""] = multiswitch(""Wing/Empennage Ice Protection Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_wing, 384, {-1, 1}, 1)
elements[""PNT_ICE_ANI_DE_ICE_382""] = multiswitch(""Anti-Ice/De-Ice Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.de_ice, 382, {0, 1}, 1)
elements[""PNT_ICE_P_PITOT_378""] = multiswitch(""Pilot Pitot Heat Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_pitot_c, 378, {0, 1}, 1)
elements[""PNT_ICE_CP_PITOT_379""] = multiswitch(""Copilot Pitot Heat Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_pitot_cp, 379, {0, 1}, 1)
elements[""PNT_ICE_NESA_CTR_380""] = multiswitch(""Center NESA Heat Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_nesa_ctr, 380, {0, 1}, 1)
elements[""PNT_ICE_NESA_SIDE_LWR_381""] = multiswitch(""Side/Lower NESA Heat Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ice_nesa_side, 381, {0, 1}, 1)

elements[""PNT_ELEV_BOOST_335""] = boost_guard(""Elevator Control Boost Switch Guard"", devices.HYDRAULICS, HYD_SYSTEM.elevator_boost_guard, 335, 6)
elements[""PNT_ELEV_UTIL_336""] = boost_guard(""Elevator Utility Control Boost Switch Guard"", devices.HYDRAULICS, HYD_SYSTEM.elevator_util_guard, 336, 6)
elements[""PNT_RUDDER_BOOST_337""] = boost_guard(""Rudder Control Boost Switch Guard"", devices.HYDRAULICS, HYD_SYSTEM.rudder_boost_guard, 337, 6)
elements[""PNT_RUDDER_UTIL_338""] = boost_guard(""Rudder Utility Control Boost Switch Guard"", devices.HYDRAULICS, HYD_SYSTEM.rudder_util_guard, 338, 6)
elements[""PNT_AILERON_BOOST_339""] = boost_guard(""Aileron Control Boost Switch Guard"", devices.HYDRAULICS, HYD_SYSTEM.ale_boost_guard, 339, 6)
elements[""PNT_AILERON_UTIL_340""] = boost_guard(""Aileron Utility Control Boost Switch Guard"", devices.HYDRAULICS, HYD_SYSTEM.ale_util_guard, 340, 6)
elements[""PNT_ELEV_BOOST_500""] = two_pos_switch(""Elevator Control Boost Switch"", devices.HYDRAULICS, HYD_SYSTEM.elevator_boost, 500, 6)
elements[""PNT_ELEV_UTIL_501""] = two_pos_switch(""Elevator Utility Control Boost Switch"", devices.HYDRAULICS, HYD_SYSTEM.elevator_util, 501, 6)
elements[""PNT_RUDDER_BOOST_502""] = two_pos_switch(""Rudder Control Boost Switch"", devices.HYDRAULICS, HYD_SYSTEM.rudder_boost, 502, 6)
elements[""PNT_RUDDER_UTIL_503""] = two_pos_switch(""Rudder Utility Control Boost Switch"", devices.HYDRAULICS, HYD_SYSTEM.rudder_util, 503, 6)
elements[""PNT_AILERON_BOOST_504""] = two_pos_switch(""Aileron Control Boost Switch"", devices.HYDRAULICS, HYD_SYSTEM.ale_boost, 504, 6)
elements[""PNT_AILERON_UTIL_505""] = two_pos_switch(""Aileron Utility Control Boost Switch"", devices.HYDRAULICS, HYD_SYSTEM.ale_util, 505, 6)

elements[""PNT_ANTI_SKID_37""] = two_pos_switch(""Anti-Skid Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.anti_skid, 37)

elements[""PNT_EMG_HYD_PUMP_45""] = two_pos_switch(""Auxiliary Hydraulic Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.aux_pump, 45, 8)
elements[""PNT_EMRG_BRAKE_SELECT_99""] = push_button(""Emergency Brake Select Switch"", devices.HYDRAULICS, HYD_SYSTEM.emerg_brake_sel, 99)
elements[""PNT_ENG_PUMP_1_39""] = push_button(""Engine 1 Hydraulic Utility Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.hyd_util_1, 39)
elements[""PNT_ENG_PUMP_2_40""] = push_button(""Engine 2 Hydraulic Utility Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.hyd_util_2, 40)
elements[""PNT_ENG_PUMP_3_41""] = push_button(""Engine 3 Hydraulic Boost Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.hyd_boost_1, 41)
elements[""PNT_ENG_PUMP_4_42""] = push_button(""Engine 4 Hydraulic Boost Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.hyd_boost_2, 42)
elements[""PNT_SUCT_BOOST_PMP_UTIL_43""] = push_button(""Utility Suction Boost Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.suction_boost_pump_util, 43)
elements[""PNT_SUCT_BOOST_PMP_44""] = push_button(""Suction Boost Pump Switch"", devices.HYDRAULICS, HYD_SYSTEM.suction_boost_pump_boost, 44)

elements[""PNT_ELECT_GEN_1_341""] = generator_switch(""Generator 1 Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.gen_1, 341, 8)
elements[""PNT_ELECT_GEN_2_342""] = generator_switch(""Generator 2 Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.gen_2, 342, 8)
elements[""PNT_ELECT_GEN_3_343""] = generator_switch(""Generator 3 Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.gen_3, 343, 8)
elements[""PNT_ELECT_GEN_4_344""] = generator_switch(""Generator 4 Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.gen_4, 344, 8)

elements[""PNT_CP_CNI_DISPL1_1170""] = cni_key_no_stop(""Copilot CNI-MU LSK L1"", C_CNI.SELECT_1, 1170, devices.C_CNI)
elements[""PNT_CP_CNI_DISPL2_1171""] = cni_key_no_stop(""Copilot CNI-MU LSK L2"", C_CNI.SELECT_2, 1171, devices.C_CNI)
elements[""PNT_CP_CNI_DISPL3_1172""] = cni_key_no_stop(""Copilot CNI-MU LSK L3"", C_CNI.SELECT_3, 1172, devices.C_CNI)
elements[""PNT_CP_CNI_DISPL4_1173""] = cni_key_no_stop(""Copilot CNI-MU LSK L4"", C_CNI.SELECT_4, 1173, devices.C_CNI)
elements[""PNT_CP_CNI_DISPL5_1174""] = cni_key_no_stop(""Copilot CNI-MU LSK L5"", C_CNI.SELECT_5, 1174, devices.C_CNI)
elements[""PNT_CP_CNI_DISPL6_1175""] = cni_key_no_stop(""Copilot CNI-MU LSK L6"", C_CNI.SELECT_6, 1175, devices.C_CNI)
elements[""PNT_CP_CNI_DISPR1_1176""] = cni_key_no_stop(""Copilot CNI-MU LSK R1"", C_CNI.SELECT_7, 1176, devices.C_CNI)
elements[""PNT_CP_CNI_DISPR2_1177""] = cni_key_no_stop(""Copilot CNI-MU LSK R2"", C_CNI.SELECT_8, 1177, devices.C_CNI)
elements[""PNT_CP_CNI_DISPR3_1178""] = cni_key_no_stop(""Copilot CNI-MU LSK R3"", C_CNI.SELECT_9, 1178, devices.C_CNI)
elements[""PNT_CP_CNI_DISPR4_1179""] = cni_key_no_stop(""Copilot CNI-MU LSK R4"", C_CNI.SELECT_10, 1179, devices.C_CNI)
elements[""PNT_CP_CNI_DISPR5_1180""] = cni_key_no_stop(""Copilot CNI-MU LSK R5"", C_CNI.SELECT_11, 1180, devices.C_CNI)
elements[""PNT_CP_CNI_DISPR6_1181""] = cni_key_no_stop(""Copilot CNI-MU LSK R6"", C_CNI.SELECT_12, 1181, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_COMM_1182""] = cni_key_no_stop(""Copilot CNI-MU COMM TUNE Key"", C_CNI.COMM_TUNE, 1182, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_NAV_1183""] = cni_key_no_stop(""Copilot CNI-MU NAV TUNE Key"", C_CNI.NAV_TUNE, 1183, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_IFF_1184""] = cni_key_no_stop(""Copilot CNI-MU IFF Key"", C_CNI.IFF, 1184, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_NAVCTRL_1185""] = cni_key_no_stop(""Copilot CNI-MU NAV CTRL Key"", C_CNI.NAV_CTRL, 1185, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_MSN_1186""] = cni_key_no_stop(""Copilot CNI-MU MSN Key"", C_CNI.MSN, 1186, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_EXEC_1187""] = cni_key_no_stop(""Copilot CNI-MU EXEC Key"", C_CNI.EXEC, 1187, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_CAPS_1188""] = cni_key_no_stop(""Copilot CNI-MU CAPS Key"", C_CNI.CAPS, 1188, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_MC_INDEX_1189""] = cni_key_no_stop(""Copilot CNI-MU MC INDX Key"", C_CNI.MC_INDX, 1189, devices.C_CNI)
elements[""PNT_CP_CNI_BRTUP_1190""] = cni_brt(""Copilot CNI-MU Brightness Increase"", C_CNI.BRT_UP, 1190, devices.C_CNI, 1)
elements[""PNT_CP_CNI_BRTDWN_1190""] = cni_brt(""Copilot CNI-MU Brightness Decrease"", C_CNI.BRT_DN, 1190, devices.C_CNI, 0 )
elements[""PNT_CPPNT_P_CNI_INDX_1191""] = cni_key_no_stop(""Copilot CNI-MU INDX Key"", C_CNI.INDEX, 1191, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_DIR_1192""] = cni_key_no_stop(""Copilot CNI-MU DIR INTC Key"", C_CNI.DIR, 1192, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_TOLD_1193""] = cni_key_no_stop(""Copilot CNI-MU TOLD Key"", C_CNI.TOLD, 1193, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_LEGS_1194""] = cni_key_no_stop(""Copilot CNI-MU LEGS Key"", C_CNI.LEGS, 1194, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_MARK_1195""] = cni_key_no_stop(""Copilot CNI-MU MARK Key"", C_CNI.MARK, 1195, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_PREV_1196""] = cni_key_no_stop(""Copilot CNI-MU PREV PAGE Key"", C_CNI.PREV, 1196, devices.C_CNI)
elements[""PNT_CPPNT_P_CNI_NEXT_1197""] = cni_key_no_stop(""Copilot CNI-MU NEXT PAGE Key"", C_CNI.NEXT, 1197, devices.C_CNI)
elements[""PNT_CP_CNI_1_1198""] = cni_key_no_stop(""Copilot CNI-MU 1 Key"", C_CNI.KBD_1, 1198, devices.C_CNI)
elements[""PNT_CP_CNI_2_1199""] = cni_key_no_stop(""Copilot CNI-MU 2 Key"", C_CNI.KBD_2, 1199, devices.C_CNI)
elements[""PNT_CP_CNI_3_1200""] = cni_key_no_stop(""Copilot CNI-MU 3 Key"", C_CNI.KBD_3, 1200, devices.C_CNI)
elements[""PNT_CP_CNI_4_1201""] = cni_key_no_stop(""Copilot CNI-MU 4 Key"", C_CNI.KBD_4, 1201, devices.C_CNI)
elements[""PNT_CP_CNI_5_1202""] = cni_key_no_stop(""Copilot CNI-MU 5 Key"", C_CNI.KBD_5, 1202, devices.C_CNI)
elements[""PNT_CP_CNI_6_1203""] = cni_key_no_stop(""Copilot CNI-MU 6 Key"", C_CNI.KBD_6, 1203, devices.C_CNI)
elements[""PNT_CP_CNI_7_1204""] = cni_key_no_stop(""Copilot CNI-MU 7 Key"", C_CNI.KBD_7, 1204, devices.C_CNI)
elements[""PNT_CP_CNI_8_1205""] = cni_key_no_stop(""Copilot CNI-MU 8 Key"", C_CNI.KBD_8, 1205, devices.C_CNI)
elements[""PNT_CP_CNI_9_1206""] = cni_key_no_stop(""Copilot CNI-MU 9 Key"", C_CNI.KBD_9, 1206, devices.C_CNI)
elements[""PNT_CP_CNI_DOT_1207""] = cni_key_no_stop(""Copilot CNI-MU Decimal Key"", C_CNI.KBD_DOT, 1207, devices.C_CNI)
elements[""PNT_CP_CNI_0_1208""] = cni_key_no_stop(""Copilot CNI-MU 0 Key"", C_CNI.KBD_0, 1208, devices.C_CNI)
elements[""PNT_CP_CNI_PLUSMINUS_1209""] = cni_key_no_stop(""Copilot CNI-MU Minus Key"", C_CNI.KBD_PLUSMINUS, 1209, devices.C_CNI)
elements[""PNT_CP_CNI_A_1210""] = cni_key_no_stop(""Copilot CNI-MU A Key"", C_CNI.KBD_A, 1210, devices.C_CNI)
elements[""PNT_CP_CNI_B_1211""] = cni_key_no_stop(""Copilot CNI-MU B Key"", C_CNI.KBD_B, 1211, devices.C_CNI)
elements[""PNT_CP_CNI_C_1212""] = cni_key_no_stop(""Copilot CNI-MU C Key"", C_CNI.KBD_C, 1212, devices.C_CNI)
elements[""PNT_CP_CNI_D_1213""] = cni_key_no_stop(""Copilot CNI-MU D Key"", C_CNI.KBD_D, 1213, devices.C_CNI)
elements[""PNT_CP_CNI_E_1214""] = cni_key_no_stop(""Copilot CNI-MU E Key"", C_CNI.KBD_E, 1214, devices.C_CNI)
elements[""PNT_CP_CNI_F_1215""] = cni_key_no_stop(""Copilot CNI-MU F Key"", C_CNI.KBD_F, 1215, devices.C_CNI)
elements[""PNT_CP_CNI_G_1216""] = cni_key_no_stop(""Copilot CNI-MU G Key"", C_CNI.KBD_G, 1216, devices.C_CNI)
elements[""PNT_CP_CNI_H_1217""] = cni_key_no_stop(""Copilot CNI-MU H Key"", C_CNI.KBD_H, 1217, devices.C_CNI)
elements[""PNT_CP_CNI_I_1218""] = cni_key_no_stop(""Copilot CNI-MU I Key"", C_CNI.KBD_I, 1218, devices.C_CNI)
elements[""PNT_CP_CNI_J_1219""] = cni_key_no_stop(""Copilot CNI-MU J Key"", C_CNI.KBD_J, 1219, devices.C_CNI)
elements[""PNT_CP_CNI_K_1220""] = cni_key_no_stop(""Copilot CNI-MU K Key"", C_CNI.KBD_K, 1220, devices.C_CNI)
elements[""PNT_CP_CNI_L_1221""] = cni_key_no_stop(""Copilot CNI-MU L Key"", C_CNI.KBD_L, 1221, devices.C_CNI)
elements[""PNT_CP_CNI_M_1222""] = cni_key_no_stop(""Copilot CNI-MU M Key"", C_CNI.KBD_M, 1222, devices.C_CNI)
elements[""PNT_CP_CNI_N_1223""] = cni_key_no_stop(""Copilot CNI-MU N Key"", C_CNI.KBD_N, 1223, devices.C_CNI)
elements[""PNT_CP_CNI_O_1224""] = cni_key_no_stop(""Copilot CNI-MU O Key"", C_CNI.KBD_O, 1224, devices.C_CNI)
elements[""PNT_CP_CNI_P_1225""] = cni_key_no_stop(""Copilot CNI-MU P Key"", C_CNI.KBD_P, 1225, devices.C_CNI)
elements[""PNT_CP_CNI_Q_1226""] = cni_key_no_stop(""Copilot CNI-MU Q Key"", C_CNI.KBD_Q, 1226, devices.C_CNI)
elements[""PNT_CP_CNI_R_1227""] = cni_key_no_stop(""Copilot CNI-MU R Key"", C_CNI.KBD_R, 1227, devices.C_CNI)
elements[""PNT_CP_CNI_S_1228""] = cni_key_no_stop(""Copilot CNI-MU S Key"", C_CNI.KBD_S, 1228, devices.C_CNI)
elements[""PNT_CP_CNI_T_1229""] = cni_key_no_stop(""Copilot CNI-MU T Key"", C_CNI.KBD_T, 1229, devices.C_CNI)
elements[""PNT_CP_CNI_U_1230""] = cni_key_no_stop(""Copilot CNI-MU U Key"", C_CNI.KBD_U, 1230, devices.C_CNI)
elements[""PNT_CP_CNI_V_1231""] = cni_key_no_stop(""Copilot CNI-MU V Key"", C_CNI.KBD_V, 1231, devices.C_CNI)
elements[""PNT_CP_CNI_W_1232""] = cni_key_no_stop(""Copilot CNI-MU W Key"", C_CNI.KBD_W, 1232, devices.C_CNI)
elements[""PNT_CP_CNI_X_1233""] = cni_key_no_stop(""Copilot CNI-MU X Key"", C_CNI.KBD_X, 1233, devices.C_CNI)
elements[""PNT_CP_CNI_Y_1234""] = cni_key_no_stop(""Copilot CNI-MU Y Key"", C_CNI.KBD_Y, 1234, devices.C_CNI)
elements[""PNT_CP_CNI_Z_1235""] = cni_key_no_stop(""Copilot CNI-MU Z Key"", C_CNI.KBD_Z, 1235, devices.C_CNI)
elements[""PNT_CP_CNI_DEL_1237""] = cni_key_no_stop(""Copilot CNI-MU DEL Key"", C_CNI.DEL, 1237, devices.C_CNI)
elements[""PNT_CP_CNI_MINUS_1236""] = cni_key_no_stop(""Copilot CNI-MU Unused Key"", C_CNI.KBD_SPACE, 1236, devices.C_CNI)
elements[""PNT_CP_CNI_CLR_1239""] = cni_key_no_stop(""Copilot CNI-MU CLR Key"", C_CNI.CLR, 1239, devices.C_CNI)
elements[""PNT_CP_CNI_SLASH_1238""] = cni_key_no_stop(""Copilot CNI-MU Slash Key"", C_CNI.KBD_SLASH, 1238, devices.C_CNI)

elements[""PNT_AUG_CNI_DISPL1_1240""] = cni_key_no_stop(""Aug Crew CNI-MU LSK L1"",  AC_CNI.SELECT_1, 1240, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPL2_1241""] = cni_key_no_stop(""Aug Crew CNI-MU LSK L2"",  AC_CNI.SELECT_2, 1241, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPL3_1242""] = cni_key_no_stop(""Aug Crew CNI-MU LSK L3"",  AC_CNI.SELECT_3, 1242, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPL4_1243""] = cni_key_no_stop(""Aug Crew CNI-MU LSK L4"",  AC_CNI.SELECT_4, 1243, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPL5_1244""] = cni_key_no_stop(""Aug Crew CNI-MU LSK L5"",  AC_CNI.SELECT_5, 1244, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPL6_1245""] = cni_key_no_stop(""Aug Crew CNI-MU LSK L6"",  AC_CNI.SELECT_6, 1245, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPR1_1246""] = cni_key_no_stop(""Aug Crew CNI-MU LSK R1"",  AC_CNI.SELECT_7, 1246, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPR2_1247""] = cni_key_no_stop(""Aug Crew CNI-MU LSK R2"",  AC_CNI.SELECT_8, 1247, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPR3_1248""] = cni_key_no_stop(""Aug Crew CNI-MU LSK R3"",  AC_CNI.SELECT_9, 1248, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPR4_1249""] = cni_key_no_stop(""Aug Crew CNI-MU LSK R4"",  AC_CNI.SELECT_10, 1249, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPR5_1250""] = cni_key_no_stop(""Aug Crew CNI-MU LSK R5"",  AC_CNI.SELECT_11, 1250, devices.AC_CNI)
elements[""PNT_AUG_CNI_DISPR6_1251""] = cni_key_no_stop(""Aug Crew CNI-MU LSK R6"",  AC_CNI.SELECT_12, 1251, devices.AC_CNI)
elements[""PNT_AUG_CNI_COMM_1252""] = cni_key_no_stop(""Aug Crew CNI-MU COMM TUNE Key"",  AC_CNI.COMM_TUNE, 1252, devices.AC_CNI)
elements[""PNT_AUG_CNI_NAV_1253""] = cni_key_no_stop(""Aug Crew CNI-MU NAV TUNE Key"",  AC_CNI.NAV_TUNE, 1253, devices.AC_CNI)
elements[""PNT_AUG_CNI_IFF_1254""] = cni_key_no_stop(""Aug Crew CNI-MU IFF Key"",  AC_CNI.IFF, 1254, devices.AC_CNI)
elements[""PNT_AUG_CNI_NAVCTRL_1255""] = cni_key_no_stop(""Aug Crew CNI-MU NAV CTRL Key"",  AC_CNI.NAV_CTRL, 1255, devices.AC_CNI)
elements[""PNT_AUG_CNI_MSN_1256""] = cni_key_no_stop(""Aug Crew CNI-MU MSN Key"",  AC_CNI.MSN, 1256, devices.AC_CNI)
elements[""PNT_AUG_CNI_DIR_1257""] = cni_key_no_stop(""Aug Crew CNI-MU DIR INTC Key"",  AC_CNI.DIR, 1257, devices.AC_CNI)
elements[""PNT_AUG_CNI_TOLD_1258""] = cni_key_no_stop(""Aug Crew CNI-MU TOLD Key"",  AC_CNI.TOLD, 1258, devices.AC_CNI)
elements[""PNT_AUG_CNI_INDX_1259""] = cni_key_no_stop(""Aug Crew CNI-MU INDX Key"",  AC_CNI.INDEX, 1259, devices.AC_CNI)
elements[""PNT_AUG_CNI_MC_INDEX_1260""] = cni_key_no_stop(""Aug Crew CNI-MU MC INDX Key"",  AC_CNI.MC_INDX, 1260, devices.AC_CNI)
elements[""PNT_AUG_CNI_CAPS_1261""] = cni_key_no_stop(""Aug Crew CNI-MU CAPS Key"",  AC_CNI.CAPS, 1261, devices.AC_CNI)
elements[""PNT_AUG_CNI_EXEC_1262""] = cni_key_no_stop(""Aug Crew CNI-MU EXEC Key"",  AC_CNI.EXEC, 1262, devices.AC_CNI)
elements[""PNT_AUG_CNI_BRTUP_1263""] = cni_brt(""Aug Crew CNI-MU Brightness Increase"",  AC_CNI.BRT_UP, 1263, devices.AC_CNI, 1)
elements[""PNT_AUG_CNI_BRTDWN_1263""] = cni_brt(""Aug Crew CNI-MU Brightness Decrease"",  AC_CNI.BRT_DN, 1263, devices.AC_CNI, 0)
elements[""PNT_AUG_CNI_LEGS_1264""] = cni_key_no_stop(""Aug Crew CNI-MU LEGS Key"",  AC_CNI.LEGS, 1264, devices.AC_CNI)
elements[""PNT_AUG_CNI_MARK_1265""] = cni_key_no_stop(""Aug Crew CNI-MU MARK Key"",  AC_CNI.MARK, 1265, devices.AC_CNI)
elements[""PNT_AUG_CNI_PREV_1266""] = cni_key_no_stop(""Aug Crew CNI-MU PREV PAGE Key"",  AC_CNI.PREV, 1266, devices.AC_CNI)
elements[""PNT_AUG_CNI_NEXT_1267""] = cni_key_no_stop(""Aug Crew CNI-MU NEXT PAGE Key"",  AC_CNI.NEXT, 1267, devices.AC_CNI)
elements[""PNT_AUG_CNI_1_1268""] = cni_key_no_stop(""Aug Crew CNI-MU 1 Key"",  AC_CNI.KBD_1, 1268, devices.AC_CNI)
elements[""PNT_AUG_CNI_2_1269""] = cni_key_no_stop(""Aug Crew CNI-MU 2 Key"",  AC_CNI.KBD_2, 1269, devices.AC_CNI)
elements[""PNT_AUG_CNI_3_1270""] = cni_key_no_stop(""Aug Crew CNI-MU 3 Key"",  AC_CNI.KBD_3, 1270, devices.AC_CNI)
elements[""PNT_AUG_CNI_4_1271""] = cni_key_no_stop(""Aug Crew CNI-MU 4 Key"",  AC_CNI.KBD_4, 1271, devices.AC_CNI)
elements[""PNT_AUG_CNI_5_1272""] = cni_key_no_stop(""Aug Crew CNI-MU 5 Key"",  AC_CNI.KBD_5, 1272, devices.AC_CNI)
elements[""PNT_AUG_CNI_6_1273""] = cni_key_no_stop(""Aug Crew CNI-MU 6 Key"",  AC_CNI.KBD_6, 1273, devices.AC_CNI)
elements[""PNT_AUG_CNI_7_1274""] = cni_key_no_stop(""Aug Crew CNI-MU 7 Key"",  AC_CNI.KBD_7, 1274, devices.AC_CNI)
elements[""PNT_AUG_CNI_8_1275""] = cni_key_no_stop(""Aug Crew CNI-MU 8 Key"",  AC_CNI.KBD_8, 1275, devices.AC_CNI)
elements[""PNT_AUG_CNI_9_1276""] = cni_key_no_stop(""Aug Crew CNI-MU 9 Key"",  AC_CNI.KBD_9, 1276, devices.AC_CNI)
elements[""PNT_AUG_CNI_DOT_1277""] = cni_key_no_stop(""Aug Crew CNI-MU Decimal Key"",  AC_CNI.KBD_DOT, 1277, devices.AC_CNI)
elements[""PNT_AUG_CNI_0_1278""] = cni_key_no_stop(""Aug Crew CNI-MU 0 Key"",  AC_CNI.KBD_0, 1278, devices.AC_CNI)
elements[""PNT_AUG_CNI_PLUSMINUS_1279""] = cni_key_no_stop(""Aug Crew CNI-MU Minus Key"",  AC_CNI.KBD_PLUSMINUS, 1279, devices.AC_CNI)
elements[""PNT_AUG_CNI_A_1280""] = cni_key_no_stop(""Aug Crew CNI-MU A Key"",  AC_CNI.KBD_A, 1280, devices.AC_CNI)
elements[""PNT_AUG_CNI_B_1281""] = cni_key_no_stop(""Aug Crew CNI-MU B Key"",  AC_CNI.KBD_B, 1281, devices.AC_CNI)
elements[""PNT_AUG_CNI_C_1282""] = cni_key_no_stop(""Aug Crew CNI-MU C Key"",  AC_CNI.KBD_C, 1282, devices.AC_CNI)
elements[""PNT_AUG_CNI_D_1283""] = cni_key_no_stop(""Aug Crew CNI-MU D Key"",  AC_CNI.KBD_D, 1283, devices.AC_CNI)
elements[""PNT_AUG_CNI_E_1284""] = cni_key_no_stop(""Aug Crew CNI-MU E Key"",  AC_CNI.KBD_E, 1284, devices.AC_CNI)
elements[""PNT_AUG_CNI_F_1285""] = cni_key_no_stop(""Aug Crew CNI-MU F Key"",  AC_CNI.KBD_F, 1285, devices.AC_CNI)
elements[""PNT_AUG_CNI_G_1286""] = cni_key_no_stop(""Aug Crew CNI-MU G Key"",  AC_CNI.KBD_G, 1286, devices.AC_CNI)
elements[""PNT_AUG_CNI_H_1287""] = cni_key_no_stop(""Aug Crew CNI-MU H Key"",  AC_CNI.KBD_H, 1287, devices.AC_CNI)
elements[""PNT_AUG_CNI_I_1288""] = cni_key_no_stop(""Aug Crew CNI-MU I Key"",  AC_CNI.KBD_I, 1288, devices.AC_CNI)
elements[""PNT_AUG_CNI_J_1289""] = cni_key_no_stop(""Aug Crew CNI-MU J Key"",  AC_CNI.KBD_J, 1289, devices.AC_CNI)
elements[""PNT_AUG_CNI_K_1290""] = cni_key_no_stop(""Aug Crew CNI-MU K Key"",  AC_CNI.KBD_K, 1290, devices.AC_CNI)
elements[""PNT_AUG_CNI_L_1291""] = cni_key_no_stop(""Aug Crew CNI-MU L Key"",  AC_CNI.KBD_L, 1291, devices.AC_CNI)
elements[""PNT_AUG_CNI_M_1292""] = cni_key_no_stop(""Aug Crew CNI-MU M Key"",  AC_CNI.KBD_M, 1292, devices.AC_CNI)
elements[""PNT_AUG_CNI_N_1293""] = cni_key_no_stop(""Aug Crew CNI-MU N Key"",  AC_CNI.KBD_N, 1293, devices.AC_CNI)
elements[""PNT_AUG_CNI_O_1294""] = cni_key_no_stop(""Aug Crew CNI-MU O Key"",  AC_CNI.KBD_O, 1294, devices.AC_CNI)
elements[""PNT_AUG_CNI_P_1295""] = cni_key_no_stop(""Aug Crew CNI-MU P Key"",  AC_CNI.KBD_P, 1295, devices.AC_CNI)
elements[""PNT_AUG_CNI_Q_1296""] = cni_key_no_stop(""Aug Crew CNI-MU Q Key"",  AC_CNI.KBD_Q, 1296, devices.AC_CNI)
elements[""PNT_AUG_CNI_R_1297""] = cni_key_no_stop(""Aug Crew CNI-MU R Key"",  AC_CNI.KBD_R, 1297, devices.AC_CNI)
elements[""PNT_AUG_CNI_S_1298""] = cni_key_no_stop(""Aug Crew CNI-MU S Key"",  AC_CNI.KBD_S, 1298, devices.AC_CNI)
elements[""PNT_AUG_CNI_T_1299""] = cni_key_no_stop(""Aug Crew CNI-MU T Key"",  AC_CNI.KBD_T, 1299, devices.AC_CNI)
elements[""PNT_AUG_CNI_U_1300""] = cni_key_no_stop(""Aug Crew CNI-MU U Key"",  AC_CNI.KBD_U, 1300, devices.AC_CNI)
elements[""PNT_AUG_CNI_V_1301""] = cni_key_no_stop(""Aug Crew CNI-MU V Key"",  AC_CNI.KBD_V, 1301, devices.AC_CNI)
elements[""PNT_AUG_CNI_W_1302""] = cni_key_no_stop(""Aug Crew CNI-MU W Key"",  AC_CNI.KBD_W, 1302, devices.AC_CNI)
elements[""PNT_AUG_CNI_X_1303""] = cni_key_no_stop(""Aug Crew CNI-MU X Key"",  AC_CNI.KBD_X, 1303, devices.AC_CNI)
elements[""PNT_AUG_CNI_Y_1304""] = cni_key_no_stop(""Aug Crew CNI-MU Y Key"",  AC_CNI.KBD_Y, 1304, devices.AC_CNI)
elements[""PNT_AUG_CNI_Z_1305""] = cni_key_no_stop(""Aug Crew CNI-MU Z Key"",  AC_CNI.KBD_Z, 1305, devices.AC_CNI)
elements[""PNT_AUG_CNI_MINUS_1306""] = cni_key_no_stop(""Aug Crew CNI-MU Unused Key"",  AC_CNI.KBD_SPACE, 1306, devices.AC_CNI)
elements[""PNT_AUG_CNI_DEL_1307""] = cni_key_no_stop(""Aug Crew CNI-MU DEL Key"",  AC_CNI.DEL, 1307, devices.AC_CNI)
elements[""PNT_AUG_CNI_SLASH_1308""] = cni_key_no_stop(""Aug Crew CNI-MU Slash Key"",  AC_CNI.KBD_SLASH, 1308, devices.AC_CNI)
elements[""PNT_AUG_CNI_CLR_1309""] = cni_key_no_stop(""Aug Crew CNI-MU CLR Key"",  AC_CNI.CLR, 1309, devices.AC_CNI)
elements[""PNT_WIPERS_323""] =  wiper(""Windshield Wiper Control Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.wipers, 323, {-0.2, 0.8}, 0.2, 
    nil, nil, ENGINE_ELEC_APU_CTRL.wipers_stop)

----------------------------------------------------------------------Remote_heading_course_select_panel
-- ------------------------------------------------------------------------ pilot
elements[""PNT_P_HEADING_490_562""] = knob_360_press(""Pilot Heading Adjust - Push to Sync"",PILOT_COCKPIT_INTERFACE.HDG_SET, 490, 1.0, PILOT_COCKPIT_INTERFACE.HDG_RESET, devices.PILOT_CPT_INTERFACE, 562)
elements[""PNT_P_COURSE_491_563""] = knob_360_press(""Pilot Course Adjust - Push to Sync"", PILOT_COCKPIT_INTERFACE.CRS_SET, 491, 1.0, PILOT_COCKPIT_INTERFACE.CRS_RESET, devices.PILOT_CPT_INTERFACE, 563)
-- -- ------------------------------------------------------------------------ copilot
elements[""PNT_CP_HEADING_492_564""] = knob_360_press(""Copilot Heading Adjust - Push to Sync"", COPILOT_COCKPIT_INTERFACE.HDG_SET, 492, 1.0, COPILOT_COCKPIT_INTERFACE.HDG_RESET, devices.COPILOT_CPT_INTERFACE, 564)
elements[""PNT_CP_COURSE_493_565""] = knob_360_press(""Copilot Course Adjust - Push to Sync"", COPILOT_COCKPIT_INTERFACE.CRS_SET, 493, 1.0, COPILOT_COCKPIT_INTERFACE.CRS_RESET, devices.COPILOT_CPT_INTERFACE, 565)

------------------------------------------------------------------------- COPILOT_LIGHTING_PANEL

-------------------------------------------------------------------------------------------REF_MODE_PANEL
-------------------------------------------------------------------------- pilot
elements[""PNT_L_REF_SELECT_110""] = rotary(""Pilot Reference Select Switch"", devices.PILOT_REF_MODE_PANEL, REF_MODE_PANEL.ref_select, 110, {-1, 1}, 0.4, 6, nil)
elements[""PNT_L_REF_SET_109_556""] = knob_360_press(""Pilot Reference Set Knob - Push to Hide/Show"", REF_MODE_PANEL.ref_set, 109, 1.0, REF_MODE_PANEL.ref_set_press, devices.PILOT_REF_MODE_PANEL, 556)
elements[""PNT_L_REF_ALTSEL_108_557""] = knob_360_press(""Pilot Altitude Alerter Set Knob - Push to Sync"", REF_MODE_PANEL.alt_alert_set, 108, 1.0, REF_MODE_PANEL.alt_alert_reset, devices.PILOT_REF_MODE_PANEL, 557)
elements[""PNT_L_REF_BAROSEL_107_558""] = knob_360_press(""Pilot Baro Set Knob - Push to Set 29.92"", REF_MODE_PANEL.baro_set, 107, 1.0, REF_MODE_PANEL.baro_ISA_set, devices.PILOT_REF_MODE_PANEL, 558)
elements[""PNT_L_MASTERWARNING_80""] = master_warning(""Pilot Master Warning Button - Push to Reset"", devices.PILOT_REF_MODE_PANEL, REF_MODE_PANEL.master_warning, 80)
elements[""PNT_L_MASTERCAUTION_81""] = master_caution(""Pilot Master Caution Button - Push to Reset"", devices.PILOT_REF_MODE_PANEL, REF_MODE_PANEL.master_caution, 81)
elements[""PNT_L_MODE_ALT_82""] = ref_btn(""Pilot ALT Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.alt_hold, 82)
elements[""PNT_L_MODE_SEL_84""] = ref_btn(""Pilot SEL Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.alt_sel, 84)
elements[""PNT_L_MODE_HDG_86""] = ref_btn(""Pilot HDG Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.hdg_sel, 86)
elements[""PNT_L_MODE_NAV_88""] = ref_btn(""Pilot NAV Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.nav_sel, 88)
elements[""PNT_L_MODE_APPR_90""] = ref_btn(""Pilot APPR Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.appr_sel, 90)
elements[""PNT_L_MODE_VS_83""] = ref_btn(""Pilot VS Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.vs_hold_sel, 83)
elements[""PNT_L_MODE_IAS_85""] = ref_btn(""Pilot IAS Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.ias_sel, 85)
elements[""PNT_L_AP_ON_OFF_87""] = ref_btn(""Pilot Unused Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.blank_sel, 87)
elements[""PNT_L_MODE_CAPS_89""] = ref_btn(""Pilot CAPS Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.caps_sel, 89)
elements[""PNT_L_MODE_AT_91""] = ref_btn(""Pilot A/T Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.autothrottle_sel, 91)
elements[""PNT_R_MASTERWARNING_92""] = master_warning(""Copilot Master Warning Button – Push to Reset"" , devices.COPILOT_REF_MODE_PANEL, REF_MODE_PANEL.master_warning, 92)
elements[""PNT_R_MASTERCAUTION_93""] = master_caution(""Copilot Master Caution Button – Push to Reset"" , devices.COPILOT_REF_MODE_PANEL, REF_MODE_PANEL.master_caution, 93)
elements[""PNT_R_MODE_ALT_94""] = ref_btn(""Copilot ALT Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_alt_hold, 94)
elements[""PNT_R_MODE_SEL_96""] = ref_btn(""Copilot SEL Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_alt_sel, 96)
elements[""PNT_R_MODE_HDG_98""] = ref_btn(""Copilot HDG Mode Switch"", devices.AP_INTERFACE, PILOT_AP.cp_hdg_sel, 98)
elements[""PNT_R_MODE_NAV_100""] = ref_btn(""Copilot NAV Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_nav_sel, 100)
elements[""PNT_R_MODE_APPR_102""] = ref_btn(""Copilot APPR Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_appr_sel, 102)
elements[""PNT_R_MODE_VS_95""] = ref_btn(""Copilot VS Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_vs_hold_sel, 95)
elements[""PNT_R_MODE_IAS_97""] = ref_btn(""Copilot IAS Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_ias_sel, 97)
elements[""PNT_R_AP_ON_OFF_99""] = ref_btn(""Copilot Unused Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_blank_sel, 99)
elements[""PNT_R_MODE_CAPS_101""] = ref_btn(""Copilot CAPS Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_caps_sel, 101)
elements[""PNT_R_MODE_AT_103""] = ref_btn(""Copilot A/T Mode Switch"" , devices.AP_INTERFACE, PILOT_AP.cp_autothrottle_sel, 103)
elements[""PNT_AFCS_LAT_73""] = ref_btn(""AFCS Lateral Axis Deselect Switch"" , devices.AP_INTERFACE, PILOT_AP.afcs_lat, 73)
elements[""PNT_AFCS_PITCH_51""] = ref_btn(""AFCS Pitch Axis Deselect Switch"", devices.AP_INTERFACE, PILOT_AP.afcs_pitch, 51)

elements[""PNT_R_REF_SELECT_111""] = rotary(""Copilot Reference Select Switch"", devices.COPILOT_REF_MODE_PANEL, REF_MODE_PANEL.ref_select, 111, {-1,1}, 0.4, 6, nil)
elements[""PNT_R_REF_SET_106_559""] = knob_360_press(""Copilot Reference Set Knob – Push to Hide/Show"", REF_MODE_PANEL.ref_set, 106, 1.0, REF_MODE_PANEL.ref_set_press, devices.COPILOT_REF_MODE_PANEL, 559)
elements[""PNT_R_REF_ALTSEL_105_560""] = knob_360_press(""Copilot Altitude Alerter Set Knob - Push to Sync"", REF_MODE_PANEL.alt_alert_set,  105, 1.0, REF_MODE_PANEL.alt_alert_reset, devices.COPILOT_REF_MODE_PANEL, 560)
elements[""PNT_R_REF_BAROSEL_104_561""] = knob_360_press(""Copilot Baro Set Knob - Push to Set 29.92"", REF_MODE_PANEL.baro_set, 104, 1.0, REF_MODE_PANEL.baro_ISA_set, devices.COPILOT_REF_MODE_PANEL, 561)

elements[""PNT_AFCS_PITCH_KNOB_71""] = knob_360_0_1(""AFCS Pitch Control Wheel"", PILOT_AP.afcs_pitch_knob, 71, -0.15, devices.AP_INTERFACE)
elements[""PNT_AFCS_TURN_KNOB_70""] = knob_fixed(""AFCS Turn Control Knob / Center Detent"", PILOT_AP.afcs_lat_knob, 70, 0.1, devices.AP_INTERFACE, {-1, 1}, true, PILOT_AP.afcs_reset_wheel_lat)
elements[""PNT_AFCS_P_52""] = two_pos_switch_ap(""Pilot AFCS Engage Switch"", devices.AP_INTERFACE, PILOT_AP.afcs_toggle_p, 52, 8)
elements[""PNT_AFCS_CP_53""] = two_pos_switch_ap(""Copilot AFCS Engage Switch"", devices.AP_INTERFACE, PILOT_AP.afcs_toggle_cp, 53, 8)

---------------------------------------------------------------------------------------------ICS_PANEL
elements[""PNT_P_ISCP_VOL_1355""] = knob_rot(""Pilot Master Volume Knob"", devices.VOLUME_MANAGER, VOL_CTRL.master_vol, 1355)
elements[""PNT_P_ISCP_VOXSENS_1354""] = knob_rot(""Pilot VOX SENS Knob"", devices.VOLUME_MANAGER, VOL_CTRL.vox_sense_vol, 1354)
elements[""PNT_P_ISCP_INT_204_205""] = ics_knob(""Pilot INT Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.int_pull, VOL_CTRL.int_vol, 204, 205, 0.5, 1)
elements[""PNT_P_ISCP_H1_206_207""] = ics_knob(""Pilot HF 1 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,     VOL_CTRL.h1_pull,VOL_CTRL.h1_vol,  206, 207,  0.5, 1)
elements[""PNT_P_ISCP_H2_208_209""] = ics_knob(""Pilot HF 2 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,     VOL_CTRL.h2_pull,VOL_CTRL.h2_vol,  208, 209,  0.5, 1)
elements[""PNT_P_ISCP_PA_210_211""] = ics_knob(""Pilot PA Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,      VOL_CTRL.pa_pull,VOL_CTRL.pa_vol,  210, 211,  0.5, 1)
elements[""PNT_P_ISCP_V1_212_213""] = ics_knob(""Pilot VHF 1 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,    VOL_CTRL.v1_pull,VOL_CTRL.v1_vol,  212,  213,  0.5, 1)
elements[""PNT_P_ISCP_V2_214_215""] = ics_knob(""Pilot VHF 2 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,    VOL_CTRL.v2_pull,VOL_CTRL.v2_vol,  214, 215,  0.5 , 1)
elements[""PNT_P_ISCP_SATCOM_216_217""] = ics_knob(""Pilot SAT Volume Knob – Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.sat_pull,VOL_CTRL.sat_vol,  216, 217,  0.5, 1)
elements[""PNT_P_ISCP_PVT_218_219""] = ics_knob(""Pilot PVT Volume Knob – Pull to Monitor"", devices.VOLUME_MANAGER,    VOL_CTRL.pvt_pull,VOL_CTRL.pvt_vol,  218, 219,  0.5 , 1)
elements[""PNT_P_ISCP_VOXHM_220_221""] = ics_knob(""Pilot VOX/HM Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,      VOL_CTRL.vox_hm_pull,VOL_CTRL.vox_hm_vol,  220, 221,  0.5, 1)
elements[""PNT_P_ISCP_U1_222_223""] = ics_knob(""Pilot UHF 1 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,    VOL_CTRL.u1_pull,VOL_CTRL.u1_vol,  222, 223,  0.5, 1)
elements[""PNT_P_ISCP_U2_224_225""] = ics_knob(""Pilot UHF 2 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,    VOL_CTRL.u2_pull,VOL_CTRL.u2_vol,  224, 225,  0.5, 1)
elements[""PNT_P_ISCP_INT_291""] = one_way_rocker(""Pilot Intercom Transmit"", devices.VOLUME_MANAGER, VOL_CTRL.int_radio_toggle, 291, -1)
elements[""PNT_P_ISCP_RADIO_291""] = one_way_rocker(""Pilot Radio Transmit"", devices.VOLUME_MANAGER, VOL_CTRL.int_radio_toggle, 291, 1)
elements[""PNT_P_ISCP_MODE_293""] = rotary(""Pilot Interphone Mode Switch"", devices.VOLUME_MANAGER, VOL_CTRL.mic_mode, 293, {0, 1}, 1/3, 4, 1)
elements[""PNT_P_ISCP_SELECTOR_294""] = rotary(""Pilot Transmission Selector Switch"", devices.VOLUME_MANAGER, VOL_CTRL.trans_mode, 294, {(-1/9)*2, 0.8}, 1/9, 2, 1)
elements[""PNT_P_ISCP_PAGAIN_1353""] = knob_rot(""Pilot PA GAIN Knob"",devices.VOLUME_MANAGER,   VOL_CTRL.pa_gain_vol,  1353)

elements[""PNT_CP_ICS_VOL_1358""] = knob_rot(""Copilot Master Volume Knob"", devices.VOLUME_MANAGER, VOL_CTRL.cp_master_vol, 1358)
elements[""PNT_CP_ICS_VOXSENS_1357""] = knob_rot(""Copilot VOX SENS Knob"", devices.VOLUME_MANAGER, VOL_CTRL.vox_sense_vol_cp, 1357)
elements[""PNT_CP_ICS_INT_226_227""] = ics_knob(""Copilot INT Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER,   VOL_CTRL.int_pull_cp, VOL_CTRL.int_vol_cp,  226, 227, 0.5, 0)
elements[""PNT_CP_ICS_H1_228_229""] = ics_knob(""Copilot HF 1 Volume Knob - Pull to Monitor"",devices.VOLUME_MANAGER,          VOL_CTRL.h1_pull_cp, VOL_CTRL.h1_vol_cp,    228, 229, 0.5, 0)
elements[""PNT_CP_ICS_H2_230_231""] = ics_knob(""Copilot HF 2 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,          VOL_CTRL.h2_pull_cp, VOL_CTRL.h2_vol_cp,    230, 231, 0.5, 2)
elements[""PNT_CP_ICS_PA_232_233""] = ics_knob(""Copilot PA Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,           VOL_CTRL.pa_pull_cp, VOL_CTRL.pa_vol_cp,    232, 233, 0.5, 2)
elements[""PNT_CP_ICS_V1_234_235""] = ics_knob(""Copilot VHF 1 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,         VOL_CTRL.v1_pull_cp, VOL_CTRL.v1_vol_cp,    234, 235,  0.5, 2)
elements[""PNT_CP_ICS_V2_236_237""] = ics_knob(""Copilot VHF 2 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,         VOL_CTRL.v2_pull_cp, VOL_CTRL.v2_vol_cp,    236, 237, 0.5 , 2)
elements[""PNT_CP_ICS_SATCOM_238_239""] = ics_knob(""Copilot SAT Volume Knob – Pull to Monitor"", devices.VOLUME_MANAGER,      VOL_CTRL.sat_pull_cp, VOL_CTRL.sat_vol_cp,  238, 239, 0.5, 2)
elements[""PNT_CP_ICS_PVT_240_241""] = ics_knob(""Copilot PVT Volume Knob – Pull to Monitor"", devices.VOLUME_MANAGER,         VOL_CTRL.pvt_pull_cp, VOL_CTRL.pvt_vol_cp,  240, 241, 0.5 , 2)
elements[""PNT_CP_ICS_VOXHM_242_243""] = ics_knob(""Copilot VOX/HM Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,           VOL_CTRL.vox_hm_pull_cp, VOL_CTRL.vox_hm_vol_cp, 242, 243, 0.5, 2)
elements[""PNT_CP_ICS_U1_244_245""] = ics_knob(""Copilot UHF 1 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,         VOL_CTRL.u1_pull_cp, VOL_CTRL.u1_vol_cp,    244, 245, 0.5, 2)
elements[""PNT_CP_ICS_U2_246_247""] = ics_knob(""Copilot UHF 2 Volume Knob – Pull to Monitor"",devices.VOLUME_MANAGER,         VOL_CTRL.u2_pull_cp, VOL_CTRL.u2_vol_cp,    246, 247, 0.5, 2)
elements[""PNT_CP_ICS_INT_292""] = one_way_rocker(""Copilot Intercom Transmit"", devices.VOLUME_MANAGER, VOL_CTRL.int_radio_toggle_cp, 292, -1)
elements[""PNT_CP_ICS_RADIO_292""] = one_way_rocker(""Copilot Radio Transmit"", devices.VOLUME_MANAGER, VOL_CTRL.int_radio_toggle_cp, 292, 1)
elements[""PNT_CP_ISCP_MODE_295""] = rotary(""Copilot Interphone Mode Switch"", devices.VOLUME_MANAGER, VOL_CTRL.mic_mode_cp, 295, {0, 1}, 1/3, 4, 6)
elements[""PNT_CP_ICS_SELECTOR_296""] = rotary(""Copilot Transmission Selector Switch"", devices.VOLUME_MANAGER, VOL_CTRL.trans_mode_cp, 296, {(-1/9)*2, 1}, 1/9, 2, 2)
elements[""PNT_CP_ICS_PAGAIN_1356""] = knob_rot(""Copilot PA GAIN Knob"",devices.VOLUME_MANAGER,   VOL_CTRL.pa_gain_vol_cp,  1356)


elements[""PNT_CCP_SELECTOR_65""] = multiswitch(""Cursor Priority Switch"", devices.MECH_INTERFACE,  MECH_INTERFACE.cni_pri_toggle, 65, {0.0, 1}, 0.5)
elements[""PNT_CCP_RESET_68""] = cni_key_no_stop(""Cursor Reset Switch"", MECH_INTERFACE.cursor_reset, 68, devices.MECH_INTERFACE)
elements[""PNT_CCP_HUD_69""] = cni_key_no_stop(""HUD Cursor Off/On Switch"", MECH_INTERFACE.cursor_hud, 69, devices.MECH_INTERFACE)
elements[""PNT_CCP_RANGE_UP_66""] = one_way_rocker(""Display Range Increase"", devices.MECH_INTERFACE, MECH_INTERFACE.cursor_rng, 66, 1)
elements[""PNT_CCP_RANGE_DN_66""] = one_way_rocker(""Display Range Decrease"", devices.MECH_INTERFACE, MECH_INTERFACE.cursor_rng, 66, -1)
elements[""PNT_CCP_ZOOM_UP_67""] = one_way_rocker(""Display Zoom Increase"", devices.MECH_INTERFACE, MECH_INTERFACE.cursor_zoom, 67, 1)
elements[""PNT_CCP_ZOOM_DN_67""] = one_way_rocker(""Display Zoom Decrease"", devices.MECH_INTERFACE, MECH_INTERFACE.cursor_zoom, 67, -1)
elements[""PNT_CCP_DISPLAY_72""] = rotary(""Cursor Display Select Switch"", devices.MECH_INTERFACE,  MECH_INTERFACE.cursor_hdd, 72, {-0.163, 0.83}, 1/6, 4)

elements[""PNT_TILT_483""] = rocker_centering(""Cursor - Tilt"", devices.MECH_INTERFACE, MECH_INTERFACE.tilt, 483)

elements[""PNT_RCP_PSEL_405""] = push_button(""Radar PSEL Mode Switch"", devices.NAV_RADAR, NAV_RADAR.psel, 405)
elements[""PNT_RCP_PRCN_398""] = push_button(""Radar PRCN Mode Switch"", devices.NAV_RADAR, NAV_RADAR.prcn, 398)
elements[""PNT_RCP_MAP_399""] = push_button(""Radar MAP Mode Switch"", devices.NAV_RADAR, NAV_RADAR.map_mode, 399)
elements[""PNT_RCP_WX_400""] = push_button(""Radar WX Mode Switch"", devices.NAV_RADAR, NAV_RADAR.wx_mode, 400)
elements[""PNT_RCP_SP_401""] = push_button(""Radar SP Mode Switch"", devices.NAV_RADAR, NAV_RADAR.sp_mode, 401)
elements[""PNT_RCP_MGM_402""] = push_button(""Radar MGM Mode Switch"", devices.NAV_RADAR, NAV_RADAR.msn_mode, 402)
elements[""PNT_RCP_WS_403""] = push_button(""Radar WS Mode Switch"", devices.NAV_RADAR, NAV_RADAR.ws_mode, 403)
elements[""PNT_RCP_BCN_404""] = push_button(""Radar BCN Mode Switch"", devices.NAV_RADAR, NAV_RADAR.bcn_mode, 404)
elements[""PNT_RCP_OFS_406""] = push_button(""Radar OFS Mode Switch"", devices.NAV_RADAR, NAV_RADAR.ofs, 406)
elements[""PNT_RCP_FRZ_407""] = push_button(""Radar FRZ Mode Switch"", devices.NAV_RADAR, NAV_RADAR.frz, 407)
elements[""PNT_RCP_PEN_408""] = push_button(""Radar PEN Mode Switch"", devices.NAV_RADAR, NAV_RADAR.pen, 408)
elements[""PNT_RCP_SCTR_409""] = push_button(""Radar SCTR Mode Switch"", devices.NAV_RADAR, NAV_RADAR.sctr, 409)

elements[""PNT_RCP_INTENSITY_INC_UP_410""] = one_way_rocker(""Radar Intensity Increase"", devices.NAV_RADAR, NAV_RADAR.intensity, 410, 1)
elements[""PNT_RCP_INTENSITY_INC_DN_410""] = one_way_rocker(""Radar Intensity Decrease"", devices.NAV_RADAR, NAV_RADAR.intensity, 410, -1)
elements[""PNT_RCP_GAIN_INC_UP_411""] = one_way_rocker(""Radar Gain Increase"", devices.NAV_RADAR, NAV_RADAR.gain, 411, 1)
elements[""PNT_RCP_GAIN_INC_DN_411""] = one_way_rocker(""Radar Gain Decrease"", devices.NAV_RADAR, NAV_RADAR.gain, 411, -1)
elements[""PNT_RCP_MASTER_485""] = rotary(""Radar Master Power Switch"", devices.NAV_RADAR, NAV_RADAR.master_mode, 485, {0, 1}, 0.5, 6)
elements[""PNT_INTENSITY_SEL_486""] = rotary(""Intensity Target Select Switch"", devices.NAV_RADAR, NAV_RADAR.intensity_mode, 486, {0, 1}, 1/3, 6)

-- ---------------------------------------------------------------------------------------------------------Standby instruments
elements[""PNT_STBY_BARO_KNB_125""] = stby_altim(""Standby Altimeter Baro Adjust"", MECH_INTERFACE.baro_offset, 125, 1, -1, devices.MECH_INTERFACE)

elements[""PNT_STBY_CAGE_127_128""]     = adi_cage(""Standby Attitude Indicator Bias Adjust - Pull to Cage"", MECH_INTERFACE.pitch_bar, 127, 0.1, MECH_INTERFACE.pitch_bar_cage,    devices.MECH_INTERFACE, 128)

-- elements[""PNT_FLAP_LVR_15_16""] = default_2_position_tumb(""Flaps Up/Down"", devices.MECH_INTERFACE, MECH_INTERFACE.flap_lever, nil, 4, 1.0, 0)
elements[""PNT_FLAP_LVR_15_16""] = flap_switch(""Flap Control Lever"", devices.MECH_INTERFACE, MECH_INTERFACE.flap_axis, 16)
-- elements[""PNT_FLAP_LVR_15_16""] = multiswitch(""Flaps Up/Down"", devices.MECH_INTERFACE, MECH_INTERFACE.flap_lever, 16, {0, 1}, 1/9, 1/4)

-- elements[""PNT_FLAP_LVR_15_16""] = flap_switch(""Flaps Up/Down"", devices.MECH_INTERFACE, MECH_INTERFACE.flap_lever, 16)
------------------------------------------------------------------ HUD_PANELS
------------------------------------------------------------------ pilot
elements[""PNT_P_HUD_VIS_1311""] = hud_btn(""Pilot HUD Visual Mode"",  devices.P_DISPLAYS, P_AMU.vis_disp_mode, 1311)
elements[""PNT_P_HUD_TACT_1318""] = hud_btn(""Pilot HUD Tactical Mode"", devices.P_DISPLAYS,  P_AMU.tac_data_layer, 1318)
elements[""PNT_HUD_ARG_006""] = hud_latch(""Pilot HUD Latch"", devices.P_DISPLAYS, P_AMU.hud_latch, 6, 0.75, 1)
elements[""PNT_P_HUD_BRT_1319_1320""] = hud_brt_knob(""Pilot HUD Brightness Control Knob - Pull for Auto"", devices.P_DISPLAYS,
    P_AMU.brightness_knob, 1319, P_AMU.brightness_knob_auto, 1320)
elements[""PNT_P_HUD_CAT2_1313""] = hud_btn(""Pilot HUD CAT2 Mode"",  devices.P_DISPLAYS, P_AMU.CAT2_sub_mode, 1313)
elements[""PNT_P_HUD_O/S_1314""] = hud_btn(""Pilot HUD Offside Mode"",  devices.P_DISPLAYS, P_AMU.offside_switch, 1314)
elements[""PNT_P_HUD_NAV_1316""] = hud_btn(""Pilot HUD Nav Mode"",  devices.P_DISPLAYS, P_AMU.nav_data_layer, 1316)
elements[""PNT_P_HUD_UNCG_1315""] = hud_btn(""Pilot HUD Uncage Mode"",  devices.P_DISPLAYS, P_AMU.FPM_cage, 1315)
elements[""PNT_P_HUD_BANK1_1312""] = hud_btn("",  devices.P_DISPLAYS, P_AMU.blank1, 1312)
elements[""PNT_P_HUD_BLANK2_1317""] = hud_btn("",  devices.P_DISPLAYS, P_AMU.blank2, 1317)
---------------------------------------------------------------------------- copilot
elements[""PNT_HUD_ARG_007""] = hud_latch(""Copilot HUD Latch"", devices.C_DISPLAYS, C_AMU.hud_latch , 7,  0.75, 2)
elements[""PNT_CP_HUD_BRT_1330_1331""] = hud_brt_knob(""Copilot HUD Brightness Control Knob - Pull for Auto"",  devices.C_DISPLAYS,
    C_AMU.brightness_knob, 1331, C_AMU.brightness_knob_auto, 1330)
elements[""PNT_CP_HUD_VIS_1322""] = hud_btn(""Copilot HUD Visual Mode"",  devices.C_DISPLAYS, C_AMU.vis_disp_mode, 1322)
elements[""PNT_CP_HUD_CAT2_1324""] = hud_btn(""Copilot HUD CAT2 Mode"",  devices.C_DISPLAYS, C_AMU.CAT2_sub_mode, 1324)
elements[""PNT_CP_HUD_O/S_1325""] = hud_btn(""Copilot HUD Offside Mode"",  devices.C_DISPLAYS, C_AMU.offside_switch, 1325)
elements[""PNT_CP_HUD_TACT_1329""] = hud_btn(""Copilot HUD Tactical Mode"",  devices.C_DISPLAYS, C_AMU.tac_data_layer, 1329)
elements[""PNT_CP_HUD_NAV_1327""] = hud_btn(""Copilot HUD Navigation Mode"",  devices.C_DISPLAYS, C_AMU.nav_data_layer, 1327)
elements[""PNT_CP_HUD_UNCG_1326""] = hud_btn(""Copilot HUD Uncage Mode"",  devices.C_DISPLAYS, C_AMU.FPM_cage, 1326)
elements[""PNT_CP_HUD_BANK1_1323""] = hud_btn("",  devices.C_DISPLAYS, C_AMU.blank1, 1323)
elements[""PNT_CP_HUD_BLANK2_1328""] = hud_btn("",  devices.C_DISPLAYS, C_AMU.blank2, 1328)

elements[""PNT_P_MON_VOR1_427_428""] = ics_knob(""Pilot VOR 1 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.vor1_pull, VOL_CTRL.vor1, 427, 428, 0.5)
elements[""PNT_P_MON_TACAN1_429_430""] = ics_knob(""Pilot TACAN 1 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.tacan1_pull, VOL_CTRL.tacan1, 429, 430, 0.5)
elements[""PNT_P_MON_ADF1_431_432""] = ics_knob(""Pilot ADF 1 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.adf1_pull, VOL_CTRL.adf1, 431, 432, 0.5)
elements[""PNT_P_MON_SAR_433_434""] = ics_knob(""Pilot SAR Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.sar_pull, VOL_CTRL.sar, 433, 434, 0.5)
elements[""PNT_P_MON_BCN_435_436""] = ics_knob(""Pilot BCN Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.bcn_pull, VOL_CTRL.bcn, 435, 436, 0.5)
elements[""PNT_P_MON_VOR2_437_438""] = ics_knob(""Pilot VOR 2 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.vor2_pull, VOL_CTRL.vor2, 437, 438, 0.5)
elements[""PNT_P_MON_TACAN2_439_440""] = ics_knob(""Pilot TACAN 2 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.tacan2_pull, VOL_CTRL.tacan2, 439, 440, 0.5)
elements[""PNT_P_MON_ADF2_441_442""] = ics_knob(""Pilot ADF 2 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.adf2_pull, VOL_CTRL.adf2, 441, 442, 0.5)
elements[""PNT_P_MON_SPARE_443_444""] = ics_knob(""Pilot Unused Volume Knob"", devices.VOLUME_MANAGER,    VOL_CTRL.blank_pull, VOL_CTRL.blank,    443, 444, 0.5)
elements[""PNT_P_MON_RWR_445_446""] = ics_knob(""Pilot RWR Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER,    VOL_CTRL.rwr_pull, VOL_CTRL.rwr,    445, 446, 0.5)

elements[""PNT_CP_MON_VOR1_447_448""] = ics_knob(""Copilot VOR 1 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.vor1_pull_cp, VOL_CTRL.vor1_cp, 447, 448, 0.5)
elements[""PNT_CP_MON_TACAN1_449_450""] = ics_knob(""Copilot TACAN 1 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.tacan1_pull_cp, VOL_CTRL.tacan1_cp, 449, 450, 0.5)
elements[""PNT_CP_MON_ADF1_451_452""] = ics_knob(""Copilot ADF 1 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.adf1_pull_cp, VOL_CTRL.adf1_cp, 451, 452, 0.5)
elements[""PNT_CP_MON_SAR_453_454""] = ics_knob(""Copilot SAR Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.sar_pull_cp, VOL_CTRL.sar_cp, 453, 454, 0.5)
elements[""PNT_CP_MON_BCN_455_456""] = ics_knob(""Copilot BCN Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.bcn_pull_cp, VOL_CTRL.bcn_cp, 455, 456, 0.5)
elements[""PNT_CP_MON_VOR2_457_458""] = ics_knob(""Copilot VOR 2 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.vor2_pull_cp, VOL_CTRL.vor2_cp, 457, 458, 0.5)
elements[""PNT_CP_MON_TACAN2_459_460""] = ics_knob(""Copilot TACAN 2 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.tacan2_pull_cp, VOL_CTRL.tacan2_cp, 459, 460, 0.5)
elements[""PNT_CP_MON_ADF2_461_462""] = ics_knob(""Copilot ADF 2 Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER, VOL_CTRL.adf2_pull_cp, VOL_CTRL.adf2_cp, 461, 462, 0.5)
elements[""PNT_CP_MON_SPARE_463_464""] = ics_knob(""Copilot Unused Volume Knob"", devices.VOLUME_MANAGER,    VOL_CTRL.blank_pull_cp, VOL_CTRL.blank_cp,    463, 464, 0.5)
elements[""PNT_CP_MON_RWR_465_466""] = ics_knob(""Copilot RWR Volume Knob - Pull to Monitor"", devices.VOLUME_MANAGER,    VOL_CTRL.rwr_pull_cp, VOL_CTRL.rwr_cp,    465, 466, 0.5)

elements[""PNT_AUG_CTR_ICS_VOL_1361""] = knob_rot(""Aug ICS - Volume Master"", devices.VOLUME_MANAGER, VOL_CTRL.aug_master_vol, 1361)
elements[""PNT_AUG_CTR_ICS_VOXSENS_1360""] = knob_rot(""Aug ICS - VOX Sensitivity"", devices.VOLUME_MANAGER, VOL_CTRL.vox_sense_vol_aug, 1360)
elements[""PNT_AUG_CTR_ICS_INT_268_269""] = ics_knob(""AUG ICS - Int Volume"",devices.VOLUME_MANAGER,     VOL_CTRL.int_pull_aug, VOL_CTRL.int_vol_aug,  268, 269,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_H1_270_271""] = ics_knob(""AUG ICS - H1 Volume"",devices.VOLUME_MANAGER,     VOL_CTRL.h1_pull_aug, VOL_CTRL.h1_vol_aug,  270, 271,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_H2_272_273""] = ics_knob(""AUG ICS - H2 Volume"",devices.VOLUME_MANAGER,     VOL_CTRL.h2_pull_aug, VOL_CTRL.h2_vol_aug,  272, 273,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_PA_274_275""] = ics_knob(""AUG ICS - PA Volume"",devices.VOLUME_MANAGER,     VOL_CTRL.pa_pull_aug, VOL_CTRL.pa_vol_aug,  274, 275,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_PAGAIN_1359""] = knob_rot(""AUG ICS - PA Gain"",devices.VOLUME_MANAGER,   VOL_CTRL.pa_gain_vol_aug,  1359)
elements[""PNT_AUG_CTR_ICS_V1_276_277""] = ics_knob(""AUG ICS - V1 Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.v1_pull_aug, VOL_CTRL.v1_vol_aug,  276, 277,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_V2_278_279""] = ics_knob(""AUG ICS - V2 Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.v2_pull_aug, VOL_CTRL.v2_vol_aug,  278, 279,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_SATCOM_280_281""] = ics_knob(""AUG ICS - Sat Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.sat_pull_aug, VOL_CTRL.sat_vol_aug,  280, 281,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_PVT_282_283""] = ics_knob(""AUG ICS - PVT Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.pvt_pull_aug, VOL_CTRL.pvt_vol_aug,  282, 283,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_VOXHM_284_285""] = ics_knob(""AUG ICS - HM Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.vox_hm_pull_aug, VOL_CTRL.vox_hm_vol_aug,  284, 285,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_U1_286_287""] = ics_knob(""AUG ICS - U1 Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.u1_pull_aug, VOL_CTRL.u1_vol_aug,  286, 287,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_U2_288_289""] = ics_knob(""AUG ICS - U2 Volume"", devices.VOLUME_MANAGER,  VOL_CTRL.u2_pull_aug, VOL_CTRL.u2_vol_aug,  288, 289,  0.5, 1)
elements[""PNT_AUG_CTR_ICS_INT_290""] = one_way_rocker(""AUG ICS - Intercom"", devices.VOLUME_MANAGER, VOL_CTRL.int_radio_toggle_aug, 290, -1)
elements[""PNT_AUG_CTR_ICS_RADIO_290""] = one_way_rocker(""AUG ICS - Radio"", devices.VOLUME_MANAGER, VOL_CTRL.int_radio_toggle_aug, 290, 1)
elements[""PNT_AUG_CTR_ICS_MODE_297""] = rotary(""AUG ICS - Mic Mode"", devices.VOLUME_MANAGER, VOL_CTRL.mic_mode_aug, 297, {0, 1}, 1/3, 25, 1)
elements[""PNT_AUG_CTR_ICS_SELECTOR_298""] = rotary(""AUG ICS - Transmit Selector"", devices.VOLUME_MANAGER, VOL_CTRL.trans_mode_aug, 298, {(-1/9)*2, 1}, 1/9, 6, 2)
elements[""PNT_AUG_CTR_MON_VOR1_248_249""] = ics_knob(""Aug Mon - VOR 1 Volume"", devices.VOLUME_MANAGER, VOL_CTRL.vor1_pull_aug, VOL_CTRL.vor1_aug, 248, 249, 0.5)
elements[""PNT_AUG_CTR_MON_TACAN1_250_251""] = ics_knob(""Aug Mon - TACAN 1 Volume"", devices.VOLUME_MANAGER, VOL_CTRL.tacan1_pull_aug, VOL_CTRL.tacan1_aug, 250, 251, 0.5)
elements[""PNT_AUG_CTR_MON_ADF1_252_253""] = ics_knob(""Aug Mon - ADF 1 Volume"", devices.VOLUME_MANAGER, VOL_CTRL.adf1_pull_aug, VOL_CTRL.adf1_aug, 252, 253, 0.5)
elements[""PNT_AUG_CTR_MON_SAR_254_255""] = ics_knob(""Aug Mon - SAR Volume"", devices.VOLUME_MANAGER, VOL_CTRL.sar_pull_aug, VOL_CTRL.sar_aug, 254, 255, 0.5)
elements[""PNT_AUG_CTR_MON_BCN_256_257""] = ics_knob(""Aug Mon - BCN Volume"", devices.VOLUME_MANAGER, VOL_CTRL.bcn_pull_aug, VOL_CTRL.bcn_aug, 256, 257, 0.5)
elements[""PNT_AUG_CTR_MON_VOR2_258_259""] = ics_knob(""Aug Mon - VOR 2 Volume"", devices.VOLUME_MANAGER, VOL_CTRL.vor2_pull_aug, VOL_CTRL.vor2_aug, 258, 259, 0.5)
elements[""PNT_AUG_CTR_MON_TACAN2_260_261""] = ics_knob(""Aug Mon - TACAN 2 Volume"", devices.VOLUME_MANAGER, VOL_CTRL.tacan2_pull_aug, VOL_CTRL.tacan2_aug, 260, 261, 0.5)
elements[""PNT_AUG_CTR_MON_ADF2_262_263""] = ics_knob(""Aug Mon - ADF 2 Volume"", devices.VOLUME_MANAGER, VOL_CTRL.adf2_pull_aug, VOL_CTRL.adf2_aug, 262, 263, 0.5)
elements[""PNT_AUG_CTR_MON_SPARE_264_265""] = ics_knob(""Aug Mon - Spare"", devices.VOLUME_MANAGER,    VOL_CTRL.blank_pull_aug, VOL_CTRL.blank_aug,    264, 265, 0.5)
elements[""PNT_AUG_CTR_MON_RWR_266_267""] = ics_knob(""Aug Mon - RWR"", devices.VOLUME_MANAGER,    VOL_CTRL.rwr_pull_aug, VOL_CTRL.rwr_aug,    266, 267, 0.5)

elements[""PNT_DSC_CMDS_COVER_59""] = guard(""CMS Jettison Switch Guard"",      devices.CMS_MGR,  CMS.emerg_jett_cover, 59, 6)
elements[""PNT_DSC_EMERG_JETT_60""] = two_pos_switch_spring(""CMS Jettison Switch"" , devices.CMS_MGR,  CMS.emerg_jett, 60)
elements[""PNT_DSC_CMDS_MASTER_61""] = two_pos_switch(""Defensive Systems Master Switch"", devices.CMS_MGR,         CMS.arm_toggle_cms, 61, 15)
elements[""PNT_DSC_CMDS_ECM_62""] = two_pos_switch(""ECM Master Switch"",          devices.CMS_MGR,         CMS.mws_arm, 62, 15)
elements[""PNT_DSC_CMDS_IRCM_63""] = two_pos_switch(""IRCM Master Switch"",        devices.CMS_MGR,         CMS.ircm_toggle, 63, 15)
elements[""PNT_DSC_CMDS_PRGM_64""] = multiswitch(""MAN PRGMS Switch"",        devices.CMS_MGR,         CMS.man_prog, 64, {0, 1}, 0.5)
elements[""PNT_DSC_CMDS_MODE_74""] = multiswitch(""CMDS Mode Selector"", devices.CMS_MGR, CMS.cms_profile_mode, 74, {0, 1}, 0.25)
elements[""PNT_DSC_SRCH_54""] = push_button(""RWR SRCH Switch"", devices.CMS_MGR, CMS.rwr_search, 54)
elements[""PNT_DSC_MODE_55""] = push_button(""RWR MODE Switch"", devices.CMS_MGR, CMS.rwr_track, 55)
elements[""PNT_DSC_HANDOFF_56""] = push_button(""RWR HANDOFF Switch"", devices.CMS_MGR, CMS.rwr_handoff, 56)
elements[""PNT_DSC_ALT_57""] = push_button(""RWR ALT Switch"", devices.CMS_MGR, CMS.rwr_alt, 57)
elements[""PNT_DSC_TGT_SEP_58""] = push_button(""RWR TGT SEP Switch"", devices.CMS_MGR, CMS.rwr_sep, 58)

elements[""PNT_P_DISPLAYMASTER_1335""] = knob_rot(""Pilot Master Display Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.display_master, 1335, 0.25)
elements[""PNT_CP_MASTER_1349""] = knob_rot(""Copilot Master Display Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.copilot_display_master, 1349, 0.25)
elements[""PNT_PLT_DOME_1340""]  = knob_rot(""Cockpit Dome Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.dome_light, 1340)
elements[""PNT_CP_OVH_PANEL_1347""]  = knob_rot(""Overhead Panel Backlighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.overhead, 1347)
elements[""PNT_CP_OVH_FLOOD_1346""]  = knob_rot(""Overhead Panel Flood Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ovh_flood, 1346)
elements[""PNT_PLT_PANEL_1342""]  = knob_rot(""Pilot Panel Backlighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.pilot_side_panel, 1342)
elements[""PNT_PLT_FLOOD_1343""]  = knob_rot(""Pilot Panel Flood Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.pilot_side_flood, 1343)
elements[""PNT_CP_SI_PANEL_1350""]  = knob_rot(""Copilot Panel Backlighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.copilot_side_panel, 1350)
elements[""PNT_CP_SI_FLOOD_1351""]  = knob_rot(""Copilot Panel Flood Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.copilot_side_flood, 1351)
elements[""PNT_P_MASTER_LTE_MODE_1337""]  = multiswitch(""Lighting Mode Master Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.master_mode, 1337, {-1, 1}, 1)

elements[""PNT_PLT_CKT_BRK_1341""]  = knob_rot(""Pilot Circuit Breaker Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.p_circuit_breaker, 1341)
elements[""PNT_PLT_FLOOR_1344""]  = knob_rot(""Floor Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.floor, 1344)
elements[""PNT_CP_CKT_BRK_1345""]  = knob_rot(""Copilot Circuit Breaker Lighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.cp_circuit_breaker, 1345)
elements[""PNT_CP_CTRCNSL_1348""]  = knob_rot(""Center Console Backlighting Brightness Knob"", devices.LIGHTING_PANELS, LIGHTING_PANELS.cp_ctrl_cnsl, 1348)
elements[""PNT_CP_LAMP_TEST_1352""]  = three_pos_switch_spring(""Display/Lamp Test Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.cp_lamp_test, 1352)
elements[""PNT_P_ANN_1336""]  = three_pos_switch_spring(""Annunciator Light Brightness Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ann_dim_brt, 1336)

elements[""PNT_ELEC_BTRY_371""] = two_pos_switch_rev(""Battery Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.battery, 371, 8)
elements[""PNT_ELECT_EXT_APU_467""] = rotary(""External Power/APU Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.ext_elec, 467, {-1, 1}, 1, 12)
elements[""PNT_BATT_TEST_383""] = three_pos_switch_spring(""Battery Test Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.bat_test, 383)

elements[""PNT_FUEL_QTY_SET_487""] = knob_rot_rel(""Fuel Transfer Quantity Set Knob"", devices.FUEL_SYSTEM, FUEL.qty_set, 487,  0.1)
elements[""PNT_FUEL_TRANSF1_356""] = fuel_transfer(""Left Auxiliary Tank Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_ext_1, 356, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF2_357""] = fuel_transfer(""Main Tank 1 Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_main_1, 357, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF3_358""] = fuel_transfer(""Main Tank 2 Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_main_2, 358, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF4_359""] = fuel_transfer(""Left Auxiliary Tank Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_aux_1, 359, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF5_361""] = fuel_transfer(""Right External Tank Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_aux_2, 361, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF6_363""] = fuel_transfer(""Main Tank 3 Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_main_3, 363, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF7_364""] = fuel_transfer(""Main Tank 4 Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_main_4, 364, {-1, 1}, 1, 8)
elements[""PNT_FUEL_TRANSF8_365""] = fuel_transfer(""Right External Tank Transfer Pump Selector"", devices.FUEL_SYSTEM, FUEL.transfer_ext_2, 365, {-1, 1}, 1, 8)

elements[""PNT_FUEL_MAIN_XFEED_360""] = fuel_xfeed(""Crosship Separation Valve Switch"", devices.FUEL_SYSTEM, FUEL.main_xfeed, 360, 8)
elements[""PNT_FUEL_XFEED_ENG1_366""] = fuel_xfeed(""Engine 1 Crossfeed Valve Switch"", devices.FUEL_SYSTEM, FUEL.xfeed_eng1, 366, 8)
elements[""PNT_FUEL_XFEED_ENG2_367""] = fuel_xfeed(""Engine 2 Crossfeed Valve Switch"", devices.FUEL_SYSTEM, FUEL.xfeed_eng2, 367, 8)
elements[""PNT_FUEL_XFEED_ENG3_368""] = fuel_xfeed(""Engine 3 Crossfeed Valve Switch"", devices.FUEL_SYSTEM, FUEL.xfeed_eng3, 368, 8)
elements[""PNT_FUEL_XFEED_ENG4_369""] = fuel_xfeed(""Engine 4 Crossfeed Valve Switch"", devices.FUEL_SYSTEM, FUEL.xfeed_eng4, 369, 8)

elements[""PNT_FUEL_DUMP_L_GUARD_332""] = guard(""Left Fuel Dump Switch Guard"",  devices.FUEL_SYSTEM,  FUEL.dump_guard_left, 332, 6)
elements[""PNT_FUEL_DUMP_R_GUARD_333""] = guard(""Right Fuel Dump Switch Guard"", devices.FUEL_SYSTEM, FUEL.dump_guard_right, 333, 6)
elements[""PNT_FUEL_DUMP_R_1339""] = two_pos_switch(""Right Fuel Dump Switch"", devices.FUEL_SYSTEM, FUEL.dump_right, 1339)
elements[""PNT_FUEL_DUMP_L_1338""] = two_pos_switch(""Left Fuel Dump Switch"", devices.FUEL_SYSTEM, FUEL.dump_left, 1338)

elements[""PNT_FUEL_TANKSEL_370""] = fuel_transfer(""Fuel Tank Select"", devices.FUEL_SYSTEM, FUEL.tank_select, 370, {0,1}, 1/8)
elements[""PNT_SPR_VALVE_362""] = three_pos_spring_load_on(""SPR Valve Switch"", devices.FUEL_SYSTEM, FUEL.spr_valve, 362)
elements[""PNT_FUEL_TEST_354""] = cni_key_no_stop(""FLCV Test Button"", FUEL.test, 354, devices.FUEL_SYSTEM)

elements[""PNT_ATCS_GUARD_327""] = guard(""ATCS Switch Guard"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.atcs_guard, 327, 6)
elements[""PNT_ATCS_416""] = two_pos_switch(""ATCS Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.atcs, 416)

elements[""PNT_FADEC_GUARD_1_328""] = guard(""Engine 1 FADEC Switch Guard"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.guard_1, 328, nil, true)
elements[""PNT_FADEC_GUARD_2_329""] = guard(""Engine 2 FADEC Switch Guard"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.guard_2, 329, nil, true)
elements[""PNT_FADEC_GUARD_3_330""] = guard(""Engine 3 FADEC Switch Guard"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.guard_3, 330, nil, true)
elements[""PNT_FADEC_GUARD_4_331""] = guard(""Engine 4 FADEC Switch Guard"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.guard_4, 331, nil, true)

elements[""PNT_FADEC_1_412""] = three_pos_switch_spring(""Engine 1 FADEC Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.fadec_1, 412)
elements[""PNT_FADEC_2_413""] = three_pos_switch_spring(""Engine 2 FADEC Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.fadec_2, 413)
elements[""PNT_FADEC_3_414""] = three_pos_switch_spring(""Engine 3 FADEC Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.fadec_3, 414)
elements[""PNT_FADEC_4_415""] = three_pos_switch_spring(""Engine 4 FADEC Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.fadec_4, 415)

elements[""PNT_PROP_CNTRL_1_372""] = three_pos_spring_load_on_inv(""Propeller 1 Control Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.prop_ctrl_1, 372)
elements[""PNT_PROP_CNTRL_2_373""] = three_pos_spring_load_on_inv(""Propeller 2 Control Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.prop_ctrl_2, 373)
elements[""PNT_PROP_CNTRL_3_374""] = three_pos_spring_load_on_inv(""Propeller 3 Control Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.prop_ctrl_3, 374)
elements[""PNT_PROP_CNTRL_4_375""] = three_pos_spring_load_on_inv(""Propeller 4 Control Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.prop_ctrl_4, 375)

elements[""PNT_PROPSYNC_376""] = air_deflector(""Prop Sync Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.prop_sync, 376, 5)

elements[""PNT_FIRE_HANDLE_APU_324_325""] = fire_pull(""APU Fire Handle"",  devices.ENGINE_APU_CTRL,  ENGINE_ELEC_APU_CTRL.apu_fire_pull, ENGINE_ELEC_APU_CTRL.apu_fire, 324, 325, 1)
elements[""PNT_APU_ALARM_425""] = two_pos_switch(""APU Alarm Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.apu_alarm, 425)
elements[""PNT_EMEREXITLT_377""] = emg_ext_light(""Emergency Exit Light Extinguish Button"",  ENGINE_ELEC_APU_CTRL.emerg_xlt, 377, devices.ENGINE_APU_CTRL)
elements[""PNT_APU_CONTROL_322""] = multiswitch_stop(""APU Start Switch"", devices.ENGINE_APU_CTRL, ENGINE_ELEC_APU_CTRL.apu_selector, 322, {0, 1}, 0.5, ENGINE_ELEC_APU_CTRL.apu_select_stop)

--------------------------------------------- External Lighting Panel

elements[""PNT_EXTLT_COVERT_424""] = knob_rot(""Covert/Formation Light Brightness Control"", devices.LIGHTING_PANELS, LIGHTING_PANELS.covert_form_brt, 424, 0.25)
elements[""PNT_EXTLT_MASTER_421""] = two_pos_switch(""Exterior Lighting Master Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_master, 421, 8)
elements[""PNT_EXTLT_NAVMODE_422""] = multiswitch(""Navigation Light Mode Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_nav, 422, {-1, 1}, 1, 12)
elements[""PNT_EXTLT_DIM_423""] = two_pos_switch(""Navigation Light Brightness Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_dim, 423, 8)
elements[""PNT_EXTLT_STROBETOP_418""] = multiswitch(""Top Strobe Light Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_strobe_top, 418, {-1, 1}, 1, 12)
elements[""PNT_EXTLT_STROBEBOTTOM_419""] = multiswitch(""Bottom Strobe Light Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_strobe_btm, 419, {-1, 1}, 1, 12)
elements[""PNT_EXTLT_STROBETEST_420""] = two_pos_switch_spring(""Bottom Strobe Light Test Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_strobe_test, 420, 20)
elements[""PNT_EXTLT_LEDGE_417""] = two_pos_switch(""Leading Edge Light Switch"", devices.LIGHTING_PANELS, LIGHTING_PANELS.ext_ledge, 417, 8)

elements[""P_CABINET_BOX_73""] = two_pos_switch(""Cabinet Open/Close"", devices.MECH_INTERFACE, MECH_INTERFACE.open_close_cabinet, 73, 3)
elements[""PNT_RUDDER_TRIM_075""] = rudder_trim(""Rudder Trim Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.rudder_trim, 75)
elements[""PNT_TRIM_ELEV_1334""] = multiswitch(""Elevator Trim Tab Power Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.elev_trim, 1334, {-1,1}, 1)

elements[""PNT_PRES_MODE_468""] = rotary(""Pressurization Mode Switch"", devices.PLANE_ATM, AIRCOND.press_mode, 468, {-0.5, 0.5}, 0.25, 6)
elements[""PNT_PRESS_AUTO_RATE_1333""] = knob_rot(""Pressurization Rate Control Knob"", devices.PLANE_ATM, AIRCOND.auto_rate, 1333, 0.15)
elements[""PNT_PRESS_LND_ALT_1332""] = knob_rot_rel(""Landing/Constant Altitude Selector"", devices.PLANE_ATM, AIRCOND.landing_press, 1332, 0.1)

elements[""PNT_AIRCOND_FLTSTA_PWR_UP_346""] = one_way_rocker(""Flight Station Temperature Increase"", devices.PLANE_ATM, AIRCOND.flt_temp, 346, 1)
elements[""PNT_AIRCOND_FLTSTA_PWR_DN_346""] = one_way_rocker(""Flight Station Temperature Decrease"", devices.PLANE_ATM, AIRCOND.flt_temp, 346, -1)

elements[""PNT_AIRCOND_CARGO_UP_347""] = one_way_rocker(""Cargo Compartment Temperature Increase"", devices.PLANE_ATM, AIRCOND.cargo_temp, 347, 1)
elements[""PNT_AIRCOND_CARGO_DN_347""] = one_way_rocker(""Cargo Compartment Temperature Decrease"", devices.PLANE_ATM, AIRCOND.cargo_temp, 347, -1)

elements[""PNT_AIRCOND_CROSSFLOW_UP_348""] = one_way_rocker(""Crossflow Valve Open"", devices.PLANE_ATM, AIRCOND.cross_flow_valve_open, 348, 1)
elements[""PNT_AIRCOND_CROSSFLOW_DN_348""] = one_way_rocker(""Crossflow Valve Close"", devices.PLANE_ATM, AIRCOND.cross_flow_valve_open, 348, -1)

elements[""PNT_AIRCOND_UNDERFLOOR_469""] = rotary(""Underfloor Heat/Fan Switch"", devices.PLANE_ATM, AIRCOND.underfloor, 469, {-1, 1}, 1.0, 12)
elements[""PNT_AIRCOND_CROSSFLOW_MAN_349""] = push_button(""Crossflow Valve Manual Mode Switch"", devices.PLANE_ATM, AIRCOND.cross_flow_value_man , 349)
elements[""PNT_AIRCOND_FLTSTA_MAN_350""] = push_button(""Flight Station Manual Mode Switch"", devices.PLANE_ATM, AIRCOND.flt_man, 350)
elements[""PNT_AIRCOND_CARGO_MAN_351""] = push_button(""Cargo Compartment Manual Mode Switch"", devices.PLANE_ATM, AIRCOND.cargo_man, 351)
elements[""PNT_AIRCOND_FLTSTA_PWR_352""] = push_button(""Flight Station Air Conditioning Power Switch"", devices.PLANE_ATM, AIRCOND.flt_power, 352)
elements[""PNT_AIRCOND_CARGO_PWR_353""] = push_button(""Cargo Compartment Air Conditioning Power Switch"", devices.PLANE_ATM, AIRCOND.cargo_power, 353)

elements[""PNT_PRESS_MANUAL_VALVE_OPEN_345""] = one_way_rocker(""Outflow Valve Open"", devices.PLANE_ATM, AIRCOND.press_valve_man_open, 345, 1)
elements[""PNT_PRESS_MANUAL_VALVE_CLOSE_345""] = one_way_rocker(""Outflow Valve Close"", devices.PLANE_ATM, AIRCOND.press_valve_man_open, 345, -1)

elements[""PNT_EMER_DUMP_GUARD_334""] = guard(""Emergency Depressurize Switch Guard"", devices.PLANE_ATM, AIRCOND.emerg_press_dump_guard, 334)
elements[""PNT_EMER_DUMP_1363""] = two_pos_switch(""Emergency Depressurize Switch"", devices.PLANE_ATM, AIRCOND.emerg_press_dump, 1363)

elements[""PNT_ADP_RAMP_479""] = three_pos_spring_load_on(""Ramp/Door Control Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.ramp_ctrl, 479)
elements[""PNT_DROP_WAIT_397""] = push_button(""Airdrop Caution Light Control Button"", devices.MECH_INTERFACE, MECH_INTERFACE.airdrop_caution, 397)
elements[""PNT_DROP_JUMP_326""] = push_button(""Airdrop Jump Light Control Button"", devices.MECH_INTERFACE, MECH_INTERFACE.airdrop_jump, 326)
elements[""PNT_ADP_COVER_78""] = guard(""Cargo Bay Alarm Switch Guard"", devices.MECH_INTERFACE, MECH_INTERFACE.airdrop_alarm_cover, 78)
elements[""PNT_ADP_ALARM_79""] = two_pos_switch(""Cargo Bay Alarm Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.airdrop_alarm, 79)

elements[""PNT_CHUTE_BUTTON_77""] = push_button(""Chute Release Button"", devices.MECH_INTERFACE, MECH_INTERFACE.chute_release, 77)
elements[""PNT_CHUTE_COVER_76""] = two_pos_switch(""Chute Release Button Cover"", devices.MECH_INTERFACE, MECH_INTERFACE.chute_release_cover, 76)
elements[""PNT_ADP_DEFLECTORS_478""] = air_deflector(""Air Deflector Control Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.air_defelector, 478, 6)
elements[""PNT_ADP_COMPDROP_474""] = multiswitch(""Computer Drop Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.computer_drop, 474, {0, 1}, 0.5)

elements[""PNT_PARKING_BRAKE_29""] = parking_brake(""Parking Brake Handle"", devices.MECH_INTERFACE, MECH_INTERFACE.parking_brake, 29)

elements[""PNT_OIL_FLAP_1_495""] = oil_flap_switch(""Engine 1 Oil Cooler Flap Fixed"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_1_fixed)
elements[""PNT_OIL_FLAP_1_OPEN""] = oil_flap_switch_open_close(""Engine 1 Oil Cooler Flap Open"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_1_open)
elements[""PNT_OIL_FLAP_1_CLOSE""] = oil_flap_switch_open_close(""Engine 1 Oil Cooler Flap Close"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_1_close)
elements[""PNT_OIL_FLAP_1_AUTO""] = oil_flap_switch(""Engine 1 Oil Cooler Flap Auto"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_1_auto)

elements[""PNT_OIL_FLAP_2_496""] = oil_flap_switch(""Engine 2 Oil Cooler Flap Fixed"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_2_fixed)
elements[""PNT_OIL_FLAP_2_OPEN""] = oil_flap_switch_open_close(""Engine 2 Oil Cooler Flap Open"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_2_open)
elements[""PNT_OIL_FLAP_2_CLOSE""] = oil_flap_switch_open_close(""Engine 2 Oil Cooler Flap Close"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_2_close)
elements[""PNT_OIL_FLAP_2_AUTO""] = oil_flap_switch(""Engine 2 Oil Cooler Flap Auto"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_2_auto)

elements[""PNT_OIL_FLAP_3_497""] = oil_flap_switch(""Engine 3 Oil Cooler Flap Fixed"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_3_fixed)
elements[""PNT_OIL_FLAP_3_OPEN""] = oil_flap_switch_open_close(""Engine 3 Oil Cooler Flap Open"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_3_open)
elements[""PNT_OIL_FLAP_3_CLOSE""] = oil_flap_switch_open_close(""Engine 3 Oil Cooler Flap Close"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_3_close)
elements[""PNT_OIL_FLAP_3_AUTO""] = oil_flap_switch(""Engine 3 Oil Cooler Flap Auto"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_3_auto)

elements[""PNT_OIL_FLAP_4_498""] = oil_flap_switch(""Engine 4 Oil Cooler Flap Fixed"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_4_fixed)
elements[""PNT_OIL_FLAP_4_OPEN""] = oil_flap_switch_open_close(""Engine 4 Oil Cooler Flap Open"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_4_open)
elements[""PNT_OIL_FLAP_4_CLOSE""] = oil_flap_switch_open_close(""Engine 4 Oil Cooler Flap Close"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_4_close)
elements[""PNT_OIL_FLAP_4_AUTO""] = oil_flap_switch(""Engine 4 Oil Cooler Flap Auto"", devices.MECH_INTERFACE, MECH_INTERFACE.oil_flap_4_auto)

elements[""PNT_P_OXYGEN_EMERG_507""] = three_pos_spring_load_on_inv(""Pilot Oxygen Emergency Lever"", devices.PLANE_ATM, AIRCOND.emerg_pilot_regulator, 507)
elements[""PNT_P_OXYGEN_SUPPLY_509""] = oxygen_switch(""Pilot Oxygen Supply Lever"", devices.PLANE_ATM, AIRCOND.pilot_oxy_supply, 509, {-1, 1}, 2.0, 6)
elements[""PNT_P_OXYGEN_PERC_508""] = oxygen_switch(""Pilot Oxygen Diluter Lever"", devices.PLANE_ATM, AIRCOND.pilot_regulator_pct, 508, {-1, 1}, 2.0, 6)

elements[""PNT_CP_OXYGEN_EMERG_193""] = three_pos_spring_load_on_inv(""Copilot Oxygen Emergency Lever"", devices.PLANE_ATM, AIRCOND.emerg_copilot_regulator, 193)
elements[""PNT_CP_OXYGEN_PERC_192""] = oxygen_switch(""Copilot Oxygen Supply Lever"", devices.PLANE_ATM, AIRCOND.copilot_oxy_supply, 192, {-1, 1}, 2.0, 6)
elements[""PNT_CP_OXYGEN_SUPPLY_191""] = oxygen_switch(""Copilot Oxygen Diluter Lever"", devices.PLANE_ATM, AIRCOND.copilot_regulator_pct, 191, {-1, 1}, 2.0, 6)

elements[""PNT_DOOR_RIGHT""] = two_pos_switch(""Trooper Door - Right"", devices.MECH_INTERFACE, MECH_INTERFACE.trooper_door_right, 5000, 1.0)
elements[""PNT_DOOR_LEFT""] = two_pos_switch(""Trooper Door - Left"", devices.MECH_INTERFACE, MECH_INTERFACE.trooper_door_left, 5001, 1.0)

elements[""PNT_TOILET""] = two_pos_switch(""Toilet"", devices.MECH_INTERFACE, MECH_INTERFACE.toilet, 5002, 10)

elements[""PNT_PUMP_ON""] = two_pos_switch(""Aux Pump On/Off"", devices.HYDRAULICS, HYD_SYSTEM.cargo_aux_pump_on, 7010)
elements[""PNT_CLOSE_DOOR""] = two_pos_switch(""Door Open/Close"", devices.MECH_INTERFACE, MECH_INTERFACE.cargo_aux_pump_door, 7011)
elements[""PNT_RAISE_RAMP""] = two_pos_switch(""Ramp Open Close"", devices.MECH_INTERFACE, MECH_INTERFACE.cargo_aux_pump_ramp, 7012)

elements[""PNT_P_VISOR_RAIL_1612""] = glare_activate(""Pilot Sun Visor - Show/Hide"", devices.PILOT_CPT_INTERFACE, 
    PILOT_COCKPIT_INTERFACE.shade_show_hide, 1612,
    PILOT_COCKPIT_INTERFACE.shade_rotate, 1610)

elements[""PNT_CP_VISOR_RAIL_1613""] = glare_activate(""Coilot Sun Visor - Show/Hide"", devices.COPILOT_CPT_INTERFACE, 
    COPILOT_COCKPIT_INTERFACE.shade_show_hide, 1613,
    COPILOT_COCKPIT_INTERFACE.shade_rotate, 1611)

elements[""PNT_MICROW_START_1651""] = microwave_key(""Microwave - Start"", GALLEY.micro_start, 1651, devices.GALLEY)
elements[""PNT_MICROW_CLEAR_1652""] = microwave_key(""Microwave - Clear"", GALLEY.micro_clear, 1652, devices.GALLEY)
elements[""PNT_MICROW_1_1653""] = microwave_key(""Microwave - 1"", GALLEY.micro_1, 1653, devices.GALLEY)
elements[""PNT_MICROW_2_1654""] = microwave_key(""Microwave - 2"", GALLEY.micro_2, 1654, devices.GALLEY)
elements[""PNT_MICROW_3_1655""] = microwave_key(""Microwave - 3"", GALLEY.micro_3, 1655, devices.GALLEY)
elements[""PNT_MICROW_4_1656""] = microwave_key(""Microwave - 4"", GALLEY.micro_4, 1656, devices.GALLEY)
elements[""PNT_MICROW_5_1657""] = microwave_key(""Microwave - 5"", GALLEY.micro_5, 1657, devices.GALLEY)
elements[""PNT_MICROW_6_1658""] = microwave_key(""Microwave - 6"", GALLEY.micro_6, 1658, devices.GALLEY)
elements[""PNT_MICROW_7_1659""] = microwave_key(""Microwave - 7"", GALLEY.micro_7, 1659, devices.GALLEY)
elements[""PNT_MICROW_8_1660""] = microwave_key(""Microwave - 8"", GALLEY.micro_8, 1660, devices.GALLEY)
elements[""PNT_MICROW_9_1661""] = microwave_key(""Microwave - 9"", GALLEY.micro_9, 1661, devices.GALLEY)
elements[""PNT_MICROW_POPCORN_1662""] = microwave_key(""Microwave - Popcorn"", GALLEY.popcorn, 1662, devices.GALLEY)
elements[""PNT_MICROW_0_1663""] = microwave_key(""Microwave - 0"", GALLEY.micro_0, 1663, devices.GALLEY)
elements[""PNT_MICROW_MIN_1664""] = microwave_key(""Microwave - Min"", GALLEY.min, 1664, devices.GALLEY)

elements[""PNT_GALY_STOPPR_1677""] = two_pos_switch(""Microwave Latch"", devices.J_WORLD, GALLEY.microwave_stopper, 1677, 4.0)

elements[""PNT_GALY_STOPPR_1678""] = two_pos_switch(""Btm drawer stopper - left"", devices.GALLEY, GALLEY.btm_shelf_stop_l, 1678, 6.0)
elements[""PNT_GALY_STOPPR_1679""] = two_pos_switch(""Btm drawer stopper - center"", devices.GALLEY, GALLEY.btm_shelf_stop_c, 1679, 6.0)
elements[""PNT_GALY_STOPPR_1680""] = two_pos_switch(""Btm drawer stopper - right"", devices.GALLEY, GALLEY.btm_shelf_stop_r, 1680, 6.0)

elements[""PNT_GALY_SHELF_1642""] = scroll_point_axis(""Galley Middle Shelf"", devices.GALLEY, GALLEY.ctr_shelf, 1.0)
elements[""PNT_GALY_STOPPR_1675""] = two_pos_switch(""Top shelf stopper - left"", devices.GALLEY, GALLEY.ctr_shelf_stop_l, 1675,  6.0)
elements[""PNT_GALY_STOPPR_1676""] = two_pos_switch(""Top shelf stopper - right"", devices.GALLEY, GALLEY.ctr_shelf_stop_r, 1676, 6.0)

elements[""PNT_GALY_SM_DRAWER_1667""] = two_pos_switch(""Galley Bottom Shelf"", devices.GALLEY, GALLEY.btm_shelf, 1667, 6.0)

elements[""PNT_GALY_PWR_1636""] = two_pos_switch(""Galley Master Power"", devices.GALLEY, GALLEY.power, 1636, 6.0)
elements[""PNT_GALY_COFFEE_1631""] = two_pos_switch(""Galley Coffee Power"", devices.GALLEY, GALLEY.coffee, 1631, 6.0)

elements[""PNT_GALY_LQDCNT_1_1632""] = two_pos_switch(""Galley LQCDNT 1"", devices.GALLEY, GALLEY.lqcdnt_1, 1632, 6.0)
elements[""PNT_GALY_LQDCNT_2_1633""] = two_pos_switch(""Galley LQCDNT 2"", devices.GALLEY, GALLEY.lqcdnt_2, 1633, 6.0)

elements[""PNT_GALY_STOPPR_1670""] = two_pos_switch(""Galley Top Cabinet Stopper"", devices.GALLEY, GALLEY.top_cab_stopper, 1670, 6.0)
elements[""PNT_TOP_CABINET_1640""] = two_pos_switch(""Galley Top Cabinet Latch"", devices.GALLEY, GALLEY.top_cab_open, 1640, 1.0)

elements[""PNT_GALY_STOPPR_1681""] = two_pos_switch(""Galley Trash Door Stopper"", devices.GALLEY, GALLEY.trash_door_stopper, 1681, 6.0)
elements[""PNT_GALY_TRASH_LATCH_1669""] = two_pos_switch(""Galley Trash Door Latch"", devices.GALLEY, GALLEY.trash_door_latch, 1669, 1.0)
elements[""PNT_GALY_STOPPR_1671""] = two_pos_switch(""Shelf stopper 1"", devices.GALLEY, GALLEY.shelf_stopper_1, 1671, 6.0)
elements[""PNT_GALY_STOPPR_1672""] = two_pos_switch(""Shelf stopper 2"", devices.GALLEY, GALLEY.shelf_stopper_2, 1672, 6.0)
elements[""PNT_GALY_STOPPR_1673""] = two_pos_switch(""Shelf stopper 3"", devices.GALLEY, GALLEY.shelf_stopper_3, 1673, 6.0)
elements[""PNT_GALY_STOPPR_1674""] = two_pos_switch(""Shelf stopper 4"", devices.GALLEY, GALLEY.shelf_stopper_4, 1674, 6.0)


elements[""P_COFFEE_CUP_1687""] = base_btn_cycle(""Coffee Cup"", devices.PILOT_CPT_INTERFACE, PILOT_COCKPIT_INTERFACE.coffee_cup, 1687)
elements[""PNT_GALY_BEV_SHEILD_1641""] = base_btn_cycle2(""Galley Center Cabinet"", devices.GALLEY, GALLEY.bev_shield)
elements[""PNT_MICROW_OPEN_1650""] = base_btn_cycle_release(""Microwave - Open"", devices.J_WORLD, GALLEY.micro_door)
elements[""PNT_GALY_TRASH_DOOR_1666""] = base_btn_cycle2(""Galley Trash Door"", devices.GALLEY, GALLEY.trash_opening)
elements[""PNT_GALY_INSUL_OPEN_1668""] = base_btn_cycle2(""Btm Cabinet latch"", devices.GALLEY, GALLEY.bottom_cab_latch)

-- elements[""PNT_AUG_ARM_R_1697""] = two_pos_switch(""Aug Crew Right Armrest"", devices.MECH_INTERFACE, MECH_INTERFACE.aug_seat_arm_r, 1697, 2.0)
-- elements[""PNT_AUG_ARM_L_1696""] = two_pos_switch(""Aug Crew Left Armrest"", devices.MECH_INTERFACE, MECH_INTERFACE.aug_seat_arm_l, 1696, 2.0)
-- elements[""PNT_CP_ARM_R_1695""] = two_pos_switch(""Copilot Right Armrest"", devices.MECH_INTERFACE, MECH_INTERFACE.cp_seat_arm_r, 1695, 2.0)
-- elements[""PNT_CP_ARM_L_1694""] = two_pos_switch(""Copilot Left Armrest"", devices.MECH_INTERFACE, MECH_INTERFACE.cp_seat_arm_l, 1694, 2.0)
-- elements[""PNT_P_ARM_R_1693""] = two_pos_switch(""Pilot Right Armrest"", devices.MECH_INTERFACE, MECH_INTERFACE.p_seat_arm_r, 1693, 2.0)
-- elements[""PNT_P_ARM_L_1692""] = two_pos_switch(""Pilot Left Armrest"", devices.MECH_INTERFACE, MECH_INTERFACE.p_seat_arm_l, 1692, 2.0)

elements[""p_ics_helper_02""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,2, 12)
elements[""p_ics_helper_03""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,3, 12)
elements[""p_ics_helper_04""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,4, 12)
elements[""p_ics_helper_05""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,5, 12)
elements[""p_ics_helper_06""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,6, 12)
elements[""p_ics_helper_07""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,7, 12)
elements[""p_ics_helper_08""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,8, 12)
elements[""p_ics_helper_09""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,9, 12)
elements[""p_ics_helper_10""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,10, 12)
elements[""p_ics_helper_11""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,11, 12)
elements[""p_ics_helper_12""] = cable_interaction(""Pilot ICS Cable"", devices.J_WORLD, GALLEY.p_pull_cable,nil,12, 12)

elements[""cp_ics_helper_02""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,2, 12)
elements[""cp_ics_helper_03""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,3, 12)
elements[""cp_ics_helper_04""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,4, 12)
elements[""cp_ics_helper_05""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,5, 12)
elements[""cp_ics_helper_06""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,6, 12)
elements[""cp_ics_helper_07""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,7, 12)
elements[""cp_ics_helper_08""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,8, 12)
elements[""cp_ics_helper_09""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,9, 12)
elements[""cp_ics_helper_10""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,10, 12)
elements[""cp_ics_helper_11""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,11, 12)
elements[""cp_ics_helper_12""] = cable_interaction(""Copilot ICS Cable"", devices.J_WORLD, GALLEY.cp_pull_cable,nil,12, 12)


elements[""PNT_EMERG_NGEAR_COVER_131""] = two_pos_switch(""Emergency Nose Gear Release Lever Cover"", devices.MECH_INTERFACE, MECH_INTERFACE.nose_release_gear_cover, 131, 2.0)
elements[""PNT_EMRG_NGEAR_REL_132""] = two_pos_switch(""Emergency Nose Gear Release Lever"", devices.MECH_INTERFACE, MECH_INTERFACE.nose_release_gear, 132, 2.0)

elements[""PNT_ELT_ON_149""] = two_pos_switch(""ELT"", devices.MECH_INTERFACE, MECH_INTERFACE.elt_on_off, 149, 7.0)

--- ARC 210
elements[""PNT_ARC210_RCU_LSK_1_550""] = cni_key_no_stop(""ARC-210 LSK 1"", VOL_CTRL.arc_l1, 550, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_LSK_2_549""] = cni_key_no_stop(""ARC-210 LSK 2"", VOL_CTRL.arc_l2, 549, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_LSK_3_548""] = cni_key_no_stop(""ARC-210 LSK 3"", VOL_CTRL.arc_l3, 548, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_EMERG_TOD_SND_551""] = cni_key_no_stop(""ARC-210 TOD SND Key"", VOL_CTRL.arc_tod_snd, 551, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_EMERG_TOD_RCV_552""] = cni_key_no_stop(""ARC-210 TOD RCV Key"", VOL_CTRL.arc_tod_rcv, 552, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_GPS_553""] = cni_key_no_stop(""ARC-210 GPS Key"", VOL_CTRL.arc_gps, 553, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_RT_SELECT_554""] = cni_key_no_stop(""ARC-210 RT SELECT Key"", VOL_CTRL.arc_rt_select, 554, devices.VOLUME_MANAGER)
elements[""PNT_RCU_MENU_533""] = cni_key_no_stop(""ARC-210 MENU/TIME Key"", VOL_CTRL.arc_menu, 533, devices.VOLUME_MANAGER)
elements[""PNT_RCU_AMFM_534""] = cni_key_no_stop(""ARC-210 AM/FM Key"", VOL_CTRL.arc_am_fm, 534, devices.VOLUME_MANAGER)
elements[""PNT_RCU_XMTRCV_535""] = cni_key_no_stop(""ARC-210 XMT/REC or SEND Key"", VOL_CTRL.arc_xmt_rec_send, 535, devices.VOLUME_MANAGER)
elements[""PNT_RCU_OFFSET_536""] = cni_key_no_stop(""ARC-210 OFFSET or RCV Key"", VOL_CTRL.arc_rcu_offset, 536, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_ENTER_537""] = cni_key_no_stop(""ARC-210 ENTER Key"", VOL_CTRL.arc_enter, 537, devices.VOLUME_MANAGER)

elements[""PNT_ARC210_RCU_BRT_INC_547""] = cni_key_no_stop(""ARC-210 Brightness Increase"", VOL_CTRL.arc_brt_up, 547, devices.VOLUME_MANAGER)
elements[""PNT_ARC210_RCU_BRT_DEC_546""] = cni_key_no_stop(""ARC-210 Brightness Decrease"", VOL_CTRL.arc_brt_dn, 546, devices.VOLUME_MANAGER)

-- elements[""ARC210_RCU_POWER_532""] = knob_fixed(""ARC 210 Power"", VOL_CTRL.arc_power, 532, 0.5, devices.VOLUME_MANAGER, {-1, 1})
elements[""ARC210_RCU_POWER_532""] = multiswitch(""ARC-210 Squelch Switch"",  devices.VOLUME_MANAGER, VOL_CTRL.arc_sql, 532, {-1, 1}, 2)
elements[""PNT_RCU_CHNL_544""] = knob_360(""ARC-210 Channel Selector"", VOL_CTRL.arc_channel, 544, 0.2, nil, devices.VOLUME_MANAGER)
elements[""PNT_RCU_OP_MODE_543""] = multiswitch(""ARC-210 Operational Mode Switch"", devices.VOLUME_MANAGER, VOL_CTRL.arc_op_mode, 543, {0, 1}, 1/6)
elements[""PNT_RCU_Q_MODE_545""] = multiswitch(""ARC-210 Frequency Mode Switch"", devices.VOLUME_MANAGER, VOL_CTRL.arc_q_mode, 545, {0, 1}, 1/6)
elements[""PNT_RCU_1_542""] = multiswitch(""ARC-210 100 MHz Selector"", devices.VOLUME_MANAGER, VOL_CTRL.arc_rcu_1, 542, {0, 1}, 1/3)
elements[""PNT_RCU_5_538""] = multiswitch(""ARC-210 25 KHz Selector"", devices.VOLUME_MANAGER, VOL_CTRL.arc_rcu_5, 538, {0, 1}, 1/3)
elements[""PNT_RCU_2_541""] = multiswitch(""ARC-210 10 MHz Selector"", devices.VOLUME_MANAGER, VOL_CTRL.arc_rcu_2, 541, {0, 1}, 1/9)
elements[""PNT_RCU_3_540""] = multiswitch(""ARC-210 1 MHz Selector"", devices.VOLUME_MANAGER, VOL_CTRL.arc_rcu_3, 540, {0, 1}, 1/9)
elements[""PNT_RCU_4_539""] = multiswitch(""ARC-210 100 KHz Selector"", devices.VOLUME_MANAGER, VOL_CTRL.arc_rcu_4, 539, {0, 1}, 1/9)

elements[""PNT_TRIM_MULTI_UP_1364""] = emergency_trim(""Elevator Trim Nose Down"", devices.MECH_INTERFACE, MECH_INTERFACE.trim_box_up, 1364, 1.0, {0, 1}, 0)
elements[""PNT_TRIM_MULTI_DN_1364""] = emergency_trim(""Elevator Trim Nose Up"", devices.MECH_INTERFACE,  MECH_INTERFACE.trim_box_dn, 1364, -1, {-1, 0}, 0)
elements[""PNT_TRIM_MULTI_LEFT_1365""] = emergency_trim(""Aileron Trim Left Wing Down"", devices.MECH_INTERFACE,  MECH_INTERFACE.trim_box_left, 1365, -1, {-1, 0}, 0)
elements[""PNT_TRIM_MULTI_RIGHT_1365""] = emergency_trim(""Aileron Trim Right Wing Down"", devices.MECH_INTERFACE,  MECH_INTERFACE.trim_box_right, 1365, 1, {0, 1}, 0)

elements[""PNT_LM_GLASS_2100""] = two_pos_switch(""Loadmaster Station Cover"", devices.CARGO_HANDLER, CARGO_HANDLER.loadmaster_cover, 2100, 1.0)
elements[""PNT_LM_RAMP_2101""] = three_pos_spring_load_on(""Loadmaster Ramp/Door Control Switch"", devices.MECH_INTERFACE, MECH_INTERFACE.loadmaster_ramp, 2101)

elements[""PNT_CREW_DOOR_2110""] = two_pos_switch(""Crew Door External"", devices.MECH_INTERFACE, MECH_INTERFACE.crew_door_external, 2110, 1.0)
elements[""PNT_CREW_DOOR_INTERNAL_2111""] = two_pos_switch(""Crew Door"", devices.MECH_INTERFACE, MECH_INTERFACE.crew_door_internal, 2200, 12.0)

elements[""PNT_TABLET_CLICK""] = tab(""Tablet"", CARGO_HANDLER.TABLET_CLICK, nil, devices.CARGO_HANDLER)
";
            #endregion
        }

        private static string[,] LampsToArray()
        {
            return new string[,]
           { {"4000", "CNI (All)", "CNI Power Lights "},
{"4011", "Pilot Displays", "HUD Vis Mode"},
{"4012", "Pilot Displays", "CAT 2"},
{"4013", "Pilot Displays", "O/S"},
{"4014", "Pilot Displays", "UNCG"},
{"4015", "Pilot Displays", "NAV"},
{"4016", "Pilot Displays", "TACT"},
{"4017", "Copilot Displays", "HUD Vis Mode"},
{"4018", "Copilot Displays", "CAT 2"},
{"4019", "Copilot Displays", "O/S"},
{"4020", "Copilot Displays", "UNCG"},
{"4021", "Copilot Displays", "NAV"},
{"4022", "Copilot Displays", "TACT"},
{"4023", "Engine", "Engine 1 Start"},
{"4024", "Engine", "Engine 2 Start"},
{"4025", "Engine", "Engine 3 Start"},
{"4026", "Engine", "Engine 4 Start"},
{"4027", "Engine", "APU Start"},
{"4028", "Fuel System", "SPR Valve "},
{"4030", "Hydraulics", "AUX Pump On"},
{"4032", "Landing Gear", "Nose Gear"},
{"4033", "Landing Gear", "Left Gear"},
{"4034", "Landing Gear", "Right Gear"},
{"4035", "Landing Gear", "Warning"},
{"4036", "Engine", "Generator 1"},
{"4037", "Engine", "Generator 2"},
{"4038", "Engine", "Generator 3"},
{"4039", "Engine", "Generator 4"},
{"4040", "Indicators", "Button Brightness"},
{"4042", "Indicators", "Button Legend Brightness"},
{"4045", "Ref Panel", "Master Warning"},
{"4046", "Ref Panel", "Master Caution"},
{"4047", "AutoPilot", "Mode ALT ON"},
{"4048", "AutoPilot", "Mode VS ON"},
{"4049", "AutoPilot", "Mode SEL ON"},
{"4050", "AutoPilot", "Mode IAS ON"},
{"4051", "AutoPilot", "Mode HDG ON"},
{"4052", "AutoPilot", "Mode NAV ON"},
{"4053", "AutoPilot", "Mode CAPS ON"},
{"4054", "AutoPilot", "Mode APPR ON "},
{"4055", "AutoPilot", "Mode A/T ON"},
{"4056", "Caution Panel Pilot", "AP ON"},
{"4057", "Caution Panel Pilot", "PITCH OFF"},
{"4058", "Caution Panel Pilot", "NAV ARM"},
{"4059", "Caution Panel Pilot", "GS ARM"},
{"4060", "Caution Panel Pilot", "GO ARND"},
{"4061", "Caution Panel Pilot", "CAT2 ARM"},
{"4062", "Caution Panel Pilot", "AP DSGN"},
{"4063", "Caution Panel Pilot", "LAT OFF"},
{"4064", "Caution Panel Pilot", "NAV CAPT"},
{"4065", "Caution Panel Pilot", "GS CAPT"},
{"4066", "Caution Panel Pilot", "BACK LOC"},
{"4067", "Caution Panel Pilot", "CAT2"},
{"4068", "Hydraulics", "EMER Brake Sel"},
{"4069", "Hydraulics", "Engine 1 Pumps OFF"},
{"4070", "Hydraulics", "Engine 2 Pumps OFF"},
{"4071", "Hydraulics", "Engine 3 Pumps OFF"},
{"4072", "Hydraulics", "Engine 4 Pumps OFF"},
{"4073", "Hydraulics", "Util Suction Pump OFF "},
{"4074", "Hydraulics", "Boost Suction Pump OFF "},
{"4075", "Aerial Delivery", "Ramp Door FULL"},
{"4077", "RADAR", "PRCN"},
{"4078", "RADAR", "MAP Mode"},
{"4079", "RADAR", "WX Mode"},
{"4080", "RADAR", "SP Mode"},
{"4081", "RADAR", "MGM Mode"},
{"4082", "RADAR", "WS Mode"},
{"4083", "RADAR", "BCN Mode"},
{"4084", "RADAR", "PSEL"},
{"4085", "RADAR", "OFS Function"},
{"4086", "RADAR", "FRZ Function"},
{"4087", "RADAR", "PEN Function"},
{"4088", "RADAR", "SCTR Function"},
{"4089", "AFCS", "Pitch OFF"},
{"4090", "AFCS", "Lat OFF"},
{"4091", "Engine", "1 Low Speed"},
{"4092", "Engine", "2 Low Speed"},
{"4093", "Engine", "3 Low Speed"},
{"4094", "Engine", "4 Low Speed"},
{"4095", "Aerial Delivery", "Caution"},
{"4096", "Aerial Delivery", "Jump"},
{"4097", "RWR", "SRCH ON"},
{"4098", "RWR", "MODE PRI"},
{"4099", "RWR", "HAND OFF DIAMOND"},
{"4100", "RWR", "ALT HIGH"},
{"4101", "RWR", "TGT SEP ON"},
{"4102", "Air Conditioning", "Flight Station Power OFF"},
{"4103", "Air Conditioning", "Cargo Compartment Power OFF"},
{"4104", "Air Conditioning", "Flight Station Man ON"},
{"4105", "Air Conditioning", "Cargo Compartment Man ON"},
{"4106", "Air Conditioning", "Flight Station X-Flow Man ON"},
{"4107", "FLCV", "FLCV TEST ON"},
{"4108", "Bleed Air", "APU OPEN"},
{"4114", "Caution Panel Copilot", "AP ON"},
{"4115", "Caution Panel Copilot", "PITCH OFF"},
{"4116", "Caution Panel Copilot", "NAV ARM"},
{"4117", "Caution Panel Copilot", "GS ARM"},
{"4118", "Caution Panel Copilot", "GO ARND"},
{"4119", "Caution Panel Copilot", "CAT2 ARM"},
{"4120", "Caution Panel Copilot", "AP DSGN"},
{"4121", "Caution Panel Copilot", "LAT OFF"},
{"4122", "Caution Panel Copilot", "NAV CAPT"},
{"4123", "Caution Panel Copilot", "GS CAPT"},
{"4124", "Caution Panel Copilot", "BACK LOC"},
{"4125", "Caution Panel Copilot", "CAT2"},
{"4131", "Fire Panel", "Eng 1 Fire"},
{"4132", "Fire Panel", "Eng 2 Fire"},
{"4133", "Fire Panel", "Eng 3 Fire"},
{"4134", "Fire Panel", "Eng 4 Fire"},
{"4135", "Fire Panel", "APU Fire"},
{"4137", "CNI Pilot", "DSPY"},
{"4138", "CNI Pilot", "MSG"},
{"4139", "CNI Pilot", "FAIL"},
{"4140", "CNI Pilot", "OFST"},
{"4141", "CNI Copilot", "DSPY"},
{"4142", "CNI Copilot", "MSG"},
{"4143", "CNI Copilot", "FAIL"},
{"4144", "CNI Copilot", "OFST"},
{"4145", "CNI Aug Crew", "DSPY"},
{"4146", "CNI Aug Crew", "MSG"},
{"4147", "CNI Aug Crew", "FAIL"},
{"4148", "CNI Aug Crew", "OFST"}};
        }
    }
    internal class FunctionData
    {
        internal string[] Name;
        internal string Fn;
        internal string[] Val;
        internal string[] Arg;
        internal string Device;
        internal string[] Command;
        internal string Head;
        internal string Tail;
        internal string Description;
        internal bool Duplicate;
    }
}
