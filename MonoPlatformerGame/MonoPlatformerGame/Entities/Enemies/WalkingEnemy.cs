﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    class WalkingEnemy : Enemy
    {
        Vector2 startPos;
        Vector2 endPos;

        public WalkingEnemy(float x, float y, float Width)
            : base(x, y)
        {
            mTexture = ResourceManager.GetTexture("Player");
            startPos = new Vector2(x, y);
            endPos = new Vector2(x + Width - mTexture.Width, y);
            mAccel.X = 1;

            Facing = eFacing.Right;
            UpdateBoundingBox();
        }

        public override void Update(float deltaTime)
        {
            if (Accel.X == 1 && Position.X >= endPos.X)
            {
                mAccel.X = -1;
            }
            else if (Accel.X == -1 && Position.X <= startPos.X)
            {
                mAccel.X = 1;
            }
        }

    }
}
