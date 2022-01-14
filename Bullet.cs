﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class Bullet : Entity {
        private static Texture2D _bulletTexture;
        private static Texture2D _bulletGlowTexture; // TODO add bullet glow
        public PlayerIndex ParentPlayerIndex;
        private int _age;
        private const int Lifespan = 120;
        public Bullet(Vector2 position, Vector2 velocity, Color color, PlayerIndex playerIndex) {
            Image = _bulletTexture;
            Position = position;
            Velocity = velocity;
            EntityColor = color;
            ParentPlayerIndex = playerIndex;
            Orientation = Velocity.ToAngle();
            Radius = 9; // Min(image width, image height)
        }
        public void Expire() {
            IsExpired = true;
            /* TODO Summon particles */
            //ParticleTemplates.Explosion(Position, 1f, 2f, 20);
        }
        public override void Update(GameTime gameTime) {
            Position += Velocity;
            /* Delete bullet after lifespan reached */
            _age++;
            if (_age > Lifespan)
                IsExpired = true;
            /* Delete bullet if collided with a wall */
            if (TileMap.Instance.GetTileFromWorld(Position).Id <= 0) return;
            IsExpired = true;
            ParticleTemplates.Explosion(Position, 1f, 2f, 20);
        }
        public static void LoadContent(ContentManager content) {
            _bulletTexture = content.Load<Texture2D>("Textures/GameScreen/Bullet");
            _bulletGlowTexture = content.Load<Texture2D>("Textures/GameScreen/BulletGlow");
        }
        public static void UnloadContent() {
            _bulletTexture = null;
            _bulletGlowTexture = null;
        }
    }
}
