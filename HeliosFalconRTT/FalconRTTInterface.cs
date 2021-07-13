//  Copyright 2014 Craig Courtney
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
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Falcon;

namespace GadrocsWorkshop.Helios.Interfaces.FalconRTT
{
    [HeliosInterface("Helios.FalconRTT.Interface", "Falcon RTT", typeof(FalconRTTInterfaceEditor), typeof(UniqueHeliosInterfaceFactory))]
    public class FalconRTTInterface : HeliosInterface, IReadyCheck, IStatusReportNotify, IExtendedDescription
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected enum RTTKEY
        {
            HUD,
            PFL,
            DED,
            RWR,
            MFDLEFT,
            MFDRIGHT,
            HMS
        }
        private Dictionary<string, string> _rttConfig = new Dictionary<string, string>{
            {"RENDERER", "0" },
            {"NETWORKED", "0" },
            {"HOST","127.0.0.1" },
            {"PORT","44000" },
            {"DATA_F4","0" },
            {"DATABMS","0" },
            {"DATA_OBS","0" },
            {"DATA_IVIBE","0" },
            {"FPS","30" },
            {"USE_HUD","0" },
            {"USE_PFL","0" },
            {"USE_DED","0" },
            {"USE_RWR","0" },
            {"USE_MFDLEFT","0" },
            {"USE_MFDRIGHT","0" },
            {"USE_HMS","0" }
            };
        private readonly FalconInterface falconInterface;

        public FalconRTTInterface()
            : base("FalconRTT")
        {
            string falconPath = falconInterface.FalconPath;
            _rttConfig = ReadRTTConfig(Path.Combine(falconPath, "Tools", "RTTRemote", "RTTClient.ini"));

        }

        private Dictionary<string,string> ReadRTTConfig(string inipath)
        {
            var dict = new Dictionary<string, string>();
            foreach (var line in File.ReadAllLines(inipath))
            {
                var parts = line.Split('=');
                dict.Add(parts[0], parts[1]);
            }
            return dict;
        }

        public string Description => $"This interface provides configuration and execution of Falcon BMS RTT client utility";
        public string RemovalNarrative => $"Removing this interface will remove the ability to configure and execute the Falcon BMS RTT client utility";

        public void InvalidateStatusReport()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            throw new NotImplementedException();
        }

        public void PublishStatusReport(IList<StatusReportItem> statusReport)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IStatusReportObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IStatusReportObserver observer)
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
