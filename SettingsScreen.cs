using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Mono_Ether {
    public class SettingsScreen : GameState
    {
        private readonly ButtonManager buttonManager = new ButtonManager();
        private readonly SliderManager sliderManager = new SliderManager();
        private Song _music;
        public SettingsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {
            GameSettings.LoadSettings();
            buttonManager.Buttons.Add(new Button(new Vector2(200f, GameSettings.ScreenSize.Y - 100f), new Vector2(200, 120), "Back"));
            sliderManager.Sliders.Add(new Slider(new Vector2(400f, 200f), 400f, SliderType.MASTER, GameSettings.MasterVolume));
            sliderManager.Sliders.Add(new Slider(new Vector2(400f, 400f), 400f, SliderType.SFX, GameSettings.SoundEffectVolume));
            sliderManager.Sliders.Add(new Slider(new Vector2(400f, 600f), 400f, SliderType.MUSIC, GameSettings.MusicVolume));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);
        }
        public override void Pause() {
        }
        public override void Resume() {
        }

        public override void LoadContent(ContentManager content)
        {
            Slider._sliderBall = content.Load<Texture2D>("Textures/SettingsScreen/SliderBall");
            _music = content.Load<Song>("Songs/Settings");
        }

        public override void UnloadContent() {
            MediaPlayer.Stop();
            Slider._sliderBall = null;
            _music = null;
        }

        public override void Update(GameTime gameTime) {
            buttonManager.Update();
            sliderManager.Update();
            if (buttonManager.PressedButton == "Back")
            {
                GameSettings.SaveSettings();
                ScreenManager.RemoveScreen();
            }

            foreach (Slider slider in sliderManager.Sliders)
            {
                if (slider.IsBeingDragged)
                {
                    switch (slider.Type) {
                        case SliderType.MASTER:
                            GameSettings.MasterVolume = slider.Value;
                            break;
                        case SliderType.SFX:
                            GameSettings.SoundEffectVolume = slider.Value;
                            break;
                        case SliderType.MUSIC:
                            GameSettings.MusicVolume = slider.Value;
                            break;
                    }
                    GameSettings.ApplyChanges();
                }
            }
        }
        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black);
            sliderManager.Draw(batch);
            buttonManager.Draw(batch);
        }
    }
    public class Slider
    {
        public static Texture2D _sliderBall;
        private int _clickSfxDelay;
        public Vector2 SliderPos; // Centre of slider
        public float Width;
        public SliderType Type;
        public float Value;
        public bool IsHovered = false;
        public bool IsBeingDragged = false;
        private Vector2 SliderBallPos => new Vector2(SliderPos.X + (Value - 0.5f) * Width, SliderPos.Y);
        private const float Radius = 20f;
        public Slider(Vector2 sliderPos, float width, SliderType type, float value = 0.5f)
        {
            SliderPos = sliderPos;
            Width = width;
            Type = type;
            Value = value;
        }

        public void Update()
        {
            IsHovered = Vector2.DistanceSquared(Input.Mouse.Position.ToVector2(), SliderBallPos) < Radius * Radius;
            if (!IsBeingDragged && IsHovered && Input.WasLeftButtonJustDown)
                IsBeingDragged = true;
            if (IsBeingDragged)
            {
                if (Input.WasLeftButtonJustUp)
                    IsBeingDragged = false;
                Value = (Input.Mouse.X - Width / 2f) / Width;
                Value = Math.Clamp(Value, 0f, 1f);
                _clickSfxDelay++;
                if (_clickSfxDelay >= 6) {
                    _clickSfxDelay = 0;
                    GlobalAssets.Click.Play(GameSettings.SoundEffectVolume, 1f, 0); // TODO replace Click with PlayerShoot
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(GlobalAssets.Pixel, SliderPos, null, Color.White, 0f, new Vector2(0.5f), new Vector2(Width, 10f), 0, 0);  // Bar
            batch.Draw(_sliderBall, SliderBallPos, null, IsHovered ? Color.LightCyan : Color.White, 0f, _sliderBall.Size() / 2f, 1f, 0, 0); // Ball
            batch.DrawStringCentered(GlobalAssets.NovaSquare24, MyUtils.SliderTypeToName(Type), SliderPos + new Vector2(0f, -50f), Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"{Value:0.00}", SliderPos + new Vector2(Width / 2f + 50f, -GlobalAssets.NovaSquare24.MeasureString("a").Y / 2f),
                Color.White);
        }
    }
    public class SliderManager
    {
        public List<Slider> Sliders = new List<Slider>();
        public void Update() {
            foreach (Slider slider in Sliders)
                slider.Update();
        }
        public void Draw(SpriteBatch batch) {
            foreach (Slider slider in Sliders)
                slider.Draw(batch);
        }
    }
}
