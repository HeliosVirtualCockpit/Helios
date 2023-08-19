// Copyright 2020 Ammo Goettsch
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GadrocsWorkshop.Helios.Controls.Special;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Util.DCS;
using GadrocsWorkshop.Helios.Util.Shadow;
using GadrocsWorkshop.Helios.Windows;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    /// <summary>
    /// view model for preview and interaction with DCS Monitor Setup
    /// </summary>
    internal class MonitorSetupViewModel : HeliosViewModel<MonitorSetup>, IStatusReportObserver
    {
        private const double DEFAULT_SCALE = 0.075;

        private readonly Dictionary<DCSMonitor, MonitorViewModel> _monitors =
            new Dictionary<DCSMonitor, MonitorViewModel>();

        private readonly Dictionary<ShadowVisual, ViewportViewModel> _viewports =
            new Dictionary<ShadowVisual, ViewportViewModel>();

        internal const string PREFERENCES_SETTINGS_GROUP = "DCSMonitorSetupPreferences";

        /// <summary>
        /// backing field for property ConfigureCommand, contains
        /// command handlers for "configure monitor setups"
        /// </summary>
        private ICommand _configureCommand;

        /// <summary>
        /// true if we are done loaded and should process changes
        /// </summary>
        private readonly bool _loaded;

        internal MonitorSetupViewModel(MonitorSetup data) : base(data)
        {
            Scale = ConfigManager.SettingsManager.LoadSetting(MonitorSetup.PREFERENCES_SETTINGS_GROUP, "Scale",
                DEFAULT_SCALE);
            CombinedMonitorSetup = new CombinedMonitorSetupViewModel(Data);
            Monitors = new ObservableCollection<MonitorViewModel>();
            Viewports = new ObservableCollection<ViewportViewModel>();
            SourceOfAdditionalViewports = Data.UsingViewportProvider
                ? SourceOfAdditionalViewports.AdditionalViewportsInterface
                : SourceOfAdditionalViewports.ThirdPartySolution;
            foreach (DCSMonitor monitor in Data.Monitors)
            {
                AddMonitor(monitor, Data.GlobalOffset);
            }

            foreach (ShadowVisual viewport in Data.Viewports)
            {
                if(!(viewport.Viewport is DCSMonitorScriptModifier))
                {
                    AddViewport(viewport, Data.GlobalOffset);
                }
            }

            UpdateAllGeometry();
            _loaded = true;

            // register for changes and get latest report
            Data.GeometryChangeDelayed += Data_GeometryChangeDelayed;
            Data.GlobalOffsetChanged += Data_GlobalOffsetChanged;
            Data.MonitorAdded += Data_MonitorAdded;
            Data.MonitorRemoved += Data_MonitorRemoved;
            Data.ViewportAdded += Data_ViewportAdded;
            Data.ViewportRemoved += Data_ViewportRemoved;
            Data.Subscribe(this);

            Data.InvalidateStatusReport();
        }

        private static void OnScaleChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonitorSetupViewModel model = (MonitorSetupViewModel) d;
            if (!model._loaded)
            {
                return;
            }

            ConfigManager.SettingsManager.SaveSetting(MonitorSetup.PREFERENCES_SETTINGS_GROUP, "Scale", model.Scale);

            foreach (MonitorViewModel monitor in model._monitors.Values)
            {
                monitor.Update(model.Scale);
            }

            foreach (ViewportViewModel viewport in model._viewports.Values)
            {
                viewport.Update(model.Scale);
            }

            model.UpdateBounds();
        }

        private static void OnSourceOfAdditionalViewportsChange(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            MonitorSetupViewModel model = (MonitorSetupViewModel)d;
            if (!model._loaded)
            {
                return;
            }

            model.Data.UsingViewportProvider = ((SourceOfAdditionalViewports) e.NewValue) ==
                                               SourceOfAdditionalViewports.AdditionalViewportsInterface;
            model.Data.InvalidateStatusReport();
        }

        private void ProtectLastMonitor(List<MonitorViewModel> monitors, Action<MonitorViewModel, bool> setter)
        {
            if (monitors.Count == 1)
            {
                // can't remove the last main view
                setter(monitors[0], false);
            }
            else
            {
                monitors.ForEach(m => setter(m, true));
            }
        }

        private void Data_ViewportRemoved(object sender, ShadowViewportEventArgs e)
        {
            if (_viewports.TryGetValue(e.Data, out ViewportViewModel model))
            {
                _viewports.Remove(e.Data);
                Viewports.Remove(model);
            }
        }

        private void Data_ViewportAdded(object sender, ShadowViewportEventArgs e)
        {
            IShadowVisualParent parent = (IShadowVisualParent) sender;
            AddViewport(e.Data, parent.GlobalOffset);
        }

        private void AddViewport(ShadowVisual viewport, Vector globalOffset)
        {
            ViewportViewModel model = new ViewportViewModel(viewport, globalOffset, Scale);
            _viewports[viewport] = model;
            Viewports.Add(model);
        }

        private void Data_MonitorRemoved(object sender, DCSMonitorEventArgs e)
        {
            if (_monitors.TryGetValue(e.Data, out MonitorViewModel model))
            {
                _monitors.Remove(e.Data);
                Monitors.Remove(model);
                model.Data.PropertyChanged -= Shadow_PropertyChanged;
            }
        }

        private void Data_MonitorAdded(object sender, DCSMonitorEventArgs e)
        {
            IShadowVisualParent parent = (IShadowVisualParent) sender;
            AddMonitor(e.Data, parent.GlobalOffset);
        }

        private void Shadow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // some settings about our shadow objects have changed
            UpdateAllGeometry();
        }

        private void AddMonitor(DCSMonitor shadow, Vector globalOffset)
        {
            MonitorViewModel model = new MonitorViewModel(shadow, globalOffset, Scale);
            _monitors[shadow] = model;
            Monitors.Add(model);

            // register to changes to our settings (not the geometry of the observed visuals)
            model.Data.PropertyChanged += Shadow_PropertyChanged;
        }

        private void Data_GlobalOffsetChanged(object sender, MonitorSetup.GlobalOffsetEventArgs e)
        {
            foreach (MonitorViewModel monitor in _monitors.Values)
            {
                monitor.Update(e.GlobalOffset);
            }

            foreach (ViewportViewModel viewport in _viewports.Values)
            {
                viewport.Update(e.GlobalOffset);
            }
        }

        /// <summary>
        /// not automatically called via IDisposable, because of
        /// strong references in events
        /// </summary>
        public void Dispose()
        {
            CombinedMonitorSetup.Dispose();
            Data.Unsubscribe(this);
            Data.GeometryChangeDelayed -= Data_GeometryChangeDelayed;
            Data.GlobalOffsetChanged -= Data_GlobalOffsetChanged;
            Data.MonitorAdded -= Data_MonitorAdded;
            Data.MonitorRemoved -= Data_MonitorRemoved;
            Data.ViewportAdded -= Data_ViewportAdded;
            Data.ViewportRemoved -= Data_ViewportRemoved;
        }

        private void Data_GeometryChangeDelayed(object sender, EventArgs e)
        {
            UpdateAllGeometry();
        }

        private void UpdateAllGeometry()
        {
            ClassifyMonitors(out List<MonitorViewModel> mainMonitors, out List<MonitorViewModel> uiMonitors);
            UpdateBounds();
            ProtectLastMonitor(mainMonitors, (m, value) => m.MainAssignmentCanBeChanged = value);
            ProtectLastMonitor(uiMonitors, (m, value) => m.UserInterfaceAssignmentCanBeChanged = value);
            UpdateResolutionRectangle();
        }

        private void UpdateResolutionRectangle()
        {
            Rect resolution = Data.Rendered;
            Data.Renderer.ConvertToDCS(ref resolution);
            ResolutionRectangle = resolution;
        }

        private void ClassifyMonitors(out List<MonitorViewModel> mainMonitors, out List<MonitorViewModel> uiMonitors)
        {
            List<MonitorViewModel> main = new List<MonitorViewModel>();
            List<MonitorViewModel> ui = new List<MonitorViewModel>();
            foreach (MonitorViewModel monitor in Monitors)
            {
                monitor.UpdateFromPermissions();

                if (monitor.Data.Main)
                {
                    main.Add(monitor);
                }

                if (monitor.Data.UserInterface)
                {
                    ui.Add(monitor);
                }
            }

            mainMonitors = main;
            uiMonitors = ui;
        }

        private void UpdateBounds()
        {
            Rect totalBounds = new Rect(0, 0, 1, 1);
            Rect mainBounds = Rect.Empty;
            Rect uiBounds = Rect.Empty;

            foreach (MonitorViewModel monitor in Monitors)
            {
                totalBounds.Union(monitor.Rect);
                if (monitor.Data.Main)
                {
                    if (Data.MonitorLayoutMode == MonitorLayoutMode.TopLeftQuarter)
                    {
                        // special
                        Rect scaled = monitor.Rect;
                        scaled.Width = scaled.Width * 0.5d;
                        scaled.Height = scaled.Height * 0.5d;
                        mainBounds.Union(scaled);
                    }
                    else
                    {
                        mainBounds.Union(monitor.Rect);
                    }
                }

                if (monitor.Data.UserInterface)
                {
                    uiBounds.Union(monitor.Rect);
                }

                if (!monitor.Data.Included)
                {
                    continue;
                }

                // convert monitor coordinates to DCS coordinates and always include 0,0 in the union
                // to calculate the resolution required by DCS
                Rect rawDcsCoordinates = monitor.RawRect;
                rawDcsCoordinates.Offset(Data.GlobalOffset);
            }

            // update the rectangle showing where the DCS "Resolution" will be mapped
            Rect rendered = Data.Rendered;
            rendered.Offset(Data.GlobalOffset);
            rendered.Scale(Scale, Scale);
            ScaledRenderedRectangle = rendered;

            // trim display areas to resolution rectangle
            mainBounds.Intersect(rendered);
            uiBounds.Intersect(rendered);

            // other UI-scale markers
            ScaledTotalWidth = totalBounds.Width;
            ScaledTotalHeight = totalBounds.Height;

            // set valid rectangles to avoid warnings or errors logged from bindings
            ScaledMain = mainBounds.IsEmpty ? new Rect(0d, 0d, 1d, 1d) : mainBounds;
            ScaledUserInterface = uiBounds.IsEmpty ? new Rect(0d, 0d, 1d, 1d) : uiBounds;

            // reasonable scale for Icons shown inside the screen rectangles
            IconScale = (Scale - 0.02) * 0.3 / 0.02;
        }

        public void ReceiveStatusReport(string name, string description, IList<StatusReportItem> statusReport)
        {
            _ = name;
            _ = description;

            // figure out our UI status
            StatusCodes newStatus = StatusCodes.Unknown;
            if (!Data.CheckMonitorsValid)
            {
                newStatus = StatusCodes.ResetMonitorsRequired;
            }
            else if (string.IsNullOrWhiteSpace(Data.Profile.Path))
            {
                newStatus = StatusCodes.ProfileSaveRequired;
            }
            else if (!InstallationLocations.Singleton.Active.Any())
            {
                newStatus = StatusCodes.NoLocations;
            }
            else
            {
                // see if any items aren't "configuration up to date"
                bool outOfDate = statusReport.Any(item =>
                    !item.Flags.HasFlag(StatusReportItem.StatusFlags.ConfigurationUpToDate));
                newStatus = outOfDate ? StatusCodes.OutOfDate : StatusCodes.UpToDate;
            }

            Status = newStatus;
        }

        private void Configure(object parameter)
        {
            // need a host in the visual tree for our dialog boxes, so it is passed as the parameter
            InstallationDialogs installationDialogs = new InstallationDialogs((IInputElement) parameter);

            // gather information about warnings and get consent to install anyway, if applicable
            List<StatusReportItem> items = new List<StatusReportItem>();
            List<string> lines = new List<string>();
            foreach (ViewportSetupFileViewModel model in CombinedMonitorSetup.Combined)
            {
                items.AddRange(GatherWarnings(model)
                    .Where(i => i.Severity >= StatusReportItem.SeverityCode.Warning));
            }

            if (items.Count > 0)
            {
                // need a header
                lines.Add("Problems will exist in the combined Monitor Setup 'Helios'");
                lines.Add("");
            }

            if (!Data.GenerateCombined)
            {
                IList<StatusReportItem> currentItems = GatherWarnings(CombinedMonitorSetup.CurrentViewportSetup)
                    .Where(i => i.Severity >= StatusReportItem.SeverityCode.Warning)
                    .ToList();
                if (currentItems.Any())
                {
                    lines.Add("Problems will exist in the separate Monitor Setup for this Profile");
                    lines.Add("");
                    items.AddRange(currentItems);
                }
            }

            if (items.Any())
            {
                string extraText = "";
                if (items.Any(i => !i.Flags.HasFlag(StatusReportItem.StatusFlags.ConfigurationUpToDate)))
                {
                    extraText = "  The Monitor Setup may work but it will be considered out of date until you resolve these problems, so Profile Editor will continue to prompt you to generate it again.";
                }
                lines.Add($"You can generate the Monitor Setup anyway and just use it with those limitations.{extraText}");

                // present
                InstallationPromptResult result = installationDialogs.DangerPrompt("Incomplete Monitor Setup",
                    string.Join(Environment.NewLine, lines),
                    items);
                if (result == InstallationPromptResult.Cancel)
                {
                    // don't do it
                    return;
                }
            }

            // install
            Data.Install(installationDialogs);
        }

        private IEnumerable<StatusReportItem> GatherWarnings(ViewportSetupFileViewModel model)
        {
            switch (model.Status)
            {
                case ViewportSetupFileStatus.OK:
                    break;
                case ViewportSetupFileStatus.Unknown:
                case ViewportSetupFileStatus.NotGenerated:
                    yield return new StatusReportItem
                    {
                        Status =
                            $"Viewport information for profile '{model.ProfileName}' will not be included because it is not available.",
                        Severity = StatusReportItem.SeverityCode.Warning
                    };
                    break;
                case ViewportSetupFileStatus.Conflict:
                    if (!Data.GenerateCombined)
                    {
                        // conflict does not matter if we have a separate file
                        break;
                    }

                    yield return new StatusReportItem
                    {
                        Status =
                            $"At least one viewports for profile '{model.ProfileName}' conflicts with other viewports in the combined monitor setup:",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };
                    yield return new StatusReportItem
                    {
                        Status = $"{model.ProblemNarrative}.",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };
                    break;
                case ViewportSetupFileStatus.OutOfDate:
                    yield return new StatusReportItem
                    {
                        Status =
                            $"Viewports for profile '{model.ProfileName}' may be in the wrong location because the saved information is out of date.",
                        Severity = StatusReportItem.SeverityCode.Warning
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Properties

        /// <summary>
        /// command handlers for "configure monitor setups"
        /// </summary>
        public ICommand ConfigureCommand
        {
            get
            {
                _configureCommand = _configureCommand ?? new RelayCommand(Configure);
                return _configureCommand;
            }
        }

        public StatusCodes Status
        {
            get => (StatusCodes) GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StatusCodes), typeof(MonitorSetupViewModel),
                new PropertyMetadata(StatusCodes.Unknown));

        public ObservableCollection<MonitorViewModel> Monitors
        {
            get => (ObservableCollection<MonitorViewModel>) GetValue(MonitorsProperty);
            set => SetValue(MonitorsProperty, value);
        }

        public static readonly DependencyProperty MonitorsProperty =
            DependencyProperty.Register("Monitors", typeof(ObservableCollection<MonitorViewModel>),
                typeof(MonitorSetupViewModel), new PropertyMetadata(null));

        public ObservableCollection<ViewportViewModel> Viewports
        {
            get => (ObservableCollection<ViewportViewModel>) GetValue(ViewportsProperty);
            set => SetValue(ViewportsProperty, value);
        }

        public static readonly DependencyProperty ViewportsProperty =
            DependencyProperty.Register("Viewports", typeof(ObservableCollection<ViewportViewModel>),
                typeof(MonitorSetupViewModel), new PropertyMetadata(null));

        public double ScaledTotalWidth
        {
            get => (double) GetValue(ScaledTotalWidthProperty);
            set => SetValue(ScaledTotalWidthProperty, value);
        }

        public static readonly DependencyProperty ScaledTotalWidthProperty =
            DependencyProperty.Register("ScaledTotalWidth", typeof(double), typeof(MonitorSetupViewModel),
                new PropertyMetadata(1.0));

        public double ScaledTotalHeight
        {
            get => (double) GetValue(ScaledTotalHeightProperty);
            set => SetValue(ScaledTotalHeightProperty, value);
        }

        public static readonly DependencyProperty ScaledTotalHeightProperty =
            DependencyProperty.Register("ScaledTotalHeight", typeof(double), typeof(MonitorSetupViewModel),
                new PropertyMetadata(1.0));

        public Rect ScaledRenderedRectangle
        {
            get => (Rect) GetValue(ScaledRenderedRectangleProperty);
            set => SetValue(ScaledRenderedRectangleProperty, value);
        }

        public static readonly DependencyProperty ScaledRenderedRectangleProperty =
            DependencyProperty.Register("ScaledRenderedRectangle", typeof(Rect), typeof(MonitorSetupViewModel),
                new PropertyMetadata(null));

        public Rect ScaledMain
        {
            get => (Rect) GetValue(ScaledMainProperty);
            set => SetValue(ScaledMainProperty, value);
        }

        public static readonly DependencyProperty ScaledMainProperty =
            DependencyProperty.Register("ScaledMain", typeof(Rect), typeof(MonitorSetupViewModel),
                new PropertyMetadata(null));

        public Rect ScaledUserInterface
        {
            get => (Rect) GetValue(ScaledUserInterfaceProperty);
            set => SetValue(ScaledUserInterfaceProperty, value);
        }

        public static readonly DependencyProperty ScaledUserInterfaceProperty =
            DependencyProperty.Register("ScaledUserInterface", typeof(Rect), typeof(MonitorSetupViewModel),
                new PropertyMetadata(null));


        public double Scale
        {
            get => (double) GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(MonitorSetupViewModel),
                new PropertyMetadata(DEFAULT_SCALE, OnScaleChange));

        public double IconScale
        {
            get => (double) GetValue(IconScaleProperty);
            set => SetValue(IconScaleProperty, value);
        }

        public static readonly DependencyProperty IconScaleProperty =
            DependencyProperty.Register("IconScale", typeof(double), typeof(MonitorSetupViewModel),
                new PropertyMetadata(1.0));
        public CombinedMonitorSetupViewModel CombinedMonitorSetup { get; }

        public SourceOfAdditionalViewports SourceOfAdditionalViewports
        {
            get => (SourceOfAdditionalViewports) GetValue(SourceOfAdditionalViewportsProperty);
            set => SetValue(SourceOfAdditionalViewportsProperty, value);
        }

        public static readonly DependencyProperty SourceOfAdditionalViewportsProperty =
            DependencyProperty.Register("SourceOfAdditionalViewports", typeof(SourceOfAdditionalViewports),
                typeof(MonitorSetupViewModel),
                new PropertyMetadata(default(SourceOfAdditionalViewports), OnSourceOfAdditionalViewportsChange));

        /// <summary>
        /// The desktop rectangle (in DCS coordinates) that DCS will select for rendering, based on specifying its size as the "Resolution" parameter
        /// </summary>
        public Rect ResolutionRectangle
        {
            get => (Rect) GetValue(ResolutionRectangleProperty);
            set => SetValue(ResolutionRectangleProperty, value);
        }

        public static readonly DependencyProperty ResolutionRectangleProperty = DependencyProperty.Register("ResolutionRectangle",
            typeof(Rect), typeof(MonitorSetupViewModel), new PropertyMetadata(null));

        #endregion
    }
}