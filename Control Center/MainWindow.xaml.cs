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

using System.Linq;
using System.Windows.Controls;
using GadrocsWorkshop.Helios.Interfaces.Capabilities.ProfileAwareInterface;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.ControlCenter
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using GadrocsWorkshop.Helios.Windows;
    using GadrocsWorkshop.Helios.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const long TOPMOST_TICK_COUNT = 3 * TimeSpan.TicksPerSecond;

        private readonly List<string> _profiles = new List<string>();
        private int _profileIndex = -1;

        private DispatcherTimer _dispatcherTimer;
        private List<MonitorWindow> _windows = new List<MonitorWindow>();
        private long _lastTick;

        private WindowInteropHelper _helper;
        private bool _prefsShown;

        private HotKey _hotkey;

        // current status message to display
        private StatusValue _status = StatusValue.Empty;

        // most recently received simulator info
        private string _lastProfileHint = "";
        private string _lastDriverStatus = "";

        // our writable preferences
        // NOTE: we have readonly access to the global helios settings being written by Profile Editor
        public SettingsManager PreferencesFile { get; }

        // model for our preferences
        public Preferences Preferences { get; }

        public MainWindow()
        {
            PreferencesFile = new SettingsManager(Path.Combine(ConfigManager.DocumentPath, "ControlCenterPreferences.xml"))
            {
                Writable = true
            };
            Preferences = new Preferences(PreferencesFile); 
            InitializeComponent();

            // this is gross, but correct:  this time is before any read access to settings, meaning before we load them
            SettingsLoadTime = DateTime.Now;
            
            // starting
            ConfigManager.LogManager.LogDebug("Control Center initializing");

            StatusMessage = StatusValue.RunningVersion;

            if (Preferences.SplashScreen)
            {
                // Display a dynamic splash panel with release and credits
                DisplaySplash(4000);
            }
            else
            {
                // add the list of authors and contributors to the console instead, without adding to log
                StatusViewer.AddItem(new StatusReportItem {
                    Status = $"Authors: {string.Join(", ", About.Authors)}",
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate });
                StatusViewer.AddItem(new StatusReportItem
                {
                    Status = $"Contributors: {string.Join(", ", About.Contributors)}",
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                });
            }

            LoadProfileList(PreferencesFile.LoadSetting("ControlCenter", "LastProfile", ""));
            if (_profileIndex == -1 && _profiles.Count > 0)
            {
                _profileIndex = 0;
            }
            if (_profileIndex > -1 && _profileIndex < _profiles.Count)
            {
                SelectedProfileName = System.IO.Path.GetFileNameWithoutExtension(_profiles[_profileIndex]);
            }
            else
            {
                SelectedProfileName = "- No Profiles Available -";
            }

            // fix up UI state that isn't implemented as bindings
            TouchscreenCheckBox.IsChecked = (Preferences.SuppressMouseAfterTouchDuration > 0);
            try
            {
                RegistryKey pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                using (pathKey)
                {
                    if (pathKey.GetValue("Helios") != null)
                    {
                        Preferences.AutoStart = true;
                    }
                    pathKey.Close();
                }
            }
            catch (Exception e)
            {
                AutoStartCheckBox.IsChecked = false;
                AutoStartCheckBox.IsEnabled = false;
                AutoStartCheckBox.ToolTip = "Unable to read/write registry to enable auto start.  Control Center may require administrator rights for this feature.";
                ConfigManager.LogManager.LogError("Error checking for auto start.", e);
            }
        }

        private void DisplaySplash(Int32 splashDelay)
        {
            About aboutDialog = new About();
            aboutDialog.InitializeComponent();
            aboutDialog.Show();
            System.Threading.Thread.Sleep(splashDelay);
            aboutDialog.Close();
        }

        private void ComposeStatusMessage(string firstLine, Uri _)
        {
            string message = firstLine;
            if (_lastProfileHint.Length > 0)
            {
                message += $"\nSimulator is {_lastProfileHint}";
            }
            else
            {
                message += "\n";
            }
            if (_lastDriverStatus.Length > 0)
            {
                message += $"\nSimulator is running {_lastDriverStatus}";
            }
            Message = message;
        }

        private void ComposeErrorMessage(string status, string recommendation, Uri _)
        {
            // this multiline message does not combine with anything
            Message = $"{status} {recommendation}";
        }

        private void ComposePopupMessage(string message)
        {
            Message = message;
        }

        private void ReportStatus(string status, Uri link = null) {
            if (status.Length < 1)
            {
                return;
            }

            StatusReportItem statusReportItem = new StatusReportItem()
            {
                Status = status,
                Link = link
            };
            StatusViewer.AddItem(statusReportItem);
            statusReportItem.Log(ConfigManager.LogManager);
        }

        private void ReportError(string status, string recommendation, Uri link)
        {
            StatusReportItem statusReportItem = new StatusReportItem()
            {
                Severity = StatusReportItem.SeverityCode.Error,
                Status = status,
                Recommendation = recommendation,
                Link = link
            };
            StatusViewer.AddItem(statusReportItem);
            statusReportItem.Log(ConfigManager.LogManager);
        }

        // helper
        private void UpdateStatusMessage()
        {
            UpdateStatusMessage(ComposeStatusMessage, ComposeErrorMessage, ComposePopupMessage);
        }

        // helper
        private void ReportStatusToStatusViewer()
        {
            UpdateStatusMessage(ReportStatus, ReportError, null);
        }

        private void UpdateStatusMessage(Action<string, Uri> updateInfoStatus, Action<string, string, Uri> updateErrorStatus, Action<string> updatePopup)
        {
            // centralize all these messages, because they all need to fit in the same space
            switch (_status)
            {
                case StatusValue.Empty:
                // fall through
                case StatusValue.License:
                    updateInfoStatus("", null);
                    break;
                case StatusValue.Running:
                    updateInfoStatus("Running Profile", null);
                    break;
                case StatusValue.Loading:
                    updateInfoStatus("Loading Profile...", null);
                    break;
                case StatusValue.LoadError:
                    updateInfoStatus("Error loading Profile", null);
                    break;
                case StatusValue.RunningVersion:
                    Version ver = RunningVersion.FromHeliosAssembly();
                    string message =
                        $"Helios {ver.Major}.{ver.Minor}.{ver.Build:0000}.{ver.Revision:0000}";
                    message += $"\n{KnownLinks.GitRepoUrl() ?? "HeliosVirtualCockpit fork"}";
                    updateInfoStatus(message, new Uri(KnownLinks.GitRepoUrl()));
                    break;
                case StatusValue.ProfileVersionHigher:
                    updateErrorStatus(
                        "Cannot display this profile because it was created with a newer version of Helios.",
                        "Please upgrade to the latest version.",
                        null);
                    break;
                case StatusValue.BadMonitorConfig:
                    updateErrorStatus(
                        "Cannot display this profile because it has an invalid monitor configuration.",
                        "Select reset monitors from the profile menu.",
                        StatusReportItem.ProfileEditor);
                    break;
                case StatusValue.FailedPreflight:
                    updateErrorStatus(
                        "Failed preflight check.",
                        "Please resolve problems or disable preflight check in preferences.",
                        null
                    );
                    break;
                case StatusValue.UpgradeAvailable:
                    updateInfoStatus("New Helios version available; check Console for details", null);
                    break;
            }
        }


        #region Properties

        public HeliosProfile ActiveProfile
        {
            get { return (HeliosProfile)GetValue(ActiveProfileProperty); }
            set
            {
                if (ActiveProfile == value)
                {
                    return;
                }
                ActiveProfile?.Unload();
                SetValue(ActiveProfileProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ActiveProfile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveProfileProperty =
            DependencyProperty.Register("ActiveProfile", typeof(HeliosProfile), typeof(MainWindow), new UIPropertyMetadata(null));

        public string SelectedProfileName
        {
            get { return (string)GetValue(SelectedProfileNameProperty); }
            set { SetValue(SelectedProfileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedProfileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedProfileNameProperty =
            DependencyProperty.Register("SelectedProfileName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public enum StatusValue
        {
            Empty,
            License, // Helios used to display a license; this case is here in case we need to put that back
            Running,
            Loading,
            LoadError,
            RunningVersion,
            ProfileVersionHigher,
            BadMonitorConfig,
            FailedPreflight,
            UpgradeAvailable
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public StatusValue StatusMessage
        {
            get { return _status; }
            set {
                _status = value;
                UpdateStatusMessage();
                ReportStatusToStatusViewer();
            }
        }

        public string HotKeyDescription
        {
            get { return (string)GetValue(HotKeyDescriptionProperty); }
            set { SetValue(HotKeyDescriptionProperty, value); }
        }
        public static readonly DependencyProperty HotKeyDescriptionProperty =
            DependencyProperty.Register("HotKeyDescription", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        // no need for dependency property, this is constant
        public StatusViewer.StatusViewer StatusViewer { get; } = new StatusViewer.StatusViewer();

        #endregion

        private void MinimizeWindow()
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        internal void ShowNormalWindow()
        {
            PowerButton.IsChecked = true;
            WindowState = System.Windows.WindowState.Normal;
            if (_helper != null)
            {
                NativeMethods.BringWindowToTop(_helper.Handle);
            }
        }

        #region Commands

        private void RunProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string profileToLoad = e.Parameter as string;
            if (profileToLoad == null || !File.Exists(profileToLoad))
            {
                NativeMethods.BringWindowToTop(_helper.Handle);
            }
            else
            {
                if (ActiveProfile != null)
                {
                    if (ActiveProfile.IsStarted)
                    {
                        ActiveProfile.Stop();
                    }
                    ActiveProfile = null;
                }

                // try to load the profile, setting SelectedProfileName in the process, and reusing cached images (if enabled) so we can fast switch
                SettingsLoadTime = DateTime.Now;
                (ConfigManager.SettingsManager as ISettingsManager2)?.SynchronizeSettings(null);
                LoadProfile(profileToLoad, false);

                if (ActiveProfile != null)
                {
                    // we need to fix up the selection index or Next/Prev buttons will not work correctly
                    // because LoadProfile sets SelectedProfileName but assumes the prev/next has already been set
                    // by user interaction.  however, we selected this profile without user interaction
                    LoadProfileList(profileToLoad);
                }

                StartProfile();
            }
        }

        public void StartProfile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StartProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool settingsChanged = false;
            if (ConfigManager.SettingsManager is ISettingsManager2 settings)
            {
                DateTime checkTime = DateTime.Now;
                settingsChanged = settings.SynchronizeSettings(SettingsLoadTime);
                SettingsLoadTime = checkTime;
            }

            if (ActiveProfile != null)
            {
                if (ActiveProfile.IsStarted)
                {
                    if (!settingsChanged)
                    {
                        return;
                    }
                    ConfigManager.LogManager.LogInfo("Stopping profile due to external settings changes");
                    StopProfile();
                }

                StatusViewer.ResetCautionLight();
                if (settingsChanged || File.GetLastWriteTime(ActiveProfile.Path) > ActiveProfile.LoadTime)
                {
                    if (settingsChanged)
                    {
                        ConfigManager.LogManager.LogInfo("Reloading profile due to external settings changes");
                    }

                    // do the load, synchronous and starting from empty image cache, since this isn't an auto start
                    LoadProfile(ActiveProfile.Path, true);
                }
            }
            else if (_profileIndex >= 0)
            {
                StatusViewer.ResetCautionLight();

                // do the load, synchronous and starting from empty image cache, since this isn't an auto start
                LoadProfile(_profiles[_profileIndex], true);
            }
            
            StartProfile();
        }

        public DateTime? SettingsLoadTime { get; set; }

        public void StopProfile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StopProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StopProfile();
            StatusMessage = StatusValue.License;
        }

        public void ResetProfile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ResetProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetProfile();
            StatusViewer.ResetCautionLight();
            ReportStatus("Profile was manually reset.");
        }

        private void OpenControlCenter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowNormalWindow();
        }

        private void TogglePreferences_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _prefsShown = !_prefsShown;
            if (_prefsShown)
            {
                PreferencesCanvas.Visibility = System.Windows.Visibility.Visible;
                Height = 277 + 320;
            }
            else
            {
                PreferencesCanvas.Visibility = System.Windows.Visibility.Collapsed;
                Height = 277;
            }
        }

        private void PrevProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatusMessage = StatusValue.License;

            LoadProfileList();
            _profileIndex = _profileIndex == 0 ? _profiles.Count : _profileIndex;
            if (_profileIndex > 0 && _profiles.Count > 0)
            {
                if (ActiveProfile != null && ActiveProfile.IsStarted)
                {
                    StopProfile();
                }

                ActiveProfile = null;
                _profileIndex--;
                SelectedProfileName = System.IO.Path.GetFileNameWithoutExtension(_profiles[_profileIndex]);
            }
        }

        private void NextProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatusMessage = StatusValue.License;

            LoadProfileList();

            _profileIndex = _profileIndex == _profiles.Count -1 ? -1 : _profileIndex;

            if (_profileIndex < _profiles.Count - 1)
            {
                if (ActiveProfile != null && ActiveProfile.IsStarted)
                {
                    StopProfile();
                }

                ActiveProfile = null;
                _profileIndex++;
                SelectedProfileName = System.IO.Path.GetFileNameWithoutExtension(_profiles[_profileIndex]);
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // close
            Close();
        }

        private void ResetCaution_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatusViewer.ResetCautionLight();
        }


        private void DialogShowModal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // this is the source of the event, and we will resolve DataTemplate from their position
            FrameworkElement host = (FrameworkElement)e.OriginalSource;

            // crash if incorrect parameter type
            ShowModalParameter parameter = (ShowModalParameter)e.Parameter;

            // resolve the data template
            DataTemplate template = parameter.DataTemplate ?? (DataTemplate)host.TryFindResource(new DataTemplateKey(parameter.Content.GetType()));

            // display the dialog appropriate to the content
            Window generic = new DefaultDialogWindow
            {
                ContentTemplate = template,
                Content = parameter.Content
            };

            generic.ShowDialog();
        }

        #endregion

        #region Profile Running

        private void StartProfile()
        {
            if (ActiveProfile == null || ActiveProfile.IsStarted)
            {
                return;
            }
            
            if (Preferences.PreflightCheck)
            {
                if (!PerformReadyCheck())
                {
                    // this is already logged as an error because we set the StatusMessage to an error type
                    ConfigManager.LogManager.LogDebug("Aborted start up of Profile due to failed preflight check.");
                    return;
                }
            }
            else
            {
                ReportStatus("Preflight check is disabled.  Helios will not be able to ensure configuration is correct.");
            }

            ActiveProfile.ControlCenterShown += Profile_ShowControlCenter;
            ActiveProfile.ControlCenterHidden += Profile_HideControlCenter;
            ActiveProfile.ProfileStopped += Profile_ProfileStopped;
            ActiveProfile.ProfileHintReceived += Profile_ProfileHintReceived;
            ActiveProfile.DriverStatusReceived += Profile_DriverStatusReceived;
            ActiveProfile.ClientChanged += Profile_ClientChanged;

            ActiveProfile.Start();

            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 33);
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Start();

            foreach (Monitor monitor in ActiveProfile.Monitors)
            {
                try
                {
                    if (monitor.Children.Count > 0 || monitor.FillBackground || !String.IsNullOrWhiteSpace(monitor.BackgroundImage))
                    {
                        monitor.SuppressMouseAfterTouchDuration = Preferences.SuppressMouseAfterTouchDuration;
                        ConfigManager.LogManager.LogDebug("Creating window (Monitor=\"" + monitor.Name + "\")" + " with touchscreen mouse event suppression delay set to " + Convert.ToString(monitor.SuppressMouseAfterTouchDuration) + " msec.");
                        MonitorWindow window = new MonitorWindow(monitor, Preferences.HighQualityBitmapScaling, true);
                        window.Show();
                        _windows.Add(window);
                    }
                }
                catch (Exception ex)
                {
                    ConfigManager.LogManager.LogError("Error creating monitor window (Monitor=\"" + monitor.Name + "\")", ex);
                }
            }

            // success, consider this the most recently run profile for its tags (usually just the aircraft types supported)
            foreach (string tag in ActiveProfile.Tags)
            {
                PreferencesFile.SaveSetting("RecentByTag", tag, ActiveProfile.Path);
            }

            StatusMessage = StatusValue.Running;

            if (Preferences.AutoHide)
            {
                MinimizeWindow();
            }
            else
            {
                NativeMethods.BringWindowToTop(_helper.Handle);
            }
        }

        private static readonly Dictionary<string, string> DriverDescriptions = new Dictionary<string, string>
        {
            { "HeliosDriver16", "Helios driver"},
            { "CaptZeenModule1", "module by Capt Zeen"},
            { "TelemetryOnly", "basic telemetry only"}
        };

        private void Profile_DriverStatusReceived(object sender, DriverStatus e)
        {
            string oldValue = _lastDriverStatus;
            if (!DriverDescriptions.TryGetValue(e.DriverType, out _lastDriverStatus))
            {
                // just use the raw value
                _lastDriverStatus = e.DriverType;
            }
            ConfigManager.LogManager.LogDebug($"received profile status indicating that simulator is running exports for '{e.DriverType}'");
            if (oldValue != _lastDriverStatus)
            {
                UpdateStatusMessage();
                // send only a simple status message to status viewer instead of whole status via ReportStatusToStatusViewer
                ReportStatus($"Simulator is running {_lastDriverStatus}");
            }
        }

        private void Profile_ProfileHintReceived(object sender, ProfileHint e)
        {
            string oldValue = _lastProfileHint;
            _lastProfileHint = e.Tag;
            if (oldValue != _lastProfileHint)
            {
                UpdateStatusMessage();
                // send only a simple status message to status viewer instead of whole status via ReportStatusToStatusViewer
                ReportStatus($"Simulator is '{_lastProfileHint}'");
            }
            if (!Preferences.ProfileAutoStart)
            {
                return;
            }
            ConfigManager.LogManager.LogDebug($"received profile hint with tag '{e.Tag}'");
            string mostRecent = PreferencesFile.LoadSetting("RecentByTag", e.Tag, null);
            if (mostRecent == null)
            {
                ConfigManager.LogManager.LogInfo($"received profile hint with tag '{e.Tag}' but no matching profile has been loaded; cannot auto load");
                return;
            }
            if ((ActiveProfile != null) && (ActiveProfile.Path == mostRecent))
            {
                ConfigManager.LogManager.LogDebug($"most recent profile for profile hint with tag '{e.Tag}' is already active");
                // ask simulator to use the one we are running, if possible
                ActiveProfile.RequestProfileSupport();
                return;
            }
            // execute auto load
            ConfigManager.LogManager.LogDebug($"trying to start most recent matching profile '{mostRecent}'");
            ControlCenterCommands.RunProfile.Execute(mostRecent, Application.Current.MainWindow);
        }

        private void Profile_ClientChanged(object sender, ClientChange e)
        {
            if (e.FromOpaqueHandle != ClientChange.NO_CLIENT)
            {
                // this is a change during our profile's lifetime, so we need to discard information we have
                _lastProfileHint = "";
            }
            _lastDriverStatus = "";
        }

        private void StopProfile()
        {
            if (ActiveProfile != null && ActiveProfile.IsStarted)
            {
                ActiveProfile.Stop();
            }
        }

        private void ResetProfile()
        {
            if (ActiveProfile != null)
            {
                ActiveProfile.Reset();
                if (ActiveProfile.IsStarted)
                {
                    StatusMessage = StatusValue.Running;
                    return;
                }
            }
            StatusMessage = StatusValue.RunningVersion;
        }

        private void Profile_ShowControlCenter(object sender, EventArgs e)
        {
            ShowNormalWindow();
        }

        private void Profile_HideControlCenter(object sender, EventArgs e)
        {
            MinimizeWindow();
        }

        void Profile_ProfileStopped(object sender, EventArgs e)
        {
            foreach (MonitorWindow window in _windows)
            {
                window.Close();
            }

            _windows.Clear();

            HeliosProfile profile = sender as HeliosProfile;
            if (profile != null)
            {
                profile.ControlCenterShown -= Profile_ShowControlCenter;
                profile.ControlCenterHidden -= Profile_HideControlCenter;
                profile.ProfileStopped -= Profile_ProfileStopped;
                profile.ProfileHintReceived -= Profile_ProfileHintReceived;
                profile.DriverStatusReceived -= Profile_DriverStatusReceived;
                profile.ClientChanged -= Profile_ClientChanged;
            }

            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
                _dispatcherTimer = null;
            }

            StatusMessage = StatusValue.License;

            //EGalaxTouch.ReleaseTouchScreens();
        }

        void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (ActiveProfile != null)
            {
                try
                {
                    ActiveProfile.Tick();
                    long currentTick = DateTime.Now.Ticks;
                    if (currentTick - _lastTick > TOPMOST_TICK_COUNT)
                    {
                        for (int i = 0; i < _windows.Count; i++)
                        {
                            if (_windows[i].Monitor.AlwaysOnTop)
                            {
                                NativeMethods.SetWindowPos(_windows[i].Handle, HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                            }
                        }
                        NativeMethods.SetWindowPos(_helper.Handle, HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                        _lastTick = currentTick;
                    }
                }
                catch (Exception exception)
                {
                    ConfigManager.LogManager.LogError("Error processing profile tick or refresh.", exception);
                }
            }
        }

        #endregion

        #region Profile Persistance

        private void LoadProfile(string path, bool dropImageCache)
        {
            StatusMessage = StatusValue.Loading;

            // pump UI events to update UI (NOTE: we are the main thread)
            Dispatcher.Invoke(DispatcherPriority.Loaded, (Action)delegate { });

            // configure ImageManager for this load
            if (ConfigManager.ImageManager is IImageManager4 cacheCapable) {
                cacheCapable.CacheObjects = Preferences.CacheImages;
                if (Preferences.CacheImages && dropImageCache)
                {
                    // start with an empty cache, becase this is not a fast switch
                    cacheCapable.DropObjectCache();
                }
            }

            // now do the load that might take a while
            ActiveProfile = ConfigManager.ProfileManager.LoadProfile(path);

            if (ActiveProfile != null)
            {
                NativeMethods.SHAddToRecentDocs(0x00000003, path);

                SelectedProfileName = System.IO.Path.GetFileNameWithoutExtension(ActiveProfile.Path);
#if !DEBUG
                if (!ActiveProfile.IsValidMonitorLayout)
                {
                    StatusMessage = StatusValue.BadMonitorConfig;
                    ActiveProfile = null;
                    return;
                }
#endif

                if (ActiveProfile.IsInvalidVersion)
                {
                    StatusMessage = StatusValue.ProfileVersionHigher;
                    ActiveProfile = null;
                    return;
                }
            }
            else
            {
                StatusMessage = StatusValue.LoadError;
            }
        }

        private void LoadProfileList()
        {
            string currentProfilePath = "";
            if (_profileIndex >= 0 && _profileIndex < _profiles.Count)
            {
                currentProfilePath = _profiles[_profileIndex];
            }
            LoadProfileList(currentProfilePath);
        }

        /// <summary>
        /// reload the list of profiles on disk and set the current selected index if
        /// the given path references a file that still exists
        /// </summary>
        /// <param name="currentProfilePath"></param>
        private void LoadProfileList(string currentProfilePath)
        {
            _profileIndex = -1;
            _profiles.Clear();

            IEnumerable<string> profilePaths = Directory.GetFiles(ConfigManager.ProfilePath, "*.hpf");
            string secondaryDirectory =
                ConfigManager.SettingsManager.LoadSetting("Helios", "SecondaryProfileDirectory", null);
            if (null != secondaryDirectory)
            {
                profilePaths = profilePaths.Concat(Directory.GetFiles(secondaryDirectory, "*.hpf"));
            }
            foreach (string path in profilePaths)
            {
                if (currentProfilePath != null && path.Equals(currentProfilePath))
                {
                    _profileIndex = _profiles.Count;
                    SelectedProfileName = System.IO.Path.GetFileNameWithoutExtension(path);
                }
                _profiles.Add(path);
            }
        }

        private bool PerformReadyCheck()
        {
            bool success = true;
            foreach (StatusReportItem status in ActiveProfile.PerformReadyCheck())
            {
                if (status.Severity == StatusReportItem.SeverityCode.Error)
                {
                    success = false;
                }
                StatusViewer.AddItem(status);
                status.Log(ConfigManager.LogManager);
            }
            StatusCanvas.StatusLines?.ScrollToBottom();
            if (!success)
            {
                StatusMessage = StatusValue.FailedPreflight;

                // preflight only:  we automatically pop up the status view
                StatusCheckBox.IsChecked = true;
            }
            return success;
        }

        #endregion

        #region Windows Event Handlers

        private void MoveThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (e.Source is Slider)
            {
                // don't move the whole window
                return;
            }
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }

        private void PowerButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DispatcherTimer minimizeTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 250), DispatcherPriority.Normal, TimedMinimize, Dispatcher);
        }

        private void TimedMinimize(object sender, EventArgs e)
        {
            ((DispatcherTimer) sender).Stop();
            Dispatcher.Invoke(new Action(Close), null);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            bool loaded = ActiveProfile != null;
            if (loaded)
            {
                if (ActiveProfile.IsStarted)
                {
                    ConfigManager.LogManager.LogInfo("Stopping profile on exit.");
                    ActiveProfile.Stop();
                }
                ConfigManager.LogManager.LogInfo("Unloading profile on exit.");
                ActiveProfile = null;
            }

            ConfigManager.LogManager.LogInfo("Saving control center window position.");

            // Persist window placement details to application settings
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.GetWindowPlacement(hwnd, out wp);

            //Properties.PreferencesFile.Default.ControlCenterPlacement = wp;
            PreferencesFile.SaveSetting("ControlCenter", "WindowLocation", wp.normalPosition);

            if (loaded && _profileIndex >= 0 && _profileIndex < _profiles.Count)
            {
                PreferencesFile.SaveSetting("ControlCenter", "LastProfile", _profiles[_profileIndex]);
            }

            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_hotkey != null)
            {
                _hotkey.UnregisterHotKey();
                _hotkey.Dispose();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            try
            {
                // Load window placement details for previous application session from application settings
                // Note - if window was closed on a monitor that is now disconnected from the computer,
                //        SetWindowPlacement will place the window onto a visible monitor.

                if (PreferencesFile.IsSettingAvailable("ControlCenter", "WindowLocation"))
                {
                    WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
                    wp.normalPosition = PreferencesFile.LoadSetting("ControlCenter", "WindowLocation",
                        new RECT(0, 0, (int) Width, (int) Height));
                    wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                    wp.flags = 0;
                    wp.showCmd = (wp.showCmd == NativeMethods.SW_SHOWMINIMIZED
                        ? NativeMethods.SW_SHOWNORMAL
                        : wp.showCmd);
                    IntPtr hwnd = new WindowInteropHelper(this).Handle;
                    NativeMethods.SetWindowPlacement(hwnd, ref wp);
                }

                if (!Enum.TryParse(Preferences.HotKeyModifiers, out ModifierKeys mods))
                {
                    HotKeyDescription = "None";
                    return;
                }
                if (!Enum.TryParse(Preferences.HotKey, out Keys hotKey))
                {
                    HotKeyDescription = "None";
                    return;
                }
                if (hotKey != Keys.None)
                {
                    _hotkey = new HotKey(mods, hotKey, this);
                    _hotkey.HotKeyPressed += new Action<HotKey>(HotKeyPressed);
                    HotKeyDescription = KeyboardEmulator.ModifierKeysToString(_hotkey.KeyModifier) +
                                        _hotkey.Key.ToString();
                }
                else
                {
                    HotKeyDescription = "None";
                }
            }
            catch (System.Exception ex)
            {
                ConfigManager.LogManager.LogError("exception thrown during hotkey initialization", ex);
                RemoveHotkey();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the window style to noactivate.
            _helper = new WindowInteropHelper(this);

            NativeMethods.SetWindowLong(_helper.Handle,
                NativeMethods.GWL_EXSTYLE,
                NativeMethods.GetWindowLong(_helper.Handle, NativeMethods.GWL_EXSTYLE) | NativeMethods.WS_EX_NOACTIVATE);

            if (Preferences.StartMinimized)
            {
                MinimizeWindow();
            }
            else
            {
                ShowNormalWindow();
            }
        }

        private void Window_Opened(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major > 5 && PreferencesFile.LoadSetting("ControlCenter", "AeroWarning", true))
            {
                bool aeroEnabled;
                NativeMethods.DwmIsCompositionEnabled(out aeroEnabled);
                if (!aeroEnabled)
                {
                    AeroWarning warningDialog = new AeroWarning();
                    warningDialog.Owner = this;
                    warningDialog.ShowDialog();

                    if (warningDialog.DisplayAgainCheckbox.IsChecked == true)
                    {
                        PreferencesFile.SaveSetting("ControlCenter", "AeroWarning", false);
                    }
                }
            }


            App app = Application.Current as App;
            if (app != null && app.StartupProfile != null)
            {
                string primaryProfilePath = ConfigManager.ProfilePath;
                string secondaryProfilePath = ConfigManager.SettingsManager.LoadSetting("Helios", "SecondaryProfileDirectory", null);
                string startupProfile = File.Exists(app.StartupProfile) ? app.StartupProfile :
                    (File.Exists(Path.Combine(primaryProfilePath, app.StartupProfile)) ? Path.Combine(primaryProfilePath, app.StartupProfile) :
                    (File.Exists(Path.Combine(secondaryProfilePath, app.StartupProfile)) ? Path.Combine(secondaryProfilePath, app.StartupProfile) : ""));
                if(startupProfile != "")
                {
                    LoadProfileList(startupProfile);
                    LoadProfile(startupProfile, true);
                    StartProfile();
                }
            }

            VersionChecker.Check versionCheck = ConfigManager.VersionChecker.CheckAvailableVersion(false);
            if (!ConfigManager.VersionChecker.ShouldOfferNewVersion(versionCheck))
            {
                return;
            }

            // can't use the interactive dialog, because this would require writing settings
            StatusMessage = StatusValue.UpgradeAvailable;

            StatusReportItem item = new StatusReportItem
            {
                Status = $"new version {versionCheck.AvailableVersion} is available",
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
            if (versionCheck.DownloadUrl != null)
            {
                item.Link = new Uri(versionCheck.DownloadUrl);
                item.Recommendation = $"Use Profile Editor to check for new version or download directly from {versionCheck.DownloadUrl}";
            }
            else
            {
                item.Link = StatusReportItem.ProfileEditor;
                item.Recommendation = "check for new version and download it";
            }
            StatusViewer.AddItem(item);
        }

        private void HideButton_Clicked(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
        }

        #endregion

        #region Preferences

        private void TouchscreenCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Preferences.SuppressMouseAfterTouchDuration = 0;
        }

        private void SetHotkey_Click(object sender, RoutedEventArgs e)
        {
            HotKeyDetector detector = new HotKeyDetector();
            detector.Owner = this;
            detector.ShowDialog();

            if (_hotkey != null)
            {
                _hotkey.UnregisterHotKey();
                _hotkey.Dispose();
            }

            _hotkey = new HotKey(detector.Modifiers, detector.Key, _helper.Handle);
            _hotkey.HotKeyPressed += HotKeyPressed;

            Preferences.HotKeyModifiers = detector.Modifiers.ToString();
            Preferences.HotKey = detector.Key.ToString();
            HotKeyDescription = KeyboardEmulator.ModifierKeysToString(_hotkey.KeyModifier) + _hotkey.Key.ToString();
        }

        private void ClearHotkey_Click(object sender, RoutedEventArgs e)
        {
            RemoveHotkey();
        }

        private void RemoveHotkey()
        {
            if (_hotkey != null)
            {
                _hotkey.HotKeyPressed -= HotKeyPressed;
                _hotkey.UnregisterHotKey();
                _hotkey.Dispose();
            }

            Preferences.HotKeyModifiers = ModifierKeys.None.ToString();
            Preferences.HotKey = Keys.None.ToString();
            HotKeyDescription = "None";
        }

        #endregion

        #region Hotkey Handling

        void HotKeyPressed(HotKey obj)
        {
            ShowNormalWindow();
        }

        #endregion
    }
}
