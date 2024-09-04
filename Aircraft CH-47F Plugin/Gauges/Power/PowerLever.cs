//  Copyright 2014 Craig Courtney
//  Copyright 2024 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.CH47F.PowerLevers
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using static GadrocsWorkshop.Helios.Interfaces.DCS.CH47F.Commands;

    [HeliosControl("Helios.CH47F.Power", "Engine Power", "CH-47F Chinook", typeof(BackgroundImageRenderer),HeliosControlFlags.None)]
    public class Lever : CompositeVisualWithBackgroundImage
    {
        private HeliosPanel _panel;
        private string _interfaceDevice = "Engine Control (Overhead Console)";
        private const string PANEL_IMAGE = "{CH-47F}/Images/Power/CH-47F_Engine_Cond_Panel.png";
        private static readonly double scaling = 0.629; 
        public Lever()
            : base("Power Levers", new Size(400, 432))  // Full Size = 636d, 687d
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.CH47F.CH47FInterface) };

            _panel = AddPanel("Panel Image", new Point(0d, 115d * scaling), new Size(636d * scaling, 572d * scaling), PANEL_IMAGE, _interfaceDevice);
            _panel.Opacity = 1d;
            _panel.FillBackground = false;
            _panel.DrawBorder = false;
            AddLinearPotentiometerDetentsAnimated("Engine 1 Power", new Point(184 * scaling, 2 * scaling), new Size(119 * scaling, 640 * scaling), $"{{CH-47F}}/Images/Power/Left_Power_Lever_0.png", _interfaceDevice, "Engine 1 Power Lever", "Engine 1 Lever Detent Gate");
            AddLinearPotentiometerDetentsAnimated("Engine 2 Power", new Point(334 * scaling, 1 * scaling), new Size(105 * scaling, 642 * scaling), $"{{CH-47F}}/Images/Power/Right_Power_Lever_0.png", _interfaceDevice, "Engine 2 Power Lever", "Engine 2 Lever Detent Gate");
        }
        protected HeliosPanel AddPanel(string name, Point posn, Size size, string background, string interfaceDevice)
        {
            HeliosPanel panel = AddPanel
                (
                name: name,
                posn: posn,
                size: size,
                background: background
                );
            // in this instance, we want to all the panels to be hide-able so the actions need to be added
            IBindingAction panelAction = panel.Actions["toggle.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
            }
            panelAction = panel.Actions["set.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
            }
            return panel;
        }
        private void AddLinearPotentiometerDetentsAnimated(string name, Point posn, Size size, string imagePrefix, string interfaceDeviceName, string interfaceElement1Name, string interfaceElement2Name)
        {
            LinearPotentiometerDetentsAnimated pot = new LinearPotentiometerDetentsAnimated()
            {
                Name = $"{Name}_{name}",
                Left = posn.X,
                Top = posn.Y,
                Width = size.Width,
                Height = size.Height,
                AnimationFrameImageNamePattern = imagePrefix,
                AnimationFrameNumber = 0,
                AnimationFrameCount = 5,
                ClickType = LinearClickType.Swipe,
                InvertedVertical = true,
                Sensitivity = 0,
                InitialValue = 0,
                StepValue = 0.1,
                MinValue = 0,
                InitialPosition = 0
            };
            pot.AddDetent(0.5d);

            Children.Add(pot);
            foreach (IBindingAction action in pot.Actions)
            {
                AddAction(action, name);
            }
            foreach (IBindingTrigger trigger in pot.Triggers)
            {
                AddTrigger(trigger, name);
            }
            AddDefaultInputBinding(
                childName: $"{Name}_{name}",
                interfaceTriggerName: $"{interfaceDeviceName}.{interfaceElement1Name}.changed",
                deviceActionName: "set.value");
            AddDefaultOutputBinding(
                childName: $"{Name}_{name}",
                deviceTriggerName: "value.changed",
                interfaceActionName: $"{interfaceDeviceName}.set.{interfaceElement1Name}"
                );
            AddDefaultOutputBinding(
                childName: $"{Name}_{name}",
                deviceTriggerName: "maximum value position.released",
                interfaceActionName: $"{interfaceDeviceName}.push.{interfaceElement2Name}"
                );
            AddDefaultOutputBinding(
                childName: $"{Name}_{name}",
                deviceTriggerName: "minimum value position.released",
                interfaceActionName: $"{interfaceDeviceName}.release.{interfaceElement2Name}"
                );
            AddDefaultOutputBinding(
                childName: $"{Name}_{name}",
                deviceTriggerName: "detent 1.holding",
                interfaceActionName: $"{interfaceDeviceName}.release.{interfaceElement2Name}"
                );
            AddDefaultOutputBinding(
                childName: $"{Name}_{name}",
                deviceTriggerName: "detent 1.released",
                interfaceActionName: $"{interfaceDeviceName}.push.{interfaceElement2Name}"
                );

        }
        private string ComponentName(string name)
        {
            return $"{Name}_{name}";
        }
        private new void AddTrigger(IBindingTrigger trigger, string name)
        {
            trigger.Device = ComponentName(name);
            if (!Triggers.ContainsKey(Triggers.GetKeyForItem(trigger))) Triggers.Add(trigger);

        }
        private new void AddAction(IBindingAction action, string name)
        {
            action.Device = ComponentName(name);
            if (!Actions.ContainsKey(Actions.GetKeyForItem(action))) Actions.Add(action);
        }
        public override string DefaultBackgroundImage => "";

        protected override void OnBackgroundImageChange()
        {
            _panel.BackgroundImage = BackgroundImageIsCustomized ? null : "";
        }

        public override void MouseDown(Point location)
        {
            // No-Op
        }
        public override void MouseDrag(Point location)
        {
            // No-Op
        }
        public override void MouseUp(Point location)
        {
            // No-Op
        }

    }
}
