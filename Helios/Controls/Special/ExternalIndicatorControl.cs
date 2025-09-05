// Copyright 2020 Ammo Goettsch
// Copyright 2022 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// This control is to enable the control of LEDs on DirectX USB Devices.  It has now been 
    /// superseded by the interface changes for Warthog and Virpil.  Retained just in case someone
    /// is still using it, but is no longer appearing in the toolbox.
    /// </summary>
#if DEBUG
    [HeliosControl("Helios.Base.Special.ExternalIndicatorControl", "External Indicator Control", "Special Controls", typeof(ImageDecorationRenderer), HeliosControlFlags.None)]
#else
    [HeliosControl("Helios.Base.Special.ExternalIndicatorControl", "External Indicator Control", "Special Controls", typeof(ImageDecorationRenderer), HeliosControlFlags.NotShownInUI)]
#endif
    public class ExternalIndicatorControl : ImageDecorationBase, IWindowsMouseInput, IWindowsPreviewInput
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string NAME = "External Indicator Control";
        private List<bool> _leds = new List<bool>();
        private HeliosValue _groupValue;
        private HeliosValue _groupValueBitString;
        private string _prefixText = "";
        private string _suffixText = "";
        private string _resetText = "";
        private double _numberOfIndicators = 5d;


        public ExternalIndicatorControl() : base(NAME, new Size(96d, 128d))
        {
            DesignTimeOnly = true;
            Image = "{Helios}/Images/General/Green_LED.xaml";
            Alignment = ImageAlignment.Stretched;
            Width = 96;
            Height = 128;
            CreateActionsValuesTriggers(_numberOfIndicators);

            _groupValue = new HeliosValue(this, BindingValue.Empty, "", "Indicator Group", "Current state of Indicators within this group as a string of lit indicator numbers.", "List of lit Indicators.", BindingValueUnits.Text);
            Triggers.Add(_groupValue);
            Values.Add(_groupValue);
            _groupValueBitString = new HeliosValue(this, BindingValue.Empty, "", "Indicator Group Bitstring", "Current state of Indicators within this group as a decimal representation of the binary bitstring of the indicator states.", "List of Indicator states as a bitstring in decimal format.", BindingValueUnits.Numeric);
            Triggers.Add(_groupValueBitString);
            Values.Add(_groupValueBitString);
        }
        private void CreateActionsValuesTriggers(double numberOfIndicators)
            { CreateActionsValuesTriggers(numberOfIndicators, 0); }
        private void CreateActionsValuesTriggers(double numberOfIndicators, double previousNumberOfIndicators)
            {
                for (int i = (int)previousNumberOfIndicators; i > numberOfIndicators; i--)
                {
                    HeliosValue hv = Values[$"Indicator {i}"] as HeliosValue;
                    hv.Execute -= LED_Execute;
                    Actions.Remove(hv);
                    Triggers.Remove(hv);
                    Values.Remove(hv);
                    _leds.RemoveAt(i-1);
                }
                for (int i = (int) previousNumberOfIndicators; i < numberOfIndicators; i++)
                {
                    HeliosValue hv = new HeliosValue(this, BindingValue.Empty, "", $"Indicator {i + 1}", $"Current State of Indicator {i + 1}.", "Triggers Indicator change on output.", BindingValueUnits.Boolean);
                    hv.Execute += new HeliosActionHandler(LED_Execute);
                    Actions.Add(hv);
                    Triggers.Add(hv);
                    Values.Add(hv);
                    _leds.Add(false);
                }
        }
        #region Properties
        public string ResetText
        {
            get => _resetText;
            set
            {
                if (value != _resetText)
                {
                    string oldValue = _resetText;
                    _resetText = value;
                    OnPropertyChanged("ResetText", oldValue, value, true);

                }
            }
        }
        public string PrefixText
        {
            get => _prefixText;
            set
            {
                if (value != _prefixText)
                {
                    string oldValue = _prefixText;
                    _prefixText = value;
                    OnPropertyChanged("PrefixText", oldValue, value, true);
                }
            }
        }
        public string SuffixText
        {
            get => _suffixText;
            set
            {
                if (value != _suffixText)
                {
                    string oldValue = _suffixText;
                    _suffixText = value;
                    OnPropertyChanged("SuffixText", oldValue, value, true);
                }
            }
        }

        public double NumberOfIndicators
        {
            get => _numberOfIndicators; 
            set
            {
                if (value != _numberOfIndicators)
                {
                    double oldValue = _numberOfIndicators;
                    _numberOfIndicators = value;
                    CreateActionsValuesTriggers(value, oldValue);
                    OnPropertyChanged("NumberOfIndicators", oldValue, value, true);
                    Refresh();
                }

            }
        }
        #endregion

        #region Event Handlers

        public void LED_Execute(object action, HeliosActionEventArgs e)
        {
           if (action is HeliosValue hValue && Int32.TryParse(hValue.Name.Substring("Indicator ".Length, hValue.Name.Length - "Indicator ".Length), out int ledNumber)){
                if (_leds[ledNumber-1] != e.Value.BoolValue)
                {
                    _leds[ledNumber-1] = e.Value.BoolValue;
                    hValue.SetValue(e.Value, false);
                    string litLedGroup = "";
                    int ledGroupBitstring = 0;
                    for (int i = 0; i < _leds.Count; i++)
                    {
                        litLedGroup += _leds[i]?(i+1).ToString():"";
                        ledGroupBitstring += _leds[i] ? (int) Math.Pow(2d, i) : 0;
                    }
                    _groupValue.SetValue(new BindingValue(litLedGroup.Length > 0 ? $"{_prefixText}{litLedGroup}{_suffixText}" : _resetText), false);
                    _groupValueBitString.SetValue(new BindingValue(ledGroupBitstring), false);
                }
            }
        }

        public void GotMouseCapture(object sender, MouseEventArgs e)
        {
            Logger.Debug("GotMouseCapture");
            e.Handled = true;
        }

        public void LostMouseCapture(object sender, MouseEventArgs e)
        {
            Logger.Debug("LostMouseCapture");
            e.Handled = true;
        }

        public void MouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Logger.Debug("MouseDown");
            mouseButtonEventArgs.Handled = true;
        }

        public void MouseEnter(object sender, MouseEventArgs e)
        {
            Logger.Debug("MouseEnter");
            e.Handled = true;
        }

        public void MouseLeave(object sender, MouseEventArgs e)
        {
            Logger.Debug("MouseLeave");
            e.Handled = true;
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            Logger.Debug("MouseMove");
            e.Handled = true;
        }

        public void MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Logger.Debug("MouseUp");
            mouseButtonEventArgs.Handled = true;
        }

        public void PreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Logger.Debug("PreviewMouseDown");
        }

        public void PreviewMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Logger.Debug("PreviewMouseUp");
        }

        public void PreviewTouchDown(object sender, TouchEventArgs touchEventArgs)
        {
            Logger.Debug("PreviewTouchDown");
        }

        public void PreviewTouchUp(object sender, TouchEventArgs touchEventArgs)
        {
            Logger.Debug("PreviewTouchUp");
        }

        #endregion

        #region Overrides

        public override bool EffectsExclusion
        {
            get => true;
            set { }
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("NumberOfIndicators", _numberOfIndicators.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("PrefixText", _prefixText);
            writer.WriteElementString("SuffixText", _suffixText);
            writer.WriteElementString("ResetText", _resetText);
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            NumberOfIndicators = double.Parse(reader.ReadElementString("NumberOfIndicators"), CultureInfo.InvariantCulture);
            PrefixText = reader.ReadElementString("PrefixText");
            SuffixText = reader.ReadElementString("SuffixText");
            ResetText = reader.ReadElementString("ResetText");
            base.ReadXml(reader);
        }

        public override void MouseDown(Point location)
        {
            throw new Exception("Helios MouseDown should not be called because we implement IWindowsMouseInput");
        }

        public override void MouseDrag(Point location)
        {
            throw new Exception("Helios MouseDrag should not be called because we implement IWindowsMouseInput");
        }

        public override void MouseUp(Point location)
        {
            throw new Exception("Helios MouseUp should not be called because we implement IWindowsMouseInput");
        }

        #endregion
    }
}