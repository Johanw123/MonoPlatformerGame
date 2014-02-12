using System;
using Lidgren.Network;

namespace MonoPlatformerGame
{
	public class ChatNetComponent : NetComponent
	{
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
			string playerName = msg.ReadString();
			string message = msg.ReadString();

			JapeLog.WriteLine(playerName + " : " + message);
		}


	}
}