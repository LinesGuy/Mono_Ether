using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.LevelSelection {
    public class LevelSelectionScreen : States.GameState {
        private MyButtonManager buttonManager;
        public LevelSelectionScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            buttonManager = new MyButtonManager();
            buttonManager.Add("Level One");

        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            var clickedButton = buttonManager.getClickedButton();
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            buttonManager.Draw(spriteBatch);
            spriteBatch.DrawString(Art.DebugFont, "asdf", Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
