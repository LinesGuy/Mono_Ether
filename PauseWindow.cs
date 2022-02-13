using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Mono_Ether.GameSettings;

namespace Mono_Ether {
    public class PauseWindow {
        public bool Visible;
        private string _state = "Hidden";
        private TimeSpan _timeSinceTransition = TimeSpan.Zero;
        private readonly ButtonManager _pauseButtonManager = new ButtonManager();
        public PauseWindow()
        {
            /* Add buttons to button manager */
            _pauseButtonManager.Buttons.Add(new Button(ScreenSize / 2f + new Vector2(-300f, 250f), new Vector2(300, 120), "Back"));
            _pauseButtonManager.Buttons.Add(new Button(ScreenSize / 2f + new Vector2(300f, 250f), new Vector2(300, 120), "Resume"));
        }
        public void Pause() {
            _timeSinceTransition = TimeSpan.Zero;
            _state = "SlideIn";
            Visible = true;
        }
        public void UnPause() {
            _timeSinceTransition = TimeSpan.Zero;
            _state = "SlideOut";
        }

        public void Update(GameTime gameTime) {
            _timeSinceTransition += gameTime.ElapsedGameTime;
            switch (_state) {
                case "SlideIn":
                    if (_timeSinceTransition > TimeSpan.FromSeconds(0.5)) SetState("Normal");
                    HandlePauseButtons(gameTime);
                    break;
                case "Normal":
                    HandlePauseButtons(gameTime);
                    break;
                case "SlideOut":
                    if (_timeSinceTransition > TimeSpan.FromSeconds(0.5)) {
                        SetState("Hidden");
                        Visible = false;
                    }
                    HandlePauseButtons(gameTime);
                    break;
                case "Hidden":
                    break;

            }
        }
        private void HandlePauseButtons(GameTime gameTime)
        {
            _pauseButtonManager.Buttons.ForEach(b => b.Update(gameTime));
            switch (_pauseButtonManager.PressedButton)
            {
                case "Back":
                    ScreenManager.RemoveScreen();
                    break;
                case "Resume":
                    UnPause();
                    GameScreen.Instance.Mode = GameMode.Playing;
                    break;
            }
        }
        private void SetState(string state) {
            _state = state;
            _timeSinceTransition = TimeSpan.Zero;
        }
        public void Draw(SpriteBatch batch) {
            Vector2 offset;
            switch (_state) {
                case "SlideIn":
                    offset = MyUtils.EInterpolate(new Vector2(0f, ScreenSize.Y), Vector2.Zero,
                        _timeSinceTransition.Milliseconds);
                    batch.Draw(GlobalAssets.Pixel, ScreenSize / 2f + offset, null,
                        new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    _pauseButtonManager.Draw(batch, offset);
                    batch.DrawStringCentered(GlobalAssets.NovaSquare48, "GAME PAUSED", ScreenSize / 2f + offset, Color.White);
                    break;
                case "Normal":
                    batch.Draw(GlobalAssets.Pixel, ScreenSize / 2f, null, new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    _pauseButtonManager.Draw(batch);
                    batch.DrawStringCentered(GlobalAssets.NovaSquare48, "GAME PAUSED", ScreenSize / 2f, Color.White);
                    break;
                case "SlideOut":
                    offset = MyUtils.EInterpolate(Vector2.Zero, new Vector2(0f, ScreenSize.Y),
                        _timeSinceTransition.Milliseconds);
                    batch.Draw(GlobalAssets.Pixel, ScreenSize / 2f + offset, null,
                        new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0); batch.Draw(GlobalAssets.Pixel, ScreenSize / 2f + offset, null,
                         new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    _pauseButtonManager.Draw(batch, offset);
                    batch.DrawStringCentered(GlobalAssets.NovaSquare48, "GAME PAUSED", ScreenSize / 2f + offset, Color.White);
                    break;
                case "Hidden":
                    break;
            }
        }
    }
}
