using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether.Ether {
    /*
    class FloatingText {
        readonly string Text;
        Vector2 Velocity; // not be used for all types
        Vector2 StartPos;
        Vector2 Pos; // MIDDLE of text
        int Age;
        readonly int Lifespan;
        public bool IsExpired;
        public float Scale;
        public string Type;
        Color Color;
        private static readonly Random rand = new Random();
        public FloatingText(string text, Vector2 pos, Color color, string type) {
            Text = text;
            Pos = pos;
            StartPos = Pos;
            Velocity = Vector2.Zero;
            Color = color;
            Type = type;
            switch (Type) {
                case "bounce":
                    Velocity.X = rand.NextFloat(-3f, 3f);
                    Velocity.Y = rand.NextFloat(0.5f, 1.5f);
                    break;
                default:
                    break;
            }
            IsExpired = false;
            Scale = 1f;
            Age = 0;
            Lifespan = 118;
        }
        public void Update() {
            switch (Type) {
                case "rise":
                    Pos.Y -= 1;
                    break;
                case "bounce":
                    Pos = Camera.WorldToScreen(StartPos);
                    Pos.X += Velocity.X * Age * Camera.Zoom;
                    Pos.Y -= Velocity.Y * MathF.Abs(MathF.Sin(0.08f * Age) * (144f - Age * 0.8f)) * Camera.Zoom;
                    break;
            }
            Age += 1;
            if (Age > Lifespan)
                IsExpired = true;
        }
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Fonts.NovaSquare24, Text, Pos, Color, 0f, Fonts.NovaSquare24.MeasureString(Text) / 2f, Scale * Camera.Zoom, SpriteEffects.None, 0);
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
        public static void Add(string text, Vector2 pos, Color color, String type) {
            texts.Add(new FloatingText(text, pos, color, type));
        }
        public static void Clear() {
            texts.Clear();
        }
    }
    */
}
