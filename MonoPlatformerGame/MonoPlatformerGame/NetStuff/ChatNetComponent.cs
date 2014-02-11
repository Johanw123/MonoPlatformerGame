using System;
using Lidgren.Network;

namespace MonoPlatformerGame
{
	public class ChatNetComponent : NetComponent
	{
		public ChatNetComponent()
		{
				 
		}

		public override bool IncomingData(DataType type, NetIncomingMessage msg)
		{
			switch (type)
			{
				case DataType.ChatMessage:
					IncomingChatMessage(msg);
				return true;
			}
			return false;
		}

		void IncomingChatMessage(NetIncomingMessage msg)
		{
			JapeLog.WriteLine(msg.ReadString());
		}


	}
}