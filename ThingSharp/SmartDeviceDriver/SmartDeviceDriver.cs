using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThingSharp.Drivers
{
    public class SmartDeviceDriver
    {
        // #TODO: DEVICE SERVICE REFERENCE
        // Example: Setting up a global variable to the Smart Device Service (DLL)
        //          The 'DeviceService' reference will change to match the Class name
        //          defined in the DLL being referenced.
        //
        //private DeviceService mDeviceService;

        // Constructor 
        public SmartDeviceDriver(IPAddress localEndpoint)
        {
            // #TODO: INITIALIZE SERVICE REFERENCE
            // Example: Initializing the Device Service instance.
            //mDeviceService = new DeviceService(localEndpoint);
        }

        //********************************************************************
        // Discovery
        //********************************************************************

        public List<Object> DiscoverSmartDevices()
        {
            List<object> smartDevices = new List<object>();

            // #TODO: DEVICE DISCOVERY
            //        Add discovery call to Smart Device Service Library
            // Example: Call the Discovery Service to find a list of all the 
            //          devices on the network. The method being called will
            //          need to change to match the device service(DLL) being referenced.
            //
            //smartDevices = mDeviceService.GetDiscoveredDevices();

            return smartDevices;
        }
        //-------------------------------------------------------------

        //********************************************************************
        // Read / Write Property Calls Go Below
        //********************************************************************

        // #TODO: CONNECTION WITH DEVICE SERVICE
        //        Add read/write calls to the Smart Device Service Library
        //
        // Example:
        // Power - Get/Set
        // Note: Update the return value to appropraite type
        //
        //public <variable_type> GetDevicePower(Object device)
        //{
        //    IDevice b = (IDevice)device;
        //    return mDeviceService.DeviceGetPower(b);
        //}
        ////-------------------------------------------------------------
        //public <variable_type> SetDevicePower(Object device, object state)
        //{
        //    IDevice b = (IDevice)device;
        //    return mDeviceService.DeviceSetPower(b, (bool)state);
        //}
        ////-------------------------------------------------------------
    }


}
