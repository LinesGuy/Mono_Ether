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
                        state = "camera";
                    }
                    break;
                case "camera":
                    tutorialText = "Use arrow keys to move the camera around";
                    if (Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Left) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Right) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Up) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Down)) {
                        state = "zooming";
                    }
                    break;
                case "zooming":
                    tutorialText = "Use Q and E or scroll wheel to zoom";
                    if (Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Q) ||
                        Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.E) ||
                        Input.Mouse.DeltaScrollWheelValue != 0)
                        state = "disableLerp";
                    break;
                case "disableLerp":
                    tutorialText = "Press C to disable free camera";
                    if (Input.Keyboard.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.C))
                        state = "shooting";
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
                        state = "powerpacks";
                    break;
                case "powerpacks":
                    tutorialText = "Power pack spawning is enabled. Move over a power pack to pick it up";
                    PowerPackSpawner.enabled = true;
                    if (EntityManager.player1.activePowerPacks.Count > 0)
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
