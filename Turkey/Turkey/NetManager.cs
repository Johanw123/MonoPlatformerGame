using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Turkey
{
	class NetManager
	{
		enum ClientToServerMessageType
		{
			AppStart = 0,
			AppStop,
			AppInput,
			AppUpdate,
		};

		enum ServerToClientMessageType
		{
			ShuttingDown = 0,
		};

		NetClient m_client;

		public NetManager()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("Jape.Moose");
			m_client = new NetClient(config);
			m_client.Start();
		}

		public void Connect(string host, int port)
		{
			m_client.Connect(host, port);
		}

		public void SendInput(string message)
		{
			NetOutgoingMessage om = m_client.CreateMessage();
			om.Write((int)ClientToServerMessageType.AppInput);
			om.Write(message);
			m_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
		}
	}
}
