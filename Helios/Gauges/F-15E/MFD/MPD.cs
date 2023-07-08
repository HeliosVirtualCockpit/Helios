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

namespace GadrocsWorkshop.Helios.Gauges.F15E.MPD
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.F15E.MPD", "Multi Function Display", "F-15E Strike Eagle", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class MPD : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(109, 88, 500, 500);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{F-15E}/Images/MFD/MFD_Bezel.png";


        public MPD(string interfaceDevice)
            : base(interfaceDevice, new Size(846, 834))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.F15E.F15EInterface) };
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
                    _vpName = "F_15E_RIGHT_MPD_WSO";
                    break;
                default:
                    break;
            }
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);
            _frameGlassPanel = AddPanel("MFD Glass", new Point(108, 85), new Size(638, 638), "{Helios}/Images/AH-64D/MFD/MFD_glass.png", _interfaceDevice);
            _frameGlassPanel.Opacity = 0.3d;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;

            _frameBezelPanel = AddPanel("MFD Frame", new Point(Left, Top), NativeSize, PANEL_IMAGE, _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;
            int maxLabelButtons = _interfaceDevice.Contains("MPCD") ? 20 : -1;
            string[] buttonLabel = { "R_", "RC_", "C_", "LC_", "L_" };
            int buttonNumber = 0;

            for (int y = 169; y <= 569; y += 100)
            {
                AddButton($"Push Button {buttonNumber + 1}", new Point(21, y));
                buttonNumber++;
            }
            for (int x = 201; x <= 601; x += 100)
            {
                AddButton($"Push Button {buttonNumber + 1}", new Point(x, 756));
                buttonNumber++;
            }
            for (int y = 569; y >= 169; y -= 100)
            {
                AddButton($"Push Button {buttonNumber + 1}", new Point(755, y));
                buttonNumber++;
            }
            for (int x = 601; x >= 201; x -= 100)
            {
                AddButton($"Push Button {buttonNumber + 1}", new Point(x,6), buttonNumber <= maxLabelButtons ? buttonLabel[buttonNumber-15] : "");
                buttonNumber++;
            }

            AddRocker("Power Switch", 779, 27, _interfaceDevice, "Power Switch");
            AddRocker("Brightness Control", 28, 698, _interfaceDevice, "Brightness Control");
            AddRocker("Contrast Control", 789, 698, _interfaceDevice, "Contrast Control");


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
            Rect vpRect = new Rect(108, 85, 638, 638);
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
        private void AddButton(string name, Point pos, string imageModifier) { AddButton(name, new Rect(pos.X, pos.Y, 60, 60), imageModifier); }
        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 60, 60), ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 60), label); }
        private void AddButton(string name, Rect rect, string label)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = rect.Y * _size_Multiplier;
            button.Left = rect.X * _size_Multiplier;
            button.Width = rect.Width * _size_Multiplier;
            button.Height = rect.Height * _size_Multiplier;

            button.Image = $"{{F-15E}}/Images/MFD/MPC_{label}Button_Norm.png";
            button.PushedImage = $"{{F-15E}}/Images/MFD/MPC_{label}Button_Pressed.png";
            
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
            rocker.PositionOneImage = "{F-15E}/Images/MFD/Rocker_Up.png";
            rocker.PositionTwoImage = "{F-15E}/Images/MFD/Rocker_Norm.png";
            rocker.PositionThreeImage = "{F-15E}/Images/MFD/Rocker_Down.png";
            rocker.Height = 83;
            rocker.Width = 31;
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
