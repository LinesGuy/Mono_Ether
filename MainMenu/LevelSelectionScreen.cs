using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether.MainMenu {
    public class LevelSelectionScreen : States.GameState {
        private ButtonManager levelButtonManager;
        private ButtonManager difficultyButtonManager;
        private float buttonOffsetVelocity = 0f;
        public LevelSelectionScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            levelButtonManager = new ButtonManager();
            levelButtonManager.AddButton(new LevelButton(0, "Test stage"));
            levelButtonManager.AddButton(new LevelButton(1, "Tutorial"));
            levelButtonManager.AddButton(new LevelButton(2, "Level one"));
            levelButtonManager.AddButton(new LevelButton(3, "Level two"));
            levelButtonManager.AddButton(new LevelButton(4, "Level three"));
            levelButtonManager.AddButton(new LevelButton(5, "Super secret stage????????"));
            difficultyButtonManager = new ButtonManager();
            difficultyButtonManager.AddButton(new MenuButton(new Vector2(200f, 200f), "Easy"));
            difficultyButtonManager.AddButton(new MenuButton(new Vector2(200f, 350f), "Normal"));
            difficultyButtonManager.AddButton(new MenuButton(new Vector2(200f, 500f), "Hard"));
        }
        public override void LoadContent(ContentManager content) {

        }
        public override void UnloadContent() {

        }
        public override void Update(GameTime gameTime) {
            NewtonsBackground.Update();
            levelButtonManager.Update();
            difficultyButtonManager.Update();
            var selectedLevel = levelButtonManager.GetClickedButton();
            if (selectedLevel == "Test stage")
                GameRoot.Instance.TransitionScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics, "debugMap.txt"));
            else if (selectedLevel == "Tutorial")
                GameRoot.Instance.TransitionScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics, "Tutorial.txt"));
            else if (selectedLevel == "Level one")
                GameRoot.Instance.TransitionScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics, "LevelOne.txt"));
            else if (selectedLevel == "Level two")
                GameRoot.Instance.TransitionScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics, "LevelTwo.txt"));
            else if (selectedLevel == "Level three")
                GameRoot.Instance.TransitionScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics, "LevelThree.txt"));
            else if (selectedLevel == "Super secret stage????????")
                GameRoot.Instance.TransitionScreen(new Ether.EtherRoot(GameRoot.Instance.myGraphics, "Secret.txt"));
            // Difficulty
            var selectedDifficulty = difficultyButtonManager.GetClickedButton();
            if (selectedDifficulty != null) {
                GameSettings.Difficulty = selectedDifficulty;
                GameSettings.SaveSettings();
            }
            // Up/down arrow keys to scroll through items
            if (Input.keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                buttonOffsetVelocity -= 0.005f;
            if (Input.keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                buttonOffsetVelocity += 0.005f;
            // Scroll mouse wheel to scroll through items
            buttonOffsetVelocity -= Input.DeltaScrollWheelValue() / 10000f;
            // If user let go of mouse, apply mouse velocity to scroll offset velocity
            if (Input.WasLeftButtonJustUp())
                buttonOffsetVelocity = (Input.mouse.Y - Input.lastMouse.Y) / LevelButton.RADIUS;
            // If user is holding down left click, allow "dragging" of buttons
            if (Input.mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {

                foreach (LevelButton button in levelButtonManager.Buttons)
                    button.Offset -= (Input.mouse.Y - Input.lastMouse.Y) / LevelButton.RADIUS;
            } else {
                // Otherwise, apply offset velocity
                foreach (Button button in levelButtonManager.Buttons)
                    if (button is LevelButton levelButton)
                        levelButton.Offset -= buttonOffsetVelocity;
                // Apply friction to velocity
                buttonOffsetVelocity /= 1.1f;
            }
            // Ensure that all buttons are visible on the screen at all times
            LevelButton firstButton = (LevelButton)levelButtonManager.Buttons[0];
            LevelButton lastButton = (LevelButton)levelButtonManager.Buttons[^1];
            var slack = 0.975f;
            if (firstButton.Offset < MathF.PI * slack) {
                var delta = MathF.PI * slack - firstButton.Offset;
                foreach (LevelButton button in levelButtonManager.Buttons)
                    button.Offset += delta / 5f;
            }
            if (lastButton.Offset > MathF.PI / slack) {
                var delta = lastButton.Offset - MathF.PI / slack;
                foreach (LevelButton button in levelButtonManager.Buttons)
                    button.Offset -= delta / 5f;
            }
            // Esc to go back to title screen
            if (Input.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                GameRoot.Instance.RemoveScreenTransition();
        }
        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            NewtonsBackground.Draw(spriteBatch);
            spriteBatch.DrawString(Fonts.NovaSquare24, "Level selection screen", Vector2.Zero, Color.White);
            spriteBatch.DrawString(Fonts.NovaSquare24, "Use arrow keys, drag the mouse or scroll the mouse wheel to scroll up/down", new Vector2(0, 30), Color.White);
            spriteBatch.DrawString(Fonts.NovaSquare24, $"Currently selected difficutly: {GameSettings.Difficulty}", new Vector2(0, 60), Color.White);
            levelButtonManager.Draw(spriteBatch);
            difficultyButtonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
