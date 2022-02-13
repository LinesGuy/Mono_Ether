using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
// ReSharper disable All

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
            for (var i = 0; i < 3; i++)
                EntityManager.Instance.Add(new BossOneChild(position, MathF.PI * 2 * i / 3));
            IsBoss = true;
        }
        protected override void WasKilled(PlayerIndex playerIndex) {
            IsExpired = true;
            EntityManager.Instance.Players[(int)playerIndex].Score += Worth; // Add score to player
            ParticleTemplates.Explosion(Position, 0f, 20f, 1000, Color.White, true); // Summon particles
            EntityManager.Instance.Enemies.ForEach(e => e.Suicide());
            EntityManager.Instance.PowerPacks.Clear();
            PowerPackSpawner.Instance.Enabled = false;
            EnemySpawner.Enabled = false;
            Hud.Instance.ChangeStatus(HudStatus.Win);

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
            //AddBehaviour(_followCursor());
            AddBehaviour(UpdateBossBar());
            AddBehaviour(MoveRandomly(1f, 0.3f, 0.3f));
            AddBehaviour(EnemyFacesVelocity());
            for (var i = 0; i <= 100; i++)
                Tail.Add(new BossTwoTail(position));
            AddBehaviour(UpdateTail(40));
        }
        private IEnumerable<int> _followCursor() {
            while (true) {
                var delta = (EntityManager.Instance.Players[0].PlayerCamera.MouseWorldCoords() - Position).ScaleTo(3f);
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
        protected override void WasKilled(PlayerIndex playerIndex) {
            IsExpired = true;
            EntityManager.Instance.Players[(int)playerIndex].Score += Worth; // Add score to player
            ParticleTemplates.Explosion(Position, 0f, 20f, 1000, Color.White, true); // Summon particles
            EntityManager.Instance.Enemies.ForEach(e => e.Suicide());
            EntityManager.Instance.PowerPacks.Clear();
            PowerPackSpawner.Instance.Enabled = false;
            EnemySpawner.Enabled = false;
            Hud.Instance.ChangeStatus(HudStatus.Win);
        }
    }
    public class BossTwoTail : SnakeTail {
        public BossTwoTail(Vector2 position) : base(position) {
            IsBoss = true;
            TimeUntilStart = 0;
            EntityColor = Color.White;
            Image = BossTwoTail;
            AddBehaviour(EnemyFacesVelocity());
        }
    }
    public class BossThree : Enemy {
        private readonly int _size;
        public BossThree(Vector2 position, int size) : base(EnemyType.BossThree, position) {
            IsBoss = true;
            _size = size;
            if (size == 7) Hud.Instance.BossBarValue = 1f;
            Image = BossThree[size];
            Friction = 0.95f;
            TimeUntilStart = 0;
            EntityColor = Color.White;
            Health = (int) MyUtils.Interpolate(1, 10, size / 7f);
            AddBehaviour(MoveRandomly(0.4f));
            AddBehaviour(RotateOrientationConstantly());
        }
        protected override void WasKilled(PlayerIndex playerIndex) {
            base.WasKilled(playerIndex);
            if (_size <= 0)
                return;
            for (var i = 0; i < 2; i++) // Summon two smaller clones of this boss
                EntityManager.Instance.Add(new BossThree(Position, _size - 1));
            Hud.Instance.BossBarValue -= 1f / 127f;
            if (Hud.Instance.BossBarValue <= 0.01f)
                Hud.Instance.ChangeStatus(HudStatus.Win);
        }
    }
}
