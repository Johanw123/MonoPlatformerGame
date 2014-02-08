using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoPlatformerGame
{
    class CircleEmitter : Emitter
    {
        public override void Fire()
        {
            List<Particle> particles = new List<Particle>();

            float Radius = 5;
            int count = 25;
            int maxParticleCount = 300;
            int activeParticles = ActiveParticleCount();

            if (count +  activeParticles > maxParticleCount)
                count = maxParticleCount - activeParticles;

            for (int i = 0; i < count; i++)
            {
                Particle particle = new Particle(this);
                particle.TTL = Runtime.GetRandom().Next(100,500);
                float rads = (float)(Runtime.GetRandom().NextDouble() * MathHelper.TwoPi);
                Vector2 offset = new Vector2((float)Math.Cos(rads) * Radius, (float)Math.Sin(rads) * Radius);
                Vector2 emitPosition = Vector2.Add(Position, offset);
                particle.X = emitPosition.X;
                particle.Y = emitPosition.Y;
                Vector2 vel = new Vector2(Runtime.GetRandom().Next(-50,50), Runtime.GetRandom().Next(-50,50));
                vel.Normalize();
                particle.Velocity = vel * Runtime.GetRandom().Next(85,185);
                particles.Add(particle);
            }

            this.activeParticles.AddRange(particles);

        }

    }
}
