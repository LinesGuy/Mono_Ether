using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public abstract class Entity {
        protected Texture2D Image = GlobalAssets.Default;
        public Color EntityColor = Color.White;
        public Vector2 Position = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;
        public float Orientation = 0f;
        public float Radius;
        public bool IsExpired = false;
        private Vector2 Size => Image.Size();
        public abstract void Update(GameTime gameTime);
        public virtual void Draw(SpriteBatch batch, Camera camera) {
            Vector2 screenPos = camera.WorldToScreen(Position);
            batch.Draw(Image, screenPos, null, EntityColor, Orientation + camera.Orientation, Size / 2f, camera.Zoom, 0, 0);
        }
        public virtual void HandleTilemapCollision() {
            /* Get tile that the entity is currently in */
            var tile = TileMap.Instance.GetTileFromWorld(Position);
            /* Return if entity is not in a solid tile */
            if (tile.Id <= 0)
                return;
            /* Temporary variable for the wall the player will be pushed to */
            Vector2 destination;
            /* Check for rare corner exception */
            if (tile.SolidCorners[0] || tile.SolidCorners[1] || tile.SolidCorners[2] || tile.SolidCorners[3]) {
                destination = tile.TopLeft;
                float distSquared = float.MaxValue;
                if (tile.SolidCorners[0] && Vector2.DistanceSquared(Position, tile.TopLeft) < distSquared) {
                    destination = tile.TopLeft + new Vector2(-0.001f);
                    distSquared = Vector2.DistanceSquared(Position, tile.TopLeft);
                }
                if (tile.SolidCorners[1] && Vector2.DistanceSquared(Position, tile.TopRight) < distSquared) {
                    destination = tile.TopRight + new Vector2(0, -0.001f);
                    distSquared = Vector2.DistanceSquared(Position, tile.TopRight);
                }
                if (tile.SolidCorners[2] && Vector2.DistanceSquared(Position, tile.BottomRight) < distSquared) {
                    destination = tile.BottomRight;
                    distSquared = Vector2.DistanceSquared(Position, tile.BottomRight);
                }
                if (tile.SolidCorners[3] && Vector2.DistanceSquared(Position, tile.BottomLeft) < distSquared) {
                    destination = tile.BottomLeft + new Vector2(-0.001f, 0);
                }
                if (Vector2.DistanceSquared(Position, destination) < (Tile.Length / 2f) * (Tile.Length / 2f)) {
                    Position = destination;
                    return;
                }
            }
            /* Find any valid destination wall */
            if (tile.SolidWalls[0])
                destination = tile.Left;
            else if (tile.SolidWalls[1])
                destination = tile.Top;
            else if (tile.SolidWalls[2])
                destination = tile.Right;
            else
                destination = tile.Bottom;
            /* Now find nearest destination wall */
            if (tile.SolidWalls[0] && Vector2.DistanceSquared(Position, tile.Left) < Vector2.DistanceSquared(Position, destination))
                destination = tile.Left;
            if (tile.SolidWalls[1] && Vector2.DistanceSquared(Position, tile.Top) < Vector2.DistanceSquared(Position, destination))
                destination = tile.Top;
            if (tile.SolidWalls[2] && Vector2.DistanceSquared(Position, tile.Right) < Vector2.DistanceSquared(Position, destination))
                destination = tile.Right;
            if (tile.SolidWalls[3] && Vector2.DistanceSquared(Position, tile.Bottom) < Vector2.DistanceSquared(Position, destination))
                destination = tile.Bottom;
            /* Now move entity to said destination wall */
            if (destination == tile.Left)
                Position.X = destination.X - 0.001f;
            else if (destination == tile.Top)
                Position.Y = destination.Y - 0.001f;
            else if (destination == tile.Right)
                Position.X = destination.X + 0.001f;
            else
                Position.Y = destination.Y + 0.001f;
            return;
        }
    }
}
