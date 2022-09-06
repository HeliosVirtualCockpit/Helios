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


/// <summary>
/// This has been deprecated in favour of UFC_1 which uses text displays and a higher res background image
/// </summary>

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.Gauges.AV8B;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Threading;


    [HeliosControl("Helios.AV8B.UFC", "Up Front Controller", "AV-8B Gauges", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    class UFC_AV8B : AV8BDevice
    {
        // these three sections are the dead space in the UFC image.
        private static readonly Rect SCREEN_RECT_L = new Rect(0, 135, 38, 415);
        private Rect _scaledScreenRectL = SCREEN_RECT_L;
        private static readonly Rect SCREEN_RECT_LB = new Rect(38, 476, 103, 74);
        private Rect _scaledScreenRectLB = SCREEN_RECT_LB;
        private static readonly Rect SCREEN_RECT_R = new Rect(743, 102, 65, 448);
        private Rect _scaledScreenRectR = SCREEN_RECT_R;
        private String _font = "MS 33558";

        public UFC_AV8B()
            : base("Up Front Controller", new Size(808, 550))
        {

            AddButton("TMR", 190, 54, new Size(60, 60));
            AddButton("TOO", 190, 126, new Size(60, 60));
            AddButton("1", 266, 126, new Size(60, 60));
            AddButton("2", 337, 126, new Size(60, 60));
            AddButton("3", 410, 126, new Size(60, 60));
            AddButton("CLR", 482, 126, new Size(60, 60));
            AddButton("4", 266, 196, new Size(60, 60));
            AddButton("5", 337, 196, new Size(60, 60));
            AddButton("6", 410, 196, new Size(60, 60));
            AddButton("SVE", 482, 196, new Size(60, 60));
            AddButton("7", 266, 267, new Size(60, 60));
            AddButton("8", 337, 267, new Size(60, 60));
            AddButton("9", 410, 267, new Size(60, 60));
            AddButton("Dash", 482, 267, new Size(60, 60));
            AddButton("ENT", 266, 337, new Size(60, 60));
            AddButton("0", 337, 337, new Size(60, 60));
            AddButton("Dot", 410, 337, new Size(60, 60));
            AddButton("ONOFF", 482, 337, new Size(60, 60));
            AddButton("IFF", 266, 407, new Size(60, 60));
            AddButton("TCN", 337, 407, new Size(60, 60));
            AddButton("AWL", 410, 407, new Size(60, 60));
            AddButton("WPN", 482, 407, new Size(60, 60));
            AddButton("WOF", 266, 477, new Size(60, 60));
            AddButton("BCN", 337, 477, new Size(60, 60));
            AddButton("ALT", 410, 477, new Size(60, 60));
            AddButton("EMCON", 482, 477, new Size(60, 60));
            AddButtonIP("IP", 566, 129, new Size(50, 50));

            AddIndicator("15 SEC", 50, 231, new Size(60, 31));
            AddIndicator("MFS", 50, 272, new Size(60, 31));
            AddIndicator("BINGO", 50, 313, new Size(60, 31));
            AddIndicator("H2O", 50, 354, new Size(60, 31));
            AddIndicator("Unknown 1", 50, 394, new Size(60, 31));
            AddIndicator("Unknown 2", 50, 433, new Size(60, 31));
            AddIndicator("L FUEL", 48, 145, new Size(32, 73));
            AddIndicator("R FUEL", 90, 145, new Size(32, 73));
            AddIndicator("FIRE", 678, 108, new Size(60, 31));
            AddIndicator("LAW", 678, 149, new Size(60, 31));
            AddIndicator("FLAPS", 678, 190, new Size(60, 31));
            AddIndicator("L TANK", 676, 231, new Size(60, 31), true);
            AddIndicator("R TANK", 676, 272, new Size(60, 31), true);
            AddIndicator("HYD", 675, 314, new Size(60, 31));
            AddIndicator("GEAR", 675, 354, new Size(60, 31));
            AddIndicator("OT", 674, 394, new Size(60, 31));
            AddIndicator("JPTL", 674, 435, new Size(60, 31));
            AddIndicator("EFC", 673, 475, new Size(60, 31));
            AddIndicator("GEN", 673, 514, new Size(60, 31));
            AddIndicatorPushButton("MASTER CAUTION", 9, 9, new Size(110, 80));
            AddIndicatorPushButton("MASTER WARNING", 687, 9, new Size(110, 80));

            AddPot("UFC Display Brightness", new Point(564, 42), new Size(50, 50));
            AddPot("Radio Volume 1", new Point(196, 210), new Size(50, 50));
            AddPot("Radio Volume 2", new Point(570, 210), new Size(50, 50));
            AddEncoder("Radio 1", new Point(160, 405), new Size(100, 100));
            AddEncoder("Radio 2", new Point(557, 405), new Size(100, 100));
            AddButtonComm("Comm 1", new Point(190, 435), new Size(40, 40));
            AddButtonComm("Comm 2", new Point(587, 435), new Size(40, 40));

        }

        public override string DefaultBackgroundImage
        {
            get { return "{AV-8B}/Images/AV-8B UFC 1080.png"; }
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
        private void AddPot(string name, Point posn, Size size)
        {
            Helios.Controls.Potentiometer _knob = new Helios.Controls.Potentiometer();
            _knob.Name = name;
            _knob.KnobImage = "{AV-8B}/Images/Common Knob.png";
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
            AddAction(_knob.Actions["set.value"], name);
        }
        private void AddEncoder(string name, Point posn, Size size)
        {
            //Helios.Controls.RotaryEncoderPushable _knob = new Helios.Controls.RotaryEncoderPushable();
            Helios.Controls.RotaryEncoder _knob = new Helios.Controls.RotaryEncoder();
            _knob.Name = name;
            _knob.KnobImage = "{AV-8B}/Images/AV8BNA_Rotary5.png";
            _knob.StepValue = 0.1;
            _knob.RotationStep = 5;
            _knob.Top = posn.Y;
            _knob.Left = posn.X;
            _knob.Width = size.Width;
            _knob.Height = size.Height;

            Children.Add(_knob);
            foreach (IBindingTrigger trigger in _knob.Triggers)
            {
                AddTrigger(trigger, name);
            }
            //AddAction(_knob.Actions["push"], name);
            //AddAction(_knob.Actions["release"], name);
        }
        private void AddButton(string name, double x, double y) { AddButton(name, x, y, false); }
        private void AddButton(string name, double x, double y, Size size) { AddButton(name, x, y, size, false); }
        private void AddButton(string name, double x, double y, bool horizontal) { AddButton(name, x, y, new Size(40, 40), false); }
        private void AddButton(string name, double x, double y, Size size, bool horizontal) { AddButton(name, x, y, size, horizontal, false); }
        private void AddButton(string name, double x, double y, Size size, bool horizontal, bool altImage)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = y;
            button.Left = x;
            button.Width = size.Width;
            button.Height = size.Height;
            button.Image = "{AV-8B}/Images/UFC Button Up " + name + ".png";
            button.PushedImage = "{AV-8B}/Images/UFC Button Dn " + name + ".png";
            button.Text = "";
            button.Name = "UFC Key " + name;

            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], "UFC Key " + name);
            AddTrigger(button.Triggers["released"], "UFC Key " + name);

            AddAction(button.Actions["push"], "UFC Key " + name);
            AddAction(button.Actions["release"], "UFC Key " + name);
            AddAction(button.Actions["set.physical state"], "UFC Key " + name);
        }
        private void AddButtonIP(string name, double x, double y, Size size)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = y;
            button.Left = x;
            button.Width = size.Width;
            button.Height = size.Height;
            button.Image = "{Helios}/Images/Buttons/tactile-dark-round.png";
            button.PushedImage = "{Helios}/Images/Buttons/tactile-dark-round-in.png";
            button.Text = "";
            button.Name = "UFC Key " + name;
            button.Glyph = PushButtonGlyph.Circle;
            button.GlyphThickness = 3;
            button.GlyphColor = Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0);
            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], "UFC Key " + name);
            AddTrigger(button.Triggers["released"], "UFC Key " + name);

            AddAction(button.Actions["push"], "UFC Key " + name);
            AddAction(button.Actions["release"], "UFC Key " + name);
            AddAction(button.Actions["set.physical state"], "UFC Key " + name);
        }
        private void AddButtonComm(string name, Point posn, Size size)
        {
            Helios.Controls.PushButton button = new Helios.Controls.PushButton();
            button.Top = posn.Y;
            button.Left = posn.X;
            button.Width = size.Width;
            button.Height = size.Height;
            button.Image = "";
            button.PushedImage = "";
            button.Text = "";
            button.Name = "UFC " + name;
            button.Glyph = PushButtonGlyph.Circle;
            button.GlyphThickness = 3;
            button.GlyphColor = Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0);
            Children.Add(button);

            AddTrigger(button.Triggers["pushed"], "UFC " + name);
            AddTrigger(button.Triggers["released"], "UFC " + name);

            AddAction(button.Actions["push"], "UFC " + name);
            AddAction(button.Actions["release"], "UFC " + name);
            AddAction(button.Actions["set.physical state"], "UFC " + name);
        }

        private void AddIndicator(string name, double x, double y, Size size) { AddIndicator(name, x, y, size, false); }
        private void AddIndicator(string name, double x, double y, Size size, bool _vertical)
        {
            Helios.Controls.Indicator indicator = new Helios.Controls.Indicator();
            indicator.Top = y;
            indicator.Left = x;
            indicator.Width = size.Width;
            indicator.Height = size.Height;
            indicator.OnImage = "{Helios}/Images/Indicators/anunciator.png";
            indicator.OffImage = "{Helios}/Images/Indicators/anunciator.png";
            if (name == "Unknown 1" || name == "Unknown 2")
            {
                indicator.Text = ".";
            }
            else
            {
                indicator.Text = name;
            }
            indicator.Name = "Annunciator " + name;
            indicator.OnTextColor = Color.FromArgb(0xff, 0x94, 0xEB, 0xA6);
            indicator.OffTextColor = Color.FromArgb(0xff, 0x10, 0x10, 0x10);
            indicator.TextFormat.FontStyle = FontStyles.Normal;
            indicator.TextFormat.FontWeight = FontWeights.Normal;
            if (_vertical)
            {
                if (_font == "MS 33558")
                {
                    indicator.TextFormat.FontSize = 8;
                }
                else
                {
                    indicator.TextFormat.FontSize = 11;
                }
            }
            else
            {
                indicator.TextFormat.FontSize = 12;
            }
            indicator.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName(_font);  
            indicator.TextFormat.PaddingLeft = 0;
            indicator.TextFormat.PaddingRight = 0;
            indicator.TextFormat.PaddingTop = 0;
            indicator.TextFormat.PaddingBottom = 0;
            indicator.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
            indicator.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;

            Children.Add(indicator);
            foreach (IBindingTrigger trigger in indicator.Triggers)
            {
                AddTrigger(trigger, name);
            }
            foreach (IBindingAction action in indicator.Actions)
            {
                AddAction(action, name);
            }
        }
        private void AddIndicatorPushButton(string name, double x, double y, Size size)
        {
            Helios.Controls.IndicatorPushButton indicator = new Helios.Controls.IndicatorPushButton();
            indicator.Top = y;
            indicator.Left = x;
            indicator.Width = size.Width;
            indicator.Height = size.Height;
            indicator.Image = "{Helios}/Images/Indicators/indicator.png";
            indicator.PushedImage = "{Helios}/Images/Indicators/indicator-push.png";
            indicator.Text = name;
            indicator.Name = name;
            indicator.ScalingMode = TextScalingMode.Height;
            if (name == "MASTER WARNING")
            {
                indicator.OnTextColor = Color.FromArgb(0xff, 0xc7, 0x1e, 0x1e);
            }
            else
            {
                indicator.OnTextColor = Color.FromArgb(0xff, 0xb3, 0xa2, 0x29);
            }
            indicator.TextColor = Color.FromArgb(0xff, 0x1C, 0x1C, 0x1C);
            indicator.TextFormat.FontStyle = FontStyles.Normal;
            indicator.TextFormat.FontWeight = FontWeights.Normal;
            indicator.TextFormat.FontSize = 18;
            indicator.TextFormat.FontFamily = ConfigManager.FontManager.GetFontFamilyByName(_font);
            indicator.TextFormat.PaddingLeft = 0;
            indicator.TextFormat.PaddingRight = 0;
            indicator.TextFormat.PaddingTop = 0;
            indicator.TextFormat.PaddingBottom = 0;
            indicator.TextFormat.VerticalAlignment = TextVerticalAlignment.Center;
            indicator.TextFormat.HorizontalAlignment = TextHorizontalAlignment.Center;

            Children.Add(indicator);
            AddTrigger(indicator.Triggers["pushed"], name);
            AddTrigger(indicator.Triggers["released"], name);

            AddAction(indicator.Actions["push"], name);
            AddAction(indicator.Actions["release"], name);
            AddAction(indicator.Actions["set.indicator"], name);
        }

        public override bool HitTest(Point location)
        {
            if (_scaledScreenRectL.Contains(location) || _scaledScreenRectLB.Contains(location) || _scaledScreenRectR.Contains(location))
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