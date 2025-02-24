//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using System.Xml;
    using System.Globalization;
    using System.ComponentModel;

    [HeliosControl("Helios.FA18C.IFEI", "IFEI", "F/A-18C", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class IFEI_FA18C : FA18CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "IFEI";

        private String _font = "Helios Virtual Cockpit F/A-18C Hornet IFEI"; // "Segment7 Standard"; //"Seven Segment";
        private Color _textColor = Color.FromArgb(0xff, 220, 220, 220);
        private Color _backGroundColor = Color.FromArgb(100, 100, 20, 50);
        private string _imageLocation = "{FA-18C}/Gauges/IFEI/";
        private bool _useBackGround = false;
        private IFEI_Gauges _IFEI_gauges;

        private HeliosValue _alternateImages;
        private string _altImageLocation = "";

        public IFEI_FA18C()
            : base("IFEI_Gauge", new Size(779, 702))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };

            _alternateImages = new HeliosValue(this, new BindingValue(false), "", "Enable Alternate Image Set", "Indicates whether the alternate image set is to be used", "True or False", BindingValueUnits.Boolean);
            _alternateImages.Execute += new HeliosActionHandler(EnableAltImages_Execute);
            Actions.Add(_alternateImages);
            AddDefaultInputBinding(
                    childName: "",
                    deviceActionName: "Cockpit Lights.MODE Switch.changed",
                    interfaceTriggerName: "set.Enable Alternate Image Set",
                    deviceTriggerName: "",
                    triggerBindingValue: new BindingValue("return TriggerValue<3"),
                    triggerBindingSource: BindingValueSources.LuaScript
                    );
 
            //DefaultInputBindings.Add(new DefaultInputBinding(
            //childName: "",
            //interfaceTriggerName: "Cockpit Lights.MODE Switch.changed",
            //deviceActionName: "set.Enable Alternate Image Set",
            //deviceTriggerName: "",
            //deviceTriggerBindingValue: new BindingValue("return TriggerValue<3"),
            //bindingValueSource: BindingValueSources.LuaScript
            //));

            // adding the text displays
            double dispHeight = 50;
            double fontSize = 42;

            double clockDispWidth = 50;
            double clockSpreadWidth = 3;
            double clockX = 524;
            double clockY = 355;
            // test string 00000000*4000=1:4001=1:4002=1:4003=1:4004=1:4005=1:4006=1:4007=1:4008=1:4009=1:4010=1:4011=1:4012=1:4013=1:4014=1:4015=1:4016=1:4017=1:4018=1:2063=81000I:2064=81000T:2066=88:2065=99:2073=9:2072=59:2071=59:2053=23:2054=59:2055=59:2061=2000:2062=4000:2067=100:2068=100:2052=10000:2069=1200:2070=1200:2056=1:2057=1:2058=1:2060=1:4019=50:4020=50:
            AddTextDisplay("Clock HH", clockX, clockY, new Size(clockDispWidth, dispHeight), fontSize, "23", _interfaceDeviceName, "Clock hours");
            AddTextDisplay("Clock MM", clockX + clockDispWidth + clockSpreadWidth, clockY, new Size(clockDispWidth, dispHeight), fontSize, "59", _interfaceDeviceName, "Clock minutes");
            AddTextDisplay("Clock SS", clockX + 2* (clockDispWidth + clockSpreadWidth), clockY, new Size(clockDispWidth, dispHeight), fontSize, "59", _interfaceDeviceName, "Clock seconds");
            clockY = 412;
            AddTextDisplay("Elapsed Time H", clockX + clockDispWidth/2, clockY, new Size(clockDispWidth/2, dispHeight), fontSize, "01", _interfaceDeviceName, "Timer hours");
            AddTextDisplay("Elapsed Time MM", clockX + clockDispWidth + clockSpreadWidth, clockY, new Size(clockDispWidth, dispHeight), fontSize, "59", _interfaceDeviceName, "Timer minutes");
            AddTextDisplay("Elapsed Time SS", clockX + 2*(clockDispWidth + clockSpreadWidth), clockY, new Size(clockDispWidth, dispHeight), fontSize, "59", _interfaceDeviceName, "Timer seconds");

            // Fuel info

            AddTextDisplay("Bingo", 545, 258, new Size(133, dispHeight), fontSize, "2000", _interfaceDeviceName, "Bingo Value");

            double fuelX = 530;
            double fuelWidth = 154;
            AddTextDisplay("Fuel Total", fuelX, 93, new Size(fuelWidth, dispHeight), fontSize, "10780T", _interfaceDeviceName, "Total Fuel Amount");
            AddTextDisplay("Fuel Internal", fuelX, 159, new Size(fuelWidth, dispHeight), fontSize, "10780I", _interfaceDeviceName, "Internal Fuel Amount");
            AddTextDisplay("T Value", fuelX, 93, new Size(fuelWidth, dispHeight), fontSize, "T", _interfaceDeviceName, "T Value");
            AddTextDisplay("Time Set Mode", fuelX, 159, new Size(fuelWidth, dispHeight), fontSize, "H", _interfaceDeviceName, "Time Set Mode");

            double RPMWidth = 60;
            AddTextDisplay("RPM Left", 104, 86, new Size(RPMWidth, dispHeight), fontSize, "65", _interfaceDeviceName, "Left RPM Value");
            AddTextDisplay("RPM Right", 255, 86, new Size(RPMWidth, dispHeight), fontSize, "65", _interfaceDeviceName, "Right RPM Value");
            
            double TempWidth = 92;
            AddTextDisplay("Temp Left", 80, 143, new Size(TempWidth, dispHeight), fontSize, "330", _interfaceDeviceName, "Left Temperature Value");
            AddTextDisplay("Temp Right", 261, 143, new Size(TempWidth, dispHeight), fontSize, "330", _interfaceDeviceName, "Right Temperature Value");
            AddTextDisplay("SP", 80, 143, new Size(TempWidth, dispHeight), fontSize, "SP", _interfaceDeviceName, "SP");
            AddTextDisplay("SP Code", 261, 143, new Size(TempWidth, dispHeight), fontSize, "999", _interfaceDeviceName, "SP Code");

            AddTextDisplay("FF Left", 80, 199, new Size(TempWidth, dispHeight), fontSize, "6", _interfaceDeviceName, "Left Fuel Flow Value");
            AddTextDisplay("FF Right", 261, 199, new Size(TempWidth, dispHeight), fontSize, "6", _interfaceDeviceName, "Right Fuel Flow Value");

            double oilWidth = 64;
            AddTextDisplay("Oil Left", 107, 433, new Size(oilWidth, dispHeight), fontSize, "60", _interfaceDeviceName, "Left Oil Pressure");
            AddTextDisplay("Oil Right", 262, 433, new Size(oilWidth, dispHeight), fontSize, "60", _interfaceDeviceName, "Right Oil Pressure");

            AddIFEIParts("Gauges", 0, 0, new Size(779, 702), _interfaceDeviceName, "IFEI Needles & Flags");

            double spacing = 70;
            double start = 64;
            double left = 400;
            // adding the control buttons
            AddButton("MODE", left, start, new Size(87, 62), _interfaceDeviceName, "IFEI Mode Button");
            AddButton("QTY", left, start + spacing, new Size(87, 62), _interfaceDeviceName, "IFEI QTY Button");
            AddButton("UP", left, start + 2 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI Up Arrow Button");
            AddButton("DOWN", left, start + 3 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI Down Arrow Button");
            AddButton("ZONE", left, start + 4 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI ZONE Button");
            AddButton("ET", left, start + 5 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI ET Button");

            AddPot(
                name: "Brightness Control",
                posn: new Point(82, 630),
                size: new Size(60, 60),
                knobImage: "{FA-18C}/Images/Common Knob.png",
                initialRotation: 219,
                rotationTravel: 291,
                minValue: 0,
                maxValue: 1,
                initialValue: 0,
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: "IFEI Brightness Control Knob",
                fromCenter: true
                );
            Size ThreeWayToggleSize = new Size(70, 140);
            Add3PosnToggle(
                name: "Video Record DDI",
                posn: new Point(236, 570),
                size: ThreeWayToggleSize,
                image: "{FA-18C}/Gauges/IFEI/orange-round-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Video Record Selector Switch HMD/LDDI/RDDI",
                fromCenter: false
                );

            Add3PosnToggle(
                name: "Video Record HUD",
                posn: new Point(395, 570),
                size: ThreeWayToggleSize,
                image: "{FA-18C}/Gauges/IFEI/orange-round-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Video Record Selector Switch, HUD/LDIR/RDDI",
                fromCenter: false
                );

            Add3PosnToggle(
                name: "Video Record Control",
                posn: new Point(584, 570),
                size: ThreeWayToggleSize,
                image: "{FA-18C}/Gauges/IFEI/orange-round-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "Video Record Mode Selector Switch, MAN/OFF/AUTO",
                fromCenter: false
                );
       }

        #region Properties
        public double GlassReflectionOpacity
        {
            get
            {
                return _IFEI_gauges.GlassReflectionOpacity;
            }
            set
            {
                double oldValue = _IFEI_gauges.GlassReflectionOpacity;
                _IFEI_gauges.GlassReflectionOpacity = value;
                if (value != oldValue)
                {
                    OnPropertyChanged("GlassReflectionOpacity", oldValue, value, true);
                }
            }
        }
        public bool EnableAlternateImageSet
        {
            get
            {
                return _altImageLocation != "";
            }
            set
            {
                bool newValue = value;
                bool oldValue = _altImageLocation != "";

                if (newValue != oldValue)
                {
                    _altImageLocation = newValue ? "/Alt" : "";
                    _imageLocation = $"{{FA-18C}}/Gauges/IFEI{_altImageLocation}/";
                    _IFEI_gauges.EnableAlternateImageSet = newValue;
                    foreach(HeliosVisual hv in this.Children)
                    {
                        if (hv is TextDisplay txtDisplay)
                        {
                            txtDisplay.OnTextColor = newValue ? Color.FromArgb(0xff, 0, 220, 0) : Color.FromArgb(0xff, 220, 220, 220);
                            continue;
                        }
                        //if (hv is ImageTranslucent img)
                        //{
                        //    img.Image = $"{_imageLocation}{System.IO.Path.GetFileName(img.Image)}";
                        //    continue;
                        //}
                        if (hv is PushButton pb)
                        {
                            pb.Image = $"{_imageLocation}{System.IO.Path.GetFileName(pb.Image)}";
                            pb.PushedImage = $"{_imageLocation}{System.IO.Path.GetFileName(pb.PushedImage)}";
                            continue;
                        }
                        if (hv is ThreeWayToggleSwitch sw)
                        {
                            sw.PositionOneImage = $"{_imageLocation}{System.IO.Path.GetFileName(sw.PositionOneImage)}";
                            sw.PositionTwoImage = $"{_imageLocation}{System.IO.Path.GetFileName(sw.PositionTwoImage)}";
                            sw.PositionThreeImage = $"{_imageLocation}{System.IO.Path.GetFileName(sw.PositionThreeImage)}";
                            continue;
                        }
                        if (hv is Potentiometer pot)
                        {
                            pot.KnobImage = $"{_imageLocation}{System.IO.Path.GetFileName(pot.KnobImage)}";
                            continue;
                        }
                    }
                    BackgroundImage = _imageLocation + "IFEI.png";
                    // notify change after change is made
                    OnPropertyChanged("EnableAlternateImageSet", oldValue, newValue, true);
                }
                
            }
        }
        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile) {
            base.OnProfileChanged(oldProfile);
        }

        public override string DefaultBackgroundImage
        {
            get { return _imageLocation + "IFEI.png"; }
        }

        void EnableAltImages_Execute(object sender, HeliosActionEventArgs e)
        {
            EnableAlternateImageSet = e.Value.BoolValue;
            _alternateImages.SetValue(e.Value, e.BypassCascadingTriggers);
        }

        private void AddTextDisplay(string name, double x, double y, Size size, double baseFontsize, string testDisp,
            string interfaceDevice, string interfaceElement)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: new Point(x, y),
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: TextHorizontalAlignment.Right,
                verticalAligment: TextVerticalAlignment.Top,
                testTextDisplay: testDisp,
                textColor: _textColor,
                backgroundColor: _backGroundColor,
                useBackground: _useBackGround,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                textDisplayDictionary: ""
                );
            display.TextFormat.FontWeight = FontWeights.Heavy;
        }

        private void AddButton(string name, double x, double y, Size size, string interfaceDevice, string interfaceElement)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: _imageLocation + "IFEI_" + name + ".png",
                pushedImage: _imageLocation + "IFEI_" + name + "_DN.png",
                buttonText: "",
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                fromCenter: false
                );
        }
        private void Add3PosnToggle(string name, Point posn, Size size, string image, string interfaceDevice, string interfaceElement, bool fromCenter)
        {
            AddThreeWayToggle(
                name: name,
                posn: posn,
                size: size,
                positionOneImage: image + "up.png",
                positionTwoImage: image + "norm.png",
                positionThreeImage: image + "down.png",
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.OnOnOn,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                fromCenter: false
                );
        }
        private void AddImage(string name, string imageName, Rect rect)
        {
            ImageTranslucent image = new ImageTranslucent()
            {
                Name = name,
                Left = rect.Left,
                Top = rect.Top,
                Width = rect.Width,
                Height = rect.Height,
                Alignment = ImageAlignment.Stretched,
                Image = imageName,
                AllowInteraction = true,
                Value = 1d,
                IsHidden = false
            };
            Children.Add(image);
        }
        private void AddIFEIParts(string name, double x, double y, Size size, string interfaceDevice, string interfaceElement)
        {
            _IFEI_gauges = new IFEI_Gauges
            {
                Top = y,
                Left = x,
                Height = size.Height,
                Width = size.Width,
                Name = name
            };

            Children.Add(_IFEI_gauges);
            foreach (IBindingTrigger trigger in _IFEI_gauges.Triggers)
            {
                AddTrigger(trigger, trigger.Name);
            }
            foreach (IBindingAction action in _IFEI_gauges.Actions)
            {
                AddAction(action, action.Name);
                // Create the automatic input bindings for the IFEI_Gauge sub component
                AddDefaultInputBinding(
                    childName: "Gauges",
                    deviceActionName: action.ActionVerb +"." +action.Name,
                    interfaceTriggerName: interfaceDevice +"."+ action.Name + ".changed"
                    );
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
            base.WriteXml(writer);
            if (_IFEI_gauges.GlassReflectionOpacity != IFEI_Gauges.GLASS_REFLECTION_OPACITY_DEFAULT)
            {
                writer.WriteElementString("GlassReflectionOpacity", GlassReflectionOpacity.ToString(CultureInfo.InvariantCulture));
            }
            if (EnableAlternateImageSet) writer.WriteElementString("EnableAlternateImageSet", EnableAlternateImageSet.ToString(CultureInfo.InvariantCulture));

        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            base.ReadXml(reader);
            if (reader.Name.Equals("GlassReflectionOpacity"))
            {
                GlassReflectionOpacity = double.Parse(reader.ReadElementString("GlassReflectionOpacity"), CultureInfo.InvariantCulture);
            }
            if (reader.Name.Equals("EnableAlternateImageSet"))
            {
                bool enableAlternateImageSet = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("EnableAlternateImageSet"));
                _textColor = enableAlternateImageSet ? Color.FromArgb(0xff, 0, 220, 0) : Color.FromArgb(0xff, 220, 220, 220);
                EnableAlternateImageSet = enableAlternateImageSet;
            }
            else
            {
                _textColor = Color.FromArgb(0xff, 220, 220, 220);
                EnableAlternateImageSet = false;
            }
        }
    }
}
