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

using GadrocsWorkshop.Helios.UDPInterface;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class PushButton : DCSFunctionWithButtons
    {
        private string _id;
        private string _format;

        private string _pushActionData;
        private string _releaseActionData;
        private string _pushValue;
        private string _releaseValue;

        private HeliosAction _pushAction;
        private HeliosAction _releaseAction;
        private HeliosTrigger _pushedTrigger;
        private HeliosTrigger _releasedTrigger;
        private HeliosValue _value;

        public PushButton(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, string device, string name)
            : this(sourceInterface, deviceId, buttonId, argId, device, name, "1.0", "0.0", "%.1f")
        {
        }
        public PushButton(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, string device, string name, string exportFormat)
            : this(sourceInterface, deviceId, buttonId, argId, device, name, "1.0", "0.0", exportFormat)
        {
        }

        public PushButton(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, string device, string name, string pushValue, string releaseValue, string exportFormat)
            : base(sourceInterface, device, name, "Current state of this button.")
        {
            SerializedButtons.Add(new SerializedButton()
            {
                DeviceID = deviceId,
                PushID = buttonId,
                PushValue = pushValue,
                ReleaseID = buttonId,
                ReleaseValue = releaseValue
            });
            _id = argId;
            _format = exportFormat;
            DoBuild();
        }

        // deserialization constructor
        public PushButton(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _pushValue = SerializedButtons[0].PushValue;
            _releaseValue = SerializedButtons[0].ReleaseValue;

            _pushActionData = "C" + SerializedButtons[0].DeviceID + "," + SerializedButtons[0].PushID + "," +
                              SerializedButtons[0].PushValue;
            _releaseActionData = "C" + SerializedButtons[0].DeviceID + "," + SerializedButtons[0].ReleaseID + "," +
                                 SerializedButtons[0].ReleaseValue;

            _value = new HeliosValue(SourceInterface, new BindingValue(false), SerializedDeviceName, SerializedFunctionName,
                SerializedDescription,
                "True if the button is currently pushed (either via pressure or toggle), otherwise false.",
                BindingValueUnits.Boolean);
            Values.Add(_value);
            Triggers.Add(_value);

            _pushedTrigger = new HeliosTrigger(SourceInterface, SerializedDeviceName, SerializedFunctionName, "pushed",
                "Fired when this button is pushed in the simulator.");
            Triggers.Add(_pushedTrigger);
            _releasedTrigger = new HeliosTrigger(SourceInterface, SerializedDeviceName, SerializedFunctionName, "released",
                "Fired when this button is released in the simulator.");
            Triggers.Add(_releasedTrigger);

            _pushAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "push",
                "Pushes this button in the simulator");
            _pushAction.Execute += new HeliosActionHandler(PushAction_Execute);
            Actions.Add(_pushAction);
            _releaseAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "release",
                "Releases the button in the simulator.");
            _releaseAction.Execute += new HeliosActionHandler(ReleaseAction_Execute);
            Actions.Add(_releaseAction);
        }

        void ReleaseAction_Execute(object action, HeliosActionEventArgs e)
        {
            SourceInterface.SendData(_releaseActionData);
        }

        void PushAction_Execute(object action, HeliosActionEventArgs e)
        {
            SourceInterface.SendData(_pushActionData);
        }

        public override void ProcessNetworkData(string id, string value)
        {
            if (value.Equals(_pushValue))
            {
                _value.SetValue(new BindingValue(true), false);
                _pushedTrigger.FireTrigger(BindingValue.Empty);
            }

            else if (value.Equals(_releaseValue))
            {
                _value.SetValue(new BindingValue(false), false);
                _releasedTrigger.FireTrigger(BindingValue.Empty);
            }
        }

        protected override ExportDataElement[] DefaultDataElements =>
            new ExportDataElement[] { new DCSDataElement(_id, _format) };

        public override void Reset()
        {
            _value.SetValue(BindingValue.Empty, true);
        }

    }
}
