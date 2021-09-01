using Microsoft.Xna.Framework;
using System;

namespace Mono_Ether.Ether
{
    class Bullet : Entity
    {
        private int age;
        private readonly int lifespan;
        private readonly Random rand = new Random();
        public Bullet(Vector2 position, Vector2 velocity)
        {
            Image = Art.Bullet;
            Position = position;
            Velocity = velocity;
            Orientation = Velocity.ToAngle();
            Radius = 8;
            age = 0;
            lifespan = 120;  // Frames
        }

        public override void Update()
        {
            /*if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();*/

            Position += Velocity;

            // Delete bullets after a certain time
            age += 1;
            if (age > lifespan)
                IsExpired = true;
        }

        public override void HandleTilemapCollision()
        {
            var tile = Map.GetTileFromWorld(Position);
            if (tile.TileId > 0)
            {
                IsExpired = true;

                float hue1 = rand.NextFloat(0, 6);
                float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;
                Color color1 = ColorUtil.HsvToColor(hue1, 0.5f, 1);
                Color color2 = ColorUtil.HsvToColor(hue2, 0.5f, 1);

                for (var i = 0; i < 20; i++)
                {
                    var speed = 7f * (1f - 1 / rand.NextFloat(1f, 10f));
                    var state = new ParticleState()
                    {
                        Velocity = rand.NextVector2(speed, speed),
                        Type = ParticleType.Enemy,
                        LengthMultiplier = 1f
                    };

                    //Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));
                    Color color = new Color(235, 222, 77);
                    EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
                }
            }
                
        }
    }

    class Starburst : Entity
    {
        private int age;
        private readonly int lifespan;
        static readonly Random Rand = new Random();
        static float bullet_speed = 15f;

        public Starburst(Vector2 position, Vector2 destination)
        {
            Image = Art.StarBurst;
            Position = position;
            Velocity = Vector2.Normalize(destination - position) * bullet_speed;
            Orientation = Velocity.ToAngle();
            age = 0;
            lifespan = (int)((destination - position).Length() / bullet_speed);
        }

        public override void Update()
        {
            Position += Velocity;
            Orientation += 0.3f;
            age += 1;
            if (age > lifespan || Map.GetTileFromWorld(Position).TileId > 0)
            {
                Position -= Velocity;
                IsExpired = true;
                for (int i = 0; i < 50; i++)
                {
                    Vector2 bulletVelocity = MathUtil.FromPolar(Rand.NextFloat((float)-Math.PI, (float)Math.PI), Rand.NextFloat(8f, 16f));
                    EntityManager.Add(new Bullet(Position, bulletVelocity));
                }

            }
        }
    }
}
