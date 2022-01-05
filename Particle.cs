using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public abstract class Particle {
        public static Texture2D PointParticle;
        protected Vector2 Position;
        protected Color Color;

        private static readonly Random Rand = new Random();
        protected Vector2 Velocity;
        protected float Friction = Rand.NextFloat(0.95f, 1f);
        public bool IsExpired;
        protected Particle(Vector2 position, Color color) {
            Position = position;
            Color = color;
        }
        public virtual void Update(GameTime gameTime) {
            Position += Velocity;
            Velocity *= Friction;
        }

        public abstract void Draw(SpriteBatch batch, Camera camera);
    }

    public class SmallParticle : Particle {
        protected TimeSpan Age = TimeSpan.Zero;
        private readonly TimeSpan _lifeSpan = TimeSpan.FromSeconds(0.3);
        private static readonly Random _rand = new Random();
        private Color _transparentColor;
        public override void Update(GameTime gameTime) {
            Age += gameTime.ElapsedGameTime;
            if (Age > _lifeSpan)
                IsExpired = true;
            _transparentColor = Color * (float) (1 - Age / _lifeSpan);
            base.Update(gameTime);
        }
        public SmallParticle(Vector2 position, Color color) : base(position, color) {
            Position = position;
            Color = color;
            Velocity = _rand.NextVector2(0f, 0.2f);
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            batch.Draw(PointParticle, camera.WorldToScreen(Position), null, _transparentColor, 0f, PointParticle.Size() / 2f, 1f, 0, 0);
        }
    }
    public class BigParticle : Particle {
        public BigParticle(Vector2 position, Color color, Vector2 velocity, float friction) : base(position, color) {
            Velocity = velocity;
            Friction = friction;
        }
        public override void Update(GameTime gameTime)
        {
            ParticleManager.Instance.Add(new SmallParticle(Position, Color));
            if (Velocity.LengthSquared() < 0.5f)
                IsExpired = true;
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            //batch.Draw(PointParticle, camera.WorldToScreen(Position), null, Color * (float)(1 - Age / LifeSpan), 0f, Vector2.One / 2f, 1.5f, 0, 0);
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
            var orientationOffset = Random.NextFloat(-0.3f, 0.3f);
            var positionOffset = Random.NextVector2(0f, 10f);
            Color color = new Color(0.2f, 0.8f - MathF.Abs(orientationOffset), 1f);
            ParticleManager.Instance.Add(new BigParticle(
                position + positionOffset, color,
                MyUtils.FromPolar(orientation + orientationOffset, Random.NextFloat(1f, 6f)), 0.99f));
        }
    }
}
