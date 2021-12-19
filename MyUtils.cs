using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public enum SliderType {
        MASTER, SFX, MUSIC
    }
    internal static class MyUtils {
        public static Rectangle RectangleF(float x, float y, float width, float height) => new Rectangle((int)x, (int)y, (int)width, (int)height);
        public static Vector2 Size(this Texture2D texture) => new Vector2(texture.Width, texture.Height);
        public static void DrawStringCentered(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color) => batch.DrawString(font, text, position, color, 0f, font.MeasureString(text) / 2f, 1f, 0, 0);
        public static Vector2 Rotate(this Vector2 vector, float radians) {
            Vector2 rotated;
            rotated.X = MathF.Cos(radians) * vector.X - MathF.Sin(radians) * vector.Y;
            rotated.Y = MathF.Sin(radians) * vector.X + MathF.Cos(radians) * vector.Y;
            return rotated;
        }
        public static string SliderTypeToName(SliderType type)
        {
            return type switch
            {
                SliderType.MASTER => "Master volume",
                SliderType.SFX => "Sound volume",
                SliderType.MUSIC => "Music volume",
                _ => "asdf"
            };
        }
    }
}
