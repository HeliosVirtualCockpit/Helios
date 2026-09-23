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
    using System;
    using System.Windows;

    public abstract class ToggleSwitchBase : HeliosVisual
    {
        protected enum SwitchAction
        {
            None,
            Increment,
            Decrement
        }

        private LinearClickType _clickType = LinearClickType.Swipe;

        private Point _mouseDownLocation;
        private bool _mouseAction;
        private ToggleSwitchOrientation _orientation;
        private bool _hasIndicator;
        private bool _indicatorOn;

        private HeliosValue _indicatorValue;
        private HeliosTrigger _releaseTrigger;

        protected ToggleSwitchBase(string name, Size nativeSize)
            : base(name, nativeSize)
        {
            _indicatorValue = new HeliosValue(this, new BindingValue(false), "", "indicator", "Current On/Off State for this buttons indicator.", "True if the indicator is on, otherwise false.", BindingValueUnits.Boolean);
            _indicatorValue.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorValue);

            _releaseTrigger = new HeliosTrigger(this, "", "", "released", "This trigger is fired when the user releases pressure on the switch (lifts finger or mouse button.).");
            Triggers.Add(_releaseTrigger);
        }

        #region Properties

        public LinearClickType ClickType
        {
            get
            {
                return _clickType;
            }
            set
            {
                if (!_clickType.Equals(value))
                {
                    LinearClickType oldValue = _clickType;
                    _clickType = value;
                    OnPropertyChanged("ClickType", oldValue, value, true);
                }
            }
        }

        public ToggleSwitchOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (!_orientation.Equals(value))
                {
                    ToggleSwitchOrientation oldValue = _orientation;
                    _orientation = value;
                    OnPropertyChanged("Orientation", oldValue, value, true);
                }
            }
        }

        public bool HasIndicator
        {
            get
            {
                return _hasIndicator;
            }
            set
            {
                if (!_hasIndicator.Equals(value))
                {
                    bool oldValue = _hasIndicator;

                    _hasIndicator = value;

                    if (value && !Actions.Contains(_indicatorValue))
                    {
                        Actions.Add(_indicatorValue);
                    }
                    else if (!value && Actions.Contains(_indicatorValue))
                    {
                        Actions.Remove(_indicatorValue);
                    }

                    OnPropertyChanged("HasIndicator", oldValue, value, true);
                }
            }
        }

        public bool IndicatorOn
        {
            get
            {
                return _indicatorOn;
            }
            set
            {
                if (!_indicatorOn.Equals(value))
                {
                    bool oldValue = _indicatorOn;
                    _indicatorOn = value;
                    OnPropertyChanged("IndicatorOn", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }

        #endregion 

        void Indicator_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            IndicatorOn = e.Value.BoolValue;
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        public override void MouseDown(Point location)
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
                SwitchAction action = SwitchAction.Increment;

                switch (Orientation)
                {
                    case ToggleSwitchOrientation.Vertical:
                        if (location.Y < Height / 2d)
                        {
                            action = SwitchAction.Decrement;
                        }
                        break;

                    case ToggleSwitchOrientation.VerticalReversed:
                        if (location.Y > Height / 2d)
                        {
                            action = SwitchAction.Decrement;
                        }
                        break;

                    case ToggleSwitchOrientation.Horizontal:
                        if (location.X < Width / 2d)
                        {
                            action = SwitchAction.Decrement;
                        }
                        break;

                    case ToggleSwitchOrientation.HorizontalReversed:
                        if (location.X > Width / 2d)
                        {
                            action = SwitchAction.Decrement;
                        }
                        break;
                }

                if (action != SwitchAction.None)
                {
                    ThrowSwitch(action);
                    _mouseAction = true;
                }
            }

            else if (ClickType == LinearClickType.OneTouch)
            {
                ThrowSwitch(SwitchAction.Increment);
                _mouseAction = true;
            }
        }

        public override void MouseDrag(Point location)
        {
            if (ClickType == LinearClickType.Swipe)
            {
                Vector swipeVector = location - _mouseDownLocation;

                SwitchAction action = SwitchAction.None;
                switch (Orientation)
                {
                    case ToggleSwitchOrientation.Vertical:
                        if (swipeVector.Y < -5)
                        {
                            action = SwitchAction.Decrement;
                        }
                        else if (swipeVector.Y > 5)
                        {
                            action = SwitchAction.Increment;
                        }
                        break;

                    case ToggleSwitchOrientation.VerticalReversed:
                        if (swipeVector.Y < -5)
                        {
                            action = SwitchAction.Increment;
                        }
                        else if (swipeVector.Y > 5)
                        {
                            action = SwitchAction.Decrement;
                        }
                        break;

                    case ToggleSwitchOrientation.Horizontal:
                        if (swipeVector.X < -5)
                        {
                            action = SwitchAction.Decrement;
                        }
                        else if (swipeVector.X > 5)
                        {
                            action = SwitchAction.Increment;
                        }
                        break;

                    case ToggleSwitchOrientation.HorizontalReversed:
                        if (swipeVector.X < -5)
                        {
                            action = SwitchAction.Increment;
                        }
                        else if (swipeVector.X > 5)
                        {
                            action = SwitchAction.Decrement;
                        }
                        break;
                }

                if (_mouseAction == false && action != SwitchAction.None)
                {
                    ThrowSwitch(action);
                    _mouseAction = true;
                }
            }
        }

        public override void MouseUp(Point location)
        {
            if (_mouseAction)
            {
                _releaseTrigger.FireTrigger(BindingValue.Empty);
            }
        }

        protected abstract void ThrowSwitch(SwitchAction action);
    }
}
