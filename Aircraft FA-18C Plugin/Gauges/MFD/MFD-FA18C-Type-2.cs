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

// ReSharper disable once CheckNamespace

using GadrocsWorkshop.Helios.Controls;

namespace GadrocsWorkshop.Helios.Gauges.FA_18C.MFD
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Xml;
    using System.Windows.Media;


    [HeliosControl("FA18C.AMPCD", "AMPCD", "F/A-18C", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class AMPCD_FA18C : Gauges.MFD
    {
        private static readonly Rect SCREEN_RECT = new Rect(88, 161, 551, 532);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private HeliosValue _alternateImages;
        private string _altImageLocation = "Alt";
        private bool _enableAlternateImageSet = false;
        private string _defaultBackgroundImage = "{FA-18C}/Images/AMPCD frame.png";
        private bool _includeViewport = true;
        private string _vpName = "";
        private string _interfaceDeviceName = "AMPCD";

        public AMPCD_FA18C()
            : base("AMPCD", new Size(727, 746))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };

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


            _vpName = "FA_18C_CENTER_MFCD";
            if (_vpName != "" && _includeViewport) AddViewport(_vpName);

            AddButton("OSB1", 39, 567, true);
            AddButton("OSB2", 39, 487, true);
            AddButton("OSB3", 39, 409, true);
            AddButton("OSB4", 39, 329, true);
            AddButton("OSB5", 39, 247, true);

            AddButton("OSB6", 186, 112, false);
            AddButton("OSB7", 266, 112, false);
            AddButton("OSB8", 346, 112, false);
            AddButton("OSB9", 424, 112, false);
            AddButton("OSB10", 506, 112, false);

            AddButton("OSB11", 645, 247, true);
            AddButton("OSB12", 645, 329, true);
            AddButton("OSB13", 645, 409, true);
            AddButton("OSB14", 645, 487, true);
            AddButton("OSB15", 645, 567, true);

            AddButton("OSB16", 506, 696, false);
            AddButton("OSB17", 424, 696, false);
            AddButton("OSB18", 346, 696, false);
            AddButton("OSB19", 266, 696, false);
            AddButton("OSB20", 186, 696, false);
            AddRocker("Day / Night", "MFD Rocker", "L", 90, 75);
            AddRocker("Symbols", "MFD Rocker", "R", 550, 75);
            AddRocker("Gain", "MFD Rocker", "V", 39, 650);
            AddRocker("Contrast", "MFD Rocker", "V", 645, 650);

            AddThreeWayToggle("Heading", 1, 51, new Size(50, 100));
            AddThreeWayToggle("Course", 622, 50, new Size(50, 100));

            AddKnob("Mode Knob",new Point(336,37),new Size(60,60));
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
        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(109, 168, 509, 509);
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

        private void AddButton(string name, double x, double y, bool horizontal)
        {
            PushButton button = new PushButton();
            button.Top = y;
            button.Left = x;
            button.Width = 42;
            button.Height = 42;
            if (!horizontal)
            {
                button.Image = "{FA-18C}/Images/MFD Button 1 UpV.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button 1 DnV.png";
            }
            else
            {
                button.Image = "{FA-18C}/Images/MFD Button 1 UpH.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button 1 DnH.png";
            }
            button.Name = name;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], name);
            AddTrigger(button.Triggers["released"], name);

            AddAction(button.Actions["push"], name);
            AddAction(button.Actions["release"], name);
            AddAction(button.Actions["set.physical state"], name);

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
        private void AddKnob(string name, Point posn, Size size)
        {
            Potentiometer knob = new Potentiometer
            {
                Name = name,
                KnobImage = "{FA-18C}/Images/Common Knob.png",
                InitialRotation = 219,
                RotationTravel = 291,
                MinValue = 0,
                MaxValue = 1,
                InitialValue = 0,
                StepValue = 0.1,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height
            };

            Children.Add(knob);
            foreach (IBindingTrigger trigger in knob.Triggers)
            {
                AddTrigger(trigger, name);
            }
            AddAction(knob.Actions["set.value"], name);

            string interfaceElementName = $"{_interfaceDeviceName} Off/Brightness Control Knob";

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

        private void AddRocker(string name, string imagePrefix, string imageOrientation, double x, double y)
        {
            ThreeWayToggleSwitch rocker = new ThreeWayToggleSwitch();
            rocker.Name = name;
            rocker.SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            rocker.ClickType = LinearClickType.Touch;
            rocker.PositionTwoImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Mid.png";

            rocker.Top = y;
            rocker.Left = x;
            switch (imageOrientation)
            {
                case ("V"):
                    rocker.PositionOneImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    rocker.Height = 84;
                    rocker.Width = 40;
                    break;
                case ("L"):
                    rocker.PositionOneImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    rocker.Width = 86;
                    rocker.Height = 71;
                    break;
                case ("R"):
                    rocker.PositionOneImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    rocker.Width = 86;
                    rocker.Height = 71;
                    break;
                default:
                    break;
            }

            Children.Add(rocker);
            foreach (IBindingTrigger trigger in rocker.Triggers)
            {
                AddTrigger(trigger, name);
            }

            AddAction(rocker.Actions["set.position"], name);
            string interfaceElementName;
            switch (name)
            {
                case "Day / Night":
                    interfaceElementName = "AMPCD Night/Day Brightness Selector DAY";
                    break;
                case "Symbols":
                    interfaceElementName = "AMPCD Symbology Control Switch UP";
                    break;
                case "Gain":
                    interfaceElementName = "AMPCD Gain Control Switch UP";
                    break;
                case "Contrast":
                    interfaceElementName = "AMPCD Contrast Control Switch UP";
                    break;
                default:
                    interfaceElementName = "";
                    break;
            }

            AddDefaultOutputBinding(
                childName: name,
                deviceTriggerName: "position.changed",
                interfaceActionName: _interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                childName: name,
                interfaceTriggerName: _interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");
        }

        private void AddThreeWayToggle(string name, double x, double y, Size size)
        {
            Helios.Controls.ThreeWayToggleSwitch toggle = new Helios.Controls.ThreeWayToggleSwitch();
            toggle.Top = y;
            toggle.Left = x;
            toggle.Width = size.Width;
            toggle.Height = size.Height;
            toggle.DefaultPosition = ThreeWayToggleSwitchPosition.Two;
            toggle.Orientation = ToggleSwitchOrientation.Vertical; // this seems to just control the swipe direction
            toggle.Rotation = HeliosVisualRotation.CW;
            toggle.PositionOneImage = "{Helios}/Images/Toggles/orange-round-up.png";
            toggle.PositionTwoImage = "{Helios}/Images/Toggles/orange-round-norm.png";
            toggle.PositionThreeImage = "{Helios}/Images/Toggles/orange-round-down.png";
            toggle.SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            toggle.Name = name;
            Children.Add(toggle);
            foreach (IBindingTrigger trigger in toggle.Triggers)
            {
                AddTrigger(trigger, name);
            }
            foreach (IBindingAction action in toggle.Actions)
            {
                AddAction(action, name);
            }

            string interfaceElementName = $"{name} Set Switch";
            AddDefaultOutputBinding(
                childName: name,
                deviceTriggerName: "position.changed",
                interfaceActionName: _interfaceDeviceName + ".set." + interfaceElementName
            );
            AddDefaultInputBinding(
                childName: name,
                interfaceTriggerName: _interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");
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
