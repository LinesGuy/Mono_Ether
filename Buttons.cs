using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether {
    internal abstract class Button {
        public Vector2 Pos;
        public string Text;
        protected Rectangle Rect;
        protected Texture2D Texture;
        protected Color FontColor = Color.Black;

        /*public MenuButton(Vector2 pos,  string text) {
            Pos = pos - texture.Size() / 2f;
            Rect = new Rectangle(Pos.ToPoint(), texture.Size().ToPoint());
        }*/
        public bool IsActive() {
            return Rect.Contains(Input.MousePosition.ToPoint());
        }
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Texture, Pos, Color.White);
            DrawText(spriteBatch);
        }
        public virtual void DrawText(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Art.DebugFont, Text, Pos + Texture.Size() / 2f - Art.DebugFont.MeasureString(Text), FontColor, 0f, Vector2.Zero, 2f, 0, 0);
            var asdf = Pos + Texture.Size() / 2f - Art.DebugFont.MeasureString(Text);
        }
    }
    class MenuButton : Button
    {
        public MenuButton(Vector2 pos, string text)
        {
            Texture = Art.MenuButtonBlank;
            Text = text;
            Pos = pos - Texture.Size() / 2f;
            Rect = new Rectangle(Pos.ToPoint(), Texture.Size().ToPoint());
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                spriteBatch.Draw(Texture, Pos, Color.Green);
                base.DrawText(spriteBatch);
            }
            else
            {
                base.Draw(spriteBatch);
            }
        }
    }
    class LevelButton : Button
    {
        public float offset;
        public LevelButton(int index, string text)
        {
            Texture = Art.MenuButtonBlank;
            Text = text;
            offset = 0f;
            Pos = new Vector2((float)(GameRoot.ScreenSize.X - Math.Cos(index + offset) * GameRoot.ScreenSize.Y / 2f), (float)(GameRoot.ScreenSize.Y / 2f + Math.Sin(index + offset) * GameRoot.ScreenSize.Y / 2f));
        }
    }
    class ButtonManager {
        public List<Button> Buttons = new List<Button>();

        public void Add(string name) {
            Buttons.Add(new MenuButton(new Vector2(GameRoot.ScreenSize.X / 2f, (Buttons.Count + 1) * 150), name));
        }
        public void Add(string name, Vector2 pos) {
            Buttons.Add(new MenuButton(pos, name));
        }
        public void AddButton(Button button)
        {
            Buttons.Add(button);
        }
        public string getClickedButton() {
            if (Input.Mouse.WasButtonJustDown(MouseButton.Left))
                foreach (var button in Buttons)
                    if (button.IsActive())
                        return button.Text;
            return null;
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (var button in Buttons) {
                button.Draw(spriteBatch);
            }
        }
    }
}
