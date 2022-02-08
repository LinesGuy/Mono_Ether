using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class Hud {
        public static Hud Instance;
        public bool BossBarEnabled;
        public float BossBarValue;
        public Hud(bool bossBarEnabled = false) {
            Instance = this;
            BossBarEnabled = bossBarEnabled;
        }
        public void Draw(SpriteBatch batch) {
            if (!BossBarEnabled) return;
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(50, 50, GameSettings.ScreenSize.X - 100, 70), Color.Red);
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(50, 50, (GameSettings.ScreenSize.X - 100) * BossBarValue, 70), Color.Green);
        }
    }
}
