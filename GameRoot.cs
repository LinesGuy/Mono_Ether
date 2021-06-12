using Mono_Ether.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public GameRoot()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Art.Load(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameStateManager.Instance.SetContent(Content);
            // Game
            GameStateManager.Instance.AddScreen(new Ether.EtherRoot(GraphicsDevice));
            // Intro:
            //GameStateManager.Instance.AddScreen(new Mono_Ether.MainMenu.IntroWelcome(GraphicsDevice));
            // Main menu:
            //GameStateManager.Instance.AddScreen(new Mono_Ether.MainMenu.MainMenu(GraphicsDevice));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            GameStateManager.Instance.UnloadContent();
            //base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.Update();
            GameStateManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*
            
            */
            GameStateManager.Instance.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
