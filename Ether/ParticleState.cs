using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Mono_Ether.Ether {
    public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

    public struct ParticleState {
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;

        private static readonly Random rand = new Random();

        public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f) {
            Velocity = velocity;
            Type = type;
            LengthMultiplier = lengthMultiplier;
        }

        public static ParticleState GetRandom(float minVel, float maxVel) {
            var state = new ParticleState {
                Velocity = rand.NextVector2(minVel, maxVel),
                Type = ParticleType.None,
                LengthMultiplier = 1
            };

            return state;
        }

        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle) {
            var vel = particle.State.Velocity;

            float speed = vel.Length();

            // Using Vector2.Add() should be slightly faster than doing "x.Position += vel;" because the Vector2s
            // are passed by reference and don't need to be copied. Since we may have to update a very large
            // number of particles, this method is a good candidate for optimizations.
            Vector2.Add(ref particle.Position, ref vel, out particle.Position);

            // Fade the particle if its PercentLife or speed is too low.
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;

            particle.Tint.A = (byte)(255 * alpha);

            // The length of bullet particles will be less dependent on their speed than other particles
            if (particle.State.Type == ParticleType.Bullet)
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
            else
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

            particle.Orientation = vel.ToAngle();

            var pos = particle.Position;

            if (particle.State.Type != ParticleType.IgnoreGravity) {
                // foreach blackholes stuff here
            }

            // Denormalized floats cause significant performance issues
            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
                vel = Vector2.Zero;
            else if (particle.State.Type == ParticleType.Enemy)
                vel *= 0.94f;
            else
                vel *= 0.96f + Math.Abs(pos.X) % 0.04f; // use the position for pseudo-randomness.
            particle.State.Velocity = vel;
        }
    }
}