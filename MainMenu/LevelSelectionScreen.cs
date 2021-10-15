using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mono_Ether.MainMenu {
    public class LevelSelectionScreen : States.GameState {
        private ButtonManager buttonManager;
        private float buttonOffsetVelocity = 0f;
        public LevelSelectionScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            buttonManager = new ButtonManager();
            buttonManager.AddButton(new LevelButton(0, "Test stage"));
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
            NewtonsBackground.update();
            buttonManager.Update();
            var clickedButton = buttonManager.getClickedButton();
            switch (clickedButton) {
                case "Test stage":
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
            // Scroll mouse wheel to scroll through items
            buttonOffsetVelocity -= Input.Mouse.DeltaScrollWheelValue / 10000f;
            // If user let go of mouse, apply mouse velocity to scroll offset velocity
            if (Input.Mouse.WasButtonJustUp(MonoGame.Extended.Input.MouseButton.Left))
                buttonOffsetVelocity = (Input.Mouse.Y - Input.LastMouse.Y) / LevelButton.RADIUS;
            // If user is holding down left click, allow "dragging" of buttons
            if (Input.Mouse.IsButtonDown(MonoGame.Extended.Input.MouseButton.Left))
            {
                
                foreach (LevelButton button in buttonManager.Buttons)
                    button.Offset -= (Input.Mouse.Y - Input.LastMouse.Y) / LevelButton.RADIUS;
            }
            else
            {
                // Otherwise, apply offset velocity
                foreach (LevelButton button in buttonManager.Buttons)
                    button.Offset -= buttonOffsetVelocity;
                // Apply friction to velocity
                buttonOffsetVelocity /= 1.1f;
            }
            

        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            NewtonsBackground.draw(spriteBatch);
            spriteBatch.DrawString(Art.DebugFont, "Level selection screen", Vector2.Zero, Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Use arrow keys, drag the mouse or scroll the mouse wheel to scroll up/down", new Vector2(0, 28), Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
