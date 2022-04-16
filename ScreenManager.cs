using System.Collections.Generic;

namespace Mono_Ether {
    public static class ScreenManager {
        private static readonly Stack<GameState> _screenStack = new Stack<GameState>();
        public static GameState CurrentScreen => _screenStack.Peek();
        public static void AddScreen(GameState screen) {
            // Suspend the current screen
            if (_screenStack.Count > 0)
                CurrentScreen.Suspend();
            // Add the new screen to the top of the screen stack
            _screenStack.Push(screen);
            // Load the screen content
            CurrentScreen.LoadContent(GameRoot.Instance.Content);
            // Initialize the screen
            CurrentScreen.Initialize();
        }
        public static void RemoveScreen()
        {
            // Unload the screen
            CurrentScreen.UnloadContent();
            // Remove the screen from the top of the screen stack
            _screenStack.Pop();
            // If there are no screens left, exit the program
            if (_screenStack.Count == 0)
                GameRoot.Instance.Exit();
            else
            // Otherwise, un-suspend the screen
                CurrentScreen.Resume();
        }
    }
}
