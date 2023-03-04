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
    /// Relates to methods needed to take the DCS Devices.Lua for
    /// a module and create the contents in a form which is usable
    /// by Helios
    /// </summary>
    abstract class BaseDCSDevicesCreator
    {
        protected abstract void WriteDevicesEnum(string inputPath, string documentPath, string nameSpace);
        #region properties
        protected abstract string Devices { get; set; }
        protected abstract string DevicesPattern { get; set; }
        protected abstract string DocumentPath { get; set; }
        protected abstract string ModulePath { get; set; }
        protected abstract List<string> SourceCodeLines { get; set; }
        protected abstract string NameSpace { get; set; }
        #endregion

    }
}
