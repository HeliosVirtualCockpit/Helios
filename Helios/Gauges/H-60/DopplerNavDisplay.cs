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

namespace GadrocsWorkshop.Helios.Gauges.H60.ASN128B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.H60.ASN128B", "Doppler Nav Display", "H-60", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    public class DopplerNavDisplay : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "Doppler Nav";
        private string _font = "LED Counter 7";
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;

        public DopplerNavDisplay()
            : base("Doppler Nav Display", new Size(320d, 128d))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface), typeof(Interfaces.DCS.H60.UH60L.UH60LInterface), typeof(Interfaces.DCS.H60.MH60R.MH60RInterface) };
            AddTextDisplay("Display Line 1", new Point(0d, 0d), new Size(320d, 32d), _interfaceDeviceName, "Display Line 1", 24, "", TextHorizontalAlignment.Left, "!=:");
            AddTextDisplay("Display Line 2", new Point(0d, 32d), new Size(320d, 32d), _interfaceDeviceName, "Display Line 2", 24, "", TextHorizontalAlignment.Left, "!=:");
            AddTextDisplay("Display Line 3", new Point(0d, 64d), new Size(320d, 32d), _interfaceDeviceName, "Display Line 3", 24, "", TextHorizontalAlignment.Left, "!=:");
            AddTextDisplay("Display Line 4", new Point(0d, 96), new Size(320d, 32d), _interfaceDeviceName, "Display Line 4", 24, "", TextHorizontalAlignment.Left, "!=:");
        }
        private void AddTextDisplay(string name, Point posn, Size size,
    string interfaceDeviceName, string interfaceElementName, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                posn: posn,
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: hTextAlign,
                verticalAligment: TextVerticalAlignment.Center,
                testTextDisplay: testDisp,
                textColor: Color.FromArgb(0xf0, 0x40, 0xb3, 0x29),
                backgroundColor: Color.FromArgb(0xff, 0x10, 0x20, 0x10),
                useBackground: true,
                interfaceDeviceName: interfaceDeviceName,
                interfaceElementName: interfaceElementName,
                textDisplayDictionary: devDictionary
                );
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
