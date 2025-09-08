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

using GadrocsWorkshop.Helios.Interfaces.Capabilities;

namespace GadrocsWorkshop.Helios
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows.Threading;
    using System.Xml;

    public class HeliosSerializer : BaseDeserializer
    {
        private HeliosBindingCollection _skipbindings = new HeliosBindingCollection();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public HeliosSerializer(Dispatcher dispatcher)
            : base()
        {
            // we don't use this any more since all loading is done on main thread
            _ = dispatcher;
        }

        public HeliosSerializer()
            : base()
        {
            // no code
        }

        #region Monitors

        public void SerializeMonitor(Monitor monitor, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Monitor");
            monitor.WriteXml(xmlWriter);
            SerializeControls(monitor.Children, xmlWriter);
            xmlWriter.WriteEndElement();
        }

        public void SerializeMonitors(MonitorCollection monitors, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Monitors");
            foreach (Monitor display in monitors)
            {
                SerializeMonitor(display, xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        public IEnumerable<string> DeserializeMonitor(MonitorCollection destination, XmlReader xmlReader, int monitorNumber)
        {
            xmlReader.ReadStartElement("Monitor");
            Monitor display = (Monitor)CreateNewObject("Monitor", "");
            display.ReadXml(xmlReader);
            foreach (string progress in DeserializeControls(display.Children, xmlReader))
            {
                yield return progress;
            }
            xmlReader.ReadEndElement();
            display.Name = "Monitor " + monitorNumber;
            destination.Add(display);
            yield return $"loaded {display.Name}";
        }

        public IEnumerable<string> DeserializeMonitors(MonitorCollection destination, XmlReader xmlReader)
        {
            int i = 1;
            if (xmlReader.Name.Equals("Monitors"))
            {
                if (!xmlReader.IsEmptyElement)
                {
                    xmlReader.ReadStartElement("Monitors");
                    while (xmlReader.NodeType != XmlNodeType.EndElement)
                    {
                        foreach (string progress in DeserializeMonitor(destination, xmlReader, i++))
                        {
                            yield return progress;
                        }
                    }
                    xmlReader.ReadEndElement();
                }
                else
                {
                    xmlReader.Read();
                }
            }

            yield return "loaded monitors";
        }

        #endregion

        #region Controls

        public void SerializeControl(HeliosVisual control, XmlWriter xmlWriter)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            xmlWriter.WriteStartElement("Control");
            xmlWriter.WriteAttributeString("TypeIdentifier", control.TypeIdentifier);
            xmlWriter.WriteAttributeString("Name", control.Name);
            xmlWriter.WriteAttributeString("SnapTarget", boolConverter.ConvertToInvariantString(control.IsSnapTarget));
            xmlWriter.WriteAttributeString("Locked", boolConverter.ConvertToInvariantString(control.IsLocked));
            control.WriteXml(xmlWriter);
            bool childrenAsComment = (control.PersistChildrenAsComment || GlobalOptions.HasPersistChildrenAsComment);
            if (control.PersistChildren)
            {
                SerializeControls(control.Children, xmlWriter);
            }
            else if (childrenAsComment && control.Children.Count > 0)
            {
                    control.InsideCommentBlock = control.Parent.InsideCommentBlock;
                    if (!control.InsideCommentBlock)
                    {
                        xmlWriter.WriteStartElement("Children");
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteComment($"Internal Child Controls for CompositeVisual {control.Name}");
                        xmlWriter.WriteRaw($"{Environment.NewLine}<!-- {Environment.NewLine}");
                        control.InsideCommentBlock = true;
                    }

                    SerializeControls(control.Children, xmlWriter);

                    if (!control.Parent.InsideCommentBlock)
                    {
                        xmlWriter.WriteRaw($"{Environment.NewLine} -->{Environment.NewLine}");
                        control.InsideCommentBlock = false;
                    }
            }
            else
            {
                xmlWriter.WriteStartElement("Children");
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        public void SerializeControls(HeliosVisualCollection controls, XmlWriter xmlWriter)
        {   
            xmlWriter.WriteStartElement("Children");
            foreach (HeliosVisual control in controls)
            {
                SerializeControl(control, xmlWriter);
            }
            xmlWriter.WriteEndElement();  // Controls
        }

        public IEnumerable<string> DeserializeControl(HeliosVisualCollection controls, XmlReader xmlReader)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            HeliosVisual control = (HeliosVisual)CreateNewObject("Visual", xmlReader.GetAttribute("TypeIdentifier"));
            if (control != null)
            {   
                // NOTE: don't do this in finally block, because we DO want to bypass rendering if we fail to load
                control.BypassRendering();

                string name = xmlReader.GetAttribute("Name");
                if (xmlReader.GetAttribute("SnapTarget") != null)
                {
                    control.IsSnapTarget = (bool)boolConverter.ConvertFromInvariantString(xmlReader.GetAttribute("SnapTarget"));
                }
                if (xmlReader.GetAttribute("Locked") != null)
                {
                    control.IsLocked = (bool)boolConverter.ConvertFromInvariantString(xmlReader.GetAttribute("Locked"));
                }

                if (xmlReader.IsEmptyElement)
                {
                    xmlReader.Read();
                }
                else
                {
                    xmlReader.ReadStartElement("Control");
                    control.ReadXml(xmlReader);
                    foreach (string progress in DeserializeControls(control.Children, xmlReader))
                    {       
                        yield return progress;
                    };
                    xmlReader.ReadEndElement();
                }
                control.Name = name;
                if (controls.ContainsKey(name))
                {
                    /// ToDo:  Complete PersistChildren implimentation for Composite Visual.
                    /// 
                    /// * * * Currently no composite visual has PersistChidren or should have PersistChidren specified.
                    /// 
                    /// This code is for the situation where a composite visual has been saved with
                    /// PersistChidren so the controls added by the composite visual reappear in the 
                    /// profile.  This child controls take precedence over the composite visual
                    /// control which allows the ability to override control positions and images.
                    /// There is no user interface mechanism to override the settings.
                    foreach (HeliosBinding bind in controls[name].InputBindings)
                    {
                        control.InputBindings.Add(bind);
                    }
                    foreach (HeliosBinding bind in controls[name].OutputBindings)
                    {
                        control.OutputBindings.Add(bind);
                    }
                    if (controls[name] is CompositeVisual)
                    {
                        foreach (DefaultInputBinding bind in (controls[name] as CompositeVisual)?.DefaultInputBindings)
                        {
                            (control as CompositeVisual)?.DefaultInputBindings.Add(bind);
                        }
                        foreach (DefaultOutputBinding bind in (controls[name] as CompositeVisual)?.DefaultOutputBindings)
                        {
                            (control as CompositeVisual)?.DefaultOutputBindings.Add(bind);
                        }
                    }
                    controls.RemoveKey(name);
                }
                controls.Add(control);
                control.ResumeRendering();
                yield return $"loaded {control.TypeIdentifier}";
            }
            else
            {
                xmlReader.Skip();
                yield return "failed to load a control";
            }
        }

        public IEnumerable<string> DeserializeControls(HeliosVisualCollection controls, XmlReader xmlReader)
        {
            if (!xmlReader.IsEmptyElement)
            {
                xmlReader.ReadStartElement("Children");
                while (xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    foreach (string progress in DeserializeControl(controls, xmlReader))
                    {
                        yield return progress;
                    }
                }
                xmlReader.ReadEndElement();
            }
            else
            {
                xmlReader.Read();
            }
        }

        #endregion

        #region Bindings

        public void SerializeBinding(HeliosBinding binding, XmlWriter xmlWriter)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            if (binding.Action != null && binding.Trigger != null)
            {

                xmlWriter.WriteStartElement("Binding");

                xmlWriter.WriteAttributeString("BypassCascadingTriggers", boolConverter.ConvertToInvariantString(binding.BypassCascadingTriggers));
                if (binding.LuaScriptUTF8Encoding)
                {
                    xmlWriter.WriteAttributeString("LuaScriptUTF8Encoding", boolConverter.ConvertToInvariantString(binding.LuaScriptUTF8Encoding));
                }
                xmlWriter.WriteStartElement("Trigger");
                xmlWriter.WriteAttributeString("Source", GetReferenceName(binding.Trigger.Source));
                xmlWriter.WriteAttributeString("Name", binding.Trigger.TriggerID);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Action");
                xmlWriter.WriteAttributeString("Target", GetReferenceName(binding.Action.Target));
                xmlWriter.WriteAttributeString("Name", binding.Action.ActionID);
                xmlWriter.WriteEndElement();

                switch (binding.ValueSource)
                {
                    case BindingValueSources.StaticValue:
                        xmlWriter.WriteElementString("StaticValue", binding.Value);
                        break;
                    case BindingValueSources.TriggerValue:
                        xmlWriter.WriteStartElement("TriggerValue");
                        xmlWriter.WriteEndElement();
                        break;
                    case BindingValueSources.LuaScript:
                        xmlWriter.WriteElementString("LuaScript", binding.Value);
                        break;
                }

                if (binding.HasCondition)
                {
                    xmlWriter.WriteElementString("Condition", binding.Condition);
                }

                xmlWriter.WriteEndElement(); // Binding
            }
            else
            {
                Logger.Warn("Dangling bindings found during save.");
            }
        }

        public void SerializeBindings(HeliosBindingCollection bindings, XmlWriter xmlWriter)
        {
            foreach (HeliosBinding binding in bindings)
            {
                //SerializeBinding(binding, xmlWriter);
                //_skipbindings.Add(binding);
                if (!_skipbindings.Contains(binding))
                {
                    SerializeBinding(binding, xmlWriter);
                    _skipbindings.Add(binding);
                }

            }
        }

        public void SerializeBindings(HeliosBindingCollection bindings, XmlWriter xmlWriter, HeliosBindingCollection skipBindings)
        {
            foreach (HeliosBinding binding in bindings)
            {
                if (!skipBindings.Contains(binding))
                {
                    SerializeBinding(binding, xmlWriter);
                    skipBindings.Add(binding);
                }
            }
        }

        public void SerializeBindings(HeliosVisual visual, XmlWriter xmlWriter)
        {
            SerializeBindings(visual.OutputBindings, xmlWriter);
            foreach (HeliosVisual control in visual.Children)
            {
                SerializeBindings(control, xmlWriter);
            }
        }

        private HeliosBinding DeserializeBinding(HeliosProfile profile, HeliosVisual root, string copyRoot, List<HeliosVisual> localObjects, XmlReader xmlReader)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            HeliosBinding binding = (HeliosBinding)CreateNewObject("Binding", "");
            binding.BypassCascadingTriggers = (bool)boolConverter.ConvertFromString(null, CultureInfo.InvariantCulture, xmlReader.GetAttribute("BypassCascadingTriggers"));
            if(xmlReader.GetAttribute("LuaScriptUTF8Encoding") != null)
            {
                binding.LuaScriptUTF8Encoding = (bool)boolConverter.ConvertFromString(null, CultureInfo.InvariantCulture, xmlReader.GetAttribute("LuaScriptUTF8Encoding"));
            }
            xmlReader.ReadStartElement("Binding");

            string sourcePath = xmlReader.GetAttribute("Source");
            // Logger.Debug("Creating binding from {HeliosPath}", sourcePath);

            HeliosObject source = ResolveReferenceName(profile, root, copyRoot, localObjects, sourcePath);
            if (source != null)
            {
                string trigger = xmlReader.GetAttribute("Name");
                if (source is IDynamicBindings dynamic)
                {
                    binding.Trigger = dynamic.ResolveTrigger(trigger);
                }
                else if (source.Triggers.ContainsKey(trigger))
                {
                    binding.Trigger = source.Triggers[trigger];
                }
                else if (source is HeliosVisual visual)
                {
                    HeliosVisual parent = visual.Parent;
                    if (parent.Triggers.ContainsKey(trigger))
                    {
                        source = parent;
                        binding.Trigger = source.Triggers[trigger];
                    }
                }

                if (binding.Trigger == null)
                {
                    Logger.Error("Binding source trigger {Name} not found at path {HeliosPath}", trigger, sourcePath);
                }
            } 
            else
            {
                Logger.Error("Binding source reference unresolved: {HeliosPath}", sourcePath);
            }

            xmlReader.Read();
            string targetPath = xmlReader.GetAttribute("Target");
            // Logger.Debug("Creating binding to {HeliosPath}", targetPath);

            HeliosObject target = ResolveReferenceName(profile, root, copyRoot, localObjects, targetPath);
            if (target != null)
            {
                string action = xmlReader.GetAttribute("Name");
                if (target is IDynamicBindings dynamic)
                {
                    binding.Action = dynamic.ResolveAction(action);
                }
                else if (target.Actions.ContainsKey(action))
                {
                    binding.Action = target.Actions[action];
                }
                else if (target is HeliosVisual visual)
                {
                    HeliosVisual parent = visual.Parent;
                    if (parent.Actions.ContainsKey(action))
                    {
                        target = parent;
                        binding.Action = target.Actions[action];
                    }
                }
                if (binding.Action == null)
                {
                    // keep this message short, because it may end up in Control Center console
                    Logger.Error("Binding action {Name} not found at {HeliosPath}", action, targetPath);
                }
            }
            else
            {
                Logger.Error("Binding target reference unresolved: {HeliosPath}", targetPath);
            }
            xmlReader.Read();
            switch (xmlReader.Name)
            {
                case "StaticValue":
                    binding.ValueSource = BindingValueSources.StaticValue;
                    binding.Value = xmlReader.ReadElementString("StaticValue");
                    break;
                case "TriggerValue":
                    binding.ValueSource = BindingValueSources.TriggerValue;
                    xmlReader.Read();
                    break;
                case "LuaScript":
                    binding.ValueSource = BindingValueSources.LuaScript;
                    binding.Value = xmlReader.ReadElementString("LuaScript");
                    break;
            }

            if (xmlReader.Name.Equals("Condition"))
            {
                binding.Condition = xmlReader.ReadElementString("Condition");
            }

            xmlReader.ReadEndElement();

            return binding;
        }

        private IEnumerable<HeliosBinding> DeserializeBindings(HeliosProfile profile, HeliosVisual root, string copyRoot, List<HeliosVisual> localObjects, XmlReader xmlReader)
        {
            if (!xmlReader.IsEmptyElement)
            {
                xmlReader.ReadStartElement("Bindings");
                while (xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    HeliosBinding binding = DeserializeBinding(profile, root, copyRoot, localObjects, xmlReader);
                    if (binding?.Action != null && binding.Trigger != null)
                    {
                        yield return binding;
                    }
                    else
                    {
                        Logger.Warn("Dangling binding discarded on Profile load");
                    }
                }
                xmlReader.ReadEndElement();
            }
            else
            {
                xmlReader.Read();
            }
        }

        public IEnumerable<HeliosBinding> DeserializeBindings(HeliosProfile profile, List<HeliosVisual> localObjects, XmlReader xmlReader)
        {
            return DeserializeBindings(profile, null, null, localObjects, xmlReader);
        }

        public IEnumerable<HeliosBinding> DeserializeBindings(HeliosVisual root, string copyRoot, List<HeliosVisual> localObjects, XmlReader xmlReader)
        {
            return DeserializeBindings(root.Profile, root, copyRoot, localObjects, xmlReader);
        }

        #endregion

        #region References

        private static List<HeliosVisual> EMPTYLOCALS = new List<HeliosVisual>();

        public static HeliosObject ResolveReferenceName(HeliosProfile profile, string reference)
        {
            return ResolveReferenceName(profile, null, null, EMPTYLOCALS, reference);
        }

        public static HeliosObject ResolveReferenceName(HeliosVisual root, string copyRoot, string reference)
        {
            return ResolveReferenceName(root.Profile, root, copyRoot, EMPTYLOCALS, reference);
        }

        private static HeliosObject ResolveReferenceName(HeliosProfile profile, HeliosVisual root, string copyRoot, List<HeliosVisual> localObjects, string reference)
        {
            string[] components = Tokenizer.TokenizeAtLeast(reference.StartsWith("{") ? reference.Substring(1, reference.Length - 2) : reference, 4, ';');
            string refType = components[0];
            string path = components[1];
            string typeId = components[2];
            string name = components[3];

            switch (refType)
            {
                case "Visual":

                    HeliosVisual visual = null;
                    if (!string.IsNullOrWhiteSpace(copyRoot) && !path.Equals(copyRoot, StringComparison.InvariantCultureIgnoreCase) && path.StartsWith(copyRoot, StringComparison.InvariantCultureIgnoreCase))
                    {
                        visual = GetVisualByPath(localObjects, path.Substring(copyRoot.Length + 1));
                    }

                    if (visual == null)
                    {
                        if (root == null)
                        {
                            visual = GetVisualByPath(profile, path);
                        }
                        else
                        {
                            visual = GetVisualByPath(root, path);
                        }
                    }
                    return visual;

                case "Interface":
                    if (profile.Interfaces.ContainsKey(name))
                    {
                        return profile.Interfaces[name];
                    }
                    break;
            }

            return null;
        }

        private static HeliosVisual GetVisualByPath(HeliosProfile profile, string path)
        {
            HeliosVisual visual = null;
            foreach (HeliosVisual monitor in profile.Monitors)
            {
                visual = GetVisualByPath(monitor, path);
                if (visual != null) break;
            }
            return visual;
        }

        private static HeliosVisual GetVisualByPath(HeliosVisual visual, string path)
        {
            return GetVisualByPath(visual, new Queue<string>(path.Split('.')));
        }

        private static HeliosVisual GetVisualByPath(HeliosVisual visual, Queue<string> nameQueue)
        {
            HeliosVisual returnValue = null;

            string name = nameQueue.Dequeue();
            if (visual.Name.Equals(name))
            {
                if (nameQueue.Count == 0)
                {
                    returnValue = visual;
                }
                else
                {
                    string childName = nameQueue.Peek();
                    foreach (HeliosVisual child in visual.Children)
                    {
                        if (childName.Equals(child.Name))
                        {
                            returnValue = GetVisualByPath(child, nameQueue);
                            if (returnValue != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        private static HeliosVisual GetVisualByPath(List<HeliosVisual> visuals, string path)
        {
            HeliosVisual returnVisual = null;
            foreach (HeliosVisual child in visuals)
            {
                returnVisual = GetVisualByPath(child, path);
                if (returnVisual != null) break;
            }
            return returnVisual;
        }

        private static bool ComparePaths(string path1, string path2)
        {
            List<string> path1Components = new List<string>(path1.Split('.'));
            List<string> path2Components = new List<string>(path2.Split('.'));

            path1Components.Reverse();
            path2Components.Reverse();

            for (int i = 0; i < path1Components.Count && i < path2Components.Count; i++)
            {
                if (!path1Components[i].Equals(path2Components[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetReferenceName(HeliosObject refObject)
        {
            StringBuilder sb = new StringBuilder("");

            if (refObject is HeliosInterface refInterface)
            {
                sb.Append("Interface;;");
            }

            if (refObject is HeliosVisual refControl)
            {
                sb.Append("Visual;");
                sb.Append(GetVisualPath(refControl));
                sb.Append(";");
            }

            sb.Append(refObject.TypeIdentifier);
            sb.Append(";");
            sb.Append(refObject.Name);
            sb.Append("");

            return sb.ToString();
        }

        /// <summary>
        /// generate a very expensive descriptive path to the object provided, not usable for
        /// binding or any functionality other than UI and logging
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetDescriptivePath(HeliosObject source)
        {
            switch (source)
            {
                case HeliosVisual visual:
                    return GetVisualPath(visual);
                case HeliosInterface heliosInterface:
                    return $"Interface '{heliosInterface.Name}'";
                default:
                    return $"{source.TypeIdentifier} '{source.Name}'";
            }
        }

        public static string GetVisualPath(HeliosVisual visual)
        {
            List<string> names = new List<string>();
            HeliosVisual pathItem = visual;
            while (pathItem != null)
            {
                names.Add(pathItem.Name);
                pathItem = pathItem.Parent;
            }
            names.Reverse();
            return string.Join(".", names);
        }

        #endregion

        #region Interfaces

        public void SerializeInterface(HeliosInterface heliosInterface, HashSet<HeliosInterface> serialized, XmlWriter xmlWriter)
        {
            if (serialized.Contains(heliosInterface))
            {
                // already done
                return;
            }
            // stop any loops
            serialized.Add(heliosInterface);
            if (heliosInterface.ParentInterface != null)
            {
                // recurse
                SerializeInterface(heliosInterface.ParentInterface, serialized, xmlWriter);
            }
            xmlWriter.WriteStartElement("Interface");
            xmlWriter.WriteAttributeString("TypeIdentifier", heliosInterface.TypeIdentifier);
            xmlWriter.WriteAttributeString("Name", heliosInterface.Name);
            if (heliosInterface.UnsupportedSeverity != default)
            {
                xmlWriter.WriteAttributeString("UnsupportedSeverity", heliosInterface.UnsupportedSeverity.ToString());
            }
            heliosInterface.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement(); // Interface
        }

        public IEnumerable<string> DeserializeInterface(HeliosInterfaceCollection destination, XmlReader xmlReader)
        {
            string interfaceType = xmlReader.GetAttribute("TypeIdentifier");

            ComponentUnsupportedSeverity unsupportedSeverity = ReadUnsupportedSeverity(xmlReader);
            HeliosInterface heliosInterface = (HeliosInterface)CreateNewInterface(interfaceType, destination, unsupportedSeverity);
            if (heliosInterface != null)
            {
                string name = xmlReader.GetAttribute("Name");
                if (xmlReader.IsEmptyElement)
                {
                    // don't read from empty XML element
                    xmlReader.Read();
                }
                else
                {
                    xmlReader.ReadStartElement("Interface");
                    heliosInterface.ReadXml(xmlReader);
                    xmlReader.ReadEndElement();
                }
                heliosInterface.Name = name;
                heliosInterface.UnsupportedSeverity = unsupportedSeverity;
                destination.Add(heliosInterface);
                yield return $"loaded {heliosInterface.TypeIdentifier} {heliosInterface.Name}";
            }
            else
            {
                xmlReader.Skip();
                yield return "failed to load interface";
            }
        }

        private ComponentUnsupportedSeverity ReadUnsupportedSeverity(XmlReader xmlReader)
        {
            return Enum.TryParse(xmlReader.GetAttribute("UnsupportedSeverity"), out ComponentUnsupportedSeverity severity) ? severity : default;
        }

        public void SerializeInterfaces(HeliosInterfaceCollection interfaces, XmlWriter xmlWriter)
        {
            // track what interfaces are already serialized
            HashSet<HeliosInterface> serialized = new HashSet<HeliosInterface>();

            // now write them, with any parent interfaces emitted first by recursion
            xmlWriter.WriteStartElement("Interfaces");
            foreach (HeliosInterface heliosInterface in interfaces)
            {
                SerializeInterface(heliosInterface, serialized, xmlWriter);
            }
            xmlWriter.WriteEndElement(); // Interfaces
        }

        public IEnumerable<string> DeserializeInterfaces(HeliosInterfaceCollection destination, XmlReader xmlReader)
        {
            if (!xmlReader.IsEmptyElement)
            {
                DiscoveredAliases = new Dictionary<string, HeliosInterfaceDescriptor>();
                xmlReader.ReadStartElement("Interfaces");
                while (xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    foreach (string progress in DeserializeInterface(destination, xmlReader))
                    {
                        yield return progress;
                    }
                }
                xmlReader.ReadEndElement();

                // now allow use of aliases to resolve types
                foreach (KeyValuePair<string, HeliosInterfaceDescriptor> pair in DiscoveredAliases)
                {
                    ConfigManager.ModuleManager.InterfaceDescriptors.AddAlias(pair.Key, pair.Value);
                }
            }
            else
            {
                xmlReader.Read();
            }

            yield return "loaded interfaces";
        }

        #endregion

        #region Profiles

        public void SerializeProfile(HeliosProfile profile, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("HeliosProfile");
            xmlWriter.WriteElementString("Version", "3");

            SerializeMonitors(profile.Monitors, xmlWriter);

            SerializeInterfaces(profile.Interfaces, xmlWriter);

            xmlWriter.WriteStartElement("Bindings");
            foreach (HeliosInterface heliosInterface in profile.Interfaces)
            {
                SerializeBindings(heliosInterface.OutputBindings, xmlWriter);
            }

            foreach (Monitor visual in profile.Monitors)
            {
                SerializeBindings(visual, xmlWriter);
            }

            xmlWriter.WriteEndElement();  // Bindings

            xmlWriter.WriteEndElement(); // HeliosProfile
        }

        public int GetProfileVersion(XmlReader xmlReader)
        {
            xmlReader.ReadStartElement("HeliosProfile");
            if (xmlReader.Name.Equals("Version"))
            {
                string version = xmlReader.ReadElementString("Version");
                if (version.Contains('.'))
                {
                    return 3;
                }
                return int.Parse(version, CultureInfo.InvariantCulture);
            }

            return 0;
        }

        /// <summary>
        /// enumeration reports a string status item for each significant portion of work
        ///
        /// caller can choose to deserialize profile in parallel with other work by enumerating
        /// this enumeration, since profile deserializing suspends after each item
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public IEnumerable<string> DeserializeProfile(HeliosProfile profile, XmlReader xmlReader)
        {
            profile.Monitors.Clear();
            foreach (string progress in DeserializeMonitors(profile.Monitors, xmlReader))
            {
                yield return progress;
            }

            profile.Interfaces.Clear();
            foreach (string progress in DeserializeInterfaces(profile.Interfaces, xmlReader))
            {
                yield return progress;
            }

            foreach (HeliosBinding binding in DeserializeBindings(profile, new List<HeliosVisual>(), xmlReader))
            {
                binding.Trigger.Source.OutputBindings.Add(binding);
                binding.Action.Target.InputBindings.Add(binding);
                yield return $"loaded binding {binding.Description}";
            }

            xmlReader.ReadEndElement();
            yield return "loaded profile";
        }

        #endregion
    }
}
