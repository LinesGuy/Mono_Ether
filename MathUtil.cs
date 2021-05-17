using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;
namespace Mono_Ether
{
    static class MathUtil
    {
        public static Vector2 FromPolar(float angle, float magnitude)
        {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float Interpolate(float start, float end, float value)
        {
            return start + (end - start) * value;
        }

        public static Vector2 InterpolateV(Vector2 start, Vector2 end, float value)
        {
            return new Vector2(start.X + (end.X - start.X) * value, start.Y + (end.Y - start.Y) * value);
        }

        public static Rectangle Vect2rect(Vector2 vector, float size)
        {
            return new RectangleF(vector.X - size / 2, vector.Y - size / 2, size, size).ToRectangle();
        }

        public static Rectangle Vect2rect(Vector2 vector, float width, float height)
        {
            return new RectangleF(vector.X - width / 2, vector.Y - width / 2, width, height).ToRectangle();
        }
    }
}
