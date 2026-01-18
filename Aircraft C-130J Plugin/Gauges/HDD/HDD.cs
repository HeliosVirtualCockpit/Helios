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

namespace GadrocsWorkshop.Helios.Gauges.C130J.HDD
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using NLog.Filters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.C130J.HDD", "HDD", "C-130J Hercules", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class HDD : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(60, 89, 465, 619);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private readonly string _interfaceDevice = "";
        private readonly string _interfaceElement = "";
        //private HeliosPanel _frameGlassPanel;
        //private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{C-130J}/Gauges/HDD/HDD_Bezel.png";
        private const string IMAGE_PATH = "{C-130J}/Gauges/HDD/";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.30d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;

        public HDD(string interfaceDevice)
            : base(interfaceDevice, new Size(586, 800))
        {
            bool slip = false;
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.C130J.C130JInterface) };
            _interfaceDevice = interfaceDevice;
            switch (_interfaceDevice)
            {
                case "HDD Pilot Left":
                    _vpName = "C130J_HDD_1";
                    _interfaceDevice = "Displays Pilot";
                    _interfaceElement = "HDD 1 Brightness";
                    slip = true;
                    break;
                case "HDD Pilot Right":
                    _vpName = "C130J_HDD_2";
                    _interfaceDevice = "Displays Pilot";
                    _interfaceElement = "HDD 2 Brightness";
                    slip = false;
                    break;
                case "HDD Copilot Left":
                    _vpName = "C130J_HDD_3";
                    _interfaceDevice = "Displays Copilot";
                    _interfaceElement = "HDD 3 Brightness";
                    slip = false;
                    break;
                case "HDD Copilot Right":
                    _vpName = "C130J_HDD_4";
                    _interfaceDevice = "Displays Copilot";
                    _interfaceElement = "HDD 4 Brightness";
                    slip = true;
                    break;
                default:
                    break;
            }

            if (!slip)
            {
                AddImage("Blanking", new Point(225, 38), new Size(136, 40), $"{IMAGE_PATH}HDD_Blank.png");
            }
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);
            //_frameGlassPanel = AddPanel("MFD Glass", new Point(Left + (109), Top + (88)), new Size(500d, 500d), "{AH-64D}/Images/MFD/MFD_glass.png", _interfaceDevice);
            //_frameGlassPanel.Opacity = _glassReflectionOpacity;
            //_frameGlassPanel.DrawBorder = false;
            //_frameGlassPanel.FillBackground = false;

            //_frameBezelPanel = AddPanel("CNI Panel", new Point(Left, Top), NativeSize, PANEL_IMAGE, _interfaceDevice);
            //_frameBezelPanel.Opacity = 1d;
            //_frameBezelPanel.FillBackground = false;
            //_frameBezelPanel.DrawBorder = false;

            AddRocker("HDD_Brt", new Point(8, 467), new Size(30, 65), _interfaceDevice, _interfaceElement);
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
            get => false;
            set => _ = value;
        }
        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(60, 89, 465, 619);
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
                BackgroundColor = Color.FromArgb(0x80, 0xd9, 0x27, 0x62),
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
        private void AddRocker(string image, Point location, Size size, string device, string element)
        {
            RockerSwitch rocker = new RockerSwitch();
            rocker.Name = element;
            rocker.SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            rocker.ClickType = LinearClickType.Touch;
            rocker.Top = location.Y;
            rocker.Left = location.X;
            rocker.PositionOneImage = $"{IMAGE_PATH}{image}_Inc.png";
            rocker.PositionTwoImage = $"{IMAGE_PATH}{image}_Norm.png";
            rocker.PositionThreeImage = $"{IMAGE_PATH}{image}_Dec.png";
            rocker.Height = size.Height;
            rocker.Width = size.Width;
            rocker.Text = "";

            Children.Add(rocker);

            AddTrigger(rocker.Triggers["position one.entered"], element);
            AddTrigger(rocker.Triggers["position one.exited"], element);
            AddTrigger(rocker.Triggers["position two.entered"], element);
            AddTrigger(rocker.Triggers["position two.exited"], element);
            AddTrigger(rocker.Triggers["position three.entered"], element);
            AddTrigger(rocker.Triggers["position three.exited"], element);
            AddTrigger(rocker.Triggers["position.changed"], element);
            AddDefaultOutputBinding(
                childName: element,
                deviceTriggerName: "position.changed",
                interfaceActionName: $"{device}.set.{element}");

            AddAction(rocker.Actions["set.position"], element);
            AddDefaultInputBinding(
                childName: element,
                interfaceTriggerName: $"{device}.{element}.changed",
                deviceActionName: "set.position");
        }
        private void AddImage(string name, Point posn, Size size, string imageName)
        {
            Children.Add(new ImageTranslucent()
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
            });
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

        public override string DefaultBackgroundImage => $"{IMAGE_PATH}HDD_Bezel.png";

        protected override void OnBackgroundImageChange()
        {
            //_frameBezelPanel.BackgroundImage = BackgroundImageIsCustomized ? null : PANEL_IMAGE;
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
                    //_frameGlassPanel.IsHidden = _glassReflectionOpacity == 0d ? true : false;
                    //_frameGlassPanel.Opacity = _glassReflectionOpacity;
                }
            }
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
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            base.WriteXml(writer);
            if (_includeViewport)
            {
                writer.WriteElementString("EmbeddedViewportName", _vpName);
                if (RequiresPatches) writer.WriteElementString("RequiresPatches", boolConverter.ConvertToInvariantString(RequiresPatches));
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
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            base.ReadXml(reader);
            _includeViewport = true;
            ViewportName = reader.Name.Equals("EmbeddedViewportName") ? reader.ReadElementString("EmbeddedViewportName") : "";
            RequiresPatches = reader.Name.Equals("RequiresPatches") ? (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("RequiresPatches")) : false;
            if (_vpName == "")
            {
                _includeViewport = false;
                RemoveViewport("");
            }
            GlassReflectionOpacity = reader.Name.Equals("GlassReflectionOpacity") ? double.Parse(reader.ReadElementString("GlassReflectionOpacity"), CultureInfo.InvariantCulture) : 0d;
        }
    }
}
