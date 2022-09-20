//  Copyright 2014 Craig Courtney
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

using GadrocsWorkshop.Helios.Gauges;

namespace GadrocsWorkshop.Helios.M2000C
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Gauges.M2000C.Mk2CDrumGauge;
    using GadrocsWorkshop.Helios.Gauges.M2000C.Mk2CNeedle;
    using GadrocsWorkshop.Helios.Gauges.M2000C.TACAN;

    public abstract class M2000CCompositeVisual : CompositeVisualWithBackgroundImage
    {
        private Dictionary<HeliosVisual, Rect> _nativeSizes = new Dictionary<HeliosVisual, Rect>();

        protected M2000CCompositeVisual(string name, Size nativeSize)
            : base(name, nativeSize)
        {
            PersistChildren = false;
            Children.CollectionChanged += Children_CollectionChanged;
            _defaultBindingName = "";
            _defaultInterface = null;
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
 
        #endregion


        private Point FromCenter(Point pos, Size size) {
            return new Point(pos.X - size.Width / 2.0, pos.Y - size.Height / 2.0);
        }

        protected RotarySwitch AddRotarySwitch(string name, Point posn, Size size,
            string knobImage, int defaultPosition, RotaryClickType clickType,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter, NonClickableZone[] nonClickableZones = null)
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
                DefaultPosition = defaultPosition,
                ClickType = clickType,
                NonClickableZones = nonClickableZones,
            };
            knob.Positions.Clear();

            Children.Add(knob);

            foreach (IBindingTrigger trigger in knob.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in knob.Actions)
            {
                AddAction(action, componentName);
            }

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName);

            return knob;
        }

        protected new Indicator AddIndicator(string name, Point posn, Size size,
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

        protected RectangleFill AddRectangleFill(string name, Point posn, Size size, Color color, Double initialValue,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);
            RectangleFill rectangleFill = new RectangleFill
            {
                Name = componentName,
                Left = posn.X,
                Top = posn.Y,
                Height = size.Height,
                Width = size.Width,
                FillColor = color,
                FillHeight = initialValue
            };

            Children.Add(rectangleFill);
            foreach (IBindingTrigger trigger in rectangleFill.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(rectangleFill.Actions["set.Height"], componentName);

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.Height");
                
            return rectangleFill;
        }

        protected Mk2CDrumGauge AddDrumGauge(string name, string gaugeImage, Point posn, Size size, Size renderSize, string format,
            string interfaceDeviceName, string interfaceElementName, string actionIdentifier, string valueDescription, bool fromCenter) {
            return AddDrumGauge(name, gaugeImage, posn, size, renderSize, format, interfaceDeviceName, interfaceElementName, actionIdentifier, valueDescription, fromCenter, 10d, 0d);
        }
        protected Mk2CDrumGauge AddDrumGauge(string name, string gaugeImage, Point posn, Size size, Size renderSize, string format,
            string interfaceDeviceName, string interfaceElementName, string actionIdentifier, string valueDescription, bool fromCenter, double multiplier, double offset)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);

            Mk2CDrumGauge newGauge = new Mk2CDrumGauge(componentName, gaugeImage, actionIdentifier, valueDescription, format, posn, size, renderSize, multiplier, offset);

            Children.Add(newGauge);
            foreach (IBindingTrigger trigger in newGauge.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in newGauge.Actions)
            {
                AddAction(action, componentName);
            }

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set." + actionIdentifier);

            return newGauge;
        }
        protected TACANDrumGauge AddDrumGauge(string name, string gaugeImage, Point posn, Size size, Size renderSize, string format,
            string interfaceDeviceName, string interfaceElementName, string actionIdentifier, string valueDescription, bool fromCenter, double drumDigits = 10d)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);

            TACANDrumGauge newGauge = new TACANDrumGauge(componentName, gaugeImage, actionIdentifier, valueDescription, format, posn, size, renderSize, drumDigits);

            Children.Add(newGauge);
            foreach (IBindingTrigger trigger in newGauge.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in newGauge.Actions)
            {
                AddAction(action, componentName);
            }

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set." + actionIdentifier);

            return newGauge;
        }


        protected TACANChannelDrum AddTacanDrum(string name, Point posn, Size size, 
            string interfaceDeviceName, string interfaceElementName, bool fromCenter)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);

            TACANChannelDrum newGauge = new TACANChannelDrum(componentName, posn, size);

            Children.Add(newGauge);
            foreach (IBindingTrigger trigger in newGauge.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in newGauge.Actions)
            {
                AddAction(action, componentName);
            }

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set." + componentName);

            return newGauge;
        }

        protected Mk2CNeedle AddNeedle(string name, string needleImage, Point posn, Size size, Point centerPoint, 
            string interfaceDeviceName, string interfaceElementName, string actionIdentifier, string valueDescription, BindingValueUnit typeValue, 
            double[] initialCalibration, double[,] calibrationPoints, bool fromCenter)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);

            Mk2CNeedle newNeedle = new Mk2CNeedle(componentName, needleImage, actionIdentifier, valueDescription, posn, size, centerPoint, typeValue, initialCalibration, calibrationPoints);

            Children.Add(newNeedle);
            foreach (IBindingTrigger trigger in newNeedle.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in newNeedle.Actions)
            {
                AddAction(action, componentName);
            }

            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set." + actionIdentifier);

            return newNeedle;
        }

        protected ToggleSwitch AddToggleSwitch(string name, Point posn, Size size, ToggleSwitchPosition defaultPosition, 
            string positionOneImage, string positionTwoImage, ToggleSwitchType defaultType, string interfaceDeviceName, string interfaceElementName, 
            bool fromCenter, NonClickableZone[] nonClickableZones = null, bool horizontal = false, bool horizontalRender = false)
        {
            if (fromCenter)
                posn = FromCenter(posn, size);
            string componentName = GetComponentName(name);

            ToggleSwitch newSwitch = new ToggleSwitch();
            newSwitch.Name = componentName;
            newSwitch.SwitchType = defaultType;
            newSwitch.ClickType = LinearClickType.Touch;
            newSwitch.DefaultPosition = defaultPosition;
            newSwitch.PositionOneImage = positionOneImage;
            newSwitch.PositionTwoImage = positionTwoImage;
            newSwitch.Width = size.Width;
            newSwitch.Height = size.Height;
            newSwitch.NonClickableZones = nonClickableZones;
            if (horizontal)
            {
                newSwitch.Orientation = ToggleSwitchOrientation.Horizontal;
            }
            else
            {
                newSwitch.Orientation = ToggleSwitchOrientation.Vertical;
            }

            newSwitch.Top = posn.Y;
            newSwitch.Left = posn.X;
            if (horizontalRender)
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

        protected IndicatorPushButton AddIndicatorPushButton(string name, Point pos, Size size, string image, string pushedImage, Color textColor, Color onTextColor, string font, 
            string interfaceDeviceName = "", string interfaceElementName = "", string onImage = "", bool fromCenter = false, bool withText = true)
        {
            if (fromCenter)
                pos = FromCenter(pos, size);
            string componentName = GetComponentName(name);

            IndicatorPushButton indicator = new Helios.Controls.IndicatorPushButton
            {
                Top = pos.Y,
                Left = pos.X,
                Width = size.Width,
                Height = size.Height,
                Image = image,
                PushedImage = pushedImage,
                IndicatorOnImage = onImage,
                PushedIndicatorOnImage = onImage,
                Name = componentName,
                OnTextColor = onTextColor,
                TextColor = textColor
            };
            if(withText)
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
            foreach (IBindingTrigger trigger in indicator.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            foreach (IBindingAction action in indicator.Actions)
            {
                AddAction(action, componentName);
            }
            
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.indicator");
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + " Button.changed",
                deviceActionName: "set.physical state");
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "pushed",
                interfaceActionName: interfaceDeviceName + ".push." + interfaceElementName + " Button");
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "released",
                interfaceActionName: interfaceDeviceName + ".release." + interfaceElementName + " Button");

            return indicator;
        }

        protected ThreeWayToggleSwitch AddThreeWayToggle(string name, Point pos, Size size,
            ThreeWayToggleSwitchPosition defaultPosition, ThreeWayToggleSwitchType switchType,
            string interfaceDeviceName, string interfaceElementName, bool fromCenter,
            string positionOneImage = "{Helios}/Images/Toggles/round-up.png",
            string positionTwoImage = "{Helios}/Images/Toggles/round-norm.png",
            string positionThreeImage = "{Helios}/Images/Toggles/round-down.png",
            LinearClickType clickType = LinearClickType.Swipe,
            bool horizontal = false,
            bool horizontalRender = false)
        {
            string componentName = GetComponentName(name);
            ThreeWayToggleSwitch toggle = new ThreeWayToggleSwitch
            {
                Top = pos.Y,
                Left =  pos.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = defaultPosition,
                PositionOneImage = positionOneImage,
                PositionTwoImage = positionTwoImage,
                PositionThreeImage = positionThreeImage,
                SwitchType = switchType,
                Name = componentName
            };
            toggle.ClickType = clickType;
            if (horizontal)
            {
                toggle.Orientation = ToggleSwitchOrientation.Horizontal;
            }
            else
            {
                toggle.Orientation = ToggleSwitchOrientation.Vertical;
            }
            if (horizontalRender)
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
    }
}
