// Copyright 2020 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System.Globalization;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class RotaryEncoder : DCSFunctionWithActions
    {
        private string _incrementData;
        private string _decrementData;

        private string _incrementPrefix;
        private string _decrementPrefix;

        private readonly string _id;
        private readonly string _format = "";

        private HeliosAction _incrementAction;
        private HeliosAction _decrementAction;
        private HeliosValue _value;

        [JsonProperty("argumentValue")]
        private double _argValue;

        public RotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double argValue, string device, string name, string exportFormat)
            : this(sourceInterface, deviceId, buttonId, buttonId, argId, argValue, device, name, exportFormat)
        {
        }
        public RotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double argValue, string device, string name)
            : this(sourceInterface, deviceId, buttonId, buttonId, argId, argValue, device, name, "")
        {
        }
        public RotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double argValue, string device, string name)
            : this(sourceInterface, deviceId, buttonId, button2Id, argId, argValue, device, name, "")
        {
        }
        public RotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double argValue, string device, string name, string exportFormat)
            : base(sourceInterface, device, name, null)
        {
            if (!string.IsNullOrWhiteSpace(exportFormat))
            {
                _id = argId;
                _format = exportFormat;
            }
            SerializedActions.Add("increment", new SerializedAction()
            {
                DeviceID = deviceId,
                ActionID = button2Id
            });
            SerializedActions.Add("decrement", new SerializedAction()
            {
                DeviceID = deviceId,
                ActionID = buttonId
            });
            _argValue = argValue;
            DoBuild();
        }

        // deserialization constructor
        public RotaryEncoder(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _incrementPrefix = "C" + SerializedActions["increment"].DeviceID + "," + SerializedActions["increment"].ActionID +
                               ",";
            _decrementPrefix = "C" + SerializedActions["decrement"].DeviceID + "," + SerializedActions["decrement"].ActionID +
                               ",";
            _incrementData = _incrementPrefix + _argValue.ToString(CultureInfo.InvariantCulture);
            _decrementData = _decrementPrefix + (-_argValue).ToString(CultureInfo.InvariantCulture);

            _incrementAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "increment",
                "Increments this setting.",
                "Value to increment by. (Defaults to " + _argValue + " if input evaluates to zero.)",
                BindingValueUnits.Numeric);
            _incrementAction.Execute += new HeliosActionHandler(IncrementAction_Execute);
            Actions.Add(_incrementAction);

            _decrementAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "decrement",
                "Decrement this setting.",
                "Value to increment by. (Defaults to " + -_argValue + " if input evaluates to zero.)",
                BindingValueUnits.Numeric);
            _decrementAction.Execute += new HeliosActionHandler(DecrementAction_Execute);
            Actions.Add(_decrementAction);

            _value = new HeliosValue(SourceInterface, new BindingValue(0.0d), SerializedDeviceName,
                SerializedFunctionName, "Current value of this encoder.", "", BindingValueUnits.Numeric);
            Values.Add(_value);
            Triggers.Add(_value);

        }

        void DecrementAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (e.Value.DoubleValue == 0d)
            {
                SourceInterface.SendData(_decrementData);
            }
            else
            {
                SourceInterface.SendData(_decrementPrefix + e.Value.StringValue);
            }
                
        }

        void IncrementAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (e.Value.DoubleValue == 0d)
            {
                SourceInterface.SendData(_incrementData);
            }
            else
            {
                SourceInterface.SendData(_incrementPrefix + e.Value.StringValue);
            }
        }

        public override void ProcessNetworkData(string id, string value)
        {
            if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out double parseValue))
            {
                return;
            }
            _value.SetValue(new BindingValue(parseValue), false);
        }

        protected override ExportDataElement[] DefaultDataElements
        {
            get
            {
                return (string.IsNullOrEmpty(_format) ? new ExportDataElement[0] : new ExportDataElement[] { new DCSDataElement(_id, _format) });
            }
        }
        public override void Reset()
        {
            // No-Op
        }
    }
}
