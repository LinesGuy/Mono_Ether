using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Mono_Ether.Ether
{
    public class Node
    {
        public Node Parent;
        public Vector2 Position;
        public float G;
        public float H;
        public float F;

        public Node(Node parent = null, Vector2 position = new Vector2())
        {
            this.Parent = parent;
            this.Position = position;
            this.G = 0;
            this.H = 0;
            this.F = 0;
        }
    }
    public static class MyAStar
    {
        public const int CellSize = 50;

        public static List<Vector2> AStar(Vector2 start, Vector2 end)
        {
            //start = new Vector2((int) start.X, (int) start.Y);
            //end = new Vector2((int) end.X, (int) end.Y);
            List<Node> openList = new List<Node> { new Node(position: start) };
            Node endNode = new Node(position: end);
            List<Node> closedList = new List<Node>();

            int iterations = 0;
            while (openList.Count > 0)
            {
                iterations += 1;
                if (iterations % 300 == 0)
                    Debug.WriteLine($"Current iterations: {iterations}");
                //Debug.WriteLine("iterations=" + iterations.ToString());
                // Get current node
                Node currentNode = openList[0];
                int bestIndex = 0;
                int index = 0;
                foreach (Node node in openList)
                {
                    if (node.F < currentNode.F)
                    {
                        currentNode = node;
                        bestIndex = index;
                    }
                    index += 1;
                }
                
                openList.RemoveAt(bestIndex);
                closedList.Add(currentNode);
                
                // Check if found goal
                //if (currentNode.Position == endNode.Position)
                List<Vector2> path = new List<Vector2> {endNode.Position};
                if (Vector2.DistanceSquared(currentNode.Position, endNode.Position) <= (CellSize * 1.5f) * (CellSize * 1.5f))
                {
                    Debug.WriteLine($"found goal in iterations: {iterations}");
                    var current = currentNode;
                    while (current != null)
                    {
                        path.Add(current.Position);
                        current = current.Parent;
                    }

                    path.Reverse();
                    path.RemoveAt(0); // The first position is the entity's current position, so we remove it
                    return path;
                }
                
                // Generate children
                List<Node> children = new List<Node>();
                List<Vector2> offsets = new List<Vector2>
                {
                    new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
                    new Vector2(-1, 0), new Vector2(1, 0),
                    new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1)
    
                };
                foreach (var offset in offsets)
                {
                    Vector2 nodePosition = currentNode.Position + offset * CellSize;
                    
                    // Here we check that we are within the bounds of the map
                    // TODO: check that we are within the bounds of the map
                    // xd
                    
                    // Check if this node is in the closed list
                    index = 0;
                    bool found = false;
                    foreach (Node closedNode in closedList)
                    {
                        if (nodePosition == closedNode.Position)
                        {
                            if (closedNode.G > currentNode.G + 1)
                                closedList[index].Parent = currentNode;
                            found = true;
                            break;
                        }
                        index += 1;
                    }
                    if (found)
                        continue;
                    
                    // Here we check the terrain is walkable
                    // TODO: check the terrain is walkable
    
                    // Create and append new node
                    Node newNode = new Node(parent: currentNode, position: nodePosition);
                    children.Add(newNode);
                }
    
                foreach (Node child in children)
                {
                    // Create f, g, and h values
                    child.G = currentNode.G + 1;
                    child.H = Vector2.DistanceSquared(child.Position, endNode.Position);  // Length SQUARED
                    child.F = child.G + child.H;
                    
                    bool appendChild = true;
                    // Child is already in the open list
                    index = 0;
                    foreach (Node node in openList)
                    {
                        if (child.Position == node.Position)
                        {
                            appendChild = false;
                            if (child.G < node.G)
                                openList[index] = child;
                        }
                        index += 1;
                    }
                    
                    // Add child to open list
                    if (appendChild)
                        openList.Add(child);
                }
            }
            Debug.WriteLine("ENDING EARLY");
            return null;
        }
        
        public static int[,] Grid = new int[100, 100];
        public static void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }

    class Tiles
    {
        protected Texture2D Texture;
        protected Vector2 Position;
        public int Size { get; set; }

        public static ContentManager Content { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var screenPos = Camera.world_to_screen_pos(Position);
            var scale = Camera.Zoom * Size / Texture.Width;
            //spriteBatch.Draw(texture, screenPos, null, Color.White, 0f, size / 2f, scale, 0f, 0f);
            spriteBatch.Draw(Texture, screenPos, null, Color.White, 0f, new Vector2(Size) / 2f, scale, 0, 0);
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

        public void Generate(int[,] map, int size)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];
                    
                    if (number > 0)
                        collisionTiles.Add(new CollisionTiles(number, new Vector2(x * size + size / 2, y * size + size/2), size));
                    width = (x + 1) * size;
                    height = (y + 1) * size;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTiles tile in collisionTiles)
                tile.Draw(spriteBatch);
        }
    }
}