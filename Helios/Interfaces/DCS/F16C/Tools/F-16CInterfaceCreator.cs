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
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.UDPInterface;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Web;
using static GadrocsWorkshop.Helios.NativeMethods;
using System.Windows.Shapes;
using System.Linq;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.F16C.Tools
{

    internal class F16CInterfaceCreator: InterfaceCreator, IInterfaceCreator
    {

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal F16CInterfaceCreator()
        {
            NetworkFunctions.Clear();
        }

        protected override void FunctionRouter(BaseUDPInterface UdpInterface, Match eM)
        {

        }
        #region Interface Contract
        public override MatchCollection GetSections(string clickablesFromDCS)
        {
            string pattern = @"[\-]{2}\s*(?<deviceName>[^\r\n]*)(?:(?![\-]{2})[\s\S])*";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            return Regex.Matches(clickablesFromDCS, pattern, options);
        }
        public override MatchCollection GetElements(string section)
        {
            string pattern = @"(?<!--)elements\[""PTR-.*-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Z0-9_]*)[,\s]*((?<commandName>[a-zA-Z0-9]+)(?:_commands.)(?<command>[a-zA-Z0-9_]+)[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_/\*]*)[\,\s\)]+)+[\r\n\s]{1}";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            return Regex.Matches(section, pattern, options);
        }

        public override void ProcessFunction(Match eM)
        {
            string argPattern = @"(?<name>.*)(?:\,+\s*)(((?<position>[a-zA-z0-9\-\s]+)(\/|$))+)";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;

            string exportValue = "%0.1f";
            double modifier;
            string[] posnName = new string[0];
            switch (eM.Groups["function"].Value)
            {
                case "default_red_cover": // elements["PTR-FUELCP-CVR-MASTER-558"]	= default_red_cover(_("FUEL MASTER Switch Cover, OPEN/CLOSE"), devices.FUEL_INTERFACE, fuel_commands.FuelMasterSwCvr, 558)
                case "guard_switch":
                    posnName = FindPositionNames(eM.Groups["name"].Value);
                    if (posnName.Length != 2)
                    {
                        posnName = new string[2];
                        posnName[0] = "Posn 1";
                        posnName[1] = "Posn 2";
                    }
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("0.0", posnName[0], CommandItems[0][0]), new SwitchPosition((1 * modifier).ToString("F1"), posnName[1], CommandItems[0][0]) }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"{posnName[0]}\", {CommandItems[0][1]}),new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[1]}\", {CommandItems[0][1]})}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "default_3_position_tumb_small": // elements["PTR-CLCP-TMB-ROLL-108"] = default_3_position_tumb_small(_("Autopilot ROLL Switch, STRG SEL/ATT HOLD/HDG SEL"), devices.CONTROL_INTERFACE, control_commands.ApRoll, 108)
                case "default_3_position_tumb":
                    ProcessDefault3PositionTumb(UdpInterface, SectionName, eM, Devices, CommandItems, Arguments);
                    break;

                case "Rocker_switch_positive": // elements["PTR-ICP-RS-OFF-UP-183"]	= Rocker_switch_positive(_("ICP DED Increment/Decrement Switch, Up"),devices.UFC, ufc_commands.DED_INC,183)
                case "Rocker_switch_negative": // elements["PTR-ICP-RS-OFF-DN-183"]	= Rocker_switch_negative(_("ICP DED Increment/Decrement Switch, Down"),devices.UFC, ufc_commands.DED_DEC,183)
                                               // rockers are two elements but need to result in a single Helios function
                    if (TwoPartElement.TryGetValue(int.Parse(eM.Groups["arg"].Value), out string firstPart))
                    {
                        string argName;
                        MatchCollection rockerMatches = GetElements(firstPart);
                        if (rockerMatches.Count > 0)
                        {
                            Type enumType = typeof(F16CCommands).GetNestedType($"{eM.Groups["commandName"].Value}Commands", BindingFlags.NonPublic);
                            Match rockerMatch = rockerMatches[0];
                            CommandItems[0][0] = ((int)Enum.Parse(enumType, rockerMatch.Groups["command"].Value)).ToString("d");
                            CommandItems[0][1] = $"F16CCommands.{rockerMatch.Groups["commandName"].Value}Commands.{rockerMatch.Groups["command"].Value}.ToString(\"d\")";
                            string[] temp = rockerMatch.Groups["name"].Value.Split(',');
                            argName = temp[0];
                            posnName = new string[3];
                            posnName[0] = temp.Length == 2 ? temp[1].Trim() : "Posn 1";
                            posnName[1] = "Middle";
                            temp = eM.Groups["name"].Value.Split(',');
                            posnName[2] = temp.Length == 2 ? temp[1].Trim() : "Posn 3";
                            CommandItems[1][0] = ((int)Enum.Parse(enumType, eM.Groups["command"].Value)).ToString("d");
                            CommandItems[1][1] = $"F16CCommands.{eM.Groups["commandName"].Value}Commands.{eM.Groups["command"].Value}.ToString(\"d\")";

                            modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                            AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], CommandItems[0][0], CommandItems[0][0], "0.0", "0.0"), new SwitchPosition("0.0", posnName[1], null), new SwitchPosition((-1 * modifier).ToString("F1"), posnName[2], CommandItems[1][0], CommandItems[1][0], "0.0", "0.0") }, SectionName, argName, "%0.1f"));
                            AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"{posnName[1]}\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"{posnName[2]}\", {CommandItems[1][1]},{CommandItems[1][1]},\"0.0\",\"0.0\")}}, \"{SectionName}\", \"{argName}\", \"%0.1f\"));");
                        }
                        TwoPartElement.Remove(int.Parse(eM.Groups["arg"].Value));
                    }
                    else
                    {
                        TwoPartElement.Add(int.Parse(eM.Groups["arg"].Value), eM.Value);
                    }

                    break;
                case "springloaded_3_pos_tumb": // elements["PTR-CLCP-TMB-PITCH-109"] = springloaded_3_pos_tumb(_("Autopilot PITCH Switch, ATT HOLD/ A/P OFF/ ALT HOLD"), devices.CONTROL_INTERFACE, control_commands.ApPitchAtt, control_commands.ApPitchAlt, 109)
                case "springloaded_3_pos_tumb_small": // elements["PTR-RLGT-TMB-MALIND-691"]		= springloaded_3_pos_tumb_small(_("MAL & IND LTS Switch, BRT/Center/DIM"),	devices.CPTLIGHTS_SYSTEM, cptlights_commands.MalIndLtsDim, cptlights_commands.MalIndLtsBrt,	691)
                    posnName = FindPositionNames(eM.Groups["name"].Value);
                    if (posnName.Length != 3)
                    {
                        posnName = new string[3];
                        posnName[0] = "Posn 1";
                        posnName[1] = "Posn 2";
                        posnName[2] = "Posn 3";
                    }
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], CommandItems[0][0], CommandItems[0][0], "0.0", "0.0"), new SwitchPosition("0.0", posnName[1], null), new SwitchPosition((-1 * modifier).ToString("F1"), posnName[2], CommandItems[1][0], CommandItems[1][0], "0.0", "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"{posnName[1]}\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"{posnName[2]}\", {CommandItems[1][1]},{CommandItems[1][1]},\"0.0\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "springloaded_2_pos_tumb": // elements["PTR-FLTCP-TMB-BIT-574"]= springloaded_2_pos_tumb(_("BIT Switch, OFF/BIT"),devices.CONTROL_INTERFACE, control_commands.BitSw,574)
                case "springloaded_2_pos_tumb_small": // elements["PTR-FLTCP-TMB-FLCS-573"]= springloaded_2_pos_tumb_small(_("FLCS RESET Switch, OFF/RESET"),devices.CONTROL_INTERFACE, control_commands.FlcsReset,573)
                case "default_2_position_tumb_small":  // elements["PTR-LGCP-TMB-STCONF-358"]= default_2_position_tumb_small(_("STORES CONFIG Switch, CAT III/CAT I"),devices.CONTROL_INTERFACE, control_commands.StoresConfig,358)
                case "circuit_breaker":
                case "default_2_position_tumb":
                    ProcessDefault2PositionTumb(UdpInterface, SectionName, eM, Devices, CommandItems, Arguments);
                    break;

                case "springloaded_2pos_switch":
                    posnName = FindPositionNames(eM.Groups["name"].Value);
                    if (posnName.Length != 2)
                    {
                        posnName = new string[2];
                        posnName[0] = "Posn 1";
                        posnName[1] = "Posn 2";
                    }
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), posnName[0], CommandItems[0][0], CommandItems[0][0], "0.0"), new SwitchPosition((1 * modifier).ToString("F1"), posnName[1], CommandItems[1][0], CommandItems[1][0], "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1.0 * modifier:F1}\", \"{posnName[0]}\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\"), new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[1]}\", {CommandItems[1][1]},{CommandItems[1][1]},\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "short_way_button": // elements["PTR-ICP-BTN-NMB1-171"]	= short_way_button(_("ICP Priority Function Button, 1(T-ILS)"),devices.UFC, ufc_commands.DIG1_T_ILS,171)
                case "mfd_button":
                case "default_button":
                    ProcessDefaultButton(UdpInterface, SectionName, eM, Devices, CommandItems, Arguments);
                    break;
                case "default_button_knob": // elements["PTR-EHSI-LVR-CRS-44"]	= default_button_knob(_("CRS Set / Brightness Control Knob"),	devices.EHSI, ehsi_commands.RightKnobBtn, ehsi_commands.RightKnob, 43, 44)
                case "default_button_axis":
                    AddFunction(new PushButton(UdpInterface, Devices[0], CommandItems[0][0], Arguments[0].Value, SectionName, $"Button {eM.Groups["name"].Value}", "%1d"));
                    AddFunctionList.Add($"AddFunction(new PushButton(this, {Devices[1]}, {CommandItems[0][1]}, \"{Arguments[0].Value}\", \"{SectionName}\", \"Button {eM.Groups["name"].Value}\", \"%1d\"));");
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[1][0], Arguments[1].Value, 0.5d, 0.0d, 1.0d, SectionName, $"Lamp {eM.Groups["name"].Value}", false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[1][1]}, \"{Arguments[1].Value}\", 0.5d, 0.0d, 1.0d, \"{SectionName}\", \"Lamp {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                case "default_tumb_button":
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 1", CommandItems[1][0], CommandItems[1][0], "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((1 * modifier).ToString("F1"), "Posn 3", CommandItems[0][0], CommandItems[0][0], "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 1\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\"), new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 2\", null),new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 3\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "default_button_tumb":
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0], CommandItems[0][0], "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 3", CommandItems[1][0], CommandItems[1][0], "0.0", "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 1\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"Posn 2\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 3\", {CommandItems[1][1]},{CommandItems[1][1]},\"0.0\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "multiposition_switch_tumb":  // elements["PTR-ANARC164-CHNL-SELECTOR-410"] = multiposition_switch_tumb(_("UHF CHAN Knob"), devices.UHF_CONTROL_PANEL, uhf_commands.ChannelKnob, 410, 20, 0.05, NOT_INVERSED, 0.0, anim_speed_default * 0.05, NOT_CYCLED)
                case "multiposition_switch":
                    string[] positionNames = new string[] { };
                    MatchCollection argMatches = Regex.Matches(eM.Groups["name"].Value, argPattern, options);
                    if (argMatches.Count > 0 && argMatches[0].Groups.Count > 1)
                    {
                        positionNames = argMatches[0].Groups[1].Value.Split('/');

                    }
                    if (!double.TryParse(Arguments[2].Value, out double stepValue))
                    {
                        if (Arguments[2].Value.Contains("/"))
                        {
                            string[] numberPortion = Arguments[2].Value.Split('/');
                            stepValue = Math.Round((double.Parse(numberPortion[0]) / double.Parse(numberPortion[1])), 3);
                            exportValue = "%0.3f";
                        }
                        else
                        {
                            stepValue = 0;
                        }
                    }
                    else
                    {
                        exportValue = $"%0.{Arguments[2].Value.Length - 2}f";
                    }
                    if (int.TryParse(Arguments[1].Value, out int stepCount) &&
                        double.TryParse(Arguments[4].Value == "nil" ? "0.0" : Arguments[4].Value, out double startValue)
                        && stepValue != 0)
                    {
                        if (positionNames.Length != stepCount)
                        {
                            AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, CommandItems[0][0], "Posn", exportValue), SectionName, eM.Groups["name"].Value, exportValue));
                            AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, {CommandItems[0][1]}, \"Posn\", \"{exportValue}\"), \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");

                        }
                        else
                        {
                            AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, CommandItems[0][0], flatten(positionNames), exportValue), SectionName, eM.Groups["name"].Value, exportValue));
                            AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, {CommandItems[0][1]}, {flatten(positionNames)}, \"{exportValue}\"), \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");

                        }

                    }
                    else
                    {
                        Logger.Warn($"Unable to create function {eM.Groups["function"].Value} for {eM.Value}.  Unexpected element Arguments.");
                    }
                    break;
                case "multiposition_switch_cycled_relative":
                    if (!double.TryParse(Arguments[2].Value, out stepValue))
                    {
                        if (Arguments[2].Value.Contains("/"))
                        {
                            string[] numberPortion = Arguments[2].Value.Split('/');
                            stepValue = Math.Round((double.Parse(numberPortion[0]) / double.Parse(numberPortion[1])), 3);
                            exportValue = "%0.3f";
                        }
                        else
                        {
                            stepValue = 0;
                        }
                    }
                    else
                    {
                        exportValue = $"%0.{Arguments[2].Value.Length - 2}f";
                    }
                    if (int.TryParse(Arguments[1].Value, out stepCount) &&
                        double.TryParse(Arguments[4].Value == "nil" ? "0.0" : Arguments[4].Value, out startValue)
                        && stepValue != 0)
                    {
                        AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, CommandItems[0][0], "Posn", exportValue), SectionName, eM.Groups["name"].Value, exportValue));
                        AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, {CommandItems[0][1]}, \"Posn\", \"{exportValue}\"), \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");
                    }
                    else
                    {
                        Logger.Warn($"Unable to create function {eM.Groups["function"].Value} for {eM.Value}.  Unexpected element Arguments.");
                    }
                    break;
                case "default_axis":
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, double.Parse(Arguments[2].Value), 0.0d, double.Parse(Arguments[1].Value), SectionName, eM.Groups["name"].Value, false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[0][1]}, \"{eM.Groups["arg"].Value}\", {Arguments[2].Value}d, 0.0d, {Arguments[1].Value}d, \"{SectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                case "default_axis_limited_1_side": // elements["PTR-MANTRIM-LVR-ROLL-560"]	= default_axis_limited_1_side(_("ROLL TRIM Wheel"), devices.CONTROL_INTERFACE, control_commands.RollTrim, 560, 0.0, 0.1, NOT_UPDATABLE, NOT_RELATIVE, {-1,1}, {0,160},{0,-135})
                case "default_axis_limited":
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, 0.1d, 0.0d, 1.0d, SectionName, eM.Groups["name"].Value, false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[0][1]}, \"{eM.Groups["arg"].Value}\", 0.1d, 0.0d, 1.0d, \"{SectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                case "intercom_rotate_tumb":
                    AddFunction(new Switch(UdpInterface, Devices[0], Arguments[0].Value, new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", CommandItems[0][0]), new SwitchPosition("1.0", "Posn 2", CommandItems[0][0]) }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{Arguments[0].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"Posn 1\", {CommandItems[0][1]}),new SwitchPosition(\"1.0\", \"Posn 2\", {CommandItems[0][1]})}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[1][0], Arguments[1].Value, 0.1d, 0.0d, 1.0d, SectionName, $"Rotate {eM.Groups["name"].Value}", false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[1][1]}, \"{Arguments[1].Value}\", 0.1d, 0.0d, 1.0d, \"{SectionName}\", \"Rotate {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                default:
                    FunctionRouter(UdpInterface, eM);
                    break;
            }
        }

        #endregion
        protected override string[][] ParseCommandGroup(Match match)
        {
            Group cmd = match.Groups["command"];
            Group cmdName = match.Groups["commandName"];
            string enumValueSuffix = "Commands";
            Type typeEnumClass = typeof(F16CCommands);
            string[][] cmds = { new string[2], new string[2] };
            Type enumType = typeEnumClass.GetNestedType($"{cmdName.Value}{enumValueSuffix}", BindingFlags.NonPublic);
            string commandName = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Value}{enumValueSuffix}.";
            if (cmd.Captures.Count == 1)
            {
                cmds[0][0] = ((int)Enum.Parse(enumType, cmd.Value)).ToString("d");
                cmds[0][1] = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Value}{enumValueSuffix}.{cmd.Value}.ToString(\"d\")";
            }
            else
            {
                cmds[0][0] = ((int)Enum.Parse(enumType, cmd.Captures[0].Value)).ToString("d");
                cmds[0][1] = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Value}{enumValueSuffix}.{cmd.Captures[0].Value}.ToString(\"d\")";
                cmds[1][0] = ((int)Enum.Parse(enumType, cmd.Captures[1].Value)).ToString("d");
                cmds[1][1] = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Value}{enumValueSuffix}.{cmd.Captures[1].Value}.ToString(\"d\")";
            }
            return cmds;
        }
        public override string[] ParseDeviceGroup(Group deviceGroup)
        {
            string[] device = new string[2];
            if (Enum.TryParse(deviceGroup.Value, out devices dev))
            {
                device[0] = dev.ToString("d");
                device[1] = $"devices.{deviceGroup.Value}.ToString(\"d\")";
            }
            return device;
        }

    }

}
