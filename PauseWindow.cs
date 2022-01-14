using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether {
    public class PauseWindow {
        public bool Visible;
        private string _state = "Hidden";
        private TimeSpan _timeSinceTransition = TimeSpan.Zero;
        public void Pause() {
            _timeSinceTransition = TimeSpan.Zero;
            _state = "SlideIn";
        }
        public void Unpause() {
            _timeSinceTransition = TimeSpan.Zero;
            _state = "SlideOut";
        }
        public void Update(GameTime gameTime) {
            _timeSinceTransition += gameTime.ElapsedGameTime;
        }
        public void Draw(SpriteBatch batch) {
            Vector2 offset;
            switch (_state) {
                case "SlideIn":
                    offset = MyUtils.EInterpolate(new Vector2(0f, GameSettings.ScreenSize.Y), Vector2.Zero,
                        _timeSinceTransition.Milliseconds);
                    batch.Draw(GlobalAssets.Pixel, GameSettings.ScreenSize / 2f + offset, null,
                        new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    break;
                case "Normal":
                    batch.Draw(GlobalAssets.Pixel, GameSettings.ScreenSize / 2f, null, new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    break;
                case "SlideOut":
                    offset = MyUtils.EInterpolate(Vector2.Zero, new Vector2(0f, GameSettings.ScreenSize.Y),
                        _timeSinceTransition.Milliseconds);
                    batch.Draw(GlobalAssets.Pixel, GameSettings.ScreenSize / 2f + offset, null,
                        new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0); batch.Draw(GlobalAssets.Pixel, GameSettings.ScreenSize / 2f + offset, null,
                         new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    break;
                case "Hidden":
                    break;
            }
        }
    }
}
