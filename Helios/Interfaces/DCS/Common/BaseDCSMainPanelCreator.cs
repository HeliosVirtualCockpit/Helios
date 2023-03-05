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
    /// Relates to methods needed to take the DCS MainPanel.Lua for
    /// a module and create the contents in a form which is usable
    /// by Helios
    /// </summary>
    abstract class BaseDCSMainPanelCreator
    {
/// <summary>
/// 
/// </summary>
/// <param name="function"></param>
/// <param name="functionname"></param>
/// <param name="arg"></param>
/// <param name="device"></param>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="valuedescription"></param>
/// <param name="inputs">array of input values for the function</param>
/// <param name="outputs">array of output values for the function</param>
/// <returns></returns>
        protected abstract string MainPanelCreateFunction(string function, string functionname, string arg, string device, string name, string description, string valuedescription, double[] inputs, double[] outputs);
        protected abstract NetworkFunctionCollection WriteMainPanelEnum(string inputPath, string documentPath);
        protected abstract void AddFunction(NetworkFunction netFunction);
        internal abstract NetworkFunctionCollection GetNetworkFunctions(BaseUDPInterface udpInterface);
        protected abstract string[] MainPanelCorrections(string functionName, string arg);
        #region properties
        protected abstract string MainPanel { get; }
        protected abstract string SectionPattern { get; set; }
        protected abstract string DocumentPath { get; set; }
        protected abstract string ModulePath { get; set; }
        protected abstract NetworkFunctionCollection NetworkFunctions { get; set; }
        protected abstract List<string> SourceCodeFunctions { get; set; }
        protected abstract BaseUDPInterface UDPInterface { get; set; }
        #endregion

    }
}
