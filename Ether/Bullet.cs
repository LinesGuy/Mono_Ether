using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.Ether
{
    class Bullet : Entity
    {
        protected int age;
        protected int lifespan;

        public Bullet(Vector2 position, Vector2 velocity)
        {
            image = Art.Bullet;
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
        protected int age;
        protected int lifespan;
        static Random rand = new Random();
        static float bullet_speed = 15f;

        public Starburst(Vector2 position, Vector2 destination)
        {
            image = Art.StarBurst;
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
                    Vector2 bullet_velocity = MathUtil.FromPolar(rand.NextFloat((float)-Math.PI, (float)Math.PI), rand.NextFloat(8f, 16f));
                    EntityManager.Add(new Bullet(Position, bullet_velocity));
                }

            }
        }
    }
}
