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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.KU
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.KU", "Keyboard Unit", "AH-64D", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class KU : CompositeVisualWithBackgroundImage
    {
        private Dictionary<HeliosVisual, Rect> _nativeSizes = new Dictionary<HeliosVisual, Rect>();
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameBezelPanel;
        private const string Panel_Image = "{Helios}/Images/AH-64D/KU/KU_Frame.png";

        public KU(string interfaceDevice)
            : base(interfaceDevice, new Size(600, 465))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.AH64D.AH64DInterface) };
            _interfaceDevice = interfaceDevice;
            string[] labels = {"A","B","C","D","E","F","1","2","3","G","H","I","J","K","L","4","5","6","M","N","O","P","Q","R","7","8","9","S","T","U","V","W","X","Decimal","0","+/-","Y","Z","/","BKS","SPC","*","DIV","+","-","CLR","<",">","ENTER"};
            _frameBezelPanel = AddPanel("KU Frame", new Point(Left, Top), NativeSize, "{Helios}/Images/AH-64D/KU/KU_Frame.png", _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;

            int ii = 0;
            for(double buttonY = 105;buttonY <= 335; buttonY += 230 / 4)
            {
                for (double buttonX = 39; buttonX <= 520; buttonX += 481 / 8)
                {
                    AddButton($"Key {labels[ii]}", new Rect(buttonX, buttonY, 40d, 40d), $"{labels[ii++]}");
                }
            }
            AddButton($"Key {labels[ii]}", new Rect(160d, 391d, 40d, 40d), $"{labels[ii++]}");
            AddButton($"Key {labels[ii]}", new Rect(220d, 391d, 40d, 40d), $"{labels[ii++]}");
            AddButton($"Key {labels[ii]}", new Rect(280d, 391d, 40d, 40d), $"{labels[ii++]}");
            AddButton($"Key {labels[ii]}", new Rect(400d, 391d, 96d, 40d), $"{labels[ii++]}");

            // Top Row 104 Bottom Row 334
            // Left col 39 Right Col 520 
            AddPot("Brightness Control", new Point(59d, 389d), new Size(62d, 62d), "Brightness Control Knob");
            AddTextDisplay("Scratchpad", new Point(58d, 32d), new Size(484d, 52d), _interfaceDevice, "Scratchpad", 22, "KEYBOARD UNIT", TextHorizontalAlignment.Left, "#=$;!=:");

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
        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 40, 40), ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 50), label); }
        private void AddButton(string name, Rect rect, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y * _size_Multiplier;
            button.Left = rect.X * _size_Multiplier;
            button.Width = rect.Width * _size_Multiplier;
            button.Height = rect.Height * _size_Multiplier;
            string shape = (button.Top < 280d && button.Left > 398d) ? "round" : "square";

            string image = (label == "N" || label == "S" || label == "E" || label == "W") ? $"{{Helios}}/Images/AH-64D/KU/tactile-dark-{shape}-emphasised" : $"{{Helios}}/Images/Buttons/tactile-dark-{shape}";
            switch (label)
            {
                case "+/-":
                    label = "±";
                    break;
                case "Decimal":
                    label = ".";
                    break;
                case "DIV":
                    label = "÷";
                    break;
                case "<":
                    label = "🠸";
                    break;
                case ">":
                    label = "🠺";
                    break;
                default:
                    break;
            }
            bool boostSize = (label == "*" || label == "." || label == "+" || label == "-" || label == "÷" || label == "±" || label == "🠸" || label == "🠺") ? true : false;
            if (label != "")
            {
                button.Image = $"{image}.png";
                button.PushedImage = $"{image}-in.png";
                button.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName("MS 33558");
                button.TextFormat.FontStyle = FontStyles.Normal;
                button.TextFormat.FontWeight = FontWeights.Bold;
                if (boostSize) button.TextFormat.FontSize = 28; else button.TextFormat.FontSize = 15;
                if (boostSize) {
                    button.TextFormat.FontSize = 28;
                    button.TextFormat.FontWeight = FontWeights.Normal;
                    button.TextFormat.VerticalAlignment = TextVerticalAlignment.Top;
                }
                else {
                    button.TextFormat.FontSize = 15;
                    button.TextFormat.FontWeight = FontWeights.Normal;
                    button.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
                }
                button.TextFormat.PaddingLeft = 0;
                button.TextFormat.PaddingRight = 0;
                button.TextFormat.PaddingTop = 0;
                button.TextFormat.PaddingBottom = 0;

                button.TextColor = Color.FromArgb(230, 240, 240, 240);
                button.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                button.Text = label;
            } else
            {
                button.Image = $"{image}.png";
                button.PushedImage = $"{image}-in.png";
            }
            button.TextFormat.ConfiguredFontSize = button.TextFormat.FontSize;
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
        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            Potentiometer knob = AddPot(
                name: name,
                posn: posn,
                size: size,
                knobImage: "{Helios}/Images/AH-64D/Common/Common Knob.png",
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
        private void AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            //string font = "MS 33558";
            string font = "Helios Virtual Cockpit A-10C_ALQ_213";
            
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xCC, 0x40, 0xB3, 0x29),
                backgroundColor: Color.FromArgb(0x20, 0x04, 0x2a, 0x00),
                useBackground: true,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
            display.TextValue = "";
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
