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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Chronograph
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    //using System.Drawing;
    using System.Windows;
    using System.Windows.Media;

    public class ChronographDisplay : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "";
        private string _font = "Helios Virtual Cockpit A-10C_Digital_Clock";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private FLYER _flyer;
        public ChronographDisplay(FLYER flyer)
            : base($"Chronograph Display ({flyer})", new Size(80d, 32d))
        {
            _flyer = flyer;
            _interfaceDeviceName = $"Chronometer ({flyer})";
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface) };
            AddTextDisplay("Time HH:MM", new Point(0d, 0d), new Size(60d, 32d), _interfaceDeviceName, "Time hh:mm", 20, "88:88", TextHorizontalAlignment.Left, "!=:");
            AddTextDisplay("Time ss", new Point(60d, 0d), new Size(20d, 32d), _interfaceDeviceName, "Time ss", 14, "88", TextHorizontalAlignment.Left, "!=:");
        }
        private void AddTextDisplay(string name, Point posn, Size size,
    string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            Controls.TextDecoration displayBackground = new Controls.TextDecoration()
            {
                Name = $"{name}_background",
                Width = size.Width,
                Height = size.Height,
                Top = posn.Y,
                Left = posn.X,
                Text = testDisp,
                ScalingMode = TextScalingMode.Height,
                FontColor = Color.FromArgb(0x20, 0xa9, 0xed, 0x07),
                FillBackground = true,
                BackgroundColor = Color.FromArgb(0xff, 0x04, 0x2a, 0x00),
                Format = new TextFormat
                {
                    FontFamily = ConfigManager.FontManager.GetFontFamilyByName(_font),
                    FontStyle = FontStyles.Normal,
                    FontWeight = FontWeights.Normal,
                    HorizontalAlignment = hTextAlign,
                    VerticalAlignment = TextVerticalAlignment.Center,
                    FontSize = baseFontsize,
                    ConfiguredFontSize = baseFontsize,
                    PaddingRight = 0,
                    PaddingLeft = 0,
                    PaddingTop = 0,
                    PaddingBottom = 0
                },
                IsHidden = false,
            };
            Children.Add(displayBackground);

            TextDisplay display = AddTextDisplay(
                name: $"{name}",
                posn: posn,
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xff, 0xa3, 0x8d, 0x36),
                backgroundColor: Color.FromArgb(0xff, 0x10, 0x13, 0x17),
                useBackground: false,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
            display.ScalingMode = TextScalingMode.Height;
            Children.Add(display);
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
