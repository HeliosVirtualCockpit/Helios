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

using System.Collections.ObjectModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Common;
using GadrocsWorkshop.Helios.ProfileEditor.ArchiveInstall;
using GadrocsWorkshop.Helios.ProfileEditor.ViewModel;
using GadrocsWorkshop.Helios.Tools;
using GadrocsWorkshop.Helios.Tools.Capabilities;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Windows;

namespace GadrocsWorkshop.Helios.ProfileEditor
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.ProfileEditor.UndoEvents;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using GadrocsWorkshop.Helios.Windows.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using AvalonDock.Layout;
    using AvalonDock.Layout.Serialization;
    using System.CodeDom;
    using System.ServiceModel.Channels;
    using System.Windows.Forms.ComponentModel.Com2Interop;
    using System.Windows.Media;


    /// <summary>
    /// we may refactor profile loading into here if we can untangle it enough
    /// for now, it just hosts an extra logger with a civilized name
    /// </summary>
    internal class ProfileLoader
    {
        internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ResetMonitorsWork.ICallbacks
    { 
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Internal Class used to track open documents
        /// </summary>
        private class DocumentMeta
        {
            public HeliosObject hobj;
            public LayoutDocument document;
            public HeliosEditorDocument editor;
        }

        private delegate void LayoutDelegate(string filename);

        private readonly XmlLayoutSerializer _layoutSerializer;
        private string _systemDefaultLayout;
        private readonly string _defaultLayoutFile;

        private LoadingAdorner _loadingAdorner;
        private readonly List<DocumentMeta> _documents = new List<DocumentMeta>();

        private readonly InterfaceStatusScanner _configurationCheck = new InterfaceStatusScanner();

        /// <summary>
        /// true if trigger was fired since profile was changed
        /// </summary>
        private bool _triggered;

        /// <summary>
        /// true if initial load (first Activation) has been done
        /// </summary>
        private bool _loaded;

        /// <summary>
        /// true if Closing event has been raised
        /// </summary>
        private bool _closing;

        /// <summary>
        /// true if there are images which have changed and need to be checked
        /// </summary>

        private bool _changedImages = false;

        /// <summary>
        /// all plugin tools, indexed by their descriptor Id
        /// </summary>
        private Dictionary<string, object> _tools = new Dictionary<string, object>();

        private FileSystemWatcher _imageFileWatcher;

        private List<string> _processedChangedImages = new List<string>();

        public MainWindow()
        {
            if (!HeliosInit.HeliosInitSuccess)
            {
                this.Close();
                return;
            }
            InitializeComponent();

            ToolMenuItems = new ObservableCollection<MenuItemModel>();
            ProfileEditorCommands.AddInterface.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
            LoadTools();

            DataContext = this;
            InterfaceStatusPanel.DataContext = new InterfaceStatusViewModel(_configurationCheck);
            DockManager.ActiveContentChanged += DockManager_ActiveDocumentChanged;
            NewProfile();

            _layoutSerializer = new XmlLayoutSerializer(DockManager);
            _layoutSerializer.LayoutSerializationCallback += LayoutSerializer_LayoutSerializationCallback;

            _defaultLayoutFile = Path.Combine(ConfigManager.DocumentPath, "DefaultLayout.hply");

            // arm this to tell us if any interface reports status above a configured threshold
            _configurationCheck.Triggered += ConfigurationCheck_Triggered;

            // on finish loading, check version
            Loaded += PresentVersionCheck;
        }
        
        /// <summary>
        /// load any HeliosTool items discovered in our modules
        /// </summary>
        private void LoadTools()
        {
            // first add our hard coded items
            ToolMenuItems.Add(new MenuItemModel("Save Template", ProfileEditorCommands.SaveTemplate));
            ToolMenuItems.Add(new MenuItemModel("Template _Manager", new RelayCommand(parameter =>
            {
                TemplateManagerWindow tm = new TemplateManagerWindow { Owner = this };
                tm.ShowDialog();
            })));

            if (!(ConfigManager.ModuleManager is IModuleManager2 modules))
            {
                return;
            }

            foreach (HeliosToolDescriptor descriptor in modules.Tools)
            {
                _tools[descriptor.Id] = descriptor.CreateInstance();
            }

            foreach (IMenuSectionFactory factory in _tools.Values.OfType<IMenuSectionFactory>())
            {
                // our WPF implementation does not do well with nested menus so we flatten everything
                foreach (MenuItemModel menuItemModel in factory.CreateMenuSection().Items)
                {
                    ToolMenuItems.Add(menuItemModel);
                }
            }

            // finally add hard coded items intended to be last
            ToolMenuItems.Add(new MenuItemModel("Options", new RelayCommand(parameter =>
            {
                GlobalOptionsWindow options = new GlobalOptionsWindow
                {
                    DataContext = new ProfileEditor.GlobalOptions()
                };
                options.ShowDialog();
            })));

        }

        private static void PresentVersionCheck(object sender, RoutedEventArgs e)
        {
            // NOTE: don't open version check before main window is loaded, otherwise closing the dialog will kill the application since it will be the only window
            ConfigManager.VersionChecker.CheckAvailableVersionsWithDialog(false, false);

            ((MainWindow)sender).Loaded -= PresentVersionCheck;
        }

        void LayoutSerializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            if (e.Model.ContentId == Welcome.CONTENT_ID)
            {
                Welcome.Create(e.Model as LayoutAnchorable, out object content);
                e.Content = content;
                return;
            }

            if (!(e.Model is LayoutDocument layoutDocument))
            {
                // code below here just deserializes documents such as monitors and panels
                return;
            }

            HeliosObject profileObject = HeliosSerializer.ResolveReferenceName(Profile, layoutDocument.ContentId);
            if (profileObject != null)
            {
                HeliosEditorDocument editor = CreateDocumentEditor(profileObject);
                if (editor == null)
                {
                    // this component used to have an editor and now it does not, or more likely this is
                    // an UnsupportedInterface and therefore has no editor
                    Logger.Debug("Layout Serializer: Document {ContentId} does not have an editor; ignored", layoutDocument.ContentId);
                    return;
                }
                profileObject.PropertyChanged += DocumentObject_PropertyChanged;
                e.Content = CreateDocumentContent(editor);
                //DocumentPane.Children.Add((LayoutDocument)e.Model);
                layoutDocument.Closed += Document_Closed;
                AddDocumentMeta(profileObject, layoutDocument, editor);
            } else
            {
                Logger.Debug("Layout Serializer: Unable to resolve Layout Document " + layoutDocument.ContentId);
            }
        }

        #region Properties

        public HeliosProfile Profile
        {
            get { return (HeliosProfile)GetValue(ProfileProperty); }
            set
            {
                OnProfileChange();
                SetValue(ProfileProperty, value);
            }
        }

        public static readonly DependencyProperty ProfileProperty =
            DependencyProperty.Register("Profile", typeof(HeliosProfile), typeof(MainWindow), new PropertyMetadata(null));

        public int Monitor
        {
            get { return (int)GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }

        public static readonly DependencyProperty MonitorProperty =
            DependencyProperty.Register("Monitor", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public string StatusBarMessage
        {
            get { return (string)GetValue(StatusBarMessageProperty); }
            set { SetValue(StatusBarMessageProperty, value); }
        }

        public static readonly DependencyProperty StatusBarMessageProperty =
            DependencyProperty.Register("StatusBarMessage", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public HeliosEditorDocument CurrentEditor
        {
            get { return (HeliosEditorDocument)GetValue(CurrentEditorProperty); }
            set { SetValue(CurrentEditorProperty, value); }
        }

        public static readonly DependencyProperty CurrentEditorProperty =
            DependencyProperty.Register("CurrentEditor", typeof(HeliosEditorDocument), typeof(MainWindow), new PropertyMetadata(null));

        public ObservableCollection<MenuItemModel> ToolMenuItems
        {
            get => (ObservableCollection<MenuItemModel>) GetValue(ToolMenuSectionsProperty);
            set => SetValue(ToolMenuSectionsProperty, value);
        }

        public static readonly DependencyProperty ToolMenuSectionsProperty = DependencyProperty.Register("ToolMenuItems", typeof(ObservableCollection<MenuItemModel>), typeof(MainWindow), new PropertyMetadata(null));

        #endregion

        #region Event Handlers

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // do this once
            if (!_changedImages && _loaded)
            {
                return;
            }
            _loaded = true;

            if (!(Application.Current is App profileEditor))
            {
                // not running in our expected Profile Editor application
                return;
            }

            _processedChangedImages.Clear();
            if (_changedImages && ConfigManager.ImageManager.ChangedImages.Count() > 0)
            {
                if(ConfigManager.ImageManager.ChangedImages.Count() <= 3) // We only process small numbers of changed images
                {
                    foreach (string imageName in ConfigManager.ImageManager.ChangedImages)
                    {
                        foreach (Monitor monitor in Profile.Monitors)
                        {
                            monitor.ConditionalImageRefresh(imageName);
                            monitor.ImageRefresh = false;
                            ProcessVisualChildren(monitor.Children, imageName);
                        }
                        if(!_processedChangedImages.Contains(imageName)) _processedChangedImages.Add(imageName);
                    }
                    foreach (string ProcessedImageName in _processedChangedImages)
                    {
                        ConfigManager.ImageManager.ChangedImages.Remove(ProcessedImageName);
                    }
                    _changedImages = ConfigManager.ImageManager.ChangedImages.Count() == 0 ? false : true;
                }
                else
                {
                    // Ignore large numbers of changed images.
                    _changedImages = false;
                    ConfigManager.ImageManager.ChangedImages.Clear();
                }
                return;
            }

            string primaryProfilePath = ConfigManager.ProfilePath;
            string secondaryProfilePath = ConfigManager.SettingsManager.LoadSetting("Helios", "SecondaryProfileDirectory", null);
            string startupProfile = profileEditor.StartupFile;

            if(Path.GetExtension(startupProfile) == ".hpf")
            {
                startupProfile = File.Exists(startupProfile) ? startupProfile :
                  (File.Exists(Path.Combine(primaryProfilePath, startupProfile)) ? System.IO.Path.Combine(primaryProfilePath, startupProfile) :
                  (File.Exists(Path.Combine(secondaryProfilePath, startupProfile)) ? System.IO.Path.Combine(secondaryProfilePath, startupProfile) : ""));
            }

            if (profileEditor.StartupFile != null && File.Exists(startupProfile))
            {
                string extension = Path.GetExtension(startupProfile);
                switch (extension)
                {
                    case ".hpf":
                        LoadProfile(startupProfile);
                        break;
                    case ".helios16":
                        InstallArchive(startupProfile);
                        break;
                    default:
                        // ignore
                        Logger.Warn("Ignoring input file with unrecognized file extension {extension}", extension);
                        break;
                }
            }
        }

        void DockManager_ActiveDocumentChanged(object sender, EventArgs e)
        {
            // Interface Editor documents are embeded in a scrollviewer.  Unwrap them if they are
            // the current content.
            HeliosEditorDocument activeDocument = DockManager.ActiveContent is ScrollViewer viewer
                ? viewer.Content as HeliosEditorDocument
                : DockManager.ActiveContent as HeliosEditorDocument;

            if (activeDocument == null)
            {
                return;
            }

            if (activeDocument != CurrentEditor)
            {
                // this switches all of our UI panels to show content for this editor, because of XAML bindings to this property
                CurrentEditor = activeDocument;

                // find selection-capable editor or null
                HeliosVisualContainerEditor editor = (activeDocument as PanelDocument)?.PanelEditor ??
                                                     (activeDocument as MonitorDocument)?.MonitorEditor;

                // also notify tools, which do not have a UI element that they could be binding
                foreach (ISelectionAwareTool tool in _tools.Values.OfType<ISelectionAwareTool>())
                {
                    tool.AttachToSelection(editor?.SelectedItems);
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ProfileProperty)
            {
                if (e.OldValue is HeliosProfile oldProfile)
                {
                    oldProfile.PropertyChanged -= new PropertyChangedEventHandler(Profile_PropertyChanged);
                }

                foreach (DocumentMeta meta in _documents.ToArray())
                {
                    meta.document.Close();
                    meta.hobj.PropertyChanged -= DocumentObject_PropertyChanged;
                }
                _documents.Clear();

                if (Profile != null)
                {
                    Profile.PropertyChanged += new PropertyChangedEventHandler(Profile_PropertyChanged);
                }
            }
            base.OnPropertyChanged(e);
        }

        void Profile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e is PropertyNotificationEventArgs args)
            {
                ConfigManager.UndoManager.AddPropertyChange(sender, args);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CheckSave())
            {
                e.Cancel = true;
            }

            if (e.Cancel == false)
            {
                Logger.Debug("Profile Editor main window has raised OnClosing and is going down");
                _closing = true;
                ConfigManager.UndoManager.ClearHistory();

                // Persist window placement details to application settings
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                NativeMethods.GetWindowPlacement(hwnd, out NativeMethods.WINDOWPLACEMENT wp);
                if (wp.normalPosition.Width > 0)
                { 
                    // if running non-interactive, our window size is 0 and we don't want to store that
                    ConfigManager.SettingsManager.SaveSetting("ProfileEditor", "WindowLocation", wp.normalPosition);
                }

                // Close all open documents so they can clean up
                while (_documents.Count > 0)
                {
                    _documents[0].document.Close();
                }

                // unload profile if present
                Profile = null;
            }

            base.OnClosing(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // Load window placement details for previous application session from application settings
            // Note - if window was closed on a monitor that is now disconnected from the computer,
            //        SetWindowPlacement will place the window onto a visible monitor.

            if (ConfigManager.SettingsManager.IsSettingAvailable("ProfileEditor", "WindowLocation"))
            {
                NativeMethods.WINDOWPLACEMENT wp = new NativeMethods.WINDOWPLACEMENT
                {
                    normalPosition = ConfigManager.SettingsManager.LoadSetting("ProfileEditor", "WindowLocation", new NativeMethods.RECT(0, 0, (int)Width, (int)Height)),
                    length = Marshal.SizeOf(typeof(NativeMethods.WINDOWPLACEMENT)),
                    flags = 0
                };
                wp.showCmd = (wp.showCmd == NativeMethods.SW_SHOWMINIMIZED ? NativeMethods.SW_SHOWNORMAL : wp.showCmd);
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                NativeMethods.SetWindowPlacement(hwnd, ref wp);
            }

            base.OnSourceInitialized(e);
        }

        private void OnImageFileChanged(object source, FileSystemEventArgs e)
        {
            if ((e.ChangeType.Equals(WatcherChangeTypes.Changed)|| e.ChangeType.Equals(WatcherChangeTypes.Renamed)) & (System.IO.Path.GetExtension(e.Name.ToUpper()) == ".PNG" | System.IO.Path.GetExtension(e.Name.ToUpper()) == ".JPG" | System.IO.Path.GetExtension(e.Name.ToUpper()) == ".JPEG"))
            {
                if (!ConfigManager.ImageManager.ChangedImages.Contains(e.Name.ToLower()))
                {
                    if (e.Name != null)
                    {
                        ConfigManager.ImageManager.ChangedImages.Add(e.Name.ToLower());
                        _changedImages = true;
                        Logger.Debug($@"File change detected for '{e.Name}';");
                    }
                    else
                    {
                        Logger.Debug($@"File change triggered but Name is null '{e}';");
                    }
                }
            }
        }
        #endregion

        #region Adorners

        private LoadingAdorner GetLoadingAdorner()
        {
            if (_loadingAdorner == null)
            {
                _loadingAdorner = new LoadingAdorner(PrimaryGrid);
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(PrimaryGrid);
                layer.Add(_loadingAdorner);
            }
            return _loadingAdorner;
        }

        private void RemoveLoadingAdorner()
        {
            if (_loadingAdorner != null)
            {
                _loadingAdorner.Visibility = Visibility.Hidden;
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(PrimaryGrid);
                layer.Remove(_loadingAdorner);
                _loadingAdorner = null;
            }
        }

        #endregion

        #region Helper Methods

        private DocumentMeta AddDocumentMeta(HeliosObject profileObject, LayoutDocument document, HeliosEditorDocument editor)
        {
            DocumentMeta meta = new DocumentMeta {editor = editor, document = document, hobj = profileObject};
            _documents.Add(meta);
            return meta;
        }

        private DocumentMeta FindDocumentMeta(HeliosObject profileObject)
        {
            return _documents.FirstOrDefault(meta => meta.hobj == profileObject);
        }

        private DocumentMeta FindDocumentMeta(LayoutDocument document)
        {
            return _documents.FirstOrDefault(meta => meta.document == document);
        }

        // XXX: investigate why this was here and is no longer used
        private DocumentMeta FindDocumentMeta(HeliosEditorDocument editor)
        {
            return _documents.FirstOrDefault(meta => meta.editor == editor);
        }

        private DocumentMeta AddNewDocument(HeliosObject profileObject)
        {
            DocumentMeta meta = FindDocumentMeta(profileObject);
            if (meta != null)
            {
                meta.document.IsActive = true;
                return meta;
            }


            HeliosEditorDocument editor = CreateDocumentEditor(profileObject);
            if (editor == null)
            {
                return null;
            }

            LayoutDocument document = new LayoutDocument
            {
                Title = editor.Title,
                ContentId = HeliosSerializer.GetReferenceName(profileObject),
                Content = CreateDocumentContent(editor)
            };

            // Since a new LayoutRoot object is created upon de-serialization, the Child LayoutDocumentPane no longer belongs to the LayoutRoot 
            // therefore the LayoutDocumentPane 'DocumentPane' must be referred to dynamically
            LayoutDocumentPane documentPane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            documentPane?.Children.Add(document);
            document.IsActive = true;
            document.Closed += Document_Closed;

            meta = AddDocumentMeta(profileObject, document, editor);
            profileObject.PropertyChanged += DocumentObject_PropertyChanged;
            
            return meta;
        }

        private HeliosEditorDocument CreateDocumentEditor(HeliosObject profileObject)
        {
            switch (profileObject)
            {
                case Monitor monitor:
                    return new MonitorDocument(monitor);
                case HeliosPanel panel:
                    return new PanelDocument(panel);
                case HeliosInterface heliosInterface:
                {
                    HeliosEditorDocument editor = ConfigManager.ModuleManager.CreateInterfaceEditor(heliosInterface, Profile);
                    if (editor != null)
                    {
                        editor.Style = Application.Current.Resources["InterfaceEditor"] as Style;
                    }
                    return editor;
                }
                default:
                    throw new ArgumentException(@"Cannot create a editor document for profileobject requested.", nameof(profileObject));
            }
        }

        private object CreateDocumentContent(HeliosEditorDocument editor)
        {
            if (editor.HandlesScroll)
            {
                return editor;
            }

            ScrollViewer scroller = new ScrollViewer
            {
                Content = editor,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            return scroller;
        }

        void Document_Closed(object sender, EventArgs e)
        {
            DocumentMeta meta = FindDocumentMeta(sender as LayoutDocument);

            if (meta == null)
            {
                throw new InvalidOperationException("Document closed called for a document not found in meta data.");
            }

            meta.editor.Closed();
            meta.hobj.PropertyChanged -= DocumentObject_PropertyChanged;
            meta.document.Closed -= Document_Closed;

            _documents.Remove(meta);
        }

        void DocumentObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DocumentMeta meta = FindDocumentMeta(sender as HeliosObject);
            if (meta == null)
            {
                throw new InvalidOperationException("Property Changed closed called for a profile object not found in meta data.");
            }

            if (e.PropertyName.Equals("Name"))
            {
                meta.document.Title = meta.editor.Title;
                meta.document.ContentId = "doc:" + HeliosSerializer.GetReferenceName(meta.hobj);
            }
        }

        #endregion

        #region Profile Persistance

        private void NewProfile()
        {
            if (CheckSave())
            {
                if (ConfigManager.ImageManager is IImageManager2 imageManager2)
                {
                    imageManager2.ClearFailureTracking();
                }
                WatchImages();  

                Profile = new HeliosProfile();

                // configure default layout options
                bool fillSecondaryMonitors = GlobalOptions.HasDefaultFillSecondaryMonitors;
                bool defaultAlwaysOnTop = GlobalOptions.HasDefaultAlwaysOnTop;
                foreach (Monitor monitor in Profile.Monitors)
                {
                    monitor.FillBackground = (!monitor.IsPrimaryDisplay) && fillSecondaryMonitors;
                    monitor.AlwaysOnTop = defaultAlwaysOnTop;
                }

                // any changes made before now are not considered changes any more
                ConfigManager.UndoManager.ClearHistory();

                AddNewDocument(Profile.Monitors[0]);
                OnProfileChangeComplete();

                // clean up memory
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private bool CheckSave()
        {
            if (!ConfigManager.UndoManager.CanUndo && (Profile == null || !Profile.IsDirty))
            {
                return true;
            }

            MessageBoxResult result = MessageBox.Show(this, "There are changes to the current profile.  If you continue without saving your changes, they will be lost.  Would you like to save the current profile?", "Save Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                return SaveProfile();
            }
            return (result != MessageBoxResult.Cancel);
        }

        private void OpenProfile()
        {
            if (!CheckSave())
            {
                return;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = Profile.Name, // Default file name
                DefaultExt = ".hpf", // Default file extension
                Filter = "Helios Profiles (.hpf)|*.hpf", // Filter files by extension
                InitialDirectory = ConfigManager.ProfilePath,
                ValidateNames = true,
                AddExtension = true,
                Multiselect = false,
                Title = "Open Profile"
            };

            // Show open file dialog box
            bool? result = dlg.ShowDialog(this);

            // Process open file dialog box results
            if (result == true)
            {
                LoadProfile(dlg.FileName);
                WatchImages();
            }
        }

        private void WatchImages()
        {
            if (_imageFileWatcher == null)
            {
                _imageFileWatcher = new FileSystemWatcher()
                {
                    Path = ConfigManager.ImagePath,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName,
                    Filter = "*.*",                             // multiple filetypes are not currently supported
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true,
                };
                _imageFileWatcher.Changed += new FileSystemEventHandler(OnImageFileChanged);
                _imageFileWatcher.Renamed += new RenamedEventHandler(OnImageFileChanged);
            }
            ConfigManager.ImageManager.ChangedImages.Clear();
            _changedImages = false;

        }

        private void ProcessVisualChildren(HeliosVisualCollection visuals, string imageName)
        {
            foreach (HeliosVisual visual in visuals)
            {
                if(visual.Children.Count > 0)
                {
                    ProcessVisualChildren(visual.Children, imageName);
                }

                if (visual is IRefreshableImage refreshableControl)
                {
                    if (refreshableControl.ConditionalImageRefresh(imageName)){
                        Logger.Debug($"Image reload requested for control {visual.GetType().Name} \"{visual.Name}\" image: \"{imageName}\"");
                    }
                } else
                {
                    Logger.Debug($"Visual investigation not performed on \"{visual.GetType().Name}\" - \"{visual.Name}\" and file \"{imageName}\" ");
                }
                visual.ImageRefresh = false;
            }
        }
        private void LoadProfile(string path, bool suppressLayout = false)
        {
            StatusBarMessage = "Loading Profile...";
            GetLoadingAdorner();

            // configure image caching appropriately for this load
            if (ConfigManager.ImageManager is IImageManager4 cacheCapable)
            {
                cacheCapable.CacheObjects = GlobalOptions.HasCacheImages;

                if (cacheCapable.CacheObjects)
                {
                    // always start fresh
                    cacheCapable.DropObjectCache();
                }
            }

            // create profile
            HeliosProfile profile = ConfigManager.ProfileManager.LoadProfile(path, out IEnumerable<string> loadingWork);

            // now load profile components in little chunks, because it may take a while
            foreach (string progress in loadingWork)
            {
                if (!PumpUserInterface())
                {
                    return;
                }
                 
                // log progress
                ProfileLoader.Logger.Debug(progress);
            }

            // everything is considered clean now
            ConfigManager.UndoManager.ClearHistory();

            // Load the graphics so everything is more responsive after load
            if (profile != null)
            {
                foreach (Monitor monitor in profile.Monitors)
                {
                    LoadVisual(monitor);
                    if (!PumpUserInterface())
                    {
                        return;
                    }
                }

                // NOTE: only install profile if not null (i.e. load succeeded)
                Profile = profile;
                if (!suppressLayout)
                {
                    RestoreSavedLayout(profile);
                }
            } 
            else
            {
                Logger.Error($"Failed to load profile '{path}'; continuing with previously loaded profile");
                MessageBox.Show(
                    "Profile Editor will continuing with the previously loaded profile.  Please inspect the Profile Editor logs and file a bug if appropriate.",
                    $"Failed to load profile '{path}'",
                    MessageBoxButton.OK);
            }

            RemoveLoadingAdorner();
            SetValue(StatusBarMessageProperty, "");
            OnProfileChangeComplete();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// called when the current profile and it is about to be replaced
        /// </summary>
        private void OnProfileChange()
        {
            if (Profile == null)
            {
                return;
            }

            // tell profile objects to disengage
            Profile.Unload();

            // clean up any running tools
            foreach (IProfileTool tool in _tools.Values.OfType<IProfileTool>())
            {
                tool.Close(Profile);
            }

            // dont' show stale sources and bindings
            BindingsPanel.Clear();
        }

        /// <summary>
        /// called when creating or loading a profile is complete
        /// </summary>
        private void OnProfileChangeComplete()
        {
            DocumentMeta selected = _documents.FirstOrDefault(meta => meta.document.IsSelected) ?? _documents.FirstOrDefault();
            if (selected != null)
            {
                selected.document.IsSelected = true;
                selected.document.IsActive = true;
            }

            // connect any loaded tools
            foreach (IProfileTool tool in _tools.Values.OfType<IProfileTool>())
            {
                tool.Open(Profile);
            }

            // start checking the configuration
            _triggered = false;
            _configurationCheck?.Reload(Profile);
        }

        private void RestoreSavedLayout(HeliosProfile profile)
        {
            string layoutFileName = System.IO.Path.ChangeExtension(profile.Path, "hply");
            if (File.Exists(layoutFileName))
            {
                try
                {
                    if (LayoutIsComplete(_systemDefaultLayout, layoutFileName))
                    {
                        _layoutSerializer.Deserialize(layoutFileName);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "failed to restore saved layout from {Path}", Anonymizer.Anonymize(layoutFileName));
                    // fall through to restore system default layout
                }
            }
            RestoreDefaultLayout();
        }

        private void RestoreDefaultLayout()
        {
            // we need to close any editor documents before we deserialize the layout because they will be removed but not closed and
            // then can never be recovered
            CloseAllDocuments();
            StringReader reader = new StringReader(_systemDefaultLayout);
            _layoutSerializer.Deserialize(reader);
        }

        /// <summary>
        /// pump the UI and abort if the our window is closing or the Dispatcher is going down
        /// </summary>
        /// <returns>false if we are exiting</returns>
        private bool PumpUserInterface()
        {
            // first check if our window is being closed, because we will exit if we pump the UI in that state
            if (_closing)
            {
                // Profile Editor main window canceling work because window is closing
                // NOTE: logger is dead at this point
                return false;
            }

            // now pump the UI or find out that the Dispatcher is null or dead
            bool bRunning = false;
            Dispatcher?.Invoke(() => { bRunning = true; }, DispatcherPriority.ContextIdle);
            if (_closing || !bRunning)
            {
                // Profile Editor main window canceling work because window is closing or application is shut down
                // NOTE: logger is dead at this point
                return false;
            }

            return true;
        }

        /// <summary>
        /// tries to determine if a given layout file will abandon any controls if loaded, by comparing
        /// to the system default layout that is dynamically generated on every startup
        /// </summary>
        /// <param name="layoutFileName"></param>
        /// <returns></returns>
        private static bool LayoutIsComplete(string systemLayoutText, string layoutFileName)
        {
            Regex contentId = new Regex("ContentId=\"([^\"]+)\"", RegexOptions.Compiled);
            string layout = File.ReadAllText(layoutFileName);
            foreach (Match match in contentId.Matches(systemLayoutText))
            {
                if (match.Groups.Count < 2)
                {
                    continue;
                }
                if (!match.Groups[1].Success)
                {
                    continue;
                }
                if (match.Groups[1].Value.StartsWith("Visual;"))
                {
                    // ignore monitor that is added by default
                    continue;
                }
                if (!layout.Contains(match.Groups[0].Value))
                {
                    // there is a named panel in this version of the software that
                    // would be removed if we load this layout
                    Logger.Info($"ignoring saved editor layout because it does not specify a location for the required control '{match.Groups[0].Value}'");
                    return false;
                }
            }
            return true;
        }

        private void ConfigurationCheck_Triggered(object sender, EventArgs e)
        {
            if (_triggered)
            {
                // only do this once
                return;
            }

            _triggered = true;
            LayoutAnchorable interfaceStatus = ShowCurrentLayoutAnchorable(InterfaceStatusPanel);
            if (interfaceStatus != null)
            {
                interfaceStatus.IsActive = true;
            }
        }

        private void LoadVisual(HeliosVisual visual)
        {
            visual.Renderer.Refresh();
            foreach (HeliosVisual control in visual.Children)
            {
                if (!PumpUserInterface())
                {
                    return;
                }
                LoadVisual(control);
            }
        }

        private bool SaveProfile()
        {
            if (string.IsNullOrEmpty(Profile.Path))
            {
                return SaveAsProfile();
            }

            WriteProfile(Profile);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return true;
        }

        private void WriteProfile(HeliosProfile profile)
        {
            Logger.Debug("saving profile to file");
            StatusBarMessage = "Saving Profile...";
            GetLoadingAdorner();
            if (!ConfigManager.ProfileManager.SaveProfile(profile))
            {
                throw new Exception("There was an error saving your profile.  Please contact support.");
            }
            ConfigManager.UndoManager.ClearHistory();
            profile.IsDirty = false;

            Logger.Debug("profile saved");
            RemoveLoadingAdorner();
            StatusBarMessage = "";

            string layoutFileName = System.IO.Path.ChangeExtension(profile.Path, "hply");
            if (File.Exists(layoutFileName))
            {
                Logger.Debug("deleting previous layout file");
                // ReSharper disable once AssignNullToNotNullAttribute file exists asserted above
                File.Delete(layoutFileName);
            }

            Logger.Debug("saving layout to file");
            _layoutSerializer.Serialize(layoutFileName);

            Logger.Debug("layout saved");
        }

        private bool SaveAsProfile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = Profile.Name, // Default file name
                DefaultExt = ".hpf", // Default file extension
                Filter = "Helios Profiles (.hpf)|*.hpf", // Filter files by extension
                InitialDirectory = ConfigManager.ProfilePath,
                OverwritePrompt = true,
                ValidateNames = true,
                AddExtension = true,
                Title = "Save Profile As"
            };

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                Profile.Path = dlg.FileName;
                Profile.Name = System.IO.Path.GetFileNameWithoutExtension(Profile.Path);
                WriteProfile(Profile);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return true;
            }

            return false;
        }

#endregion

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About dialog = new About {Owner = this};
            dialog.ShowDialog();
        }

        private void NewVersionCheck_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.VersionChecker.CheckAvailableVersionsWithDialog(true, true);
        }

#region Commands

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewProfile();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenProfile();
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveProfile();
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAsProfile();
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OpenProfileItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentMeta meta = AddNewDocument(e.Parameter as HeliosObject);
            if (meta?.document.Content is HeliosVisualContainerEditor)
            {
                meta.editor.SetBindingFocus((HeliosObject)e.Parameter);
            }
        }

        private void CloseProfileItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseProfileItem(e.Parameter);
        }

        public void CloseProfileItem(object item)
        {
            DocumentMeta meta = FindDocumentMeta(item as HeliosObject);

            if (meta == null)
            {
                return;
            }

            meta.document.Close();
            meta.hobj.PropertyChanged -= DocumentObject_PropertyChanged;
        }

        public void OnExecuteUndo(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigManager.UndoManager.Undo();
        }

        public void OnCanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ConfigManager.UndoManager.CanUndo;
        }

        public void OnExecuteRedo(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigManager.UndoManager.Redo();
        }

        public void OnCanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ConfigManager.UndoManager.CanRedo;
        }

        private void AddInterface_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                AvailableInterfaces availableInterfaces = new AvailableInterfaces();

                try
                {
                    availableInterfaces.Start(Profile);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error from interface enumeration during Add Interface dialog; some interfaces will not be available");

                    // show the user, but survive
                    ExceptionViewer.DisplayException(ex);
                }

                AddInterfaceDialog dialog = new AddInterfaceDialog
                {
                    DataContext = availableInterfaces,
                    Owner = this
                };
                bool? result = dialog.ShowDialog();

                // shut down async searches
                availableInterfaces.Dispose();
                dialog.DataContext = null;

                // anything selected?
                if (result != true || availableInterfaces.SelectedInterface == null)
                {
                    return;
                }
                
                // retrieve the interface instance, which may be created now or may already be in the item
                HeliosInterface heliosInterface = availableInterfaces.SelectedInterface.HeliosInterface;
                string name = heliosInterface.Name;
                int count = 0;
                while (Profile.Interfaces.ContainsKey(name))
                {
                    name = heliosInterface.Name + " " + ++count;
                }
                heliosInterface.Name = name;

                ConfigManager.UndoManager.AddUndoItem(new InterfaceAddUndoEvent(Profile, heliosInterface));
                Profile.Interfaces.Add(heliosInterface);
                AddNewDocument(heliosInterface);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error during Add Interface dialog or interface creation");
            }
        }

        private void SaveLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (File.Exists(_defaultLayoutFile))
            {
                File.Delete(_defaultLayoutFile);
            }
            _layoutSerializer.Serialize(_defaultLayoutFile);
        }

        private void LoadLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (File.Exists(_defaultLayoutFile))
            {
                _layoutSerializer.Deserialize(_defaultLayoutFile);
            }
            else
            {
                StringReader reader = new StringReader(_systemDefaultLayout);
                _layoutSerializer.Deserialize(reader);
            }
        }

        private void RestoreDefaultLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (File.Exists(_defaultLayoutFile))
            {
                File.Delete(_defaultLayoutFile);
            }
            RestoreDefaultLayout();
        }

        private void DialogShowModal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new DefaultDialogWindow().ShowModal(sender, e);
        }

        private void CloseAllDocuments()
        {
            // get as stable snapshot to enumerate
            List<DocumentMeta> documents = new List<DocumentMeta>(_documents);
            foreach (DocumentMeta meta in documents)
            {
                meta.document.Close();
            }
        }

        private void GoThere_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InterfaceStatusViewSection viewSection = (InterfaceStatusViewSection)(e.Parameter);
            if (viewSection.Data.HasEditor)
            {
                AddNewDocument(viewSection.Data.Interface);
            }
        }

        private void DeleteInterface_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // redirect delete received from check list to profile explorer
            if (e.Parameter is HeliosInterface heliosInterface)
            {
                ExplorerPanel.DeleteInterface(heliosInterface);
            }
        }

        private void ViewInterfaceStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LayoutAnchorable interfaceStatus = ShowCurrentLayoutAnchorable(InterfaceStatusPanel);
            if (interfaceStatus != null)
            {
                // switch tab also
                interfaceStatus.IsActive = true;
            }
        }

        #endregion

        private void Show_Preview(object sender, RoutedEventArgs e)
        {
            ShowCurrentLayoutAnchorable(PreviewPanel);
        }

        private void Show_Toolbox(object sender, RoutedEventArgs e)
        {
            ShowCurrentLayoutAnchorable(ToolboxPanel);
        }

        /// <summary>
        /// try to find the layout panel that is now hosting the specified control
        /// and display it.  During every layout deserialization, these layout panels
        /// are replaced so we don't keep references to them.
        /// </summary>
        /// <param name="withContent"></param>
        /// <returns>the LayoutAnchorable associated with the content or null</returns>
        private LayoutAnchorable ShowCurrentLayoutAnchorable(object withContent)
        {
            // LayoutAnchorable current = FindLayoutAnchorable(DockManager.Layout, withContent);
            LayoutAnchorable current = DockManager.Layout.Descendents().OfType<LayoutAnchorable>().First(l => l.Content == withContent);
            current?.Show();
            return current;
        }

        /// <summary>
        /// try to find the layout panel that is now hosting the specified control
        /// and hide or close it.  During every layout deserialization, these layout panels
        /// are replaced so we don't keep references to them.
        /// </summary>
        /// <param name="withContent"></param>
        /// <returns>the LayoutAnchorable associated with the content or null</returns>
        private ILayoutElement CloseCurrentLayoutAnchorable(object withContent)
        {
            // LayoutAnchorable current = FindLayoutAnchorable(DockManager.Layout, withContent);
            LayoutAnchorable current = DockManager.Layout.Descendents().OfType<LayoutAnchorable>().First(l => l.Content == withContent);
            current?.Hide();
            return current;
        }

        private void Show_Explorer(object sender, RoutedEventArgs e)
        {
            _ = ShowCurrentLayoutAnchorable(ExplorerPanel);
        }

        private void Show_Properties(object sender, RoutedEventArgs e)
        {
            _ = ShowCurrentLayoutAnchorable(PropertiesPanel);
        }

        private void Show_Bindings(object sender, RoutedEventArgs e)
        {
            _ = ShowCurrentLayoutAnchorable(BindingsPanel);
        }

        private void Show_Layers(object sender, RoutedEventArgs e)
        {
            _ = ShowCurrentLayoutAnchorable(LayersPanel);
        }

        private void ResetMonitors_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetMonitors resetDialog = new ResetMonitors(Profile) {Owner = this};
            bool? reset = resetDialog.ShowDialog();

            if (!reset.HasValue || !reset.Value)
            {
                return;
            }

            GetLoadingAdorner();
            IEnumerable<IResetMonitorsObserver> resetMonitorsObservers = Profile.Interfaces.OfType<IResetMonitorsObserver>();
            foreach (IResetMonitorsObserver interestedInterface in resetMonitorsObservers)
            {
                interestedInterface.NotifyResetMonitorsStarting();
            }

            try {
                ConfigManager.UndoManager.StartBatch();
                foreach (string progress in ResetMonitorsWork.ResetMonitors(this, resetDialog, Profile))
                {
                    if (!PumpUserInterface())
                    {
                        Logger.Warn("Aborting and rolling back any undoable operations from monitor reset");
                        ConfigManager.UndoManager.UndoBatch();
                        return;
                    }
                    ResetMonitorsWork.Logger.Debug(progress);
                }
                ConfigManager.UndoManager.CloseBatch();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Reset Monitors - Unhandled exception");
                Logger.Error("Rolling back any undoable operations from monitor reset");
                ConfigManager.UndoManager.UndoBatch();
                MessageBox.Show($"Error encountered while resetting monitors:{Environment.NewLine}{ex.Message}{Environment.NewLine}Please file a bug with the contents of the application log", "Error");
            }
            finally
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(RemoveLoadingAdorner));
                Dispatcher.Invoke(DispatcherPriority.Background, new Action<IEnumerable<IResetMonitorsObserver>>(OnResetMonitorsComplete), resetMonitorsObservers);
            }
        }

        /// <summary>
        /// callback on main thread after reset monitors thread completes and all change events on Profile.Monitors collection
        /// and properties on Monitor objects have been delivered
        /// </summary>
        private void OnResetMonitorsComplete(IEnumerable<IResetMonitorsObserver> resetMonitorsObservers)
        {
            foreach (IResetMonitorsObserver interestedInterface in resetMonitorsObservers)
            {
                interestedInterface.NotifyResetMonitorsComplete();
            }
        }

        private void DockManager_Loaded(object sender, RoutedEventArgs e)
        {
            StringWriter systemDefaultLayoutWriter = new StringWriter();
            _layoutSerializer.Serialize(systemDefaultLayoutWriter);
            _systemDefaultLayout = systemDefaultLayoutWriter.ToString();
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/HeliosVirtualCockpit/Helios/wiki/Donations");
        }

        private void Wiki_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/HeliosVirtualCockpit/Helios/wiki");
        }

        private void ReleaseNotes_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/HeliosVirtualCockpit/Helios/wiki/Change-Log");
        }

        private void Explorer_ItemDeleting(object sender, ItemDeleteEventArgs e)
        {
            CloseProfileItem(e.DeletedItem);
        }

        private void InstallArchive(string startupFile)
        {
            Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action<string>(InstallMode), startupFile);
        }

        private void InstallMode(string startupFile)
        {
            // reconfigure UI to just view the interface status and current document view
            ArchiveInstall.ArchiveInstall archiveInstall = new ArchiveInstall.ArchiveInstall(startupFile, this);

            // run install process, usually displaying some dialogs
            InstallationDialogs responseDialogs = new InstallationDialogs(this);
            if (InstallationResult.Success != archiveInstall.Install(responseDialogs))
            {
                return;
            }

            if (archiveInstall.HasMainProfile)
            {
                // change the layout to something small that makes sense for initial install
                ILayoutElement welcome = ConfigureUserInterfaceForInstall();

                // load the given profile
                LoadProfile(archiveInstall.MainProfilePath, true);

                // show the welcome screen delayed, because we probably triggered the interface status
                Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action<LayoutAnchorable>(ShowWelcome), welcome);
            }
            else if (!archiveInstall.ProfilePaths.Any())
            {
                // no profiles found.  this could happen if we decide to package up some files to push as an archive
                responseDialogs.ImportantMessage("Archive Contents Installed", 
                    $"Files contained in the Helios Archive '{startupFile}' were installed.  There were no profiles included, so there is nothing to configure.");
            }
            else
            {
                // modally, tell the user about the multiple profiles and that they should open one and configure it
                IEnumerable<string> lines = new[] {"", ""}.Concat(archiveInstall.ProfilePaths);
                string profiles = string.Join(Environment.NewLine, lines);
                responseDialogs.ImportantMessage("Multiple Profiles Installed",
                    $"Files contained in the Helios Archive '{startupFile}' were installed.  There were multiple profiles included.  Open the profiles you intend to use via the Profile menu and configure them: {profiles}");
            }
        }

        private void ShowWelcome(LayoutAnchorable welcome)
        {
            welcome.Show();
            welcome.IsActive = true;
        }

        private ILayoutElement ConfigureUserInterfaceForInstall()
        {
            // remove unused stuff
            CloseCurrentLayoutAnchorable(PreviewPanel);
            CloseCurrentLayoutAnchorable(ExplorerPanel);
            CloseCurrentLayoutAnchorable(ToolboxPanel);
            CloseCurrentLayoutAnchorable(LayersPanel);
            CloseCurrentLayoutAnchorable(BindingsPanel);
            CloseCurrentLayoutAnchorable(PropertiesPanel);

            // move the interface status
            LayoutAnchorable interfaceStatus = DockManager.Layout.Descendents().OfType<LayoutAnchorable>()
                .First(l => Equals(l.Content, InterfaceStatusPanel));
            if (interfaceStatus != null)
            {
                interfaceStatus.Parent.RemoveChild(interfaceStatus);
                ((LayoutAnchorablePaneGroup) DockManager.Layout.RootPanel.Children.First()).DockWidth = new GridLength(Width / 2);
                interfaceStatus.AddToLayout(DockManager, AnchorableShowStrategy.Left);
            }

            // open an "installation mode" window docked on the left, to tell the user what is going to happen, and have them click start to go to interface status
            // also include a button to reset the layout when they are done 
            LayoutAnchorable welcomePanel = new LayoutAnchorable();
            Welcome.Create(welcomePanel, out object _);
            welcomePanel.AddToLayout(DockManager, AnchorableShowStrategy.Left);
            return welcomePanel;
        }
    }
}

