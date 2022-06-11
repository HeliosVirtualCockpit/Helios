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

using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

namespace GadrocsWorkshop.Helios.Interfaces.Keyboard.Input
{
    internal abstract class KeyTriggers
    {
        // REVISIT whether each of these is a modifer could be configurable 
        public static readonly HashSet<Key> Modifiers = new HashSet<Key>
        {
            Key.LeftAlt,
            Key.LeftControl,
            Key.LeftShift,
            Key.LeftWindowsKey,
            Key.RightAlt,
            Key.RightControl,
            Key.RightShift,
            Key.RightWindowsKey
        };

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected Dictionary<Key, HeliosTrigger> _triggers;

        protected static IList<Key> CalculateAllKeys()
        {
            IList<Key> allKeys = System.Enum
                .GetValues(typeof(Key))
                .OfType<Key>()
                .Where(key => !Modifiers.Contains(key))
                .OrderBy(key => key.ToString())
                .ToList();
            return allKeys;
        }

        public void FireTrigger(Key key, BindingValue bindingValue)
        {
            if (!_triggers.TryGetValue(key, out HeliosTrigger trigger))
            {
                Logger.Warn("keyboard input interface ignoring unimplemented key {Name}", key);
                return;
            }

            trigger.FireTrigger(bindingValue);
        }

        /// <summary>
        /// construct string containing active modifiers in canonical order
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string ConstructModifierString(IEnumerable<Key> keys)
        {
            string modifierString = string.Join(" ", keys
                .Where(key => Modifiers.Contains(key))
                .OrderBy(key => key)
                .Select(AsString));
            return modifierString;
        }

        protected static string AsString(Key key) => key.ToString();

        #region Properties

        public IEnumerable<HeliosTrigger> Triggers => _triggers.Values;

        #endregion
    }
}