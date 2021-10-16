using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether.Ether {
    public class BackgroundParticle {
        public Vector2 Pos;
        public float Size;
        public float CycleSpeed;
        public float RotationSpeed;
        public float Age;
        public float Rotation;
        public float Brightness;

        private readonly static Random rand = new Random();
        public BackgroundParticle(Vector2 bounds) {
            Pos = new Vector2(rand.NextFloat(0, bounds.X), rand.NextFloat(0, bounds.Y));
            Size = rand.NextFloat(0.6f, 2.5f);
            RotationSpeed = rand.NextFloat(0.01f, 0.03f);
            CycleSpeed = rand.NextFloat(0.005f, 0.03f);
        }

        public void Update() {
            Age += CycleSpeed;
            Brightness = (float)Math.Sin(Age) / 2f + 0.5f;
            Rotation += RotationSpeed;
        }

        public void Draw(SpriteBatch spriteBatch) {
            var particlePos = Camera.WorldToScreen(Pos);
            var radius = Art.BackgroundParticle.Width / 2f;
            if (particlePos.X < GameRoot.ScreenSize.X + radius && particlePos.Y < GameRoot.ScreenSize.Y + radius && particlePos.X > 0 - radius && particlePos.Y > 0 - radius)
                spriteBatch.Draw(Art.BackgroundParticle, Camera.WorldToScreen(Pos), null, Color.White * Brightness, Rotation, new Vector2(radius), Camera.Zoom * Size * Brightness, SpriteEffects.None, 0);
        }
    }

    public static class BackgroundParticleManager {
        private static List<BackgroundParticle> Particles = new List<BackgroundParticle>();

        public static void Populate(Vector2 size, int num) {
            for (int i = 0; i < num; i++) {
                Particles.Add(new BackgroundParticle(size));
            }
            Particles = Particles.Where(x => !(Map.GetTileFromWorld(x.Pos).TileId > 0)).ToList();
        }
        public static void Clear() {
            Particles.Clear();
        }
        public static void Update() {
            foreach (var particle in Particles) {
                particle.Update();
            }
        }
        public static void Draw(SpriteBatch spriteBatch) {
            foreach (var particle in Particles) {
                particle.Draw(spriteBatch);
            }
        }
    }
}