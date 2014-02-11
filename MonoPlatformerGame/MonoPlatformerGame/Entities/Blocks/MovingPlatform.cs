using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoPlatformerGame
{
    class MovingPlatform : Entity
    {

        Vector2 startPos;
        Vector2 endPos;
        float mSpeed;

        public MovingPlatform(float x, float y, int width, float startOffset = 0, float speed = 10, SortedList<string,string> settings = null)
            : base(x + startOffset, y, settings)
        {
            mTexture = ResourceManager.GetTexture("elevator");
            startPos = new Vector2(x, y);
            endPos = new Vector2(x + width - mTexture.Width, y);
            mAccel.X = 1;
            mSpeed = speed;
            UpdateBoundingBox();
        }

        public override string GetName()
        {
            return "MovingPlatform";
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


            mVelocity = mAccel * Constants.MOVE_ACC * mSpeed * deltaTime;

            mVelocity.X *= Constants.AIR_FRICTION;

            //mVelocity.X = MathHelper.Clamp(mVelocity.X, -Constants.MAX_MOVE_SPEED, Constants.MAX_MOVE_SPEED);

            Position += mVelocity * deltaTime;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));


            UpdateBoundingBox();
        }

        public override void HandleCollide(Entity other)
        {
           
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(mTexture, mPosition, null, Color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
           // spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, null, Color, 0f, origin, SpriteEffects.None, 0);

            Color = Color.White;
        }


    }

}
