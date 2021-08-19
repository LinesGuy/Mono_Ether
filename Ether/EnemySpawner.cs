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
                if (_rand.Next((int)_inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

                if (_rand.Next((int)_inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
            }

            // Slowly increase spawn rate as time progresses
            if (_inverseSpawnChance > 20)
                _inverseSpawnChance -= 0.005f;
        }

        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos;
            Vector2 playerPos = PlayerShip.Instance.Position;
            do
            {
                pos = new Vector2(_rand.NextFloat(playerPos.X - 500, playerPos.X + 500), _rand.NextFloat(playerPos.Y - 500, playerPos.Y + 500));
                if (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250) Debug.WriteLine("check one fail");
                if (Map.GetTileFromMap(Map.WorldtoMap(pos)) > 0) Debug.WriteLine("check two fail");
            }
            while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250 || Map.GetTileFromMap(Map.WorldtoMap(pos)) > 0);

            return pos;
        }

        public static void Reset()
        {
            _inverseSpawnChance = 60;
        }
    }
}
