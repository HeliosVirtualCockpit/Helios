//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios
{
    using NLua;
    using NLua.Exceptions;
    using System;

    /// <summary>
    /// A binding from a source trigger to a target action
    ///
    /// This object implements its own view model, so there are several properties that only exist for the
    /// benefit of the user interface and logging.
    /// </summary>
    public class HeliosBinding : NotificationObject, INamedBindingElement
    {
        private Lua _luaInterpreter;
        private bool _active = true;
        private WeakReference _triggerSource = new WeakReference(null);
        private WeakReference _targetAction = new WeakReference(null);
        private bool _bypassTargetTriggers;
        private BindingValueSources _valueSource;
        private BindingValue _value = BindingValue.Empty;
        private string _condition = "";

        private bool _needsConversion = false;
        private BindingValueUnitConverter _converter = null;

        private bool _valid = false;
        private string _error = "";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // deserialization constructor
        public HeliosBinding()
        {
            // no code in base
        }

        // constructor used when building new binding
        public HeliosBinding(IBindingTrigger trigger, IBindingAction action)
            : this()
        {
            Trigger = trigger;
            Action = action;
        }

        public void RecalculateName()
        {
            // just signal all these out of date and let any clients generate the full versions they care about
            OnPropertyChanged("Description", null, Description, false);
            OnPropertyChanged("InputDescription", null, Description, false);
            OnPropertyChanged("OutputDescription", null, Description, false);
            OnPropertyChanged("LongDescription", null, Description, false);
        }

        public void Reset()
		{
            // this will reset all lua variables by initiating the creation of a new instance of the Lua Interpreter
            _luaInterpreter = null;
        }

        #region Properties

        /// <summary>
        /// if this is set to non-null (only safe on main thread and only for developer use) then the
        /// bindings will call the provided interface for more expensive binding tracing
        /// </summary>
        public static IHeliosBindingTracer BindingTracer { get; set; }

        /// <summary>
        /// true if this binding is currently on the stack of bindings being evaluated
        /// </summary>
        public bool IsExecuting { get; private set; }

        public string Description => IsValid
            ? ReplaceValue(Trigger.TriggerBindingDescription + " " + Action.ActionBindingDescription)
            : ErrorMessage;

        public string InputDescription => IsValid
            ? ReplaceValue(Action.ActionInputBindingDescription + " " + Trigger.TriggerBindingDescription)
            : ErrorMessage;

        public string OutputDescription => IsValid
            ? ReplaceValue(Action.ActionBindingDescription)
            : ErrorMessage;

        /// <summary>
        /// extremely expensive generator for a string that the caller is expected to cache and only read again when we signal a property change
        /// </summary>
        public string LongDescription=> IsValid
            ? $"binding from '{SourcePath}' to '{TargetPath}'"
            : ErrorMessage;

        public string ErrorMessage
        {
            get => _error;
            private set
            {
                if ((_error != null || value == null) && (_error == null || _error.Equals(value)))
                {
                    return;
                }

                string oldValue = _error;
                _error = value;
                OnPropertyChanged("ErrorMessage", oldValue, value, false);
                RecalculateName();
            }
        }

        public bool IsValid
        {
            get => _valid;
            private set
            {
                if (_valid.Equals(value))
                {
                    return;
                }

                bool oldValue = _valid;
                _valid = value;
                OnPropertyChanged("IsValid", oldValue, value, false);
                RecalculateName();
            }
        }

        public bool IsActive
        {
            get => _active;
            set
            {
                if (!_active.Equals(value))
                {
                    bool oldValue = _active;
                    _active = value;
                    OnPropertyChanged("IsActive", oldValue, value, false);
                }
            }
        }

        public bool HasCondition => Condition != null && Condition.Trim().Length > 0;

        public string Condition
        {
            get => _condition;
            set
            {
                if ((_condition == null && value != null)
                    || (_condition != null && !_condition.Equals(value)))
                {
                    string oldValue = _condition;
                    _condition = value;
                    OnPropertyChanged("Condition", oldValue, value, true);
                }
            }
        }

        public BindingValueSources ValueSource
        {
            get => _valueSource;
            set
            {
                if (!_valueSource.Equals(value))
                {
                    BindingValueSources oldValue = _valueSource;
                    _valueSource = value;
                    Validate();
                    OnPropertyChanged("ValueSource", oldValue, value, true);
                    RecalculateName();
                }
            }
        }

        public string Value
        {
            get => _value.StringValue;
            set
            {
                if ((_value == null && value != null)
                    || (_value != null && !_value.Equals(value)))
                {
                    string oldValue = _value.StringValue;
                    _value = new BindingValue(value);
                    OnPropertyChanged("Value", oldValue, value, true);
                    Validate();
                    if (ValueSource == BindingValueSources.StaticValue)
                    {
                        RecalculateName();
                    }
                }
            }
        }

        public IBindingTrigger Trigger
        {
            get => _triggerSource.Target as IBindingTrigger;
            set
            {
                IBindingTrigger oldTrigger = _triggerSource.Target as IBindingTrigger;
                if ((oldTrigger == null && value != null)
                    || (oldTrigger != null && !oldTrigger.Equals(value)))
                {

                    _triggerSource = new WeakReference(value);

                    UpdateConverter();
                    Validate();

                    OnPropertyChanged("Trigger", oldTrigger, value, true);
                    RecalculateName();
                }
            }
        }

        public IBindingAction Action
        {
            get => _targetAction.Target as IBindingAction;
            set
            {
                IBindingAction oldAction = _targetAction.Target as IBindingAction;

                if ((oldAction == null && value != null)
                    || (oldAction != null && !oldAction.Equals(value)))
                {
                    _targetAction = new WeakReference(value);

                    UpdateConverter();
                    Validate();

                    OnPropertyChanged("Action", oldAction, value, true);
                    RecalculateName();
                }
            }
        }

        public bool BypassCascadingTriggers
        {
            get => _bypassTargetTriggers;
            set
            {
                if (!_bypassTargetTriggers.Equals(value))
                {
                    bool oldBypass = _bypassTargetTriggers;
                    _bypassTargetTriggers = value;
                    // XXX nobody is listening to these property changes on an Interface's OutputBindings, so it never generates dirty/undo
                    OnPropertyChanged("BypassCascadingTriggers", oldBypass, value, true);
                }
            }
        }

        private Lua LuaInterpreter
        {
            get
            {
                if (_luaInterpreter == null)
                {
                    _luaInterpreter = new Lua();

                    // add lua environment variables
                    _luaInterpreter.DoString("HeliosPath = " + "'" + ConfigManager.DocumentPath.Replace("\\", "\\\\") + "'");
                    _luaInterpreter.DoString("BMSFalconPath = " + "'" + ConfigManager.BMSFalconPath.Replace("\\", "\\\\") + "'");
                }
                return _luaInterpreter;
            }
        }

        #endregion

        private string ReplaceValue(string inputString)
        {
            string valueString;
            switch (ValueSource)
            {
                case BindingValueSources.StaticValue:
                    valueString = Value;
                    break;
                case BindingValueSources.TriggerValue:
                    valueString = "trigger value";
                    break;
                case BindingValueSources.LuaScript:
                    valueString = "script results";
                    break;
                default:
                    valueString = "";
                    break;
            }

            return inputString?.Replace("%value%", valueString);
        }

        private BindingValue CreateBindingValue(object value)
        {
            switch (value)
            {
                case string stringValue:
                    return new BindingValue(stringValue);
                case double doubleValue:
                    return new BindingValue(doubleValue);
                case bool boolValue:
                    return new BindingValue(boolValue);
                case long longValue:
                    // construct double from long
                    return new BindingValue(longValue);
            }

            return BindingValue.Empty;
        }

        // log filters to log certain messages only once per binding, because otherwise we will crush the logger and 
        // keep the program from running under load
        private static readonly Util.OnceLogger TriggeredLogger = new Util.OnceLogger(Logger);
        private static readonly Util.OnceLogger LuaConditionLogger = new Util.OnceLogger(Logger);
        private static readonly Util.OnceLogger LuaValueLogger = new Util.OnceLogger(Logger);
        private static readonly Util.OnceLogger LuaNoValueLogger = new Util.OnceLogger(Logger);

        /// <summary>
        /// this object defers formatting of the complex description of a binding until ToString is actually called, meaning this information
        /// is actually being logged
        /// </summary>
        private class DeferredBindingDescription
        {
            private readonly HeliosBinding _binding;

            public DeferredBindingDescription(HeliosBinding binding)
            {
                _binding = binding;
            }

            public override string ToString()
            {
                return $"{_binding.Description} (from '{_binding.SourcePath}' to '{_binding.TargetPath}')";
            }
        }

        private string SourcePath => Trigger?.Source != null
            ? HeliosSerializer.GetDescriptivePath(Trigger.Source)
            : "null";

        private string TargetPath => Action?.Target != null 
            ? HeliosSerializer.GetDescriptivePath(Action.Target) 
            : "null";

        public void OnTriggerFired(object trigger, HeliosTriggerEventArgs e)
        {
            if (!IsActive)
            {
                return;
            }

            string loggingId = ((IBindingTrigger)_triggerSource.Target).TriggerID;
            BindingTracer?.TraceTriggerFired(this);

            // scope of binding trace
            try
            {
                if (IsExecuting)
                {
                    // target of trigger is currently on the stack
                    Logger.Warn("Binding loop condition detected, binding {Binding} aborted.",
                        new HeliosBinding.DeferredBindingDescription(this));
                    return;
                }

                TriggeredLogger.InfoOnceUnlessDebugging(loggingId, "Binding {Binding} triggered with value {Value}",
                    new DeferredBindingDescription(this), e.Value.StringValue);
                IsExecuting = true;

                // scope of IsExecuting
                try
                {
                    BindingValue value = BindingValue.Empty;

                    if (HasCondition)
                    {
                        try
                        {
                            InitializeLua(e);
                            object[] conditionReturnValues = LuaInterpreter.DoString(_condition);
                            if (conditionReturnValues.Length >= 1)
                            {
                                BindingValue returnValue = CreateBindingValue(conditionReturnValues[0]);
                                if (returnValue.BoolValue == false)
                                {
                                    if (Logger.IsDebugEnabled)
                                    {
                                        Logger.Debug("Binding condition evaluated to false, binding {Binding} aborted.",
                                            new DeferredBindingDescription(this));
                                    }
                                    return;
                                }
                            }
                        }
                        catch (LuaScriptException luaException)
                        {
                            LuaConditionLogger.WarnOnceUnlessDebugging(loggingId,
                                "Binding condition Lua error {Error} from script {Script} on binding {Binding}",
                                luaException.Message, Condition, new DeferredBindingDescription(this));
                        }
                        catch (Exception conditionException)
                        {
                            Logger.Error(conditionException,
                                "Binding condition has thown an unhandled exception for binding {Binding} with condition {Condition}",
                                new DeferredBindingDescription(this),
                                Condition);
                            return;
                        }
                    }

                    switch (ValueSource)
                    {
                        case BindingValueSources.StaticValue:
                            value = _value;
                            break;
                        case BindingValueSources.TriggerValue:
                            if (_needsConversion && _converter != null)
                            {
                                value = _converter.Convert(e.Value, Trigger.Unit, Action.Unit);
                                if (Logger.IsDebugEnabled)
                                {
                                    Logger.Debug("Binding {Binding} converted value {Original} to {NewValue}",
                                        new DeferredBindingDescription(this), e.Value.StringValue, value.StringValue);
                                }
                            }
                            else
                            {
                                value = e.Value;
                            }

                            break;
                        case BindingValueSources.LuaScript:
                            try
                            {
                                InitializeLua(e);

                                object[] returnValues = LuaInterpreter.DoString(Value);
                                if ((returnValues != null) && (returnValues.Length >= 1))
                                {
                                    value = CreateBindingValue(returnValues[0]);
                                    if (Logger.IsDebugEnabled)
                                    {
                                        Logger.Debug(
                                            "Lua script for binding {Binding} evaluated {Expression} for value {TriggerValue} and got result {ReturnValue}",
                                            new DeferredBindingDescription(this), Value, LuaInterpreter["TriggerValue"],
                                            returnValues[0]);
                                    }
                                }
                                else
                                {
                                    LuaNoValueLogger.WarnOnceUnlessDebugging(loggingId,
                                        "Binding value Lua script did not return a value from script {Script} for trigger value {TriggerValue} on binding {Binding}",
                                        Value, LuaInterpreter["TriggerValue"], new DeferredBindingDescription(this));
                                }
                            }
                            catch (LuaScriptException luaException)
                            {
                                LuaValueLogger.WarnOnceUnlessDebugging(loggingId,
                                    "Binding value Lua error {Error} from script {Script} for trigger value {TriggerValue} on binding {Binding}",
                                    luaException.Message, Value, LuaInterpreter["TriggerValue"],
                                    new DeferredBindingDescription(this));
                            }
                            catch (Exception valueException)
                            {
                                // these are exceptions thrown by the Lua implementation
                                Logger.Error(valueException,
                                    "Binding value Lua script has thown an unhandled exception from script {Script} for trigger value {triggerValue} on binding {Binding}",
                                    Value, LuaInterpreter["TriggerValue"], new DeferredBindingDescription(this));
                            }

                            break;
                    }

                    Action.ExecuteAction(value, BypassCascadingTriggers);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Unhandled exception thrown from binding {Binding}",
                        new DeferredBindingDescription(this));
                }
                finally
                {
                    IsExecuting = false;
                }
            }
            finally
            {
                BindingTracer?.EndTraceTriggerFired(this);
            }
        }

        // we call this function on any paths that use Lua as an optimization, so we don't allocate Lua unless we need it
        private void InitializeLua(HeliosTriggerEventArgs e)
        {
            //LuaInterpreter["TriggerValue"] = e.Value.NaitiveValue;
            switch (e.Value.NaitiveType)
            {
                case BindingValueType.Boolean:
                    LuaInterpreter["TriggerValue"] = e.Value.BoolValue;
                    break;
                case BindingValueType.String:
                    LuaInterpreter["TriggerValue"] = e.Value.StringValue;
                    break;
                case BindingValueType.Double:
                    LuaInterpreter["TriggerValue"] = e.Value.DoubleValue;
                    break;
            }
        }


        public void Clone(HeliosBinding binding)
        {
            Trigger = binding.Trigger;
            Action = binding.Action;
            ValueSource = binding.ValueSource;
            Value = binding.Value;
            BypassCascadingTriggers = binding.BypassCascadingTriggers;
        }

        private void UpdateConverter()
        {
            if (Trigger != null && Action != null)
            {
                _needsConversion = !Trigger.Unit.Equals(Action.Unit);
                _converter = ConfigManager.ModuleManager.GetUnitConverter(Trigger.Unit, Action.Unit);
            }
            else
            {
                _needsConversion = false;
                _converter = null;
            }
        }

        private void Validate()
        {
            if (Trigger == null)
            {
                IsValid = false;
                ErrorMessage = "Invalid Trigger - Please select a new trigger event.";
                return;
            }

            if (Action == null)
            {
                IsValid = false;
                ErrorMessage = "Invalid Action - Please select a new action.";
                return;
            }

            if (Action.ActionRequiresValue)
            {
                if (ValueSource == BindingValueSources.TriggerValue && _needsConversion && _converter == null)
                {
                    IsValid = true;
                    ErrorMessage = "Action Value Warning - Cannot convert trigger value to action value.";
                    return;
                }

                if (ValueSource == BindingValueSources.TriggerValue 
                    && _needsConversion 
                    && _converter.IsRaw)
                {
                    IsValid = true;
                    ErrorMessage = "Disregarding Units - Scaling may need to be adjusted by another control or Lua script.";
                    return;
                }

                if (ValueSource == BindingValueSources.StaticValue && (Value == null || Value.Length == 0))
                {
                    IsValid = true;
                    ErrorMessage = "Action Value Warning - Value cannot be empty.";
                    return;
                }

                if (ValueSource == BindingValueSources.LuaScript && (Value == null || Value.Length == 0))
                {
                    IsValid = true;
                    ErrorMessage = "Action Value Warning - Script cannot be empty.";
                    return;
                }
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                IsValid = true;
                ErrorMessage = "";
            }
        }

        public interface IHeliosBindingTracer
        {
            void TraceTriggerFired(HeliosBinding heliosBinding);
            void EndTraceTriggerFired(HeliosBinding heliosBinding);
        }
    }
}
