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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.MFD
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using NLog.Filters;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.AH64D.MFD", "Multi Function Display", "AH-64D", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class MFD : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(109, 88, 500, 500);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{Helios}/Images/AH-64D/MFD/MFD_Frame.png";


        public MFD(string interfaceDevice)
            : base(interfaceDevice, new Size(1469 / 2, 1381 / 2))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.AH64D.AH64DInterface) };
            _interfaceDevice = interfaceDevice;
            switch (_interfaceDevice)
            {
                case "MFD Left (Pilot)":
                    _vpName = "AH_64D_LEFT_MFCD_PLT";
                    break;
                case "MFD Right (Pilot)":
                    _vpName = "AH_64D_RIGHT_MFCD_PLT";
                    break;
                case "MFD Left (CP/G)":
                    _vpName = "AH_64D_LEFT_MFCD_CPG";
                    break;
                case "MFD Right (CP/G)":
                    _vpName = "AH_64D_RIGHT_MFCD_CPG";
                    break;
                default:
                    break;
            }
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);
            _frameGlassPanel = AddPanel("MFD Glass", new Point(Left + (109), Top + (88)), new Size(500d, 500d), "{Helios}/Images/AH-64D/MFD/MFD_glass.png", _interfaceDevice);
            _frameGlassPanel.Opacity = 0.3d;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;

            _frameBezelPanel = AddPanel("MFD Frame", new Point(Left, Top), NativeSize, "{Helios}/Images/AH-64D/MFD/MFD_Frame.png", _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;


            double ypos = 20;
            AddButton("Button T1", new Point(183, ypos));
            AddButton("Button T2", new Point(245, ypos));
            AddButton("Button T3", new Point(307, ypos));
            AddButton("Button T4", new Point(369, ypos));
            AddButton("Button T5", new Point(430, ypos));
            AddButton("Button T6", new Point(493, ypos));
            double xpos = 640;
            AddButton("Button Asterisk", new Point(xpos, 102), 50d, "*");
            AddButton("Button R1", new Point(xpos, 163));
            AddButton("Button R2", new Point(xpos, 223));
            AddButton("Button R3", new Point(xpos, 284));
            AddButton("Button R4", new Point(xpos, 346));
            AddButton("Button R5", new Point(xpos, 400));
            AddButton("Button R6", new Point(xpos, 455));
            AddButton("Button VID", new Point(xpos, 512), 50d, "VID");
            AddButton("Button COM", new Point(xpos, 568), 50d, "COM");
            xpos = 40;
            AddButton("Button L1", new Point(xpos, 168));
            AddButton("Button L2", new Point(xpos, 228));
            AddButton("Button L3", new Point(xpos, 286));
            AddButton("Button L4", new Point(xpos, 343));
            AddButton("Button L5", new Point(xpos, 397));
            AddButton("Button L6", new Point(xpos, 454));
            xpos = 30;
            AddButton("Button FCR", new Point(xpos, 513), 50d, "FCR");
            AddButton("Button WPN", new Point(xpos, 573), 50d, "WPN");
            ypos = 620;
            AddButton("Button TSD", new Point(109, ypos), 50d, "TSD");
            AddButton("Button B1/M(Menu)", new Point(183, ypos), 40d, "M");
            AddButton("Button B2", new Point(245, ypos));
            AddButton("Button B3", new Point(307, ypos));
            AddButton("Button B4", new Point(369, ypos));
            AddButton("Button B5", new Point(430, ypos));
            AddButton("Button B6", new Point(493, ypos));
            AddButton("Button A/C", new Point(553, ypos), 50d, "A/C");

            AddThreePositionRotarySwitch("Display Mode", new Point(551d, 7d), new Size(62d, 62d), _interfaceDevice, "Mode Knob");
            AddPot("Brightness Control", new Point(24d, 91d), new Size(62d, 62d), "Brightness Control Knob");
            AddPot("Video Control", new Point(108d, 7d), new Size(62d, 62d), "Video Control Knob");

        }
        public string ViewportName
        {
            get => _vpName;
            set
            {
                if (_vpName != value)
                {
                    if (_vpName == "")
                    {
                        AddViewport(value);
                        OnDisplayUpdate();
                    }
                    else if (value != "")
                    {
                        foreach (HeliosVisual visual in this.Children)
                        {
                            if (visual.TypeIdentifier == "Helios.Base.ViewportExtent")
                            {
                                Controls.Special.ViewportExtent viewportExtent = visual as Controls.Special.ViewportExtent;
                                viewportExtent.ViewportName = value;
                                break;
                            }
                        }
                    }
                    else
                    {
                        RemoveViewport(value);
                    }
                    OnPropertyChanged("ViewportName", _vpName, value, false);
                    _vpName = value;
                }
            }
        }
        public bool RequiresPatches
        {
            get => _vpName != "" ? true : false;
            set => _ = value;
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
        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(109, 93, 500, 500);
            vpRect.Scale(Width / NativeSize.Width, Height / NativeSize.Height);
            TextFormat tf = new TextFormat()
            {
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Normal,
                FontSize = 1.2,
                FontFamily = ConfigManager.FontManager.GetFontFamilyByName("Franklin Gothic"),
                ConfiguredFontSize = 1.2,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                VerticalAlignment = TextVerticalAlignment.Center
            };
            Children.Add(new Helios.Controls.Special.ViewportExtent
            {
                FillBackground = true,
                BackgroundColor = Color.FromArgb(128, 128, 0, 0),
                FontColor = Color.FromArgb(255, 255, 255, 255),
                ViewportName = name,
                TextFormat = tf,
                Left = vpRect.Left,
                Top = vpRect.Top,
                Width = vpRect.Width,
                Height = vpRect.Height
            });
        }
        private void RemoveViewport(string name)
        {
            _includeViewport = false;
            foreach (HeliosVisual visual in this.Children)
            {
                if (visual.TypeIdentifier == "Helios.Base.ViewportExtent")
                {
                    Children.Remove(visual);
                    break;
                }
            }
        }
        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 40, 40), true, ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 40), true, label); }
        private void AddButton(string name, Rect rect, bool horizontal, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y * _size_Multiplier;
            button.Left = rect.X * _size_Multiplier;
            button.Width = rect.Width * _size_Multiplier;
            button.Height = rect.Height * _size_Multiplier;

            if (label != "")
            {
                button.Image = "{Helios}/Images/AH-64D/MFD/MFD Button 2 UpH.png";
                button.PushedImage = "{Helios}/Images/AH-64D/MFD/MFD Button 2 DnH.png";
                button.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName("MS 33558");
                button.TextFormat.FontStyle = FontStyles.Normal;
                button.TextFormat.FontWeight = FontWeights.Normal;
                if (label == "*") button.TextFormat.FontSize = 32; else button.TextFormat.FontSize = 16;
                button.TextFormat.PaddingLeft = 0;
                button.TextFormat.PaddingRight = 0;
                button.TextFormat.PaddingTop = 0;
                button.TextFormat.PaddingBottom = 0;
                button.TextColor = Color.FromArgb(230, 240, 240, 240);
                button.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
                button.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                button.Text = label;
            }
            else
            {
                button.Image = "{Helios}/Images/AH-64D/MFD/MFD Button 1 UpH.png";
                button.PushedImage = "{Helios}/Images/AH-64D/MFD/MFD Button 1 DnH.png";
            }
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
        private void AddThreePositionRotarySwitch(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName)
        {
            Helios.Controls.RotarySwitch knob = new Helios.Controls.RotarySwitch();
            knob.Name = Name + "_" + name;
            knob.KnobImage = "{Helios}/Images/AH-64D/Common/Common Knob.png";
            knob.DrawLabels = false;
            knob.DrawLines = false;
            knob.Positions.Clear();
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 0, "Day", 45d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 1, "Night", 90d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 2, "Mono", 135d));
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
                interfaceDeviceName: _interfaceDevice,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            knob.Name = Name + "_" + name;
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

        public override string DefaultBackgroundImage => "{Helios}/Images/AH-64D/MFD/MFD_Frame.png";

        protected override void OnBackgroundImageChange()
        {
            _frameBezelPanel.BackgroundImage = BackgroundImageIsCustomized ? null : PANEL_IMAGE;
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

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (_includeViewport)
            {
                writer.WriteElementString("EmbeddedViewportName", _vpName);
            }
            else
            {
                writer.WriteElementString("EmbeddedViewportName", "");
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            _includeViewport = true;
            if (reader.Name != "EmbeddedViewportName")
            {
                return;
            }
            _vpName = reader.ReadElementString("EmbeddedViewportName");
            if (_vpName == "")
            {
                _includeViewport = false;
                RemoveViewport("");
            }
        }
    }
}
