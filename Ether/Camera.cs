using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    static class Camera {
        public static Vector2 CameraPosition = new Vector2(0, 0);
        public static float Zoom = 1;
        public static float Orientation = 0f;
        private static bool _isLerping = true;
        public static Vector2 WorldToScreen(Vector2 worldPosition) { return ((worldPosition - CameraPosition) * Zoom).Rotate(Orientation) + GameRoot.ScreenSize / 2f; }
        public static Vector2 ScreenToWorld(Vector2 screenPos) { return (screenPos - GameRoot.ScreenSize / 2).Rotate(-Orientation) / Zoom + CameraPosition; }
        public static void Update() {
            // Freecam (disables lerp if used)
            Vector2 direction = Vector2.Zero;
            if (Input.keyboard.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (Input.keyboard.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (Input.keyboard.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (Input.keyboard.IsKeyDown(Keys.Down))
                direction.Y += 1;
            direction = direction.Rotate(-Orientation);
            if (direction != Vector2.Zero) {
                _isLerping = false;
                CameraPosition += direction * 5 / Zoom;
            }
            // Zoom (Q and E)
            if (Input.keyboard.IsKeyDown(Keys.Q))
                Zoom /= 1.03f;
            if (Input.keyboard.IsKeyDown(Keys.E))
                Zoom *= 1.03f;
            // Zoom (mouse wheel)
            if (Input.DeltaScrollWheelValue() < 0)
                Zoom *= 1.1f;
            else if (Input.DeltaScrollWheelValue() > 0)
                Zoom /= 1.1f;
            // Zoom bounds
            if (Zoom > 3f)
                Zoom = 3f;
            if (Zoom < 0.1f)
                Zoom = 0.1f;
            // Rotate
            if (Input.keyboard.IsKeyDown(Keys.Z))
                Orientation += 0.01f;
            if (Input.keyboard.IsKeyDown(Keys.X))
                Orientation -= 0.01f;
            // Lerp
            if (_isLerping)
                Lerp(EntityManager.Player1.Position);
            else if (Input.keyboard.IsKeyDown(Keys.C))
                _isLerping = true;  // Press 'c' to enable lerp
        }
        private static void Lerp(Vector2 destination) {
            // Lerps (moves) the camera towards a given destination
            float lerp_speed = 0.1f;  // Higher values (between 0 and 1) move towards destination faster
            CameraPosition = (1 - lerp_speed) * CameraPosition + destination * lerp_speed;
        }

        public static Vector2 MouseWorldCoords() {
            return ScreenToWorld(Input.mouse.Position.ToVector2());
        }
        public static Vector2 GetMouseAimDirection(Vector2 source) {
            Vector2 direction = Camera.ScreenToWorld(Input.mouse.Position.ToVector2()) - source;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }
    }
}
