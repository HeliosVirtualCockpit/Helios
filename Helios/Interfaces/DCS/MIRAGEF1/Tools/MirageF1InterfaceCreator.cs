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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.UDPInterface;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.MIRAGEF1.Tools
{

    internal class MirageF1InterfaceCreator : InterfaceCreator, IInterfaceCreator
    {

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal MirageF1InterfaceCreator()
        {
            NetworkFunctions.Clear();
        }

        #region Interface Requirements
        public override void ProcessFunction(Match eM)
        {
            double modifier;
            string exportValue = "%0.1f";
            switch (eM.Groups["function"].Value)
            {
                case "guard_switch":
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", CommandItems[0][0]), new SwitchPosition((1 * modifier).ToString("F1"), "Posn 2", CommandItems[0][0]) }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"Posn 1\", \"{CommandItems[0][0]}\"),new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 2\", \"{CommandItems[0][0]}\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "default_3_position_tumb":
                    modifier = Arguments.Count >= 3 ? (Arguments[2].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0]), new SwitchPosition("0.0", "Posn 2", CommandItems[0][0]), new SwitchPosition((1 * modifier).ToString("F1"), "Posn 3", CommandItems[0][0]) }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 1\", \"{CommandItems[0][0]}\"),new SwitchPosition(\"0.0\", \"Posn 2\", \"{CommandItems[0][0]}\"),new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 3\", \"{CommandItems[0][0]}\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;

                case "circuit_breaker":
                case "default_2_position_tumb":
                    modifier = Arguments.Count >= 6 ? (Arguments[5].Value == "true" ? -1 : 1) : 1;
                    if (!eM.Groups["name"].Value.ToLower().Contains("button"))
                    {
                        AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0]), new SwitchPosition("0.0", "Posn 2", CommandItems[0][0]) }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                        AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 1\", \"{CommandItems[0][0]}\"),new SwitchPosition(\"0.0\", \"Posn 2\", \"{CommandItems[0][0]}\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    }
                    else
                    {
                        AddFunction(new PushButton(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, SectionName, eM.Groups["name"].Value, "%1d"));
                        AddFunctionList.Add($"AddFunction(new PushButton(this, {Devices[1]}, \"{CommandItems[0][0]}\", \"{eM.Groups["arg"].Value}\", \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%1d\"));");
                    }
                    break;

                case "springloaded_2pos_switch":
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0], CommandItems[0][0], "0.0"), new SwitchPosition((1 * modifier).ToString("F1"), "Posn 2", CommandItems[1][0], CommandItems[1][0], "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1.0 * modifier:F1}\", \"Posn 1\", \"{CommandItems[0][0]}\",\"{CommandItems[0][0]}\",\"0.0\"), new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 2\", \"{CommandItems[1][0]}\",\"{CommandItems[1][0]}\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;

                case "default_button":
                    AddFunction(new PushButton(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, SectionName, eM.Groups["name"].Value, "%1d"));
                    AddFunctionList.Add($"AddFunction(new PushButton(this, {Devices[1]}, \"{CommandItems[0][0]}\", \"{eM.Groups["arg"].Value}\", \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%1d\"));");
                    break;

                case "default_button_axis":
                    AddFunction(new PushButton(UdpInterface, Devices[0], CommandItems[0][0], Arguments[0].Value, SectionName, $"Button {eM.Groups["name"].Value}", "%1d"));
                    AddFunctionList.Add($"AddFunction(new PushButton(this, {Devices[1]}, \"{CommandItems[0][0]}\", \"{Arguments[0].Value}\", \"{SectionName}\", \"Button {eM.Groups["name"].Value}\", \"%1d\"));");
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[1][0], Arguments[1].Value, 0.5d, 0.0d, 1.0d, SectionName, $"Lamp {eM.Groups["name"].Value}", false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, \"{CommandItems[1][0]}\", \"{Arguments[1].Value}\", 0.5d, 0.0d, 1.0d, \"{SectionName}\", \"Lamp {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                case "default_tumb_button":
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 1", CommandItems[1][0], CommandItems[1][0], "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((1 * modifier).ToString("F1"), "Posn 3", CommandItems[0][0], CommandItems[0][0], "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 1\", \"{CommandItems[0][0]}\",\"{CommandItems[0][0]}\",\"0.0\"), new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 2\", null),new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 3\", \"{CommandItems[0][0]}\",\"{CommandItems[0][0]}\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "default_button_tumb":
                    modifier = Arguments.Count >= 2 ? (Arguments[1].Value == "true" ? -1 : 1) : 1;
                    AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), "Posn 1", CommandItems[0][0], CommandItems[0][0], "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((-1 * modifier).ToString("F1"), "Posn 3", CommandItems[1][0], CommandItems[1][0], "0.0", "0.0") }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 1\", \"{CommandItems[0][0]}\",\"{CommandItems[0][0]}\",\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"Posn 2\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 3\", \"{CommandItems[1][0]}\",\"{CommandItems[1][0]}\",\"0.0\",\"0.0\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    break;
                case "multiposition_switch":
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
                        double.TryParse(Arguments[5].Value == "nil" ? "0.0" : Arguments[5].Value, out double startValue)
                        && stepValue != 0)
                    {
                        AddFunction(new Switch(UdpInterface, Devices[0], eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, CommandItems[0][0], "Posn", exportValue), SectionName, eM.Groups["name"].Value, exportValue));
                        AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, \"{CommandItems[0][0]}\", \"Posn\", \"{exportValue}\"), \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");

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
                        AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, \"{CommandItems[0][0]}\", \"Posn\", \"{exportValue}\"), \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");
                    }
                    else
                    {
                        Logger.Warn($"Unable to create function {eM.Groups["function"].Value} for {eM.Value}.  Unexpected element Arguments.");
                    }
                    break;
                case "default_axis":
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, double.Parse(Arguments[2].Value), 0.0d, double.Parse(Arguments[1].Value), SectionName, eM.Groups["name"].Value, false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, \"{CommandItems[0][0]}\", \"{eM.Groups["arg"].Value}\", {Arguments[2].Value}d, 0.0d, {Arguments[1].Value}d, \"{SectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                case "default_axis_limited":
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[0][0], eM.Groups["arg"].Value, 0.1d, 0.0d, 1.0d, SectionName, eM.Groups["name"].Value, false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, \"{CommandItems[0][0]}\", \"{eM.Groups["arg"].Value}\", 0.1d, 0.0d, 1.0d, \"{SectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                case "intercom_rotate_tumb":
                    AddFunction(new Switch(UdpInterface, Devices[0], Arguments[0].Value, new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", CommandItems[0][0]), new SwitchPosition("1.0", "Posn 2", CommandItems[0][0]) }, SectionName, eM.Groups["name"].Value, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Switch(this, {Devices[1]}, \"{Arguments[0].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"Posn 1\", \"{CommandItems[0][0]}\"),new SwitchPosition(\"1.0\", \"Posn 2\", \"{CommandItems[0][0]}\")}}, \"{SectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                    AddFunction(new Axis(UdpInterface, Devices[0], CommandItems[1][0], Arguments[1].Value, 0.1d, 0.0d, 1.0d, SectionName, $"Rotate {eM.Groups["name"].Value}", false, "%0.1f"));
                    AddFunctionList.Add($"AddFunction(new Axis(this, {Devices[1]}, \"{CommandItems[1][0]}\", \"{Arguments[1].Value}\", 0.1d, 0.0d, 1.0d, \"{SectionName}\", \"Rotate {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                    break;
                default:
                    Logger.Warn($"Unknown function encountered while creating interface: {eM.Groups["function"].Value}");
                    break;
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
        public override MatchCollection GetSections(string clickablesFromDCS)
        {
            string pattern = @"[\-]{62,66}[\r\n]+-- (?<deviceName>[^\r]*)[\s\n\r]+(?:(?![\-]{62,66}[\r\n]+--)[\s\S])*";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            return Regex.Matches(clickablesFromDCS, pattern, options);
        }
        public override MatchCollection GetElements(string section)
        {
            string pattern = @"(?<!--)elements\[""PNT-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Z0-9]*)[,\s]*(?:devCmds\.Cmd(?<command>[0-9]{1,4})[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_/\*]*)[\,\s\)]+)+[\r\n\s]{1}";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            return Regex.Matches(section, pattern, options);
        }
        #endregion
    }
}
