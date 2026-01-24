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

using System.Windows.Controls;

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System.ComponentModel;
    using System.Globalization;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;


    [HeliosControl("Helios.Base.RotaryEncoderIndicatorClickable", "Rotary Encoder Clickable with Indicator - Knob 1", "Rotary Encoders", typeof(RotaryKnobRenderer))]
    public class RotaryEncoderIndcatorClickable : RotaryEncoderClickable, IConfigurableImageLocation, IRefreshableImage
    {
        private string _indicatorOnNormalImage = "";
        private string _indicatorOnClickedImage = "";


        private bool _on;

        private HeliosAction _toggleAction;
        private HeliosValue _value;

        public RotaryEncoderIndcatorClickable() : base("Rotary Encoder with Click and Indicator")
        {
            Pushed = false;
            On = false;
            _value = new HeliosValue(this, new BindingValue(false), "", "indicator", "Current On/Off State for this indicator.", "True if the indicator is on, otherwise False.", BindingValueUnits.Boolean);
            _value.Execute += new HeliosActionHandler(On_Execute);
            Values.Add(_value);
            Actions.Add(_value);

            _toggleAction = new HeliosAction(this, "", "", "toggle indicator", "Toggles this indicator between on and off.");
            _toggleAction.Execute += new HeliosActionHandler(ToggleAction_Execute);
            Actions.Add(_toggleAction);
        }
        #region Properties

        public override bool On
        {
            get
            {
                return _on;
            }
            set
            {
                if (!_on.Equals(value))
                {
                    bool oldValue = _on;

                    _on = value;
                    _value.SetValue(new BindingValue(_on), BypassTriggers);

                    OnPropertyChanged("On", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }
        public override string IndicatorOnNormalImage
        {
            get
            {
                return _indicatorOnNormalImage;
            }
            set
            {
                if ((_indicatorOnNormalImage == null && value != null)
                    || (_indicatorOnNormalImage != null && !_indicatorOnNormalImage.Equals(value)))
                {
                    string oldValue = _indicatorOnNormalImage;
                    _indicatorOnNormalImage = value;
                    if (!Pushed && On)
                    {
                        KnobImage = value;
                    }
                    OnPropertyChanged("IndicatorOnNormalImage", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public override string IndicatorOnClickedImage
        {
            get
            {
                return _indicatorOnClickedImage;
            }
            set
            {
                if ((_indicatorOnClickedImage == null && value != null)
                    || (_indicatorOnClickedImage != null && !_indicatorOnClickedImage.Equals(value)))
                {
                    string oldValue = _indicatorOnClickedImage;
                    _indicatorOnClickedImage = value;
                    if (Pushed && On)
                    {
                        KnobImage = value;
                    }
                    OnPropertyChanged("IndicatorOnClickedImage", oldValue, value, true);
                    Refresh();
                }            
            }
        }
        public override bool IndicatorConfigurable
        {
            get => true;
        }
        #endregion

        void ToggleAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            On = !On;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        void On_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            On = e.Value.BoolValue;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "On":
                case "Pushed":
                    KnobImage = Pushed ? (On ? _indicatorOnClickedImage : PushedImage) : (On ? _indicatorOnNormalImage : UnpushedImage);
                    break;
                 default:
                    base.OnPropertyChanged(args);
                    break;
            }
        }
        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public new void ReplaceImageNames(string oldName, string newName)
        {
            base.ReplaceImageNames(oldName, newName);
            IndicatorOnNormalImage = string.IsNullOrEmpty(IndicatorOnNormalImage) ? IndicatorOnNormalImage : string.IsNullOrEmpty(oldName) ? newName + IndicatorOnNormalImage : IndicatorOnNormalImage.Replace(oldName, newName);
            IndicatorOnClickedImage = string.IsNullOrEmpty(IndicatorOnClickedImage) ? IndicatorOnClickedImage : string.IsNullOrEmpty(oldName) ? newName + IndicatorOnClickedImage : IndicatorOnClickedImage.Replace(oldName, newName);
        }

        public override bool ConditionalImageRefresh(string imageName)
        {
            ImageRefresh = base.ConditionalImageRefresh(imageName);
            if ((IndicatorOnNormalImage ?? "").ToLower().Replace("/", @"\") == imageName && KnobImage != imageName)
            {
                ImageRefresh = true;
                ReloadImage(imageName);
            }
            if ((IndicatorOnClickedImage ?? "").ToLower().Replace("/", @"\") == imageName && KnobImage != imageName)
            {
                ImageRefresh = true;
                ReloadImage(imageName);
            }

            return ImageRefresh;
        }

        public override void MouseDown(Point location)
        {
            base.MouseDown(location);
        }
        public override void MouseUp(Point location)
        {
            base.MouseUp(location);
        }

        public override void ReadXml(XmlReader reader)
        {
            IndicatorOnClickedImage = reader.ReadElementString("PushedIndicatorOnImage");
            IndicatorOnNormalImage = reader.ReadElementString("UnpushedIndicatorOnImage");
            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("PushedIndicatorOnImage", IndicatorOnClickedImage);
            writer.WriteElementString("UnpushedIndicatorOnImage", IndicatorOnNormalImage);
            base.WriteXml(writer);
        }
    }
}
