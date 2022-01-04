using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public enum EnemyType { BlueSeeker, PurpleWanderer }
    public class Enemy : Entity {
        private static Texture2D BlueSeeker;
        private static Texture2D PurpleWanderer;

        public int TimeUntilStart = 60;
        public int Health;
        public int Worth;
        public EnemyType Type;
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private static readonly Random _rand = new Random();
        private Enemy(EnemyType type, Vector2 position) {
            if (type == EnemyType.BlueSeeker) Image = BlueSeeker;
            Type = type;
            Position = position;
            Health = 1; // TODO difficulty
            Radius = Image.Width / 2f;
            EnemyColor = Color.Transparent; // TODO remove this?
        }
        public override void Update(GameTime gameTime) {
            if (TimeUntilStart > 0) {
                TimeUntilStart--;
                EnemyColor = Color.White * (1 - TimeUntilStart / 60f);
                return;
            }
            /* Apply enemy behaviours */
            for (int i = 0; i < behaviours.Count; i++)
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);

            Position += Velocity;
            Velocity *= 0.8f;
        }
        private void AddBehaviour(IEnumerable<int> behaviour) {
            behaviours.Add(behaviour.GetEnumerator()); // TODO remove?
        }
        public static void LoadContent(ContentManager content) {
            BlueSeeker = content.Load<Texture2D>("Textures/GameScreen/Enemies/BlueSeeker");
            PurpleWanderer = content.Load<Texture2D>("Textures/GameScreen/Enemies/PurpleWanderer");
        }
        public static void UnloadContent() {
            BlueSeeker = null;
            PurpleWanderer = null;
        }
        private IEnumerable<int> MoveRandomly(float speed = 0.4f, float rotationSpeed = 0.1f, float bounds = 0.1f) {
            float direction = _rand.NextFloat(0, MathHelper.TwoPi);
            Vector2 acceleration = MyUtils.FromPolar(direction, speed);
            //float rotationDelta = 0f;
            Vector2 lastPos = Position;
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
    }
}
