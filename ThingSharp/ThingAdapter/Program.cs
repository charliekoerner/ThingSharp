using System;
using System.IO;
using System.Net;
using ThingSharp.Bindings;
using ThingSharp.Server;
using ThingSharp.Types;
using ThingSharp.Utils;
using System.ServiceProcess;
using System.Reflection;

using System.ServiceModel.Web;


namespace ThingSharp.ThingSharp
{
    class Program
    {
        // ServiceName is required by the ServiceBase interface
        public const string ServiceName = "";

        private static ThingServer _Server = null;
        private static ArgParser _Args = null;

        /// <summary>
        /// Main Entry point
        /// </summary>
        static void Main(string[] args)
        {
            string myExeDir = System.IO.Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            string appName = Assembly.GetExecutingAssembly().GetName().Name;

            _Args = new ArgParser(args);

            ServiceManager SvcMgr = new ServiceManager();

            if (SvcMgr.IsService(appName))
            {
                // running as service
                using (var service = new Service())
                    ServiceBase.Run(service);
            }
            else
            {
                // running as console app                

                if (_Args.exists(@"-?") || _Args.exists(@"/?"))
                {
                    usage(appName);
                }
                else
                {
                    if (!_Args.VerifyFormat())
                    {
                        // If the args were entered incorrectly, then display the usage
                        // repoart and quit.
                        Console.WriteLine("ERROR: Command line arguments are not formatted correctly.");
                        usage(appName);
                        Console.Read();
                        return;
                    }

                    if (_Args.exists(@"-service"))
                    {
                        SvcMgr.InstallService(_Args);
                    }
                    else if (_Args.exists(@"-uninstall"))
                    {
                        SvcMgr.UninstallService();
                    }
                    else
                    {
                        // Check if the App is also installed as a service
                        SvcMgr.CheckIfServiceIsInstalled(appName);

                        // Run the Program normally
                        Start(args, false);

                        //Thats it. Press any key to end the show.
                        Console.Read();
                        Stop();
                        
                    }
                }
            }
        }
        //--------------------------------------------------------------------

        /// <summary>
        /// Service Start
        /// </summary>
        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args, true);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        //--------------------------------------------------------------------

        /// <summary>
        /// Does all the work to link upa nd start the Adapter
        /// </summary>
        private static void Start(string[] args, bool isService)
        {
            IPAddress localEndpoint;
            int httpPort = 8080;
            string ipDisplay = String.Empty;
            string portDisplay = String.Format("{0} (automatic)", httpPort);
            string securityDisplay = "(unsecure)";
            string urlPrefix = "http";


            if (_Args.exists(@"-ip"))
            {
                localEndpoint = IPAddress.Parse(_Args.getValue(@"-ip"));
                ipDisplay = String.Format("{0} (manual)", localEndpoint);

            }
            else
            {
                // Find the IP Address of the Wireless Connection
                IpAddressHelper IpHelper = new IpAddressHelper();
                string ipString = IpHelper.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211);
                if (!string.IsNullOrEmpty(ipString))
                {
                    localEndpoint = IPAddress.Parse(ipString);
                    ipDisplay = String.Format("{0} (automatic)", localEndpoint);
                }
                else
                {
                    Console.WriteLine("Error: Could not automatically retreive the IP address for the Wireless adapter. \n\nPlease make sure the wireless adapter is turned on and connected to the network.");
                    Console.Read();
                    return;
                }
            }

            if (_Args.exists(@"-port"))
            {
                httpPort = int.Parse(_Args.getValue(@"-port"));
                portDisplay = String.Format("{0} (manual)", httpPort);
            }

            if(_Args.exists(@"-secure"))
            {
                securityDisplay = "(secure)";
                urlPrefix = "https";
            }            

            String httpEnpoint = String.Format("{0}://{1}:{2}/", urlPrefix, localEndpoint.ToString(), httpPort);

            Console.WriteLine("EndPoint:");
            Console.WriteLine("      IP: {0}", ipDisplay);
            Console.WriteLine("    Port: {0}", portDisplay);
            Console.WriteLine("Security: {0} {1}", urlPrefix, securityDisplay);
            Console.WriteLine("-------------------------------------------");

            // First we choose what kind of protocol binding we want on top.
            // At the moment we have HTTP in stock, but expect WebSockets, CoAP etc next season.
            Uri baseUri = new Uri(httpEnpoint);
            ProtocolBinding httpBinding = new HTTPBinding(new string[] { httpEnpoint });

            //Next we create an Adapter. The Adapter is the stuff you will develop. It contains
            // a "logical adaption" and a driver layer.
            Adapter adapter1 = new LifxAdapter(localEndpoint);

            //Now, we glue the protocol binding and the adapter using the ThingServer.
            _Server = new ThingServer(httpBinding, adapter1);

            //Things are glued. Lets kick start the adapter. This should prompt it to setup its driver,
            //discover datapoints in the sub-system below and then create intances of Things (which will be stored 
            //in the resource container of the ThingServer).
            bool successful = adapter1.Initialize(baseUri, isService);
            _Server.SetStatus(successful);

            if (successful)
            {
                //Finally, now that the Driver, Adapter, and the ThingServer are ready to serve out Things, we
                //will ask the ThingServer to open its north-side doors! (i.e. enable the protocol binding endpoint to listen).
                _Server.Start();
            }
        }
        //--------------------------------------------------------------------

        /// <summary>
        /// Stops all listening threads and closes socket bindings
        /// </summary>
        private static void Stop()
        {
            _Server.Stop();
        }
        //--------------------------------------------------------------------
        
        /// <summary>
        /// Display Console Usage
        /// </summary>
        private static void usage(string appName)
        {
            string description = String.Format(
                " DESCRIPTION:\n" +
                "        The Adapter is a program that allows communication between the \n" +
                "        Management Workstation and a Smart Device service. \n" +
                "        The Adapter can be ran as either a command line program, or \n" +
                "        installed to run as a service.\n");
            string usage = String.Format(
                " USAGE:\n" +
                "        <AdapterName>.exe -[command] -<option1> -<option2>\n\n" +
                "        NOTE: All commands and options need to start with a dash '-'\n\n" +
                "        Commands:\n" +
                "          service-------Installs and starts Adapter as a service. If \n" +
                "                         already installed as a service, this command\n" +
                "                         will update the service command line arguments.\n" +
                "          uninstall-----Stops and Uninstalls the service.\n\n" +
                "          secure--------Opens the TCP connection with HTTPS security." +
                "                        NOTE: Requires a Security Certificate to being installed." +
                "        Options:\n" +
                "          ip:<address>--Manually set the IP address used to communicate on the\n" +
                "                         desired network.\n" +
                "                         By default, the IP address of the wireless network \n" +
                "                         adapter will be used.\n" +
                "          port:<port>---Manually set the Port number used to communicate with \n" +
                "                         the Management Workstation. If multiple adpaters are \n" +
                "                         running on the same machine, each one needs a \n" +
                "                         different port number.\n" +
                "                         Default port is 8080.\n");
            string example = String.Format(
                " EXAMPLE:\n" +
                "        <AdapterName>.exe -service -ip:192.168.1.123 -port:8085 \n\n" +
                "        -- this example will install the Adapter as a service with the\n" +
                "           entered IP and Port as command line arguments.\n");

            Console.WriteLine(description);
            Console.WriteLine(usage);
            Console.WriteLine(example);
        }
    }
}
