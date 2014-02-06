using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace MonoPlatformerGame
{
    public class NetComponent
    {
        public virtual bool IncomingData(DataType type, NetIncomingMessage msg)
        {
            return false;
        }

        public virtual bool IncomingUnknownData(NetIncomingMessage msg)
        {
            return false;
        }

        public virtual void StartGame()
        {
        }
    }
}
