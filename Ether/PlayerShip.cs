using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mono_Ether.Ether
{
    class PlayerShip : Entity
    {
        private static PlayerShip instance;
        public static PlayerShip Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerShip();
                return instance;
            }
        }

        private PlayerShip()
        {
            image = Art.Player;
            // Position = GameRoot.ScreenSize / 2;
            Position = new Vector2(0, 0);
            Radius = 10;
        }

        const int cooldownFrames = 6;
        int cooldownRemaining = 0;
        static Random rand = new Random();
        bool autofire = false;  // If true, hold left click to stop fire
        int framesUntilRespawn = 0;

        public bool IsDead { get { return framesUntilRespawn > 0; } }

        public override void Update()
        {
            // Do nothing if dead
            if (IsDead)
            {
                framesUntilRespawn--;
                return;
            }

            // Movement
            const float acceleration = 5;
            Velocity += acceleration * Input.GetMovementDirection();  // Normalised direction vector
            Velocity = Velocity / 1.5f;  // Friction
            Position += Velocity;
            // Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);
            // Change orientation if velocity is non-zero:
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            // Shooty
            var aim = Camera.GetAimDirection();
            if ((autofire ^ Input.mouseState.LeftButton == ButtonState.Pressed) && aim.LengthSquared() > 0 && cooldownRemaining <= 0)
            {
                cooldownRemaining = cooldownFrames;
                float aimangle = aim.ToAngle();
                int bulletCount = 3; 
                for (int i = 0; i < bulletCount; i++)
                {
                    float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                    float offsetAngle = aimangle + MathUtil.Interpolate(-.2f, .2f, (float)i / (bulletCount - 0.999f));
                    Vector2 offset = MathUtil.FromPolar(offsetAngle, rand.NextFloat(15f, 40f));
                    Vector2 vel = MathUtil.FromPolar(aimangle + randomSpread, 18f);
                    EntityManager.Add(new Bullet(Position + offset, vel));
                }
            }

            if (cooldownRemaining > 0)
                cooldownRemaining--;

            if (Input.WasMouseClicked("right"))
            {
                EntityManager.Add(new Starburst(Position, Camera.mouse_world_coords()));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
                base.Draw(spriteBatch);
        }

        public void Kill()
        {
            framesUntilRespawn = 60;
            EnemySpawner.Reset();
        }
    }
}
