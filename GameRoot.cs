using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class GameRoot : Game {
        public static GameRoot Instance { get; private set; }
        public readonly GraphicsDeviceManager Graphics;
        private SpriteBatch _batch;
        public GameRoot() {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            /* If debug mode is enabled, skip straight to the testing stage */
            // TODO implement
            //if (GameSettings.DebugMode)
            //ScreenManager.AddScreen(new Ether.EtherRoot(GraphicsDevice, "debugMap.txt"));
        }

        protected override void UnloadContent() {
            /* This is empty as this is only ever called when the game is closed */
        }

        protected override void Update(GameTime gameTime) {
            Input.Update();
            ScreenManager.CurrentScreen.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            _batch.Begin();
            /* Draw current screen */
            ScreenManager.CurrentScreen.Draw(_batch);
            /* Draw FPS (if enabled) */
            if (GameSettings.ShowFPS) {
                var Font = GlobalAssets.NovaSquare24;
                var Text = $"{(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}FPS";
                _batch.DrawString(Font, Text, GameSettings.ScreenSize - Font.MeasureString(Text) - new Vector2(10f, 5f), Color.White);
            }
            _batch.End();
            base.Draw(gameTime);
        }
    }
}
