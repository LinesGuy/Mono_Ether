using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.MainMenu {
    public class TitleScreen : States.GameState {
        private ButtonManager buttonManager;
        public TitleScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            buttonManager = new ButtonManager();
            buttonManager.Add("play");
            buttonManager.Add("settings");
            buttonManager.Add("credits");
            buttonManager.Add("exit");
        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            var clickedButton = buttonManager.getClickedButton();
            switch (clickedButton) {
                case "play":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
                case "credits":
                    GameRoot.Instance.AddScreen(new MainMenu.CreditsScreen(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
                case "settings":
                    GameRoot.Instance.AddScreen(new MainMenu.SettingsScreen(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
                case "exit":
                    GameRoot.Instance.RemoveScreen();
                    break;
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(Art.DebugFont, "welcome to ether", Vector2.Zero, Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
