﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether.Ether {
    class Enemy : Entity {
        private int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        private readonly int Worth; // The amount of score given when this enemy is killed
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private readonly Random rand = new Random();

        private Enemy(Texture2D image, Vector2 position) {
            this.Image = image;
            Position = position;
            Worth = rand.Next(50, 150);
            Radius = image.Width / 2f;
            Color = Color.Transparent;
        }
        public override void Update() {
            if (timeUntilStart <= 0) {
                ApplyBehaviours();
            } else {
                timeUntilStart--;
                Color = Color.White * (1 - timeUntilStart / 60f);
            }

            Position += Velocity;
            Velocity *= 0.8f;  // Friction
        }
        public void WasShot(int PlayerIndex) {
            IsExpired = true;
            // Increment player score
            EntityManager.Players[PlayerIndex].score += Worth;
            // Floating text
            FloatingTextManager.Add(Worth.ToString(), Position, Color.White);
            // Particles
            float hue1 = rand.NextFloat(0, 6);
            float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;
            Color color1 = ColorUtil.HsvToColor(hue1, 0.5f, 1);
            Color color2 = ColorUtil.HsvToColor(hue2, 0.5f, 1);

            for (var i = 0; i < 120; i++) {
                var speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState() {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }
        }
        private void AddBehaviour(IEnumerable<int> behaviour) {
            behaviours.Add(behaviour.GetEnumerator());
        }
        private void ApplyBehaviours() {
            for (int i = 0; i < behaviours.Count; i++) {
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }
        IEnumerable<int> FollowPlayerAStar(float acceleration = 1f) {
            while (true) {
                // If enemy is within Map.cellSize units from the player, move straight towards the player
                var dash = false;
                foreach (PlayerShip player in EntityManager.Players) {
                    if (Vector2.DistanceSquared(player.Position, Position) <= MyAStar.CellSize * MyAStar.CellSize) {
                        Velocity += (player.Position - Position).ScaleTo(acceleration);
                        if (Velocity != Vector2.Zero)
                            Orientation = Velocity.ToAngle();
                        yield return 0;
                        dash = true;
                        break;
                    }
                }
                if (dash)
                    continue;
                //else if (Map.GetTileFromWorld(PlayerShip.Instance.Position).TileId > 0) {
                else if (EntityManager.Players.TrueForAll(player => Map.GetTileFromWorld(player.Position).TileId > 0)) {
                    // All players are in solid tiles, do nothing.
                    yield return 0;
                } else {
                    // Use A* to move towards the nearest player
                    PlayerShip nearestPlayer = EntityManager.player1;
                    foreach (PlayerShip player in EntityManager.Players)
                        if (Vector2.DistanceSquared(Position, player.Position) < Vector2.DistanceSquared(Position, nearestPlayer.Position))
                            nearestPlayer = player;
                    var path = MyAStar.AStar(Position, nearestPlayer.Position);
                    // Instead of calculating a new path every frame or whatever, we will calculate a new path
                    // based on how far the enemy is from the player:
                    // - If the enemy is within 1000 units of the player, update the path every 30 frames
                    // - If within 2500 units, update every 60 frames
                    // - If within 5000 units, every 120 frames
                    // - Otherwise, every 240 frames
                    // TODO implement the above
                    for (int i = 0; i < 60; i++) {
                        if (path is null) {
                            yield return 0;
                            continue;
                        }
                        // If enemy is at current target position, update target position
                        if (Vector2.DistanceSquared(Position, path[0]) <= Math.Pow(MyAStar.CellSize * 0.8f, 2))
                            path.RemoveAt(0);
                        // If path is empty, make a new path
                        if (path.Count <= 0)
                            break;
                        // Move towards player
                        Velocity += (path[0] - Position).ScaleTo(acceleration);
                        if (Velocity != Vector2.Zero)
                            Orientation = Velocity.ToAngle();

                        yield return 0;
                    }
                }
            }
        }
        IEnumerable<int> MoveRandomly() {
            float direction = rand.NextFloat(0, MathHelper.TwoPi);
            Vector2 acceleration = MathUtil.FromPolar(direction, 0.4f);
            Vector2 lastPos = Position;
            while (true) {
                if (Math.Abs(Position.X - lastPos.X) < 0.001)
                    acceleration.X = -acceleration.X;
                if (Math.Abs(Position.Y - lastPos.Y) < 0.001)
                    acceleration.Y = -acceleration.Y;

                lastPos = Position;

                acceleration.Rotate(rand.NextFloat(-0.1f, 0.1f));

                for (int i = 0; i < 6; i++) {
                    Velocity += acceleration;
                    Orientation -= 0.01f;
                    yield return 0;
                }
            }
        }
        private IEnumerable<int> DodgeBullets(float distance = 100f, float acceleration = 1f) {
            while (true) {
                foreach (var bullet in EntityManager.Bullets)
                    if (Vector2.DistanceSquared(bullet.Position, Position) < distance * distance)
                        Velocity -= (bullet.Position - Position).ScaleTo(acceleration);
                yield return 0;
            }
        }
        public static Enemy CreateSeeker(Vector2 position) {
            var enemy = new Enemy(Art.Seeker, position);
            enemy.AddBehaviour(enemy.FollowPlayerAStar());

            return enemy;
        }
        public static Enemy CreateWanderer(Vector2 position) {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehaviour(enemy.MoveRandomly());
            enemy.AddBehaviour(enemy.DodgeBullets());
            return enemy;
        }
        public void HandleCollision(Enemy other) {
            var delta = Position - other.Position;
            Velocity += 10 * delta / (delta.LengthSquared() + 1);
            // ^ Push current enemy away from other enemy. The closer they are, the harder the push.
        }
    }
}
