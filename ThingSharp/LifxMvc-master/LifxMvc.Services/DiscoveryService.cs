using LifxMvc.Domain;
using LifxMvc.Services.Discovery;
using LifxMvc.Services.UdpHelper;
using LifxNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace LifxMvc.Services
{
	public class DiscoveryService : IDisposable, IDiscoveryService
	{
		ConcurrentBag<IBulb> Bulbs { get; set; }
        ConcurrentBag<IBulb> NewBulbList { get; set; }
		ManualResetEventSlim Wait { get; set; }

        DiscoveryUdpHelper _udp = null;

        Stopwatch _sw;

        public DiscoveryService(IPAddress localIP)
        {
            _udp = UdpHelperManager.Instance.DiscoveryUdpHelper;
            _udp.SetLocalEndPoint(localIP);
        }

        public void StopDiscovery()
        {
            _udp.StopDiscoveringBulbs();
        }

        public List<IBulb> GetDiscoveredBulbs()
        {
            List<IBulb> bulbList = new List<IBulb>();

            // Copy the bulb list to send back because we are going to clear out the new bulb list before returning
            foreach (IBulb b in NewBulbList)
            {
                bulbList.Add(b);                
            }

            // Now that we got the new bulbs, clear the list
            IBulb item;
            while (NewBulbList.TryTake(out item)) ;

            return bulbList;
        }

		public void DiscoverAsync()
		{
			Bulbs = new ConcurrentBag<IBulb>();
            NewBulbList = new ConcurrentBag<IBulb>();
            var packet = new DeviceGetServicePacket();            

            _udp.DiscoverBulbs(this, packet);
		}

        public void Udp_DeviceDiscovered(object sender, DiscoveryEventArgs e)
        {
            var action = new Action(() => Udp_DeviceDiscoveredAsync(sender, e));
            var task = Task.Factory.StartNew(action);
        }

        private void Udp_DeviceDiscoveredAsync(object sender, DiscoveryEventArgs e)
        {
            try
            {
                var ctx = e.DiscoveryContext;

                // Check if we already found the bulb
                IBulb ibulb = Bulbs.FirstOrDefault(x => x.IPEndPoint.ToString() == ctx.Sender.ToString());

                if (ibulb != null)
                {
                    // if the buld is already found, then make sure it's Offline flag is FALSE
                    if (ibulb.isOffline)
                    {
                        ibulb.isOffline = false;
                        Console.WriteLine("Device is back online: {0}", ctx.Sender.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Received response from: {0}", ctx.Sender.ToString());

                    var bulb = new Bulb()
                    {
                        IPEndPoint = ctx.Sender,
                        Service = ctx.Response.Service,
                        Port = ctx.Response.Port,
                        TargetMacAddress = ctx.Response.TargetMacAddress,
                        LastSeen = DateTime.UtcNow
                    };

                    bulb.isOffline = false;

                    var bulbSvc = new BulbService();
                    bool gotLightData = bulbSvc.Initialize(bulb);


                    // If we had an issue getting a response from the bulb, then don't add it to the list so
                    // we can try to re-discover it again.
                    if (gotLightData)
                    {
                        Bulbs.Add(bulb);
                        NewBulbList.Add(bulb); // list gets emptied when ThingServer requests new found bulbs

                        Debug.WriteLine(Bulbs.Count);
                    }
                    else
                    {
                        // Since the light data was not properly read, we need to remove the IP from the 
                        // Discovered bulb list so we can try discovering it again and hopefully read the 
                        // light data
                        string bulbAddress = bulb.IPEndPoint.Address.ToString();
                        //_udp.RemoveFromDiscoveredBulbList(bulbAddress);
                        Debug.WriteLine("-- Trouble getting response from bulb: " + bulbAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: {0} {1}", "Udp_DeviceDiscoveredAsync", ex.Message);
            }
        }



		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DiscoveryService() {
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
