using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class PauseMenu {
        private static ButtonManager buttonManager = new ButtonManager();
        public static void Initialize() {
            buttonManager.Add("Exit", new Vector2(GameRoot.ScreenSize.X * 0.32f, GameRoot.ScreenSize.Y * 0.8f));
            buttonManager.Add("Resume", new Vector2(GameRoot.ScreenSize.X * 0.68f, GameRoot.ScreenSize.Y * 0.8f));
        }

        public static void Update() {
            switch (buttonManager.getClickedButton()) {
                case "Exit":
                    GameRoot.Instance.RemoveScreen();
                    break;
                case "Resume":
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