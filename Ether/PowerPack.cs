using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether
{
    class PowerPack : Entity
    {
        private int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        public string PowerType;
        private readonly Random rand = new Random();
        private PowerPack(Texture2D image, Vector2 position, string powerType)
        {
            Image = image;
            Position = position;
            Radius = image.Width / 2f;
            Color = Color.Transparent;
            PowerType = powerType;
        }

        public override void Update()
        {
            if (timeUntilStart > 0)
            {
                timeUntilStart--;
                Color = Color.White * (1 - timeUntilStart / 60f);
            }
        }

        public void WasPickedUp()
        {
            // TODO: probably some particle animations or something idk lmao
            Art.PowerPackPickup.CreateInstance().Play();
            for (var i = 0; i < 50; i++)
            {
                var speed = 30f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = new Color(100, 200, 0);
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 10f, state);
            }
        }

        public static PowerPack CreateShootSpeedIncrease(Vector2 position)
        {
            return new PowerPack(Art.PowerShootSpeedIncrease, position, "ShootSpeedIncrease");
        }
    }
    static class PowerPackSpawner
    {
        static Random _rand = new Random();
        static float _inverseSpawnChance = 50;
        public static bool enabled = true;
        private const int numTypes = 1;
        public static void Update()
        {
            if (!enabled)
                return;

            if (!PlayerShip.Instance.IsDead && EntityManager.PowerPacks.Count < 3)
            {
                if (_rand.Next((int)_inverseSpawnChance) != 0)
                    return;
                var pos = EnemySpawner.GetSpawnPosition();
                if (pos == Vector2.Zero)
                    return;

                int powerTypeInt = _rand.Next(0, numTypes);
                switch (powerTypeInt)
                {
                    case (0):
                        EntityManager.Add(PowerPack.CreateShootSpeedIncrease(pos));
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
