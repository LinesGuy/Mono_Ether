using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether {
    public class Bullet : Entity {
        private static Texture2D _bulletTexture;
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
        }
        public void Expire() {
            IsExpired = true;
            ParticleTemplates.Explosion(Position, 1f, 2f, 20, Color.Yellow);
        }
        public override void Update(GameTime gameTime) {
            Position += Velocity; // Update position
            _age++;
            if (_age > Lifespan) // Delete bullet after lifespan reached
                IsExpired = true;
            if (TileMap.Instance.GetTileFromWorld(Position).Id <= 0) // Delete bullet if collided with a wall
                return;
            IsExpired = true;
            ParticleTemplates.Explosion(Position, 1f, 2f, 20, new Color(255, 255, 0)); // Summon explosion effect upon expiring
        }
        public static void LoadContent(ContentManager content) {
            _bulletTexture = content.Load<Texture2D>("Textures/GameScreen/Bullet");
        }
        public static void UnloadContent() {
            _bulletTexture = null;
        }
    }
    public class StarBurst : Entity {
        private static Texture2D _starBurstTexture;
        private int _age;
        private readonly int _lifespan;
        private readonly PlayerIndex _parentPlayerIndex;
        private static readonly Random Random = new Random();
        private const float BulletSpeed = 15f;
        public StarBurst(Vector2 position, Vector2 destination, PlayerIndex playerIndex) {
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
            Position += Velocity; // Update position based on velocity
            Orientation += 0.3f; // Rotate projectile slightly
            _age++;
            // Return if the age is less than the lifespan and the bullet hasn't hit a wall yet
            if (_age <= _lifespan && TileMap.Instance.GetTileFromWorld(Position).Id <= 0)
                return;
            // Move the bullet backwards one unit in case the bullet collided with a wall, so the summoned bullets don't spawn inside the wall
            Position -= Velocity;
            IsExpired = true;
            for (var i = 0; i < 50; i++) {
                // Summon bullet in random direction
                var bulletVelocity = MyUtils.FromPolar(Random.NextFloat((float)-Math.PI, (float)Math.PI), Random.NextFloat(8f, 16f));
                EntityManager.Instance.Add(new Bullet(Position, bulletVelocity, new Color(128, 128, 0), _parentPlayerIndex));
            }
        }
    }
}
