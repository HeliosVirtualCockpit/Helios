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

using System;
using System.Windows;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;

// ReSharper disable once CheckNamespace
namespace GadrocsWorkshop.Helios.Gauges.FA_18C.MFD
{
    [HeliosControl("FA18C.MPCD2", "MPCD - Night", "F/A-18C - Night", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class MPCD_FA18C2 : Gauges.MFD
    {
        private static readonly Rect SCREEN_RECT = new Rect(72, 137, 497, 493);
        private Rect _scaledScreenRect = SCREEN_RECT;

        public MPCD_FA18C2()
            : base("MPCD", new Size(656, 706))
        {
            AddButton("OSB1", 14, 540, true);
            AddButton("OSB2", 14, 455, true);
            AddButton("OSB3", 14, 370, true);
            AddButton("OSB4", 14, 285, true);
            AddButton("OSB5", 14, 200, true);

            AddButton("OSB6", 150, 74, false);
            AddButton("OSB7", 230, 74, false);
            AddButton("OSB8", 310, 74, false);
            AddButton("OSB9", 390, 74, false);
            AddButton("OSB10", 470, 74, false);

            AddButton("OSB11", 604, 200, true);
            AddButton("OSB12", 604, 285, true);
            AddButton("OSB13", 604, 370, true);
            AddButton("OSB14", 604, 455, true);
            AddButton("OSB15", 604, 540, true);

            AddButton("OSB16", 470, 652, false);
            AddButton("OSB17", 390, 652, false);
            AddButton("OSB18", 310, 652, false);
            AddButton("OSB19", 230, 652, false);
            AddButton("OSB20", 150, 652, false);

            //AddKnob("Mode Knob", new Point(298, 14), new Size(50, 50));
            AddRotarySwitch("Mode Knob", new Point(298, 14), new Size(50, 50));
            AddKnob("Brightness Knob",  new Point(14, 632), new Size(50, 50),1);
            AddKnob("Contrast Knob",   new Point(592, 632), new Size(50, 50),1);
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{FA-18C}/Images/MPCD frame.png"; }
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
        private void AddRotarySwitch(string name, Point posn, Size size)
        {
            Helios.Controls.RotarySwitch _knob = new Helios.Controls.RotarySwitch();
            _knob.Name = name;
            _knob.KnobImage = "{FA-18C}/Images/Common Knob.png";
            _knob.DrawLabels = false;
            _knob.DrawLines = false;
            _knob.Positions.Clear();
            _knob.Positions.Add(new RotarySwitchPosition(_knob, 0, "Off", 270d));
            _knob.Positions.Add(new RotarySwitchPosition(_knob, 1, "Night", 330d));
            _knob.Positions.Add(new RotarySwitchPosition(_knob, 2, "Day", 30d));
            _knob.CurrentPosition = 1;
            _knob.DefaultPosition = 1;
            _knob.Top = posn.Y;
            _knob.Left = posn.X;
            _knob.Width = size.Width;
            _knob.Height = size.Height;

            Children.Add(_knob);
            AddTrigger(_knob.Triggers["position.changed"], name);
            AddAction(_knob.Actions["set.position"], name);
        }
        private void AddKnob(string name, Point posn, Size size) { AddKnob(name, posn, size, 0); }
        private void AddKnob(string name, Point posn, Size size, Int16 knobType)
        {
            Helios.Controls.Potentiometer _knob = new Helios.Controls.Potentiometer();
            _knob.Name = name;
            switch (knobType)
            {
                case 1:
                    _knob.KnobImage = "{FA-18C}/Images/MPCD Knob.png";
                    break;
                default:
                    _knob.KnobImage = "{FA-18C}/Images/Common Knob.png";
                    break;                
            }
            _knob.InitialRotation = 219;
            _knob.RotationTravel = 291;
            _knob.MinValue = 0;
            _knob.MaxValue = 1;
            _knob.InitialValue = 0;
            _knob.StepValue = 0.1;
            _knob.Top = posn.Y;
            _knob.Left = posn.X;
            _knob.Width = size.Width;
            _knob.Height = size.Height;

            Children.Add(_knob);
            foreach (IBindingTrigger trigger in _knob.Triggers)
            {
                AddTrigger(trigger, name);
            }
            foreach (IBindingAction action in _knob.Actions)
            {
                AddAction(action, name);
            }
        }

        private void AddButton(string name, double x, double y, bool horizontal)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = y;
            button.Left = x;
            button.Width = 40;
            button.Height = 40;
            //button.TextPushOffset = new System.Windows.Media.TranslateTransform(1,1);
            //button.Image = "{FA-18C}/Images/MFD Button Up.png";
            //button.PushedImage = "{FA-18C}/Images/MFD Button Dn.png";
            if (!horizontal)
            {
                button.Image = "{FA-18C}/Images/MFD Button UpV.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button DnV.png";
                //button.Rotation = HeliosVisualRotation.CCW;
            }
            else
            {
                button.Image = "{FA-18C}/Images/MFD Button UpH.png";
                button.PushedImage = "{FA-18C}/Images/MFD Button DnH.png";
            }

            button.Name = name;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], name);
            AddTrigger(button.Triggers["released"], name);

            AddAction(button.Actions["push"], name);
            AddAction(button.Actions["release"], name);
            AddAction(button.Actions["set.physical state"], name);
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
