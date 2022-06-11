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
    internal class KeyTriggersWithModifiers : KeyTriggers
    {
        public KeyTriggersWithModifiers(HeliosObject parent)
        {
            // document the modifiers for inclusion in value descriptions
            string modifierOrder = ConstructModifierString(System.Enum.GetValues(typeof(Key)).OfType<Key>());

            IList<Key> allKeys = CalculateAllKeys();

            // debug output all key names
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Bindable Keys: {Text}", string.Join(", ", allKeys.Select(AsString)));
            }

            // construct triggers
            _triggers = allKeys.ToDictionary(key => key,
                key => ConstructPressed(parent, "Any Modifiers", key, modifierOrder));
        }

        private static HeliosTrigger ConstructPressed(HeliosObject parent, string deviceName, Key key,
            string modifierOrder) =>
            new HeliosTrigger(parent, deviceName, key.ToString(), "pressed",
                $"triggered when the '{key}' key is pressed with any modifiers",
                $"the currently active modifier keys in order, like '{modifierOrder}'", BindingValueUnits.Text);
    }
}