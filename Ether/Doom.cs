using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether.Ether {
    public static class Doom {
        static float fov = MathF.PI;
        static float stepDist = 8;
        static int maxSteps = 128;
        static float maxDist = stepDist * maxSteps;
        public static void Update() {

        }
        public static void Draw(SpriteBatch spriteBatch) {
            
            Vector2 p1Pos = EntityManager.Player1.Position;
            float p1Angle = EntityManager.Player1.Orientation;
            for (int i = 0; i < (int)GameRoot.ScreenSize.X; i++) {
                float deltaAngle = MathUtil.Interpolate(-fov / 2f, fov / 2f, (GameRoot.ScreenSize.X - i) / GameRoot.ScreenSize.X);
                float rayAngle = p1Angle - deltaAngle;                
                float texX = -1f;
                int tileId = -1;
                int step;
                Vector2 currentRayPos = p1Pos;
                Vector2 stepVector = MathUtil.FromPolar(rayAngle, stepDist);
                float dist = 0f;
                for (step = 0; step <= maxSteps; step++) {
                    currentRayPos += stepVector;
                    tileId = Map.GetTileFromWorld(currentRayPos).TileId;
                    if (tileId > 0) {
                        
                        // Collision! Let's find the nearest intersection between the ray and tile

                        var tile = Map.GetTileFromWorld(currentRayPos);
                        Vector2 nearestCollision = Vector2.Zero;
                        float nearestDistSquared = float.MaxValue;

                        float gradient = stepVector.Y / stepVector.X;
                        float m = -gradient;
                        float c = -currentRayPos.Y + gradient * currentRayPos.X;
                        // Left wall
                        float ty = -c - m * tile.Left.X;
                        if (tile.Walls[0] && tile.Bottom.Y > ty && ty > tile.Top.Y) {
                            Vector2 collision = new Vector2(tile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texX = (collision.Y - tile.Top.Y) / (tile.Bottom.Y - tile.Top.Y);
                            }
                            //spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(new Vector2(tile.Left.X, ty)), null, Color.Blue, 0f, Vector2.Zero, 5f, 0, 0);
                        }
                        // Top wall
                        float tx = (c + tile.Top.Y) / gradient;
                        if (tile.Walls[1] && tile.Left.X < tx && tx < tile.Right.X) {
                            Vector2 collision = new Vector2(tx, tile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texX = (collision.X - tile.Right.X) / (tile.Left.X - tile.Right.X);
                            }
                            //spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(new Vector2(tx, tile.Top.Y)), null, Color.Blue, 0f, Vector2.Zero, 5f, 0, 0);
                        }
                        // Right wall
                        ty = -c - m * tile.Right.X;
                        if (tile.Walls[2] && tile.Bottom.Y > ty && ty > tile.Top.Y) {
                            Vector2 collision = new Vector2(tile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texX = (collision.Y - tile.Top.Y) / (tile.Bottom.Y - tile.Top.Y);
                            }
                            //spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(new Vector2(tile.Right.X, ty)), null, Color.Blue, 0f, Vector2.Zero, 5f, 0, 0);
                        }
                        // Bottom wall
                        tx = (c + tile.Bottom.Y) / gradient;
                        if (tile.Walls[3] && tile.Left.X < tx && tx < tile.Right.X) {
                            Vector2 collision = new Vector2(tx, tile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texX = (collision.X - tile.Right.X) / (tile.Left.X - tile.Right.X);
                            }
                            //spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(new Vector2(tx, tile.Bottom.Y)), null, Color.Blue, 0f, Vector2.Zero, 5f, 0, 0);
                        }
                        dist = MathF.Sqrt(nearestDistSquared);
                        spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(nearestCollision), null, Color.Blue, 0f, Vector2.Zero, 5f, 0, 0);
                        break;
                    }
                    spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(currentRayPos), Color.Red);
                }
                float ed = dist / 32f;
                if (texX >= 0) {
                    Texture2D img = Art.TileGrass;
                    if (tileId == 2)
                        img = Art.TileDirt;
                    if (tileId == 3)
                        img = Art.TileStone;
                    if (tileId == 4)
                        img = Art.TileSus;
                    //spriteBatch.Draw(img, new Rectangle(i, (int)(GameRoot.ScreenSize.Y / 2f - GameRoot.ScreenSize.Y / 2f / ed), 1, (int)(GameRoot.ScreenSize.Y / ed)), new Rectangle((int)(Art.TileDirt.Width * texX), 0, 1, Art.TileDirt.Height), Color.White);
                    spriteBatch.Draw(img, new Vector2(i, GameRoot.ScreenSize.Y / 2f), new Rectangle((int)(Art.TileDirt.Width * texX), 0, 1, Art.TileDirt.Height), Color.White * (1 - dist / maxDist), 0f, new Vector2(32f, 32f), new Vector2(1, 16f / ed), 0, 0);
                } else {
                    //spriteBatch.Draw(Art.Pixel, new Rectangle(i, (int)(topLeft.Y + delta.Y / 2f - delta.Y / 2f / ed), 1, (int)(delta.Y / ed)), new Color(0, 255 - (int)MathUtil.Interpolate(0, 255, step / 445f), 0, 64));
                }
                //spriteBatch.Draw(Art.Pixel, new Rectangle(i, (int)(GameRoot.ScreenSize.Y / 2f - GameRoot.ScreenSize.Y / 2f / ed + GameRoot.ScreenSize.Y / ed), 1, (int)(GameRoot.ScreenSize.Y + GameRoot.ScreenSize.Y / 2f - GameRoot.ScreenSize.Y / 2f / ed + GameRoot.ScreenSize.Y / ed)), new Color(255,0 ,  255, 64));
            }
        }
    }
}