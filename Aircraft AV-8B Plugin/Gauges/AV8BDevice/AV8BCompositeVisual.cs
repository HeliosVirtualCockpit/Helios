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

using System.Windows;
using GadrocsWorkshop.Helios.Controls;
using GadrocsWorkshop.Helios.Gauges;

// ReSharper disable once CheckNamespace
namespace GadrocsWorkshop.Helios
{
    public enum AutoBindActionType { Set,Increment,Decrement};
    public abstract class AV8BCompositeVisual : CompositeVisualWithBackgroundImage
    {
        protected new HeliosInterface _defaultInterface;

        protected AV8BCompositeVisual(string name, Size nativeSize)
            : base(name, nativeSize)
        {
            // all code in base
        }

        //
        // For gauges with more than one input function, we need this override to map the 
        // different interface element names to the individual functions.  This carries with it
        // a requirement for the gauge function name to equal the interface element name
        //

        protected new BaseGauge AddGauge(
            string name,
            BaseGauge gauge,
            Point posn,
            Size size,
            string interfaceDeviceName,
            string interfaceElementName
        ) => AddGauge(name, gauge, posn, size, interfaceDeviceName, new string[1] {interfaceElementName});

        protected BaseGauge AddGauge(
            string name,
            BaseGauge gauge,
            Point posn,
            Size size,
            string interfaceDeviceName,
            string[] interfaceElementNames
        )
        {
            if (interfaceElementNames.Length > 1)
                //if (name == "Altimeter Gauge" || name == "SAI Gauge" || name == "Slip/Turn Gauge" || name == "AOA Gauge")
                {
                gauge.Name = name;
                gauge.Top = posn.Y;
                gauge.Left = posn.X;
                gauge.Width = size.Width;
                gauge.Height = size.Height;

                string componentName = GetComponentName(name);
                gauge.Name = componentName;

                Children.Add(gauge);
                foreach (IBindingTrigger trigger in gauge.Triggers)
                {
                    AddTrigger(trigger, trigger.Device);
                }

                int i = 0;
                foreach (IBindingAction action in gauge.Actions)
                {
                    if (action.Name != "hidden" && !action.Name.Contains("Lighting"))
                    {
                        AddAction(action, action.Device);

                        AddDefaultInputBinding(
                            componentName,
                            interfaceDeviceName + "." + interfaceElementNames[i++] + ".changed",
                            action.Device + "." + action.ActionVerb + "." + action.Name
                        );
                    }
                }

                return gauge;
            }

            return base.AddGauge(
                name,
                gauge,
                posn,
                size,
                interfaceDeviceName,
                interfaceElementNames[0]
            );
        }

        private Point FromCenter(Point posn, Size size) =>
            new Point(posn.X - size.Width / 2.0, posn.Y - size.Height / 2.0);

        protected RotaryEncoder AddEncoder(string name, Point posn, Size size,
            string knobImage, double stepValue, double rotationStep,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter,
            RotaryClickType clickType = RotaryClickType.Swipe, AutoBindActionType bindType = AutoBindActionType.Increment)
        {
            if (fromCenter)
            {
                posn = FromCenter(posn, size);
            }

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
                if (action.Name != "hidden")
                {
                    AddAction(action, componentName);
                }
            }

            if (bindType == AutoBindActionType.Increment || bindType == AutoBindActionType.Decrement)
            {
                AddDefaultOutputBinding(
                    componentName,
                    "encoder.incremented",
                    interfaceDeviceName + ".increment." + interfaceElementName
                );
                AddDefaultOutputBinding(
                    componentName,
                    "encoder.decremented",
                    interfaceDeviceName + ".decrement." + interfaceElementName
                );
            } else if(bindType == AutoBindActionType.Set)
            {
                AddDefaultOutputBinding(
                    componentName,
                    "encoder.incremented",
                    interfaceDeviceName + ".set." + interfaceElementName
                );
                AddDefaultOutputBinding(
                    componentName,
                    "encoder.decremented",
                    interfaceDeviceName + ".set." + interfaceElementName
                );
            } else
            {

            }

            return knob;
        }

        protected ToggleSwitch AddToggleSwitch(string name, Point posn, Size size, ToggleSwitchPosition defaultPosition,
            string positionOneImage, string positionTwoImage, ToggleSwitchType defaultType, LinearClickType clickType,
            string interfaceDeviceName, string interfaceElementName,
            bool fromCenter, bool horizontal = false, string interfaceIndicatorElementName = "")
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
                HasIndicator = true
            };
            if (interfaceIndicatorElementName != "")
            {
                // if there is an indicatorElementname then the image names will be partial
                // and need to be completed
                newSwitch.PositionOneImage = positionOneImage + " off.png";
                newSwitch.PositionTwoImage = positionTwoImage + " off.png";
                newSwitch.PositionOneIndicatorOnImage = positionOneImage + " on.png";
                newSwitch.PositionTwoIndicatorOnImage = positionTwoImage + " on.png";
                newSwitch.HasIndicator = true;
            }
            else
            {
                newSwitch.PositionOneImage = positionOneImage;
                newSwitch.PositionTwoImage = positionTwoImage;
                newSwitch.HasIndicator = false;
            }

            newSwitch.Width = size.Width;
            newSwitch.Height = size.Height;

            newSwitch.Top = posn.Y;
            newSwitch.Left = posn.X;
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
                componentName,
                "position.changed",
                interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                componentName,
                interfaceDeviceName + "." + interfaceElementName + ".changed",
                "set.position");

            if (newSwitch.HasIndicator)
            {
                AddAction(newSwitch.Actions["set.indicator"], componentName);

                AddDefaultInputBinding(
                    componentName,
                    interfaceDeviceName + "." + interfaceIndicatorElementName + ".changed",
                    "set.indicator");
            }

            return newSwitch;
        }
    }
}