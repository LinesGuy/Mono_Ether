using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether {
    public class MenuButton {
        Vector2 Pos;
        public string Name;
        Rectangle Rect;
        Texture2D texture;

        public MenuButton(Vector2 pos, string name) {
            Name = name;
            switch (Name) {
                case "play": texture = Art.MenuPlayButton; break;
                case "credits": texture = Art.MenuCreditsButton; break;
                case "exit": texture = Art.MenuExitButton; break;
                case "settings": texture = Art.MenuSettingsButton; break;
                case "back": texture = Art.MenuBackButton; break;
                case "pauseExit": texture = Art.pauseExit; break;
                case "pauseResume": texture = Art.pauseResume; break;
                default: texture = Art.Default; break;
            }
            Pos = pos - texture.Size() / 2f;
            Rect = new Rectangle(Pos.ToPoint(), texture.Size().ToPoint());
        }
        public bool IsActive() {
            return Rect.Contains(Input.MousePosition.ToPoint());
        }
        public void Draw(SpriteBatch spriteBatch) {
            if (IsActive())
                spriteBatch.Draw(texture, Pos, Color.Green);
            else
                spriteBatch.Draw(texture, Pos, Color.CornflowerBlue);
        }
    }

    public class MenuButtonManager {
        public List<MenuButton> Buttons = new List<MenuButton>();

        public void Add(string name) {
            Buttons.Add(new MenuButton(new Vector2(GameRoot.ScreenSize.X / 2f, (Buttons.Count + 1) * 150), name));
        }
        public void Add(string name, Vector2 pos) {
            Buttons.Add(new MenuButton(pos, name));
        }

        public string getClickedButton() {
            if (Input.Mouse.WasButtonJustDown(MouseButton.Left))
                foreach (var button in Buttons) {
                    if (button.IsActive()) {
                        return button.Name;
                    }
                }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (var button in Buttons) {
                button.Draw(spriteBatch);
            }
        }
    }
}
