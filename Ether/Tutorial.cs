using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class Tutorial {
        public static string state = "none";
        public static string tutorialText = "";
        public static float timer = 0f;
        public static void Update() {
            switch (state) {
                case "none":
                    break;
                case "movement":
                    tutorialText = "Use WASD to move the player around";
                    EnemySpawner.enabled = false;
                    PowerPackSpawner.enabled = false;
                    if (Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.W) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.A) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.S) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.D)) {
                        state = "shooting";
                    }
                    break;
                case "shooting":
                    tutorialText = "Move the cursor around and hold left click to shoot";
                    if (Input.Mouse.WasButtonJustDown(MonoGame.Extended.Input.MouseButton.Left)) {
                        state = "shootingEnemies";
                        EnemySpawner.enabled = true;
                        timer = 10f;
                    }

                    break;
                case "shootingEnemies":
                    tutorialText = $"Enemy spawning is now enabled. Please survive {timer:0} more seconds.";
                    timer -= 0.0166f;
                    if (timer <= 0)
                        state = "starburst";
                    break;
                case "starburst":
                    tutorialText = "Right click anywhere to shoot a starburst bullet";
                    if (Input.Mouse.WasButtonJustDown(MonoGame.Extended.Input.MouseButton.Right))
                        state = "end";
                    break;
                case "end":
                    tutorialText = "That's the end of the tutorial (for now), press esc and exit";
                    break;
                default:
                    tutorialText = "this text shouldn't appear, press esc and exit pwease";
                    break;
            }
        }

        public static void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Art.DebugFont, tutorialText, GameRoot.ScreenSize / 4f, Color.White);
        }
    }
}
