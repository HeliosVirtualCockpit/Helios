﻿//  Copyright 2014 Craig Courtney
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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using Microsoft.Win32;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon
{
    [HeliosInterface("Helios.Falcon.Interface", "Falcon", typeof(FalconIntefaceEditor), typeof(UniqueHeliosInterfaceFactory))]
    public class FalconInterface : HeliosInterface, IReadyCheck, IStatusReportNotify, IExtendedDescription
    {
        private FalconTypes _falconType;
        private string _falconPath;
        private string _keyFile;
        private string _cockpitDatFile;
        private bool _focusAssist;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private FalconDataExporter _dataExporter;

        private FalconKeyFile _callbacks = new FalconKeyFile("");
        private readonly HashSet<IStatusReportObserver> _observers = new HashSet<IStatusReportObserver>();

        public FalconInterface()
            : base("Falcon")
        {
            FalconType = FalconTypes.BMS;
            _dataExporter = new BMS.BMSFalconDataExporter(this);
            KeyFileName = System.IO.Path.Combine(FalconPath, "User\\Config\\BMS - Full.key");

            HeliosAction sendAction = new HeliosAction(this, "", "callback", "send", "Press and releases a keyboard callback for falcon.", "Callback name", BindingValueUnits.Text)
            {
                ActionBindingDescription = "send %value% callback for falcon.",
                ActionInputBindingDescription = "send %value% callback",
                ValueEditorType = typeof(FalconCallbackValueEditor)
            };
            sendAction.Execute += new HeliosActionHandler(SendAction_Execute);
            Actions.Add(sendAction);

            HeliosAction pressAction = new HeliosAction(this, "", "callback", "press", "Press a keyboard callback for falcon and leave it pressed.", "Callback name", BindingValueUnits.Text)
            {
                ActionBindingDescription = "press %value% callback for falcon.",
                ActionInputBindingDescription = "press %value% callback",
                ValueEditorType = typeof(FalconCallbackValueEditor)
            };
            pressAction.Execute += new HeliosActionHandler(PressAction_Execute);
            Actions.Add(pressAction);

            HeliosAction releaseAction = new HeliosAction(this, "", "callback", "release", "Releases a previously pressed keyboard callback for falcon.", "Callback name", BindingValueUnits.Text)
            {
                ActionBindingDescription = "release %value% callback for falcon.",
                ActionInputBindingDescription = "release %value% callback",
                ValueEditorType = typeof(FalconCallbackValueEditor)
            };
            releaseAction.Execute += new HeliosActionHandler(ReleaseAction_Execute);
            Actions.Add(releaseAction);
        }

        #region Properties

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
                            _dataExporter = new BMS.BMSFalconDataExporter(this);
                            KeyFileName = System.IO.Path.Combine(FalconPath, "User\\Config\\BMS - Full.key");
                            break;
                        case FalconTypes.OpenFalcon:
                            _dataExporter = new OpenFalcon.OpenFalconDataExporter(this);
                            KeyFileName = System.IO.Path.Combine(FalconPath, "config\\OFKeystrokes.key");
                            break;
                        case FalconTypes.AlliedForces:
                        default:
                            _dataExporter = new AlliedForces.AlliedForcesDataExporter(this);
                            KeyFileName = System.IO.Path.Combine(FalconPath, "config\\keystrokes.key");
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
                return _keyFile;
            }
            set
            {
                if ((_keyFile == null && value != null)
                    || (_keyFile != null && !_keyFile.Equals(value)))
                {
                    string oldValue = _keyFile;
                    FalconKeyFile oldKeyFile = _callbacks;
                    _keyFile = value;
                    _callbacks = new FalconKeyFile(_keyFile);
                    OnPropertyChanged("KeyFileName", oldValue, value, true);
                    OnPropertyChanged("KeyFile", oldKeyFile, _callbacks, false);
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
                if (_falconPath == null)
                {
                    RegistryKey pathKey = null;
                    switch (FalconType)
                    {
                        case FalconTypes.BMS:
                            pathKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Benchmark Sims\Falcon BMS 4.34");
                            break;

                        case FalconTypes.OpenFalcon:
                            pathKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MicroProse\Falcon\4.0");
                            break;

                        case FalconTypes.AlliedForces:
                            pathKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Lead Pursuit\Battlefield Operations\Falcon");
                            break;
                    }
                    
                    if (pathKey != null)
                    {
                        _falconPath = (string)pathKey.GetValue("baseDir");
                    }
                    else
                    {
                        _falconPath = "";
                    }
                }
                return _falconPath;
            }
        }

        internal RadarContact[] RadarContacts => _dataExporter?.RadarContacts;

        #endregion

        public BindingValue GetValue(string device, string name)
        {
            return _dataExporter?.GetValue(device, name) ?? BindingValue.Empty;
        }

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);

            if (oldProfile != null)
            {
                oldProfile.ProfileStarted -= new EventHandler(Profile_ProfileStarted);
                oldProfile.ProfileTick -= new EventHandler(Profile_ProfileTick);
                oldProfile.ProfileStopped -= new EventHandler(Profile_ProfileStopped);
            }

            if (Profile != null)
            {
                Profile.ProfileStarted += new EventHandler(Profile_ProfileStarted);
                Profile.ProfileTick += new EventHandler(Profile_ProfileTick);
                Profile.ProfileStopped += new EventHandler(Profile_ProfileStopped);
            }
        }

        void Profile_ProfileStopped(object sender, EventArgs e)
        {
            _dataExporter?.CloseData();
        }

        void Profile_ProfileTick(object sender, EventArgs e)
        {
            _dataExporter?.PollData();
        }

        void Profile_ProfileStarted(object sender, EventArgs e)
        {
            _dataExporter?.InitData();
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
                    default:
                        // ignore unsupported settings
                        string elementName = reader.Name;
                        string discard = reader.ReadElementString(reader.Name);
                        Logger.Warn($"Ignored unsupported {GetType().Name} setting '{elementName}' with value '{discard}'");
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("FalconType", FalconType.ToString());
            writer.WriteElementString("KeyFile", KeyFileName);
            writer.WriteElementString("CockpitDatFile", CockpitDatFile);
            writer.WriteElementString("FocusAssist", FocusAssist.ToString());
        }

        #region IReadyCheck
        public IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            // XXX perform integrity check.  any warnings will light caution on Control Center
            // XXX any error will block control center from starting profile unless user selects to disable ready check
            yield return new StatusReportItem
            {
                Status = $"Selected Falcon interface driver is {FalconType}",
                Severity = StatusReportItem.SeverityCode.Info,
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
            };
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

        public void PublishStatusReport(IList<StatusReportItem> statusReport)
        {
            foreach (IStatusReportObserver observer in _observers)
            {
                observer.ReceiveStatusReport(Name, Description, statusReport);
            }
        }

        public void InvalidateStatusReport()
        {
            List<StatusReportItem> newReport = new List<StatusReportItem>();
            newReport.AddRange(PerformReadyCheck());
            PublishStatusReport(newReport);
        }
        #endregion
        
        #region IExtendedDescription
        public string Description => $"Interface to {FalconType}";
        public string RemovalNarrative => $"Delete this interface and remove all of its bindings from the Profile";
        #endregion
    }
}
