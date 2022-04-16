using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Mono_Ether {
    internal static class MyUtils {
        // Create a viewport from floating point values, since the Viewport class only accepts integers
        public static Viewport ViewportF(float x, float y, float width, float height) => new Viewport((int)x, (int)y, (int)width, (int)height);
        // Create a Rectangle from floats from same reason above
        public static Rectangle RectangleF(float x, float y, float width, float height) => new Rectangle((int)x, (int)y, (int)width, (int)height);
        // Get the size of a texture
        public static Vector2 Size(this Texture2D texture) => new Vector2(texture.Width, texture.Height);
        // Draw a given string at a given position, where the centre of the string should be drawn at the position.
        public static void DrawStringCentered(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color) => batch.DrawString(font, text, position, color, 0f, font.MeasureString(text) / 2f, 1f, 0, 0);
        // Exponential interpolation, used to smoothly slide things in and out.
        public static Vector2 EInterpolate(Vector2 start, Vector2 end, float frames, float slowness = 60f) => new Vector2(start.X + (end.X - start.X) * (1 - MathF.Exp(-frames / slowness)), start.Y + (end.Y - start.Y) * (1 - MathF.Exp(-frames / slowness)));
        // Rotate a Vector2 by a given angle
        public static Vector2 Rotate(this Vector2 vector, float radians) => new Vector2(MathF.Cos(radians) * vector.X - MathF.Sin(radians) * vector.Y, MathF.Sin(radians) * vector.X + MathF.Cos(radians) * vector.Y);
        // Interpolate linearly
        public static float Interpolate(float start, float end, float value) => start + (end - start) * value;
        // Get the angle that a vector2 is pointing towards (0 radians = right)
        public static float ToAngle(this Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
        // Get a random float between two given floats
        public static float NextFloat(this Random rand, float minValue, float maxValue) => (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        // Resize a Vector2 to a given length
        public static Vector2 ScaleTo(this Vector2 vector, float length) => vector * (length / vector.Length());
        // Taxicab distance is literally the horizontal distance plus the vertical distance between two points, can be used to approximate roughly how far two objects are with very little computation.
        public static float TaxicabDistanceTo(this Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        // Create a vector2 given polar coordinates (an angle and magnitude)
        public static Vector2 FromPolar(float angle, float magnitude) => magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        // Convert elapsed game time to a ratio of the current time scale to the ideal time scale (if the program is running smoothly this will always return 1, if the program is running slow it will be less than 1)
        public static float GetTimeScalar(GameTime gameTime) => (float)(gameTime.ElapsedGameTime / TimeSpan.FromMilliseconds(16.67));
        // Get a random vector2 between two given lengths
        public static Vector2 NextVector2(this Random rand, float minLength, float maxLength) => FromPolar(rand.NextFloat(0f, MathHelper.TwoPi), rand.NextFloat(minLength, maxLength));
        // Draw a line of a given colour between two given positions
        public static void DrawLine(SpriteBatch batch, Vector2 start, Vector2 end, Color color, float width) => batch.Draw(GlobalAssets.Pixel, start, null, color, MathF.Atan2(end.Y - start.Y, end.X - start.X), new Vector2(0, 0.5f), new Vector2(MathF.Sqrt(MathF.Pow(end.X - start.X, 2) + MathF.Pow(end.Y - start.Y, 2)), width), 0, 0);
    }
}
