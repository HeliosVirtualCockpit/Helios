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
    internal class DCSDevicesCreator: BaseDCSDevicesCreator
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<string> _enumSourceCode = new List<string>();
        private string _devices = string.Empty;
        private string _devicesPattern = string.Empty;
        private string _documentPath = string.Empty;
        private string _modulePath = string.Empty;
        private string _nameSpace;
        internal DCSDevicesCreator(string path, string documentPath)
        {
            ModulePath = Path.Combine(path, "Cockpit", "Scripts", "devices.lua");
            Logger.Debug($"Reading DCS Module Device Definitions from file: {ModulePath}");
            SourceCodeLines.Clear();
            DocumentPath = documentPath;
            try
            {
                using (StreamReader streamReader = new StreamReader(ModulePath))
                {
                    Devices = streamReader.ReadToEnd();
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
        protected override void WriteDevicesEnum(string path, string documentPath, string nameSpace = "")
        {
            SourceCodeLines.Add($"namespace {(nameSpace==""?NameSpace:nameSpace)}");
            SourceCodeLines.Add("{");
            SourceCodeLines.Add("    internal enum devices {");
            path = Path.Combine(path, "Cockpit", "Scripts", "devices.lua");
            int counter = 1;
            foreach (Match m in FindDevices())
            {
                string d = m.Groups["dev"].Value;
                SourceCodeLines.Add($"{m.Groups["dev"].Value} = {counter++},");
            }
            SourceCodeLines.Add("    }");
            SourceCodeLines.Add("}");
            string fn = "";
            bool append = false;
            string outputFilename = Path.Combine(DocumentPath, $"{fn}{(fn == "" ? "" : "_")}devices.cs");
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(outputFilename, append: append))
                {
                    Logger.Debug($"Writing devices enumeration to file: \"{outputFilename}\"");
                    foreach (string line in SourceCodeLines)
                    {
                        streamWriter.WriteLine(line);
                    }
                }
            } catch (IOException ex)
            {
                Logger.Error($"Exception while writing file: \"{outputFilename}\".  {ex.Message}");
            }
            return;
        }

        private MatchCollection FindDevices()
        {
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Compiled;
            Regex regex = new Regex(DevicesPattern, options);
            return regex.Matches(Devices ?? String.Empty);
        }

        #region properties
        protected override string Devices
        {
            get => _devices;
            set => _devices = value;
        }
        protected override string DevicesPattern
        {
            get => _devicesPattern;
            set => _devicesPattern = value;
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
        protected override string NameSpace
        {
            get => _nameSpace;
            set => _nameSpace = value;
        }
        #endregion properties
    }
}
