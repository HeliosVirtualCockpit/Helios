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

namespace GadrocsWorkshop.Helios.Gauges.A10C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// This is a version of the Counter Measure System Panel which uses a bespoke font to provide data in a text display instead of cutouts for the exported viewport.
    /// </summary>
    /// 
    [HeliosControl("Helios.A10C.CMSP", "Counter Measures System Panel", "A-10C Gauges", typeof(BackgroundImageRenderer))]
    class CMSP : A10CDevice
    {
        // these two sections are the dead space in the CMSP image.
        //private Rect _scaledScreenRectTL = new Rect(0, 0, 398, 116);
        //private Rect _scaledScreenRectB = new Rect(76, 384, 648, 87);
        private string _interfaceDeviceName = "CMSP";
        private string _CMSPConversion = "";  // One character (diamond which might need this, but so far not seen this character being sent)
        private string _imageLocation = "{A-10C}/Images/A-10C/";
        private readonly RotarySwitchPositionCollection _positions = new RotarySwitchPositionCollection();

        /// <summary>
        /// a panel that is also installed as one of our children
        /// </summary>
        private readonly HeliosPanel _bezel;

        /// <summary>
        /// the image we use as the background or our panel, if we are using default backgrounds
        /// </summary>
        private const string PANEL_IMAGE = "A-10C_CMSP_Panel.png";

        public CMSP()
            : base("CMSP", new Size(666, 404))
        {
            //AddPanel("Filters", new Point(0, 0), new Size(666, 404), _imageLocation + "A-10C_CMSP_Filter_Panel.png", _interfaceDeviceName, "Display Filters");
            _bezel = AddPanel("Bezel", new Point(0, 0), new Size(666, 404), _imageLocation + PANEL_IMAGE, _interfaceDeviceName, "CMSP Bezel");
            AddTextDisplay("Display Line 1 Text", 73, 35, new Size(430, 50), "Line 1 Display", "a1B2C3D4m5s6x7O8", "    =[]]]];   =[]]];  =[]]; =][;");  // These substitutions are for blanks of different widths
            AddOSBButton("OSB 1", 86, 168, new Size(64, 64), "OSB 1");
            AddOSBButton("OSB 2", 175, 168, new Size(64, 64), "OSB 2");
            AddOSBButton("OSB 3", 275, 168, new Size(64, 64), "OSB 3");
            AddOSBButton("OSB 4", 367, 168, new Size(64, 64), "OSB 4");
            AddRTNButton("RTN", 546, 50, new Size(68,67), "Return to Previous Rotary Menu");

            AddRocker("Page Cycle", new Point(492, 38), new Size(51, 97), 
                ThreeWayToggleSwitchPosition.Two, ThreeWayToggleSwitchType.MomOnMom, 
                _interfaceDeviceName, "Page Cycle", false,
                _imageLocation + "A-10C_CMSP_NXT_Rocker_Up.png",
                _imageLocation + "A-10C_CMSP_NXT_Rocker_Middle.png",
                _imageLocation + "A-10C_CMSP_NXT_Rocker_Down.png",
                LinearClickType.Touch, false);

            AddThreeWayToggle("MWS Switch", new Point(93, 262), new Size(41, 139), "MWS");
            AddThreeWayToggle("JMR Switch", new Point(186, 262), new Size(41, 139), "JMR");
            AddThreeWayToggle("RWR Switch", new Point(278, 262), new Size(41, 139), "RWR");
            AddThreeWayToggle("Disp Switch", new Point(374, 262), new Size(41, 139), "DISP");
            AddTwoWayToggle("ECM Pod Jettison", 471, 139, new Size(53,128), "ECM Pod Jettison");
            AddPot("Brightness", new Point(550,176), new Size(60, 60), "Brightness");
            AddTextDisplay("Display MWS Text", 70, 87, new Size(98, 50), "Line 2 MWS Display", "amsx", _CMSPConversion);
            AddTextDisplay("Display JMR Text", 168, 87, new Size(98, 50), "Line 2 JMR Display", "amsx", _CMSPConversion);
            AddTextDisplay("Display RWR Text", 266, 87, new Size(98, 50), "Line 2 RWR Display", "amsx", _CMSPConversion);
            AddTextDisplay("Display Disp Text", 364, 87, new Size(98, 50), "Line 2 DISP Display", "amsx", _CMSPConversion);
            _positions.Add(new RotarySwitchPosition(this, 1, "OFF", 230d));
            _positions.Add(new RotarySwitchPosition(this, 2, "STBY", 270d));
            _positions.Add(new RotarySwitchPosition(this, 3, "MAN", 305d));
            _positions.Add(new RotarySwitchPosition(this, 4, "SEMI", 335d));
            _positions.Add(new RotarySwitchPosition(this, 5, "AUTO", 25d));
            RotarySwitch knob = AddRotarySwitch("Mode Dial", new Point(478, 269), new Size(125, 125), _imageLocation + "A-10C_CMSP_Knob.png", 1, _positions, _interfaceDeviceName, "Mode Select Dial", false);
            knob.IsContinuous = false;
        }

        public override string DefaultBackgroundImage => _imageLocation + "A-10C_CMSP_Filter_Panel.png";

        protected override void OnBackgroundImageChange()
        {
            _bezel.BackgroundImage = BackgroundImageIsCustomized ? null : System.IO.Path.Combine(_imageLocation, PANEL_IMAGE);
        }

        private void AddOSBButton(string name, double x, double y, Size size, string interfaceElement)
        {
            AddButton(
                name: name,
                posn: new Point(x, y),
                size: size,
                image: _imageLocation + "A-10C_CMSP_OSB_Pushbutton_Unpressed.png",
                pushedImage: _imageLocation + "A-10C_CMSP_OSB_Pushbutton_Pressed.png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElement,
                fromCenter: false
               );
        }
        private void AddRTNButton(string name, double x, double y, Size size, string interfaceElement)
        {
            AddButton(
                name: name,
                posn: new Point(x, y),
                size: size,
                image: _imageLocation + "A-10C_CMSP_RTN_Pushbutton_Unpressed.png",
                pushedImage: _imageLocation + "A-10C_CMSP_RTN_Pushbutton_Pressed.png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElement,
                fromCenter: false
               );
        }

        private void AddPot(string name, Point posn, Size size, string interfaceElementName)
        {
            AddPot(name: name,
                posn: posn,
                size: size,
                knobImage: _imageLocation + "A-10C_HARS_Heading_Knob.png",
                initialRotation: 0,
                rotationTravel: 360,
                minValue: 0,
                maxValue: 1,
                initialValue: 0,
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                isContinuous: true,
                fromCenter: false);
        }

        private void AddTextDisplay(string name, double x, double y, Size size,
            string interfaceElementName, string testDisp, string conversionDictionary)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: new Point(x, y),
                size: size,
                font: "Helios Virtual Cockpit A-10C_ALQ_213",
                baseFontsize: 22,
                horizontalAlignment: TextHorizontalAlignment.Left,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xd5, 0x9c, 0xcf, 0x08),
                backgroundColor: Color.FromArgb(0xff, 0x00, 0x00, 0x00),
                useBackground: false,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: conversionDictionary
                );
        }
        private void AddTwoWayToggle(string name, double x, double y, Size size, string interfaceElementName)
        {
            ToggleSwitch toggle = AddToggleSwitch(
                name: name,
                posn: new Point(x, y),
                size: size,
                defaultPosition: ToggleSwitchPosition.One,
                defaultType: ToggleSwitchType.OnOn,
                positionOneImage: _imageLocation + "A-10C_CMSP_Jettison_Toggle_Up.png",
                positionTwoImage: _imageLocation + "A-10C_CMSP_Jettison_Toggle_Down.png",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                clickType: LinearClickType.Swipe,
                fromCenter: false
                );
            toggle.Name = "CMSP_" + name;
        }

        private void AddThreeWayToggle(string name, Point posn, Size size,
            string interfaceElementName)
        {
            string componentName = _interfaceDeviceName + "_" + name;
            ThreeWayToggleSwitch toggle = new ThreeWayToggleSwitch
            {
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height,
                DefaultPosition = ThreeWayToggleSwitchPosition.Two,
                PositionOneImage = _imageLocation + "A-10C_CMSP_Toggle_Up.png",
                PositionTwoImage = _imageLocation + "A-10C_CMSP_Toggle_Middle.png",
                PositionThreeImage = _imageLocation + "A-10C_CMSP_Toggle_Down.png",
                SwitchType = ThreeWayToggleSwitchType.MomOnOn,
                Name = _interfaceDeviceName + "_" + name,
                ClickType = LinearClickType.Swipe
            };

            Children.Add(toggle);
            foreach (IBindingTrigger trigger in toggle.Triggers)
            {
                AddTrigger(trigger, componentName);
            }
            AddAction(toggle.Actions["set.position"], componentName);
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position.changed",
                interfaceActionName: _interfaceDeviceName + ".set." + interfaceElementName);
            AddDefaultOutputBinding(
                childName: componentName,
                deviceTriggerName: "position two.entered",
                interfaceActionName: _interfaceDeviceName + ".release." + interfaceElementName);
            AddDefaultInputBinding(
                childName: componentName,
                interfaceTriggerName: _interfaceDeviceName + "." + interfaceElementName + ".changed",
                deviceActionName: "set.position");
        }
    }
}