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

using CommandLine.Text;
using GadrocsWorkshop.Helios.Interfaces.DirectX;
using HidSharp;
using NLog;
using SharpDX.DirectInput;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GadrocsWorkshop.Helios.Interfaces.Vendor.Functions
{
    public class VirpilHotasIndicators : IHotasFunctions
    {

        private string _deviceName;
        private Joystick _device;
        private HidDevice _hotasDevice;
        private HidStream _hotasStream;

        private HeliosValue _indicatorValue;
        private HeliosAction _resetIndicatorAction;
        private DirectXControllerInterface _sourceInterface;
        private int _subDeviceCount = Enum.GetNames(typeof(VirpilSubDeviceFlagEnum)).Length;
        private const int _maxBufferSize = 38;
        private byte[][] _writeBuffers = new byte[][] { new byte[_maxBufferSize], new byte[_maxBufferSize], new byte[_maxBufferSize], new byte[_maxBufferSize], new byte[_maxBufferSize] };
        private TypeConverter _colorConverter = TypeDescriptor.GetConverter(typeof(Color));

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private enum VirpilSubDeviceFlagEnum
        {
            DEFAULT = 0x0064,
            ADDONGRIPS = 0x0065,
            ONBOARDLEDS = 0x0066,
            SLAVELEDS = 0x0067,
            EXTRALEDS = 0x0068
        }

        public VirpilHotasIndicators(DirectXControllerInterface sourceInterface, Joystick joystickDevice, string deviceName, string name)
        {
            _deviceName = deviceName;
            _device = joystickDevice;
            _sourceInterface = sourceInterface;
        }

        public void CreateActionsAndValues()
        {
            DoBuild();
        }

        private void DoBuild()
        {
            foreach (VirpilSubDeviceFlagEnum subDevice in Enum.GetValues(typeof(VirpilSubDeviceFlagEnum)))
            {
                int j = 0;
                _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x02;
                _writeBuffers[(int)subDevice - 0x64][j++] = (byte)subDevice;
                _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x25;  // length?
                _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x00;  // address?
                _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x00;  // address?

                for (int i = 0; i < 32; i++)
                {
                    _writeBuffers[(int)subDevice - 0x64][i + j] = (byte)0x00; // start without changing any of the indicators.
                    if (subDevice != VirpilSubDeviceFlagEnum.DEFAULT)
                    {
                        _indicatorValue = new HeliosValue(_sourceInterface, new BindingValue(null), $"{subDevice} {_deviceName}", $"indicator color {i + 1}", $"the #AARRGGBB color value of indicator {i + 1} on {subDevice}.", "Color in the hex format #AARRGGBB.", BindingValueUnits.Text);
                        _indicatorValue.Execute += new HeliosActionHandler(IndicatorAction_Execute);
                        _sourceInterface.Values.Add(_indicatorValue);
                        _sourceInterface.Actions.Add(_indicatorValue);
                    }
                }
                _writeBuffers[(int)subDevice - 0x64][_maxBufferSize-1] = (byte)0xf0;   // marker for the  end of the buffer
                if (subDevice != VirpilSubDeviceFlagEnum.DEFAULT)
                {
                    _resetIndicatorAction = new HeliosAction(_sourceInterface, $"{subDevice} {_deviceName}", $"{subDevice} Indicators", "reset", $"Sets all of the indicators on {subDevice} to off.");
                    _resetIndicatorAction.Execute += new HeliosActionHandler(Reset_Execute);
                    _sourceInterface.Actions.Add(_resetIndicatorAction);
                } else
                {
                    _resetIndicatorAction = new HeliosAction(_sourceInterface, "", $"{subDevice}", "set", $"Sends the DEFAULT command to the devices on the interface.");
                    _resetIndicatorAction.Execute += new HeliosActionHandler(SetDefault_Execute);
                    _sourceInterface.Actions.Add(_resetIndicatorAction);
                }
            }

            _resetIndicatorAction = new HeliosAction(_sourceInterface, "", "All Indicators", "reset", $"sets all of the indicators on this interface to off.");
            _resetIndicatorAction.Execute += new HeliosActionHandler(Reset_Execute);
            _sourceInterface.Actions.Add(_resetIndicatorAction);
            _resetIndicatorAction = new HeliosAction(_sourceInterface, "", "All Indicators", "refresh", $"refreshes all of the indicators to the last values set on this interface.");
            _resetIndicatorAction.Execute += new HeliosActionHandler(Reset_Execute);
            _sourceInterface.Actions.Add(_resetIndicatorAction);

        }

        void IndicatorAction_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hAction)
            {
                if (Int32.TryParse(hAction.Name.Split(' ')[2], out Int32 indicatorIndex))
                {
                    if (Enum.TryParse(hAction.Device.Split(' ')[0], out VirpilSubDeviceFlagEnum subDevice))
                    {
                        byte oldValue = _writeBuffers[(int)subDevice - 0x64][indicatorIndex - 1 + 5];
                        oldValue |= (byte)0x80;

                        try
                        {
                            _writeBuffers[(int)subDevice - 0x64][indicatorIndex - 1 + 5] = VirpilColorConverter((Color)_colorConverter.ConvertFromInvariantString(e.Value.StringValue));
                            _writeBuffers[(int)subDevice - 0x64][indicatorIndex - 1 + 5] |= (byte)0x80; // set changed flag.
                            if (_writeBuffers[(int)subDevice - 0x64][indicatorIndex - 1 + 5] != oldValue) SendHidData(_writeBuffers[(int)subDevice - 0x64]);
                            _writeBuffers[(int)subDevice - 0x64][indicatorIndex - 1 + 5] &= (byte)0x7F;  //Reset the changed flag
                        }
                        catch
                        {
                            Logger.Warn($"Error converting color value. (Device=\"{hAction.Device}\" Name=\"{ hAction.Name}\", Value=\"{e.Value.StringValue}\")");
                        }
                    }
                }
            }
        }

        void Reset_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosAction hAction)
            {
                if (hAction.ActionVerb == "reset")
                {
                    if (hAction.Name == "All Indicators")
                    {
                        Reset();
                    }
                    else
                    {
                        if (Enum.TryParse(hAction.Name.Split(' ')[0], out VirpilSubDeviceFlagEnum subDevice))
                        {
                            Reset(subDevice);
                        }
                    }
                } else if (hAction.ActionVerb == "refresh")
                {
                    Refresh();
                } else
                {
                    // no code
                }
            }
        }

        void SetDefault_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosAction hAction)
            {
                if (hAction.ActionVerb == "set" && hAction.Name == $"{VirpilSubDeviceFlagEnum.DEFAULT}")
                {
                    int j = 0;
                    _writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64][j++] = (byte)0x02;
                    _writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64][j++] = (byte)VirpilSubDeviceFlagEnum.DEFAULT;
                    _writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64][j++] = (byte)0x25;                   // length?
                    _writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64][j++] = (byte)0x00;                   // address?
                    _writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64][j++] = (byte)0x00;                   // address?

                    _writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64][_maxBufferSize - 1] = (byte)0xf0;    // marker for the  end of the buffer
                    SendHidData(_writeBuffers[(int)VirpilSubDeviceFlagEnum.DEFAULT - 0x64]);
                }
            }
        }

        /// <summary>
        /// Convert a Color value to a value to be used by Viril and adjust for the alpha channel
        /// </summary>
        /// <param name="color">color object with alpha channel</param>
        /// <returns>byte in the form 0b11BBGGRR which is used by Virpil</returns>
        private byte VirpilColorConverter(Color color)
        {
            byte colorValue = 0x00;
            int alphaAdjustment = color.A >> 6;
            colorValue |= (byte)((byte)((Convert.ToInt16(color.B) / 3 * alphaAdjustment) >> 2) & (byte) 0b00110000);
            colorValue |= (byte)((byte)((Convert.ToInt16(color.G) / 3 * alphaAdjustment) >> 4) & (byte) 0b00001100);
            colorValue |= (byte)((byte)((Convert.ToInt16(color.R) / 3 * alphaAdjustment) >> 6) & (byte) 0b00000011);
            return colorValue;
        }

        public void OpenHidDevice() { OpenHidDevice(_device); }
        public void OpenHidDevice(Joystick device)
        {
            _hotasDevice = DeviceList.Local.GetHidDevices().Where(d => d.VendorID == device.Properties.VendorId && d.ProductID == device.Properties.ProductId && d.GetProductName() == device.Properties.ProductName).FirstOrDefault(d => d.GetMaxFeatureReportLength() > 0);
            if (_hotasDevice == null)
            {
                Logger.Info($"Unable to find USB device with VendorID: {device.Properties.VendorId} and ProductID: {device.Properties.ProductId}.");
                foreach (HidDevice hD in DeviceList.Local.GetHidDevices().Where(d => d.VendorID == device.Properties.VendorId && d.ProductID == device.Properties.ProductId))
                {
                    Logger.Debug($"#800 Unable to find USB device. HID Device ProductName={hD.GetProductName()} | Device ProductName={device.Properties.ProductName}");
                    Logger.Debug($"#800 Unable to find USB device. HID Device Dump: {hD}. MaxFeatureReportLength ({hD.GetMaxFeatureReportLength()}) MaxInputReportLength ({hD.GetMaxInputReportLength()}) DevicePath ({hD.DevicePath})");
                }
                /// TODO: Remove this next line when #800 is resolved.
                _hotasDevice = DeviceList.Local.GetHidDevices().Where(d => d.VendorID == device.Properties.VendorId && d.ProductID == device.Properties.ProductId).FirstOrDefault(d => d.GetMaxFeatureReportLength() > 0);
            }
        }

        /// <summary>
        /// Send the data to the HID Device
        /// </summary>
        /// <param name="buffer">The byte array containing the data to be reported</param>
        public void SendHidData(byte[] buffer)
        {
            if (_hotasDevice != null)
            {
                if (_hotasDevice.TryOpen(out _hotasStream))
                {
                    try
                    {
                        if (_hotasStream.CanWrite && _hotasDevice.GetMaxFeatureReportLength() >= buffer.Length)
                        {
                            _hotasStream.SetFeature(buffer, 0, buffer.Length);
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
                    if(_hotasStream != null) _hotasStream.Close();
                }
            }
        }

        /// <summary>
        /// Iterates through all of the subdevices and refreshes each one
        /// </summary>
        /// <remarks></remarks>
        public void Refresh()
        {
            Reset(true);
        }

        /// <summary>
        /// Iterates through all of the subdevices and resets each one
        /// </summary>
        /// <remarks>Resetting DEFAULT sets all the LEDS to yellow instead of Off so we don't do it.
        ///          DEFAULT also appears to not be interested in the contents of the indicator
        ///          buffer.</remarks>
        public void Reset()
        {
            Reset(false);
        }

        public void Reset(bool refresh)
        {
            foreach (VirpilSubDeviceFlagEnum subDevice in Enum.GetValues(typeof(VirpilSubDeviceFlagEnum)))
            {
                if (subDevice != VirpilSubDeviceFlagEnum.DEFAULT) Reset(subDevice, refresh);
            }
        }

        /// <summary>
        /// Resets a single Virpil subdevice
        /// </summary>
        /// <param name="subDevice">The Virpil sub device to be set to zero</param>
        private void Reset(VirpilSubDeviceFlagEnum subDevice, bool refresh = false)
        {
            int j = 0;
            _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x02;
            _writeBuffers[(int)subDevice - 0x64][j++] = (byte)subDevice;
            _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x25;  // length?
            _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x00;  // address?
            _writeBuffers[(int)subDevice - 0x64][j++] = (byte)0x00;  // address?

            for (int i = 0; i < 32; i++)
            {
                if (refresh)
                {
                    _writeBuffers[(int)subDevice - 0x64][i + j] |= (byte)0x80; // set the changed bit
                }
                else
                {
                    _writeBuffers[(int)subDevice - 0x64][i + j] = (byte)0x80; // clear and set the changed bit
                }
            }

            _writeBuffers[(int)subDevice - 0x64][_maxBufferSize - 1] = (byte)0xf0;   // marker for the  end of the buffer
            SendHidData(_writeBuffers[(int)subDevice - 0x64]);

            for (int i = 0; i < 32; i++)
            {
                _writeBuffers[(int)subDevice - 0x64][i + j] &= (byte)0x7f; // turn off the changed bit
            }
        }
    }
}

