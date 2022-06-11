//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

using System;
using GadrocsWorkshop.Helios.Windows.Controls;

namespace GadrocsWorkshop.Helios
{
    public class HeliosValue : NotificationObject, IBindingAction, IBindingTrigger, IHeliosValue, INamedBindingElement
    {
        private string _device;
        private string _name;

        /// <summary>
        /// an explicitly configured id that does not have to be equal to the name, or null to use the name as id
        /// </summary>
        private readonly string _explicitId;

        private readonly WeakReference _owner;
        private WeakReference _context = new WeakReference(null);

        /// <summary>
        /// true if we believe any trigger listeners are up to date, so that we don't fire triggers
        /// unless the value actually changes
        /// </summary>
        private bool _synchronized = GlobalOptions.HasUseLegacyResetBehavior;

        public HeliosValue(HeliosObject owner, BindingValue initialValue, string device, string name,
            string description, string valueDescription, BindingValueUnit unit)
            : this(owner, initialValue, device, null, name, description, valueDescription, unit)
        {
            // all code in referenced constructor
        }

        /// <summary>
        /// to be used if id is different from name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initialValue"></param>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="valueDescription"></param>
        /// <param name="unit"></param>
        public HeliosValue(HeliosObject owner, BindingValue initialValue, string device, string id, string name, string description, string valueDescription, BindingValueUnit unit)
        {
            _explicitId = id;
            _device = device;
            _name = name;
            ActionDescription = description;
            ActionValueDescription = valueDescription;

            _owner = new WeakReference(owner);
            Value = initialValue;
            Unit = unit;

            UpdateId();
            RecalculateName();
        }

        public void RecalculateName()
        {
            string ownerName = _owner.IsAlive ? Owner.Name : "";
            ActionBindingDescription = "set " + (Device.Length > 0 ? _device + " " : "") + _name + " on " +
                                       ownerName + " to  %value%";
        }

        private void UpdateId()
        {
            ValueID = _explicitId ?? _name;
            if (!string.IsNullOrEmpty(_device))
            {
                ActionID = $"{_device}.set.{ValueID}";
                ValueID = $"{_device}.{ValueID}";
            }
            else
            {
                ActionID = $"set.{ValueID}";
            }
            TriggerID = $"{ValueID}.changed";
        }

        /// <summary>
        /// Fired when a set action is called on this value object.
        /// </summary>
        public event HeliosActionHandler Execute;

        // XXX this is gross... we churn HeliosTriggerEventArgs and HeliosActionEventArgs objects for no reason.  let's have one EventArgs object that represents a value change
        // XXX and then we can reuse it through the action/trigger/action/trigger stack frames and even use it for tracing
        protected void OnFireTrigger(BindingValue value)
        {
            HeliosTriggerEventArgs args = new HeliosTriggerEventArgs(value);
            HeliosTriggerHandler handler = TriggerFired;
            handler?.Invoke(this, args);
        }

        /// <summary>
        /// Sets a new value for this helios value object
        /// </summary>
        /// <param name="value">Value to be sent to bindings.</param>
        /// <param name="bypassCascadingTriggers">True if bindings should not trigger further triggers.</param>
        public void SetValue(BindingValue value, bool bypassCascadingTriggers)
        {
            // factored this value out for readability
            bool valueChanged = (Value == null && value != null)
                                || (Value != null && !Value.Equals(value));

            if (bypassCascadingTriggers)
            {
                if (valueChanged)
                {
                    // a normal local write
                    Value = value;
                }

                // either way, we are done.  a local write never sets _synchronized
                return;
            }

            // NOTE: cases broken out for breakpointing
            if (!_synchronized)
            {
                // need to send this to our bound targets to synchronize them
                Value = value;
                _synchronized = true;
                OnFireTrigger(value);
                return;
            }

            if (valueChanged)
            {
                // new value of interest to our bound targets
                Value = value;
                OnFireTrigger(value);
            }
        }

        #region IHeliosValue Members

        public string ValueID { get; private set; }

        public BindingValue Value { get; private set; }

        public string ValueDescription
        {
            get => ActionValueDescription;
            set
            {
                if ((ActionValueDescription == null && value != null)
                    || (ActionValueDescription != null && !ActionValueDescription.Equals(value)))
                {
                    string oldValue = ActionValueDescription;
                    ActionValueDescription = value;
                    OnPropertyChanged("ValueDescription", oldValue, value, false);
                }
            }
        }

        #endregion

        #region IBindingElement Members

        public object Context
        {
            get => _context.Target;
            set => _context = new WeakReference(value);
        }

        public HeliosObject Owner => _owner.Target as HeliosObject;

        public string Device
        {
            get => _device;
            set
            {
                _device = value;
                UpdateId();
            }
        }

        public string Name
        {
            get => _name;

            set
            {
                string oldValue = _name;
                _name = value;
                OnPropertyChanged("Name", oldValue, value, false);
            }
        }

        public BindingValueUnit Unit { get; }

        #endregion

        #region IBindingAction Members

        public string ActionID { get; private set; }

        public string ActionName => "set " + _name;

        public string ActionVerb => "set";

        public HeliosObject Target => _owner.Target as HeliosObject;

        public string ActionDescription { get; }

        public bool ActionRequiresValue => true;

        public string ActionValueDescription { get; private set; }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="value">Value to be processed by this action.</param>
        /// <param name="bypassCascadingTriggers">If true this action will not fire additional triggers.</param>
        public void ExecuteAction(BindingValue value, bool bypassCascadingTriggers)
        {
            HeliosActionEventArgs args = new HeliosActionEventArgs(value, bypassCascadingTriggers);
            Execute?.Invoke(this, args);
        }

        public void Reset()
        {
            if (GlobalOptions.HasUseLegacyResetBehavior)
            {
                // just do nothing here, which happens only once on reset, so we don't have to
                // do anything special at update time, which happens continuously
                return;
            }
            // the next update to this value should fire triggers, even if the
            // actual value has not changed.  Observers of related triggers may be
            // out of sync, because the were reset to their configured initial states.
            _synchronized = false;
        }

        public Type ValueEditorType { get; set; } = typeof(TextStaticEditor);

        public string ActionBindingDescription { get; private set; }

        public string ActionInputBindingDescription => "to %value%";

        #endregion

        #region IBindingTrigger Members

        public event HeliosTriggerHandler TriggerFired;

        public string TriggerID { get; private set; }

        public string TriggerName => _name + " changed";

        public string TriggerVerb => "changed";

        public string TriggerDescription => ActionDescription;

        public string TriggerValueDescription => ActionValueDescription;

        public HeliosObject Source => _owner.Target as HeliosObject;

        public bool TriggerSuppliesValue => true;

        public string TriggerBindingDescription => "when" + (Device.Length > 0 ? " " + _device + " " : " ") + _name +
                                                   " on " + Owner.Name + " changes";

        #endregion
    }
}