using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public abstract class Particle {
        protected Vector2 Position;
        protected Color Color;
        protected static TimeSpan LifeSpan = TimeSpan.FromSeconds(1);
        protected TimeSpan Age = TimeSpan.Zero;
        private static readonly Random Rand = new Random();
        protected Vector2 Velocity = Rand.NextVector2(0f, 0.21f);
        protected float Friction = Rand.NextFloat(0.95f, 1f);
        public bool IsExpired;
        protected Particle(Vector2 position, Color color) {
            Position = position;
            Color = color;
        }
        public virtual void Update(GameTime gameTime) {
            Age += gameTime.ElapsedGameTime;
            if (Age > LifeSpan)
                IsExpired = true;
            Position += Velocity;
            Velocity *= Friction;
        }

        public abstract void Draw(SpriteBatch batch, Camera camera);
    }

    public class SmallParticle : Particle {
        public SmallParticle(Vector2 position, Color color) : base(position, color) {
            Position = position;
            Color = color;
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            batch.Draw(GlobalAssets.Pixel, camera.WorldToScreen(Position), null, Color, 0f, Vector2.One / 2f, 1f, 0, 0);
        }
    }
    public class BigParticle : Particle {
        private TimeSpan _timeSinceSpawn = TimeSpan.Zero;
        private static readonly TimeSpan TimeBetweenSpawns = TimeSpan.FromMilliseconds(50);
        public BigParticle(Vector2 position, Color color, Vector2 velocity, float friction, TimeSpan lifeSpan) : base(position, color) {
            Velocity = velocity;
            Friction = friction;
            LifeSpan = lifeSpan;
        }
        public override void Update(GameTime gameTime) {
            _timeSinceSpawn += gameTime.ElapsedGameTime;
            if (_timeSinceSpawn > TimeBetweenSpawns)
            {
                _timeSinceSpawn -= TimeBetweenSpawns;
                ParticleManager.Instance.Add(new SmallParticle(Position, Color));
            }
            
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            batch.Draw(GlobalAssets.Pixel, camera.WorldToScreen(Position), null, Color, 0f, Vector2.One / 2f, 1f, 0, 0);
        }
    }

    public class ParticleManager {
        public static ParticleManager Instance;
        public Particle[] Particles = new Particle[10000];
        private int _index;
        private readonly List<Particle> _addedParticles = new List<Particle>();
        private bool _isUpdating;
        public void Add(Particle particle) {
            if (!_isUpdating) {
                Particles[_index] = particle;
                _index++;
                if (_index == Particles.Length)
                    _index = 0;
            } else
                _addedParticles.Add(particle);
        }
        public void Update(GameTime gameTime) {
            _isUpdating = true;
            // TODO remove particles inside walls?
            foreach (var particle in Particles)
                particle?.Update(gameTime);
            _isUpdating = false;
            _addedParticles.ForEach(Add);
            _addedParticles.Clear();
            for (var i = 0; i < Particles.Length; i++) {
                var particle = Particles[i];
                if (particle is { IsExpired: true }) Particles[i] = null;
            }
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (var particle in Particles) {
                particle?.Draw(batch, camera);
            }
        }
    }
    public static class ParticleTemplates {
        private static readonly Random Random = new Random();
        public static void ExhaustFire(Vector2 position, float orientation) {
            var perpendicularPositionOffset = Random.NextFloat(-0.17f, 0.17f);
            ParticleManager.Instance.Add(new BigParticle(
                position + MyUtils.FromPolar(orientation, 17f), Color.CornflowerBlue,
                MyUtils.FromPolar(orientation + perpendicularPositionOffset, 5f), 0.97f, TimeSpan.FromSeconds(1)));
        }
    }
}
