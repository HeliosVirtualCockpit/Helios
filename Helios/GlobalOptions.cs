// Copyright 2021 Ammo Goettsch
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

namespace GadrocsWorkshop.Helios
{
    /// <summary>
    /// Global options for Helios Library configured via Tools | Options menu or similar.
    /// Client code may use static accessors to check settings.  UI code will use properties against an instance.
    /// NOTE: the string names of properties in the XML file are intentionally not set from the property names
    /// in case the property names change later.
    /// </summary>
    public class GlobalOptions : NotificationObject
    {
        /// <summary>
        /// name used in he settings; may differ from property name and must never change
        /// </summary>
        private const string SETTING_ALLOW_RAW_CONVERSION = "AllowRawConversion";

        /// <summary>
        /// name used in the settings; may differ from property name and must never change
        /// </summary>
        private const string SETTING_ALLOW_SOFT_INTERFACES = "AllowSoftInterfaces";

        /// <summary>
        /// name used in the settings; may differ from property name and must never change
        /// </summary>
        private const string SETTING_SCALE_ALL_TEXT = "ScaleAllText";

        /// <summary>
        /// name used in the settings; may differ from property name and must never change
        /// </summary>
        private const string SETTING_USE_LEGACY_RESET_BEHAVIOR = "UseLegacyResetBehavior";

        /// <summary>
        /// name used in the settings; may differ from property name and must never change
        /// </summary>
        private const string SETTING_USE_LEGACY_LUA_RESET_BEHAVIOR = "UseLegacyLuaResetBehavior";

        /// <summary>
        /// Name used in the settings
        /// </summary>
        private const string SETTING_PERSIST_CHILDREN_AS_COMMENT = "PersistChildrenAsComment";

        /// <summary>
        /// Name used in the settings
        /// </summary>
        private const string SETTING_FUNCTION_DICTIONARY_TO_LOG = "LogFunctionDictionary";

        /// <summary>
        /// global options group name used in the settings, must never change
        /// </summary>
        private const string SETTINGS_GROUP = "Helios";

        /// <summary>
        /// backing field for property ScaleAllText, contains
        /// true if text attached to buttons and similar controls is scaled during reset monitors and similar operations
        /// </summary>
        private bool _scaleAllText;

        /// <summary>
        /// backing field for property UseLegacyResetBehavior, contains
        /// true if profile reset should act like it did in previous releases, not firing bindings for values
        /// that happen to already have the right value, even though their observers may have just been reset to
        /// an inconsistent initial state
        /// </summary>
        private bool _useLegacyResetBehavior;

        /// <summary>
        /// backing field for property UseLegacyLuaResetBehavior, contains
        /// true if profile reset should act like it did in previous releases and not reset all Lua variables
        /// </summary>
        private bool _useLegacyLuaResetBehavior;

        /// <summary>
        /// backing field for propert PersistChildrenAsComment / "Expose Internal Controls"
        /// if True, the serialization will wrapper the output of PersistChildren with a block comment
        /// so that the data appears in the profile.
        /// </summary>
        private bool _persistChildrenAsComment;

        /// <summary>
        /// backing field for property LogFunctionDictionary / "Save DCS Interface Functions to Log"
        /// if True, the Log file will contain a list of the DCS Interface Devices and Elements
        /// in the interfaces being added - primarily for profile creators to locate specific controls.
        /// </summary>
        private bool _logFunctionDictionary;

        public GlobalOptions()
        {
            _scaleAllText = HasScaleAllText;
            _useLegacyResetBehavior = HasUseLegacyResetBehavior;
            _useLegacyLuaResetBehavior = HasUseLegacyLuaResetBehavior;
            _persistChildrenAsComment = HasPersistChildrenAsComment;
        }

        #region Properties

        /// <summary>
        /// true if text attached to buttons and similar controls is scaled during reset monitors and similar operations
        /// </summary>
        public bool ScaleAllText
        {
            get => _scaleAllText;
            set
            {
                if (_scaleAllText == value)
                {
                    return;
                }

                bool oldValue = _scaleAllText;
                _scaleAllText = value;
                ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, SETTING_SCALE_ALL_TEXT, value);
                OnPropertyChanged("ScaleAllText", oldValue, value, true);
                ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, SETTING_SCALE_ALL_TEXT, value);
                OnPropertyChanged(nameof(ScaleAllText), oldValue, value, true);
            }
        }

        /// <summary>
        /// true if profile reset should act like it did in previous releases, not firing bindings for values
        /// that happen to already have the right value, even though their observers may have just been reset to
        /// an inconsistent initial state
        /// </summary>
        public bool UseLegacyResetBehavior
        {
            get => _useLegacyResetBehavior;
            set
            {
                if (_useLegacyResetBehavior == value)
                {
                    return;
                }

                bool oldValue = _useLegacyResetBehavior;
                _useLegacyResetBehavior = value;
                ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, SETTING_USE_LEGACY_RESET_BEHAVIOR, value);
                OnPropertyChanged(nameof(UseLegacyResetBehavior), oldValue, value, true);
            }
        }

        /// <summary>
        /// true if profile reset should act like it did in previous releases and not reset all Lua variables
        /// </summary>
        public bool UseLegacyLuaResetBehavior
        {
            get => _useLegacyLuaResetBehavior;
            set
            {
                if (_useLegacyLuaResetBehavior == value)
                {
                    return;
                }

                bool oldValue = _useLegacyLuaResetBehavior;
                _useLegacyLuaResetBehavior = value;
                ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, SETTING_USE_LEGACY_LUA_RESET_BEHAVIOR, value);
                OnPropertyChanged(nameof(UseLegacyLuaResetBehavior), oldValue, value, true);
            }
        }

        /// <summary>
        /// true if composite visual controls should serialize their internal controls to the profile as a comment.
        /// </summary>
        public bool PersistChildrenAsComment
        {
            get => _persistChildrenAsComment;
            set
            {
                if (value == _persistChildrenAsComment)
                {
                    return;
                }
                bool oldValue = _persistChildrenAsComment;
                ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, SETTING_PERSIST_CHILDREN_AS_COMMENT, value);
                _persistChildrenAsComment = value;
                OnPropertyChanged(nameof(PersistChildrenAsComment), oldValue, value, true);

            }
        }

        /// <summary>
        /// true if the Functions added by an interface are added to the log.
        /// </summary>
        public bool LogFunctionDictionary
        {
            get => _logFunctionDictionary;
            set
            {
                if (value == _logFunctionDictionary)
                {
                    return;
                }
                bool oldValue = _logFunctionDictionary;
                ConfigManager.SettingsManager.SaveSetting(SETTINGS_GROUP, SETTING_FUNCTION_DICTIONARY_TO_LOG, value);
                _logFunctionDictionary = value;
                OnPropertyChanged(nameof(LogFunctionDictionary), oldValue, value, true);

            }
        }

        /// <summary>
        /// accessible as utility for client code that can't get access to the GlobalOptions instance readily
        /// </summary>
        /// <returns>
        /// true if text attached to buttons and similar controls is scaled during reset monitors and similar operations
        /// </returns>
        public static bool HasScaleAllText =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_SCALE_ALL_TEXT, true);

        /// <summary>
        /// accessible as utility for client code that can't get access to the GlobalOptions instance readily
        /// </summary>
        /// <returns>
        /// true if interface classes may be defined entirely in external files without being instantiated in the code
        /// </returns>
        public static bool HasAllowSoftInterfaces =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_ALLOW_SOFT_INTERFACES, true);

        /// <summary>
        /// NOTE: there is current no UI for this
        /// </summary>
        public static bool HasAllowRawConversion =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_ALLOW_RAW_CONVERSION, true);

        /// <summary>
        /// accessible as utility for client code that can't get access to the GlobalOptions instance readily
        /// </summary>
        /// <returns>
        /// true if profile reset should act like it did in previous releases, not firing bindings for values
        /// that happen to already have the right value, even though their observers may have just been reset to
        /// an inconsistent initial state
        /// </returns>
        public static bool HasUseLegacyResetBehavior =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_USE_LEGACY_RESET_BEHAVIOR, false);

        /// <summary>
        /// accessible as utility for client code that can't get access to the GlobalOptions instance readily
        /// </summary>
        /// <returns>
        /// true if profile reset should act like it did in previous releases and not reset all Lua variables
        /// </returns>
        public static bool HasUseLegacyLuaResetBehavior =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_USE_LEGACY_LUA_RESET_BEHAVIOR, false);

        /// <summary>
        /// accessible as utility for client code that can't get access to the GlobalOptions instance readily
        /// </summary>
        /// <returns>
        /// true if composite visual controls should serialize their internal controls to the profile as a comment.
        /// </returns>
        public static bool HasPersistChildrenAsComment =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_PERSIST_CHILDREN_AS_COMMENT, false);

        /// <summary>
        /// accessible as utility for client code that can't get access to the GlobalOptions instance readily
        /// </summary>
        /// <returns>
        /// true if the Functions added by an interface are added to the log.
        /// </returns>
        public static bool HasLogFunctionDictionary =>
            ConfigManager.SettingsManager.LoadSetting(SETTINGS_GROUP, SETTING_FUNCTION_DICTIONARY_TO_LOG, false);

        #endregion
    }
}