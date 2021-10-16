using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

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
            buttonManager.AddButton(new LevelButton(3, "Level two"));
            buttonManager.AddButton(new LevelButton(4, "Level three"));
            buttonManager.AddButton(new LevelButton(5, "Super secret stage????????"));
        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime) {
            NewtonsBackground.Update();
            buttonManager.Update();
            var clickedButton = buttonManager.GetClickedButton();
            switch (clickedButton) {
                case "Test stage":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics));
                    Ether.Map.LoadFromFile("debugMap.txt", new Vector2(64, 64));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);
                    break;
                case "Tutorial":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics));
                    Ether.Map.LoadFromFile("Tutorial.txt", new Vector2(32, 32));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 64);
                    Ether.Tutorial.state = "movement";
                    break;
                case "Level one":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics));
                    Ether.Map.LoadFromFile("LevelOne.txt", new Vector2(64, 64));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);
                    break;
                case "Level two":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics));
                    Ether.Map.LoadFromFile("LevelTwo.txt", new Vector2(64, 64));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);
                    break;
                case "Level three":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics));
                    Ether.Map.LoadFromFile("LevelThree.txt", new Vector2(64, 64));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);
                    break;
                case "Super secret stage????????":
                    GameRoot.Instance.AddScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics));
                    Ether.Map.LoadFromFile("Secret.txt", new Vector2(32, 32));
                    Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 32);
                    break;
                default:
                    break;
            }
            // Up/down arrow keys to scroll through items
            if (Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                buttonOffsetVelocity -= 0.005f;
            if (Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                buttonOffsetVelocity += 0.005f;
            // Scroll mouse wheel to scroll through items
            buttonOffsetVelocity -= Input.Mouse.DeltaScrollWheelValue / 10000f;
            // If user let go of mouse, apply mouse velocity to scroll offset velocity
            if (Input.Mouse.WasButtonJustUp(MonoGame.Extended.Input.MouseButton.Left))
                buttonOffsetVelocity = (Input.Mouse.Y - Input.LastMouse.Y) / LevelButton.RADIUS;
            // If user is holding down left click, allow "dragging" of buttons
            if (Input.Mouse.IsButtonDown(MonoGame.Extended.Input.MouseButton.Left)) {

                foreach (LevelButton button in buttonManager.Buttons)
                    button.Offset -= (Input.Mouse.Y - Input.LastMouse.Y) / LevelButton.RADIUS;
            } else {
                // Otherwise, apply offset velocity
                foreach (LevelButton button in buttonManager.Buttons)
                    button.Offset -= buttonOffsetVelocity;
                // Apply friction to velocity
                buttonOffsetVelocity /= 1.1f;
            }

            // Ensure that all buttons are visible on the screen at all times
            LevelButton firstButton = (LevelButton)buttonManager.Buttons[0];
            LevelButton lastButton = (LevelButton)buttonManager.Buttons[^1];
            var slack = 0.975f;
            if (firstButton.Offset < MathF.PI * slack) {
                var delta = MathF.PI * slack - firstButton.Offset;
                foreach (LevelButton button in buttonManager.Buttons)
                    button.Offset += delta / 5f;
            }
            if (lastButton.Offset > MathF.PI / slack) {
                var delta = lastButton.Offset - MathF.PI / slack;
                foreach (LevelButton button in buttonManager.Buttons)
                    button.Offset -= delta / 5f;
            }
            /*Debug.WriteLine($"first {firstButton.Offset}");
            Debug.WriteLine($"last {lastButton.Offset}");*/
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            NewtonsBackground.Draw(spriteBatch);
            spriteBatch.DrawString(Art.DebugFont, "Level selection screen", Vector2.Zero, Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Use arrow keys, drag the mouse or scroll the mouse wheel to scroll up/down", new Vector2(0, 28), Color.White);
            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
