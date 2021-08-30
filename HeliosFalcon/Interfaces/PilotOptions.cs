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

using GadrocsWorkshop.Helios.Windows;
using GadrocsWorkshop.Helios.Windows.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

/*
 Design:
    PilotOptions is a Helios global settings that the user enables to allow
    Helios to update the Pilot Options file of the current Falcon BMS callsign.
    
    Currently only the key file being used by the current profile will be updated
    in the callsign.pop file.

    The callsign.pop file is NOT a Helios managed file but does require Helios
    to backup the file prior to update.

Profile Users:

- on load
    - Check if PilotOptions is enabled and if not go to special status
- on status report / ready check
    - If Enabled but callsign is not available report an error
- on profile start
    - Update callsign.pop file if PilotOptions enabled
- interactive
    - on enable, if file exists show dialog and backup file (Profile Editor)
 */



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
        /// backing field for property EnabledCommand, contains
        /// handler for interaction with the visual representation (such as checkbox) of the Enabled property
        /// </summary>
        private ICommand _enabledCommand;

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
                if (null == PilotCallsign)
                {
                    yield return new StatusReportItem
                    {
                        Status = $"Pilot Callsign not set in BMS",
                        Severity = StatusReportItem.SeverityCode.Error,
                        Recommendation = "Run Falcon and set your pilot callsign"
                    };
                }

                if(null == Parent.KeyFileName && null == PilotCallsign)
                {
                    yield return new StatusReportItem
                    {
                        Status = $"No Key File defined in profile",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Recommendation = "Set a valid Key File for this profile in order for the Force Key File feature to work properly"
                    };
                }

                if(PilotCallsign != "")
                {
                    yield return new StatusReportItem
                    {
                        Status = $"Profile has set pilot callsign  {PilotCallsign} to use key file { Parent.KeyFileName }",
                        Severity = StatusReportItem.SeverityCode.Info
                    };
                }

                CalculatePaths(out string _, out string outputPath);
                if (!File.Exists(outputPath))
                {
                    yield return new StatusReportItem
                    {
                        Status = $"Pilot Options file {outputPath} is missing from Falcon installation.",
                        Severity = StatusReportItem.SeverityCode.Error,
                        Recommendation = "Check your Falcon setup"
                    };
                }
            }
        }

        public PilotOptions() : base(XML_NAMESPACE)
        {
            // nothing to do here
        }

        internal void OnProfileStart()
        {
            /*
             * Set the Pilot Callsign if Enabled
             */
            if(Enabled)
            {
                SetPilotOptions();
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
            CalculatePaths(out string _, out string outputPath);

            if (File.Exists(outputPath))
            {
                File.SetAttributes(outputPath, File.GetAttributes(outputPath) & ~FileAttributes.ReadOnly);

                FileStream fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
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
                    (outputPath, FileMode.Create, FileAccess.Write);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
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

        private void CalculatePaths(out string outputDirectory, out string outputPath)
        {
            outputDirectory = Path.Combine(Parent.FalconPath, "User", "Config");
            outputPath = Path.Combine(outputDirectory, PilotCallsign + ".pop");
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

        /// <summary>
        /// handler for interaction with the visual representation (such as checkbox) of the Enabled property
        /// </summary>
        public ICommand EnabledCommand
        {
            get
            {
                _enabledCommand = _enabledCommand ?? new RelayCommand(parameter =>
                {
                    CheckBox source = (CheckBox)parameter;
                    if (!source.IsChecked ?? false)
                    {
                        // nothing to do here
                        return;
                    }

                    OnInteractivelyEnabled(source);
                });
                return _enabledCommand;
            }
        }

        internal void OnInteractivelyEnabled(CheckBox source)
        {
            CalculatePaths(out string _, out string outputPath);
            
            if (!File.Exists(outputPath))
            {
                //We can't enable if there is no Pilot Options file
                Enabled = false;
                return;
            }

            // determine what the backup file might be right now
            string backupPath = Path.ChangeExtension(outputPath, "pop.txt");
            int n = 1;
            while (File.Exists(backupPath))
            {
                n++;
                backupPath = Path.ChangeExtension(outputPath, $"pop{n}.txt");
            }

            // display a warning
            InstallationDangerPromptModel warningModel = new InstallationDangerPromptModel
            {
                Title = "Advanced Operation Requested",
                Message = "You are about to enable the Falcon Force Key File feature of Helios.  Doing so will grant Helios permission to change this file when you start a profile.",
                Info = new List<StructuredInfo>
                {
                    new StructuredInfo
                    {
                        Message = $"A backup of your current Pilot Options file '{outputPath}' will be stored at {backupPath} for you."
                    }
                }
            };
            Dialog.ShowModalCommand.Execute(new ShowModalParameter
            {
                Content = warningModel
            }, source);

            switch (warningModel.Result)
            {
                case InstallationPromptResult.Cancel:
                    {
                        // undo it
                        Enabled = false;
                        break;
                    }

                case InstallationPromptResult.Ok:
                    {
                        // take over, so from now on we own this file
                        File.Copy(outputPath, backupPath);
                        SetPilotOptions();
                        break;
                    }
            }
        }

        [XmlIgnore]
        public IFalconInterfaceHost Parent { get; internal set; }

        internal void OnLoaded()
        {
            if (!Enabled)
            {
                return;
            }

            GetPilotCallsign();
        }

        #endregion
    }
}
