using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moose
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

		private NetServer m_server;

		public NetManager()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("Jape.Moose");
			config.Port = 10900;
			config.LocalAddress = NetUtility.Resolve("localhost");
			m_server = new NetServer(config);
		}

		public void Listen()
		{
			NetIncomingMessage incoming;
			while ((incoming = m_server.ReadMessage()) != null)
			{
				Program.Print("Parsing incoming message", ConsoleColor.Magenta);
				switch (incoming.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						break;
					case NetIncomingMessageType.StatusChanged:
						ParseStatus(incoming);
						break;
					case NetIncomingMessageType.Data:
						ParseData(incoming);
						break;
					default:
						Program.Print(incoming.SenderConnection + ": Unknown incoming message type (" + incoming.MessageType + ")", ConsoleColor.Yellow);
						break;
				}
				m_server.Recycle(incoming);
			}
		}

		private void ParseData(NetIncomingMessage incoming)
		{
			Program.Print("Parsing incoming data", ConsoleColor.Magenta);
			ClientToServerMessageType type = (ClientToServerMessageType)incoming.ReadInt32();
			switch (type)
			{
				case ClientToServerMessageType.AppInput:
					Program.ProcessManager.SendInput(incoming.ReadString());
					break;
			}
		}

		private void ParseStatus(NetIncomingMessage incoming)
		{
			Program.Print("Parsing incoming status", ConsoleColor.Magenta);
			NetConnectionStatus status = (NetConnectionStatus)incoming.ReadByte();
			switch (status)
			{
				case NetConnectionStatus.Connected:
					Program.Print("Client connected: " + incoming.SenderConnection + " (" + incoming.SenderEndPoint.Address + ":" + incoming.SenderEndPoint.Port + ")");
					break;
				default:
					Program.Print(incoming.SenderConnection + ": " + status + " {" + incoming.ReadString() + "}");
					break;
			}
		}

		private void ReceiveFile()
		{

		}

		public void Exit()
		{
			foreach (NetConnection client in m_server.Connections)
			{
				NetOutgoingMessage om = client.Peer.CreateMessage();
				om.Write((int)ServerToClientMessageType.ShuttingDown);
				client.SendMessage(om, NetDeliveryMethod.ReliableOrdered, 1);
			}

			m_server.Shutdown("Disconnecting Moose server...");
		}

		//private static void FetchExternalIP()
		//{
		//	using (WebClient client = new WebClient())
		//	{
		//		string result = client.DownloadString("http://bot.whatismyipaddress.com");
		//		if (!IPAddress.TryParse(result, out s_externalIP))
		//		{
		//			Print("External IP address could not be fetched.", ConsoleColor.Red);
		//			Exit();
		//		}
		//	}
		//}
	}
}
