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


    [HeliosControl("Helios.Base.PotentiometerIndicatorClickable", "Clickable Potentiometer with Indicator - Knob 1", "Potentiometers", typeof(RotaryKnobRenderer))]
    public class PotentiometerIndcatorClickable : PotentiometerClickable, IConfigurableImageLocation, IRefreshableImage
    {
        private string _indicatorOnNormalImageFile = "";
        private string _indicatorOnClickedImageFile = "";
        private string _indicatorOffClickedImageFile = "";
        private string _indicatorOffNormalImageFile = "";

        private bool _on;

        private HeliosAction _toggleAction;
        private HeliosValue _value;

        public PotentiometerIndcatorClickable() : base("Clickable Potentiometer with Indicator")
        {
            ContinuousConfigurable = true;
            IsContinuous = false;
            _indicatorOffClickedImageFile = PushedImage;
            _indicatorOffNormalImageFile = UnpushedImage;
            Pushed = false;

            _value = new HeliosValue(this, new BindingValue(false), "", "indicator", "Current On/Off State for this indicator.", "True if the indicator is on, otherwise False.", BindingValueUnits.Boolean);
            _value.Execute += new HeliosActionHandler(On_Execute);
            Values.Add(_value);
            Actions.Add(_value);

            _toggleAction = new HeliosAction(this, "", "", "toggle indicator", "Toggles this indicator between on and off.");
            _toggleAction.Execute += new HeliosActionHandler(ToggleAction_Execute);
            Actions.Add(_toggleAction);
        }
        #region Properties

        public bool On
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

        public string IndicatorOnNormalImage
        {
            get
            {
                return _indicatorOnNormalImageFile;
            }
            set
            {
                if ((_indicatorOnNormalImageFile == null && value != null)
                    || (_indicatorOnNormalImageFile != null && !_indicatorOnNormalImageFile.Equals(value)))
                {
                    string oldValue = _indicatorOnNormalImageFile;
                    _indicatorOnNormalImageFile = value;
                    OnPropertyChanged("IndicatorOnNormalImage", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public string IndicatorOnClickedImage
        {
            get
            {
                return _indicatorOnClickedImageFile;
            }
            set
            {
                if ((_indicatorOnClickedImageFile == null && value != null)
                    || (_indicatorOnClickedImageFile != null && !_indicatorOnClickedImageFile.Equals(value)))
                {
                    string oldValue = _indicatorOnClickedImageFile;
                    _indicatorOnClickedImageFile = value;
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
                    UnpushedImage = On ? IndicatorOnNormalImage : _indicatorOffNormalImageFile;
                    PushedImage = On ? IndicatorOnClickedImage : _indicatorOffClickedImageFile;
                    return;
                case "Pushed":
                    if (AllowRotation != RotaryClickAllowRotationType.Both)
                    {
                        Value = InitialValue;
                    }
                    break;
                default:
                    break;
            }
            base.OnPropertyChanged(args);
        }
        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public new void ReplaceImageNames(string oldName, string newName)
        {
            base.ReplaceImageNames(oldName, newName);
            IndicatorOnNormalImage = string.IsNullOrEmpty(_indicatorOnNormalImageFile) ? _indicatorOnNormalImageFile : string.IsNullOrEmpty(oldName) ? newName + _indicatorOnNormalImageFile : _indicatorOnNormalImageFile.Replace(oldName, newName);
            _indicatorOffNormalImageFile = string.IsNullOrEmpty(_indicatorOffNormalImageFile) ? _indicatorOffNormalImageFile : string.IsNullOrEmpty(oldName) ? newName + _indicatorOffNormalImageFile : _indicatorOffNormalImageFile.Replace(oldName, newName);
            IndicatorOnClickedImage = string.IsNullOrEmpty(_indicatorOnClickedImageFile) ? _indicatorOnClickedImageFile : string.IsNullOrEmpty(oldName) ? newName + _indicatorOnClickedImageFile : _indicatorOnClickedImageFile.Replace(oldName, newName);
            _indicatorOffClickedImageFile = string.IsNullOrEmpty(_indicatorOffClickedImageFile) ? _indicatorOffClickedImageFile : string.IsNullOrEmpty(oldName) ? newName + _indicatorOffClickedImageFile : _indicatorOffClickedImageFile.Replace(oldName, newName);
        }

        public override bool ConditionalImageRefresh(string imageName)
        {
            ImageRefresh = base.ConditionalImageRefresh(imageName);

            if ((IndicatorOnNormalImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (IndicatorOnClickedImage ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
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
            _indicatorOffClickedImageFile = PushedImage;
            _indicatorOffNormalImageFile = UnpushedImage;

        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("PushedIndicatorOnImage", IndicatorOnClickedImage);
            writer.WriteElementString("UnpushedIndicatorOnImage", IndicatorOnNormalImage);
            base.WriteXml(writer);
        }
    }
}
