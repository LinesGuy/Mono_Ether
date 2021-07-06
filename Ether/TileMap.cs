using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Mono_Ether.Ether
{
    class Tiles
    {
        protected Texture2D Texture;
        protected Vector2 Position;
        protected int Size { get; set; }

        public static ContentManager Content { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var screenPos = Camera.world_to_screen_pos(Position);
            var scale = Camera.Zoom * Size / Texture.Width;
            //spriteBatch.Draw(texture, screenPos, null, Color.White, 0f, size / 2f, scale, 0f, 0f);
            spriteBatch.Draw(Texture, screenPos, null, Color.White, 0f, new Vector2(Size) / 2f, scale, 0, 0);
            //spriteBatch.Draw(Texture, screenPos, null, Color.White, 0f, Vector2.Zero, scale, 0, 0);
            spriteBatch.Draw(Art.Pixel, screenPos, Color.White);
        }
    }
    
    class CollisionTiles : Tiles
    {
        public CollisionTiles(int i, Vector2 position, int size)
        {
            Texture = Content.Load<Texture2D>("Textures/tiles/tile" + i);
            this.Position = position;
            this.Size = size;
        }
    }

    class Map
    {
        private readonly List<CollisionTiles> collisionTiles = new List<CollisionTiles>();

        public List<CollisionTiles> CollisionTiles => collisionTiles;

        private int width, height;

        public int Width => width;

        public int Height => height;
        private static int Size;
        public void Generate(int[,] map, int size)
        {
            Size = size;
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

        public Vector2 WorldToMap(Vector2 worldPos)
        {
            var pos = worldPos / Size;
            pos = new Vector2((int)Math.Round(pos.X), (int)Math.Round(pos.Y));
            return pos;
        }

        public Vector2 MapToWorld(Vector2 mapPos)
        {
            var pos = mapPos * Size;
            return pos;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTiles tile in collisionTiles)
                tile.Draw(spriteBatch);
            
            // DRAW NEAREST TILE FROM CURSOR
            spriteBatch.Draw(Art.Wanderer, Camera.world_to_screen_pos(MapToWorld(WorldToMap(Camera.mouse_world_coords()))), null, Color.Green, 0f, Art.Wanderer.Size() / 2f, Camera.Zoom, SpriteEffects.None, 0);
        }
    }
}