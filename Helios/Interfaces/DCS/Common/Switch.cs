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
using System.Text.RegularExpressions;
using System.Windows.Controls;
using GadrocsWorkshop.Helios.UDPInterface;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class Switch : DCSFunction
    {
        const int SWITCHINCREMENT = 0;
        const int SWITCHDECREMENT = 1;
        const int SWITCHNEUTRAL = 2;
        protected string _id;
        protected string _format;
        protected bool _everyframe;

        private string[] _sendAction;
        private string[] _sendStopAction;
        private string[] _exitAction;
        private string[] _sendPulse;
        private int _currentPosition = -1;

        private HeliosValue _value;
        private HeliosAction _releaseAction;

        private int _lastSetPosition = -1;
        private bool _incrementalPulseSwitch = false;

        [JsonProperty("deviceId")]
        private string _deviceId;

        [JsonProperty("incrementalPulseValue", NullValueHandling = NullValueHandling.Ignore)]
        private string _incrementalPulseValue = "";

        [JsonProperty("positions")]
        private SwitchPosition[] _positions;

        #region Static Factories

        public static Switch CreateToggleSwitch(BaseUDPInterface sourceInterface, string deviceId, string action, string argId, 
                                                    string position1Value, string position1Name, 
                                                    string position2Value, string position2Name, 
                                                    string device, string name, string exportFormat)
        {
            return new Switch(sourceInterface, deviceId, argId, new SwitchPosition[] { new SwitchPosition(position1Value, position1Name, action), new SwitchPosition(position2Value, position2Name, action) }, device, name, exportFormat, "");
        }

        public static Switch CreateThreeWaySwitch(BaseUDPInterface sourceInterface, string deviceId, string action, string argId,
                                                    string position1Value, string position1Name,
                                                    string position2Value, string position2Name,
                                                    string position3Value, string position3Name,
                                                    string device, string name, string exportFormat)
        {
            return new Switch(sourceInterface, deviceId, argId,
                    new SwitchPosition[] { new SwitchPosition(position1Value, position1Name, action), new SwitchPosition(position2Value, position2Name, action), new SwitchPosition(position3Value, position3Name, action) },
                    device, name, exportFormat, "");
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

            return new Switch(sourceInterface, deviceId, argId, positions.ToArray(), device, name, exportFormat, "");
        }

        #endregion
        public Switch(BaseUDPInterface sourceInterface, string deviceId, string argId, SwitchPosition[] positions, string device, string name, string exportFormat, string incrementalPulseValue)
            : base(sourceInterface, device, name, "Current position of this switch.")
        {
            bool build = true;
            _id = argId;
            _format = exportFormat;
            _everyframe = false;
            _deviceId = deviceId;
            _incrementalPulseValue = incrementalPulseValue;
            _incrementalPulseSwitch = !string.IsNullOrWhiteSpace(incrementalPulseValue);
            _positions = positions;
            if (build)
            {
                DoBuild();
            }
        }

        public Switch(BaseUDPInterface sourceInterface, string deviceId, string argId, SwitchPosition[] positions, string device, string name, string exportFormat)
            : this(sourceInterface, deviceId, argId, positions, device, name, exportFormat, "", false)
        {
            // all code in referenced constructor
        }

        public Switch(BaseUDPInterface sourceInterface, string deviceId, string argId, SwitchPosition[] positions, string device, string name, string exportFormat, bool everyFrame, bool build = true) 
            : this(sourceInterface, deviceId, argId, positions, device, name, exportFormat, "", everyFrame, build)
        {
            // all code in referenced constructor
        }

        public Switch(BaseUDPInterface sourceInterface, string deviceId, string argId, SwitchPosition[] positions,
            string device, string name, string exportFormat, string incrementalPulseValue, bool everyFrame, bool build = true)
            : base(sourceInterface, device, name, "Current position of this switch.")
        {
            _id = argId;
            _format = exportFormat;
            _everyframe = everyFrame;
            _deviceId = deviceId;
            _incrementalPulseValue = incrementalPulseValue;
            _incrementalPulseSwitch = !string.IsNullOrWhiteSpace(incrementalPulseValue); 
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
            _id = DataElements[0].ID;
            _format = (DataElements[0] as DCSDataElement).Format;
            _everyframe = (DataElements[0] as DCSDataElement).IsExportedEveryFrame;
            DoBuild();
        }

        protected void DoBuild()
        {
            _sendAction = new string[_positions.Length];
            _sendStopAction = new string[_positions.Length];
            _exitAction = new string[_positions.Length];
            _sendPulse = new string[3];

            _incrementalPulseSwitch = !string.IsNullOrWhiteSpace(_incrementalPulseValue);

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
                       _sendAction[i] = $"C{_deviceId},{position.Action},{(position.EntryActionValue == null ? position.ArgValue : position.EntryActionValue)}";
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

            if (_incrementalPulseSwitch)
            {
                Debug.Assert(_format == (DataElements[0] as DCSDataElement).Format, $"Export Format is not available to Incremental Pulse Switch for arg ID {DataElements[0].ID}");

                _sendPulse[SWITCHINCREMENT] = $"C{_deviceId},{_positions[0].Action},{_incrementalPulseValue}";
                _sendPulse[SWITCHDECREMENT] = $"C{_deviceId},{_positions[0].Action},{(-1d * double.Parse(_incrementalPulseValue, System.Globalization.CultureInfo.InvariantCulture)).ToString("N"+FormatDigits(_format).ToString())}";
                _sendPulse[SWITCHNEUTRAL] =   $"C{_deviceId},{_positions[0].Action},0.0";
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
        /// <summary>
        /// Convert Lua style string format
        /// </summary>
        /// <param name="exportFormat"></param>
        /// <returns>Number to be used in C# Nx formatting</returns>
        private static int FormatDigits(string exportFormat)
        {
            if (!string.IsNullOrEmpty(exportFormat))
            {
                // expecting %0.1f or %2d type input group 1 will have the number before the f or d
                Regex rx = new Regex(@"\%(?:[0-9|#]?\.?)([0-9])[f|d]\z", RegexOptions.Compiled);
                Match match = rx.Match(exportFormat);
                if (match.Success && match.Groups[1].Captures.Count == 1 && match.Groups[1].Captures[0] != null && int.TryParse(match.Groups[1].Captures[0].Value, out int result))
                {
                    return result;
                }
            } else
            {
                //Todo:  Investigate whether a log message is warrented for here.
            }
            return 0;
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

                if (_currentPosition > -1 && !string.IsNullOrWhiteSpace(_exitAction[_currentPosition]) && !_incrementalPulseSwitch)
                {
                        SourceInterface.SendData(_exitAction[_currentPosition]);
                }

                _currentPosition = index;

                if (!_incrementalPulseSwitch)
                {
                    SourceInterface.SendData(_sendAction[ _currentPosition]);
                } else
                {
                    _lastSetPosition = (_lastSetPosition == _positions.Length - 1 && _currentPosition == 0) ? -1 : (_lastSetPosition == 0 && _currentPosition == _positions.Length - 1) ? _positions.Length : _lastSetPosition;
                    if (_lastSetPosition < _currentPosition)
                    {
                        SourceInterface.SendData(_sendPulse[SWITCHINCREMENT]);
                    }
                    else
                    {
                        SourceInterface.SendData(_sendPulse[SWITCHDECREMENT]);
                    }
                    SourceInterface.SendData(_sendPulse[SWITCHNEUTRAL]);
                }
                _lastSetPosition = _currentPosition;
            }
        }

        public override void ProcessNetworkData(string id, string value)
        {
            // our descendant classes are supposed to ensure this
            Debug.Assert(id == DataElements[0].ID);

            for (int i = 0; i < _positions.Length; i++)
            {
                bool numericallyEqual = false;
                if(double.TryParse(_positions[i].ArgValue, out double argValue) && double.TryParse(value, out double netValue))
                {
                    numericallyEqual = argValue == netValue ? true : false; 
                }
                if (numericallyEqual || _positions[i].ArgValue.Equals(value))
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
