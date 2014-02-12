using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoPlatformerGame
{
    abstract class Enemy : Entity
    {
        public Enemy(float x, float y, SortedList<string,string> settings = null)
            : base(x, y, settings)
        {
            
        }

        public override void Update(float deltaTime)
        {
            UpdateFacing();
            ApplyPhysics(deltaTime);
            UpdateBoundingBox();
        }

        public override string GetName()
        {
            return "Enemy";
        }

        private void UpdateFacing()
        {
            if (mAccel.X > 0)
            {
                Facing = eFacing.Left;
            }
            else if (mAccel.X < 0)
            {
                Facing = eFacing.Right;
            }

        }

        protected virtual void ApplyPhysics(float deltaTime)
        {
            Vector2 previousPosition = Position;

            mVelocity += mAccel * Constants.MOVE_ACC * 0.5f * deltaTime;

            mVelocity.X *= Constants.AIR_FRICTION;
            mVelocity.Y *= Constants.AIR_FRICTION;

            mVelocity.X = MathHelper.Clamp(mVelocity.X, -Constants.MAX_MOVE_SPEED, Constants.MAX_MOVE_SPEED);

            Position += mVelocity * deltaTime;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            if (Position.X == previousPosition.X)
                mVelocity.X = 0;

            if (Position.Y == previousPosition.Y)
                mVelocity.Y = 0;

        }

        bool isOnGround = false;
        float previousBottom;
        public override void HandleCollide(Entity other)
        {
            switch (other.GetName())
            {
                case "Block":
                    HandleBlockCollision(other);
                    break;
                case "Player":
                    HandlePlayerCollision(other);
                    break;
                case "StandardBullet":
                    IsDead = true;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(mTexture.Width / 2, mTexture.Height / 2);
            origin = Vector2.Zero;

            spriteBatch.Draw(mTexture, mPosition, null, Color, 0f, origin, Scale, SpriteEffects, 0);
            //spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, null, Color, 0f, origin, SpriteEffects, 0);
            Color = Color.White;
            //spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, Color.White); //Draw Bounding Box
        }

        //private functions
        private void HandlePlayerCollision(Entity other)
        {
            Color = Color.Black;
        }

        private void HandleBlockCollision(Entity other)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(BoundingBox, other.BoundingBox);

            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.
                if (absDepthY < absDepthX)
                {

                    // Ignore platforms, unless we are on the ground.
                    if (true)
                    {
                        if (depth.Y < -10)
                            isOnGround = true;

                        // Resolve the collision along the Y axis.
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        UpdateBoundingBox();
                    }
                }
                else
                {
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    UpdateBoundingBox();
                }
            }

            previousBottom = other.BoundingBox.Bottom;
        }

       

    }
}
