using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoPlatformerGame
{
    class ParticleSystem
    {
        private static Dictionary<string, Emitter> mEmitters;

        public static void Init()
        {
            mEmitters = new Dictionary<string, Emitter>();
        }
        public static void AddEmitter(string emitterNameIdentifier, Emitter emitter)
        {
            mEmitters.Add(emitterNameIdentifier, emitter);
        }
        public static Emitter[] GetAllEmitters()
        {
            Emitter[] emitterArray = new Emitter[mEmitters.Values.Count];

            mEmitters.Values.CopyTo(emitterArray, 0);

            return emitterArray;
        }
        public static Particle[] GetAllParticles()
        {
            List<Particle> particles = new List<Particle>();

            foreach (Emitter emitter in GetAllEmitters())
            {
                particles.AddRange(emitter.GetParticleList());
            }

            return particles.ToArray();
        }
        public static Emitter GetEmitter(string emitterNameIdentifier)
        {
            Emitter emitter;

            mEmitters.TryGetValue(emitterNameIdentifier, out emitter);

            return emitter;
        }
        public static void FireEmitter(string emitterNameIdentifier)
        {
            Emitter emitter = GetEmitter(emitterNameIdentifier);

            if (emitter != null)
                emitter.Fire();
        }
        public static void FireEmitter(Emitter emitter)
        {
            emitter.Fire();
        }
        public static void FireEmitterAt(string emitterNameIdentifier, Vector2 position)
        {
            Emitter emitter = GetEmitter(emitterNameIdentifier);
            emitter.Position = position;
            FireEmitter(emitter);
        }

        public static void RemoveEmitter(Emitter emitter)
        {

        }

        public static void RemoveEmitter(string emitterNameIdentifier)
        {

        }

        public static void Update(float dt)
        {
            foreach (Emitter emitter in mEmitters.Values)
            {
                emitter.Update(dt);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null,  EntityManager.GetCameraMatrix());
            foreach (Emitter emitter in mEmitters.Values)
            {
                emitter.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

    }
}
