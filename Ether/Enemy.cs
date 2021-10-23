using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    class Enemy : Entity {
        public int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        private readonly int Worth; // The amount of score given when this enemy is killed
        public List<Enemy> tail; // Used for snake enemy type
        public string Type;
        public bool invincible = false;
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private static readonly Random rand = new Random();

        private Enemy(Texture2D image, Vector2 position, string type) {
            this.Image = image;
            Position = position;
            Type = type;
            Worth = rand.Next(1, 10);
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
            EntityManager.Players[PlayerIndex].AddPoints(Worth);
            // Summon geom
            EntityManager.Add(new Geom(Position));
            // Floating text
            FloatingTextManager.Add((Worth * EntityManager.Players[PlayerIndex].multiplier).ToString(), Position, Color.White, "bounce");
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
        #region IEnumerables
        private IEnumerable<int> FollowPlayerAStar(float acceleration = 1f) {
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
                    PlayerShip nearestPlayer = EntityManager.Player1;
                    foreach (PlayerShip player in EntityManager.Players)
                        if (Vector2.DistanceSquared(Position, player.Position) < Vector2.DistanceSquared(Position, nearestPlayer.Position))
                            nearestPlayer = player;
                    var path = MyAStar.AStar(Position, nearestPlayer.Position);

                    // Instead of calculating a new path every frame or whatever, we will calculate a new path
                    // based on how far the enemy is from the player:
                    var timeUntilNextCalculation = 240;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 2500 * 2500)
                        timeUntilNextCalculation = 120;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 1500 * 1500)
                        timeUntilNextCalculation = 60;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 750 * 750)
                        timeUntilNextCalculation = 20;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 300 * 300)
                        timeUntilNextCalculation = 10;
                    for (int i = 0; i < timeUntilNextCalculation; i++) {
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
                        yield return 0;
                    }
                }
            }
        }
        private IEnumerable<int> MoveRandomly(float speed = 0.4f, float rotationSpeed = 0.1f, float bounds = 0.1f) {
            float direction = rand.NextFloat(0, MathHelper.TwoPi);
            Vector2 acceleration = MathUtil.FromPolar(direction, speed);
            float rotationDelta = 0f;
            Vector2 lastPos = Position;
            while (true) {
                if (Math.Abs(Position.X - lastPos.X) < 0.001)
                    acceleration.X = -acceleration.X;
                if (Math.Abs(Position.Y - lastPos.Y) < 0.001)
                    acceleration.Y = -acceleration.Y;

                lastPos = Position;
                rotationDelta += rand.NextFloat(-rotationSpeed, rotationSpeed);
                if (rotationDelta < -bounds)
                    rotationDelta = -bounds;
                if (rotationDelta > bounds)
                    rotationDelta = bounds;
                acceleration = acceleration.Rotate(rotationDelta);

                for (int i = 0; i < 6; i++) {
                    Velocity += acceleration;
                    yield return 0;
                }
            }
        }
        private IEnumerable<int> BounceOffWalls(float angle, float speed = 1.7f) {
            Vector2 lastPos = Position;
            Vector2 acceleration = MathUtil.FromPolar(angle, speed);
            while (true) {
                if (Math.Abs(Position.X - lastPos.X) < 0.001)
                    acceleration.X = -acceleration.X;
                if (Math.Abs(Position.Y - lastPos.Y) < 0.001)
                    acceleration.Y = -acceleration.Y;
                lastPos = Position;
                Velocity += acceleration;
                yield return 0;
            }
        }
        private IEnumerable<int> ExhaustFire() {
            while (true) {
                if (Velocity.LengthSquared() > 0.1f) {
                    Vector2 baseVel = Velocity.ScaleTo(-3).Rotate(rand.NextFloat(-0.3f, 0.3f));
                    EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.OrangeRed * 0.7f, 60f, new Vector2(0.5f, 1),
                        new ParticleState(baseVel, ParticleType.Enemy));
                }
                yield return 0;
            }
        }
        private IEnumerable<int> EnemyFacesVelocity() {
            Vector2 lastPos = Position;
            while (true) {
                Vector2 delta = Position - lastPos;
                if (delta != Vector2.Zero)
                    Orientation = delta.ToAngle();
                lastPos = Position;
                yield return 0;
            }
        }
        private IEnumerable<int> RotateOrientationConstantly(float speed = 0.1f) {
            while (true) {
                Orientation += speed;
                yield return 0;
            }
        }
        private IEnumerable<int> MoveOrthonallyOccasionally(int activeFrames = 30, int sleepFrames = 60, float speed = 2f) {
            while (true) {
                Vector2 velocity = MathUtil.FromPolar(rand.Next(4) * MathF.PI / 2f, speed);
                for (int i = 0; i < activeFrames; i++) {
                    Position += velocity;
                    yield return 0;
                }
                for (int i = 0; i < sleepFrames; i++) {
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
        private IEnumerable<int> UpdateTail(float distance = 20f) {
            while (true) {
                for (int i = 1; i < tail.Count; i++) {
                    Enemy bodyTail = tail[i];
                    if (bodyTail.timeUntilStart > 0) {
                        bodyTail.timeUntilStart--;
                        bodyTail.Color = Color.White * (1 - timeUntilStart / 60f);
                    }
                    if (Vector2.DistanceSquared(bodyTail.Position, tail[i - 1].Position) > distance * distance) {
                        bodyTail.Position = tail[i - 1].Position + (bodyTail.Position - tail[i - 1].Position).ScaleTo(distance);
                    }
                    bodyTail.ApplyBehaviours();
                }
                yield return 0;
            }
        }
        private IEnumerable<int> RotateAroundPosition() {
            float orientation = rand.NextFloat(0, MathF.PI * 2f);
            const float rotationSpeed = 0.1f;
            const float speed = 1f;
            while (true) {
                Velocity += MathUtil.FromPolar(orientation, speed);
                orientation += rotationSpeed;
                yield return 0;
            }
        }
        private IEnumerable<int> InvincibleForTime(int frames) {
            invincible = true;
            for (int i = 0; i < frames; i++) {
                yield return 0;
            }
            invincible = false;
            while (true) {
                yield return 0;
            }
        }
        #endregion IEnumerables
        #region CreateEnemies
        public static Enemy CreateBlueSeeker(Vector2 position) {
            var enemy = new Enemy(Art.BlueSeeker, position, "BlueSeeker");
            enemy.AddBehaviour(enemy.FollowPlayerAStar(0.8f));
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreatePurpleWanderer(Vector2 position) {
            var enemy = new Enemy(Art.PurpleWanderer, position, "PurpleWanderer");
            enemy.AddBehaviour(enemy.MoveRandomly());
            enemy.AddBehaviour(enemy.RotateOrientationConstantly());
            return enemy;
        }
        public static Enemy CreateSnake(Vector2 position) {
            var enemy = new Enemy(Art.SnakeHead, position, "Snake");
            
            enemy.AddBehaviour(enemy.MoveRandomly(1f, 0.3f, 0.3f));
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            enemy.tail = new List<Enemy> { enemy };
            int tailLength = rand.Next(5, 15);
            for (int i = 0; i < tailLength; i++) {
                enemy.tail.Add(Enemy.CreateSnakeBody(position));
            }
            enemy.AddBehaviour(enemy.UpdateTail());
            return enemy;
        }
        public static Enemy CreateSnakeBody(Vector2 position) {
            var enemy = new Enemy(Art.SnakeBody, position, "SnakeBody");
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreateBackAndForther(Vector2 position) {
            Enemy enemy = new Enemy(Art.BackAndForther, position, "BackAndForther") {
                Orientation = new Random().Next(4) * MathF.PI / 2
            };
            enemy.AddBehaviour(enemy.BounceOffWalls(enemy.Orientation + MathF.PI));
            enemy.AddBehaviour(enemy.ExhaustFire());
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreatePinkWanderer(Vector2 position) {
            var enemy = new Enemy(Art.PinkWanderer, position, "PinkWanderer");
            enemy.AddBehaviour(enemy.MoveOrthonallyOccasionally());
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreateGreenSeeker(Vector2 position) {
            var enemy = new Enemy(Art.GreenSeeker, position, "GreenSeeker");
            enemy.AddBehaviour(enemy.FollowPlayerAStar(1.2f));
            enemy.AddBehaviour(enemy.DodgeBullets(100f, 1.5f));
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreatePinkSeeker(Vector2 position) {
            var enemy = new Enemy(Art.PinkSeeker, position, "PinkSeeker");
            enemy.AddBehaviour(enemy.FollowPlayerAStar(1.2f));
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreatePinkSeekerChild(Vector2 position) {
            var enemy = new Enemy(Art.PinkSeekerChild, position, "PinkSeekerChild");
            enemy.AddBehaviour(enemy.RotateAroundPosition());
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            enemy.AddBehaviour(enemy.InvincibleForTime(6));
            enemy.Radius = 10f;
            return enemy;
        }
        #endregion CreateEnemies
        public void HandleCollision(Enemy other) {
            var delta = Position - other.Position;
            Velocity += 10 * delta / (delta.LengthSquared() + 1);
            // ^ Push current enemy away from other enemy. The closer they are, the harder the push.
        }
    }
}
