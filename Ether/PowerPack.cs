using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    class PowerPack : Entity {
        private int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        public string PowerType;
        public int initialFramesRemaining; // Used for drawing time remaining on the hud
        public int framesRemaining;
        public int lifeSpan; // When framesExisted is greater than this, the powerup will expire
        public int framesExisted;
        public bool isExpended; // When the player uses up the power
        public bool isGood; // true = speed increase etc, false = speed decrease etc
        private readonly Random rand = new Random();
        public PowerPack(Texture2D image, Vector2 position, string powerType, int duration) {
            Image = image;
            Position = position;
            Radius = image.Width / 2f;
            Color = Color.Transparent;
            PowerType = powerType;
            initialFramesRemaining = duration;
            framesRemaining = initialFramesRemaining;
            if (powerType == "MoveSpeedDecrease" || powerType == "ShootSpeedDecrease")
                isGood = false;
            else
                isGood = true;
            if (isGood)
                lifeSpan = 1200; // 20 seconds
            else
                lifeSpan = 600; // 10 seconds

        }

        public override void Update() {
            // Fade-in powerup on spawn
            if (timeUntilStart > 0) {
                timeUntilStart--;
                Color = Color.White * (1 - timeUntilStart / 60f);
            }
            // If powerup is uncollected for lifeSpan frames, powerup expires
            framesExisted++;
            if (framesExisted >= lifeSpan)
                IsExpired = true;
        }

        public void WasPickedUp() {
            if (isGood)
                Sounds.PowerPackPickup.CreateInstance().Play();
            else
                Sounds.PowerPackPickupBad.CreateInstance().Play();
            for (var i = 0; i < 50; i++) {
                var speed = 30f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState() {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };
                Color color = new Color(100, 200, 0); // Green
                if (!isGood)
                    color = new Color(200, 100, 0); // Red
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 10f, state);
            }
        }
    }
    static class PowerPackSpawner {
        static readonly Random _rand = new Random();
        static readonly float _inverseSpawnChance = 50;
        public static bool enabled = true;
        public static void Update() {
            if (!enabled)
                return;

            if (EntityManager.Players.TrueForAll(p => !p.IsDead) && EntityManager.PowerPacks.Count < 3) {
                if (_rand.Next((int)_inverseSpawnChance) != 0)
                    return;
                var pos = EnemySpawner.GetSpawnPosition(2500f, 20);
                if (pos == Vector2.Zero)
                    return;

                int powerTypeInt = _rand.Next(4, 5);
                switch (powerTypeInt) {
                    case (0): // ShootSpeedIncrease
                        EntityManager.Add(new PowerPack(Art.PowerShootSpeedIncrease, pos, "ShootSpeedIncrease", 300));
                        break;
                    case (1): // ShootSpeedDecrease
                        EntityManager.Add(new PowerPack(Art.PowerShootSpeedDecrease, pos, "ShootSpeedDecrease", 300));
                        break;
                    case (2): // MoveSpeedIncrease
                        EntityManager.Add(new PowerPack(Art.PowerMoveSpeedIncrease, pos, "MoveSpeedIncrease", 300));
                        break;
                    case (3): // MoveSpeedDecrease
                        EntityManager.Add(new PowerPack(Art.PowerMoveSpeedDecrease, pos, "MoveSpeedDecrease", 300));
                        break;
                    case (4): // Doom
                        EntityManager.Add(new PowerPack(Art.PowerSecret, pos, "Doom", 1800));
                        break;
                    default:
                        // this shouldn't happen
                        Debug.WriteLine("PowerPack.cs powerTypeInt was unhandled");
                        break;
                }
            }
        }
    }
}
