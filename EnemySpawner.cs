using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Mono_Ether {
    public class EnemySpawner {
        private static readonly Random Rand = new Random();
        public static float InverseSpawnChance = 30f;
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
            // Get the position of a random player. Doesn't matter which one, as long as we spawn an enemy close to any player.
            var playerIndex = Rand.Next(entityManager.Players.Count);
            var playerPos = entityManager.Players[playerIndex].Position;
            // Try to spawn an any a set number of times. If we fail this many times, just don't spawn an enemy.
            // Realistically it should never take more than a few attempts to find a valid spawn position, but if the player
            // is in a position where it is impossible to spawn any enemies (e.g outside of the map) then this prevents a never-ending loop.
            var remainingAttempts = 10;
            // The maximum distance the enemy will spawn from the player:
            const float radius = 500f;
            // Here we use a do-while loop solely to annoy Matthew, who hates it when I use a do-while loop
            do {
                // Get a random position
                spawnPos = new Vector2(Rand.NextFloat(playerPos.X - radius, playerPos.X + radius),
                    Rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
                remainingAttempts -= 1;
            } //.. while the current position in invalid
            while ((Vector2.DistanceSquared(spawnPos, playerPos) < Math.Pow(radius / 2f, 2)
                     || tileMap.GetTileFromMap(tileMap.WorldtoMap(spawnPos)).Id > 0
                     || spawnPos.X < 0 || spawnPos.Y < 0 || spawnPos.X > tileMap.WorldSize.X || spawnPos.Y > tileMap.WorldSize.Y)
                    && remainingAttempts > 0);

            if (remainingAttempts == 0) {
                Debug.WriteLine("Skipping enemy spawn");
                return;
            }
            // Set the type of the enemy to some random spawnable enemy type.
            var enemyType = SpawnableEnemyTypes[Rand.Next(SpawnableEnemyTypes.Length)];
            entityManager.Add(Enemy.CreateEnemy(enemyType, spawnPos));
            if (InverseSpawnChance > 15)
                InverseSpawnChance -= 0.005f; // Slowly increase spawn rate as time progresses
        }
    }
}
