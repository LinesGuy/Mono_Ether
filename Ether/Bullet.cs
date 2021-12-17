using Microsoft.Xna.Framework;
using System;

namespace Mono_Ether.Ether {
    /*
    class Bullet : Entity {
        private int age;
        private readonly int lifespan;
        public readonly int PlayerIndex;
        private readonly Random rand = new Random();
        public Bullet(Vector2 position, Vector2 velocity, Color color, int playerIndex) {
            Image = GlobalAssets.Bullet;
            Position = position;
            Velocity = velocity;
            PlayerIndex = playerIndex;
            Orientation = Velocity.ToAngle();
            Radius = 8;
            age = 0;
            lifespan = 120;  // Bullet will disappear after two seconds if it doesn't hit something
            Color = color;
        }
        public override void Update() {
            Position += Velocity;
            // Delete bullets after a certain time:
            age += 1;
            if (age > lifespan)
                IsExpired = true;
        }
        public override void HandleTilemapCollision() {
            var tile = Map.GetTileFromWorld(Position);
            if (tile.TileId > 0) {
                Expire();
            }
        }
        public void Expire() {
            IsExpired = true;
            // Particles
            for (var i = 0; i < 20; i++) {
                var speed = 7f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState() {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = new Color(235, 222, 77);
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LineParticle, Position, color, 190, 1.5f, state);
            }
            // Explosion
            ExplosionManager.Add(new Explosion(Position));
        }
    }

    class Starburst : Entity {
        private int age;
        private readonly int lifespan;
        private readonly int PlayerIndex;
        static readonly Random Rand = new Random();
        static readonly float bullet_speed = 15f;
        public Starburst(Vector2 position, Vector2 destination, int playerIndex) {
            Image = GlobalAssets.StarBurst;
            Position = position;
            PlayerIndex = playerIndex;
            Velocity = Vector2.Normalize(destination - position) * bullet_speed;
            Orientation = Velocity.ToAngle();
            age = 0;
            lifespan = (int)((destination - position).Length() / bullet_speed);
        }

        public override void Update() {
            Position += Velocity;
            Orientation += 0.3f;
            age += 1;
            if (age > lifespan || Map.GetTileFromWorld(Position).TileId > 0) {
                Position -= Velocity;
                IsExpired = true;
                for (int i = 0; i < 50; i++) {
                    Vector2 bulletVelocity = MathUtil.FromPolar(Rand.NextFloat((float)-Math.PI, (float)Math.PI), Rand.NextFloat(8f, 16f));
                    EntityManager.Add(new Bullet(Position, bulletVelocity, new Color(128, 128, 0), PlayerIndex));
                }
            }
        }
    }
    */
}
