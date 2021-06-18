using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether.Ether
{
    static class Camera
    {
        public static Vector2 CameraPosition = new Vector2(0, 0);
        public static float Zoom = 1;
        private static bool _isLerping = true;
        private static bool isAimingWithMouse = true;
        public static Vector2 world_to_screen_pos(Vector2 worldPosition)
        {
            // Scale
            Vector2 position = (worldPosition - CameraPosition) * Zoom + CameraPosition;
            // Translate
            position = position - CameraPosition;
            // Translate by half screen size
            position = position + GameRoot.ScreenSize / 2;

            return position;
        }

        private static Vector2 screen_to_world_pos(Vector2 screenPos)
        {
            // Translate by half screen size
            Vector2 position = screenPos - GameRoot.ScreenSize / 2;
            // Translate
            position = position + CameraPosition;
            //Scale
            position = (position - CameraPosition) / Zoom + CameraPosition;

            return position;
        }

        public static void Update()
        {
            // Freecam (disables lerp if used)
            Vector2 direction = Vector2.Zero;
            if (Input.Keyboard.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (Input.Keyboard.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Down))
                direction.Y += 1;
            if (direction != Vector2.Zero)
            {
                _isLerping = false;
                direction *= 5 / Zoom;
                move_relative(direction);
            }

            // Zoom (Q and E)
            if (Input.Keyboard.IsKeyDown(Keys.Q))
                Zoom /= 1.03f;
            if (Input.Keyboard.IsKeyDown(Keys.E))
                Zoom *= 1.03f;

            // Lerp
            if (_isLerping)
                Lerp(PlayerShip.Instance.Position);
            else if (Input.Keyboard.IsKeyDown(Keys.C))
                _isLerping = true;  // Press 'c' to enable lerp

        }

        private static void move_relative(Vector2 direction)
        {
            CameraPosition += direction;
        }

        private static void Lerp(Vector2 destination)
        {
            // Lerps (moves) the camera towards a given destination
            float lerp_speed = 0.1f;  // Higher values (between 0 and 1) move towards destination faster
            CameraPosition = (1 - lerp_speed) * CameraPosition + destination * lerp_speed;
        }

        public static Vector2 mouse_world_coords()
        {
            return screen_to_world_pos(Input.Mouse.Position.ToVector2());
        }
        private static Vector2 GetMouseAimDirection()
        {
            Vector2 direction = Camera.screen_to_world_pos(Input.MousePosition) - PlayerShip.Instance.Position;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }

        public static Vector2 GetAimDirection()
        {
            if (isAimingWithMouse)
                return GetMouseAimDirection();

            Vector2 direction = Vector2.Zero;

            if (Input.Keyboard.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (Input.Keyboard.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Down))
                direction.Y += 1;

            // If no aim input, return zero, otherwise normalize direction
            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }
    }
}
