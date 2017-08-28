using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetVersionPacket : LifxPacketBase<DeviceStateVersionResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetVersion; } }

		public DeviceGetVersionPacket(IBulb bulb)
			: base(bulb)
		{
			this.Header.ResponseRequired = true;
            this.Header.AcknowledgeRequired = false;
		}

		public DeviceGetVersionPacket(FrameHeader header)
			: base(header)
		{ }
	}//class
}
