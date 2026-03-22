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

using System.Linq;
using GadrocsWorkshop.Helios.Tools;

namespace GadrocsWorkshop.Helios
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// ModuleManager gives access to all plugin component (Controls, Interfaces, Converters and Property Editors).
    /// </summary>
    internal class ModuleManager : IModuleManager3, IModuleManagerWritable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<BindingValueUnitConverter> _converters = new List<BindingValueUnitConverter>();
        private readonly Dictionary<string, HeliosPropertyEditorDescriptorCollection> _propertyEditors = new Dictionary<string, HeliosPropertyEditorDescriptorCollection>();
        private readonly List<HeliosToolDescriptor> _tools = new List<HeliosToolDescriptor>();
        private readonly List<HeliosCapabilityEditorDescriptor> _capabilityEditors = new List<HeliosCapabilityEditorDescriptor>();
        private List<string> _softInterfaceKnownVehicles = new List<string>();

        internal ModuleManager(string applicationPath)
        {
            Logger.Debug($"Helios will search for Helios modules in {applicationPath}");
        }

        public HeliosDescriptorCollection ControlDescriptors { get; } = new HeliosDescriptorCollection();

        public HeliosInterfaceDescriptorCollection InterfaceDescriptors { get; } = new HeliosInterfaceDescriptorCollection();

        public IList<string> InterfaceKnownVehicles
        {
            get {
                return InterfaceDescriptors.Where(descriptor => descriptor.UniquenessKey == "Helios.DCSInterface" 
                    && descriptor.ImpersonatedVehicleNames != null).
                    Select(descriptor => descriptor.ImpersonatedVehicleNames).
                    SelectMany(list => list).Concat(_softInterfaceKnownVehicles).Distinct().ToList();
            }
        }
        public void AddSoftInterfaceKnownVehicles(List<string> vehiclesToAdd)
        {
            _softInterfaceKnownVehicles = _softInterfaceKnownVehicles.Union(vehiclesToAdd).ToList();
        }

        public HeliosVisual CreateControl(string typeIdentifier)
        {
            HeliosVisual control = null;
            HeliosDescriptor descriptor = ControlDescriptors[typeIdentifier];
            if (descriptor != null)
            {
                control = (HeliosVisual)Activator.CreateInstance(descriptor.ControlType);
            }
            return control;
        }

        public HeliosInterfaceEditor CreateInterfaceEditor(HeliosInterface item, HeliosProfile profile)
        {
            HeliosInterfaceEditor editor = null;
            if (item != null)
            {
                HeliosInterfaceDescriptor descriptor = InterfaceDescriptors[item.GetType()];
                if (descriptor != null && descriptor.InterfaceEditorType != null)
                {
                    editor = (HeliosInterfaceEditor)Activator.CreateInstance(descriptor.InterfaceEditorType);
                    editor.Interface = item;
                    editor.Profile = profile;
                }
            }
            return editor;
        }

        public HeliosVisualRenderer CreaterRenderer(HeliosVisual visual)
        {
            HeliosVisualRenderer renderer = null;
            Type visualType = visual.GetType();

            HeliosDescriptor descriptor = ControlDescriptors[visualType];
            if (descriptor != null)
            {
                renderer = (HeliosVisualRenderer)Activator.CreateInstance(descriptor.Renderer);
                renderer.Visual = visual;
            }

            return renderer;
        }

        public HeliosPropertyEditorDescriptorCollection GetPropertyEditors(string typeIdentifier)
        {
            if (_propertyEditors.ContainsKey(typeIdentifier))
            {
                return _propertyEditors[typeIdentifier];
            }

            return new HeliosPropertyEditorDescriptorCollection();
        }

        public BindingValueUnitConverter GetUnitConverter(BindingValueUnit from, BindingValueUnit to)
        {
            foreach (BindingValueUnitConverter converter in _converters)
            {
                if (converter.CanConvert(from, to))
                {
                    return converter;
                }
            }
            return null;
        }

        public bool CanConvertUnit(BindingValueUnit from, BindingValueUnit to)
        {
            if (from.Equals(to)) return true;

            return (GetUnitConverter(from, to) != null);
        }

        public BindingValue ConvertUnit(BindingValue value, BindingValueUnit from, BindingValueUnit to)
        {
            BindingValueUnitConverter converter = GetUnitConverter(from, to);
            if (converter != null)
            {
                return converter.Convert(value, from, to);
            }
            return BindingValue.Empty;
        }

        public void RegisterModule(Assembly asm)
        {
            if (asm == null)
            {
                return;
            }

            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    object[] attrs = type.GetCustomAttributes(false);
                    string fullName = asm.FullName;
                    foreach (object attribute in attrs)
                    {
                        switch (attribute)
                        {
                            case HeliosControlAttribute controlAttribute:
                                Logger.Debug("Control found " + type.Name);
                                ControlDescriptors.Add(new HeliosDescriptor(type, controlAttribute));
                                break;
                            case HeliosInterfaceAttribute interfaceAttribute:
                                Logger.Debug("Interface found " + type.Name);
                                InterfaceDescriptors.Add(new HeliosInterfaceDescriptor(type, interfaceAttribute));
                                break;
                            case HeliosUnitConverterAttribute converterAttribute:
                                _ = converterAttribute;
                                Logger.Debug("Converter found " + type.Name);
                                _converters.Add((BindingValueUnitConverter)Activator.CreateInstance(type));
                                break;
                            case HeliosPropertyEditorAttribute editorAttribute:
                            {
                                Logger.Debug("Property editor found " + type.Name);
                                HeliosPropertyEditorDescriptorCollection editors;
                                if (_propertyEditors.ContainsKey(editorAttribute.TypeIdentifier))
                                {
                                    editors = _propertyEditors[editorAttribute.TypeIdentifier];
                                }
                                else
                                {
                                    editors = new HeliosPropertyEditorDescriptorCollection();
                                    _propertyEditors.Add(editorAttribute.TypeIdentifier, editors);
                                }

                                editors.Add(new HeliosPropertyEditorDescriptor(type, editorAttribute));
                                break;
                            }
                            case HeliosCapabilityEditorAttribute capabilityEditorAttribute:
                            {
                                Logger.Debug("Capability editor found " + type.Name);
                                _capabilityEditors.Add(new HeliosCapabilityEditorDescriptor(type, capabilityEditorAttribute));
                                break;
                            }
                            case HeliosToolAttribute tool:
                            {
                                _ = tool;
                                _tools.Add(new HeliosToolDescriptor(type));
                                break;
                            }
                        }
                    }

                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.Error(e, "Failed reflecting assembly " + asm.FullName);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed adding module " + asm.FullName);
            }
        }

        public IEnumerable<HeliosCapabilityEditorDescriptor> GetCapabilityEditors(HeliosVisual visual)
        {
            // NOTE: we could check for each interface and keep a list of editors for that interface, but that
            // complexity is not reasonable, considering it will typically be 1:1
            return (_capabilityEditors.Where(editor => editor.InterfaceType.IsInstanceOfType(visual)));
        }

        public IEnumerable<HeliosToolDescriptor> Tools => _tools;
    }
}
