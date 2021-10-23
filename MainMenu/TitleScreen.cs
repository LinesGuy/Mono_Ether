using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
            // throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            NewtonsBackground.Update();
            var clickedButton = buttonManager.GetClickedButton();
            switch (clickedButton) {
                case "play":
                    GameRoot.Instance.TransitionScreen(new LevelSelectionScreen(GameRoot.Instance.myGraphics));
                    break;
                case "credits":
                    GameRoot.Instance.TransitionScreen(new CreditsScreen(GameRoot.Instance.myGraphics));
                    break;
                case "settings":
                    GameRoot.Instance.TransitionScreen(new SettingsScreen(GameRoot.Instance.myGraphics));
                    break;
                case "exit":
                    GameRoot.Instance.RemoveScreenTransition();
                    break;
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            NewtonsBackground.Draw(spriteBatch);
            spriteBatch.DrawString(Art.NovaSquare24, "welcome to ether", Vector2.Zero, Color.White);
            spriteBatch.DrawString(Art.NovaSquare24, "(this background has nothing to do with the game I just thought it looked cool)", new Vector2(0, 28), Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
