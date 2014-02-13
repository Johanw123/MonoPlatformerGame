using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class ClientGameplayNetComponent : GameplayNetComponent
    {

        protected override void NewPlayer(NetIncomingMessage msg)
        {
            NetPlayer player = new NetPlayer(0, 0);
            string name = msg.ReadString();
            int uid = msg.ReadInt32();
            player.UID = uid;

            ClientInfo clientInfo = new ClientInfo();
            clientInfo.Name = name;
            clientInfo.X = player.X;
            clientInfo.Y = player.Y;
            clientInfo.UID = uid;
            if (!NetManager.connectedClients.ContainsKey(uid))
            {
                NetManager.connectedClients.Add(clientInfo.UID, clientInfo);
                EntityManager.AddNetPlayer(player);
                JapeLog.WriteLine("New Player Added: " + name);
            }
        }

		float previousX;
		float previousY;
        protected override void SendGameState()
        {
			float currentX = EntityManager.GetPlayer().Position.X;
			float currentY = EntityManager.GetPlayer().Position.Y;

			if(previousX == currentX && previousY == currentY)
				return;

			NetManager.SendMessageParams(NetDeliveryMethod.UnreliableSequenced,
			                                     (int)DataType.GameState,
			                                     NetManager.RemoteUID,
			                                     currentX,
			                                     currentY
												 );
			

			previousX = currentX;
			previousY = currentY;
        }

        protected override void IncomingGameState(NetIncomingMessage msg)
        {
            int who = msg.ReadInt32();
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();

            if (NetManager.connectedClients.ContainsKey(who))
            {
                NetManager.connectedClients[who].X = x;
                NetManager.connectedClients[who].Y = y;
            }
        }
    }
}
