using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether {
    public class TileMap {
        public static TileMap Instance;
        public readonly Tile[][] Grid;
        private static int _selectedTileId = 1;
        public Vector2 GridSize => new Vector2(Grid[0].Length, Grid.Length);
        public Vector2 WorldSize => GridSize * Tile.Length;
        public TileMap(string fileName) {
            Instance = this;
            var lines = File.ReadAllLines(@"Content/TileMapData/" + fileName).Where(l => l != "").ToArray();
            var gridList = new List<List<Tile>>();
            var y = 0;
            foreach (var line in lines) {
                // First line is tile map dimensions
                var rowList = new List<Tile>();
                var x = 0;
                foreach (var id in line.Trim().Split(',')) {
                    rowList.Add(new Tile(new Vector2(x, y), int.Parse(id)));
                    x++;
                }

                gridList.Add(rowList);
                y++;
            }
            Grid = gridList.Select(r => r.ToArray()).ToArray(); // 2D list to 2D array
            foreach (var row in Grid)
                foreach (var tile in row)
                    tile.UpdateWalls();
        }
        public void Update(bool editorMode) {
            if (!editorMode) return;
            /* TODO press r to save map */
            var tile = GetTileFromWorld(EntityManager.Instance.Players[0].PlayerCamera.MouseWorldCoords());
            // Set Tile ID
            if (Input.Keyboard.IsKeyDown(Keys.D1))
                _selectedTileId = 1;
            else if (Input.Keyboard.IsKeyDown(Keys.D2))
                _selectedTileId = 2;
            else if (Input.Keyboard.IsKeyDown(Keys.D3))
                _selectedTileId = 3;
            else if (Input.Keyboard.IsKeyDown(Keys.D4))
                _selectedTileId = 4;
            else if (Input.Keyboard.IsKeyDown(Keys.D0))
                _selectedTileId = 0;
            if (Input.Mouse.LeftButton == ButtonState.Pressed) { // Place last placed tile ID at cursor
                tile.Id = _selectedTileId;
                tile.UpdateNeighbouringWalls();
            }
            if (Input.Mouse.RightButton == ButtonState.Pressed) { // Delete tile at cursor
                tile.Id = 0;
                tile.UpdateNeighbouringWalls();
            }
        }
        public void Draw(SpriteBatch batch, Camera camera, bool editorMode) {
            /* Don't draw every tile in the grid. Instead, create an array of the coordinates of all four corners of the SCREEN, start from
             * the top-left most coordinate and iterate until the bottom-right most coordinate. If the camera orientation is a multiple of
             * Pi / 2 (or 90 degrees) then this will have 100% efficiency and draw ONLY tiles visible to the camera. If the orientation is
             * anything else, it will draw AT MOST twice as many tiles than is actually visible, but for most other angles it will draw far
             * less than this. Even in the worst case scenario this is significantly more efficient than drawing all tiles.
             */
            var corners = new[]
            {
                camera.ScreenToWorld(Vector2.Zero), // Top-left
                camera.ScreenToWorld(camera.ScreenSize), // Bottom-right
                camera.ScreenToWorld(new Vector2(camera.ScreenSize.X, 0)), // Top-right
                camera.ScreenToWorld(new Vector2(0, camera.ScreenSize.Y)) // Bottom-left
            };

            var xCoords = corners.Select(v => v.X).ToList();
            var yCoords = corners.Select(v => v.Y).ToList();
            const int extraTiles = 1; // Num of tiles outside of camera to render to prevent parallax'd tiles from "popping in"
            var startCol = Math.Max(0, (int)(xCoords.Min() / Tile.Length) - extraTiles);
            var endCol = Math.Min(GridSize.X, 1 + (int)(xCoords.Max() / Tile.Length) + extraTiles);
            var startRow = Math.Max(0, (int)(yCoords.Min() / Tile.Length) - extraTiles);
            var endRow = Math.Min(GridSize.Y, 1 + (int)(yCoords.Max() / Tile.Length) + extraTiles);
            if (editorMode) {
                for (var row = startRow; row < endRow; row++) {
                    for (var col = startCol; col < endCol; col++) {
                        Grid[row][col].Draw(batch, camera, 0, true);
                    }
                }
            } else {
                for (var layer = 0; layer < 4; layer++) {
                    for (var row = startRow; row < endRow; row++) {
                        for (var col = startCol; col < endCol; col++) {
                            Grid[row][col].Draw(batch, camera, layer / 5f);
                        }
                    }
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
            if (x < 0 || x >= GridSize.X || y < 0 || y >= GridSize.Y)
                return new Tile(Vector2.Zero, -1);
            return Grid[(int)y][(int)x];
        }

        public Tile GetTileFromWorld(Vector2 worldPos) {
            var mapPos = WorldtoMap(worldPos);
            return GetTileFromMap(mapPos);
        }

        public Vector2 WorldtoMap(Vector2 worldPos) => Vector2.Floor(worldPos / Tile.Size);
        public static Vector2 MapToWorld(Vector2 mapPos) => mapPos * Tile.Length;
        public static Vector2 MapToScreen(Vector2 mapPos, Camera camera) => camera.WorldToScreen(MapToWorld(mapPos));
        private const int MaxIterations = 250;
        private class Node {
            public Node Parent;
            public readonly Vector2 Position;
            public float G;
            public float H;
            public float F;

            public Node(Node parent = null, Vector2 position = new Vector2()) {
                Parent = parent;
                Position = position;
                G = 0;
                H = 0;
                F = 0;
            }
        }
        public List<Vector2> AStar(Vector2 start, Vector2 end) {
            start = new Vector2((int)(start.X / Tile.Length) * Tile.Length + Tile.Length / 2f, (int)(start.Y / Tile.Length) * Tile.Length + Tile.Length / 2f);
            /* List of pending nodes to check, initially containing the start node */
            var openList = new List<Node> { new Node(position: start) };
            /* List of nodes that have already been checked */
            var closedList = new List<Node>();
            /* Create end node from position */
            var endNode = new Node(position: end);
            /* Keep track of how many iterations we've done so far */
            var iterations = 0;
            while (openList.Count > 0) {
                iterations++;
                /* Occasionally print how many iterations we've done */
                if (iterations % (MaxIterations / 5) == 0) {
                    if (GameSettings.DebugMode)
                        Debug.WriteLine($"Current iterations: {iterations}");
                }
                /* End after threshold */
                if (iterations > MaxIterations) {
                    if (GameSettings.DebugMode)
                        Debug.WriteLine($"Ending AStar early after {iterations} iterations");
                    return null;
                }
                /* Get the next node to check from the open list */
                var currentNode = openList.First();
                var bestIndex = 0;
                var index = 0;
                foreach (var node in openList) {
                    if (node.F < currentNode.F) {
                        currentNode = node;
                        bestIndex = index;
                    }

                    index++;
                }

                /* Remove this node from the open list and add it to the closed list */
                openList.RemoveAt(bestIndex);
                closedList.Add(currentNode);
                /* Check if the current node is the end node (if so, end and return) */
                var path = new List<Vector2> { endNode.Position };
                if (Vector2.DistanceSquared(currentNode.Position, endNode.Position) <= MathF.Pow(Tile.Length * 1.5f, 2)) {
                    /* Start from the end node and keep adding the parent node to a list until we are at the start node */
                    var current = currentNode; // TODO can current be replaced with currentNode?
                    while (current != null) {
                        path.Add(current.Position);
                        current = current.Parent;
                    }

                    /* The path is currently from the end node to the start node, so we reverse it */
                    path.Reverse();
                    /* The first position is the entity's current position, so we remove it */
                    path.RemoveAt(0);
                    if (Vector2.Distance(EntityManager.Instance.Players.First().PlayerCamera.MouseWorldCoords(), start) < 30f) {
                        Debug.WriteLine("asdf");
                        foreach (var line in path) {
                            Debug.WriteLine(line);
                        }
                    }

                    return path;
                }

                /* Generate list of potential child nodes */
                var childNodes = new List<Node>();
                var offsets = new List<Vector2> {
                    new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
                    new Vector2(-1, 0), new Vector2(1, 0),
                    new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1)
                };
                /* Iterate over offsets to create new child nodes */
                foreach (var nodePosition in offsets.Select(offset => currentNode.Position + offset * Tile.Length)) {
                    /* Check if this node is in the closed list */
                    index = 0;
                    var found = false;
                    foreach (var closedNode in closedList) {
                        if (nodePosition == closedNode.Position) {
                            if (closedNode.G > currentNode.G + 1)
                                closedList[index].Parent = currentNode;
                            found = true;
                            break;
                        }
                        index += 1;
                    }
                    /* If it does exist, move to the next child node */
                    if (found) continue;
                    /* Ensure this node is not on a solid tile */
                    if (GetTileFromWorld(nodePosition).Id > 0) continue;
                    /* Create and append new node */
                    var newNode = new Node(parent: currentNode, position: nodePosition);
                    childNodes.Add(newNode);
                }
                /* Iterate over child nodes to see if we should add them to the open list */
                foreach (var child in childNodes) {
                    /* Create F, G and H values for the node */
                    child.G = currentNode.G + 1;
                    child.H = Vector2.DistanceSquared(child.Position, endNode.Position); // Length SQUARED
                    child.F = child.G + child.H;
                    /* Check if this node already exists in the open list */
                    var appendChild = true;
                    index = 0;
                    for (var i = 0; i < openList.Count; i++) {
                        var node = openList[i];
                        if (child.Position == node.Position) {
                            appendChild = false;
                            if (child.G < node.G)
                                openList[index] = child;
                        }
                        index += 1;
                    }
                    /* Add child to open list */
                    if (appendChild)
                        openList.Add(child);
                }
            }
            /* There are no more nodes to check in the open list */
            Debug.WriteLine("NO PATH FOUND ENDING EARLY");
            return null;
        }
    }
    public class Tile {
        private static Texture2D[] _textures;
        private static Texture2D[] _collisionWallTextures;
        private static Texture2D[] _collisionCornerTextures;
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
            _textures = textureList.ToArray();
            List<Texture2D> collisionWallTextureList = new List<Texture2D>
            {
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/Left"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/Up"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/Right"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/Down")
            };
            _collisionWallTextures = collisionWallTextureList.ToArray();
            List<Texture2D> collisionCornerTextureList = new List<Texture2D>
            {
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/TopLeft"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/TopRight"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/BottomRight"),
                content.Load<Texture2D>("Textures/GameScreen/Tiles/Collisions/BottomLeft")
            };
            _collisionCornerTextures = collisionCornerTextureList.ToArray();
        }
        public static void UnloadContent() {
            _textures = null;
        }
        public Vector2 TopLeft { get => Pos * Length; }
        public Vector2 Top { get => new Vector2(TopLeft.X + Length / 2f, TopLeft.Y); }
        public Vector2 TopRight { get => new Vector2(TopLeft.X + Length, TopLeft.Y); }
        public Vector2 Right { get => new Vector2(TopLeft.X + Length, TopLeft.Y + Length / 2f); }
        public Vector2 BottomRight { get => new Vector2(TopLeft.X + Length, TopLeft.Y + Length); }
        public Vector2 Bottom { get => new Vector2(TopLeft.X + Length / 2f, TopLeft.Y + Length); }
        public Vector2 BottomLeft { get => new Vector2(TopLeft.X, TopLeft.Y + Length); }
        public Vector2 Left { get => new Vector2(TopLeft.X, TopLeft.Y + Length / 2f); }
        public void UpdateWalls() {
            if (Id <= 0) {
                SolidWalls = new bool[4];
                SolidCorners = new bool[4];
                return;
            }

            // Update wall values based on surrounding tiles
            SolidWalls[0] = TileMap.Instance.GetTileFromMap(new Vector2(Pos.X - 1, Pos.Y)).Id <= 0; // Left
            SolidWalls[1] = TileMap.Instance.GetTileFromMap(new Vector2(Pos.X, Pos.Y - 1)).Id <= 0; // Top
            SolidWalls[2] = TileMap.Instance.GetTileFromMap(new Vector2(Pos.X + 1, Pos.Y)).Id <= 0; // Right
            SolidWalls[3] = TileMap.Instance.GetTileFromMap(new Vector2(Pos.X, Pos.Y + 1)).Id <= 0; // Bottom

            SolidCorners[0] = !SolidWalls[0] && !SolidWalls[1]; // Top left
            SolidCorners[1] = !SolidWalls[1] && !SolidWalls[2]; // Top right
            SolidCorners[2] = !SolidWalls[2] && !SolidWalls[3]; // Bottom right
            SolidCorners[3] = !SolidWalls[3] && !SolidWalls[0]; // Bottom left
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
                Vector2 offsetPos = Pos + offset;
                if (offsetPos.X < 0 || offsetPos.Y < 0 || offsetPos.X >= TileMap.Instance.GridSize.X || offsetPos.Y >= TileMap.Instance.GridSize.Y)
                    continue;
                TileMap.Instance.Grid[(int)offsetPos.Y][(int)offsetPos.X].UpdateWalls();
            }
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            if (Id == 0) return;
            batch.Draw(_textures[Id - 1], TileMap.MapToScreen(Pos, camera), null, Color.White, camera.Orientation,
                Vector2.Zero, camera.Zoom, 0, 0);
        }
        public void Draw(SpriteBatch batch, Camera camera, float parallax, bool editorMode = false) {
            if (Id == 0) return;
            float zoom = 1f - parallax / 10;
            float transparency = 1 - parallax;
            batch.Draw(_textures[Id - 1], (camera.WorldToScreen(TileMap.MapToWorld(Pos)) - camera.ScreenSize / 2f) * zoom + camera.ScreenSize / 2f, null, Color.White * transparency, camera.Orientation,
                Vector2.Zero, camera.Zoom, 0, 0);
            if (editorMode) {
                for (int i = 0; i < 4; i++) {
                    if (SolidWalls[i]) batch.Draw(_collisionWallTextures[i], TileMap.MapToScreen(Pos, camera), null, Color.White, camera.Orientation, Vector2.Zero, camera.Zoom, 0, 0);
                    if (SolidCorners[i]) batch.Draw(_collisionCornerTextures[i], TileMap.MapToScreen(Pos, camera), null, Color.White, camera.Orientation, Vector2.Zero, camera.Zoom, 0, 0);
                }
            }
        }
    }
}