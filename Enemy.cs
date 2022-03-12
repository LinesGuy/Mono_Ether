using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mono_Ether {
    public enum EnemyType { BlueSeeker, PurpleWanderer, GreenSeeker, BackAndForther, PinkSeeker, PinkSeekerChild, PinkWanderer, SnakeHead, SnakeTail, BossOne, BossOneChild, BossTwo, BossTwoTail, BossThree }
    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    public class Enemy : Entity {
        private static Texture2D _blueSeeker;
        private static Texture2D _purpleWanderer;
        private static Texture2D _greenSeeker;
        private static Texture2D _backAndForther;
        private static Texture2D _pinkSeeker;
        private static Texture2D _pinkWanderer;
        protected static Texture2D SnakeHead;
        protected static Texture2D SnakeTail;
        protected static Texture2D BossOne;
        protected static Texture2D BossOneChild;
        protected static Texture2D BossTwoHead;
        protected static Texture2D BossTwoTail;
        protected static Texture2D[] BossThree;
        protected static Texture2D PinkSeekerChild;
        protected static SoundEffect DeathSound;
        protected float Friction = 0.8f;
        public int TimeUntilStart = 60;
        public int Health;
        public int Worth;
        public bool Invincible;
        public bool IsBoss;
        public bool IsInvincible;
        public EnemyType Type;
        protected internal readonly List<IEnumerator<int>> Behaviors = new List<IEnumerator<int>>();
        protected static readonly Random Rand = new Random();
        public bool IsActive => TimeUntilStart == 0;
        protected Enemy(EnemyType type, Vector2 position) {
            Type = type;
            Position = position;
            Health = 1;
            Worth = Rand.Next(50, 100);
            EntityColor = Color.Transparent;
        }
        public static Enemy CreateEnemy(EnemyType type, Vector2 position) {
            var enemy = new Enemy(type, position);
            switch (type) {
                case (EnemyType.BlueSeeker):
                    enemy.Image = _blueSeeker;
                    enemy.AddBehaviour(enemy.FollowPlayerAStar(0.8f));
                    enemy.AddBehaviour(enemy.EnemyFacesVelocity());
                    break;
                case EnemyType.PurpleWanderer:
                    enemy.Image = _purpleWanderer;
                    enemy.AddBehaviour(enemy.MoveRandomly());
                    enemy.AddBehaviour(enemy.RotateOrientationConstantly());
                    break;
                case EnemyType.BackAndForther:
                    enemy.Image = _backAndForther;
                    enemy.AddBehaviour(enemy.BounceOffWalls(enemy.Orientation + MathF.PI));
                    enemy.AddBehaviour(enemy.ExhaustFire());
                    enemy.AddBehaviour(enemy.EnemyFacesVelocity());
                    break;
                case EnemyType.GreenSeeker:
                    enemy.Image = _greenSeeker;
                    enemy.AddBehaviour(enemy.FollowPlayerAStar(1.2f));
                    enemy.AddBehaviour(enemy.DodgeBullets(100f, 1.5f));
                    enemy.AddBehaviour(enemy.EnemyFacesVelocity());
                    break;
                case EnemyType.PinkSeeker:
                    enemy.Image = _pinkSeeker;
                    enemy.AddBehaviour(enemy.FollowPlayerAStar(1.2f));
                    enemy.AddBehaviour(enemy.EnemyFacesVelocity());
                    break;
                case EnemyType.PinkWanderer:
                    enemy.Image = _pinkWanderer;
                    enemy.AddBehaviour(enemy.MoveOrthogonallyOccasionally());
                    enemy.AddBehaviour(enemy.EnemyFacesVelocity());
                    break;
                case EnemyType.SnakeHead:
                    return new SnakeHead(position);
                case EnemyType.BossOne:
                    break;
                case EnemyType.BossTwo:
                    break;
                case EnemyType.BossThree:
                    break;
                case EnemyType.BossOneChild:
                case EnemyType.BossTwoTail:
                case EnemyType.PinkSeekerChild:
                case EnemyType.SnakeTail:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return enemy;
        }
        public void HandleCollision(Enemy other) {
            var delta = Position - other.Position;
            /* Push current enemy away from other enemy. The closer they are, the harder the push */
            Velocity += 10 * delta / (delta.LengthSquared() + 1);
        }
        protected void AddBehaviour(IEnumerable<int> behaviour) {
            Behaviors.Add(behaviour.GetEnumerator());
        }
        public override void Update(GameTime gameTime) {
            if (TimeUntilStart > 0) {
                TimeUntilStart--;
                EntityColor = Color.White * (1 - TimeUntilStart / 60f);
                return;
            }
            /* Apply enemy behaviors */
            for (var i = 0; i < Behaviors.Count; i++)
                if (!Behaviors[i].MoveNext())
                    Behaviors.RemoveAt(i--);
            Position += Velocity;
            Velocity *= Friction;
            HandleTilemapCollision();
        }
        public static void LoadContent(ContentManager content) {
            _blueSeeker = content.Load<Texture2D>("Textures/GameScreen/Enemies/BlueSeeker");
            _purpleWanderer = content.Load<Texture2D>("Textures/GameScreen/Enemies/PurpleWanderer");
            _greenSeeker = content.Load<Texture2D>("Textures/GameScreen/Enemies/GreenSeeker");
            _backAndForther = content.Load<Texture2D>("Textures/GameScreen/Enemies/BackAndForther");
            _pinkSeeker = content.Load<Texture2D>("Textures/GameScreen/Enemies/PinkSeeker");
            _pinkWanderer = content.Load<Texture2D>("Textures/GameScreen/Enemies/PinkWanderer");
            SnakeHead = content.Load<Texture2D>("Textures/GameScreen/Enemies/SnakeHead");
            SnakeTail = content.Load<Texture2D>("Textures/GameScreen/Enemies/SnakeTail");
            BossOne = content.Load<Texture2D>("Textures/GameScreen/Enemies/BossOne");
            BossOneChild = content.Load<Texture2D>("Textures/GameScreen/Enemies/BossOneChild");
            BossTwoHead = content.Load<Texture2D>("Textures/GameScreen/Enemies/BossTwoHead");
            BossTwoTail = content.Load<Texture2D>("Textures/GameScreen/Enemies/BossTwoTail");
            var bossThreeList = new List<Texture2D>();
            for (var i = 0; i < 8; i++)
                bossThreeList.Add(content.Load<Texture2D>($"Textures/GameScreen/Enemies/BossThree/{i}"));
            BossThree = bossThreeList.ToArray();
            PinkSeekerChild = content.Load<Texture2D>("Textures/GameScreen/Enemies/PinkSeekerChild");
            DeathSound = content.Load<SoundEffect>("SoundEffects/EnemyDeath");
        }
        public static void UnloadContent() {
            _blueSeeker = null;
            _purpleWanderer = null;
            _greenSeeker = null;
            _backAndForther = null;
            _pinkSeeker = null;
            _pinkWanderer = null;
            SnakeHead = null;
            SnakeTail = null;
            BossOne = null;
            BossOneChild = null;
            BossTwoHead = null;
            BossTwoTail = null;
            BossThree = null;
            PinkSeekerChild = null;
            DeathSound = null;
        }
        protected IEnumerable<int> EnemyFacesVelocity() {
            var lastPos = Position;
            while (true) {
                var delta = Position - lastPos;
                if (delta != Vector2.Zero)
                    Orientation = delta.ToAngle();
                lastPos = Position;
                yield return 0;
            }
        }
        protected IEnumerable<int> DodgeBullets(float distance = 100f, float acceleration = 1f) {
            while (true) {
                foreach (var bullet in EntityManager.Instance.Bullets.Where(bullet => Vector2.DistanceSquared(bullet.Position, Position) < distance * distance))
                    Velocity -= (bullet.Position - Position).ScaleTo(acceleration);
                yield return 0;
            }
        }
        protected IEnumerable<int> BounceOffWalls(float angle, float speed = 1.7f) {
            var lastPos = Position;
            var acceleration = MyUtils.FromPolar(angle, speed);
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
        protected IEnumerable<int> ExhaustFire() {
            while (true) {
                if (Velocity.LengthSquared() > 0.1f) {
                    ParticleTemplates.ExhaustFire(Position, Velocity.ToAngle() + MathF.PI, new Color(1f, 0.5f, 0.2f));
                }
                yield return 0;
            }
        }
        protected IEnumerable<int> FollowPlayerAStar(float acceleration) {
            while (true) {
                /* Get nearest player from current position */
                var players = EntityManager.Instance.Players;
                var nearestPlayer = players.First();
                foreach (var player in players)
                    if (Vector2.DistanceSquared(Position, player.Position) < Vector2.DistanceSquared(Position, nearestPlayer.Position))
                        nearestPlayer = player;
                /* If enemy is within Map.cellSize units from the player, move straight towards the player */
                var dash = false;
                foreach (var player in players) {
                    var distSquared = Vector2.DistanceSquared(player.Position, Position);
                    if (distSquared <= MathF.Pow(Tile.Length, 2)) {
                        if (distSquared >= 0.1f) {
                            Velocity += (player.Position - Position).ScaleTo(acceleration);
                            if (Velocity != Vector2.Zero)
                                Orientation = Velocity.ToAngle();
                        }
                        yield return 0;
                        dash = true;
                        break;
                    }
                }
                if (dash)
                    continue;
                if (players.TrueForAll(player => TileMap.Instance.GetTileFromWorld(player.Position).Id > 0)) {
                    /* All players are in solid tiles, do nothing */
                    yield return 0;
                } else {
                    /* Use A* to move towards the nearest player */
                    var path = TileMap.Instance.AStar(Position, nearestPlayer.Position);
                    /* Instead of calculating a new path every frame or whatever, we will calculate a new path
                     * based on how far the enemy is from the player: */
                    var timeUntilNextCalculation = 240;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 2500 * 2500)
                        timeUntilNextCalculation = 120;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 1500 * 1500)
                        timeUntilNextCalculation = 60;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 750 * 750)
                        timeUntilNextCalculation = 20;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 300 * 300)
                        timeUntilNextCalculation = 10;
                    for (var i = 0; i < timeUntilNextCalculation; i++) {
                        if (path is null) {
                            yield return 0;
                            continue;
                        }
                        /* If enemy is at current target position, update target position */
                        if (Vector2.DistanceSquared(Position, path[0]) <= Math.Pow(Tile.Length * 0.8f, 2))
                            path.RemoveAt(0);
                        /* If path is empty, make a new path */
                        if (path.Count <= 0)
                            break;
                        /* Move towards player */
                        Velocity += (path[0] - Position).ScaleTo(acceleration);

                        yield return 0;
                    }
                }
            }
        }
        protected IEnumerable<int> MoveRandomly(float speed = 0.4f, float rotationSpeed = 0.1f, float bounds = 0.1f) {
            var direction = Rand.NextFloat(0, MathHelper.TwoPi);
            var acceleration = MyUtils.FromPolar(direction, speed);
            var rotationDelta = 0f;
            var lastPos = Position;
            while (true) {
                if (Math.Abs(Position.X - lastPos.X) < 0.001)
                    acceleration.X = -acceleration.X;
                if (Math.Abs(Position.Y - lastPos.Y) < 0.001)
                    acceleration.Y = -acceleration.Y;

                lastPos = Position;
                rotationDelta += Rand.NextFloat(-rotationSpeed, rotationSpeed);
                if (rotationDelta < -bounds)
                    rotationDelta = -bounds;
                if (rotationDelta > bounds)
                    rotationDelta = bounds;
                acceleration = acceleration.Rotate(rotationDelta);

                for (var i = 0; i < 6; i++) {
                    Velocity += acceleration;
                    yield return 0;
                }
                Velocity += acceleration;
                yield return 0;
            }
        }
        protected IEnumerable<int> RotateOrientationConstantly(float speed = 0.1f) {
            while (true) {
                Orientation += speed;
                yield return 0;
            }
        }

        protected IEnumerable<int> UpdateBossBar() {
            float maxHealth = Health;
            while (true) {
                Hud.Instance.BossBarValue = Health / maxHealth;
                yield return 0;
            }
        }
        protected IEnumerable<int> MoveOrthogonallyOccasionally(int activeFrames = 30, int sleepFrames = 60, float speed = 2f) {
            var velocity = MyUtils.FromPolar(Rand.Next(4) * MathF.PI / 2f, speed);
            while (true) {
                for (var i = 0; i < activeFrames; i++) {
                    Position += velocity;
                    yield return 0;
                }
                for (var i = 0; i < sleepFrames; i++) {
                    yield return 0;
                }
            }
        }
        public void WasShot(PlayerIndex playerIndex) {
            if (IsInvincible)
                return;
            Health--;
            if (Health <= 0)
                WasKilled(playerIndex);
        }

        protected virtual void WasKilled(PlayerIndex playerIndex) {
            IsExpired = true;
            /* Add score to player */
            EntityManager.Instance.Players[(int)playerIndex].Score += Worth;
            EntityManager.Instance.Add(new Geom(Position));
            DeathSound.Play(GameSettings.SoundEffectVolume, Rand.NextFloat(-0.2f, 0.2f), 0);
            /* Summon particles */
            ParticleTemplates.Explosion(Position, 5f, 10f, 30, Color.CornflowerBlue);
            /* If enemy was pink seeker, summon two pink seeker children */
            if (Type == EnemyType.PinkSeeker)
                for (var i = 0; i < 2; i++)
                    EntityManager.Instance.Add(new PinkSeekerChild(Position) {
                        Velocity = MyUtils.FromPolar(Rand.NextFloat(0, MathF.PI * 2), 10f),
                        TimeUntilStart = 0,
                        EntityColor = Color.White,
                        Invincible = true
                    });
        }
        public void Suicide() {
            IsExpired = true;
            ParticleTemplates.Explosion(Position, 5f, 10f, 30, Color.CornflowerBlue);
        }
    }
}
