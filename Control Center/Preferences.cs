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

using System.Security;
using System.Windows.Input;
using Microsoft.Win32;

namespace GadrocsWorkshop.Helios.ControlCenter
{
    public class Preferences : NotificationObject
    {
        private readonly ISettingsManager2 _preferences;
        private static readonly string GROUP_NAME = "Preferences";

        public Preferences(ISettingsManager2 preferences)
        {
            _preferences = preferences;
            _profileAutoStart = LoadSetting("ProfileAutoStart", true);
            _startMinimized = LoadSetting("StartMinimized", false);
            _autoHide = LoadSetting("AutoHide", false);
            _preflightCheck = LoadSetting("PreflightCheck", true);
            _suppressMouseAfterTouchDuration = LoadSetting("SuppressMouseAfterTouchDuration", 0);
            _hotKey = LoadSetting("HotKey", Key.None.ToString());
            _hotKeyModifiers = LoadSetting("HotKeyModifiers", ModifierKeys.None.ToString());
            _splashScreen = LoadSetting("SplashScreen", true);
            _highQualityBitmapScaling = LoadSetting("HighQualityBitmapScaling", false);
            _cacheImages = LoadSetting("CacheImages", true);

            // NOTE: do not try to implement auto start in registry even if enabled, we may not have rights
            _autoStart = LoadSetting("AutoStart", false);
        }

        // initialize each preference from our preferences file, defaulting to any settings left over in
        // the settings manager, where these used to come from
        private T LoadSetting<T>(string name, T defaultValue)
        {
            T maybeSetting = ConfigManager.SettingsManager.LoadSetting("ControlCenter", name, defaultValue);
            return _preferences.LoadSetting(GROUP_NAME, name, maybeSetting);
        }

        /// <summary>
        /// backing field for property ProfileAutoStart, contains
        /// true if the automatic starting of profile when plane changes are received is enabled
        /// </summary>
        private bool _profileAutoStart;

        /// <summary>
        /// true if the automatic starting of profile when plane changes are received is enabled
        /// </summary>
        public bool ProfileAutoStart
        {
            get => _profileAutoStart;
            set
            {
                if (_profileAutoStart == value)
                {
                    return;
                }

                bool oldValue = _profileAutoStart;
                _profileAutoStart = value;
                _preferences.SaveSetting(GROUP_NAME, "ProfileAutoStart", value);
                OnPropertyChanged("ProfileAutoStart", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property StartMinimized, contains
        /// true if Control Center should minimize on launch
        /// </summary>
        private bool _startMinimized;

        /// <summary>
        /// true if Control Center should minimize on launch
        /// </summary>
        public bool StartMinimized
        {
            get => _startMinimized;
            set
            {
                if (_startMinimized == value) return;
                bool oldValue = _startMinimized;
                _startMinimized = value;
                _preferences.SaveSetting(GROUP_NAME, "StartMinimized", value);
                OnPropertyChanged("StartMinimized", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property AutoHide, contains
        /// true if Control Center should hide after successfully starting a profile
        /// </summary>
        private bool _autoHide;

        /// <summary>
        /// true if Control Center should hide after successfully starting a profile
        /// </summary>
        public bool AutoHide
        {
            get => _autoHide;
            set
            {
                if (_autoHide == value) return;
                bool oldValue = _autoHide;
                _autoHide = value;
                _preferences.SaveSetting(GROUP_NAME, "AutoHide", value);
                OnPropertyChanged("AutoHide", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property AutoStart, contains
        /// true if Helios should start on Windows startup
        /// </summary>
        private bool _autoStart;

        /// <summary>
        /// true if Helios should start on Windows startup
        /// </summary>
        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                if (_autoStart == value) return;
                bool oldValue = _autoStart;
                _autoStart = value;
                _preferences.SaveSetting(GROUP_NAME, "AutoStart", value);
                UpdateRegistry();
                OnPropertyChanged("AutoStart", oldValue, value, true);
            }
        }

        private void UpdateRegistry()
        {
            if (_autoStart)
            {
                RegistryKey pathKey =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                pathKey?.SetValue("Helios",
                    "\"" + System.IO.Path.Combine(ConfigManager.ApplicationPath, "Control Center.exe") + "\"");
                pathKey?.Close();
            }
            else
            {
                RegistryKey pathKey =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                pathKey?.DeleteValue("Helios", false);
                pathKey?.Close();
            }
        }

        /// <summary>
        /// backing field for property PreflightCheck, contains
        /// true if preflight check is requested
        /// </summary>
        private bool _preflightCheck;

        /// <summary>
        /// true if preflight check is requested
        /// </summary>
        public bool PreflightCheck
        {
            get => _preflightCheck;
            set
            {
                if (_preflightCheck == value) return;
                bool oldValue = _preflightCheck;
                _preflightCheck = value;
                _preferences.SaveSetting(GROUP_NAME, "PreflightCheck", value);
                OnPropertyChanged("PreflightCheck", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property SuppressMouseAfterTouchDuration, contains
        /// number of mlliseconds to ignore mouse events after a touch event, or 0 to disable
        /// </summary>
        private int _suppressMouseAfterTouchDuration;

        /// <summary>
        /// number of mlliseconds to ignore mouse events after a touch event, or 0 to disable
        /// </summary>
        public int SuppressMouseAfterTouchDuration
        {
            get => _suppressMouseAfterTouchDuration;
            set
            {
                if (_suppressMouseAfterTouchDuration == value) return;
                int oldValue = _suppressMouseAfterTouchDuration;
                _suppressMouseAfterTouchDuration = value;
                _preferences.SaveSetting(GROUP_NAME, "SuppressMouseAfterTouchDuration", value);
                OnPropertyChanged("SuppressMouseAfterTouchDuration", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property HotKeyModifiers, contains
        /// string representation of selected hotkey modifiers, for serialization
        /// </summary>
        private string _hotKeyModifiers;

        /// <summary>
        /// string representation of selected hotkey modifiers, for serialization
        /// </summary>
        public string HotKeyModifiers
        {
            get => _hotKeyModifiers;
            set
            {
                if (_hotKeyModifiers != null && _hotKeyModifiers == value) return;
                string oldValue = _hotKeyModifiers;
                _hotKeyModifiers = value;
                _preferences.SaveSetting(GROUP_NAME, "HotKeyModifiers", value);
                OnPropertyChanged("HotKeyModifiers", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property HotKey, contains
        /// string representation of currently selected hotkey
        /// </summary>
        private string _hotKey;

        /// <summary>
        /// string representation of currently selected hotkey
        /// </summary>
        public string HotKey
        {
            get => _hotKey;
            set
            {
                if (_hotKey != null && _hotKey == value) return;
                string oldValue = _hotKey;
                _hotKey = value;
                _preferences.SaveSetting(GROUP_NAME, "HotKey", value);
                OnPropertyChanged("HotKey", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property SplashScreen, contains
        /// true if the splash screen on Control Center startup is enabled
        /// </summary>
        private bool _splashScreen;

        /// <summary>
        /// true if the splash screen on Control Center startup is enabled
        /// </summary>
        public bool SplashScreen
        {
            get => _splashScreen;
            set
            {
                if (_splashScreen == value) return;
                bool oldValue = _splashScreen;
                _splashScreen = value;
                _preferences.SaveSetting(GROUP_NAME, "SplashScreen", value);
                OnPropertyChanged("SplashScreen", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property HighQualityBitmapScaling,
        /// if true, all visuals on all monitors use slower "Fant" scaling for bitmaps to increase visual quality 
        /// </summary>
        private bool _highQualityBitmapScaling;

        /// <summary>
        /// if true, all visuals on all monitors use slower "Fant" scaling for bitmaps to increase visual quality 
        /// </summary>
        public bool HighQualityBitmapScaling
        {
            get => _highQualityBitmapScaling;
            set
            {
                if (_highQualityBitmapScaling == value) return;
                bool oldValue = _highQualityBitmapScaling;
                _highQualityBitmapScaling = value;
                _preferences.SaveSetting(GROUP_NAME, "HighQualityBitmapScaling", value);
                OnPropertyChanged("", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property CacheImages, contains
        /// true if object caching should be enabled in ImageManager
        /// </summary>
        private bool _cacheImages;

        /// <summary>
        /// true if object caching should be enabled in ImageManager
        /// </summary>
        public bool CacheImages
        {
            get => _cacheImages;
            set
            {
                if (_cacheImages == value) return;
                bool oldValue = _cacheImages;
                _cacheImages = value;
                _preferences.SaveSetting(GROUP_NAME, "CacheImages", value);
                OnPropertyChanged(nameof(CacheImages), oldValue, value, true);
            }
        }
    }
}