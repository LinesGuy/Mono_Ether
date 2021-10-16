using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Mono_Ether.Ether {
    public class Tile {
        public Vector2 pos;
        public int TileId;
        public Boolean[] Walls = new Boolean[8];
        public Tile(Vector2 mapPos, int tileId) {
            pos = mapPos;
            TileId = tileId;
        }

        public void Draw(SpriteBatch spriteBatch) {
            Texture2D texture;
            switch (TileId) // Get texture based on TextureID
            {
                case 1:
                    texture = Art.TileGrass;
                    break;
                case 2:
                    texture = Art.TileDirt;
                    break;
                case 3:
                    texture = Art.TileStone;
                    break;
                case 4:
                    texture = Art.TileSus;
                    break;
                default:
                    return;
            }
            var position = Map.MapToScreen(new Vector2(pos.X, pos.Y));
            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
            if (EtherRoot.Instance.editorMode) {
                if (Walls[0]) spriteBatch.Draw(Art.CollisionLeft, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[1]) spriteBatch.Draw(Art.CollisionUp, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[2]) spriteBatch.Draw(Art.CollisionRight, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[3]) spriteBatch.Draw(Art.CollisionDown, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[4]) spriteBatch.Draw(Art.CollisionTopLeft, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[5]) spriteBatch.Draw(Art.CollisionTopRight, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[6]) spriteBatch.Draw(Art.CollisionBottomRight, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[7]) spriteBatch.Draw(Art.CollisionBottomLeft, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
            }
        }

        public Vector2 TopLeft { get => pos * Map.cellSize; }
        public Vector2 Top { get => new Vector2(TopLeft.X + Map.cellSize / 2f, TopLeft.Y); }
        public Vector2 TopRight { get => new Vector2(TopLeft.X + Map.cellSize, TopLeft.Y); }
        public Vector2 Right { get => new Vector2(TopLeft.X + Map.cellSize, TopLeft.Y + Map.cellSize / 2f); }
        public Vector2 BottomRight { get => new Vector2(TopLeft.X + Map.cellSize, TopLeft.Y + Map.cellSize); }
        public Vector2 Bottom { get => new Vector2(TopLeft.X + Map.cellSize / 2f, TopLeft.Y + Map.cellSize); }
        public Vector2 BottomLeft { get => new Vector2(TopLeft.X, TopLeft.Y + Map.cellSize); }
        public Vector2 Left { get => new Vector2(TopLeft.X, TopLeft.Y + Map.cellSize / 2f); }

        public void UpdateWalls() {
            Walls = new Boolean[8]; // Set all walls to False (temp)
            // Update wall values based on surrounding tiles
            if (Map.GetTileFromMap(new Vector2(pos.X - 1, pos.Y)).TileId <= 0)
                Walls[0] = true; // Left
            if (Map.GetTileFromMap(new Vector2(pos.X, pos.Y - 1)).TileId <= 0)
                Walls[1] = true; // Top
            if (Map.GetTileFromMap(new Vector2(pos.X + 1, pos.Y)).TileId <= 0)
                Walls[2] = true; // Right
            if (Map.GetTileFromMap(new Vector2(pos.X, pos.Y + 1)).TileId <= 0)
                Walls[3] = true; // Down
            if (!Walls[0] && !Walls[1])
                Walls[4] = true; // Top left
            if (!Walls[1] && !Walls[2])
                Walls[5] = true; // Top right
            if (!Walls[2] && !Walls[3])
                Walls[6] = true; // Bottom right
            if (!Walls[3] && !Walls[0])
                Walls[7] = true; // Bottom left
        }
        public void UpdateNeighbouringWalls() {
            // Updates this tile's walls and all 8 surrounding tile's walls
            List<Vector2> offsets = new List<Vector2>
            {
                new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
                new Vector2(-1,  0), new Vector2(0,  0), new Vector2(1,  0),
                new Vector2(-1,  1), new Vector2(0,  1), new Vector2(1,  1)
            };
            foreach (var offset in offsets) {
                Vector2 offsetPos = pos + offset;
                if (offsetPos.X < 0 || offsetPos.Y < 0 || offsetPos.X >= Map._size.X || offsetPos.Y >= Map._size.Y)
                    continue;
                Map._grid[(int)offsetPos.X, (int)offsetPos.Y].UpdateWalls();
            }
        }
    }
    static class Map {
        public const float cellSize = 64f;
        public static Tile[,] _grid;
        public static Vector2 _size;
        private static int SelectedId = 1; // Currently selected Tile ID for editor mode
        public static Vector2 WorldSize { get { return _size * cellSize; } }
        public static void LoadFromFile(string filename, Vector2 size) {
            _size = size;
            // Load TileId's from TileMapData to _grid
            string lines = File.ReadAllText(@"Content/TileMapData/" + filename);
            int i = 0;
            _grid = new Tile[(int)size.X, (int)size.Y];
            foreach (var row in lines.Split('\n')) {
                if (row.Length == 0) continue;
                int j = 0;
                foreach (var col in row.Trim().Split(',')) {
                    var id = int.Parse(col);
                    _grid[j, i] = new Tile(new Vector2(j, i), id);
                    j++;
                }
                i++;
            }
            // Create tile collision data
            foreach (var tile in _grid) {
                tile.UpdateWalls();
            }
            // Adjust player position to middle of map
            foreach(PlayerShip player in EntityManager.Players)
                player.Position = WorldSize / 2f;
            Camera.CameraPosition = EntityManager.player1.Position;
        }

        public static Tile GetTileFromMap(Vector2 mapPos) {
            var (x, y) = mapPos;
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
                return new Tile(Vector2.Zero, -1);
            return _grid[(int)x, (int)y];
        }

        public static Tile GetTileFromWorld(Vector2 worldPos) {
            var mapPos = WorldtoMap(worldPos);
            return GetTileFromMap(mapPos);
        }
        public static Vector2 WorldtoMap(Vector2 worldPos) => Vector2.Floor(worldPos / cellSize);
        public static Vector2 MapToWorld(Vector2 mapPos) => mapPos * cellSize;
        public static Vector2 MapToScreen(Vector2 mapPos) => Camera.WorldToScreen(MapToWorld(mapPos));
        public static void Draw(SpriteBatch spriteBatch) {
            /* Instead of iterating over every tile in the 2d array, we only iterate over tiles that are visible by the
            camera (taking position and scaling into account), this significantly improves drawing performance,
            especially when zoomed in. */
            var startCol = Math.Max(0, (int)(Camera.ScreenToWorld(Vector2.Zero).X / cellSize));
            var endCol = Math.Min(_size.X, 1 + (int)(Camera.ScreenToWorld(GameRoot.ScreenSize).X / cellSize));
            var startRow = Math.Max(0, (int)(Camera.ScreenToWorld(Vector2.Zero).Y / cellSize));
            var endRow = Math.Min(_size.Y, 1 + (int)(Camera.ScreenToWorld(GameRoot.ScreenSize).Y / cellSize));
            for (int row = startRow; row < endRow; row++) {
                for (int col = startCol; col < endCol; col++) {
                    var cell = _grid[col, row];
                    cell.Draw(spriteBatch);
                }
            }
            // Draw tile cursor is in if in editor mode
            if (EtherRoot.Instance.editorMode) {
                var screenCoords = MapToScreen(Vector2.Floor(Camera.MouseWorldCoords() / cellSize));
                spriteBatch.Draw(Art.Pixel, screenCoords, null, new Color(255, 255, 255, 32), 0f, Vector2.Zero, Camera.Zoom * cellSize, 0, 0);
            }
        }

        public static void Update() {
            if (EtherRoot.Instance.editorMode) {
                // Press 'R' to save map
                if (Input.Keyboard.WasKeyJustDown(Keys.R)) {
                    string filename = "susMap3.txt";
                    Debug.WriteLine("Wrote to Content/TileMapData/" + filename);
                    List<string> lines = new List<string>();
                    for (int row = 0; row < _size.Y; row++) {
                        string line = "";
                        for (int col = 0; col < _size.X; col++) {
                            var cell = _grid[col, row];
                            line += $"{cell.TileId},";
                        }
                        line = line.Remove(line.Length - 1);
                        lines.Add(line);
                    }
                    File.WriteAllLines(@"Content/TileMapData/" + filename, lines.ToArray());
                }
                var tile = GetTileFromWorld(Camera.MouseWorldCoords());
                // Set Tile ID
                if (Input.Keyboard.IsKeyDown(Keys.D1))
                    SelectedId = 1;
                else if (Input.Keyboard.IsKeyDown(Keys.D2))
                    SelectedId = 2;
                else if (Input.Keyboard.IsKeyDown(Keys.D3))
                    SelectedId = 3;
                else if (Input.Keyboard.IsKeyDown(Keys.D4))
                    SelectedId = 4;
                else if (Input.Keyboard.IsKeyDown(Keys.D0))
                    SelectedId = 0;
                // Toggle cell walls
                if (Input.Keyboard.WasKeyJustDown(Keys.J)) // Left
                    tile.Walls[0] = !tile.Walls[0];
                else if (Input.Keyboard.WasKeyJustDown(Keys.I)) // Up
                    tile.Walls[1] = !tile.Walls[1];
                else if (Input.Keyboard.WasKeyJustDown(Keys.L)) // Right
                    tile.Walls[2] = !tile.Walls[2];
                else if (Input.Keyboard.WasKeyJustDown(Keys.K)) // Down
                    tile.Walls[3] = !tile.Walls[3];

                if (Input.Mouse.IsButtonDown(MouseButton.Left)) // Place last placed tile ID at cursor
                {
                    tile.TileId = SelectedId;
                    tile.UpdateNeighbouringWalls();
                }

                if (Input.Mouse.IsButtonDown(MouseButton.Right)) // Delete tile at cursor
                {
                    tile.TileId = 0;
                    tile.UpdateNeighbouringWalls();
                }
            }
        }
    }
}