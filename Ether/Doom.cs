using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mono_Ether.Ether {
    public static class Doom {
        public static void Update() {

        }
        public static void Draw(SpriteBatch spriteBatch) {
            Vector2 topLeft = new Vector2(910, 512);
            Vector2 bottomRight = new Vector2(1366, 768);
            float fov = MathF.PI * (2f / 3f);
            for (int i = (int)topLeft.X; i < (int)bottomRight.X; i++) {

                float deltaAngle = MathUtil.Interpolate(-fov / 2f, fov / 2f, (bottomRight.X - i) / (bottomRight.X - topLeft.X));
                Vector2 p1Pos = EntityManager.Player1.Position;
                float p1Angle = EntityManager.Player1.Orientation;
                int d;
                float rayAngle = p1Angle + deltaAngle;
                for (d = 0; d <= 1024; d += 32) {
                    Vector2 currentRayPos = p1Pos + MathUtil.FromPolar(rayAngle, d);
                    if (Map.GetTileFromWorld(currentRayPos).TileId > 0) {
                        break;
                    }
                    spriteBatch.Draw(Art.Pixel, Camera.WorldToScreen(currentRayPos), Color.Red);
                }
                spriteBatch.Draw(Art.Pixel, new Rectangle(i, (int)topLeft.Y, 1, 256 - (int)MathUtil.Interpolate(0, 128, d / 1024f)), new Color(0, 255 - (int)MathUtil.Interpolate(0, 255, d / 1024f), 0, 64));
            }
        }
    }
}