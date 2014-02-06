using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    class Runtime
    {
        public static int LevelWidth { get; set; }
        public static int LevelHeight { get; set; }
        public static int TileSize { get; set; }

        private static Random random = new Random();

        public static Random GetRandom()
        {
            return random;
        }

       
        
        
    }
}
