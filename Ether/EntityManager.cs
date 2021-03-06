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
        public static List<Drone> Drones = new List<Drone>();

        static bool _isUpdating;
        static readonly List<Entity> AddedEntities = new List<Entity>();
        public static PlayerShip Player1 => Players[0];
        public static PlayerShip Player2 => Players[1];
        public static Random rand = new Random();
        public static int Count => Entities.Count;
        public static void Killall() {
            Players.Clear();
            Entities.Clear();
            Enemies.Clear();
            Geoms.Clear();
            Bullets.Clear();
            PowerPacks.Clear();
            Drones.Clear();
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
            else if (entity is Drone drone)
                Drones.Add(drone);
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
                    if (enemy.Type == "Snake" || enemy.Type == "BossTwoHead")
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
                    if (IsColliding(Enemies[i], bullet)) {
                        if (Enemies[i].invincible) {
                            bullet.Expire();
                            continue;
                        }
                        Enemies[i].WasShot(bullet.PlayerIndex);
                        bullet.Expire();
                    }
                    // If bullet collides with snake body, destroy bullet but not snake
                    if (Enemies[i].Type == "Snake" || Enemies[i].Type == "BossTwoHead") {
                        for (int j = 1; j < Enemies[i].tail.Count; j++) {
                            Enemy tail = Enemies[i].tail[j];
                            if (IsColliding(tail, bullet))
                                bullet.Expire();
                        }
                    }
                }
            }
            #endregion Handle collisions between bullets and enemies
            #region Handle collisions between the players and enemies
            foreach (PlayerShip player in Players) {
                if (player.IsDead)
                    continue;
                for (int i = 0; i < Enemies.Count; i++) {
                    if (Enemies[i].IsActive && IsColliding(player, Enemies[i])) {
                        player.Kill();
                        foreach (Enemy enemy in Enemies) {
                            if (!enemy.IsBoss)
                                enemy.SummonParticles();
                        }
                        Enemies = Enemies.Where(e => (e.IsBoss)).ToList();
                        Entities = Entities.Where(e => !(e is Enemy enemy && !enemy.IsBoss)).ToList();
                        break;
                    }
                    if (Enemies[i].Type == "Snake" || Enemies[i].Type == "BossTwoHead")
                        for (int j = 1; j < Enemies[i].tail.Count; j++) {
                            Enemy tail = Enemies[i].tail[j];
                            if (IsColliding(tail, player)) {
                                player.Kill();
                                foreach (Enemy enemy in Enemies) {
                                    if (!enemy.IsBoss)
                                        enemy.SummonParticles();
                                }
                                Enemies = Enemies.Where(e => (e.IsBoss)).ToList();
                                Entities = Entities.Where(e => !(e is Enemy enemy && !enemy.IsBoss)).ToList();
                                break;
                            }
                        }
                }
            }
            #endregion Handle collisions between the players and enemies
            #region Handle collisions between walls and enemies
            for (var i = 0; i < Enemies.Count; i++) {
                Enemies[i].HandleTilemapCollision();
                if (Enemies[i].Type == "Snake" || Enemies[i].Type == "BossTwoHead") {
                    for (int j = 1; j < Enemies[i].tail.Count; j++) {
                        Enemies[i].tail[j].HandleTilemapCollision();
                    }
                }
            }

            #endregion Handle collisions between walls and enemies + the player
            #region Handle collisions walls and bullets
            for (var i = 0; i < Bullets.Count; i++)
                Bullets[i].HandleTilemapCollision();
            #endregion Same as above but for bullets
            #region Handle collisions between powerpacks and the player
            foreach (PlayerShip player in Players) {
                for (var i = 0; i < PowerPacks.Count; i++)
                    if (IsColliding(PowerPacks[i], player)) {
                        PowerPacks[i].WasPickedUp();
                        PowerPacks[i].IsExpired = true;
                        if (PowerPacks[i].PowerType == "Doom") {
                            ScreenManager.TransitionScreen(new DoomRoot(GameRoot.Instance.myGraphics, "Secret.txt"));
                            player.activePowerPacks.Add(new PowerPack(Art.PowerMoveSpeedIncrease, Vector2.Zero, "MoveSpeedIncrease", 3600));
                            player.activePowerPacks.Add(new PowerPack(Art.PowerShootSpeedIncrease, Vector2.Zero, "ShootSpeedIncrease", 3600));
                            continue;
                        }
                        player.activePowerPacks.Add(PowerPacks[i]);
                    }
            }
            #endregion Handle collisions between powerpacks and the player
            #region Handle players and geoms
            foreach (Geom geom in Geoms) {
                for (int i = 0; i < Players.Count; i++) {
                    PlayerShip player = Players[i];
                    if (IsColliding(geom, player)) {
                        geom.Pickup(i);
                    }
                    if (Vector2.DistanceSquared(player.Position, geom.Position) < 150f * 150f)
                        geom.Velocity += (player.Position - geom.Position).ScaleTo(1.3f);
                }
            }
            #endregion  Handle players and geoms
            #region Handle geom drones and geoms
            List<Drone> geomDrones = Drones.Where(drone => drone.Type == "geomCollector").ToList();
            foreach (Geom geom in Geoms) {
                for (int i = 0; i < geomDrones.Count; i++) {
                    Drone drone = geomDrones[i];
                    if (IsColliding(geom, drone)) {
                        geom.Pickup(drone.PlayerIndex);
                    }
                    if (Vector2.DistanceSquared(drone.Position, geom.Position) < 150f * 150f)
                        geom.Velocity += (drone.Position - geom.Position).ScaleTo(1.3f);
                }
            }
            #endregion  Handle players and geoms
        }
    }
}
