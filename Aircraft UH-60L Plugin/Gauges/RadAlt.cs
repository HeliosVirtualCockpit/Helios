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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Gauges.UH60L.Instruments;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.UH60L.RadAlt", "RADAR Altimeter", "UH-60L Blackhawk", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    public class RadAlt : CompositeVisualWithBackgroundImage
    {
        private string _interfaceDeviceName = "RADAR Alt (Pilot)";
        private string _font = "Helios Virtual Cockpit APN209 LED";
        private FontStyle _fontStyle = FontStyles.Normal;
        private FontWeight _fontWeight = FontWeights.Normal;

        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private RadAltInstrument _digitalAltitudeDisplay;
        private Controls.TextDecoration _digitalDisplayBackground;
        private NumericTextDisplay _digitalAltDisplay;
        private FLYER _flyer;

        public RadAlt( FLYER flyer, Size size)
            : base($"RADAR Altimeter ({flyer})", size)
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface) };
            _flyer = flyer;
            _interfaceDeviceName = $"RADAR Alt ({flyer})";
            AddLabel("Digital Altitude Background", new Point(63d, 239d), new Size(310d, 113d), 81, "\ufb01\ufb01\ufb01\ufb01", TextHorizontalAlignment.Right);
            AddNumericTextDisplay("Digital Altitude", new Point(63d, 239d), new Size(296d, 113d), _interfaceDeviceName, "Digital Altitude", 81, "8888", TextHorizontalAlignment.Right, "");
            AddPart("Instrument", new Point(0d, 0d), new Size(420d, 420d), _interfaceDeviceName, "Instrument");
        }
        private void AddNumericTextDisplay(string name, Point posn, Size size,
    string interfaceDevice, string interfaceElement, double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign, string devDictionary)
        {
            _digitalAltDisplay = new NumericTextDisplay()
            {
                Name = $"{name}",
                Width = size.Width,
                Height = size.Height,
                Top = posn.Y,
                Left = posn.X,
                ParserDictionary = devDictionary,
                UseParseDictionary = true,
                OnTextColor = Color.FromArgb(0xff, 0xa9, 0xed, 0x07),               
                ScalingMode = TextScalingMode.Height,
                UseBackground = false,
                TextTestValue = testDisp,
                Unit = BindingValueUnits.Feet,
                TextFormat = new TextFormat
                {
                    FontFamily = ConfigManager.FontManager.GetFontFamilyByName(_font),
                    FontStyle = _fontStyle,
                    FontWeight = _fontWeight,
                    HorizontalAlignment = hTextAlign,
                    VerticalAlignment = TextVerticalAlignment.Center,
                    FontSize = baseFontsize,
                    ConfiguredFontSize = baseFontsize,
                    PaddingRight = 0.006,
                    PaddingLeft = 0,
                    PaddingTop = 0,
                    PaddingBottom = 0
                }
            };
            _digitalAltDisplay.IsHidden = false;
            Children.Add(_digitalAltDisplay);
            foreach (IBindingAction action in _digitalAltDisplay.Actions)
            {
                if (action.Name != "hidden")
                {

                    AddAction(action, name);
                    //Create the automatic input bindings for the sub component
                    AddDefaultInputBinding(
                       childName: _digitalAltDisplay.Name,
                       deviceActionName: action.ActionVerb + "." + action.Name,
                       interfaceTriggerName: $"{interfaceDevice}.{name}.changed"
                       );
                }
            }
        }
        private void AddLabel(string name, Point posn, Size size,
     double baseFontsize, string testDisp, TextHorizontalAlignment hTextAlign)
        {
            _digitalDisplayBackground = new Controls.TextDecoration() { 
                Name = name,
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
                    FontFamily= ConfigManager.FontManager.GetFontFamilyByName(_font),
                    FontStyle =  _fontStyle,
                    FontWeight = _fontWeight,
                    HorizontalAlignment = hTextAlign,
                    VerticalAlignment = TextVerticalAlignment.Center,
                    FontSize = baseFontsize,
                    ConfiguredFontSize = baseFontsize,
                    PaddingRight = 0.006,
                    PaddingLeft = 0,
                    PaddingTop = 0,
                    PaddingBottom = 0
                },
                IsHidden = false,
            };
            Children.Add(_digitalDisplayBackground);
        }
        private void AddPart(string name, Point pos, Size size, string interfaceDevice, string interfaceElement)
        {
            _digitalAltitudeDisplay = new RadAltInstrument(name, size, this, _flyer)
            {
                Top = pos.Y,
                Left = pos.X,
            };
            Children.Add(_digitalAltitudeDisplay);
            // Note:  we have the actions against the new RadAltGauge but to expose those
            // actions in the interface, we copy the actions to the Parent.  This is a new 
            // HeliosActionCollection with the keys equal to the new ActionIDs, however the original
            // HeliosActionCollection which is on the child part will have the original keys, even though
            // we might have changed the values of the ActionIDs.  This has the result that autobinding
            // in CompositeVisual (OnProfileChanged) might not be able to find the actions when doing
            // the "ContainsKey()" for the action.
            // This is why the _display.Name is in the deviceActionName of the AddDefaultInputBinding
            // and *MUST* match the BindingValue device parameter for the gauge being added.

            //foreach (IBindingTrigger trigger in _display.Triggers)
            //{
            //    AddTrigger(trigger, trigger.Name);
            //}
            foreach (IBindingAction action in _digitalAltitudeDisplay.Actions)
            {
                if(action.Name != "hidden")
                {

                AddAction(action, _digitalAltitudeDisplay.Name);
                //Create the automatic input bindings for the sub component
                AddDefaultInputBinding(
                   childName: _digitalAltitudeDisplay.Name,
                   deviceActionName: _digitalAltitudeDisplay.Name +"." + action.ActionVerb + "." + action.Name,
                   interfaceTriggerName: interfaceDevice + "." + action.Name + ".changed"
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
            get { return null; }
        }
        internal bool TextVisibility { get => !_digitalAltDisplay.IsHidden; set => _digitalAltDisplay.IsHidden = !value; } 

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
