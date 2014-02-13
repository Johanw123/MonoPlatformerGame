using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace MonoPlatformerGame
{
    public enum DataType
    {
        StartGame = 0,
        Hail,
        Login,
        NewPlayer,
        NewPlayerResponse,
		PlayerDisconnected,

        //Gameplay
        GameState,
        BroadcastMessage,
        ChatMessage,
        PlayerFinish,
        ChangeLevel,

        PeerInfo = 5000,
        
        Ping = 6000,
        Pong,

        RegisterLobby = 7000,
        UpdateLobby,
        UnregisterLobby,
        GetLobbyList,
        DirectConnect,
        JoinLobby,
    }

    public static class NetManager
    {
        public static bool IsNetPlay{ get; private set; }
        public static int RemoteUID{ get; set; }
        public static bool GameStarted{ get; set; }
        public static bool IsHost { get; set; }
        public static bool IsDedicatedHost { get; set; }
        public static int CreateNewUID() { return ++uID; }
        public static string CurrentLevelName { get; set; }
        
        private static int uID = 0;
        private static NetPeer netPeer;
        private static List<NetComponent> components = new List<NetComponent>();
        public static Dictionary<int, ClientInfo> connectedClients = new Dictionary<int, ClientInfo>();
       
        private static Thread t = new Thread(DoThreadInit);

        

        private static void DoInit()
        {

            if (IsHost)
            {
                NetPeerConfiguration config = new NetPeerConfiguration("MonoPlatformerGame");
                config.Port = DataStorage.GetLocalPlayerInfo().ServerPort;
                config.MaximumConnections = 32;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                netPeer = new NetServer(config);
                netPeer.Start();
				JapeLog.WriteLine("Server Started");
            }
            else
            {
                NetPeerConfiguration config = new NetPeerConfiguration("MonoPlatformerGame");
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                netPeer = new NetClient(config);
                
                NetOutgoingMessage hailMessage = CreateMessage();
                hailMessage.Write((int)DataType.Login);
                hailMessage.Write("Hello there");
                netPeer.Start();
                netPeer.Connect(DataStorage.GetLocalPlayerInfo().ServerIP, DataStorage.GetLocalPlayerInfo().ServerPort, hailMessage);
            }
            //JapeLog.WriteLine("Connected to Server");
        }

        public static ClientInfo GetClient(int id)
        {
            ClientInfo clientInfo;

            connectedClients.TryGetValue(id, out clientInfo);

            return clientInfo;
        }

		public static ClientInfo GetClient(string name)
		{
			foreach (var item in connectedClients)
			{
				if (item.Value.Name == name)
				{
					return item.Value;
				}
			}
			return null;
		}

		public static ClientInfo GetClient(NetConnection connection)
		{
			foreach (var item in connectedClients)
			{
				if (item.Value.ClientNetConnection == connection)
				{
					return item.Value;
				}
			}
			return null;
		}

		public static void KickPlayer(string name)
		{
			ClientInfo client = GetClient (name);
			KickClient(client);
		}

		public static void KickPlayer(int id)
		{
			ClientInfo client = GetClient (id);
			KickClient(client);
		}

		private static void KickClient(ClientInfo client)
		{
			client.ClientNetConnection.Disconnect("You were kicked by the host");

            string reason = "Kicked in the ass";

			SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
			                  (int)DataType.PlayerDisconnected,
			                  client.Name,
                              reason
			                  );

			connectedClients.Remove(client);
		}

		public static void SendMessageParamsStringsOnly(NetDeliveryMethod method, int type, NetConnection reciever, params string[] stringParameters)
		{
			if(stringParameters.Length <= 0)
				return;

			NetOutgoingMessage oMsg = CreateMessage();

			oMsg.Write(type);

			foreach (string item in stringParameters)
			{
				oMsg.Write(item);
			}

			SendMessage(method, oMsg, reciever);
		}

		public static void SendMessageParamsStringsOnly(NetDeliveryMethod method, int type, params string[] stringParameters)
		{
			SendMessageParamsStringsOnly(method, type, null, stringParameters);
		}

        public static void SendMessageParams(NetDeliveryMethod method, params object[] parameters)
        {
            SendMessageParams(method, null, parameters);
        }

        public static void SendMessageParams(NetDeliveryMethod method, NetConnection reciever, params object[] parameters)
        {
            if (parameters.Length <= 0)
                return;

            NetOutgoingMessage oMsg = CreateMessage();

            foreach (var item in parameters)
            {
                try
                {
                    oMsg.WriteAllFields(item);
                }
                catch (Exception)
                {
                    oMsg.Write((string)item);
                }
                
            }

            SendMessage(method, oMsg, reciever);
        }

        public static void StartGame()
        {
            if(IsHost)
            {

                //SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableSequenced, 
                //                  (int)DataType.StartGame,
                //                  levelName
                //                  );
                
                JapeLog.WriteLine(String.Format("Starting the game with {0} number of players", (IsDedicatedHost) ? connectedClients.Count : connectedClients.Count + 1));

                foreach (var item in components)
                {
                    item.StartGame();
                }
            }
            GameStarted = true;
        }

        public static void Init(bool isHosting)
        {
            IsHost = isHosting;

            bool threadInit = true;

            if (threadInit)
                t.Start(); 
            else
                DoInit();
        }

        private static void DoThreadInit()
        {
            DoInit();
            
            t.Abort();
        }

        public static NetOutgoingMessage CreateMessage()
        {
            return netPeer.CreateMessage();;
        }

        private static void IncomingData(NetIncomingMessage msg)
        {
            DataType type = (DataType)msg.ReadInt32();

            foreach (NetComponent component in components)
            {
                if (component.IncomingData(type, msg))
                {
                    return;
                }
            }
        }

        public static void AddComponent(NetComponent component)
        {
            components.Add(component);
        }

        public static void RemoveComponent(NetComponent component)
        {
            components.Remove(component);
        }

        public static void SendMessage(NetDeliveryMethod method, NetOutgoingMessage msg, NetConnection reciever = null)
        {
            if (IsHost)
            {
                if (reciever != null)
                {
                    netPeer.SendMessage(msg, reciever, method);
                    return;
                }

                netPeer.SendMessage(msg, netPeer.Connections, method, 0);
            }
            else
            {
                ((NetClient)netPeer).SendMessage(msg, method);
            }
        }

        public static void RedirectMessage(NetIncomingMessage msg)
        {
            NetConnection excludeConnection = msg.SenderConnection;
            if (IsHost)
            {
                foreach (NetConnection conn in netPeer.Connections)
                {
                    if (conn != excludeConnection)
                    {
                        NetOutgoingMessage oMsg = CreateMessage();
                        oMsg.Write(msg);
                        SendMessage(NetDeliveryMethod.ReliableOrdered, oMsg, conn);
                    }
                }
            }
        }

        public static void SendBroadcast(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            if (IsHost)
            {
                SendMessage(method, msg);
            }
            else
            {
                SendMessageParams(method, (int)DataType.BroadcastMessage, msg);
            }
        }

        public static void PlayerReachedFinish(int id, int timeMS)
        {
            PlayerReachedFinish(id, new TimeSpan(0, 0, 0, 0, timeMS));
        }
        public static void PlayerReachedFinish(int id, TimeSpan time)
        {
            TimeSpan prevTime = TimeSpan.MaxValue;
            TimeList.AddTime(id, time);

            //if (elapsedTimes.TryGetValue(id, out prevTime))
            //{
            //    if (time < prevTime)
            //    {
            //        elapsedTimes[id] = time;
            //    }
            //}
            //else
            //{
            //    elapsedTimes.Add(id, time);
            //}



            //foreach (var item in elapsedTimes.Values)
            //{
            //    JapeLog.WriteLine(item);
            //}
        }

        public static void Listen()
        {
            Listen(netPeer);
        }

        private static void Listen(NetPeer peer)
        {
            //TODO FIXME!

            if (peer == null)
                return;

            NetIncomingMessage msg;
           
            while ((msg = peer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        //JapeLog.WriteLine("Message received: Data");
                        IncomingData(msg);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:

                        // Read the first byte of the packet
                        // ( Enums can be casted to bytes, so it be used to make bytes human readable )

                        int type = msg.ReadInt32();
                        if (type == (int)DataType.Login)
                        {
                            JapeLog.WriteLine("Incoming LOGIN");

                            msg.SenderConnection.Approve();
                            string hailMessage = msg.ReadString();

                            JapeLog.WriteLine("Approved new connection: " + hailMessage);
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (msg.SenderConnection.Status == NetConnectionStatus.Connected)
                        {
                            if (!IsDedicatedHost)
                            {
                                SendMessageParams(NetDeliveryMethod.ReliableOrdered,
                                    (int)DataType.NewPlayer,
                                    DataStorage.GetLocalPlayerInfo().UserName,
                                    (IsHost) ? 0 : -1,
                                    RemoteUID
                                    );

                                JapeLog.WriteLine("SEND NEWPLAYER");
                            }
                            else
                                JapeLog.WriteLine("This server is Dedicated (Not sending a new player message)");
                        }
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //JapeLog.WriteLine(msg.ReadString());
                        break;
                }
                peer.Recycle(msg);
            }
        }
    }
}
