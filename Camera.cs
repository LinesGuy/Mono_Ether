using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mono_Ether {
    public class Camera {
        public Vector2 Position = Vector2.Zero;
        public float Zoom = 1f;
        public float Orientation;
        public bool IsLerping = true;
        public Viewport CameraViewport;
        public Vector2 ScreenSize => new Vector2(CameraViewport.Width, CameraViewport.Height);
        public RenderTarget2D Screen;

        public Camera(GraphicsDevice graphicsDevice, Viewport cameraViewPort) {
            CameraViewport = cameraViewPort;
            Screen = new RenderTarget2D(graphicsDevice, CameraViewport.Width, CameraViewport.Height);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition) =>
            ((worldPosition - Position) * Zoom).Rotate(Orientation) + ScreenSize / 2f;

        public Vector2 ScreenToWorld(Vector2 screenPos) =>
            (screenPos - ScreenSize / 2f).Rotate(-Orientation) / Zoom + Position;

        public void Update(GameTime gameTime, Vector2 playerPosition, PlayerIndex playerIndex) {
            var timeScalar = MyUtils.GetTimeScalar(gameTime);
            /* Freecam (disables lerp if used) */
            var direction = Vector2.Zero;
            switch (playerIndex) {
                case PlayerIndex.One: {
                        if (Input.Keyboard.IsKeyDown(Keys.Left))
                            direction.X -= 1;
                        if (Input.Keyboard.IsKeyDown(Keys.Right))
                            direction.X += 1;
                        if (Input.Keyboard.IsKeyDown(Keys.Up))
                            direction.Y -= 1;
                        if (Input.Keyboard.IsKeyDown(Keys.Down))
                            direction.Y += 1;
                        direction *= 5 * timeScalar;
                        break;
                    }
                case PlayerIndex.Two:
                    direction += Input.GamePad.ThumbSticks.Right;
                    direction.Y = -direction.Y;
                    direction *= 10f * timeScalar;
                    break;
                case PlayerIndex.Three:
                    break;
                case PlayerIndex.Four:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }

            direction = direction.Rotate(-Orientation);
            if (direction != Vector2.Zero) {
                IsLerping = false;
                Position += direction / Zoom;
            }

            switch (playerIndex) {
                /* Zoom */
                case PlayerIndex.One: {
                        // Q and E
                        if (Input.Keyboard.IsKeyDown(Keys.Q))
                            Zoom /= 1f + 0.03f * timeScalar;
                        if (Input.Keyboard.IsKeyDown(Keys.E))
                            Zoom *= 1f + 0.03f * timeScalar;
                        // Mouse Wheel
                        if (Input.DeltaScrollWheelValue < 0)
                            Zoom *= 1f + 0.1f * timeScalar;
                        else if (Input.DeltaScrollWheelValue > 0)
                            Zoom /= 1f + 0.1f * timeScalar;
                        break;
                    }
                case PlayerIndex.Two: {
                        // Shoulder buttons
                        if (Input.GamePad.IsButtonDown(Buttons.LeftShoulder))
                            Zoom /= 1f + 0.03f * timeScalar;
                        if (Input.GamePad.IsButtonDown(Buttons.RightShoulder))
                            Zoom *= 1f + 0.03f * timeScalar;
                        break;
                    }
                case PlayerIndex.Three:
                    break;
                case PlayerIndex.Four:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }

            /* Zoom bounds */
            if (Zoom > 3f)
                Zoom = 3f;
            if (Zoom < 0.1f)
                Zoom = 0.1f;
            switch (playerIndex) {
                /* Rotate */
                case PlayerIndex.One: {
                        if (Input.Keyboard.IsKeyDown(Keys.Z))
                            Orientation += 0.01f * timeScalar;
                        if (Input.Keyboard.IsKeyDown(Keys.X))
                            Orientation -= 0.01f * timeScalar;
                        break;
                    }
                case PlayerIndex.Two: {
                        if (Input.GamePad.IsButtonDown(Buttons.LeftStick))
                            Orientation += 0.01f * timeScalar;
                        if (Input.GamePad.IsButtonDown(Buttons.RightStick))
                            Orientation -= 0.01f * timeScalar;
                        break;
                    }
                case PlayerIndex.Three:
                    break;
                case PlayerIndex.Four:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }

            /* Lerp */
            if (IsLerping) {
                var lerpSpeed = 0.1f * timeScalar; // Higher values (between 0 and 1) move towards destination faster
                Position = (1 - lerpSpeed) * Position + playerPosition * lerpSpeed;
            } else {
                switch (playerIndex) {
                    case PlayerIndex.One when Input.Keyboard.IsKeyDown(Keys.C):
                    case PlayerIndex.Two when Input.GamePad.IsButtonDown(Buttons.Back):
                        IsLerping = true; /* Press 'c' to enable lerp */
                        break;
                    case PlayerIndex.Three:
                        break;
                    case PlayerIndex.Four:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
                }
            }
        }
        public Vector2 MouseWorldCoords() => ScreenToWorld(Input.Mouse.Position.ToVector2());
    }
}
