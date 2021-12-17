using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    /*
    static class EnemySpawner {
        static readonly Random _rand = new Random();
        static float _inverseSpawnChance = 60;
        public static bool enabled = true;
        public static void Update() {
            if (!enabled)
                return;

            if (EntityManager.Players.TrueForAll(p => !p.IsDead) && EntityManager.Count < 200) {
                if (_rand.Next((int)_inverseSpawnChance) != 0)
                    return;

                var pos = GetSpawnPosition();
                if (pos == Vector2.Zero)
                    return; // Couldn't find valid spawn position

                switch (_rand.Next(7)) {
                    case (0):
                        EntityManager.Add(Enemy.CreateBlueSeeker(pos));
                        break;
                    case (1):
                        EntityManager.Add(Enemy.CreatePurpleWanderer(pos));
                        break;
                    case (2):
                        EntityManager.Add(Enemy.CreateSnake(pos));
                        break;
                    case (3):
                        EntityManager.Add(Enemy.CreateBackAndForther(pos));
                        break;
                    case (4):
                        EntityManager.Add(Enemy.CreatePinkWanderer(pos));
                        break;
                    case (5):
                        EntityManager.Add(Enemy.CreateGreenSeeker(pos));
                        break;
                    case (6):
                        EntityManager.Add(Enemy.CreatePinkSeeker(pos));
                        break;
                    default:
                        Debug.WriteLine("ur dum");
                        break;
                }
            }

            // Slowly increase spawn rate as time progresses
            if (_inverseSpawnChance > 20)
                _inverseSpawnChance -= 0.005f;
        }

        public static Vector2 GetSpawnPosition(float radius = 500f, int attempts = 10) {
            // If returns Vector2.Zero, could not find valid spawn position
            Vector2 pos;
            int randIndex = _rand.Next(EntityManager.Players.Count);
            Vector2 playerPos = EntityManager.Players[randIndex].Position; // Pos of random player
            int remainingAttempts = attempts;
            do {
                pos = new Vector2(_rand.NextFloat(playerPos.X - radius, playerPos.X + radius), _rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
                remainingAttempts -= 1;
            }
            while ((Vector2.DistanceSquared(pos, playerPos) < Math.Pow(radius / 2f, 2)
                   || Map.GetTileFromMap(Map.WorldtoMap(pos)).TileId > 0
                   || pos.X < 0 || pos.Y < 0 || pos.X > Map._size.X * Map.cellSize || pos.Y > Map._size.Y * Map.cellSize)
                   && remainingAttempts > 0);
            if (remainingAttempts == 0) {
                Debug.WriteLine($"Could not find spawn position after {attempts} attempts, skipping");
                return Vector2.Zero;
            }
            return pos;
        }

        public static void Reset() {
            _inverseSpawnChance = 60;
        }
    }
    */
}
