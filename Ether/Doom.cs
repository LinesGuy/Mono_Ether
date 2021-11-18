using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether.Ether {
    public static class Doom {
        public static void Update() {

        }
        public static void Draw(SpriteBatch spriteBatch) {
            float fov = MathF.PI;
            for (int i = 0; i < (int)GameRoot.ScreenSize.X; i++) {

                float deltaAngle = MathUtil.Interpolate(-fov / 2f, fov / 2f, (GameRoot.ScreenSize.X - i) / GameRoot.ScreenSize.X);
                Vector2 p1Pos = EntityManager.Player1.Position;
                float p1Angle = EntityManager.Player1.Orientation;
                float rayAngle = p1Angle - deltaAngle;
                int step;
                const float maxDist = 100;
                float myPow = MathF.Pow(GameRoot.ScreenSize.Y, 1 / maxDist);
                float d = myPow;
                
                float texX = -1f;
                int tileId = -1;
                
                for (step = 0; step <= GameRoot.ScreenSize.Y; step++) {
                    //d = MathF.Pow(myPow, step); // / MathF.Cos(deltaAngle);
                    d *= myPow;
                    Vector2 currentRayPos = p1Pos + MathUtil.FromPolar(rayAngle, d);
                    tileId = Map.GetTileFromWorld(currentRayPos).TileId;
                    if (tileId > 0) {
                        var tile = Map.GetTileFromWorld(currentRayPos);
                        Vector2 topLeftTile = tile.pos * Map.cellSize; // World coords of top left of tile

                        // Generate left-middle, upper-middle etc world coords of tile
                        Vector2 left = new Vector2(topLeftTile.X, topLeftTile.Y + Map.cellSize / 2f);
                        Vector2 up = new Vector2(topLeftTile.X + Map.cellSize / 2f, topLeftTile.Y);
                        Vector2 right = new Vector2(topLeftTile.X + Map.cellSize, topLeftTile.Y + Map.cellSize / 2f);
                        Vector2 down = new Vector2(topLeftTile.X + Map.cellSize / 2f, topLeftTile.Y + Map.cellSize);

                        Vector2 destination; // The destination wall that the entity will be moved to

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
                        if (tile.Walls[0] && Vector2.DistanceSquared(currentRayPos, left) < Vector2.DistanceSquared(currentRayPos, destination))
                            destination = left;
                        if (tile.Walls[1] && Vector2.DistanceSquared(currentRayPos, up) < Vector2.DistanceSquared(currentRayPos, destination))
                            destination = up;
                        if (tile.Walls[2] && Vector2.DistanceSquared(currentRayPos, right) < Vector2.DistanceSquared(currentRayPos, destination))
                            destination = right;
                        if (tile.Walls[3] && Vector2.DistanceSquared(currentRayPos, down) < Vector2.DistanceSquared(currentRayPos, destination))
                            destination = down;

                        // Now find how far along the wall we are
                        if (destination == left)
                            texX = MathUtil.Interpolate(0, 1, (currentRayPos.Y - up.Y) / (down.Y - up.Y));
                        else if (destination == up)
                            texX = MathUtil.Interpolate(0, 1, (currentRayPos.X - right.X) / (left.X - right.X));
                        else if (destination == right)
                            texX = MathUtil.Interpolate(0, 1, (currentRayPos.Y - up.Y) / (down.Y - up.Y));
                        else // down
                            texX = MathUtil.Interpolate(0, 1, (currentRayPos.X - left.X) / (right.X - left.X));
                        break;  
                    }
                    //spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(currentRayPos), Color.Red);
                }
                float ed = d / 32f;
                if (texX >= 0) {
                    Texture2D img = Art.TileGrass;
                    if (tileId == 2)
                        img = Art.TileDirt;
                    if (tileId == 3)
                        img = Art.TileStone;
                    if (tileId == 4)
                        img = Art.TileSus;
                    spriteBatch.Draw(img, new Rectangle(i, (int)(GameRoot.ScreenSize.Y / 2f - GameRoot.ScreenSize.Y / 2f / ed), 1, (int)(GameRoot.ScreenSize.Y / ed)), new Rectangle((int)(Art.TileDirt.Width * texX), 0, 1, Art.TileDirt.Height), Color.White);
                } else {
                    //spriteBatch.Draw(Art.Pixel, new Rectangle(i, (int)(topLeft.Y + delta.Y / 2f - delta.Y / 2f / ed), 1, (int)(delta.Y / ed)), new Color(0, 255 - (int)MathUtil.Interpolate(0, 255, step / 445f), 0, 64));
                }
                spriteBatch.Draw(Art.Pixel, new Rectangle(i, (int)(GameRoot.ScreenSize.Y / 2f - GameRoot.ScreenSize.Y / 2f / ed + GameRoot.ScreenSize.Y / ed), 1, (int)(GameRoot.ScreenSize.Y + GameRoot.ScreenSize.Y / 2f - GameRoot.ScreenSize.Y / 2f / ed + GameRoot.ScreenSize.Y / ed)), new Color(255,0 ,  255, 64));
            }
        }
    }
}