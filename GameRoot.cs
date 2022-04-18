using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono_Ether.Ether;

namespace Mono_Ether {
    public class GameRoot : Game {
        public static GameRoot Instance { get; private set; }
        public readonly GraphicsDeviceManager Graphics;
        private SpriteBatch _batch;
        private Stopwatch _updateStopwatch;
        private Stopwatch _drawStopwatch;
        private const int HistogramLength = 300;
        private readonly float[] _updateHistory = new float[HistogramLength];
        private readonly float[] _drawHistory = new float[HistogramLength];
        private int _historyIndex;
        public GameRoot() {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize() {
            /* Handle screen resizing */
            Window.ClientSizeChanged += GameSettings.OnScreenResize;
            /* Load game settings from .txt file */
            GameSettings.LoadSettings();
            base.Initialize();
        }

        protected override void LoadContent() {
            _batch = new SpriteBatch(GraphicsDevice);
            /* Load assets used in more than one screen */
            GlobalAssets.LoadContent(Content);
            /* Add Title screen to screen stack */
            ScreenManager.AddScreen(new TitleScreen(GraphicsDevice));
            /* If debug mode is Enabled, skip straight to the testing stage */
            if (GameSettings.DebugMode) {
                MediaPlayer.Stop();
                ScreenManager.AddScreen(new GameScreen(GraphicsDevice, Level.Debug));
                //ScreenManager.AddScreen(new DoomScreen(GraphicsDevice));
            }
        }

        protected override void UnloadContent() {
            /* This is empty as this is only ever called when the game is closed */
        }

        protected override void Update(GameTime gameTime) {
            EntityManager.Instance = EntityManager.Instance;
            _updateStopwatch = Stopwatch.StartNew();
            Input.Update();
            ScreenManager.CurrentScreen.Update(gameTime);
            base.Update(gameTime);
            _updateStopwatch.Stop();
            if (!GameSettings.ShowFps) return;
            _updateHistory[_historyIndex] = _updateStopwatch.ElapsedTicks / 167000f;
            _updateHistory[(_historyIndex + 30) % HistogramLength] = 0;
            _drawHistory[(_historyIndex + 30) % HistogramLength] = 0;
            _historyIndex++;
            if (_historyIndex >= HistogramLength) _historyIndex = 0;
        }

        protected override void Draw(GameTime gameTime) {
            _drawStopwatch = Stopwatch.StartNew();
            /* Draw current screen */
            ScreenManager.CurrentScreen.Draw(_batch);
            _drawStopwatch.Stop();
            _drawHistory[_historyIndex] = _drawStopwatch.ElapsedTicks / 167000f;
            /* Draw FPS (if Enabled) */
            if (GameSettings.ShowFps) {
                _batch.Begin();
                var Font = GlobalAssets.NovaSquare24;
                var Text = $"{(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}FPS";
                _batch.DrawString(Font, Text,
                    GameSettings.ScreenSize - Font.MeasureString(Text) + new Vector2(-10f, -5f), Color.White);
                Text = $"{(int)(_drawStopwatch.ElapsedTicks / 1670f)}% draw";
                _batch.DrawString(Font, Text,
                    GameSettings.ScreenSize - Font.MeasureString(Text) + new Vector2(-10f, -34f), Color.White);
                Text = $"{(int)(_updateStopwatch.ElapsedTicks / 1670f)}% update";
                _batch.DrawString(Font, Text,
                    GameSettings.ScreenSize - Font.MeasureString(Text) + new Vector2(-10f, -63f), Color.White);
                /* Histogram */
                for (var i = 0; i < _updateHistory.Length; i++) {
                    _batch.Draw(GlobalAssets.Pixel, new Vector2(i, GameSettings.ScreenSize.Y), null, Color.CornflowerBlue, 0f, Vector2.One, new Vector2(1f, 100f * _updateHistory.ToList()[i]), 0, 0);
                    _batch.Draw(GlobalAssets.Pixel, new Vector2(i, GameSettings.ScreenSize.Y - 100f * _updateHistory.ToList()[i]), null, Color.Red, 0f, Vector2.One, new Vector2(1f, 100f * _drawHistory.ToList()[i]), 0, 0);
                }
                _batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(0f, GameSettings.ScreenSize.Y - 100f, HistogramLength, 2f), Color.White);
                _batch.End();
            }
            base.Draw(gameTime);
        }
    }
}