using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoPlatformerGame
{
    public enum eFacing
    {
        Left,
        Right,
        None
    }

    public abstract class Entity
    {
        protected Texture2D mTexture;
        protected Vector2 mVelocity;
        protected Vector2 mPosition;
        protected Vector2 mAccel;

        public Entity(float x, float y, SortedList<string,string> settings = null)
        {
            Position = new Vector2(x, y);
            Color = Color.White;
            Rotation = 0.0f;
            Scale = 1.0f;
            Facing = eFacing.Right;
        }

        public virtual void UpdateBoundingBox()
        {
            int width = 0;
            int height = 0;

            if (mTexture != null)
            {
                width = mTexture.Width;
                height = mTexture.Height;
            }

            BoundingBox = new Rectangle((int)X, (int)Y, width, height);
        }

        private SpriteEffects mSpriteEffects;
        public SpriteEffects SpriteEffects
        {
            get 
            {
                if (Facing == eFacing.Left)
                    mSpriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;

                return mSpriteEffects;
            }
            set { mSpriteEffects = value; }
        }

        public bool IsDead { get; set; }
        public Color Color { get; set; }
        public Rectangle BoundingBox { get; set; }
        public float Life { get; set; }
        public eFacing Facing { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }

        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public Vector2 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        public Vector2 Accel
        {
            get { return mAccel; }
            set { mAccel = value; }
        }

        public Point Size
        {
            get { return new Point(mTexture.Width, mTexture.Height); }
        }

        public int Width
        {
            get { return mTexture.Width; }
        }

        public int Height
        {
            get { return mTexture.Height; }
        }

        public Texture2D Texture
        {
            get { return mTexture; }
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public float VelX
        {
            get { return Velocity.X; }
            set { Velocity = new Vector2(Velocity.X, value); }
        }

        public float VelY
        {
            get { return Velocity.Y; }
            set { Velocity = new Vector2(value, Velocity.Y); }
        }

        public virtual string GetName()
        {
            return "Null";
        }

        public abstract void HandleCollide(Entity other);
        public abstract void Update(float deltaTime);
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }

    
}
