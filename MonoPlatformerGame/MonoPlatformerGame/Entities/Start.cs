using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    class Start : Entity
    {
        public Start(float x, float y, SortedList<string,string> settings = null)
            : base(x, y, settings)
        {

            mTexture = ResourceManager.GetTexture("Start");




            UpdateBoundingBox();
        }
        public override string GetName()
        {
            return "Start";
        }
        public override void Update(float deltaTime)
        {

        }
        public override void HandleCollide(Entity other)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(mTexture.Width / 2, mTexture.Height / 2);
            origin = Vector2.Zero;

            spriteBatch.Draw(mTexture, mPosition, null, Color, 0f, origin, Scale, SpriteEffects, 0);
            spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, null, Color, 0f, origin, SpriteEffects.None, 0);
            Color = Color.White;
            //spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, Color.White); //Draw Bounding Box
        }


    }
}
