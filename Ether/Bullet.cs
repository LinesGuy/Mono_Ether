using Microsoft.Xna.Framework;
using System;

namespace Mono_Ether.Ether
{
    class Bullet : Entity
    {
        private int age;
        private readonly int lifespan;

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
            if (age >= lifespan)
            {
                IsExpired = true;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 bulletVelocity = MathUtil.FromPolar(Rand.NextFloat((float)-Math.PI, (float)Math.PI), Rand.NextFloat(8f, 16f));
                    EntityManager.Add(new Bullet(Position, bulletVelocity));
                }

            }
        }
    }
}
