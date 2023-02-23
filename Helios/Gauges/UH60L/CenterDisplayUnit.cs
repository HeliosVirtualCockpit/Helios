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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments.CDU
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    //using System.Drawing;
    using System.Windows;
    using System.Windows.Forms.VisualStyles;
    using System.Windows.Forms;
    using System.Windows.Media;

    [HeliosControl("Helios.UH60L.CenterDisplayUnit", "Center Display Unit", "H-60", typeof(BackgroundImageRenderer),HeliosControlFlags.None)]
    public class CenterDisplayUnit : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Engine Management";

        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;

 
        public CenterDisplayUnit()
            : base("Center Display Unit", new Size(1250,712))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface), typeof(Interfaces.DCS.UH60L.MH60RInterface) };
            AddBarGauge("Fuel Quantity L", new Point(19, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30Fuel.xaml", _interfaceDeviceName, "Fuel Quantity Left");
            AddBarGauge("Fuel Quantity R", new Point(140, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30Fuel.xaml", _interfaceDeviceName, "Fuel Quantity Right");
            AddBarGauge("Transmission Temp Gauge", new Point(220, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30TransmissionTemp.xaml", _interfaceDeviceName, "Transmission Temperature");
            AddBarGauge("Transmission Pressure Gauge", new Point(319, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30TransmissionPressure.xaml", _interfaceDeviceName, "Transmission Pressure");
            AddBarGauge("Oil Temp 1", new Point(420, 145), new Size(40, 522), 29d, "{Helios}/Images/UH60L/SegmentBarDisplay29OilTemp.xaml", _interfaceDeviceName, "Engine 1 Oil Temperature");
            AddBarGauge("Oil Temp 2", new Point(540, 145), new Size(40, 522), 29d, "{Helios}/Images/UH60L/SegmentBarDisplay29OilTemp.xaml", _interfaceDeviceName, "Engine 2 Oil Temperature");
            AddBarGauge("Oil Pressure 1", new Point(619, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30OilPressure.xaml", _interfaceDeviceName, "Engine 1 Oil Pressure");
            AddBarGauge("Oil Pressure 2", new Point(740, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30OilPressure.xaml", _interfaceDeviceName, "Engine 2 Oil Pressure");
            AddBarGauge("Engine 1 TGT Temp", new Point(840, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30TGT.xaml", _interfaceDeviceName, "Engine 1 TGT");
            AddBarGauge("Engine 2 TGT Temp", new Point(960, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30TGT.xaml", _interfaceDeviceName, "Engine 2 TGT");
            AddBarGauge("Engine 1 Ng Speed", new Point(1060, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30Ng.xaml", _interfaceDeviceName, "Engine 1 Ng");
            AddBarGauge("Engine 2 Ng Speed", new Point(1180, 128), new Size(40, 540), 30d, "{Helios}/Images/UH60L/SegmentBarDisplay30Ng.xaml", _interfaceDeviceName, "Engine 2 Ng");
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
            get { return "{helios}/Images/UH60L/CenterDisplayUnitBackground.xaml"; }
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
