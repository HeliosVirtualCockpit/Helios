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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class Switch : DCSFunction
    {
        protected readonly string _id;
        protected readonly string _format;
        protected bool _everyframe;

        private string[] _sendAction;
        private string[] _sendStopAction;
        private string[] _exitAction;
        private int _currentPosition = -1;

        private HeliosValue _value;
        private HeliosAction _releaseAction;

        private int _lastSetPosition = -1;

        [JsonProperty("deviceId")]
        private string _deviceId;

        [JsonProperty("positions")]
        private SwitchPosition[] _positions;

        #region Static Factories

        public static Switch CreateToggleSwitch(BaseUDPInterface sourceInterface, string deviceId, string action, string argId, 
                                                    string position1Value, string position1Name, 
                                                    string position2Value, string position2Name, 
                                                    string device, string name, string exportFormat)
        {
            return new Switch(sourceInterface, deviceId, argId, new SwitchPosition[] { new SwitchPosition(position1Value, position1Name, action), new SwitchPosition(position2Value, position2Name, action) }, device, name, exportFormat);
        }

        public static Switch CreateThreeWaySwitch(BaseUDPInterface sourceInterface, string deviceId, string action, string argId,
                                                    string position1Value, string position1Name,
                                                    string position2Value, string position2Name,
                                                    string position3Value, string position3Name,
                                                    string device, string name, string exportFormat)
        {
            return new Switch(sourceInterface, deviceId, argId,
                    new SwitchPosition[] { new SwitchPosition(position1Value, position1Name, action), new SwitchPosition(position2Value, position2Name, action), new SwitchPosition(position3Value, position3Name, action) },
                    device, name, exportFormat);
        }

        public static Switch CreateRotarySwitch(BaseUDPInterface sourceInterface, string deviceId, string action, string argId, string device, string name, string exportFormat, params string[] positionData)
        {
            Debug.Assert(positionData.Length > 2, "DCS rotary switch definition must have more than one position.");
            Debug.Assert(positionData.Length % 2 == 0, "DCS rotary switch definition data inclomplete.");

            List<SwitchPosition> positions = new List<SwitchPosition>();
            for (int i = 0; i < positionData.Length; i++)
            {
                positions.Add(new SwitchPosition(positionData[i++], positionData[i], action));
            }

            return new Switch(sourceInterface, deviceId, argId, positions.ToArray(), device, name, exportFormat);
        }

        #endregion

        public Switch(BaseUDPInterface sourceInterface, string deviceId, string argId, SwitchPosition[] positions, string device, string name, string exportFormat)
            : this(sourceInterface, deviceId, argId, positions, device, name, exportFormat, false)
        {
            // all code in referenced constructor
        }

        public Switch(BaseUDPInterface sourceInterface, string deviceId, string argId, SwitchPosition[] positions,
            string device, string name, string exportFormat, bool everyFrame, bool build = true)
            : base(sourceInterface, device, name, "Current position of this switch.")
        {
            _id = argId;
            _format = exportFormat;
            _everyframe = everyFrame;
            _deviceId = deviceId;
            _positions = positions;
            if (build)
            {
                DoBuild();
            }
        }

        // deserialization constructor
        public Switch(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            DoBuild();
        }

        protected void DoBuild()
        {
            _sendAction = new string[_positions.Length];
            _sendStopAction = new string[_positions.Length];
            _exitAction = new string[_positions.Length];

            ValueDescriptions = "";
            for (int i = 0; i < _positions.Length; i++)
            {
                SwitchPosition position = _positions[i];

                if (ValueDescriptions.Length > 0)
                {
                    ValueDescriptions += ",";
                }

                ValueDescriptions += (i + 1).ToString() + "=" + position.Name;
                if (position.Action != null)
                {
                    _sendAction[i] = "C" + _deviceId + "," + position.Action + "," + position.ArgValue;
                }

                if (position.StopAction != null)
                {
                    _sendStopAction[i] = "C" + _deviceId + "," + position.StopAction + "," + position.StopActionValue;
                }

                if (position.ExitValue != null)
                {
                    _exitAction[i] = "C" + _deviceId + "," + position.Action + "," + position.ExitValue;
                }
            }

            _releaseAction = new HeliosAction(SourceInterface, SerializedDeviceName, SerializedFunctionName, "release",
                "Releases pressure on current position (allows momentary and electronically held switch to revert to another position if necessary).");
            _releaseAction.Execute += new HeliosActionHandler(Release_Execute);
            Actions.Add(_releaseAction);

            _value = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, ValueDescriptions, BindingValueUnits.Numeric);
            _value.Execute += new HeliosActionHandler(Value_Execute);
            Actions.Add(_value);
            Triggers.Add(_value);
            Values.Add(_value);

            _currentPosition = -1;
        }

        #region Properties

        [JsonIgnore] 
        public IEnumerable<SwitchPosition> Positions => _positions;

        [JsonIgnore]
        protected string ValueDescriptions { get; private set; } = "";

        #endregion

        protected virtual void Release_Execute(object action, HeliosActionEventArgs e)
        {
            if (_lastSetPosition > -1 && !string.IsNullOrWhiteSpace(_sendStopAction[_lastSetPosition]))
            {
                SourceInterface.SendData(_sendStopAction[_lastSetPosition]);
            }
        }

        protected virtual void Value_Execute(object action, HeliosActionEventArgs e)
        {
            int index = (int)e.Value.DoubleValue - 1;
            if (index >= 0 && index < _positions.Length)
            {
                _value.SetValue(e.Value, e.BypassCascadingTriggers);

                if (_currentPosition > -1 && !string.IsNullOrWhiteSpace(_exitAction[_currentPosition]))
                {
                    SourceInterface.SendData(_exitAction[_currentPosition]);
                }

                _currentPosition = index;

                if (!string.IsNullOrWhiteSpace(_sendAction[_currentPosition]))
                {
                    SourceInterface.SendData(_sendAction[_currentPosition]);
                    _lastSetPosition = _currentPosition;
                }
            }
        }

        public override void ProcessNetworkData(string id, string value)
        {
            // our descendant classes are supposed to ensure this
            Debug.Assert(id == DataElements[0].ID);

            for (int i = 0; i < _positions.Length; i++)
            {
                if (_positions[i].ArgValue.Equals(value))
                {
                    _currentPosition = i;
                    _value.SetValue(new BindingValue((double)(i + 1)), false);
                }
            }
        }

        protected override ExportDataElement[] DefaultDataElements => new ExportDataElement[] { new DCSDataElement(_id, _format, _everyframe) };

        public override void Reset()
        {
            _currentPosition = -1;
            _lastSetPosition = -1;
            _value.SetValue(BindingValue.Empty, true);
        }

    }
}
