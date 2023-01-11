// Copyright 2023 Helios Contributors
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

using GadrocsWorkshop.Helios.Interfaces.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadrocsWorkshop.Helios.Interfaces.Vendor.Functions
{
    public class ThrustmasterWarthogThrottleIndicators : IHotasFunctions
    {
        private string _device;

        private HeliosValue _indicatorValue;
        private DirectXControllerInterface _sourceInterface;

        private uint[] _indicatorBits = new uint[6] {0x01 << 3, 0x01 << 2, 0x01 << 1, 0x01 << 4, 0x01 << 0, 0x01 << 6 };
        private uint _indicators = 0x00;
        private uint _intensity = 2;


        public ThrustmasterWarthogThrottleIndicators(DirectXControllerInterface sourceInterface, string device, string name)
        {
            _device = device;
            _sourceInterface = sourceInterface;
            _intensity = 2;
        }

        //// deserialization constructor
        //public ThrustmasterWarthogThrottleIndicators(DirectXControllerInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
        //{
        //    DoBuild();
        //}

        public void CreateActionsAndValues()
        {
            DoBuild();
        }

        private void DoBuild()
        {
            for(int i = 0; i < 5; i++)
            {
                _indicatorValue = new HeliosValue(_sourceInterface, new BindingValue(null), _device, $"indicator {i+1}", $"the value of indicator {i+1} on {_device}.","true = on, false = off", BindingValueUnits.Boolean);
                _indicatorValue.Execute += new HeliosActionHandler(Action_Execute);
                _sourceInterface.Values.Add(_indicatorValue);
                _sourceInterface.Actions.Add(_indicatorValue);
            }
            
            _indicatorValue = new HeliosValue(_sourceInterface, new BindingValue(null), _device, $"backlight", $"the backlight state on {_device}.", "true = on, false = off", BindingValueUnits.Boolean);
            _indicatorValue.Execute += new HeliosActionHandler(Action_Execute);
            _sourceInterface.Actions.Add(_indicatorValue);
            _sourceInterface.Values.Add(_indicatorValue);
            _indicatorValue = new HeliosValue(_sourceInterface, new BindingValue(null), _device, $"brightness", $"the light intesity level on {_device}.", "0 = off, 1 to 5 is the light level", BindingValueUnits.Numeric);
            _indicatorValue.Execute += new HeliosActionHandler(Action_Execute);
            _sourceInterface.Actions.Add(_indicatorValue);
            _sourceInterface.Values.Add(_indicatorValue);

        }

        void Action_Execute(object action, HeliosActionEventArgs e)
        {
            uint indicator = 0;
            if (action is HeliosValue hAction){
                string name = hAction.Name;
                switch (hAction.Name)
                {
                    case "brightness":
                        _intensity = e.Value.DoubleValue>5? 0x05: (e.Value.DoubleValue < 0? 0x00: (uint) e.Value.DoubleValue);
                        break;
                    case "backlight":
                        if (e.Value.BoolValue)
                        {
                            _indicators |= _indicatorBits[0];
                        }
                        else
                        {
                            _indicators &= ~_indicatorBits[0];
                        }
                        break;
                    default:
                        indicator = (uint) Int32.Parse(hAction.Name.Substring(10,1));
                        if (e.Value.BoolValue)
                        {
                            _indicators |= _indicatorBits[indicator];
                        }
                        else
                        {
                            _indicators &= ~_indicatorBits[indicator];
                        }
                        break;
                }
                byte[] writeBuffer = new byte[4];
                writeBuffer[0] = 0x01;
                writeBuffer[1] = 0x06;
                writeBuffer[2] = (byte) _indicators;
                writeBuffer[3] = (byte) _intensity;
                _sourceInterface.SendUsbData(writeBuffer);
            }
        }
        public void Reset()
        {
            _indicators = 0x00;
            _intensity = 0;
            _indicatorValue.SetValue(BindingValue.Empty, true);
        }

    }
}

