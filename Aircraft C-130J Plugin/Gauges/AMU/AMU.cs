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

namespace GadrocsWorkshop.Helios.Gauges.C130J.AMU
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

    [HeliosControl("Helios.C130J.AMU", "AMU", "C-130J Hercules", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class AMU : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(56, 32, 379, 283);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        // private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{C-130J}/Gauges/AMU/AMU_Bezel.png";
        private const string IMAGE_PATH = "{C-130J}/Gauges/AMU/";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.30d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;
        private List<hControl> _dimensions;

        public AMU(string interfaceDevice)
            : base(interfaceDevice, new Size(1000, 350))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.C130J.C130JInterface) };
            _interfaceDevice = interfaceDevice;
            InitDimensions();
            switch (_interfaceDevice)
            {
                case "AMU Pilot":
                    _vpName = "C130J_PILOT_AMU";
                    break;
                case "AMU Copilot":
                    _vpName = "C130J_COPILOT_AMU";
                    break;
                 default:
                    break;
            }

            if (_vpName != "" && _includeViewport)
            {
                AddViewport($"{_vpName}_LEFT", new Rect(56, 34, 379, 283));
                AddViewport($"{_vpName}_RIGHT", new Rect(563, 34, 379, 283));
            }

            //_frameGlassPanel = AddPanel("MFD Glass", new Point(Left + (109), Top + (88)), new Size(500d, 500d), "{AH-64D}/Images/MFD/MFD_glass.png", _interfaceDevice);
            //_frameGlassPanel.Opacity = _glassReflectionOpacity;
            //_frameGlassPanel.DrawBorder = false;
            //_frameGlassPanel.FillBackground = false;

            //_frameBezelPanel = AddPanel("CNI Panel", new Point(Left, Top), NativeSize, PANEL_IMAGE, _interfaceDevice);
            //_frameBezelPanel.Opacity = 1d;
            //_frameBezelPanel.FillBackground = false;
            //_frameBezelPanel.DrawBorder = false;

            foreach (hControl amuC in _dimensions)
            {
                switch (amuC.Type)
                {
                    case "BUTTON":
                        AddButton(amuC.Image, amuC.Location, amuC.Size, amuC.Device, amuC.Element);
                        break;
                    case "ROCKER":
                        AddRocker(amuC.Image, amuC.Location, amuC.Size, amuC.Device, amuC.Element);
                        break;
                    default:
                        break;
                }
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
                        AddViewport($"{value}_LEFT", new Rect(56, 34, 379, 283));
                        AddViewport($"{value}_RIGHT", new Rect(563, 34, 379, 283));
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
        private void AddViewport(string name, Rect vpRect)
        {
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
        private void AddButton(string image, Point location, Size size, string device, string element)
        {
            PushButton button = new PushButton();
            button.Top = location.Y * _size_Multiplier;
            button.Left = location.X * _size_Multiplier;
            button.Width = size.Width * _size_Multiplier;
            button.Height = size.Height * _size_Multiplier;
            button.Image = $"{IMAGE_PATH}{image}_Norm.png";
            button.PushedImage = $"{IMAGE_PATH}{image}_Pressed.png";
            button.Text = "";
            button.Name = element ;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], element);
            AddTrigger(button.Triggers["released"], element);

            AddAction(button.Actions["push"], element);
            AddAction(button.Actions["release"], element);
            AddAction(button.Actions["set.physical state"], element);
            // add the default bindings
            AddDefaultOutputBinding(
                childName: element,
                deviceTriggerName: "pushed",
                interfaceActionName: $"{device}.push.{element}"
                );
            AddDefaultOutputBinding(
                childName: element,
                deviceTriggerName: "released",
                interfaceActionName: $"{device}.release.{element}"
                );
            AddDefaultInputBinding(
                childName: element,
                interfaceTriggerName: $"{device}.{element}.changed",
                deviceActionName: "set.physical state");
        }
        private void AddRocker(string image, Point location, Size size, string device, string element)
        {
            RockerSwitch rocker = new RockerSwitch();
            rocker.Name = element;
            rocker.SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            rocker.Rotation = HeliosVisualRotation.CW;
            rocker.ClickType = LinearClickType.Touch;
            rocker.Top = location.Y;
            rocker.Left = location.X;
            rocker.PositionOneImage = $"{IMAGE_PATH}{image}_Incr.png";
            rocker.PositionTwoImage = $"{IMAGE_PATH}{image}_norm.png";
            rocker.PositionThreeImage = $"{IMAGE_PATH}{image}_Decr.png";
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

        public override string DefaultBackgroundImage => $"{IMAGE_PATH}AMU_Bezel.png";

        protected override void OnBackgroundImageChange()
        {
            _frameBezelPanel.BackgroundImage = BackgroundImageIsCustomized ? null : PANEL_IMAGE;
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
        private void InitDimensions()
        {  
             _dimensions = new List<hControl> () {
                {new hControl("AMU_LSK_Left", new Rect(4,56,37,44), "BUTTON",  _interfaceDevice, "Left LSK L1")},
                {new hControl("AMU_LSK_Left", new Rect(4,109,37,44), "BUTTON",  _interfaceDevice, "Left LSK L2")},
                {new hControl("AMU_LSK_Left", new Rect(4,162,37,44), "BUTTON",  _interfaceDevice, "Left LSK L3")},
                {new hControl("AMU_LSK_Left", new Rect(4,215,37,44), "BUTTON",  _interfaceDevice, "Left LSK L4")},
                {new hControl("AMU_LSK_Left", new Rect(511,56,37,44), "BUTTON",  _interfaceDevice, "Right LSK L1")},
                {new hControl("AMU_LSK_Left", new Rect(511,109,37,44), "BUTTON", _interfaceDevice, "Right LSK L2")},
                {new hControl("AMU_LSK_Left", new Rect(511,162,37,44), "BUTTON", _interfaceDevice, "Right LSK L3")},
                {new hControl("AMU_LSK_Left", new Rect(511,215,37,44), "BUTTON", _interfaceDevice, "Right LSK L4")},
                {new hControl("AMU_LSK_Right", new Rect(450,83,37,44), "BUTTON", _interfaceDevice, "Left LSK R1")},
                {new hControl("AMU_LSK_Right", new Rect(450,136,37,44), "BUTTON", _interfaceDevice, "Left LSK R2")},
                {new hControl("AMU_LSK_Right", new Rect(450,189,37,44), "BUTTON", _interfaceDevice, "Left LSK R3")},
                {new hControl("AMU_LSK_Right", new Rect(450,242,37,44), "BUTTON", _interfaceDevice, "Left LSK R4")},
                {new hControl("AMU_LSK_Right", new Rect(956,83,37,44), "BUTTON", _interfaceDevice, "Right LSK R1")},
                {new hControl("AMU_LSK_Right", new Rect(956,136,37,44), "BUTTON", _interfaceDevice, "Right LSK R2")},
                {new hControl("AMU_LSK_Right", new Rect(956,189,37,44), "BUTTON", _interfaceDevice, "Right LSK R3")},
                {new hControl("AMU_LSK_Right", new Rect(956,242,37,44), "BUTTON", _interfaceDevice, "Right LSK R4")},
                {new hControl("AMU_Brt", new Rect(460,298,33,77), "ROCKER", _interfaceDevice, "Brightness Switch")},             };
        }
    }
    internal class hControl
    {
        private readonly string _image;
        private readonly Rect _rect;
        private readonly string _controlType;
        private readonly string _device;
        private readonly string _element;

        internal hControl(string image, Rect rect, string controlType, string device, string element)
        {
            _image = image;
            _rect = rect;
            _controlType = controlType;
            _element = element;
            _device = device;
        }
        #region Properties
        internal Point Location { get => new Point(_rect.X, _rect.Y); }
        internal Size Size { get => new Size(_rect.Width, _rect.Height); }
        internal String Type { get => _controlType; }
        internal String Element { get => _element; }
        internal String Device { get => _device; }
        internal String Image { get => _image; }

        #endregion Properties
    }
}
