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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System;
    using System.Xml;

    [HeliosControl("Helios.Base.ToggleSwitch", "Toggle Switch", "Toggle Switches", typeof(ToggleSwitchRenderer))]
    public class ToggleSwitch : ToggleSwitchBase, IConfigurableImageLocation
    {
        private ToggleSwitchType _switchType = ToggleSwitchType.OnOn;
        private ToggleSwitchPosition _position = ToggleSwitchPosition.Two;

        private string _positionOneImage;
        private string _positionOneImageIndicatorOn;
        private string _positionTwoImage;
        private string _positionTwoImageIndicatorOn;
        private ToggleSwitchPosition _defaultPosition = ToggleSwitchPosition.Two;

        private HeliosValue _positionValue;
        private HeliosTrigger _positionOneEnterAction;
        private HeliosTrigger _positionOneExitAction;
        private HeliosTrigger _positionTwoEnterAction;
        private HeliosTrigger _positionTwoExitAction;
        
        public ToggleSwitch()
            : base("Toggle Switch", new System.Windows.Size(50, 100))
        {
            _positionOneImage = "{Helios}/Images/Toggles/toggle-up.png";
            _positionTwoImage = "{Helios}/Images/Toggles/toggle-down.png";

            _positionOneEnterAction = new HeliosTrigger(this, "", "position one", "entered", "Triggered when position one is entered or depressed.");
            Triggers.Add(_positionOneEnterAction);
            _positionOneExitAction = new HeliosTrigger(this, "", "position one", "exited", "Triggered when posotion one is exited or released.");
            Triggers.Add(_positionOneExitAction);
            _positionTwoEnterAction = new HeliosTrigger(this, "", "position two", "entered", "Triggered when position two is entered or depressed.");
            Triggers.Add(_positionTwoEnterAction);
            _positionTwoExitAction = new HeliosTrigger(this, "", "position two", "exited", "Triggered when posotion two is exited or released.");
            Triggers.Add(_positionTwoExitAction);

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the switch.", "Position number 1 or 2.  Positions are numbered from top to bottom.", BindingValueUnits.Numeric);
            _positionValue.Execute += new HeliosActionHandler(SetPositionAction_Execute);
            Values.Add(_positionValue);
            Actions.Add(_positionValue);
            Triggers.Add(_positionValue);
        }

        #region Properties

        public ToggleSwitchType SwitchType
        {
            get
            {
                return _switchType;
            }
            set
            {
                if (!_switchType.Equals(value))
                {
                    ToggleSwitchType oldValue = _switchType;
                    _switchType = value;
                    OnPropertyChanged("SwitchType", oldValue, value, false);
                }
            }
        }

        public ToggleSwitchPosition DefaultPosition
        {
            get
            {
                return _defaultPosition;
            }
            set
            {
                if (!_defaultPosition.Equals(value))
                {
                    ToggleSwitchPosition oldValue = _defaultPosition;
                    _defaultPosition = value;
                    OnPropertyChanged("DefaultPosition", oldValue, value, true);
                }
            }
        }

        public ToggleSwitchPosition SwitchPosition
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    ToggleSwitchPosition oldValue = _position;

                    _position = value;
                    _positionValue.SetValue(new BindingValue((double)_position), BypassTriggers);

                    if (!BypassTriggers)
                    {
                        switch (oldValue)
                        {
                            case ToggleSwitchPosition.One:
                                _positionOneExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ToggleSwitchPosition.Two:
                                _positionTwoExitAction.FireTrigger(BindingValue.Empty);
                                break;
                        }

                        switch (value)
                        {
                            case ToggleSwitchPosition.One:
                                _positionOneEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case ToggleSwitchPosition.Two:
                                _positionTwoEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                        }
                    }

                    OnPropertyChanged("SwitchPosition", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public string PositionOneImage
        {
            get
            {
                return _positionOneImage;
            }
            set
            {
                if ((_positionOneImage == null && value != null)
                    || (_positionOneImage != null && !_positionOneImage.Equals(value)))
                {
                    string oldValue = _positionOneImage;
                    _positionOneImage = value;
                    OnPropertyChanged("PositionOneImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionOneIndicatorOnImage
        {
            get
            {
                return _positionOneImageIndicatorOn;
            }
            set
            {
                if ((_positionOneImageIndicatorOn == null && value != null)
                    || (_positionOneImageIndicatorOn != null && !_positionOneImageIndicatorOn.Equals(value)))
                {
                    string oldValue = _positionOneImageIndicatorOn;
                    _positionOneImageIndicatorOn = value;
                    OnPropertyChanged("PositionOneIndicatorOnImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionTwoImage
        {
            get
            {
                return _positionTwoImage;
            }
            set
            {
                if ((_positionTwoImage == null && value != null)
                    || (_positionTwoImage != null && !_positionTwoImage.Equals(value)))
                {
                    string oldValue = _positionTwoImage;
                    _positionTwoImage = value;
                    OnPropertyChanged("PositionTwoImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionTwoIndicatorOnImage
        {
            get
            {
                return _positionTwoImageIndicatorOn;
            }
            set
            {
                if ((_positionTwoImageIndicatorOn == null && value != null)
                    || (_positionTwoImageIndicatorOn != null && !_positionTwoImageIndicatorOn.Equals(value)))
                {
                    string oldValue = _positionTwoImageIndicatorOn;
                    _positionTwoImageIndicatorOn = value;
                    OnPropertyChanged("PositionTwoIndicatorOnImage", oldValue, value, true);
                    Refresh();
                }
            }
        }


        #endregion

        #region HeliosControl Implementation

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            SwitchPosition = DefaultPosition;
            EndTriggerBypass(true);
        }
        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceImageNames(string oldName, string newName)
        {
            PositionOneImage = string.IsNullOrEmpty(PositionOneImage) ? PositionOneImage : string.IsNullOrEmpty(oldName) ? newName + PositionOneImage : PositionOneImage.Replace(oldName, newName);
            PositionOneIndicatorOnImage = string.IsNullOrEmpty(PositionOneIndicatorOnImage) ? PositionOneIndicatorOnImage : string.IsNullOrEmpty(oldName) ? newName + PositionOneIndicatorOnImage : PositionOneIndicatorOnImage.Replace(oldName, newName);
            PositionTwoImage = string.IsNullOrEmpty(PositionTwoImage) ? PositionTwoImage : string.IsNullOrEmpty(oldName) ? newName + PositionTwoImage : PositionTwoImage.Replace(oldName, newName);
            PositionTwoIndicatorOnImage = string.IsNullOrEmpty(PositionTwoIndicatorOnImage) ? PositionTwoIndicatorOnImage : string.IsNullOrEmpty(oldName) ? newName + PositionTwoIndicatorOnImage : PositionTwoIndicatorOnImage.Replace(oldName, newName);
        }
        protected override void ThrowSwitch(SwitchAction action)
       {
            if (action == SwitchAction.Increment)
            {
                switch (SwitchPosition)
                {
                    case ToggleSwitchPosition.One:
                        SwitchPosition = ToggleSwitchPosition.Two;
                        break;
                }
            }
            else if (action == SwitchAction.Decrement)
            {
                switch (SwitchPosition)
                {
                    case ToggleSwitchPosition.Two:
                        SwitchPosition = ToggleSwitchPosition.One;
                        break;
                }
            }
        }

        public override void MouseUp(System.Windows.Point location)
        {
            base.MouseUp(location);

            switch (SwitchPosition)
            {
                case ToggleSwitchPosition.One:
                    if (SwitchType == ToggleSwitchType.MomOn)
                    {
                        SwitchPosition = ToggleSwitchPosition.Two;
                    }
                    break;
                case ToggleSwitchPosition.Two:
                    if (SwitchType == ToggleSwitchType.OnMom)
                    {
                        SwitchPosition = ToggleSwitchPosition.One;
                    }
                    break;
            }
        }

        public override void MouseDown(System.Windows.Point location)
        {
            if (NonClickableZones != null)
            {
                foreach (NonClickableZone zone in NonClickableZones)
                {
                    if (SwitchPosition.ToString() == zone.PositionWhenApplicable.ToString() && zone.isClickInZone(location))
                    {
                        zone.ChildVisual.MouseDown(new System.Windows.Point(location.X-(zone.ChildVisual.Left-this.Left),location.Y-(zone.ChildVisual.Top-this.Top)));
                        return; //we get out to let the ChildVisual using the click
                    }
                }
            }

            base.MouseDown(location);

            if(SwitchPosition == ToggleSwitchPosition.One)
            {
                if (NonClickableZones != null)
                {
                    foreach (NonClickableZone zone in NonClickableZones)
                    {
                        switch (zone.ChildVisual.GetType().ToString())
                        {
                            case "GadrocsWorkshop.Helios.Controls.ToggleSwitch":
                                ToggleSwitch tSwitch = (ToggleSwitch)zone.ChildVisual;
                                tSwitch.SwitchPosition = zone.GuardedChildPosition;
                                break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SwitchType", SwitchType.ToString());
            writer.WriteElementString("Orientation", Orientation.ToString());
            writer.WriteElementString("ClickType", ClickType.ToString());
            writer.WriteElementString("PositionOneImage", PositionOneImage);
            writer.WriteElementString("PositionTwoImage", PositionTwoImage);
            if (HasIndicator)
            {
                writer.WriteStartElement("Indicator");
                writer.WriteElementString("PositionOneImage", PositionOneIndicatorOnImage);
                writer.WriteElementString("PositionTwoImage", PositionTwoIndicatorOnImage);
                writer.WriteEndElement();
            }
            writer.WriteElementString("DefaultPosition", DefaultPosition.ToString());
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            SwitchType = (ToggleSwitchType)Enum.Parse(typeof(ToggleSwitchType), reader.ReadElementString("SwitchType"));
            Orientation = (ToggleSwitchOrientation)Enum.Parse(typeof(ToggleSwitchOrientation), reader.ReadElementString("Orientation"));
            if (reader.Name.Equals("ClickType"))
            {
                ClickType = (LinearClickType)Enum.Parse(typeof(LinearClickType), reader.ReadElementString("ClickType"));
            }
            else
            {
                ClickType = LinearClickType.Swipe;
            }
            PositionOneImage = reader.ReadElementString("PositionOneImage");
            PositionTwoImage = reader.ReadElementString("PositionTwoImage");
            if (reader.Name == "Indicator")
            {
                HasIndicator = true;
                reader.ReadStartElement("Indicator");
                PositionOneIndicatorOnImage = reader.ReadElementString("PositionOneImage");
                PositionTwoIndicatorOnImage = reader.ReadElementString("PositionTwoImage");
                reader.ReadEndElement();
            }
            else
            {
                HasIndicator = false;
            }
            if (reader.Name == "DefaultPosition")
            {
                DefaultPosition = (ToggleSwitchPosition)Enum.Parse(typeof(ToggleSwitchPosition), reader.ReadElementString("DefaultPosition"));
                BeginTriggerBypass(true);
                SwitchPosition = DefaultPosition;
                EndTriggerBypass(true);
            }
        }

        #endregion

        #region Actions

        void SetPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                BeginTriggerBypass(e.BypassCascadingTriggers);
                int newPosition = 0;
                if (int.TryParse(e.Value.StringValue, out newPosition))
                {
                    if (newPosition > 0 && newPosition <= 3)
                    {
                        SwitchPosition = (ToggleSwitchPosition)newPosition;
                    }
                }

                EndTriggerBypass(e.BypassCascadingTriggers);
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        #endregion
    }
}
