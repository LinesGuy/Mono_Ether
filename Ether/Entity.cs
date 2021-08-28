using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether.Ether
{
    internal abstract class Entity
    {
        protected Texture2D Image = Art.Default;  // Tint of image
        protected Color Color = Color.White;

        public Vector2 Position;
        protected Vector2 Velocity;
        protected float Orientation;
        public float Radius = 20;  // Used for circular collision detection
        public bool IsExpired;  // If true, entity will be removed on next update

        private Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var screenPos = Camera.world_to_screen_pos(Position);
            //spriteBatch.Draw(image, screen_pos, null, color, Orientation + Camera.orientation, Size / 2f, Camera.zoom, 0, 0);
            spriteBatch.Draw(Image, screenPos, null, Color, Orientation, Size / 2f, Camera.Zoom, 0, 0);
        }

        public void HandleTilemapCollision()
        {
            var tile = Map.GetTileFromWorld(Position);

            // Return if entity is not in a solid tile
            if (tile.TileId <= 0)
                return;

            Vector2 topLeft = tile.pos * Map.cellSize; // World coords of top left of tile

            // Generate left-middle, upper-middle etc world coords of tile
            Vector2 left = new Vector2(topLeft.X, topLeft.Y + Map.cellSize / 2f);
            Vector2 up = new Vector2(topLeft.X + Map.cellSize / 2f, topLeft.Y);
            Vector2 right = new Vector2(topLeft.X + Map.cellSize, topLeft.Y + Map.cellSize / 2f);
            Vector2 down = new Vector2(topLeft.X + Map.cellSize / 2f, topLeft.Y + Map.cellSize);

            Vector2 destination; // The destination wall that the entity will be moved to

            // Check for rare corner exception
            if (tile.Walls[4] || tile.Walls[5] || tile.Walls[6] || tile.Walls[7])
            {
                if (tile.Walls[4])
                    destination = tile.TopLeft;
                else if (tile.Walls[5])
                    destination = tile.TopRight;
                else if (tile.Walls[6])
                    destination = tile.BottomRight;
                else
                    destination = tile.BottomLeft;

                if (Vector2.DistanceSquared(Position, destination) < (Map.cellSize / 2f) * (Map.cellSize / 2f))
                {
                    Position = destination;
                    return;
                }
            }

            // Find any valid destination wall
            if (tile.Walls[0])
                destination = left;
            else if (tile.Walls[1])
                destination = up;
            else if (tile.Walls[2])
                destination = right;
            else
                destination = down;

            // Now find nearest destination wall
            if (tile.Walls[0] && Vector2.DistanceSquared(Position, left) < Vector2.DistanceSquared(Position, destination))
                destination = left;
            if (tile.Walls[1] && Vector2.DistanceSquared(Position, up) < Vector2.DistanceSquared(Position, destination))
                destination = up;
            if (tile.Walls[2] && Vector2.DistanceSquared(Position, right) < Vector2.DistanceSquared(Position, destination))
                destination = right;
            if (tile.Walls[3] && Vector2.DistanceSquared(Position, down) < Vector2.DistanceSquared(Position, destination))
                destination = down;

            // Now move entity to said destination wall
            if (destination == left)
                Position.X = destination.X - 0.001f;
            else if (destination == up)
                Position.Y = destination.Y - 0.001f;
            else if (destination == right)
                Position.X = destination.X;
            else
                Position.Y = destination.Y;

            return; 
            /*
            // Check if we are still inside a tile
            tile = Map.GetTileFromWorld(Position);
            if (tile.TileId <= 0)
                return;

            // At this point we are still inside a tile so we perform a second iteration
            // TODO: The following code is almost identical to the above code, there must be same way to eliminate the redundancy
            Vector2 blockedDestination = destination;
            topLeft = tile.pos * Map.cellSize;

            left = new Vector2(topLeft.X, topLeft.Y + Map.cellSize / 2f);
            up = new Vector2(topLeft.X + Map.cellSize / 2f, topLeft.Y);
            right = new Vector2(topLeft.X + Map.cellSize, topLeft.Y + Map.cellSize / 2f);
            down = new Vector2(topLeft.X + Map.cellSize / 2f, topLeft.Y + Map.cellSize);

            // Find any valid destination wall
            if (tile.Walls[0] && left != blockedDestination)
                destination = left;
            else if (tile.Walls[1] && up != blockedDestination)
                destination = up;
            else if (tile.Walls[2] && right != blockedDestination)
                destination = right;
            else
                destination = down;

            // Now find nearest destination wall
            if (Vector2.DistanceSquared(Position, left) < Vector2.DistanceSquared(Position, destination) && left != blockedDestination)
                destination = left;
            if (Vector2.DistanceSquared(Position, up) < Vector2.DistanceSquared(Position, destination) && up != blockedDestination)
                destination = up;
            if (Vector2.DistanceSquared(Position, right) < Vector2.DistanceSquared(Position, destination) && right != blockedDestination)
                destination = right;
            if (Vector2.DistanceSquared(Position, down) < Vector2.DistanceSquared(Position, destination) && down != blockedDestination)
                destination = down;

            // Now move entity to said destination wall
            if (destination == left)
                Position.X = destination.X - 0.001f;
            else if (destination == up)
                Position.Y = destination.Y - 0.001f;
            else if (destination == right)
                Position.X = destination.X;
            else
                Position.Y = destination.Y;
            */
        }
    }
}
