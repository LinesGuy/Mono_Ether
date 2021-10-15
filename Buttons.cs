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
        protected Color ActiveButtonColor, InactiveButtonColor, ActiveFontColor, InactiveFontColor;
        public bool IsActive() {
            return Rect.Contains(Input.MousePosition.ToPoint());
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                spriteBatch.Draw(Texture, Pos, ActiveButtonColor);
                spriteBatch.DrawString(Art.DebugFont, Text, Pos + Texture.Size() / 2f - Art.DebugFont.MeasureString(Text), ActiveFontColor, 0f, Vector2.Zero, 2f, 0, 0);
            }
            else
            {
                spriteBatch.Draw(Texture, Pos, InactiveButtonColor);
                spriteBatch.DrawString(Art.DebugFont, Text, Pos + Texture.Size() / 2f - Art.DebugFont.MeasureString(Text), InactiveFontColor, 0f, Vector2.Zero, 2f, 0, 0);
            }
        }

        public abstract void Update();
    }
    class MenuButton : Button
    {
        public MenuButton(Vector2 pos, string text)
        {
            Texture = Art.MenuButtonBlank;
            Text = text;
            Pos = pos - Texture.Size() / 2f;
            Rect = new Rectangle(Pos.ToPoint(), Texture.Size().ToPoint());
            ActiveButtonColor = Color.Green;
            InactiveButtonColor = Color.White;
            ActiveFontColor = Color.Black;
            InactiveFontColor = Color.Black;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
    class LevelButton : Button
    {
        public float Offset;
        public const float RADIUS = 1366f;
        public LevelButton(int index, string text)
        {
            Texture = Art.MenuButtonBlank;
            Text = text;
            Offset = MathF.PI - index * (Art.MenuButtonBlank.Height / RADIUS);
            Pos = new Vector2((float)(GameRoot.ScreenSize.X / 1.2f - Math.Cos(Offset) * GameRoot.ScreenSize.Y / 2f), (float)(GameRoot.ScreenSize.Y / 2f + Math.Sin(Offset) * GameRoot.ScreenSize.Y / 1.5f));
            Rect = new Rectangle(Pos.ToPoint(), Texture.Size().ToPoint());
            ActiveButtonColor = Color.Green;
            InactiveButtonColor = Color.White;
            ActiveFontColor = Color.Black;
            InactiveFontColor = Color.Black;
        }

        public override void Update()
        {
            //Pos = new Vector2((float)(GameRoot.ScreenSize.X / 1.2f - Math.Cos(Offset) * GameRoot.ScreenSize.Y / 2f), (float)(GameRoot.ScreenSize.Y / 2f + Math.Sin(Offset) * GameRoot.ScreenSize.Y / 1.5f));
            Pos = MathUtil.FromPolar(Offset, RADIUS);
            Pos.X += GameRoot.ScreenSize.X * 1.5f;
            Pos.Y += GameRoot.ScreenSize.Y / 2f;
            Rect = new Rectangle(Pos.ToPoint(), Texture.Size().ToPoint());
            if (Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                Offset += 0.05f;
            if (Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                Offset -= 0.05f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
    class ButtonManager {
        public List<Button> Buttons = new List<Button>();
        public void Add(string name) {
            Buttons.Add(new MenuButton(new Vector2(GameRoot.ScreenSize.X / 1.3f, (Buttons.Count + 1) * 150), name));
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
        public void Update()
        {
            foreach (Button button in Buttons)
                button.Update();
        }
        public void Draw(SpriteBatch spriteBatch) {
            foreach (var button in Buttons) {
                button.Draw(spriteBatch);
            }
        }
    }
}
