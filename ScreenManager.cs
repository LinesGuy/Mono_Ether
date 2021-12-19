using System.Collections.Generic;

namespace Mono_Ether {
    public static class ScreenManager {
        private static readonly Stack<GameState> _screenStack = new Stack<GameState>();
        public static GameState CurrentScreen => _screenStack.Peek();
        public static void AddScreen(GameState screen) {
            if (_screenStack.Count > 0)
                CurrentScreen.Suspend();
            _screenStack.Push(screen);
            CurrentScreen.LoadContent(GameRoot.Instance.Content);
            CurrentScreen.Initialize();
        }
        public static void RemoveScreen()
        {
            CurrentScreen.UnloadContent();
            _screenStack.Pop();
            if (_screenStack.Count == 0)
                GameRoot.Instance.Exit();
            else
                CurrentScreen.Resume();
        }
    }
}
