using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
        public static Enemy CreateEnemy(EnemyType type, Vector2 position)
        {
            var enemy = new Enemy(type, position);
            switch (type) {
                case (EnemyType.BlueSeeker):
                    enemy.Image = _blueSeeker;
                    // TODO behavior
                    break;
                case EnemyType.PurpleWanderer:
                    enemy.Image = _purpleWanderer;
                    enemy.behaviors.Add(enemy.MoveRandomly().GetEnumerator());
                    enemy.behaviors.Add(enemy.RotateOrientationConstantly().GetEnumerator());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return enemy;
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

        private IEnumerable<int> MoveRandomly(float speed = 0.4f, float rotationSpeed = 0.1f, float bounds = 0.1f) {
            var direction = Rand.NextFloat(0, MathHelper.TwoPi);
            var acceleration = MyUtils.FromPolar(direction, speed);
            //float rotationDelta = 0f;
            var lastPos = Position;
            while (true) {
                if (Math.Abs(Position.X - lastPos.X) < 0.001)
                    acceleration.X = -acceleration.X;
                if (Math.Abs(Position.Y - lastPos.Y) < 0.001)
                    acceleration.Y = -acceleration.Y;

                lastPos = Position;
                /* TODO uncomment
                rotationDelta += _rand.NextFloat(-rotationSpeed, rotationSpeed);
                if (rotationDelta < -bounds)
                    rotationDelta = -bounds;
                if (rotationDelta > bounds)
                    rotationDelta = bounds;
                acceleration = acceleration.Rotate(rotationDelta);
                
                for (int i = 0; i < 6; i++) {
                    Velocity += acceleration;
                    yield return 0;
                }
                */
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
