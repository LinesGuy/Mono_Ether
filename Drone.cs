using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether {
    public enum DroneType
    { //TODO
    }

    public class Drone : Entity
    {
        public DroneType Type;
        private readonly List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        public int PlayerIndex;
        private static readonly Random Rand = new Random();
        public Drone(int playerIndex, string type) {
            Position = EntityManager.Players[playerIndex].Position;
            Type = type;
            PlayerIndex = playerIndex;
            Image = GlobalAssets.Drone;
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
                    if (Input.Mouse.LeftButton == ButtonState.Pressed && aim.LengthSquared() > 0 && cooldownRemaining <= 0 && !EntityManager.Players[playerIndex].IsDead && !EtherRoot.Instance.EditorMode) {
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
        public static Drone CreateGeomCollector(int playerIndex) {
            var drone = new Drone(playerIndex, "geomCollector");
            drone.AddBehaviour(drone.DroneFacesVelocity());
            IEnumerable<int> collectGeomsAStar() {
                // Also follow player when no geoms are to be found
                const float acceleration = 1.5f;

                while (true) {
                    // If Geom is within Map.Cellsize of drone, move straight towards it
                    var dash = false;
                    foreach (Geom geom in EntityManager.Geoms) {
                        if (Vector2.DistanceSquared(geom.Position, drone.Position) < MyAStar.CellSize * MyAStar.CellSize) {
                            drone.Velocity += (geom.Position - drone.Position).ScaleTo(acceleration);
                            if (drone.Velocity != Vector2.Zero)
                                drone.Orientation = drone.Velocity.ToAngle();
                            yield return 0;
                            dash = true;
                            break;
                        }
                    }
                    if (dash)
                        continue;
                    else {
                        Vector2 destination;
                        List<Geom> accessibleGeoms = EntityManager.Geoms.Where(geom => Map.GetTileFromWorld(geom.Position).TileId <= 0).ToList();
                        if (accessibleGeoms.Count == 0) {
                            // There are no accessible geoms, return to player
                            destination = EntityManager.Players[drone.PlayerIndex].Position;
                            // If player is within Map.Cellsize of drone, do nothing
                            if (Vector2.DistanceSquared(EntityManager.Players[drone.PlayerIndex].Position, drone.Position) < Map.cellSize * Map.cellSize)
                                yield return 0;
                        } else {
                            // Go to nearest accessible geom
                            Geom nearestGeom = accessibleGeoms[0];
                            foreach (Geom geom in accessibleGeoms) {
                                if (Vector2.DistanceSquared(drone.Position, geom.Position) < Vector2.DistanceSquared(drone.Position, nearestGeom.Position))
                                    nearestGeom = geom;
                            }
                            destination = nearestGeom.Position;
                        }
                        List<Vector2> path = MyAStar.AStar(drone.Position, destination);
                        for (int i = 0; i < 6; i++) { // 15 frames per path re-calculation
                            if (path is null) {
                                yield return 0;
                                continue;
                            }
                            // If drone is at current target position, update target position
                            if (Vector2.DistanceSquared(drone.Position, path[0]) <= Math.Pow(MyAStar.CellSize * 0.8f, 2))
                                path.RemoveAt(0);
                            // If path is empty, make a new path
                            if (path.Count <= 0)
                                break;
                            // Move towards destination
                            drone.Velocity += (path[0] - drone.Position).ScaleTo(acceleration);
                            yield return 0;
                        }
                    }
                }
            }
            drone.AddBehaviour(collectGeomsAStar());
            return drone;
        }
        #endregion CreateDrones
    }
}
}
