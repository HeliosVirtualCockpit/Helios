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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.EUFD
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Xml;
    using System.Windows;
    using System.Windows.Media;
    using System.Globalization;
    using System.ComponentModel;

    [HeliosControl("Helios.AH64D.EUFD", "Enhanced Up Front Display", "AH-64D", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class EUFD : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(173, 25, 600, 300);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string Panel_Image = "{AH-64D}/Images/EUFD/EUFD.png";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.30d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;

        public EUFD(string interfaceDevice)
            : base(interfaceDevice, new Size(1004, 348))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.AH64D.AH64DInterface) };
            _interfaceDevice = interfaceDevice;
            _vpName = "";
            switch (_interfaceDevice)
            {
                case "Up Front Display (Pilot)":
                    _vpName = "AH_64D_EUFD_PLT";
                    break;
                case "Up Front Display (CP/G)":
                    _vpName = "AH_64D_EUFD_CPG";
                    break;
                default:
                    break;
            }
            if (_includeViewport) AddViewport(_vpName);
            _frameGlassPanel = AddPanel("EUFD Glass", new Point(Left + (173), Top + (25)), new Size(600d, 300d), "{AH-64D}/Images/MFD/MFD_glass.png", _interfaceDevice);
            _frameGlassPanel.Opacity = _glassReflectionOpacity;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground=false;
 
            _frameBezelPanel = AddPanel("EUFD Frame", new Point(Left, Top), NativeSize, "{AH-64D}/Images/EUFD/EUFD.png", _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;

            {
                double ypos = 196;
                AddButton("Preset Button", new Point(804, ypos));
                AddButton("Enter Button", new Point(891, ypos));
                ypos = 279;
                AddButton("Stopwatch Button", new Point(804, ypos));
                AddButton("Swap Button", new Point(891, ypos));
            }

            AddRocker("IDM Switch", 12, 99, _interfaceDevice, "IDM Rocker Switch");
            AddRocker("WCA Switch", 91, 24, _interfaceDevice, "WCA Rocker Switch");
            AddRocker("RTS Switch", 91, 181, _interfaceDevice, "RTS Rocker Switch");

            AddPot("Brightness Control", new Point(814d, 81d), new Size(75d, 75d), "Brightness Control Knob");

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
        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(173, 24, 600, 300);
            vpRect.Scale(Width / NativeSize.Width, Height / NativeSize.Height);
            TextFormat tf = new TextFormat()
            {
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Normal,
                FontSize = 2,
                FontFamily = ConfigManager.FontManager.GetFontFamilyByName("Franklin Gothic"),
                ConfiguredFontSize = 2,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                VerticalAlignment = TextVerticalAlignment.Center
            };
            Children.Insert(0, new Helios.Controls.Special.ViewportExtent
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

        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 50, 50), true, ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 50), true, label); }
        private void AddButton(string name, Rect rect, bool horizontal, string label)
        {
            PushButton button = new PushButton();
            button.Top = rect.Y * _size_Multiplier;
            button.Left = rect.X * _size_Multiplier;
            button.Width = rect.Width * _size_Multiplier;
            button.Height = rect.Height * _size_Multiplier;

            if (label != "")
            {
                button.Image = $"{{AH-64D}}/Images/EUFD/EUFD {name} UpH.png";
                button.PushedImage = $"{{AH-64D}}/Images/EUFD/EUFD {name} DnH.png";
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
            } else
            {
                button.Image = $"{{AH-64D}}/Images/EUFD/EUFD {name} UpH.png";
                button.PushedImage = $"{{AH-64D}}/Images/EUFD/EUFD {name} DnH.png";
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
        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            Potentiometer knob = AddPot(
                name: name,
                posn: posn,
                size: size,
                knobImage: "{AH-64D}/Images/Common/Common Knob.png",
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
        private void AddRocker(string name, double x, double y) { AddRocker(name, x, y, _interfaceDevice, name); }
        private void AddRocker(string name, double x, double y, string interfaceDeviceName, string interfaceElementName)
        {
            Helios.Controls.RockerSwitch rocker = new Helios.Controls.RockerSwitch();
            rocker.Name = ComponentName(name);
            rocker.SwitchType = Helios.Controls.ThreeWayToggleSwitchType.MomOnMom;
            rocker.ClickType = Helios.Controls.LinearClickType.Touch;
            rocker.Top = y;
            rocker.Left = x;
            rocker.PositionOneImage = "{Helios}/Images/Rockers/triangles-dark-up.png";
            rocker.PositionTwoImage = "{Helios}/Images/Rockers/triangles-dark-norm.png";
            rocker.PositionThreeImage = "{Helios}/Images/Rockers/triangles-dark-down.png";
            rocker.Height = 140;
            rocker.Width = 50;
            rocker.TextFormat.FontWeight = FontWeights.SemiBold;
            rocker.TextFormat.FontSize = 15;
            rocker.TextFormat.ConfiguredFontSize = 15;
            rocker.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName("MS 33558");
            rocker.Text = interfaceElementName.Substring(0, 3).Replace("IDM","DL");
            rocker.TextColor = Color.FromArgb(0xe0, 0xff, 0xff, 0xff);
            rocker.TextPushOffset = new Point(0, 2);

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
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));
            TypeConverter doubleConverter = TypeDescriptor.GetConverter(typeof(double));

            base.WriteXml(writer);
            if (_includeViewport)
            {
                writer.WriteElementString("EmbeddedViewportName", ViewportName);
                if (RequiresPatches) writer.WriteElementString("RequiresPatches", boolConverter.ConvertToInvariantString(RequiresPatches));
            }
            else
            {
                writer.WriteElementString("EmbeddedViewportName", "");
            }
            if (_glassReflectionOpacity > 0d)
            {
                writer.WriteElementString("GlassReflectionOpacity", doubleConverter.ConvertToInvariantString(GlassReflectionOpacity));
            }

        }
        public override void ReadXml(XmlReader reader)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));
            TypeConverter doubleConverter = TypeDescriptor.GetConverter(typeof(double));

            base.ReadXml(reader);
            _includeViewport = true;

            ViewportName = reader.Name.Equals("EmbeddedViewportName") ? reader.ReadElementString("EmbeddedViewportName") : "";
            RequiresPatches = reader.Name.Equals("RequiresPatches") ? (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("RequiresPatches")) : false;
            if (_vpName == "")
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
            GlassReflectionOpacity = reader.Name.Equals("GlassReflectionOpacity") ? (double)doubleConverter.ConvertFromInvariantString(reader.ReadElementString("GlassReflectionOpacity")) : 0d;
        }
    }
}
