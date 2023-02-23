//  Copyright 2023 Helios Contributors
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
using GadrocsWorkshop.Helios.Interfaces.DCS.H60;
using System.Drawing.Printing;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Tools
{

    internal class H60InterfaceCreation : InterfaceCreation, IInterfaceCreation
    {

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal H60InterfaceCreation()
        {
            NetworkFunctions.Clear();
        }

        #region Interface Requirements
        public override void ProcessFunction(Match eM)
        {
            if (!eM.Groups["name"].Value.Contains("(Inop.)"))
            {
                string argPattern = @"(?:((?<position>[A-Z0-9\-\s-[ ]]+)(\/|$))+)";
                RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
                double modifier;
                string exportValue = "%0.1f";
                string[] posnName = new string[0];
                switch (eM.Groups["function"].Value)
                {
                    case "default_red_cover":
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
                    case "default_3_position_tumb_small":
                    case "default_3_position_tumb":
                        ProcessDefault3PositionTumb(UdpInterface, SectionName, eM, Devices, CommandItems, Arguments);
                        break;

                    case "Rocker_switch_positive":
                    case "Rocker_switch_negative":
                                                  
                        if (TwoPartElement.TryGetValue(int.Parse(eM.Groups["arg"].Value), out string firstPart))
                        {
                            string argName;
                            MatchCollection rockerMatches = GetElements(firstPart);
                            if (rockerMatches.Count > 0)
                            {
                                Type enumType = typeof(H60Commands).GetNestedType($"{eM.Groups["commandName"].Value}Commands", BindingFlags.NonPublic);
                                Match rockerMatch = rockerMatches[0];
                                CommandItems[0][0] = ((int)Enum.Parse(enumType, rockerMatch.Groups["command"].Value)).ToString("d");
                                CommandItems[0][1] = $"H60Commands.{rockerMatch.Groups["commandName"].Value}Commands.{rockerMatch.Groups["command"].Value}.ToString(\"d\")";
                                string[] temp = rockerMatch.Groups["name"].Value.Split(',');
                                argName = temp[0];
                                posnName = new string[3];
                                posnName[0] = temp.Length == 2 ? temp[1].Trim() : "Posn 1";
                                posnName[1] = "Middle";
                                temp = eM.Groups["name"].Value.Split(',');
                                posnName[2] = temp.Length == 2 ? temp[1].Trim() : "Posn 3";
                                CommandItems[1][0] = ((int)Enum.Parse(enumType, eM.Groups["command"].Value)).ToString("d");
                                CommandItems[1][1] = $"H60Commands.{eM.Groups["commandName"].Value}Commands.{eM.Groups["command"].Value}.ToString(\"d\")";

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
                    case "springloaded_3_pos_tumb2":
                    case "springloaded_3_pos_tumb":
                    case "springloaded_3_pos_tumb_small":
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
                    case "springloaded_2_pos_tumb":
                    case "springloaded_2_pos_tumb_small":
                    case "default_2_position_tumb_small":
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
                    case "short_way_button":
                    case "mfd_button":
                    case "push_button_tumb":
                    case "default_button":
                        ProcessDefaultButton(UdpInterface, SectionName, eM, Devices, CommandItems, Arguments);
                        break;
                    case "default_button_knob":
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
                    case "default_button_tumb_v2_inverted":
                        modifier = -1;
                        AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0], CommandItems[0][0], "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 3", CommandItems[1][0], CommandItems[1][0], "0.0", "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                        AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 1\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"Posn 2\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 3\", {CommandItems[1][1]},{CommandItems[1][1]},\"0.0\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                        break;
                    case "default_button_tumb":
                        modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                        AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0], CommandItems[0][0], "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 3", CommandItems[1][0], CommandItems[1][0], "0.0", "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                        AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 1\", {CommandItems[0][1]},{CommandItems[0][1]},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"Posn 2\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 3\", {CommandItems[1][1]},{CommandItems[1][1]},\"0.0\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                        break;
                    case "wiper_selector":
                    case "multiposition_switch_relative":
                    case "multiposition_switch_tumb":
                    case "multiposition_switch":
                        int argModifier = 0;
                        double stepValue = 0;
                        string[] positionNames = new string[] { };
                        MatchCollection argMatches = Regex.Matches(eM.Groups["name"].Value, argPattern, options);
                        if (argMatches.Count > 0 && argMatches[0].Groups.Count > 1)
                        {
                            positionNames = argMatches[0].Groups[0].Value.Split('/');
                        }
                        if (!double.TryParse(Arguments[2 + argModifier].Value, out stepValue))
                        {
                            if (Arguments[2 + argModifier].Value.Contains("/"))
                            {
                                string[] numberPortion = Arguments[2 + argModifier].Value.Split('/');
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
                            exportValue = $"%0.{Arguments[2 + argModifier].Value.Length - 2}f";
                        }
                        if (int.TryParse(Arguments[1 + argModifier].Value, out int stepCount) &&
                            double.TryParse(Arguments[4 + argModifier].Value == "nil" ? "0.0" : Arguments[4 + argModifier].Value, out double startValue)
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
                            Logger.Warn($"Unable to create function {eM.Groups["function"].Value} for {eM.Value}.  Unexpected element Arguments.  Actual number of arguments: {Arguments.Count}.");
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
                        if (Arguments.Count >= 3)
                        {
                            AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, double.Parse(Arguments[2].Value), 0.0d, double.Parse(Arguments[1].Value), SectionName, eM.Groups["name"].Value, false, "%0.1f"));
                            AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[0][1]}, \"{eM.Groups["arg"].Value}\", {Arguments[2].Value}d, 0.0d, {Arguments[1].Value}d, \"{SectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                        } else
                        {
                            AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, 0.1d, 0.0d, 1.0d, SectionName, eM.Groups["name"].Value, false, "%0.1f"));
                            AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[0][1]}, \"{eM.Groups["arg"].Value}\", 0.1d, 0.0d, 1.0d, \"{SectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                        }
                        break;
                    case "default_axis_limited_1_side":
                    case "default_axis_limited":
                        string dupFix = "";
                        if (eM.Groups["arg"].Value == "267") { dupFix = "Copilot "; }
                        AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, 0.1d, 0.0d, 1.0d, SectionName, dupFix + eM.Groups["name"].Value, false, "%0.1f"));
                        AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, {CommandItems[0][1]}, \"{eM.Groups["arg"].Value}\", 0.1d, 0.0d, 1.0d, \"{SectionName}\", \"{dupFix}{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
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
            else
            {
                Logger.Debug($"Skipping element {eM.Groups["name"].Value} because it appears to be inoperable.");
            }
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
        protected override string[][] ParseCommandGroup(Match match)
        {
            Group cmd = match.Groups["command"];
            Group cmdName = match.Groups["commandName"];
            string[][] cmds = { new string[2], new string[2] };

            if (!match.Groups["name"].Value.Contains("(Inop.)")){
                string enumValueSuffix = "";
                Type typeEnumClass = typeof(H60Commands);
                Type enumType = typeEnumClass.GetNestedType($"{cmdName.Captures[0].Value}{enumValueSuffix}", BindingFlags.NonPublic);
                string commandName = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Captures[0].Value}{enumValueSuffix}.";
                if (cmd.Captures.Count == 1)
                {
                    cmds[0][0] = ((int)Enum.Parse(enumType, cmd.Captures[0].Value)).ToString("d");
                    cmds[0][1] = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Captures[0].Value}{enumValueSuffix}.{cmd.Captures[0].Value}.ToString(\"d\")";
                }
                else
                {
                    cmds[0][0] = ((int)Enum.Parse(enumType, cmd.Captures[0].Value)).ToString("d");
                    cmds[0][1] = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Captures[0].Value}{enumValueSuffix}.{cmd.Captures[0].Value}.ToString(\"d\")";
                    cmds[1][0] = ((int)Enum.Parse(enumType, cmd.Captures[1].Value)).ToString("d");
                    cmds[1][1] = $"{(typeEnumClass.Name == "" ? "" : typeEnumClass.Name + ".")}{cmdName.Captures[1].Value}{enumValueSuffix}.{cmd.Captures[1].Value}.ToString(\"d\")";
                }

            }
            return cmds;
        }
        protected override void ProcessDefault2PositionTumb(BaseUDPInterface iface, string sectionName, Match eM, string[] devices, string[][] commandItems, CaptureCollection arguments)
        {
            string dupFix = "";
            if (eM.Groups["arg"].Value == "301" || eM.Groups["arg"].Value == "302" || eM.Groups["arg"].Value == "303") { 
                dupFix = eM.Groups["command"].Captures[0].Value.Substring(0,3);
                if (dupFix == "pil") { dupFix = "Pilot "; } 
                else if (dupFix == "cop") { dupFix = "Copilot "; } 
                else if (dupFix == "cdu") { dupFix = "CDU "; } 
                else { dupFix = ""; }
            }
            string[] posnName = FindPositionNames(eM.Groups["name"].Value);
            if (posnName.Length != 2)
            {
                posnName = new string[2];
                posnName[0] = "Posn 1";
                posnName[1] = "Posn 2";
            }
            double modifier = arguments.Count >= 6 ? (arguments[5].Value == "true" ? -1 : 1) : 1;
            AddFunction(new Switch(iface, devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], commandItems[0][0]), new SwitchPosition("0.0", posnName[1], commandItems[0][0]) }, sectionName, dupFix+eM.Groups["name"].Value, "%0.1f"));
            AddFunctionList.Add($"AddFunction(new Switch(this, {devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {commandItems[0][1]}),new SwitchPosition(\"0.0\", \"{posnName[1]}\", {commandItems[0][1]})}}, \"{sectionName}\", \"{dupFix}{eM.Groups["name"].Value}\", \"%0.1f\"));");
        }

        public override MatchCollection GetSections(string clickablesFromDCS)
        {
            string pattern = @"(?'startcomment'^--)(?<deviceName>[.\t\s\S-[\)]]*)(?'-startcomment'\n)(?:(?<elements>^elements.*)[\n]{1,2}|(?:--.*[\)\.\?]{1}\n)*)*";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            return Regex.Matches(clickablesFromDCS, pattern, options);
        }
        public override MatchCollection GetElements(string section)
        {
            string pattern = @"(?<!--)elements\[""PNT.*-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Za-z0-9_]*)[,\s]*((?<commandName>[a-zA-Z0-9_]+)\.(?<command>[a-zA-Z0-9_]+)[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_/\*]*)[\,\s\)]+)+[\r\n\s]{1}";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            return Regex.Matches(section, pattern, options);
        }
        #endregion
    }
}
