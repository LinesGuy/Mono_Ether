using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Mono_Ether.States
{
    public class GameStateManager
    {
        // Instance of game state manager
        private static GameStateManager _instance;

        // Stck for screens
        private Stack<GameState> _screens = new Stack<GameState>();

        private ContentManager _content;
        // Sets content manager
        public void SetContent(ContentManager content)
        {
            _content = content;
        }

        public static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameStateManager();
                return _instance;
            }
        }

        // Add new screen to stack
        public void AddScreen(GameState screen)
        {
            // Add screen to stack
            _screens.Push(screen);
            // Initialize screen
            _screens.Peek().Initialize();
            // Call loadcontent on screen
            if (_content != null)
                _screens.Peek().LoadContent(_content);
        }

        public void RemoveScreen()
        {
            if (_screens.Count > 0)
            {
                var screen = _screens.Peek();
                _screens.Pop();

            }
        }

        // Clear all screens from the stack
        public void ClearScreens()
        {
            while (_screens.Count > 0)
            {
                _screens.Pop();
            }
        }

        public void ChangeScreen(GameState screen)
        {
            ClearScreens();
            AddScreen(screen);
        }

        // Update the top screen
        public void Update(GameTime gameTime)
        {
            if (_screens.Count > 0)
                _screens.Peek().Update(gameTime);
        }

        // Render top screen
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_screens.Count > 0)
                _screens.Peek().Draw(spriteBatch);
        }

        public void UnloadContent()
        {
            foreach (GameState state in _screens)
                state.UnloadContent();
        }
    }
}
