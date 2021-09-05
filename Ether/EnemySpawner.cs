using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether
{
    static class EnemySpawner
    {
        static Random _rand = new Random();
        static float _inverseSpawnChance = 60;
        public static bool enabled = true;
        public static void Update()
        {
            if (!enabled)
                return;

            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
            {
                if (_rand.Next((int)_inverseSpawnChance) != 0)
                    return;

                var pos = GetSpawnPosition();
                if (pos == Vector2.Zero)
                    return; // Couldn't find valid spawn position

                if (_rand.Next(0, 2) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(pos));
                else
                    EntityManager.Add(Enemy.CreateWanderer(pos));
            }

            // Slowly increase spawn rate as time progresses
            if (_inverseSpawnChance > 20)
                _inverseSpawnChance -= 0.005f;
        }

        public static Vector2 GetSpawnPosition(float radius = 500f, int attempts = 10)
        {
            // If returns Vector2.Zero, could not find valid spawn position
            Vector2 pos;
            Vector2 playerPos = PlayerShip.Instance.Position;
            int remainingAttempts = attempts;
            do
            {
                pos = new Vector2(_rand.NextFloat(playerPos.X - radius, playerPos.X + radius), _rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
                remainingAttempts -= 1;
            }
            while ((Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < Math.Pow(radius / 2f, 2)
                   || Map.GetTileFromMap(Map.WorldtoMap(pos)).TileId > 0
                   || pos.X < 0 || pos.Y < 0 || pos.X > Map._size.X * Map.cellSize || pos.Y > Map._size.Y * Map.cellSize)
                   && remainingAttempts > 0);
            if (remainingAttempts == 0)
            {
                Debug.WriteLine($"Could not find spawn position after {attempts} attempts, skipping");
                return Vector2.Zero;
            }
            return pos;
        }

        public static void Reset()
        {
            _inverseSpawnChance = 60;
        }
    }
}
