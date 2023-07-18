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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Clock
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F15E.Clock", "Clock Panel", "F-15E Strike Eagle", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    public class ClockPanel: CompositeVisualWithBackgroundImage
    {
        private string _imageLocation = "{AV-8B}/Images/";
        private string _interfaceDeviceName = "Clock";
        private static string _generalComponentName = "Clock";
        private double _GlassReflectionOpacity = 0;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 1.0;
        private ClockFace _display;

        public ClockPanel(string name)
            : base(_generalComponentName, new Size(483, 532))
        {
            AddClockFace("Clock", new Point(150, 150), new Size(300, 300), _interfaceDeviceName, new string[3] { "Hours", "Minutes", "Seconds" }, _generalComponentName);
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

        private void AddClockFace(string name, Point pos, Size size, string interfaceDevice, string[] interfaceElementNames, string _componentName)
        {
            _display = new ClockFace
            (pos,size
            );
            //interfaceDeviceName: interfaceDevice,
            //interfaceElementNames: interfaceElementNames
            Children.Add(_display);
            // Note:  we have the actions against the new embedded gauge but to expose those
            // actions in the interface, we copy the actions to the Parent.  This is a new 
            // HeliosActionCollection with the keys equal to the new ActionIDs, however the original
            // HeliosActionCollection which is on the child part will have the original keys, even though
            // we might have changed the values of the ActionIDs.  This has the result that autobinding
            // in CompositeVisual (OnProfileChanged) might not be able to find the actions when doing
            // the "ContainsKey()" for the action.
            // This is why the _display.Name is in the deviceActionName of the AddDefaultInputBinding
            // and *MUST* match the BindingValue device parameter for the gauge being added.

            //foreach (IBindingTrigger trigger in _display.Triggers)
            //{
            //    AddTrigger(trigger, trigger.Name);
            //}
            foreach (IBindingAction action in _display.Actions)
            {
                if (action.Name != "hidden")
                {

                    AddAction(action, _display.Name);
                    //Create the automatic input bindings for the sub component
                    AddDefaultInputBinding(
                       childName: _display.Name,
                       deviceActionName: _display.Name + "." + action.ActionVerb + "." + action.Name,
                       interfaceTriggerName: interfaceDevice + "." + action.Name + ".changed"
                       );
                }

            }
            //_display.Actions.Clear();
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

