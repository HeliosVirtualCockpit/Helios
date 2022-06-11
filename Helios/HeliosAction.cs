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
    /// Delegate to handle action invocation.
    /// </summary>
    public delegate void HeliosActionHandler(object action, HeliosActionEventArgs e);

    /// <summary>
    /// a bindable target-only property that can be attached to a Helios object
    /// </summary>
    public class HeliosAction : NotificationObject, IBindingAction, INamedBindingElement
    {
        /// <summary>
        /// an explicitly configured id that does not have to be equal to the name, or null to use the name as id
        /// </summary>
        private readonly string _explicitId;

        private string _device;
        private string _name;
        private string _bindingDescription = "";
        private string _inputBindingDescription = "";

        private readonly WeakReference _target;
        private WeakReference _context = new WeakReference(null);

        public HeliosAction(HeliosObject target, string device, string name, string verb, string description)
            : this(target, device, null, name, verb, description, "", BindingValueUnits.NoValue)
        {
            // all code in referenced constructor
        }

        public HeliosAction(HeliosObject target, string device, string name, string verb, string description,
            string valueDescription, BindingValueUnit unit)
            : this(target, device, null, name, verb, description, valueDescription, unit)
        {
            // all code in referenced constructor
        }

        /// <summary>
        /// constructor to be used when id does not match name
        /// </summary>
        /// <param name="target"></param>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="verb"></param>
        /// <param name="description"></param>
        /// <param name="valueDescription"></param>
        /// <param name="unit"></param>
        public HeliosAction(HeliosObject target, string device, string id, string name, string verb, string description, string valueDescription, BindingValueUnit unit)
        {
            _explicitId = id;
            _device = device;
            _target = new WeakReference(target);
            _name = name;
            ActionVerb = verb;
            ActionDescription = description;
            ActionValueDescription = valueDescription;
            Unit = unit;

            UpdateId();

            if (ActionRequiresValue)
            {
                ActionInputBindingDescription = "to %value%";
            }

            // NOTE: we do not subscribe to name changes on our target object, because we don't know when to unregister, as there is
            // no explicit cleanup of action objects
            RecalculateName();
        }

        public void RecalculateName()
        {
            string targetName = _target.IsAlive ? Target.Name : "";
            ActionBindingDescription = ActionVerb + (Device.Length > 0 ? " " + _device : "") +
                                       (_name.Length > 0 ? " " + _name + " on" : "") + " " + targetName +
                                       (ActionRequiresValue ? " to %value%" : "");
        }

        private void UpdateId()
        {
            ActionID = _explicitId ?? _name;
            string prefix = "";
            if (!string.IsNullOrEmpty(_device))
            {
                prefix = $"{_device}.";
            }
            if (ActionID.Length < 1)
            {
                // NOTE: this was allowed for some reason in original
                // code for HeliosAction, but it is unclear why
                // NOTE: this will generate the same id as a HeliosTrigger without a name
                ActionID = $"{prefix}{ActionVerb}";
            }
            else
            {
                ActionID = $"{prefix}{ActionVerb}.{ActionID}";
            }
        }

        public event HeliosActionHandler Execute;

        #region IBindingElement Members

        public object Context
        {
            get => _context.Target;
            set => _context = new WeakReference(value);
        }

        public HeliosObject Owner => Target;

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

        #endregion

        #region IBindingAction Members

        public string ActionID { get; private set; }

        public string ActionName => ActionVerb + " " + _name;

        /// <summary>
        /// Name used to identify this binding action. (Ex: Press Button 1)
        /// </summary>
        public string ActionVerb { get; }

        /// <summary>
        /// Target object which this action acts on.
        /// </summary>
        public HeliosObject Target => _target.Target as HeliosObject;

        /// <summary>
        /// Short description of what this action does.
        /// </summary>
        public string ActionDescription { get; }

        /// <summary>
        /// Description of the valid values that this action accepts.
        /// </summary>
        public string ActionValueDescription { get; }

        public string ActionBindingDescription
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

        public string ActionInputBindingDescription
        {
            get => _inputBindingDescription;
            set
            {
                if ((_inputBindingDescription == null && value != null)
                    || (_inputBindingDescription != null && !_inputBindingDescription.Equals(value)))
                {
                    string oldValue = _inputBindingDescription;
                    _inputBindingDescription = value;
                }
            }
        }

        public bool ActionRequiresValue => !((Unit == null) || (Unit.Equals(BindingValueUnits.NoValue)));

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="value">Value to be processed by this action.</param>
        /// <param name="bypassCascadingTriggers">If true this action will not fire additional triggers.</param>
        public void ExecuteAction(BindingValue value, bool bypassCascadingTriggers)
        {
            HeliosActionEventArgs args = new HeliosActionEventArgs(value, bypassCascadingTriggers);
            HeliosActionHandler handler = Execute;
            handler?.Invoke(this, args);
        }

        public void Reset()
        {
            // we don't cache values, no code
        }

        public Type ValueEditorType { get; set; } = null;

        #endregion
    }
}