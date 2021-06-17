using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether
{
    static class Extensions
    {
        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float NextFloat(this Random rand, float minValue, float maxValue)
        {
            return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static Vector2 NextVector2(this Random rand, float minLength, float maxLength)
        {
            double theta = rand.NextDouble() * 2 * Math.PI;
            float length = rand.NextFloat(minLength, maxLength);
            return new Vector2(length * (float) Math.Cos(theta), length * (float) Math.Sin(theta));
        }

        public static Vector2 Rotate(this Vector2 vector, float radians)
        {
            Vector2 rotated;
            rotated.X = (float)(Math.Cos(radians) * vector.X - Math.Sin(radians) * vector.Y);
            rotated.Y = (float)(Math.Sin(radians) * vector.X + Math.Cos(radians) * vector.Y);
            return rotated;
        }

        public static Vector2 ScaleTo(this Vector2 vector, float length)
        {
            return vector * (length / vector.Length());
        }

        // Taxicab distance: super mega fast method for **approximating** distance from vector to another
        // dx = |x1 - x2|, dy = |y1 - y2|, distance = dx + dy
        public static float TaxicabDistanceTo(this Vector2 vector, Vector2 other)
        {
            return Math.Abs(vector.X - other.X) + Math.Abs(vector.Y - other.Y);
        }

        public static Vector2 Size(this Texture2D image)
        {
            return new Vector2(image.Width, image.Height);
        }
    }
}
