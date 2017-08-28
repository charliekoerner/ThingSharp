using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxMvc.Domain
{
	public class Bulb : IBulb
	{
		static int _nextBulbId = 0;
		Color _color;
		string _colorString;

        bool _isOffline;
        int _failed_response_count = 0;

		public int BulbId { get; private set; }
		public string ColorString
		{
			get { return _colorString; }
		}
		public Color Color
		{
			get { return _color; }
			set { this.SetColor(value); }
		}


		public string Group { get; set; }

		/// <summary>
		/// Hue
		/// </summary>
		public UInt16 Hue { get; set; }
		/// <summary>
		/// Saturation (0=desaturated, 65535 = fully saturated)
		/// </summary>
		public UInt16 Saturation { get; set; }
		/// <summary>
		/// Brightness (0=off, 65535=full brightness)
		/// </summary>
		public UInt16 Brightness { get; set; }
		/// <summary>
		/// Bulb color temperature
		/// </summary>
		public UInt16 Kelvin { get; set; }

		public IHSBK HSBK
		{
			get
			{
				IHSBK result = null;
				if (this.Product.IsColor())
					result = new HSBK(this.Hue, this.Saturation, this.Brightness);
				else
					result = new HSBK(this.Kelvin, this.Brightness);
				return result;
			}
		}

		/// <summary>
		/// Power state
		/// </summary>
		public bool IsOn { get; set; }
		/// <summary>
		/// Light label
		/// </summary>
		public string Label { get; set; }
		public string Location { get; set; }

		public Single Signal { get; set; }
		public UInt32 TxCount { get; set; }
		public UInt32 RxCount { get; set; }
		public Single WifiInfoSignal { get; set; }
		public UInt32 WifiInfoTxCount { get; set; }
		public UInt32 WifiInfoRxCount { get; set; }
		public DateTime Time { get; set; }
		public DateTime Uptime { get; set; }
		public DateTime Downtime { get; set; }
		/// <summary>
		/// Firmware build time
		/// </summary>
		public DateTime Build { get; set; }
		/// <summary>
		/// Firmware version
		/// </summary>
		public UInt32 WifiFirmwareVersion { get; set; }

		/// <summary>
		/// Firmware build time
		/// </summary>
		public DateTime WifiFirmwareBuild { get; set; }
		/// <summary>
		/// Firmware version
		/// </summary>
		public UInt32 HostFirmwareVersion { get; set; }
		public DateTime HostFirmwareBuild { get; set; }


		public IPEndPoint IPEndPoint { get; set; }
		public byte[] TargetMacAddress { get; set; }

		public byte Service { get; set; }
		public uint Port { get; set; }
		public DateTime LastSeen {get;set;}

        public DateTime LastStateRequest { get; set; }
        public DateTime LastPowerRequest { get; set; }
        
        public bool isOffline 
        {
            get
            {
                return _isOffline;
            }
            set
            {
                bool offline_flag = value;
                if(offline_flag == true)
                {
                    _failed_response_count += 1;

                    // Note: This code currently has no effect because it will set the
                    //       bulb to offline after the first failure. If we want to increase
                    //       the tries before setting the bulb to offline, then set the value
                    //       below to something greater than zero.

                    if(_failed_response_count > 1)
                    {
                        _isOffline = true;
                        IsOn = false;
                    }
                }
                else
                {
                    _failed_response_count = 0;
                    _isOffline = false;
                }

            }
        }
        

		public uint Vendor { get; set; }
		public LifxProductEnum Product { get; set; }
		public uint Version { get; set; }

		public bool IsColor
		{
			get
			{
				return this.Product.IsColor();
			}
		}

		public bool IsKelvin
		{
			get
			{
				return !this.IsColor;
			}
		}

		public Bulb()
		{
			TargetMacAddress = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

			BulbId = ++_nextBulbId;
			Color = Color.Black;
			Group = null;
			Hue = UInt16.MaxValue;
			Saturation = UInt16.MaxValue;
			Brightness = UInt16.MaxValue;
			Kelvin = UInt16.MaxValue;
			IsOn = false;
			Label = null;
			Location = null;
			Signal = Single.MaxValue;
			TxCount = UInt32.MaxValue;
			RxCount = UInt32.MaxValue;
			WifiInfoSignal = Single.MaxValue;
			WifiInfoTxCount = UInt32.MaxValue;
			WifiInfoRxCount = UInt32.MaxValue;
			Time = DateTime.MaxValue;
			Uptime = DateTime.MaxValue;
			Downtime = DateTime.MaxValue;
			Build = DateTime.MaxValue;
			WifiFirmwareVersion = UInt32.MaxValue;
			WifiFirmwareBuild = DateTime.MaxValue;
			HostFirmwareVersion = UInt32.MaxValue;
			HostFirmwareBuild = DateTime.MaxValue;
			IPEndPoint = null;
			Service = byte.MaxValue;
			LastSeen = DateTime.MaxValue;
			Vendor = UInt32.MaxValue;
			Product = LifxProductEnum.Unknown;
			Version = UInt32.MaxValue;

            LastStateRequest = DateTime.MaxValue;
            LastPowerRequest = DateTime.MaxValue;

            isOffline = false;

		}        

		private void SetColor(Color value)
		{
			this._color = value;
			const string RGB_FORMAT = "rgb({0},{1},{2})";
			this._colorString = string.Format(RGB_FORMAT, value.R, value.G, value.B);
		}

		public void SetHSBK(IHSBK hsbk)
		{
			if (this.Product.IsColor())
			{
				this.Hue = hsbk.Hue;
				this.Saturation = hsbk.Saturation;
				this.Brightness = hsbk.Brightness;
			}
			else
			{
				this.Kelvin = hsbk.Kelvin;
			}

            var color = hsbk.ToColor(this.IsKelvin);
			this.SetColor(color);
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString() + ": ");
			if (!string.IsNullOrEmpty(this.Label))
				sb.AppendFormat("{0}, ", this.Label);

			sb.AppendFormat("{0} ", IPEndPoint.ToString());
			return sb.ToString();
		}
	}
}
