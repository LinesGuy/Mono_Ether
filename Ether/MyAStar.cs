using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    public class Node {
        public Node Parent;
        public Vector2 Position;
        public float G;
        public float H;
        public float F;

        public Node(Node parent = null, Vector2 position = new Vector2()) {
            this.Parent = parent;
            this.Position = position;
            this.G = 0;
            this.H = 0;
            this.F = 0;
        }
    }
    public static class MyAStar {
        public const float CellSize = Map.cellSize;

        public static List<Vector2> AStar(Vector2 start, Vector2 end) {
            start = new Vector2((int)(start.X / Map.cellSize) * Map.cellSize + Map.cellSize / 2f, (int)(start.Y / Map.cellSize) * Map.cellSize + Map.cellSize / 2f);

            List<Node> openList = new List<Node> { new Node(position: start) };
            Node endNode = new Node(position: end);
            List<Node> closedList = new List<Node>();

            int iterations = 0;
            while (openList.Count > 0) {
                iterations += 1;
                if (iterations % 50 == 0) {
                    Debug.WriteLine($"Current iterations: {iterations}");
                }

                if (iterations >= 250) {
                    Debug.WriteLine("250 ITERATIONS, ENDING EARLY");
                    return null;
                }

                // Get current node
                Node currentNode = openList[0];
                int bestIndex = 0;
                int index = 0;
                foreach (Node node in openList) {
                    if (node.F < currentNode.F) {
                        currentNode = node;
                        bestIndex = index;
                    }
                    index += 1;
                }

                openList.RemoveAt(bestIndex);
                closedList.Add(currentNode);

                // Check if found goal
                List<Vector2> path = new List<Vector2> { endNode.Position };
                if (Vector2.DistanceSquared(currentNode.Position, endNode.Position) <= (CellSize * 1.5f) * (CellSize * 1.5f)) {
                    var current = currentNode;
                    while (current != null) {
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
                foreach (var offset in offsets) {
                    Vector2 nodePosition = currentNode.Position + offset * CellSize;

                    // Check if this node is in the closed list
                    index = 0;
                    bool found = false;
                    foreach (Node closedNode in closedList) {
                        if (nodePosition == closedNode.Position) {
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
                    if (Map.GetTileFromWorld(nodePosition).TileId > 0)
                        continue;

                    // Create and append new node
                    Node newNode = new Node(parent: currentNode, position: nodePosition);
                    children.Add(newNode);
                }

                foreach (Node child in children) {
                    // Create f, g, and h values
                    child.G = currentNode.G + 1;
                    child.H = Vector2.DistanceSquared(child.Position, endNode.Position);  // Length SQUARED
                    child.F = child.G + child.H;

                    bool appendChild = true;
                    // Child is already in the open list
                    index = 0;
                    for (int i = 0; i < openList.Count; i++) {
                        Node node = openList[i];
                        if (child.Position == node.Position) {
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
            Debug.WriteLine("NO PATH FOUND ENDING EARLY");
            return null;
        }
    }
}