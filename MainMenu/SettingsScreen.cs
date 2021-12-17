using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Mono_Ether.MainMenu {
    /*
    public class SettingsScreen : GameState {
        private ButtonManager buttonManager;
        private readonly Dictionary<string, Slider> Sliders = new Dictionary<string, Slider>();
        public SettingsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            GameSettings.LoadSettings();
            buttonManager = new ButtonManager();
            buttonManager.Add("back");
            Sliders.Add("Master Volume", new Slider(new Vector2(400, 200), "Master Volume", 400f, GameSettings.MasterVolume));
            Sliders.Add("SFX Volume", new Slider(new Vector2(400, 400), "SFX Volume", 400f, GameSettings.SoundEffectVolume));
            Sliders.Add("Music Volume", new Slider(new Vector2(400, 600), "Music Volume", 400f, GameSettings.MusicVolume));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Sounds.Music);
        }
        public override void LoadContent(ContentManager content) {
            //throw new NotImplementedException();
        }
        public override void UnloadContent() {
            MediaPlayer.Stop();
        }
        public override void Update(GameTime gameTime) {
            var clickedButton = buttonManager.GetClickedButton();
            if (clickedButton == "back") {
                GameSettings.SaveSettings();
                ScreenManager.RemoveScreenTransition();
            }
            foreach (Slider slider in Sliders.Values) {
                slider.Update();
                if (slider.IsBeingDragged) {
                    switch (slider.Text) {
                        case "Master Volume":
                            GameSettings.MasterVolume = slider.Value;
                            break;
                        case "SFX Volume":
                            GameSettings.SoundEffectVolume = slider.Value;
                            break;
                        case "Music Volume":
                            GameSettings.MusicVolume = slider.Value;
                            break;
                    }
                    GameSettings.ApplyChanges();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            foreach (Slider slider in Sliders.Values)
                slider.Draw(spriteBatch);
            spriteBatch.DrawString(Fonts.NovaSquare24, "Settings", Vector2.Zero, Color.White);
            buttonManager.Draw(spriteBatch);
        }
    }
    class Slider {
        public Vector2 SliderPos;
        public Vector2 BallPos;
        public string Text;
        private readonly float Width; // pixels
        protected Texture2D Texture;
        private float framesSincePlay;
        public float Value;
        public bool IsBeingHovered;
        public bool IsBeingDragged;
        public Slider(Vector2 sliderPos, string text, float width, float startValue) {
            SliderPos = sliderPos;
            Text = text;
            Width = width;
            Texture = GlobalAssets.Default;
            framesSincePlay = 0;
            Value = startValue;
            BallPos = new Vector2(SliderPos.X + (Value - 0.5f) * Width, SliderPos.Y);
            IsBeingHovered = false;
            IsBeingDragged = false;
        }

        public void Update() {
            IsBeingHovered = Vector2.DistanceSquared(Input.Mouse.Position.ToVector2(), BallPos) < Math.Pow(GlobalAssets.SettingsSliderBall.Width / 2f, 2);
            if (IsBeingHovered && Input.WasLeftButtonJustDown())
                IsBeingDragged = true;
            else if (Input.WasLeftButtonJustUp())
                IsBeingDragged = false;
            if (IsBeingDragged) {
                Value = (Input.Mouse.X - Width / 2f) / Width;
                Value = Math.Clamp(Value, 0f, 1f);
                BallPos = new Vector2(SliderPos.X + (Value - 0.5f) * Width, SliderPos.Y);
                // sfx
                if (Text == "SFX Volume") {
                    framesSincePlay++;
                    if (framesSincePlay >= 6) {
                        framesSincePlay = 0;
                        Sounds.PlayerShoot.Play(GameSettings.SoundEffectVolume, 0.1f, 0);
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(GlobalAssets.SettingsSliderMiddle, new Rectangle((int)(SliderPos.X - Width / 2f), (int)(SliderPos.Y - GlobalAssets.SettingsSliderMiddle.Height / 2f), (int)Width, (int)GlobalAssets.SettingsSliderMiddle.Height), Color.White);
            spriteBatch.Draw(GlobalAssets.SettingsSliderLeft, new Vector2(SliderPos.X - Width / 2f - GlobalAssets.SettingsSliderLeft.Width / 2f, SliderPos.Y - GlobalAssets.SettingsSliderLeft.Height / 2f), Color.White);
            spriteBatch.Draw(GlobalAssets.SettingsSliderRight, new Vector2(SliderPos.X + Width / 2f - GlobalAssets.SettingsSliderLeft.Width / 2f, SliderPos.Y - GlobalAssets.SettingsSliderRight.Height / 2f), Color.White);
            spriteBatch.DrawString(Fonts.NovaSquare48, Text, new Vector2(SliderPos.X, SliderPos.Y - 75f), Color.White, 0f, Fonts.NovaSquare48.MeasureString(Text) / 2f, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Fonts.NovaSquare24, $"{Value * 100:0}%", new Vector2(SliderPos.X + Width / 2f + GlobalAssets.SettingsSliderRight.Width + 35f, SliderPos.Y), Color.White, 0f, Fonts.NovaSquare24.MeasureString($"{Value * 100:0}%") / 2f, 1f, SpriteEffects.None, 0);
            Color sliderBallColor;
            if (IsBeingHovered || IsBeingDragged)
                sliderBallColor = new Color(0, 255, 0);
            else
                sliderBallColor = new Color(0, 128, 0);
            spriteBatch.Draw(GlobalAssets.SettingsSliderBall, BallPos, null, sliderBallColor, 0, GlobalAssets.SettingsSliderBall.Size() / 2f, 1f, SpriteEffects.None, 0);
        }
    }
    */
}
