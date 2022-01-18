using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public abstract class Particle {
        protected static Texture2D PointParticle;
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

        public static void LoadContent(ContentManager content) {
            PointParticle = content.Load<Texture2D>("Textures/GameScreen/Particles/Point");
        }

        public static void UnloadContent() {
            PointParticle = null;
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
            _transparentColor = Color * (float)(1 - Age / _lifeSpan);
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
        public override void Update(GameTime gameTime) {
            ParticleManager.Instance.Add(new SmallParticle(Position, Color));
            if (Velocity.LengthSquared() < 1.5f)
                IsExpired = true;
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            //batch.Draw(PointParticle, camera.WorldToScreen(Position), null, Color * (float)(1 - Age / LifeSpan), 0f, Vector2.One / 2f, 1.5f, 0, 0);
        }
    }

    public class ParticleManager {
        public static ParticleManager Instance;
        public BigParticle[] BigParticles = new BigParticle[10000];
        public SmallParticle[] SmallParticles = new SmallParticle[10000];
        private int _bigIndex;
        private int _smallIndex;
        private readonly List<Particle> _addedParticles = new List<Particle>();
        private bool _isUpdating;
        public void Add(Particle particle) {
            if (!_isUpdating) {
                if (particle is BigParticle bigParticle) {
                    BigParticles[_bigIndex] = bigParticle;
                    _bigIndex++;
                    if (_bigIndex == BigParticles.Length)
                        _bigIndex = 0;
                } else if (particle is SmallParticle smallParticle) {
                    SmallParticles[_smallIndex] = smallParticle;
                    _smallIndex++;
                    if (_smallIndex == SmallParticles.Length)
                        _smallIndex = 0;
                }
            } else
                _addedParticles.Add(particle);
        }
        public void Update(GameTime gameTime) {
            _isUpdating = true;
            // TODO remove particles inside walls?
            foreach (var particle in BigParticles)
                particle?.Update(gameTime);
            foreach (var particle in SmallParticles)
                particle?.Update(gameTime);
            _isUpdating = false;
            _addedParticles.ForEach(Add);
            _addedParticles.Clear();
            for (var i = 0; i < BigParticles.Length; i++)
                if (BigParticles[i] != null && BigParticles[i].IsExpired)
                    BigParticles[i] = null;
            for (var i = 0; i < SmallParticles.Length; i++)
                if (SmallParticles[i] != null && SmallParticles[i].IsExpired)
                    SmallParticles[i] = null;
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (var particle in BigParticles)
                particle?.Draw(batch, camera);
            foreach (var particle in SmallParticles)
                particle?.Draw(batch, camera);
        }
    }
    public static class ParticleTemplates {
        private static readonly Random Random = new Random();
        public static void ExhaustFire(Vector2 position, float orientation) { // TODO add count parameter
            var orientationOffset = Random.NextFloat(-0.3f, 0.3f);
            var positionOffset = Random.NextVector2(0f, 10f);
            var color = new Color(0.2f, 0.8f - MathF.Abs(orientationOffset), 1f);
            ParticleManager.Instance.Add(new BigParticle(
                position + positionOffset, color,
                MyUtils.FromPolar(orientation + orientationOffset, Random.NextFloat(1f, 6f)), 0.99f));
        }
        public static void Explosion(Vector2 position, float minSpeed, float maxSpeed, int count) { // TODO add color parameter
            for (var i = 0; i < count; i++) {
                var orientation = Random.NextFloat(0f, MathF.PI * 2f);
                var speed = Random.NextFloat(minSpeed, maxSpeed);
                var color = new Color(255, 255, 0); // TODO
                ParticleManager.Instance.Add(new BigParticle(
                    position, color, MyUtils.FromPolar(orientation, speed), 0.99f));
            }
        }
    }
}
