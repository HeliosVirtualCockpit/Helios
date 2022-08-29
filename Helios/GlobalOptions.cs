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

        public GlobalOptions()
        {
            _scaleAllText = HasScaleAllText;
            _useLegacyResetBehavior = HasUseLegacyResetBehavior;
            _useLegacyLuaResetBehavior = HasUseLegacyLuaResetBehavior;
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

        #endregion
    }
}