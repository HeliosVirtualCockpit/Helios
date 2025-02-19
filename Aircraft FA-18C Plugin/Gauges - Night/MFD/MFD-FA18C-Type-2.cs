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

// ReSharper disable once CheckNamespace

using GadrocsWorkshop.Helios.Controls;

namespace GadrocsWorkshop.Helios.Gauges.FA_18C.MFD
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Globalization;
    using System.Windows;

    [HeliosControl("FA18C.AMPCD2", "AMPCD - Night", "F/A-18C - Night", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class AMPCD_FA18C2 : Gauges.MFD
    {
        private static readonly Rect SCREEN_RECT = new Rect(88, 161, 551, 532);
        private Rect _scaledScreenRect = SCREEN_RECT;

        public AMPCD_FA18C2()
            : base("AMPCD", new Size(727, 746))
        {
            AddButton("OSB1", 39, 567, true);
            AddButton("OSB2", 39, 487, true);
            AddButton("OSB3", 39, 409, true);
            AddButton("OSB4", 39, 329, true);
            AddButton("OSB5", 39, 247, true);

            AddButton("OSB6", 186, 112, false);
            AddButton("OSB7", 266, 112, false);
            AddButton("OSB8", 346, 112, false);
            AddButton("OSB9", 424, 112, false);
            AddButton("OSB10", 506, 112, false);

            AddButton("OSB11", 645, 247, true);
            AddButton("OSB12", 645, 329, true);
            AddButton("OSB13", 645, 409, true);
            AddButton("OSB14", 645, 487, true);
            AddButton("OSB15", 645, 567, true);

            AddButton("OSB16", 506, 696, false);
            AddButton("OSB17", 424, 696, false);
            AddButton("OSB18", 346, 696, false);
            AddButton("OSB19", 266, 696, false);
            AddButton("OSB20", 186, 696, false);
            AddRocker("Day / Night", "MFD Rocker", "L", 90, 75);
            AddRocker("Symbols", "MFD Rocker", "R", 550, 75);
            AddRocker("Gain", "MFD Rocker", "V", 39, 650);
            AddRocker("Contrast", "MFD Rocker", "V", 645, 650);

            AddThreeWayToggle("Heading", 1, 51, new Size(50, 100));
            AddThreeWayToggle("Course", 622, 50, new Size(50, 100));

            AddKnob("Mode Knob",new Point(336,37),new Size(60,60));
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{FA-18C}/Images/AMPCD frame.png"; }
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

        private void AddButton(string name, double x, double y, bool horizontal)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = y;
            button.Left = x;
            button.Width = 42;
            button.Height = 42;
            //button.TextPushOffset = new System.Windows.Media.TranslateTransform(1,1);
            //button.Image = "{FA-18C}/Images/MFD Button 1 Up.png";
            //button.PushedImage = "{FA-18C}/Images/MFD Button 1 Dn.png";
            if (!horizontal)
            {
                button.Image = "{FA-18C}/Images/MFD Button 1 UpV.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button 1 DnV.png";
                //button.Rotation = HeliosVisualRotation.CCW;
            }
            else
            {
                button.Image = "{FA-18C}/Images/MFD Button 1 UpH.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button 1 DnH.png";
            }
            button.Name = name;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], name);
            AddTrigger(button.Triggers["released"], name);

            AddAction(button.Actions["push"], name);
            AddAction(button.Actions["release"], name);
            AddAction(button.Actions["set.physical state"], name);
        }
        private void AddKnob(string name, Point posn, Size size)
        {
            Helios.Controls.Potentiometer knob = new Helios.Controls.Potentiometer
            {
                Name = name,
                KnobImage = "{FA-18C}/Images/Common Knob.png",
                InitialRotation = 219,
                RotationTravel = 291,
                MinValue = 0,
                MaxValue = 1,
                InitialValue = 0,
                StepValue = 0.1,
                Top = posn.Y,
                Left = posn.X,
                Width = size.Width,
                Height = size.Height
            };

            Children.Add(knob);
            foreach (IBindingTrigger trigger in knob.Triggers)
            {
                AddTrigger(trigger, name);
            }
            AddAction(knob.Actions["set.value"], name);
        }

        private new void AddTrigger(IBindingTrigger trigger, string device)
        {
            trigger.Device = device;
            Triggers.Add(trigger);
        }

        private new void AddAction(IBindingAction action, string device)
        {
            action.Device = device;
            Actions.Add(action);
        }

        private void AddRocker(string name, string imagePrefix, string imageOrientation, double x, double y)
        {
            Helios.Controls.ThreeWayToggleSwitch rocker = new Helios.Controls.ThreeWayToggleSwitch();
            rocker.Name = name;
            rocker.SwitchType = Helios.Controls.ThreeWayToggleSwitchType.MomOnMom;
            rocker.ClickType = Helios.Controls.LinearClickType.Touch;
            rocker.PositionTwoImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Mid.png";

            rocker.Top = y;
            rocker.Left = x;
            switch (imageOrientation)
            {
                case ("V"):
                    //rocker.Orientation = Helios.Controls.ToggleSwitchOrientation.Horizontal;
                    rocker.PositionOneImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    rocker.Height = 84;
                    rocker.Width = 40;
                    break;
                case ("L"):
                    rocker.PositionOneImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    rocker.Width = 86;
                    rocker.Height = 71;
                    break;
                case ("R"):
                    rocker.PositionOneImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Up.png";
                    rocker.PositionThreeImage = "{FA-18C}/Images/" + imagePrefix + " " + imageOrientation + " Dn.png";
                    rocker.Width = 86;
                    rocker.Height = 71;
                    break;
                default:
                    break;
            }

            Children.Add(rocker);
            foreach (IBindingTrigger trigger in rocker.Triggers)
            {
                AddTrigger(trigger, name);
            }

            AddAction(rocker.Actions["set.position"], name);
        }

        private void AddThreeWayToggle(string name, double x, double y, Size size)
        {
            Helios.Controls.ThreeWayToggleSwitch toggle = new Helios.Controls.ThreeWayToggleSwitch();
            toggle.Top = y;
            toggle.Left = x;
            toggle.Width = size.Width;
            toggle.Height = size.Height;
            toggle.DefaultPosition = ThreeWayToggleSwitchPosition.Two;
            toggle.Orientation = ToggleSwitchOrientation.Vertical; // this seems to just control the swipe direction
            toggle.Rotation = HeliosVisualRotation.CW;
            toggle.PositionOneImage = "{Helios}/Images/Toggles/orange-round-up.png";
            toggle.PositionTwoImage = "{Helios}/Images/Toggles/orange-round-norm.png";
            toggle.PositionThreeImage = "{Helios}/Images/Toggles/orange-round-down.png";
            toggle.SwitchType = ThreeWayToggleSwitchType.MomOnMom;
            toggle.Name = name;
            Children.Add(toggle);
            foreach (IBindingTrigger trigger in toggle.Triggers)
            {
                AddTrigger(trigger, name);
            }
            foreach (IBindingAction action in toggle.Actions)
            {
                AddAction(action, name);
            }

            //AddTrigger(toggle.Triggers["pushed"], name);
            //AddTrigger(toggle.Triggers["released"], name);

            //AddAction(toggle.Actions["push"], name);
            //AddAction(toggle.Actions["release"], name);
            //AddAction(toggle.Actions["set.physical state"], name);
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
