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
    internal class DCSCommandsCreator: BaseDCSCommandsCreator
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<string> _enumSourceCode = new List<string>();
        private string _commands = string.Empty;
        private string _commandsPattern = string.Empty;
        private string _documentPath = string.Empty;
        private string _modulePath = string.Empty;
        private string _className;
        internal DCSCommandsCreator(string path, string documentPath)
        {
            ModulePath = Path.Combine(path, "Cockpit", "Scripts", "command_defs.lua");
            Logger.Debug($"Reading DCS Module Command Definitions from file: {ModulePath}");
            SourceCodeLines.Clear();
            DocumentPath = documentPath;
            try
            {
                using (StreamReader streamReader = new StreamReader(ModulePath))
                {
                    Commands = streamReader.ReadToEnd();
                }
            }
            catch(IOException ex){
                Logger.Error($"Exception while reading file: {ModulePath}.  {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the c# source code for the devices enumeration
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <summary>
        /// Creates the c# source code for the Commands enumeration
        /// </summary>
        /// <returns></returns>
        protected override void WriteCommandsEnum()
        {
            bool firstTime = true;
            int counter = 0;
            SourceCodeLines.Add($"internal class {ClassName}");
            SourceCodeLines.Add("{");
            foreach (Match m in FindCommands())
            {
                if (int.TryParse(m.Groups["startcount"].Value, out int updateCounter))
                {
                    counter = updateCounter;
                    continue;
                }
                else if (m.Groups["functionname"].Success)
                {
                    if (!firstTime) SourceCodeLines.Add("}");
                    firstTime = false;
                    SourceCodeLines.Add($"    internal enum {m.Groups["functionname"].Value} {{");
                    continue;
                }
                else if (m.Groups["command"].Success)
                {
                    if (m.Groups["command"].Value.Contains("--"))
                    {
                        SourceCodeLines.Add($"    //    {m.Groups["command"].Value.Trim()}");
                        continue;
                    }
                    if (int.TryParse(m.Groups["commandval"].Value, out int commandCode))
                    {
                        SourceCodeLines.Add($"    {(CommandValidation(m.Groups["command"].Value.Trim(),commandCode.ToString()))} = {commandCode},");
                        continue;
                    }
                    else
                    {
                        SourceCodeLines.Add($"    {(CommandValidation(m.Groups["command"].Value.Trim(), (counter + 1).ToString()))} = {++counter},");
                        continue;
                    }
                }
                else if (m.Groups["comment"].Success)
                {
                    SourceCodeLines.Add($"    //    {m.Groups["comment"].Value.Trim()}");
                    continue;
                }
                else if (m.Groups["commented_command"].Success)
                {
                    SourceCodeLines.Add($"    //    {m.Groups["commented_command"].Value.Trim()}");
                    continue;
                }
                else
                {
                    continue;
                }

            }
            SourceCodeLines.Add("}");
            SourceCodeLines.Add("}");
            bool append = false;
            WriteEnumClass(SourceCodeLines, ClassName, append);
        }

        virtual protected void WriteEnumClass(List<string> sourceCodeLines, string fileNameModifier = "", bool append = false)
        {
            string outputFilename = Path.Combine(DocumentPath, $"{fileNameModifier}{(fileNameModifier == "" ? "" : "_")}commands.cs");

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(outputFilename, append: append))
                {
                    Logger.Debug($"Writing command definition enumerations to file: \"{outputFilename}\"");
                    foreach (string line in SourceCodeLines)
                    {
                        streamWriter.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Error($"Exception while writing file: \"{outputFilename}\".  {ex.Message}");
            }

        }
        private MatchCollection FindCommands()
        {
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            Regex regex = new Regex(CommandsPattern, options);
            return regex.Matches(Commands ?? String.Empty);
        }
        virtual protected string CommandValidation(string command, string commandCode = "")
        {
            return command;
        }
        #region properties
        protected override string Commands
        {
            get => _commands;
            set => _commands = value;
        }
        protected override string CommandsPattern
        {
            get => _commandsPattern;
            set => _commandsPattern = value;
        }
        protected override List<string> SourceCodeLines
        {
            get => _enumSourceCode;
            set => _enumSourceCode = value;
        }
        protected override string DocumentPath
        {
            get => _documentPath;
            set => _documentPath = value;
        }
        protected override string ModulePath
        {
            get => _modulePath;
            set => _modulePath = value;
        }
        protected override string ClassName
        {
            get => _className;
            set => _className = value;
        }
        #endregion properties
    }
}
