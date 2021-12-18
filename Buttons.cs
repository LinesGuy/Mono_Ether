using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class Button {
        public Vector2 Pos;
        public Vector2 Size => GlobalAssets.Button.Size();
        public string Text;
        private readonly Color _hoveredButtonColor, _unhoveredButtonColor, _hoveredFontColor, _unhoveredFontColor;
        public bool LastHovered = false;
        public bool IsHovered = false;
        public Button(Vector2 pos, string text) {
            Pos = pos;
            Text = text;
            _hoveredButtonColor = new Color(171, 255, 255);
            _unhoveredButtonColor = Color.White;
            _hoveredFontColor = Color.Black;
            _unhoveredFontColor = Color.Black;
        }
        public void Update() {
            LastHovered = IsHovered;
            IsHovered = MyUtils.RectangleF(Pos.X - Size.X / 2f, Pos.Y - Size.Y / 2f, Size.X, Size.Y).Contains(Input.Mouse.Position); ;
        }

        public void Draw(SpriteBatch batch, Vector2 offset) {
            batch.Draw(GlobalAssets.Button, Pos + offset, null, IsHovered ? _hoveredButtonColor : _unhoveredButtonColor, 0f, Size / 2f, 1f, 0, 0);
            batch.DrawStringCentered(GlobalAssets.NovaSquare48, Text, Pos + offset, IsHovered ? _hoveredFontColor : _unhoveredFontColor);
        }
    }
    public class ButtonManager {
        public List<Button> Buttons = new List<Button>();
        public void Update() {
            foreach (Button button in Buttons)
                button.Update();
        }
        public void Draw(SpriteBatch batch, Vector2 globalOffset) {
            foreach (Button button in Buttons)
                button.Draw(batch, globalOffset);
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