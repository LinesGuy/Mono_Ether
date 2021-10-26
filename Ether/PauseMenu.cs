using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class PauseMenu {
        public static float yOffset = 0f;
        public static string state = "hidden";
        private static int slideRemainingFrames = 0;
        private static readonly ButtonManager buttonManager = new ButtonManager();
        public static void Initialize() {
            buttonManager.Add("Exit", new Vector2(GameRoot.ScreenSize.X * 0.32f, GameRoot.ScreenSize.Y * 0.8f));
            buttonManager.Add("Resume", new Vector2(GameRoot.ScreenSize.X * 0.68f, GameRoot.ScreenSize.Y * 0.8f));
        }

        public static void Update() {
            switch (state) {
                case "slideIn":
                    yOffset = (10/9) * slideRemainingFrames * slideRemainingFrames;
                    slideRemainingFrames--;
                    if (slideRemainingFrames == 0) {
                        state = "visible";
                        yOffset = 0f;
                        foreach (Button button in buttonManager.Buttons) {
                            button.Pos.Y = GameRoot.ScreenSize.Y * 0.8f - Art.MenuButtonBlank.Height / 2f;
                        }
                    }
                    foreach (Button button in buttonManager.Buttons) {
                        button.Pos.Y = GameRoot.ScreenSize.Y * 0.8f - Art.MenuButtonBlank.Height / 2f + yOffset;
                    }
                    break;
                case "slideOut":
                    yOffset = 1000f - (10/9) * slideRemainingFrames * slideRemainingFrames;
                    slideRemainingFrames--;
                    if (slideRemainingFrames == 0) {
                        state = "hidden";
                        yOffset = 1000f;
                        foreach (Button button in buttonManager.Buttons) {
                            button.Pos.Y = GameRoot.ScreenSize.Y * 0.8f - Art.MenuButtonBlank.Height / 2f + yOffset;
                        }
                    }
                    foreach (Button button in buttonManager.Buttons) {
                        button.Pos.Y = GameRoot.ScreenSize.Y * 0.8f - Art.MenuButtonBlank.Height / 2f + yOffset;
                    }
                    break;
                default:
                    break;
            }
            buttonManager.Update();
            if (state == "visible") {
                switch (buttonManager.GetClickedButton()) {
                    case "Exit":
                        GameRoot.Instance.RemoveScreenTransition();
                        break;
                    case "Resume":
                        SlideOut();
                        EtherRoot.Instance.paused = false;
                        break;

                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Art.PauseBg, GameRoot.ScreenSize / 2f + new Vector2(0, yOffset), null, Color.White, 0f, Art.PauseBg.Size() / 2f, 1f, SpriteEffects.None, 0);
            buttonManager.Draw(spriteBatch);
        }
        public static void SlideIn() {
            state = "slideIn";
            yOffset = 1000f;
            slideRemainingFrames = 30;
        }
        public static void SlideOut() {
            state = "slideOut";
            yOffset = 0f;
            slideRemainingFrames = 30;
        }
    }
}