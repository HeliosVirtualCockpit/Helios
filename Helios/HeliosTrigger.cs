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

namespace GadrocsWorkshop.Helios
{
    /// <summary>
    /// Abstract base class for all triggers.  Trigger represent changes in there source which
    /// can be used to trigger other actions.
    /// </summary>
    public class HeliosTrigger : NotificationObject, IBindingTrigger, INamedBindingElement
    {
        private string _device;
        private string _name;

        /// <summary>
        /// an explicitly configured id that does not have to be equal to the name, or null to use the name as id
        /// </summary>
        private readonly string _explicitId;

        private string _bindingDescription;

        private readonly WeakReference _source;
        private WeakReference _context = new WeakReference(null);

        public HeliosTrigger(HeliosObject source, string device, string name, string verb, string description)
            : this(source, device, null, name, verb, description, "", BindingValueUnits.NoValue)
        {
            // all code in referenced constructor
        }

        public HeliosTrigger(HeliosObject source, string device, string name, string verb, string description,
            string valueDescription, BindingValueUnit unit)
            : this(source, device, null, name, verb, description, valueDescription, unit)
        {
            // all code in referenced constructor
        }

        public HeliosTrigger(HeliosObject source, string device, string id, string name, string verb, string description,
            string valueDescription, BindingValueUnit unit)
        {
            _explicitId = id;
            _device = device;
            _name = name;
            TriggerVerb = verb;
            TriggerDescription = description;
            _source = new WeakReference(source);
            TriggerValueDescription = valueDescription;
            Unit = unit;
            UpdateId();

            // NOTE: we do not subscribe to name changes on our target object, because we don't know when to unregister, as there is
            // no explicit cleanup of action objects
            RecalculateName();
        }

        public void RecalculateName()
        {
            string sourceName = _source.IsAlive ? Source.Name : "";
            TriggerBindingDescription = "when" + (Device.Length > 0 ? " " + _device : "") +
                                        (_name.Length > 0 ? " " + _name + " on" : "") + " " + sourceName + " " +
                                        TriggerVerb;
        }

        private void UpdateId()
        {
            TriggerID = _explicitId ?? _name;
            string prefix = "";
            if (!string.IsNullOrEmpty(_device))
            {
                prefix = $"{_device}.";
            }
            if (TriggerID.Length < 1)
            {
                // NOTE: this was allowed for some reason in original
                // code for HeliosTrigger, but it is unclear why
                // NOTE: this will generate the same id as a HeliosAction without a name
                TriggerID = $"{prefix}{TriggerVerb}";
            }
            else
            {
                TriggerID = $"{prefix}{TriggerID}.{TriggerVerb}";
            }
        }

        #region IBindingElement Members

        public object Context
        {
            get => _context.Target;
            set => _context = new WeakReference(value);
        }

        public HeliosObject Owner => _source.Target as HeliosObject;

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
                UpdateId();
            }
        }


        public BindingValueUnit Unit { get; }

        public void Reset()
        {
            // we don't cache anything, no code
        }

        #endregion

        #region IBindingTrigger Members

        /// <summary>
        /// Event which is fired when ever the trigger is activated.
        /// </summary>
        public event HeliosTriggerHandler TriggerFired;

        public string TriggerID { get; private set; }

        public string TriggerName => string.IsNullOrEmpty(_name)? TriggerVerb : $"{_name} {TriggerVerb}";

        /// <summary>
        /// Name used to identify this binding trigger. (Ex: Button 1 Pressed)
        /// </summary>
        public string TriggerVerb { get; }

        /// <summary>
        /// Gets the description of when this trigger is fired.
        /// </summary>
        public string TriggerDescription { get; set; }

        /// <summary>
        /// Gets the description of the contents of the supplied value when this trigger is fired.
        /// </summary>
        public string TriggerValueDescription { get; }

        /// <summary>
        /// Source object which fires this trigger.
        /// </summary>
        public HeliosObject Source => _source.Target as HeliosObject;

        public bool TriggerSuppliesValue => !((Unit == null) || (Unit.Equals(BindingValueUnits.NoValue)));

        public string TriggerBindingDescription
        {
            get => _bindingDescription;
            set
            {
                if ((_bindingDescription == null && value != null)
                    || (_bindingDescription != null && !_bindingDescription.Equals(value)))
                {
                    string oldValue = _bindingDescription;
                    _bindingDescription = value;
                }
            }
        }

        #endregion

        public void FireTrigger(BindingValue value)
        {
            HeliosTriggerEventArgs args = new HeliosTriggerEventArgs(value);
            HeliosTriggerHandler handler = TriggerFired;
            handler?.Invoke(this, args);
        }
    }
}