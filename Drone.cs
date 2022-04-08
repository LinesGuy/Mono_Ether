using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    public enum DroneType { Shooter, Collector, Defender }

    public class Drone : Entity {
        private static Texture2D _shooterTexture;
        private static Texture2D _collectorTexture2D;
        private static Texture2D _defenderTexture;
        private static readonly Random Rand = new Random();
        public DroneType Type;
        private readonly List<IEnumerator<int>> _behaviors = new List<IEnumerator<int>>();
        public PlayerIndex OwnerPlayerIndex;
        private static readonly Random Random = new Random();
        public Drone(PlayerIndex ownerPlayerIndex, DroneType type) {
            OwnerPlayerIndex = ownerPlayerIndex;
            Type = type;
            // Drone position is initially equal to owner position
            Position = EntityManager.Instance.Players[(int)ownerPlayerIndex].Position;
            // Change drone texture based on the drone type
            switch (type) {
                case DroneType.Collector:
                    Image = _collectorTexture2D;
                    break;
                case DroneType.Shooter:
                    Image = _shooterTexture;
                    break;
                case DroneType.Defender:
                    Image = _defenderTexture;
                    break;
            }
        }
        public static void LoadContent(ContentManager content) {
            _shooterTexture = content.Load<Texture2D>("Textures/GameScreen/Drones/Shooter");
            _collectorTexture2D = content.Load<Texture2D>("Textures/GameScreen/Drones/Collector");
            _defenderTexture = content.Load<Texture2D>("Textures/GameScreen/Drones/Defender");
        }
        public static void UnloadContent() {
            _shooterTexture = null;
            _collectorTexture2D = null;
            _defenderTexture = null;
        }

        public override void Update(GameTime gameTime) {
            /* Apply enemy behaviors */
            for (var i = 0; i < _behaviors.Count; i++)
                if (!_behaviors[i].MoveNext())
                    _behaviors.RemoveAt(i--);
            // Update position based on velocity
            Position += Velocity;
            // Friction
            Velocity *= 0.8f;
        }

        private void AddBehaviour(IEnumerable<int> behaviour) {
            _behaviors.Add(behaviour.GetEnumerator());
        }
        #region IEnumerables
        private IEnumerable<int> CollideWithTilemap() {
            while (true) {
                HandleTilemapCollision();
                yield return 0;
            }
        }
        private IEnumerable<int> CirclePlayer(float rotationSpeed = 0.07f) {
            // Follow the player, and if within range of the player, move in circles around the player
            const float radius = 80f;
            var radians = 0f;
            while (true) {
                radians += rotationSpeed;
                radians %= MathF.PI * 2;
                Position = EntityManager.Instance.Players[(int)OwnerPlayerIndex].Position + new Vector2(radius, 0).Rotate(radians);
                yield return 0;
            }
        }
        private IEnumerable<int> DroneFacesVelocity() {
            var lastPos = Position;
            while (true) {
                var delta = Position - lastPos;
                if (delta != Vector2.Zero)
                    Orientation = delta.ToAngle();
                lastPos = Position;
                yield return 0;
            }
        }
        #endregion IEnumerables
        #region CreateDrones
        public static Drone CreateShooter(PlayerIndex ownerPlayerIndex) {
            var drone = new Drone(ownerPlayerIndex, DroneType.Shooter);
            drone.AddBehaviour(drone.CirclePlayer());
            drone.AddBehaviour(drone.DroneFacesVelocity());
            IEnumerable<int> ShootWhenPlayerShoots() {
                var cooldownRemaining = 0;
                // Only shoot every cooldownFrames number
                const int cooldownFrames = 7;
                while (true) {
                    // Get the player with index of the stored owner player index
                    var player = EntityManager.Instance.Players[(int)ownerPlayerIndex];
                    // Get the direction to shoot (mouse position minus drone position)
                    var aim = player.PlayerCamera.MouseWorldCoords() - drone.Position;
                    if (cooldownRemaining > 0)
                        cooldownRemaining--;
                    if (Input.Mouse.LeftButton == ButtonState.Pressed && aim.LengthSquared() > 0 && cooldownRemaining <= 0 && !player.IsDead && GameScreen.Instance.Mode != GameMode.Editor) {
                        // Play a sound effect of random pitch
                        PlayerShip.ShotSoundEffect.Play(GameSettings.SoundEffectVolume / 3f, Rand.NextFloat(-0.2f, 0.2f), 0);
                        // Reset cooldown
                        cooldownRemaining = cooldownFrames;
                        var aimAngle = aim.ToAngle();
                        var offset = MyUtils.FromPolar(aimAngle, 32f);
                        var vel = MyUtils.FromPolar(aimAngle, 18f);
                        // Add this bullet to the list of bullets
                        EntityManager.Instance.Add(new Bullet(drone.Position + offset, vel, Color.HotPink, drone.OwnerPlayerIndex));
                    }
                    yield return 0;
                }
            }
            drone.AddBehaviour(ShootWhenPlayerShoots());
            return drone;
        }
        public static Drone CreateCollector(PlayerIndex ownerPlayerIndex) {
            var drone = new Drone(ownerPlayerIndex, DroneType.Collector);
            drone.AddBehaviour(drone.DroneFacesVelocity());
            // The AStar algorithm used here is almost identical to the AStar used in the enemy pathfinding,
            // the main difference is that instead of having just one goal, there are multiple goals (one for
            // each geom), and the algorithm simply finds the path to the nearest one.
            IEnumerable<int> CollectGeomsAStar() {
                // Also follow player when no geoms are to be found
                const float acceleration = 1.5f;
                while (true) {
                    // If Geom is within Map.Cellsize of drone, move straight towards it
                    var dash = false;
                    foreach (Geom geom in EntityManager.Instance.Geoms) {
                        if (!(Vector2.DistanceSquared(geom.Position, drone.Position) <
                              Math.Pow(Tile.Length, 2))) continue;
                        drone.Velocity += (geom.Position - drone.Position).ScaleTo(acceleration);
                        if (drone.Velocity != Vector2.Zero)
                            drone.Orientation = drone.Velocity.ToAngle();
                        yield return 0;
                        dash = true;
                        break;
                    }
                    if (dash)
                        continue;
                    else {
                        Vector2 destination;
                        var accessibleGeoms = EntityManager.Instance.Geoms.Where(geom => TileMap.Instance.GetTileFromWorld(geom.Position).Id <= 0).ToList();
                        if (accessibleGeoms.Count == 0) {
                            // There are no accessible geoms, return to player
                            destination = EntityManager.Instance.Players[(int)drone.OwnerPlayerIndex].Position;
                            // If player is within Map.Cellsize of drone, do nothing
                            if (Vector2.DistanceSquared(EntityManager.Instance.Players[(int)drone.OwnerPlayerIndex].Position, drone.Position) < Math.Pow(Tile.Length, 2))
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
                        List<Vector2> path = TileMap.Instance.AStar(drone.Position, destination);
                        for (int i = 0; i < 6; i++) { // 6 frames per path re-calculation
                            if (path is null) {
                                yield return 0;
                                continue;
                            }
                            // If drone is at current target position, update target position
                            if (Vector2.DistanceSquared(drone.Position, path[0]) <= Math.Pow(Tile.Length * 0.8f, 2))
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
            drone.AddBehaviour(CollectGeomsAStar());
            drone.AddBehaviour(drone.CollideWithTilemap());
            return drone;
        }
        #endregion CreateDrones
    }
}
