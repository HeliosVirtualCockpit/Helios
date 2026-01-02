//  Copyright 2025 Helios Contributors
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
using GadrocsWorkshop.Helios.Util.DCS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace GadrocsWorkshop.Helios.Interfaces.DCS.C130J
{
    internal static class ProcessInterfaceFunctions
    {
        private static string _pattern;
        private static string _input;
        private static StreamWriter _streamWriter;
        private static Dictionary<string, FunctionData> _functions = new Dictionary<string, FunctionData>();
        private static NetworkFunctionCollection _functionList = new NetworkFunctionCollection();
        private static BaseUDPInterface _baseUDPInterface;
        private static readonly Dictionary <string, string> _categorySubstitutions, _nameSubstitutions, _clickableSubstitutions;
        private static readonly RegexOptions _options = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
        private static Dictionary<string, FunctionData> _dualFunctions = new Dictionary<string, FunctionData>();
        private static string _DCSAircraft = $@"{Environment.GetEnvironmentVariable("ProgramFiles")}\Eagle Dynamics\DCS World\Mods\aircraft\C130J\Cockpit\Scripts";
        //private static string _DCSAircraft = _DCSAircraft = $@"\\atlas\users\Neil\Documents\DCS\C-130J-30";



        static ProcessInterfaceFunctions() {
            _categorySubstitutions = CategoryInit();
            _nameSubstitutions = ElementInit();
            _clickableSubstitutions = ClickableCorrectionsInit();
        }
        internal static NetworkFunctionCollection Process(BaseUDPInterface udpInterface)
        {
            InstallationLocations locations = InstallationLocations.Singleton;
            if(locations.Items.Count > 0)
            {
                _DCSAircraft = $@"{locations.Items[0].Path}\Mods\aircraft\C130J\Cockpit\Scripts";
            }
            _baseUDPInterface = udpInterface;
            SetDevices();
            _input = SetClickables();
            //  we do a quick edit on mis-coded clickable data in order to avoid downstream issues

            foreach (var pair in _clickableSubstitutions)
            {
                _input = _input.Replace(pair.Key, pair.Value);
            }

            int i = 0;
            foreach (Match m in Regex.Matches(_input, _pattern, _options))
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
                    argKey = argArray[0];
                    if (m.Groups["function"].Value == "display_rocker_hdd")
                    {
                        argKey = m.Groups["value"].Captures[0].Value;
                        argArray = new string[] { argKey };
                    }
                    duplicateArg = _functions.ContainsKey(argKey);
                    argKey = duplicateArg ? "D_" + argKey : argKey;  // this obviously will only allow one duplicate
                }
                _functions.Add(argKey, new FunctionData()
                {
                    Fn = m.Groups["function"].Value,
                    ElementName = m.Groups["element"].Value,
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
                    Device = "",
                    Command = null,
                    Head = "",
                    Tail = "",
                    Description = "Boolean value. Illuminated when value is true",
                    Duplicate = false
                });
            }

            string[,] indications = IndicationsToTextArray();
            for (int j = 0; j < indications.GetLength(0); j++)
            {
                if (string.IsNullOrEmpty(indications[j, 2]))
                {
                    continue;
                }
                _functions.Add(indications[j, 0], new FunctionData()
                {
                    Fn = "IndicationText",
                    Name = new string[] { indications[j, 1], indications[j, 2] },
                    Val = null,
                    Arg = new string[] { indications[j, 0] },
                    Device = "",
                    Command = null,
                    Head = "",
                    Tail = "",
                    Description = indications[j, 3],
                    Duplicate = false
                });
            }

            string clickableFilename = "C-130J_Functions";
#if (DEBUG)
            _streamWriter = new StreamWriter($@"{Environment.GetEnvironmentVariable("userprofile")}\Documents\HeliosDev\Interfaces\{clickableFilename}.txt", false);
#endif
            foreach (FunctionData fd in _functions.Values)
            {
                if (!fd.Duplicate)
                {
                    FunctionBuilder(fd);
                }
                else
                {
#if (DEBUG)
                    if (string.Join("_", fd.Name).Contains("Brightness") || fd.Fn.Contains("rocker"))
                    {
                        FunctionBuilder(fd);
                    }
                    else
                    {
                        WriteCsFunction($"Not creating function for duplicate \"{fd.Arg[0]}\" - \"{string.Join("_", fd.Name)}\"");
                    }
#endif
                }

            }
#if (DEBUG)
            _streamWriter.Close();
#endif
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
                    WriteCsFunction($"\t\t{BuildFnLamp(fd)}");
                    break;
                case "IndicationText":
                    WriteCsFunction($"\t\t{BuildIndicationsText(fd)}");
                    break;
                case "at_disconnect":
                case "hud_btn":
                case "lsgi_btn":
                case "master_warning":
                case "master_caution":
                case "ref_btn":
                case "push_button":
                case "base_btn":
                case "microwave_key":
                case "amu_key":
                case "cni_key_no_stop":
                    WriteCsFunction($"\t\t{BuildFnButton(fd)}");
                    break;
                case "flap_switch":
                case "boost_guard":
                case "fuel_xfeed":
                case "parking_brake":
                case "air_deflector":
                case "landing_lights":
                case "two_pos_switch_ap":
                case "two_pos_switch_spring":
                case "two_pos_switch_cargo":
                case "generator_switch":
                case "two_pos_switch_rev":
                case "gear_handle":
                case "two_pos_switch":
                    WriteCsFunction($"\t\t{BuildFnToggle(fd)}");
                    break;
                case "multiswitch_stop":
                    WriteCsFunction($"\t\t{BuildFnMultiSwitchStop(fd)}");
                    break;
                case "oxygen_switch":
                case "landing_lights_motor":
                case "multiswitch":
                    WriteCsFunction($"\t\t{BuildFnMultiSwitch(fd, true)}");
                    break;
                case "rotary":
                    WriteCsFunction($"\t\t{BuildFnMultiSwitch(fd)}");
                    break;
                case "rudder_trim":
                case "rocker_centering":
                case "three_pos_switch_spring":
                case "three_pos_spring_load_on_inv":
                case "three_pos_spring_load_on":
                    WriteCsFunction($"\t\t{BuildFnThreeWayToggle(fd)}");
                    break;
               case "one_way_rocker":
                case "display_rocker_hdd":
                case "cni_brt":
                    WriteCsFunction($"\t\t{BuildRocker(fd)}");
                    break;
                case "knob_fixed":
                    WriteCsFunction($"\t\t{BuildKnob(fd)}");  // note that this has a sep command to reset which is not currently in the interface.
                    break;
                case "knob_360_0_1":
                case "knob_360":
                case "knob_rot_rel":
                case "knob_rot":
                    WriteCsFunction($"\t\t{BuildKnob(fd)}");
                    break;
                case "hud_brt_knob":
                    WriteCsFunction($"\t\t{BuildKnobWithPull2(fd)}");
                    break;
                case "knob_360_press":
                    WriteCsFunction($"\t\t{BuildKnobWithPull1(fd)}");
                    break;
                case "ics_knob":
                    WriteCsFunction($"\t\t{BuildKnobWithPull(fd)}");
                    break;
                case "oil_flap_switch":
                case "oil_flap_switch_open_close":
                    WriteCsFunction($"\t\t{BuildYSwitch(fd)}");
                    break;
                case "base_btn_cycle3":
                    // these are used to combine two arguments which are covered elsewhere so these are a nop
                    break;
                case "fire_pull":
                    break;
                case "wiper":
                    break;
                case "stby_altim":
                    break;
                case "adi_cage":
                    break;
                case "hud_latch":
                    break;
                case "guard":
                    break;
                case "fuel_transfer":
                    break;
                case "emg_ext_light":
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
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            _functionList.Add(new FlagValue(_baseUDPInterface, fd.Arg[0], category, name, fd.Description));
            return $"AddFunction(new FlagValue(this, \"{fd.Arg[0]}\", \"{category}\", \"{name}\", \"{fd.Description}\"));";
        }
        private static string BuildIndicationsText(FunctionData fd)
        {
            _functionList.Add(new Text(_baseUDPInterface, fd.Arg[0], fd.Name[0], fd.Name[1], fd.Description));
            return $"AddFunction(new Text(this, \"{fd.Arg[0]}\", \"{fd.Name[0]}\", \"{fd.Name[1]}\", \"{fd.Description}\"));";
        }
        private static string BuildFnButton(FunctionData fd)
        {
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName, fd);
            _functionList.Add(new PushButton(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], category, name));
            return $"AddFunction(new PushButton(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", \"{category}\", \"{name}\"));";
        }
        private static string BuildFnToggle(FunctionData fd)
        {
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            if(fd.Fn.EndsWith("cargo"))
            {
                name = "Cargo " + name;
            }
            string startVal, endVal;
            if (fd.Fn.EndsWith("_rev") || fd.Fn.StartsWith("generator"))
            {
                startVal = "0.0";
                endVal = "1.0";
            } else
            {
                startVal = "1.0";
                endVal = "0.0";
            }
            _functionList.Add(Switch.CreateToggleSwitch(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], startVal, "OPEN", endVal, "CLOSE", category, name, "%0.1f"));
            return $"AddFunction(Switch.CreateToggleSwitch(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", \"{startVal}\", \"OPEN\", \"{endVal}\", \"CLOSE\", \"{category}\", \"{name}\", \"%0.1f\"));";
        }
        
        private static string BuildFnThreeWayToggle(FunctionData fd)
        {
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            string startVal, endVal, midVal;
            if (fd.Fn.EndsWith("_inv"))
            {
                startVal = "1.0";
                endVal = "-1.0";
                midVal = "0.0";
            }
            else
            {
                startVal = "1.0";
                endVal = "-1.0";
                midVal = "0.0";
            }
            _functionList.Add(Switch.CreateThreeWaySwitch(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], startVal, "Posn 1", midVal, "Posn 2", endVal, "Posn 3", category, name, "%0.1f"));
            return $"AddFunction(Switch.CreateThreeWaySwitch(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", \"{startVal}\", \"Posn 1\", \"{midVal}\", \"Posn 2\", \"{endVal}\", \"Posn 3\", \"{category}\", \"{name}\", \"%0.1f\"));";
        }

        private static string BuildFnMultiSwitchStop(FunctionData fd)
        {
            SwitchPosition[] sp = new SwitchPosition[] { new SwitchPosition("-0.33", "MOTOR", CommandEnumToString(fd.Command[0]), CommandEnumToString(fd.Val[4]), "1.0", null), new SwitchPosition("0.00", "STOP", CommandEnumToString(fd.Command[0]), CommandEnumToString(fd.Val[4]), null, null), new SwitchPosition("0.5", "RUN", CommandEnumToString(fd.Command[0]), CommandEnumToString(fd.Val[4]), null, null), new SwitchPosition("1.00", "START", CommandEnumToString(fd.Command[0]), CommandEnumToString(fd.Val[4]), "-1.0", null) };
            string switchPositionsInsert = fd.Val[1] == "{0" ? "" : $"new SwitchPosition(\"-0.33\", \"MOTOR\", Commands.{fd.Command[0]}.ToString(\"d\"), Commands.{fd.Val[4]}.ToString(\"d\"), \"1.0\", null), ";
            string swPositions = $"{{{switchPositionsInsert}new SwitchPosition(\"0.0\", \"STOP\", Commands.{fd.Command[0]}.ToString(\"d\"), Commands.{fd.Val[4]}.ToString(\"d\"), null, null), new SwitchPosition(\"0.0\", \"RUN\", Commands.{fd.Command[0]}.ToString(\"d\"), Commands.{fd.Val[4]}.ToString(\"d\"), null, null), new SwitchPosition(\"1.0\", \"START\", Commands.{fd.Command[0]}.ToString(\"d\"), Commands.{fd.Val[4]}.ToString(\"d\"), \"-1.0\", null) }}";
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            _functionList.Add(new Switch(_baseUDPInterface, DeviceEnumToString(fd.Device), fd.Val[0], fd.Val[1] == "{0" ? new SwitchPosition[] { sp[1], sp[2], sp[3] } : sp, category, name, "%0.2f"));
            return $"AddFunction(new Switch(this, devices.{fd.Device}.ToString(\"d\"), \"{fd.Val[0]}\", new SwitchPosition[] {swPositions}, \"{category}\", \"{name}\", \"%0.2f\"));";
        }
        private static string BuildFnMultiSwitch(FunctionData fd, bool invert = false)
        {
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            DataTable dt = new DataTable();
            double startVal = Convert.ToDouble(dt.Compute(fd.Val[1].Replace("{", ""), ""));
            double endVal = Convert.ToDouble(dt.Compute(fd.Val[2].Replace("}", ""), ""));
            double intervalVal = Convert.ToDouble(dt.Compute(fd.Val[3], "")) ;
            int positions = Convert.ToInt32(((endVal - startVal) / intervalVal) + 1);
            if (invert)
            {
                double tempVal = endVal;
                endVal = startVal;
                startVal = tempVal;
                intervalVal *= -1;
            }
            _functionList.Add(new Switch(_baseUDPInterface, DeviceEnumToString(fd.Device), fd.Val[0], SwitchPositions.Create(positions, startVal, intervalVal, CommandEnumToString(fd.Command[0]), "Posn", "%.2f"), category, name, "%0.2f"));
            return $"AddFunction(new Switch(this, devices.{fd.Device}.ToString(\"d\"), \"{fd.Val[0]}\", SwitchPositions.Create({positions}, {startVal}, {endVal}, Commands.{fd.Command[0]}.ToString(\"d\"), \"%.2f\"), \"{category}\", \"{name}\", \"%0.2f\"));";
        }
        private static string BuildRocker(FunctionData fd)
        {
            // This is a rocker which is defined as two elements and two buttons in the clickables so we get called twice for each rocker.
            // For rockers especially, the arg in the element name is unreliable so we use the real one from Val[0]

            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            name = name.Replace(" Increase", "").Replace(" Decrease", "");
            if (_dualFunctions.ContainsKey($"{fd.Val[0]}"))
                {
                    FunctionData fd1 = _dualFunctions[$"{fd.Val[0]}"];
                    _dualFunctions.Remove($"{fd.Val[0]}");
                    _functionList.Add(new Switch(_baseUDPInterface, DeviceEnumToString(fd.Device), fd.Val[0], new SwitchPosition[] { new SwitchPosition("1.0", "Increase", CommandEnumToString(fd1.Command[0]), CommandEnumToString(fd1.Command[0]), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Decrease", CommandEnumToString(fd.Command[0]), CommandEnumToString(fd.Command[0]), "0.0", "0.0") }, category, name, "%0.1f"));
                    return $"AddFunction(new Switch(this, devices.{fd.Device}.ToString(\"d\"), \"{fd.Val[0]}\", new SwitchPosition[] {{ new SwitchPosition(\"1.0\", \"Up\", Commands.{fd1.Command[0]}.ToString(\"d\"), Commands.{fd1.Command[0]}.ToString(\"d\"), \"0.0\", \"0.0\"), new SwitchPosition(\"0.0\", \"Middle\", null), new SwitchPosition(\"-1.0\", \"Down\", Commands.{fd.Command[0]}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"0.0\", \"0.0\") }}, \"{category}\", \"{name}\", \"%0.1f\"));";
                }
                else
                {
                    _dualFunctions.Add($"{fd.Val[0]}", fd);
                }

            return "";
        }
        private static string BuildYSwitch(FunctionData fd)
        {
            // This is a new Y Switch and is defined by four elements in the clickables so we get called four times for each switch.
            // The Arg has value 0 as the centre, 0.333 is the bottom position (Auto), 0.6667 is the -60 deg postion (Open) and 1.0 is the +60 deg postion (Closed)
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            if (_dualFunctions.Count < 3)
            {
                _dualFunctions.Add($"{fd.Name[0]}", fd);
            } else
            {
                _dualFunctions.Add($"{fd.Name[0]}", fd);
                List <string> positionClickables = new List<string>();
                name = "";  
                foreach (string k in _dualFunctions.Keys)
                {
                    positionClickables.Add(k.Split(' ').Last());
                    if (string.IsNullOrEmpty(name))
                    {
                        name = k.Replace(positionClickables[0], "").Trim();
                    }
                }
                string[] posns = new string[] { "Fixed", "Auto", "Open", "Close"};
                FunctionData[] fDs = new FunctionData[] {
                    _dualFunctions[$"{name} {posns[0]}"],
                    _dualFunctions[$"{name} {posns[1]}"],
                    _dualFunctions[$"{name} {posns[2]}"],
                    _dualFunctions[$"{name} {posns[3]}"],
                };

                fd = fDs[0];
                category = "Engine";
                _functionList.Add(new Switch(_baseUDPInterface, DeviceEnumToString(fd.Device), fd.Arg[0], new SwitchPosition[] { new SwitchPosition("0.00", posns[0], CommandEnumToString(fDs[0].Command[0])), new SwitchPosition("0.33", posns[1], CommandEnumToString(fDs[1].Command[0])), new SwitchPosition("0.70", posns[2], CommandEnumToString(fDs[2].Command[0]), CommandEnumToString(fDs[2].Command[0]), "0.50"), new SwitchPosition("1.00", posns[3], CommandEnumToString(fDs[3].Command[0]), CommandEnumToString(fDs[3].Command[0]),"0.50") }, category, name, "%0.2f"));
                string retValue =  $"AddFunction(new Switch(this, devices.{fd.Device}.ToString(\"d\"), \"{fd.Arg[0]}\",  new SwitchPosition[] {{ new SwitchPosition(\"0.00\", \"{posns[0]}\", Commands.{fDs[0].Command[0]}.ToString(\"d\")), new SwitchPosition(\"0.33\", \"{posns[1]}\", Commands.{fDs[1].Command[0]}.ToString(\"d\")), new SwitchPosition(\"0.67\", \"{posns[2]}\", Commands.{fDs[2].Command[0]}.ToString(\"d\"), Commands.{fDs[2].Command[0]}.ToString(\"d\"), \"0.50\"), new SwitchPosition(\"1.00\", \"{posns[3]}\", Commands.{fDs[3].Command[0]}.ToString(\"d\"), Commands.{fDs[3].Command[0]}.ToString(\"d\"), \"0.50\")}}, \"{category}\", \"{name}\", \"%0.2f\"));";
                _dualFunctions.Clear();
                return retValue;
            }
            return "";
        }

        private static string BuildKnobWithPull(FunctionData fd)
        {
            string retValue = "";
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);

            _functionList.Add(new Axis(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Val[0]), fd.Arg[1], 0.05, 0d, 1d, category, name, false, "%0.2f"));
            retValue += $"AddFunction(new Axis(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Val[0]}.ToString(\"d\"), \"{fd.Arg[1]}\", 0.05d, 0d, 1, \"{category}\", \"{name}\", false, \"%0.2f\"));";
            _functionList.Add(new PushButton(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], category, name + " Monitor Switch"));
            retValue += $"\n\t\tAddFunction(new PushButton(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", \"{category}\", \"{name}\" + \"  Monitor Switch\"));";
            return retValue;

        }
        private static string BuildKnobWithPull1(FunctionData fd)
        {
            string retValue = "";
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);

            _functionList.Add(new RotaryEncoder(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Val[0], 0.05, category, name.Split('-')[0].Trim(), "%0.2f"));
            //_functionList.Add(new Axis(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Val[0], 0.01, -1d, 1d, category, name.Split('-')[0].Trim(), true, "%0.2f"));
            retValue += $"AddFunction(new Axis(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Val[0]}\", 0.05d, 0d, 1, \"{category}\", \"{name.Split('-')[0].Trim()}\", false, \"%0.2f\"));";
            _functionList.Add(new PushButton(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Val[2]), fd.Val[3], category, name + " Push Switch"));
            retValue += $"\n\t\tAddFunction(new PushButton(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Val[2]}.ToString(\"d\"), \"{fd.Val[3]}\", \"{category}\", \"{name}\" + \"  Push Switch\"));";
            return retValue;

        }
        private static string BuildKnobWithPull2(FunctionData fd)
        {
            string retValue = "";
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);

            _functionList.Add(new RotaryEncoder(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], 0.05, category, name.Split('-')[0].Trim(), "%0.2f"));
            retValue += $"AddFunction(new RotaryEncoder(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", 0.05d, \"{category}\", \"{name.Split('-')[0].Trim()}\", false, \"%0.2f\"));";
            _functionList.Add(new Switch(_baseUDPInterface, DeviceEnumToString(fd.Device), fd.Arg[1], new SwitchPosition[] { new SwitchPosition("1.0", "Out", CommandEnumToString(fd.Val[1])), new SwitchPosition("0.0", "In", CommandEnumToString(fd.Val[1]))}, category, name + " Push", "%0.1f"));
            retValue += $"\n\t\tAddFunction(new Switch(this, devices.{fd.Device}.ToString(\"d\"), \"{fd.Arg[1]}\", new SwitchPosition[] {{ new SwitchPosition(\"1.0\", \"Out\", Commands.{fd.Val[1]}.ToString(\"d\")),  new SwitchPosition(\"0.0\", \"In\", Commands.{fd.Val[1]}.ToString(\"d\"))}}, \"{category}\", \"{name}\" + \" Push\", \"%0.1f\"));";
            return retValue;

        }
        private static string BuildKnob(FunctionData fd)
        {
            string retValue = "";
            (string category, string name) = AdjustName(fd.Name, fd.Device, fd.ElementName);
            double startVal, endVal, intervalVal;
            if (fd.Fn == "knob_fixed")
            {
                startVal = Convert.ToDouble(fd.Val[2].Replace("{", ""));
                endVal = Convert.ToDouble(fd.Val[3].Replace("}", ""));
                intervalVal = Convert.ToDouble(fd.Val[1]);
            }
            else if (fd.Fn == "knob_360")
            {
                startVal = -1d;
                endVal = 1d;
                intervalVal = 0.1d;
            }
            else

            {
                startVal = 0d;
                endVal = 1d;
                intervalVal = 0.05d;
            }

            _functionList.Add(new Axis(_baseUDPInterface, DeviceEnumToString(fd.Device), CommandEnumToString(fd.Command[0]), fd.Arg[0], intervalVal, startVal, endVal, category, name, false, "%0.2f"));
            retValue += $"AddFunction(new Axis(this, devices.{fd.Device}.ToString(\"d\"), Commands.{fd.Command[0]}.ToString(\"d\"), \"{fd.Arg[0]}\", {intervalVal}d, {startVal}d, {endVal}d, \"{category}\", \"{name}\", false, \"%0.2f\"));";
            return retValue;

        }

        private static string CommandEnumToString(string commandEnum)
        {
            return ((int)Enum.Parse(Type.GetType($"GadrocsWorkshop.Helios.Interfaces.DCS.C130J.Commands+{commandEnum.Split('.')[0]}"), commandEnum.Split('.')[1])).ToString();
        }
        private static string DeviceEnumToString(string deviceEnum)
        {
            return ((devices)Enum.Parse(typeof(devices), deviceEnum)).ToString("d");
        }
        private static (string, string) AdjustName(string[] origName, string origDevice, string elementName, FunctionData fd = null)
        {
            string category;
            string name;
            if (origName.Length > 1)
            {
                if (origDevice.Contains("_CPT_INTERFACE"))
                {
                    category = $"Cockpit {origName[0].Split(' ')[0]}";
                    name = $"{origName[0].Replace(origName[0].Split(' ')[0], "").Trim()} - {origName[1]}";
                }
                else if (origDevice.Contains("_REF_MODE_PANEL"))
                {
                    category = $"Ref Mode {origName[0].Split(' ')[0]}";
                    name = $"{origName[0].Replace(origName[0].Split(' ')[0], "").Trim()} - {origName[1]}";
                }
                else if (origName[0] == "ARC")
                {
                    category = "ARC-210";
                    name = origName[1].Substring(origName[1].IndexOf("210") + 3).Trim();
                }
                else if (origName[0].Contains("ilot HUD"))
                {
                    category = origName[0].StartsWith("Pilot HUD") ? "Displays Pilot" : "Displays Copilot";
                    name = origName[0].Split(new char[] { ' ' }, 2)[1].Trim();
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
                else if (origDevice.Contains("VOLUME_MANAGER"))
                {
                    
                    string[] origWords = origName[0].Split(' ');
                    string panel = "";
                    if (elementName.Contains("ICS_"))
                    {
                        panel = "ICS";
                    } else if (elementName.Contains("MON_"))
                    {
                        panel = "Monitor";
                    } else
                    {
                        panel = "Unknown";
                    }
                    category = $"{panel} Control {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(origWords[0].ToLower())}{(origWords[0].ToLower() == "aug" ? " Crew" : "")}";
                    name = origName[0].Replace(origWords[0], "").Replace("ICS ", "").Replace("Mon", "").Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        name = origName[1];
                    }
                }
                else
                {
                    category = origName[0].Trim();
                    name = origName[1].Trim();
                }
            }
            else
            {
                if (origName[0].Contains("AMU Brightness"))
                {
                    string[] namePieces = origName[0].Split(' ');
                    category = $"{namePieces[1]} {namePieces[0]}";
                    name = $"{namePieces[2]} Switch"; ;
                }
                else if (origName[0].Contains("AMU LSK"))
                {
                    string[] namePieces = origName[0].Split(' ');
                    category = $"{namePieces[0]} {namePieces[2]}";
                    name = $"{namePieces[1]} {namePieces[3]} {namePieces[4]}"; ;
                }
                else if(origName[0].Contains("LSK"))
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
                else if (origDevice.Contains("_REF_MODE_PANEL"))
                {
                    category = $"Ref Mode {origName[0].Split(' ')[0]}";
                    name = $"{origName[0].Replace(origName[0].Split(' ')[0], "").Replace("Reference ", "").Trim()}";
                }
                else if (origName[0].Contains("ilot HUD"))
                {
                    category = origName[0].StartsWith("Pilot HUD") ? "Displays Pilot" : "Displays Copilot";
                    name = origName[0].Split(new char[] { ' ' }, 2)[1].Trim();
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
                else if (origDevice.Contains("VOLUME_MANAGER"))
                {
                    string panel = "";
                    if (elementName.Contains("ICS"))
                    {
                        panel = "ICS";
                    }
                    else if (elementName.Contains("MON"))
                    {
                        panel = "Monitor";
                    }
                    else
                    {
                        panel = "Unknown";
                    }

                    string[] origWords = origName[0].Split(' ');
                    category = $"{panel} Control {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(origWords[0].ToLower())}{(origWords[0].ToLower() == "aug" ? " Crew" : "")}";
                    name = origName[0].Replace(origWords[0], "").Trim();
                }
                else if (origName[0].Contains("Landing Gear"))
                {
                    category = "Landing Gear";
                    name = origName[0].Substring(origName[0].IndexOf("Landing Gear") + 12).Trim();
                }
                else if (string.IsNullOrEmpty(origName[0]))
                {
                    category = origDevice;
                    name = fd.Command[0].Replace("AMU.blank", "HUD Blank ").Replace("C_", "").Replace("P_", "");
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

            foreach (var pair in _nameSubstitutions)
            {
                name = name.Replace(pair.Key, pair.Value);
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
                { "Ap Interface", "AFCS" },
                { "Cms Mgr", "Counter Measure System" },
                { "Atm", "Environment" },
                { " Apu Ctrl", "" },
                { "Copilot Ref Mode Panel", "Ref Mode Copilot" },
                { "Pilot Ref Mode Panel", "Ref Mode Pilot" },
                { "Ref Panel", "Ref Mode Pilot" },
                { "FLCV", "Fuel" },
                { "Fuel System", "Fuel" },
                { "Air Conditioning","Environment"},
                { "Plane ",""},
                { "AUG ICS","ICS Control Aug Crew"},
                { "Radar","RADAR"},
                { "P_DISPLAYS","Displays Pilot"},
                { "C_DISPLAYS","Displays Copilot"},
                { "C_AMU.","Copilot HUD "},
                { "P_AMU.","Pilot HUD "},
                };
        }
        private static Dictionary<string, string> ElementInit()
        {
            return new Dictionary<string, string> {
                { "Brighness ","Brightness "},
                };
        }
        private static Dictionary<string, string> ClickableCorrectionsInit()
        {
            return new Dictionary<string, string> {
                { "Brighness ","Brightness "},
                { "PNT_CNBP_NUM8_168","PNT_CNBP_NUM8_169"},
                { "PNT_CNBP_NUM7_167","PNT_CNBP_NUM7_168"},
                { "ilot Radio Transmit","ilot Radio/Intercom Transmit"},
                { "AUG ICS - Radio","Aug Crew ICS - Radio/Intercom Transmit"},
                { "_ISCP_","_ICS_"},
                {"REF_MODE_PANEL.ref_select, 110, {-1, 1}", "REF_MODE_PANEL.ref_select, 110, {-0.8, 0.8}"},
                {"REF_MODE_PANEL.ref_select, 111, {-1, 1}", "REF_MODE_PANEL.ref_select, 111, {-0.8, 0.8}"},
                };
        }
        private static string SetClickables()
        {
            #region Process Clickables
            string path = $@"{_DCSAircraft}\clickabledata.lua";
            _pattern = @"^elements\[""PNT_(?'element'[^""]*?)(?:_(?<arg>\d{2,4}))*""\].*=\s*(?'function'.*)\((?:""(?'name'[^-,]*)(?:\s*-\s*(?'name'[^-,]*))*"")(((?:\s*,\s*devices\.(?'device'[^,\s\)]*))(?:\s*,\s*(?'command'[^,\s\)]*))(?:\s*,\s*((?'value'[^,\s]*)))*)|((?:\s*,\s*(?'command'[^,\s\)]*))(?:\s*,\s*(?'value'[^,\s\)]*))*)(?:\s*,\s*devices\.(?'device'[^,\s\)]*)))(?:\s*,\s*(?'value'[^,\s\)]*))*\s*\)\s*";
            string input = "";
            string clickableFilename = Path.GetFileNameWithoutExtension(path);
            using (StreamReader streamReader = new StreamReader(path))
            {
                input = streamReader.ReadToEnd();
            }
            // Console.Write(_input);
            #endregion
            return input;
        }
        private static void SetDevices()
        {
#if (DEBUG)
            #region Process Devices
            string path = $@"{_DCSAircraft}\devices.lua";
            _pattern = @"^devices\[""(?'device'.*)""\].*";
            string input = "";
            string clickableFilename = Path.GetFileNameWithoutExtension(path);
            using (StreamReader streamReader = new StreamReader(path))
            {
                input = streamReader.ReadToEnd();
            }
            _streamWriter = new StreamWriter($@"{Environment.GetEnvironmentVariable("userprofile")}\Documents\HeliosDev\Interfaces\devicesEnum.txt", false);
            _streamWriter.WriteLine("    internal enum devices\r\n    {\r\n");
            int i = 1;
            foreach (Match m in Regex.Matches(input, _pattern, _options))
            {
                _streamWriter.WriteLine($"     {m.Groups["device"].Value} = {i++},");
            }
            _streamWriter.WriteLine("    }");
            _streamWriter.Close();

            #endregion
#endif
        }
        private static string[,] LampsToArray()
        {
            return new string[,]
           { {"3990", "CNI Pilot", "Exec"},
            {"3992", "CNI Copilot", "Exec"},
            {"3994", "CNI Aug Crew", "Exec"},
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
            {"4030", "Hydraulics", "Auxiliary Pump On"},
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
            {"4045", "Ref Mode Pilot", "Master Warning"},
            {"4046", "Ref Mode Pilot", "Master Caution"},
            {"4047", "AFCS", "Mode ALT ON"},
            {"4048", "AFCS", "Mode VS ON"},
            {"4049", "AFCS", "Mode SEL ON"},
            {"4050", "AFCS", "Mode IAS ON"},
            {"4051", "AFCS", "Mode HDG ON"},
            {"4052", "AFCS", "Mode NAV ON"},
            {"4053", "AFCS", "Mode CAPS ON"},
            {"4054", "AFCS", "Mode APPR ON "},
            {"4055", "AFCS", "Mode A/T ON"},
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
            {"4107", "Fuel", "FLCV TEST ON"},
            {"4108", "Engine", "Bleed Air APU OPEN"},
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
        private static string[,] IndicationsToTextArray()
        {
            return new string[,]
           {
            {"2900", "Ref Mode Pilot", "Ref Mode Display", "Text Value"},     //  Pilot Ref Mode
            {"2901", "Ref Mode Copilot", "Ref Mode Display", "Text Value"},     //  Copilot Ref Mode
            {"2902", "Electrics", "Battery Voltage Display", "Text Value"},     //  Battery Voltage
            {"2905", "Fuel", "Total Amount Display", "Text Value"},     //  Fuel Total
            {"2906", "Fuel", "1 Main Amount Display", "Text Value"},     //  Fuel 1 Main
            {"2907", "Fuel", "2 Main Amount Display", "Text Value"},     //  Fuel 2 Main
            {"2908", "Fuel", "3 Main Amount Display", "Text Value"},     //  Fuel 3 Main
            {"2909", "Fuel", "4 Main Amount Display", "Text Value"},     //  Fuel 4 Main
            {"2910", "Fuel", "L Aux Amount Display", "Text Value"},     //  Fuel L Aux
            {"2911", "Fuel", "R Aux Amount Display", "Text Value"},     //  Fuel R Aux
            {"2912", "Fuel", "L Ext Amount Display", "Text Value"},     //  Fuel L Ext
            {"2913", "Fuel", "R Ext Amount Display", "Text Value"},     //  Fuel R Ext
            {"2914", "Engine", "APU % RPM Display", "Text Value"},     //  APU % RPM
            {"2915", "Engine", "APU EGT Display", "Text Value"},     //  APU EGT
            {"2916", "Engine", "Bleed Air Pressure Display", "Text Value"},     //  Bleed Air Pressure
            {"2917", "Environment", "Flight Deck Air Con Temp Display", "Text Value"},     //  Flight Deck Air Con Temp "84" "86"
            {"2918", "Environment", "Flight Deck Air Con Temp Set Display", "Text Value"},     //  Flight Deck Air Con Temp Set "84" "86"
            {"2919", "Environment", "Cargo Air Con Temp Display", "Text Value"},     //  Cargo Air Con  Temp "86" "86"
            {"2920", "Environment", "Cargo Air Con Temp Set Display", "Text Value"},     //  Cargo Air Con  Temp Set "86" "86"
            {"2921", "Environment", "Pressurization Rate Display", "Text Value"},     //  Pressurization Rate
            {"2922", "Environment", "Pressurization Cabin Alt Display", "Text Value"},     //  Pressurization Cabin Alt
            {"2923", "Environment", "Pressurization Difference Display", "Text Value"},     //  Pressurization Difference "88" "." "8"
            {"2924", "Environment", "LGD/CONST Display", "Text Value"},     //  LGD/CONST					 
            {"2925", "Fuel", "Pressure Display", "Text Value"},     //  Fuel Pressure				 
            {"2926", "Hydraulics", "Aux Hydraulic Pump Display", "Text Value"}};    //  Aux Hydraulic Pump
        }
        private static void WriteCsFunction(string fn)
        {
#if (DEBUG)
            if(!string.IsNullOrEmpty(fn)) _streamWriter.WriteLine(fn);
#endif
        } 
    }
    internal class FunctionData
    {
        internal string[] Name;
        internal string Fn;
        internal string ElementName;
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
