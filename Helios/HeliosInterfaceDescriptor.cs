// Copyright 2014 Craig Courtney
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

using System;
using System.Collections.Generic;
using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios
{
    public class HeliosInterfaceDescriptor
    {
        private HeliosInterfaceFactory _factory;

        public HeliosInterfaceDescriptor(Type type, HeliosInterfaceAttribute attribute)
        {
            InterfaceType = type;

            // copy data from interface attribute markup
            Name = attribute.Name;
            TypeIdentifier = attribute.TypeIdentifier;
            ParentTypeIdentifier = attribute.Parent;
            InterfaceEditorType = attribute.InterfaceEditorType;
            FactoryType = attribute.Factory;
            UniquenessKey = attribute.UniquenessKey ?? attribute.TypeIdentifier;
            AutoAdd = attribute.AutoAdd;
            Deprecated = attribute.Deprecated;
        }

        protected HeliosInterfaceDescriptor(Type interfaceType, string name, string typeIdentifier, string parentTypeIdentifier, Type interfaceEditorType, Type factoryType, string uniquenessKey, bool autoAdd, bool deprecated = false)
        {
            InterfaceType = interfaceType;
            Name = name;
            TypeIdentifier = typeIdentifier;
            ParentTypeIdentifier = parentTypeIdentifier;
            InterfaceEditorType = interfaceEditorType;
            FactoryType = factoryType;
            UniquenessKey = uniquenessKey;
            AutoAdd = autoAdd;
            Deprecated = deprecated;
        }

        public virtual HeliosInterface CreateInstance()
        {
            // does this interface require its name?
            System.Reflection.ConstructorInfo ctor = InterfaceType.GetConstructor(new[] {typeof(string)});
            if (ctor != null)
            {
                return (HeliosInterface) Activator.CreateInstance(InterfaceType, Name);
            }

            // default construct or crash if no such constructor defined
            return (HeliosInterface) Activator.CreateInstance(InterfaceType);
        }

        public virtual HeliosInterface CreateInstance(HeliosInterface parent)
        {
            // does this interface require its parent and its own name?
            System.Reflection.ConstructorInfo ctor =
                InterfaceType.GetConstructor(new[] {typeof(HeliosInterface), typeof(string)});
            if (ctor != null)
            {
                return (HeliosInterface) Activator.CreateInstance(InterfaceType, parent, Name);
            }

            // construct with reference to parent or crash if no such constructor defined
            return (HeliosInterface) Activator.CreateInstance(InterfaceType, parent);
        }

        public List<HeliosInterface> GetNewInstances(HeliosProfile profile) =>
            Factory.GetInterfaceInstances(this, profile);

        public List<HeliosInterface> GetAutoAddInstances(HeliosProfile profile) =>
            Factory.GetAutoAddInterfaces(this, profile);

        #region Properties

        public Type InterfaceType { get; }
        public string Name { get; }
        public string TypeIdentifier { get; }
        public string ParentTypeIdentifier { get; }
        public Type InterfaceEditorType { get; }
        public Type FactoryType { get; }
        public string UniquenessKey { get; }

        /// <summary>
        /// true if an instance of this control will automatically be added to a new profile.
        /// </summary>
        public bool AutoAdd { get; }
        /// <summary>
        /// true if this control will not be added to a new profile.
        /// </summary>
        public bool Deprecated { get; set; }

        public HeliosInterfaceFactory Factory =>
            _factory ??
            (_factory = (HeliosInterfaceFactory) Activator.CreateInstance(FactoryType));

        #endregion
    }
}