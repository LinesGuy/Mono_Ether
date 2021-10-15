using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Mono_Ether.MainMenu
{
    public static class NewtonsBackground
    {
        // This is the code for the animated background in the title screen. It is very CPU intensive, makes the game very laggy, but it looks cool
        // also the code for it is like really complex so i figured i'd include it here

        // The class name is called Newton's Background, not because I stole the code from newton but because the background is actually Newton's Fractal,
        // which ironically Newton knew nothing about, so I thought it would be funny to name this class Newton's Background despite Newton
        // knowing nothing about it

        // Settings
        public static int pixelSize = 8; // Increase = less detail, more performance
        public static int iterations = 2; // Increase = more detail, less performance
        public static List<Vector2> polynomial = new List<Vector2> { new Vector2(1, 3), new Vector2(-1, 0) };
        public static List<Complex> solutions = new List<Complex> { new Complex(1, 0), new Complex(-0.809, -0.588), new Complex(0.309, 0.951), new Complex(0.309, -0.951), new Complex(-0.809, 0.588) };
        public static float offset;
        // Camera stuffs
        public static Vector2 CameraPosition = new Vector2(1, 0);
        public static float Zoom = 200;
        public static Vector2 world_to_screen(Vector2 worldPosition) { return ((worldPosition - CameraPosition) * Zoom) + GameRoot.ScreenSize / 2; }
        public static Vector2 screen_to_world_pos(Vector2 screenPos) { return (screenPos - GameRoot.ScreenSize / 2) / Zoom + CameraPosition; }
        public static void update()
        {
            solutions = new List<Complex>();
            offset += 0.005f;
            for (int i = 0; i < 5; i++)
            {
                float z = (float)i / 5 * MathF.PI * 2;
                solutions.Add(new Complex(Math.Cos(z + offset), Math.Sin(z + offset)));
            }
            // Camera (arrow keys or WASD)
            Vector2 direction = Vector2.Zero;
            if (Input.Keyboard.IsKeyDown(Keys.Left) || Input.Keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Right) || Input.Keyboard.IsKeyDown(Keys.D)) direction.X += 1;
            if (Input.Keyboard.IsKeyDown(Keys.Up) || Input.Keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Down) || Input.Keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
            if (direction != Vector2.Zero) CameraPosition += direction * 5 / Zoom;
            // Zoom (Q and E)
            if (Input.Keyboard.IsKeyDown(Keys.Q))
                Zoom /= 1.03f;
            if (Input.Keyboard.IsKeyDown(Keys.E))
                Zoom *= 1.03f;
            // Change pixel size (keys 1 through 5)
            if (Input.Keyboard.WasKeyJustDown(Keys.D1)) pixelSize = 1;
            else if (Input.Keyboard.WasKeyJustDown(Keys.D2)) pixelSize = 2;
            else if (Input.Keyboard.WasKeyJustDown(Keys.D3)) pixelSize = 4;
            else if (Input.Keyboard.WasKeyJustDown(Keys.D4)) pixelSize = 8;
            else if (Input.Keyboard.WasKeyJustDown(Keys.D5)) pixelSize = 16;
        }
        public static void draw(SpriteBatch spriteBatch)
        {
            List<List<int>> grid = new List<List<int>>();
            // Populate grid with zeros
            List<int> zeroRow = new List<int>();
            for (int x = 0; x < GameRoot.ScreenSize.X; x += pixelSize)
                zeroRow.Add(0);
            for (int y = 0; y < (int)GameRoot.ScreenSize.Y; y += pixelSize)
                grid.Add(zeroRow);
            // For each row in grid
            Parallel.For(0, (int)(GameRoot.ScreenSize.Y / pixelSize), z =>
            {
                var y = z * pixelSize;
                List<int> row = new List<int>();
                // For each pixel in row
                for (int x = 0; x < GameRoot.ScreenSize.X; x += pixelSize)
                {
                    Vector2 coords = screen_to_world_pos(new Vector2(x, y)); // Get pixel coordinates on screen and convert to cartesian coordinates
                    coords = new Vector2(coords.X * MathF.Cos(offset) - coords.Y * MathF.Sin(offset), coords.X * MathF.Sin(offset) + coords.Y * MathF.Cos(offset)); // rotate for fancy effect (not canonically part of newton method fractal)
                    Complex coordsComplex = new Complex(coords.X, coords.Y); // Convert cartesian to complex
                    Complex newtonCoordsComplex = coordsComplex; // Variable for storing newton iteration result
                    for (int i = 0; i <= iterations; i++)
                    {
                        Complex polySum = Complex.Zero; // f(x)
                        Complex derSum = Complex.Zero; // f'(x)
                        // Calculate f(x) and f'(x) values
                        foreach (var term in polynomial)
                        {
                            polySum += term.X * Complex.Pow(newtonCoordsComplex, term.Y);
                            derSum += term.X * term.Y * Complex.Pow(newtonCoordsComplex, term.Y - 1);
                        }
                        newtonCoordsComplex = newtonCoordsComplex - polySum / derSum; // x = x - f(x) / f'(x)
                    }
                    Vector2 newtonCoords = new Vector2((float)newtonCoordsComplex.Real, (float)newtonCoordsComplex.Imaginary); // Convert complex to cartesian
                    Complex bestSol = Complex.Zero; // Variable for storing nearest solution so far
                    float bestDist = float.PositiveInfinity; // Distance (squared) of nearest solution so far
                    // Find nearest root
                    foreach (var sol in solutions)
                    {
                        var dist = Vector2.DistanceSquared(new Vector2((float)sol.Real, (float)sol.Imaginary), newtonCoords); // Distance squared can be calculated faster than actual distance and does not affect method
                        if (dist < bestDist) // Check if this root is closer than the current closer root
                        {
                            bestDist = dist;
                            bestSol = sol;
                        }
                    }
                    // Colour pixel based on which root was nearest
                    int color = 0;
                    for (int i = 0; i < solutions.Count; i++)
                        if (bestSol == solutions[i])
                        {
                            color = i + 1;
                            break;
                        }
                    row.Add(color); // Add pixel to row
                }
                grid[z] = row; // Add row of pixels to grid
            });
            for (int y = 0; y < (int)(GameRoot.ScreenSize.Y / pixelSize); y++)
            { // For row in grid..
                for (int x = 0; x < (int)(GameRoot.ScreenSize.X / pixelSize); x++)
                { // Row pixel in row..
                    var cell = grid[y][x];
                    // Get colour based on colour code
                    Color color = Color.Black;
                    if (cell == 1) color = Color.Red;
                    else if (cell == 2) color = Color.Green;
                    else if (cell == 3) color = Color.Blue;
                    else if (cell == 4) color = Color.Purple;
                    else if (cell == 5) color = Color.Orange;
                    spriteBatch.Draw(Art.Pixel, new Vector2(x * pixelSize, y * pixelSize), null, color, 0f, Vector2.Zero, pixelSize, SpriteEffects.None, 0); // Draw the pixel
                }
            }
            // spriteBatch.DrawString(Art.DebugFont, screen_to_world_pos(Input.Mouse.Position.ToVector2()).ToString(), Vector2.Zero, Color.White); // Mouse coordinates

            // If you got this far and are still alive to read this, I am so sorry that you had to witness this code, but you should know that I wrote 99% of it in 40 minutes in the middle of a
            // computer science class shortly after watching half of a 30 minute video about fractals, so yeah the quality of code in this particular file wasn't that great
        }
    }
}
