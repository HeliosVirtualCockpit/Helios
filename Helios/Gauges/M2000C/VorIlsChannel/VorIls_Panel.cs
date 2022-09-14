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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.VorIlsChannel
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("HELIOS.M2000C.VorIls_PANEL", "VOR/ILS Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer))]
    class M2000C_VorIlsPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 350, 314);
        private string _interfaceDeviceName = "VOR/ILS Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_VorIlsPanel()
            : base("VOR/ILS Panel", new Size(350, 314))
        {
            AddRotarySwitch("Power Selector", new Point(83, 241), new Size(153, 153), "low-switch-vor-ils");
            AddRotarySwitch("Mode Selector", new Point(255, 241), new Size(153, 153), "low-switch-vor-ils");

            AddPot("Frequency Change whole", new Point(83, 241), "up-switch-vor-ils",
                45d, 327d, 0.0d, 1d, 0.1d, 0.1d, true);
            AddPot("Frequency Change Decimal", new Point(255, 241), "up-switch-vor-ils",
                -65d, 342d, 0.0d, 1d, 0.1d, 0.05d, true);
          
            AddDrum("Channel output for display (Decimal Ones)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "decimal ones frequency", "(0 or 5)", "#",
                new Point(213, 67), new Size(10d, 15d), new Size(33d,60d));
            AddDrum("Channel output for display (Decimal Tens)", "{Helios}/Gauges/M2000C/Common/drum_tape_with_point.xaml", "decimal tens frequency", "(0 - 9)", "#",
                new Point(176, 67), new Size(11d, 15d), new Size(36d, 60d));
            AddDrum("Channel output for display (Ones)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "ones frequency", "(0 - 9)", "#",
                new Point(142, 67), new Size(10d, 15d), new Size(33d, 60d));
            AddDrum("Channel output for display (Tens)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "tens frequency", "(0 - 1)", "#",
                new Point(111, 67), new Size(10d, 15d), new Size(33d, 60d));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/VorIlsPanel/vor-ils-panel.png"; }
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

        private void AddPot(string name, Point posn, string imagePrefix, double initialRotation, double rotationTravel, double minValue, double maxValue,
            double initialValue, double stepValue, bool fromCenter)
        {
            AddPot(
                name: name,
                posn: posn,
                size: new Size(104, 104),
                knobImage: "{M2000C}/Images/VorIlsPanel/" + imagePrefix + ".png",
                initialRotation: initialRotation,
                rotationTravel: rotationTravel,
                minValue: minValue,
                maxValue: maxValue,
                initialValue: initialValue,
                stepValue: stepValue,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: fromCenter,
                clickType: RotaryClickType.Touch,
                isContinuous: true);
        }

        private void AddRotarySwitch(string name, Point posn, Size size, string imagePrefix)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: "{M2000C}/Images/VorIlsPanel/" + imagePrefix + ".png",
                defaultPosition: 0,
                clickType: RotaryClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true);
            rSwitch.Positions.Clear();
            switch(name)
            {
                case "Power Selector":
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "ON", -40d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "OFF", 40d));
                    break;
                case "Mode Selector":
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "HG", -40d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "BD", 40d));
                    break;
            }
            foreach (RotarySwitchPosition position in rSwitch.Positions)
            {
                AddTrigger(rSwitch.Triggers["position " + position.Index + ".entered"], rSwitch.Name);
            }
            rSwitch.DefaultPosition = 1;
        }

        private void AddDrum(string name, string gaugeImage, string actionIdentifier, string valueDescription, string format, Point posn, Size size, Size renderSize)
        {
            AddDrumGauge(name: name,
                gaugeImage: gaugeImage,
                posn: posn,
                size: size,
                renderSize: renderSize,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                actionIdentifier: actionIdentifier,
                valueDescription: valueDescription,
                format: format,
                fromCenter: false);
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
