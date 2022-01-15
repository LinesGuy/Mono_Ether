using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace Mono_Ether {
    public class PlayerShip : Entity {
        private static Texture2D _shipTexture;
        private static SoundEffect _shotSoundEffect; // TODO array, also move to bullet.cs?
        private const string HighScoreFilename = "highscore.txt";
        public PlayerIndex Index;
        public int Lives = 3;
        public readonly Camera PlayerCamera;
        private int _shotCooldownRemaining;
        private const int ShotCooldown = 6;
        public float Multiplier = 1;
        public int Geoms;
        private TimeSpan _exhaustFireBuffer = TimeSpan.Zero;
        private int _framesUntilRespawn;
        private static readonly Random Rand = new Random();
        public bool IsDead => _framesUntilRespawn > 0;
        public PlayerShip(GraphicsDevice graphicsDevice, Vector2 position, Viewport cameraViewport) {
            Position = position;
            PlayerCamera = new Camera(graphicsDevice, cameraViewport);
            Image = _shipTexture;
        }
        public static void LoadContent(ContentManager content) {
            _shipTexture = content.Load<Texture2D>("Textures/GameScreen/PlayerShip");
            _shotSoundEffect = content.Load<SoundEffect>("SoundEffects/PlayerShoot/Shoot-01");
        }
        public static void UnloadContent() {
            _shipTexture = null;
            _shotSoundEffect = null;
        }
        public override void Update(GameTime gameTime) {
            if (IsDead) {
                _framesUntilRespawn--;
                return;
            }
            #region Movement
            const float acceleration = 3f;
            // TODO apply speed powerpacks
            var direction = Vector2.Zero;
            switch (Index) {
                case PlayerIndex.One: {
                        if (Input.Keyboard.IsKeyDown(Keys.A))
                            direction.X -= 1;
                        if (Input.Keyboard.IsKeyDown(Keys.D))
                            direction.X += 1;
                        if (Input.Keyboard.IsKeyDown(Keys.W))
                            direction.Y -= 1;
                        if (Input.Keyboard.IsKeyDown(Keys.S))
                            direction.Y += 1;
                        break;
                    }
                case PlayerIndex.Two: {
                        direction += Input.GamePad.ThumbSticks.Left;
                        direction.Y = -direction.Y; // Joystick up = negative Y
                        if (Input.GamePad.DPad.Left == ButtonState.Pressed || Input.Keyboard.IsKeyDown(Keys.J))
                            direction.X -= 1;
                        if (Input.GamePad.DPad.Right == ButtonState.Pressed || Input.Keyboard.IsKeyDown(Keys.L))
                            direction.X += 1;
                        if (Input.GamePad.DPad.Up == ButtonState.Pressed || Input.Keyboard.IsKeyDown(Keys.I))
                            direction.Y -= 1;
                        if (Input.GamePad.DPad.Down == ButtonState.Pressed || Input.Keyboard.IsKeyDown(Keys.K))
                            direction.Y += 1;
                        break;
                    }
            }

            if (direction.LengthSquared() > 1)
                direction.Normalize();
            direction = direction.Rotate(-PlayerCamera.Orientation);

            Velocity += acceleration * direction;
            if (Velocity.LengthSquared() > 144) Velocity = Velocity.ScaleTo(12f);
            Velocity /= 1f + 0.05f * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            Position += Velocity * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            /* Update entity orientation if velocity is non-zero */
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();
            HandleTilemapCollision();
            #endregion
            #region Exhaust fire
            _exhaustFireBuffer += gameTime.ElapsedGameTime;
            if (_exhaustFireBuffer > TimeSpan.FromMilliseconds(16)) {
                _exhaustFireBuffer -= TimeSpan.FromMilliseconds(16);
                if (direction.LengthSquared() >= 0.1f) {
                    for (int i = 0; i < 3; i++)
                        ParticleTemplates.ExhaustFire(Position, Velocity.ToAngle() + MathF.PI);
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
            if (GameScreen.Instance.Mode == GameMode.Playing) {
                var aim = PlayerCamera.MouseWorldCoords() - Position; // ARBITRARY MAGNITUDE, scale later
                if (Input.Mouse.LeftButton == ButtonState.Pressed && aim.LengthSquared() > 0 && _shotCooldownRemaining == 0) {
                    /* Play shooting sound */
                    _shotSoundEffect.Play(GameSettings.SoundEffectVolume, Rand.NextFloat(-0.2f, 0.2f), 0);
                    /* TODO Cooldown calculations
                    float cooldownRemainingMultiplier = 1f;
                    foreach (var power in activePowerPacks) {
                        if (power.PowerType == "ShootSpeedIncrease")
                            cooldownRemainingMultiplier /= 1.3f;
                        else if (power.PowerType == "ShootSpeedDecrease")
                            cooldownRemainingMultiplier *= 1.3f;
                    }
                    cooldownRemaining = (int)((float)CooldownFrames * cooldownRemainingMultiplier);
                    */
                    _shotCooldownRemaining = ShotCooldown; // TODO replace with above
                    _shotCooldownRemaining--;
                    var aimAngle = aim.ToAngle();
                    const int bulletCount = 3;
                    for (var i = 0; i < bulletCount; i++) {
                        var randomSpread = Rand.NextFloat(-0.04f, 0.04f) + Rand.NextFloat(-0.04f, 0.04f);
                        var offsetAngle = aimAngle + MathHelper.Lerp(-.2f, .2f, i / (bulletCount - 0.999f));
                        var offset = MyUtils.FromPolar(offsetAngle, Rand.NextFloat(15f, 40f));
                        var vel = MyUtils.FromPolar(aimAngle + randomSpread, 18f);
                        var bulletColor = new Color(239, 247, 74);
                        /* TODO base bullet color from cooldown remaining multiplier
                        if (cooldownRemainingMultiplier < 1f)
                            bulletColor = new Color(3, 252, 252); // Baby blue
                        else if (cooldownRemainingMultiplier > 1f)
                            bulletColor = new Color(252, 123, 3); // Orange
                        else
                            bulletColor = new Color(239, 247, 74); // Yellow
                        */
                        EntityManager.Instance.Add(new Bullet(Position + offset, vel, bulletColor, Index));
                    }
                    // Knockback (dumb)
                    //Camera.CameraPosition += MathUtil.FromPolar(aimangle + MathF.PI, 5f);
                }
                /* Decrement cooldown */
                if (_shotCooldownRemaining > 0)
                    _shotCooldownRemaining--;
                /* Right click to summon a starburst */
                if (Input.WasRightMouseJustDown)
                    EntityManager.Instance.Add(new Starburst(Position, PlayerCamera.MouseWorldCoords(), Index));
            }
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

        public void Kill() {
            ParticleTemplates.Explosion(Position, 5f, 20f, 100);
            _framesUntilRespawn = 60;
            Lives--;
            if (Lives < 0) {
                _framesUntilRespawn = 99999;
                // transition to death
            }
            // reset powerpacks
            // save highscore
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            if (IsDead)
                return;
            base.Draw(batch, camera);
        }
        private int LoadHighScore() {
            // Return saved score if it exists, or return 0 if there is none
            return File.Exists(HighScoreFilename) && int.TryParse(File.ReadAllText(HighScoreFilename), out int score) ? score : 0;
        }
        private void SaveHighScore(int score) {
            // Saves the score to the highscore file, note that this does not check the saved score is greater than the new score.
            File.WriteAllText(HighScoreFilename, score.ToString());
        }
    }
}
