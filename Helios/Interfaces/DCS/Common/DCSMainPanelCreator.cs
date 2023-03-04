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
//  using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.UDPInterface;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using System.Text.RegularExpressions;


namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    internal class DCSMainPanelCreator : BaseDCSMainPanelCreator
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private NetworkFunctionCollection _networkFunctions = new NetworkFunctionCollection();
        private List<string> _enumSourceCode = new List<string>();
        private string _mainPanel = string.Empty;
        private string _sectionPattern = string.Empty;
        private string _documentPath = string.Empty;
        private string _modulePath = string.Empty;
        private BaseUDPInterface _udpInterface; 
        internal DCSMainPanelCreator(string path, string documentPath)
        {
            ModulePath = Path.Combine(path, "Cockpit", "Scripts", "mainpanel_init.lua");
            Logger.Debug($"Reading DCS Module MainPanel definitions and functions from file: {ModulePath}");
            NetworkFunctions.Clear();
            SourceCodeFunctions.Clear();
            DocumentPath = documentPath;
            try
            {
                using (StreamReader streamReader = new StreamReader(ModulePath))
                {
                    _mainPanel = streamReader.ReadToEnd();
                }
            }
            catch(IOException ex){
                Logger.Error($"Exception while reading file: {ModulePath}.  {ex.Message}");
            }
        }

        protected override NetworkFunctionCollection WriteMainPanelEnum(string path, string documentPath)
        {
            string lastRegion = "";
            string lastComment = "";
            bool inRegion = false;

            List<string> commands = new List<string> { };
            commands.Add("internal enum mainpanel {");
            foreach (Match m in FindMainPanelSections())
            {
                if (m.Groups["function"].Success)
                {
                    string[] correctValues = MainPanelCorrections(m.Groups["function"].Value.Trim(), m.Groups["arg"].Value);
                    if (m.Groups["function"].Value.Contains("--"))
                    {
                        commands.Add($"    //    {correctValues[0]}");
                        continue;
                    }
                    else if (int.TryParse(m.Groups["arg"].Value, out int argCode))
                    {
                        commands.Add($"    {correctValues[0]} = {argCode}, {correctValues[2]}");
                        if (lastRegion != lastComment)
                        {
                            if (inRegion)
                            {
                                SourceCodeFunctions.Add($"#endregion {lastRegion}");
                                inRegion = false;
                            }
                            SourceCodeFunctions.Add($"#region {lastComment}");
                            lastRegion = lastComment;
                            inRegion = true;
                        }
                        if (m.Groups["functionType"].Value == "addParamController")
                        {
                            SourceCodeFunctions.Add(MainPanelCreateFunction("FlagValue", correctValues[0], correctValues[1], lastComment, m.Groups["name"].Value));
                            continue;
                        }
                        else
                        {
                            SourceCodeFunctions.Add(MainPanelCreateFunction("NetworkValue", correctValues[0], correctValues[1], lastComment, m.Groups["name"].Value, $"Number representation of a value between {m.Groups["input"].Captures[0].Value} and {m.Groups["input"].Captures[m.Groups["input"].Captures.Count - 1].Value}", $"Numeric value between {m.Groups["output"].Captures[0].Value} and {m.Groups["output"].Captures[m.Groups["output"].Captures.Count - 1].Value}"));
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (m.Groups["comment"].Success)
                {
                    lastComment = m.Groups["comment"].Value.Trim();
                    commands.Add($"    //    {lastComment}");
                    continue;
                }
                else
                {
                    continue;
                }

            }
            if (inRegion)
            {
                SourceCodeFunctions.Add($"#endregion {lastRegion}");
                inRegion = false;
            }
            commands.Add("    }");
            //devices.Add("}");
            string fn = "";
            bool append = false;
            WriteFunctions("FlagValues", append);

            string outputFilename = Path.Combine(documentPath, $"{fn}{(fn == "" ? "" : "_")}mainpanel.cs");
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(outputFilename, append: append))
                {
                    Logger.Debug($"Writing devices enumeration to file: \"{outputFilename}\"");
                    foreach (string command in commands)
                    {
                        streamWriter.WriteLine(command);
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Error($"Exception while writing file: \"{outputFilename}\".  {ex.Message}");
            }

            return NetworkFunctions;
        }
        internal override NetworkFunctionCollection GetNetworkFunctions(BaseUDPInterface udpInterface)
        {
            UDPInterface = udpInterface;
            return WriteMainPanelEnum(ModulePath, DocumentPath);
        }
        protected MatchCollection FindMainPanelSections()
        {
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            Regex regex = new Regex(SectionPattern, options);
            return regex.Matches(MainPanel);
        }

        protected override string[] MainPanelCorrections(string functionName, string arg)
        {
            return new string[3] { functionName, arg, "" };
        }
        virtual protected string MainPanelCreateFunction(string function, string functionname, string arg, string device, string name)
        {
            return MainPanelCreateFunction(function, functionname, arg, device, name, "", "");
        }

        protected override string MainPanelCreateFunction(string function, string functionname, string arg, string device, string name, string description = "", string valuedescription = "")
        {
            return string.Empty;
        }
        /// <summary>
        /// Saves the c# AddFunction statements to an external file for later inclusion in the interface.
        /// </summary>
        protected void WriteFunctions(string fn = "", bool append = false)
        {
            string outputFilename = Path.Combine(_documentPath, $"{fn}{(fn == "" ? "" : "_")}InterfaceAddFunctions.txt");

            try {
                using (StreamWriter streamWriter = new StreamWriter(outputFilename, append: append))
                {
                    Logger.Debug($"Writing Interface Functions to file: \"{outputFilename}\"");
                    foreach (string a in SourceCodeFunctions)
                    {
                        streamWriter.WriteLine(a);
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Error($"Exception while writing file: \"{outputFilename}\".  {ex.Message}");
            }
        }

        protected override void AddFunction(NetworkFunction netFunction)
        {
            NetworkFunctions.Add(netFunction);
        }
        #region properties
        protected override string MainPanel
        {
            get => _mainPanel;
        }
        protected override string SectionPattern
        {
            get => _sectionPattern;
            set => _sectionPattern = value;
        }
        protected override NetworkFunctionCollection NetworkFunctions
        {
            get => _networkFunctions;
            set => _networkFunctions = value;
        }
        protected override List<string> SourceCodeFunctions
        {
            get => _enumSourceCode;
            set => _enumSourceCode = value;
        }
        protected override string DocumentPath
        {
            get => _documentPath;
            set => _documentPath = value;
        }
        protected override BaseUDPInterface UDPInterface
        {
            get => _udpInterface;   
            set => _udpInterface = value;   
        }
        protected override string ModulePath
        {
            get => _modulePath;
            set => _modulePath = value;
        }
        #endregion properties
    }
}
