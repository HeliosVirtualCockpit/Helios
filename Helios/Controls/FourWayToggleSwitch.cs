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
    using GadrocsWorkshop.Helios.Interfaces.DirectX;
    using System;
    using System.Drawing;
    using System.Windows.Input;
    using System.Xml;
    using static GadrocsWorkshop.Helios.Interfaces.DCS.Common.NetworkTriggerValue;

    [HeliosControl("Helios.Base.FourWayToggleSwitch", "\"Y\" Toggle Switch", "Toggle Switches (Multi-Way)", typeof(FourWayToggleSwitchRenderer))]
    public class FourWayToggleSwitch : ToggleSwitchBase, IConfigurableImageLocation, IRefreshableImage
    {
        private FourWayToggleSwitchType _switchType1 = FourWayToggleSwitchType.On;
        private FourWayToggleSwitchType _switchType2 = FourWayToggleSwitchType.On;
        private FourWayToggleSwitchType _switchType3 = FourWayToggleSwitchType.On;
        private FourWayToggleSwitchPosition _position = FourWayToggleSwitchPosition.Center;
        private readonly FourWayToggleSwitchPosition[] _positionArray = new FourWayToggleSwitchPosition[] { FourWayToggleSwitchPosition.Center, FourWayToggleSwitchPosition.One, FourWayToggleSwitchPosition.Center, FourWayToggleSwitchPosition.Two, FourWayToggleSwitchPosition.Center, FourWayToggleSwitchPosition.Three };
        private int _positionArrayIndex = 0;
        private FourWayToggleSwitchOrientation _switchOrientation = FourWayToggleSwitchOrientation.None;
        private string _positionOneImage;
        private string _positionOneImageIndicatorOn;
        private string _positionTwoImage;
        private string _positionTwoImageIndicatorOn;
        private string _positionThreeImage;
        private string _positionThreeImageIndicatorOn;
        private string _positionCenterImage;
        private string _positionCenterImageIndicatorOn;

        private FourWayToggleSwitchPosition _defaultPosition = FourWayToggleSwitchPosition.Center;

        private HeliosValue _positionValue;
        private HeliosTrigger _positionOneEnterAction;
        private HeliosTrigger _positionOneExitAction;
        private HeliosTrigger _positionTwoEnterAction;
        private HeliosTrigger _positionTwoExitAction;
        private HeliosTrigger _positionThreeEnterAction;
        private HeliosTrigger _positionThreeExitAction;
        private HeliosTrigger _positionCenterEnterAction;
        private HeliosTrigger _positionCenterExitAction;

        private System.Windows.Point _mouseDownLocation;
        private bool _mouseAction;


        public FourWayToggleSwitch() : this("Four Way Toggle Switch", new System.Windows.Size(100, 100)) {}
        public FourWayToggleSwitch(string name, System.Windows.Size size)
            : base(name, size)
        {
            _positionOneImage = "{Helios}/Images/Toggles/Orange-Round-000.png";
            _positionTwoImage = "{Helios}/Images/Toggles/Orange-Round-120.png";
            _positionThreeImage = "{Helios}/Images/Toggles/Orange-Round-240.png";
            _positionCenterImage = "{Helios}/Images/Toggles/Orange-Round-Center.png";

            _positionOneEnterAction = new HeliosTrigger(this, "", "position one", "entered", "Triggered when position one is entered or depressed.");
            Triggers.Add(_positionOneEnterAction);
            _positionOneExitAction = new HeliosTrigger(this, "", "position one", "exited", "Triggered when position one is exited or released.");
            Triggers.Add(_positionOneExitAction);
            _positionTwoEnterAction = new HeliosTrigger(this, "", "position two", "entered", "Triggered when position two is entered or depressed.");
            Triggers.Add(_positionTwoEnterAction);
            _positionTwoExitAction = new HeliosTrigger(this, "", "position two", "exited", "Triggered when position two is exited or released.");
            Triggers.Add(_positionTwoExitAction);
            _positionThreeEnterAction = new HeliosTrigger(this, "", "position three", "entered", "Triggered when position three is entered or depressed.");
            Triggers.Add(_positionThreeEnterAction);
            _positionThreeExitAction = new HeliosTrigger(this, "", "position three", "exited", "Triggered when position three is exited or released.");
            Triggers.Add(_positionThreeExitAction);
            _positionCenterEnterAction = new HeliosTrigger(this, "", "position center", "entered", "Triggered when position Center is entered or depressed.");
            Triggers.Add(_positionCenterEnterAction);
            _positionCenterExitAction = new HeliosTrigger(this, "", "position center", "exited", "Triggered when position Center is exited or released.");
            Triggers.Add(_positionCenterExitAction);

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the switch.", "Position number 1,2 or 3.  Positions are numbered from top to bottom.", BindingValueUnits.Numeric);
            _positionValue.Execute += new HeliosActionHandler(SetPositionAction_Execute);
            Values.Add(_positionValue);
            Actions.Add(_positionValue);
            Triggers.Add(_positionValue);

        }
        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((PositionOneImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionThreeImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionCenterImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionOneIndicatorOnImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionTwoIndicatorOnImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionThreeIndicatorOnImage ?? "").ToLower().Replace("/", @"\") == imageName ||
                (PositionCenterIndicatorOnImage ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                OnPropertyChanged("PositionTwoImage", PositionTwoImage, PositionTwoImage, true);
                Refresh();
            }
            return ImageRefresh;
        }

        #region Properties

        public FourWayToggleSwitchPosition DefaultPosition
        {
            get
            {
                return _defaultPosition;
            }
            set
            {
                if (!_defaultPosition.Equals(value))
                {
                    FourWayToggleSwitchPosition oldValue = _defaultPosition;
                    _defaultPosition = value;
                    OnPropertyChanged("DefaultPosition", oldValue, value, true);
                }
            }
        }
        public FourWayToggleSwitchOrientation SwitchOrientation
        {
            get
            {
                return _switchOrientation;
            }
            set
            {
                if (!_switchOrientation.Equals(value))
                {
                    FourWayToggleSwitchOrientation oldValue = _switchOrientation;
                    _switchOrientation = value;
                    OnPropertyChanged("SwitchOrientation", oldValue, value, true);
                }
            }
        }
        public FourWayToggleSwitchType SwitchType1
        {
            get
            {
                return _switchType1;
            }
            set
            {
                if (!_switchType1.Equals(value))
                {
                    FourWayToggleSwitchType oldValue = _switchType1;
                    _switchType1 = value;
                    OnPropertyChanged("SwitchType1", oldValue, value, true);
                }
            }
        }
        public FourWayToggleSwitchType SwitchType2
        {
            get
            {
                return _switchType2;
            }
            set
            {
                if (!_switchType2.Equals(value))
                {
                    FourWayToggleSwitchType oldValue = _switchType2;
                    _switchType2 = value;
                    OnPropertyChanged("SwitchType2", oldValue, value, true);
                }
            }
        }
        public FourWayToggleSwitchType SwitchType3
        {
            get
            {
                return _switchType3;
            }
            set
            {
                if (!_switchType3.Equals(value))
                {
                    FourWayToggleSwitchType oldValue = _switchType3;
                    _switchType3 = value;
                    OnPropertyChanged("SwitchType3", oldValue, value, true);
                }
            }
        }

        public FourWayToggleSwitchPosition SwitchPosition
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    FourWayToggleSwitchPosition oldValue = _position;

                    if (!BypassTriggers)
                    {
                        switch (oldValue)
                        {
                            case FourWayToggleSwitchPosition.One:
                                _positionOneExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case FourWayToggleSwitchPosition.Two:
                                _positionTwoExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case FourWayToggleSwitchPosition.Three:
                                _positionThreeExitAction.FireTrigger(BindingValue.Empty);
                                break;
                            case FourWayToggleSwitchPosition.Center:
                                _positionCenterExitAction.FireTrigger(BindingValue.Empty);
                                break;
                        }
                    }

                    _position = value;
                    _positionValue.SetValue(new BindingValue((double)_position), BypassTriggers);

                    if (!BypassTriggers)
                    {
                        switch (value)
                        {
                            case FourWayToggleSwitchPosition.One:
                                _positionOneEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case FourWayToggleSwitchPosition.Two:
                                _positionTwoEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case FourWayToggleSwitchPosition.Three:
                                _positionThreeEnterAction.FireTrigger(BindingValue.Empty);
                                break;
                            case FourWayToggleSwitchPosition.Center:
                                _positionCenterEnterAction.FireTrigger(BindingValue.Empty);
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
        public string PositionThreeImage
        {
            get
            {
                return _positionThreeImage;
            }
            set
            {
                if ((_positionThreeImage == null && value != null)
                    || (_positionThreeImage != null && !_positionThreeImage.Equals(value)))
                {
                    string oldValue = _positionThreeImage;
                    _positionThreeImage = value;
                    OnPropertyChanged("PositionThreeImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionThreeIndicatorOnImage
        {
            get
            {
                return _positionThreeImageIndicatorOn;
            }
            set
            {
                if ((_positionThreeImageIndicatorOn == null && value != null)
                    || (_positionThreeImageIndicatorOn != null && !_positionThreeImageIndicatorOn.Equals(value)))
                {
                    string oldValue = _positionThreeImageIndicatorOn;
                    _positionThreeImageIndicatorOn = value;
                    OnPropertyChanged("PositionThreeIndicatorOnImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionCenterImage
        {
            get
            {
                return _positionCenterImage;
            }
            set
            {
                if ((_positionCenterImage == null && value != null)
                    || (_positionCenterImage != null && !_positionCenterImage.Equals(value)))
                {
                    string oldValue = _positionCenterImage;
                    _positionCenterImage = value;
                    OnPropertyChanged("PositionCenterImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string PositionCenterIndicatorOnImage
        {
            get
            {
                return _positionCenterImageIndicatorOn;
            }
            set
            {
                if ((_positionCenterImageIndicatorOn == null && value != null)
                    || (_positionCenterImageIndicatorOn != null && !_positionCenterImageIndicatorOn.Equals(value)))
                {
                    string oldValue = _positionCenterImageIndicatorOn;
                    _positionCenterImageIndicatorOn = value;
                    OnPropertyChanged("PositionCenterIndicatorOnImage", oldValue, value, true);
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
            PositionThreeImage = string.IsNullOrEmpty(PositionThreeImage) ? PositionThreeImage : string.IsNullOrEmpty(oldName) ? newName + PositionThreeImage : PositionThreeImage.Replace(oldName, newName);
            PositionThreeIndicatorOnImage = string.IsNullOrEmpty(PositionThreeIndicatorOnImage) ? PositionThreeIndicatorOnImage : string.IsNullOrEmpty(oldName) ? newName + PositionThreeIndicatorOnImage : PositionThreeIndicatorOnImage.Replace(oldName, newName);
            PositionCenterImage = string.IsNullOrEmpty(PositionCenterImage) ? PositionCenterImage : string.IsNullOrEmpty(oldName) ? newName + PositionCenterImage : PositionCenterImage.Replace(oldName, newName);
            PositionCenterIndicatorOnImage = string.IsNullOrEmpty(PositionCenterIndicatorOnImage) ? PositionCenterIndicatorOnImage : string.IsNullOrEmpty(oldName) ? newName + PositionCenterIndicatorOnImage : PositionCenterIndicatorOnImage.Replace(oldName, newName);
        }

        protected override void ThrowSwitch(ToggleSwitchBase.SwitchAction action)
        {
            if (action == SwitchAction.Increment)
            {
                _positionArrayIndex = _positionArrayIndex <= _positionArray.Length - 2 ? ++_positionArrayIndex : 0;
                SwitchPosition = _positionArray[_positionArrayIndex];
            }
            else if (action == SwitchAction.Decrement)
            {
                _positionArrayIndex = _positionArrayIndex != 0 ? --_positionArrayIndex : _positionArray.Length - 1;
                SwitchPosition = _positionArray[_positionArrayIndex];
            }
        }

        public override void MouseUp(System.Windows.Point location)
        {
            //base.MouseUp(location);
            SetSwitchPosition(
                rectX: 0,
                rectY: 0,
                rectWidth: Width,
                rectHeight: Height,
                orientation: _switchOrientation,
                location: location);

        }
        public override void MouseDown(System.Windows.Point location)
        {
            _mouseAction = false;

            if (ClickType == LinearClickType.Swipe)
            {
                _mouseDownLocation = location;
                if (DesignMode && HasIndicator)
                {
                    IndicatorOn = !IndicatorOn;
                }
            }
            else if (ClickType == LinearClickType.Touch)
            {

                //if (action != SwitchAction.None)
                //{
                //    ThrowSwitch(action);
                //    _mouseAction = true;
                //}
            }
        }
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SwitchType1", SwitchType1.ToString());
            writer.WriteElementString("SwitchType2", SwitchType2.ToString());
            writer.WriteElementString("SwitchType3", SwitchType3.ToString());
            writer.WriteElementString("SwitchOrientation", SwitchOrientation.ToString());
            writer.WriteElementString("ClickType", ClickType.ToString());
            writer.WriteElementString("PositionOneImage", PositionOneImage);
            writer.WriteElementString("PositionTwoImage", PositionTwoImage);
            writer.WriteElementString("PositionThreeImage", PositionThreeImage);
            writer.WriteElementString("PositionCenterImage", PositionCenterImage);
            if (HasIndicator)
            {
                writer.WriteStartElement("Indicator");
                writer.WriteElementString("PositionOneImage", PositionOneIndicatorOnImage);
                writer.WriteElementString("PositionTwoImage", PositionTwoIndicatorOnImage);
                writer.WriteElementString("PositionThreeImage", PositionThreeIndicatorOnImage);
                writer.WriteElementString("PositionCenterImage", PositionCenterIndicatorOnImage);
                writer.WriteEndElement();
            }
            writer.WriteElementString("DefaultPosition", DefaultPosition.ToString());
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            SwitchType1 = (FourWayToggleSwitchType)Enum.Parse(typeof(FourWayToggleSwitchType), reader.ReadElementString("SwitchType1"));
            SwitchType2 = (FourWayToggleSwitchType)Enum.Parse(typeof(FourWayToggleSwitchType), reader.ReadElementString("SwitchType2"));
            SwitchType3 = (FourWayToggleSwitchType)Enum.Parse(typeof(FourWayToggleSwitchType), reader.ReadElementString("SwitchType3"));
            SwitchOrientation = (FourWayToggleSwitchOrientation)Enum.Parse(typeof(FourWayToggleSwitchOrientation), reader.ReadElementString("SwitchOrientation"));
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
            PositionThreeImage = reader.ReadElementString("PositionThreeImage");
            PositionCenterImage = reader.ReadElementString("PositionCenterImage");
            if (reader.Name == "Indicator")
            {
                HasIndicator = true;
                reader.ReadStartElement("Indicator");
                PositionOneIndicatorOnImage = reader.ReadElementString("PositionOneImage");
                PositionTwoIndicatorOnImage = reader.ReadElementString("PositionTwoImage");
                PositionThreeIndicatorOnImage = reader.ReadElementString("PositionThreeImage");
                PositionCenterIndicatorOnImage = reader.ReadElementString("PositionCenterImage");
                reader.ReadEndElement();
            }
            else
            {
                HasIndicator = false;
            }
            if (reader.Name == "DefaultPosition")
            {
                DefaultPosition = (FourWayToggleSwitchPosition)Enum.Parse(typeof(FourWayToggleSwitchPosition), reader.ReadElementString("DefaultPosition"));
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
                        SwitchPosition = (FourWayToggleSwitchPosition)newPosition;
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
        private void SetSwitchPosition(
            double rectX,
            double rectY,
            double rectWidth,
            double rectHeight,
            FourWayToggleSwitchOrientation orientation,
            System.Windows.Point location)
        {
            double pointX = location.X, pointY = location.Y;
            double circleRadius = rectHeight / 3;
            int offsetAngle = (int) orientation;
            
            // Rectangle bounds check
            if (pointX < rectX || pointX > rectX + rectWidth ||
                pointY < rectY || pointY > rectY + rectHeight)
            {
                return;
            }

            // Rectangle centre
            double cx = rectX + rectWidth / 2.0;
            double cy = rectY + rectHeight / 2.0;

            double dx = pointX - cx;
            double dy = pointY - cy;

            // Central circle check
            if (dx * dx + dy * dy <= circleRadius * circleRadius)
            {
                SwitchPosition = FourWayToggleSwitchPosition.Center;
                return;
            }

            // Angle from centre (degrees, 0° = +X axis)
            double angleRadians = Math.Atan2(dx, -dy);
            double angleDegrees = angleRadians * (180.0 / Math.PI);

            if(IsAngleBetween(angleDegrees, -60d, 60d, offsetAngle))
            {
                SwitchPosition = FourWayToggleSwitchPosition.One;
            } else if (IsAngleBetween(angleDegrees, 60d, 180d, offsetAngle))
            {
                SwitchPosition = FourWayToggleSwitchPosition.Two;
            } else
            {
                SwitchPosition = FourWayToggleSwitchPosition.Three;
            }
            return;
        }
        private static bool IsAngleBetween(double angle, double start, double end, int offsetAngle)
        {
            angle = (angle + 360) % 360;
            start = (start + offsetAngle + 360) % 360;
            end = (end + offsetAngle + 360) % 360;

            return start <= end
                ? angle >= start && angle <= end
                : angle >= start || angle <= end;
        }

    }
}
