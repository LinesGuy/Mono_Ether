using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using MonoGame.Extended;

namespace Mono_Ether.Ether
{
    class PlayerShip : Entity
    {
        private static PlayerShip _instance;
        public static PlayerShip Instance => _instance ??= new PlayerShip();
        public bool GodMode = false;  // If true, player can't die
        public int lives;
        private PlayerShip()
        {
            Image = Art.Player;
            // Position = GameRoot.ScreenSize / 2;
            Position = new Vector2(0, 0);
            Radius = 10;
            lives = 3;
        }

        const int CooldownFrames = 6;
        int cooldownRemaining;
        static readonly Random Rand = new Random();
        private readonly bool autoFire = false;  // If true, hold left click to stop fire
        int framesUntilRespawn;

        // PowerPack stuffs
        public int ShootSpeedIncreaseFramesRemaining = 0;
        public bool IsDead => framesUntilRespawn > 0;

        public override void Update()
        {
            // Press G to toggle GodMode (for debugging purposes)
            if (Input.Keyboard.WasKeyJustDown(Keys.G))
                GodMode = !GodMode;
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
            // Change orientation if velocity is non-zero:
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            HandleTilemapCollision();

            // Exhaust fire
            if (Velocity.LengthSquared() > 0.1f)
            {
                Orientation = Velocity.ToAngle();
                double t = EtherRoot.CurrentGameTime.TotalGameTime.TotalSeconds;
                Vector2 baseVel = Velocity.ScaleTo(-3);
                Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float) Math.Sin(t * 10));
                Color sideColor = new Color(200, 38, 9);   // Deep red
                Color midColor = new Color(255, 187, 30);  // Orange-yellow
                Vector2 pos = Position + MathUtil.FromPolar(Orientation, -25);
                const float alpha = 0.7f;
                
                // Middle particle stream
                Vector2 velMid = baseVel + Rand.NextVector2(0, 1);
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(Art.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));
                
                // Side particle streams
                Vector2 vel1 = baseVel + perpVel + Rand.NextVector2(0, 0.3f);
                Vector2 vel2 = baseVel - perpVel + Rand.NextVector2(0, 0.3f);
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));
                
            }

            // Shoot
            if (!EtherRoot.Instance.editorMode)
            {
                var aim = Camera.GetAimDirection();
                if ((autoFire ^ Input.Mouse.LeftButton == ButtonState.Pressed) && aim.LengthSquared() > 0 && cooldownRemaining <= 0)
                {
                    Art.PlayerShoot.CreateInstance().Play();
                    cooldownRemaining = CooldownFrames;
                    var aimangle = aim.ToAngle();
                    const int bulletCount = 3;
                    for (var i = 0; i < bulletCount; i++)
                    {
                        var randomSpread = Rand.NextFloat(-0.04f, 0.04f) + Rand.NextFloat(-0.04f, 0.04f);
                        var offsetAngle = aimangle + MathUtil.Interpolate(-.2f, .2f, i / (bulletCount - 0.999f));
                        var offset = MathUtil.FromPolar(offsetAngle, Rand.NextFloat(15f, 40f));
                        var vel = MathUtil.FromPolar(aimangle + randomSpread, 18f);
                        Color bulletColor;
                        if (ShootSpeedIncreaseFramesRemaining > 0)
                            bulletColor = new Color(3, 252, 252);
                        else
                            bulletColor = new Color(239, 247, 74);
                        EntityManager.Add(new Bullet(Position + offset, vel, bulletColor));
                    }
                }

                if (cooldownRemaining > 0)
                {
                    cooldownRemaining--;
                    if (ShootSpeedIncreaseFramesRemaining > 0)
                    {
                        ShootSpeedIncreaseFramesRemaining--;
                        cooldownRemaining--;
                    }
                }
                    
                if (Input.Mouse.WasButtonJustDown(MonoGame.Extended.Input.MouseButton.Right))
                {
                    EntityManager.Add(new Starburst(Position, Camera.mouse_world_coords()));
                }
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
            Art.PlayerDeath.CreateInstance().Play();
            lives -= 1;

            for (int i = 0; i < 1200; i++)
            {
                float speed = 18f * (1f - 1 / Rand.NextFloat(1f, 10f));
                Color color = Color.Lerp(Color.White, Color.Yellow, Rand.NextFloat(0, 1));
                var state = new ParticleState()
                {
                    Velocity = Rand.NextVector2(speed, speed),
                    Type = ParticleType.None,
                    LengthMultiplier = 1
                };
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }
            EnemySpawner.Reset();
        }

        public void ApplyPowerPack(string powerPackType)
        {
            switch (powerPackType)
            {
                case "ShootSpeedIncrease":
                    ShootSpeedIncreaseFramesRemaining = 600;
                    break;
                default:
                    Debug.WriteLine("!!! PlayerShip.cs ApplyPowerPack() unhandled powerPackType");
                    break;
            }
        }
    }
}
