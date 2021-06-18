using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
namespace Mono_Ether
{
    static class Input
    {
        public static KeyboardStateExtended Keyboard;
        public static MouseStateExtended Mouse;

        public static Vector2 MousePosition { get { return new Vector2(Mouse.X, Mouse.Y); } }

        public static void Update()
        {
            Keyboard = KeyboardExtended.GetState();
            Mouse = MouseExtended.GetState();
        }
        public static Vector2 GetMovementDirection()
        {
            Vector2 direction = Vector2.Zero;

            if (Keyboard.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (Keyboard.IsKeyDown(Keys.D))
                direction.X += 1;
            if (Keyboard.IsKeyDown(Keys.W))
                direction.Y -= 1;
            if (Keyboard.IsKeyDown(Keys.S))
                direction.Y += 1;

            if (direction.LengthSquared() > 1)
                direction.Normalize();

            return direction;
        }
    }
}
