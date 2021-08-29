using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System.IO;

namespace Mono_Ether.Ether
{
    public class Tile
    {
        public Vector2 pos;
        public int TileId;
        public Boolean[] Walls = new Boolean[8];
        public Tile(Vector2 mapPos, int tileId)
        {
            this.pos = mapPos;
            this.TileId = tileId;
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
            if (EtherRoot.Instance.editorMode)
            {
                if (Walls[0]) spriteBatch.Draw(Art.collisionLeft, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[1]) spriteBatch.Draw(Art.collisionUp, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[2]) spriteBatch.Draw(Art.collisionRight, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
                if (Walls[3]) spriteBatch.Draw(Art.collisionDown, position, null, Color.White, 0f, Vector2.Zero, Camera.Zoom, 0, 0);
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

        public void updateWalls()
        {
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
    }
    static class Map
    {
        public const float cellSize = 64f;
        private static Tile[,] _grid;
        public static Vector2 _size;
        private static int SelectedId = 1; // Currently selected Tile ID for editor mode
        public static void LoadFromFile(string filename, Vector2 size)
        {
            _size = size;
            string lines = File.ReadAllText(@"Content/TileMapData/" + filename);
            int i = 0, j = 0;
            _grid = new Tile[(int)size.X, (int)size.Y];
            foreach (var row in lines.Split('\n'))
            {
                if (row.Length == 0) continue;
                j = 0;
                foreach (var col in row.Trim().Split(','))
                {
                    //var tileData = col.Split('/');
                    //var id = int.Parse(tileData[0]);
                    //var collisionValue = int.Parse(tileData[1]);
                    var id = int.Parse(col);
                    _grid[j, i] = new Tile(new Vector2(j, i), id);
                    j++;
                }
                i++;
            }
            // Create collision data
            foreach (var tile in _grid)
            {
                tile.updateWalls();
            }
        }

        public static Tile GetTileFromMap(Vector2 mapPos)
        {
            var (x, y) = mapPos;
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
                return new Tile(Vector2.Zero, -1);
            return _grid[(int)x, (int)y];
        }

        public static Tile GetTileFromWorld(Vector2 worldPos)
        {
            var mapPos = WorldtoMap(worldPos);
            return GetTileFromMap(mapPos);
        }

        public static void SetTile(Vector2 mapPos, int id = -1)
        {
            var (x, y) = mapPos;
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
                return;
            _grid[(int)x, (int)y].TileId = id;
        }
        public static void ToggleCellWall(Vector2 mapPos, int wallId)
        {
            var (x, y) = mapPos;
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
                return;
            _grid[(int)x, (int)y].Walls[wallId] = !_grid[(int)x, (int)y].Walls[wallId];
        }
        public static Vector2 WorldtoMap(Vector2 worldPos) => Vector2.Floor(worldPos / cellSize);
        public static Vector2 MapToWorld(Vector2 mapPos) => mapPos * cellSize;
        public static Vector2 MapToScreen(Vector2 mapPos) => Camera.world_to_screen_pos(MapToWorld(mapPos));
        public static void Draw(SpriteBatch spriteBatch)
        {
            /* Instead of iterating over every tile in the 2d array, we only iterate over tiles that are visible by the
            camera (taking position and scaling into account), this significantly improves drawing performance,
            especially when zoomed in. */
            var startCol = Math.Max(0, (int)(Camera.screen_to_world_pos(Vector2.Zero).X / cellSize));
            var endCol = Math.Min(_size.X, 1 + (int)(Camera.screen_to_world_pos(new Vector2(1280, 720)).X / cellSize));
            var startRow = Math.Max(0, (int)(Camera.screen_to_world_pos(Vector2.Zero).Y / cellSize));
            var endRow = Math.Min(_size.Y, 1 + (int)(Camera.screen_to_world_pos(new Vector2(1280, 720)).Y / cellSize));
            for (int row = startRow; row < endRow; row++)
            {
                for (int col = startCol; col < endCol; col++)
                {
                    var cell = _grid[col, row];
                    cell.draw(spriteBatch);
                }
            }
            // Draw tile cursor is in if in editor mode
            if (EtherRoot.Instance.editorMode)
            {
                var screenCoords = MapToScreen(Vector2.Floor(Camera.mouse_world_coords() / cellSize));
                spriteBatch.Draw(Art.Pixel, screenCoords, null, new Color(255, 255, 255, 32), 0f, Vector2.Zero, Camera.Zoom * cellSize, 0, 0);
            }
            
        }

        public static void Update()
        {
            if (EtherRoot.Instance.editorMode)
            {
                // Press 'R' to save map
                if (Input.Keyboard.WasKeyJustDown(Keys.R))
                {
                    string filename = "susMap3.txt";
                    Debug.WriteLine("Wrote to Content/TileMapData/" + filename);
                    List<string> lines = new List<string>();
                    for (int row = 0; row < _size.Y; row++)
                    {
                        string line = "";
                        for (int col = 0; col < _size.X; col++)
                        {
                            var cell = _grid[col, row];
                            line += $"{cell.TileId},";
                        }
                        line = line.Remove(line.Length - 1);
                        lines.Add(line);
                    }
                    File.WriteAllLines(@"Content/TileMapData/" + filename, lines.ToArray());
                }
                var tileCoords = WorldtoMap(Camera.mouse_world_coords());
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
                    ToggleCellWall(tileCoords, 0);
                else if (Input.Keyboard.WasKeyJustDown(Keys.I)) // Up
                    ToggleCellWall(tileCoords, 1);
                else if (Input.Keyboard.WasKeyJustDown(Keys.L)) // Right
                    ToggleCellWall(tileCoords, 2);
                else if (Input.Keyboard.WasKeyJustDown(Keys.K)) // Down
                    ToggleCellWall(tileCoords, 3);

                if (Input.Mouse.IsButtonDown(MouseButton.Left)) // Place last placed tile ID at cursor
                    SetTile(tileCoords, SelectedId);
                if (Input.Mouse.IsButtonDown(MouseButton.Right)) // Delete tile at cursor
                    SetTile(tileCoords, 0);
            }
        }
    }
}