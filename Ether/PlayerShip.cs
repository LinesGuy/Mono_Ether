using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mono_Ether.Ether {
    class PlayerShip : Entity {
        public int lives;
        public int Score { get; private set; }
        public int Geoms { get; private set; }
        public int Multiplier { get; private set; }
        public int playerIndex;
        public int HighScore { get; private set; }
        private static readonly string highScoreFilename = "highscore.txt";

        public PlayerShip() {
            Image = Art.Player;
            Position = new Vector2(0, 0);
            Radius = 10;
            Geoms = 0;
            Multiplier = 1;
            lives = 3;
            Score = 0;
            HighScore = LoadHighScore();
            playerIndex = EntityManager.Players.Count;
        }

        const int CooldownFrames = 6;
        int cooldownRemaining;
        static readonly Random Rand = new Random();
        private readonly bool autoFire = false;  // If true, hold left click to stop fire
        int framesUntilRespawn;

        public List<PowerPack> activePowerPacks = new List<PowerPack>();
        public bool IsDead => framesUntilRespawn > 0;

        public override void Update() {
            // Do nothing if dead
            if (IsDead) {
                framesUntilRespawn--;
                return;
            }
            #region Movement
            float acceleration = 5;
            foreach (var power in activePowerPacks) {
                if (power.PowerType == "MoveSpeedIncrease")
                    acceleration *= 1.3f;
                else if (power.PowerType == "MoveSpeedDecrease")
                    acceleration /= 1.3f;
            }
            Vector2 direction = Vector2.Zero;
            if (this == EntityManager.Player1) {
                if (Input.keyboard.IsKeyDown(Keys.A))
                    direction.X -= 1;
                if (Input.keyboard.IsKeyDown(Keys.D))
                    direction.X += 1;
                if (Input.keyboard.IsKeyDown(Keys.W))
                    direction.Y -= 1;
                if (Input.keyboard.IsKeyDown(Keys.S))
                    direction.Y += 1;
            }
            if (direction.LengthSquared() > 1)
                direction.Normalize();
            direction = direction.Rotate(-Camera.Orientation);

            Velocity += acceleration * direction;  // Normalised direction vector
            Velocity /= 1.5f;  // Friction
            Position += Velocity;
            // Change orientation if velocity is non-zero:
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            HandleTilemapCollision();
            #endregion Movement
            #region Exhaust fire
            if (Velocity.LengthSquared() > 0.1f) {
                Orientation = Velocity.ToAngle();
                double t = EtherRoot.CurrentGameTime.TotalGameTime.TotalSeconds;
                Vector2 baseVel = Velocity.ScaleTo(-3);
                Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
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
            #endregion Exhaust fire
            #region Shooting
            if (!EtherRoot.Instance.editorMode) {
                var aim = Camera.GetAimDirection();
                if ((autoFire ^ Input.mouse.LeftButton == ButtonState.Pressed) && aim.LengthSquared() > 0 && cooldownRemaining <= 0) {
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
                    var aimangle = aim.ToAngle();
                    const int bulletCount = 3;
                    for (var i = 0; i < bulletCount; i++) {
                        var randomSpread = Rand.NextFloat(-0.04f, 0.04f) + Rand.NextFloat(-0.04f, 0.04f);
                        var offsetAngle = aimangle + MathUtil.Interpolate(-.2f, .2f, i / (bulletCount - 0.999f));
                        var offset = MathUtil.FromPolar(offsetAngle, Rand.NextFloat(15f, 40f));
                        var vel = MathUtil.FromPolar(aimangle + randomSpread, 18f);
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

                if (Input.WasRightButtonJustDown()) {
                    EntityManager.Add(new Starburst(Position, Camera.MouseWorldCoords(), playerIndex));
                }
            }
            #endregion Shooting
            #region Power packs
            foreach (var powerPack in activePowerPacks) {
                powerPack.framesRemaining--;
                if (powerPack.framesRemaining <= 0)
                    powerPack.isExpended = true;
            }
            activePowerPacks = activePowerPacks.Where(x => !x.isExpended).ToList();
            #endregion Power packs
        }
        public override void Draw(SpriteBatch spriteBatch) {
            if (!IsDead) {
                base.Draw(spriteBatch);
            }
        }
        public void AddPoints(int basePoints) {
            if (IsDead)
                return;
            Score += basePoints * Multiplier;
        }
        public void AddGeoms(int amount) {
            if (IsDead)
                return;
            Geoms += amount;
            Multiplier += amount;
        }
        public void Kill() {
            framesUntilRespawn = 60;
            Sounds.PlayerDeath.CreateInstance().Play();
            lives -= 1;
            if (lives < 0) {
                framesUntilRespawn = 99999;
                EnemySpawner.enabled = false;
                PowerPackSpawner.enabled = false;
                EtherRoot.hud.playingYouDied = true;
            }

            activePowerPacks = new List<PowerPack>();

            for (int i = 0; i < 1200; i++) {
                float speed = 18f * (1f - 1 / Rand.NextFloat(1f, 10f));
                Color color = Color.Lerp(Color.White, Color.Yellow, Rand.NextFloat(0, 1));
                var state = new ParticleState() {
                    Velocity = Rand.NextVector2(speed, speed),
                    Type = ParticleType.None,
                    LengthMultiplier = 1
                };
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }

            EnemySpawner.Reset();

            if (Score > HighScore)
                SaveHighScore(Score);

            Multiplier = 1;
        }
        private int LoadHighScore() {
            // Return saved score if it exists, or return 0 if there is none
            return File.Exists(highScoreFilename) && int.TryParse(File.ReadAllText(highScoreFilename), out int score) ? score : 0;
        }
        private void SaveHighScore(int score) {
            // Saves the score to the highscore file, note that this does not check the saved score is greater than the new score.
            File.WriteAllText(highScoreFilename, score.ToString());
        }
    }
}
