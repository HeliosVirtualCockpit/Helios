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

namespace GadrocsWorkshop.Helios.ComponentModel
{
    using System;

    using GadrocsWorkshop.Helios;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HeliosInterfaceAttribute : Attribute
    {
        /// <param name="typeIdentifier">Unique identifier used for persistance.
        /// Recommended to follow conventions of {module name}.{interface}.  Helios.* is reserved for helios's included controls.</param>
        /// <param name="name">Display name used for this interface in the ui.</param>
        /// <param name="interfaceEditor">Instance factory for interface editor dialog.</param>
        /// 
        public HeliosInterfaceAttribute(string typeIdentifier, string name, Type interfaceEditor) : this(typeIdentifier, name, interfaceEditor, typeof(HeliosInterfaceFactory))
        {
        }

        /// <param name="typeIdentifier">Unique identifier used for persistance.
        /// Recommended to follow conventions of {module name}.{interface}.  Helios.* is reserved for helios's included controls.
        /// This value must never change once an interface is shipped.</param>
        /// <param name="name">Default instance name for this interface in the UI and Profile.  This value may be changed in future versions of the interface.</param>
        /// <param name="interfaceEditor">Instance factory for interface editor dialog.</param>
        /// <param name="factory">Instance factory used to populate "Add Interface" dialog with available interfaces.</param>
        /// 
        public HeliosInterfaceAttribute(string typeIdentifier, string name, Type interfaceEditor, Type factory)
        {
            TypeIdentifier = typeIdentifier;
            InterfaceEditorType = interfaceEditor;
            Name = name;
            Factory = factory;
        }

        public HeliosInterfaceAttribute(string typeIdentifier, string name) : this(typeIdentifier, name, null, typeof(HeliosInterfaceFactory))
        {
            // utility
        }

        public string TypeIdentifier { get; }

        /// <summary>
        /// if not null, interface must be created as child of interface with the given type ID
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// override of the type identifier to use for uniqueness test;  any interfaces
        /// with the same TypeIdentifier OR UniquenessKey are considered equivalent for 
        /// uniqueness test.
        /// </summary>
        public string UniquenessKey
        {
            get;
            set;
        }

        public string Name { get; }

        public Type InterfaceEditorType { get; }

        /// <summary>
        /// If true an instance of this control will automatically be added to a new profile.
        /// </summary>
        public bool AutoAdd { get; set; }

        /// <summary>
        /// If true the interface will not be available to add to a new profile.
        /// </summary>
        public bool Deprecated { get; set; }

        public Type Factory { get; set; }
    }
}