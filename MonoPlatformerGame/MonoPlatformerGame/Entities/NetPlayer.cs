using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class NetPlayer : Player
    {
        public NetPlayer(float x, float y) :
            base(x, y)
        {

        }

        public int UID { get; set; }

        public override void Update(float deltaTime)
        {
            
        }

        public override string GetName()
        {
            return "NetPlayer";
        }

    }
}
