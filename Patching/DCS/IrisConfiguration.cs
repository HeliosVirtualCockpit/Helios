// Copyright 2023 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Util.DCS;
using System.Xml;
using System.ComponentModel;
using System.Windows.Media;
using GadrocsWorkshop.Helios.Controls.Special;
using RectpackSharp;
using System.Diagnostics.Contracts;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    internal class IrisConfiguration : DCSConfiguration
    {

        #region Private

        private readonly MonitorSetup _parent;

        private bool _isOpen = false;
        private bool _isProfileOpen = false;
        private bool _isRemote = false;
        private string _configName;
        private string _profileName;
        private string _irisPath;
        private string _irisProfilePath;
        private string _tempPath;
        private string _backupPath;
        private string _tempProfilePath;
        private string _backupProfilePath;
        private string _remoteHost;
        private double _portNumber = 9091;
        private string _vehicleName = "";

        private bool _hasBackground = true;
        private Rect _backgroundRect = new Rect(0,0,1920,1080);
        private TextWriter _writer;
        private XmlWriterSettings _settings;
        private XmlWriter _xmlWriter;
        private TextWriter _profileWriter;
        private XmlWriter _xmlProfileWriter;
        private HeliosProfile _localProfile;
        private PackingRectangle[] _packingRectangles = new PackingRectangle[20];
        private Dictionary<int,string> _rectangleID = new Dictionary<int,string>();
        private int _rectangleIdNumber = 0;

        private static readonly Color[] _colors =
        {
            Color.FromArgb(0x80,0xDB,0x29,0x29),
            Color.FromArgb(0x80,0xD9,0x27,0x62),
            Color.FromArgb(0x80,0x95,0x45,0xD8),
            Color.FromArgb(0x80,0x4C,0x4C,0xE0),
            Color.FromArgb(0x80,0x2F,0x9F,0xE0),
            Color.FromArgb(0x80,0x20,0xBA,0xA3),
            Color.FromArgb(0x80,0x1F,0xC0,0x6F),
            Color.FromArgb(0x80,0x81,0xD4,0x2F),
            Color.FromArgb(0x80,0xE3,0xA3,0x22),
            Color.FromArgb(0x80,0xE0,0x5B,0x2B),
            Color.FromArgb(0x80,0x59,0x82,0x96),
            Color.FromArgb(0x80,0x59,0x63,0x87)
        };

        private string _networkAddress = "";
        #endregion
        /// <summary>
        /// The Iris configuration requires information from both the sending and receiving profiles.  The receiving side
        /// ultimately determines the size of the viewport, so the receiving side is the one creating the Iris configuration
        /// from the ViewportSetupFiles.  In order to facilitate the setup on the sending side, a viewport-only profile is 
        /// created to help the viewport layout on the local system.
        /// TODO: There should be a method to merge the sending and receiving data into a single Iris Config.
        /// </summary>
        /// <param name="parent"></param>
        public IrisConfiguration(MonitorSetup parent)
        {
            _parent = parent;
            SubscribeToLocationChanges();
            // _isRemote = ConfigManager.SettingsManager.LoadSetting("DCSInstallation", "IsRemote", false);
            _isRemote = (_parent.IrisConfigurationType == IrisConfigurationType.IrisClientConfig);
            _networkAddress = ConfigManager.SettingsManager.LoadSetting("DCSInterface", "IPAddress", System.Net.IPAddress.Loopback.ToString());
            _portNumber = ConfigManager.SettingsManager.LoadSetting("DCSInterface", "Port", 9089+5);
        }
        /// <summary>
        /// Open the Iris xml file for writing the Iris configuration into 
        /// </summary>
        /// <param name="configName">The name of the profile used for the file naming</param>
        /// <returns>success / failure</returns>
        public bool Open(string configName)
        {
            try
            {
                _configName = Path.ChangeExtension(configName, "iris");
                _configName = $"{Path.GetFileNameWithoutExtension(_configName)}_{(_isRemote ? "Client" : "Server")}.iris";
                _irisPath = Path.Combine(ConfigManager.DocumentPath, "Iris_Partial_Configs");
                _tempPath = Path.Combine(_irisPath, Path.ChangeExtension(_configName, "tmp"));
                _backupPath = Path.Combine(_irisPath, Path.ChangeExtension(_configName, "bak"));
                _remoteHost = _networkAddress;


                if (!Directory.Exists(_irisPath))
                {
                    Directory.CreateDirectory(_irisPath);
                }
                // Delete tmp file if exists
                if (File.Exists(_tempPath))
                {
                    File.Delete(_tempPath);
                }
                _writer = new StreamWriter(_tempPath, false);
                TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

                _settings = new XmlWriterSettings();
                _settings.Indent = true;

                _xmlWriter = XmlWriter.Create(_writer, _settings);
                _xmlWriter.WriteStartElement("IrisConfig");
                _xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                _xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                _xmlWriter.WriteStartElement("ViewPorts");
                _isOpen = true;
            }
            catch (Exception ex)
            {
                string message = $"Iris Configuration failure:  '{ex.Message}'";
                ConfigManager.LogManager.LogError(message);
                _isOpen = false;
            }
            if (_isRemote)
            {
                OpenServerProfile(_parent.CurrentProfileName);
            }
            return _isOpen;
        }
        /// <summary>
        /// Open a new Helios profile to contain a packed set of viewports needed on the
        /// local system to configure the Monitor Setup Lua.
        /// </summary>
        /// <param name="configName"></param>
        private void OpenServerProfile(string configName)
        {
            _profileName =  $"{((!_isRemote) ? configName : VehicleName)}_Server_Viewports.hpf";
            _irisProfilePath = Path.Combine(ConfigManager.DocumentPath, "Profiles");
            _tempProfilePath = Path.Combine(_irisProfilePath, Path.ChangeExtension(_profileName, "tmp"));
            _backupProfilePath = Path.Combine(_irisProfilePath, Path.ChangeExtension(_profileName, "bak"));

            _localProfile = new HeliosProfile();
            _localProfile.Name = Path.GetFileNameWithoutExtension(_profileName);

            _isProfileOpen = true;
        }
        /// <summary>
        /// Writes a single viewport into the Iris config file
        /// </summary>
        /// <remarks>The Iris coordinates use Windows coordinates where the primary display origin is 0,0
        /// and this is different from DCS where the leftmost monitor is 0,0</remarks>
        /// <param name="viewport">Name and the viewport rectangle as a KeyValuePair</param>
        /// <returns>Success/Failure</returns>
        public bool WriteViewport(KeyValuePair<string, Rect> viewport)
        {
            if (_isOpen)
            {
                Rect viewportRect = viewport.Value;
                viewportRect.Intersect(_parent.Rendered);
                if (viewportRect.Width < viewport.Value.Width || viewportRect.Height < viewport.Value.Height)
                {
                    // viewports that aren't entirely rendered do not work
                    string message = $"viewport '{viewport.Key}' not included in IRIS configuration because it is not entirely contained in rendered resolution";
                    ConfigManager.LogManager.LogInfo(message);
                    return false;
                }
                else
                {
                    _xmlWriter.WriteStartElement("ViewPort");
                    _xmlWriter.WriteElementString("Name", viewport.Key);
                    _xmlWriter.WriteElementString("Description", $"Viewport for {viewport.Key}");
                    _xmlWriter.WriteElementString("Host", _remoteHost);
                    _xmlWriter.WriteElementString("Port", $"{_portNumber++}");
                    _xmlWriter.WriteElementString("ScreenCaptureX", $"{(!_isRemote ? viewportRect.Left : 0)}");
                    _xmlWriter.WriteElementString("ScreenCaptureY", $"{(!_isRemote ? viewportRect.Top : 0)}");
                    _xmlWriter.WriteElementString("SizeX", viewportRect.Width.ToString());
                    _xmlWriter.WriteElementString("SizeY", viewportRect.Height.ToString());
                    _xmlWriter.WriteElementString("ScreenPositionX", $"{(_isRemote ? viewportRect.Left : 0)}");
                    _xmlWriter.WriteElementString("ScreenPositionY", $"{(_isRemote ? viewportRect.Top : 0)}");
                    _xmlWriter.WriteEndElement();  // ViewPort

                    if (_isRemote) CreateViewportVisual(viewport);

                    return true;
                }
            }else
            {
                return false;
            }
        }
        /// <summary>
        /// Creates a new Viewport control for inclusion in the Local profile.
        /// </summary>
        private void CreateViewportVisual(KeyValuePair<string, Rect> viewport)
        {
            if(_rectangleIdNumber < _packingRectangles.Count())
            {
                Rect viewportRect = viewport.Value;
                _rectangleID.Add(_rectangleIdNumber, viewport.Key);
                _packingRectangles[_rectangleIdNumber] = new PackingRectangle(0, 0, Convert.ToUInt32(viewportRect.Width), Convert.ToUInt32(viewportRect.Height), _rectangleIdNumber++);
            }
            else
            {
                string message = $"Maximum number of viewports for server profile has been exceeded: '{_packingRectangles.Count()}'.";
                ConfigManager.LogManager.LogError(message);
                return;
            }
        }
        /// <summary>
        /// Closes the Iris configuration xml file.
        /// </summary>
        public void Close()
        {
            if (_isOpen)
            {
                if (_isRemote) WriteServerProfile();
                if (_hasBackground)
                {
                    _xmlWriter.WriteStartElement("Viewport");
                    _xmlWriter.WriteElementString("Name", "Background");
                    _xmlWriter.WriteElementString("Description", "This is the full screen background for the back of the screen");
                    _xmlWriter.WriteElementString("Host", _remoteHost);
                    _xmlWriter.WriteElementString("Port", $"{_portNumber++}");
                    _xmlWriter.WriteElementString("ScreenCaptureX", "0");
                    _xmlWriter.WriteElementString("ScreenCaptureY", "0");
                    _xmlWriter.WriteElementString("SizeX", _backgroundRect.Width.ToString());
                    _xmlWriter.WriteElementString("SizeY", _backgroundRect.Height.ToString());
                    _xmlWriter.WriteElementString("ScreenPositionX", _backgroundRect.X.ToString());
                    _xmlWriter.WriteElementString("ScreenPositionY", _backgroundRect.Y.ToString());
                    _xmlWriter.WriteEndElement();  // background viewport
                }
                _xmlWriter.WriteEndElement();  // Viewports
                _xmlWriter.WriteElementString("PollingInterval", "100");

                _xmlWriter.WriteStartElement("GlobalImageAdjustment");
                _xmlWriter.WriteElementString("Brightness", "1.0");
                _xmlWriter.WriteElementString("RedBrightness", "1.0");
                _xmlWriter.WriteElementString("GreenBrightness", "1.0");
                _xmlWriter.WriteElementString("BlueBrightness", "1.0");
                _xmlWriter.WriteElementString("AlphaBrightness", "1.0");
                _xmlWriter.WriteElementString("Gamma", "1.0");
                _xmlWriter.WriteElementString("Contrast", "1.0");
                _xmlWriter.WriteEndElement(); // GlobalImageAdjustment

                _xmlWriter.WriteEndElement(); // IrisConfig

                _xmlWriter.Close();
                _writer.Close();
                _isOpen = false;
                // Delete existing backup
                if (File.Exists(_backupPath))
                {
                    File.Delete(_backupPath);
                }
                _configName = Path.Combine(_irisPath, _configName);
                // backup existing file
                if (File.Exists(_configName))
                {
                    File.Move(_configName, _backupPath);
                }

                // Rename .tmp to actual
                File.Move(_tempPath, _configName);

            }
        }
        /// <summary>
        /// Writes a minimal Helios profile to contain the viewports, and incluide the DCS Monitor Setup 
        /// and Additional Viewports interfaces.
        /// </summary>
        private void WriteServerProfile()
        {
            if (_isProfileOpen)
            {
                try
                {
                    if (!Directory.Exists(_irisProfilePath))
                    {
                        Directory.CreateDirectory(_irisProfilePath);
                    }
                    // Delete tmp file if exists
                    if (File.Exists(_tempProfilePath))
                    {
                        File.Delete(_tempProfilePath);
                    }
                    _profileWriter = new StreamWriter(_tempProfilePath, false);
                    TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

                    if(_rectangleIdNumber > 0) PlaceViewportsOnMonitor();
                    _localProfile.Interfaces.Add(new Patching.DCS.AdditionalViewports());
                    _localProfile.Interfaces.Add(new Patching.DCS.MonitorSetup());
                    if (_parent.Profile.Interfaces.FirstOrDefault(i => i is DCSInterface) is DCSInterface dcsInterface)
                    {
                        _localProfile.Interfaces.Add(dcsInterface);
                    }

                    _isProfileOpen = true;
                    _settings = new XmlWriterSettings();
                    _settings.Indent = true;

                    _xmlProfileWriter = XmlWriter.Create(_profileWriter, _settings);

                    HeliosSerializer serializer = new HeliosSerializer(null);
                    serializer.SerializeProfile(_localProfile, _xmlProfileWriter);
                    _localProfile.IsDirty = false;
                    _xmlProfileWriter.Close();
                    _profileWriter.Close();
                    _isProfileOpen = false;
                }
                catch (Exception ex)
                {
                    string message = $"Iris Write of Minimal Server Profile failure:  '{ex.Message}'";
                    ConfigManager.LogManager.LogError(message);
                    _isProfileOpen = false;
                }

                _isProfileOpen = false;
                // Delete existing backup
                if (File.Exists(_backupProfilePath))
                {
                    File.Delete(_backupProfilePath);
                }
                _profileName = Path.Combine(_irisProfilePath, _profileName);
                // backup existing file
                if (File.Exists(_profileName))
                {
                    File.Move(_profileName, _backupProfilePath);
                }

                // Rename .tmp to actual
                File.Move(_tempProfilePath, _profileName);
            }
        }
        /// <summary>
        /// Packs all of the viewports onto a single monitor and names them with the original viewport name. 
        /// We don't know the configurations of the local monitors, so the profile is written with the 
        /// viewports on a single 1920x1080 monitor with the intention that this profile will be 
        /// reconfigured using "Reset Monitors" in Profile Explorer, with No Scaling selected.
        /// </summary>
        private void PlaceViewportsOnMonitor()
        {
            _localProfile.Monitors[0].Top = 0;
            _localProfile.Monitors[0].Left = 0;
            _localProfile.Monitors[0].Width = 1920;
            _localProfile.Monitors[0].Height = 1080;

            for(int i = _localProfile.Monitors.Count() -1; i > 0 ; i--)
            {
                _localProfile.Monitors.RemoveAt(i);
            }

            Array.Resize(ref _packingRectangles, _rectangleIdNumber);
            RectanglePacker.Pack(_packingRectangles, out PackingRectangle bounds, PackingHints.FindBest, 1, 1, Convert.ToUInt32(_localProfile.Monitors[0].Width), Convert.ToUInt32(_localProfile.Monitors[0].Height));
            foreach(PackingRectangle viewport in _packingRectangles)
            {
                ViewportExtent vp = new ViewportExtent()
                {
                    Top = viewport.Y,
                    Left = viewport.X,
                    Width = viewport.Width,
                    Height = viewport.Height,
                    Name = _rectangleID[viewport.Id],
                    ViewportName = _rectangleID[viewport.Id],
                };
                vp.ScalingMode = Helios.Controls.TextScalingMode.None;
                vp.TextFormat.FontSize = 12;
                vp.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                // assign a stable color based on the UI name of this viewport
                vp.BackgroundColor = _colors[Math.Abs(vp.Name.GetHashCode()) % _colors.Length];                     
                _localProfile.Monitors[0].Children.Add(vp);
            }
            string[] info = new string[] { 
                @"This profile is to aid creation of a viewport layout suitable for Iris Server to export." ,
                @"Perform a ""Reset Monitors"" specifiying ""No Scaling"" and re-adjust positions as required.",
                @"Do not change the size of any viewports."
            };
            for (int i = -1;i <= 1; i++)
            {
                Helios.Controls.TextDecoration label = new Helios.Controls.TextDecoration()
                {
                    Top = _localProfile.Monitors[0].Height / 2 + (i * 48),
                    Left = 0,
                    Width = _localProfile.Monitors[0].Width,
                    Height = 48,
                    Name = $"Viewport_Label_{i+1}"
                };
                label.Text = info[i + 1];
                label.TextFormat.FontSize = 32;
                label.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                label.ScalingMode = Helios.Controls.TextScalingMode.None;
                label.FillBackground = true;
                label.BackgroundColor = Color.FromArgb(0x80, 0x1E, 0x1E, 0x1E);
                label.FontColor = Color.FromArgb(0xff, 0x6f, 0xe2, 0x06);
                _localProfile.Monitors[0].Children.Add(label);
            }
        }

        public bool IsOpen { get => _isOpen; }
        public bool IsProfileOpen { get => _isProfileOpen; }
        public bool HasBackground { get => _hasBackground; set => _hasBackground = value; }

        public Rect BackgroundRectangle
        {
            get => _backgroundRect;

            set
            {
                Rect oldValue = _backgroundRect;
                if (!oldValue.Equals(value))
                {
                    _backgroundRect = value;
                }
            }
        }
        private string VehicleName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_vehicleName))
                {
                    if (_parent.Profile.Interfaces.FirstOrDefault(i => i is DCSInterface) is DCSInterface dcsInterface)
                    {
                        if (dcsInterface is Interfaces.DCS.Soft.SoftInterface softInterface)
                        {
                            _vehicleName = softInterface.ImpersonatedVehicles[0];
                        }
                        else
                        {
                            if (dcsInterface.ExportModuleFormat == DCSExportModuleFormat.CaptZeenModule1)
                            {
                                _vehicleName = dcsInterface.ImpersonatedVehicleName;
                            }
                            else
                            {
                                _vehicleName = dcsInterface.VehicleName != "DCSGeneric" ? dcsInterface.VehicleName : "Unknown";
                            }
                        }
                    }
                    else
                    {
                        _vehicleName = "Unknown";
                    }
                }
                return _vehicleName;
            }
        }
        protected override void Update()
        {
        }

        internal void ConvertToDCS(ref Rect windowsRect)
        {
            windowsRect.Offset(-_parent.Rendered.TopLeft.X, -_parent.Rendered.TopLeft.Y);
            windowsRect.Scale(ConfigManager.DisplayManager.PixelsPerDip, ConfigManager.DisplayManager.PixelsPerDip);
        }

        private string GenerateSavedGamesPath(string savedGamesName) =>
            Path.Combine(KnownFolders.SavedGames, savedGamesName, "Config", "MonitorSetup");

        /// <summary>
        /// attempt to write the monitor setup file to all configured Saved Games folders
        /// and also persist our viewports information
        /// </summary>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        public override InstallationResult Install(IInstallationCallbacks callbacks)
        {
            try
            {
                return InstallationResult.Success;
            }
            catch (Exception ex)
            {
                Exception _ = ex;
                return InstallationResult.Fatal;
            }
        }

        /// <summary>
        /// The Iris config is not relevant to the Pre-flight Control Center checkF
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            return null;
        }

    }
}