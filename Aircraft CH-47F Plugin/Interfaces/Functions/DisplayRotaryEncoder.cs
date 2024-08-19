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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.CH47F.Functions
{
    using Common;
    using UDPInterface;
    using System;
    using System.Globalization;

    public class DisplayRotaryEncoder : RotaryEncoder
    {
        private double _displayMultiplier = 1d;
        private double _offset = 0d;
        private double _lastData;
        private HeliosValue _displayValue;

        /// <summary>
        /// Verion of the RotaryEncoder which also can display the value of the argument.  Typically used for things like radio channel displays
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="deviceId"></param>
        /// <param name="buttonId">Command for incrementing</param>
        /// <param name="button2Id">Command for decrementing</param>
        /// <param name="argId"></param>
        /// <param name="stepValue">Value used for incrementing pulses and * -1 for decrementing</param>
        /// <param name="device"></param>
        /// <param name="name"></param>
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double stepValue, string device, string name, string exportFormat)
             : this(sourceInterface, deviceId, buttonId, button2Id, argId, stepValue, 1d, 0d, device, name, exportFormat) { }
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double stepValue, string device, string name)
             : this(sourceInterface, deviceId, buttonId, button2Id, argId, stepValue, 1d, 0d, device, name, "") { }

        /// <summary>
        /// Verion of the RotaryEncoder which also can display the value of the argument.  Typically used for things like radio channel displays
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="deviceId"></param>
        /// <param name="buttonId">Command for incrementing and decrementing</param>
        /// <param name="argId"></param>
        /// <param name="stepValue">Value used for incrementing pulses and * -1 for decrementing</param>
        /// <param name="device"></param>
        /// <param name="name"></param>
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double stepValue, string device, string name, string exportFormat)
             : this(sourceInterface, deviceId, buttonId, buttonId, argId, stepValue, 1d, 0d, device, name, exportFormat) { }
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double stepValue, string device, string name)
             : this(sourceInterface, deviceId, buttonId, buttonId, argId, stepValue, 1d, 0d, device, name, "") { }
        /// <summary>
        /// Verion of the RotaryEncoder which also can display the value of the argument.  Typically used for things like radio channel displays
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="deviceId"></param>
        /// <param name="buttonId">Command for incrementing and decrementing</param>
        /// <param name="argId"></param>
        /// <param name="stepValue">Value used for incrementing pulses and * -1 for decrementing</param>
        /// <param name="multiplier"></param>
        /// <param name="device">Argument value is multiplied by this for display</param>
        /// <param name="name"></param>
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double stepValue, double multiplier, string device, string name, string exportFormat)
             : this(sourceInterface, deviceId, buttonId, buttonId, argId, stepValue, multiplier, 0d, device, name, exportFormat) { }
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string argId, double stepValue, double multiplier, string device, string name)
             : this(sourceInterface, deviceId, buttonId, buttonId, argId, stepValue, multiplier, 0d, device, name, "") { }

        /// <summary>
        /// Verion of the RotaryEncoder which also can display the value of the argument.  Typically used for things like radio channel displays 
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="deviceId"></param>
        /// <param name="buttonId">Command for incrementing</param>
        /// <param name="button2Id">Command for decrementing</param>
        /// <param name="argId"></param>
        /// <param name="stepValue">Value used for incrementing pulses and * -1 for decrementing</param>
        /// <param name="multiplier">Argument value is multiplied by this for display</param>
        /// <param name="offset">added to value prior to calculations</param>
        /// <param name="device"></param>
        /// <param name="name"></param>
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double stepValue, double multiplier, string device, string name, string exportFormat = null)
             : this(sourceInterface, deviceId, buttonId, button2Id, argId, stepValue, multiplier, 0d, device, name, exportFormat) { }
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double stepValue, double multiplier, string device, string name)
             : this(sourceInterface, deviceId, buttonId, button2Id, argId, stepValue, multiplier, 0d, device, name, "") { }


        /// <summary>
        /// Verion of the RotaryEncoder which also can display the value of the argument.  Typically used for things like radio channel displays 
        /// </summary>
        /// <param name="sourceInterface"></param>
        /// <param name="deviceId"></param>
        /// <param name="buttonId">Command for incrementing</param>
        /// <param name="button2Id">Command for decrementing</param>
        /// <param name="argId"></param>
        /// <param name="stepValue">Value used for incrementing pulses and * -1 for decrementing</param>
        /// <param name="multiplier">Argument value is multiplied by this for display</param>
        /// <param name="calibration">Used to interpolate the argument value for display</param>
        /// <param name="device"></param>
        /// <param name="name"></param>
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, string deviceId, string buttonId, string button2Id, string argId, double stepValue, double multiplier, double offset, string device, string name, string exportFormat)
			 : base(sourceInterface, deviceId, buttonId, button2Id, argId, stepValue, device, name, exportFormat)
		{          
            _displayMultiplier = multiplier;
            _offset = offset;
            // base calls is DoBuild, we add ours
            DoBuild();
        }

        // deserialization constructor
        public DisplayRotaryEncoder(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            base.BuildAfterDeserialization();
            DoBuild();
        }

        private void DoBuild()
        {
            _displayValue = new HeliosValue(SourceInterface, new BindingValue(0.0d), SerializedDeviceName,
                SerializedFunctionName + " for display", "Current value displayed in this encoder.",
                "", BindingValueUnits.Text);
            Values.Add(_displayValue);
            Triggers.Add(_displayValue);
        }

        public override void ProcessNetworkData(string id, string value)
        {
            base.ProcessNetworkData(id, value);
            if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out double parseValue))
            {
                return;
            }

            double newValue = (parseValue + _offset) % 1;
             

            if (newValue < _lastData)
            {
                newValue = Math.Floor(newValue * _displayMultiplier);
            }
            else if (newValue > _lastData)
            {
                newValue = Math.Ceiling(newValue * _displayMultiplier);
                if (newValue == _displayMultiplier) newValue = 0;
            }
            else
            {
                return;
            }

            _lastData = parseValue;
            _displayValue.SetValue(new BindingValue(newValue.ToString(CultureInfo.InvariantCulture)), false);
        }
    }
}
