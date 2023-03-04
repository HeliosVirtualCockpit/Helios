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
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GadrocsWorkshop.Helios.UDPInterface;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    /// <summary>
    /// Relates to methods needed to take the DCS dommand_defs.Lua for
    /// a module and create the contents in a form which is usable
    /// by Helios
    /// </summary>
    abstract class BaseDCSCommandsCreator
    {
        /// <summary>
        /// Method to take process the Command_Def file, and write out
        /// the c# source code for the enumeration 
        /// </summary>
        protected abstract void WriteCommandsEnum();
        #region properties
        /// <summary>
        /// string representation of the complete contents of the command_defs.lua file
        /// </summary>
        protected abstract string Commands { get; set; }
        /// <summary>
        /// Regex pattern for processing the command_defs contents
        /// </summary>
        protected abstract string CommandsPattern { get; set; }
        /// <summary>
        /// This is the output path for the c# source files to be 
        /// written to
        /// </summary>
        protected abstract string DocumentPath { get; set; }
        /// <summary>
        /// This holds the location of the DCS module script files
        /// </summary>
        protected abstract string ModulePath { get; set; }
        /// <summary>
        /// List containing all of the lines of c# source code
        /// </summary>
        protected abstract List<string> SourceCodeLines { get; set; }
        /// <summary>
        /// This is the classname which appears in the command enumerations source code
        /// </summary>
        protected abstract string ClassName { get; set; }
        #endregion

    }
}
