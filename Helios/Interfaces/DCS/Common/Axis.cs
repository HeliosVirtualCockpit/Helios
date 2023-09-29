//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
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
using System.Globalization;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class Axis : DCSFunctionWithActions
    {
        private readonly string _id;
        private readonly string _format;

        private string _actionData;

        [JsonProperty("argumentValue")] public double StepValue { get; protected set; }

        [JsonProperty("argumentMin")] public double ArgumentMin { get; protected set; }

        [JsonProperty("argumentMax")] public double ArgumentMax { get; protected set; }

        [JsonProperty("loop")]
        protected bool _loop;

        private HeliosValue _value;

        private HeliosAction _incrementAction;
        private HeliosAction _decrementAction;

        public Axis(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double stepValue, double argMin, double argMax, string device, string name)
            : this(sourceInterface, deviceId, buttonId, argId, stepValue, argMin, argMax, device, name, false, "%.3f")
        {
        }

        public Axis(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double stepValue, double argMin, double argMax, string device, string name, bool loop, string exportFormat)
            : base(sourceInterface, device, name, "Current value of this axis.")
        {
            _id = argId;
            _format = exportFormat;
            _loop = loop;
            StepValue = stepValue;
            ArgumentMin = argMin;
            ArgumentMax = argMax;
            SerializedActions.Add("set", new SerializedAction()
            {
                DeviceID = deviceId,
                ActionID = buttonId
            });
            DoBuild();
        }

        // deserialization constructor
        public Axis(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            DoBuild();
        }

        private void DoBuild()
        {
            _actionData = "C" + SerializedActions["set"].DeviceID + "," + SerializedActions["set"].ActionID + ",";

            _value = new HeliosValue(SourceInterface, new BindingValue(0.0d), SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, ArgumentMin.ToString(CultureInfo.CurrentCulture) + " to " + ArgumentMax.ToString(CultureInfo.CurrentCulture), BindingValueUnits.Numeric);
            _value.Execute += new HeliosActionHandler(Value_Execute);
            Values.Add(_value);
            Actions.Add(_value);
            Triggers.Add(_value);

            _incrementAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "increment",
                "Increments this axis value.", "Amount to increment by (Default:" + StepValue + ")", BindingValueUnits.Numeric);
            _incrementAction.Execute += new HeliosActionHandler(IncrementAction_Execute);
            Actions.Add(_incrementAction);

            _decrementAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "decrement",
                "Decrement this axis value.", "Amount to decrement by (Default:" + -StepValue + ")", BindingValueUnits.Numeric);
            _decrementAction.Execute += new HeliosActionHandler(DecrementAction_Execute);
            Actions.Add(_decrementAction);
        }

        void DecrementAction_Execute(object action, HeliosActionEventArgs e)
        {
            double newValue = _value.Value.DoubleValue - StepValue;
            if (_loop)
            {
                while (newValue < ArgumentMin)
                {
                    newValue += (ArgumentMax - ArgumentMin);
                }
            }
            else
            {
                newValue = Math.Max(ArgumentMin, newValue);
            }
            _value.SetValue(new BindingValue(newValue), e.BypassCascadingTriggers);
            SourceInterface.SendData(_actionData + _value.Value.DoubleValue.ToString(CultureInfo.InvariantCulture));
        }

        void IncrementAction_Execute(object action, HeliosActionEventArgs e)
        {
            double newValue = _value.Value.DoubleValue + StepValue;
            if (_loop)
            {
                while (newValue > ArgumentMax)
                {
                    newValue -= (ArgumentMax - ArgumentMin);
                }
            }
            else
            {
                newValue = Math.Min(ArgumentMax, newValue);
            }
            _value.SetValue(new BindingValue(newValue), e.BypassCascadingTriggers);
            SourceInterface.SendData(_actionData + _value.Value.DoubleValue.ToString(CultureInfo.InvariantCulture));
        }

        void Value_Execute(object action, HeliosActionEventArgs e)
        {
            _value.SetValue(e.Value, e.BypassCascadingTriggers);
            SourceInterface.SendData(_actionData + _value.Value.DoubleValue.ToString(CultureInfo.InvariantCulture));
        }

        public override void ProcessNetworkData(string id, string value)
        {
            if (!double.TryParse(value, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out double newValue))
            {
                return;
            }

            if (newValue > ArgumentMax) newValue = ArgumentMax;
            if (newValue < ArgumentMin) newValue = ArgumentMin;
            _value.SetValue(new BindingValue(newValue), false);
        }

        protected override ExportDataElement[] DefaultDataElements => new ExportDataElement[] { new DCSDataElement(_id, _format) };
       
        public override void Reset()
        {
            _value.SetValue(BindingValue.Empty, true);
        }
    }
}
