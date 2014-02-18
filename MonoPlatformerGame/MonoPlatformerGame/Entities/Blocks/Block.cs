using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoPlatformerGame
{
    class Block : Entity
    {
        public Block(float x, float y, SortedList<string,string> settings = null)
            : base(x, y, settings)
        {
            mTexture = ResourceManager.GetTexture("Block");
            UpdateBoundingBox();
        }

        public override string GetName()
        {
            return "Block";
        }

        public override void Update(float deltaTime)
        {

        }

        public override void HandleCollide(Entity other)
        {
            switch (other.GetName())
            {
                case "Player":
                    //Color = Color.Black;
                    break;
            }  
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTexture, mPosition, null, Color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
           // spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, null, Color, 0f, origin, SpriteEffects.None, 0);
        }


    }

}
