using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether
{
    internal static class MyUtils
    {
        public static Rectangle RectangleF(float x, float y, float width, float height) => new Rectangle((int)x, (int)y, (int)width, (int)height);
        public static Vector2 Size(this Texture2D texture) => new Vector2(texture.Width, texture.Height);
    }
}
