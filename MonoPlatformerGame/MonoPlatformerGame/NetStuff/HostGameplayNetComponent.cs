using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;

namespace MonoPlatformerGame
{
    public class HostGameplayNetComponent : GameplayNetComponent
    {     
        protected override void NewPlayer(NetIncomingMessage msg)
        {
            NetPlayer player = new NetPlayer(0, 0);
            string name = msg.ReadString();
            msg.ReadInt32();
            int uid = NetManager.CreateNewUID();

            ClientInfo clientInfo = new ClientInfo();
            clientInfo.Name = name;
            clientInfo.X = player.X;
            clientInfo.Y = player.Y;
            clientInfo.UID = uid;
            player.UID = uid;
            NetManager.connectedClients.Add(clientInfo.UID, clientInfo);
            
            NetManager.AlertOthersNewPlayer(msg.SenderConnection, clientInfo);

            EntityManager.AddNetPlayer(player);
            JapeLog.WriteLine("New Player Added: " + name);
        }

        protected override void SendGameState()
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

            NetManager.SendMessageParams(NetDeliveryMethod.UnreliableSequenced,
                (int)DataType.GameState,
                0,
                EntityManager.GetPlayer().Position.X,
                EntityManager.GetPlayer().Position.Y
                );
        }
        protected override void IncomingGameState(NetIncomingMessage msg)
        {
            int who = msg.ReadInt32();
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();

            if (who == 0)
                return;


            NetManager.connectedClients[who].X = x;
            NetManager.connectedClients[who].Y = y;


            //NetPlayer netPlayer = NetManager.connectedClients[who].NetPlayer;

            //if (netPlayer != null)
            //{
            //    netPlayer.X = x;
            //    netPlayer.Y = y;
            //}
        }
    }
}
