using Eric.Morrison.Logging;
using LifxMvc.Services.Discovery;
using LifxNet;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LifxMvc.Services.UdpHelper
{
	public class DiscoveryUdpHelper : IDisposable
	{
        private const int PORT_NO = 56700; // LIFX Communication port
        private const int SERVICE_PORT_NO = 0; // Our discovery port
        public static IPAddress LocalEndpointIpAddress = IPAddress.Any;

        const int LISTEN_DICOVERY_TIMEOUT = 15000; // 15 seconds

        const int UDP_SERVICE_SUPPORTED = 1;

        bool KEEP_LISTENING = true;

        private UdpClient discoverySocket = null;

        //private ConcurrentDictionary<string, int> discoveredBulbList = new ConcurrentDictionary<string, int>();

        DiscoveryService DiscoverySvc = null;

        Task ListeningTask { get; set; }
        DebugLogger Logger { get; set; }

		class ListenContext
		{
			public int Timeout { get; set; }

			public ListenContext(int expectedCount, int timeout)
			{
				this.Timeout = timeout;
			}
		}
        //--------------------------------------------------------------------

        public void SetLocalEndPoint(IPAddress localIP)
        {
            LocalEndpointIpAddress = localIP;
        }
        //--------------------------------------------------------------------

        private void CreateUdpClient()
        {
            discoverySocket = new UdpClient();

            discoverySocket.DontFragment = true;
            discoverySocket.EnableBroadcast = true;

            discoverySocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            discoverySocket.Client.Bind(new IPEndPoint(LocalEndpointIpAddress, SERVICE_PORT_NO));            
        }
        //--------------------------------------------------------------------

        private void CloseUdpClient()
        {
            discoverySocket.Close();
            //discoverySocket.Dispose();
        }
        //--------------------------------------------------------------------

        public DiscoveryUdpHelper()
		{
			this.Logger = new DebugLogger();
		}
        //--------------------------------------------------------------------
        
        public void StopDiscoveringBulbs()
        {
            KEEP_LISTENING = false;

            CloseUdpClient();
        }
        //--------------------------------------------------------------------
        
        public void DiscoverBulbs(DiscoveryService ds, LifxPacketBase packet)
		{
            DiscoverySvc = ds;

			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			try
			{
                // Start the discovery thread
                StartDiscovering(packet);
			}
			catch (Exception ex)
			{
                Debug.WriteLine(ex.ToString());
				throw;
			}
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}
        //--------------------------------------------------------------------

        void StartDiscovering(LifxPacketBase packet)
        {
            var discoverProc = new Thread(this.Discover);
            discoverProc.Start(packet);
        }
        //--------------------------------------------------------------------  

        void Discover(object ob)
        {
            // SEND_DISCOVERY_WAIT is used to define how often we will broadcast a discovery request

            LifxPacketBase packet = ob as LifxPacketBase;

            Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
            Byte[] data = packet.Serialize();

            TraceData(data);
            IPAddress broadcastIP = IPAddress.Broadcast;
            IPEndPoint destEP = new IPEndPoint(broadcastIP, PORT_NO);
            packet.IPEndPoint = destEP;

            CreateUdpClient();

            // Broadcast out the discovery packet
            SendPacket(data, destEP, packet);

            IPEndPoint foundObject = new IPEndPoint(IPAddress.Any, 0);

            while (KEEP_LISTENING)
            {
                try
                {                    
                    var asyncResult = discoverySocket.BeginReceive(null, null);
                    var signaled = asyncResult.AsyncWaitHandle.WaitOne(LISTEN_DICOVERY_TIMEOUT);
                    if (signaled)
                    {
                        if (asyncResult.IsCompleted)
                        {
                            byte[] objData = discoverySocket.EndReceive(asyncResult, ref foundObject);

                            // parse the response.
                            var response = ResponseFactory.Parse(objData, foundObject);
                            response.TraceReceived(discoverySocket.Client.LocalEndPoint);

                            // Make sure we received the correct message type and Service supported
                            if (response is DeviceStateServiceResponse && ((LifxNet.DeviceStateServiceResponse)(response)).Service == UDP_SERVICE_SUPPORTED)
                            {
                                string endPoint = foundObject.Address.ToString();
                                
                                // Bundle the response and sender info into a single package
                                var ctx = new DiscoveryContext(foundObject, response as DeviceStateServiceResponse);

                                // Fire off a thread to read the bulb properties
                                OnBulbDiscovered(ctx);
                            }
                            else
                            {
                                //This is a random response broadcast from a bulb.
                                Debug.WriteLine("Ignored response {0} from {1}", response.GetType().ToString(), foundObject.Address.ToString());
                            }
                        }
                    }
                    else
                    {
                        // Re-establish the UDP connection.
                        // It seems that the udp connections will stop receiving data after aperiod of time.
                        CloseUdpClient();
                        CreateUdpClient();

                        SendPacket(data, destEP, packet);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Console.WriteLine("ERROR: {0}:{1}", MethodInfo.GetCurrentMethod(), e.Message);
                    Console.WriteLine("Waiting 30 Seconds before trying to re-establish connection");
                    System.Threading.Thread.Sleep(30000);

                    // Re-establish the UDP connection.
                    // It seems that the udp connections will stop receiving data after aperiod of time.
                    CloseUdpClient();
                    CreateUdpClient();
                }
            }

            Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        }
        //--------------------------------------------------------------------

        private void SendPacket(Byte[] data, IPEndPoint destEP, LifxPacketBase packet)
        {
            try
            {
                Console.WriteLine("Send Discovery Packet...");
                Debug.WriteLine("{0}{1}", DateTime.Now.ToString("HH:mm:ss.ffff"), " --- Send Service Discovery Packet");
                discoverySocket.Send(data, data.Length, destEP);
                packet.TraceSent(discoverySocket.Client.LocalEndPoint);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Console.WriteLine("ERROR: {0}:{1}", MethodInfo.GetCurrentMethod(), e.Message);
            }
        }
        //--------------------------------------------------------------------

		void OnBulbDiscovered(DiscoveryContext ctx)
		{
            var sender = ctx.Sender.ToString();
            Debug.Assert(!string.IsNullOrEmpty(sender));
            DiscoverySvc.Udp_DeviceDiscovered(this, new DiscoveryEventArgs(ctx));
		}
        //--------------------------------------------------------------------

		static void TraceData(byte[] data)
		{
#if true
			if (null != data)
			{
				//System.Diagnostics.Debug.WriteLine(
				//	string.Join(",", (from a in data select Convert.ToString(a, 2).PadLeft(8, '0')).ToArray()));

				System.Diagnostics.Debug.WriteLine(
					string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
			}

#endif
		}
        //--------------------------------------------------------------------

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
                //if (disposing)
                //{
                //    this.StopListening();
                //}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DiscoveryUdpHelper() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}//class 



}//ns
