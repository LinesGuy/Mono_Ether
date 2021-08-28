using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether
{
    static class EnemySpawner
    {
        static Random _rand = new Random();
        static float _inverseSpawnChance = 60;
        public static bool enabled = false; // DISABLED BY DEFAULT FOR DEBUGGING
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

        private static Vector2 GetSpawnPosition()
        {
            // If returns Vector2.Zero, could not find valid spawn position
            Vector2 pos;
            Vector2 playerPos = PlayerShip.Instance.Position;
            int remainingAttempts = 10;
            do
            {
                pos = new Vector2(_rand.NextFloat(playerPos.X - 500, playerPos.X + 500), _rand.NextFloat(playerPos.Y - 500, playerPos.Y + 500));
                remainingAttempts -= 1;
            }
            while ((Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250
                   || Map.GetTileFromMap(Map.WorldtoMap(pos)).TileId > 0
                   || pos.X < 0 || pos.Y < 0 || pos.X > Map._size.X * Map.cellSize || pos.Y > Map._size.Y * Map.cellSize)
                   && remainingAttempts > 0);
            if (remainingAttempts == 0)
            {
                Debug.WriteLine("Could not spawn enemy after 10 attempts, skipping");
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
