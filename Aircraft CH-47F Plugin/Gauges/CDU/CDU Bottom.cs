//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.CH47F.CDU
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using System.Globalization;
    using System.Xml.Linq;
    using System.Drawing.Imaging;

    [HeliosControl("Helios.CH47F.CDU.Bottom", "CDU Keyboard", "CH-47F Chinook", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class CDU_Bottom : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(145, 94, 512, 486);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private const string PANEL_IMAGE = "{CH-47F}/Gauges/CDU/Images/CDU_Bezel_Bot.png";
        private const int KBOFFSET = 659;

        public CDU_Bottom(string interfaceDevice)
            : base(interfaceDevice, new Size(810, 1000 - KBOFFSET))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.CH47F.CH47FInterface) };
            _interfaceDevice = interfaceDevice;

            int buttonNumber = 0;
            string[] labels = new string[] { "1", "2", "3",
                                            "A", "B", "C", "D", "E", "F", "G", "4", "5", "6", "H", "I", "J", "K", "L", "M", "N", "7", "8", "9", "O", "P", "Q", "R", "S", "T", "U", "0", "dot", "V",
                                            "W", "X", "Y", "Z", "SP", "MARK", "slash", "dash", "TDL", "ASE", "empty", "DATA", "STAT"};


            for (int y = 670 - KBOFFSET; y <= 808 - KBOFFSET; y += 69)
            {
                for (int x = 19; x <= 730; x += 79) 
                {
                    AddButton($"{labels[buttonNumber]}", new Rect(x, y, 63, 54), $"{labels[buttonNumber++]}");
                }
            }
            for (int x = 97; x <= 650; x += 79)
            {
                AddButton($"{labels[buttonNumber]}", new Rect(x, 875 - KBOFFSET, 63, 54), $"{labels[buttonNumber++]}");
            }
            for (int x = 66; x <= 661; x += 85)
            {
                AddButton($"{labels[buttonNumber]}", new Rect(x, 940 - KBOFFSET, 81, 60), $"{labels[buttonNumber++]}");
            }
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

        private void AddButton(string name, Rect rect, string label)
        {
            PushButton button = new PushButton();
            button.Top = rect.Y;
            button.Left = rect.X;
            button.Width = rect.Width;
            button.Height = rect.Height;
            if (label.Length == 2 && (label.StartsWith("L") || label.StartsWith("R"))) {
                label = label.Substring(0,1) + "x";
            }

            button.Image = $"{{CH-47F}}/Gauges/CDU/Images/CDU_{label.Replace(" ", "_")}_Norm.png";
            button.PushedImage = $"{{CH-47F}}/Gauges/CDU/Images/CDU_{(label.Replace(" ","_"))}_Pressed.png";

            button.Name = name;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], name);
            AddTrigger(button.Triggers["released"], name);

            AddAction(button.Actions["push"], name);
            AddAction(button.Actions["release"], name);
            AddAction(button.Actions["set.physical state"], name);
            // add the default bindings
            AddDefaultOutputBinding(
                childName: name,
                deviceTriggerName: "pushed",
                interfaceActionName: $"{_interfaceDevice}.push.{name}"
                );
            AddDefaultOutputBinding(
                childName: name,
                deviceTriggerName: "released",
                interfaceActionName: $"{_interfaceDevice}.release.{name}"
                );
            AddDefaultInputBinding(
                childName: name,
                interfaceTriggerName: $"{_interfaceDevice}.{name}.changed",
                deviceActionName: "set.physical state");
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

        public override string DefaultBackgroundImage => PANEL_IMAGE;

        protected override void OnBackgroundImageChange()
        {
            BackgroundImage = BackgroundImageIsCustomized ? null : PANEL_IMAGE;
        }
        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return false;
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
