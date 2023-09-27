//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Capabilities.ProfileAwareInterface;
using GadrocsWorkshop.Helios.UDPInterface;
using GadrocsWorkshop.Helios.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Path = System.IO.Path;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class DCSInterface : BaseUDPInterface, IProfileAwareInterface, IReadyCheck, IStatusReportNotify
    {
        private const string SETTINGS_GROUP = "DCSInterface";
        private const DCSExportModuleFormat DEFAULT_EXPORT_MODULE_FORMAT = DCSExportModuleFormat.HeliosDriver16;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// backing field for ExportModuleFormat
        /// </summary>
        private DCSExportModuleFormat _exportModuleFormat = DEFAULT_EXPORT_MODULE_FORMAT;

        // current state of the Export script, as far as we know, or null if we don't understand it at all
        protected DCSExportModuleFormat? _remoteModuleFormat;

        // phantom monitor fix 
        private DCSPhantomMonitorFix _phantomFix;

        // export configuration generation and checking
        protected DCSExportConfiguration _configuration;

        // vehicle impersonation configuration
        protected DCSVehicleImpersonation _vehicleImpersonation;

        // protocol to talk to DCS Export script (control messages)
        protected DCSExportProtocol _protocol;

        // observers of our status report that need to be told when we change status
        private readonly HashSet<IStatusReportObserver> _observers = new HashSet<IStatusReportObserver>();

        /// <summary>
        /// backing field for ImpersonatedVehicleName
        /// </summary>
        private string _impersonatedVehicleName;

        /// <summary>
        /// backing field for property ExportModuleText, contains
        /// the full text of an export module to be attached to the interface in the profile
        /// </summary>
        private string _exportModuleText;

        /// <summary>
        /// backing field for property ExportModuleBaseName, contains
        /// file base name (no path, no extension) for the ExportModuleText, because naming
        /// can differ across module formats
        /// </summary>
        private string _exportModuleBaseName;

        // XXX temporary adaptatation, as we want to re-write the ModuleName and Vehicles support
        public string ModuleName => VehicleName;
        public IList<string> Vehicles => Tags.ToList();

        /// <summary>
        /// these are allowable export device names in addition to real known vehicles
        /// </summary>
        private readonly string[] _specialExportDeviceNames = { null, "FC2", "DCSGeneric" };

        protected DCSInterface(string name, string exportDeviceName, string exportFunctionsPath)
            : base(name)
        {
            VehicleName = exportDeviceName;
            ExportFunctionsPath = exportFunctionsPath;

            // make sure we keep our list up to date and don't typo on the name of an export device
#if false
// NOTE: can't use this right now because interface files reference many unknown aircraft
            Debug.Assert(null == exportDeviceName || 
                         _specialExportDeviceNames.Contains(exportDeviceName) || 
                         DCSVehicleImpersonation.KnownVehicles.Contains(exportDeviceName));
#endif

            // create handling for DCS export meta information we handle ourselves
            NetworkTriggerValue activeVehicle = new NetworkTriggerValue(this, "ACTIVE_VEHICLE", "ActiveVehicle",
                "Vehicle currently inhabited in DCS.", "Short name of vehicle");
            AddFunction(activeVehicle);
            activeVehicle.ValueReceived += ActiveVehicle_ValueReceived;
            NetworkTriggerValue activeDriver = new NetworkTriggerValue(this, "ACTIVE_DRIVER", "ActiveDriver",
                "Export driver running on DCS.", "Short name of driver type");
            AddFunction(activeDriver);
            activeDriver.ValueReceived += ActiveDriver_ValueReceived;
            AddFunction(new NetworkTrigger(this, "ALIVE", "Heartbeat",
                "Received periodically if there is no other data received"));
            NetworkTriggerValue alertMessage = new NetworkTriggerValue(this, "ALERT_MESSAGE", "AlertMessage",
                "Export driver running on DCS.", "Most recent alert message");
            AddFunction(alertMessage);
            alertMessage.ValueReceived += AlertMessage_ValueReceived;

            // handle basic telemetry data included in all drivers now
            AddFunction(new NetworkValue(this, "T1", "Simulator Telemetry", "pitch", "Current pitch of the aircraft.", "(-180 to 180)", BindingValueUnits.Degrees, null));
            AddFunction(new NetworkValue(this, "T2", "Simulator Telemetry", "bank", "Current bank of the aircraft.", "(-180 to 180)", BindingValueUnits.Degrees, null));
            AddFunction(new NetworkValue(this, "T3", "Simulator Telemetry", "yaw", "Current yaw of the aircraft.", "(-180 to 180)", BindingValueUnits.Degrees, null));
            AddFunction(new NetworkValue(this, "T4", "Simulator Telemetry", "barometric altitude", "Current barometric altitude the aircraft.", "", BindingValueUnits.Meters, null));
            AddFunction(new NetworkValue(this, "T5", "Simulator Telemetry", "radar altitude", "Current radar altitude of the aircraft.", "", BindingValueUnits.Meters, null));
            AddFunction(new NetworkValue(this, "T13", "Simulator Telemetry", "vertical velocity", "Current vertical velocity of the aircraft.", "", BindingValueUnits.MetersPerSecond, null));
            AddFunction(new NetworkValue(this, "T14", "Simulator Telemetry", "indicated airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.MetersPerSecond, null));
            AddFunction(new NetworkValue(this, "T16", "Simulator Telemetry", "angle of attack", "Current angle of attack for the aircraft.", "", BindingValueUnits.Degrees, null));
            AddFunction(new NetworkValue(this, "T17", "Simulator Telemetry", "glide deviation", "ILS Glide Deviation", "-1 to 1", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "T18", "Simulator Telemetry", "side deviation", "ILS Side Deviation", "-1 to 1", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "T19", "Simulator Telemetry", "Mach", "Current Mach number", "number in M", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "T20", "Simulator Telemetry", "G", "Current G load", "number in g", BindingValueUnits.Numeric, null));
        }

#region Events

#endregion

#region Properties

        /// <summary>
        /// The vehicle (usually an aircraft) that DCS will report in LoGetSelfData when we are using this interface.
        /// </summary>
        public string VehicleName { get; protected set; }

        /// <summary>
        /// If not null, the this interface instance is configured to impersonate the specified vehicle name.  This means
        /// that Helios should select it for the given vehicle, instead of the one that the interface natively supports.
        /// </summary>
        public string ImpersonatedVehicleName
        {
            get => _impersonatedVehicleName;
            set
            {
                if (_impersonatedVehicleName == value)
                {
                    return;
                }

                string oldValue = _impersonatedVehicleName;
                _impersonatedVehicleName = value;
                OnPropertyChanged(nameof(ImpersonatedVehicleName), oldValue, value, true);
                InvalidateStatusReport();
            }
        }

        /// <summary>
        /// vehicle-specific file resource to include
        /// </summary>
        public string ExportFunctionsPath { get; protected set; }

        public DCSExportConfiguration Configuration => _configuration;

        public DCSVehicleImpersonation VehicleImpersonation => _vehicleImpersonation;

        public virtual string StatusName =>
            ImpersonatedVehicleName != null ? $"{Name} impersonating {ImpersonatedVehicleName}" : Name;

        /// <summary>
        /// the base name to use when writing an embedded module file during setup
        /// </summary>
        public virtual string WrittenModuleBaseName =>
            ExportModuleBaseName ??
            ImpersonatedVehicleName ??
            VehicleName;

        /// <summary>
        /// the configured format for the export module used by this interface
        /// </summary>
        public DCSExportModuleFormat ExportModuleFormat
        {
            get => _exportModuleFormat;
            set
            {
                if (_exportModuleFormat == value)
                {
                    return;
                }

                // batch change of format with erase of module text and name
                using (new HeliosUndoBatch())
                {
                    DCSExportModuleFormat oldValue = _exportModuleFormat;
                    _exportModuleFormat = value;
                    RemoveEmbeddedModule();
                    OnPropertyChanged("ExportModuleFormat", oldValue, value, true);
                }
            }
        }

        public bool CanConfigureExportModuleFormat { get; protected set; } = true;

        /// <summary>
        /// the full text of an export module to be attached to the interface in the profile
        /// </summary>
        public string ExportModuleText
        {
            get => _exportModuleText;
            set
            {
                if (value != null)
                {
                    // normalize carriage returns so we can compare it later
                    // NOTE: XMLReader drops all the carriage returns in cdata
                    value = Regex.Replace(value, "\r\n|\n\r|\n|\r", "\r\n");
                }

                if (_exportModuleText == value)
                {
                    return;
                }

                string oldValue = _exportModuleText;
                _exportModuleText = value;
                OnPropertyChanged("ExportModuleText", oldValue, value, true);
            }
        }

        /// <summary>
        /// file base name (no path, no extension) for the ExportModuleText, because naming
        /// can differ across module formats
        /// </summary>
        public string ExportModuleBaseName
        {
            get => _exportModuleBaseName;
            set
            {
                if (_exportModuleBaseName != null && _exportModuleBaseName == value)
                {
                    return;
                }

                string oldValue = _exportModuleBaseName;
                _exportModuleBaseName = value;
                OnPropertyChanged("ExportModuleBaseName", oldValue, value, true);
            }
        }

#endregion

        internal string LoadSetting(string key, string defaultValue)
        {
            if (ConfigManager.SettingsManager.IsSettingAvailable(SETTINGS_GROUP, key))
            {
                // get from shared location
                return ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, key, defaultValue);
            }

            // get from legacy location
            return ConfigManager.SettingsManager.LoadSetting(Name, key, defaultValue);
        }

        internal T LoadSetting<T>(string key, T defaultValue)
        {
            if (ConfigManager.SettingsManager.IsSettingAvailable(SETTINGS_GROUP, key))
            {
                // get from shared location, using LoadSetting<T>
                return ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, key, defaultValue);
            }

            // get from legacy location, using LoadSetting<T>
            return ConfigManager.SettingsManager.LoadSetting(Name, key, defaultValue);
        }

        internal void SaveSetting(string key, string value)
        {
            ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, key, value);
        }

        internal void SaveSetting<T>(string key, T value)
        {
            ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, key, value);
        }

        internal bool HasSetting(string key)
        {
            return ConfigManager.SettingsManager.IsSettingAvailable(SETTINGS_GROUP, key) || ConfigManager.SettingsManager.IsSettingAvailable(Name, key);
        }

        protected override void AttachToProfileOnMainThread()
        {
            base.AttachToProfileOnMainThread();
            Profile.ProfileTick += Profile_Tick;
            _configuration = new DCSExportConfiguration(this);
            CustomizeGenerator();
            _configuration.Initialize();
            _vehicleImpersonation = new DCSVehicleImpersonation(this);
        }

        protected virtual void CustomizeGenerator()
        {
            // no code in base
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            base.DetachFromProfileOnMainThread(oldProfile);
            oldProfile.ProfileTick -= Profile_Tick;
            _configuration?.Dispose();
            _configuration = null;
            _vehicleImpersonation?.Dispose();
            _vehicleImpersonation = null;
        }

        private void Profile_Tick(object sender, EventArgs e)
        {
            _phantomFix?.Profile_Tick(sender, e);
        }

        private void ActiveDriver_ValueReceived(object sender, NetworkTriggerValue.Value e)
        {
            if (Enum.TryParse(e.Text, true, out DCSExportModuleFormat remoteModuleFormat))
            {
                _remoteModuleFormat = remoteModuleFormat;
            }
            else
            {
                if (new Regex("[\\d\\w_-]+").IsMatch(e.Text))
                {
                    Logger.Error("DCS interface received advertisement for unsupported export module format '{e.Text}' from export script");
                }
                _remoteModuleFormat = null;
            }

            _protocol?.OnDriverStatus(_remoteModuleFormat);
            DriverStatusReceived?.Invoke(this, new DriverStatus {DriverType = e.Text});
        }

        private void ActiveVehicle_ValueReceived(object sender, NetworkTriggerValue.Value e)
        {
            ProfileHintReceived?.Invoke(this, new ProfileHint {Tag = e.Text});
        }

        private void AlertMessage_ValueReceived(object sender, NetworkTriggerValue.Value e)
        {
            try
            {
                // payload is Base64 with "-" used as padding instead of "=" to avoid Helios protocol
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(e.Text.Replace('-', '=')));
                Logger.Error("Error received from Export.lua: {AlertMessage}", decoded);
            }
            catch (FormatException ex)
            {
                Logger.Error(ex, "received ALERT message from Export.lua that contained invalid Base64 contents: {Raw}", e.Text);
            }
        }

        public override void Reset()
        {
            base.Reset();
            _protocol?.Reset();
        }

        protected override void OnProfileStarted()
        {
            // these parts are only used at run time (i.e. not in the Profile Editor)
            _protocol = new DCSExportProtocol(this);
            _phantomFix = new DCSPhantomMonitorFix(this);
        }

        protected override void OnProfileStopped()
        {
            _protocol?.Stop();
            _protocol = null;
        }

        protected override void OnClientChanged(string fromValue, string toValue)
        {
            base.OnClientChanged(fromValue, toValue);

            // protocol needs to know
            _protocol.OnClientChanged();

            // our information is now out of date
            _remoteModuleFormat = null;
        }

        private static readonly OnceLogger UnknownIdLogger = new OnceLogger(Logger);

        protected override void OnUnrecognizedFunction(string id, string value)
        {
            // don't log this as warning because our protocol includes times when we 
            // receive data from the wrong module and it is correct behavior
            if (IsSynchronized)
            {
                // we think we are synchronized
                // only log this once per ID so we don't flood
                UnknownIdLogger.InfoOnce(id, $"DCS interface received data for missing function (Key=\"{id}\"); we think export is running module of type '{_remoteModuleFormat}'");
            }
        }

        /// <summary>
        /// true if the export script is running the module we requested, as far as we know
        /// </summary>
        private bool IsSynchronized => _remoteModuleFormat.HasValue && _remoteModuleFormat == _exportModuleFormat;

        // WARNING: executed on LoadProfile thread
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            while (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "ExportModuleFormat":
                        TypeConverter enumConverter = TypeDescriptor.GetConverter(typeof(DCSExportModuleFormat));
                        ExportModuleFormat =
                            (DCSExportModuleFormat) enumConverter.ConvertFromInvariantString(
                                reader.ReadElementString("ExportModuleFormat"));
                        break;
                    case "ExportModuleBaseName":
                        ExportModuleBaseName = reader.ReadElementString("ExportModuleBaseName");
                        break;
                    case "ImpersonatedVehicleName":
                        ImpersonatedVehicleName = reader.ReadElementString("ImpersonatedVehicleName");
                        break;
                    case "ExportModuleText":
                        ExportModuleText = NormalizeLineEndings(reader.ReadElementContentAsString());
                        break;
                    case "ExportModuleTextBase64":
                        string encoded = reader.ReadElementContentAsString();
                        ExportModuleText = NormalizeLineEndings(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)));
                        break;
                    default:
                        string elementName = reader.Name;
                        string discard = reader.ReadInnerXml();
                        Logger.Warn(
                            $"Ignored unsupported DCS Interface setting '{elementName}' with value '{discard}'");
                        break;
                }
            }
        }

        /// <summary>
        /// converts all line endings to windows line endings, so that file deserialized from XML will match original file
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static string NormalizeLineEndings(string text)
        {
            return LineEndings.Replace(text, Environment.NewLine);
        }

        // https://stackoverflow.com/a/141069/955263
        private static readonly Regex LineEndings = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            TypeConverter enumConverter = TypeDescriptor.GetConverter(typeof(DCSExportModuleFormat));

            // we could have configuration data that is not used and should not be persisted, so clean up if we can
            CleanBeforeWriting();

            if (ExportModuleFormat != DEFAULT_EXPORT_MODULE_FORMAT)
            {
                // write new Xml only if configured, because it may break previous versions
                writer.WriteElementString("ExportModuleFormat",
                    enumConverter.ConvertToInvariantString(ExportModuleFormat));
            }

            if (ExportModuleBaseName != null)
            {
                writer.WriteElementString("ExportModuleBaseName", ExportModuleBaseName);
            }

            if (!string.IsNullOrEmpty(ExportModuleText))
            {
                if (ExportModuleText.Contains("]]>"))
                {
                    string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(ExportModuleText),
                        Base64FormattingOptions.InsertLineBreaks);
                    writer.WriteStartElement("ExportModuleTextBase64");
                    writer.WriteCData(encoded);
                    writer.WriteEndElement();
                }
                else
                {
                    // prefer readable format
                    writer.WriteStartElement("ExportModuleText");
                    writer.WriteCData(ExportModuleText);
                    writer.WriteEndElement();
                }
            }

            if (ImpersonatedVehicleName != null)
            {
                writer.WriteElementString("ImpersonatedVehicleName", ImpersonatedVehicleName);
            }
        }

        private void CleanBeforeWriting()
        {
            if (Configuration != null)
            {
                DCSExportConfiguration.ModuleFormatInfo moduleInfo = Configuration.ExportModuleFormatInfo[ExportModuleFormat];
                if (!moduleInfo.CanBeAttached)
                {
                    RemoveEmbeddedModule();
                }
            }
        }

        public bool HasEmbeddedModule => !string.IsNullOrEmpty(ExportModuleBaseName);

        internal virtual void RemoveEmbeddedModule()
        {
            ExportModuleBaseName = null;
            ExportModuleText = null;
        }

        internal virtual void SetEmbeddedModule(string path, string moduleText)
        {
            ExportModuleText = moduleText;
            ExportModuleBaseName = Path.GetFileNameWithoutExtension(path);
        }

#region IReadyCheck

        public virtual IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            // XXX check on our health

            // check on the health of our exports
            foreach (StatusReportItem item in _configuration.PerformReadyCheck())
            {
                yield return item;
            }
        }

#endregion

#region IProfileAwareInterface

        // NOTE: ClientChanged event is in base class

        // this event indicates that the interface received an indication that a profile that 
        // matches the specified hint should be loaded
        public event EventHandler<ProfileHint> ProfileHintReceived;

        // this event indicates that the interface received an indication that the specified
        // exports are loaded on the other side of the interface
        public event EventHandler<DriverStatus> DriverStatusReceived;

        // we only support selection based on which vehicle this interface supports
        public IEnumerable<string> Tags => new[] {ImpersonatedVehicleName ?? VehicleName};

        public void RequestDriver(string profileShortName)
        {
            // we don't have per-profile drivers
            _ = profileShortName;

            // the interface is supposed to have called OnProfileStarted before this is called,
            // so don't check for null; we want this to crash if this breaks in the future
            if (_remoteModuleFormat == _exportModuleFormat)
            {
                // already in correct state
                // but we let our UI know
                DriverStatusReceived?.Invoke(this, new DriverStatus {DriverType = _remoteModuleFormat.ToString()});
                return;
            }

            _protocol.SendDriverRequest(_exportModuleFormat);
        }

#endregion

#region IStatusReportNotify

        public void Subscribe(IStatusReportObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IStatusReportObserver observer)
        {
            _observers.Remove(observer);
        }

        public virtual void InvalidateStatusReport()
        {
            if (_observers.Count < 1)
            {
                return;
            }

            IList<StatusReportItem> newReport = _configuration.CheckConfig();
            PublishStatusReport(newReport);
        }

        public void PublishStatusReport(IList<StatusReportItem> statusReport)
        {
            string statusName = StatusName;
            foreach (IStatusReportObserver observer in _observers)
            {
                observer.ReceiveStatusReport(statusName,
                    "Interface listening to UDP updates from DCS export.lua and responding with commands.",
                    statusReport);
            }
        }

#endregion
    }
}