using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public enum GameMode
    {
        //You have a cerain time to finish the course and the best time wins (track-mania style).
        //several tries, dying and reaching goal will put player back at start to try again
        TimeTrial,

        //First to the goal is the winner. Also if all dies, next map will be loaded
        //A fast pased game with high tempo
        Race,

        //Screen scrolls the same for all players. One life only, survivers that reach the goal are rewarded with a point.
        //Most point at end of certain ammount of levels will win.
        //(Coop/Versus?)
        Survival,
    }

    class Runtime
    {
        private static Random random = new Random();

        public static Random GetRandom()
        {
            return random;
        }

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        public static sCurrentLevel CurrentLevel;
        public struct sCurrentLevel
        {
            public int Width;
            public int Height;
            public int TileSize;

            public string Name;
            public string Data;

            public GameMode GameMode;

            public bool Loaded;
        }
    }
}
