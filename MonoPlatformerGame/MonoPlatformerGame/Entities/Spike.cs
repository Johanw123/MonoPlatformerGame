using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    class Spike : Entity
    {
        public Spike(float x, float y, SortedList<string,string> settings = null)
            : base(x, y, settings)
        {
			ParseSettings(settings);

            UpdateBoundingBox();
        }

		private void ParseSettings (SortedList<string,string> settings)
		{
			mTexture = ResourceManager.GetTexture(settings["Texture"]);
		}

        public override void Update(float deltaTime)
        {

        }

        public override string GetName()
        {
            return "Spike";
        }

        public override void HandleCollide(Entity other)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(mTexture.Width / 2, mTexture.Height / 2);
            origin = Vector2.Zero;

            spriteBatch.Draw(mTexture, mPosition, null, Color, 0f, origin, Scale, SpriteEffects, 0);
            Color = Color.White;
        }
    }
}
