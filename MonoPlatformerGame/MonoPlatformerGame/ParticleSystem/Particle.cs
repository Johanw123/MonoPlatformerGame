using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoPlatformerGame
{
    class Particle : Entity
    {
        public bool IsFrozen{ get;set; }
        public int TTL { get; set; }
        private int timeAlive = 0;

        private Emitter mOwner;
        
        public Particle(Emitter owner) : 
            base(0,0)
        {
            mTexture = ResourceManager.GetTexture("pixel");
            mOwner = owner;
        }

        public override string GetName()
        {
            return "Particle";
        }

        public override void Update(float dt)
        {
            if (++timeAlive > TTL)
            {
                IsDead = true;
            }

            if (IsFrozen)
                return;

            X += VelX * dt;
            Y += VelY * dt;
            UpdateBoundingBox();
        }

        public override void HandleCollide(Entity other)
        {
            switch (other.GetName())
            {
                case "Block":
                    Freeze();
                    break;
                case "Spike":
                    Freeze();
                    break;
            }
        }

        private void Freeze()
        {
            VelX = 0;
            VelY = 0;
            mOwner.FreezeParticle(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {            
           spriteBatch.Draw(mTexture, Position, Color.Red);
        }        
    }
}
