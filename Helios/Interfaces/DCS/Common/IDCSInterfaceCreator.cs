using GadrocsWorkshop.Helios.UDPInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    internal interface IDCSInterfaceCreator
    {
        MatchCollection GetSections(string clickablesFromDCS);
        MatchCollection GetElements(string section);
        void ProcessFunction(Match match);
        NetworkFunctionCollection CreateFunctionsFromDcsModule(BaseUDPInterface UDPInterface, string path);
        string[] ParseDeviceGroup(Group deviceGroup);
    }
}
