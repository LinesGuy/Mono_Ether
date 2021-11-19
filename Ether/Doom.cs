using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mono_Ether.Ether {
    public static class Doom {
        static float fov = MathF.PI * 0.8f;
        static float stepDist = 4;
        static int maxSteps = 512;
        static float maxDist = stepDist * maxSteps;
        public static void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Art.Pixel, new Rectangle(0, (int)(GameRoot.ScreenSize.Y / 2f), (int)(GameRoot.ScreenSize.X), (int)(GameRoot.ScreenSize.Y / 2f)), new Color(32, 32, 32));
            Vector2 p1Pos = EntityManager.Player1.Position;
            float p1Angle = EntityManager.Player1.Orientation;
            List<float> dists = new List<float>();
            List<float> texXs = new List<float>();
            List<int> tileIds = new List<int>();
            for (int i = 0; i < (int)GameRoot.ScreenSize.X; i++) {
                dists.Add(0);
                texXs.Add(-1f);
                tileIds.Add(0);
            }
            Parallel.For(0, (int)GameRoot.ScreenSize.X, i => {
                float deltaAngle = MathUtil.Interpolate(-fov / 2f, fov / 2f, (GameRoot.ScreenSize.X - i) / GameRoot.ScreenSize.X);
                float rayAngle = p1Angle - deltaAngle;
                int tileId = -1;
                int step;
                Vector2 currentRayPos = p1Pos;
                Vector2 stepVector = MathUtil.FromPolar(rayAngle, stepDist);
                float dist = 0f;
                for (step = 0; step <= maxSteps; step++) {

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
                        Tile upperTile = Map.GetTileFromMap(tile.pos - new Vector2(0, 1));
                        Tile lowerTile = Map.GetTileFromMap(tile.pos + new Vector2(0, 1));
                        if (upperTile.Walls[0] && ty < tile.Top.Y) {

                            Vector2 collision = new Vector2(upperTile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - upperTile.Top.Y) / (upperTile.Bottom.Y - upperTile.Top.Y);
                                tileId = upperTile.TileId;
                            }
                        } else if (tile.Walls[0] && tile.Bottom.Y > ty && ty > tile.Top.Y) {
                            Vector2 collision = new Vector2(tile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - tile.Top.Y) / (tile.Bottom.Y - tile.Top.Y);
                                tileId = tile.TileId;
                            }
                        } else if (lowerTile.Walls[0] && tile.Bottom.Y < ty) {

                            Vector2 collision = new Vector2(lowerTile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - lowerTile.Top.Y) / (lowerTile.Bottom.Y - lowerTile.Top.Y);
                                tileId = lowerTile.TileId;
                            }
                        }
                        // Top wall
                        float tx = (c + tile.Top.Y) / gradient;
                        Tile leftTile = Map.GetTileFromMap(tile.pos - new Vector2(1, 0));
                        Tile rightTile = Map.GetTileFromMap(tile.pos + new Vector2(1, 0));
                        if (leftTile.Walls[1] && tile.Left.X > tx) {
                            Vector2 collision = new Vector2(tx, leftTile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - leftTile.Right.X) / (leftTile.Left.X - leftTile.Right.X);
                                tileId = leftTile.TileId;
                            }
                        } else if (tile.Walls[1] && tile.Left.X < tx && tx < tile.Right.X) {
                            Vector2 collision = new Vector2(tx, tile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - tile.Right.X) / (tile.Left.X - tile.Right.X);
                                tileId = tile.TileId;
                            }
                        } else if (rightTile.Walls[1] && tx > tile.Right.X) {
                            Vector2 collision = new Vector2(tx, rightTile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - rightTile.Right.X) / (rightTile.Left.X - rightTile.Right.X);
                                tileId = rightTile.TileId;
                            }
                        }
                        // Right wall
                        ty = -c - m * tile.Right.X;
                        upperTile = Map.GetTileFromMap(tile.pos - new Vector2(0, 1));
                        lowerTile = Map.GetTileFromMap(tile.pos + new Vector2(0, 1));
                        if (upperTile.Walls[2] && ty < tile.Top.Y) {
                            Vector2 collision = new Vector2(upperTile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - upperTile.Top.Y) / (upperTile.Bottom.Y - upperTile.Top.Y);
                                tileId = upperTile.TileId;
                            }
                        } else if (tile.Walls[2] && tile.Bottom.Y > ty && ty > tile.Top.Y) {
                            Vector2 collision = new Vector2(tile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - tile.Top.Y) / (tile.Bottom.Y - tile.Top.Y);
                                tileId = tile.TileId;
                            }
                        } else if (lowerTile.Walls[2] && tile.Bottom.Y < ty) {
                            Vector2 collision = new Vector2(lowerTile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - lowerTile.Top.Y) / (lowerTile.Bottom.Y - lowerTile.Top.Y);
                                tileId = lowerTile.TileId;
                            }
                        }
                        // Bottom wall
                        tx = (c + tile.Bottom.Y) / gradient;
                        leftTile = Map.GetTileFromMap(tile.pos - new Vector2(1, 0));
                        rightTile = Map.GetTileFromMap(tile.pos + new Vector2(1, 0));
                        if (leftTile.Walls[3] && tile.Left.X > tx) {
                            Vector2 collision = new Vector2(tx, leftTile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - leftTile.Right.X) / (leftTile.Left.X - leftTile.Right.X);
                                tileId = leftTile.TileId;
                            }
                        } else if (tile.Walls[3] && tile.Left.X < tx && tx < tile.Right.X) {
                            Vector2 collision = new Vector2(tx, tile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - tile.Right.X) / (tile.Left.X - tile.Right.X);
                                tileId = tile.TileId;
                            }
                        } else if (rightTile.Walls[3] && tx > tile.Right.X) {
                            Vector2 collision = new Vector2(tx, rightTile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - rightTile.Right.X) / (rightTile.Left.X - rightTile.Right.X);
                                tileId = rightTile.TileId;
                            }
                        }
                        dist = MathF.Sqrt(nearestDistSquared);
                        dists[(int)i] = dist;
                        tileIds[(int)i] = tileId;
                        break;
                    }
                    currentRayPos += stepVector;
                }
            });
            for (int i = 0; i < (int)GameRoot.ScreenSize.X; i++) {
                float dist = dists[i];
                float texX = texXs[i];
                int tileId = tileIds[i];
                float ed = dist / 32f;
                if (texX >= 0) {
                    Texture2D img = Art.TileGrass;
                    if (tileId == 2)
                        img = Art.TileDirt;
                    if (tileId == 3)
                        img = Art.TileStone;
                    if (tileId == 4)
                        img = Art.TileSus;
                    int c = (int)(255f * (1 - dist / maxDist));
                    spriteBatch.Draw(img, new Vector2(i, GameRoot.ScreenSize.Y / 2f), new Rectangle((int)(Art.TileDirt.Width * texX), 0, 1, Art.TileDirt.Height), new Color(c, c, c), 0f, new Vector2(32f, 32f), new Vector2(1, 16f / ed), 0, 0);
                }
            }
        }
    }
}