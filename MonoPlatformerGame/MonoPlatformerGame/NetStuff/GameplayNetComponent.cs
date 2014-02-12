using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public delegate void ChangeLevelEventHandler(string levelName);

namespace MonoPlatformerGame
{
    public abstract class GameplayNetComponent : NetComponent
    {
        public event ChangeLevelEventHandler ChangeLevelEvent;

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
                case DataType.StartGame:
                    IncomingStartGame(msg);
                    return true;
                case DataType.BroadcastMessage:
                    RedirectBroadcast(type, msg);
                    return true;
                case DataType.PlayerFinish:
                    PlayerFinish(msg);
                    return true;
                case DataType.ChangeLevel:
                    ChangeLevel(msg);
                    return true;
                case DataType.DownloadMapResponse:
                    IncomingDownloadMapResponse(msg);
                    return true;
            }
            return false;
        }

		protected void IncomingDownloadMapResponse(NetIncomingMessage msg)
		{
			string mapName = msg.ReadString();
			string mapData = msg.ReadString();

			StreamWriter writer = File.CreateText ("Content/" + mapName);
			writer.Write (mapData);
			writer.Close ();
		}

        protected void PlayerFinish(NetIncomingMessage msg)
        {
            int who = msg.ReadInt32();
            int time = msg.ReadInt32();

            NetManager.PlayerReachedFinish(who, time);                
        }

        protected void ChangeLevel(NetIncomingMessage msg)
        {
            string levelName = msg.ReadString();

            if (Level.MapExist(levelName))
                OnChangedLevel(levelName);
            else
            {
                //TODO download map from server
                NetManager.SendMessageParams(NetDeliveryMethod.ReliableOrdered,
                                             (int)DataType.DownloadMapRequest,
                                             levelName
                                             );

            }
            //OnChangedLevel(levelName);
        }

        protected virtual void OnChangedLevel(string levelName)
        {
            if (ChangeLevelEvent != null)
                ChangeLevelEvent(levelName);
        }

        protected void RedirectBroadcast(DataType type, NetIncomingMessage msg)
        {
            NetManager.RedirectMessage(msg);
            IncomingData((DataType)msg.ReadInt32(), msg);
        }

		protected void IncomingStartGame(NetIncomingMessage msg)
		{
            //string map = msg.ReadString();

            //if (Level.MapExist (map))
            //    ChangeLevel (msg);
            //else
            //{
            //    //TODO download map from server
            //    NetManager.SendMessageParams(NetDeliveryMethod.ReliableOrdered,
            //                                 (int)DataType.DownloadMapRequest,
            //                                 map
            //                                 );

            //}


			StartGame ();
		}

        public override void StartGame()
        {
            NetManager.GameStarted = true;
            EntityManager.ResetPlayer();
        }

        protected void NewPlayerResponse(NetIncomingMessage msg)
        {
            int ownUid = msg.ReadInt32();
            NetManager.RemoteUID = ownUid;
            bool gameStarted = msg.ReadBoolean();

            int otherClientsCount = msg.ReadInt32();

            for (int i = 0; i < otherClientsCount; i++)
            {
                string name = msg.ReadString();
                int uid = msg.ReadInt32();

                NetPlayer player = new NetPlayer(0, 0);
                ClientInfo clientInfo = new ClientInfo();
                clientInfo.Name = name;
                clientInfo.X = player.X;
                clientInfo.Y = player.Y;
                clientInfo.UID = uid;
                player.UID = uid;

                if (!NetManager.connectedClients.ContainsKey(uid))
                {
                    NetManager.connectedClients.Add(clientInfo.UID, clientInfo);
                    EntityManager.AddNetPlayer(player);
                    JapeLog.WriteLine("New Player Added: " + name);
                }
            }

            if (gameStarted)
                StartGame();

            JapeLog.WriteLine("Remote ID recieved: " + ownUid);
        }

        public void Update()
        {
            SendGameState();
        }

        protected abstract void IncomingGameState(NetIncomingMessage msg);
        protected abstract void NewPlayer(NetIncomingMessage msg);
        protected abstract void SendGameState();
    }
}
