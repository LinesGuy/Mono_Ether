using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    public enum EnemyType { BlueSeeker, PurpleWanderer }
    public class Enemy : Entity {
        private static Texture2D _blueSeeker;
        private static Texture2D _purpleWanderer;

        public int TimeUntilStart = 60;
        public int Health;
        public int Worth;
        public EnemyType Type;
        private readonly List<IEnumerator<int>> behaviors = new List<IEnumerator<int>>();
        private static readonly Random Rand = new Random();
        private Enemy(EnemyType type, Vector2 position) {
            Type = type;
            Position = position;
            Health = 1; // TODO difficulty
            Radius = Image.Width / 2f;
            EnemyColor = Color.Transparent; // TODO remove this?
        }
        public static Enemy CreateEnemy(EnemyType type, Vector2 position) {
            var enemy = new Enemy(type, position);
            switch (type) {
                case (EnemyType.BlueSeeker):
                    enemy.Image = _blueSeeker;
                    enemy.AddBehaviour(enemy.FollowPlayerAStar(0.8f));
                    // TODO face velocity
                    break;
                case EnemyType.PurpleWanderer:
                    enemy.Image = _purpleWanderer;
                    enemy.AddBehaviour(enemy.MoveRandomly());
                    enemy.AddBehaviour(enemy.RotateOrientationConstantly());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return enemy;
        }
        private void AddBehaviour(IEnumerable<int> behaviour) {
            behaviors.Add(behaviour.GetEnumerator());
        }
        public override void Update(GameTime gameTime) {
            if (TimeUntilStart > 0) {
                TimeUntilStart--;
                EnemyColor = Color.White * (1 - TimeUntilStart / 60f);
                return;
            }
            /* Apply enemy behaviors */
            for (int i = 0; i < behaviors.Count; i++)
                if (!behaviors[i].MoveNext())
                    behaviors.RemoveAt(i--);

            Position += Velocity;
            Velocity *= 0.8f;
        }
        public static void LoadContent(ContentManager content) {
            _blueSeeker = content.Load<Texture2D>("Textures/GameScreen/Enemies/BlueSeeker");
            _purpleWanderer = content.Load<Texture2D>("Textures/GameScreen/Enemies/PurpleWanderer");
        }
        public static void UnloadContent() {
            _blueSeeker = null;
            _purpleWanderer = null;
        }
        private IEnumerable<int> FollowPlayerAStar(float acceleration) {
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
                if (dash) continue;
                if (players.TrueForAll(player => TileMap.Instance.GetTileFromWorld(player.Position).Id > 0)) {
                    /* All players are in solid tiles, do nothing */
                    yield return 0;
                } else {
                    /* Use A* to move towards the nearest player */
                    var path = TileMap.Instance.AStar(Position, nearestPlayer.Position);
                    /* Instead of calculating a new path every frame or whatever, we will calculate a new path
                     * based on how far the enemy is from the player: */
                    var timeUntilNextCalculation = 240;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 2500 * 2500) timeUntilNextCalculation = 120;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 1500 * 1500) timeUntilNextCalculation = 60;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 750 * 750) timeUntilNextCalculation = 20;
                    if (Vector2.DistanceSquared(Position, nearestPlayer.Position) < 300 * 300) timeUntilNextCalculation = 10;
                    for (var i = 0; i < timeUntilNextCalculation; i++) {
                        if (path is null) {
                            yield return 0;
                            continue;
                        }
                        /* If enemy is at current target position, update target position */
                        if (Vector2.DistanceSquared(Position, path[0]) <= Math.Pow(Tile.Length * 0.8f, 2))
                            path.RemoveAt(0);
                        /* If path is empty, make a new path */
                        if (path.Count <= 0) break;
                        /* Move towards player */
                        Velocity += (path[0] - Position).ScaleTo(acceleration);
                        
                        yield return 0;
                    }
                }
            }
        }
        private IEnumerable<int> MoveRandomly(float speed = 0.4f, float rotationSpeed = 0.1f, float bounds = 0.1f) {
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
        private IEnumerable<int> RotateOrientationConstantly(float speed = 0.1f) {
            while (true) {
                Orientation += speed;
                yield return 0;
            }
        }
    }
}
