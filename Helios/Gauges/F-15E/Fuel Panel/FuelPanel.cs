//  Copyright 2019 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E.FuelPanel
{
    using GadrocsWorkshop.Helios.Gauges.F15E;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Globalization;
    using System.Xml;

    [HeliosControl("Helios.F15E.FuelPanel", "Fuel Monitor Panel", "F-15E Strike Eagle", typeof(BackgroundImageRenderer),HeliosControlFlags.None)]
    class FuelMonitorPanel : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Fuel Monitor Panel";
        private string _font = "MS 33558";
        private Fuel_Gauge _display;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.30d;
        private double _glassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;
        private HeliosPanel _frameGlassPanel;

        public FuelMonitorPanel()
            : base("Fuel Monitor Panel", new Size(288,384))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.F15E.F15EInterface) };

            AddGauge("Fuel Gauge", new Point(60d, 29d), new Size(164d, 164d), _interfaceDeviceName, "Fuel Gauge");
            AddDisplay("Total Tank display", new FiveDigitDisplay("Total Tank display"), new Point(104, 174), new Size(77, 29), "Total Tank display");
            AddDisplay("Left Tank display", new FourDigitDisplay("Left Tank display"), new Point(56, 241), new Size(56, 24), "Left Tank display");
            AddDisplay("Right Tank display", new FourDigitDisplay("Right Tank display"), new Point(172, 241), new Size(56, 24), "Right Tank display");
            AddIndicator("Panel off flag", 61, 172, new Size(18, 43), "Panel off flag");
            _frameGlassPanel = AddPanel("Fuel Panel Glass", new Point(43, 12), new Size(199d, 253d), "{Helios}/Images/AH-64D/MFD/MFD_glass.png", _interfaceDeviceName);
            _frameGlassPanel.Opacity = GLASS_REFLECTION_OPACITY_DEFAULT;
            _frameGlassPanel.DrawBorder = false;
            _frameGlassPanel.FillBackground = false;
            AddKnob("Fuel Totalizer Selector", new Point(77, 269), new Size(125, 125), "Fuel Totalizer Selector");
            AddEncoder("Bingo Adjustment", new Point(222, 45), new Size(65, 65), "Bingo Adjustment");

        }
        private void AddDisplay(string name, BaseGauge gauge, Point posn, Size displaySize, string interfaceElementName)
        {
            AddDisplay(
                name: name,
                gauge: gauge,
                posn: posn,
                size: displaySize,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName
                );
            gauge.Name = $"{Name}_{name}";
        }
        private void AddKnob(string name, Point posn, Size size, string interfaceElementName)
        {

            RotarySwitch _knob = new RotarySwitch();
            _knob.Name = $"{Name}_{name}";
            _knob.KnobImage = "{F-15E}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Selector_Knob.png";
            _knob.DrawLabels = false;
            _knob.DrawLines = false;
            _knob.Positions.Clear();
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 0, "BIT", 232d));
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 1, "Feed", 270d));
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 2, "Int Wing", 305d));
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 3, "Tank 1", 340d));
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 4, "Ext Wing", 54d));
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 5, "Ext Center", 90d));
            _knob.Positions.Add(new Helios.Controls.RotarySwitchPosition(_knob, 6, "Conformal", 125d));
            _knob.CurrentPosition = 2;
            _knob.DefaultPosition = 2;
            _knob.Top = posn.Y;
            _knob.Left = posn.X;
            _knob.Width = size.Width;
            _knob.Height = size.Height;

            AddRotarySwitchBindings(name, posn, size, _knob, _interfaceDeviceName, interfaceElementName);
        }

        private void AddEncoder(string name, Point posn, Size size, string interfaceElementName)
        {
            AddEncoder(
                name: name,
                size: size,
                posn: posn,
                knobImage: "{F-15E}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Bingo_Knob.png",
                stepValue: 0.1,
                rotationStep: 5,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
        }
        private void AddIndicator(string name, double x, double y, Size size, string interfaceElementName) { AddIndicator(name, x, y, size, false, interfaceElementName); }
        private void AddIndicator(string name, double x, double y, Size size, bool _vertical, string interfaceElementName)
        {
            Indicator indicator = AddIndicator(
                name: name,
                posn: new Point(x, y),
                size: size,
                offImage: "{F-15E}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Off_Flag.png",
                onImage: "{F-15E}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Off_Flag_Off.png",
                onTextColor: System.Windows.Media.Color.FromArgb(0x00, 0xff, 0xff, 0xff),
                offTextColor: System.Windows.Media.Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                font: _font,
                vertical: _vertical,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            indicator.Text = "";
            //indicator.Name = $"{Name}_{name}";
        }
        private void AddGauge(string name, Point pos, Size size, string interfaceDevice, string interfaceElement)
        {
            _display = new Fuel_Gauge
            {
                Top = pos.Y,
                Left = pos.X,
                Height = size.Height,
                Width = size.Width,
                Name = $"{Name}_{name}"
            };
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
            get { return "{F-15E}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Panel.png"; }
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
