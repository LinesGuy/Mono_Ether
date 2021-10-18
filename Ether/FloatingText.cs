using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether.Ether {
    class FloatingText {
        string Text;
        Vector2 Pos; // MIDDLE of text
        int Age;
        int Lifespan;
        public bool IsExpired;
        public float Scale;
        Color Color;
        public FloatingText(string text, Vector2 pos, Color color) {
            Text = text;
            Pos = pos;
            Color = color;
            IsExpired = false;
            Scale = 1f;
            Age = 0;
            Lifespan = 180;
        }
        public void Update() {
            Pos.Y -= 1;
            Age += 1;
            if (Age > Lifespan)
                IsExpired = true;
        }
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Art.DebugFont, Text, Camera.WorldToScreen(Pos), Color, 0f, Art.DebugFont.MeasureString(Text) / 2f, Scale, SpriteEffects.None, 0);
        }
    }
    public static class FloatingTextManager {
        static List<FloatingText> texts = new List<FloatingText>();
        public static void Update() {
            foreach (FloatingText text in texts)
                text.Update();

            texts = texts.Where(t => !t.IsExpired).ToList();
        }
        public static void Draw(SpriteBatch spriteBatch) {
            foreach (FloatingText text in texts)
                text.Draw(spriteBatch);
        }
        public static void Add(string text, Vector2 pos, Color color) {
            texts.Add(new FloatingText(text, pos, color));
        }
        public static void Clear() {
            texts.Clear();
        }
    }
}
