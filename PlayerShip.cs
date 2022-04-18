using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    public class PlayerShip : Entity {
        private static Texture2D _heart;
        private static Texture2D _shipTexture;
        public static SoundEffect ShotSoundEffect;
        private static SoundEffect _deathSound;
        public PlayerIndex Index;
        public int Lives = 3;
        public readonly Camera PlayerCamera;
        private TimeSpan _shotCooldownRemaining = TimeSpan.Zero;
        private readonly TimeSpan _shotCooldown = TimeSpan.FromSeconds(0.1);
        public float Multiplier = 1;
        public int Score;
        public int Geoms;
        public bool DoomMode;
        private TimeSpan _exhaustFireBuffer = TimeSpan.Zero;
        private int _framesUntilRespawn;
        public List<PowerPack> ActivePowerPacks = new List<PowerPack>();
        private static readonly Random Rand = new Random();
        public bool IsDead => _framesUntilRespawn > 0;
        public PlayerShip(GraphicsDevice graphicsDevice, Vector2 position, Viewport cameraViewport) {
            Position = position;
            PlayerCamera = new Camera(graphicsDevice, cameraViewport);
            Image = _shipTexture;
        }
        public static void LoadContent(ContentManager content) {
            _heart = content.Load<Texture2D>("Textures/GameScreen/Heart");
            _shipTexture = content.Load<Texture2D>("Textures/GameScreen/PlayerShip");
            ShotSoundEffect = content.Load<SoundEffect>("SoundEffects/PlayerShoot/Shoot-01");
            _deathSound = content.Load<SoundEffect>("SoundEffects/PlayerDeath");
        }
        public static void UnloadContent() {
            _heart = null;
            _shipTexture = null;
            ShotSoundEffect = null;
            _deathSound = null;
        }
        public override void Update(GameTime gameTime) {
            if (IsDead) {
                _framesUntilRespawn--;
                if (_framesUntilRespawn == 0)
                    if (GameScreen.Instance.CurrentLevel == Level.Level1 ||
                        GameScreen.Instance.CurrentLevel == Level.Level2 ||
                        GameScreen.Instance.CurrentLevel == Level.Level3)
                        Position = new Vector2(128f, 128f);
                return;
            }
            #region Movement
            var direction = Vector2.Zero;
            var acceleration = 3f;
            if (DoomMode) {
                // A and D will turn the player left or right, W and S will move the player forward and backward.
                if (Input.Keyboard.IsKeyDown(Keys.A))
                    Orientation -= 0.05f;
                if (Input.Keyboard.IsKeyDown(Keys.D))
                    Orientation += 0.05f;
                if (Input.Keyboard.IsKeyDown(Keys.W))
                    Position += Vector2.UnitX.Rotate(Orientation) * 5f;
                if (Input.Keyboard.IsKeyDown(Keys.S))
                    Position -= Vector2.UnitX.Rotate(Orientation) * 5f;
                // If the player collides with a "yellow block" then return to the previous screen (the normal game screen)
                if (TileMap.Instance.GetTileFromWorld(Position).Id == 4) ScreenManager.RemoveScreen();
            } else {
                // If the player has any move speed increase/decrease power packs then update the acceleration variable accordingly
                foreach (var power in ActivePowerPacks) {
                    if (power.Type == PowerPackType.MoveSpeedIncrease)
                        acceleration *= 3f;
                    else if (power.Type == PowerPackType.MoveSpeedDecrease)
                        acceleration /= 3f;
                }

                switch (Index) {
                    case PlayerIndex.One: {
                            // Player one moves using WASD
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
                            // Player two moves using IJKL or a controller
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
            }
            // Cap the direction length to 1
            if (direction.LengthSquared() > 1)
                direction.Normalize();
            // If the camera is rotated, rotate the direction vector so moving left, for example, still moves the player leftwards relative to user's view
            direction = direction.Rotate(-PlayerCamera.Orientation);
            // Add acceleration times direction to the current player velocity
            Velocity += acceleration * direction;
            // Cap the player velocity
            if (Velocity.LengthSquared() > 144) Velocity = Velocity.ScaleTo(12f);
            // Apply friction
            Velocity /= 1f + 0.05f * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            // Update position based on velocity
            Position += Velocity * (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
            /* Update entity orientation if velocity is non-zero */
            if (!DoomMode && Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();
            // If the player is inside a solid tile, get them out of there
            HandleTilemapCollision();
            #endregion
            #region Exhaust fire
            // If the player is moving in a direction, send exhaust fire in that direction
            _exhaustFireBuffer += gameTime.ElapsedGameTime;
            if (_exhaustFireBuffer > TimeSpan.FromMilliseconds(16)) {
                _exhaustFireBuffer -= TimeSpan.FromMilliseconds(16);
                if (direction.LengthSquared() >= 0.1f) {
                    for (int i = 0; i < 3; i++)
                        ParticleTemplates.ExhaustFire(Position, Velocity.ToAngle() + MathF.PI, new Color(0.2f, 0.8f, 1f));
                }
            }
            #endregion
            #region Shooting
            // Only shoot if the game is in playing mode (not paused or in editor mode)
            if (GameScreen.Instance.Mode == GameMode.Playing) {
                var aim = PlayerCamera.MouseWorldCoords() - Position; // Arbitrary magnitude
                // If the player is holding left mouse button and the cooldown is zero, shoot a bullet
                if (Input.Mouse.LeftButton == ButtonState.Pressed && aim.LengthSquared() > 0 && _shotCooldownRemaining <= TimeSpan.Zero) {
                    /* Play shooting sound */
                    ShotSoundEffect.Play(GameSettings.SoundEffectVolume, Rand.NextFloat(-0.2f, 0.2f), 0);
                    // Reset the cooldown
                    _shotCooldownRemaining = _shotCooldown;
                    // Get the angle of the vector from the player to the cursor
                    var aimAngle = aim.ToAngle();
                    // Shoot three bullets per shot
                    const int bulletCount = 3;
                    for (var i = 0; i < bulletCount; i++) {
                        // Each bullet moves in a slightly random angle to simulate bullet spray
                        var randomSpread = Rand.NextFloat(-0.04f, 0.04f) + Rand.NextFloat(-0.04f, 0.04f);
                        // We offset the bullet's initial position slightly in front of the player, not from the exact centre of the player
                        var offsetAngle = aimAngle + MathHelper.Lerp(-.2f, .2f, i / (bulletCount - 0.999f));
                        var offset = MyUtils.FromPolar(offsetAngle, Rand.NextFloat(15f, 40f));
                        // Get the velocity of the bullet based on the angle with a constant speed of 18 units per second
                        var vel = MyUtils.FromPolar(aimAngle + randomSpread, 18f);
                        var bulletColor = new Color(239, 247, 74);
                        // Add this bullet to the list of bullets
                        EntityManager.Instance.Add(new Bullet(Position + offset, vel, bulletColor, Index));
                    }
                }
                /* Decrement cooldown */
                if (_shotCooldownRemaining > TimeSpan.Zero) {
                    var cooldownRemainingMultiplier = 1f;
                    foreach (var power in ActivePowerPacks) {
                        // If the user has a shoot speed increase or decrease power pack, update the cooldown accordingly
                        if (power.Type == PowerPackType.ShootSpeedIncrease)
                            cooldownRemainingMultiplier *= 1.3f;
                        else if (power.Type == PowerPackType.ShootSpeedDecrease)
                            cooldownRemainingMultiplier /= 1.3f;
                    }
                    _shotCooldownRemaining -= gameTime.ElapsedGameTime * cooldownRemainingMultiplier;
                }
                /* Right click to summon a starburst */
                if (Input.WasRightMouseJustDown)
                EntityManager.Instance.Add(new StarBurst(Position, PlayerCamera.MouseWorldCoords(), Index));
            }
            #endregion Shooting
            #region Power packs
            // Remove any powerpacks that have been fully used ups
            foreach (var powerPack in ActivePowerPacks) {
                powerPack.TimeRemaining -= gameTime.ElapsedGameTime;
                if (powerPack.TimeRemaining <= TimeSpan.Zero)
                    powerPack.IsExpended = true;
            }
            ActivePowerPacks = ActivePowerPacks.Where(x => !x.IsExpended).ToList();
            #endregion Power packs
            /* Update player camera */
            PlayerCamera.Update(gameTime, Position, Index);
        }
        public void Kill() {
            // Summon an explosion at the player's position
            ParticleTemplates.Explosion(Position, 5f, 25f, 300, Color.White, true);
            // Make it so the player won't respawn for another second
            _framesUntilRespawn = 60;
            // Decrement number of lives
            Lives--;
            // If the player has run out of lives, they have lost the game
            if (Lives < 0) {
                _framesUntilRespawn = 99999;
                Hud.Instance.ChangeStatus(HudStatus.Lose);
            }
            /* Reset velocity */
            Velocity = Vector2.Zero;
            ActivePowerPacks.Clear();
            // save highscore TODO
            if (Score > HighScore.Score)
                HighScore.Score = Score;
            // Play death sound
            _deathSound.Play(GameSettings.SoundEffectVolume, 0f, 0f);

        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            if (IsDead)
                return;
            base.Draw(batch, camera);
        }
        public void DrawHud(SpriteBatch batch) {
            for (int i = 0; i < ActivePowerPacks.Count; i++) {
                var pos = new Vector2(PlayerCamera.ScreenSize.X - 100 - i * 100, PlayerCamera.ScreenSize.Y - 100);
                var powerPack = ActivePowerPacks[i];
                var icon = powerPack.Type switch {
                    PowerPackType.ShootSpeedIncrease => PowerPack._shootSpeedIncreaseTexture,
                    PowerPackType.ShootSpeedDecrease => PowerPack._shootSpeedDecreaseTexture,
                    PowerPackType.MoveSpeedIncrease => PowerPack._moveSpeedIncreaseTexture,
                    PowerPackType.MoveSpeedDecrease => PowerPack._moveSpeedDecreaseTexture,
                    PowerPackType.Doom => PowerPack._doomTexture,
                    _ => GlobalAssets.Default,
                };
                // Draw time remaining colored background
                var backgroundColor = new Color(60, 214, 91);
                if (!powerPack.IsGood)
                    backgroundColor = new Color(214, 60, 60);
                var remaining = (float)(powerPack.TimeRemaining / powerPack.InitialTime);
                batch.Draw(GlobalAssets.Pixel, new Rectangle((int)pos.X, (int)(pos.Y + 96 * (1 - remaining)), 96, (int)(96 - 96 * (1 - remaining))), backgroundColor);
                // Draw icon
                batch.Draw(icon, pos, Color.White);
            }

            for (int i = 0; i < Lives; i++) {
                var pos = new Vector2(25 + i * 100, PlayerCamera.ScreenSize.Y - 100);
                batch.Draw(_heart, pos, Color.White);
            }
            batch.DrawString(GlobalAssets.NovaSquare24, $"Score: {Score}", new Vector2(PlayerCamera.ScreenSize.X - 200, 30), Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Highscore: {HighScore.Score}", new Vector2(PlayerCamera.ScreenSize.X - 300, 60), Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Multi: {Multiplier}", new Vector2(PlayerCamera.ScreenSize.X - 200, 90), Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Geoms: {Geoms}", new Vector2(PlayerCamera.ScreenSize.X - 200, 120), Color.White);
        }
        public void AddGeoms(int amount) {
            Geoms += amount;
        }
    }
}
