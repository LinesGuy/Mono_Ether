using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    public static class Hud {
        public static void Draw(SpriteBatch spriteBatch) {
            // Debug texts
            spriteBatch.DrawString(Art.DebugFont, "Player pos: " + PlayerShip.Instance.Position.ToString(), new Vector2(0, 0), Color.White);
            if (EtherRoot.Instance.paused)
                spriteBatch.DrawString(Art.DebugFont, "PAUSED", new Vector2(0, 30), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor world/screen pos: " + Camera.mouse_world_coords().ToString() + Input.MousePosition, new Vector2(0, 60), Color.White);
            if (EtherRoot.Instance.editorMode)
                spriteBatch.DrawString(Art.DebugFont, "EDITOR MODE", new Vector2(0, 90), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor tile ID: " + Map.GetTileFromMap(Map.WorldtoMap(Camera.mouse_world_coords())).TileId, new Vector2(0, 120), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor Tile pos: " + Map.GetTileFromMap(Map.WorldtoMap(Camera.mouse_world_coords())).pos, new Vector2(0, 150), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Player Lives: " + PlayerShip.Instance.lives.ToString(), new Vector2(0, 180), Color.White);

            // bottom right powerups
            for (int i = 0; i < PlayerShip.Instance.activePowerPacks.Count; i++) {
                Vector2 pos = new Vector2(GameRoot.ScreenSize.X - 100 - i * 100, GameRoot.ScreenSize.Y - 100);
                var power = PlayerShip.Instance.activePowerPacks[i];
                Texture2D icon;
                switch (power.PowerType) {
                    case ("ShootSpeedIncrease"):
                        icon = Art.PowerShootSpeedIncrease;
                        break;
                    case ("ShootSpeedDecrease"):
                        icon = Art.PowerShootSpeedDecrease;
                        break;
                    case ("MoveSpeedIncrease"):
                        icon = Art.PowerMoveSpeedIncrease;
                        break;
                    case ("MoveSpeedDecrease"):
                        icon = Art.PowerMoveSpeedDecrease;
                        break;
                    default:
                        icon = Art.Default;
                        break;
                }
                // Draw time remaining coloured background
                Color backgroundColor = new Color(60, 214, 91);
                if (!power.isGood)
                    backgroundColor = new Color(214, 60, 60);
                float remaining = (float)power.framesRemaining / (float)power.initialFramesRemaining;
                spriteBatch.Draw(Art.Pixel, new Rectangle((int)pos.X, (int)(pos.Y + 96 * (1 - remaining)), 96, (int)(96 - 96 * (1 - remaining))), backgroundColor);
                // Draw icon
                spriteBatch.Draw(icon, pos, Color.White);
            }
        }
    }
}