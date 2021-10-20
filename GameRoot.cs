using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using System.Collections.Generic;

namespace Mono_Ether {
    public class GameRoot : Game {
        public static GameRoot Instance { get; private set; }
        public static readonly Vector2 ScreenSize = new Vector2(1366, 768);
#pragma warning disable IDE0052 // Remove unread private members
        private readonly GraphicsDeviceManager graphics;
#pragma warning restore IDE0052 // Remove unread private members
        public GraphicsDevice myGraphics;
        private SpriteBatch spriteBatch;
        public Stack<GameState> screenStack = new Stack<GameState>();
        public bool dum_mode = true;
        public GameRoot() {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize() {
            graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;
            graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent() {
            myGraphics = GraphicsDevice;
            Art.Load(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AddScreen(new MainMenu.TitleScreen(GraphicsDevice));
            if (dum_mode) {
                // Skip straight to testing stage
                AddScreen(new Ether.EtherRoot(GraphicsDevice));
                Ether.Map.LoadFromFile("debugMap.txt", new Vector2(64, 64));
                Ether.BackgroundParticleManager.Populate(Ether.Map.WorldSize, 256);
            }
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
