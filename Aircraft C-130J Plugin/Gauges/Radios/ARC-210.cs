//  Copyright 2020 Helios Contributors
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
namespace GadrocsWorkshop.Helios.Gauges.C130J.ARC210
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// This is an A-10C UHF Radio that uses text displays instead of an exported viewport.
    /// </summary>
    [HeliosControl("Helios.C130J.ARC210Radio", "ARC-210 Radio", "C-130J Hercules", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class ARC210Radio : CompositeVisualWithBackgroundImage
    {
        //private const double SCREENRES = 1.0;
        private readonly string _interfaceDeviceName = "ARC-210";
        private readonly string _imageLocation = "{C-130J}/Gauges/Radios/";
        private bool _useTextualDisplays = true;
        private ImageDecoration _displayBackground;
        private bool _includeViewport = true;
        private bool _requiresPatches = true;
        private string _vpName = "";
        private string _font = "Helios Virtual Cockpit A-10C_ARC-210_Large";
        private string _font2 = "Helios Virtual Cockpit A-10C_ARC-210_Small";
        private List<TextDisplay> _textDisplayList = new List<TextDisplay>();
        private HeliosValue _hv, _hvBrightness;
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private HeliosAction _incrementBrightnessAction;
        private HeliosAction _decrementBrightnessAction;
        private double _displayBrightness = 8;

        public ARC210Radio()
            : base("ARC-210 Radio", new Size(500, 401))
        {

            SupportedInterfaces = new[] { typeof(Interfaces.DCS.C130J.C130JInterface) };

            _hv = new HeliosValue(this, new BindingValue(false), "", "clear radio display", "When true, the display is cleared.", "True or False", BindingValueUnits.Boolean);
            _hv.Execute += new HeliosActionHandler(ClearDisplay_Execute);
            Actions.Add(_hv);
            Values.Add(_hv);

            _incrementBrightnessAction = new HeliosAction(this, "", "display brightness", "increment", "Increments the display brightness.");
            _incrementBrightnessAction.Execute += new HeliosActionHandler(IncrementBrightnessAction_Execute);
            Actions.Add(_incrementBrightnessAction);

            _decrementBrightnessAction = new HeliosAction(this, "", "display brightness", "decrement", "decrements the display brightness.");
            _decrementBrightnessAction.Execute += new HeliosActionHandler(DecrementBrightnessAction_Execute);
            Actions.Add(_decrementBrightnessAction);

            _displayBackground = AddImage($"{_imageLocation}ARC210_Display.png", new Point(116d, 75d), new Size(230d, 144d), $"{_imageLocation}ARC210_Display.png");
            UseTextualDisplays = false;

            _textDisplayList.Add(AddTextDisplay("Frequency Display", new Point(140, 172), new Size(197, 52), _interfaceDeviceName, "Frequency Display", _font, 29, "133.888", TextHorizontalAlignment.Right, ""));
            _textDisplayList.Add(AddTextDisplay("Modulation Mode", new Point(277, 151), new Size(55, 30), _interfaceDeviceName, "Modulation Mode", 15, "AM", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("XMIT/RECV Label", new Point(201, 151), new Size(55, 30), _interfaceDeviceName, "XMIT/RECV Label", 15, "XMIT", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Communications Security Mode", new Point(118, 119), new Size(221, 30), _interfaceDeviceName, "Communications Security Mode", 15, "KY-58 VOICE", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Communications Security Submode", new Point(118, 137), new Size(221, 35), _interfaceDeviceName, "Communications Security Submode", 15, "PT", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Display of Previous Manual Frequency", new Point(186, 75), new Size(137, 23), _interfaceDeviceName, "Display of Previous Manual Frequency", 15, "133.100", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("RT Label", new Point(283, 72), new Size(52, 30), _interfaceDeviceName, "RT Label", 15, "RT1", TextHorizontalAlignment.Right, ""));

            _textDisplayList.Add(AddTextDisplay("Upper Button Label", new Point(118, 75), new Size(213, 23), _interfaceDeviceName, "Upper FSK Label", 15, "LABEL 1", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Middle Button Label", new Point(118, 122), new Size(228, 46), _interfaceDeviceName, "Middle FSK Label", 15, "LABEL 2", TextHorizontalAlignment.Left, TextVerticalAlignment.Center, ""));
            _textDisplayList.Add(AddTextDisplay("Lower Button Label", new Point(118, 191), new Size(213, 23), _interfaceDeviceName, "Lower FSK Label", 15, "LABEL 3", TextHorizontalAlignment.Left, ""));

            _textDisplayList.Add(AddTextDisplay("Preset Display", new Point(283, 101), new Size(52, 52), _interfaceDeviceName, "Active Channel Number", _font, 29, "88", TextHorizontalAlignment.Right, ""));
            _textDisplayList.Add(AddTextDisplay("SatCom Type", new Point(265, 142), new Size(70, 30), _interfaceDeviceName, "Sat comm channel type", 15, "IDLE", TextHorizontalAlignment.Right, ""));
            _textDisplayList.Add(AddTextDisplay("SatCom Timeout", new Point(122, 101), new Size(107, 23), _interfaceDeviceName, "Sat comm activated time remaining", 15, "00:00:00", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("SatCom Status", new Point(171, 172), new Size(205, 52), _interfaceDeviceName, "Sat comm activated status", _font, 29, "ACTIVE", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Lower Left Number", new Point(118, 184), new Size(55, 23), _interfaceDeviceName, "Comm security sat comm delay", 15, "5", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Lower Right Status", new Point(195, 165), new Size(143, 46), _interfaceDeviceName, "Sat comm connection status", 15, "LOGGED IN-\nCONNECTING", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("SatCom Channel Label", new Point(213, 75), new Size(91, 23), _interfaceDeviceName, "Sat comm channel label", 15, "DAMA", TextHorizontalAlignment.Left, ""));
            _textDisplayList.Add(AddTextDisplay("Central Information Area", new Point(116, 76), new Size(226, 140), _interfaceDeviceName, "KY label", 15, "DAMA\nCOMSEC\nPARAMETRS\nUPDATED", TextHorizontalAlignment.Center, TextVerticalAlignment.Center, ""));
            _textDisplayList.Add(AddTextDisplay("WOD Segment Display", new Point(209, 126), new Size(52, 52), _interfaceDeviceName, "WOD Segment Display", _font, 29, "20", TextHorizontalAlignment.Right, ""));

            RotarySwitchPositionCollection positions = new RotarySwitchPositionCollection();
            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "OFF", 225d));
            positions.Add(new RotarySwitchPosition(this, 2, "TR G", 270d));
            positions.Add(new RotarySwitchPosition(this, 3, "TR", 315d));
            positions.Add(new RotarySwitchPosition(this, 4, "ADF", 360d));
            positions.Add(new RotarySwitchPosition(this, 5, "CHG PRST", 45d));
            positions.Add(new RotarySwitchPosition(this, 6, "TEST", 90d));
            positions.Add(new RotarySwitchPosition(this, 7, "ZERO (PULL)", 135d));
            AddRotarySwitch("Operational Mode Switch", new Point(107, 327), new Size(50, 50), $"{_imageLocation}ARC210_Knob_2.png", 1, positions, "Operational Mode Switch");
            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "ECCM MASTER", 225d));
            positions.Add(new RotarySwitchPosition(this, 2, "ECCM", 270d));
            positions.Add(new RotarySwitchPosition(this, 3, "PRST", 315d));
            positions.Add(new RotarySwitchPosition(this, 4, "MAN", 360d));
            positions.Add(new RotarySwitchPosition(this, 5, "MAR", 45d));
            positions.Add(new RotarySwitchPosition(this, 6, "243", 90d));
            positions.Add(new RotarySwitchPosition(this, 7, "121 (PULL)", 135d));
            AddRotarySwitch("Frequency Mode Switch", new Point(341, 327), new Size(50, 50), $"{_imageLocation}ARC210_Knob_2.png", 1, positions, "Frequency Mode Switch");

            RotaryEncoder enc;
            enc = AddEncoder("Channel Selector", new Point(223, 326), new Size(54,54),  $"{_imageLocation}ARC210_Knob_3.png", 0.1d, 30d, _interfaceDeviceName, "Channel Selector", false);
            enc.InitialRotation = 15;

            AddPushButton("LSK 1", 36, 77, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "LSK 1");
            AddPushButton("LSK 2", 36, 137, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "LSK 2");
            AddPushButton("LSK 3", 36, 197, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "LSK 3");

            AddPushButton("Brightness Increase", 20, 241, new Size(22, 28), $"{_imageLocation}ARC210_3_Norm.png", "Brightness Increase");
            AddPushButton("Brightness Decrease", 20, 297, new Size(22, 28), $"{_imageLocation}ARC210_3_Norm.png", "Brightness Decrease");

            AddPushButton("TOD SND Key", 95, 14, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "TOD SND Key");
            AddPushButton("TOD RCV Key", 184, 14, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "TOD RCV Key");
            AddPushButton("GPS Key", 249, 14, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "GPS Key");
            AddPushButton("RT SELECT Key", 312, 14, new Size(53, 21), $"{_imageLocation}ARC210_1_Norm.png", "RT SELECT Key");

            AddPushButton("MENU/TIME Key", 380, 138, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "MENU/TIME Key");
            AddPushButton("AM/FM Key", 454, 138, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "AM/FM Key");
            AddPushButton("XMT/REC or SEND Key", 380, 196, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "XMT/REC or SEND Key");
            AddPushButton("OFFSET or RCV Key", 454, 196, new Size(29d, 21d), $"{_imageLocation}ARC210_2_Norm.png", "OFFSET or RCV Key");
            AddPushButton("ENTER Key", 457, 249, new Size(30, 73), $"{_imageLocation}ARC210_4_Norm.png", "ENTER Key");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 MHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 2, "100 MHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 3, "200 MHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 4, "300 MHz", 30d));
            AddRotarySwitch("100 MHz Selector", new Point(69, 263), new Size(40,40), $"{_imageLocation}ARC210_Knob_1.png", 1, positions, "100 MHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 MHz",  270d));
            positions.Add(new RotarySwitchPosition(this, 2, "10 MHz", 290d));
            positions.Add(new RotarySwitchPosition(this, 3, "20 MHz", 310d));
            positions.Add(new RotarySwitchPosition(this, 4, "30 MHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 5, "40 MHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 6, "50 MHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 7, "60 MHz", 30d));
            positions.Add(new RotarySwitchPosition(this, 8, "70 MHz", 50d));
            positions.Add(new RotarySwitchPosition(this, 9, "80 MHz", 70d));
            positions.Add(new RotarySwitchPosition(this, 10, "90 MHz", 90d));
            AddRotarySwitch("10 MHz Selector", new Point(152, 265), new Size(36, 36), $"{_imageLocation}ARC210_Knob_1.png", 1, positions, "10 MHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 MHz", 270d));
            positions.Add(new RotarySwitchPosition(this, 2, "1 MHz", 290d));
            positions.Add(new RotarySwitchPosition(this, 3, "2 MHz", 310d));
            positions.Add(new RotarySwitchPosition(this, 4, "3 MHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 5, "4 MHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 6, "5 MHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 7, "6 MHz", 30d));
            positions.Add(new RotarySwitchPosition(this, 8, "7 MHz", 50d));
            positions.Add(new RotarySwitchPosition(this, 9, "8 MHz", 70d));
            positions.Add(new RotarySwitchPosition(this, 10, "9 MHz", 90d));
            AddRotarySwitch("1 MHz Selector", new Point(233, 265), new Size(36, 36), $"{_imageLocation}ARC210_Knob_1.png", 1, positions, "1 MHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 KHz",   270d));
            positions.Add(new RotarySwitchPosition(this, 2, "100 KHz", 290d));
            positions.Add(new RotarySwitchPosition(this, 3, "200 KHz", 310d));
            positions.Add(new RotarySwitchPosition(this, 4, "300 KHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 5, "400 KHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 6, "500 KHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 7, "600 KHz", 30d));
            positions.Add(new RotarySwitchPosition(this, 8, "700 KHz", 50d));
            positions.Add(new RotarySwitchPosition(this, 9, "800 KHz", 70d));
            positions.Add(new RotarySwitchPosition(this, 10, "900 KHz", 90d));
            AddRotarySwitch("100 KHz Selector", new Point(313, 265), new Size(36, 36), $"{_imageLocation}ARC210_Knob_1.png", 1, positions, "100 KHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "0 KHz", 330d));
            positions.Add(new RotarySwitchPosition(this, 2, "25 KHz", 350d));
            positions.Add(new RotarySwitchPosition(this, 3, "50 KHz", 10d));
            positions.Add(new RotarySwitchPosition(this, 4, "75 KHz", 30d));
            AddRotarySwitch("25 KHz Selector", new Point(395, 265), new Size(36, 36), $"{_imageLocation}ARC210_Knob_1.png", 1, positions, "25 KHz Selector");

            positions.Clear();
            positions.Add(new RotarySwitchPosition(this, 1, "OFF", 330d));
            positions.Add(new RotarySwitchPosition(this, 2, "ON", 30d));
            AddRotarySwitch("Squelch Switch", new Point(385, 68), new Size(44, 44), $"{_imageLocation}ARC210_Knob_3.png", 1, positions, "Squelch Switch");


            // this is to allow the displays to be cleared when the OFF switch is selected.
            DefaultSelfBindings.Add(new DefaultSelfBinding(
                triggerChildName: "ARC-210 Radio_Operational Mode Switch",
                deviceTriggerName: "position 1.entered",
                deviceTriggerBindingValue: new BindingValue(true),
                actionChildName: "",
                deviceActionName: "set.clear radio display"
                ));
            DefaultSelfBindings.Add(new DefaultSelfBinding(
                triggerChildName: "ARC-210 Radio_Operational Mode Switch",
                deviceTriggerName: "position 1.exited",
                deviceTriggerBindingValue: new BindingValue(false),
                actionChildName: "",
                deviceActionName: "set.clear radio display"
                ));
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "ARC-210.Operational Mode Switch.changed",
                deviceActionName: "set.clear radio display",
                triggerBindingSource: BindingValueSources.LuaScript,
                triggerBindingValue: new BindingValue("return TriggerValue==1")
                );
            DefaultSelfBindings.Add(new DefaultSelfBinding(
                triggerChildName: "",
                deviceTriggerName: "ARC-210 Radio_Brightness Increase.pushed",
                deviceTriggerBindingValue: null,
                actionChildName: "",
                deviceActionName: "increment.display brightness"
                ));
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "ARC-210.Brightness Increase.changed",
                deviceActionName: "increment.display brightness"
                );
            DefaultSelfBindings.Add(new DefaultSelfBinding(
                triggerChildName: "",
                deviceTriggerName: "ARC-210 Radio_Brightness Decrease.pushed",
                deviceTriggerBindingValue: null,
                actionChildName: "",
                deviceActionName: "decrement.display brightness"
                ));
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "ARC-210.Brightness Decrease.changed",
                deviceActionName: "decrement.display brightness"
                );
        }

        public override string DefaultBackgroundImage => _imageLocation + "ARC210_Panel.png";

        public bool UseTextualDisplays
        {
            get => _useTextualDisplays;
            set
            {
                if (value != _useTextualDisplays)
                {
                    _useTextualDisplays = value;
                    _displayBackground.IsHidden = !_useTextualDisplays;
                    foreach(TextDisplay td in _textDisplayList)
                    {
                        td.IsHidden = !_useTextualDisplays;
                    }
                    ViewportName = _useTextualDisplays ? "" : "C130J_ARC210_SCREEN";
                    Refresh();
                }
            }
        }
        public bool RequiresPatches
        {
            get => _requiresPatches;
            set
            {  
                if(value != _requiresPatches){
                    _requiresPatches = value; 
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
                        RemoveViewport(_vpName);
                    }
                    OnPropertyChanged("ViewportName", _vpName, value, false);
                    _vpName = value;
                }
            }
        }

        private void AddPushButton(string name, double x, double y, Size size, string buttonImage, string interfaceElement)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: buttonImage,
                pushedImage: buttonImage.Replace("_Norm.", "_Pressed."),
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElement,
                fromCenter: false
                );
        }
        private RotarySwitch AddRotarySwitch(string name, Point posn, Size size, string knobImage, int defaultPosition, RotarySwitchPositionCollection positions, string interfaceElementName)
        {
            RotarySwitch newSwitch = new RotarySwitch
            {
                Name = Name + "_" + name,
                KnobImage = knobImage,
                DrawLabels = false,
                DrawLines = false,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = defaultPosition,
                IsContinuous = false
            };
            newSwitch.Positions.Clear();
            foreach (RotarySwitchPosition swPosn in positions)
            {
                newSwitch.Positions.Add(swPosn);
            }

            AddRotarySwitchBindings(name, posn, size, newSwitch, _interfaceDeviceName, interfaceElementName);
            return newSwitch;
        }
        private TextDisplay AddTextDisplay(string name, Point posn, Size size,
            string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp,
            TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            return AddTextDisplay(name, posn, size, interfaceDeviceName, interfaceElementName, _font2, baseFontsize, testDisp, hTextAlign, TextVerticalAlignment.Center, devDictionary);
        }
        private TextDisplay AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, string fontFamily, double baseFontsize, string testDisp,
                TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            return AddTextDisplay(name, posn, size, interfaceDeviceName, interfaceElementName, fontFamily, baseFontsize, testDisp, hTextAlign, TextVerticalAlignment.Center, devDictionary);
        }
        private TextDisplay AddTextDisplay(string name, Point posn, Size size,
        string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp,
        TextHorizontalAlignment hTextAlign, TextVerticalAlignment vTextAlign, string devDictionary)
        {
            return AddTextDisplay(name, posn, size, interfaceDeviceName, interfaceElementName, _font2, baseFontsize, testDisp, hTextAlign, vTextAlign, devDictionary);
        }
        private TextDisplay AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, string fontFamily, double baseFontsize, string testDisp, 
                TextHorizontalAlignment hTextAlign, TextVerticalAlignment vTextAlign, string devDictionary) 
        {

            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: fontFamily,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: vTextAlign,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xe0, 0x78, 0xbf, 0x9d),
                backgroundColor: Color.FromArgb(0xff, 0x04, 0x2a, 0x00),
                useBackground: false,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
            display.IsHidden = !_useTextualDisplays;
            return display; 
        }

        private ImageDecoration AddImage(string name, Point posn, Size size, string imageName)
        {
            ImageDecoration image = new ImageDecoration()
            {
                Name = name,
                Left = posn.X,
                Top = posn.Y,
                Width = size.Width,
                Height = size.Height,
                Alignment = ImageAlignment.Stretched,
                Image = imageName,
                IsHidden = !_useTextualDisplays

            };
            Children.Add(image);
            return image;
        }


        private void AddViewport(string name)
        {
            Rect vpRect = new Rect(114d, 74d, 230d, 144d);
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

            Children.Add(new Helios.Controls.Special.ViewportExtent
            {
                FillBackground = true,
                BackgroundColor = Color.FromArgb(0x80, 0xd9, 0x27, 0x62),
                FontColor = Color.FromArgb(255, 255, 255, 255),
                TextFormat = tf, 
                ViewportName = name,
                Left = vpRect.Left,
                Top = vpRect.Top,
                Width = vpRect.Width,
                Height = vpRect.Height,
                RequiresPatches = _requiresPatches,
            });
        }

        private void RemoveViewport(string name)
        {
            foreach (HeliosVisual visual in this.Children)
            {
                if (visual.TypeIdentifier == "Helios.Base.ViewportExtent")
                {
                    Children.Remove(visual);
                    break;
                }
            }
        }
        private void ClearDisplay_Execute(object action, HeliosActionEventArgs e)
        {
            foreach (TextDisplay td in _textDisplayList)
            {
                td.IsHidden = e.Value.BoolValue;
            }
        }
        private void DisplayBrightness_Execute(object action, HeliosActionEventArgs e)
        {
            double brightness = e.Value.DoubleValue / 10d ;
            foreach (TextDisplay td in _textDisplayList)
            {
                td.OnTextColor = Color.FromArgb(0xf0, Convert.ToByte((double)0x78 * brightness), Convert.ToByte((double)0xdf * brightness), Convert.ToByte((double)0x9d * brightness));
            }
        }
        private void IncrementBrightnessAction_Execute(object action, HeliosActionEventArgs e)
        {
            _displayBrightness = _displayBrightness >= 10 ? 10 : ++_displayBrightness;
            foreach (TextDisplay td in _textDisplayList)
            {
                td.OnTextColor = Color.FromArgb(0xf0, Convert.ToByte((double)0x78 * _displayBrightness / 10d), Convert.ToByte((double)0xdf * _displayBrightness / 10d), Convert.ToByte((double)0x9d * _displayBrightness / 10d));
            }
        }
        private void DecrementBrightnessAction_Execute(object action, HeliosActionEventArgs e)
        {
            _displayBrightness = _displayBrightness <= 0 ? 0 : --_displayBrightness;
            foreach (TextDisplay td in _textDisplayList)
            {
                td.OnTextColor = Color.FromArgb(0xf0, Convert.ToByte((double)0x78 * _displayBrightness / 10d), Convert.ToByte((double)0xdf * _displayBrightness / 10d), Convert.ToByte((double)0x9d * _displayBrightness / 10d));
            }
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
                UseTextualDisplays = true;
                RemoveViewport("");
            }
            _useTextualDisplays = reader.Name.Equals("UseTextualDisplays") ? bool.Parse(reader.ReadElementString("UseTextualDisplays")) : false;
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
            writer.WriteElementString("UseTextualDisplays", boolConverter.ConvertToInvariantString(_useTextualDisplays));
        }

        public override bool HitTest(Point location) => false;
    }
}