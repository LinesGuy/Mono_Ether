using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public class GameRoot : Game {
        public static GameRoot Instance { get; private set; }
        public static Vector2 ScreenSize = new Vector2(1366, 768);
        private readonly GraphicsDeviceManager graphics;
        public GraphicsDevice myGraphics;
        private SpriteBatch spriteBatch;
        public bool DebugMode = false;
        public GameRoot() {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Disable V-sync:
            /*graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;*/
        }
        protected override void Initialize() {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            
            graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;
            graphics.ApplyChanges();

            GameSettings.LoadSettings();
            base.Initialize();
        }
        protected override void LoadContent() {
            myGraphics = GraphicsDevice;
            Art.Load(Content);
            Sounds.Load(Content);
            Fonts.Load(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.AddScreen(new MainMenu.TitleScreen(GraphicsDevice));
            if (DebugMode) {
                // Skip straight to testing stage
                ScreenManager.AddScreen(new Ether.EtherRoot(GraphicsDevice, "debugMap.txt"));
            }
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            Input.Update();
            // Handle any ongoing screen transitions
            ScreenManager.Update();
            if (ScreenManager.screenStack.Count > 0)
                ScreenManager.screenStack.Peek().Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            ScreenManager.screenStack.Peek().Draw(spriteBatch);
            // Draw transition fade
            ScreenManager.Draw(spriteBatch);
            // Draw FPS
            spriteBatch.DrawString(Fonts.NovaSquare24, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2(GameRoot.ScreenSize.X - 200, GameRoot.ScreenSize.Y - 40), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        public void OnResize(Object sender, EventArgs e)
        {
            ScreenSize.X = Window.ClientBounds.Width;
            ScreenSize.Y = Window.ClientBounds.Height;
        }
    }
}
