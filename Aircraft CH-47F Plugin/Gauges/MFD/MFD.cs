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

namespace GadrocsWorkshop.Helios.Gauges.CH47F.MFD
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

    [HeliosControl("Helios.CH47F.MFD", "Multi Function Display", "CH-47F Chinook", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class MFD : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(104, 101, 604, 803);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{CH-47F}/Images/MFD/CH_47_MFD_Bezel.png";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.10d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;


        public MFD(string interfaceDevice)
            : base(interfaceDevice, new Size(814, 1000))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.CH47F.CH47FInterface) };
            _interfaceDevice = interfaceDevice;
            switch (_interfaceDevice)
            {
                case "Left MPD (Pilot)":
                    _vpName = "F_15E_LEFT_MPD_PLT";
                    break;
                case "Center MPCD (Pilot)":
                    _vpName = "F_15E_CENTER_MPCD_PLT";
                    break;
                case "Right MPD (Pilot)":
                    _vpName = "F_15E_RIGHT_MPD_PLT";
                    break;
                case "Left MPD (WSO)":
                    _vpName = "F_15E_LEFT_MPD_WSO";
                    break;
                case "Left MPCD (WSO)":
                    _vpName = "F_15E_LEFT_MPCD_WSO";
                    break;
                case "Right MPD (WSO)":
                    _vpName = "F_15E_RIGHT_MPD_WSO";
                    break;
                case "Right MPCD (WSO)":
                    _vpName = "F_15E_RIGHT_MPCD_WSO";
                    break;
                default:
                    break;
            }
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);
            _frameGlassPanel = AddPanel("MFD Glass", new Point(104, 101), new Size(604, 803), "{Helios}/Images/AH-64D/MFD/MFD_glass.png", _interfaceDevice);
            _frameGlassPanel.Opacity = _glassReflectionOpacity;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;

            _frameBezelPanel = AddPanel("MFD Frame", new Point(Left, Top), NativeSize, PANEL_IMAGE, _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;
            int maxLabelButtons = _interfaceDevice.Contains("MFD") ? 20 : -1;
            int buttonNumber = 1;
            for (int x = 122; x <= 614; x += 82)
            {
                AddButton($"T{buttonNumber}", new Point(x, 12), "Horizontal");
                buttonNumber++;
            }
            buttonNumber = 1;
            for (int y = 148; y <= 780; y += 79)
            {
                AddButton($"R{buttonNumber}", new Point(760, y), "Vertical");
                buttonNumber++;
            }
            buttonNumber = 1;
            for (int x = 122; x <= 614; x += 82)
            {
                AddButton($"B{buttonNumber}", new Point(x, 948), "Horizontal");
                buttonNumber++;
            }
            buttonNumber = 1;
            for (int y = 148; y <= 780; y += 79)
            {
                AddButton($"L{buttonNumber}", new Point(9, y), "Vertical");
                buttonNumber++;
            }

            AddRocker("Brightness", 750, 42, _interfaceDevice, "Brightness Switch");
            AddRocker("Contrast", 750, 856, _interfaceDevice, "Brightness Switch");
            AddRocker("Backlight", 5, 856, _interfaceDevice, "Contrast Switch");
            AddThreePositionRotarySwitch("Power", new Point(0, 36), new Size(70, 70), _interfaceDevice, "Power Switch");
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
        private void AddButton(string name, Point pos, string imageModifier) { AddButton(name, new Rect(pos.X, pos.Y, 80, 46), imageModifier); }
        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 80, 46), ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 60), label); }
        private void AddButton(string name, Rect rect, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y * _size_Multiplier;
            button.Left = rect.X * _size_Multiplier;
            button.Width = (label=="Horizontal" ? rect.Width : rect.Height) * _size_Multiplier;
            button.Height = (label == "Horizontal" ? rect.Height : rect.Width) * _size_Multiplier;

            button.Image = $"{{CH-47F}}/Images/MFD/CH_47_MFD_Button_{label}_Normal.png";
            button.PushedImage = $"{{CH-47F}}/Images/MFD/CH_47_MFD_Button_{label}_Pressed.png";
            
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
        private void AddRocker(string name, double x, double y) { AddRocker(name, x, y, _interfaceDevice, name); }
        private void AddRocker(string name, double x, double y, string interfaceDeviceName, string interfaceElementName)
        {
            Helios.Controls.RockerSwitch rocker = new Helios.Controls.RockerSwitch();
            rocker.Name = ComponentName(name);
            rocker.SwitchType = Helios.Controls.ThreeWayToggleSwitchType.MomOnMom;
            rocker.ClickType = Helios.Controls.LinearClickType.Touch;
            rocker.Top = y;
            rocker.Left = x;
            rocker.PositionOneImage = $"{{CH-47F}}/Images/MFD/CH_47_MFD_Rocker_{name}_Up.png";
            rocker.PositionTwoImage = $"{{CH-47F}}/Images/MFD/CH_47_MFD_Rocker_{name}_Normal.png";
            rocker.PositionThreeImage = $"{{CH-47F}}/Images/MFD/CH_47_MFD_Rocker_{name}_Down.png";
            rocker.Height = 100;
            rocker.Width = 64;
            rocker.Text = "";


            Children.Add(rocker);

            AddTrigger(rocker.Triggers["position one.entered"], name);
            AddTrigger(rocker.Triggers["position one.exited"], name);
            AddTrigger(rocker.Triggers["position two.entered"], name);
            AddTrigger(rocker.Triggers["position two.exited"], name);
            AddTrigger(rocker.Triggers["position three.entered"], name);
            AddTrigger(rocker.Triggers["position three.exited"], name);
            AddTrigger(rocker.Triggers["position.changed"], name);
            AddDefaultOutputBinding(
                childName: ComponentName(name),
                deviceTriggerName: "position.changed",
                interfaceActionName: $"{interfaceDeviceName}.set.{interfaceElementName}");

            AddAction(rocker.Actions["set.position"], name);
            AddDefaultInputBinding(
                childName: ComponentName(name),
                interfaceTriggerName: $"{interfaceDeviceName}.{interfaceElementName}.changed",
                deviceActionName: "set.position");
        }
        private void AddThreePositionRotarySwitch(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName)
        {
            Helios.Controls.RotarySwitch knob = new Helios.Controls.RotarySwitch();
            knob.Name = Name + "_" + name;
            knob.KnobImage = "{CH-47F}/Images/MFD/CH_47_MFD_Knob.png";
            knob.DrawLabels = false;
            knob.DrawLines = false;
            knob.Positions.Clear();
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 0, "OFF", 0d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 1, "NVG", 15d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 2, "NORM", 30d));
            knob.CurrentPosition = 0;
            knob.Top = posn.Y;
            knob.Left = posn.X;
            knob.Width = size.Width;
            knob.Height = size.Height;

            AddRotarySwitchBindings(name, posn, size, knob, interfaceDeviceName, interfaceElementName);
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
