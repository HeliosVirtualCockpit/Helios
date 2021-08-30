//  Copyright 2014 Craig Courtney
//  Copyright 2021 Helios Contributors
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Common;
using Microsoft.Win32;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon
{
    [HeliosInterface("Helios.Falcon.Interface", "Falcon", typeof(FalconInterfaceEditor), typeof(UniqueHeliosInterfaceFactory))]
    public class FalconInterface : ViewportCompilerInterface<
        Interfaces.RTT.ShadowMonitor, Interfaces.RTT.ShadowMonitorEventArgs>, Interfaces.IFalconInterfaceHost
    {
        private readonly string _falconRootKey = @"SOFTWARE\WOW6432Node\Benchmark Sims\";
        private FalconTypes _falconType;
        private string _falconPath;
        
        private string _currentTheater;
        private string _keyFile;
        private string _cockpitDatFile;
        private bool _focusAssist;
        private string _falconVersion;
        private string[] _falconVersions;
        private Version _falconProfileVersion;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private FalconDataExporter _dataExporter;
        private FalconKeyFile _callbacks = new FalconKeyFile("");
		private bool _inFlight;
		private bool _inFlightLastValue;

        public FalconInterface()
            : base("Falcon")
        {
            FalconType = FalconTypes.BMS;
            _falconVersions = GetFalconVersions();
            _falconPath = GetFalconPath();
            _currentTheater = GetCurrentTheater();

            _dataExporter = new BMS.BMSFalconDataExporter(this);

            HeliosAction sendAction = new HeliosAction(this, "", "callback", "send", "Press and releases a keyboard callback for falcon.", "Callback name", BindingValueUnits.Text)
            {
                ActionBindingDescription = "send %value% callback for falcon.",
                ActionInputBindingDescription = "send %value% callback",
                ValueEditorType = typeof(FalconCallbackValueEditor)
            };
            sendAction.Execute += SendAction_Execute;
            Actions.Add(sendAction);

            HeliosAction pressAction = new HeliosAction(this, "", "callback", "press", "Press a keyboard callback for falcon and leave it pressed.", "Callback name", BindingValueUnits.Text)
            {
                ActionBindingDescription = "press %value% callback for falcon.",
                ActionInputBindingDescription = "press %value% callback",
                ValueEditorType = typeof(FalconCallbackValueEditor)
            };
            pressAction.Execute += PressAction_Execute;
            Actions.Add(pressAction);

            HeliosAction releaseAction = new HeliosAction(this, "", "callback", "release", "Releases a previously pressed keyboard callback for falcon.", "Callback name", BindingValueUnits.Text)
            {
                ActionBindingDescription = "release %value% callback for falcon.",
                ActionInputBindingDescription = "release %value% callback",
                ValueEditorType = typeof(FalconCallbackValueEditor)
            };
            releaseAction.Execute += ReleaseAction_Execute;
            Actions.Add(releaseAction);
        }

        #region Properties
        public string FalconRootKey => _falconRootKey;
        public string CurrentTheater { get { return _currentTheater; } }
        
        
        public bool FocusAssist
        {
            get { return _focusAssist; }
            set
            {
                var oldValue = _focusAssist;
                _focusAssist = value;
                OnPropertyChanged("FocusAssist", oldValue, value, true);
            }
        }

        public string[] FalconVersions
        {
            get
            {
                return _falconVersions;
            }
        }

        public string FalconVersion
        {
            get
            {
                if(_falconVersion == null && _falconVersions != null)
                {
                    _falconVersion = _falconVersions[0];
                }
                return _falconVersion;
            }
            set
            {
                if(_falconVersion == null && value != null ||
                    _falconVersion != null && !_falconVersion.Equals(value))
                {
                    string oldValue = _falconVersion;
                    _falconVersion = value;
                    OnPropertyChanged("FalconVersion", oldValue, value, false);
                    FalconProfileVersion = ParseProfileVersion(_falconVersion);
                    FalconPath = GetFalconPath();
                }
            }
        }

        public Version FalconProfileVersion
        {
            get
            {
                return _falconProfileVersion;
            }
            set
            {
                if (_falconProfileVersion == null && value != null ||
                    _falconProfileVersion != null && !_falconProfileVersion.Equals(value))
                {
                    Version oldValue = _falconProfileVersion;
                    _falconProfileVersion = value;
                    OnPropertyChanged("FalconVersion", oldValue, value, false);
                }
            }
        }

        public FalconTypes FalconType
        {
            get
            {
                return _falconType;
            }
            set
            {
                if (!_falconType.Equals(value))
                {
                    FalconTypes oldValue = _falconType;
                    if (_dataExporter != null)
                    {
                        _dataExporter.RemoveExportData(this);
                    }

                    _falconType = value;
                    _falconPath = null;

                    switch (_falconType)
                    {
                        case FalconTypes.BMS:
                        default:
                            _dataExporter = new BMS.BMSFalconDataExporter(this);
                            KeyFileName = System.IO.Path.Combine(FalconPath, "User\\Config\\BMS - Full.key");
                            break;
                    }

                    OnPropertyChanged("FalconType", oldValue, value, true);
                    InvalidateStatusReport();
                }
            }
        }

        public FalconKeyFile KeyFile
        {
            get { return _callbacks; }
        }

        public string KeyFileName
        {
            get
            {
                if(_keyFile != null)
                {
                    return _keyFile;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((_keyFile == null && value != null)
                    || (_keyFile != null && value != null))
                {
                    string oldValue = _keyFile;
                    FalconKeyFile oldKeyFile = _callbacks;
                    _keyFile = value;
                    _callbacks = new FalconKeyFile(_keyFile);
                    OnPropertyChanged("KeyFileName", oldValue, value, true);
                    OnPropertyChanged("KeyFile", oldKeyFile, _callbacks, false);
                    InvalidateStatusReport();
                }
            }
        }

        public string CockpitDatFile
        {
            get
            {
                return _cockpitDatFile;
            }
            set
            {
                if ((_cockpitDatFile == null && value != null)
                    || (_cockpitDatFile != null && !_cockpitDatFile.Equals(value)))
                {
                    string oldValue = _cockpitDatFile;
                    _cockpitDatFile = value;
                    OnPropertyChanged("CockpitDatFile", oldValue, value, true);
                }
            }
        }

        public string FalconPath
        {
            get
            {
                return _falconPath;
            }
            set
            {
                if (_falconPath == null && value != null ||
                    _falconPath != null && !_falconPath.Equals(value))
                {
                    string oldValue = _falconPath;
                    _falconPath = value;
                    OnPropertyChanged("FalconPath", oldValue, value, false);
                }
            }
        }

        internal RadarContact[] RadarContacts => _dataExporter?.RadarContacts;

        internal List<string> NavPoints => _dataExporter?.NavPoints;

        internal bool StringDataUpdated => (bool)(_dataExporter?.StringDataUpdated);

        public string[] RwrInfo => _dataExporter?.RwrInfo;

        public Interfaces.RTT.ConfigGenerator Rtt { get; private set; }

        public Interfaces.PilotOptions PilotOptions { get; private set; }

        #endregion

        public static Version ParseProfileVersion(string versionString)
        {
            Version ver = Version.Parse(System.Text.RegularExpressions.Regex.Replace(versionString, "[A-Za-z ]", ""));
            return ver;
        }

        public string[] GetFalconVersions()
        {
            string[] subkeys = null;
            if(Registry.LocalMachine.OpenSubKey(FalconRootKey) != null)
            {
                subkeys = Registry.LocalMachine.OpenSubKey(FalconRootKey).GetSubKeyNames();
                Array.Reverse(subkeys);
            }
            return subkeys;
        }     
        public string GetFalconPath()
        {
            RegistryKey pathKey = null;
            string pathValue = null;
            pathKey = Registry.LocalMachine.OpenSubKey(FalconRootKey + FalconVersion);

            if (pathKey != null)
            {
                pathValue = (string)pathKey.GetValue("baseDir");
            }
            else
            {
                pathValue = "";
            }
            return pathValue;
        }

        public string GetCurrentTheater()
        {
            RegistryKey pathKey = null;
            string pathValue = null;
            pathKey = Registry.LocalMachine.OpenSubKey(FalconRootKey + FalconVersion);

            if (pathKey != null)
            {
                pathValue = (string)pathKey.GetValue("curTheater");
            }
            else
            {
                pathValue = "";
            }
            return pathValue;
        }


        
        public BindingValue GetValue(string device, string name)
        {
            return _dataExporter?.GetValue(device, name) ?? BindingValue.Empty;
        }

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);

            if (oldProfile != null)
            {
                oldProfile.ProfileStarted -= Profile_ProfileStarted;
                oldProfile.ProfileTick -= Profile_ProfileTick;
                oldProfile.ProfileStopped -= Profile_ProfileStopped;
            }

            if (Profile != null)
            {
                Profile.ProfileStarted += Profile_ProfileStarted;
                Profile.ProfileTick += Profile_ProfileTick;
                Profile.ProfileStopped += Profile_ProfileStopped;
            }
            InvalidateStatusReport();
        }

        void Profile_ProfileStopped(object sender, EventArgs e)
        {
            _dataExporter?.CloseData();

            if (Rtt?.Enabled ?? false)
            {
                Rtt.OnProfileStop();
            }
        }

        void Profile_ProfileTick(object sender, EventArgs e)
        {
			_dataExporter?.PollUserInterfaceData();

			BindingValue runtimeFlying = GetValue("Runtime", "Flying");
			_inFlight = runtimeFlying.BoolValue;

			if (_inFlight)
			{
				_dataExporter?.PollFlightData();
			}

			if (_inFlight != _inFlightLastValue)
			{
				if (_inFlight)
				{
					_currentTheater = GetCurrentTheater();
				}

				_inFlightLastValue = _inFlight;
			}
        }

        void Profile_ProfileStarted(object sender, EventArgs e)
        {
            

            if(PilotOptions?.Enabled ?? false)
            {
                PilotOptions.OnProfileStart();
            }

            if (Rtt?.Enabled ?? false)
            {
                Rtt.OnProfileStart();
            }

            _dataExporter?.InitData();
        }

        public override void Reset()
        {
            _currentTheater = GetCurrentTheater();
        }

        void PressAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (_callbacks.HasCallback(e.Value.StringValue))
            {
                WindowFocused(_falconType);
                _callbacks[e.Value.StringValue].Down();
            }
        }

        void ReleaseAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (_callbacks.HasCallback(e.Value.StringValue))
            {
                WindowFocused(_falconType);
                _callbacks[e.Value.StringValue].Up();
            }
        }

        void SendAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (_callbacks.HasCallback(e.Value.StringValue))
            {
                WindowFocused(_falconType);
                _callbacks[e.Value.StringValue].Press();
            }
        }
        
        void WindowFocused(FalconTypes type)
        {
            if(type == FalconTypes.BMS && _focusAssist)
            {
                Process[] bms = Process.GetProcessesByName("Falcon BMS");
                if(bms.Length == 1)
                {
                    IntPtr hWnd = bms[0].MainWindowHandle;
                   SetForegroundWindow(hWnd);
                }
            }
        }

        private void Rtt_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // bubble up any undoable events without invalidating Rtt, to generate an
            // Undo record that will set our child's property back if called
            OnPropertyChanged(
                new PropertyNotificationEventArgs(this, "ChildProperty", e as PropertyNotificationEventArgs));

            // handle any changes we might care about
            switch (e.PropertyName)
            {
                case nameof(Interfaces.RTT.ConfigGenerator.Enabled):
                    HandleRttEnabledState();
                    break;
            }

            // no matter what changed, need to update the report
            InvalidateStatusReport();
        }

        private void PilotOptions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // bubble up any undoable events without invalidating PIlotOptions, to generate an
            // Undo record that will set our child's property back if called
            OnPropertyChanged(
                new PropertyNotificationEventArgs(this, "ChildProperty", e as PropertyNotificationEventArgs));

            // handle any changes we might care about
            switch (e.PropertyName)
            {
                case nameof(Interfaces.PilotOptions.Enabled):
                    HandlePilotOptionsEnabledState();
                    break;
            }

            // no matter what changed, need to update the report
            InvalidateStatusReport();
        }

        private void HandleRttEnabledState()
        {
            if (Rtt?.Enabled ?? false)
            {
                // just switched this on, need to start listening to viewports
                StartShadowing();
            }
            else
            {
                // don't bother tracking viewports
                StopShadowing();
            }
        }

        private void HandlePilotOptionsEnabledState()
        {
            if (PilotOptions?.Enabled ?? false)
            {
                PilotOptions.GetPilotCallsign();
            }
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public override void ReadXml(XmlReader reader)
        {
            while (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "FalconType":
                        FalconType = (FalconTypes)Enum.Parse(typeof(FalconTypes), reader.ReadElementString("FalconType"));
                        break;
                    case "KeyFile":
                        KeyFileName = reader.ReadElementString("KeyFile");
                        break;
                    case "CockpitDatFile":
                        CockpitDatFile = reader.ReadElementString("CockpitDatFile");
                        break;
                    case "FocusAssist":
                        FocusAssist = Convert.ToBoolean(reader.ReadElementString("FocusAssist"));
                        break;
                    case "ForceKeyFile":
                        {
                            PilotOptions = HeliosXmlModel.ReadXml<Interfaces.PilotOptions>(reader);
                            PilotOptions.Parent = this;
                            break;
                        }
                    case "FalconVersion":
                        FalconVersion = reader.ReadElementString("FalconVersion");
                        break;
                    case "RTT":
                        {
                            Rtt = HeliosXmlModel.ReadXml<Interfaces.RTT.ConfigGenerator>(reader);
                            Rtt.Parent = this;
                            break;
                        }
                    default:
                        // ignore unsupported settings, including structured ones
                        string elementName = reader.Name;
                        string discard = reader.ReadInnerXml();
                        Logger.Warn($"Ignored unsupported {GetType().Name} setting '{elementName}' with value '{discard}'");
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("FalconType", FalconType.ToString());
            if (!string.IsNullOrEmpty(FalconVersion))
            {
                writer.WriteElementString("FalconVersion", FalconVersion);
            }
            writer.WriteElementString("KeyFile", KeyFileName);
            //writer.WriteElementString("CockpitDatFile", CockpitDatFile);
            writer.WriteElementString("FocusAssist", FocusAssist.ToString());
            //writer.WriteElementString("ForceKeyFile", PilotOptions.ForceKeyFile.ToString());

            if(null != PilotOptions)
            {
                HeliosXmlModel.WriteXml(writer, PilotOptions);
            }

            if (null != Rtt)
            {
                HeliosXmlModel.WriteXml(writer, Rtt);
            }
        }

        #region Overrides of ViewportCompilerInterface<SampleMonitor,SampleMonitorEventArgs>

        protected override void AttachToProfileOnMainThread()
        {
            base.AttachToProfileOnMainThread();
            if (null == Rtt)
            {
                // this happens when we create the interface initially, otherwise Rtt is already deserialized from XML
                Rtt = new Interfaces.RTT.ConfigGenerator
                {
                    Parent = this
                };
            }

            // fix up the Rtt configuration, now that it is loaded
            Rtt.OnLoaded();

            // observe the Rtt object, either the one we deserialized or the one we just created
            Rtt.PropertyChanged += Rtt_PropertyChanged;
            HandleRttEnabledState();

            if(null == PilotOptions)
            {
                PilotOptions = new Interfaces.PilotOptions
                {
                    Parent = this
                };
            }

            PilotOptions.OnLoaded();
            PilotOptions.PropertyChanged += PilotOptions_PropertyChanged;
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            base.DetachFromProfileOnMainThread(oldProfile);
            if (Rtt != null)
            {
                Rtt.PropertyChanged -= Rtt_PropertyChanged;
                Rtt.Dispose();
                Rtt = null;
            }

            if(PilotOptions != null)
            {
                PilotOptions.PropertyChanged -= PilotOptions_PropertyChanged;
                PilotOptions = null;
            }
        }

        #endregion

        #region IReadyCheck

        public override IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            foreach (StatusReportItem statusReportItem in GenerateCommonStatusItems())
            {
                yield return statusReportItem;
            }

            if (Rtt != null)
            {
                foreach (StatusReportItem statusReportItem in Rtt.OnReadyCheck())
                {
                    yield return statusReportItem;
                }
            }

            if(PilotOptions != null)
            {
                foreach (StatusReportItem statusReportItem in PilotOptions.OnReadyCheck())
                {
                    yield return statusReportItem;
                }
            }
        }

        private IEnumerable<StatusReportItem> GenerateCommonStatusItems()
        {
            yield return new StatusReportItem
            {
                Status = $"Selected Falcon interface driver is '{FalconType}' version '{FalconVersion}'",
                Severity = StatusReportItem.SeverityCode.Info,
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };

            if (KeyFileName != null)
            {
                if (!File.Exists(KeyFileName))
                {
                    yield return new StatusReportItem
                    {
                        Status =
                            $"The key file configured in this profile does not exist at the path specified '{KeyFileName}'",
                        Recommendation = "Configure this interface with a valid key file",
                        Severity = StatusReportItem.SeverityCode.Warning,
                    };
                }
                else
                {
                    yield return new StatusReportItem
                    {
                        Status = $"The key file configured in this profile is '{KeyFileName}'\n",
                        Severity = StatusReportItem.SeverityCode.Info,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };
                }
            }
            else
            {
                yield return new StatusReportItem
                {
                    Status = $"Key file not defined",
                    Recommendation =
                        "Please configure this interface with a valid key file if this profile is designed to interact with Falcon",
                    Severity = StatusReportItem.SeverityCode.Warning,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }
        }

        public HeliosBindingCollection CheckBindings(HeliosBindingCollection heliosBindings)
        {
            HeliosBindingCollection missingCallbackBindings = new HeliosBindingCollection();
            foreach (HeliosBinding binding in heliosBindings)
            {
                if (binding.Value != "" && !_callbacks.HasCallback(binding.Value) && binding.ValueSource.ToString().Equals("StaticValue"))
                {
                    missingCallbackBindings.Add(binding);

                }
            }
            return missingCallbackBindings;
        }

        public IEnumerable<StatusReportItem> ReportBindings(HeliosBindingCollection bindings)
        {
            foreach (HeliosBinding binding in bindings)
            {
                yield return new StatusReportItem
                {
                    Status = $"callback bound in the profile is not found in the key file '{binding.Value}'",
                    Recommendation = $"Add missing callbacks to your key file.",
                    Severity = StatusReportItem.SeverityCode.Error,
                    Flags = StatusReportItem.StatusFlags.DoNotDisturb | StatusReportItem.StatusFlags.Verbose
                };
            }
        }
        #endregion

        protected override Interfaces.RTT.ShadowMonitor CreateShadowMonitor(Monitor monitor) => new Interfaces.RTT.ShadowMonitor(this, monitor, monitor, false);

        protected override void UpdateAllGeometry()
        {
            Logger.Debug("RTT geometry update due to possible change in viewports");
            Rtt?.Update(Viewports);
        }

        protected override List<StatusReportItem> CreateStatusReport()
        {
            List<StatusReportItem> newReport = new List<StatusReportItem> (GenerateCommonStatusItems());

            if (_callbacks.IsParsed)
            {
                newReport.AddRange(ReportBindings(CheckBindings(InputBindings)));
            }

            if (null != Rtt)
            {
                // write the RTT configuration status report
                newReport.AddRange(Rtt.OnStatusReport(Viewports));
            }

            if(null != PilotOptions)
            {
                // write the PilotOptions status report
                newReport.AddRange(PilotOptions.OnStatusReport());
            }

            return newReport;
        }

        public override InstallationResult Install(IInstallationCallbacks callbacks)
        {
            // there is currently no interactive creation of config, as the FalconInterface
            // just configures settings and profile options
            return InstallationResult.Success;
        }

        #region IExtendedDescription
        public override string Description => $"Interface to {FalconType}";
        public override string RemovalNarrative => $"Delete this interface and remove all of its bindings from the Profile";
        #endregion

    }
}
