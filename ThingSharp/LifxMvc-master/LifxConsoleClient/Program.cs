using LifxMvc.Domain;
using LifxMvc.Services;
using LifxMvc.Services.UdpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LifxConsoleClient
{
    class Program
    {
        private Program() { }
        private static Program instance = new Program();

        static void Main(string[] args)
        {
            string ipString = instance.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            DiscoveryUdpHelper.LocalEndpointIpAddress = IPAddress.Parse(ipString);

            Console.WriteLine("EndPoint (automatic): " + ipString + ":8080");

            DiscoveryService disco = new DiscoveryService(IPAddress.Parse(ipString));
            BulbService bulbService = new BulbService();             

            // Tell the bulb discovery thread to start
            Console.WriteLine("Listening...");
            disco.DiscoverAsync();

            // Get Found bulbs
            List<IBulb> bulbs = new List<IBulb>();
            while(bulbs.Count == 0)
            {
                // sleep for a little while to let the discovery thread find bulbs
                System.Threading.Thread.Sleep(1000);

                bulbs = disco.GetDiscoveredBulbs();
            }

            Console.WriteLine("Found {0} bulbs", bulbs.Count);
            
            //-----------------------------------------------------------------
            // Now that we found 1 or more bulbs, just do a quick test
            // with the first bulb in the list
            //-----------------------------------------------------------------


            IBulb bulb = bulbs[0];
            //IBulb bulb = bulbs.FirstOrDefault(x => x.Label == "CK Bulb");

            if (bulb != null)
            {
                Console.WriteLine("Do quick color changing test for: {0}", bulb.Label);

                // Turn bulb on
                bulbService.LightSetPower(bulb, true);
                
                // Change Color
                bulbService.LightSetColor(bulb, System.Drawing.Color.FromArgb(255, 0, 0, 128));
                System.Threading.Thread.Sleep(500);

                double red, green, blue;

                for (var frequency = .0; frequency < 1; frequency += 0.3)
                {
                    for (var i = 0; i < 32; ++i)
                    {
                        red = Math.Sin(frequency * i + 0) * 127 + 128;
                        green = Math.Sin(frequency * i + 2) * 127 + 128;
                        blue = Math.Sin(frequency * i + 4) * 127 + 128;

                        bulbService.LightSetColor(bulb, System.Drawing.Color.FromArgb(255, (int)red, (int)green, (int)blue));

                        System.Threading.Thread.Sleep(100);

                    }
                }

                Console.WriteLine("** Test Finished ***");
            }            
        }
        //--------------------------------------------------------------------

        private string GetLocalIPv4(NetworkInterfaceType type)
        {
            List<string> ipAddrList = new List<string>();

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.FirstOrDefault();
        }
        //--------------------------------------------------------------------
    }
}
