using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Mono_Ether.Ether
{
    public class Map
    {
        private int[,] Grid;
        private Vector2 Size;
        public void LoadFromFile(string filename, Vector2 size)
        {
            Size = size;
            string lines = System.IO.File.ReadAllText(@"Content/TileMapData/" + filename);
            int i = 0, j = 0;
            Grid = new int[(int)size.X, (int)size.Y];
            foreach (var row in lines.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim().Split(','))
                {
                    Grid[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
        }

        public int GetTile(Vector2 mapPos)
        {
            if (mapPos.X < 0 || mapPos.X >= Size.X || mapPos.Y < 0 || mapPos.Y >= Size.Y)
                return -1;
            return Grid[(int)mapPos.Y, (int)mapPos.X];
        }
        public Vector2 WorldtoMap(Vector2 worldPos)
        {
            return Vector2.Floor(worldPos / 64f);
        }
        public Vector2 MapToWorld(Vector2 mapPos)
        {
            return mapPos * 64; //TODO: get texture size and replace this with it
        }
        public Vector2 MapToScreen(Vector2 mapPos)
        {
            return Camera.world_to_screen_pos(MapToWorld(mapPos));
        }

        public List<Vector2> WorldToMapFourTiles(Vector2 worldPos)
        {
            List<Vector2> result = new List<Vector2>();
            var topLeft = Vector2.Floor(worldPos / 64f); 
            List<Vector2> offsets = new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)
            };
            foreach (var offset in offsets)
            {
                result.Add(topLeft + offset);
            }

            return result;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            /* Instead of iterating over every tile in the 2d array, we only iterate over tiles that are visible by the
            camera (taking position and scaling into account), this significantly improves drawing performance,
            especially when zoomed in. */
            var startCol = Math.Max(0, (int)(Camera.screen_to_world_pos(Vector2.Zero).Y / 64f));
            var endCol = Math.Min(Size.X, 1 + (int)(Camera.screen_to_world_pos(new Vector2(1280, 720)).Y / 64f));
            var startRow = Math.Max(0, (int)(Camera.screen_to_world_pos(Vector2.Zero).X / 64f));
            var endRow = Math.Min(Size.X, 1 + (int)(Camera.screen_to_world_pos(new Vector2(1280, 720)).X / 64f));
            for (int col = startCol; col < endCol; col++)
            {
                for (int row = startRow; row < endRow; row++)
                {
                    var cell = Grid[col, row];
                    Texture2D texture;
                    switch (cell) // Get texture based on TextureID
                    {
                        case 1:
                            texture = Art.tileGrass;
                            break;
                        case 2:
                            texture = Art.tileDirt;
                            break;
                        case 3:
                            texture = Art.tileStone;
                            break;
                        case 4:
                            texture = Art.tileSus;
                            break;
                        default:
                            continue;
                    }
                    var position = MapToScreen(new Vector2(row, col));
                    spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                }
            }
            // draw nearest tiles (debug)
            foreach (var tileCoords in WorldToMapFourTiles(Camera.mouse_world_coords()))
            {
                var screenCoords = MapToScreen(tileCoords);
                spriteBatch.Draw(Art.Pixel, screenCoords, null, Color.Red, 0f, Vector2.Zero, Camera.Zoom * 5f, 0, 0);
            }
        }
    }
/*
    public class Tiles
    {
        protected Texture2D Texture;
        protected Vector2 Position;
        protected int Size { get; set; }

        public static ContentManager Content { get; set; }

        public Vector2 Middle => Position + new Vector2(Size) / 2f;
        public void Draw(SpriteBatch spriteBatch)
        {
            var screenPos = Camera.world_to_screen_pos(Position);
            var scale = Camera.Zoom * Size / Texture.Width;
            spriteBatch.Draw(Texture, screenPos, null, Color.White, 0f, Vector2.Zero, scale, 0, 0);
            spriteBatch.Draw(Art.Pixel, screenPos, Color.White);
        }
    }
    
    public class CollisionTiles : Tiles
    {
        public CollisionTiles(int i, Vector2 position, int size)
        {
            Texture = Content.Load<Texture2D>("Textures/tiles/tile" + i);
            this.Position = position;
            this.Size = size;
        }
    }

    public class Map
    {
        private readonly List<CollisionTiles> collisionTiles = new List<CollisionTiles>();
        public List<CollisionTiles> CollisionTiles => collisionTiles;
        private int width, height;
        public int Width => width;
        public int Height => height;
        private static int _size;
        public void Generate(int[,] map, int size)
        {
            _size = size;
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];
                    
                    if (number > 0)
                        collisionTiles.Add(new CollisionTiles(number, new Vector2(x * size, y * size), size));
                    width = (x + 1) * size;
                    height = (y + 1) * size;
                }
            }
        }

        public Vector2 WorldToTile(Vector2 worldPos) => Vector2.Floor(worldPos / _size);
        public Vector2 WorldToNearestTile(Vector2 worldPos) => TileToWorld(WorldToTile(worldPos)) + new Vector2(_size) / 2f;
        public Vector2 TileToWorld(Vector2 mapPos) =>  mapPos * _size;
            public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTiles tile in collisionTiles)
                tile.Draw(spriteBatch);
            
            // DRAW NEAREST TILE FROM CURSOR
            spriteBatch.Draw(Art.Wanderer, Camera.world_to_screen_pos(TileToWorld(WorldToTile(Camera.mouse_world_coords()))), null, Color.Green, 0f, Art.Wanderer.Size() / 2f, Camera.Zoom, SpriteEffects.None, 0);
        }
    }
    */
}
