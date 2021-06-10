using System.Collections.Specialized;
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
    public class Map
    {
        
    }
}