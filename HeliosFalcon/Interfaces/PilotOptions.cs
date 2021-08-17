// Copyright 2021 Helios Contributors
//
// HeliosFalcon is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HeliosFalcon is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.Interfaces
{
    [XmlRoot("PilotOptions", Namespace = XML_NAMESPACE)]
    public class PilotOptions : HeliosXmlModel
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string CONFIGURATION_GROUP = "FalconPilotOptions";
        private const string CONFIGURATION_SETTING_ENABLED = "AllowPilotOptions";

        // our schema identifier, in case of future configuration model changes
        public const string XML_NAMESPACE =
            "http://Helios.local/HeliosFalcon/Interfaces/PilotOptions";

        /// <summary>
        /// backing field for property Enabled, contains
        /// true if updating piot options file allowed on this machine (not per-profile)
        /// </summary>
        private bool _enabled = ConfigManager.SettingsManager.LoadSetting(CONFIGURATION_GROUP, CONFIGURATION_SETTING_ENABLED, false);

        /// <summary>
        /// backing field for PilotCallsign
        /// </summary>
        private string _pilotCallsign;

        /// <summary>
        /// backing field for ForceKeyFile
        /// </summary>
        private bool _forceKeyFile;


        internal IEnumerable<StatusReportItem> OnStatusReport()
        {
            return CheckCommonItems();
        }

        internal IEnumerable<StatusReportItem> OnReadyCheck()
        {
            return CheckCommonItems();
        }

        private IEnumerable<StatusReportItem> CheckCommonItems()
        {
            if(Enabled)
            {
                if (PilotCallsign.Equals(""))
                {
                    yield return new StatusReportItem
                    {
                        Status = $"Pilot Callsign not set in BMS",
                        Severity = StatusReportItem.SeverityCode.Error,
                        Recommendation = "Run Falcon and set your pilot callsign"
                    };
                }
            }
        }

        public PilotOptions() : base(XML_NAMESPACE)
        {
           //no-op
        }

        internal void OnProfileStart()
        {
            /*
             * Gets the pilot callsign and populates PilotCallsign property
             */
            GetPilotCallsign();

            /*
             * Check to see if we need to rewrite pilot options file
             */
            if (Enabled)
            {
                if (PilotCallsign != "")
                {
                    Logger.Info("Profile has set pilot callsign " + PilotCallsign + " to use key file " + Parent.KeyFileName);
                    SetPilotOptions();
                }
                else
                {
                    Logger.Warn("Profile is set to force key file usage but the pilot callsign is not set in Falcon install");
                }
            }
        }

        public void GetPilotCallsign()
        {
            Microsoft.Win32.RegistryKey pathKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Parent.FalconRootKey + Parent.FalconVersion);

            if (pathKey != null)
            {
                try
                {
                    PilotCallsign = System.Text.Encoding.UTF8.GetString((byte[])pathKey.GetValue("PilotCallsign")).Replace("\0", "");
                }
                catch { }
            }
            else
            {
                PilotCallsign = null;
            }
        }

        private void SetPilotOptions()
        {
            var popFile = Path.Combine(Parent.FalconPath, "User", "Config", PilotCallsign + ".pop");
            var backupDir = Path.Combine(Parent.FalconPath, "User", "Config", "Helios");
            var backupPopFile = Path.Combine(backupDir, PilotCallsign + ".pop");

            if (File.Exists(popFile))
            {
                if (!File.Exists(backupPopFile))
                {
                    if (!Directory.Exists(backupDir))
                    {
                        _ = Directory.CreateDirectory(backupDir);
                    }
                    File.Copy(popFile, backupPopFile, true);
                    Logger.Debug("File " + Path.GetFileName(popFile) + " has been backed up to " + backupDir);
                }

                File.SetAttributes(popFile, File.GetAttributes(popFile) & ~FileAttributes.ReadOnly);

                FileStream fileStream = new FileStream(popFile, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[fileStream.Length];
                _ = fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();


                byte[] keyFileName = System.Text.Encoding.ASCII.GetBytes(Path.GetFileName(Parent.KeyFileName).Replace(".key", ""));
                for (int i = 0; i <= 15; i++)
                {
                    if (i >= keyFileName.Length)
                    {
                        bytes[336 + i] = 0x00;
                        continue;
                    }
                    bytes[336 + i] = keyFileName[i];
                }

                fileStream = new FileStream
                    (popFile, FileMode.Create, FileAccess.Write);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
                Logger.Debug(popFile + " has been modified to load key file " + Path.GetFileName(Parent.KeyFileName) + " by default");
            }
            else
            {
                Logger.Error("FILE NOT FOUND! " + popFile + " Failed to force key file usage in Falcon");
            }
        }

        #region Properties
        public string PilotCallsign
        {
            get 
            { 
                return _pilotCallsign;
            }
            set
            {
                var oldValue = _pilotCallsign;
                _pilotCallsign = value;
                OnPropertyChanged("PilotCallsign", oldValue, value, true);
            }
        }

        public bool ForceKeyFile
        {
            get { return _forceKeyFile; }
            set
            {
                var oldValue = _forceKeyFile;
                _forceKeyFile = value;
                OnPropertyChanged("ForceKeyFile", oldValue, value, true);
            }
        }

        /// <summary>
        /// true if pilot options updates are allowed on this machine (not per-profile)
        /// </summary>
        [XmlIgnore]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                {
                    return;
                }

                bool oldValue = _enabled;
                _enabled = value;
                OnPropertyChanged(nameof(Enabled), oldValue, value, true);
                ConfigManager.SettingsManager.SaveSetting(CONFIGURATION_GROUP, CONFIGURATION_SETTING_ENABLED, value);
            }
        }

        [XmlIgnore]
        public IFalconInterfaceHost Parent { get; internal set; }

        #endregion
    }
}
