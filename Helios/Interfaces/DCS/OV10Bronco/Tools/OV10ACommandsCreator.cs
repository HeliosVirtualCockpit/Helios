﻿//  Copyright 2023 Helios Contributors
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


namespace GadrocsWorkshop.Helios.Interfaces.DCS.OV10Bronco.Tools
{
    internal class OV10ACommandsCreator : DCSCommandsCreator
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal OV10ACommandsCreator(string path, string documentPath) : base(path, documentPath)
        {
            CommandsPattern = @"(?'startcomment'--\[\[)(?:[.\n\r\t\s\S]*)(?'-startcomment'\]\])|count\s*\=\s*(?<startcount>\d{1,5})|(?:(?<functionname>[a-zA-Z0-9_]+).*\=.*[\t\n\r\s]*\{)|(?:(?<command>[a-zA-Z0-9_]*)\s*\=\s*(?<commandval>.*)[\,\}]{1}.*[\n\r]+)|(?:\s*--\s*)(?<comment>[a-zA-Z0-9_\/\-\s&]*)[\n\r]+|\s--(?:(?<commented_command>[a-zA-Z0-9_]*)\s*\=\s*(?<commandval>.*)[\,\}]{1}.*[\n\r]+)";
            ClassName = "OV10ACommands";
            WriteCommandsEnum();
        }
        #region properties
        #endregion properties
    }
}
