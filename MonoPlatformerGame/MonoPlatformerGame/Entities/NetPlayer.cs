using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class NetPlayer : Player
    {
		public int UID { get; set; }
		public bool Disconnected { get; set; }

        public NetPlayer(float x, float y) :
            base(x, y)
        {

        }

        public override void Update(float deltaTime)
        {
            
        }

        public override string GetName()
        {
            return "NetPlayer";
        }

    }
}
