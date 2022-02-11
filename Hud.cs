using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether {
    public enum HudStatus { None, BossBar, Win, Lose }
    public class Hud {
        private static Texture2D _winScreen;
        private static Texture2D _loseScreen;
        public static Hud Instance;
        public HudStatus Status = HudStatus.None;
        public float BossBarValue;
        private TimeSpan _timeSinceTransition = TimeSpan.Zero;
        public Hud() {
            Instance = this;
        }
        public static void LoadContent(ContentManager content) {
            _winScreen = content.Load<Texture2D>("Textures/Hud/WinScreen");
            _loseScreen = content.Load<Texture2D>("Textures/Hud/LoseScreen");
        }
        public static void UnloadContent() {
            _winScreen = null;
            _loseScreen = null;
        }
        public void Update(GameTime gameTime) {
            _timeSinceTransition += gameTime.ElapsedGameTime;
            switch (Status) {
                case HudStatus.Win:
                case HudStatus.Lose:
                    if (_timeSinceTransition > TimeSpan.FromSeconds(3))
                        ScreenManager.RemoveScreen();
                    break;
                case HudStatus.None:
                case HudStatus.BossBar:
                default:
                    break;
            }
        }
        public void ChangeStatus(HudStatus status) {
            Status = status;
            _timeSinceTransition = TimeSpan.Zero;
        }
        public void Draw(SpriteBatch batch) {
            switch (Status) {
                case HudStatus.None:
                    break;
                case HudStatus.BossBar:
                    batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(50, 50, GameSettings.ScreenSize.X - 100, 70), Color.Red);
                    batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(50, 50, (GameSettings.ScreenSize.X - 100) * BossBarValue, 70), Color.Green);
                    break;
                case HudStatus.Win:
                    batch.Draw(_winScreen, GameSettings.ScreenSize / 2f, null, Color.White * (float)(_timeSinceTransition / TimeSpan.FromSeconds(2)), 0f, _winScreen.Size() / 2f, 1f + (float)(_timeSinceTransition / TimeSpan.FromSeconds(10)), 0, 0);
                    break;
                case HudStatus.Lose:
                    batch.Draw(_loseScreen, GameSettings.ScreenSize / 2f, null, Color.White * (float)(_timeSinceTransition / TimeSpan.FromSeconds(2)), 0f, _winScreen.Size() / 2f, 1f + (float)(_timeSinceTransition / TimeSpan.FromSeconds(10)), 0, 0);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
