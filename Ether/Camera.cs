using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    static class Camera {
        public static Vector2 CameraPosition = new Vector2(0, 0);
        public static float Zoom = 1;
        private static bool _isLerping = true;
        private static readonly bool isAimingWithMouse = true;
        public static Vector2 WorldToScreen(Vector2 worldPosition) { return ((worldPosition - CameraPosition) * Zoom) + GameRoot.ScreenSize / 2; }
        public static Vector2 ScreenToWorld(Vector2 screenPos) { return (screenPos - GameRoot.ScreenSize / 2) / Zoom + CameraPosition; }
        public static void Update() {
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
            if (direction != Vector2.Zero) {
                _isLerping = false;
                CameraPosition += direction * 5 / Zoom;
            }
            // Zoom (Q and E)
            if (Input.Keyboard.IsKeyDown(Keys.Q))
                Zoom /= 1.03f;
            if (Input.Keyboard.IsKeyDown(Keys.E))
                Zoom *= 1.03f;
            // Zoom (mouse wheel)
            if (Input.Mouse.DeltaScrollWheelValue < 0)
                Zoom *= 1.1f;
            else if (Input.Mouse.DeltaScrollWheelValue > 0)
                Zoom /= 1.1f;
            // Lerp
            if (_isLerping)
                Lerp(EntityManager.Player1.Position);
            else if (Input.Keyboard.IsKeyDown(Keys.C))
                _isLerping = true;  // Press 'c' to enable lerp
        }
        private static void Lerp(Vector2 destination) {
            // Lerps (moves) the camera towards a given destination
            float lerp_speed = 0.1f;  // Higher values (between 0 and 1) move towards destination faster
            CameraPosition = (1 - lerp_speed) * CameraPosition + destination * lerp_speed;
        }

        public static Vector2 MouseWorldCoords() {
            return ScreenToWorld(Input.Mouse.Position.ToVector2());
        }
        private static Vector2 GetMouseAimDirection() {
            Vector2 direction = Camera.ScreenToWorld(Input.MousePosition) - EntityManager.Player1.Position;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }

        public static Vector2 GetAimDirection() {
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
