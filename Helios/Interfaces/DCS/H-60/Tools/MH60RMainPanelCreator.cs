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
using System.Windows.Documents;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.H_60.Tools
{
    internal class MH60RMainPanelCreator : H60MainPanelCreator
    {
        internal MH60RMainPanelCreator(string path, string documentPath) : base(path, documentPath)
        {
            SectionPattern = @"(?'startcomment'--\[\[)(?:[.\n\r\t\s\S]*)(?'-startcomment'\]\])|^((?<function>\w*)(?=.*\=\s*CreateGauge\(""parameter""\))).*[\r\n]*.*\.arg_number\s*\=\s*(?<arg>\d{1,4}).*[\r\n]*.*\.input\s*\=\s*\{(?:(?<input>[a-z0-9\.\-\(\)]*)[\,\s\}]+)+(?:--.*)*[\r\n].*\.output\s*\=\s*\{(?:(?<output>[0-9\.\-]*)[\,\s\}]+)+(?:--.*)*[\r\n].*\.parameter_name\s*\=\s*""(?<name>.*)""|^((?<function>.*)\s{0,20}=\s*(?=CreateGauge\(""parameter""\)))|(?:(?<function>[a-zA-Z0-9_\-]*)\s*\=\s*(?<functionType>.*)\((?<arg>\d{1,4})[\,]{1}\s*""(?<name>.*)"".*[\n\r]+)|(?:(?<function>^[a-zA-Z0-9_-[\.]]+)\s*\=.*[\t\n\r\s]*[\}])|^(?:\s*--\s*)(?<comment>[a-zA-Z0-9_\/\-\s&]*)[\r\n]{1,2}";
        }

        /// <summary>
        /// specific changes needed due to "anomalies" in the 
        /// input file which we do not want to correct in the 
        /// input file.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected override string[] MainPanelCorrections(string functionName, string arg)
        {
            string comment = $"// * * * Helios correction: previously {functionName}";
            switch (arg)
            {
                case "453":
                    functionName = "pduCpltOverspeed1";
                    break;
                case "454":
                    functionName = "pduCpltOverspeed2";
                    break;
                case "61":
                    functionName = "pilotBaroAlt1000s";
                    break;
                case "62":
                    functionName = "pilotBaroAlt10000s";
                    break;
                case "64":
                    functionName = "pilotPressureScale1";
                    break;
                case "65":
                    functionName = "pilotPressureScale2";
                    break;
                case "66":
                    functionName = "pilotPressureScale3";
                    break;
                case "67":
                    functionName = "pilotPressureScale4";
                    break;
                case "68":
                    functionName = "pilotBaroAltEncoderFlag";
                    break;
                default:
                    comment = "";
                    break;
            }
            return new string[3] { functionName, arg, comment };
        }


        protected override string MainPanelCreateFunction(string function, string functionname, string arg, string device, string name, string description = "", string valuedescription = "")
        {
            string sourceCode;
            int argNumber = int.Parse(arg);
            if (function == "FlagValue" && ((argNumber <= 558 && argNumber >= 554) || (argNumber <= 67 && argNumber >= 60) || (argNumber <= 77 && argNumber >= 70)))
            {
                function = "NetworkValue";
                valuedescription = "numeric value between 0.0 and 1.0";
            }
            if (functionname.Contains("pilotVSI"))
            {
                device = "PILOT VSI";
            }
            if (functionname.Contains("copilotVSI"))
            {
                device = "COPILOT VSI";
            }
            if (functionname.Contains("StabInd"))
            {
                device = "Stabilator";
            }
            if (functionname.Contains("Door"))
            {
                device = "Doors";
            }
            switch (function)
            {
                case "FlagValue":
                    AddFunction(new FlagValue(UDPInterface, arg, device, name, "", "%1d"));
                    sourceCode = $"AddFunction(new FlagValue(this,  mainpanel.{functionname}.ToString(\"d\"), \"{device}\", \"{name}\",\"\", \"%1d\"));";
                    break;
                case "NetworkValue":
                    name = name == "" ? functionname : name;
                    AddFunction(new NetworkValue(UDPInterface, arg, device, name, description, valuedescription, BindingValueUnits.Numeric, "%0.3f"));
                    sourceCode = $"AddFunction(new NetworkValue(this,  mainpanel.{functionname}.ToString(\"d\"), \"{device}\", \"{name}\", \"{description}\", \"{valuedescription}\", BindingValueUnits.Numeric, \"%0.3f\"));";
                    break;
                default:
                    sourceCode = "";
                    break;

            }
            return sourceCode;
        }
        #region properties
        #endregion
    }
}
