using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class PauseMenu {
        private static readonly ButtonManager buttonManager = new ButtonManager();
        public static void Initialize() {
            buttonManager.Add("Exit", new Vector2(GameRoot.ScreenSize.X * 0.32f, GameRoot.ScreenSize.Y * 0.8f));
            buttonManager.Add("Resume", new Vector2(GameRoot.ScreenSize.X * 0.68f, GameRoot.ScreenSize.Y * 0.8f));
        }

        public static void Update() {
            switch (buttonManager.GetClickedButton()) {
                case "Exit":
                    GameRoot.Instance.RemoveScreenTransition();
                    break;
                case "Resume":
                    EtherRoot.Instance.paused = false;
                    break;
            }
        }
        public static void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Art.PauseBg, GameRoot.ScreenSize / 2f, null, Color.White, 0f, Art.PauseBg.Size() / 2f, 1f, SpriteEffects.None, 0);
            buttonManager.Draw(spriteBatch);
        }
    }
}