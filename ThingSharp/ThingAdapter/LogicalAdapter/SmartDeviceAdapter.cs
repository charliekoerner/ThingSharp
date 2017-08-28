using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ThingSharp.Bindings;
using ThingSharp.Drivers;
using ThingSharp.Server;
using ThingSharp.Types;
using ThingSharp.Utils;
using SorisAdapter;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ThingSharp.ThingSharp
{
    class SmartDeviceAdapter : Adapter
    {
        // Class variables
        static int count = 1;

        // Create an instance of the SmartDeviceDriver
        private SmartDeviceDriver driver = null;

        public SmartDeviceAdapter(IPAddress localEndpoint)
        {
            driver = new SmartDeviceDriver(localEndpoint);
        }
        //--------------------------------------------------------------------

        public override bool Initialize(Uri baseUri, bool isRunningAsService = false)
        {
            bool isSuccessful = true;

            //******************************************************
            // #TODO: Uncomment line once Adapter code is updated
            //Console.WriteLine("Initializing adapter..");
            //******************************************************

            // Remove Console.WriteLines below once code is updated.
            Console.WriteLine("Adapter code is empty");
            Console.WriteLine("Search project for '#TODO' to find all locations that require changes.");

            //***************************************************************************************
            // #TODO: DISCOVERY CODE
            //
            // The Initialize function is called when the project is launched.  This is where
            // the driver discovery method is called. 
            //
            // The code below gets a list of smart devices from the driver and creates objects
            // based off the Data Model that was imported from DCC. For more information about
            // creating and importing the Data Model, see the help documentation.
            //
            //
            // Example: For a detailed example of working code, download and review the SORIS Server 
            //          project created for the Lifx LED Smart Light from: 
            //          <path>
            //
            //***************************************************************************************

            List<object> devices = driver.DiscoverSmartDevices();

            foreach (object device in devices)
            {
                //**********************************************************************************************************
                // #TODO: CREATE SMART DEVICE OBJECTS
                //        Un-comment the following lines of code that start with '//(uncomment)' and update with 
                //        the followng: 
                //        - Data Model Class Name - this is defined in teh data model created by DCC
                //        - Label String - This label is used to find and command the Device, so it must be unique
                //        - SubsystemContext - this holds a unique reference to the Device Object
                //**********************************************************************************************************

                // Create a Smart Device Object
                // Replace the text 'REPLACE_WITH_DATA_MODEL_CLASS_NAME' with the actual class name from the Data Model that was imported from DCC                
                //(uncomment)REPLACE_WITH_DATA_MODEL_CLASS_NAME smartDevice = new REPLACE_WITH_DATA_MODEL_CLASS_NAME(new Uri(baseUri, replace_with_unique_object_label_string));

                // REQUIRED:
                // Need to set the following properties of the new Smart Device Object:

                //  -The device ID. This is used in the address path.
                //(uncomment)smartDevice.Id = replace_with_unique_object_label_string;

                //  - SubsystemContext. This holds the object reference of the smart device. Depending on how
                //                      the Smart Device Driver is written, may hold an Object Model or just 
                //                      an address to the device. This data passed along to the driver, so it needs
                //                      to store whatever is needed to know what Object we're dealing with. 
                //                      In the Lifx example project, the bulb object is stored and passed back to the 
                //                      Lifx Service for every call so it knows what bulb we need to comminucate with.
                //(uncomment)smartDevice.SubsystemContext = device;

                // Add the new object to the dictionary
                //(uncomment)RaiseOnThingAdded(smartDevice);          

                //**********************************************************************************************************
            }

            return isSuccessful;
        }
        //--------------------------------------------------------------------

        public override void Stop()
        {
            // Add code to properly shut down the Smart Device Service
        }
        //--------------------------------------------------------------------

        public override object Read(Resource obj)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (obj is Thing)
                return obj;

            object value = null;

            PropertyBase property = (PropertyBase)obj;

            switch (property.Name)
            {

                //*************************************************************
                // #TODO: READ REQUESTS
                //        Create case statements for Read(Get) requests
                //        The Case will call the appropriate Driver Method
                //
                // Example:
                //
                // case "Power":
                //    value = driver.GetDevicePower(property.Parent.SubsystemContext);
                //    break;        
                //
                //*************************************************************

                default:
                    break;
            }

            sw.Stop();
            Console.WriteLine("GET:{0}\t -- TimeElapsed: {1}   ({2})", property.Name, sw.Elapsed, count++);

            return value;
        }
        //--------------------------------------------------------------------

        public override object Write(Resource obj, object value)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (obj is Thing)
                return "Thing Description";

            object returnStatus = null;

            PropertyBase property = (PropertyBase)obj;

            switch (property.Name)
            {

                //*************************************************************
                // #TODO: WRITE REQUESTS
                //        Create case statements for Write(Put) requests
                //
                // Example:
                //
                // case "Power":
                //    returnStatus = driver.SetDevicePower(property.Parent.SubsystemContext, value);
                //    break;   
                //
                //*************************************************************

                default:
                    break;
            }

            sw.Stop();
            Console.WriteLine("PUT:{0}\t -- TimeElapsed: {1}", property.Name, sw.Elapsed);

            return returnStatus;
        }
        //--------------------------------------------------------------------

    }
}
