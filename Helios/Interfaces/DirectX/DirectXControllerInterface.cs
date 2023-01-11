//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Interfaces.DirectX
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.Vendor.Functions;
    using HidSharp;
    using System.Linq;
    using SharpDX.DirectInput;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Xml;
    using System.Security.Cryptography;

    [HeliosInterface("Helios.Base.DirectXController", "DirectX Controller", typeof(DirectXControllerInterfaceEditor), typeof(DirectXControllerInterfaceFactory))]
    public class DirectXControllerInterface : HeliosInterface
    {
        private DirectXControllerGuid _deviceId;
        private Joystick _device;

        private List<DirectXControllerFunction> _functions = new List<DirectXControllerFunction>();

        private IntPtr _hWnd;
        private delegate IntPtr GetMainHandleDelegate();

        private HidStream _hotasStream;
        private HidDevice _hotasDevice;
        private IHotasFunctions _hotasFunctions;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DirectXControllerInterface()
            : base("DirectX Controller")
        {
            if (Application.Current == null)
            {
                _hWnd = IntPtr.Zero;
                return;
            }
            _hWnd = (IntPtr)Application.Current.Dispatcher.Invoke(new GetMainHandleDelegate(GetMainWindowHandle));
        }

        private IntPtr GetMainWindowHandle()
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                WindowInteropHelper helper = new WindowInteropHelper(Application.Current.MainWindow);
                return helper.Handle;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        #region Properties

        internal DirectXControllerGuid ControllerId
        {
            get
            {
                return _deviceId;
            }
            set
            {
                if ((_deviceId == null && value != null)
                    || (_deviceId != null && !_deviceId.Equals(value)))
                {
                    DirectXControllerGuid oldValue = _deviceId;

                    DirectXControllerGuid newDeviceId = value;
                    Joystick newDevice = null;

                    try
                    {
                        newDevice = new Joystick(DirectXControllerInterfaceFactory.DirectInput, newDeviceId.InstanceGuid);
                    }
                    catch (SharpDX.SharpDXException)
                    {
                        //    // Check to see if any enumerable controllers have the same DisplayName
                        //    // This should address https://github.com/BlueFinBima/Helios/issues/235 and https://github.com/BlueFinBima/Helios/issues/231
                        bool alternateDeviceFound = false;
                        foreach (DeviceInstance controllerInstance in DirectXControllerInterfaceFactory.DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
                        {
                            if (controllerInstance.ProductName == value.ProductName)
                            {
                                Logger.Info($"Unable to create device for \"{value.ProductName}\" with the GUID {value.InstanceGuid} which is specified in the profile.  Instance GUID {controllerInstance.InstanceGuid} will be used instead.  This can happen when the profile is running with a different account or on a different machine than the machine used to create the profile.  It can also happen when the device is not plugged in.");
                                newDeviceId = new DirectXControllerGuid(controllerInstance.ProductName, controllerInstance.InstanceGuid);
                                newDevice = new Joystick(DirectXControllerInterfaceFactory.DirectInput, controllerInstance.InstanceGuid);
                                alternateDeviceFound = true;
                                break;
                            }
                        }
                        if (!alternateDeviceFound)
                        {
                            Logger.Debug($"Unable to create device for \"{value.ProductName}\" with the GUID {value.InstanceGuid} which is  specified in the profile.  No obvious alternative devices have been found.  This can happen when the profile is running with a different account or on a different machine than the machine used to create the profile.  It can also happen when the device is not plugged in.");
                        }
                    }

                    if (newDeviceId == null)
                    {
                        _deviceId = value;
                    }
                    else
                    {
                        _deviceId = newDeviceId;
                    }

                    if (newDevice != null)
                    {
                        if (_device != null)
                        {
                            _device.Unacquire();
                            _device.Dispose();
                        }

                        _device = newDevice;
                        //_device.SetCooperativeLevel(_hWnd, CooperativeLevel.Background | CooperativeLevel.Exclusive);
                        //_device.SetDataFormat(DeviceDataFormat.Joystick);
                        _device.Acquire();

                        Name = _deviceId.ProductName;

                        PopulateFunctions(_device.GetObjects(DeviceObjectTypeFlags.All));
                    }

                    _device = newDevice;

                    OnPropertyChanged("ControllerId", oldValue, _deviceId, false);
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return _device != null;
            }
        }

        public List<DirectXControllerFunction> Functions
        {
            get
            {
                return _functions;
            }
        }

        #endregion

        private void PopulateFunctions(IList<DeviceObjectInstance> objects)
        {
            Triggers.Clear();
            Values.Clear();
            _functions.Clear();

            _device.Poll();
            JoystickState state = GetState();

            int lastButton = -1;
            int lastSlider = -1;
            int lastPov = -1;

            foreach (DeviceObjectInstance obj in objects)
            {
                int controlNum = -1;
                if (obj.ObjectType == ObjectGuid.Button)
                {
                    controlNum = ++lastButton;
                }
                else if (obj.ObjectType == ObjectGuid.Slider)
                {
                    controlNum = ++lastSlider;
                }
                else if (obj.ObjectType == ObjectGuid.PovController)
                {
                    controlNum = ++lastPov;
                }
                else if (obj.ObjectType != ObjectGuid.Unknown)
                {
                    controlNum = 0;
                }

                if (controlNum > -1)
                {
                    AddFunction(DirectXControllerFunction.Create(this, obj.ObjectType, controlNum, state));
                }
            }
            switch (_device.Properties.VendorId)
            {
                case 0x044f:  // Thrustmaster
                    switch (_device.Properties.ProductId)
                    {
                        case 0x0404: // Warthog Throttle
                            _hotasFunctions = new ThrustmasterWarthogThrottleIndicators(this, "Indicators", "Warthog Throttle Indicators");
                            _hotasFunctions.CreateActionsAndValues();
                            if (!DesignMode)
                            {
                                _hotasDevice = DeviceList.Local.GetHidDevices().Where(d => d.VendorID == _device.Properties.VendorId && d.ProductID == _device.Properties.ProductId).FirstOrDefault();
                                if (_hotasDevice == null)
                                {
                                    Logger.Info($"Unable to find USB device with VendorID: {_device.Properties.VendorId} and ProductID: {_device.Properties.ProductId}.");
                                }
                            }
                            break;
                        case 0xb351: // Cougar MFD
                        case 0x0402: // Warthog Joystick
                        case 0xb68f: // T-Pendular-Rudder
                        default:
                            break;

                    }
                    break;
                case 0x0194:  // Virpil
                    _hotasFunctions = new VirpilHotasIndicators(this, "Indicators", "Virpil HOTAS Indicators");
                    _hotasFunctions.CreateActionsAndValues();
                    if (!DesignMode)
                    {
                        _hotasDevice = DeviceList.Local.GetHidDevices().Where(d => d.VendorID == _device.Properties.VendorId && d.ProductID == _device.Properties.ProductId).FirstOrDefault();
                        if (_hotasDevice == null)
                        {
                            Logger.Info($"Unable to find USB device with VendorID: {_device.Properties.VendorId} and ProductID: {_device.Properties.ProductId}.");
                        }
                    }
                    break;
                default:
                    break;
            } 
        }

        internal void SendUsbData(byte[] buffer)
        {
            if(_hotasDevice != null)
            {
                if (_hotasDevice.TryOpen(out _hotasStream))
                {
                    //Console.WriteLine("CanWrite: {0}", _hotasStream.CanWrite);
                    //byte[] indicators = new byte[] { 0x01, 0x06, 0x50, 0x01 };
                    try
                    {
                        if (_hotasStream.CanWrite)
                        {
                            _hotasStream.Write(buffer, 0, buffer.Length);
                            _hotasStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error while writing to {_hotasDevice.VendorID} {_hotasDevice.GetProductName()} {ex.Message}.");
                        if (_hotasStream != null)
                        {
                            _hotasStream.Close();     
                        }
                        _hotasDevice = null;
                    }
                }
            }
        }

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            if (oldProfile != null)
            {
                oldProfile.ProfileTick -= new EventHandler(Profile_ProfileTick);
            }

            if (Profile != null)
            {
                Profile.ProfileTick += new EventHandler(Profile_ProfileTick);
            }
            base.OnProfileChanged(oldProfile);
        }

        void Profile_ProfileTick(object sender, EventArgs e)
        {
            if (_device != null)
            {
                JoystickState state = GetState();
                foreach (DirectXControllerFunction function in _functions)
                {
                    function.PollValue(state);
                }
            }
        }

        internal JoystickState GetState()
        {
            if (_device != null)
            {
                return _device.GetCurrentState();
            }
            return new JoystickState();
        }

        private void AddFunction(DirectXControllerFunction function)
        {
            if (function != null)
            {
                if (!Values.Contains(function.Value))
                {
                    Values.Add(function.Value);
                    foreach (IBindingTrigger trigger in function.Triggers)
                    {
                        Triggers.Add(trigger);
                    }

                    if (!_functions.Contains(function))
                    {
                        _functions.Add(function);
                        Logger.Info($"Adding {function.Name}. Function: {function}, Product Name {_deviceId.ProductName} GUID {_deviceId.InstanceGuid}");
                    }
                    else
                    {
                        Logger.Error($"Attempt to add {function.Name} which already exists. Function: {function}, Product Name {_deviceId.ProductName} GUID {_deviceId.InstanceGuid}");
                    }
                }
                else
                {
                    Logger.Error($"Attempt to add Value {function.Value.Name} which already exists. Function: {function}, Product Name {_deviceId.ProductName} GUID {_deviceId.InstanceGuid}");
                }
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            string name = reader.GetAttribute("Name");
            string guid = reader.GetAttribute("GUID");
            reader.ReadStartElement("Controller");
            name = reader.ReadElementString("Name");
            guid = reader.ReadElementString("GUID");
            ControllerId = new DirectXControllerGuid(name, new Guid(guid));
            if (_device == null)
            {
                if (!reader.IsEmptyElement)
                {
                    reader.ReadStartElement("Functions");
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.ReadStartElement("Function");
                        string functionName = reader.ReadElementString("Type");
                        int objectNumber = int.Parse(reader.ReadElementString("Number"), CultureInfo.InvariantCulture);
                        reader.ReadEndElement();
                        AddFunction(DirectXControllerFunction.CreateDummy(this, functionName, objectNumber));
                    }
                    reader.ReadEndElement();
                }
                else
                {
                    reader.Read();
                }
            }
            else
            {
                reader.Skip();
            }
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Controller");

            writer.WriteElementString("Name", _deviceId.ProductName);
            writer.WriteElementString("GUID", _deviceId.InstanceGuid.ToString());

            writer.WriteStartElement("Functions");
            foreach (DirectXControllerFunction function in _functions)
            {
                writer.WriteStartElement("Function");
                writer.WriteElementString("Type", function.FunctionType);
                writer.WriteElementString("Number", function.ObjectNumber.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
