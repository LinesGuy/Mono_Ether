using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether.Ether
{
    static class EntityManager
    {
        public static List<Entity> Entities = new List<Entity>();
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Bullet> Bullets = new List<Bullet>();

        static bool _isUpdating;
        static readonly List<Entity> AddedEntities = new List<Entity>();

        public static int Count => Entities.Count;

        public static void Add(Entity entity)
        {
            if (!_isUpdating)
                AddEntity(entity);
            else
                AddedEntities.Add(entity);
        }

        private static void AddEntity(Entity entity)
        {
            Entities.Add(entity);
            if (entity is Bullet bullet)
                Bullets.Add(bullet);
            else if (entity is Enemy enemy)
                Enemies.Add(enemy);
        }

        public static void Update()
        {
            _isUpdating = true;

            HandleCollisions();

            foreach (var entity in Entities)
                entity.Update();

            _isUpdating = false;

            foreach (var entity in AddedEntities)
                AddEntity(entity);

            AddedEntities.Clear();

            // Remove expired entities
            Entities = Entities.Where(x => !x.IsExpired).ToList();
            Bullets = Bullets.Where(x => !x.IsExpired).ToList();
            Enemies = Enemies.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in Entities)
                entity.Draw(spriteBatch);
        }

        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        static void HandleCollisions()
        {
            // Handle collisions between enemies
            for (int i = 0; i < Enemies.Count; i++)
            {
                for (int j = i + 1; j < Enemies.Count; j++)
                {

                    if (IsColliding(Enemies[i], Enemies[j]))
                    {
                        Enemies[i].HandleCollision(Enemies[j]);
                        Enemies[j].HandleCollision(Enemies[i]);
                    }
                }
            }

            // Handle collisions between bullets and enemies
            for (var i = 0; i < Enemies.Count; i++)
            {
                foreach (var t in Bullets)
                {
                    if (IsColliding(Enemies[i], t))
                    {
                        Enemies[i].WasShot();
                        t.IsExpired = true;
                        // Play enemy_explosion.wav
                        Art.EnemyExplosion.CreateInstance().Play();
                    }
                }
            }

            // Handle collisions between the player and enemies
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].IsActive && IsColliding(PlayerShip.Instance, Enemies[i]))
                {
                    PlayerShip.Instance.Kill();
                    Enemies.ForEach(x => x.WasShot());
                    break;
                }
            }
        }
    }
}
