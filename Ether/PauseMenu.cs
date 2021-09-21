using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class PauseMenu {
        private static ButtonManager buttonManager = new ButtonManager();
        public static void Initialize() {
            buttonManager.Add("pauseExit", new Vector2(320, 600));
            buttonManager.Add("pauseResume", new Vector2(940, 600));
        }

        public static void Update() {
            switch (buttonManager.getClickedButton()) {
                case "pauseExit":
                    GameRoot.Instance.RemoveScreen();
                    break;
                case "pauseResume":
                    EtherRoot.Instance.paused = false;
                    break;
            }
        }
        public static void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Art.pauseBg, GameRoot.ScreenSize / 2f, null, Color.White, 0f, Art.pauseBg.Size() / 2f, 1f, SpriteEffects.None, 0);
            buttonManager.Draw(spriteBatch);
        }
    }
}