using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether.Ether {
    class Geom : Entity {
        private readonly static Random rand = new Random();
        public Geom(Vector2 position) {
            Position = position;
            Image = Art.Geom;
            Velocity = new Vector2(rand.Next(-10, 10), rand.Next(-10, 10));
        }

        public override void Update() {
            Position += Velocity;
            Velocity *= 0.95f;  // Friction
            Orientation += Velocity.Length() / 10f - 0.03f;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
        }
        public void Pickup(int playerIndex) {
            IsExpired = true;
            EntityManager.Players[playerIndex].AddGeoms(1);
            Sounds.GeomPickup.Play(GameSettings.SoundEffectVolume, rand.NextFloat(-0.2f, 0.2f), 0);
        }
    }
}
