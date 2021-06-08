using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
namespace Mono_Ether
{
    static class Input
    {
        public static KeyboardState keyboardState, lastKeyboardState;
        public static MouseStateExtended mouseState, lastMouseState;

        public static Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

        public static void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;

            keyboardState = Keyboard.GetState();
            mouseState = MouseExtended.GetState();

            // THIS IS DISABLED DURING TESTING
            // If player presses arrow keys, aim with keyboard
            // If player moves mouse, aim with mose
            /*
            if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x)))
                isAimingWithMouse = false;
            else if (MousePosition != new Vector2(lastMouseState.X, lastMouseState.Y))
                isAimingWithMouse = true;
            */
        }

        public static bool WasMouseClicked(string button)
        {
            switch (button)
            {
                case "left":
                    return lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;
                case "right":
                    return lastMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }


        // Check if key was just pressed
        public static bool WasKeyPressed(Keys key)
        {
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public static Vector2 GetMovementDirection()
        {
            Vector2 direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                direction.X += 1;
            if (keyboardState.IsKeyDown(Keys.W))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S))
                direction.Y += 1;

            if (direction.LengthSquared() > 1)
                direction.Normalize();

            return direction;
        }
    }
}
