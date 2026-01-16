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

namespace GadrocsWorkshop.Helios.Gauges.C130J.CNI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J;
    using NLog.Filters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.C130J.CNI", "CNI", "C-130J Hercules", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class AMU : CompositeVisualWithBackgroundImage
    {
        private static readonly Rect SCREEN_RECT = new Rect(86, 74, 522, 359);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDevice = "";
        private double _size_Multiplier = 1;
        private HeliosPanel _frameGlassPanel;
        private HeliosPanel _frameBezelPanel;
        private bool _includeViewport = true;
        private string _vpName = "";
        private const string PANEL_IMAGE = "{C-130J}/Gauges/CNI/CNI_Bezel.png";
        private const string IMAGE_PATH = "{C-130J}/Gauges/CNI/";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.30d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;
        private List<CNIControl> _dimensions;

        public AMU(string interfaceDevice)
            : base(interfaceDevice, new Size(693, 1000))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.C130J.C130JInterface) };
            _interfaceDevice = interfaceDevice;
            InitDimensions();
            switch (_interfaceDevice)
            {
                case "CNI Pilot":
                    _vpName = "C130J_PILOT_CNI";
                    break;
                case "CNI Copilot":
                    _vpName = "C130J_COPILOT_CNI";
                    break;
                case "CNI Aug Crew":
                    _vpName = "C130J_AC_CNI";
                    break;
                default:
                    break;
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

            foreach(CNIControl cniC in _dimensions)
            {
                switch (cniC.Type)
                {
                    case "BUTTON":
                        AddButton(cniC.Image, cniC.Location, cniC.Size, cniC.Device, cniC.Element);
                        break;
                    case "INDICATOR":
                        AddIndicator(cniC.Image, cniC.Location, cniC.Size, cniC.Device, cniC.Element);
                        break;
                    case "ROCKER":
                        AddRocker(cniC.Image, cniC.Location, cniC.Size, cniC.Device, cniC.Element);
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
            Rect vpRect = new Rect(86, 74, 522, 359);
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
        private void AddIndicator(string image, Point location, Size size, string device, string element)
        {
            Indicator indicator = new Indicator
            {
                Top = location.Y,
                Left = location.X,
                Width = size.Width,
                Height = size.Height,
                OnImage = $"{IMAGE_PATH}{image}.png",
                OffImage = "",
                Text = "",
                Name = element
            };

            Children.Add(indicator);
            foreach (IBindingTrigger trigger in indicator.Triggers)
            {
                AddTrigger(trigger, $"{device}.{element}");
            }
            AddAction(indicator.Actions["set.indicator"], $"{device}.{element}");

            AddDefaultInputBinding(
                childName: $"{element}",
                interfaceTriggerName: $"{device}.{element}.changed",
                deviceActionName: "set.indicator");
        }
        private void AddRocker(string image, Point location, Size size, string device, string element)
        {
            RockerSwitch rocker = new RockerSwitch();
            rocker.Name = element;
            rocker.SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            rocker.ClickType = LinearClickType.Touch;
            rocker.Top = location.Y;
            rocker.Left = location.X;
            rocker.PositionOneImage = $"{IMAGE_PATH}{image}_UP.png";
            rocker.PositionTwoImage = $"{IMAGE_PATH}{image}_norm.png";
            rocker.PositionThreeImage = $"{IMAGE_PATH}{image}_DN.png";
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

        public override string DefaultBackgroundImage => $"{IMAGE_PATH}CNI_Bezel.png";

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
             _dimensions = new List<CNIControl> () {
                {new CNIControl("CNI_LSK", new Rect(22,126,43,31), "BUTTON",  _interfaceDevice, "MU LSK L1")},
                {new CNIControl("CNI_LSK", new Rect(22,176,43,31), "BUTTON",  _interfaceDevice, "MU LSK L2")},
                {new CNIControl("CNI_LSK", new Rect(22,226,43,31), "BUTTON",  _interfaceDevice, "MU LSK L3")},
                {new CNIControl("CNI_LSK", new Rect(22,276,43,31), "BUTTON",  _interfaceDevice, "MU LSK L4")},
                {new CNIControl("CNI_LSK", new Rect(22,326,43,31), "BUTTON",  _interfaceDevice, "MU LSK L5")},
                {new CNIControl("CNI_LSK", new Rect(22,376,43,31), "BUTTON",  _interfaceDevice, "MU LSK L6")},
                {new CNIControl("CNI_LSK", new Rect(629,126,43,31), "BUTTON",  _interfaceDevice, "MU LSK R1")},
                {new CNIControl("CNI_LSK", new Rect(629,176,43,31), "BUTTON",  _interfaceDevice, "MU LSK R2")},
                {new CNIControl("CNI_LSK", new Rect(629,226,43,31), "BUTTON",  _interfaceDevice, "MU LSK R3")},
                {new CNIControl("CNI_LSK", new Rect(629,276,43,31), "BUTTON",  _interfaceDevice, "MU LSK R4")},
                {new CNIControl("CNI_LSK", new Rect(629,326,43,31), "BUTTON",  _interfaceDevice, "MU LSK R5")},
                {new CNIControl("CNI_LSK", new Rect(629,376,43,31), "BUTTON",  _interfaceDevice, "MU LSK R6")},
                {new CNIControl("Comm_Tune_Key", new Rect(80,475,68,44), "BUTTON",  _interfaceDevice, "MU COMM TUNE Key")},
                {new CNIControl("Nav_Tune_Key", new Rect(168,475,67,44), "BUTTON",  _interfaceDevice, "MU NAV TUNE Key")},
                {new CNIControl("Iff_Key", new Rect(256,475,67,44), "BUTTON",  _interfaceDevice, "MU IFF Key")},
                {new CNIControl("Nav_Ctrl_Key", new Rect(343,475,68,44), "BUTTON",  _interfaceDevice, "MU NAV CTRL Key")},
                {new CNIControl("Msn_Key", new Rect(431,475,67,44), "BUTTON",  _interfaceDevice, "MU MSN Key")},
                {new CNIControl("Caps_Key", new Rect(432,543,67,44), "BUTTON",  _interfaceDevice, "MU CAPS Key")},
                {new CNIControl("Mc_Indx_Key", new Rect(344,543,67,44), "BUTTON",  _interfaceDevice, "MU MC INDX Key")},
                {new CNIControl("Indx_Key", new Rect(256,543,67,44), "BUTTON",  _interfaceDevice, "MU INDX Key")},
                {new CNIControl("Told_Key", new Rect(168,543,68,44), "BUTTON",  _interfaceDevice, "MU TOLD Key")},
                {new CNIControl("Dir_Intc_Key", new Rect(80,543,68,44), "BUTTON",  _interfaceDevice, "MU DIR INTC Key")},
                {new CNIControl("Legs_Key", new Rect(80,611,68,44), "BUTTON",  _interfaceDevice, "MU LEGS Key")},
                {new CNIControl("Mark_Key", new Rect(168,611,68,44), "BUTTON",  _interfaceDevice, "MU MARK Key")},
                {new CNIControl("Next_Page_Key", new Rect(168,679,68,44), "BUTTON",  _interfaceDevice, "MU NEXT PAGE Key")},
                {new CNIControl("Prev_Page_Key", new Rect(80,679,68,44), "BUTTON",  _interfaceDevice, "MU PREV PAGE Key")},
                {new CNIControl("Exec_Key", new Rect(518,518,41,64), "BUTTON",  _interfaceDevice, "MU EXEC Key")},
                {new CNIControl("One_Key", new Rect(78,739,46,46), "BUTTON",  _interfaceDevice, "MU 1 Key")},
                {new CNIControl("Two_Key", new Rect(145,739,46,46), "BUTTON",  _interfaceDevice, "MU 2 Key")},
                {new CNIControl("Three_Key", new Rect(212,739,46,46), "BUTTON",  _interfaceDevice, "MU 3 Key")},
                {new CNIControl("Four_Key", new Rect(78,803,46,46), "BUTTON",  _interfaceDevice, "MU 4 Key")},
                {new CNIControl("Five_Key", new Rect(145,803,46,46), "BUTTON",  _interfaceDevice, "MU 5 Key")},
                {new CNIControl("Six_Key", new Rect(212,803,46,46), "BUTTON",  _interfaceDevice, "MU 6 Key")},
                {new CNIControl("Seven_Key", new Rect(78,867,46,46), "BUTTON",  _interfaceDevice, "MU 7 Key")},
                {new CNIControl("Eight_Key", new Rect(145,867,46,46), "BUTTON",  _interfaceDevice, "MU 8 Key")},
                {new CNIControl("Nine_Key", new Rect(212,867,46,46), "BUTTON",  _interfaceDevice, "MU 9 Key")},
                {new CNIControl("Decimal_Key", new Rect(78,931,46,46), "BUTTON",  _interfaceDevice, "MU Decimal Key")},
                {new CNIControl("Zero_Key", new Rect(144,931,47,46), "BUTTON",  _interfaceDevice, "MU 0 Key")},
                {new CNIControl("Minus_Key", new Rect(211,931,46,46), "BUTTON",  _interfaceDevice, "MU Minus Key")},
                {new CNIControl("A_Key", new Rect(290,607,47,47), "BUTTON",  _interfaceDevice, "MU A Key")},
                {new CNIControl("B_Key", new Rect(358,607,47,47), "BUTTON",  _interfaceDevice, "MU B Key")},
                {new CNIControl("C_Key", new Rect(426,607,47,47), "BUTTON",  _interfaceDevice, "MU C Key")},
                {new CNIControl("D_Key", new Rect(494,607,47,47), "BUTTON",  _interfaceDevice, "MU D Key")},
                {new CNIControl("E_Key", new Rect(562,607,47,47), "BUTTON",  _interfaceDevice, "MU E Key")},
                {new CNIControl("F_Key", new Rect(290,672,47,47), "BUTTON",  _interfaceDevice, "MU F Key")},
                {new CNIControl("G_Key", new Rect(358,672,47,47), "BUTTON",  _interfaceDevice, "MU G Key")},
                {new CNIControl("H_Key", new Rect(426,672,47,47), "BUTTON",  _interfaceDevice, "MU H Key")},
                {new CNIControl("I_Key", new Rect(494,672,47,47), "BUTTON",  _interfaceDevice, "MU I Key")},
                {new CNIControl("J_Key", new Rect(562,672,47,47), "BUTTON",  _interfaceDevice, "MU J Key")},
                {new CNIControl("K_Key", new Rect(290,737,47,47), "BUTTON",  _interfaceDevice, "MU K Key")},
                {new CNIControl("L_Key", new Rect(358,737,47,47), "BUTTON",  _interfaceDevice, "MU L Key")},
                {new CNIControl("M_Key", new Rect(426,737,47,47), "BUTTON",  _interfaceDevice, "MU M Key")},
                {new CNIControl("N_Key", new Rect(494,737,47,47), "BUTTON",  _interfaceDevice, "MU N Key")},
                {new CNIControl("O_Key", new Rect(562,737,47,47), "BUTTON",  _interfaceDevice, "MU O Key")},
                {new CNIControl("P_Key", new Rect(290,802,47,47), "BUTTON",  _interfaceDevice, "MU P Key")},
                {new CNIControl("Q_Key", new Rect(358,802,47,47), "BUTTON",  _interfaceDevice, "MU Q Key")},
                {new CNIControl("R_Key", new Rect(426,802,47,47), "BUTTON",  _interfaceDevice, "MU R Key")},
                {new CNIControl("S_Key", new Rect(494,802,47,47), "BUTTON",  _interfaceDevice, "MU S Key")},
                {new CNIControl("T_Key", new Rect(562,802,47,47), "BUTTON",  _interfaceDevice, "MU T Key")},
                {new CNIControl("U_Key", new Rect(290,867,47,47), "BUTTON",  _interfaceDevice, "MU U Key")},
                {new CNIControl("V_Key", new Rect(358,867,47,47), "BUTTON",  _interfaceDevice, "MU V Key")},
                {new CNIControl("W_Key", new Rect(426,867,47,47), "BUTTON",  _interfaceDevice, "MU W Key")},
                {new CNIControl("X_Key", new Rect(494,867,47,47), "BUTTON",  _interfaceDevice, "MU X Key")},
                {new CNIControl("Y_Key", new Rect(562,867,47,47), "BUTTON",  _interfaceDevice, "MU Y Key")},
                {new CNIControl("Z_Key", new Rect(290,932,47,47), "BUTTON",  _interfaceDevice, "MU Z Key")},
                {new CNIControl("Empty_Key", new Rect(358,932,47,47), "BUTTON",  _interfaceDevice, "MU Unused Key")},
                {new CNIControl("Del_Key", new Rect(426,932,47,47), "BUTTON",  _interfaceDevice, "MU DEL Key")},
                {new CNIControl("Slash_Key", new Rect(494,932,47,47), "BUTTON",  _interfaceDevice, "MU Slash Key")},
                {new CNIControl("Clr_Key", new Rect(562,932,47,47), "BUTTON",  _interfaceDevice, "MU CLR Key")},
                {new CNIControl("Msg", new Rect(636,663,43,98), "INDICATOR",  _interfaceDevice, "MSG Indicator")},
                {new CNIControl("Ofst", new Rect(636,774,43,98), "INDICATOR",  _interfaceDevice, "OFST Indicator")},
                {new CNIControl("Dspy", new Rect(12,663,43,98), "INDICATOR",  _interfaceDevice, "DSPY Indicator")},
                {new CNIControl("Fail", new Rect(11,773,43,98), "INDICATOR",  _interfaceDevice, "FAIL Indicator")},
                {new CNIControl("Power_Indicator_On", new Rect(512,478,50,30), "INDICATOR", _interfaceDevice, "Exec Indicator")},
                {new CNIControl("Brt_Key", new Rect(576,476,44,110), "ROCKER",  _interfaceDevice, "MU Brightness")},
             };
        }
    }
    internal class CNIControl
    {
        private readonly string _image;
        private readonly Rect _rect;
        private readonly string _controlType;
        private readonly string _device;
        private readonly string _element;

        internal CNIControl(string image, Rect rect, string controlType, string device, string element)
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
