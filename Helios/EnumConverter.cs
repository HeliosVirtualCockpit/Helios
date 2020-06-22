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

namespace GadrocsWorkshop.Helios
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;

    public class EnumConverter : IValueConverter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Type type;
        private IDictionary displayValues;
        private IDictionary reverseValues;
        private List<EnumDisplayEntry> overriddenDisplayEntries;

        public EnumConverter()
        {
        }

        public EnumConverter(Type type)
        {
            this.Type = type;
        }

        public Type Type
        {
            get { return type; }
            set
            {
                if (!value.IsEnum)
                    throw new ArgumentException("parameter is not an enumerated type", nameof(value));
                this.type = value;
            }
        }

        private void EnsureLoaded()
        {
            if (displayValues != null)
            {
                return;
            }

            Type displayValuesType = typeof(Dictionary<,>).GetGenericTypeDefinition().MakeGenericType(type, typeof(string));
            this.displayValues = (IDictionary)Activator.CreateInstance(displayValuesType);

            this.reverseValues =
                (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                    .GetGenericTypeDefinition()
                    .MakeGenericType(typeof(string), type));

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                DescriptionAttribute[] a = (DescriptionAttribute[])
                    field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                string displayString = GetDisplayStringValue(a);
                object enumValue = field.GetValue(null);

                if (displayString == null)
                {
                    displayString = GetBackupDisplayStringValue(enumValue);
                }
                if (displayString != null)
                {
                    displayValues.Add(enumValue, displayString);
                    reverseValues.Add(displayString, enumValue);
                }
            }
        }

        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                EnsureLoaded();
                return new List<string>((IEnumerable<string>)displayValues.Values).AsReadOnly();
            }
        }

        private string GetDisplayStringValue(DescriptionAttribute[] a)
        {
            if (a == null || a.Length == 0) return null;
            DescriptionAttribute dsa = a[0];
            return dsa.Description;
        }

        private string GetBackupDisplayStringValue(object enumValue)
        {
            if (overriddenDisplayEntries != null && overriddenDisplayEntries.Count > 0)
            {
                EnumDisplayEntry foundEntry = overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry)
                {
                    object e = Enum.Parse(type, entry.EnumValue);
                    return enumValue.Equals(e);
                });
                if (foundEntry != null)
                {
                    if (foundEntry.ExcludeFromDisplay) return null;
                    return foundEntry.DisplayString;

                }
            }
            return Enum.GetName(type, enumValue);
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                if (overriddenDisplayEntries == null)
                    overriddenDisplayEntries = new List<EnumDisplayEntry>();
                return overriddenDisplayEntries;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnsureLoaded();
            Logger.Debug("attempting to convert from {Value} to {Type}", value, targetType);
            if (!displayValues.Contains(value))
            {
                Logger.Debug("value {Value} of type {Type} was not found in mappings:", value, value.GetType());
                foreach (DictionaryEntry displayValue in displayValues)
                {
                    Logger.Debug("{Source} of type {Type} -> {Target}", displayValue.Key, displayValue.Key.GetType(), displayValue.Value);
                }
            }
            return displayValues[value];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnsureLoaded();
            Logger.Debug("attempting to convert back from from {Value} to {Type}", value, targetType);
            return reverseValues[value];
        }
    }
}
