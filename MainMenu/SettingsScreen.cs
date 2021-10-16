using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.MainMenu {
    public class SettingsScreen : States.GameState {
        private ButtonManager buttonManager;
        public SettingsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            buttonManager = new ButtonManager();
            buttonManager.Add("back");
        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            // throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            var clickedButton = buttonManager.GetClickedButton();
            if (clickedButton == "back")
                GameRoot.Instance.RemoveScreen();
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(Art.DebugFont, $"there are none", Vector2.Zero, Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
