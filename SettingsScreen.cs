using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Mono_Ether {
    public class SettingsScreen : GameState {
        //private ButtonManager buttonManager = new ButtonManager();
        private SliderManager sliderManager = new SliderManager();
        private Song _music;
        public SettingsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {
            sliderManager.Sliders.Add(new Slider(new Vector2(200f), 75f, "asdf", 0.5f));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);
        }

        public override void LoadContent(ContentManager content) {
            _music = content.Load<Song>("Songs/Settings");
        }

        public override void UnloadContent() {
            _music.Dispose();
        }

        public override void Update(GameTime gameTime) {
            sliderManager.Update();
        }
        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black);
            batch.DrawString(GlobalAssets.NovaSquare48, "asdf", Vector2.Zero, Color.White); // TODO remove this
            sliderManager.Draw(batch);
        }
    }

    public class Slider
    {
        public Vector2 Pos; // Centre of slider
        public float Width;
        public string Text;
        public float Value;
        public bool LastHovered = false;
        public bool IsHovered = false;
        private Vector2 _sliderPos => new Vector2(Pos.X + Width * Value / 2f, Pos.Y);
        private const float Radius = 10f;
        public Slider(Vector2 pos, float width, string text, float value = 0.5f)
        {
            Pos = pos;
            Width = width;
            Text = text;
            Value = value;
        }

        public void Update()
        {
            LastHovered = IsHovered;
            IsHovered = Vector2.DistanceSquared(Input.Mouse.Position.ToVector2(), _sliderPos) < Radius * Radius;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(GlobalAssets.Pixel, Pos, null, Color.White, 0f, new Vector2(0.5f), new Vector2(Width, 10f), 0, 0);  // Bar
            batch.Draw(GlobalAssets.Pixel, _sliderPos, null, IsHovered ? Color.Green : Color.Red, 0f, new Vector2(0.5f), new Vector2(Radius * 2f), 0, 0); // Ball
            // TODO text
        }
    }
    public class SliderManager
    {
        public List<Slider> Sliders = new List<Slider>();
        public void Update() {
            foreach (Slider slider in Sliders)
                slider.Update();
        }
        public void Draw(SpriteBatch batch) {
            foreach (Slider slider in Sliders)
                slider.Draw(batch);
        }
    }
    /*
    private static class NewtonsBackground {
        public static int pixelSize = 8; // Increase = less detail, more performance
        public static List<Vector2> polynomial = new List<Vector2> { new Vector2(1, 3), new Vector2(-1, 0) };
        public static List<Complex> solutions = new List<Complex> { new Complex(1, 0), new Complex(-0.809, -0.588), new Complex(0.309, 0.951), new Complex(0.309, -0.951), new Complex(-0.809, 0.588) };
        public static float offset;
        public static Vector2 CameraPosition = new Vector2(1, 0);
        public static float Zoom = 200;
        public static Vector2 ScreenToWorld(Vector2 screenPos) { return (screenPos - GameSettings.ScreenSize / 2) / Zoom + CameraPosition; }
        public static void Update() {
            solutions = new List<Complex>();
            offset += 0.005f;
            for (int i = 0; i < 5; i++) {
                float z = (float)i / 5 * MathF.PI * 2;
                solutions.Add(new Complex(Math.Cos(z + offset), Math.Sin(z + offset)));
            }
        }
        public static void Draw(SpriteBatch spriteBatch) {
            List<List<int>> grid = new List<List<int>>();
            List<int> zeroRow = new List<int>();
            for (int x = 0; x < GameSettings.ScreenSize.X; x += pixelSize)
                zeroRow.Add(0);
            for (int y = 0; y < (int)GameSettings.ScreenSize.Y; y += pixelSize)
                grid.Add(zeroRow);
            Parallel.For(0, (int)(GameSettings.ScreenSize.Y / pixelSize), z => {
                var y = z * pixelSize;
                List<int> row = new List<int>();
                for (int x = 0; x < GameSettings.ScreenSize.X; x += pixelSize) {
                    Vector2 coords = ScreenToWorld(new Vector2(x, y)); // Get pixel coordinates on screen and convert to cartesian coordinates
                    coords = new Vector2(coords.X * MathF.Cos(offset) - coords.Y * MathF.Sin(offset), coords.X * MathF.Sin(offset) + coords.Y * MathF.Cos(offset)); // rotate for fancy effect (not canonically part of newton method fractal)
                    Complex coordsComplex = new Complex(coords.X, coords.Y); // Convert cartesian to complex
                    Complex newtonCoordsComplex = coordsComplex; // Variable for storing newton iteration result
                    for (int i = 0; i <= 2; i++) { // Default iterations = 2
                        Complex polySum = Complex.Zero; // f(x)
                        Complex derSum = Complex.Zero; // f'(x)
                        foreach (var term in polynomial) { // Calculate f(x) and f'(x) values
                            polySum += term.X * Complex.Pow(newtonCoordsComplex, term.Y);
                            derSum += term.X * term.Y * Complex.Pow(newtonCoordsComplex, term.Y - 1);
                        }
                        newtonCoordsComplex -= polySum / derSum; // x = x - f(x) / f'(x)
                    }
                    Vector2 newtonCoords = new Vector2((float)newtonCoordsComplex.Real, (float)newtonCoordsComplex.Imaginary); // Convert complex to cartesian
                    Complex bestSol = Complex.Zero; // Variable for storing nearest solution so far
                    float bestDist = float.PositiveInfinity; // Distance (squared) of nearest solution so far
                    foreach (var sol in solutions) { // Find nearest root
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
                        if (bestSol == solutions[i]) {
                            color = i + 1;
                            break;
                        }
                    row.Add(color); // Add pixel to row
                }
                grid[z] = row; // Add row of pixels to grid
            });
            for (int y = 0; y < (int)(GameSettings.ScreenSize.Y / pixelSize); y++) {
                for (int x = 0; x < (int)(GameSettings.ScreenSize.X / pixelSize); x++) {
                    var cell = grid[y][x];
                    // Get colour based on colour code
                    Color color = Color.Black;
                    if (cell == 1) color = Color.Red;
                    else if (cell == 2) color = Color.Green;
                    else if (cell == 3) color = Color.Blue;
                    else if (cell == 4) color = Color.Purple;
                    else if (cell == 5) color = Color.Orange;
                    spriteBatch.Draw(GlobalAssets.Pixel, new Vector2(x * pixelSize, y * pixelSize), null, color, 0f, Vector2.Zero, pixelSize, SpriteEffects.None, 0); // Draw the pixel
                }
            }
        }
    }
    */
}
