using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoPlatformerGame
{
    abstract class Emitter
    {
        public Vector2 Position { get; set; }
        protected List<Particle> activeParticles = new List<Particle>();
        protected List<Particle> frozenParticles = new List<Particle>();

        public List<Particle> GetParticleList()
        {
            return activeParticles;
        }

        public abstract void Fire();

        public int ActiveParticleCount()
        {
            return activeParticles.Count;
        }

        public void FreezeParticle(Particle particle)
        {
            particle.IsFrozen = true;
            //frozenParticlesTMP.Add(particle);
            activeParticles.Remove(particle);
            frozenParticles.Add(particle);
        }

        public virtual void Update(float dt)
        {
            Particle[] active = activeParticles.ToArray();
            Particle[] frozen = frozenParticles.ToArray();

			Console.WriteLine (activeParticles.Count);
            
            foreach (Particle particle in active)
            {
                if (particle.IsDead)
                {
                    activeParticles.Remove(particle);
                }
                else
                {
                    particle.Update(dt);
                }
            }

            foreach (Particle particle in frozen)
            {
                if (particle.IsDead)
                {
                    frozenParticles.Remove(particle);
                }
                else
                {
                    particle.Update(dt);
                }
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in activeParticles)
            {
                particle.Draw(spriteBatch);
            }

            foreach (Particle particle in frozenParticles)
            {
                particle.Draw(spriteBatch);
            }
        }
       

    }
}
