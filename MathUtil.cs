using Microsoft.Xna.Framework;
using System;
namespace Mono_Ether {
    static class MathUtil {
        public static Vector2 FromPolar(float angle, float magnitude) {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float Interpolate(float start, float end, float value) {
            return start + (end - start) * value;
        }

        public static Vector2 InterpolateV(Vector2 start, Vector2 end, float value) {
            return new Vector2(start.X + (end.X - start.X) * value, start.Y + (end.Y - start.Y) * value);
        }
    }
}
