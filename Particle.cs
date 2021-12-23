using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    public class Particle {
        public static Texture2D Laser;
        public static Texture2D LaserGlow;
        public static Texture2D PointGlow;

        private readonly Texture2D _image;
        private Color _color;
        private Vector2 _position;
        private Vector2 _velocity;
        private readonly float _friction;
        private readonly TimeSpan _lifespan;
        private TimeSpan _age;
        public bool IsExpired;
        public Particle(Texture2D texture, Color color, Vector2 position, Vector2 velocity, float friction, TimeSpan lifespan) {
            _image = texture;
            _color = color;
            _position = position;
            _velocity = velocity;
            _friction = friction;
            _lifespan = lifespan;
        }
        public void Update(GameTime gameTime) {
            _age += gameTime.ElapsedGameTime;
            if (_age >= _lifespan)
                IsExpired = true;
            _position += _velocity;
            _velocity *= _friction;
        }
        public static void LoadContent(ContentManager content) {
            Laser = content.Load<Texture2D>("Textures/GameScreen/Particles/Laser");
            LaserGlow = content.Load<Texture2D>("Textures/GameScreen/Particles/LaserGlow");
            PointGlow = content.Load<Texture2D>("Textures/GameScreen/Particles/PointGlow");
        }
        public static void UnloadContent() {
            Laser = null;
            LaserGlow = null;
            PointGlow = null;
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            Vector2 screenPos = camera.WorldToScreen(_position);
            var orientation = _velocity.ToAngle();
            double transparency;
            if (_age / _lifespan <= 0.1f)
                transparency = _age / _lifespan * 10f;
            else
                transparency = 1 - (_age / _lifespan - 0.1f) * 1.1f;
            transparency /= 2f;
            batch.Draw(_image, screenPos, null, _color * (float)transparency, orientation + camera.Orientation, _image.Size() / 2, camera.Zoom, 0, 0);
        }
    }
    public class ParticleManager {
        public static ParticleManager Instance;
        public List<Particle> Particles = new List<Particle>();
        public void Add(Particle particle) => Particles.Add(particle);
        public void Update(GameTime gameTime) {
            foreach (var particle in Particles)
                particle.Update(gameTime);
            Particles = Particles.Where(p => !p.IsExpired).ToList();
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (var particle in Particles) {
                particle?.Draw(batch, camera);
            }
        }
    }

    public static class ParticleTemplates
    {
        private static readonly Random Random = new Random();
        public static void ExhaustFire(Vector2 position, float orientation)
        {
            Color color = new Color(Random.Next(100, 255), Random.Next(0, 120), 0);
            var perpendicularPositionOffset = Random.NextFloat(-5f, 5f);
            ParticleManager.Instance.Add(new Particle(Particle.LaserGlow, color, position + MyUtils.FromPolar(orientation, 17f) + MyUtils.FromPolar(orientation + MathF.PI / 2, perpendicularPositionOffset), MyUtils.FromPolar(orientation + perpendicularPositionOffset / 30f, 5f), 0.97f, TimeSpan.FromSeconds(1)));
            ParticleManager.Instance.Add(new Particle(Particle.Laser, Color.White, position + MyUtils.FromPolar(orientation, 17f) + MyUtils.FromPolar(orientation + MathF.PI / 2, perpendicularPositionOffset), MyUtils.FromPolar(orientation + perpendicularPositionOffset / 30f, 5f), 0.97f, TimeSpan.FromSeconds(1)));
            
        }
    }
}
