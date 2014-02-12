using Lidgren.Network;
using MonoPlatformerGame;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DedicatedServerConsole
{
    class DedicatedServerNetComponent : NetComponent
    {
        public override bool IncomingData(DataType type, NetIncomingMessage msg)
        {
            switch (type)
            {
	            case DataType.GameState:
	                IncomingGameState(msg);
	                return true;
	            case DataType.NewPlayer:
	                NewPlayer(msg);
	                return true;
	            case DataType.NewPlayerResponse:
	                NewPlayerResponse(msg);
	                return true;
	            case DataType.BroadcastMessage:
	                RedirectBroadcast(type, msg);
	                return true;
	            case DataType.PlayerFinish:
	                PlayerFinish(msg);
	                return true;
				case DataType.ChatMessage:
					RedirectChatMessage(msg);
	                return true;
				case DataType.DownloadMapRequest:
					DownloadRequest(msg);
					return true;
            }
            return false;
        }

		void RedirectChatMessage(NetIncomingMessage msg)
		{
			/*string playerName = msg.ReadString();
			string message = msg.ReadString();

			NetManager.SendMessageParams(NetDeliveryMethod.ReliableOrdered,
			                             (int)DataType.ChatMessage,
			                             playerName,
			                             message
			                             );*/

			NetManager.RedirectMessage(msg);


		}

		protected void DownloadRequest(NetIncomingMessage msg)
		{
			string mapName = msg.ReadString();
			string path = "Maps/" + mapName;

			if (File.Exists (path))
			{
				string mapData = File.ReadAllText (path);
				NetManager.SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
				                             (int)DataType.DownloadMapResponse,
				                             mapName,
				                             mapData
				                             );
			}


		}

        protected void PlayerFinish(NetIncomingMessage msg)
        {
            int who = msg.ReadInt32();
            int time = msg.ReadInt32();

            NetManager.PlayerReachedFinish(who, time);
        }

        protected void RedirectBroadcast(DataType type, NetIncomingMessage msg)
        {
            NetManager.RedirectMessage(msg);
            IncomingData((DataType)msg.ReadInt32(), msg);
        }

        protected void NewPlayerResponse(NetIncomingMessage msg)
        {
            int ownUid = msg.ReadInt32();
            NetManager.RemoteUID = ownUid;

            int otherClientsCount = msg.ReadInt32();

            for (int i = 0; i < otherClientsCount; i++)
            {

                string name = msg.ReadString();
                int uid = msg.ReadInt32();

                ClientInfo clientInfo = new ClientInfo();
                clientInfo.Name = name;
                clientInfo.X = 0;
                clientInfo.Y = 0;
                clientInfo.UID = uid;
                if (!NetManager.connectedClients.ContainsKey(uid))
                {
                    NetManager.connectedClients.Add(clientInfo.UID, clientInfo);
                    JapeLog.WriteLine("New Player Added: " + name);
                }
            }

            JapeLog.WriteLine("Remote ID recieved: " + ownUid);
        }

        public void Update()
        {
            SendGameState();
        }

        protected void NewPlayer(NetIncomingMessage msg)
        {
            string name = msg.ReadString();
            msg.ReadInt32();
            int uid = NetManager.CreateNewUID();

            ClientInfo clientInfo = new ClientInfo();
            clientInfo.Name = name;
            clientInfo.X = 0;
            clientInfo.Y = 0;
			clientInfo.ClientNetConnection = msg.SenderConnection;
            clientInfo.UID = uid;
            NetManager.connectedClients.Add(clientInfo.UID, clientInfo);

            NetManager.AlertOthersNewPlayer(msg.SenderConnection, clientInfo);

            JapeLog.WriteLine("New Player Added: " + name);
        }

        protected void SendGameState()
        {
            foreach (var clients in NetManager.connectedClients)
            {
                NetManager.SendMessageParams(NetDeliveryMethod.UnreliableSequenced,
                    (int)DataType.GameState,
                    clients.Value.UID,
                    clients.Value.X,
                    clients.Value.Y
                    );
            }
        }
        protected void IncomingGameState(NetIncomingMessage msg)
        {
            int who = msg.ReadInt32();
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();

            if (who == 0)
                return;

            NetManager.connectedClients[who].X = x;
            NetManager.connectedClients[who].Y = y;
        }

    }
}
