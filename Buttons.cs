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
        //private readonly Color _hoveredButtonColor, _unhoveredButtonColor, _hoveredFontColor, _unhoveredFontColor;
        public bool IsHovered = false;
        public Button(Vector2 pos, Vector2 size, string text) {
            Pos = pos;
            Size = size;
            Text = text;
            /*_hoveredButtonColor = new Color(171, 255, 255);
            _unhoveredButtonColor = Color.White;
            _hoveredFontColor = Color.Black;
            _unhoveredFontColor = Color.Black;*/
        }
        public void Update() {
            IsHovered = MyUtils.RectangleF(Pos.X - Size.X / 2f, Pos.Y - Size.Y / 2f, Size.X, Size.Y).Contains(Input.Mouse.Position); ;
        }

        public void Draw(SpriteBatch batch, Vector2 offset, int frame) {
            //batch.Draw(GlobalAssets.Pixel, Pos + offset, null, new Color(100, 149, 237, 0.1f), 0f, GlobalAssets.Pixel.Size() / 2f, Size, 0, 0);

            DrawBox(batch, offset + new Vector2(0f, 6f).Rotate(frame / 25f), Color.CornflowerBlue);
            DrawBox(batch, offset + new Vector2(0f, 6f).Rotate(frame / 25f + MathF.PI * 2f / 3), Color.Violet);
            DrawBox(batch, offset + new Vector2(0f, 6f).Rotate(frame / 25f + MathF.PI * 4f / 3), Color.LightGreen);

            // Text
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
        private int _frame;
        public void Update()
        {
            _frame++;
            foreach (Button button in Buttons)
                button.Update();
        }
        public void Draw(SpriteBatch batch, Vector2 globalOffset) {
            foreach (Button button in Buttons)
                button.Draw(batch, globalOffset, _frame);
        }
        public void Draw(SpriteBatch batch) {
            foreach (Button button in Buttons)
                button.Draw(batch, Vector2.Zero, _frame);
        }
        public string PressedButton // null if no button is pressed
        {
            get
            {
                if (!Input.WasLeftButtonJustDown)
                    return null;
                foreach (Button button in Buttons) {
                    if (button.IsHovered)
                        return button.Text;
                }
                return null;
            }
        }
    }
}