// Copyright 2021 Ammo Goettsch
// 
// Patching is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Patching is distributed in the hope that it will be useful,
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
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Common;
using GadrocsWorkshop.Helios.Patching.DCS.Controls;

// REVISIT missing feature: support explicit view ports for MAIN and UI
// REVISIT factor out ShadowModel (IShadowVisualParent) into field?
namespace GadrocsWorkshop.Helios.Patching.DCS
{
    /// <summary>
    /// This interface represents the capability of generating a MonitorSetup lua file for DCS.
    /// </summary>
    [HeliosInterface("Patching.DCS.MonitorSetup", "DCS Monitor Setup", typeof(MonitorSetupEditor),
        Factory = typeof(UniqueHeliosInterfaceFactory))]
    public class MonitorSetup : ViewportCompilerInterface<DCSMonitor, DCSMonitorEventArgs>
    {
        internal const string DISPLAYS_SETTINGS_GROUP = "DCSMonitorSetup";
        internal const string PREFERENCES_SETTINGS_GROUP = "DCSMonitorSetupPreferences";

        /// <summary>
        /// fired when new viewport configuration is calculated
        /// </summary>
        public event EventHandler<UpdatedViewportsEventArgs> UpdatedViewports;

        private MonitorSetupGenerator _renderer;

        /// <summary>
        /// backing field for property CombinedMonitorSetupName, contains
        /// the name of the combined monitor setup that needs to be selected in DCS
        /// </summary>
        private string _combinedMonitorSetupName;

        /// <summary>
        /// backing field for property GenerateCombined, contains
        /// true if this profile uses the combined monitor setup file
        /// </summary>
        private bool _generateCombined = true;

        /// <summary>
        /// backing field for property MonitorLayoutMode, contains
        /// the currently selected method for mapping the DCS resolution to the desktop
        /// </summary>
        private MonitorLayoutMode _monitorLayoutMode;

        /// <summary>
        /// backing field for property UsingViewportProvider, contains
        /// true if Helios IViewportProvider is the source of
        /// additional viewports, and false if external solution is used
        /// </summary>
        private bool _usingViewportProvider = true;

        /// <summary>
        /// backing field for property MFDViewportInVR, contains
        /// true if an extra line to enable this feature is to be 
        /// included by MonitorSetupGenerator into the lua
        /// </summary>
        private bool _mfdViewportsInVR = false;

        /// <summary>
        /// backing field for property Rendered, contains
        /// The desktop rectangle (in Windows coordinates) that DCS will select for rendering, based on specifying its size as the
        /// "Resolution" parameter
        /// </summary>
        private Rect _rendered;

        public MonitorSetup()
            : base("DCS Monitor Setup")
        {
            // don't do any work here since we don't have a profile yet and we may just be
            // test instantiated for add interface dialog
        }

        /// <summary>
        /// find the minimum required DCS resolution to contain all the configured content
        /// </summary>
        private void UpdateRenderedRectangle()
        {
            // NOTE: we need this for status reporting, so this can't be in view model only
            IList<DCSMonitor> included = _monitors.Values.Where(m => m.Included).ToList();
            Point topLeft;
            Point bottomRight;
            switch (_monitorLayoutMode)
            {
                case MonitorLayoutMode.FromTopLeftCorner:
                    bottomRight = new Point(
                        included.Select(m => m.Visual.Left + m.Visual.Width).Max<double>(),
                        included.Select(m => m.Visual.Top + m.Visual.Height).Max<double>());
                    topLeft = new Point(0, 0);
                    topLeft -= GlobalOffset;
                    break;
                case MonitorLayoutMode.Column:
                {
                    Monitor primary = GetPrimaryMonitor();
                    if (primary == null)
                    {
                        // transient state during reset monitors can end up here
                        return;
                    }

                    bottomRight = new Point(
                        primary.Right,
                        included.Select(m => m.Visual.Top + m.Visual.Height).Max<double>());
                    topLeft = new Point(
                        primary.Left,
                        // NOTE: DCS will position at the minimum y coordinate, even if it doesn't end up using that monitor
                        _monitors.Values.Select(m => m.Visual.Top).Min<double>());
                    break;
                }
                case MonitorLayoutMode.Row:
                {
                    Monitor primary = GetPrimaryMonitor();
                    if (primary == null)
                    {
                        // transient state during reset monitors can end up here
                        return;
                    }

                    bottomRight = new Point(
                        included.Select(m => m.Visual.Left + m.Visual.Width).Max<double>(),
                        primary.Bottom);
                    topLeft = new Point(
                        // NOTE: DCS will position at the minimum x coordinate, even if it doesn't end up using that monitor
                        _monitors.Values.Select(m => m.Visual.Left).Min<double>(),
                        primary.Top);
                    break;
                }
                case MonitorLayoutMode.PrimaryOnly:
                case MonitorLayoutMode.TopLeftQuarter:
                {
                    Monitor primary = GetPrimaryMonitor();
                    if (primary == null)
                    {
                        // transient state during reset monitors can end up here
                        return;
                    }

                    bottomRight = new Point(primary.Right, primary.Bottom);
                    topLeft = new Point(primary.Left, primary.Top);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Rendered = new Rect(topLeft, bottomRight);
        }

        private Monitor GetPrimaryMonitor()
        {
            return _monitors.Values.Select(shadow => shadow.Monitor).FirstOrDefault(m => m.IsPrimaryDisplay);
        }

        /// <summary>
        /// if we somehow end up with no main view, select one
        /// this also happens when we start fresh
        /// </summary>
        private void AutoSelectMainView()
        {
            // REVISIT add support for explicit main and UI view viewports, which will require updates to the editor also
            if (_monitors.Values.Any(m => m.Main && m.Included))
            {
                // got at least one, don't even create the list
                return;
            }

            IList<DCSMonitor> selectedMonitors = _monitors.Values
                .Where(m => m.Included)
                .ToList();

            // select any monitors with minimal viewports (might not be entirely empty)
            int min = selectedMonitors
                .Select(m => m.ViewportCount)
                .Min<int>();
            foreach (DCSMonitor monitor in selectedMonitors.Where(m => m.ViewportCount == min))
            {
                monitor.Main = true;
            }
        }

        /// <summary>
        /// if we somehow end up with no UI view, select one
        /// this also happens when we start fresh
        /// </summary>
        private void AutoSelectUserInterfaceView()
        {
            // REVISIT add support for explicit main and UI view viewports, which will require updates to the editor also
            if (_monitors.Values.Any(m => m.UserInterface && m.Included))
            {
                // got at least one, don't even create the list
                return;
            }

            IList<DCSMonitor> selectedMonitors = _monitors.Values
                .Where(m => m.Included)
                .ToList();

            // select any monitors with minimal viewports (might not be entirely empty)
            int minViewports = selectedMonitors
                .Select(m => m.ViewportCount)
                .Min<int>();
            IList<DCSMonitor> sortedMinPopulated = selectedMonitors
                .Where(m => m.ViewportCount == minViewports)
                .OrderBy(m => m.Monitor.Left)
                .ToList();
            double maxSize = sortedMinPopulated.Select(m => m.Monitor.Width).Max<double>();
            DCSMonitor ui = sortedMinPopulated.First(m => Math.Abs(m.Monitor.Width - maxSize) < 0.001);
            ui.UserInterface = true;
        }

        /// <summary>
        /// check monitor settings and clean up any abandoned ones
        /// this is necessary because when we run with a profile that has not been reset,
        /// we end up with settings for monitors that don't actually exist.  Also, actual
        /// changes to our windows monitors will abandon settings for monitors that have
        /// changed locations or have been removed.
        /// </summary>
        private void CheckMonitorSettings()
        {
            // get the list of real monitors and all their serialized settings names
            HashSet<string> displayKeys = new HashSet<string>();
            foreach (Monitor monitor in ConfigManager.DisplayManager.Displays)
            {
                displayKeys.UnionWith(DCSMonitor.GetAllKeys(monitor));
            }

            if (!(ConfigManager.SettingsManager is ISettingsManager2 settings))
            {
                return;
            }

            // get the list of configured monitors in the settings file
            List<string> stableKeys = settings.EnumerateSettingNames(DISPLAYS_SETTINGS_GROUP).ToList();

            if (!ConfigManager.Application.SettingsAreWritable)
            {
                // this is a readonly application, let another application (i.e. Profile Editor) clean this up
                return;
            }

            // clean up settings for monitors that are not existing any more or are phantoms from unreset profiles
            foreach (string key in stableKeys.Where(key => !displayKeys.Contains(key)))
            {
                ConfigManager.LogManager.LogDebug($"removed setting '{key}' that does not refer to a real monitor");
                settings.DeleteSetting(DISPLAYS_SETTINGS_GROUP, key);
            }
        }

        /// <summary>
        /// Calculate a key/hash that identifies a set of monitors and their assignment to
        /// Main and UI views.  Any change to the monitor setup that is not an extra viewport
        /// will change this value.
        /// </summary>
        private string CalculateMonitorLayoutKey()
        {
            IEnumerable<string> keys = Monitors
                .Where(m => m.Included)
                .OrderBy(m => m.Monitor.Left)
                .ThenBy(m => m.Monitor.Top)
                .Select(m => m.CreateStateKey())
                .Append(_monitorLayoutMode.ToString());

            return string.Join(", ", keys);
        }

        private void EnsureValidMonitorSelections()
        {
            // in case of changes to the monitor layout mode, scan all monitors and make sure our selections are valid
            switch (_monitorLayoutMode)
            {
                case MonitorLayoutMode.FromTopLeftCorner:
                    EnsureTopLeftLayout();
                    break;
                case MonitorLayoutMode.Column:
                    EnsureColumnLayout();
                    break;
                case MonitorLayoutMode.Row:
                    EnsureRowLayout();
                    break;
                case MonitorLayoutMode.PrimaryOnly:
                case MonitorLayoutMode.TopLeftQuarter:
                    EnsurePrimaryLayout();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EnsureTopLeftLayout()
        {
            // NOTE: always legal, just reset defaults
            foreach (DCSMonitor shadow in _monitors.Values)
            {
                shadow.Included = shadow.Included || shadow.HasContent;
                shadow.Permissions = DCSMonitor.PermissionsFlags.CanInclude |
                                     DCSMonitor.PermissionsFlags.CanExclude;
            }
        }

        private void EnsureRowLayout()
        {
            Monitor primary = GetPrimaryMonitor();
            if (primary == null)
            {
                // transient state during reset monitors can end up here
                return;
            }

            // WARNING: Rect does not understand rectangles that extend to negative infinity
            Rect allowed = new Rect(
                new Point(double.MinValue, primary.DisplayRectangle.Top),
                new Point(double.MaxValue, primary.DisplayRectangle.Bottom));
            foreach (DCSMonitor shadow in _monitors.Values)
            {
                Rect intersection = shadow.Monitor.DisplayRectangle;
                intersection.Intersect(allowed);
                // WARNING: IntersectsWith is true even if the intersection is a touch line of 0 height, so we can't use it
                if (intersection.Height >= 1d)
                {
                    if (shadow.Monitor.Left <= primary.Left)
                    {
                        // cannot exclude main and area to the left of main
                        shadow.Included = true;
                        shadow.Permissions = DCSMonitor.PermissionsFlags.CanInclude;
                    }
                    else
                    {
                        shadow.Included = shadow.Included || shadow.HasContent;
                        shadow.Permissions = DCSMonitor.PermissionsFlags.CanInclude |
                                             DCSMonitor.PermissionsFlags.CanExclude;
                    }
                }
                else
                {
                    // monitor is not part of row
                    shadow.Lockout();
                }
            }
        }

        private void EnsureColumnLayout()
        {
            Monitor primary = GetPrimaryMonitor();
            if (primary == null)
            {
                // transient state during reset monitors can end up here
                return;
            }

            // WARNING: Rect does not understand rectangles that extend to negative infinity
            Rect allowed = new Rect(
                new Point(primary.DisplayRectangle.Left, double.MinValue),
                new Point(primary.DisplayRectangle.Right, double.MaxValue));
            foreach (DCSMonitor shadow in _monitors.Values)
            {
                Rect intersection = shadow.Monitor.DisplayRectangle;
                intersection.Intersect(allowed);
                // WARNING: IntersectsWith is true even if the intersection is a touch line of 0 width, so we can't use it
                if (intersection.Width >= 1d)
                {
                    if (shadow.Monitor.Top <= primary.Monitor.Top)
                    {
                        // cannot exclude main and area above main
                        shadow.Included = true;
                        shadow.Permissions = DCSMonitor.PermissionsFlags.CanInclude;
                    }
                    else
                    {
                        shadow.Included = shadow.Included || shadow.HasContent;
                        shadow.Permissions = DCSMonitor.PermissionsFlags.CanInclude |
                                             DCSMonitor.PermissionsFlags.CanExclude;
                    }
                }
                else
                {
                    // monitor is not part of column
                    shadow.Lockout();
                }
            }
        }

        /// <summary>
        /// Reports if the viewports are being hidden by the parent monitor by a Fill Background
        /// </summary>
        private IEnumerable<StatusReportItem> ReportViewportMasking()
        {
            bool isMasked = false;

            foreach(Util.Shadow.ShadowVisual viewport in Viewports)
            {
                if (viewport.IsViewport)
                {
                    isMasked = viewport.Monitor.FillBackground;
                }
            }

            if (isMasked)
            {
                yield return new StatusReportItem
                {
                    Status = "One or more DCS viewports are masked by a monitor with a fill background. The result is you won't see the DCS viewports being rendered to the monitor.",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Warning,
                    Recommendation = "Review your profile and ensure monitors do not have Fill Background enabled when a viewport is configured for that monitor"
                };
            }
        }

        private void EnsurePrimaryLayout()
        {
            foreach (DCSMonitor shadow in _monitors.Values)
            {
                if (shadow.Monitor.IsPrimaryDisplay)
                {
                    // cannot exclude main
                    shadow.Included = true;
                    shadow.Permissions = DCSMonitor.PermissionsFlags.CanInclude;
                }
                else
                {
                    // monitor is not allowed
                    shadow.Lockout();
                }
            }
        }

        #region Overrides

        // all we have right now is the monitor setup file generator
        public override InstallationResult Install(IInstallationCallbacks callbacks) => _renderer.Install(callbacks);

        protected override List<StatusReportItem> CreateStatusReport()
        {
            MFDViewportsInVR = ConfigManager.SettingsManager.LoadSetting(PREFERENCES_SETTINGS_GROUP, "MFDViewportsInVRMode", false);

            // all we have right now is the monitor setup file generator
            // actually enumerate the report now and store it
            List<StatusReportItem> newReport = _renderer.PerformReadyCheck().ToList();

            // send newly calculated viewports data to any observers (such as combined monitor setup view model)
            UpdatedViewports?.Invoke(this, new UpdatedViewportsEventArgs(_renderer.LocalViewports));

            newReport.AddRange(ReportViewportMasking());

            return newReport;
        }

        public override IEnumerable<StatusReportItem> PerformReadyCheck() {
            MFDViewportsInVR = ConfigManager.SettingsManager.LoadSetting(MonitorSetup.PREFERENCES_SETTINGS_GROUP, "MFDViewportsInVRMode", false);
            return _renderer.PerformReadyCheck(); 
        }

        protected override void UpdateAllGeometry()
        {
            if (!_monitorsValid)
            {
                // don't process events if monitors are not valid
                // we will schedule an update once they are
                MonitorLayoutKey = null;
                return;
            }

            ReportViewportMasking();
            CheckMonitorSettings();
            EnsureValidMonitorSelections();
            AutoSelectMainView();
            AutoSelectUserInterfaceView();
            UpdateGlobalOffset();
            UpdateRenderedRectangle();
            MonitorLayoutKey = CalculateMonitorLayoutKey();
        }

        protected override void AttachToProfileOnMainThread()
        {
            // customize naming in case of custom Documents folder
            string documentsFolderName = Path.GetFileName(ConfigManager.DocumentPath);
            _combinedMonitorSetupName = string.IsNullOrEmpty(documentsFolderName) ? "Helios" : documentsFolderName;

            // read persistent config
            _monitorLayoutMode = ConfigManager.SettingsManager.LoadSetting(PREFERENCES_SETTINGS_GROUP,
                "MonitorLayoutMode", MonitorLayoutMode.FromTopLeftCorner);

            // real initialization, not just a test instantiation
            Combined.Initialize();

            _renderer = new MonitorSetupGenerator(this);

            base.AttachToProfileOnMainThread();
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            base.DetachFromProfileOnMainThread(oldProfile);

            // deallocate renderer we allocate on Attach
            _renderer?.Dispose();
            _renderer = null;
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            while (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "GenerateCombined":
                    {
                        string text = reader.ReadElementString("GenerateCombined");
                        _generateCombined = bc.ConvertFromInvariantString(text) as bool? ?? true;
                        break;
                    }
                    case "UsingViewportProvider":
                    {
                        string text = reader.ReadElementString("UsingViewportProvider");
                        _usingViewportProvider = bc.ConvertFromInvariantString(text) as bool? ?? true;
                        break;
                    }
                    default:
                    {
                        // ignore unsupported settings
                        string elementName = reader.Name;
                        string discard = reader.ReadInnerXml();
                        ConfigManager.LogManager.LogWarning(
                            $"Ignored unsupported {GetType().Name} setting '{elementName}' with value '{discard}'");
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (!_generateCombined)
            {
                writer.WriteElementString("GenerateCombined", bc.ConvertToInvariantString(false));
            }

            if (!_usingViewportProvider)
            {
                writer.WriteElementString("UsingViewportProvider", bc.ConvertToInvariantString(false));
            }
        }

        protected override DCSMonitor CreateShadowMonitor(Monitor monitor) => new DCSMonitor(this, monitor);

        public override string Description =>
            "Utility interface that writes a DCS MonitorSetup Lua file to configure screen layout for the current profile.";

        public override string RemovalNarrative =>
            "Delete this interface to no longer let Helios manage the monitor setup file for this profile";

        #endregion

        #region Properties

        /// <summary>
        /// true if this profile uses the combined monitor setup file
        /// </summary>
        public bool GenerateCombined
        {
            get => _generateCombined;
            set
            {
                if (_generateCombined == value)
                {
                    return;
                }

                bool oldValue = _generateCombined;
                _generateCombined = value;
                OnPropertyChanged("GenerateCombined", oldValue, value, true);
            }
        }

        /// <summary>
        /// The desktop rectangle (in Windows coordinates) that DCS will select for rendering, based on specifying its size as the
        /// "Resolution" parameter, not persisted
        /// </summary>
        public Rect Rendered
        {
            get => _rendered;
            set
            {
                if (_rendered == value)
                {
                    return;
                }

                Rect oldValue = _rendered;
                _rendered = value;
                OnPropertyChanged("Rendered", oldValue, value, false);
            }
        }
        
        public bool MFDViewportsInVR
        {
            get => _mfdViewportsInVR;
            set => _mfdViewportsInVR = value;
        }

        /// <summary>
        /// the name of the combined monitor setup that needs to be selected in DCS
        /// </summary>
        public string CombinedMonitorSetupName
        {
            get => _combinedMonitorSetupName;
            set
            {
                if (_combinedMonitorSetupName != null && _combinedMonitorSetupName == value)
                {
                    return;
                }

                string oldValue = _combinedMonitorSetupName;
                _combinedMonitorSetupName = value;
                OnPropertyChanged("CombinedMonitorSetupName", oldValue, value, true);
            }
        }

        /// <summary>
        /// true if Helios IViewportProvider is the source of
        /// additional viewports, and false if external solution is used
        /// </summary>
        public bool UsingViewportProvider
        {
            get => _usingViewportProvider;
            set
            {
                if (_usingViewportProvider == value)
                {
                    return;
                }

                bool oldValue = _usingViewportProvider;
                _usingViewportProvider = value;
                OnPropertyChanged("UsingViewportProvider", oldValue, value, true);
            }
        }


        /// <summary>
        /// the currently selected method for mapping the DCS resolution to the desktop, global setting, so no Undo support
        /// </summary>
        public MonitorLayoutMode MonitorLayoutMode
        {
            get => _monitorLayoutMode;
            set
            {
                if (_monitorLayoutMode == value)
                {
                    return;
                }

                MonitorLayoutMode oldValue = _monitorLayoutMode;
                _monitorLayoutMode = value;
                ConfigManager.SettingsManager.SaveSetting(PREFERENCES_SETTINGS_GROUP, "MonitorLayoutMode",
                    _monitorLayoutMode);
                ScheduleGeometryChange();
                OnPropertyChanged("MonitorLayoutMode", oldValue, value, false);
            }
        }

        public string StatusName => Name;

        /// <summary>
        /// functionality supporting combined monitor setups
        /// </summary>
        internal CombinedMonitorSetup Combined { get; } = new CombinedMonitorSetup();

        internal string MonitorLayoutKey { get; private set; }

        internal MonitorSetupGenerator Renderer => _renderer;

        #endregion

        // having a subset or superset of the monitors in the profile is considered reset but it may
        // break monitor setup, so we have to do this additional check
        public class UpdatedViewportsEventArgs : EventArgs
        {
            public UpdatedViewportsEventArgs(ViewportSetupFile localViewports)
            {
                LocalViewports = localViewports;
            }

            #region Properties

            public ViewportSetupFile LocalViewports { get; }

            #endregion
        }
    }
}