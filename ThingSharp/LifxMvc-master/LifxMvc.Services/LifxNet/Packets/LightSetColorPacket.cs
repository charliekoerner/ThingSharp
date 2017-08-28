using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightSetHSBKPacket : LifxPacketBase<LightStateResponse>
	{
		override public PacketType MessageType { get { return PacketType.LightSetColor; } }

		public byte Reserved { get; private set; }
		public UInt16 Hue { get; private set; }
		public UInt16 Saturation { get; private set; }
		public UInt16 Brightness { get; private set; }
		public UInt16 Kelvin { get; private set; }

		public UInt32 Duration { get; set; }

		public LightSetHSBKPacket(IBulb bulb)
			: base(bulb)
		{
			//if (null == hsbk)
			//	throw new ArgumentNullException();

			this.Hue = bulb.Hue;
			this.Saturation = bulb.Saturation;
            this.Brightness = bulb.Brightness;
            this.Kelvin = bulb.Kelvin;

			this.Header.ResponseRequired = true;
		}
        public LightSetHSBKPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			Reserved = payload[0];
			Hue = BitConverter.ToUInt16(payload, 1);
			Saturation = BitConverter.ToUInt16(payload, 3);
			Brightness = BitConverter.ToUInt16(payload, 5);
			Kelvin = BitConverter.ToUInt16(payload, 7);
			Duration = BitConverter.ToUInt32(payload, 9);

            this.Header.AcknowledgeRequired = true;
		}


		override protected object[] GetPayloadParams()
		{
			return new object[] { this.Reserved, this.Hue, this.Saturation, this.Brightness, this.Kelvin, this.Duration };
		}
	}
}
