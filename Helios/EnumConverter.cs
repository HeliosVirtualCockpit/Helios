//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Documents;

namespace GadrocsWorkshop.Helios
{
    public class EnumConverter : IValueConverter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Type _type;
        private IDictionary _displayValues;
        private IDictionary _reverseValues;
        private List<EnumDisplayEntry> _overriddenDisplayEntries;

        public EnumConverter()
        {
        }

        public EnumConverter(Type type)
        {
            Type = type;
        }

        public Type Type
        {
            get => _type;
            set
            {
                if (!value.IsEnum)
                {
                    throw new ArgumentException("parameter is not an enumerated type", nameof(value));
                }

                _type = value;
            }
        }

        private void EnsureLoaded()
        {
            if (_displayValues != null)
            {
                return;
            }

            Type displayValuesType = typeof(Dictionary<,>).GetGenericTypeDefinition().MakeGenericType(_type, typeof(string));
            _displayValues = (IDictionary) Activator.CreateInstance(displayValuesType);

            _reverseValues =
                (IDictionary) Activator.CreateInstance(typeof(Dictionary<,>)
                    .GetGenericTypeDefinition()
                    .MakeGenericType(typeof(string), _type));

            FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (!(field.GetValue(null) is Enum enumValue))
                {
                    continue;
                };

                if (Filtered?.Contains(enumValue) ?? false)
                {
                    // we don't offer this value for selection
                    continue;
                }
                DescriptionAttribute[] a = (DescriptionAttribute[])
                    field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                // get a human-readable name for the enum
                string displayString = GetDisplayStringValue(a) ?? GetBackupDisplayStringValue(enumValue);
                if (displayString == null)
                {
                    // failed or excluded via code
                    continue;
                }

                _displayValues.Add(enumValue, displayString);
                _reverseValues.Add(displayString, enumValue);
            }
        }

        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                EnsureLoaded();
                return new List<string>((IEnumerable<string>) _displayValues.Values).AsReadOnly();
            }
        }

        private string GetDisplayStringValue(DescriptionAttribute[] a)
        {
            if (a == null || a.Length == 0)
            {
                return null;
            }

            DescriptionAttribute dsa = a[0];
            return dsa.Description;
        }

        private string GetBackupDisplayStringValue(object enumValue)
        {
            if (_overriddenDisplayEntries == null || _overriddenDisplayEntries.Count <= 0)
            {
                return Enum.GetName(_type, enumValue);
            }

            EnumDisplayEntry foundEntry = _overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry)
            {
                object e = Enum.Parse(_type, entry.EnumValue);
                return enumValue.Equals(e);
            });

            if (foundEntry == null)
            {
                return Enum.GetName(_type, enumValue);
            }

            return foundEntry.ExcludeFromDisplay ? null : foundEntry.DisplayString;

        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                if (_overriddenDisplayEntries == null)
                {
                    _overriddenDisplayEntries = new List<EnumDisplayEntry>();
                }

                return _overriddenDisplayEntries;
            }
        }

        public List<Enum> Filtered { get; set; } = new List<Enum>();

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnsureLoaded();
            Logger.Debug("attempting to convert from {Value} to {Type}", value, targetType);
            if (!_displayValues.Contains(value))
            {
                Logger.Warn("value {Value} of type {Type} was not found in mappings:", value, value.GetType());
                foreach (DictionaryEntry displayValue in _displayValues)
                {
                    Logger.Warn("{Source} of type {Type} -> {Target}", displayValue.Key, displayValue.Key.GetType(),
                        displayValue.Value);
                }
            }

            return _displayValues[value];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnsureLoaded();
            Logger.Debug("attempting to convert back from {Value} to {Type}", value, targetType);
            return _reverseValues[value];
        }
    }
}