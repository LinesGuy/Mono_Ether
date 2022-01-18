using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Audio;

namespace Mono_Ether {
    public class Geom : Entity {
        private static Texture2D _geomTexture;
        private static SoundEffect _pickupSound;
        private static readonly Random Rand = new Random();
        private int _age;
        private const int Lifespan = 600;
        public Geom(Vector2 position) {
            Position = position;
            Image = _geomTexture;
        }
        public static void LoadContent(ContentManager content) {
            _geomTexture = content.Load<Texture2D>("Textures/GameScreen/Geom");
            _pickupSound = content.Load<SoundEffect>("SoundEffects/GeomPickup");
        }
        public static void UnloadContent() {
            _geomTexture = null;
            _pickupSound = null;
        }
        public override void Update(GameTime gameTime) {
            Position += Velocity;
            Velocity *= 0.95f; // Friction
            Orientation += Velocity.Length() / 10f - 0.03f;

            _age++;
            if (TileMap.Instance.GetTileFromWorld(Position).Id > 0)
                /* If Geom is in solid tile, expire 5x as fast */
                _age += 4;
            if (_age > Lifespan)
                IsExpired = true;
        }
        public override void Draw(SpriteBatch spriteBatch, Camera camera) {
            if (_age > (int)(Lifespan * 0.75f)) {
                if (_age % 30 < 15)
                    base.Draw(spriteBatch, camera);
            } else {
                base.Draw(spriteBatch, camera);
            }
        }
        public void Pickup(PlayerIndex ownerPlayerIndex) {
            IsExpired = true;
            EntityManager.Instance.Players[(int)ownerPlayerIndex].AddGeoms(1);
            /* Play sound effect */
            _pickupSound.Play(GameSettings.SoundEffectVolume, Rand.NextFloat(-0.2f, 0.2f), 0);
        }
    }
}
