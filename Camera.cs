using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether {
    public class Camera {
        public Vector2 Position = Vector2.Zero;
        public float Zoom = 1f;
        public float Orientation = 0f;
        public bool IsLerping = true;
        public Viewport CameraViewport;
        public Vector2 ScreenSize => new Vector2(CameraViewport.Width, CameraViewport.Height);
        public RenderTarget2D Screen;
        public Camera(GraphicsDevice graphicsDevice, Viewport cameraViewPort) {
            CameraViewport = cameraViewPort;
            Screen = new RenderTarget2D(graphicsDevice, CameraViewport.Width, CameraViewport.Height);
        }
        public Vector2 WorldToScreen(Vector2 worldPosition) => ((worldPosition - Position) * Zoom).Rotate(Orientation) + ScreenSize / 2f;
        public Vector2 ScreenToWorld(Vector2 screenPos) => (screenPos - ScreenSize / 2f).Rotate(-Orientation) / Zoom + Position;
        public void Update(Vector2 playerPosition, PlayerIndex playerIndex) {
            /* Freecam (disables lerp if used) */
            Vector2 direction = Vector2.Zero;
            if (playerIndex == PlayerIndex.One) {
                if (Input.Keyboard.IsKeyDown(Keys.Left))
                    direction.X -= 1;
                if (Input.Keyboard.IsKeyDown(Keys.Right))
                    direction.X += 1;
                if (Input.Keyboard.IsKeyDown(Keys.Up))
                    direction.Y -= 1;
                if (Input.Keyboard.IsKeyDown(Keys.Down))
                    direction.Y += 1;
                direction *= 5;
            } else if (playerIndex == PlayerIndex.Two) {
                direction += Input.GamePad.ThumbSticks.Right;
                direction.Y = -direction.Y;
                direction *= 10f;
            }
            direction = direction.Rotate(-Orientation);
            if (direction != Vector2.Zero) {
                IsLerping = false;
                Position += direction / Zoom;
            }
            /* Zoom */
            if (playerIndex == PlayerIndex.One) {
                // Q and E
                if (Input.Keyboard.IsKeyDown(Keys.Q))
                    Zoom /= 1.03f;
                if (Input.Keyboard.IsKeyDown(Keys.E))
                    Zoom *= 1.03f;
                // Mouse Wheel
                if (Input.DeltaScrollWheelValue < 0)
                    Zoom *= 1.1f;
                else if (Input.DeltaScrollWheelValue > 0)
                    Zoom /= 1.1f;
            } else if (playerIndex == PlayerIndex.Two) {
                // Shoulder buttons
                if (Input.GamePad.IsButtonDown(Buttons.LeftShoulder))
                    Zoom /= 1.03f;
                if (Input.GamePad.IsButtonDown(Buttons.RightShoulder))
                    Zoom *= 1.03f;
            }
            /* Zoom bounds */
            if (Zoom > 3f)
                Zoom = 3f;
            if (Zoom < 0.1f)
                Zoom = 0.1f;
            /* Rotate */
            if (playerIndex == PlayerIndex.One) {
                if (Input.Keyboard.IsKeyDown(Keys.Z))
                    Orientation += 0.01f;
                if (Input.Keyboard.IsKeyDown(Keys.X))
                    Orientation -= 0.01f;
            } else if (playerIndex == PlayerIndex.Two) {
                if (Input.GamePad.IsButtonDown(Buttons.LeftStick))
                    Orientation += 0.01f;
                if (Input.GamePad.IsButtonDown(Buttons.RightStick))
                    Orientation -= 0.01f;
            }
            /* Lerp */
            if (IsLerping) {
                const float lerpSpeed = 0.1f; // Higher values (between 0 and 1) move towards destination faster
                Position = (1 - lerpSpeed) * Position + playerPosition * lerpSpeed;
            }
            else
            {
                if (playerIndex == PlayerIndex.One && Input.Keyboard.IsKeyDown(Keys.C))
                    IsLerping = true;  /* Press 'c' to enable lerp */
                else if (playerIndex == PlayerIndex.Two && Input.GamePad.IsButtonDown(Buttons.Back))
                    IsLerping = true;
            }
        }
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
