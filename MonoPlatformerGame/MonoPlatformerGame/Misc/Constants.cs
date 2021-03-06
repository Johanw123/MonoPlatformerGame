﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    class Constants
    {
        // Constants for controling horizontal movement
        public static float MOVE_ACC = 2000.0f;
        public static float MAX_MOVE_SPEED = 450.0f;
        public static float GROUND_FRICTION = 0.45f;
        public static float AIR_FRICTION = 0.98f;

        // Constants for controlling vertical movement
        public static float MAX_JUMP_TIME = 0.55f;
        public static float JUMP_LAUNCH_VELOCITY = -3500.0f;
        public static float GRAVITY_ACC = 3400.0f;
        public static float MAX_FALL_SPEED = 850.0f; //350 for slowfall mode?
        public static float JUMP_CONTROL_POWER = 0.14f;

       


    }
}
