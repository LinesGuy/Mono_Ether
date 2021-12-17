using System.Collections.Generic;

namespace Mono_Ether {
    public static class ScreenManager {
        private static Stack<GameState> _screenStack = new Stack<GameState>();
        public static GameState CurrentScreen => _screenStack.Peek();
        public static void AddScreen(GameState screen) {
            _screenStack.Push(screen);
            CurrentScreen.Initialize();
            CurrentScreen.LoadContent(GameRoot.Instance.Content);
        }
        public static void RemoveScreen()
        {
            CurrentScreen.UnloadContent();
            _screenStack.Pop();
            if (_screenStack.Count == 0)
            {
                GameRoot.Instance.Exit();
            }
        }
    }
}
