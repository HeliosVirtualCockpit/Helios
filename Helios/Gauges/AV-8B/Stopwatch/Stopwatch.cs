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

    [HeliosControl("Helios.AV8B.Stopwatch", "Stopwatch Panel", "AV-8B Gauges", typeof(BackgroundImageRenderer))]
    class StopwatchPanel: AV8BDevice
    {
        private string _imageLocation = "{AV-8B}/Images/";
        private string _interfaceDeviceName = "Stopwatch";
        private static string _generalComponentName = "Stopwatch";
        private double _GlassReflectionOpacity = 0;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 1.0;

        public StopwatchPanel()
            : base(_generalComponentName, new Size(412, 475))
        {
            AddButton("Start/Stop Button", 176, 1, new Size(51, 87), _interfaceDeviceName, "Start/Stop Button");
            AddButton("Lap/Reset Button", 314, 65, new Size(98, 99), _interfaceDeviceName, "Lap/Reset Button", "_Lap");
            AddGauge("Stopwatch", new StopwatchDial(), new Point(3, 85), new Size(390, 390), _interfaceDeviceName, new string[2] { "Minutes", "Seconds" }, _generalComponentName);
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
            get { return $"{_imageLocation}Stopwatch Mounting Panel.png"; }
        }

        private void AddButton(string name, double x, double y, Size size, string interfaceDeviceName, string interfaceElementName, string imageModifier = "")
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: $"{_imageLocation}Stopwatch{imageModifier}_Button_Unpushed.png",
                pushedImage: $"{_imageLocation}Stopwatch{imageModifier}_Button_Pushed.png",
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

