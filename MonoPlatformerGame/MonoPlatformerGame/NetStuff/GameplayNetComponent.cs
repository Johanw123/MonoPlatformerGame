using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public delegate void ChangeLevelEventHandler(string levelName);
public delegate void ChangeLevelPrepEventHandler(double delayTime);

namespace MonoPlatformerGame
{
    public abstract class GameplayNetComponent : NetComponent
    {
        //skicka en timed changeLevelDirekt ist.
        //Kan slås ihop dessa två event förmodligen.
        public event ChangeLevelEventHandler ChangeLevelEvent;
        public event ChangeLevelPrepEventHandler ChangeLevelPrepEvent;

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
                    IncomingChangeLevel(msg);
                    return true;
                case DataType.PrepareLevelChange:
                    IncomingPrepareLevelChange(msg);
                    return true;
                case DataType.PlayerDisconnected:
                    IncomingPlayerDisconnect(msg);
                    return true;
                case DataType.Ping:
                    IncomingPing(msg);
                    return true;
            }
            return false;
        }

        private void IncomingPing(NetIncomingMessage msg)
        {
            NetManager.SendMessageParams(NetDeliveryMethod.ReliableOrdered,
                                        (int)DataType.Pong
                                        );
        }

        private void IncomingPrepareLevelChange(NetIncomingMessage msg)
        {
            double delayTime = msg.ReadDouble();
            double recieveTime = msg.ReceiveTime / 1000;
            double actualDelayTime = delayTime - recieveTime;

            if (ChangeLevelPrepEvent != null)
                ChangeLevelPrepEvent(actualDelayTime);
        }

        private void IncomingPlayerDisconnect(NetIncomingMessage msg)
        {
            int uID = int.Parse(msg.ReadString());
            string playerName = msg.ReadString();
            string reason = msg.ReadString();

			//int id = msg.ReadInt32();
			//TODO send id
            EntityManager.RemoveNetPlayer(uID);

            JapeLog.WriteLine(playerName + "Disconnected" + " - Reason: " + reason);
        }

        protected void PlayerFinish(NetIncomingMessage msg)
        {
            int who = msg.ReadInt32();
            int time = msg.ReadInt32();

            NetManager.PlayerReachedFinish(who, time);                
        }

        protected void IncomingChangeLevel(NetIncomingMessage msg)
        {
            string levelName = msg.ReadString();
            string levelData = msg.ReadString();

            DownloadLevel(levelName, levelData);
            OnChangedLevel(levelName);
        }

        protected void DownloadLevel(string name, string data)
        {
            StreamWriter writer = File.CreateText("Content/" + name);
            writer.Write(data);
            writer.Close();
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
            //string levelName = msg.ReadString();
            //ChangeOrDownloadLevel(levelName);

            string levelName = msg.ReadString();
            string levelData = msg.ReadString();

            DownloadLevel(levelName, levelData);
            OnChangedLevel(levelName);
		}

        protected void NewPlayerResponse(NetIncomingMessage msg)
        {
            int ownUid = msg.ReadInt32();
            NetManager.RemoteUID = ownUid;
            bool gameStarted = msg.ReadBoolean();
            string levelName = msg.ReadString();

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

            //if (gameStarted)
            //    ChangeOrDownloadLevel(levelName);

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
