//  Copyright 2014 Craig Courtney
//  Copyright 2025 Helios Contributors
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
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System;
    using System.Xml;

    [HeliosControl("Helios.Base.LockingFourWayToggleSwitch", "Locking 4 Position (Center Off) Toggle Switch", "Toggle Switches (Multi-Way)", typeof(FourWayToggleSwitchRenderer), HeliosControlFlags.None)]
    public class FourWayToggleSwitchLocking : FourWayToggleSwitch
    {
        private HeliosValue _positionOneLockValue;
        private HeliosValue _positionTwoLockValue;
        private HeliosValue _positionThreeLockValue;
        private HeliosValue _positionCenterLockValue;

        private bool _positionOneLocked = false; 
        private bool _positionTwoLocked = false;
        private bool _positionThreeLocked = false;
        private bool _positionCenterLocked = false;
        public FourWayToggleSwitchLocking() : this("Locking Four Way Toggle Switch", new System.Windows.Size(100, 100)) {}
        public FourWayToggleSwitchLocking(string name, System.Windows.Size size)
            : base(name, size)
        {
            _positionOneLockValue = new HeliosValue(this, BindingValue.Empty, "", "position one lock", "Lock Position 1.", "True / False", BindingValueUnits.Boolean);
            _positionOneLockValue.Execute += PositionOneLock_Execute;
            Values.Add(_positionOneLockValue);
            Actions.Add(_positionOneLockValue);
            _positionTwoLockValue = new HeliosValue(this, BindingValue.Empty, "", "position two lock", "Lock Position 2.", "True / False", BindingValueUnits.Boolean);
            _positionTwoLockValue.Execute += PositionTwoLock_Execute;
            Values.Add(_positionTwoLockValue);
            Actions.Add(_positionTwoLockValue);
            _positionThreeLockValue = new HeliosValue(this, BindingValue.Empty, "", "position three lock", "Lock Position 3.", "True / False", BindingValueUnits.Boolean);
            _positionThreeLockValue.Execute += PositionThreeLock_Execute;
            Values.Add(_positionThreeLockValue);
            Actions.Add(_positionThreeLockValue);
            _positionCenterLockValue = new HeliosValue(this, BindingValue.Empty, "", "Center position lock", "Lock Center Position.", "True / False", BindingValueUnits.Boolean);
            _positionCenterLockValue.Execute += PositionCenterLock_Execute;
            Values.Add(_positionCenterLockValue);
            Actions.Add(_positionCenterLockValue);

        }

        #region Properties

        #endregion

        void PositionOneLock_Execute(object action, HeliosActionEventArgs e)
        {
            _positionOneLocked = e.Value.BoolValue;
        }
        void PositionTwoLock_Execute(object action, HeliosActionEventArgs e)
        {
            _positionTwoLocked = e.Value.BoolValue;
        }
        void PositionThreeLock_Execute(object action, HeliosActionEventArgs e)
        {
            _positionThreeLocked = e.Value.BoolValue;
        }
        void PositionCenterLock_Execute(object action, HeliosActionEventArgs e)
        {
            _positionCenterLocked = e.Value.BoolValue;
        }
        #region HeliosControl Implementation

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            SwitchPosition = DefaultPosition;
            EndTriggerBypass(true);
        }

        protected override void ThrowSwitch(ToggleSwitchBase.SwitchAction action)
        {
            if (action == SwitchAction.Increment)
            {
                switch (SwitchPosition)
                {
                    case FourWayToggleSwitchPosition.One:
                        if (!_positionTwoLocked)
                        {
                            SwitchPosition = FourWayToggleSwitchPosition.Two;
                        }
                        break;
                    case FourWayToggleSwitchPosition.Two:
                        if (!_positionThreeLocked)
                        {
                            SwitchPosition = FourWayToggleSwitchPosition.Three;
                        }
                        break;
                }                
            }
            else if (action == SwitchAction.Decrement)
            {
                switch (SwitchPosition)
                {
                    case FourWayToggleSwitchPosition.Two:
                        if (!_positionOneLocked)
                        {
                            SwitchPosition = FourWayToggleSwitchPosition.One;
                        }
                        break;
                    case FourWayToggleSwitchPosition.Three:
                        if (!_positionTwoLocked)
                        {
                            SwitchPosition = FourWayToggleSwitchPosition.Two;
                        }
                        break;
                }
            }
        }

        public override void MouseUp(System.Windows.Point location)
        {
            base.MouseUp(location);

            switch (SwitchPosition)
            {
                case FourWayToggleSwitchPosition.One:
                    if (SwitchType1 == FourWayToggleSwitchType.Mom || SwitchType1 == FourWayToggleSwitchType.On)
                    {
                        if (!_positionTwoLocked)
                        {
                            SwitchPosition = FourWayToggleSwitchPosition.Two;
                        }
                    }
                    break;
                case FourWayToggleSwitchPosition.Three:
                    if (SwitchType3 == FourWayToggleSwitchType.On || SwitchType3 == FourWayToggleSwitchType.Mom)
                    {
                        if (!_positionTwoLocked)
                        {
                            SwitchPosition = FourWayToggleSwitchPosition.Two;
                        }
                    }
                    break;
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
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
    }
}
