using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Mono_Ether.Ether
{
    public class Tile
    {
        public Vector2 pos;
        public int TileId;
        public Boolean[] Walls = new Boolean[4];
        public Tile(Vector2 mapPos, int tileId, int collisionValue)
        {
            this.pos = mapPos;
            this.TileId = tileId;
            if (collisionValue >= 8)
            {
                collisionValue -= 8;
                Walls[3] = true;
            }
            if (collisionValue >= 4)
            {
                collisionValue -= 4;
                Walls[2] = true;
            }
            if (collisionValue >= 2)
            {
                collisionValue -= 2;
                Walls[1] = true;
            }
            if (collisionValue >= 1)
            {
                Walls[0] = true;
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            Texture2D texture;
            switch (TileId) // Get texture based on TextureID
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
                    return;
            }
            var position = Map.MapToScreen(new Vector2(pos.X, pos.Y));
            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
        }
    }
     static class Map
    {
        private static Tile[,] _grid;
        private static Vector2 _size;
        public static void LoadFromFile(string filename, Vector2 size)
        {
            _size = size;
            string lines = System.IO.File.ReadAllText(@"Content/TileMapData/" + filename);
            int i = 0, j = 0;
            _grid = new Tile[(int)size.X, (int)size.Y];
            foreach (var row in lines.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim().Split(','))
                {
                    var tileData = col.Split('/');
                    var id = int.Parse(tileData[0]);
                    var collisionValue = int.Parse(tileData[1]);
                    _grid[j, i] = new Tile(new Vector2(j, i), id, collisionValue);
                    j++;
                }
                i++;
            }
        }

        public static int GetTileFromMap(Vector2 mapPos)
        {
            var (x, y) = mapPos;
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
                return -1;
            return _grid[(int)x, (int)y].TileId;
        }

        public static int GetTileFromWorld(Vector2 worldPos)
        {
            var mapPos = WorldtoMap(worldPos);
            return GetTileFromMap(mapPos);
        }

        public static Vector2 GetValidMovement(Vector2 worldPos, Vector2 velocity)
        {
            return worldPos;
        }
        public static Vector2 WorldtoMap(Vector2 worldPos) => Vector2.Floor(worldPos / 64f);
        public static Vector2 MapToWorld(Vector2 mapPos) => mapPos * 64; //TODO: get texture size and replace this with it
        public static Vector2 MapToScreen(Vector2 mapPos) => Camera.world_to_screen_pos(MapToWorld(mapPos));

        /*public static List<Vector2> WorldToMapFourTiles(Vector2 worldPos)
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
        }*/
        public static void Draw(SpriteBatch spriteBatch)
        {
            /* Instead of iterating over every tile in the 2d array, we only iterate over tiles that are visible by the
            camera (taking position and scaling into account), this significantly improves drawing performance,
            especially when zoomed in. */
            var startCol = Math.Max(0, (int)(Camera.screen_to_world_pos(Vector2.Zero).X / 64f));
            var endCol = Math.Min(_size.X, 1 + (int)(Camera.screen_to_world_pos(new Vector2(1280, 720)).Y / 64f));
            var startRow = Math.Max(0, (int)(Camera.screen_to_world_pos(Vector2.Zero).Y / 64f));
            var endRow = Math.Min(_size.Y, 1 + (int)(Camera.screen_to_world_pos(new Vector2(1280, 720)).X / 64f));
            for (int col = startCol; col < endCol; col++)
            {
                for (int row = startRow; row < endRow; row++)
                {
                    var cell = _grid[col, row];
                    cell.draw(spriteBatch);
                }
            }
            // draw nearest tiles (debug)
            var screenCoords = MapToScreen(Vector2.Floor(Camera.mouse_world_coords() / 64f));
            spriteBatch.Draw(Art.Pixel, screenCoords, null, new Color(255, 255, 255, 128), 0f, Vector2.Zero, Camera.Zoom * 64f, 0, 0);
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
