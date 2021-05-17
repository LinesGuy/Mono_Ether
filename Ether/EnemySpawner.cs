using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.Ether
{
    static class EnemySpawner
    {
        static Random rand = new Random();
        static float inverseSpawnChance = 60;
        static bool enabled = true;
        public static void Update()
        {
            if (!enabled)
                return;

            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
            {
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
            }

            // Slowly increase spawn rate as time progresses
            if (inverseSpawnChance > 20)
                inverseSpawnChance -= 0.005f;
        }

        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos;
            Vector2 playerPos = PlayerShip.Instance.Position;
            do
            {
                pos = new Vector2(rand.NextFloat(playerPos.X - 500, playerPos.X + 500), rand.NextFloat(playerPos.Y - 500, playerPos.Y + 500)); ;
            }
            while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

            return pos;
        }

        public static void Reset()
        {
            inverseSpawnChance = 60;
        }
    }
}
