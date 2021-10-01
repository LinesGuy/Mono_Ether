using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono_Ether.MainMenu {
    public class LevelSelectionScreen : States.GameState {
        private ButtonManager buttonManager;
        public LevelSelectionScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            buttonManager = new ButtonManager();
            buttonManager.AddButton(new LevelButton(0, "0"));
            buttonManager.AddButton(new LevelButton(1, "1"));
            buttonManager.AddButton(new LevelButton(2, "2"));
            buttonManager.AddButton(new LevelButton(3, "3"));
            buttonManager.AddButton(new LevelButton(4, "4"));
            buttonManager.AddButton(new LevelButton(5, "5"));
            buttonManager.AddButton(new LevelButton(6, "6"));
            buttonManager.AddButton(new LevelButton(7, "7"));
            buttonManager.AddButton(new LevelButton(8, "8"));
            buttonManager.AddButton(new LevelButton(9, "9"));
            buttonManager.AddButton(new LevelButton(10, "10"));
            buttonManager.AddButton(new LevelButton(11, "11"));
            buttonManager.AddButton(new LevelButton(12, "12"));
        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            buttonManager.Update();
            var clickedButton = buttonManager.getClickedButton();
            switch (clickedButton) {
                case "asdf":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.graphicsasdfasdfasdf));
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.DrawString(Art.DebugFont, "levels xd", Vector2.Zero, Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
