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
    using GadrocsWorkshop.Helios.Gauges.AH64D.CMWS;

    [HeliosControl("Helios.F15E.FuelPanel", "Fuel Monitor Panel", "F-15E Strike Eagle", typeof(BackgroundImageRenderer),HeliosControlFlags.None)]
    class FuelMonitorPanel : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Fuel Monitor Panel";
        private string _font = "MS 33558";
        private Fuel_Gauge _display;

        public FuelMonitorPanel()
            : base("Fuel Monitor Panel", new Size(288,384))
        {
            AddGauge("Fuel Gauge", new Point(60d, 29d), new Size(164d, 164d), _interfaceDeviceName, "Fuel Gauge");
            AddDisplay("Total Tank display", new FiveDigitDisplay(), new Point(104, 174), new Size(77, 28), "Total Tank display");
            AddDisplay("Left Tank display", new FourDigitDisplay(), new Point(56, 241), new Size(56, 23), "Left Tank display");
            AddDisplay("Right Tank display", new FourDigitDisplay(), new Point(172, 241), new Size(56, 23), "Right Tank display");
            AddKnob("Fuel Totalizer Selector", new Point(77,269), new Size(125, 125), "Fuel Totalizer Selector");
            AddEncoder("Bingo Selection", new Point(233,45), new Size(65,65), "Bingo Selection");
            AddIndicator("Off Flag", 60, 173, new Size(19, 41), "Off Flag");


        }
        private void AddDisplay(string name, BaseGauge _gauge, Point posn, Size displaySize, string interfaceElementName)
        {
            AddDisplay(
                name: name,
                gauge: _gauge,
                posn: posn,
                size: displaySize,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName
                );
            _gauge.Name = $"{Name}_{name}";
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
                onImage: "{F-15E}/Images/Fuel_Quantity_Panel/Fuel_Quantity_Off_Flag.png",
                offImage: "{AV-8B}/Images/_transparent.png",
                onTextColor: System.Windows.Media.Color.FromArgb(0x00, 0xff, 0xff, 0xff),
                offTextColor: System.Windows.Media.Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                font: _font,
                vertical: _vertical,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                fromCenter: false
                );
            indicator.Text = "";
            indicator.Name = $"{Name}_{name}";
        }
        private void AddGauge(string name, Point pos, Size size, string interfaceDevice, string interfaceElement)
        {
            _display = new Fuel_Gauge
            {
                Top = pos.Y,
                Left = pos.X,
                Height = size.Height,
                Width = size.Width,
                Name = GetComponentName(name)
            };
            Children.Add(_display);
            // Note:  we have the actions against the new CMWSThreatDisplay but to expose those
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
    }
}
