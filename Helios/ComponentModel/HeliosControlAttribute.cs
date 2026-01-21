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

namespace GadrocsWorkshop.Helios.ComponentModel
{
    using System;

    [Flags]
    public enum HeliosControlFlags
    {
        None = 1,
        NotShownInUI = 2,
        NotRotatable = 4,
        NoEffects = 8,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HeliosControlAttribute : Attribute
    {
        /// <param name="typeIdentifier">Unique identifier used for persistance.
        /// Recommended to follow conventions of {module name}.{interface}.  Helios.* is reserved for helios's included controls.</param>
        /// <param name="name">Display name used for this control in the ui.</param>
        /// <param name="flags">combinatation (via OR operator) of flags to control various aspects of how this control can be used.</param>
        public HeliosControlAttribute(string typeIdentifier, string name, string category, Type renderer, HeliosControlFlags flags = HeliosControlFlags.None)
        {
            TypeIdentifier = typeIdentifier;
            Category = category;
            Renderer = renderer;
            Name = name;
            Flags = flags;
        }

        public string TypeIdentifier { get; }

        public string Name { get; }

        public string Category { get; }

        public Type Renderer { get; }

        public HeliosControlFlags Flags { get; }
    }
}
