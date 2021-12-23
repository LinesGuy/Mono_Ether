using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether {
    public enum SliderType { Master, Sfx, Music }
    internal static class MyUtils {
        public static Rectangle RectangleF(float x, float y, float width, float height) => new Rectangle((int)x, (int)y, (int)width, (int)height);
        public static Vector2 Size(this Texture2D texture) => new Vector2(texture.Width, texture.Height);
        public static void DrawStringCentered(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color) => batch.DrawString(font, text, position, color, 0f, font.MeasureString(text) / 2f, 1f, 0, 0);
        public static Vector2 EInterpolate(Vector2 start, Vector2 end, float frames, float slowness = 60f) => new Vector2(start.X + (end.X - start.X) * (1 - MathF.Exp(-frames / slowness)), start.Y + (end.Y - start.Y) * (1 - MathF.Exp(-frames / slowness)));
        public static Vector2 Rotate(this Vector2 vector, float radians) => new Vector2(MathF.Cos(radians) * vector.X - MathF.Sin(radians) * vector.Y, MathF.Sin(radians) * vector.X + MathF.Cos(radians) * vector.Y);
        public static float ToAngle(this Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
        public static float NextFloat(this Random rand, float minValue, float maxValue) => (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        public static Vector2 ScaleTo(this Vector2 vector, float length) => vector * (length / vector.Length());
        public static float TaxicabDistanceTo(this Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        public static Vector2 NextVector2(this Random rand, float minLength, float maxLength) {
            double theta = rand.NextDouble() * 2 * Math.PI;
            float length = rand.NextFloat(minLength, maxLength);
            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
        }
        public static string SliderTypeToName(SliderType type) {
            return type switch {
                SliderType.Master => "Master volume",
                SliderType.Sfx => "Sound volume",
                SliderType.Music => "Music volume",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
