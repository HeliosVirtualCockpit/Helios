//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later versionCannot find interface trigger
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Linq;

namespace GadrocsWorkshop.Helios
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;

    // for inputs, a trigger on the interface creates an action on the device
    public struct DefaultInputBinding
    {
        public string ChildName, InterfaceTriggerName, DeviceActionName, DeviceTriggerName;
        public BindingValue DeviceTriggerBindingValue;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DefaultInputBinding(string childName, string interfaceTriggerName, string deviceActionName)
        {
            ChildName = childName;
            InterfaceTriggerName = interfaceTriggerName;
            DeviceTriggerName = "";
            DeviceActionName = deviceActionName;
            DeviceTriggerBindingValue = null;
            Logger.Info("Default Input Binding: Trigger " + interfaceTriggerName + " to action " + deviceActionName + " for child " + childName);
        }
        /// <summary>
        /// Contains information to allow AutoBinding in the onProfileChanged method.  
        /// </summary>
        /// <param name="childName">Name of the device which has the action</param>
        /// <param name="interfaceTriggerName">The name of the trigger when that trigger is from an interface</param>
        /// <param name="deviceActionName">The name of the action to be performed</param>
        /// <param name="deviceTriggerName">The name of the trigger if it is from a visual component</param>
        /// <param name="deviceTriggerBindingValue">Static trigger value which only used with deviceTriggerValue</param>
        public DefaultInputBinding(string childName, string interfaceTriggerName, string deviceActionName, string deviceTriggerName, BindingValue deviceTriggerBindingValue)
        {
            ChildName = childName;
            InterfaceTriggerName = "";
            DeviceTriggerName = deviceTriggerName;
            DeviceActionName = deviceActionName;
            DeviceTriggerBindingValue = deviceTriggerBindingValue; 
            Logger.Info("Default Input Binding: Trigger " + deviceTriggerName + " to action " + deviceActionName + " for child " + childName);
        }
    }

    // for output, a triggeer on the device leads to an action on the interface
    public struct DefaultOutputBinding
    {
        public string ChildName, DeviceTriggerName, InterfaceActionName;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DefaultOutputBinding(string childName, string deviceTriggerName, string interfaceActionName)
        {
            ChildName = childName;
            DeviceTriggerName = deviceTriggerName;
            InterfaceActionName = interfaceActionName;
            Logger.Info("Default Output Binding: Trigger " + deviceTriggerName + " to action " + interfaceActionName + " for child " + childName);
        }
    }

    /// <summary>
    /// for binding between actions and triggers withing the CompositeVisual
    /// </summary>
    public struct DefaultSelfBinding
    {
        public string TriggerChildName, DeviceTriggerName, ActionChildName, DeviceActionName;
        public BindingValue DeviceTriggerBindingValue;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DefaultSelfBinding(string triggerChildName, string deviceTriggerName, string actionChildName, string deviceActionName) {
            TriggerChildName = triggerChildName;
            DeviceTriggerName = deviceTriggerName;
            ActionChildName = actionChildName;
            DeviceActionName = deviceActionName;
            DeviceTriggerBindingValue = null;
            Logger.Info($"Default Self Binding: Trigger {triggerChildName}.{deviceTriggerName} to action {deviceActionName} for child {actionChildName}");
        }

        public DefaultSelfBinding(string triggerChildName, string deviceTriggerName, string actionChildName, string deviceActionName, BindingValue deviceTriggerBindingValue)
        {
            TriggerChildName = triggerChildName;
            DeviceTriggerName = deviceTriggerName;
            ActionChildName = actionChildName;
            DeviceActionName = deviceActionName;
            DeviceTriggerBindingValue = deviceTriggerBindingValue;
            Logger.Info($"Default Self Binding: Trigger {triggerChildName}.{deviceTriggerName} to action {deviceActionName} for child {actionChildName}");
        }
    }

    public abstract class CompositeVisual : HeliosVisual
    {
        private Dictionary<HeliosVisual, Rect> _nativeSizes = new Dictionary<HeliosVisual, Rect>();
        protected List<DefaultOutputBinding> _defaultOutputBindings;
        protected List<DefaultInputBinding> _defaultInputBindings;
        protected List<DefaultSelfBinding> _defaultSelfBindings;
        protected string _defaultBindingName;   // the name of the default binding in the interface
        protected HeliosInterface _defaultInterface;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Type[] SupportedInterfaces { get; protected set; } = new Type[0];
        public string[] SupportedInterfaceNames { get; protected set; } = new string[0];  // used only for SoftInterfaces

        protected CompositeVisual(string name, Size nativeSize)
            : base(name, nativeSize)
        {
            PersistChildren = false;
            Children.CollectionChanged += Children_CollectionChanged;
            _defaultBindingName = "";
            _defaultInterface = null;
            _defaultInputBindings = new List<DefaultInputBinding>();
            _defaultOutputBindings = new List<DefaultOutputBinding>();
            _defaultSelfBindings = new List<DefaultSelfBinding>();
        }

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) ||
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (HeliosVisual control in e.NewItems)
                {
                    if (!_nativeSizes.ContainsKey(control))
                    {
                        _nativeSizes.Add(control, new Rect(control.Left, control.Top, control.Width, control.Height));
                    }
                }
            }

            if ((e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) ||
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (HeliosVisual control in e.OldItems)
                {
                    if (_nativeSizes.ContainsKey(control))
                    {
                        _nativeSizes.Remove(control);
                    }
                }
            }
        }

        #region Properties

        public string DefaultBindingName
        {
            set
            {
                _defaultBindingName = value;
            }
        }

        public List<DefaultInputBinding> DefaultInputBindings
        {
            get
            {
                return _defaultInputBindings;
            }
        }
        public List<DefaultOutputBinding> DefaultOutputBindings
        {
            get
            {
                return _defaultOutputBindings;
            }
        }

        public List<DefaultSelfBinding> DefaultSelfBindings
        {
            get
            {
                return _defaultSelfBindings;
            }
        }

        #endregion

        public override void Reset()
        {
            base.Reset();
            foreach (HeliosVisual child in Children)
            {
                child.Reset();
            }
        }

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                double scaleX = Width / NativeSize.Width;
                double scaleY = Height / NativeSize.Height;
                ScaleChildrenInt(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }

        private void ScaleChildrenInt(double scaleX, double scaleY)
        {
            foreach (KeyValuePair<HeliosVisual, Rect> item in _nativeSizes)
            {
                if (item.Value.Left > 0)
                {
                    double locXDif = item.Value.Left;
                    item.Key.Left = item.Value.Left + (locXDif * scaleX) - locXDif;
                }
                item.Key.Width = Math.Max(item.Value.Width * scaleX, 1d);
                if (item.Value.Top > 0)
                {
                    double locYDif = item.Value.Top;
                    item.Key.Top = item.Value.Top + (locYDif * scaleY) - locYDif;
                }
                item.Key.Height = Math.Max(item.Value.Height * scaleY, 1d);
                if (GlobalOptions.HasScaleAllText)
                {
                    switch (item.Key.TypeIdentifier)
                    {
                        // These scaling operations on fonts rely on the ConfiguredFontSize of the button being the initial fontsize, 
                        // so that multiple scalings, either up or down, are done with reference to the starting fontsize rather than
                        // the current font being used.  It is therefore important that when the child is created, the 
                        // TextFormat.ConfiguredFontSize should be set to the TextFormat.FontSize.
                        case "Helios.Base.PushButton":
                            PushButton pb = item.Key as PushButton;
                            if (pb.TextFormat != null)
                            {
                                pb.TextFormat.FontSize = Clamp(pb.TextFormat.ConfiguredFontSize * scaleY, 1, 2000);
                            }
                            break;
                        case "Helios.Base.RockerSwitch":
                            RockerSwitch rs = item.Key as RockerSwitch;
                            if (rs.TextFormat != null)
                            {
                                rs.TextFormat.FontSize = Clamp(rs.TextFormat.ConfiguredFontSize * scaleY, 1, 2000);
                            }
                            break;
                        case "Helios.Base.Text":
                            Controls.TextDecoration td = item.Key as Controls.TextDecoration;
                            if (td.Format != null)
                            {
                                td.Format.FontSize = Clamp(td.Format.ConfiguredFontSize * scaleY, 1, 2000);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected virtual void AddDefaultInputBinding(string childName, string interfaceTriggerName, string deviceActionName)
        {
            DefaultInputBindings.Add(new DefaultInputBinding(
                childName: childName,
                interfaceTriggerName: interfaceTriggerName,
                deviceActionName: deviceActionName
                ));
        }

        protected virtual void AddDefaultOutputBinding(string childName, string deviceTriggerName, string interfaceActionName)
        {
            DefaultOutputBindings.Add(new DefaultOutputBinding(
                childName: childName,
                deviceTriggerName: deviceTriggerName,
                interfaceActionName: interfaceActionName
                ));
        }

        protected virtual void AddDefaultSelfBinding(string triggerChildName, string deviceTriggerName, string actionChildName, string deviceActionName)
        {
            AddDefaultSelfBinding(triggerChildName, deviceTriggerName, actionChildName, deviceActionName, null);
        }
        protected virtual void AddDefaultSelfBinding(string triggerChildName, string deviceTriggerName, string actionChildName, string deviceActionName, BindingValue deviceBindingValue)
        {
            DefaultSelfBindings.Add(new DefaultSelfBinding(
                triggerChildName: triggerChildName,
                deviceTriggerName: deviceTriggerName,
                actionChildName: actionChildName,
                deviceActionName: deviceActionName,
                deviceTriggerBindingValue: deviceBindingValue
                ));
        }

        protected HeliosBinding CreateNewBinding(IBindingTrigger trigger, IBindingAction action)
        {
           return CreateNewBinding(trigger, action, new BindingValue(null));
        }
        private HeliosBinding CreateNewBinding(IBindingTrigger trigger, IBindingAction action, BindingValue bindingValue)
        {
            HeliosBinding binding = new HeliosBinding(trigger, action);

            binding.BypassCascadingTriggers = true;

            if (action.ActionRequiresValue && (ConfigManager.ModuleManager.CanConvertUnit(trigger.Unit, action.Unit)))
            {
                binding.ValueSource = BindingValueSources.TriggerValue;
            }
            else
            {
                binding.ValueSource = BindingValueSources.StaticValue;
                if (bindingValue is null)
                {
                } else
                {
                    if (action.Unit.ShortName   == "Boolean")
                    {
                        binding.Value = bindingValue.BoolValue ? "True" : "False";
                    } else
                    {
                        binding.Value = bindingValue.StringValue;
                    }
                }
            }
            return binding;
        }

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
            if (!DesignMode || (oldProfile != null && !oldProfile.DesignMode))
                return;

            // grab the default interface, if it exists
            foreach (Type supportedInterface in SupportedInterfaces)
            {
                _defaultInterface = Profile.Interfaces.FirstOrDefault(i => supportedInterface.IsInstanceOfType(i));
                if (_defaultInterface != null)
                {
                    if(_defaultInterface.GetType() == typeof(Interfaces.DCS.Soft.SoftInterface))
                    {
                        // need additional check for SoftInterfaces
                        bool softInterfaceFound = false;
                        foreach(string supportedInterfaceName in SupportedInterfaceNames)
                        {
                            if(_defaultInterface.Name == supportedInterfaceName)
                            {
                                softInterfaceFound = true;
                                break;
                            }
                        }
                        if (!softInterfaceFound)
                        {
                            _defaultInterface = null;
                            break;
                        }
                    }
                    Logger.Info($"{Name} auto binding to interface '{_defaultInterface.Name}'");
                    break;
                }
            }

            if (_defaultInterface == null)
            {
                Logger.Info($"{Name} could not locate any supported interface for auto binding");
                return;
            }

            // looping for all default input bindings to assign the value
            foreach (DefaultInputBinding defaultBinding in _defaultInputBindings)
            {
                if (!Children.ContainsKey(defaultBinding.ChildName))
                {
                    Logger.Error("Cannot find child " + defaultBinding.ChildName);
                    continue;
                }
                Logger.Debug("Auto binding child " + defaultBinding.ChildName);
                HeliosVisual child = Children[defaultBinding.ChildName];
                if (!child.Actions.ContainsKey(defaultBinding.DeviceActionName))
                {
                    Logger.Error("Cannot find action " + defaultBinding.DeviceActionName);
                    continue;
                }
                if (defaultBinding.InterfaceTriggerName != "")
                {
                    if (!_defaultInterface.Triggers.ContainsKey(defaultBinding.InterfaceTriggerName))
                    {
                        Logger.Error("Cannot find interface trigger " + defaultBinding.InterfaceTriggerName);
                        continue;
                    }

                    Logger.Debug("Auto binding interface trigger " + defaultBinding.InterfaceTriggerName + " to " + defaultBinding.ChildName + "." + defaultBinding.DeviceActionName);
                    child.OutputBindings.Add(CreateNewBinding(_defaultInterface.Triggers[defaultBinding.InterfaceTriggerName],
                        child.Actions[defaultBinding.DeviceActionName]));
                } else
                {

                    if (!Triggers.ContainsKey(defaultBinding.DeviceTriggerName))
                    {
                        Logger.Error("Cannot find interface trigger " + defaultBinding.DeviceTriggerName);
                        continue;
                    }

                    Logger.Debug("Auto binding device trigger " + defaultBinding.DeviceTriggerName + " to " + defaultBinding.ChildName + "." + defaultBinding.DeviceActionName);
                    child.OutputBindings.Add(CreateNewBinding(Triggers[defaultBinding.DeviceTriggerName],
                        child.Actions[defaultBinding.DeviceActionName],defaultBinding.DeviceTriggerBindingValue));

                }
            }

            // now looping for all default output bindings to assign the value
            foreach (DefaultOutputBinding defaultBinding in _defaultOutputBindings)
            {
                if (!Children.ContainsKey(defaultBinding.ChildName))
                {
                    Logger.Error("Cannot find child " + defaultBinding.ChildName);
                    continue;
                }
                HeliosVisual child = Children[defaultBinding.ChildName];
                if (!child.Triggers.ContainsKey(defaultBinding.DeviceTriggerName))
                {
                    Logger.Error("Cannot find trigger " + defaultBinding.DeviceTriggerName);
                    continue;
                }
                if (!_defaultInterface.Actions.ContainsKey(defaultBinding.InterfaceActionName))
                {
                    Logger.Error("Cannot find action " + defaultBinding.InterfaceActionName);
                    continue;
                }
                Logger.Debug("Child Output binding trigger " + defaultBinding.DeviceTriggerName + " to " + defaultBinding.InterfaceActionName);
                child.OutputBindings.Add(CreateNewBinding(child.Triggers[defaultBinding.DeviceTriggerName],
                                      _defaultInterface.Actions[defaultBinding.InterfaceActionName]));

            }

            // now looping for all default self bindings to link trigger with actions
            foreach (DefaultSelfBinding defaultBinding in _defaultSelfBindings)
            {
                if (defaultBinding.TriggerChildName != "" && !Children.ContainsKey(defaultBinding.TriggerChildName))
                {
                    Logger.Error($"Cannot find Trigger child for Self Bind {defaultBinding.TriggerChildName}");
                    continue;
                }
                if (defaultBinding.ActionChildName != "" && !Children.ContainsKey(defaultBinding.ActionChildName))
                {
                    Logger.Error($"Cannot find Action child for Self Bind {defaultBinding.ActionChildName}");
                    continue;
                }
                HeliosVisual triggerChild = defaultBinding.TriggerChildName != "" ? Children[defaultBinding.TriggerChildName]: this;
                HeliosVisual actionChild = defaultBinding.ActionChildName != "" ? Children[defaultBinding.ActionChildName]: this;
                if (!triggerChild.Triggers.ContainsKey(defaultBinding.DeviceTriggerName))
                {
                    Logger.Error("Cannot find trigger " + defaultBinding.DeviceTriggerName);
                    continue;
                }
                if (!actionChild.Actions.ContainsKey(defaultBinding.DeviceActionName))
                {
                    Logger.Error("Cannot find action " + defaultBinding.DeviceActionName);
                    continue;
                }
                Logger.Debug($"Child Self binding trigger {defaultBinding.TriggerChildName} {defaultBinding.DeviceTriggerName} to action {defaultBinding.ActionChildName} {defaultBinding.DeviceActionName}");
                actionChild.InputBindings.Add(CreateNewBinding(triggerChild.Triggers[defaultBinding.DeviceTriggerName],
                                      actionChild.Actions[defaultBinding.DeviceActionName]));

            }

        }

        private Point FromCenter(Point posn, Size size)
        {
            return new Point(posn.X - size.Width / 2.0, posn.Y - size.Height / 2.0);
        }

        protected void AddTrigger(IBindingTrigger trigger, string device)
        {
            trigger.Device = device;
            Triggers.Add(trigger);
        }

        protected void AddAction(IBindingAction action, string device)
        {
            action.Device = device;
            Actions.Add(action);
        }

        protected string GetComponentName(string name)
        {
            return Name + "_" + name;
        }

        protected Potentiometer AddPot(string name, Point posn, Size size, string knobImage,
            double initialRotation, double rotationTravel, double minValue, double maxValue,
            double initialValue, double stepValue,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter, RotaryClickType clickType = RotaryClickType.Swipe, bool isContinuous = false)
        {
            string componentName = GetComponentName(name);
            if (fromCenter)
                posn = FromCenter(posn, size);
            Potentiometer knob = new Potentiometer
            {
                Name = componentName,
                KnobImage = knobImage,
                InitialRotation = initialRotation,
                RotationTravel = rotationTravel,
                MinValue = minValue,
                MaxValue = maxValue,
                InitialValue = initialValue,
                StepValue = stepValue,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                ClickType = clickType,
                IsContinuous = isContinuous
            };

            Children.Add(knob);
            foreach (IBindingTrigger trigger in knob.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(knob.Actions["set.value"], componentName);

            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "value.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName
           );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.value");
            return knob;
        }

        protected RotaryEncoder AddEncoder(string name, Point posn, Size size,
            string knobImage, double stepValue, double rotationStep,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter, RotaryClickType clickType = RotaryClickType.Swipe)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);
            RotaryEncoder knob = new RotaryEncoder
            {
                Name = componentName,
                KnobImage = knobImage,
                StepValue = stepValue,
                RotationStep = rotationStep,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                ClickType = clickType
            };

            Children.Add(knob);
            foreach (IBindingTrigger trigger in knob.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in knob.Actions)
            {
                AddAction(action, componentName);
            }
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "encoder.incremented",
                interfaceActionName: interfaceDeviceName + ".increment." + interfaceElementName
            );
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "encoder.decremented",
                interfaceActionName: interfaceDeviceName + ".decrement." + interfaceElementName
                );

            return knob;
        }

        protected RotarySwitch AddRotarySwitch(string name, Point posn, Size size,
            string knobImage, int defaultPosition, //Helios.Gauges.M2000C.RSPositions[] positions,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter)
        {
            RotarySwitchPositionCollection positions = new RotarySwitchPositionCollection();
            return AddRotarySwitch(name, posn, size,
              knobImage, defaultPosition, positions,
              interfaceDeviceName, interfaceElementName, fromCenter);
        }
        protected RotarySwitch AddRotarySwitch(string name, Point posn, Size size,
            string knobImage, int defaultPosition, RotarySwitchPositionCollection positions,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);
            RotarySwitch knob = new RotarySwitch
            {
                Name = componentName,
                KnobImage = knobImage,
                DrawLabels = false,
                DrawLines = false,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = defaultPosition
            };
            knob.Positions.Clear();
            knob.DefaultPosition = defaultPosition;
            foreach(RotarySwitchPosition swPosn in positions)
            {
                knob.Positions.Add(swPosn);
            }
            
            Children.Add(knob);

            foreach (IBindingTrigger trigger in knob.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(knob.Actions["set.position"], componentName);

            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");

            return knob;
        }

        protected void AddRotarySwitchBindings(string name, Point posn, Size size, RotarySwitch rotarySwitch,
            string interfaceDeviceName, string interfaceElementName)
        {
            string componentName = GetComponentName(name);
            Children.Add(rotarySwitch);

            foreach (IBindingTrigger trigger in rotarySwitch.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(rotarySwitch.Actions["set.position"], componentName);

            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");
        }

        protected PushButton AddButton(string name, Point posn, Size size, string image, string pushedImage,
            string buttonText, string interfaceDeviceName, string interfaceElementName, bool fromCenter)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);
            PushButton button = new PushButton
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                Image = image,
                PushedImage = pushedImage,
                Text = buttonText,
                Name = componentName
            };
            button.TextFormat.ConfiguredFontSize = button.TextFormat.FontSize;
            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], componentName);
            AddTrigger(button.Triggers["released"], componentName);

            AddAction(button.Actions["push"], componentName);
            AddAction(button.Actions["release"], componentName);
            AddAction(button.Actions["set.physical state"], componentName);

            // add the default actions
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "pushed",
                interfaceActionName: interfaceDeviceName + ".push." + interfaceElementName
                );
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "released",
                interfaceActionName: interfaceDeviceName + ".release." + interfaceElementName
                );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.physical state");

            return button;
        }

        protected Indicator AddIndicator(string name, Point posn, Size size,
            string onImage, string offImage, Color onTextColor, Color offTextColor, string font,
            bool vertical, string interfaceDeviceName, string interfaceElementName, bool fromCenter, bool withText = true)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);
            Indicator indicator = new Helios.Controls.Indicator
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                OnImage = onImage,
                OffImage = offImage,
                Name = componentName
            };

            if (withText)
            {
                indicator.Text = name;
                indicator.OnTextColor = onTextColor;
                indicator.OffTextColor = offTextColor;
                indicator.TextFormat.FontStyle = FontStyles.Normal;
                indicator.TextFormat.FontWeight = FontWeights.Normal;
                if (vertical)
                {
                    indicator.TextFormat.FontSize = 8;
                }
                else
                {
                    indicator.TextFormat.FontSize = 12;
                }
                indicator.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName(font);
                indicator.TextFormat.PaddingLeft = 0;
                indicator.TextFormat.PaddingRight = 0;
                indicator.TextFormat.PaddingTop = 0;
                indicator.TextFormat.PaddingBottom = 0;
                indicator.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
                indicator.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
            }
            else
            {
                indicator.Text = "";
            }

            Children.Add(indicator);
            foreach (IBindingTrigger trigger in indicator.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(indicator.Actions["set.indicator"], componentName);

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.indicator");

            return indicator;
        }

        protected ToggleSwitch AddToggleSwitch(string name, Point posn, Size size, ToggleSwitchPosition defaultPosition,
           string positionOneImage, string positionTwoImage, ToggleSwitchType defaultType, string interfaceDeviceName, string interfaceElementName,
           bool fromCenter, bool horizontal = false)
        {
            return AddToggleSwitch(name, posn, size, defaultPosition, positionOneImage, positionTwoImage, defaultType, LinearClickType.Touch, interfaceDeviceName, interfaceElementName,
                fromCenter, horizontal);
        }

        protected ToggleSwitch AddToggleSwitch(string name, Point posn, Size size, ToggleSwitchPosition defaultPosition,
            string positionOneImage, string positionTwoImage, ToggleSwitchType defaultType, LinearClickType clickType, string interfaceDeviceName, string interfaceElementName,
            bool fromCenter, bool horizontal = false)
        {
            if (fromCenter)
            {
                posn = FromCenter(posn, size);
            }
            string componentName = GetComponentName(name);

            ToggleSwitch newSwitch = new ToggleSwitch
            {
                Name = componentName,
                SwitchType = defaultType,
                ClickType = clickType,
                DefaultPosition = defaultPosition,
                PositionOneImage = positionOneImage,
                PositionTwoImage = positionTwoImage,
                Width = size.Width,
                Height = size.Height,
                Top = posn.Y,
                Left = posn.X
            };

            if (horizontal)
            {
                newSwitch.Rotation = HeliosVisualRotation.CW;
                newSwitch.Orientation = ToggleSwitchOrientation.Horizontal;
            }

            Children.Add(newSwitch);

            foreach (IBindingTrigger trigger in newSwitch.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(newSwitch.Actions["set.position"], componentName);

            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");

            return newSwitch;
        }

        protected ThreeWayToggleSwitch AddThreeWayToggle(string name, Point posn, Size size,
            ThreeWayToggleSwitchPosition defaultPosition, ThreeWayToggleSwitchType defaultType,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter,
            string positionOneImage = "{Helios}/Images/Toggles/round-up.png",
            string positionTwoImage = "{Helios}/Images/Toggles/round-norm.png",
            string positionThreeImage = "{Helios}/Images/Toggles/round-down.png",
            LinearClickType clickType = LinearClickType.Swipe,
            bool horizontal = false)
        {
            string componentName = GetComponentName(name);
            ThreeWayToggleSwitch toggle = new ThreeWayToggleSwitch
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = defaultPosition,
                PositionOneImage = positionOneImage,
                PositionTwoImage = positionTwoImage,
                PositionThreeImage = positionThreeImage,
                SwitchType = defaultType,
                Name = componentName,
                ClickType = clickType
            };
            if (horizontal)
            {
                toggle.Rotation = HeliosVisualRotation.CW;
                toggle.Orientation = ToggleSwitchOrientation.Horizontal;
            }

            Children.Add(toggle);
            foreach (IBindingTrigger trigger in toggle.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(toggle.Actions["set.position"], componentName);

            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");

            return toggle;
        }

        protected ThreeWayToggleSwitch AddRocker(string name, Point posn, Size size,
            ThreeWayToggleSwitchPosition defaultPosition, ThreeWayToggleSwitchType defaultType,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter,
            string positionOneImage = "{Helios}/Images/Toggles/round-up.png",
            string positionTwoImage = "{Helios}/Images/Toggles/round-norm.png",
            string positionThreeImage = "{Helios}/Images/Toggles/round-down.png",
            LinearClickType clickType = LinearClickType.Touch,
            bool horizontal = false)
        {
            string componentName = GetComponentName(name);
            ThreeWayToggleSwitch rocker = new ThreeWayToggleSwitch
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = defaultPosition,
                PositionOneImage = positionOneImage,
                PositionTwoImage = positionTwoImage,
                PositionThreeImage = positionThreeImage,
                SwitchType = defaultType,
                Name = componentName,
                ClickType = clickType
            };
            if (horizontal)
            {
                rocker.Rotation = HeliosVisualRotation.CW;
                rocker.Orientation = ToggleSwitchOrientation.Horizontal;
            }

            Children.Add(rocker);
            foreach (IBindingTrigger trigger in rocker.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(rocker.Actions["set.position"], componentName);
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position one.entered",
                interfaceActionName: interfaceDeviceName + ".push up." + interfaceElementName
            );
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position two.entered",
                interfaceActionName: interfaceDeviceName + ".release." + interfaceElementName
            );
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position three.entered",
                interfaceActionName: interfaceDeviceName + ".push down." + interfaceElementName
            );

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");

            return rocker;
        }

        protected IndicatorPushButton AddIndicatorPushButton(string name, Point posn, Size size,
            string image, string pushedImage, Color textColor, Color onTextColor, string font,
            string interfaceDeviceName, string interfaceElementName,
            bool withText = true, TextScalingMode scalingMode = TextScalingMode.Legacy)
        {
            string componentName = GetComponentName(name);
            IndicatorPushButton indicator = new Helios.Controls.IndicatorPushButton
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                Image = image,
                PushedImage = pushedImage,
                Name = componentName,
                OnTextColor = onTextColor,
                TextColor = textColor,
                ScalingMode = scalingMode
            };
            if (withText)
            {
                indicator.TextFormat.FontStyle = FontStyles.Normal;
                indicator.TextFormat.FontWeight = FontWeights.Normal;
                indicator.TextFormat.FontSize = 18;
                indicator.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName(font);
                indicator.TextFormat.PaddingLeft = 0;
                indicator.TextFormat.PaddingRight = 0;
                indicator.TextFormat.PaddingTop = 0;
                indicator.TextFormat.PaddingBottom = 0;
                indicator.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
                indicator.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                indicator.Text = name;
            }
            else
            {
                indicator.Text = "";
            }

            Children.Add(indicator);
            AddTrigger(indicator.Triggers["pushed"], componentName);
            AddTrigger(indicator.Triggers["released"], componentName);

            AddAction(indicator.Actions["push"], componentName);
            AddAction(indicator.Actions["release"], componentName);
            AddAction(indicator.Actions["set.indicator"], componentName);

            // add the default actions
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "pushed",
                interfaceActionName: interfaceDeviceName + ".push." + interfaceElementName
                );
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "released",
                interfaceActionName: interfaceDeviceName + ".release." + interfaceElementName
                );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.physical state");
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.indicator");
            return indicator;
        }

        protected TextDisplay AddTextDisplay(
            string name,
            Point posn,
            Size size,
            string font,
            TextHorizontalAlignment horizontalAlignment,
            TextVerticalAlignment verticalAligment,
            double baseFontsize,
            string testTextDisplay,
            Color textColor,
            Color backgroundColor,
            bool useBackground,
            string interfaceDeviceName,
            string interfaceElementName,
            string textDisplayDictionary
            )
        {
            string componentName = GetComponentName(name);
            TextDisplay display = new TextDisplay
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                Name = componentName
            };
            TextFormat textFormat = new TextFormat
            {
                FontFamily = ConfigManager.FontManager.GetFontFamilyByName(font),
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAligment,
                FontSize = baseFontsize,
                ConfiguredFontSize = baseFontsize,
                PaddingRight = 0,
                PaddingLeft = 0,
                PaddingTop = 0,
                PaddingBottom = 0
            };

            // NOTE: for scaling purposes, we commit to the reference height at the time we set TextFormat, since that indirectly sets ConfiguredFontSize 
            display.TextFormat = textFormat;
            display.OnTextColor = textColor; // Color.FromArgb(0xff, 0x40, 0xb3, 0x29);
            display.BackgroundColor = backgroundColor; // Color.FromArgb(0xff, 0x00, 0x00, 0x00);
            display.UseBackground = useBackground;
            if (textDisplayDictionary.Equals(""))
            {
                display.ParserDictionary = "";
            }
            else
            {
                display.ParserDictionary = textDisplayDictionary;
                display.UseParseDictionary = true;
            }
            display.TextTestValue = testTextDisplay;
            Children.Add(display);
            AddAction(display.Actions["set.TextDisplay"], componentName);

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.TextDisplay");

            return display;
        }

        protected Gauges.BaseGauge AddDisplay(
            string name,
            Gauges.BaseGauge gauge,
            Point posn,
            Size size,
            string interfaceDeviceName,
            string interfaceElementName
            )
        {
            gauge.Name = GetComponentName(name);
            gauge.Top = posn.Y;
            gauge.Left = posn.X;
            gauge.Width = size.Width;
            gauge.Height = size.Height;

            Children.Add(gauge);

            AddAction(gauge.Actions["set.value"], GetComponentName(name));

            AddDefaultInputBinding(
                childName: GetComponentName(name),
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.value");
            return gauge;
        }

        protected CompositeVisual AddDevice(
            string name,
            CompositeVisual device,
            Point posn,
            Size size,
            string interfaceDeviceName,
            string interfaceElementName
            )
        {
            device.Name = GetComponentName(name);
            device.Top = posn.Y;
            device.Left = posn.X;
            device.Width = size.Width;
            device.Height = size.Height;

            Children.Add(device);
            foreach (IBindingTrigger trigger in device.Triggers)
            {
                AddTrigger(trigger, trigger.Device);
            }
            foreach (IBindingAction action in device.Actions)
            {
                if (action.Name == "hidden")
                {
                    continue;
                }

                AddAction(action, action.Device);
                AddDefaultInputBinding(
                    childName: name,
                    deviceActionName: action.ActionVerb + "." + action.Device,
                    interfaceTriggerName: name + "." + action.Device + ".changed"
                );
            }
            return device;
        }

        protected Gauges.BaseGauge AddGauge(
            string name,
            Gauges.BaseGauge gauge,
            Point posn,
            Size size,
            string interfaceDeviceName,
            string interfaceElementName
            )
        {
            gauge.Name = GetComponentName(name);
            gauge.Top = posn.Y;
            gauge.Left = posn.X;
            gauge.Width = size.Width;
            gauge.Height = size.Height;

            Children.Add(gauge);
            foreach (IBindingTrigger trigger in gauge.Triggers)
            {
                AddTrigger(trigger, trigger.Device); 
            }
            foreach (IBindingAction action in gauge.Actions)
            {
                if (action.Name == "hidden")
                {
                    continue;
                }

                AddAction(action, action.Device);
                AddDefaultInputBinding(
                    childName: GetComponentName(name),
                    interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                    deviceActionName: action.Device + "." + action.ActionVerb + "." + action.Name
                );
            }
            return gauge;
        }

        protected HeliosPanel AddPanel(string name, Point posn, Size size, string background)
        {
            HeliosPanel panel = new HeliosPanel
            {
                Left = posn.X,
                Top = posn.Y,
                Width = size.Width,
                Height = size.Height,
                BackgroundImage = background,
                Name = GetComponentName(name)
            };
            Children.Add(panel);
            return panel;
        }
        private double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }
}
