using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public class GameRoot : Game {
        public static GameRoot Instance { get; private set; }
        public static readonly Vector2 ScreenSize = new Vector2(1366, 768);
        private readonly GraphicsDeviceManager graphics;
        public GraphicsDevice myGraphics;
        private SpriteBatch spriteBatch;
        public Stack<GameState> screenStack = new Stack<GameState>();
        public GameState pendingScreen;
        public int framesUntilTransition;
        public int transitionState; // 0 = none, 1 = load screen, -1 = unload screen
        private const int transitionLength = 30; // frames
        public bool dum_mode = true;
        public GameRoot() {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            /*graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;*/
        }
        protected override void Initialize() {
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
            AddScreen(new MainMenu.TitleScreen(GraphicsDevice));
            if (dum_mode) {
                // Skip straight to testing stage
                AddScreen(new Ether.EtherRoot(GraphicsDevice, "debugMap.txt"));
            }
            framesUntilTransition = 0;
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            Input.Update();
            // Handle any ongoing screen transitions
            if (framesUntilTransition > 0) {
                framesUntilTransition -= 1;
                if (framesUntilTransition == 0) {
                    if (transitionState == 1) {
                        AddScreen(pendingScreen);
                    } else if (transitionState == -1) {
                        RemoveScreen();
                    }
                    transitionState = 0;
                }
            } else if (framesUntilTransition < 0)
                framesUntilTransition += 1;
            if (screenStack.Count > 0)
                screenStack.Peek().Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            screenStack.Peek().Draw(spriteBatch);
            // Draw transition fade
            if (framesUntilTransition != 0) {
                float transparency;
                if (framesUntilTransition > 0)
                    transparency = 1 - framesUntilTransition / (float)transitionLength;
                else
                    transparency = Math.Abs(framesUntilTransition / (float)transitionLength);
                spriteBatch.Begin();
                spriteBatch.Draw(Art.Pixel, new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y), Color.Black * transparency);
                spriteBatch.End();
            }
            
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.DrawString(Fonts.NovaSquare24, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2(0, 120), Color.White);
            spriteBatch.End();
        }
        private void AddScreen(GameState screen) {
            screenStack.Push(screen);
            screenStack.Peek().Initialize();
            screenStack.Peek().LoadContent(Content);
            framesUntilTransition = -transitionLength;
        }
        public void TransitionScreen(GameState screen) {
            pendingScreen = screen;
            transitionState = 1;
            framesUntilTransition = transitionLength;
        }
        private void RemoveScreen() {
            screenStack.Peek().UnloadContent();
            screenStack.Pop();
            if (screenStack.Count == 0) {
                Exit();
            }
            framesUntilTransition = -transitionLength;
        }
        public void RemoveScreenTransition() {
            transitionState = -1;
            framesUntilTransition = transitionLength;
        }
    }
}
