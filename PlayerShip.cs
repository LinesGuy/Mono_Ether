using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mono_Ether {
    public class PlayerShip : Entity {
        public static Texture2D Texture;
        public PlayerIndex Index;
        public Camera PlayerCamera;
        private TimeSpan _exhaustFireBuffer = TimeSpan.Zero;
        public PlayerShip(GraphicsDevice graphicsDevice, Vector2 position, Viewport cameraViewport) {
            Position = position;
            PlayerCamera = new Camera(graphicsDevice, cameraViewport);
            Image = Texture;
        }
        public override void Update(GameTime gameTime) {
            // TODO do nothing if dead
            #region Movement
            const float acceleration = 5f;
            // TODO apply speed powerpacks
            Vector2 direction = Vector2.Zero;
            if (Index == PlayerIndex.One) {
                if (Input.Keyboard.IsKeyDown(Keys.A))
                    direction.X -= 1;
                if (Input.Keyboard.IsKeyDown(Keys.D))
                    direction.X += 1;
                if (Input.Keyboard.IsKeyDown(Keys.W))
                    direction.Y -= 1;
                if (Input.Keyboard.IsKeyDown(Keys.S))
                    direction.Y += 1;
            } else if (Index == PlayerIndex.Two) {
                direction += Input.GamePad.ThumbSticks.Left;
                direction.Y = -direction.Y; // Joystick up = negative Y
                if (Input.GamePad.DPad.Left == ButtonState.Pressed)
                    direction.X -= 1;
                if (Input.GamePad.DPad.Right == ButtonState.Pressed)
                    direction.X += 1;
                if (Input.GamePad.DPad.Up == ButtonState.Pressed)
                    direction.Y -= 1;
                if (Input.GamePad.DPad.Down == ButtonState.Pressed)
                    direction.Y += 1;
            }

            if (direction.LengthSquared() > 1)
                direction.Normalize();
            //direction = direction.Rotate(-camera.Orientation); // TODO workaround?

            Velocity += acceleration * direction * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            Velocity /= 1f + 0.5f * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            Position += Velocity * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            /* Update entity orientation if velocity is non-zero */
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();
            #endregion
            #region Exhaust fire
            _exhaustFireBuffer += gameTime.ElapsedGameTime;
            if (_exhaustFireBuffer > TimeSpan.FromMilliseconds(16)) {
                _exhaustFireBuffer -= TimeSpan.FromMilliseconds(16);
                if (Velocity.LengthSquared() > 0.1) {
                    ParticleTemplates.ExhaustFire(Position, Orientation + MathF.PI);
                }
            }


            /* TODO add exhaust fire
            if (Velocity.LengthSquared() > 0.1f) {
                float orientation = Velocity.ToAngle();
                double t = EtherRoot.CurrentGameTime.TotalGameTime.TotalSeconds;
                Vector2 baseVel = Velocity.ScaleTo(-3);
                Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
                Color sideColor = new Color(200, 38, 9);   // Deep red
                Color midColor = new Color(255, 187, 30);  // Orange-yellow
                Vector2 pos = Position + MathUtil.FromPolar(orientation, -25);
                const float alpha = 0.7f;

                // Middle particle stream
                Vector2 velMid = baseVel + Rand.NextVector2(0, 1);
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LaserGlow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));

                // Side particle streams
                Vector2 vel1 = baseVel + perpVel + Rand.NextVector2(0, 0.3f);
                Vector2 vel2 = baseVel - perpVel + Rand.NextVector2(0, 0.3f);
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LaserGlow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                EtherRoot.ParticleManager.CreateParticle(GlobalAssets.LaserGlow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));
            }
            */
            #endregion
            #region Shooting
            /* TODO add shooting
            if (!EtherRoot.Instance.EditorMode) {
                var aim = Camera.GetMouseAimDirection(Position);
                if (Input.Mouse.LeftButton == ButtonState.Pressed && aim.LengthSquared() > 0 && cooldownRemaining <= 0) {
                    // Play shooting sound
                    Sounds.PlayerShoot.Play(GameSettings.SoundEffectVolume, Rand.NextFloat(-0.2f, 0.2f), 0);
                    // Cooldown calculations
                    float cooldownRemainingMultiplier = 1f;
                    foreach (var power in activePowerPacks) {
                        if (power.PowerType == "ShootSpeedIncrease")
                            cooldownRemainingMultiplier /= 1.3f;
                        else if (power.PowerType == "ShootSpeedDecrease")
                            cooldownRemainingMultiplier *= 1.3f;
                    }
                    cooldownRemaining = (int)((float)CooldownFrames * cooldownRemainingMultiplier);
                    var aimAngle = aim.ToAngle();
                    const int bulletCount = 3;
                    for (var i = 0; i < bulletCount; i++) {
                        var randomSpread = Rand.NextFloat(-0.04f, 0.04f) + Rand.NextFloat(-0.04f, 0.04f);
                        var offsetAngle = aimAngle + MathUtil.Interpolate(-.2f, .2f, i / (bulletCount - 0.999f));
                        var offset = MathUtil.FromPolar(offsetAngle, Rand.NextFloat(15f, 40f));
                        var vel = MathUtil.FromPolar(aimAngle + randomSpread, 18f);
                        Color bulletColor;
                        if (cooldownRemainingMultiplier < 1f)
                            bulletColor = new Color(3, 252, 252); // Baby blue
                        else if (cooldownRemainingMultiplier > 1f)
                            bulletColor = new Color(252, 123, 3); // Orange
                        else
                            bulletColor = new Color(239, 247, 74); // Yellow
                        EntityManager.Add(new Bullet(Position + offset, vel, bulletColor, playerIndex));
                    }
                    // Knockback (dumb)
                    //Camera.CameraPosition += MathUtil.FromPolar(aimangle + MathF.PI, 5f);
                }

                if (cooldownRemaining > 0)
                    cooldownRemaining--;

                if (Input.WasRightMouseJustDown()) {
                    EntityManager.Add(new Starburst(Position, Camera.MouseWorldCoords(), playerIndex));
                }
            }
            */
            #endregion Shooting
            #region Power packs
            /* TODO add powerpacks
            foreach (var powerPack in activePowerPacks) {
                powerPack.framesRemaining--;
                if (powerPack.framesRemaining <= 0)
                    powerPack.isExpended = true;
            }
            activePowerPacks = activePowerPacks.Where(x => !x.isExpended).ToList();
            */
            #endregion Power packs
            /* Update player camera */
            PlayerCamera.Update(gameTime, Position, Index);
        }
    }
}
