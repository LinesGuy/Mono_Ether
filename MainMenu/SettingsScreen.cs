using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.MainMenu {
    public class SettingsScreen : States.GameState {
        private MyButtonManager menuButtonManager;
        public SettingsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            menuButtonManager = new MyButtonManager();
            menuButtonManager.Add("back");
        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            var clickedButton = menuButtonManager.getClickedButton();
            if (clickedButton == "back")
                GameRoot.Instance.RemoveScreen();
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(Art.DebugFont, $"there are none", Vector2.Zero, Color.White);
            menuButtonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
