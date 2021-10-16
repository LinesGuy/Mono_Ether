using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class Hud {
        public static void Draw(SpriteBatch spriteBatch) {
            // Top-left debug texts
            if (GameRoot.Instance.dum_mode) {
                spriteBatch.DrawString(Art.DebugFont, $"Player XY: {PlayerShip.Instance.Position.X:0.0}, {PlayerShip.Instance.Position.Y:0.0}", new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(Art.DebugFont, $"Cursor XY: {Camera.MouseWorldCoords().X:0.0}, {Camera.MouseWorldCoords().Y:0.0}", new Vector2(0, 30), Color.White);
                spriteBatch.DrawString(Art.DebugFont, $"Tile ID: {Map.GetTileFromMap(Map.WorldtoMap(Camera.MouseWorldCoords())).TileId}", new Vector2(0, 60), Color.White);
                spriteBatch.DrawString(Art.DebugFont, $"Tile XY: {Map.GetTileFromMap(Map.WorldtoMap(Camera.MouseWorldCoords())).pos}", new Vector2(0, 90), Color.White);
            }
            // Bottom-right powerups
            for (int i = 0; i < PlayerShip.Instance.activePowerPacks.Count; i++) {
                Vector2 pos = new Vector2(GameRoot.ScreenSize.X - 100 - i * 100, GameRoot.ScreenSize.Y - 100);
                var power = PlayerShip.Instance.activePowerPacks[i];
                Texture2D icon = power.PowerType switch {
                    ("ShootSpeedIncrease") => Art.PowerShootSpeedIncrease,
                    ("ShootSpeedDecrease") => Art.PowerShootSpeedDecrease,
                    ("MoveSpeedIncrease") => Art.PowerMoveSpeedIncrease,
                    ("MoveSpeedDecrease") => Art.PowerMoveSpeedDecrease,
                    _ => Art.Default,
                };
                // Draw time remaining coloured background
                Color backgroundColor = new Color(60, 214, 91);
                if (!power.isGood)
                    backgroundColor = new Color(214, 60, 60);
                float remaining = (float)power.framesRemaining / (float)power.initialFramesRemaining;
                spriteBatch.Draw(Art.Pixel, new Rectangle((int)pos.X, (int)(pos.Y + 96 * (1 - remaining)), 96, (int)(96 - 96 * (1 - remaining))), backgroundColor);
                // Draw icon
                spriteBatch.Draw(icon, pos, Color.White);
            }
            // Bottom-left hearts
            for (int i = 0; i < PlayerShip.Instance.lives; i++) {
                Vector2 pos = new Vector2(0 + i * 100, GameRoot.ScreenSize.Y - 100);
                spriteBatch.Draw(Art.Heart, pos, Color.White);
            }
        }
    }
}