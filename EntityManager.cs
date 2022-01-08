﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    public class EntityManager {
        public static EntityManager Instance;
        public List<Entity> Entities = new List<Entity>();
        public List<PlayerShip> Players = new List<PlayerShip>();
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Bullet> Bullets = new List<Bullet>();

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
            //else if (entity is PowerPack powerPack)
            //PowerPacks.Add(powerPack);
            //else if (entity is Geom geom)
            //Geoms.Add(geom);
            //else if (entity is Drone drone)
            //Drones.Add(drone);
        }
        public void Update(GameTime gameTime) {
            _isUpdating = true;

            HandleCollisions();

            foreach (Entity entity in Entities)
                entity.Update(gameTime);

            _isUpdating = false;

            _addedEntities.ForEach(AddEntity);
            _addedEntities.Clear();

            Entities = Entities.Where(x => !x.IsExpired).ToList();
            //Geoms = Geoms.Where(x => !x.IsExpired).ToList();
            //Bullets = Bullets.Where(x => !x.IsExpired).ToList();
            Enemies = Enemies.Where(x => !x.IsExpired).ToList();
            //PowerPacks = PowerPacks.Where(x => !x.IsExpired).ToList();
        }
        private bool IsColliding(Entity a, Entity b) => !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < Math.Pow(a.Radius + b.Radius, 2);
        private void HandleCollisions() {
            #region Enemies <-> Enemies
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
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (Entity entity in Entities)
                entity.Draw(batch, camera);
        }
    }
}
