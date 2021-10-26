using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Mono_Ether.Ether {
    class Drone : Entity{
        public string Type;
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        public int PlayerIndex;
        private static readonly Random Rand = new Random();
        public Drone(int playerIndex, string type) {
            Position = EntityManager.Players[playerIndex].Position;
            Type = type;
            PlayerIndex = playerIndex;
            Image = Art.Drone;
            Radius = Image.Width / 2f;
        }
        public override void Update() {
            ApplyBehaviours();
            Position += Velocity;
            Velocity *= 0.8f;
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
        private IEnumerable<int> CirclePlayer(float rotationSpeed = 0.07f) {
            const float Radius = 80f;
            float Radians = 0f;
            float RotationSpeed = rotationSpeed;
            while (true) {
                Radians += RotationSpeed;
                Radians %= MathF.PI * 2;
                Position = EntityManager.Players[PlayerIndex].Position + new Vector2(Radius, 0).Rotate(Radians);
                yield return 0;
            }
        }
        private IEnumerable<int> DroneFacesVelocity() {
            Vector2 lastPos = Position;
            while (true) {
                Vector2 delta = Position - lastPos;
                if (delta != Vector2.Zero)
                    Orientation = delta.ToAngle();
                lastPos = Position;
                yield return 0;
            }
        }
        
        #endregion IEnumerables
        #region CreateDrones
        public static Drone CreateShooter(int playerIndex) {
            var drone = new Drone(playerIndex, "shooter");
            drone.AddBehaviour(drone.CirclePlayer());
            drone.AddBehaviour(drone.DroneFacesVelocity());
            IEnumerable<int> ShootWhenPlayerShoots() {
                int cooldownRemaining = 0;
                const int CooldownFrames = 7;
                while (true) {
                    Vector2 aim = Camera.GetMouseAimDirection(drone.Position);
                    if (cooldownRemaining > 0)
                        cooldownRemaining--;
                    if (Input.mouse.LeftButton == ButtonState.Pressed && aim.LengthSquared() > 0 && cooldownRemaining <= 0) {
                        // Sound
                        Sounds.PlayerShoot.Play(GameSettings.SoundEffectVolume / 3f, Rand.NextFloat(-0.2f, 0.2f), 0);
                        cooldownRemaining = CooldownFrames;
                        float aimAngle = aim.ToAngle();
                        Vector2 offset = MathUtil.FromPolar(aimAngle, 32f);
                        Vector2 vel = MathUtil.FromPolar(aimAngle, 18f);
                        EntityManager.Add(new Bullet(drone.Position + offset, vel, Color.HotPink, drone.PlayerIndex));
                    }
                    yield return 0;
                }
            }
            drone.AddBehaviour(ShootWhenPlayerShoots());
            return drone;
        }
        #endregion CreateEnemies
    }
}
