using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Mono_Ether {
    public class EnemySpawner {
        private static readonly Random Rand = new Random();
        public static float InverseSpawnChance = 30;
        public static bool Enabled = true;
        private static readonly EnemyType[] SpawnableEnemyTypes = new EnemyType[] { EnemyType.BlueSeeker, EnemyType.PurpleWanderer, EnemyType.GreenSeeker, EnemyType.BackAndForther, EnemyType.PinkSeeker, EnemyType.PinkWanderer, EnemyType.SnakeHead };
        public EnemySpawner() {
            Enabled = true;
        }
        public void Update(EntityManager entityManager, TileMap tileMap) {
            /* Return if enemy spawning is disabled */
            if (!Enabled)
                return;
            /* Spawn every InverseSpawnChance frames */
            if (Rand.Next((int)InverseSpawnChance) != 0)
                return;
            /* Get valid spawn position */
            Vector2 spawnPos;
            var playerIndex = Rand.Next(entityManager.Players.Count);
            var playerPos = entityManager.Players[playerIndex].Position;
            var remainingAttempts = 10;
            const float radius = 500f;
            do {
                spawnPos = new Vector2(Rand.NextFloat(playerPos.X - radius, playerPos.X + radius),
                    Rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
                remainingAttempts -= 1;
            }
            while ((Vector2.DistanceSquared(spawnPos, playerPos) < Math.Pow(radius / 2f, 2)
                     || tileMap.GetTileFromMap(tileMap.WorldtoMap(spawnPos)).Id > 0
                     || spawnPos.X < 0 || spawnPos.Y < 0 || spawnPos.X > tileMap.WorldSize.X || spawnPos.Y > tileMap.WorldSize.Y)
                    && remainingAttempts > 0);

            if (remainingAttempts == 0) {
                Debug.WriteLine("Skipping enemy spawn");
                return;
            }

            var enemyType = SpawnableEnemyTypes[Rand.Next(SpawnableEnemyTypes.Length)];
            entityManager.Add(Enemy.CreateEnemy(enemyType, spawnPos));
        }
    }
}
