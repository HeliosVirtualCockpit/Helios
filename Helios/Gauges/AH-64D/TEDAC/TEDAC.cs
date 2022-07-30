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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.TEDAC
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Xml;
    using System.Windows;
    using System.Windows.Media;


    [HeliosControl("Helios.AH64D.TEDAC", "TADS Electronic Display and Control", "AH-64D", typeof(BackgroundImageRenderer))]
    public class TEDAC : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(142, 150, 800, 800);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "TEDAC";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";

        public TEDAC()
            : base("TEDAC", new Size(1089, 1080))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.AH64D.AH64DInterface) };

            _vpName = "AH_64D_TEDAC";
            if (_includeViewport) AddViewport(_vpName);
            _frameGlassPanel = AddPanel("TEDAC Glass", new Point(142, 150), new Size(800d, 800d), "{Helios}/Images/AH-64D/MFD/MFD_glass.png", _interfaceDevice);
            _frameGlassPanel.Opacity = 0.3d;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground=false;
 
            _frameBezelPanel = AddPanel("TEDAC Frame", new Point(Left, Top), NativeSize, "{Helios}/Images/AH-64D/TEDAC/TEDAC_frame.png", _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;

            double ypos = 1003;
            AddButton("AZ/EL Boresight Enable Button", new Point(395, ypos));
            AddButton("ACM Button", new Point(538, ypos));
            AddButton("FREEZE Button", new Point(678, ypos));
            AddButton("FILTER Button", new Point(824, ypos));
            AddButton("Asterisk (*) Button", new Point(1011,819), 60d,"*");
            ypos = 31;
            AddButton("TAD Video Select Button", new Point(159, ypos), 90d, "TAD");
            AddButton("FCR Video Select Button", new Point(337, ypos), 90d, "FCR");
            AddButton("PNV Video Select Button", new Point(487, ypos), 90d, "PNV");
            AddButton("G/S Video Select Button", new Point(641, ypos), 90d, "G/S");

            AddRocker("Symbol Rocker", 1009, 232, _interfaceDevice, "SYM Rocker Switch");
            AddRocker("Brightness Rocker", 1009, 418, _interfaceDevice, "BRT Rocker Switch");
            AddRocker("Contrast Rocker", 1009, 610, _interfaceDevice, "CON Rocker Switch");
            AddRocker("R/F Rocker", 16, 610, _interfaceDevice, "R/F Rocker Switch");
            AddRocker("Elevation Rocker", 16, 802, _interfaceDevice, "EL Adjust Rocker Switch");
            AddRocker("Azimuth Rocker", 190, 1003, _interfaceDevice, "AZ Adjust Rocker Switch", true);

            AddThreePositionRotarySwitch("Display Mode", new Point(823d,12d),new Size(95d,95d), _interfaceDevice, "Display Mode");
            AddPot("FLIR Gain", new Point(0, 417), new Size(95d, 95d), "FLIR GAIN Control");
            AddPot("FLIR Level", new Point(0, 210), new Size(95d, 95d), "FLIR LEV Control");

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
            get => _vpName != "" ? true:false;
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
            Rect vpRect = new Rect(142, 150, 800, 800);
            vpRect.Scale(Width / NativeSize.Width, Height / NativeSize.Height);
            Children.Insert(0,new Controls.Special.ViewportExtent
            {
                FillBackground = true,
                BackgroundColor = Color.FromArgb(128, 128, 0, 0),
                FontColor = Color.FromArgb(255, 255, 255, 255),
                ViewportName = name,
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
        private void AddButton(string name, Point pos) { AddButton(name, new Rect(pos.X, pos.Y, 60, 60), true, ""); }
        private void AddButton(string name, Point pos, double buttonWidth, string label) { AddButton(name, new Rect(pos.X, pos.Y, buttonWidth, 60), true, label); }
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
                if (label == "*") button.TextFormat.FontSize = 32; else button.TextFormat.FontSize = 20;
                button.TextFormat.PaddingLeft = 0;
                button.TextFormat.PaddingRight = 0;
                button.TextFormat.PaddingTop = 0;
                button.TextFormat.PaddingBottom = 0;
                button.TextFormat.FontWeight = FontWeights.SemiBold;
                button.TextColor = Color.FromArgb(0xE0, 0xf0, 0xf0, 0xf0);
                button.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
                button.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;
                button.Text = label;
                button.TextFormat.ConfiguredFontSize = button.TextFormat.FontSize;
            } else
            {
                button.Image = "{Helios}/Images/AH-64D/MFD/MFD Button 1 UpH.png";
                button.PushedImage = "{Helios}/Images/AH-64D/MFD/MFD Button 1 DnH.png";
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
        private void AddThreePositionRotarySwitch(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName)
        {
            Helios.Controls.RotarySwitch knob = new Helios.Controls.RotarySwitch();
            knob.Name = Name + "_" + name; 
            knob.KnobImage = "{Helios}/Images/AH-64D/Common/Common Knob.png";
            knob.DrawLabels = false;
            knob.DrawLines = false;
            knob.Positions.Clear();
            knob.ClickType = RotaryClickType.Swipe;
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 0, "Day", 45d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 1, "Night", 90d));
            knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(knob, 2, "Off", 135d));
            knob.CurrentPosition = 0;
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
        private void AddRocker(string name, double x, double y) { AddRocker(name, x, y, _interfaceDevice, name,false); }
        private void AddRocker(string name, double x, double y, string interfaceDeviceName, string interfaceElementName) { AddRocker(name, x, y, interfaceDeviceName, interfaceElementName, false); }
        private void AddRocker(string name, double x, double y, string interfaceDeviceName, string interfaceElementName, bool horizontal)
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
            rocker.Height = 120;
            rocker.Width = 60;
            string rockerLabelText = (interfaceElementName.Substring(0, 3) == "Ele" || interfaceElementName.Substring(0, 2) == "AZ") ? interfaceElementName.Substring(0, 2).ToUpper() : interfaceElementName.Substring(0, 3).ToUpper();
            rocker.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName("MS 33558");
            rocker.TextFormat.FontSize = 20;
            rocker.TextFormat.ConfiguredFontSize = 20;
            rocker.TextFormat.FontWeight = FontWeights.SemiBold;
            rocker.Text = rockerLabelText;
            rocker.TextColor = Color.FromArgb(0xe0, 0xff, 0xff, 0xff);
            rocker.TextPushOffset= new Point(0, 2);
            Children.Add(rocker);
            if (!horizontal)
            {
            }
            else
            {
                // this is needed because RockerSwitch labels rotate with the switch and for this horizontal rocker, the text needs to be unchanged.
                rocker.Text = "";
                rocker.Rotation = HeliosVisualRotation.CCW;
                Controls.TextDecoration td = new Controls.TextDecoration();
                td.Text = rockerLabelText;
                td.Width = 30;
                td.Height = 60;
                td.Top = y + (rocker.Width / 2) - (td.Height / 2);
                td.Left = x + (rocker.Height / 2) - (td.Width / 2);
                td.FontColor = Color.FromArgb(0xe0, 0xff, 0xff, 0xff);
                td.Format.FontSize = 14;
                td.Format.FontWeight = FontWeights.SemiBold;
                td.Format.ConfiguredFontSize = 14;
                td.Format.PaddingLeft = 0;
                td.Format.PaddingRight = 0;
                td.Format.PaddingTop = 0;
                td.Format.PaddingBottom = 0;
                td.Format.VerticalAlignment = TextVerticalAlignment.Center;
                td.Format.HorizontalAlignment = TextHorizontalAlignment.Center;
                td.Format.FontFamily = ConfigManager.FontManager.GetFontFamilyByName("MS 33558");
                Children.Add(td);
            }

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

        public override string DefaultBackgroundImage
        {
            get { return null; }
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
                foreach (HeliosVisual visual in this.Children)
                {
                    if(visual.TypeIdentifier == "Helios.Base.ViewportExtent")
                    {
                        Children.Remove(visual);
                        break;
                    }
                }
            }
        }
    }
}
