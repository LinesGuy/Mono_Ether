using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether.Ether
{
    class Enemy : Entity
    {
        private int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private readonly Random rand = new Random();

        private Enemy(Texture2D image, Vector2 position)
        {
            this.Image = image;
            Position = position;
            Radius = image.Width / 2f;
            Color = Color.Transparent;
        }
        public override void Update()
        {
            if (timeUntilStart <= 0)
            {
                ApplyBehaviours();
            }
            else
            {
                timeUntilStart--;
                Color = Color.White * (1 - timeUntilStart / 60f);
            }

            Position += Velocity;
            Velocity *= 0.8f;  // Friction
        }
        public void WasShot()
        {
            IsExpired = true;

            float hue1 = rand.NextFloat(0, 6);
            float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;
            Color color1 = ColorUtil.HsvToColor(hue1, 0.5f, 1);
            Color color2 = ColorUtil.HsvToColor(hue2, 0.5f, 1);

            for (var i = 0; i < 120; i++)
            {
                var speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));
                EtherRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }
        }
        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            behaviours.Add(behaviour.GetEnumerator());
        }
        private void ApplyBehaviours()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }
        IEnumerable<int> FollowPlayerAStar(float acceleration = 1f)
        {
            while (true)
            {
                // If enemy is with Map.cellSize units (e.g 25 units) from the player, move straight towards the player
                if (Vector2.DistanceSquared(PlayerShip.Instance.Position, Position) <= MyAStar.CellSize * MyAStar.CellSize)
                {
                    Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
                    if (Velocity != Vector2.Zero)
                        Orientation = Velocity.ToAngle();
                    yield return 0;
                }
                else // else, use AStar to move towards the player
                {
                    var path = MyAStar.AStar(Position, PlayerShip.Instance.Position);
                    // Instead of calculating a new path every frame or whatever, we will calculate a new path
                    // every second (60 frames)
                    // TODO: scale this for distance between enemy and player
                    for (int i = 0; i < 60; i++)
                    {
                        // If enemy is at current target position, update target position
                        if (Vector2.DistanceSquared(Position, path[0]) <= MyAStar.CellSize * MyAStar.CellSize)
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
            // ReSharper disable once IteratorNeverReturns
        }
        IEnumerable<int> MoveRandomly()
        {
            float direction = rand.NextFloat(0, MathHelper.TwoPi);
            while (true)
            {
                direction += rand.NextFloat(-0.1f, 0.1f);
                direction = MathHelper.WrapAngle(direction);

                for (int i = 0; i < 6; i++)
                {
                    Velocity += MathUtil.FromPolar(direction, 0.4f);
                    Orientation -= 0.01f;
                    yield return 0;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private IEnumerable<int> DodgeBullets(float distance = 100f, float acceleration = 1f)
        {
            while (true)
            {
                foreach (var bullet in EntityManager.Bullets)
                    if (Vector2.DistanceSquared(bullet.Position, Position) < distance * distance)
                        Velocity -= (bullet.Position - Position).ScaleTo(acceleration);
                yield return 0;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(Art.Seeker, position);
            //enemy.AddBehaviour(enemy.FollowPlayer());
            enemy.AddBehaviour(enemy.FollowPlayerAStar());

            return enemy;
        }
        public static Enemy CreateWanderer(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehaviour(enemy.MoveRandomly());
            //enemy.AddBehaviour(enemy.SocialDistance());
            enemy.AddBehaviour(enemy.DodgeBullets());
            return enemy;
        }
        public void HandleCollision(Enemy other)
        {
            var delta = Position - other.Position;
            Velocity += 10 * delta / (delta.LengthSquared() + 1);
            // ^ Push current enemy away from other enemy. The closer they are, the harder the push.
        }
    }
}
