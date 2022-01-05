using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mono_Ether {
    public class TileMap {
        public static TileMap Instance;
        private readonly Tile[][] _grid;
        public Vector2 GridSize => new Vector2(_grid[0].Length, _grid.Length);
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

            _grid = gridList.Select(r => r.ToArray()).ToArray(); // 2D list to 2D array
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
            if (x < 0 || x >= GridSize.X || y < 0 || y >= GridSize.Y)
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
                    Debug.WriteLine($"Current iterations: {iterations}");
                }
                /* End after threshold */
                if (iterations > MaxIterations) {
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
            batch.Draw(_textures[Id - 1], TileMap.MapToScreen(Pos, camera), null, Color.White, camera.Orientation,
                Vector2.Zero, camera.Zoom, 0, 0);
        }
    }
}