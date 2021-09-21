using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.MainMenu {
    public class TitleScreen : States.GameState {
        private MyButtonManager menuButtonManager;
        public TitleScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            menuButtonManager = new MyButtonManager();
            menuButtonManager.Add("play");
            menuButtonManager.Add("settings");
            menuButtonManager.Add("credits");
            menuButtonManager.Add("exit");
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
                    GameRoot.Instance.AddScreen(new LevelSelectionScreen(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
                case "credits":
                    GameRoot.Instance.AddScreen(new CreditsScreen(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
                case "settings":
                    GameRoot.Instance.AddScreen(new SettingsScreen(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
                case "exit":
                    GameRoot.Instance.RemoveScreen();
                    break;
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.DrawString(Art.DebugFont, "welcome to ether", Vector2.Zero, Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
