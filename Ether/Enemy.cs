using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    class Enemy : Entity {
        public int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        public int Health;
        private readonly int Worth; // The amount of score given when this enemy is killed
        public List<Enemy> tail; // Used for snake enemy type
        public string Type;
        public bool IsBoss = false;
        public bool invincible = false;
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private static readonly Random rand = new Random();

        private Enemy(Texture2D image, Vector2 position, string type) {
            Image = image;
            Position = position;
            Type = type;
            Health = 1;
            Worth = rand.Next(1, 10);
            Radius = image.Width / 2f;
            Color = Color.Transparent;
            Sounds.EnemySpawn.Play(GameSettings.SoundEffectVolume, rand.NextFloat(-0.2f, 0.2f), 0);
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
            Health--;
            if (Health <= 0)
                WasKilled(PlayerIndex);
        }
        public void WasKilled(int PlayerIndex) {
            IsExpired = true;
            // End game if boss
            if (IsBoss)
                Hud.transitionImage = "youWon";
            // Increment player score
            EntityManager.Players[PlayerIndex].AddPoints(Worth);
            // Summon geom
            EntityManager.Add(new Geom(Position));
            // Floating text
            FloatingTextManager.Add((Worth * EntityManager.Players[PlayerIndex].Multiplier).ToString(), Position, Color.White, "bounce");
            // Play sound
            Sounds.EnemyExplosion.Play(GameSettings.SoundEffectVolume, rand.NextFloat(-0.2f, 0.2f), 0);
            // Particles
            SummonParticles();
            // If enemy type is PinkSeeker, summon two more enemies
            if (Type == "PinkSeeker") {
                for (int j = 0; j < 2; j++) {
                    var enemy = Enemy.CreatePinkSeekerChild(Position);
                    enemy.Velocity = MathUtil.FromPolar(rand.NextFloat(0, MathF.PI * 2), 10f);
                    enemy.timeUntilStart = 0;
                    enemy.Color = Color.White;
                    enemy.invincible = true;
                    EntityManager.Add(enemy);
                }
            }
        }
        public void SummonParticles() {
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
        public void HandleCollision(Enemy other) {
            var delta = Position - other.Position;
            Velocity += 10 * delta / (delta.LengthSquared() + 1);
            // ^ Push current enemy away from other enemy. The closer they are, the harder the push.
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
        private IEnumerable<int> DodgeBullets(float distance = 100f, float acceleration = 1f) {
            while (true) {
                foreach (var bullet in EntityManager.Bullets)
                    if (Vector2.DistanceSquared(bullet.Position, Position) < distance * distance)
                        Velocity -= (bullet.Position - Position).ScaleTo(acceleration);
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
            IEnumerable<int> UpdateTail(float distance = 20f) {
                while (true) {
                    for (int i = 1; i < enemy.tail.Count; i++) {
                        Enemy bodyTail = enemy.tail[i];
                        if (bodyTail.timeUntilStart > 0) {
                            bodyTail.timeUntilStart--;
                            bodyTail.Color = Color.White * (1 - enemy.timeUntilStart / 60f);
                        }
                        if (Vector2.DistanceSquared(bodyTail.Position, enemy.tail[i - 1].Position) > distance * distance) {
                            bodyTail.Position = enemy.tail[i - 1].Position + (bodyTail.Position - enemy.tail[i - 1].Position).ScaleTo(distance);
                        }
                        bodyTail.ApplyBehaviours();
                    }
                    yield return 0;
                }
            }
            enemy.AddBehaviour(UpdateTail());
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
            IEnumerable<int> BounceOffWalls(float angle, float speed = 1.7f) {
                Vector2 lastPos = enemy.Position;
                Vector2 acceleration = MathUtil.FromPolar(angle, speed);
                while (true) {
                    if (Math.Abs(enemy.Position.X - lastPos.X) < 0.001)
                        acceleration.X = -acceleration.X;
                    if (Math.Abs(enemy.Position.Y - lastPos.Y) < 0.001)
                        acceleration.Y = -acceleration.Y;
                    lastPos = enemy.Position;
                    enemy.Velocity += acceleration;
                    yield return 0;
                }
            }
            enemy.AddBehaviour(BounceOffWalls(enemy.Orientation + MathF.PI));
            IEnumerable<int> ExhaustFire() {
                while (true) {
                    if (enemy.Velocity.LengthSquared() > 0.1f) {
                        Vector2 baseVel = enemy.Velocity.ScaleTo(-3).Rotate(rand.NextFloat(-0.3f, 0.3f));
                        EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, enemy.Position, Color.OrangeRed * 0.7f, 60f, new Vector2(0.5f, 1),
                            new ParticleState(baseVel, ParticleType.Enemy));
                    }
                    yield return 0;
                }
            }
            enemy.AddBehaviour(ExhaustFire());
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            return enemy;
        }
        public static Enemy CreatePinkWanderer(Vector2 position) {
            var enemy = new Enemy(Art.PinkWanderer, position, "PinkWanderer");
            IEnumerable<int> MoveOrthonallyOccasionally(int activeFrames = 30, int sleepFrames = 60, float speed = 2f) {
                while (true) {
                    Vector2 velocity = MathUtil.FromPolar(rand.Next(4) * MathF.PI / 2f, speed);
                    for (int i = 0; i < activeFrames; i++) {
                        enemy.Position += velocity;
                        yield return 0;
                    }
                    for (int i = 0; i < sleepFrames; i++) {
                        yield return 0;
                    }
                }
            }
            enemy.AddBehaviour(MoveOrthonallyOccasionally());
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
            IEnumerable<int> WalkInCircles() {
                float orientation = rand.NextFloat(0, MathF.PI * 2f);
                const float rotationSpeed = 0.1f;
                const float speed = 1f;
                while (true) {
                    enemy.Velocity += MathUtil.FromPolar(orientation, speed);
                    orientation += rotationSpeed;
                    yield return 0;
                }
            }
            enemy.AddBehaviour(WalkInCircles());
            enemy.AddBehaviour(enemy.EnemyFacesVelocity());
            IEnumerable<int> InvincibleForTime(int frames) {
                enemy.invincible = true;
                for (int i = 0; i < frames; i++) {
                    yield return 0;
                }
                enemy.invincible = false;
                while (true) {
                    yield return 0;
                }
            }
            enemy.AddBehaviour(InvincibleForTime(6));
            enemy.Radius = 10f;
            return enemy;
        }
        public static Enemy CreateBossOne(Vector2 position) {
            var enemy = new Enemy(Art.BossOne, position, "BossOne") {
                Health = 1000,
                Radius = 400,
                IsBoss = true
            };
            IEnumerable<int> UpdateBossBar() {
                while (true) {
                    Hud.bossBarFullness = enemy.Health / 1000f;
                    yield return 0;
                }
            }
            enemy.AddBehaviour(UpdateBossBar());
            enemy.AddBehaviour(enemy.RotateOrientationConstantly());
            for (int i = 0; i < 3; i++) {
                EntityManager.Add(CreateBossOneChild(position, MathF.PI * 2 * i / 3));
            }
            return enemy;
        }
        public static Enemy CreateBossOneChild(Vector2 centre, float initialRadians) {
            var enemy = new Enemy(Art.BossOneChild, centre, "BossOneChild") {
                invincible = true,
                IsBoss = true,
                Radius = 80
            };
            IEnumerable<int> BossChildOneAI(Vector2 centre, float radians) {
                float dist = 600f;
                while (true) {
                    for (int i = 0; i < 180; i++) {
                        // Idle rotate
                        radians += 0.05f;
                        enemy.Position = centre + new Vector2(dist, 0).Rotate(radians);
                        yield return 0;
                    }
                    for (int i = 0; i < 60; i++) {
                        // Extend
                        radians += 0.05f;
                        dist += 20f;
                        enemy.Position = centre + new Vector2(dist, 0).Rotate(radians);
                        yield return 0;
                    }
                    for (int i = 0; i < 180; i++) {
                        // Idle rotate
                        radians += 0.05f;
                        enemy.Position = centre + new Vector2(dist, 0).Rotate(radians);
                        yield return 0;
                    }
                    for (int i = 0; i < 60; i++) {
                        // Reduce
                        radians += 0.05f;
                        dist -= 20f;
                        enemy.Position = centre + new Vector2(dist, 0).Rotate(radians);
                        yield return 0;
                    }
                }
            }
            enemy.AddBehaviour(BossChildOneAI(centre, initialRadians)); 
            return enemy;
        }
        #endregion CreateEnemies
    }
}
