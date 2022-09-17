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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.PCNPanel
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

    [HeliosControl("HELIOS.M2000C.PCN_PANEL", "PCN Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_PCNPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 690, 530);
        private string _interfaceDeviceName = "PCN Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _font = "Helios Virtual Cockpit F/A-18C Hornet IFEI";
        private bool _useTextualDisplays = false;
        private PCNPanelGauge _pcnGauge;

        public M2000C_PCNPanel()
            : base("PCN Panel", new Size(690, 530))
        {
            _pcnGauge = new PCNPanelGauge(this, "PCN Gauge", NativeSize);
            Children.Add(_pcnGauge);
            //Children.Add(new PCNPanelGauge(this, "PCN Gauge", NativeSize));
            int row0 = 231, row2 = 233, row3 = 316, row4 = 323, row5 = 396, row6 = 394, row7 = 468, row8 = 481, row9 = 127, row10 = 100, row11 = 140;
            int column0 = 123, column2 = 210, column3 = 324, column4 = 327, column5 = 429, column6 = 507, column7 = 587, column8 = 398, column9 = 452, column10 = 503, column11 = 557, column12 = 610;
            AddIndicatorPushButton("PREP", "prep", new Point(column0, row0), new Size(50, 50));
            AddIndicatorPushButton("DEST", "dest", new Point(column3, row0), new Size(50, 50));
            AddPushButton("INS Button 1", "ins-1" ,new Point(column5, row2), new Size(52, 60));
            AddPushButton("INS Button 2", "ins-2", new Point(column6, row2), new Size(52, 60));
            AddPushButton("INS Button 3", "ins-3", new Point(column7, row2), new Size(52, 60));
            AddPushButton("INS Button 4", "ins-4", new Point(column5, row3), new Size(52, 60));
            AddPushButton("INS Button 5", "ins-5", new Point(column6, row3), new Size(52, 60));
            AddPushButton("INS Button 6", "ins-6", new Point(column7, row3), new Size(52, 60));
            AddPushButton("INS Button 7", "ins-7", new Point(column5, row5), new Size(52, 60));
            AddPushButton("INS Button 8", "ins-8", new Point(column6, row5), new Size(52, 60));
            AddPushButton("INS Button 9", "ins-9", new Point(column7, row5), new Size(52, 60));
            AddPushButton("INS Button 0", "ins-0", new Point(column6, row8), new Size(52, 60));
            AddIndicatorPushButton("EFF", "eff", new Point(column5, row8), new Size(50, 50));
            AddIndicatorPushButton("INS", "ins", new Point(column7, row8), new Size(50, 50));
            //The ENC button has been removed from the aircraft
            //AddIndicatorPushButton("Offset Waypoint/Target", "enc", new Point(column1, row1), new Size(58, 40));
            AddIndicatorPushButton("AUTO Navigation", "bad", new Point(column4, row4), new Size(58, 40));
            AddIndicatorPushButton("INS Update", "rec", new Point(column4, row6), new Size(58, 40));
            AddIndicatorPushButton("Marq Position", "mrq", new Point(column4, row7), new Size(58, 40));
            AddIndicatorPushButton("Validate Data Entry", "val", new Point(column2, row7), new Size(58, 40));

            AddIndicator("M91", "m91", new Point(column8, row10), new Size(25, 13));
            AddIndicator("M92", "m92", new Point(column9, row10), new Size(27, 13));
            AddIndicator("M93", "m93", new Point(column10, row10), new Size(27, 13));
            AddIndicator("PRET", "pret", new Point(column8, row9), new Size(40, 13));
            AddIndicator("ALN", "aln", new Point(column9, row9), new Size(30, 13));
            AddIndicator("MIP", "mip", new Point(column10, row9), new Size(25, 13));
            AddIndicator("NDEG", "ndeg", new Point(column11, row9), new Size(45, 13));
            AddIndicator("SEC", "sec", new Point(column12, row9), new Size(32, 13));
            AddIndicator("UNI", "uni", new Point(column9, row11), new Size(24, 13));

            AddSwitch("INS Parameter Selector", "{M2000C}/Images/PCNPanel/ins-parameter-selector.png", new Point(149, 349), new Size(118, 118), true);

            AddTextDisplay("PCN Latitude Display", new Point(96d, 6d), new Size(251d, 72d), _interfaceDeviceName, "PCN Latitude Display", 64, "1234567", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PCN Longitude Display", new Point(406d, 6d), new Size(251d, 72d), _interfaceDeviceName, "PCN Longitude Display", 64, "123456", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PCN Left Points Position", new Point(82d, 13d), new Size(251d, 72d), _interfaceDeviceName, "PCN Left Points Position", 64,"   .  .", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PCN Right Points Position", new Point(392d, 13d), new Size(251d, 72d), _interfaceDeviceName, "PCN Right Points Position", 64, "  .  .", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PCN Lower Left Display", new Point(82d, 82d), new Size(120d, 72d), _interfaceDeviceName, "PCN Lower Left Display", 64, "01", TextHorizontalAlignment.Left, "");
            AddTextDisplay("PCN Lower Right Display", new Point(288d, 82d), new Size(120d, 72d), _interfaceDeviceName, "PCN Lower Right Display", 64, "01", TextHorizontalAlignment.Left, "");


        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/PCNPanel/pcn-panel.png"; }
        }

        public bool UseTextualDisplays
        {
            get => _useTextualDisplays;
            set
            {
                if (value != _useTextualDisplays)
                {
                    _useTextualDisplays = value;
                    _pcnGauge.IsHidden = !_useTextualDisplays;
                    Refresh();
                }
            }
        }

        #endregion

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

        private void AddPushButton(string name, string imagePrefix, Point posn, Size size)
        {
            AddButton(name: name,
                posn: posn,
                size: size,
                image: "{M2000C}/Images/PCNPanel/" + imagePrefix + ".png",
                pushedImage: "{M2000C}/Images/PCNPanel/" + imagePrefix + ".png",
                buttonText: "",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true);
        }

        private void AddIndicatorPushButton(string name, string imagePrefix, Point pos, Size size)
        {
            AddIndicatorPushButton(name: name,
                pos: pos,
                size: size,
                image: "{M2000C}/Images/PCNPanel/" + imagePrefix + ".png",
                pushedImage: "{M2000C}/Images/PCNPanel/" + imagePrefix + ".png",
                textColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text,
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text,
                font: "",
                onImage: "{M2000C}/Images/PCNPanel/" + imagePrefix + "-on.png",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true,
                withText: false);
        }

        private void AddIndicator(string name, string imagePrefix, Point posn, Size size)
        {
            AddIndicator(
                name: name,
                posn: posn,
                size: size,
                onImage: "{M2000C}/Images/PCNPanel/" + imagePrefix + "-on.png",
                offImage: "{M2000C}/Images/Miscellaneous/void.png", //empty picture to permit the indicator to work because I’ve nothing to display when off
                onTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                offTextColor: Color.FromArgb(0xff, 0x7e, 0xde, 0x72), //don’t need it because not using text
                font: "", //don’t need it because not using text
                vertical: false, //don’t need it because not using text
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true,
                withText: false); //added in Composite Visual as an optional value with a default value set to true
        }

        private void AddSwitch(string name, string knobImage, Point posn, Size size, bool fromCenter)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: knobImage,
                defaultPosition: 0, 
                clickType: RotaryClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: fromCenter);
            rSwitch.IsContinuous = true;
            rSwitch.Positions.Clear();
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 0, "TR/VS", 220d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "D/RLT", 240d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "CP/PD", 270d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 3, "ALT", 300d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 4, "L/G", 325d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 5, "RD/TD", 0d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 6, "dL/dG", 40d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 7, "dALT", 70d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 8, "P/t", 95d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 9, "REC", 135d));
            rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 10, "DV/FV", 185d));
        }
        private void AddTextDisplay(string name, Point posn, Size size,
                string interfaceDeviceName, string interfaceElementName, double baseFontsize, 
                string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xcc, 0x50, 0xc3, 0x39),
                backgroundColor: Color.FromArgb(0xff, 0x04, 0x2a, 0x00),
                useBackground: false,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
        }
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name.Equals("UseTextualDisplays"))
            {
                UseTextualDisplays = bool.Parse(reader.ReadElementString("UseTextualDisplays"));
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("UseTextualDisplays", _useTextualDisplays.ToString(CultureInfo.InvariantCulture));
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
    }
}
