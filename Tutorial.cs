using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mono_Ether {
    public enum TutorialState { Movement, Camera, Zooming, DisableLerp, Shooting, ShootingEnemies, PowerPacks, StarBurst, End }
    public static class Tutorial {
        public static TutorialState state = TutorialState.Movement;
        public static string TutorialText = "";
        public static TimeSpan timer = TimeSpan.Zero;
        public static void Update(GameTime gameTime) {
            switch (state) {
                case TutorialState.Movement:
                    TutorialText = "Use WASD to move the player around";
                    EnemySpawner.Enabled = false;
                    //PowerPackSpawner.Enabled = false;
                    if (Input.WasKeyJustDown(Keys.W) ||
                        Input.WasKeyJustDown(Keys.A) ||
                        Input.WasKeyJustDown(Keys.S) ||
                        Input.WasKeyJustDown(Keys.D)) {
                        state = TutorialState.Camera;
                    }
                    break;
                case TutorialState.Camera:
                    TutorialText = "Use arrow keys to move the camera around";
                    if (Input.WasKeyJustDown(Keys.Left) ||
                        Input.WasKeyJustDown(Keys.Right) ||
                        Input.WasKeyJustDown(Keys.Up) ||
                        Input.WasKeyJustDown(Keys.Down)) {
                        state = TutorialState.Zooming;
                    }
                    break;
                case TutorialState.Zooming:
                    TutorialText = "Use Q and E or scroll wheel to zoom";
                    if (Input.WasKeyJustDown(Keys.Q) ||
                        Input.WasKeyJustDown(Keys.E) ||
                        Input.DeltaScrollWheelValue != 0)
                        state = TutorialState.DisableLerp;
                    break;
                case TutorialState.DisableLerp:
                    TutorialText = "Press C to disable free camera";
                    if (Input.WasKeyJustDown(Keys.C))
                        state = TutorialState.Shooting;
                    break;
                case TutorialState.Shooting:
                    TutorialText = "Move the cursor around and hold left click to shoot";
                    if (Input.WasLeftMouseJustDown) {
                        state = TutorialState.ShootingEnemies;
                        EnemySpawner.Enabled = true;
                        timer = TimeSpan.FromSeconds(10);
                    }
                    break;
                case TutorialState.ShootingEnemies:
                    TutorialText = $"Enemy spawning is now Enabled. Please survive {timer.Seconds} more seconds.";
                    timer -= gameTime.ElapsedGameTime;
                    if (timer <= TimeSpan.Zero)
                        state = TutorialState.PowerPacks;
                    break;
                case TutorialState.PowerPacks:
                    TutorialText = "Power pack spawning is Enabled. Move over a power pack to pick it up";
                    PowerPackSpawner.Instance.Enabled = true;
                    if (EntityManager.Instance.Players[0].ActivePowerPacks.Count > 0)
                        state = TutorialState.StarBurst;
                    break;
                case TutorialState.StarBurst:
                    TutorialText = "Right click anywhere to shoot a StarBurst bullet";
                    if (Input.WasRightMouseJustDown)
                        state = TutorialState.End;
                    break;
                case TutorialState.End:
                    TutorialText = "That's the end of the tutorial (for now), press esc and exit";
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Draw(SpriteBatch spriteBatch, Camera camera) {
            spriteBatch.DrawStringCentered(GlobalAssets.NovaSquare24, TutorialText, new Vector2(camera.ScreenSize.X / 2f, camera.ScreenSize.Y / 3f), Color.White);
        }
    }
}
