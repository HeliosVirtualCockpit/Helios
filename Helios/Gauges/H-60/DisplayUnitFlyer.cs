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

namespace GadrocsWorkshop.Helios.Gauges.H60.Instruments.DisplayUnit
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System.Windows;


    [HeliosControl("Helios.H60.FlyerDisplayUnit", "Flyer Display Unit", "UH-60L", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class FlyerDisplayUnit : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Engine Management";

        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;


        public FlyerDisplayUnit(FLYER flyer)
            : base($"{flyer} Display Unit", new Size(492, 840))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.H60.UH60L.UH60LInterface), typeof(Interfaces.DCS.H60.MH60R.MH60RInterface) };
            _interfaceDeviceName = $"Engine Management ({flyer})";
            AddBarGauge("RPM Engine 1", new Point(59, 50), new Size(40, 738), 41d, "{Helios}/Images/H-60/SegmentBarDisplay41RPM.xaml", _interfaceDeviceName, "Engine 1 RPM");
            AddBarGauge("RPM Rotor", new Point(119, 50), new Size(40, 738), 41d, "{Helios}/Images/H-60/SegmentBarDisplay41RPM.xaml", _interfaceDeviceName, "Rotor RPM");
            AddBarGauge("RPM Engine 2", new Point(179, 50), new Size(40, 738), 41d, "{Helios}/Images/H-60/SegmentBarDisplay41RPM.xaml", _interfaceDeviceName, "Engine 2 RPM");
            AddBarGauge("Torque Engine 1", new Point(358, 50), new Size(20, 540), 30d, "{Helios}/Images/H-60/SegmentBarDisplay30Torque.xaml", _interfaceDeviceName, "Engine 1 Torque");
            AddBarGauge("Torque Engine 2", new Point(409, 50), new Size(20, 540), 30d, "{Helios}/Images/H-60/SegmentBarDisplay30Torque.xaml", _interfaceDeviceName, "Engine 2 Torque");
        }
        private void AddBarGauge(string name, Point posn, Size size, double segmentCount, string imageName, string interfaceDevice, string interfaceElement)
        {
            BarGauge barGauge = new BarGauge($"{Name}_{name}", size, imageName, segmentCount)
            {
                Top = posn.Y,
                Left = posn.X
            };
            Children.Add(barGauge);
            foreach (IBindingAction action in barGauge.Actions)
            {
                if (action.Name != "hidden")
                {
                    AddAction(action, $"{name}");
                    //Create the automatic input bindings for the sub component
                    AddDefaultInputBinding(
                       childName: barGauge.Name,
                       deviceActionName: $"{barGauge.Name}.{action.ActionVerb}.{action.Name}",
                       interfaceTriggerName: $"{interfaceDevice}.{interfaceElement}.{action.Name}.changed"
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
            get { return "{helios}/Images/H-60/FlyerDisplayUnitBackground.xaml"; }
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
