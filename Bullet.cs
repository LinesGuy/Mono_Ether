using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

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
    public class Starburst : Entity {
        private static Texture2D _starBurstTexture;
        private int _age;
        private readonly int _lifespan;
        private readonly PlayerIndex _parentPlayerIndex;
        private static readonly Random Random = new Random();
        private const float BulletSpeed = 15f;
        public Starburst(Vector2 position, Vector2 destination, PlayerIndex playerIndex) {
            Image = _starBurstTexture;
            Position = position;
            _parentPlayerIndex = playerIndex;
            Velocity = Vector2.Normalize(destination - position) * BulletSpeed;
            Orientation = Velocity.ToAngle();
            _lifespan = (int)((destination - position).Length() / BulletSpeed);
        }
        public static void LoadContent(ContentManager content) {
            _starBurstTexture = content.Load<Texture2D>("Textures/GameScreen/StarBurst");
        }
        public static void UnloadContent() {
            _starBurstTexture = null;
        }
        public override void Update(GameTime gameTime) {
            Position += Velocity;
            Orientation += 0.3f;
            _age++;
            if (_age <= _lifespan && TileMap.Instance.GetTileFromWorld(Position).Id <= 0) return;
            Position -= Velocity;
            IsExpired = true;
            for (var i = 0; i < 50; i++) {
                var bulletVelocity = MyUtils.FromPolar(Random.NextFloat((float)-Math.PI, (float)Math.PI), Random.NextFloat(8f, 16f));
                EntityManager.Instance.Add(new Bullet(Position, bulletVelocity, new Color(128, 128, 0), _parentPlayerIndex));
            }
        }
    }
}
