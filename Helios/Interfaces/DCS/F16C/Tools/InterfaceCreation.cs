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

    internal class InterfaceCreation
    {

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<string> _functionList = new List<string>();
        private List<string> _addFunctionList = new List<string>();
        private List<string> _unknownFunctions = new List<string>();
        private Dictionary<int,string> _twoPartElement = new Dictionary<int,string>();
        private NetworkFunctionCollection _networkFunctions = new NetworkFunctionCollection();

        internal InterfaceCreation()
        {
            _networkFunctions.Clear();
        }

        internal NetworkFunctionCollection CreateFunctionsFromClickable(BaseUDPInterface iface, string path)
        {
            Logger.Debug($"\n// * * * Creating Interface functions from file: {path}\n");
            _networkFunctions.Clear();
            _addFunctionList.Clear();
            _functionList.Clear();

            string input;
            using (StreamReader streamReader = new StreamReader(path))
            {
                input = streamReader.ReadToEnd();
            }
            return CreateFunctions(iface, input);
        }
        /// <summary>
        /// Decodes clickable elements into Helios functions 
        /// </summary>
        /// <param name="iface">UDP Interface needed for the functions to be added to</param>
        /// <param name="clickable">Single string containing the clickabledata file</param>
        /// <returns>NetworkFunctionCollection containing all of the functions which could be deciphered</returns>
        /// <remarks>
        /// The format of the clickableData file is Lua, but varies significantly from module to module
        /// so it is anticipated that changes to the regex's and functions might be necessary since the 
        /// sections and switch positions rely on the hope that the module author has been consistantly writing
        /// their code, comments and function names(!?!?!)
        /// clickable elements can also be overeridden/extended by the module author, so parameter parsing can 
        /// also be module specific.
        /// Results are also written to an external file whose contents can be copied into the DCS
        /// interface file to have a record of the decoding once interface creation has stabilised.
        /// *** Note *** there are complex elements in most modules which are not identified or processed by
        /// this class and will typically need to be manually converted to Helios.
        /// </remarks>
        private NetworkFunctionCollection CreateFunctions(BaseUDPInterface iface, string clickable)
        {
            string sectionalPattern = @"[\-]{2}\s*(?<deviceName>[^\r\n]*)(?:(?![\-]{2})[\s\S])*";
            //string fullPattern = @"[\-]{62,66}[\r\n]+-- (?<deviceName>[^\r]*)[\s\n\r]+(([\-]{2,4} .*[\n\r\s])*(:[\-]{2,4}elements.*\)[\r\n\s]*)*((-- .*[\r\n\s]*)*elements\[""PNT-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Z0-9]*)[,\s]*(?:devCmds\.Cmd(?<comand>[0-9]{1,4})[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_]*)[,\s]+)+(?<lastarg>.*)\)[\s\n\r]*)*)*";
            string elementalPattern = @"(?<!--)elements\[""PTR-.*-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Z0-9_]*)[,\s]*((?<commandName>[a-zA-Z0-9]+)(?:_commands.)(?<command>[a-zA-Z0-9_]+)[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_/\*]*)[\,\s\)]+)+[\r\n\s]{1}";
            string argPattern = @"(?<name>.*)(?:\,+\s*)(((?<position>[a-zA-z0-9\-\s]+)(\/|$))+)";
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            MatchCollection sectionMatches = Regex.Matches(clickable, sectionalPattern, options);
            if (sectionMatches.Count > 0)
            {
                foreach (Match sM in sectionMatches)
                {
                    MatchCollection elementalMatches = Regex.Matches(sM.Value, elementalPattern, options);
                    string sectionName = sM.Groups["deviceName"].Value;
                    _addFunctionList.Add($"#region {sectionName}");
                    //Logger.Debug($"Device Name: {sectionName} Count of Elements: {elementalMatches.Count}");
                    foreach (Match eM in elementalMatches)
                    {
                        //Logger.Debug($"Element Name: {eM.Groups["name"].Value} Name of Functions: {eM.Groups["function"].Value}");
                        string device = "";
                        if (Enum.TryParse(eM.Groups["device"].Value, out devices dev))
                        {
                            device = dev.ToString("d");
                        }
                        string commandCode1 = "";
                        string commandCode2 = "";
                        string commandCodeName1 = "";
                        string commandCodeName2 = "";
                        Type enumType = typeof(F16CCommands).GetNestedType($"{eM.Groups["commandName"].Value}Commands", BindingFlags.NonPublic);
                        string commandName = $"F16CCommands.{eM.Groups["commandName"].Value}Commands.";
                        if (eM.Groups["command"].Captures.Count == 1)
                        {
                            commandCode1 = ((int)Enum.Parse(enumType, eM.Groups["command"].Value)).ToString("d");
                            commandCodeName1 = $"F16CCommands.{eM.Groups["commandName"].Value}Commands.{eM.Groups["command"].Value}.ToString(\"d\")";
                        }
                        else
                        {
                            commandCodeName1 = $"F16CCommands.{eM.Groups["commandName"].Value}Commands.{eM.Groups["command"].Captures[0].Value}.ToString(\"d\")";
                            commandCodeName2 = $"F16CCommands.{eM.Groups["commandName"].Value}Commands.{eM.Groups["command"].Captures[1].Value}.ToString(\"d\")";
                            commandCode1 = ((int)Enum.Parse(enumType, eM.Groups["command"].Captures[0].Value)).ToString("d");
                            commandCode2 = ((int)Enum.Parse(enumType, eM.Groups["command"].Captures[1].Value)).ToString("d");
                        }
                        CaptureCollection arguments = eM.Groups["args"].Captures;
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
                                modifier = arguments.Count >= 2 ? (arguments[1].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("0.0", posnName[0], commandCode1), new SwitchPosition((1*modifier).ToString("F1"), posnName[1], commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"{posnName[0]}\", {commandCodeName1}),new SwitchPosition(\"{1*modifier:F1}\", \"{posnName[1]}\", {commandCodeName1})}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "default_3_position_tumb_small": // elements["PTR-CLCP-TMB-ROLL-108"] = default_3_position_tumb_small(_("Autopilot ROLL Switch, STRG SEL/ATT HOLD/HDG SEL"), devices.CONTROL_INTERFACE, control_commands.ApRoll, 108)
                            case "default_3_position_tumb":
                                posnName = FindPositionNames(eM.Groups["name"].Value);
                                if (posnName.Length != 3)
                                {
                                    posnName = new string[3];
                                    posnName[0] = "Posn 1";
                                    posnName[1] = "Posn 2";
                                    posnName[2] = "Posn 3";
                                }
                                modifier = arguments.Count >= 3 ? (arguments[2].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), posnName[0], commandCode1), new SwitchPosition("0.0", posnName[1], commandCode1), new SwitchPosition((1 * modifier).ToString("F1"), posnName[2], commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1 * modifier:F1}\", \"{posnName[0]}\", {commandCodeName1}),new SwitchPosition(\"0.0\", \"{posnName[1]}\", {commandCodeName1}),new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[2]}\", {commandCodeName1})}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;

                            case "Rocker_switch_positive": // elements["PTR-ICP-RS-OFF-UP-183"]	= Rocker_switch_positive(_("ICP DED Increment/Decrement Switch, Up"),devices.UFC, ufc_commands.DED_INC,183)
                            case "Rocker_switch_negative": // elements["PTR-ICP-RS-OFF-DN-183"]	= Rocker_switch_negative(_("ICP DED Increment/Decrement Switch, Down"),devices.UFC, ufc_commands.DED_DEC,183)
                                // rockers are two elements but need to result in a single Helios function
                                if (_twoPartElement.TryGetValue(int.Parse(eM.Groups["arg"].Value),out string firstPart))
                                {
                                    string argName;
                                    MatchCollection rockerMatches = Regex.Matches(firstPart, elementalPattern, options);
                                    if(rockerMatches.Count > 0)
                                    {
                                        Match rockerMatch = rockerMatches[0];
                                        commandCode1 = ((int)Enum.Parse(enumType, rockerMatch.Groups["command"].Value)).ToString("d");
                                        commandCodeName1 = $"F16CCommands.{rockerMatch.Groups["commandName"].Value}Commands.{rockerMatch.Groups["command"].Value}.ToString(\"d\")";
                                        string[] temp = rockerMatch.Groups["name"].Value.Split(',');
                                        argName = temp[0];
                                        posnName = new string[3];
                                        posnName[0] = temp.Length == 2 ? temp[1].Trim() : "Posn 1";
                                        posnName[1] = "Middle";
                                        temp = eM.Groups["name"].Value.Split(',');
                                        posnName[2] = temp.Length == 2? temp[1].Trim(): "Posn 3";
                                        commandCode2 = ((int)Enum.Parse(enumType, eM.Groups["command"].Value)).ToString("d");
                                        commandCodeName2 = $"F16CCommands.{eM.Groups["commandName"].Value}Commands.{eM.Groups["command"].Value}.ToString(\"d\")";

                                        modifier = arguments.Count >= 2 ? (arguments[1].Value == "true" ? -1 : 1) : 1;
                                        AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], commandCode1, commandCode1, "0.0", "0.0"), new SwitchPosition("0.0", posnName[1], null), new SwitchPosition((-1 * modifier).ToString("F1"), posnName[2], commandCode2, commandCode2, "0.0", "0.0") }, sectionName, argName, "%0.1f"));
                                        _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {commandCodeName1},{commandCodeName1},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"{posnName[1]}\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"{posnName[2]}\", {commandCodeName2},{commandCodeName2},\"0.0\",\"0.0\")}}, \"{sectionName}\", \"{argName}\", \"%0.1f\"));");
                                    }
                                    _twoPartElement.Remove(int.Parse(eM.Groups["arg"].Value));
                                }
                                else
                                {
                                    _twoPartElement.Add(int.Parse(eM.Groups["arg"].Value), eM.Value);
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
                                modifier = arguments.Count >= 2 ? (arguments[1].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], commandCode1, commandCode1, "0.0", "0.0"), new SwitchPosition("0.0", posnName[1], null), new SwitchPosition((-1 * modifier).ToString("F1"), posnName[2], commandCode2, commandCode2, "0.0", "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {commandCodeName1},{commandCodeName1},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"{posnName[1]}\", null),new SwitchPosition(\"{-1 * modifier:F1}\", \"{posnName[2]}\", {commandCodeName2},{commandCodeName2},\"0.0\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "springloaded_2_pos_tumb": // elements["PTR-FLTCP-TMB-BIT-574"]= springloaded_2_pos_tumb(_("BIT Switch, OFF/BIT"),devices.CONTROL_INTERFACE, control_commands.BitSw,574)
                            case "springloaded_2_pos_tumb_small": // elements["PTR-FLTCP-TMB-FLCS-573"]= springloaded_2_pos_tumb_small(_("FLCS RESET Switch, OFF/RESET"),devices.CONTROL_INTERFACE, control_commands.FlcsReset,573)
                            case "default_2_position_tumb_small":  // elements["PTR-LGCP-TMB-STCONF-358"]= default_2_position_tumb_small(_("STORES CONFIG Switch, CAT III/CAT I"),devices.CONTROL_INTERFACE, control_commands.StoresConfig,358)
                            case "circuit_breaker":
                            case "default_2_position_tumb":
                                posnName = FindPositionNames(eM.Groups["name"].Value);
                                if (posnName.Length != 2)
                                {
                                    posnName = new string[2];
                                    posnName[0] = "Posn 1";
                                    posnName[1] = "Posn 2";
                                }
                                modifier = arguments.Count >= 6 ? (arguments[5].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], commandCode1), new SwitchPosition("0.0", posnName[1], commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {commandCodeName1}),new SwitchPosition(\"0.0\", \"{posnName[1]}\", {commandCodeName1})}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;

                            case "springloaded_2pos_switch":
                                posnName = FindPositionNames(eM.Groups["name"].Value);
                                if (posnName.Length != 2)
                                {
                                    posnName = new string[2];
                                    posnName[0] = "Posn 1";
                                    posnName[1] = "Posn 2";
                                }
                                modifier = arguments.Count >= 2 ? (arguments[1].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1*modifier).ToString("F1"), posnName[0], commandCode1, commandCode1, "0.0"), new SwitchPosition((1*modifier).ToString("F1"), posnName[1], commandCode2, commandCode2, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1.0*modifier:F1}\", \"{posnName[0]}\", {commandCodeName1},{commandCodeName1},\"0.0\"), new SwitchPosition(\"{1*modifier:F1}\", \"{posnName[1]}\", {commandCodeName2},{commandCodeName2},\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "short_way_button": // elements["PTR-ICP-BTN-NMB1-171"]	= short_way_button(_("ICP Priority Function Button, 1(T-ILS)"),devices.UFC, ufc_commands.DIG1_T_ILS,171)
                            case "mfd_button":
                            case "default_button":
                                AddFunction(new PushButton(iface, device, commandCode1, eM.Groups["arg"].Value, sectionName, eM.Groups["name"].Value, "%1d"));
                                _addFunctionList.Add($"AddFunction(new PushButton(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), {commandCodeName1}, \"{eM.Groups["arg"].Value}\", \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%1d\"));");
                                break;
                            case "default_button_knob": // elements["PTR-EHSI-LVR-CRS-44"]	= default_button_knob(_("CRS Set / Brightness Control Knob"),	devices.EHSI, ehsi_commands.RightKnobBtn, ehsi_commands.RightKnob, 43, 44)
                            case "default_button_axis":
                                AddFunction(new PushButton(iface, device, commandCode1, arguments[0].Value, sectionName, $"Button {eM.Groups["name"].Value}", "%1d"));
                                _addFunctionList.Add($"AddFunction(new PushButton(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), {commandCodeName1}, \"{arguments[0].Value}\", \"{sectionName}\", \"Button {eM.Groups["name"].Value}\", \"%1d\"));");
                                AddFunction(new Axis(iface, device, commandCode2, arguments[1].Value, 0.5d, 0.0d, 1.0d, sectionName, $"Lamp {eM.Groups["name"].Value}", false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), {commandCodeName2}, \"{arguments[1].Value}\", 0.5d, 0.0d, 1.0d, \"{sectionName}\", \"Lamp {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            case "default_tumb_button":
                                modifier = arguments.Count >= 2? (arguments[1].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1*modifier).ToString("F1"), "Posn 1", commandCode2, commandCode2, "0.0"), new SwitchPosition("0.0", "Posn 2", null ), new SwitchPosition((1 * modifier).ToString("F1"), "Posn 3", commandCode1, commandCode1, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1*modifier:F1}\", \"Posn 1\", {commandCodeName1},{commandCodeName1},\"0.0\"), new SwitchPosition(\"{-1 * modifier:F1}\", \"Posn 2\", null),new SwitchPosition(\"{1*modifier:F1}\", \"Posn 3\", {commandCodeName1},{commandCodeName1},\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "default_button_tumb":
                                modifier = arguments.Count >= 2 ? (arguments[1].Value == "true" ? -1 : 1) : 1;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), "Posn 1", commandCode1, commandCode1, "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition((-1*modifier).ToString("F1"), "Posn 3", commandCode2, commandCode2, "0.0", "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"Posn 1\", {commandCodeName1},{commandCodeName1},\"0.0\",\"0.0\"), new SwitchPosition(\"0.0\", \"Posn 2\", null),new SwitchPosition(\"{-1*modifier:F1}\", \"Posn 3\", {commandCodeName2},{commandCodeName2},\"0.0\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "multiposition_switch_tumb":  // elements["PTR-ANARC164-CHNL-SELECTOR-410"] = multiposition_switch_tumb(_("UHF CHAN Knob"), devices.UHF_CONTROL_PANEL, uhf_commands.ChannelKnob, 410, 20, 0.05, NOT_INVERSED, 0.0, anim_speed_default * 0.05, NOT_CYCLED)
                            case "multiposition_switch":
                                string[] positionNames = new string[] { };
                                MatchCollection argMatches = Regex.Matches(eM.Groups["name"].Value, argPattern, options);
                                if(argMatches.Count > 0 && argMatches[0].Groups.Count > 1)
                                {
                                    positionNames = argMatches[0].Groups[1].Value.Split('/');

                                }
                                if (!double.TryParse(arguments[2].Value,out double stepValue))
                                {
                                    if (arguments[2].Value.Contains("/"))
                                    {
                                        string[] numberPortion = arguments[2].Value.Split('/');
                                        stepValue = Math.Round((double.Parse(numberPortion[0]) / double.Parse(numberPortion[1])),3);
                                        exportValue = "%0.3f";
                                    } else
                                    {
                                        stepValue = 0;
                                    }
                                } else
                                {
                                    exportValue = $"%0.{arguments[2].Value.Length-2}f";
                                }
                                if (int.TryParse(arguments[1].Value,out int stepCount) && 
                                    double.TryParse(arguments[4].Value == "nil" ? "0.0" : arguments[4].Value,out double startValue)
                                    && stepValue!= 0)
                                {
                                    if(positionNames.Length != stepCount)
                                    {
                                        AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, commandCode1, "Posn", exportValue), sectionName, eM.Groups["name"].Value, exportValue));
                                        _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, {commandCodeName1}, \"Posn\", \"{exportValue}\"), \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");

                                    } else
                                    {
                                        AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, commandCode1, flatten(positionNames), exportValue), sectionName, eM.Groups["name"].Value, exportValue));
                                        _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, {commandCodeName1}, {flatten(positionNames)}, \"{exportValue}\"), \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");

                                    }

                                } else
                                {
                                    Logger.Warn($"Unable to create function {eM.Groups["function"].Value} for {eM.Value}.  Unexpected element arguments.");
                                }
                                break;
                            case "multiposition_switch_cycled_relative":
                                if (!double.TryParse(arguments[2].Value, out stepValue))
                                {
                                    if (arguments[2].Value.Contains("/"))
                                    {
                                        string[] numberPortion = arguments[2].Value.Split('/');
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
                                    exportValue = $"%0.{arguments[2].Value.Length - 2}f";
                                }
                                if (int.TryParse(arguments[1].Value, out stepCount) &&
                                    double.TryParse(arguments[4].Value == "nil" ? "0.0" : arguments[4].Value, out startValue)
                                    && stepValue != 0)
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, SwitchPositions.Create(stepCount, startValue, stepValue, commandCode1, "Posn", exportValue), sectionName, eM.Groups["name"].Value, exportValue));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({stepCount}, {startValue}d, {stepValue}d, {commandCodeName1}, \"Posn\", \"{exportValue}\"), \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"{exportValue}\"));");
                                }
                                else
                                {
                                    Logger.Warn($"Unable to create function {eM.Groups["function"].Value} for {eM.Value}.  Unexpected element arguments.");
                                }
                                break;
                            case "default_axis":
                                AddFunction(new Axis(iface, device, commandCode1, eM.Groups["arg"].Value, double.Parse(arguments[2].Value), 0.0d, double.Parse(arguments[1].Value), sectionName, eM.Groups["name"].Value, false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), {commandCodeName1}, \"{eM.Groups["arg"].Value}\", {arguments[2].Value}d, 0.0d, {arguments[1].Value}d, \"{sectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            case "default_axis_limited_1_side": // elements["PTR-MANTRIM-LVR-ROLL-560"]	= default_axis_limited_1_side(_("ROLL TRIM Wheel"), devices.CONTROL_INTERFACE, control_commands.RollTrim, 560, 0.0, 0.1, NOT_UPDATABLE, NOT_RELATIVE, {-1,1}, {0,160},{0,-135})
                            case "default_axis_limited":
                                AddFunction(new Axis(iface, device, commandCode1, eM.Groups["arg"].Value, 0.1d, 0.0d, 1.0d, sectionName, eM.Groups["name"].Value, false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), {commandCodeName1}, \"{eM.Groups["arg"].Value}\", 0.1d, 0.0d, 1.0d, \"{sectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            case "intercom_rotate_tumb":
                                AddFunction(new Switch(iface, device, arguments[0].Value, new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", commandCode1), new SwitchPosition("1.0", "Posn 2", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), \"{arguments[0].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"Posn 1\", {commandCodeName1}),new SwitchPosition(\"1.0\", \"Posn 2\", {commandCodeName1})}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                AddFunction(new Axis(iface, device, commandCode2, arguments[1].Value, 0.1d, 0.0d, 1.0d, sectionName, $"Rotate {eM.Groups["name"].Value}", false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, devices.{eM.Groups["device"].Value}.ToString(\"d\"), {commandCodeName2}, \"{arguments[1].Value}\", 0.1d, 0.0d, 1.0d, \"{sectionName}\", \"Rotate {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            default:
                                //Logger.Warn($"Unknown function encountered while creating interface: {eM.Groups["function"].Value}");
                                if (!_unknownFunctions.Contains(eM.Groups["function"].Value))
                                {
                                    _unknownFunctions.Add(eM.Groups["function"].Value);
                                    _unknownFunctions.Add(eM.Value);
                                }
                                break;
                        }
                        if (!_functionList.Contains(eM.Groups["function"].Value))
                        {
                            _functionList.Add(eM.Groups["function"].Value);
                        }
                    }
                    _addFunctionList.Add($"#endregion {sectionName}");
                }
            }
            //foreach (string a in _functionList)
            //{
            //    Logger.Debug($"case \"{a}\":break;");
            //}
            WriteFunctions();
            WriteMissingFunctions();
            return _networkFunctions;
        }
        private void AddFunction(NetworkFunction netFunction)
        {
            _networkFunctions.Add(netFunction); 
        }
        /// <summary>
        /// Creates a string for use in a string array statement
        /// </summary>
        /// <param name="input">string array containing position names</param>
        /// <returns></returns>
        private string flatten(string[] input)
        {
            string strings = "";
            foreach (string s in input)
            {
                strings += $", \"{s}\"";
            }
            return $"new string[]{{{strings.Substring(2)}}}";
        }
        /// <summary>
        /// Saves the c# AddFunction statements to an external file for later inclusion in the interface.
        /// </summary>
        private void WriteFunctions()
        {
            string DCSAircraftFunctions = $@"{Environment.GetEnvironmentVariable("Temp")}\InterfaceAddFunctions.txt";

            using (StreamWriter streamWriter = new StreamWriter(DCSAircraftFunctions))
            {
                Logger.Debug($"Writing Interface Functions to file: \"{DCSAircraftFunctions}\"");
                foreach (string a in _addFunctionList)
                {
                    streamWriter.WriteLine(a);
                }
            }
        }
        /// <summary>
        /// Saves the function names which are not currently handled into an external file in 
        /// a format suitable for a switch statement.  It also saves an example of this function 
        /// usage by a cliakcable.
        /// </summary>
        private void WriteMissingFunctions()
        {
            string DCSAircraftFunctions = $@"{Environment.GetEnvironmentVariable("Temp")}\ClickableMissingFunctions.txt";

            using (StreamWriter streamWriter = new StreamWriter(DCSAircraftFunctions))
            {
                Logger.Debug($"Writing Unhandled Clickable Functions to file: \"{DCSAircraftFunctions}\"");
                bool IsExample = false;
                foreach (string a in _unknownFunctions)
                {
                    if (!IsExample)
                    {
                        streamWriter.WriteLine($"case \"{a}\":");
                        streamWriter.WriteLine($"  break;");
                    }
                    else
                    {
                        streamWriter.WriteLine($"// {a}");
                    }
                    IsExample ^= true;
                }
            }
        }
        /// <summary>
        /// Used to extract the names of the switch positions from the function name
        /// </summary>
        /// <param name="nameContainingPositions">string</param>
        /// <returns> a string array containing the names for the switch positions or empty array if the names cannot be decerned</returns>
        /// <remarks>some labels contain the splitting character and sometimes whent his is the case, the split is the splitting character followed by a blank.</remarks>
        private string[] FindPositionNames(string nameContainingPositions)
        {
            string[] names;
            if (nameContainingPositions.Contains(", ") && nameContainingPositions.Contains("/"))
            {
                string[] temp = nameContainingPositions.Split(',');
                if (nameContainingPositions.Contains("/ "))
                {
                    // probably means that one of the labels contains a slash so use / blank as the splitter
                    temp[temp.Length - 1] = temp[temp.Length - 1].Replace( "/ ", "|");
                    names = temp[temp.Length - 1].Split('|');

                } else
                {
                    names = temp[temp.Length - 1].Split('/');
                }
            } else
            {
                names = new string[0];
            }
            return names;
        }
    }
}
