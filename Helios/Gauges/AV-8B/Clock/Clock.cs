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

    [HeliosControl("Helios.AV8B.Clock", "Clock Panel", "AV-8B Gauges", typeof(BackgroundImageRenderer))]
    class ClockPanel: AV8BDevice
    {
        private string _imageLocation = "{AV-8B}/Images/";
        private string _interfaceDeviceName = "Clock";
        private static string _generalComponentName = "Clock";
        private double _GlassReflectionOpacity = 0;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 1.0;

        public ClockPanel()
            : base(_generalComponentName, new Size(483, 532))
        {
            AddGauge("Clock", new ClockFace(), new Point(48, 43), new Size(390, 390), _interfaceDeviceName, new string[3] { "Hours", "Minutes", "Seconds" }, _generalComponentName);
            AddButton("Button", new Point(346, 19), new Size(80, 80), _interfaceDeviceName, "Button");
            AddEncoder("Knob", new Point(11,351), new Size(120, 120), _interfaceDeviceName, "Knob", "Knob");
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
            get { return $"{_imageLocation}Clock Panel.png"; }
        }

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
        }

        private void AddButton(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, string imageModifier = "")
        {
            imageModifier = imageModifier == "" ? "Clock" : imageModifier;
            AddButton(
                name: name,
                posn: posn,
                size: size,
                image: $"{_imageLocation}{imageModifier} Button Unpushed.png",
                pushedImage: $"{_imageLocation}{imageModifier} Button Pushed.png",
                buttonText: "",
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }

        private void AddEncoder(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, string knobName = "")
        {
            AddEncoder(
                name: name,
                size: size,
                posn: posn,
                knobImage: $"{_imageLocation}Clock {knobName}.png",
                stepValue: 0.5,
                rotationStep: 5,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false,
                clickType: RotaryClickType.Swipe
                );
        }

        private void AddGauge(string name, BaseGauge part, Point posn, Size size, string interfaceDevice, string interfaceElement, string _componentName) =>
            AddGauge(name, part, posn, size, interfaceDevice, new string[1] { interfaceElement }, _componentName);
        private void AddGauge(string name, BaseGauge part, Point posn, Size size, string interfaceDevice, string[] interfaceElementNames, string _componentName)
        {
            part.Name = name;
            BaseGauge gauge = base.AddGauge(
                name: name,
                gauge: part,
                posn: posn,
                size: size,
                interfaceDeviceName: interfaceDevice,
                interfaceElementNames: interfaceElementNames
                );
            {
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

