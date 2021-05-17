using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.Ether
{
    static class Camera
    {
        public static Vector2 cameraPosition = new Vector2(0, 0);
        public static float zoom = 1;
        public static bool isLerping = true;
        private static bool isAimingWithMouse = true;
        public static Vector2 world_to_screen_pos(Vector2 world_position)
        {
            // Scale
            Vector2 position = (world_position - cameraPosition) * zoom + cameraPosition;
            // Translate
            position = position - cameraPosition;
            // Translate by half screen size
            position = position + GameRoot.ScreenSize / 2;

            return position;
        }

        public static Vector2 screen_to_world_pos(Vector2 screen_pos)
        {
            // Translate by half screen size
            Vector2 position = screen_pos - GameRoot.ScreenSize / 2;
            // Translate
            position = position + cameraPosition;
            //Scale
            position = (position - cameraPosition) / zoom + cameraPosition;

            return position;
        }

        public static void Update()
        {
            // Freecam (disables lerp if used)
            Vector2 direction = Vector2.Zero;
            if (Input.keyboardState.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (Input.keyboardState.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (Input.keyboardState.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (Input.keyboardState.IsKeyDown(Keys.Down))
                direction.Y += 1;
            if (direction != Vector2.Zero)
            {
                isLerping = false;
                direction *= 5 / zoom;
                move_relative(direction);
            }

            // Zoom (Q and E)
            if (Input.keyboardState.IsKeyDown(Keys.Q))
                zoom /= 1.03f;
            if (Input.keyboardState.IsKeyDown(Keys.E))
                zoom *= 1.03f;

            // Lerp
            if (isLerping)
                lerp(PlayerShip.Instance.Position);
            else if (Input.keyboardState.IsKeyDown(Keys.C))
                isLerping = true;  // Press 'c' to enable lerp

        }

        private static void move_relative(Vector2 direction)
        {
            cameraPosition += direction;
        }

        private static void lerp(Vector2 destination)
        {
            // Lerps (moves) the camera towards a given destination
            float lerp_speed = 0.1f;  // Higher values (between 0 and 1) move towards destination faster
            cameraPosition = (1 - lerp_speed) * cameraPosition + destination * lerp_speed;
        }

        public static Vector2 mouse_world_coords()
        {
            return screen_to_world_pos(Input.mouseState.Position.ToVector2());
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

            if (Input.keyboardState.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (Input.keyboardState.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (Input.keyboardState.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (Input.keyboardState.IsKeyDown(Keys.Down))
                direction.Y += 1;

            // If no aim input, return zero, otherwise normalize direction
            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }
    }
}
