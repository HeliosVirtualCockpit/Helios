//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Gauges.UH60L.Instruments;
    using System;
    //using System.Drawing;
    using System.Windows;
    using System.Windows.Forms.VisualStyles;
    using System.Windows.Forms;
    using System.Windows.Media;

    [HeliosControl("Helios.UH60L.Stabilator", "Stabilator Position Indicator", "UH-60L", typeof(BackgroundImageRenderer),HeliosControlFlags.None)]
    public class StabilatorPositionIndicator : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Stability Control";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private StabInstrument _stabInstrument;

        public StabilatorPositionIndicator()
            : base("Stabilator Position Indicator", new Size(276, 233))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface) };
            AddPart("Stabilator Position", new Point(0d, 0d), new Size(420d, 420d), _interfaceDeviceName, "Stabilator Position");
        }
        private void AddPart(string name, Point pos, Size size, string interfaceDeviceName, string interfaceElement)
        {
            _stabInstrument = new StabInstrument($"{name}", size)
            {
                Top = pos.Y,
                Left = pos.X,
            };
            Children.Add(_stabInstrument);
            // Note:  we have the actions against the new RadAltGauge but to expose those
            // actions in the interface, we copy the actions to the Parent.  This is a new 
            // HeliosActionCollection with the keys equal to the new ActionIDs, however the original
            // HeliosActionCollection which is on the child part will have the original keys, even though
            // we might have changed the values of the ActionIDs.  This has the result that autobinding
            // in CompositeVisual (OnProfileChanged) might not be able to find the actions when doing
            // the "ContainsKey()" for the action.
            // This is why the _display.Name is in the deviceActionName of the AddDefaultInputBinding
            // and *MUST* match the BindingValue device parameter for the gauge being added.

            foreach (IBindingAction action in _stabInstrument.Actions)
            {
                if (action.Name != "hidden")
                {

                    AddAction(action, _stabInstrument.Name);
                    //Create the automatic input bindings for the sub component
                    AddDefaultInputBinding(
                       childName: _stabInstrument.Name,
                       deviceActionName: $"{_stabInstrument.Name}.{action.ActionVerb}.{action.Name}",
                       interfaceTriggerName: interfaceDeviceName + "." + action.Name + ".changed"
                       );
                }
            }

        }
 
        private string ComponentName(string name)
        {
            return $"{Name}_{name}";
        }

        private new void AddTrigger(IBindingTrigger trigger, string name)
        {
            trigger.Device = ComponentName(name);
            if (!Triggers.ContainsKey(Triggers.GetKeyForItem(trigger))) Triggers.Add(trigger);

        }

        private new void AddAction(IBindingAction action, string name)
        {
            action.Device = ComponentName(name);
            if (!Actions.ContainsKey(Actions.GetKeyForItem(action))) Actions.Add(action);
        }

        public override string DefaultBackgroundImage
        {
            get { return null; }
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
