using Mono_Ether.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether {
    public class GameRoot : Game {
        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }

        private GraphicsDeviceManager graphics;
        public GraphicsDevice graphicsasdfasdfasdf;
        private SpriteBatch spriteBatch;
        public Stack<GameState> screenStack = new Stack<GameState>();
        public bool dum_mode = false;
        public GameRoot() {
            Instance = this;
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 1366,
                PreferredBackBufferHeight = 768
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent() {
            graphicsasdfasdfasdf = GraphicsDevice;
            Art.Load(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AddScreen(new MainMenu.TitleScreen(GraphicsDevice));
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            Input.Update();
            screenStack.Peek().Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            screenStack.Peek().Draw(spriteBatch);
            base.Draw(gameTime);
        }

        public void AddScreen(GameState screen) {
            screenStack.Push(screen);
            screenStack.Peek().Initialize();
            screenStack.Peek().LoadContent(Content);
        }
        public void RemoveScreen() {
            screenStack.Peek().UnloadContent();
            screenStack.Pop();
            if (screenStack.Count == 0) {
                Exit();
            }
        }
    }
}
