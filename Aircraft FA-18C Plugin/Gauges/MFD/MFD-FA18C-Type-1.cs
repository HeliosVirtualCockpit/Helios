//  Copyright 2014 Craig Courtney
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

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;
using System.Windows.Media;


// ReSharper disable once CheckNamespace
namespace GadrocsWorkshop.Helios.Gauges.FA_18C.MFD
{
    public class MPCD_FA18C : Gauges.MFD
    {
        private static readonly Rect SCREEN_RECT = new Rect(72, 137, 497, 493);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private HeliosValue _alternateImages;
        private string _altImageLocation = "Alt";
        private bool _enableAlternateImageSet = false;
        private string _defaultBackgroundImage = "{FA-18C}/Images/MPCD frame.png";
        private bool _includeViewport = true;
        private string _vpName = "";
        private string _interfaceDeviceName = "";

        public MPCD_FA18C(string name = "MPCD")
            : base(name, new Size(656, 706))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };

            _interfaceDeviceName = $"{name.Split(' ')[0]} MDI";
            switch (name)
            {
                case "Left MPCD":
                    _vpName = "FA_18C_LEFT_MFCD";
                    break;
                case "Right MPCD":
                    _vpName = "FA_18C_RIGHT_MFCD";
                    break;
                default:
                    _includeViewport = false;
                    break;
            }
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);

            _alternateImages = new HeliosValue(this, new BindingValue(false), "", "Enable Alternate Image Set", "Indicates whether the alternate image set is to be used", "True or False", BindingValueUnits.Boolean);
            _alternateImages.Execute += new HeliosActionHandler(EnableAltImages_Execute);
            Actions.Add(_alternateImages);
            AddDefaultInputBinding(
                    childName: "",
                    deviceActionName: "set.Enable Alternate Image Set",
                    interfaceTriggerName: "Cockpit Lights.MODE Switch.changed",
                    deviceTriggerName: "",
                    triggerBindingValue: new BindingValue("return TriggerValue<3"),
                    triggerBindingSource: BindingValueSources.LuaScript
                    );

            AddButton("OSB1", 14, 540, true);
            AddButton("OSB2", 14, 455, true);
            AddButton("OSB3", 14, 370, true);
            AddButton("OSB4", 14, 285, true);
            AddButton("OSB5", 14, 200, true);

            AddButton("OSB6", 150, 74, false);
            AddButton("OSB7", 230, 74, false);
            AddButton("OSB8", 310, 74, false);
            AddButton("OSB9", 390, 74, false);
            AddButton("OSB10", 470, 74, false);

            AddButton("OSB11", 604, 200, true);
            AddButton("OSB12", 604, 285, true);
            AddButton("OSB13", 604, 370, true);
            AddButton("OSB14", 604, 455, true);
            AddButton("OSB15", 604, 540, true);

            AddButton("OSB16", 470, 652, false);
            AddButton("OSB17", 390, 652, false);
            AddButton("OSB18", 310, 652, false);
            AddButton("OSB19", 230, 652, false);
            AddButton("OSB20", 150, 652, false);

            AddRotarySwitch("Mode Knob", new Point(298, 14), new Size(50, 50));
            AddKnob("Brightness Knob", new Point(14, 632), new Size(50, 50), 1);
            AddKnob("Contrast Knob", new Point(592, 632), new Size(50, 50), 1);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get => _defaultBackgroundImage;
        }

        public bool EnableAlternateImageSet
        {
            get => _enableAlternateImageSet;
            set
            {
                bool newValue = value;
                bool oldValue = _enableAlternateImageSet;

                if (newValue != oldValue)
                {
                    _enableAlternateImageSet = newValue;

                    foreach (HeliosVisual hv in this.Children)
                    {
                        if (hv is PushButton pb)
                        {
                            pb.Image = ImageSwitchName(pb.Image);
                            pb.PushedImage = ImageSwitchName(pb.PushedImage);
                            continue;
                        }
                        if (hv is ThreeWayToggleSwitch sw)
                        {
                            sw.PositionOneImage = ImageSwitchName(sw.PositionOneImage);
                            sw.PositionTwoImage = ImageSwitchName(sw.PositionTwoImage);
                            sw.PositionThreeImage = ImageSwitchName(sw.PositionThreeImage);
                            continue;
                        }
                        if (hv is Indicator ind)
                        {
                            ind.OnImage = ImageSwitchName(ind.OnImage);
                            ind.OffImage = ImageSwitchName(ind.OffImage);
                            continue;
                        }
                        if (hv is Potentiometer pot)
                        {
                            pot.KnobImage = ImageSwitchName(pot.KnobImage);
                            continue;
                        }
                        if (hv is RotaryEncoder enc)
                        {
                            enc.KnobImage = ImageSwitchName(enc.KnobImage);
                            continue;
                        }
                        if (hv is RotarySwitch rsw)
                        {
                            rsw.KnobImage = ImageSwitchName(rsw.KnobImage);
                            continue;
                        }
                    }

                    BackgroundImage = ImageSwitchName(BackgroundImage);
                    // notify change after change is made
                    OnPropertyChanged("EnableAlternateImageSet", oldValue, newValue, true);
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
            get => _vpName != "" ? true : false;
            set => _ = value;
        }

        #endregion
        private string ImageSwitchName(string imageName)
        {
            string imageSubfolder = _enableAlternateImageSet ? $"/{_altImageLocation}" : "";

            string dir = Path.GetDirectoryName(imageName);
            if (new DirectoryInfo(dir).Name == _altImageLocation)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return $"{dir}{imageSubfolder}/{Path.GetFileName(imageName)}";
        }

        void EnableAltImages_Execute(object sender, HeliosActionEventArgs e)
        {
            EnableAlternateImageSet = e.Value.BoolValue;
            _alternateImages.SetValue(e.Value, e.BypassCascadingTriggers);
        }

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                double scaleX = Width / NativeSize.Width;
                double scaleY = Height / NativeSize.Height;
                _scaledScreenRect.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }
        private void AddRotarySwitch(string name, Point posn, Size size)
        {
            RotarySwitch _knob = new RotarySwitch();
            _knob.Name = name;
            _knob.KnobImage = "{FA-18C}/Images/Common Knob.png";
            _knob.DrawLabels = false;
            _knob.DrawLines = false;
            _knob.Positions.Clear();
            _knob.Positions.Add(new RotarySwitchPosition(_knob, 0, "Off", 270d));
            _knob.Positions.Add(new RotarySwitchPosition(_knob, 1, "Night", 330d));
            _knob.Positions.Add(new RotarySwitchPosition(_knob, 2, "Day", 30d));
            _knob.CurrentPosition = 1;
            _knob.DefaultPosition = 1;
            _knob.Top = posn.Y;
            _knob.Left = posn.X;
            _knob.Width = size.Width;
            _knob.Height = size.Height;

            Children.Add(_knob);
            AddTrigger(_knob.Triggers["position.changed"], name);
            AddAction(_knob.Actions["set.position"], name);

            if (_interfaceDeviceName == "Left MDI" || _interfaceDeviceName == "Right MDI")
            {
                string interfaceElementName = $"{_interfaceDeviceName} Brightness Selector Knob";

                // add the default bindings
                AddDefaultOutputBinding(
                    childName: name,
                    deviceTriggerName: "position.changed",
                    interfaceActionName: $"{_interfaceDeviceName}.set.{interfaceElementName}"
                    );
                AddDefaultInputBinding(
                    childName: name,
                    interfaceTriggerName: $"{_interfaceDeviceName}.{interfaceElementName}.changed",
                    deviceActionName: "set.position");
            }
        }

        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(76, 132, 494, 494);
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
                BackgroundColor = Color.FromArgb(128, 128, 0, 64),
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
        private void AddKnob(string name, Point posn, Size size, Int16 knobType)
        {
            Potentiometer _knob = new Potentiometer();
            _knob.Name = name;
            switch (knobType)
            {
                case 1:
                    _knob.KnobImage = "{FA-18C}/Images/MPCD Knob.png";
                    break;
                default:
                    _knob.KnobImage = "{FA-18C}/Images/Common Knob.png";
                    break;
            }
            _knob.InitialRotation = 219;
            _knob.RotationTravel = 291;
            _knob.MinValue = 0;
            _knob.MaxValue = 1;
            _knob.InitialValue = 0;
            _knob.StepValue = 0.1;
            _knob.Top = posn.Y;
            _knob.Left = posn.X;
            _knob.Width = size.Width;
            _knob.Height = size.Height;

            Children.Add(_knob);
            foreach (IBindingTrigger trigger in _knob.Triggers)
            {
                AddTrigger(trigger, name);
            }
            foreach (IBindingAction action in _knob.Actions)
            {
                AddAction(action, name);
            }

            if (_interfaceDeviceName == "Left MDI" || _interfaceDeviceName == "Right MDI")
            {
                string interfaceElementName = $"{_interfaceDeviceName} {name.Split(' ')[0]} Control Knob";

                // add the default bindings
                AddDefaultOutputBinding(
                    childName: name,
                    deviceTriggerName: "value.changed",
                    interfaceActionName: $"{_interfaceDeviceName}.set.{interfaceElementName}"
                    );
                AddDefaultInputBinding(
                    childName: name,
                    interfaceTriggerName: $"{_interfaceDeviceName}.{interfaceElementName}.changed",
                    deviceActionName: "set.value");
            }
        }

        private void AddButton(string name, double x, double y, bool horizontal)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = y;
            button.Left = x;
            button.Width = 40;
            button.Height = 40;

            if (!horizontal)
            {
                button.Image = "{FA-18C}/Images/MFD Button UpV.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button DnV.png";
            }
            else
            {
                button.Image = "{FA-18C}/Images/MFD Button UpH.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button DnH.png";
            }

            button.Name = name;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], name);
            AddTrigger(button.Triggers["released"], name);

            AddAction(button.Actions["push"], name);
            AddAction(button.Actions["release"], name);
            AddAction(button.Actions["set.physical state"], name);

            if (_interfaceDeviceName == "Left MDI" || _interfaceDeviceName == "Right MDI")
            {
                string interfaceElementName = $"{name.Substring(0, 3)} {double.Parse(name.Substring(3)):00}";

                // add the default bindings
                AddDefaultOutputBinding(
                    childName: name,
                    deviceTriggerName: "pushed",
                    interfaceActionName: $"{_interfaceDeviceName}.push.{interfaceElementName}"
                    );
                AddDefaultOutputBinding(
                    childName: name,
                    deviceTriggerName: "released",
                    interfaceActionName: $"{_interfaceDeviceName}.release.{interfaceElementName}"
                    );
                AddDefaultInputBinding(
                    childName: name,
                    interfaceTriggerName: $"{_interfaceDeviceName}.{interfaceElementName}.changed",
                    deviceActionName: "set.physical state");
            }
        }

        private new void AddTrigger(IBindingTrigger trigger, string device)
        {
            trigger.Device = device;
            Triggers.Add(trigger);
        }

        private new void AddAction(IBindingAction action, string device)
        {
            action.Device = device;
            Actions.Add(action);
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
            if (EnableAlternateImageSet) writer.WriteElementString("EnableAlternateImageSet", EnableAlternateImageSet.ToString(CultureInfo.InvariantCulture));
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
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (reader.Name.Equals("EnableAlternateImageSet"))
            {
                bool enableAlternateImageSet = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("EnableAlternateImageSet"));
                EnableAlternateImageSet = enableAlternateImageSet;
            }
            else
            {
                EnableAlternateImageSet = false;
            }
        }
    }
}
