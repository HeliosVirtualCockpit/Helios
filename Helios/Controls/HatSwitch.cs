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
    using System;
    using System.Globalization;
    using System.Windows;

    [HeliosControl("Helios.Base.HatSwitch", "Hat Switch", "Switches (Hat)", typeof(HatSwitchRenderer))]
    public class HatSwitch : HeliosVisual
    {

        private HatSwitchPosition _position = HatSwitchPosition.Center;
        private HatSwitchHorizontalPosition _horizontalPosition = HatSwitchHorizontalPosition.Center;
        private HatSwitchVerticalPosition _verticalPosition = HatSwitchVerticalPosition.Center;

        private string _centerImage;
        private string _upImage;
        private string _downImage;
        private string _leftImage;
        private string _rightImage;

        private HeliosValue _positionValue;
        private HeliosValue _horizontalPositionValue;
        private HeliosValue _verticalPositionValue;
        private HeliosTrigger _upTrigger;
        private HeliosTrigger _downTrigger;
        private HeliosTrigger _leftTrigger;
        private HeliosTrigger _rightTrigger;
        private HeliosTrigger _upExitTrigger;
        private HeliosTrigger _downExitTrigger;
        private HeliosTrigger _leftExitTrigger;
        private HeliosTrigger _rightExitTrigger;
        private HeliosTrigger _centerTrigger;
        private HeliosTrigger _centerExitTrigger;

        private Point _topLeft;
        private Point _topRight;
        private Point _middle;
        private Point _bottomLeft;
        private Point _bottomRight;

        public HatSwitch() : base("Hat Switch", new Size(100d, 100d))
        {
            _centerImage = "{Helios}/Images/Hats/hat-center.png";
            _upImage = "{Helios}/Images/Hats/hat-up.png";
            _downImage = "{Helios}/Images/Hats/hat-down.png";
            _leftImage = "{Helios}/Images/Hats/hat-left.png";
            _rightImage = "{Helios}/Images/Hats/hat-right.png";

            _horizontalPositionValue = new HeliosValue(this, new BindingValue((double)HorizontalSwitchPosition), "", "horizontal position", "Current horizontal position of the hat switch.", "Position 1 = left, 2 = center, 3 = right.", BindingValueUnits.Numeric);
            _horizontalPositionValue.Execute += new HeliosActionHandler(SetHorizontalPositionAction_Execute);
            Values.Add(_horizontalPositionValue);
            Triggers.Add(_horizontalPositionValue);
            Actions.Add(_horizontalPositionValue);

            _verticalPositionValue = new HeliosValue(this, new BindingValue((double)VerticalSwitchPosition), "", "vertical position", "Current vertical position of the hat switch.", "Position 1 = Up, 2 = center, 3 = down.", BindingValueUnits.Numeric);
            _verticalPositionValue.Execute += new HeliosActionHandler(SetVerticalPositionAction_Execute);
            Values.Add(_verticalPositionValue);
            Triggers.Add(_verticalPositionValue);
            Actions.Add(_verticalPositionValue);

            _positionValue = new HeliosValue(this, new BindingValue((double)SwitchPosition), "", "position", "Current position of the hat switch.", "Position 0 = center, 1 = up, 2 = down, 3 = left,  or 4 = right.", BindingValueUnits.Numeric);
            _positionValue.Execute += new HeliosActionHandler(SetPositionAction_Execute);
            Values.Add(_positionValue);
            Triggers.Add(_positionValue);
            Actions.Add(_positionValue);

            _upTrigger = new HeliosTrigger(this, "", "up", "entered", "Triggered when the hat is moved into the up position.");
            Triggers.Add(_upTrigger);
            _upExitTrigger = new HeliosTrigger(this, "", "up", "exited", "Triggered when the hat is returning from the up position.");
            Triggers.Add(_upExitTrigger);
            _downTrigger = new HeliosTrigger(this, "", "down", "entered", "Triggered when the hat is moved into the down position.");
            Triggers.Add(_downTrigger);
            _downExitTrigger = new HeliosTrigger(this, "", "down", "exited", "Triggered when the hat is returning from the down position.");
            Triggers.Add(_downExitTrigger);
            _leftTrigger = new HeliosTrigger(this, "", "left", "entered", "Triggered when the hat is moved into the left position.");
            Triggers.Add(_leftTrigger);
            _leftExitTrigger = new HeliosTrigger(this, "", "left", "exited", "Triggered when the hat is returning from the left position.");
            Triggers.Add(_leftExitTrigger);
            _rightTrigger = new HeliosTrigger(this, "", "right", "entered", "Triggered when the hat is moved into the right position.");
            Triggers.Add(_rightTrigger);
            _rightExitTrigger = new HeliosTrigger(this, "", "right", "exited", "Triggered when the hat is returning from the right position.");
            Triggers.Add(_rightExitTrigger);
            _centerTrigger = new HeliosTrigger(this, "", "center", "entered", "Triggered when the hat is moved back to the center position.");
            Triggers.Add(_centerTrigger);
            _centerExitTrigger = new HeliosTrigger(this, "", "center", "exited", "Triggered when the hat is exiting from the center position.");
            Triggers.Add(_centerExitTrigger);



            UpdatePoints();
        }

        #region Properties

        public HatSwitchPosition SwitchPosition
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    HatSwitchPosition oldValue = _position;

                    if (!BypassTriggers)
                    {
                        switch (oldValue)
                        {
                            case HatSwitchPosition.Center:
                                _centerExitTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Up:
                                _upExitTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Down:
                                _downExitTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Left:
                                _leftExitTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Right:
                                _rightExitTrigger.FireTrigger(BindingValue.Empty);
                                break;
                        }
                    }

                    _position = value;
                    _positionValue.SetValue(new BindingValue(((int)_position).ToString(CultureInfo.InvariantCulture)), BypassTriggers);

                    if (!BypassTriggers)
                    {
                        switch (_position)
                        {
                            case HatSwitchPosition.Center:
                                HorizontalSwitchPosition = HatSwitchHorizontalPosition.Center;
                                VerticalSwitchPosition = HatSwitchVerticalPosition.Center;  
                                _centerTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Up:
                                VerticalSwitchPosition = HatSwitchVerticalPosition.Up;
                                _upTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Down:
                                VerticalSwitchPosition = HatSwitchVerticalPosition.Down;
                                _downTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Left:
                                HorizontalSwitchPosition = HatSwitchHorizontalPosition.Left;
                                _leftTrigger.FireTrigger(BindingValue.Empty);
                                break;
                            case HatSwitchPosition.Right:
                                HorizontalSwitchPosition= HatSwitchHorizontalPosition.Right;
                                _rightTrigger.FireTrigger(BindingValue.Empty);
                                break;
                        }
                    }

                    OnPropertyChanged("SwitchPosition", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public HatSwitchHorizontalPosition HorizontalSwitchPosition
        {
            get => _horizontalPosition;
            set {
                if (!_horizontalPosition.Equals(value))
                {
                    HatSwitchHorizontalPosition oldValue = _horizontalPosition;
                    _horizontalPosition = value;
                    _horizontalPositionValue.SetValue(new BindingValue(((int)_horizontalPosition).ToString(CultureInfo.InvariantCulture)), BypassTriggers);
                    OnPropertyChanged("HorizontalSwitchPosition", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }

        }
        public HatSwitchVerticalPosition VerticalSwitchPosition
        {
            get => _verticalPosition;
            set
            {

                if (!_verticalPosition.Equals(value))
                {
                    HatSwitchVerticalPosition oldValue = _verticalPosition;
                    _verticalPosition = value;
                    _verticalPositionValue.SetValue(new BindingValue(((int)_verticalPosition).ToString(CultureInfo.InvariantCulture)), BypassTriggers);
                    OnPropertyChanged("VerticalSwitchPosition", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        public string UpImage
        {
            get
            {
                return _upImage;
            }
            set
            {
                if ((_upImage == null && value != null)
                    || (_upImage != null && !_upImage.Equals(value)))
                {
                    string oldValue = _upImage;
                    _upImage = value;
                    OnPropertyChanged("UpImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string DownImage
        {
            get
            {
                return _downImage;
            }
            set
            {
                if ((_downImage == null && value != null)
                    || (_downImage != null && !_downImage.Equals(value)))
                {
                    string oldValue = _downImage;
                    _downImage = value;
                    OnPropertyChanged("DownImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string LeftImage
        {
            get
            {
                return _leftImage;
            }
            set
            {
                if (!_leftImage.Equals(value))
                {
                    string oldValue = _leftImage;
                    _leftImage = value;
                    OnPropertyChanged("LeftImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string RightImage
        {
            get
            {
                return _rightImage;
            }
            set
            {
                if ((_rightImage == null && value != null)
                    || (_rightImage != null && !_rightImage.Equals(value)))
                {
                    string oldValue = _rightImage;
                    _rightImage = value;
                    OnPropertyChanged("RightImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public string CenterImage
        {
            get
            {
                return _centerImage;
            }
            set
            {
                if ((_centerImage == null && value != null)
                    || (_centerImage != null && !_centerImage.Equals(value)))
                {
                    string oldValue = _centerImage;
                    _centerImage = value;
                    OnPropertyChanged("CenterImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        #endregion

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                UpdatePoints();
            }
            base.OnPropertyChanged(args);
        }

        public override void MouseDown(System.Windows.Point location)
        {
            if (IsPointInTriangle(location, _topLeft, _topRight, _middle))
            {
                SwitchPosition = HatSwitchPosition.Up;
            }
            else if (IsPointInTriangle(location, _topRight, _bottomRight, _middle))
            {
                SwitchPosition = HatSwitchPosition.Right;
            }
            else if (IsPointInTriangle(location, _bottomRight, _bottomLeft, _middle))
            {
                SwitchPosition = HatSwitchPosition.Down;
            }
            else if (IsPointInTriangle(location, _bottomLeft, _topLeft, _middle))
            {
                SwitchPosition = HatSwitchPosition.Left;
            }
        }

        public override void MouseDrag(System.Windows.Point location)
        {
            // No-Op
        }

        public override void MouseUp(System.Windows.Point location)
        {
            SwitchPosition = HatSwitchPosition.Center;
            HorizontalSwitchPosition = HatSwitchHorizontalPosition.Center;
            VerticalSwitchPosition = HatSwitchVerticalPosition.Center;
        }

        private void UpdatePoints()
        {
            _topLeft = new Point(0d, 0d);
            _topRight = new Point(Width, 0d);
            _middle = new Point(Width / 2d, Height / 2d);
            _bottomLeft = new Point(0d, Height);
            _bottomRight = new Point(Width, Height);
        }

        private bool IsPointInTriangle(Point p, Point v1, Point v2, Point v3)
        {
            if (Sign(p, v1, v2) < 0.0d) return false;
            if (Sign(p, v2, v3) < 0.0d) return false;
            if (Sign(p, v3, v1) < 0.0d) return false;

            return true;
        }

        private double Sign(Point v1, Point v2, Point v3)
        {
            return (v1.X - v3.X) * (v2.Y - v3.Y) - (v2.X - v3.X) * (v1.Y - v3.Y);
        }

        #region Actions

        void SetPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            try
            {
                SwitchPosition = (HatSwitchPosition)Enum.Parse(typeof(HatSwitchPosition), e.Value.StringValue);
                if (Enum.TryParse(e.Value.StringValue, out HatSwitchVerticalPosition verticalPosn)) {
                    VerticalSwitchPosition = verticalPosn;
                }
                else
                {
                    //VerticalSwitchPosition = HatSwitchVerticalPosition.Center;
                }
                if (Enum.TryParse(e.Value.StringValue, out HatSwitchHorizontalPosition horizontalPosn))
                {
                    HorizontalSwitchPosition = horizontalPosn;

                } else
                {
                    // HorizontalSwitchPosition = HatSwitchHorizontalPosition.Center;
                }
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }
        void SetHorizontalPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            try
            {
                HorizontalSwitchPosition = (HatSwitchHorizontalPosition)Enum.Parse(typeof(HatSwitchHorizontalPosition), e.Value.StringValue);
                SwitchPosition = (HatSwitchPosition)Enum.Parse(typeof(HatSwitchPosition), HorizontalSwitchPosition.ToString());
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }
        void SetVerticalPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            try
            {
                VerticalSwitchPosition = (HatSwitchVerticalPosition)Enum.Parse(typeof(HatSwitchVerticalPosition), e.Value.StringValue);
                SwitchPosition = (HatSwitchPosition)Enum.Parse(typeof(HatSwitchPosition), VerticalSwitchPosition.ToString());
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }
        #endregion

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("CenterImage", CenterImage);
            writer.WriteElementString("UpImage", UpImage);
            writer.WriteElementString("DownImage", DownImage);
            writer.WriteElementString("LeftImage", LeftImage);
            writer.WriteElementString("RightImage", RightImage);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);

            if (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                CenterImage = reader.ReadElementString("CenterImage");
                UpImage = reader.ReadElementString("UpImage");
                DownImage = reader.ReadElementString("DownImage");
                LeftImage = reader.ReadElementString("LeftImage");
                RightImage = reader.ReadElementString("RightImage");
            }
        }
    }
}
