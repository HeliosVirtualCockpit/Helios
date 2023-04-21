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

namespace GadrocsWorkshop.Helios.Gauges.H60.Chronometer
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    //using System.Drawing;
    using System.Windows;
    using System.Windows.Media;

    public class ChronometerDisplay : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "";
        private string _font1 = "Helios Virtual Cockpit A-10C_Digital_Clock";
        private string _font2 = "DSEG14 Classic";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private HeliosPanel _panel;
        private FLYER _flyer;
        public ChronometerDisplay(FLYER flyer)
            : base($"Chronometer Display ({flyer})", new Size(322d, 322d))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface), typeof(Interfaces.DCS.H60.UH60L.UH60LInterface), typeof(Interfaces.DCS.H60.MH60R.MH60RInterface), typeof(Interfaces.DCS.Soft.SoftInterface) };
            SupportedInterfaceNames = new[] { "DCS H-60 (MH-60R Seahawk)", "DCS H-60 (UH-60L Blackhawk)" };
            _flyer = flyer;
            _interfaceDeviceName = $"Chronometer ({flyer})";
            AddTextDisplay("Time HH:MM", new Point(44d, 91d), new Size(180d, 74d), _interfaceDeviceName, "Time hh:mm", 50, "88:88", TextHorizontalAlignment.Center, "!=:",_font1,true);
            AddTextDisplay("Time ss", new Point(222d, 91d), new Size(87d, 74d), _interfaceDeviceName, "Time ss", 40, "88", TextHorizontalAlignment.Left, "!=:", _font1, true);
            AddTextDisplay("Time Mode", new Point(44d, 144d), new Size(224d,24d), _interfaceDeviceName, "Time Mode", 12, "", TextHorizontalAlignment.Center, "!=:", _font2, true);
            _panel = AddPanel("Panel Visibility", new Point(0d, 0d), new Size(322d, 322d), "{Helios}/Images/H-60/ChronometerPanel.xaml", _interfaceDeviceName);
            _panel.Opacity = 1.0d;
            _panel.DrawBorder = false;
            _panel.FillBackground = false;
        }

        private void AddTextDisplay(string name, Point posn, Size size, string interfaceDeviceName, string interfaceElementName, 
            double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary,string font, bool background)
        {
            if(background)
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
                    FontColor = Color.FromArgb(0x3f, 0x93, 0x7d, 0x35),
                    FillBackground = true,
                    BackgroundColor = Color.FromArgb(0xff, 0x10, 0x13, 0x17),
                    Format = new TextFormat
                    {
                        FontFamily = ConfigManager.FontManager.GetFontFamilyByName(font),
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
            }

            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: font,
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
        }
        protected HeliosPanel AddPanel(string name, Point posn, Size size, string background, string interfaceDevice)
        {
            HeliosPanel panel = AddPanel
                (
                name: name,
                posn: posn,
                size: size,
                background: background
                );
            // in this instance, we want to all the panels to be hide-able so the actions need to be added
            IBindingAction panelAction = panel.Actions["toggle.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
                //string addedKey = Actions.GetKeyForItem(panelAction);
            }
            panelAction = panel.Actions["set.hidden"];
            panelAction.Device = $"{Name}_{name}";
            panelAction.Name = "hidden";
            if (!Actions.ContainsKey(panel.Actions.GetKeyForItem(panelAction)))
            {
                Actions.Add(panelAction);
                //string addedKey = Actions.GetKeyForItem(panelAction);
            }
            return panel;
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
