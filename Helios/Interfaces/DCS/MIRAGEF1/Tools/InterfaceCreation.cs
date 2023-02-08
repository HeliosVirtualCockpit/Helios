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

    internal class InterfaceCreation
    {

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<string> _functionList = new List<string>();
        private List<string> _addFunctionList = new List<string>();

        private NetworkFunctionCollection _networkFunctions = new NetworkFunctionCollection();

        internal InterfaceCreation()
        {
            _networkFunctions.Clear();
        }

        internal NetworkFunctionCollection CreateFunctionsFromClickable(BaseUDPInterface iface, string path)
        {
            Logger.Debug($"\n* * * Creating Interface functions from file: {path}\n");
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

        private NetworkFunctionCollection CreateFunctions(BaseUDPInterface iface, string clickable)
        {
            string sectionalPattern = @"[\-]{62,66}[\r\n]+-- (?<deviceName>[^\r]*)[\s\n\r]+(?:(?![\-]{62,66}[\r\n]+--)[\s\S])*";
            //string fullPattern = @"[\-]{62,66}[\r\n]+-- (?<deviceName>[^\r]*)[\s\n\r]+(([\-]{2,4} .*[\n\r\s])*(:[\-]{2,4}elements.*\)[\r\n\s]*)*((-- .*[\r\n\s]*)*elements\[""PNT-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Z0-9]*)[,\s]*(?:devCmds\.Cmd(?<comand>[0-9]{1,4})[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_]*)[,\s]+)+(?<lastarg>.*)\)[\s\n\r]*)*)*";
            string elementalPattern = @"(?<!--)elements\[""PNT-(?<arg>\d{1,4})""\]\s*=\s*(?<function>.*)\(_\(""(?<name>.*)"".*devices\.(?<device>[A-Z0-9]*)[,\s]*(?:devCmds\.Cmd(?<command>[0-9]{1,4})[,\s]*)+(?:(?<args>[a-zA-Z0-9\{\}\.\-_/\*]*)[\,\s\)]+)+[\r\n\s]{1}";
            RegexOptions options = RegexOptions.Multiline;
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
                        if (eM.Groups["command"].Captures.Count == 1)
                        {
                            commandCode1 = (double.Parse(eM.Groups["command"].Value) + 3000).ToString();
                        }
                        else
                        {
                            commandCode1 = (double.Parse(eM.Groups["command"].Captures[0].Value) + 3000).ToString();
                            commandCode2 = (double.Parse(eM.Groups["command"].Captures[1].Value) + 3000).ToString();
                        }
                        CaptureCollection arguments = eM.Groups["args"].Captures;
                        switch (eM.Groups["function"].Value)
                        {
                            case "guard_switch":
                                if (arguments[1].Value != "true") // not inversed
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("0.0", "1", commandCode1), new SwitchPosition("1.0", "2", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                else
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("1.0", "1", commandCode1), new SwitchPosition("0.0", "2", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"1.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"0.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                break;
                            case "default_3_position_tumb":
                                if (arguments[2].Value == "true") // inverted
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("1.0", "1", commandCode1), new SwitchPosition("0.0", "2", commandCode1), new SwitchPosition("-1.0", "3", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"-1.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"0.0\", \"2\", \"{commandCode1}\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                else
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("-1.0", "1", commandCode1), new SwitchPosition("0.0", "2", commandCode1), new SwitchPosition("1.0", "3", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"-1.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"0.0\", \"2\", \"{commandCode1}\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                break;
                            case "circuit_breaker":
                            case "default_2_position_tumb":
                                if (arguments[5].Value == "true") // inversed
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("0.0", "1", commandCode1), new SwitchPosition("1.0", "2", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                else
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("1.0", "1", commandCode1), new SwitchPosition("0.0", "2", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"1.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"0.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                break;
                            case "springloaded_2pos_switch":
                                if (arguments[1].Value != "true")  // not inverted
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("-1.0", "1", commandCode1, commandCode1, "0.0"), new SwitchPosition("1.0", "2", commandCode2, commandCode2, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"-1.0\", \"1\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\"), new SwitchPosition(\"1.0\", \"2\", \"{commandCode2}\",\"{commandCode2}\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                else //inverted
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("1.0", "1", commandCode1, commandCode1, "0.0"), new SwitchPosition("-1.0", "2", commandCode2, commandCode2, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"1.0\", \"1\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\"), new SwitchPosition(\"-1.0\", \"2\", \"{commandCode2}\",\"{commandCode2}\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                break;
                            case "default_button":
                                AddFunction(new PushButton(iface, device, commandCode1, eM.Groups["arg"].Value, sectionName, eM.Groups["name"].Value, "%1d"));
                                _addFunctionList.Add($"AddFunction(new PushButton(this, \"{device}\", \"{commandCode1}\", \"{eM.Groups["arg"].Value}\", \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%1d\"));");
                                break;
                            case "default_button_axis":
                                AddFunction(new PushButton(iface, device, commandCode1, arguments[0].Value, sectionName, $"Button {eM.Groups["name"].Value}", "%1d"));
                                _addFunctionList.Add($"AddFunction(new PushButton(this, \"{device}\", \"{commandCode1}\", \"{arguments[0].Value}\", \"{sectionName}\", \"Button {eM.Groups["name"].Value}\", \"%1d\"));");
                                AddFunction(new Axis(iface, device, commandCode2, arguments[1].Value, 0.5d, 0.0d, 1.0d, sectionName, $"Lamp {eM.Groups["name"].Value}", false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, \"{device}\", \"{commandCode2}\", \"{arguments[1].Value}\", 0.5d, 0.0d, 1.0d, \"{sectionName}\", \"Lamp {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            case "default_tumb_button":
                                if (arguments[1].Value != "true") // not inversed
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("-1.0", "1", commandCode2, commandCode2, "0.0"), new SwitchPosition("1.0", "2", commandCode1, commandCode1, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"-1.0\", \"1\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                else
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("1.0", "1", commandCode2, commandCode2, "0.0"), new SwitchPosition("-1.0", "2", commandCode1, commandCode1, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"1.0\", \"1\", \"{commandCode2}\",\"{commandCode2}\",\"0.0\"),new SwitchPosition(\"-1.0\", \"2\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                break;
                            case "default_button_tumb":
                                if (arguments[1].Value != "true") // not inversed
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("-1.0", "1", commandCode1, commandCode1, "0.0"), new SwitchPosition("1.0", "2", commandCode2, commandCode2, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"-1.0\", \"1\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode2}\",\"{commandCode2}\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                else
                                {
                                    AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition("1.0", "1", commandCode1, commandCode1, "0.0"), new SwitchPosition("-1.0", "2", commandCode2, commandCode2, "0.0") }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                    _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"1.0\", \"1\", \"{commandCode1}\",\"{commandCode1}\",\"0.0\"),new SwitchPosition(\"-1.0\", \"2\", \"{commandCode2}\",\"{commandCode2}\",\"0.0\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                }
                                break;
                            case "multiposition_switch_cycled_relative":
                                double stepValue = 0.0;
                                if (arguments[2].Value.Contains("/"))
                                {
                                    string[] numberPortion = arguments[2].Value.Split('/');
                                    stepValue = double.Parse(numberPortion[0]) / double.Parse(numberPortion[1]);
                                }
                                else
                                {
                                    stepValue = double.Parse(arguments[2].Value);
                                }
                                string startValue = arguments[4].Value == "nil" ? "0.0" : arguments[4].Value;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, SwitchPositions.Create(int.Parse(arguments[1].Value), double.Parse(startValue), stepValue, commandCode1, "Position"), sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({int.Parse(arguments[1].Value)}, {double.Parse(startValue)}d, {stepValue}d, \"{commandCode1}\", \"Position\"), \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "multiposition_switch":
                                stepValue = 0.0;
                                if (arguments[2].Value.Contains("/"))
                                {
                                    string[] numberPortion = arguments[2].Value.Split('/');
                                    stepValue = double.Parse(numberPortion[0]) / double.Parse(numberPortion[1]);
                                }
                                else
                                {
                                    stepValue = double.Parse(arguments[2].Value);
                                }
                                startValue = arguments[5].Value == "nil" ? "0.0" : arguments[5].Value;
                                AddFunction(new Switch(iface, device, eM.Groups["arg"].Value, SwitchPositions.Create(int.Parse(arguments[1].Value), double.Parse(startValue), stepValue, commandCode1, "Position"), sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{eM.Groups["arg"].Value}\", SwitchPositions.Create({int.Parse(arguments[1].Value)}, {double.Parse(startValue)}d, {stepValue}d, \"{commandCode1}\", \"Position\"), \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                break;
                            case "default_axis":
                                AddFunction(new Axis(iface, device, commandCode1, eM.Groups["arg"].Value, double.Parse(arguments[1].Value), 0.0d, double.Parse(arguments[0].Value), sectionName, eM.Groups["name"].Value, false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, \"{device}\", \"{commandCode1}\", \"{eM.Groups["arg"].Value}\", {arguments[1].Value}d, 0.0d, {arguments[0].Value}d, \"{sectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            case "default_axis_limited":
                                AddFunction(new Axis(iface, device, commandCode1, eM.Groups["arg"].Value, 0.1d, 0.0d, 1.0d, sectionName, eM.Groups["name"].Value, false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, \"{device}\", \"{commandCode1}\", \"{eM.Groups["arg"].Value}\", 0.1d, 0.0d, 1.0d, \"{sectionName}\", \"{eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            case "intercom_rotate_tumb":
                                AddFunction(new Switch(iface, device, arguments[0].Value, new SwitchPosition[] { new SwitchPosition("0.0", "1", commandCode1), new SwitchPosition("1.0", "2", commandCode1) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Switch(this, \"{device}\", \"{arguments[0].Value}\", new SwitchPosition[] {{new SwitchPosition(\"0.0\", \"1\", \"{commandCode1}\"),new SwitchPosition(\"1.0\", \"2\", \"{commandCode1}\")}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
                                AddFunction(new Axis(iface, device, commandCode2, arguments[1].Value, 0.1d, 0.0d, 1.0d, sectionName, $"Rotate {eM.Groups["name"].Value}", false, "%0.1f"));
                                _addFunctionList.Add($"AddFunction(new Axis(this, \"{device}\", \"{commandCode2}\", \"{arguments[1].Value}\", 0.1d, 0.0d, 1.0d, \"{sectionName}\", \"Rotate {eM.Groups["name"].Value}\", false, \"%0.1f\"));");
                                break;
                            default:
                                Logger.Warn($"Unknown function encountered while creating interface: {eM.Groups["function"].Value}");
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
            foreach (string a in _addFunctionList)
            {
                Logger.Debug($"{a}");
            }
            return _networkFunctions;
        }
        private void AddFunction(NetworkFunction netFunction)
        {
            _networkFunctions.Add(netFunction); 
        }
    }
}
