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

namespace GadrocsWorkshop.Helios.Gauges.M2000C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using NLog;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.NetworkInformation;
    using System.Windows;
    using System.Windows.Media;
    using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

    [HeliosControl("HELIOS.M2000C.VTB_PANEL", "VTB Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_VTBPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(51, 9, 434, 420);
        private string _interfaceDeviceName = "HUD/VTB";
        private Rect _scaledScreenRect = SCREEN_RECT;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public M2000C_VTBPanel()
            : base("VTB Panel", new Size(531, 586))
        {
            int row1 = 117, row2 = 201, row3 = 284, row4 = 365;
            int column1 = 15, column2 = 500;
            Size VTB_switch_size = new Size(20, 40);

            //Brightness switches and indicators
            AddRotarySwitch("Markers Brightness", new Point(222, 473), new Size(46, 90), 8);
            AddRotarySwitch("Main Brightness", new Point(290, 473), new Size(46, 90), 8);
            AddRotarySwitch("Video Brightness", new Point(359, 473), new Size(46, 90), 8);
            AddRotarySwitch("Cavalier Brightness", new Point(428, 473), new Size(46, 90), 8);
            AddDrum("Markers Brightness Indicator", "(0-7)", new Point(222, 506), new Size(10d, 15d), new Size(16d, 24d));
            AddDrum("Main Brightness Indicator", "(0-7)", new Point(290, 506), new Size(10d, 15d), new Size(16d, 24d));
            AddDrum("Video Brightness Indicator", "(0-7)", new Point(359, 506), new Size(10d, 15d), new Size(16d, 24d));
            AddDrum("Cavalier Brightness Indicator", "(0-7)", new Point(428, 506), new Size(10d, 15d), new Size(16d, 24d));
            


            //Left Switches
            Add3PosnToggle("Target Data Manual Entry Begin/End", new Point(column1, row1), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Data Manual Entry Begin/End", false, false);
            Add3PosnToggle("Bullseye Waypoint Selector N",         new Point(column1, row2), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Bullseye Waypoint Selector N", false, false);
            Add3PosnToggle("Target Range from Bullseye Rho",         new Point(column1, row3), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Range from Bullseye Rho", false, false);
            Add3PosnToggle("Target Bearing from Bullseye Theta",       new Point(column1, row4), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Bearing from Bullseye Theta", false, false);

            //Right Switches
            Add3PosnToggle("Target Heading C",     new Point(column2, row1), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Heading C", false, false);
            Add3PosnToggle("Target Altitude Z",    new Point(column2, row2), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Altitude Z", false, false);
            Add3PosnToggle("Target Mach Number M", new Point(column2, row3), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Mach Number M", false, false);
            Add3PosnToggle("Target Age T",         new Point(column2, row4), VTB_switch_size, "{M2000C}/Images/Switches/black-circle-", ThreeWayToggleSwitchType.MomOnMom,
                ThreeWayToggleSwitchPosition.Two, _interfaceDeviceName, "Target Age T", false, false);

            //Bottom Switches
            AddSwitch("VTB Declutter", new Point(76, 501), ToggleSwitchPosition.Two, ToggleSwitchType.OnMom);
            AddSwitch("VTB Map Forward/Centered", new Point(134, 501), ToggleSwitchPosition.One, ToggleSwitchType.OnOn);
            AddSwitch("VTB Power Switch", new Point(500, 452), ToggleSwitchPosition.Two, ToggleSwitchType.OnOn);

        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/VTBPanel/VTB-panel.png"; }
        }

        #endregion

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                double scaleX = Width / NativeSize.Width;
                double scaleY = Height / NativeSize.Height;
                _scaledScreenRect.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }
 
        private void AddSwitch(string name, Point posn, ToggleSwitchPosition defaultPosition, ToggleSwitchType defaultType)
        {
            ToggleSwitch tswitch = AddToggleSwitch(name,posn,new Size(20, 40),defaultPosition,"{M2000C}/Images/Switches/short-black-up.png","{M2000C}/Images/Switches/short-black-down.png",defaultType,_interfaceDeviceName,name,false,null,false,false);
            tswitch.ClickType = LinearClickType.Swipe;
        }

        private void Add3PosnToggle(string name, Point posn, Size size, string image, ThreeWayToggleSwitchType switchType, ThreeWayToggleSwitchPosition defaultPosition,
            string interfaceDevice, string interfaceElement, bool fromCenter, bool horizontal)
        {
            AddThreeWayToggle(
                name: name,
                pos: posn,
                size: size,
                positionOneImage:   image + "up.png",
                positionTwoImage:   image + "mid.png",
                positionThreeImage: image + "down.png",
                defaultPosition: defaultPosition,
                switchType: switchType,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                horizontal: horizontal,
                horizontalRender: horizontal,
                clickType: LinearClickType.Swipe,
                fromCenter: false
                );
        }

        private void AddRotarySwitch(string name, Point posn, Size size, int positions)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: "{M2000C}/Images/Miscellaneous/void.png",
                //knobImage: "{M2000C}/Images/Switches/short-black-up.png",
                defaultPosition: 0,
                clickType: RotaryClickType.Swipe,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: false);
            rSwitch.Positions.Clear();
            for (int i = 0; i < positions; i++)
            {
                rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, i, i.ToString(), i));
            }
            AddDefaultInputBinding(
                childName: $"{Name}_{name} Indicator",
                interfaceTriggerName: $"HUD/VTB.{name}.changed",
                deviceActionName: $"set.{name} Indicator");
        }

        private void AddDrum(string name, string valueDescription, Point posn, Size size, Size renderSize)
        {
            Mk2CDrumGauge.Mk2CDrumGauge newGauge = new Mk2CDrumGauge.Mk2CDrumGauge($"{Name}_{name}", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", name, valueDescription, "#", posn, size, renderSize, 1d, -1d, 7d);
            Children.Add(newGauge);
            foreach (IBindingTrigger trigger in newGauge.Triggers)
            {
                AddTrigger(trigger, $"{Name}_{name}");
            }
            foreach (IBindingAction action in newGauge.Actions)
            {
                AddAction(action, $"{Name}_{name}");
            }
            try
            {
                /// This is an internal binding within the gauge as opposed to a binding to the default interface
                /// and it is required because the data for the drum is not passed explicity over tbe interface.
                InputBindings.Add(CreateNewBinding(Children[$"{Name}_{name.Replace(" Indicator", "")}"].Triggers["position.changed"], newGauge.Actions[$"set.{name}"]));
            }
            catch
            {
                Logger.Error($"Unable to create self-binding for gauge {Name}_{name.Replace(" Indicator", "")} trigger: {name.Replace(" Indicator", "")} \"position.changed\" action: {newGauge.Name} \"set.{name}\" ");
            }
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
