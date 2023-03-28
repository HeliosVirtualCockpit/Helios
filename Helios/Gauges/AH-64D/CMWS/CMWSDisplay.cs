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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.CMWS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.CMWS", "CMWS Display", "AH-64D", typeof(BackgroundImageRenderer))]
    public class CMWSDisplay : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "CMWS";
        private string _font = "Helios Virtual Cockpit A-10C_ALQ_213";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private CMWSThreatDisplay _display;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private HeliosPanel _displayBackgroundPanel;
        private const string Panel_Image = "{Helios}/Images/AH-64D/CMWS/cmws_frame.png";

        public CMWSDisplay()
            : base("CMWS Display", new Size(640, 409))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.AH64D.AH64DInterface) };

            _displayBackgroundPanel = AddPanel("CMWS Display Background", new Point(206, 29), new Size(230, 110), "{Helios}/Images/AH-64D/CMWS/cmws_background.xaml", _interfaceDeviceName);
            _displayBackgroundPanel.Opacity = 1d;
            _displayBackgroundPanel.FillBackground = false;
            _displayBackgroundPanel.DrawBorder = false;

            AddCMWSPart("Threat Display", new Point(326d, 29d), new Size(110d, 110d), _interfaceDeviceName, "CMWS Threat Display");
            AddTextDisplay("Line 1", new Point(206d, 29d), new Size(120d, 55d), _interfaceDeviceName, "Line 1", 26, "F OUT", TextHorizontalAlignment.Left, "");
            AddTextDisplay("Line 2", new Point(206d, 84d), new Size(120d, 55d), _interfaceDeviceName, "Line 2", 26, "C OUT", TextHorizontalAlignment.Left, "");

            _frameGlassPanel = AddPanel("CMWS Glass", new Point(206, 29), new Size(230d, 110d), "{Helios}/Images/AH-64D/MFD/MFD_glass.png", _interfaceDeviceName);
            _frameGlassPanel.Opacity = 0.3d;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;

            _frameBezelPanel = AddPanel("CMWS Frame", new Point(Left, Top), NativeSize, "{Helios}/Images/AH-64D/CMWS/cmws_frame.png", _interfaceDeviceName);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;

            AddThreePositionRotarySwitch("Power Switch", new Point(63d, 45d), new Size(105d, 105d), _interfaceDeviceName, "Power Switch");
            AddPot("Audio Volume", new Point(450d, 15d), new Size(62d, 62d), "Audio Volume Knob");
            AddPot("Display Brightness", new Point(450d, 104d), new Size(62d, 62d), "Brightness Knob");
            AddTwoWayToggle("Arm", new Point(127d, 274d), new Size(60d, 120d), _interfaceDeviceName, "Arm Switch");
            AddTwoWayToggle("Mode", new Point(244d, 274d), new Size(60d, 120d), _interfaceDeviceName, "Mode Switch",ToggleSwitchPosition.One);
            AddTwoWayToggle("Operation", new Point(361d, 274d), new Size(60d, 120d), _interfaceDeviceName, "Operation Switch");
            AddTwoWayGuardToggle("Flare Jettison", new Point(472d, 191d), new Size(81d, 220d), _interfaceDeviceName, "Flare Jettison Switch", "Jettison Switch Cover", "{Helios}/Images/AH-64D/CMWS/CMWS_Jettison_");
        }
        private void AddTextDisplay(string name, Point posn, Size size,
    string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xcc, 0x50, 0xc3, 0x39),
                backgroundColor: Color.FromArgb(0xff, 0x04, 0x2a, 0x00),
                useBackground: false,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
        }
        private void AddCMWSPart(string name, Point pos, Size size, string interfaceDevice, string interfaceElement)
        {
            _display = new CMWSThreatDisplay
            {
                Top = pos.Y,
                Left = pos.X,
                Height = size.Height,
                Width = size.Width,
                Name = GetComponentName(name)
            };
            Children.Add(_display);
            // Note:  we have the actions against the new CMWSThreatDisplay but to expose those
            // actions in the interface, we copy the actions to the Parent.  This is a new 
            // HeliosActionCollection with the keys equal to the new ActionIDs, however the original
            // HeliosActionCollection which is on the child part will have the original keys, even though
            // we might have changed the values of the ActionIDs.  This has the result that autobinding
            // in CompositeVisual (OnProfileChanged) might not be able to find the actions when doing
            // the "ContainsKey()" for the action.
            // This is why the _display.Name is in the deviceActionName of the AddDefaultInputBinding
            // and *MUST* match the BindingValue device parameter for the gauge being added.

            //foreach (IBindingTrigger trigger in _display.Triggers)
            //{
            //    AddTrigger(trigger, trigger.Name);
            //}
            foreach (IBindingAction action in _display.Actions)
            {
                if(action.Name != "hidden")
                {

                AddAction(action, _display.Name);
                //Create the automatic input bindings for the sub component
                AddDefaultInputBinding(
                   childName: _display.Name,
                   deviceActionName: _display.Name +"." + action.ActionVerb + "." + action.Name,
                   interfaceTriggerName: interfaceDevice + "." + action.Name + ".changed"
                   );
                }

            }
            //_display.Actions.Clear();
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
        private void AddThreePositionRotarySwitch(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName)
        {
            Helios.Controls.RotarySwitch knob = new Helios.Controls.RotarySwitch();
            knob.Name = Name + "_" + name;
            knob.KnobImage = "{Helios}/Images/AH-64D/Common/Selector_Knob.png";
            knob.DrawLabels = false;
            knob.DrawLines = false;
            knob.Positions.Clear();
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 0, "Off", 50));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 1, "On", 90d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 2, "Test", 130d));
            knob.CurrentPosition = 1;
            knob.Top = posn.Y;
            knob.Left = posn.X;
            knob.Width = size.Width;
            knob.Height = size.Height;
            AddRotarySwitchBindings(name, posn, size, knob, interfaceDeviceName, interfaceElementName);
        }
        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            Potentiometer knob = AddPot(
                name: name,
                posn: posn,
                size: size,
                knobImage: "{Helios}/Images/AH-64D/Common/Common Knob.png",
                initialRotation: 225,
                rotationTravel: 290,
                minValue: 0,
                maxValue: 1,
                initialValue: 1,
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            knob.Name = Name + "_" + name;
        }

        private void AddTwoWayToggle(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName) { AddTwoWayToggle(name, posn, size, interfaceDeviceName, interfaceElementName, "{Helios}/Images/Toggles/orange-round-", ToggleSwitchPosition.Two); }
        private void AddTwoWayToggle(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, ToggleSwitchPosition ToggleDefaultPosition) { AddTwoWayToggle(name, posn, size, interfaceDeviceName, interfaceElementName, "{Helios}/Images/Toggles/orange-round-", ToggleDefaultPosition); }
        private void AddTwoWayToggle(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName,string imageName, ToggleSwitchPosition ToggleDefaultPosition)
        {
            ToggleSwitch toggle = AddToggleSwitch(
                name: name,
                posn: posn,
                size: size,
                defaultPosition: ToggleDefaultPosition,
                defaultType: ToggleSwitchType.OnOn,
                positionOneImage: $"{imageName}up.png",
                positionTwoImage: $"{imageName}down.png",
                horizontal: false,
                clickType: LinearClickType.Swipe,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }
        private void AddTwoWayGuardToggle(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, string interfaceElementGuardName, string imageName)
        {
            string componentName = ComponentName(name);
            GuardedToggleSwitch newSwitch = new GuardedToggleSwitch
            {
                Name = componentName,
                SwitchType = ToggleSwitchType.OnOn,
                ClickType = LinearClickType.Swipe,
                DefaultPosition = ToggleSwitchPosition.Two,
                DefaultGuardPosition = GuardPosition.Down,
                PositionOneGuardDownImage = $"{imageName}Guard_Down.png",
                PositionOneGuardUpImage = $"{imageName}One_Up.png",
                PositionTwoGuardDownImage = $"{imageName}Guard_Down.png",
                PositionTwoGuardUpImage = $"{imageName}Two_Up.png",
                Width = size.Width,
                Height = size.Height,
                Top = posn.Y,
                Left = posn.X
            };
            Children.Add(newSwitch);

            foreach (IBindingTrigger trigger in newSwitch.Triggers)
            {
                AddTrigger(trigger, name);
            }
            AddAction(newSwitch.Actions["set.position"], name);
            AddAction(newSwitch.Actions["set.guard position"], name);

            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "guard position.changed",
                interfaceActionName: interfaceDeviceName + ".set." + interfaceElementGuardName
            );
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementGuardName + ".changed",
                deviceActionName: "set.guard position");

            //interfaceDeviceName: _interfaceDeviceName,
            //interfaceElementName: interfaceElementName,

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

        public override string DefaultBackgroundImage => Panel_Image;

        protected override void OnBackgroundImageChange()
        {
            _frameBezelPanel.BackgroundImage = BackgroundImageIsCustomized ? null : Panel_Image;
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
    }
}
