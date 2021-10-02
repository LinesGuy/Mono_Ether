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
            buttonManager.AddButton(new LevelButton(0, "Testing stage"));
            buttonManager.AddButton(new LevelButton(1, "Tutorial"));
            buttonManager.AddButton(new LevelButton(2, "Level one"));
            buttonManager.AddButton(new LevelButton(3, "Placeholder A"));
            buttonManager.AddButton(new LevelButton(4, "Placeholder B"));
            buttonManager.AddButton(new LevelButton(5, "Placeholder C"));
            buttonManager.AddButton(new LevelButton(6, "Placeholder D"));
            buttonManager.AddButton(new LevelButton(7, "Placeholder E"));
            buttonManager.AddButton(new LevelButton(8, "Placeholder F"));
            buttonManager.AddButton(new LevelButton(9, "Placeholder G"));
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
                case "Testing stage":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.graphicsasdfasdfasdf));
                    Ether.Map.LoadFromFile("debugMap.txt", new Vector2(64, 64));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);
                    break;
                case "Tutorial":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.graphicsasdfasdfasdf));
                    Ether.Map.LoadFromFile("Tutorial.txt", new Vector2(32, 32));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);

                    break;
                case "Level one":
                    break;
                default:
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
