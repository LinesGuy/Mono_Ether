using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public class SnakeHead : Enemy {
        public List<SnakeTail> Tail = new List<SnakeTail>();
        public SnakeHead(Vector2 position) : base(EnemyType.SnakeHead, position) {
            Image = SnakeHead;
            AddBehaviour(MoveRandomly(1f, 0.3f, 0.3f));
            AddBehaviour(EnemyFacesVelocity());
            var tailLength = Rand.Next(5, 15);
            for (var i = 0; i <= tailLength; i++)
                Tail.Add(new SnakeTail(position));
            AddBehaviour(UpdateTail());
        }
        private IEnumerable<int> UpdateTail(float distance = 20f) {
            while (true) {
                Tail[0].Position = Position;
                for (var i = 1; i < Tail.Count; i++) {
                    var bodyTail = Tail[i];
                    if (bodyTail.TimeUntilStart > 0) {
                        bodyTail.TimeUntilStart--;
                        bodyTail.EntityColor = Color.White * (1 - TimeUntilStart / 60f);
                    }
                    if (Vector2.DistanceSquared(bodyTail.Position, Tail[i - 1].Position) > distance * distance) {
                        bodyTail.Position = Tail[i - 1].Position + (bodyTail.Position - Tail[i - 1].Position).ScaleTo(distance);
                    }
                    /* Apply enemy behaviors */
                    for (var j = 0; j < bodyTail.Behaviors.Count; j++)
                        if (!bodyTail.Behaviors[j].MoveNext())
                            bodyTail.Behaviors.RemoveAt(i--);
                }
                yield return 0;
            }
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            /* Update tail */
            foreach (var tail in Tail)
                tail.HandleTilemapCollision();
        }

        public override void Draw(SpriteBatch batch, Camera camera) {
            base.Draw(batch, camera);
            /* Draw tail */
            foreach (var tail in Tail)
                tail.Draw(batch, camera);
        }
    }
    public class SnakeTail : Enemy {
        public SnakeTail(Vector2 position) : base(EnemyType.SnakeTail, position) {
            Image = SnakeTail;
            AddBehaviour(EnemyFacesVelocity());
        }
    }
    public class PinkSeekerChild : Enemy {
        public PinkSeekerChild(Vector2 position) : base(EnemyType.PinkSeekerChild, position) {
            Image = PinkSeekerChild;
            AddBehaviour(WalkInCircles());
            AddBehaviour(EnemyFacesVelocity());
            AddBehaviour(InvincibleForTime(6)); // This enemy will only spawn when the parent enemy dies, so we wait 6 frames so any bullets have passed by
            // TODO modify texture so radius is 10f
        }
        private IEnumerable<int> WalkInCircles() {
            var orientation = Rand.NextFloat(0, MathF.PI * 2f);
            while (true) {
                Velocity += MyUtils.FromPolar(orientation, 1f);
                orientation += 0.1f;
                yield return 0;
            }
        }
        private IEnumerable<int> InvincibleForTime(int frames) {
            Invincible = true;
            for (var i = 0; i < frames; i++) {
                yield return 0;
            }
            Invincible = false;
            while (true) {
                yield return 0;
            }
        }
    }
    public class BossOne : Enemy {
        public BossOne(Vector2 position) : base(EnemyType.BossOne, position) {
            Image = BossOne;
            Health = 100; // TODO affected by health multiplier and set to 1000
            TimeUntilStart = 0;
            EntityColor = Color.White;
            AddBehaviour(UpdateBossBar());
            AddBehaviour(RotateOrientationConstantly());
            for (var i = 0; i < 3; i++) {
                EntityManager.Instance.Add(new BossOneChild(position, MathF.PI * 2 * i / 3));
            }
            IsBoss = true;
        }
        protected override void WasKilled(PlayerIndex playerIndex) {
            IsExpired = true;
            /* Add score to player */
            EntityManager.Instance.Players[(int)playerIndex].Score += Worth;
            /* Summon particles */
            ParticleTemplates.Explosion(Position, 0f, 20f, 1000, Color.White, true);
            /* Trigger win screen */
            // TODO
        }
    }
    public class BossOneChild : Enemy {
        public BossOneChild(Vector2 position, float radians) : base(EnemyType.BossOneChild, position) {
            Image = BossOneChild;
            IsBoss = true;
            TimeUntilStart = 0;
            EntityColor = Color.White;
            IsInvincible = true;
            AddBehaviour(BossChildOneAI(position, radians));
        }
        private IEnumerable<int> BossChildOneAI(Vector2 centre, float radians) {
            var dist = 600f;
            while (true) {
                for (var i = 0; i < 180; i++) { // Idle rotate
                    radians += 0.05f;
                    Position = centre + new Vector2(dist, 0).Rotate(radians);
                    yield return 0;
                }
                for (var i = 0; i < 60; i++) { // Extend
                    radians += 0.05f;
                    dist += 20f;
                    Position = centre + new Vector2(dist, 0).Rotate(radians);
                    yield return 0;
                }
                for (var i = 0; i < 180; i++) { // Idle rotate
                    radians += 0.05f;
                    Position = centre + new Vector2(dist, 0).Rotate(radians);
                    yield return 0;
                }
                for (var i = 0; i < 60; i++) { // Reduce
                    radians += 0.05f;
                    dist -= 20f;
                    Position = centre + new Vector2(dist, 0).Rotate(radians);
                    yield return 0;
                }
            }
        }
    }
    public class BossTwo : Enemy {
        public List<BossTwoTail> Tail = new List<BossTwoTail>();
        public BossTwo(Vector2 position) : base(EnemyType.BossTwo, position) {
            Image = BossTwoHead;
            IsBoss = true;
            TimeUntilStart = 0;
            EntityColor = Color.White;
            Health = 300;
            AddBehaviour(_followCursor());
            AddBehaviour(UpdateBossBar());
            //AddBehaviour(enemy.MoveRandomly(1f, 0.3f, 0.3f));
            AddBehaviour(EnemyFacesVelocity());
            for (var i = 0; i <= 40; i++)
                Tail.Add(new BossTwoTail(position));
            AddBehaviour(UpdateTail(40));
        }
        private IEnumerable<int> _followCursor() {
            while (true) {
                Vector2 delta = (EntityManager.Instance.Players[0].PlayerCamera.MouseWorldCoords() - Position).ScaleTo(3f);
                Velocity += delta;
                yield return 0;
            }
        }
        private IEnumerable<int> UpdateTail(float distance = 20f) {
            while (true) {
                Tail[0].Position = Position;
                for (var i = 1; i < Tail.Count; i++) {
                    var bodyTail = Tail[i];
                    if (bodyTail.TimeUntilStart > 0) {
                        bodyTail.TimeUntilStart--;
                        bodyTail.EntityColor = Color.White * (1 - TimeUntilStart / 60f);
                    }
                    if (Vector2.DistanceSquared(bodyTail.Position, Tail[i - 1].Position) > distance * distance) {
                        bodyTail.Position = Tail[i - 1].Position + (bodyTail.Position - Tail[i - 1].Position).ScaleTo(distance);
                    }
                    /* Apply enemy behaviors */
                    for (var j = 0; j < bodyTail.Behaviors.Count; j++)
                        if (!bodyTail.Behaviors[j].MoveNext())
                            bodyTail.Behaviors.RemoveAt(i--);
                }
                yield return 0;
            }
        }
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            /* Update tail */
            foreach (var tail in Tail)
                tail.HandleTilemapCollision();
        }
        public override void Draw(SpriteBatch batch, Camera camera) {
            base.Draw(batch, camera);
            /* Draw tail */
            foreach (var tail in Tail)
                tail.Draw(batch, camera);
        }
    }
    public class BossTwoTail : Enemy {
        public BossTwoTail(Vector2 position) : base(EnemyType.BossTwoTail, position) {
            IsBoss = true;
            TimeUntilStart = 0;
            EntityColor = Color.White;
            Image = BossTwoTail;
            AddBehaviour(EnemyFacesVelocity());
        }
    }
    public class BossThree : Enemy {
        public BossThree(Vector2 position) : base(EnemyType.BossThree, position) {
            IsBoss = true;
            Image = BossThree;
            TimeUntilStart = 0;
            EntityColor = Color.White;
        }
    }
}
