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
    class LifxAdapter : Adapter
    {
        // Class Constants
        const int CHECK_FOR_NEW_BULBS_WAIT = 5000;

        // Class variables
        static int count = 1;
        bool _isRunningAsService = false;

        // Create an instance of the LifxDriver
        private LifxDriver driver = null;
                

        public LifxAdapter(IPAddress localEndpoint)
        {
            driver = new LifxDriver(localEndpoint);
        }
        //--------------------------------------------------------------------

        public override bool Initialize(Uri baseUri, bool isRunningAsService = false)
        {
            bool isSuccessful = true;

            _isRunningAsService = isRunningAsService;

            // Console Title is not accessable if running as a service
            if (!_isRunningAsService)
                Console.Title = String.Format("Total Bulbs Found: 0");

            Console.WriteLine("Initializing adapter..");

            isSuccessful = driver.DiscoverBulbs();
            StartCheckingForNewBulbs(baseUri);

            return isSuccessful;
        }
        //--------------------------------------------------------------------

        public override object Status()
        {
            return true;
        }
        //--------------------------------------------------------------------

        public override void Stop()
        {
            driver.StopDiscovery();
        }
        //--------------------------------------------------------------------

        void StartCheckingForNewBulbs(Uri baseUri)
        {
            var listenProc = new Thread(CheckForNewBulbs);
            listenProc.Start(baseUri);
        }
        //--------------------------------------------------------------------

        void CheckForNewBulbs(object ob)
        {
            try
            {
                Uri baseUri = ob as Uri;
                int bulbLabelIndex = 1;
                int TotalBulbCount = 0;

                while (true)
                {
                    List<object> bulbs = driver.GetNewBulbs();

                    foreach (object b in bulbs)
                    {
                        string id_string;
                        string label = driver.GetBulbObjectLabel(b);
                        if (String.IsNullOrEmpty(label))
                        {
                            id_string = String.Format("lifx_{0}", bulbLabelIndex++);
                        }
                        else
                        {
                            id_string = String.Format("lifx_{0}", label.Replace(' ', '_'));
                        }                    

                        // Create a LIFX Bulb Object
                        LIFX_WIFI_LIGHT lifxLamp = new LIFX_WIFI_LIGHT(new Uri(baseUri, id_string));

                        // Set some properties of the new Bulb Object
                        lifxLamp.Id = id_string;
                        lifxLamp.SubsystemContext = b;

                        // Add the new object to our dictionary
                        RaiseOnThingAdded(lifxLamp);

                        TotalBulbCount++;

                        Console.WriteLine("Label: " + id_string + ":" + driver.GetBulbEndPoint(b));
                    }

                    if (bulbs.Count > 0)
                    {
                        Console.WriteLine("Found {0} new bulb(s)  --  Total Bulbs: {1}", bulbs.Count, TotalBulbCount);

                        // Console Title is not accessable if running as a service
                        if (!_isRunningAsService)
                            Console.Title = String.Format("Total Bulbs Found: {0}", TotalBulbCount);

                        Console.WriteLine("Listening...");
                    }

                    // Wait some amount of time before checking for new bulbs
                    System.Threading.Thread.Sleep(CHECK_FOR_NEW_BULBS_WAIT);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
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
                case "Power":
                    value = driver.GetBulbPower(property.Parent.SubsystemContext); 
                    break;

                case "Color":
                    value = driver.GetBulbColor(property.Parent.SubsystemContext); 
                    break;

                case "Brightness":
                    value = driver.GetBulbBrightness(property.Parent.SubsystemContext);
                    break;

                case "Saturation":
                    value = driver.GetBulbSaturation(property.Parent.SubsystemContext);
                    break;

                case "Kelvin":
                    value = driver.GetBulbKelvin(property.Parent.SubsystemContext); 
                    break;

                case "Label":
                    value = driver.GetBulbObjectLabel(property.Parent.SubsystemContext);
                    break;

                default:
                    break;
            }
                
            sw.Stop();
            string offline = "";
            if (driver.IsBulbOffline(property.Parent.SubsystemContext))
                offline = "--offline";
            Console.WriteLine("GET:{0}\t -- TimeElapsed: {1}   ({2})  {3} {4}", property.Name, sw.Elapsed, count++, driver.GetBulbObjectLabel(property.Parent.SubsystemContext), offline);

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
                case "Power": 
                    returnStatus = driver.SetBulbPower(property.Parent.SubsystemContext, value); 
                    break;

                case "Color": 
                    returnStatus = driver.SetBulbColor(property.Parent.SubsystemContext, value); 
                    break;

                case "Brightness": 
                    returnStatus = driver.SetBulbBrightness(property.Parent.SubsystemContext, value); 
                    break;

                case "Saturation": 
                    returnStatus = driver.SetBulbSaturation(property.Parent.SubsystemContext, value); 
                    break;

                case "Kelvin": 
                    returnStatus = driver.SetBulbKelvin(property.Parent.SubsystemContext, value); 
                    break;

                default: 
                    break;
            }

            string offline = "";
            if (driver.IsBulbOffline(property.Parent.SubsystemContext))
                offline = "--offline";
            sw.Stop();
            Console.WriteLine("PUT:{0}\t -- TimeElapsed: {1}   ({2})  {3} {4}", property.Name, sw.Elapsed, count++, driver.GetBulbObjectLabel(property.Parent.SubsystemContext), offline);

            return returnStatus;
        }
        //--------------------------------------------------------------------
    }
}
