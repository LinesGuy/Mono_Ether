using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mono_Ether {
    public class EnemySpawner
    {
        private static readonly Random _rand = new Random();
        public static float InverseSpawnChance = 60;
        public static bool Enabled = true;
        public void Update(EntityManager entityManager, TileMap tileMap)
        {
            if (!Enabled) return;
            /* Get valid spawn position */
            Vector2 spawnPos;
            var playerIndex = _rand.Next(entityManager.Players.Count);
            var playerPos = entityManager.Players[playerIndex].Position;
            var remainingAttempts = 10;
            const float radius = 500f;
            do
            {
                spawnPos = new Vector2(_rand.NextFloat(playerPos.X - radius, playerPos.X + radius),
                    _rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
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
            var enemyType = (EnemyType)enemyTypes.GetValue(_rand.Next(enemyTypes.Length))!;
            entityManager.Add(Enemy.CreateEnemy(enemyType, spawnPos));
        }
    }
}
