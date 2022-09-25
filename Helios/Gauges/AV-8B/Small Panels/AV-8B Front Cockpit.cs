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

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using System.Xml;
    using System.Globalization;

    [HeliosControl("Helios.AV8B.Cockpit", "Front Cockpit", "AV-8B Gauges", typeof(BackgroundImageRenderer))]
    class FrontCockpit : AV8BDevice
    {
        private static readonly Double SCREENRES = 1.0;
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "AV-8B Cockpit";
        private string _font = "Helios Virtual Cockpit F/A-18C Hornet IFEI"; // "Segment7 Standard"
        private Color _textColor = Color.FromArgb(0xff, 220, 220, 220);
        private Color _backGroundColor = Color.FromArgb(100, 100, 20, 50);
        private string _imageLocation = "{AV-8B}/Images/";
        private bool _useBackGround = false;
 
        public FrontCockpit()
            : base("Front Cockpit", new Size(2560 * SCREENRES, 1440 * SCREENRES))
        {
            AddPanel("EDP Surround", new Point(1673 , 55), new Size(624 , 292), _imageLocation + "WQHD/Panel/EDP Surround.png", _interfaceDeviceName,"EDP Surround Panel Element");
            AddPart("Master Arm Device", new MasterArmPanel(), new Point(164, 304), new Size(138, 617), _interfaceDeviceName, "Master Arm Element");
            AddPart("ODU Device", new ODU_1(), new Point(351 , 42), new Size(546 , 313), _interfaceDeviceName, "ODU Element");
            AddPart("MFD Left Device", new Left_MPCD_1(), new Point(301 , 347), new Size(560 , 692), _interfaceDeviceName, "MFD Left Element");
            AddPart("MFD Right Device", new Right_MPCD_1(), new Point(1707 , 347), new Size(560 , 692), _interfaceDeviceName, "MFD Right Element");
            AddPart("EDP Device", new EDP(), new Point(1769, 85), new Size(405, 233), _interfaceDeviceName, "EDP Element");
            AddPart("Landing Gear Indicators", new GearIndicatorPanel(), new Point(859, 759), new Size(135, 225), _interfaceDeviceName, "Gear Indicator Element");
            AddButton("Landing Gear Indicators", 859, 759, new Size(135, 225), _interfaceDeviceName, "Gear Indicator Button");  // this is to allow pressing the indicator panel to activate something
            AddPart("UFC Device", new UFC_1(), new Point(856 , 20), new Size(872 , 876), _interfaceDeviceName, "UFC Element");
            AddPart("Flight Instrument Device", new FlightInstrumentPanel(), new Point(998 , 858), new Size(774 , 556), _interfaceDeviceName, "Flight Instrument Panel");
            AddPart("Slip Turn Indicator", new SlipTurnPanel(), new Point(765, 1321), new Size(225, 114), _interfaceDeviceName, "Slip Ball");
            AddPart("SMC Device", new SMC_1(), new Point(275 , 1047), new Size(708 , 271), _interfaceDeviceName, "SMC Element");
            AddPart("FQIS Device", new FuelPanel.FQIS(), new Point(1774 , 1043), new Size(484 , 191), _interfaceDeviceName, "FQIS Element");
            AddPart("H2O Device", new H2OPanel(), new Point(165 , 987), new Size(108 , 321), _interfaceDeviceName, "H2O Element");
            AddPart("RWR / ECM Device", new RWRPanel(), new Point(2291 , 410), new Size(141 , 521), _interfaceDeviceName, "RWR Element");
            AddPart("Threat Indicator Device", new ThreatIndicatorPanel(), new Point(2184 , 122), new Size(186 , 268), _interfaceDeviceName, "Threat Indicator Element");

        }

        #region Properties
        //public double GlassReflectionOpacity
        //{
        //    get
        //    {
        //        return _IFEI_gauges.GlassReflectionOpacity;
        //    }
        //    set
        //    {
        //        double oldValue = _IFEI_gauges.GlassReflectionOpacity;
        //        _IFEI_gauges.GlassReflectionOpacity = value;
        //        if (value != oldValue)
        //        {
        //            OnPropertyChanged("GlassReflectionOpacity", oldValue, value, true);
        //        }
        //    }
        //}
        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
        }

        public override string DefaultBackgroundImage
        {
            get { return _imageLocation + "WQHD/Panel/AV8B Front Cockpit 2.png"; }
        }

        private void AddTextDisplay(string name, double x, double y, Size size, double baseFontsize, string testDisp,
            string interfaceDevice, string interfaceElement)
        {
            size.Width *= SCREENRES;
            size.Height *= SCREENRES;

            TextDisplay display = AddTextDisplay(
                name: name,
                posn: new Point(x * SCREENRES, y * SCREENRES),
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
            size.Width *= SCREENRES;
            size.Height *= SCREENRES;
            AddButton(
                name: name,
                posn: new Point(x * SCREENRES, y * SCREENRES),
                size: size,
                image: _imageLocation + "_transparent.png",
                pushedImage: _imageLocation + "_transparent.png",
                buttonText: "",
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                fromCenter: false
               );
        }
        private void Add3PosnToggle(string name, Point posn, Size size, string image, string interfaceDevice, string interfaceElement, bool fromCenter)
        {
            size.Width *= SCREENRES;
            size.Height *= SCREENRES;
            posn.X *= SCREENRES;
            posn.Y *= SCREENRES;

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
        private void AddPart(string name, CompositeVisual Part, Point posn, Size size, string interfaceDevice, string interfaceElement)
        {
            size.Width *= SCREENRES;
            size.Height *= SCREENRES;
            posn.X *= SCREENRES;
            posn.Y *= SCREENRES;

            CompositeVisual _part = AddDevice(
                name: name,
                device: Part,
                size: size,
                posn: posn, 
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement
               );
            {
                _part.Name = name;
            };
        }
        private new void AddGauge(string name, BaseGauge Part, Point posn, Size size, string interfaceDevice, string interfaceElement)
        {
            size.Width *= SCREENRES;
            size.Height *= SCREENRES;
            posn.X *= SCREENRES;
            posn.Y *= SCREENRES;

            BaseGauge _part = AddGauge(
                name: name,
                gauge: Part,
                size: size,
                posn: posn,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement
               );
            {
                _part.Name = name;
            };
        }
        private void AddPanel(string name, Point posn, Size size, string background, string interfaceDevice, string interfaceElement)
        {
            size.Width *= SCREENRES;
            size.Height *= SCREENRES;
            posn.X *= SCREENRES;
            posn.Y *= SCREENRES;

            HeliosPanel _panel =  AddPanel(
                name: name,
                posn: posn,
                size: size,
                background: background
                );
            _panel.FillBackground = false;
            _panel.DrawBorder = false;
            //_panel.BackgroundAlignment = ImageAlignment.Centered;


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

        //public override void WriteXml(XmlWriter writer)
        //{
        //    base.WriteXml(writer);
        //    if (_IFEI_gauges.GlassReflectionOpacity != IFEI_Gauges.GLASS_REFLECTION_OPACITY_DEFAULT)
        //    {
        //        writer.WriteElementString("GlassReflectionOpacity", GlassReflectionOpacity.ToString(CultureInfo.InvariantCulture));
        //    }
        //}

        //public override void ReadXml(XmlReader reader)
        //{
        //    base.ReadXml(reader);
        //    if (reader.Name.Equals("GlassReflectionOpacity"))
        //    {
        //        GlassReflectionOpacity = double.Parse(reader.ReadElementString("GlassReflectionOpacity"), CultureInfo.InvariantCulture);
        //    }
        //}
    }
}
