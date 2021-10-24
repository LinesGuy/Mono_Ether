using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether {
    public class Hud {
        public bool playingYouDied;
        private int deadFrames;
        public Hud() {
            Reset();
        }
        public void Reset() {
            playingYouDied = false;
            deadFrames = 0;
        }
        public void Draw(SpriteBatch spriteBatch) {
            // Top-left debug texts
            if (GameRoot.Instance.dum_mode) {
                spriteBatch.DrawString(Fonts.NovaSquare24, $"Player1 XY: {EntityManager.Player1.Position.X:0.0}, {EntityManager.Player1.Position.Y:0.0}", new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(Fonts.NovaSquare24, $"Cursor XY: {Camera.MouseWorldCoords().X:0.0}, {Camera.MouseWorldCoords().Y:0.0}", new Vector2(0, 30), Color.White);
                spriteBatch.DrawString(Fonts.NovaSquare24, $"Tile ID: {Map.GetTileFromMap(Map.WorldtoMap(Camera.MouseWorldCoords())).TileId}", new Vector2(0, 60), Color.White);
                spriteBatch.DrawString(Fonts.NovaSquare24, $"Tile XY: {Map.GetTileFromMap(Map.WorldtoMap(Camera.MouseWorldCoords())).pos}", new Vector2(0, 90), Color.White);
            }
            // Bottom-right powerups
            // ONLY APPLIES TO player1
            for (int i = 0; i < EntityManager.Player1.activePowerPacks.Count; i++) {
                Vector2 pos = new Vector2(GameRoot.ScreenSize.X - 100 - i * 100, GameRoot.ScreenSize.Y - 100);
                var power = EntityManager.Player1.activePowerPacks[i];
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
            // ONLY APPLIES to player1
            for (int i = 0; i < EntityManager.Player1.lives; i++) {
                Vector2 pos = new Vector2(0 + i * 100, GameRoot.ScreenSize.Y - 100);
                spriteBatch.Draw(Art.Heart, pos, Color.White);
            }
            // Top-right score
            // ONLY APPLIES to player1
            spriteBatch.DrawString(Fonts.NovaSquare24, $"Player 1 score: {EntityManager.Player1.Score}", new Vector2(GameRoot.ScreenSize.X * 0.75f, 0), Color.White);
            // Top-right multiplier
            // ONLY APPLIES to player1
            spriteBatch.DrawString(Fonts.NovaSquare24, $"[x{EntityManager.Player1.Multiplier}]", new Vector2(GameRoot.ScreenSize.X * 0.75f, 30), Color.White);
            // Top-right highscore
            // ONLY APPLIES to player1
            spriteBatch.DrawString(Fonts.NovaSquare24, $"Highscore: {Math.Max(EntityManager.Player1.Score, EntityManager.Player1.HighScore)}", new Vector2(GameRoot.ScreenSize.X * 0.75f, 60), Color.White);
            // Top-right geoms
            // ONLY APPLIES to player1
            spriteBatch.DrawString(Fonts.NovaSquare24, $"Player 1 geoms: {EntityManager.Player1.Geoms}", new Vector2(GameRoot.ScreenSize.X * 0.75f, 90), Color.White);
            // You died
            if (playingYouDied) {
                int t = Math.Min(255, (int)(deadFrames / 60f * 255f));    
                spriteBatch.Draw(Art.YouDied, GameRoot.ScreenSize / 2f, null, new Color(255, 255, 255, t), 0f, Art.YouDied.Size() / 2f, MathHelper.Lerp(1f, 1.5f, deadFrames / 120f), SpriteEffects.None, 0);
                
            }
        }
        public void Update() {
            if (playingYouDied) {
                deadFrames++;
                //Debug.WriteLine(deadFrames);
                if (deadFrames == 120)
                    GameRoot.Instance.RemoveScreenTransition();
            }
        }
    }
}