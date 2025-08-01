//  Copyright 2014 Craig Courtney
//  Copyright 2025 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.DH98Mosquito
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Controls.DH98Mosquito;
    using GadrocsWorkshop.Helios.Interfaces.DCS.DH98Mosquito;
    using NLog.Targets;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.DH98Mosquito.LeverPanel", "Lever Panel", "Mosquito FB Mk VI", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class LeversPanel : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Main Panel";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;

        private HeliosPanel _displayBackgroundPanel;
        private readonly string _imageLocation = "{DH98Mosquito}/Images/";

        private ImageTranslucent _springOpen;
        private ImageTranslucent _springClosed;

        private Dictionary<string, string> _outputBindings = new Dictionary<string, string>{};

        private static string[] _outBindings = new string[]{
            "Flaps Lock Switch|position.changed|Main Panel.set.Flaps Lever Gate",
            "Bomb Doors Lever|position one.entered|Main Panel.push up.Bomb Doors Lever",
            "Bomb Doors Lever|position one.entered|Main Panel.release.Bomb Doors Lever",
            "Bomb Doors Lever|position three.entered|Main Panel.push down.Bomb Doors Lever",
            "Bomb Doors Lever|position three.entered|Main Panel.release.Bomb Doors Lever",
            "Bomb Doors Lever|position two.entered|Main Panel.release.Bomb Doors Lever",
            "Gear (Chassis) Lever|position two.entered|Main Panel.release.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position one.entered|Main Panel.push up.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position one.entered|Main Panel.release.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position three.entered|Main Panel.push down.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position three.entered|Main Panel.release.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position one.exited|Main Panel.push down.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position one.exited|Main Panel.release.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position three.exited|Main Panel.push up.Gear (Chassis) Lever",
            "Gear (Chassis) Lever|position three.exited|Main Panel.release.Gear (Chassis) Lever",
            "Flaps Lever|position one.entered|Main Panel.push up.Flaps Lever",
            "Flaps Lever|position one.entered|Main Panel.release.Flaps Lever",
            "Flaps Lever|position three.entered|Main Panel.push down.Flaps Lever",
            "Flaps Lever|position three.entered|Main Panel.release.Flaps Lever",
            "Flaps Lever|position two.entered|Main Panel.release.Flaps Lever Gate",
            "Flaps Lever|position three.exited|Main Panel.push up.Flaps Lever",
            "Flaps Lever|position three.exited|Main Panel.release.Flaps Lever",
            "Flaps Lever|position one.exited|Main Panel.push down.Flaps Lever",
            "Flaps Lever|position one.exited|Main Panel.release.Flaps Lever Gate"
};


//        private readonly RegexOptions _options = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
//        private readonly string _pattern = @"^\s*\<Trigger Source\=""Visual;.*;(?'triggerElement'.*)"" Name\=""(?'triggerName'.*)""\s*\/\>\s*^\s*\<Action Target\=""Interface;;.*Name\=""(?'actionName'.*)"".*\/\>\s*$";

//        private readonly string _bindings = @"
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.Flaps Lever Gate.changed"" />
//      <Action Target=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.LeverThreeWayToggleSwitch;Flaps Lever"" Name=""Flaps Lever.set.position three lock"" />
//      <LuaScript>return TriggerValue==1</LuaScript>
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.Gear (Chassis) Lever Gate.changed"" />
//      <Action Target=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.LeverThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""Gear (Chassis) Lever.set.position one lock"" />
//      <LuaScript>return TriggerValue == 1</LuaScript>
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lock Switch;Helios.Base.ToggleSwitch;Flaps Lock Switch"" Name=""position.changed"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.set.Flaps Lever Gate"" />
//      <TriggerValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Bomb Doors Lever;Helios.Base.ThreeWayToggleSwitch;Bomb Doors Lever"" Name=""position one.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push up.Bomb Doors Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Bomb Doors Lever;Helios.Base.ThreeWayToggleSwitch;Bomb Doors Lever"" Name=""position one.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Bomb Doors Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Bomb Doors Lever;Helios.Base.ThreeWayToggleSwitch;Bomb Doors Lever"" Name=""position three.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push down.Bomb Doors Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Bomb Doors Lever;Helios.Base.ThreeWayToggleSwitch;Bomb Doors Lever"" Name=""position three.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Bomb Doors Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Bomb Doors Lever;Helios.Base.ThreeWayToggleSwitch;Bomb Doors Lever"" Name=""position two.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Bomb Doors Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position two.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position one.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push up.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position one.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position three.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push down.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position three.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position one.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push down.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position one.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position three.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push up.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Gear (Chassis) Lever;Helios.Base.ThreeWayToggleSwitch;Gear (Chassis) Lever"" Name=""position three.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Gear (Chassis) Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position one.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push up.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position one.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position three.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push down.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position three.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position two.entered"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Flaps Lever Gate"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position three.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push up.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position three.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position one.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.push down.Flaps Lever"" />
//      <StaticValue />
//    </Binding>
//    <Binding BypassCascadingTriggers=""True"">
//      <Trigger Source=""Visual;Monitor 1.Lever Panel.Flaps Lever;Helios.Base.ThreeWayToggleSwitch;Flaps Lever"" Name=""position one.exited"" />
//      <Action Target=""Interface;;Helios.DH98Mosquito;DCS DH98 Mosquito FB Mk VI"" Name=""Main Panel.release.Flaps Lever Gate"" />
//      <StaticValue />
//    </Binding>
//";
        public LeversPanel()
            : base("Lever Panel", new Size(300, 345))
        {
            SupportedInterfaces = new[] { typeof(DH98MosquitoInterface) };

            _displayBackgroundPanel = AddPanel("Levers Background", new Point(0, 0), new Size(300, 345), _imageLocation + "Panels/Lever-Panel.png", _interfaceDeviceName);
            _displayBackgroundPanel.Opacity = 1d;
            _displayBackgroundPanel.FillBackground = false;
            _displayBackgroundPanel.DrawBorder = false;

            _springOpen = AddImage("Flaps Lock Spring Open", new Point(47, 226), new Size(153, 17), $"{_imageLocation}Levers/Flaps-Lock-Spring-Open.png");
            _springOpen.IsHidden = true;
            _springClosed = AddImage("Flaps Lock Spring Closed", new Point(47, 226), new Size(153, 17), $"{_imageLocation}Levers/Flaps-Lock-Spring-Closed.png");
            _springClosed.IsHidden = false;
            AddTwoWayToggle("Flaps Lever Gate", new Point(168d, 96d), new Size(126d, 148d), "Flaps Lever Gate", "Levers/Flaps-Lock-Wide", ToggleSwitchPosition.One);
            AddThreeWayToggle("Bomb Doors Lever", new Point(0d, 29d), new Size(92d, 279d), "Bomb Doors Lever", "Levers/Bomb-Lever");
            _outputBindings.Add($"{_interfaceDeviceName}.Bomb Doors Lever.position one.entered", $"{_interfaceDeviceName}.push up.Bomb Doors Lever");
            AddThreeWayToggle("Gear (Chassis) Lever", new Point(114d, 0d), new Size(47d, 337d), "Gear (Chassis) Lever", "Levers/Gear-Lever");
            AddThreeWayToggle("Flaps Lever", new Point(181d, 17d), new Size(112d, 321d), "Flaps Lever", "Levers/Flaps-Lever");
            CreateNewOutputBindings();
            CreateNewInputBindings();
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
                //string addedKey = Actions.GetKeyForItem(panelAction);
            }
            panelAction = panel.Actions["set.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
                //string addedKey = Actions.GetKeyForItem(panelAction);
            }
            return panel;
        }
        private void AddTwoWayToggle(string name, Point posn, Size size, string interfaceElementName, string imageStem, ToggleSwitchPosition toggleDefaultPosition)
        {
            ToggleSwitch toggle = AddToggleSwitch(
                name: name,
                posn: posn,
                size: size,
                defaultPosition: toggleDefaultPosition,
                defaultType: ToggleSwitchType.OnOn,
                positionOneImage: $"{_imageLocation}{imageStem}-Closed.png",
                positionTwoImage: $"{_imageLocation}{imageStem}-Open.png",
                horizontal: false,
                clickType: LinearClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            toggle.SwitchType = ToggleSwitchType.OnOn;

            AddDefaultInputBinding(
                childName: $"{Name}_{name}",
                interfaceTriggerName: $"{_interfaceDeviceName}.{name}.changed",
                deviceActionName: "set.position");
        }
        private void AddThreeWayToggle(string name, Point posn, Size size, string interfaceElementName, string imageStem)
        {
            LockingThreeWayToggleSwitch toggle = new LockingThreeWayToggleSwitch
            {
                Name = $"{Name}_{name}",
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = ThreeWayToggleSwitchPosition.Two,
                PositionOneImage = $"{_imageLocation}{imageStem}-Up.png",
                PositionTwoImage = $"{_imageLocation}{imageStem}-Middle.png",
                PositionThreeImage = $"{_imageLocation}{imageStem}-Down.png",
                ClickType = LinearClickType.Swipe,
                SwitchType = ThreeWayToggleSwitchType.OnOnOn,
                Rotation = HeliosVisualRotation.None,
                Orientation = ToggleSwitchOrientation.Vertical
            };

            Children.Add(toggle);
            foreach (IBindingTrigger trigger in toggle.Triggers)
            {
                AddTrigger(trigger, $"{name}");
            }
            foreach (IBindingAction action in toggle.Actions)
            {
                AddAction(action, $"{name}");
            }

            AddDefaultInputBinding(
            childName: $"{Name}_{name}",
            interfaceTriggerName: $"{_interfaceDeviceName}.{name}.changed",
            deviceActionName: "set.position");

        }
        private ImageTranslucent AddImage(string name, Point posn, Size size, string imageName)
        {
            ImageTranslucent image = new ImageTranslucent()
            {
                Name = $"{Name}_{name}",
                Left = posn.X,
                Top = posn.Y,
                Width = size.Width,
                Height = size.Height,
                Alignment = ImageAlignment.Stretched,
                Image = imageName,
                AllowInteraction = true,
                Value = 1d,
                IsHidden = false
            };
            Children.Add (image);

            foreach (IBindingAction action in image.Actions)
            {
                AddAction(action, $"{name}");
            }
            return image;
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

        public override string DefaultBackgroundImage => _imageLocation + "Panels/Lever-Panel.png";

        protected override void OnBackgroundImageChange()
        {
            _displayBackgroundPanel.BackgroundImage = BackgroundImageIsCustomized ? null : _imageLocation + "Panels/Lever-Panel.png";
        }

        private void CreateNewOutputBindings()
        {
            //foreach (Match m in Regex.Matches(_bindings, _pattern, _options))
            //{
            //    Console.WriteLine("\"{0}|{1}|{2}\",", m.Groups["triggerElement"].Value, m.Groups["triggerName"].Value, m.Groups["actionName"].Value);
            //    AddDefaultOutputBinding(
            //        childName: $"{Name}_{m.Groups["triggerElement"].Value}",
            //        deviceTriggerName: $"{m.Groups["triggerName"].Value}",
            //        interfaceActionName: $"{m.Groups["actionName"].Value}"
            //        );
            //}
            foreach(string s in _outBindings)
            {
                var ss = s.Split('|');
                AddDefaultOutputBinding(
                    childName: $"{Name}_{ss[0]}",
                    deviceTriggerName: $"{ss[1]}",
                    interfaceActionName: $"{ss[2]}"
                    );
            }
        }
        private void CreateNewInputBindings()
        {
            AddDefaultInputBinding(
                childName: "Lever Panel_Flaps Lever",
                interfaceTriggerName: "Main Panel.Flaps Lever Gate.changed",
                deviceActionName: "set.position three lock",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue==1"),
                triggerBindingSource: BindingValueSources.LuaScript
                );
            AddDefaultInputBinding(
                childName: "Lever Panel_Gear (Chassis) Lever",
                interfaceTriggerName: "Main Panel.Gear (Chassis) Lever Gate.changed",
                deviceActionName: "set.position one lock",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue==1"),
                triggerBindingSource: BindingValueSources.LuaScript
                );
            AddDefaultInputBinding(
                childName: "Lever Panel_Flaps Lock Spring Closed",
                interfaceTriggerName: "Main Panel.Flaps Lever Gate.changed",
                deviceActionName: "set.hidden",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue~=1"),
                triggerBindingSource: BindingValueSources.LuaScript
                );
            AddDefaultInputBinding(
                childName: "Lever Panel_Flaps Lock Spring Open",
                interfaceTriggerName: "Main Panel.Flaps Lever Gate.changed",
                deviceActionName: "set.hidden",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue==1"),
                triggerBindingSource: BindingValueSources.LuaScript
                );
            AddDefaultSelfBinding(
                triggerChildName: "Lever Panel_Flaps Lever Gate", 
                deviceTriggerName: "position.changed",
                actionChildName: "Lever Panel_Flaps Lever", 
                deviceActionName: "set.position three lock",
                deviceBindingSource: BindingValueSources.LuaScript,
                deviceBindingValue: new BindingValue("return TriggerValue==1")
                );
            AddDefaultSelfBinding(
                triggerChildName: "Lever Panel_Flaps Lever Gate",
                deviceTriggerName: "position one.entered",
                actionChildName: "Lever Panel_Flaps Lock Spring Open",
                deviceActionName: "set.hidden",
                deviceBindingSource: BindingValueSources.StaticValue,
                deviceBindingValue: new BindingValue(true)
                );
            AddDefaultSelfBinding(
                deviceBindingSource: BindingValueSources.StaticValue,
                deviceBindingValue: new BindingValue(false),
                triggerChildName: "Lever Panel_Flaps Lever Gate",
                deviceTriggerName: "position one.entered",
                actionChildName: "Lever Panel_Flaps Lock Spring Closed",
                deviceActionName: "set.hidden"
                );
            AddDefaultSelfBinding(
                deviceBindingSource: BindingValueSources.StaticValue,
                deviceBindingValue: new BindingValue(false),
                triggerChildName: "Lever Panel_Flaps Lever Gate",
                deviceTriggerName: "position two.entered",
                actionChildName: "Lever Panel_Flaps Lock Spring Open",
                deviceActionName: "set.hidden"
                );
            AddDefaultSelfBinding(
                deviceBindingSource: BindingValueSources.StaticValue,
                deviceBindingValue: new BindingValue(true),
                triggerChildName: "Lever Panel_Flaps Lever Gate",
                deviceTriggerName: "position two.entered",
                actionChildName: "Lever Panel_Flaps Lock Spring Closed",
                deviceActionName: "set.hidden"
                );

            //< Binding BypassCascadingTriggers = "True" >
            //  < Trigger Source = "Visual;Monitor 1.Lever Panel.Lever Panel_Flaps Lever Gate;Helios.Base.ToggleSwitch;Lever Panel_Flaps Lever Gate" Name = "Lever Panel_Flaps Lever Gate.position one.entered" />
            //  < Action Target = "Visual;Monitor 1.Lever Panel.Flaps-Lock-Spring-Open;Helios.Base.ImageTranslucent;Flaps-Lock-Spring-Open" Name = "Lever Panel_Flaps-Lock-Spring-Open.set.hidden" />
            //  < StaticValue > True </ StaticValue >
            //</ Binding >
            //< Binding BypassCascadingTriggers = "True" >
            //  < Trigger Source = "Visual;Monitor 1.Lever Panel.Lever Panel_Flaps Lever Gate;Helios.Base.ToggleSwitch;Lever Panel_Flaps Lever Gate" Name = "Lever Panel_Flaps Lever Gate.position one.entered" />
            //  < Action Target = "Visual;Monitor 1.Lever Panel.Flaps-Lock-Spring-Closed;Helios.Base.ImageTranslucent;Flaps-Lock-Spring-Closed" Name = "Lever Panel_Flaps-Lock-Spring-Closed.set.hidden" />
            //  < StaticValue > False </ StaticValue >
            //</ Binding >
            //< Binding BypassCascadingTriggers = "True" >
            //  < Trigger Source = "Visual;Monitor 1.Lever Panel.Lever Panel_Flaps Lever Gate;Helios.Base.ToggleSwitch;Lever Panel_Flaps Lever Gate" Name = "Lever Panel_Flaps Lever Gate.position two.entered" />
            //  < Action Target = "Visual;Monitor 1.Lever Panel.Flaps-Lock-Spring-Open;Helios.Base.ImageTranslucent;Flaps-Lock-Spring-Open" Name = "Lever Panel_Flaps-Lock-Spring-Open.set.hidden" />
            //  < StaticValue > False </ StaticValue >
            //</ Binding >
            //< Binding BypassCascadingTriggers = "True" >
            //  < Trigger Source = "Visual;Monitor 1.Lever Panel.Lever Panel_Flaps Lever Gate;Helios.Base.ToggleSwitch;Lever Panel_Flaps Lever Gate" Name = "Lever Panel_Flaps Lever Gate.position two.entered" />
            //  < Action Target = "Visual;Monitor 1.Lever Panel.Flaps-Lock-Spring-Closed;Helios.Base.ImageTranslucent;Flaps-Lock-Spring-Closed" Name = "Lever Panel_Flaps-Lock-Spring-Closed.set.hidden" />
            //  < StaticValue > True </ StaticValue >
            //</ Binding >

        }

        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return true;
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
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
        }

    }
}
