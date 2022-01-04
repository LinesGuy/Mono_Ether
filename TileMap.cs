using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mono_Ether {
    public class TileMap {
        private readonly Tile[][] _grid;
        public Vector2 GridSize => new Vector2(_grid[0].Length, _grid.Length);
        public Vector2 WorldSize => GridSize * Tile.Length;
        public TileMap(string fileName) {
            var lines = File.ReadAllLines(@"Content/TileMapData/" + fileName).Where(l => l != "").ToArray();
            var columns = int.Parse(lines[0].Split("x")[0]);
            var rows = int.Parse(lines[0].Split("x")[1]);
            var gridList = new List<List<Tile>>();
            var y = 0;
            foreach (var line in lines.Skip(1)) { // First line is tile map dimensions
                var rowList = new List<Tile>();
                var x = 0;
                foreach (var id in line.Trim().Split(',')) {
                    rowList.Add(new Tile(new Vector2(x, y), int.Parse(id)));
                    x++;
                }
                gridList.Add(rowList);
                y++;
            }
            _grid = gridList.Select(r => r.ToArray()).ToArray(); // 2D list to 2D array
        }
        public void Draw(SpriteBatch batch, Camera camera, bool editorMode) {
            /* Don't draw every tile in the grid. Instead, create an array of the coordinates of all four corners of the SCREEN, start from
             * the top-left most coordinate and iterate until the bottom-right most coordinate. If the camera orientation is a multiple of
             * Pi / 2 (or 90 degrees) then this will have 100% efficiency and draw ONLY tiles visible to the camera. If the orientation is
             * anything else, it will draw AT MOST twice as many tiles than is actually visible, but for most other angles it will draw far
             * less than this. Even in the worst case scenario this is significantly more efficient than drawing all tiles.
             */
            var corners = new[] {
                camera.ScreenToWorld(Vector2.Zero), // Top-left
                camera.ScreenToWorld(camera.ScreenSize), // Bottom-right
                camera.ScreenToWorld(new Vector2(camera.ScreenSize.X, 0)), // Top-right
                camera.ScreenToWorld(new Vector2(0, camera.ScreenSize.Y)) // Bottom-left
            };
            var xCoords = corners.Select(v => v.X).ToList();
            var yCoords = corners.Select(v => v.Y).ToList();
            var startCol = Math.Max(0, (int)(xCoords.Min() / Tile.Length));
            var endCol = Math.Min(GridSize.X, 1 + (int)(xCoords.Max() / Tile.Length));
            var startRow = Math.Max(0, (int)(yCoords.Min() / Tile.Length));
            var endRow = Math.Min(GridSize.Y, 1 + (int)(yCoords.Max() / Tile.Length));
            for (var row = startRow; row < endRow; row++) {
                for (var col = startCol; col < endCol; col++) { 
                    _grid[row][col].Draw(batch, camera);
                }
            }
            /* TODO probably working but verify
            // Draw tile cursor is in if in editor mode
            if (editorMode) {
                var screenCoords = MapToScreen(Vector2.Floor(camera.MouseWorldCoords() / Tile.Length), camera);
                batch.Draw(GlobalAssets.Pixel, screenCoords, null, new Color(255, 255, 255, 32), camera.Orientation, Vector2.Zero, camera.Zoom * Tile.Size, 0, 0);
            }
            */
        }
        public Tile GetTileFromMap(Vector2 mapPos) {
            var (x, y) = mapPos;
            if (x < 0 || x >= WorldSize.X || y < 0 || y >= WorldSize.Y)
                return new Tile(Vector2.Zero, -1);
            return _grid[(int)y][(int)x];
        }

        public Tile GetTileFromWorld(Vector2 worldPos) {
            var mapPos = WorldtoMap(worldPos);
            return GetTileFromMap(mapPos);
        }
        public Vector2 WorldtoMap(Vector2 worldPos) => Vector2.Floor(worldPos / Tile.Size);
        public static Vector2 MapToWorld(Vector2 mapPos) => mapPos * Tile.Length;
        public static Vector2 MapToScreen(Vector2 mapPos, Camera camera) => camera.WorldToScreen(MapToWorld(mapPos));
    }
    public class Tile {
        private static Texture2D[] _textures;
        public static Vector2 Size => _textures[0].Size();
        public static float Length => Size.X;
        public int Id;
        public readonly Vector2 Pos;
        public Vector2 WorldPos => Pos * Length;
        public bool[] SolidWalls = new bool[4]; // Left, Top, Right, Bottom
        public bool[] SolidCorners = new bool[4]; // TopLeft, TopRight, BottomRight, BottomLeft
        public Tile(Vector2 mapPos, int id) {
            Pos = mapPos;
            Id = id;

        }
        public static void LoadContent(ContentManager content) {
            List<Texture2D> textureList = new List<Texture2D>
            {
                content.Load<Texture2D>("Textures/GameScreen/Tiles/PinkNeon"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/BlueNeon"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/GreenNeon"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/RedNeon"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/PurpleNeon")
            };
            //textureList.Add(content.Load<Texture2D>("Textures/GameScreen/Tiles/ASDFASDF"));
            _textures = textureList.ToArray();
        }
        public static void UnloadContent() {
            _textures = null;
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            if (Id == 0) return;
            batch.Draw(_textures[Id - 1], TileMap.MapToScreen(Pos, camera), null, Color.White, camera.Orientation, Vector2.Zero, camera.Zoom, 0, 0);
        }
    }
}
