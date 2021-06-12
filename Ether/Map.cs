using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Mono_Ether.Ether
{
    public class Node
    {
        public Node Parent;
        public Vector2 Position;
        public float g;
        public float h;
        public float f;

        public Node(Node parent = null, Vector2 position = new Vector2())
        {
            this.Parent = parent;
            this.Position = position;
            this.g = 0;
            this.h = 0;
            this.f = 0;
        }
    }
    public static class Map
    {
        public static int cellSize = 50;
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
                    if (node.f < currentNode.f)
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
                if (Vector2.DistanceSquared(currentNode.Position, endNode.Position) <= (cellSize * 1.5f) * (cellSize * 1.5f))
                {
                    Debug.WriteLine($"found goal in iterations: {iterations}");
                    List<Vector2> path = new List<Vector2>();
                    path.Add(endNode.Position);
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
                    Vector2 nodePosition = currentNode.Position + offset * cellSize;
                    
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
                            if (closedNode.g > currentNode.g + 1)
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
                    child.g = currentNode.g + 1;
                    child.h = Vector2.DistanceSquared(child.Position, endNode.Position);  // Length SQUARED
                    child.f = child.g + child.h;
                    
                    bool appendChild = true;
                    // Child is already in the open list
                    index = 0;
                    foreach (Node node in openList)
                    {
                        if (child.Position == node.Position)
                        {
                            appendChild = false;
                            if (child.g < node.g)
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
    }
}