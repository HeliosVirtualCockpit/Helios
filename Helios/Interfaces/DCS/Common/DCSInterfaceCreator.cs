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

using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.Interfaces.DirectX;
using GadrocsWorkshop.Helios.UDPInterface;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    internal class DCSInterfaceCreator: IDCSInterfaceCreator
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<string> _functionList = new List<string>();
        private List<string> _addFunctionList = new List<string>();
        private List<string> _unknownFunctions = new List<string>();
        private Dictionary<int, string> _twoPartElement = new Dictionary<int, string>();
        private NetworkFunctionCollection _networkFunctions = new NetworkFunctionCollection();
        private string[] _devices;
        private string[][] _commandItems;
        private CaptureCollection _arguments;
        private BaseUDPInterface _udpInterface;
        private string _sectionName;
        private string _documentPath;

        protected DCSInterfaceCreator()
        {
        }
        virtual public NetworkFunctionCollection CreateFunctionsFromDcsModule(BaseUDPInterface iface, string path)
        {
            _udpInterface = iface;
            Logger.Debug($"Creating Interface functions from file: {path}");
            NetworkFunctions.Clear();
            AddFunctionList.Clear();
            FunctionList.Clear();
            return CreateFunctions(iface, ReadFunctionsFromDcsModule(path), path);
        }
        virtual protected void FunctionRouter(BaseUDPInterface iface, Match eM)
        {
           _arguments = eM.Groups["args"].Captures;
            switch (eM.Groups["function"].Value)
            {
                default:
                    AddUnknownFunction(eM.Groups["function"].Value, eM.Value);
                    break;
            }
        }

        protected string ReadFunctionsFromDcsModule(string path)
        {
            Logger.Debug($"Reading DCS Module Interface functions from file: {path}");
            NetworkFunctions.Clear();
            AddFunctionList.Clear();

            string input;
            using (StreamReader streamReader = new StreamReader(path))
            {
                input = streamReader.ReadToEnd();
            }
            return input;
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
        virtual protected NetworkFunctionCollection CreateFunctions(BaseUDPInterface iface, string clickable, string path = "")
        {
            MatchCollection sectionMatches = GetSections(clickable);
            if (sectionMatches.Count > 0)
            {
                foreach (Match sM in sectionMatches)
                {
                    SectionName = sM.Groups["deviceName"].Value.Trim();
                    if (SectionName.Contains("-- "))
                    {
                        int i = SectionName.LastIndexOf("-- ")+3;
                        SectionName = SectionName.Substring(i,SectionName.Length-i);
                    }
                    MatchCollection elementalMatches = GetElements(sM.Value);
                    AddFunctionList.Add($"#region {SectionName}");
                    foreach (Match eM in elementalMatches)
                    {
                        //Logger.Debug($"Element Name: {eM.Groups["name"].Value} Name of Functions: {eM.Groups["function"].Value}");
                        _devices = ParseDeviceGroup(eM.Groups["device"]);
                        _commandItems = ParseCommandGroup(eM);
                        _arguments = eM.Groups["args"].Captures;
                        FunctionList.Add(eM.Groups["function"].Value);
                        ProcessFunction(eM);
                    }
                    AddFunctionList.Add($"#endregion {SectionName}");
                }
            }
            // WriteUsedFunctions();
            WriteMissingFunctions();
            WriteFunctions(Path.GetFileNameWithoutExtension(path));
            return NetworkFunctions;
        }

        /// <summary>
        /// Creates a string for use in a string array c# source statement
        /// </summary>
        /// <param name="input">string array containing position names</param>
        /// <returns></returns>
        protected string Flatten(string[] input)
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
        protected void WriteFunctions(string fn = "", bool append = false )
        {
            string DCSAircraftFunctions = Path.Combine(DocumentPath , $"{fn}{(fn==""?"":"_")}InterfaceAddFunctions.txt");

            using (StreamWriter streamWriter = new StreamWriter(DCSAircraftFunctions, append: append))
            {
                Logger.Debug($"Writing Interface Functions to file: \"{DCSAircraftFunctions}\"");
                foreach (string a in AddFunctionList)
                {
                    streamWriter.WriteLine(a);
                }
            }
        }
        /// <summary>
        /// Saves the function names which are not currently handled into an external file in 
        /// a format suitable for a switch statement.  It also saves an example of this function 
        /// usage by a clickable.
        /// </summary>
        protected void WriteMissingFunctions(string fn = "", bool append = false)
        {
            string DCSAircraftFunctions = Path.Combine(DocumentPath, $"{fn}{(fn == "" ? "" : "_")}MissingFunctions.txt");
            using (StreamWriter streamWriter = new StreamWriter(DCSAircraftFunctions,append: append))
            {
                Logger.Debug($"Listing Unhandled DCS Module Functions to file: \"{DCSAircraftFunctions}\"");
                bool IsExample = false;
                foreach (string a in UnknownFunctions)
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
        /// Writes the list of functions found in the clickables to the log in a switch/case form
        /// </summary>
        protected void WriteUsedFunctions()
        {
            foreach (string a in FunctionList)
            {
                Logger.Debug($"Function Used in Clickable: case \"{a}\":break;");
            }
        }
        /// <summary>
        /// Used to extract the names of the switch positions from the function name
        /// </summary>
        /// <param name="nameContainingPositions">string</param>
        /// <returns> a string array containing the names for the switch positions or empty array if the names cannot be decerned</returns>
        /// <remarks>some labels contain the splitting character and sometimes when his is the case, the split is the splitting character followed by a blank.</remarks>
        virtual protected string[] FindPositionNames(string nameContainingPositions)
        {
            Regex positionRegex = new Regex(@"((?<=,\s*)(?<PositionName>[\-A-Za-z0-9-[\/,\s\|]]*)[\/\|]?(?<PositionName>[\-A-Za-z0-9-[\/,\s\|]]*)$)|" +
                @"((?(?<=,\s*)(?:(?<PositionName>[\/\-A-Za-z0-9-[,\s\|]]*)\s+[/\|]{1}\s+)*(?<PositionName>[\-A-Za-z0-9-[/,\|]]*/?[\-A-Za-z0-9-[/,\|]]*)" +
                @"|(?:(?<PositionName>[\-A-Za-z0-9-[/,\|]]*/?[\-A-Za-z0-9-[/,\|]]*)\s+[/\|]{1}\s+)+(?<PositionName>[\-A-Za-z0-9-[\/,\|]]*/?[\-A-Za-z0-9-[\/,\|]]*))$)|" +
                @"((?(?<=,\s*)(?:(?<PositionName>[\-A-Za-z0-9-[,\s\|]]*)[/\|]{1})*(?<PositionName>[\-A-Za-z0-9-[/,\|]]*)|(?:(?<PositionName>[\-A-Za-z0-9-[/,\|]]*)[/\|]{1})+" +
                @"(?<PositionName>[\-A-Za-z0-9-[\/,\|]]*))$)|(((?<=,\s+)(?:(?<PositionName>[ \-A-Za-z0-9-[,\|]]*)[/\|]{1})*(?<PositionName>[ \-A-Za-z0-9-[/,\|]]*))$)", 
                RegexOptions.Compiled | RegexOptions.Singleline);

            MatchCollection positionMatches = positionRegex.Matches(nameContainingPositions);
            List<string> positionList = new List<string>();
            if(positionMatches.Count> 0)
            {
                foreach(Match m in positionMatches)
                {
                    foreach(Capture c in m.Groups["PositionName"].Captures)
                    {
                        positionList.Add(c.Value);
                    }
                }

                if(positionList.Count > 0) return positionList.ToArray();  
            }

            Regex regex = new Regex(@"[.,\s]*(?:(?<PositionName>[\-A-Za-z0-9-[/\s]]*)/)(?<PositionName>.*)$", RegexOptions.Compiled|RegexOptions.Singleline);
            MatchCollection mc = regex.Matches(nameContainingPositions ?? String.Empty);
            if (nameContainingPositions.Contains(", ") && nameContainingPositions.Contains("/"))
            {
                string[] temp = nameContainingPositions.Split(',');
                if (nameContainingPositions.Contains("/ "))
                {
                    // probably means that one of the labels contains a slash so use / blank as the splitter
                    temp[temp.Length - 1] = temp[temp.Length - 1].Replace("/ ", "|");
                    return temp[temp.Length - 1].Trim().Split('|');
                }
                else
                {
                    return temp[temp.Length - 1].Trim().Split('/');
                }
            }
            else if(mc.Count > 0)
            {
                List<string> captureList = new List<string>();
                foreach (Match m in mc)
                {
                    if (m.Groups["PositionName"].Captures.Count > 0)
                    {
                        foreach (Capture c in m.Groups["PositionName"].Captures)
                        {
                            captureList.Add(c.Value);
                        }
 
                    }
                }
                return captureList.ToArray();
            } else
            {
                return new string[0];
            }
        }
        protected void AddUnknownFunction(string functionName, string element)
        {
            if (!_unknownFunctions.Contains(functionName))
            {
                _unknownFunctions.Add(functionName);
                _unknownFunctions.Add(element);
            }
        }
        protected void AddToFunctionList(string functionName)
        {
            if (!_functionList.Contains(functionName))
            {
                _functionList.Add(functionName);
            }
        }

        virtual protected void AddFunction(NetworkFunction netFunction)
        {
            NetworkFunctions.Add(netFunction);
        }

        virtual public string[] ParseDeviceGroup(Group deviceGroup)
        {
            string[] device = new string[2];
                device[0] = "";
                device[1] = $"devices.{ deviceGroup.Value}.ToString(\"d\")";
            return device;
        }
        /// <summary>
        /// generates a string array containing the command value in string form
        /// as well as the command value needed for the c# source code version of the
        /// AddFunction command.
        /// This particular version expects the commands in teh DCS module to be the 
        /// numeric command minus 3000.
        /// </summary>
        /// <param name="match"></param>
        /// <returns>Two dimensional string array for both commands if they exist.</returns>
        virtual protected string[][] ParseCommandGroup(Match match)
        {
            Group cmd = match.Groups["command"];
            string[][] cmds = { new string[2], new string[2] };
            if (cmd.Captures.Count == 1)
            {
                cmds[0][0] = (double.Parse(cmd.Value) + 3000).ToString();
            }
            else
            {
                cmds[0][0] = (double.Parse(cmd.Captures[0].Value) + 3000).ToString();
                cmds[1][0] = (double.Parse(cmd.Captures[1].Value) + 3000).ToString();
            }
            return cmds;
        }

        #region properties
        protected List<string> AddFunctionList { get => _addFunctionList; set => _addFunctionList = value; }
        protected List<string> FunctionList { get => _functionList; set => _functionList = value; }
        protected List<string> UnknownFunctions { get => _unknownFunctions; set => _unknownFunctions = value; }
        protected NetworkFunctionCollection NetworkFunctions { get => _networkFunctions; set => _networkFunctions = value; }
        protected Dictionary<int, string> TwoPartElement { get => _twoPartElement; set => _twoPartElement = value; }
        virtual public string DocumentPath {
            get {
                if (_documentPath == string.Empty || _documentPath == null)
                {
                    _documentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),  "HeliosDev");
                    if (!Directory.Exists(_documentPath))
                    {
                        Directory.CreateDirectory(_documentPath);
                    }
                    _documentPath = Path.Combine(_documentPath,"Interfaces");
                    if (!Directory.Exists(_documentPath))
                    {
                        Directory.CreateDirectory(_documentPath);
                    }
                }
                return _documentPath;   
            }
        }
        protected string[] Devices { get => _devices; set => _devices = value; }
        protected string[][] CommandItems { get => _commandItems; set => _commandItems = value; }
        protected CaptureCollection Arguments { get => _arguments; set => _arguments = value; }
        protected BaseUDPInterface UdpInterface { get => _udpInterface; set => _udpInterface = value; } 
        protected string SectionName { get => _sectionName; set => _sectionName = value; }

        #endregion

        #region Default Element Function Processing

        virtual protected void ProcessDefault2PositionTumb(BaseUDPInterface iface, string sectionName, Match eM, string[] devices, string[][] commandItems, CaptureCollection arguments)
        {
            string[] posnName = FindPositionNames(eM.Groups["name"].Value);
            if (posnName.Length != 2)
            {
                posnName = new string[2];
                posnName[0] = "Posn 1";
                posnName[1] = "Posn 2";
            }
            double modifier = arguments.Count >= 6 ? (arguments[5].Value == "true" ? -1 : 1) : 1;
            AddFunction(new Switch(iface, devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((1 * modifier).ToString("F1"), posnName[0], commandItems[0][0]), new SwitchPosition("0.0", posnName[1], commandItems[0][0]) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
            AddFunctionList.Add($"AddFunction(new Switch(this, {devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[0]}\", {commandItems[0][1]}),new SwitchPosition(\"0.0\", \"{posnName[1]}\", {commandItems[0][1]})}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
        }
        virtual protected void ProcessDefault3PositionTumb(BaseUDPInterface iface, string sectionName, Match eM, string[] devices, string[][] commandItems, CaptureCollection arguments)
        {
            string[] posnName = FindPositionNames(eM.Groups["name"].Value);
            if (posnName.Length != 3)
            {
                posnName = new string[3];
                posnName[0] = "Posn 1";
                posnName[1] = "Posn 2";
                posnName[2] = "Posn 3";
            }
            double modifier = arguments.Count >= 3 ? (arguments[2].Value == "true" ? -1 : 1) : 1;
            AddFunction(new Switch(iface, devices[0], eM.Groups["arg"].Value, new SwitchPosition[] { new SwitchPosition((-1 * modifier).ToString("F1"), posnName[0], commandItems[0][0]), new SwitchPosition("0.0", posnName[1], commandItems[0][0]), new SwitchPosition((1 * modifier).ToString("F1"), posnName[2], commandItems[0][0]) }, sectionName, eM.Groups["name"].Value, "%0.1f"));
            AddFunctionList.Add($"AddFunction(new Switch(this, {devices[1]}, \"{eM.Groups["arg"].Value}\", new SwitchPosition[] {{new SwitchPosition(\"{-1 * modifier:F1}\", \"{posnName[0]}\", {commandItems[0][1]}),new SwitchPosition(\"0.0\", \"{posnName[1]}\", {commandItems[0][1]}),new SwitchPosition(\"{1 * modifier:F1}\", \"{posnName[2]}\", {commandItems[0][1]})}}, \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%0.1f\"));");
        }
        virtual protected void ProcessDefaultButton(BaseUDPInterface iface, string sectionName, Match eM, string[] devices, string[][] commandItems, CaptureCollection arguments)
        {
            AddFunction(new PushButton(iface, devices[0], commandItems[0][0], eM.Groups["arg"].Value, sectionName, eM.Groups["name"].Value, "%1d"));
            AddFunctionList.Add($"AddFunction(new PushButton(this, {devices[1]}, {commandItems[0][1]}, \"{eM.Groups["arg"].Value}\", \"{sectionName}\", \"{eM.Groups["name"].Value}\", \"%1d\"));");
        }
        virtual protected string[] MainPanelCorrection(string functionName, string arg)
        {
            return new string[3] { functionName, arg, "" };
        }
        virtual protected string MainPanelCreateFunction(string function, string functionname, string arg, string device, string name)
        {
            return MainPanelCreateFunction(function, functionname, arg, device, name, "", "");
        }
        virtual protected string MainPanelCreateFunction(string function, string functionname, string arg, string device, string  name, string description, string valuedescription)
        {
            return "";
        }
        #endregion
        /// <summary>
        /// Decodes one clickable element into Helios function 
        /// </summary>
        /// <param name="elementMatch">Regex Match of a single Element</param>
        /// <returns></returns>
        /// <remarks>
        /// The format of the clickableData file is Lua, but varies significantly from module to module
        /// so it is anticipated that changes to the regex's and functions will be necessary so this is a  
        /// dummy method requiring the IIinterfaceCreation class to perform this work.
        /// clickable elements can also be overeridden/extended by the module author, so parameter parsing can 
        /// also be module specific.
        /// Results as c# source code are written to an external file whose contents can be copied into the DCS
        /// interface file to have a record of the decoding once interface creation has stabilised.
        /// *** Note *** there are complex elements in most modules which are not identified or processed by
        /// this class and will typically need to be manually converted to Helios Functions.
        /// </remarks>
        virtual public void ProcessFunction(Match elementMatch)
        {
            // No Code
        }
        virtual public MatchCollection GetSections(string clickablesFromDCS)
        {
            Regex regex = new Regex(@"\$\(.+?\)");
            return regex.Matches(clickablesFromDCS ?? String.Empty);
        }

        virtual public MatchCollection GetElements(string section)
        {
            Regex regex = new Regex(@"\$\(.+?\)");
            return regex.Matches(section ?? String.Empty);

        }

    }

}
