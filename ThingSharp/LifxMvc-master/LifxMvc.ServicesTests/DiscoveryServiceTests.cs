using Microsoft.VisualStudio.TestTools.UnitTesting;
using LifxMvc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxMvc.Domain;
using System.Threading;
using LifxMvc.Services.UdpHelper;
using LifxNet;
using System.Diagnostics;
using LifxNet.Domain;

namespace LifxMvc.Services.Tests
{
	[TestClass()]
	public class DiscoveryServiceTests
	{
		#region Fields

		const int EXPECTED_BULB_COUNT = 8;
		BulbService _svc;

		#endregion


		#region Properties

		static List<IBulb> Bulbs { get; set; }

		BulbService BulbService
		{
			get
			{
				if (null == _svc)
					_svc = new BulbService();
				return _svc;
			}
		}

		#endregion

		const int EXECUTE_N_TIMES = 256;


		#region Setup/ teardown

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			//GetBulbs();
		}

		static void GetBulbs()
		{
            //var svc = new DiscoveryService();

            //var sw = Stopwatch.StartNew();
            //svc.DiscoverAsync();
            //sw.Stop();
            //Debug.Write(sw.Elapsed);

            //Bulbs = new List<IBulb>(result);
            //Bulbs.Sort(new BulbComparer());

			//var bulbService = new BulbService();
			//foreach (var bulb in Bulbs)
			//{
			//	bulbService.LightGet(bulb);
			//}


		}

		[ClassCleanup()]
		public static void ClassCleanup()
		{
			UdpHelperManager.Instance.Dispose();
		}

		#endregion

		#region Utils

		class BulbComparer : IComparer<IBulb>
		{
			public int Compare(IBulb x, IBulb y)
			{
				return x.IPEndPoint.ToString().CompareTo(y.IPEndPoint.ToString());
			}
		}

        bool LightSetPower(IBulb bulb, bool power)
		{
			return BulbService.LightSetPower(bulb, power);
		}

        bool? LightGetPower(IBulb bulb)
		{
			var result = BulbService.LightGetPower(bulb);
			return result;
		}

		void DeviceSetPower(IBulb bulb, bool power)
		{
			BulbService.DeviceSetPower(bulb, power);
		}

		bool DeviceGetPower(IBulb bulb)
		{
			var result = BulbService.DeviceGetPower(bulb);
			return result;
		}

		void TurnOn(IBulb bulb)
		{
			if (!bulb.IsOn)
				this.DeviceSetPower(bulb, true);
		}

		void TurnOff(IBulb bulb)
		{
			if (!bulb.IsOn)
				this.DeviceSetPower(bulb, false);
		}


		#endregion

		[TestMethod()]
		public void DiscoveryTest()
		{
			GetBulbs();
			Assert.AreEqual(EXPECTED_BULB_COUNT, Bulbs.Count);
		}

		[TestMethod()]
		public void LightGetTest()
		{
			var expectedCount = Bulbs.Count;
			var count = Bulbs.Where(x => null == x.Label).Count();
			//Assert.AreEqual(expectedCount, count);

			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
			foreach (var bulb in Bulbs)
			{
				BulbService.LightGet(bulb);
			}

			count = Bulbs.Where(x => null != x.Label).Count();
			Assert.AreEqual(expectedCount, count);
		}


		[Ignore]
		[TestMethod()]
		public void LightSetColorTest()
		{
			Bulbs.ForEach(x => this.TurnOn(x));
			for (int x = 0; x < 10; ++x)
			{
				for (int i = 0; i < UInt16.MaxValue / 64; ++i)
				{
					foreach (var bulb in Bulbs)
					{
						if (bulb.Label == "1 o'clock")
							new object();

						var color = bulb.Color;
						//hsbk.RotateHue();
						BulbService.LightSetColor(bulb, color);
					}
				}
			}

		}


		[TestMethod()]
		public void DeviceGetVersionTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.DeviceGetVersion(bulb);

				Assert.IsNotNull(bulb.Vendor);
				Assert.IsNotNull(bulb.Product);
				Assert.IsNotNull(bulb.Version);
			}
		}

		[TestMethod()]
		public void GetHostInfoTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.GetHostInfo(bulb);

				Assert.AreNotEqual(Single.MaxValue, bulb.Signal);
				Assert.AreNotEqual(UInt32.MaxValue, bulb.TxCount);
				Assert.AreNotEqual(UInt32.MaxValue, bulb.RxCount);
			}
		}

		[TestMethod()]
		public void GetHostFirmwareTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.GetHostFirmware(bulb);

				Assert.AreNotEqual(DateTime.MaxValue, bulb.HostFirmwareBuild);
				Assert.AreNotEqual(UInt32.MaxValue, bulb.HostFirmwareVersion);
			}
		}

		[TestMethod()]
		public void GetWifiInfoTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.GetWifiInfo(bulb);

				Assert.AreNotEqual(Single.MaxValue, bulb.WifiInfoSignal);
				Assert.AreNotEqual(UInt32.MaxValue, bulb.WifiInfoTxCount);
				Assert.AreNotEqual(UInt32.MaxValue, bulb.WifiInfoRxCount);
			}
		}

		[TestMethod()]
		public void GetWifiFirmwareTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.GetWifiFirmware(bulb);

				Assert.AreNotEqual(DateTime.MaxValue, bulb.WifiFirmwareBuild);
				Assert.AreNotEqual(UInt32.MaxValue, bulb.WifiFirmwareVersion);
			}
		}

		[TestMethod()]
		public void GetLabelTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.GetLabel(bulb);

				Assert.IsNotNull(bulb.Label);
			}
		}

		[TestMethod()]
		public void SetLabelTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				var oldLabel = (bulb.Label ?? string.Empty).Trim();
				var testLabel = "THIS IS A TEST";

				BulbService.SetLabel(bulb, testLabel);
				BulbService.GetLabel(bulb);
				Assert.AreEqual(testLabel, bulb.Label);

				BulbService.SetLabel(bulb, oldLabel);
				BulbService.GetLabel(bulb);
				Assert.AreEqual(oldLabel, bulb.Label);
			}
		}

		[TestMethod()]
		public void GetInfoTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.GetInfo(bulb);

				Assert.AreNotEqual(DateTime.MaxValue, bulb.Time);
				Assert.AreNotEqual(DateTime.MaxValue, bulb.Uptime);
				Assert.AreNotEqual(DateTime.MaxValue, bulb.Downtime);
			}
		}

		[TestMethod()]
		public void EchoRequestTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.EchoRequest(bulb);
			}
		}

		[Ignore]
		[TestMethod()]
		public void DeviceGetPowerTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				var result = BulbService.DeviceGetPower(bulb);
				Assert.AreEqual(result, bulb.IsOn);
			}
		}

		[Ignore]
		[TestMethod()]
		public void DeviceSetPowerTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				var requested = !bulb.IsOn;
				this.DeviceSetPower(bulb, requested);
				var expected = this.DeviceGetPower(bulb);
				//Assert.AreEqual(requested, expected);
				//this.DeviceSetPower(bulb, !requested); // toggle back to initial state.
			}
		}

		[TestMethod()]
		public void LightGetPowerTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				var power = BulbService.LightGetPower(bulb);
				Assert.AreEqual(power, bulb.IsOn);
			}
		}

		[TestMethod()]
		public void LightSetPowerTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				var requested = !bulb.IsOn;
				this.LightSetPower(bulb, requested);

				BulbService.LightGet(bulb);

				Assert.AreEqual(requested, bulb.IsOn);
				this.LightSetPower(bulb, !requested); // toggle back to initial state.
			}

		}

		[TestMethod()]
		public void GetGroupTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.DeviceGetGroup(bulb);
			}
		}

		[TestMethod()]
		public void GetLocationTest()
		{
			for (int n = 0; n < EXECUTE_N_TIMES; ++n)
				foreach (var bulb in Bulbs)
			{
				BulbService.DeviceGetLocation(bulb);
			}
		}

		LightSetWaveformCreationContext CreateLightSetWaveformCreationContext(IBulb bulb)
		{
			var hsbk = bulb.HSBK;
			//hsbk.RotateHue(180);
			var result = new LightSetWaveformCreationContext(true, hsbk, 250, 100, 0, WaveformEnum.HalfSine);
			return result;
		}

	}//class
}//ns