using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlatformerGame
{
    public class ModManager
    {
        public static int DoubleJump = 0;
        public static int Gravity = 3000;
        public static bool JumpMania = false;
        public static bool CantStop = false;

        public static void SetupMods(Level level)
        {
            foreach (var item in level.Map.Properties)
            {
                switch (item.Key)
                {
                    case "DoubleJump":
                        DoubleJump = int.Parse(item.Value);
                        break;
                    case "Gravity":
                        Gravity = int.Parse(item.Value);
                        break;
                    case "JumpMania":
                        JumpMania = bool.Parse(item.Value);
                        break;
                    case "CantStop":
                        CantStop = bool.Parse(item.Value);
                        break;
                }
            }
        }
    }
}
