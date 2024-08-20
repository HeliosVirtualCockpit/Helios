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

    [HeliosControl("Helios.CH47F.CDU.Top", "CDU Display", "CH-47F Chinook", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class CDU_Top : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(145, 94, 512, 486);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{CH-47F}/Gauges/CDU/Images/CDU_Bezel_Top.png";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.10d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;


        public CDU_Top(string interfaceDevice)
            : base(interfaceDevice, new Size(810, 659))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.CH47F.CH47FInterface) };
            _interfaceDevice = interfaceDevice;
            switch (_interfaceDevice)
            {
                case "CDU (Right)":
                    _vpName = "CH47F_CDU_PILOT";
                    break;
                case "CDU (Left)":
                    _vpName = "CH47F_CDU_COPILOT";
                    break;
                default:
                    break;
            }
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);
            _frameGlassPanel = AddPanel("CDU Glass", new Point(SCREEN_RECT.Left, SCREEN_RECT.Top), new Size(SCREEN_RECT.Width, SCREEN_RECT.Height), "{CH-47F}/Gauges/CDU/Images/CDU_glass.png", _interfaceDevice);
            _frameGlassPanel.Opacity = _glassReflectionOpacity;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;

            _frameBezelPanel = AddPanel("CDU Frame", new Point(Left, Top), NativeSize, PANEL_IMAGE, _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;
            int buttonNumber = 0;
            string[] labels = new string[] {"MSN", "FPLN", "FD", "IDX", "DIR", "SNSR", "MFD_DATA", "L1", "L2", "L3", "L4", "L5", "L6", "R1", "R2", "R3", "R4", "R5", "R6", "BRT", "DIM", "CNI",
                                            "PAD", "arrow left", "arrow right", "arrow up", "arrow down", "CLR", "WPN", "1", "2", "3",
                                            "A", "B", "C", "D", "E", "F", "G", "4", "5", "6", "H", "I", "J", "K", "L", "M", "N", "7", "8", "9", "O", "P", "Q", "R", "S", "T", "U", "0", "dot", "V",
                                            "W", "X", "Y", "Z", "SP", "MARK", "slash", "dash", "TDL", "ASE", "empty", "DATA", "STAT"};

            for (int x = 66; x <= 660; x += 18 + 81)
            {
                AddButton($"{labels[buttonNumber]}", new Rect(x, 4 ,81,60), $"{labels[buttonNumber++]}");
                //buttonNumber++;
            }
            for (int x = 19; x <= 710; x += 691)
            {
                for (int y = 162; y <= 462; y += 60)
                {
                    AddButton($"{labels[buttonNumber]}", new Rect(x, y, 73, 45), $"{labels[buttonNumber++]}");
                }
            }
            for(int y = 547; y <= 607; y += 60)
            {
                AddButton($"{labels[buttonNumber]}", new Rect(19, y, 73, 45), $"{labels[buttonNumber++]}");
            }
            for(int x = 93; x <= 720; x += 89)
            {
                AddButton($"{labels[buttonNumber]}", new Rect(x, 595, 81, 60), $"{labels[buttonNumber++]}");
            }
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
        public double GlassReflectionOpacity
        {
            get
            {
                return _glassReflectionOpacity;
            }
            set
            {
                double oldValue = _glassReflectionOpacity;
                if (value != oldValue)
                {
                    _glassReflectionOpacity = value;
                    OnPropertyChanged("GlassReflectionOpacity", oldValue, value, true);
                    _frameGlassPanel.IsHidden = _glassReflectionOpacity == 0d ? true : false;
                    _frameGlassPanel.Opacity = _glassReflectionOpacity; 

                }
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
            Rect vpRect = SCREEN_RECT;
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
                BackgroundColor = Color.FromArgb(128, 128, 32, 64),
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
        private void AddButton(string name, Point pos, string imageModifier) { AddButton(name, new Rect(pos.X, pos.Y, 80, 46), imageModifier); }
        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 80, 46), ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 60), label); }
        private void AddButton(string name, Rect rect, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y * _size_Multiplier;
            button.Left = rect.X * _size_Multiplier;
            button.Width = rect.Width * _size_Multiplier;
            button.Height = rect.Height * _size_Multiplier;
            string tempLabel = label;
            if (label.Length == 2 && (label.StartsWith("L") || label.StartsWith("R"))) {
                label = label.Substring(0,1) + "x";
            }

            button.Image = $"{{CH-47F}}/Gauges/CDU/Images/CDU_{label.Replace(" ", "_")}_Norm.png";
            button.PushedImage = $"{{CH-47F}}/Gauges/CDU/Images/CDU_{(label.Replace(" ","_"))}_Pressed.png";

            label = tempLabel;
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
            if (_glassReflectionOpacity > 0d)
            {
                writer.WriteElementString("GlassReflectionOpacity", GlassReflectionOpacity.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            _includeViewport = true;
            if (reader.Name.Equals("EmbeddedViewportName"))
            {
                _vpName = reader.ReadElementString("EmbeddedViewportName");
                if (_vpName == "")
                {
                    _includeViewport = false;
                    RemoveViewport("");
                }
            }

            GlassReflectionOpacity = reader.Name.Equals("GlassReflectionOpacity") ? double.Parse(reader.ReadElementString("GlassReflectionOpacity"), CultureInfo.InvariantCulture) : 0d;
        }
    }
}
