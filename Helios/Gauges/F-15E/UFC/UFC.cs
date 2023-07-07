﻿//  Copyright 2014 Craig Courtney
//  Copyright 2023 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.F15E;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
    using System.Windows.Shapes;
    using GadrocsWorkshop.Helios.Gauges.AV8B;
    using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

    [HeliosControl("Helios.F15E.UFC", "Up Front Controller", "F-15E Strike Eagle", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class UFC : CompositeVisualWithBackgroundImage
    {
        private Dictionary<HeliosVisual, Rect> _nativeSizes = new Dictionary<HeliosVisual, Rect>();
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameBezelPanel;
        private const string Panel_Image = "UFC_Panel_";
        private const string ImageLocation = "{F-15E}/Images/UFC/";

        public UFC(string interfaceDevice, Cockpit cockpit)
            : base(interfaceDevice, new Size(624, 827))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.F15E.F15EInterface) };
            _interfaceDevice = interfaceDevice;
            string[] labels = { "GREC_L","A1","N2","B3","GREC_R","Mark","W4","M5","E6","IP","Decimal","7","S8","C9","Shift","AP","Clr","0","Data","Menu"};
            _frameBezelPanel = AddPanel("UFC Frame", new Point(Left, Top), NativeSize, $"{ImageLocation}{Panel_Image}Base.png", _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;

            AddTextDisplay("Option Line 1", new Point(104d, 61d), new Size(414d, 39d), _interfaceDevice, "Option Line 1", 20.6, "%%%%%%%%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0x9e, 0x9e, 0xa6));
            AddTextDisplay("Option Line 1 Center Text", new Point(248d, 61d), new Size(270d, 39d), _interfaceDevice, "Option Line 1 Center Text", 20.6, "%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0x9e, 0x9e, 0xa6));
            AddTextDisplay("Option Line 2", new Point(104d, 120d), new Size(414d, 39d), _interfaceDevice, "Option Line 2", 20.6, "%%%%%%%%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0xa2, 0x6e, 0x6d));
            AddTextDisplay("Option Line 3", new Point(104d, 179d), new Size(414d, 39d), _interfaceDevice, "Option Line 3", 20.6, "%%%%%%%%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0xa2, 0x6f, 0x6e));
            AddTextDisplay("Option Line 4", new Point(104d, 237d), new Size(414d, 39d), _interfaceDevice, "Option Line 4", 20.6, "%%%%%%%%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0xa2, 0x6f, 0x6e));
            AddTextDisplay("Option Line 5", new Point(104d, 296d), new Size(414d, 39d), _interfaceDevice, "Option Line 5", 20.6, "%%%%%%%%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0x9e, 0x9e, 0xa6));
            AddTextDisplay("Option Line 6", new Point(104d, 354d), new Size(414d, 39d), _interfaceDevice, "Option Line 6", 20.6, "%%%%%%%%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0x9e, 0x9e, 0xa6));
            AddTextDisplay("Option Line 6 Scratchpad", new Point(228d, 354d), new Size(290d, 39d), _interfaceDevice, "Option Line 6 Scratchpad", 20.6, " %%%%%%%%%%%%%%", TextHorizontalAlignment.Left, "!=:", Color.FromArgb(0xE0, 0x9e, 0x9e, 0xa6));
            AddEncoder("Left UHF Preset Channel Selector", new Point(-15, 340), new Size(112, 112), "Left UHF Preset Channel Selector");
            AddRadioButton("Left UHF Preset Channel Pull Switch", new Rect(7, 361, 70, 70), "Left UHF Preset Channel Pull Switch");
            AddEncoder("Right UHF Preset Channel Selector", new Point(526, 340), new Size(112, 112), "Right UHF Preset Channel Selector");
            AddRadioButton("Right UHF Preset Channel Pull Switch", new Rect(547, 361, 70, 70), "Right UHF Preset Channel Pull Switch");
            AddPot("UHF Radio 3 Volume", new Point(60d, 457d), new Size(90d, 90d), "UHF Radio 3 Volume", $"{ImageLocation}RadioVol_Knob_Back.png");
            AddPot("UHF Radio 1 Volume", new Point(83d, 479d), new Size(45d, 45d), "UHF Radio 1 Volume", $"{ImageLocation}UFC_Knob_1.png");
            AddPot("UHF Radio 4 Volume", new Point(473d, 457d), new Size(90d, 90d), "UHF Radio 4 Volume", $"{ImageLocation}RadioVol_Knob_Back.png");
            AddPot("UHF Radio 2 Volume", new Point(496d, 479d), new Size(45d, 45d), "UHF Radio 2 Volume", $"{ImageLocation}UFC_Knob_1.png");
            AddPot("UFC LCD Brightness", new Point(53d, 592d), new Size(75d, 75d), "UFC LCD Brightness", $"{ImageLocation}UFC_Knob_1a.png");

            int buttonNumber = 1;
            for(double i=60;i<=296;i += 59)
            {
                AddOSBButton($"Option Push Button {buttonNumber} Left", new Rect(39d, i, 37d, 37d), $"Option Push Button {buttonNumber} Left");
                AddOSBButton($"Option Push Button {buttonNumber} Right", new Rect(542d, i, 37d, 37d), $"Option Push Button {buttonNumber++} Right");
            }
            buttonNumber = 0;
            for (int j = 418; j <= 610; j += 64)
            {
                for (int i = 164;i <= 420; i+=64)
                {
                    string keyName;
                    if(labels[buttonNumber] == "GREC_L")
                    {
                        keyName = "Left Guard Receiver";
                    } else if(labels[buttonNumber] == "GREC_R")
                    {
                        keyName = "Right Guard Receiver";
                    } else
                    {
                        keyName = labels[buttonNumber]; 
                    }
                    AddKeypadButton($"{keyName} Key", new Rect(i, j, 44d, 44d), labels[buttonNumber++]);
                }
            }
            AddKeypadButton($"Emission Limit Key", new Rect(513, 612, 57d, 47d), "EMIS_LMT");



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
            // in this instance, we want to allow the panels to be hide-able so the actions need to be added
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

        private void AddKeypadButton(string name, Rect rect, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y;
            button.Left = rect.X;
            button.Width = rect.Width;
            button.Height = rect.Height;

            button.Image = $"{ImageLocation}UFC_{label}_Button.png";
            button.PushedImage = $"{ImageLocation}UFC_{label}_Button_Pressed.png";
            button.Name = name;
            AddButtonBindings(button, name);
        }
        private void AddRadioButton(string name, Rect rect, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y;
            button.Left = rect.X;
            button.Width = rect.Width;
            button.Height = rect.Height;

            button.Image = $"{ImageLocation}UFC_Knob_2_Inner.png";
            button.PushedImage = $"{ImageLocation}UFC_Knob_2_Inner.png";
            button.Name = name;
            AddButtonBindings(button, name);
        }

        private void AddOSBButton(string name, Rect rect, string label)
        {
        Helios.Controls.PushButton button = new Helios.Controls.PushButton();
        button.Top = rect.Y;
        button.Left = rect.X;
        button.Width = rect.Width;
        button.Height = rect.Height;

        button.Image = $"{ImageLocation}OSB_UFC_Button_Normal.png";
        button.PushedImage = $"{ImageLocation}OSB_UFC_Button_Pressed.png";
        button.Name = name;
            AddButtonBindings(button, name);

        }
        private void AddButtonBindings(PushButton button, string name)
        {
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
                interfaceActionName: $"{Name}.push.{name}"
                );
            AddDefaultOutputBinding(
                childName: name,
                deviceTriggerName: "released",
                interfaceActionName: $"{Name}.release.{name}"
                );
            AddDefaultInputBinding(
                childName: name,
                interfaceTriggerName: $"{Name}.{name}.changed",
                deviceActionName: "set.physical state");
        }
        private void AddEncoder(string name, Point posn, Size size, string interfaceElementName)
        {
            AddEncoder(
                name: name,
                size: size,
                posn: posn,
                knobImage: $"{ImageLocation}UFC_Knob_2.png",
                stepValue: 0.1,
                rotationStep: 5,
                interfaceDeviceName: _interfaceDevice,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }
        protected void AddPot(string name, Point posn, Size size, string interfaceElementName, string knobImage)
        {
            Potentiometer knob = AddPot(
                name: name,
                posn: posn,
                size: size,
                knobImage: knobImage,
                initialRotation: 225,
                rotationTravel: 270,
                minValue: 0,
                maxValue: 1,
                initialValue: 1,
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDevice,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            knob.Name = Name + "_" + name;
        }

        private void AddImage(string name, Point posn, Size size, string imageName)
        {
            ImageTranslucent image = new ImageTranslucent()
            {
                Name = name,
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
        }
        private void AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary, Color textColor)
        {
            //string font = "MS 33558";
            string font = "Helios Virtual Cockpit F-15E_Up_Front_Controller";
            
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: textColor,
                backgroundColor: Color.FromArgb(0x20, 0x04, 0x2a, 0x00),
                useBackground: true,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
            display.TextValue = "";
        }
        protected void AddThreeWayToggle(string name, double x, double y, Size size, string interfaceElementName)
        {

            AddThreeWayToggle(
                name: name,
                posn: new Point(x, y),
                size: size,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.OnOnOn,
                interfaceDeviceName: _interfaceDevice,
                interfaceElementName: interfaceElementName,
                positionOneImage: $"{ImageLocation}UFC_Toggle_1_Up.png",
                positionTwoImage: $"{ImageLocation}UFC_Toggle_1_Mid.png", 
                positionThreeImage: $"{ImageLocation}UFC_Toggle_1_Down.png",

                fromCenter: false
                );
        }
        protected void AddIndicatorPushButton(string name, Point pos, Size size, string image,
            string interfaceDeviceName = "", string interfaceElementName = "", string interfaceElementIndicatorName = "")
        {
            string componentName = $"{Name}_{name}";

            IndicatorPushButton indicator = new Helios.Controls.IndicatorPushButton
            {
                Top = pos.Y,
                Left = pos.X,
                Width = size.Width,
                Height = size.Height,
                Image = $"{ImageLocation}Master_Mode_Buttons_Norm.png",
                PushedImage = $"{ImageLocation}Master_Mode_Buttons_Pressed.png",
                IndicatorOnImage = $"{ImageLocation}Master_Mode_Button_{image}_Lit.png",
                PushedIndicatorOnImage = $"{ImageLocation}Master_Mode_Button_{image}_Lit.png",
                Name = componentName,
                OnTextColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                TextColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00)
            };
            indicator.Text = "";

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
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementIndicatorName + ".changed",
                deviceActionName: "set.indicator");
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.physical state");
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "pushed",
                interfaceActionName: interfaceDeviceName + ".push." + interfaceElementName);
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "released",
                interfaceActionName: interfaceDeviceName + ".release." + interfaceElementName);
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
        public override string DefaultBackgroundImage => $"{ImageLocation}{Panel_Image}Base.png";

        protected override void OnBackgroundImageChange()
        {
            _frameBezelPanel.BackgroundImage = BackgroundImageIsCustomized ? null : $"{ImageLocation}{Panel_Image}Base.png";
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
        protected string InterfaceDevice
        {
            get => _interfaceDevice;
            set => _interfaceDevice = value;    
        }
     }
}
