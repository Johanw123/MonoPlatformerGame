using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class ClientInfo
    {
        public int UID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
		public int TimeSinceLastPing { get; set; }
        //public NetPlayer NetPlayer
        //{
        //    get;
        //    set;
        //}
        public float X
        {
            get;
            set;
        }
        public float Y
        {
            get;
            set;
        }
		public NetConnection ClientNetConnection 
		{
			get;
			set;
		}
        public ClientInfo()
        {
            
        }

    }
}
