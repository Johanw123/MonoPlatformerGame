using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    class FlyingEnemy : Enemy
    {
        Vector2 startPos;
        Vector2 endPos;

        public FlyingEnemy(float x, float y, float Width, float Height)
            : base(x, y)
        {
            mTexture = ResourceManager.GetTexture("Player");
            startPos = new Vector2(x, y);
            endPos = new Vector2(x + Width - mTexture.Width, y + Height - mTexture.Height);
            mAccel.X = 1;
            mAccel.Y = 1;
            Facing = eFacing.Right;
            UpdateBoundingBox();
        }

        public override void Update(float deltaTime)
        {
            if (mAccel.X == 1 && mPosition.X >= endPos.X)
            {
                mAccel.X = -1;
            }
            else if (mAccel.X == -1 && mPosition.X <= startPos.X)
            {
                mAccel.X = 1;
            }

            if (mAccel.Y == 1 && mPosition.Y >= endPos.Y)
            {
                mAccel.Y = -1f;
                mVelocity.Y = 0;
            }
            else if (mAccel.Y == -1 && mPosition.Y <= startPos.Y)
            {
                mAccel.Y = 1f;
                mVelocity.Y = 0;
            }

            base.Update(deltaTime);
        }

        protected override void ApplyPhysics(float deltaTime)
        {
            Vector2 previousPosition = Position;

            mVelocity += mAccel * Constants.MOVE_ACC * 0.5f * deltaTime;

            mVelocity.X *= Constants.AIR_FRICTION;
            mVelocity.Y *= Constants.AIR_FRICTION;

            mVelocity.X = MathHelper.Clamp(mVelocity.X, -Constants.MAX_MOVE_SPEED, Constants.MAX_MOVE_SPEED);
            mVelocity.Y = MathHelper.Clamp(mVelocity.Y, -Constants.MAX_MOVE_SPEED * 0.1f, Constants.MAX_MOVE_SPEED * 0.1f);

            Position += mVelocity * deltaTime;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            if (Position.X == previousPosition.X)
                mVelocity.X = 0;

            if (Position.Y == previousPosition.Y)
                mVelocity.Y = 0;

        }

    }
}
