using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public enum HudStatus { None, BossBar, Win, Lose }
    public class Hud {
        public static Hud Instance;
        public HudStatus Status = HudStatus.None;
        public float BossBarValue;
        public Hud() {
            Instance = this;
        }
        public void Update(GameTime gameTime)
        {

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
                    break;
                case HudStatus.Lose:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
