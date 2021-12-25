using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class Button
    {
        public Vector2 Pos;
        public Vector2 Size;
        public string Text;
        public bool IsHovered = false;
        private float _rotOffset;
        private readonly Random _random = new Random();
        public Button(Vector2 pos, Vector2 size, string text) {
            Pos = pos;
            Size = size;
            Text = text;
            _rotOffset = (float) _random.NextDouble() * MathF.PI * 2f;
        }
        public void Update(GameTime gameTime)
        {
            _rotOffset += (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
            IsHovered = MyUtils.RectangleF(Pos.X - Size.X / 2f, Pos.Y - Size.Y / 2f, Size.X, Size.Y).Contains(Input.Mouse.Position); ;
        }

        public void Draw(SpriteBatch batch, Vector2 offset) {
            DrawBox(batch, offset + new Vector2(0f, 6f).Rotate(_rotOffset + MathF.PI * 4f / 3), Color.LightGreen);
            DrawBox(batch, offset + new Vector2(0f, 6f).Rotate(_rotOffset), Color.CornflowerBlue);
            DrawBox(batch, offset + new Vector2(0f, 6f).Rotate(_rotOffset + MathF.PI * 2f / 3), Color.Violet);
            batch.DrawStringCentered(GlobalAssets.NovaSquare48, Text, Pos + offset, Color.White);
        }

        private void DrawBox(SpriteBatch batch, Vector2 offset, Color color)
        {
            batch.Draw(GlobalAssets.ButtonCorner1, MyUtils.RectangleF(Pos.X - Size.X / 2 - 4 + offset.X, Pos.Y - Size.Y / 2 - 4 + offset.Y, 7, 7), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0); // Top left
            batch.Draw(GlobalAssets.ButtonTop, MyUtils.RectangleF(Pos.X - Size.X / 2 + 3 + offset.X, Pos.Y - Size.Y / 2 - 4 + offset.Y, Size.X - 6, 7), color); // Top
            batch.Draw(GlobalAssets.ButtonCorner1, MyUtils.RectangleF(Pos.X + Size.X / 2 - 3 + offset.X, Pos.Y - Size.Y / 2 - 4 + offset.Y, 7, 7), null, color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // Top right
            batch.Draw(GlobalAssets.ButtonSide, MyUtils.RectangleF(Pos.X + Size.X / 2 - 3 + offset.X, Pos.Y - Size.Y / 2 + 3 + offset.Y, 7, Size.Y - 6), color); // Right
            batch.Draw(GlobalAssets.ButtonCorner1, MyUtils.RectangleF(Pos.X + Size.X / 2 - 3 + offset.X, Pos.Y + Size.Y / 2 - 3 + offset.Y, 7, 7), null, color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0); // Bottom right
            batch.Draw(GlobalAssets.ButtonTop, MyUtils.RectangleF(Pos.X - Size.X / 2 + 3 + offset.X, Pos.Y + Size.Y / 2 - 3 + offset.Y, Size.X - 6, 7), color); // Bottom
            batch.Draw(GlobalAssets.ButtonCorner1, MyUtils.RectangleF(Pos.X - Size.X / 2 - 4 + offset.X, Pos.Y + Size.Y / 2 - 3 + offset.Y, 7, 7), null, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0); // Bottom left
            batch.Draw(GlobalAssets.ButtonSide, MyUtils.RectangleF(Pos.X - Size.X / 2 - 4 + offset.X, Pos.Y - Size.Y / 2 + 3 + offset.Y, 7, Size.Y - 6), color); // Left
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(Pos.X - Size.X / 2 - 1 + offset.X, Pos.Y - Size.Y / 2 - 1 + offset.Y, Size.X + 2, 1), Color.White); // Top
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(Pos.X + Size.X / 2 + offset.X, Pos.Y - Size.Y / 2 + offset.Y, 1, Size.Y), Color.White); // Right
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(Pos.X - Size.X / 2 - 1 + offset.X, Pos.Y + Size.Y / 2 + offset.Y, Size.X + 2, 1), Color.White); // Bottom
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(Pos.X - Size.X / 2 - 1 + offset.X, Pos.Y - Size.Y / 2 + offset.Y, 1, Size.Y), Color.White); // Left
        }
    }
    public class ButtonManager {
        public List<Button> Buttons = new List<Button>();
        public void Draw(SpriteBatch batch, Vector2 globalOffset) {
            foreach (Button button in Buttons)
                button.Draw(batch, globalOffset);
        }
        public void Draw(SpriteBatch batch) {
            foreach (Button button in Buttons)
                button.Draw(batch, Vector2.Zero);
        }
        public string PressedButton // null if no button is pressed
        {
            get
            {
                if (!Input.WasLeftMouseJustDown)
                    return null;
                foreach (Button button in Buttons) {
                    if (button.IsHovered)
                        return button.Text;
                }
                return null;
            }
        }
    }
    public class Slider {
        private int _clickSfxDelay;
        public Vector2 SliderPos; // Centre of slider
        public float Width;
        public SliderType Type;
        public float Value;
        public bool IsHovered;
        public bool IsBeingDragged;
        private Vector2 SliderBallPos => new Vector2(SliderPos.X + (Value - 0.5f) * Width, SliderPos.Y);
        private const float Radius = 20f;
        public Slider(Vector2 sliderPos, float width, SliderType type, float value = 0.5f) {
            SliderPos = sliderPos;
            Width = width;
            Type = type;
            Value = value;
        }
        public void Update() {
            IsHovered = Vector2.DistanceSquared(Input.Mouse.Position.ToVector2(), SliderBallPos) < Radius * Radius;
            if (!IsBeingDragged && IsHovered && Input.WasLeftMouseJustDown)
                IsBeingDragged = true;
            if (IsBeingDragged) {
                if (Input.WasLeftMouseJustUp)
                    IsBeingDragged = false;
                Value = (Input.Mouse.X - SliderPos.X + Width / 2f) / Width;
                Value = Math.Clamp(Value, 0f, 1f);
                _clickSfxDelay++;
                if (_clickSfxDelay >= 6) {
                    _clickSfxDelay = 0;
                    GlobalAssets.Click.Play(GameSettings.SoundEffectVolume, 1f, 0); // TODO replace Click with PlayerShoot
                }
            }
        }
        public void Draw(SpriteBatch batch, Vector2 offset) {
            batch.Draw(GlobalAssets.Pixel, SliderPos + offset, null, Color.White, 0f, new Vector2(0.5f), new Vector2(Width, 10f), 0, 0);  // Bar
            batch.Draw(GlobalAssets.SliderBall, SliderBallPos + offset, null, IsHovered ? Color.LightCyan : Color.White, 0f, GlobalAssets.SliderBall.Size() / 2f, 1f, 0, 0); // Ball
            batch.DrawStringCentered(GlobalAssets.NovaSquare24, MyUtils.SliderTypeToName(Type), SliderPos + new Vector2(0f, -50f) + offset, Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"{Value:0.00}", SliderPos + new Vector2(Width / 2f + 50f, -GlobalAssets.NovaSquare24.MeasureString("a").Y / 2f) + offset,
                Color.White);
        }
    }
    public class SliderManager {
        public List<Slider> Sliders = new List<Slider>();
        public void Draw(SpriteBatch batch) {
            foreach (Slider slider in Sliders)
                slider.Draw(batch, Vector2.Zero);
        }
        public void Draw(SpriteBatch batch, Vector2 globalOffset) {
            foreach (Slider slider in Sliders)
                slider.Draw(batch, globalOffset);
        }
    }
    public class Switcher {
        public Vector2 Pos;
        public string Text;
        public bool State;
        public bool IsHovered;
        public Vector2 Size => GlobalAssets.SwitchOff.Size();
        public Switcher(Vector2 pos, bool state, string text) {
            Pos = pos;
            State = state;
            Text = text;
        }
        public void Update(GameTime gameTime) {
            IsHovered = MyUtils.RectangleF(Pos.X - Size.X / 2f, Pos.Y - Size.Y / 2f, Size.X, Size.Y).Contains(Input.Mouse.Position);
        }
        public void Draw(SpriteBatch batch, Vector2 offset) {
            batch.Draw(State ? GlobalAssets.SwitchOn : GlobalAssets.SwitchOff, Pos, null, Color.White, 0f, Size / 2f, 1f, 0, 0);
            batch.DrawString(GlobalAssets.NovaSquare24, Text, Pos + new Vector2(75f, -GlobalAssets.NovaSquare24.MeasureString(Text).Y / 2f), Color.White);
        }
    }
    public class SwitcherManager
    {
        public List<Switcher> Switchers = new List<Switcher>();
        public void Draw(SpriteBatch batch, Vector2 globalOffset) {
            foreach (Switcher switchers in Switchers)
               switchers.Draw(batch, globalOffset);
        }
        public void Draw(SpriteBatch batch) {
            foreach (Switcher switchers in Switchers)
                switchers.Draw(batch, Vector2.Zero);
        }
    }
}