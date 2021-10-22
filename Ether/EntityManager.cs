using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether.Ether {
    static class EntityManager {
        public static List<PlayerShip> Players = new List<PlayerShip>();
        public static List<Entity> Entities = new List<Entity>();
        public static List<Geom> Geoms = new List<Geom>();
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Bullet> Bullets = new List<Bullet>();
        public static List<PowerPack> PowerPacks = new List<PowerPack>();

        static bool _isUpdating;
        static readonly List<Entity> AddedEntities = new List<Entity>();
        public static PlayerShip Player1 => Players[0];
        public static Random rand = new Random();
        public static int Count => Entities.Count;
        public static void Killall() {
            Players.Clear();
            Entities.Clear();
            Enemies.Clear();
            Geoms.Clear();
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
            else if (entity is Geom geom)
                Geoms.Add(geom);
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
            Geoms = Geoms.Where(x => !x.IsExpired).ToList();
            Bullets = Bullets.Where(x => !x.IsExpired).ToList();
            Enemies = Enemies.Where(x => !x.IsExpired).ToList();
            PowerPacks = PowerPacks.Where(x => !x.IsExpired).ToList();
        }
        public static void Draw(SpriteBatch spriteBatch) {
            foreach (var entity in Entities) {
                entity.Draw(spriteBatch);
                if (entity is Enemy enemy)
                    if (enemy.Type == "Snake")
                        for (int i = 1; i < enemy.tail.Count; i++) {
                            Enemy tail = enemy.tail[i];
                            tail.Draw(spriteBatch);
                        }
            }
        }
        private static bool IsColliding(Entity a, Entity b) {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        static void HandleCollisions() {
            #region Handle collisions between enemies

            for (int i = 0; i < Enemies.Count; i++) {
                for (int j = i + 1; j < Enemies.Count; j++) {

                    if (IsColliding(Enemies[i], Enemies[j])) {
                        Enemies[i].HandleCollision(Enemies[j]);
                        Enemies[j].HandleCollision(Enemies[i]);
                    }
                }
            }
            #endregion  Handle collisions between enemies
            #region Handle collisions between bullets and enemies
            for (var i = 0; i < Enemies.Count; i++) {
                foreach (var bullet in Bullets) {
                    if (Enemies[i].invincible)
                        continue;
                    if (IsColliding(Enemies[i], bullet)) {
                        Enemies[i].WasShot(bullet.PlayerIndex);
                        bullet.IsExpired = true;
                        // Play enemy_explosion.wav
                        Art.EnemyExplosion.CreateInstance().Play();
                        // If enemy type is PinkSeeker, summon two more enemies
                        if (Enemies[i].Type == "PinkSeeker") {
                            for (int j = 0; j < 2; j++) {
                                var enemy = Enemy.CreatePinkSeekerChild(Enemies[i].Position);
                                enemy.Velocity = MathUtil.FromPolar(rand.NextFloat(0, MathF.PI * 2), 10f);
                                enemy.timeUntilStart = 0;
                                enemy.Color = Color.White;
                                enemy.invincible = true;
                                EntityManager.Add(enemy);
                            }
                        }
                    // If bullet collides with snake body, destroy bullet but not snake
                    if (Enemies[i].Type == "Snake")
                        for (int j = 1; j < Enemies[i].tail.Count; j++) {
                            Enemy tail = Enemies[i].tail[j];
                            if (IsColliding(tail, bullet))
                                bullet.IsExpired = true;
                        }
                    }
                }
            }
            #endregion Handle collisions between bullets and enemies
            #region Handle collisions between the players and enemies
            foreach (PlayerShip player in Players) {
                for (int i = 0; i < Enemies.Count; i++) {
                    if (Enemies[i].IsActive && IsColliding(player, Enemies[i])) {
                        player.Kill();
                        Enemies.ForEach(e => e.WasShot(player.playerIndex));
                        break;
                    }
                    if (Enemies[i].Type == "Snake")
                        for (int j = 1; j < Enemies[i].tail.Count; j++) {
                            Enemy tail = Enemies[i].tail[j];
                            if (IsColliding(tail, player)) {
                                player.Kill();
                                Enemies.ForEach(e => e.WasShot(player.playerIndex));
                                break;
                            }
                        }
                }
            }
            #endregion Handle collisions between the players and enemies
            #region Handle collisions between walls and enemies
            for (var i = 0; i < Enemies.Count; i++)
                Enemies[i].HandleTilemapCollision();
            #endregion Handle collisions between walls and enemies + the player
            #region Same as above but for bullets
            for (var i = 0; i < Bullets.Count; i++)
                Bullets[i].HandleTilemapCollision();
            #endregion Same as above but for bullets
            #region Handle collisions between powerpacks and the player
            foreach (PlayerShip player in Players) {
                for (var i = 0; i < PowerPacks.Count; i++)
                    if (IsColliding(PowerPacks[i], player)) {
                        PowerPacks[i].WasPickedUp();
                        player.activePowerPacks.Add(PowerPacks[i]);
                        PowerPacks[i].IsExpired = true;
                    }
            }
            #endregion Handle collisions between powerpacks and the player
            #region Handle players and geoms
            foreach(Geom geom in Geoms) {
                foreach(PlayerShip player in Players) {
                    if (IsColliding(geom, player)) {
                        geom.IsExpired = true;
                        player.geoms += 1;
                    }
                    if (Vector2.DistanceSquared(player.Position, geom.Position) < 150f * 150f)
                        geom.Velocity += (player.Position - geom.Position).ScaleTo(1.3f);
                }
            }
            #endregion  Handle players and geoms
        }
    }
}
