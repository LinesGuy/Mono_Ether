using Microsoft.Xna.Framework.Input;
namespace Mono_Ether {
    static class Input {
        public static KeyboardState keyboard;
        public static KeyboardState lastKeyboard;
        public static MouseState mouse;
        public static MouseState lastMouse;
        public static void Update() {
            lastKeyboard = keyboard;
            lastMouse = mouse;
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
        }
        public static bool WasKeyJustDown(Keys key) {
            return keyboard.IsKeyDown(key) && !lastKeyboard.IsKeyDown(key);
        }
        public static bool WasKeyJustUp(Keys key) {
            return keyboard.IsKeyDown(key) && !lastKeyboard.IsKeyDown(key);
        }
        public static bool WasLeftButtonJustDown() {
            return mouse.LeftButton == ButtonState.Pressed && (lastMouse.LeftButton == ButtonState.Released);
        }
        public static bool WasLeftButtonJustUp() {
            return mouse.LeftButton == ButtonState.Released && (lastMouse.LeftButton == ButtonState.Pressed);
        }
        public static bool WasRightButtonJustDown() {
            return mouse.RightButton == ButtonState.Pressed && (lastMouse.RightButton == ButtonState.Released);
        }
        public static bool WasRightuttonJustUp() {
            return mouse.RightButton == ButtonState.Released && (lastMouse.RightButton == ButtonState.Pressed);
        }
        public static int DeltaScrollWheelValue() {
            return lastMouse.ScrollWheelValue - mouse.ScrollWheelValue;
        }
    }
}
