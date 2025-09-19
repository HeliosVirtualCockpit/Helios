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
    using System.Windows;

    [HeliosControl("Helios.AV8B.FlightInstruments", "Flight Instrument Panel", "AV-8B Harrier", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class FlightInstrumentPanel: AV8BDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _imageLocation = "{AV-8B}/Images/";
        private string _interfaceDeviceName = "Flight Instruments";
        private static string _generalComponentName = "Flight Instruments";
        private double _GlassReflectionOpacity = 0;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 1.0;

        public FlightInstrumentPanel()
            : base(_generalComponentName, new Size(1030, 739))
        {
            AddGauge("VVI Gauge", new VVI1(), new Point(387, 456), new Size(238, 247), _interfaceDeviceName, "VVI", _generalComponentName);
            AddGauge("IAS Gauge", new IAS(), new Point(70, 186), new Size(203, 203), _interfaceDeviceName, "IAS Airspeed", _generalComponentName);
            AddGauge("AOA Gauge", new AOA(), new Point(61, 473), new Size(221, 221), _interfaceDeviceName, new string[2] { "AOA Flag", "Angle of Attack" }, _generalComponentName);
            AddGauge("SAI Gauge", new SAI(), new Point(350, 59), new Size(293, 293), _interfaceDeviceName, new string[4] { "SAI Pitch" , "SAI Bank", "SAI Pitch Adjust Knob", "SAI Warning Flag" }, _generalComponentName);

            AddButton("SAI Cage Pull Switch", 515, 218, new Size(60, 60), "Flight Instruments", "SAI Cage Pull Switch");
            AddPot("SAI Pitch Adjust Knob", new Point(575, 278), new Size(60, 60), "Flight Instruments", "SAI Pitch Adjust Knob", "WQHD/Knob/Cage Knob.png");

            _interfaceDeviceName = "NAV course";
            AddThreeWayToggleHorizontal("Course Set Switch", 150, 40, new Size(89, 137), "Course Setting", "Horizontal Switch");
            _interfaceDeviceName = "Flight Instruments";
            AddGauge("Altimeter Gauge", new Altimeter(), new Point(727, 351), new Size(261, 272), _interfaceDeviceName, new string[2] { "Air Pressure", "Altitude"  }, _generalComponentName);
            AddEncoder("Altimeter Pressure Adjust", new Point(720, 558), new Size(60,60), "Barometric pressure calibration adjust");
        }

        public double GlassReflectionOpacity
        {
            get
            {
                return _GlassReflectionOpacity;
            }
            set
            {
            }
        }

        public override string DefaultBackgroundImage
        {
            get { return _imageLocation + "WQHD/Panel/Flight Instruments.png"; }
        }

        private void AddButton(string name, double x, double y, Size size, string interfaceDeviceName, string interfaceElementName)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: "{AV-8B}/Images/_transparent.png",
                pushedImage: "{AV-8B}/Images/_transparent.png",
                buttonText: "",
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
        }

        private void AddEncoder(string name, Point posn, Size size, string interfaceElementName)
        {
            AddEncoder(name, posn, size, interfaceElementName, "Rotary.png");
        }

        private void AddEncoder(string name, Point posn, Size size, string interfaceElementName, string knobName)
        {
            AddEncoder(
                name: name,
                size: size,
                posn: posn,
                knobImage: _imageLocation + knobName,
                stepValue: 0.5,
                rotationStep: 5,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false,
                clickType: RotaryClickType.Swipe
                );
        }

        private void AddPot(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, string knobName)
        {
            AddPot(name: name,
                posn: posn,
                size: size,
                knobImage: _imageLocation + knobName,
                initialRotation: -45,
                rotationTravel: 90,
                minValue: -1,
                maxValue: 1,
                initialValue: 0.0,
                stepValue: 0.01,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                isContinuous: false,
                fromCenter: false);
        }

        private void AddThreeWayToggleHorizontal(string name, double x, double y, Size size, string interfaceElementName) => AddThreeWayToggleHorizontal(name, x, y, size, interfaceElementName, name);
        private void AddThreeWayToggleHorizontal(string name, double x, double y, Size size, string interfaceElementName, string imageName)
        {
            ThreeWayToggleSwitch toggle = AddThreeWayToggle(
                name: name,
                posn: new Point(x, y),
                size: size,
                defaultPosition: ThreeWayToggleSwitchPosition.Two,
                defaultType: ThreeWayToggleSwitchType.MomOnMom,
                positionOneImage: _imageLocation + "WQHD/Switch/" + imageName + " Right.png",
                positionTwoImage: _imageLocation + "WQHD/Switch/" + imageName + " Normal.png",
                positionThreeImage: _imageLocation + "WQHD/Switch/" + imageName + " Left.png",
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                horizontal: true,
                fromCenter: false
                );
            toggle.Name = _generalComponentName + "_" + name;
        }

        private void AddPart(string name, CompositeVisual Part, Point posn, Size size, string interfaceDevice, string interfaceElement)
        {
            CompositeVisual part = AddDevice(
                name: name,
                device: Part,
                size: size,
                posn: posn,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement
                );
            {
                part.Name = _generalComponentName + "_" + name;
            };
        }

        private void AddGauge(string name, BaseGauge part, Point posn, Size size, string interfaceDevice, string interfaceElement, string _componentName) =>
            AddGauge(name, part, posn, size, interfaceDevice, new string[1] { interfaceElement }, _componentName);
        private void AddGauge(string name, BaseGauge part, Point posn, Size size, string interfaceDevice, string[] interfaceElementNames, string _componentName)
        {
            part.Name = _componentName + name;
            BaseGauge gauge = base.AddGauge(
                name: name,
                gauge: part,
                posn: posn,
                size: size,
                interfaceDeviceName: interfaceDevice,
                interfaceElementNames: interfaceElementNames
                );
            {
                gauge.Name = _componentName + "_" + name;
            };
        }

        private void AddPanel(string name, Point posn, Size size, string background, string interfaceDevice, string interfaceElement)
        {
            HeliosPanel _panel = AddPanel(
                 name: name,
                 posn: posn,
                 size: size,
                 background: background
                  );
            _panel.FillBackground = false;
            _panel.DrawBorder = false;
            //_panel.BackgroundAlignment = ImageAlignment.Centered;
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            base.ScaleChildren(scaleX, scaleY);
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

