using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.LibUsb;
using System.Linq;

namespace Examples
{
    public class USBTesting
    {
        //private const int ProductId = 0xb351;
        private const int ProductId = 0x0404;
        private const int VendorId = 0x044f;
        public static IUsbDevice usbDevice;

        public static void Main(string[] args)
        {
            Error ec = Error.Success;
            using (UsbContext context = new UsbContext())
            {
                try
                {
                    //Get a list of all connected devices
                    var usbDeviceCollection = context.List();

                    //Narrow down the device by vendor and pid
                    var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == ProductId && d.VendorId == VendorId);
                    if (selectedDevice != null)
                    {
                        usbDevice = selectedDevice as IUsbDevice;
                        usbDevice.Open();
                    }
                    // If the device is open and ready
                    if (usbDevice == null) throw new Exception("Device Not Found.");

                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    if (!ReferenceEquals(usbDevice, null))
                    {
                        // This is a "whole" USB device. Before it can be used, 
                        // the desired configuration and interface must be selected.
                        usbDevice.SetConfiguration(1);  // Select config #1                    
                        usbDevice.ClaimInterface(0);    // Claim interface #0.
                        var reader = usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                        var writer = usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
                        byte[] readBuffer = new byte[36];
                        int bytesRead;

                        // If the device hasn't sent data in the last 5 seconds,
                        // a timeout error (ec = IoTimedOut) will occur. 
                        ec = reader.Read(readBuffer, 5000, out bytesRead);

                        if (bytesRead == 0) throw new Exception(string.Format("{0}:No more bytes!", ec));

                        byte[] writeBuffer = new byte[36];
                        writeBuffer[0] = 1;
                        writeBuffer[1] = 6;
                        writeBuffer[2] = 1; // LEDs
                        writeBuffer[3] = 1; // Intensity
                        // Console.Write(Encoding.Default.GetString(writeBuffer, 0, 36));

                        ec = writer.Write(writeBuffer, 1000, out int bytesWritten);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine((ec != Error.Success ? ec + ":" : String.Empty) + ex.Message);
                }
                finally
                {
                    if (usbDevice != null)
                    {
                        if (usbDevice.IsOpen)
                        {
                            // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                            // it exposes an IUsbDevice interface. If not (WinUSB) the 
                            // 'wholeUsbDevice' variable will be null indicating this is 
                            // an interface of a device; it does not require or support 
                            // configuration and interface selection.
                            IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                            if (!ReferenceEquals(wholeUsbDevice, null))
                            {
                                wholeUsbDevice.ReleaseInterface(0);  // Release interface #0.
                            }
                            usbDevice.Close();
                        }
                        usbDevice.Dispose();
                    }


                    //int transferred;
                    //byte[] ctrlData = new byte[1];
                    //UsbSetupPacket setTestTypePacket =
                    //    new UsbSetupPacket((byte)(UsbCtrlFlags.Direction_In | UsbCtrlFlags.Recipient_Device | UsbCtrlFlags.RequestType_Vendor),
                    //        0x0E, 0x01, usbInterfaceInfo.Number, 1);
                    //transferred = MyUsbDevice.ControlTransfer(setTestTypePacket, ctrlData, 0, 1);
                }
            }
        }
    }
}