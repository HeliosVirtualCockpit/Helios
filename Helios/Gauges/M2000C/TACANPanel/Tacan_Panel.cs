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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.TACAN
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("HELIOS.M2000C.TACAN_PANEL", "TACAN Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.None)]
    class M2000C_TacanPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 256, 280);
        private string _interfaceDeviceName = "Tacan Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_TacanPanel()
            : base("TACAN Panel", new Size(350, 312))
        {
            AddRotarySwitch("X/Y Selector", new Point(83, 193), new Size(133, 133), "low-switch-tacan");
            AddRotarySwitch("Mode Selector", new Point(262, 191), new Size(133, 133), "low-switch-tacan");
            
            AddPot("Channel 10 Selector", new Point(83, 193), "up-switch-tacan",
                80d, 332d, 0.0d, 0.923d, 0.077d, 0.077d, false);
            AddPot("Channel 1 Selector", new Point(262, 190), "up-switch-tacan",
                70d, 324d, 0.0d, 0.9d, 0.6d, 0.1d, false);
            
            AddDrum("Channel output for display (Ones)", "{Helios}/Gauges/M2000C/TACANPanel/drum_tape.xaml", "ones frequency", "(0 - 9)", "#", 
                new Point(210, 60), new Size(10d, 15d), new Size(33d, 52d));
            AddDrum("Channel output for display (Tens)", "{Helios}/Gauges/M2000C/TACANPanel/drum_tape_twelve.xaml", "tens frequency", "(0 - 12)", "#", 
                new Point(144, 60), new Size(20d, 15d), new Size(66d, 52d), 14d);
            AddTacanDrum("X/Y Drum", new Point(118, 67), new Size(24d, 36d));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/TacanPanel/tacan-panel.png"; }
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
                knobImage: "{M2000C}/Images/TacanPanel/" + imagePrefix + ".png",
                initialRotation: initialRotation,
                rotationTravel: rotationTravel,
                minValue: minValue,
                maxValue: maxValue,
                initialValue: initialValue,
                stepValue: stepValue,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true,
                clickType: RotaryClickType.Touch,
                isContinuous: true);
        }

        private void AddRotarySwitch(string name, Point posn, Size size, string imagePrefix)
        {
            RotarySwitch rSwitch = AddRotarySwitch(name: name,
                posn: posn,
                size: size,
                knobImage: "{M2000C}/Images/TacanPanel/" + imagePrefix + ".png",
                defaultPosition: 0,
                clickType: RotaryClickType.Touch,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                fromCenter: true);
            rSwitch.Positions.Clear();
            switch(name)
            {
                case "X/Y Selector":
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "X", 0d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "Y", 45d));
                    break;
                case "Mode Selector":
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 1, "OFF", 215d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 2, "REC", 240d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 3, "T/R", 270d));
                    rSwitch.Positions.Add(new RotarySwitchPosition(rSwitch, 4, "A/A", 300d));
                    break;
            }
            foreach (RotarySwitchPosition position in rSwitch.Positions)
            {
                AddTrigger(rSwitch.Triggers["position " + position.Index + ".entered"], rSwitch.Name);
            }
            rSwitch.DefaultPosition = 1;
        }

        private void AddDrum(string name, string gaugeImage, string actionIdentifier, string valueDescription, string format, Point posn, Size size, Size renderSize, double drumDigits = 10d)
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
                fromCenter: false,
                drumDigits: drumDigits);
        }

        private void AddTacanDrum(string name, Point posn, Size size)
        {
            AddTacanDrum(name: name,
                posn: posn,
                size: size,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
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
