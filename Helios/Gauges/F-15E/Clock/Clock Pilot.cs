//  Copyright 2014 Craig Courtney
//  Copyright 2023 Helios Contributors
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
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.F15E.Clock.Pilot", "Clock (Pilot)", "F-15E Strike Eagle", typeof(BackgroundImageRenderer), HeliosControlFlags.None)]
    class ClockPilot : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Clock (Pilot)";
        private ClockGauge _display;
        private const string REFLECTION_IMAGE = "{A-10C}/Images/A-10C/Pilot_Reflection_25a.png";
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.30d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;
        private HeliosPanel _frameGlassPanel;

        public ClockPilot()
            : base("Clock (Pilot)", new Size(300, 300))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.F15E.F15EInterface) };
            //AddButton("Button", new Point(346, 19), new Size(80, 80), _interfaceDeviceName, "Button");
            //AddEncoder("Knob", new Point(11,351), new Size(120, 120), _interfaceDeviceName, "Knob", "Knob");
            AddGauge("Clock", new Point(0d, 0d), new Size(300d, 300d), _interfaceDeviceName, "Clock");
            _frameGlassPanel = AddPanel("Gauge Glass", new Point(0d, 0d), new Size(300d, 300d), REFLECTION_IMAGE, _interfaceDeviceName);
            _frameGlassPanel.Opacity = GLASS_REFLECTION_OPACITY_DEFAULT;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;
        }
        private void AddGauge(string name, Point pos, Size size, string interfaceDevice, string interfaceElement)
        {
            _display = new ClockGauge(name, new Size(300, 300), _interfaceDeviceName, new string[3] { "Clock Hours", "Clock Minutes", "Clock Seconds" })
            {
                Top = pos.Y,
                Left = pos.X,
                Height = size.Height,
                Width = size.Width,
                Name = $"{_interfaceDeviceName}_{name}"
            };
            _display.IsHidden = false;

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
        }

        protected HeliosPanel AddPanel(string name, Point posn, Size size, string background, string interfaceDevice)
        {
            HeliosPanel panel = AddPanel
                (
                name: name,
                posn: posn,
                size: size,
                background: background
                );
            // in this instance, we want to all the panels to be hide-able so the actions need to be added
            IBindingAction panelAction = panel.Actions["toggle.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
                //string addedKey = Actions.GetKeyForItem(panelAction);
            }
            panelAction = panel.Actions["set.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
                //string addedKey = Actions.GetKeyForItem(panelAction);
            }
            return panel;
        }
        //private void AddButton(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, string imageModifier = "")
        //{
        //    imageModifier = imageModifier == "" ? "Clock" : imageModifier;
        //    AddButton(
        //        name: name,
        //        posn: posn,
        //        size: size,
        //        image: $"{_imageLocation}{imageModifier} Button Unpushed.png",
        //        pushedImage: $"{_imageLocation}{imageModifier} Button Pushed.png",
        //        buttonText: "",
        //        interfaceDeviceName: interfaceDeviceName,
        //        interfaceElementName: interfaceElementName,
        //        fromCenter: false
        //        );
        //}

        //private void AddEncoder(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, string knobName = "")
        //{
        //    AddEncoder(
        //        name: name,
        //        size: size,
        //        posn: posn,
        //        knobImage: $"{_imageLocation}Clock {knobName}.png",
        //        stepValue: 0.5,
        //        rotationStep: 5,
        //        interfaceDeviceName: _interfaceDeviceName,
        //        interfaceElementName: interfaceElementName,
        //        fromCenter: false,
        //        clickType: RotaryClickType.Swipe
        //        );
        //}

        #region properties
        public double GlassReflectionOpacity
        {
            get
            {
                return _glassReflectionOpacity;
            }
            set
            {
                double oldValue = _glassReflectionOpacity;
                if (value != oldValue)
                {
                    _glassReflectionOpacity = value;
                    OnPropertyChanged("GlassReflectionOpacity", oldValue, value, true);
                    _frameGlassPanel.IsHidden = value == 0d ? true : false;
                    _frameGlassPanel.Opacity = _glassReflectionOpacity;
                }
            }
        }
        #endregion

        public override bool HitTest(Point location)
        {

            return true;  // nothing to press on the fuel so return false.
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
        public override string DefaultBackgroundImage
        {
            get { return ""; }
        }
        protected override void OnBackgroundImageChange()
        {
            //_bezel.BackgroundImage = BackgroundImageIsCustomized ? null : System.IO.Path.Combine(_imageLocation, PANEL_IMAGE);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (_glassReflectionOpacity > 0d)
            {
                writer.WriteElementString("GlassReflectionOpacity", GlassReflectionOpacity.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            GlassReflectionOpacity = reader.Name.Equals("GlassReflectionOpacity") ? double.Parse(reader.ReadElementString("GlassReflectionOpacity"), CultureInfo.InvariantCulture) : 0d;
        }
    }
}

