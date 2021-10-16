using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether.Ether {
    static class EntityManager {
        public static List<PlayerShip> Players = new List<PlayerShip>();
        public static List<Entity> Entities = new List<Entity>();
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Bullet> Bullets = new List<Bullet>();
        public static List<PowerPack> PowerPacks = new List<PowerPack>();

        static bool _isUpdating;
        static readonly List<Entity> AddedEntities = new List<Entity>();
        public static PlayerShip player1 => Players[0];
        public static int Count => Entities.Count;
        public static void Killall() {
            Players.Clear();
            Entities.Clear();
            Enemies.Clear();
            Bullets.Clear();
            PowerPacks.Clear();
        }
        public static void Add(Entity entity) {
            if (!_isUpdating)
                AddEntity(entity);
            else
                AddedEntities.Add(entity);
        }
        private static void AddEntity(Entity entity) {
            Entities.Add(entity);
            if (entity is PlayerShip player)
                Players.Add(player);
            else if (entity is Bullet bullet)
                Bullets.Add(bullet);
            else if (entity is Enemy enemy)
                Enemies.Add(enemy);
            else if (entity is PowerPack powerPack)
                PowerPacks.Add(powerPack);
        }

        public static void Update() {
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
            PowerPacks = PowerPacks.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch) {
            foreach (var entity in Entities)
                entity.Draw(spriteBatch);
        }

        private static bool IsColliding(Entity a, Entity b) {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        static void HandleCollisions() {
            // Handle collisions between enemies
            for (int i = 0; i < Enemies.Count; i++) {
                for (int j = i + 1; j < Enemies.Count; j++) {

                    if (IsColliding(Enemies[i], Enemies[j])) {
                        Enemies[i].HandleCollision(Enemies[j]);
                        Enemies[j].HandleCollision(Enemies[i]);
                    }
                }
            }

            // Handle collisions between bullets and enemies
            for (var i = 0; i < Enemies.Count; i++) {
                foreach (var t in Bullets) {
                    if (IsColliding(Enemies[i], t)) {
                        Enemies[i].WasShot();
                        t.IsExpired = true;
                        // Play enemy_explosion.wav
                        Art.EnemyExplosion.CreateInstance().Play();
                    }
                }
            }

            // Handle collisions between the players and enemies
            foreach(PlayerShip player in Players) {
                if (player.GodMode == false) {
                    for (int i = 0; i < Enemies.Count; i++) {
                        if (Enemies[i].IsActive && IsColliding(player, Enemies[i])) {
                            player.Kill();
                            Enemies.ForEach(x => x.WasShot());
                            break;
                        }
                    }
                }
            }
            
            // Handle collisions between walls and enemies + the player
            for (var i = 0; i < Enemies.Count; i++)
                Enemies[i].HandleTilemapCollision();
            // Same as above but for bullets
            for (var i = 0; i < Bullets.Count; i++)
                Bullets[i].HandleTilemapCollision();

            // Handle collisions between powerpacks and the player
            foreach(PlayerShip player in Players) {
                for (var i = 0; i < PowerPacks.Count; i++)
                    if (IsColliding(PowerPacks[i], player)) {
                        PowerPacks[i].WasPickedUp();
                        player.activePowerPacks.Add(PowerPacks[i]);
                        PowerPacks[i].IsExpired = true;
                    }
            }
        }
    }
}
