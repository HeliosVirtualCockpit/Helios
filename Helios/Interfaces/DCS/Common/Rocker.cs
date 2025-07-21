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

using System.Diagnostics;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class Rocker : DCSFunctionWithButtons
    {
        private string _id;
        private string _format;

        [JsonProperty("triggereveryrelease")]
        private bool _triggerEveryRelease;

        private string _position1Name;
        private string _position2Name;

        private string _pushed1ActionData;
        private string _pushed2ActionData;
        private string _release1ActionData;
        private string _release2ActionData;

        private string _position1Value;
        private string _releaseValue;
        private string _position2Value;

        private HeliosAction _push1Action;
        private HeliosAction _push2Action;
        private HeliosAction _releaseAction;

        private HeliosTrigger _pushed1Trigger;
        private HeliosTrigger _pushed2Trigger;
        private HeliosTrigger _releasedTrigger;

        private HeliosValue _value;

        private bool _release2 = false;

        [JsonProperty("vertical")]
        private bool _vertical;

        public Rocker(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, string device, string name, bool vertical)
            : this(sourceInterface, deviceId, buttonId, buttonId, buttonId, buttonId, argId, device, name, vertical, "1", "-1", "0", "%1d")
        {
        }
        public Rocker(BaseUDPInterface sourceInterface, string deviceId, string button1Id, string button2Id, string releaseButtonId, string releaseButton2Id, string argId, string device, string name, bool vertical)
            : this(sourceInterface, deviceId, button1Id, button2Id, releaseButtonId, releaseButton2Id, argId, device, name, vertical, "1", "-1", "0", "%1d")
        {
        }
        public Rocker(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, string device, string name, bool vertical, string push1Value, string push2Value, string releaseValue, string exportFormat, bool triggerEveryRelease = false)
            : this(sourceInterface, deviceId, buttonId, buttonId, buttonId, buttonId, argId, device, name, vertical, push1Value, push2Value, releaseValue, exportFormat, triggerEveryRelease) 
        {
        }

        /// <summary>
        /// DCS Interface function for a rocker switch
        /// </summary>
        /// <param name="sourceInterface">Source Interface</param>
        /// <param name="deviceId">ID of the device </param>
        /// <param name="button1Id">Button Command for Pushed value 1</param>
        /// <param name="button2Id">Button Command for Pushed value 2</param>
        /// <param name="releaseButtonId">Button Command for Released value 1</param>
        /// <param name="releaseButton2Id">Button Command for Released value 2</param>
        /// <param name="argId">DCS argument ID</param>
        /// <param name="device">argument device name</param>
        /// <param name="name">argument name</param>
        /// <param name="vertical"></param>
        /// <param name="push1Value">Value associated with the pushed value 1</param>
        /// <param name="push2Value">Value associated with the pushed value 2 </param>
        /// <param name="releaseValue">Value associated with the release</param>
        /// <param name="exportFormat">Format used for exporting the arg value</param>
        /// <param name="triggerEveryRelease">This forces a trigger on every release, even if the new value is the same as the current value.  Needed if a pushed value needs to be followed by a release.</param>
        public Rocker(BaseUDPInterface sourceInterface, string deviceId, string button1Id, string button2Id, string releaseButtonId, string releaseButton2Id, string argId, string device, string name, bool vertical, string push1Value, string push2Value, string releaseValue, string exportFormat, bool triggerEveryRelease = false)
            : base(sourceInterface, device, name, "Current state of this rocker.")
        {
            _id = argId;
            _format = exportFormat;
            _vertical = vertical;
            _triggerEveryRelease = triggerEveryRelease;
            SerializedButtons.Add(new SerializedButton() { DeviceID = deviceId, PushID = button1Id, PushValue = push1Value, ReleaseID = releaseButtonId, ReleaseValue = releaseValue });
            SerializedButtons.Add(new SerializedButton() { DeviceID = deviceId, PushID = button2Id, PushValue = push2Value, ReleaseID = releaseButton2Id, ReleaseValue = releaseValue });
            DoBuild();
        }

        // deserialization constructor
        public Rocker(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _position1Value = SerializedButtons[0].PushValue;
            _position2Value = SerializedButtons[1].PushValue;
            _releaseValue = SerializedButtons[0].ReleaseValue;

            // limitation of this implementation
            Debug.Assert(SerializedButtons[0].ReleaseValue == SerializedButtons[1].ReleaseValue);

            _pushed1ActionData = "C" + SerializedButtons[0].DeviceID + "," + SerializedButtons[0].PushID + "," +
                                 SerializedButtons[0].PushValue;
            _pushed2ActionData = "C" + SerializedButtons[1].DeviceID + "," + SerializedButtons[1].PushID + "," +
                                 SerializedButtons[1].PushValue;
            _release1ActionData = "C" + SerializedButtons[0].DeviceID + "," + SerializedButtons[0].ReleaseID + "," +
                                  SerializedButtons[0].ReleaseValue;
            _release2ActionData = "C" + SerializedButtons[1].DeviceID + "," + SerializedButtons[1].ReleaseID + "," +
                                  SerializedButtons[1].ReleaseValue;

            if (_vertical)
            {
                _position1Name = "up";
                _position2Name = "down";
            }
            else
            {
                _position1Name = "left";
                _position2Name = "right";
            }

            _value = new HeliosValue(SourceInterface, new BindingValue(false), SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, "1=" + _position1Name + ", 2=released" + ", 3=" + _position2Name,
                BindingValueUnits.Numeric);
            Values.Add(_value);
            Triggers.Add(_value);

            _pushed1Trigger = new HeliosTrigger(SourceInterface, SerializedDeviceName, SerializedFunctionName,
                "pushed " + _position1Name, "Fired when this rocker is pushed " + _position1Name + " in the simulator.");
            Triggers.Add(_pushed1Trigger);
            _pushed2Trigger = new HeliosTrigger(SourceInterface, SerializedDeviceName, SerializedFunctionName,
                "pushed " + _position2Name, "Fired when this rocker is pushed " + _position2Name + " in the simulator.");
            Triggers.Add(_pushed2Trigger);

            _releasedTrigger = new HeliosTrigger(SourceInterface, SerializedDeviceName, SerializedFunctionName, "released",
                "Fired when this rocker is released in the simulator.");
            Triggers.Add(_releasedTrigger);

            _push1Action = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName,
                "push " + _position1Name, "Pushes this rocker " + _position1Name + " in the simulator");
            _push1Action.Execute += new HeliosActionHandler(Push1Action_Execute);
            Actions.Add(_push1Action);

            _push2Action = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName,
                "push " + _position2Name, "Pushes this rocker " + _position2Name + " in the simulator");
            _push2Action.Execute += new HeliosActionHandler(Push2Action_Execute);
            Actions.Add(_push2Action);

            _releaseAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "release",
                "Releases the rocker in the simulator.");
            _releaseAction.Execute += new HeliosActionHandler(ReleaseAction_Execute);
            Actions.Add(_releaseAction);
        }

        void ReleaseAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (_release2)
            {
                SourceInterface.SendData(_release2ActionData);
            }
            else
            {
                SourceInterface.SendData(_release1ActionData);
            }
        }

        void Push1Action_Execute(object action, HeliosActionEventArgs e)
        {
            _release2 = false;
            SourceInterface.SendData(_pushed1ActionData);
        }

        void Push2Action_Execute(object action, HeliosActionEventArgs e)
        {
            _release2 = true;
            SourceInterface.SendData(_pushed2ActionData);
        }

        public override void ProcessNetworkData(string id, string value)
        {
            if (value.Equals(_position1Value))
            {
                _value.SetValue(new BindingValue(1), false);
                _pushed1Trigger.FireTrigger(BindingValue.Empty);
            }

            else if (value.Equals(_position2Value))
            {
                _value.SetValue(new BindingValue(3), false);
                _pushed1Trigger.FireTrigger(BindingValue.Empty);
            }

            else if (value.Equals(_releaseValue))
            {
                _value.SetValue(new BindingValue(2), false, _triggerEveryRelease);
                _releasedTrigger.FireTrigger(BindingValue.Empty);
            }
        }

        protected override ExportDataElement[] DefaultDataElements =>
            new ExportDataElement[] { new DCSDataElement(_id, _format, _triggerEveryRelease) };

        public override void Reset()
        {
            _value.SetValue(BindingValue.Empty, true);
        }

    }
}
