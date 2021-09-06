using Mono_Ether.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether
{
    public class GameRoot : Game
    {
        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Stack<GameState> screenStack = new Stack<GameState>();
        public GameRoot()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void LoadContent()
        {
            Art.Load(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenStack.Push(new Ether.EtherRoot(GraphicsDevice));
            screenStack.Peek().Initialize();
            screenStack.Peek().LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            screenStack.Peek().Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            screenStack.Peek().Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
