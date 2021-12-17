using Microsoft.Xna.Framework.Input;
namespace Mono_Ether {
    static class Input {
        public static KeyboardState Keyboard;
        public static KeyboardState LastKeyboard;
        public static MouseState Mouse;
        public static MouseState LastMouse;
        public static void Update() {
            LastKeyboard = Keyboard;
            LastMouse = Mouse;
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }
        public static bool WasKeyJustDown(Keys key) => Keyboard.IsKeyDown(key) && LastKeyboard.IsKeyUp(key);
        public static bool WasKeyJustUp(Keys key) => Keyboard.IsKeyUp(key) && LastKeyboard.IsKeyDown(key);
        public static bool WasLeftButtonJustDown => Mouse.LeftButton == ButtonState.Pressed && LastMouse.LeftButton == ButtonState.Released;
        public static bool WasLeftButtonJustUp => Mouse.LeftButton == ButtonState.Released && LastMouse.LeftButton == ButtonState.Pressed;
        public static bool WasRightButtonJustDown =>  Mouse.RightButton == ButtonState.Pressed && LastMouse.RightButton == ButtonState.Released;
        public static bool WasRightButtonJustUp => Mouse.RightButton == ButtonState.Released && LastMouse.RightButton == ButtonState.Pressed;
        public static int DeltaScrollWheelValue => LastMouse.ScrollWheelValue - Mouse.ScrollWheelValue;
    }
}
