using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Mono_Ether.Ether;

namespace Mono_Ether {
    public class EntityManager {
        public static EntityManager Instance;
        public List<Entity> Entities = new List<Entity>();
        public List<PlayerShip> Players = new List<PlayerShip>();
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Bullet> Bullets = new List<Bullet>();
        public List<Drone> Drones = new List<Drone>();
        public List<Geom> Geoms = new List<Geom>();
        public List<PowerPack> PowerPacks = new List<PowerPack>();

        private bool _isUpdating;
        private readonly List<Entity> _addedEntities = new List<Entity>();
        public EntityManager() {
            Instance = this;
        }
        public void Add(Entity entity) {
            if (!_isUpdating)
                AddEntity(entity);
            else
                _addedEntities.Add(entity);
        }
        private void AddEntity(Entity entity) {
            Entities.Add(entity);
            if (entity is PlayerShip player) {
                // Determine player index depending on how many players are in the players list currently
                player.Index = Players.Count switch {
                    0 => PlayerIndex.One,
                    1 => PlayerIndex.Two,
                    2 => PlayerIndex.Three,
                    3 => PlayerIndex.Four,
                    _ => player.Index
                };
                Players.Add(player);
            } else if (entity is Enemy enemy)
                Enemies.Add(enemy);
            else if (entity is Bullet bullet)
                Bullets.Add(bullet);
            else if (entity is PowerPack powerPack)
                PowerPacks.Add(powerPack);
            else if (entity is Geom geom)
                Geoms.Add(geom);
            else if (entity is Drone drone)
                Drones.Add(drone);
        }
        public void Update(GameTime gameTime) {
            _isUpdating = true;

            HandleCollisions();

            foreach (var entity in Entities)
                entity.Update(gameTime);

            _isUpdating = false;

            _addedEntities.ForEach(AddEntity);
            _addedEntities.Clear();

            Entities = Entities.Where(x => !x.IsExpired).ToList();
            Geoms = Geoms.Where(x => !x.IsExpired).ToList();
            Bullets = Bullets.Where(x => !x.IsExpired).ToList();
            Enemies = Enemies.Where(x => !x.IsExpired).ToList();
            PowerPacks = PowerPacks.Where(x => !x.IsExpired).ToList();
        }
        private static bool IsColliding(Entity a, Entity b) => !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < Math.Pow(a.Radius + b.Radius, 2);

        private void KillAll(bool includeBosses = false) {
            if (includeBosses)
                foreach (var enemy in Enemies)
                    enemy.Suicide();
            else
                foreach (var enemy in Enemies.Where(e => !e.IsBoss))
                    enemy.Suicide();
        }
        private void HandleCollisions() {
            #region Enemies <-> Enemies
            // If an enemy collides with another enemy, push the two enemies apart from each other.
            var i = 0;
            foreach (var enemyA in Enemies) {
                foreach (var enemyB in Enemies.Skip(i + 1))
                    if (IsColliding(enemyA, enemyB)) {
                        enemyA.HandleCollision(enemyB);
                        enemyB.HandleCollision(enemyA);
                    }
                i++;
            }
            #endregion
            #region Enemies <-> Bullets
            // If an enemy collides with a bullet, decrement the enemy health (if not invincible) and expire the bullet.
            foreach (var enemy in Enemies) {
                foreach (var bullet in Bullets) {
                    if (IsColliding(enemy, bullet)) {
                        if (enemy.Invincible)
                            continue;
                        bullet.Expire();
                        enemy.WasShot(bullet.ParentPlayerIndex);
                    }
                    // If the entity collided with a snake tail, don't decrement health but expire the bullet anyway.
                    if (!(enemy is SnakeHead snake))
                        continue;
                    if (!snake.Tail.Any(tail => IsColliding(bullet, tail)))
                        continue;
                    bullet.Expire();
                }
            }
            #endregion
            #region Players <-> Enemies
            // If the player collided with an enemy, kill the player and kill all enemies on screen.
            foreach (var player in Players.Where(player => !player.IsDead)) {
                foreach (var enemy in Enemies.Where(enemy => enemy.IsActive)) {
                    if (IsColliding(player, enemy)) {
                        player.Kill();
                        KillAll();
                        break;
                    }

                    switch (enemy) {
                        case SnakeHead snake when !snake.Tail.Any(tail => IsColliding(player, tail)):
                        case BossTwo bossTwo when !bossTwo.Tail.Any(tail => IsColliding(player, tail)):
                            continue;
                        case SnakeHead snake:
                        case BossTwo bossTwo:
                            player.Kill();
                            KillAll();
                            break;
                    }
                }
            }
            #endregion Handle collisions between the players and enemies
            #region Handle collisions between powerpacks and the player
            // If the player collides with a powerpack, apply the powerpack effect to the player and make the powerpack disappear
            foreach (var player in Players) {
                foreach (var powerPack in PowerPacks)
                    if (IsColliding(powerPack, player)) {
                        if (powerPack.Type == PowerPackType.Doom)
                        {
                            powerPack.IsExpired = true;
                            ScreenManager.AddScreen(new DoomScreen(GameRoot.Instance.GraphicsDevice));
                            player.ActivePowerPacks.Add(new PowerPack(PowerPackType.MoveSpeedIncrease, Vector2.Zero, TimeSpan.FromSeconds(60)));
                            player.ActivePowerPacks.Add(new PowerPack(PowerPackType.ShootSpeedIncrease, Vector2.Zero, TimeSpan.FromSeconds(60)));
                            continue;
                        }
                        powerPack.WasPickedUp();
                        powerPack.IsExpired = true;
                        player.ActivePowerPacks.Add(powerPack);
                    }
            }
            #endregion Handle collisions between powerpacks and the player
            #region Players <-> Geoms
            // If the player collides with a geom, make the geom disappear and increment the player geom count.
            foreach (Geom geom in Geoms) {
                foreach (var player in Players) {
                    if (IsColliding(geom, player)) {
                        geom.Pickup(player.Index);
                    }
                    if (Vector2.DistanceSquared(player.Position, geom.Position) < 150f * 150f)
                        geom.Velocity += (player.Position - geom.Position).ScaleTo(1.3f);
                }
            }
            #endregion  Handle players and geoms
            #region Geom drones <-> Geoms
            // Repeat above but for geom drones.
            foreach (var geom in Geoms) {
                foreach (var drone in Drones.Where(drone => drone.Type == DroneType.Collector)) {
                    if (IsColliding(geom, drone)) {
                        geom.Pickup(drone.OwnerPlayerIndex);
                    }
                    if (Vector2.DistanceSquared(drone.Position, geom.Position) < 150f * 150f)
                        geom.Velocity += (drone.Position - geom.Position).ScaleTo(1.3f);
                }
            }
            #endregion  Handle players and geoms
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (var entity in Entities)
                entity.Draw(batch, camera);
        }
    }
}
