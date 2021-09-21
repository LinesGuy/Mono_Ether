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
        protected Color FontColor;

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
        }
    }
    class MainMenuButton : Button
    {
        public MainMenuButton(Vector2 pos, string text)
        {
            Texture = Art.MenuButtonBlank;
            Text = text;
            Pos = pos - Texture.Size() / 2f;
            Rect = new Rectangle(Pos.ToPoint(), Texture.Size().ToPoint());
            FontColor = Color.Black;
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
    class ButtonManager {
        public List<MainMenuButton> Buttons = new List<MainMenuButton>();

        public void Add(string name) {
            Buttons.Add(new MainMenuButton(new Vector2(GameRoot.ScreenSize.X / 2f, (Buttons.Count + 1) * 150), name));
        }
        public void Add(string name, Vector2 pos) {
            Buttons.Add(new MainMenuButton(pos, name));
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
