using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether.Ether {
    /*
    class Geom : Entity {
        private readonly static Random rand = new Random();
        private int Age = 0;
        private const int Lifespan = 600;
        public Geom(Vector2 position) {
            Position = position;
            Image = GlobalAssets.Geom;
            Velocity = new Vector2(rand.Next(-10, 10), rand.Next(-10, 10));
        }

        public override void Update() {
            Position += Velocity;
            Velocity *= 0.95f;  // Friction
            Orientation += Velocity.Length() / 10f - 0.03f;

            Age++;
            if (Map.GetTileFromWorld(Position).TileId > 0)
                // If Geom is in solid tile, expire 5x as fast
                Age += 4;
            if (Age > Lifespan)
                IsExpired = true;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (Age > (int)(Lifespan * 0.75f)) {
                if (Age % 30 < 15) {
                    base.Draw(spriteBatch);
                }
            } else {
                base.Draw(spriteBatch);
            }
        }
        public void Pickup(int playerIndex) {
            IsExpired = true;
            EntityManager.Players[playerIndex].AddGeoms(1);
            Sounds.GeomPickup.Play(GameSettings.SoundEffectVolume, rand.NextFloat(-0.2f, 0.2f), 0);
        }
    }
    */
}
