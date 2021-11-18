using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Mono_Ether.MainMenu {
    public class CreditsScreen : States.GameState {
        private ButtonManager buttonManager;
        private List<Letter> Letters = new List<Letter>();
        public static readonly Random rand = new Random();
        private int Frame;
        private char[] credits1 = "everything by me + lamkas a cute".ToCharArray();
        public CreditsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }
        public override void Initialize() {
            buttonManager = new ButtonManager();
            buttonManager.Add("back");
            Frame = 0;
        }
        public override void LoadContent(ContentManager content) { }
        public override void UnloadContent() { }
        public override void Update(GameTime gameTime) {
            var clickedButton = buttonManager.GetClickedButton();
            if (clickedButton == "back")
                GameRoot.Instance.RemoveScreenTransition();
            if (Input.WasKeyJustDown(Keys.R)) {
                Frame = 0;
                Letters.Clear();
            }
            if (0 <= Frame && Frame <= (credits1.Length - 1) * 10) {
                if (Frame % 10 == 0)
                    Letters.Add(new Letter(GameRoot.ScreenSize.X, GameRoot.ScreenSize.Y / 2f, credits1[Frame / 10]) { Velocity = new Vector2(MathUtil.Interpolate(-25, -5, Frame / 260f), rand.NextFloat(-5, 5)), Orientation = rand.NextFloat(0, MathF.PI * 2f) });
            }
            for (int i = 0; i < Letters.Count; i++) {
                Letter letter = Letters[i];
                letter.age++;
                if (letter.age <= 120) {
                    letter.Velocity *= 0.98f;
                    letter.Orientation += 0.01f;
                }
                else {
                    Vector2 destination = new Vector2(MathUtil.Interpolate(100, 1150, (float)i / Letters.Count), GameRoot.ScreenSize.Y / 2f + MathF.Sin(i / 5f + Frame / 30f) * 150f);
                    letter.Velocity = (destination - letter.Pos) / 20f;
                    if (letter.Orientation >= 0)
                        letter.Orientation -= 0.05f;
                }
                letter.Pos += letter.Velocity;
            }
            Frame++;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            foreach (Letter letter in Letters) {
                letter.Draw(spriteBatch);
            }

            buttonManager.Draw(spriteBatch);
            spriteBatch.End();
        }
        private class Letter {
            public Vector2 Pos;
            public Vector2 Velocity = Vector2.Zero;
            public string Chr;
            public Color Color = Color.White;
            public float Scale = 1f;
            public float Orientation = 0f;
            public int age = 0;
            public Letter(Vector2 pos, char chr) {
                Pos = pos;
                Chr = chr.ToString();
            }
            public Letter(float x, float y, char chr) {
                Pos = new Vector2(x, y);
                Chr = chr.ToString();
            }
            public void Draw(SpriteBatch spriteBatch) {
                spriteBatch.DrawString(Fonts.NovaSquare48, Chr, Pos, Color, Orientation, Fonts.NovaSquare48.MeasureString(Chr) / 2f, Scale, 0, 0);
            }
        }
    }
}
