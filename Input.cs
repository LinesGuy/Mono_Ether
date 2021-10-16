using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
namespace Mono_Ether {
    static class Input {
        public static KeyboardStateExtended Keyboard;
        public static MouseStateExtended LastMouse;
        public static MouseStateExtended Mouse;
        public static Vector2 MousePosition { get { return new Vector2(Mouse.X, Mouse.Y); } }

        public static void Update() {
            Keyboard = KeyboardExtended.GetState();
            LastMouse = Mouse;
            Mouse = MouseExtended.GetState();
        }
    }
}
