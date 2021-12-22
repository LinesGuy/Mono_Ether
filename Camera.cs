using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether {
    public class Camera {
        public Vector2 Position = Vector2.Zero;
        public float Zoom = 1f;
        public float Orientation = 0f;
        //private bool _isLerping = true;

        public Vector2 WorldToScreen(Vector2 worldPosition) => ((worldPosition - Position) * Zoom).Rotate(Orientation) + GameSettings.ScreenSize / 2f;
        public Vector2 ScreenToWorld(Vector2 screenPos) => (screenPos - GameSettings.ScreenSize / 2).Rotate(-Orientation) / Zoom + Position;
        public Camera() { }
        public void Update() {
            /* Freecam (disables lerp if used) */
            Vector2 direction = Vector2.Zero;
            if (Input.Keyboard.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (Input.Keyboard.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.Down))
                direction.Y += 1;
            direction = direction.Rotate(-Orientation);
            if (direction != Vector2.Zero) {
                //_isLerping = false;
                Position += direction * 5 / Zoom;
            }
            /* Zoom (Q and E) */
            if (Input.Keyboard.IsKeyDown(Keys.Q))
                Zoom /= 1.03f;
            if (Input.Keyboard.IsKeyDown(Keys.E))
                Zoom *= 1.03f;
            /* Zoom (Mouse wheel) */
            if (Input.DeltaScrollWheelValue < 0)
                Zoom *= 1.1f;
            else if (Input.DeltaScrollWheelValue > 0)
                Zoom /= 1.1f;
            /* Zoom bounds */
            if (Zoom > 3f)
                Zoom = 3f;
            if (Zoom < 0.1f)
                Zoom = 0.1f;
            /* Rotate */
            if (Input.Keyboard.IsKeyDown(Keys.Z))
                Orientation += 0.01f;
            if (Input.Keyboard.IsKeyDown(Keys.X))
                Orientation -= 0.01f;
            /* Lerp */
            /* TODO implement lerp in GameScreen.Update()
            if (_isLerping)
            {
              // TODO lerp to average of all player positions  
                Lerp(new Vector2(
                    EntityManager
                    ));
            }
            else if (Input.Keyboard.IsKeyDown(Keys.C))
                _isLerping = true;  // Press 'c' to enable lerp
            */
        }
        // TODO remove
        /*private void Lerp(Vector2 destination) {
            // Lerps (moves) the camera towards a given destination
            float lerp_speed = 0.1f; // Higher values (between 0 and 1) move towards destination faster
            Position = (1 - lerp_speed) * Position + destination * lerp_speed;
        }*/

        public Vector2 MouseWorldCoords() {
            return ScreenToWorld(Input.Mouse.Position.ToVector2());
        }

        public Vector2 GetMouseAimDirection(Vector2 source) {
            Vector2 direction = ScreenToWorld(Input.Mouse.Position.ToVector2()) - source;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }
    }
}
