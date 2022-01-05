using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mono_Ether {
    public class EnemySpawner
    {
        private static readonly Random Rand = new Random();
        public static float InverseSpawnChance = 30;
        public static bool Enabled = true;
        public void Update(EntityManager entityManager, TileMap tileMap)
        {
            /* Return if enemy spawning is disabled */
            if (!Enabled) return;
            /* Spawn every InverseSpawnChance frames */
            if (Rand.Next((int)InverseSpawnChance) != 0) return;
            /* Get valid spawn position */
            Vector2 spawnPos;
            var playerIndex = Rand.Next(entityManager.Players.Count);
            var playerPos = entityManager.Players[playerIndex].Position;
            var remainingAttempts = 10;
            const float radius = 500f;
            do
            {
                spawnPos = new Vector2(Rand.NextFloat(playerPos.X - radius, playerPos.X + radius),
                    Rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
                remainingAttempts -= 1;
            }
            while ((Vector2.DistanceSquared(spawnPos, playerPos) < Math.Pow(radius / 2f, 2)
                     || tileMap.GetTileFromMap(tileMap.WorldtoMap(spawnPos)).Id > 0
                     || spawnPos.X < 0 || spawnPos.Y < 0 || spawnPos.X > tileMap.WorldSize.X || spawnPos.Y > tileMap.WorldSize.Y)
                    && remainingAttempts > 0);

            if (remainingAttempts == 0)
            {
                Debug.WriteLine("Skipping enemy spawn");
                return;
            }

            var enemyTypes = Enum.GetValues(typeof(EnemyType));
            var enemyType = (EnemyType)enemyTypes.GetValue(Rand.Next(enemyTypes.Length))!;
            entityManager.Add(Enemy.CreateEnemy(enemyType, spawnPos));
        }
    }
}
