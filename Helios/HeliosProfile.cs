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

using GadrocsWorkshop.Helios.Controls;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Capabilities.ProfileAwareInterface;
using System.Linq;

namespace GadrocsWorkshop.Helios
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Soft;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Media.Effects;

    public class HeliosProfile : NotificationObject, IReadyCheck
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private bool _invalidVersion;
        private bool _dirty;
        private bool _layoutChecked;

        private bool _validLayout;
        private bool _designMode;
        private bool _started;
        private string _name = "Untitled";
        private string _path = "";
        private readonly HashSet<string> _tags = new HashSet<string>();
        private ShaderEffect _shaderEffect;

        public HeliosProfile() : this(true)
        {
        }

        public HeliosProfile(bool autoAddInterfaces)
        {
            Monitors.CollectionChanged += Monitors_CollectionChanged;
            Interfaces.CollectionChanged += Interfaces_CollectionChanged;

            int i = 1;
            foreach (Monitor display in ConfigManager.DisplayManager.Displays)
            {
                Monitor monitor = new Monitor(display)
                {
                    Name = "Monitor " + i++
                };
                Monitors.Add(monitor);
            }

            if (autoAddInterfaces)
            {
                foreach (HeliosInterfaceDescriptor descriptor in ConfigManager.ModuleManager.InterfaceDescriptors)
                {
                    foreach(HeliosInterface newInterface in descriptor.GetAutoAddInstances(this))
                    {
                        Interfaces.Add(newInterface);
                    }
                }
            }

            LoadTime = DateTime.MinValue;
        }

        public event EventHandler ControlCenterShown;
        public event EventHandler ControlCenterHidden;
        public event EventHandler ProfileStarted;
        public event EventHandler ProfileTick;
        public event EventHandler ProfileStopped;
        public event EventHandler ProfileTransferControl;

        // this event indicates that some interface received an indication that a profile that 
        // matches the specified hint should be loaded
        public event EventHandler<ProfileHint> ProfileHintReceived;

        // this event indicates that some interface received an indication that the specified
        // export driver name is loaded on the other side of the interface
        public event EventHandler<DriverStatus> DriverStatusReceived;

        // this event indicates that some interface may have connected to a different endpoint
        // than before
        public event EventHandler<ClientChange> ClientChanged;

        // this event is fired when a control that allows being connected to control routers
        // has been selected for input in some way
        public event EventHandler<ControlEventArgs> RoutableControlSelected;

        #region Properties

        public IEnumerable<string> Tags => _tags;

        public bool DesignMode
        {
            get => _designMode;
            set
            {
                if (!_designMode.Equals(value))
                {
                    bool oldValue = _designMode;
                    _designMode = value;
                    OnPropertyChanged("DesignMode", oldValue, value, false);
                }
            }
        }


        public string Name
        {
            get => _name;
            set
            {
                if ((_name == null && value != null)
                    || (_name != null && !_name.Equals(value)))
                {
                    string oldValue = _name;
                    _name = value;
                    OnPropertyChanged("Name", oldValue, value, true);
                }
            }
        }

        public DateTime LoadTime { get; set; }

        /// <summary>
        /// called when a control that allows being connected to control routers
        ///  has been selected for input in some way
        /// </summary>
        /// <param name="control"></param>
        public void OnRoutableControlSelected(INamedControl control)
        {
            RoutableControlSelected?.Invoke(this, new ControlEventArgs(control));
        }

        public bool IsInvalidVersion
        {
            get => _invalidVersion;
            set
            {
                if (!_invalidVersion.Equals(value))
                {
                    bool oldValue = _invalidVersion;
                    _invalidVersion = value;
                    OnPropertyChanged("IsInvalidVersion", oldValue, value, false);
                }
            }
        }

        public HeliosInterfaceCollection Interfaces { get; } = new HeliosInterfaceCollection();

        public string Path
        {
            get => _path;
            set
            {
                if ((_path == null && value != null)
                    || (_path != null && !_path.Equals(value)))
                {
                    string oldPath = _path;
                    _path = value;
                    OnPropertyChanged("Path", oldPath, _path, false);
                }
            }
        }

        public bool IsDirty
        {
            get => _dirty;
            set
            {
                if (!_dirty.Equals(value))
                {
                    _dirty = value;
                    OnPropertyChanged("IsDirty", !_dirty, _dirty, false);
                }
            }
        }

        public MonitorCollection Monitors { get; } = new MonitorCollection();

        /// <summary>
        /// Returns true if all monitors defined in this profile match system defined monitors.
        /// </summary>
        public bool IsValidMonitorLayout
        {
            get
            {
                if (_layoutChecked)
                {
                    return _validLayout;
                }

                CheckedDisplays = ConfigManager.DisplayManager.Displays;
                _validLayout = CheckedDisplays.Count == Monitors.Count;
                foreach (Monitor display in Monitors)
                {
                    if (CheckedDisplays.Any(systemDisplay =>
                        // ReSharper disable CompareOfFloatsByEqualityOperator these really must be identical
                        systemDisplay.Top == display.Top &&
                        systemDisplay.Left == display.Left &&
                        systemDisplay.Width == display.Width &&
                        systemDisplay.Height == display.Height))
                    {
                        // found matching monitor
                        continue;
                    }

                    // did not find matching monitor, exit search
                    _validLayout = false;
                    break;
                }
                return _validLayout;
            }
        }

        /// <summary>
        /// the DisplayManager collection of monitors against which we checked the monitors
        /// to get our current value of IsValidMonitorLayout
        ///
        /// WARNING: there is currently no mechanism for Helios to learn the displays on the local machine have changed
        /// </summary>
        public MonitorCollection CheckedDisplays { get; private set; }

        public ShaderEffect Effect
        {
            get => _shaderEffect;
            set
            {
                if (!value.Equals(_shaderEffect))
                {
                    _shaderEffect = value;
                }
            }
        }

        public bool IsStarted
        {
            get => _started;
            private set
            {
                if (!_started.Equals(value))
                {
                    bool oldValue = _started;
                    _started = value;
                    OnPropertyChanged("IsStarted", oldValue, value, false);
                }
            }
        }

        /// <summary>
        /// profile instance tracking implementation
        /// </summary>
        internal ProfileInstances ProfileInstances { get; } = new ProfileInstances();

        #endregion

        #region Methods

        public void ShowControlCenter()
        {
            ControlCenterShown?.Invoke(this, EventArgs.Empty);
        }

        public void TransferControlToProfile(HeliosActionEventArgs e)
        {
            ProfileTransferControl?.Invoke(this, e);
        }
        public void HideControlCenter()
        {
            ControlCenterHidden?.Invoke(this, EventArgs.Empty);
        }

        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            Logger.Info("Profile starting. (Name=\"" + Name + "\")");
            OnProfileStarted();
            IsStarted = true;
            RequestProfileSupport();
            Logger.Info("Profile started. (Name=\"" + Name + "\")");
        }

        public void RequestProfileSupport()
        {
            // any interfaces that care should now provide information for the newly loaded profile
            string shortName = System.IO.Path.GetFileNameWithoutExtension(Path);
            foreach (IProfileAwareInterface profileAware in Interfaces.OfType<IProfileAwareInterface>())
            {
                profileAware.RequestDriver(shortName);
            }
        }

        /// <summary>
        /// called when this profile is unloaded, for example because a different profile is loaded or on exit
        /// </summary>
        public void Unload()
        {
            foreach (HeliosInterface heliosInterface in Interfaces)
            {
                // detach interfaces, so they can shut down
                ProfileInstances.Detach(heliosInterface);
                heliosInterface.Profile = null;
            }

            // clean up
            ProfileInstances.Dispose();
        }

        protected virtual void OnProfileStarted()
        {
            ProfileStarted?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            Logger.Info("Profile reseting. (Name=\"" + Name + "\")");
            foreach (Monitor monitor in Monitors)
            {
                monitor.Reset();
            }

            foreach (HeliosInterface heliosInterface in Interfaces)
            {
                heliosInterface.Reset();
            }
            Logger.Info("Profile reset completed. (Name=\"" + Name + "\")");
        }

        public void Stop()
        {
            if (IsStarted)
            {
                Logger.Info("Profile stopping. (Name=\"" + Name + "\")");
                OnProfileStopped();
                IsStarted = false;
                Logger.Info("Profile stopped. (Name=\"" + Name + "\")");
            }
        }

        protected virtual void OnProfileStopped()
        {
            ProfileStopped?.Invoke(this, EventArgs.Empty);
        }

        public void Tick()
        {
            OnProfileTick();
        }

        protected virtual void OnProfileTick()
        {
            ProfileTick?.Invoke(this, EventArgs.Empty);
        }

        private void Interfaces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (HeliosInterface heliosInterface in e.NewItems)
                {
                    heliosInterface.Profile = this;
                    ProfileInstances.Attach(heliosInterface);
                    heliosInterface.ReconnectBindings();
                    heliosInterface.PropertyChanged += new PropertyChangedEventHandler(Child_PropertyChanged);
                    if (heliosInterface is IProfileAwareInterface profileAware)
                    {
                        profileAware.ProfileHintReceived += Interface_ProfileHintReceived;
                        profileAware.DriverStatusReceived += Interface_DriverStatusReceived;
                        profileAware.ClientChanged += Interface_ClientChanged;
                        _tags.UnionWith(profileAware.Tags);
                        if(heliosInterface is SoftInterface softInterface && softInterface.ImpersonatedVehicles.Count() > 1) {
                            foreach(string xVehicle in softInterface.ImpersonatedVehicles) {
                                if (!_tags.Contains(xVehicle))
                                {
                                    _tags.Add(xVehicle);
                                }
                            }
                        }
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (HeliosInterface heliosInterface in e.OldItems)
                {
                    ProfileInstances.Detach(heliosInterface);
                    heliosInterface.Profile = null;
                    heliosInterface.DisconnectBindings();
                    heliosInterface.PropertyChanged -= new PropertyChangedEventHandler(Child_PropertyChanged);
                    if (heliosInterface is IProfileAwareInterface profileAware)
                    {
                        profileAware.ProfileHintReceived -= Interface_ProfileHintReceived;
                        profileAware.DriverStatusReceived -= Interface_DriverStatusReceived;
                        profileAware.ClientChanged -= Interface_ClientChanged;
                    }
                }
                // reindex all tags, since we have no way of removing non-unique ones
                _tags.Clear();
                foreach (HeliosInterface heliosInterface in Interfaces)
                {
                    if (heliosInterface is IProfileAwareInterface profileAware)
                    {
                        _tags.UnionWith(profileAware.Tags);
                        if (heliosInterface is SoftInterface softInterface && softInterface.ImpersonatedVehicles.Count() > 1)
                        {
                            foreach (string xVehicle in softInterface.ImpersonatedVehicles)
                            {
                                if (!_tags.Contains(xVehicle))
                                {
                                    _tags.Add(xVehicle);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Interface_DriverStatusReceived(object sender, DriverStatus e)
        {
            DriverStatusReceived?.Invoke(this, e);
        }

        private void Interface_ProfileHintReceived(object sender, ProfileHint e)
        {
            ProfileHintReceived?.Invoke(this, e);
        }

        private void Interface_ClientChanged(object sender, ClientChange e)
        {
            ClientChanged?.Invoke(this, e);
        }

        private void Monitors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                    foreach (Monitor monitor in e.NewItems)
                    {
                        monitor.Profile = this;
                        ProfileInstances.Attach(monitor);
                        monitor.PropertyChanged += new PropertyChangedEventHandler(Child_PropertyChanged);
                    }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (Monitor monitor in e.OldItems)
                {
                    ProfileInstances.Detach(monitor);
                    monitor.Profile = null;
                    monitor.PropertyChanged -= new PropertyChangedEventHandler(Child_PropertyChanged);
                }
            }

            InvalidateLayoutCheck();
        }

        /// <summary>
        /// note that something about the monitor layout may have changed, and discard cached layout check result
        /// </summary>
        public void InvalidateLayoutCheck()
        {
            _layoutChecked = false;
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is HeliosObject child && e is PropertyNotificationEventArgs args)
            {
                OnPropertyChanged(child.Name, args);
            }
        }

        public IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            foreach (HeliosInterface heliosInterface in Interfaces)
            {
                if (heliosInterface is IReadyCheck readyCheck)
                {
                    foreach (StatusReportItem item in readyCheck.PerformReadyCheck())
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerate the entire tree of HeliosVisuals, including Monitors
        ///
        /// NOTE: This is very slow and must not be used repeatedly at run time
        /// </summary>
        public IEnumerable<HeliosVisual> WalkVisuals()
        {
            foreach (Monitor monitor in Monitors)
            {
                foreach (HeliosVisual visited in WalkVisuals(monitor))
                {
                    yield return visited;
                }
            }
        }

        /// <summary>
        /// Recursively enumerate the visual given and all its descendants
        ///
        /// NOTE: This is very slow and must not be used repeatedly at run time
        /// </summary>
        public IEnumerable<HeliosVisual> WalkVisuals(HeliosVisual visual)
        {
            yield return visual;
            foreach (HeliosVisual child in visual.Children)
            {
                foreach (HeliosVisual visited in WalkVisuals(child))
                {
                    yield return visited;
                }
            }
        }

        #endregion
    }
}
