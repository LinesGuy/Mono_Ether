using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mono_Ether.GameSettings;

namespace Mono_Ether {
    public class TitleScreen : GameState {
        private Texture2D _smallStar;
        private Texture2D _bigStar;
        private Texture2D _bg;
        private Texture2D _mainBar;
        private Texture2D _subBar;
        private Texture2D _byChris;
        private Texture2D _etherText;
        private Texture2D _etherOutline;
        private Texture2D _mono;
        private Texture2D _triangle;
        private Texture2D _treeBg;
        private Texture2D _treeLeaf;
        private Song _titleMusic;

        private readonly List<Vector2> _smallStars = new List<Vector2>();
        private readonly List<Vector2> _bigStars = new List<Vector2>();
        private readonly Random _rand = new Random();
        private readonly ButtonManager _titleButtonManager = new ButtonManager();
        private readonly ButtonManager _levelButtonManager = new ButtonManager();
        private readonly ButtonManager _carouselButtonManager = new ButtonManager();
        private readonly ButtonManager _settingsButtonManager = new ButtonManager();
        private readonly SliderManager _settingsSliderManager = new SliderManager();
        private float _carouselOffset;
        private float _carouselOffsetVelocity;
        private int _framesSinceStart;
        private int _framesSinceTransition;
        private const int TransitionFrames = 30;
        private string _state;
        public TitleScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }
        public override void Initialize() {
            _state = "Title press any key";
            /* Add Title screen buttons */
            _titleButtonManager.Buttons.Add(new Button(new Vector2(ScreenSize.X / 2f - 500f, ScreenSize.Y - 100f), new Vector2(300, 120), "Start"));
            _titleButtonManager.Buttons.Add(new Button(new Vector2(ScreenSize.X / 2f - 162.5f, ScreenSize.Y - 100f), new Vector2(300, 120), "Settings"));
            _titleButtonManager.Buttons.Add(new Button(new Vector2(ScreenSize.X / 2f + 162.5f, ScreenSize.Y - 100f), new Vector2(300, 120), "Credits"));
            _titleButtonManager.Buttons.Add(new Button(new Vector2(ScreenSize.X / 2f + 500f, ScreenSize.Y - 100f), new Vector2(300, 120), "Exit"));
            /* Add Level selection screen button */
            _levelButtonManager.Buttons.Add(new Button(new Vector2(200f, ScreenSize.Y - 100f), new Vector2(200, 120), "Back"));
            /* Add carousel buttons */
            List<string> levels = new List<string> { "Level One", "Level Two", "Level Three", "Level Four", "Level Five", "Level Six", "Level Seven ", "Level Eight", "Level Nine", "Level Ten" };
            for (int i = 0; i < levels.Count; i++)
                _carouselButtonManager.Buttons.Add(new Button(new Vector2(ScreenSize.X - 300f * MathF.Exp(-i * i / 25f), ScreenSize.Y / 2f + i * 120f), new Vector2(500, 120), levels[i]));
            /* Add settings window buttons and sliders */
            _settingsButtonManager.Buttons.Add(new Button(ScreenSize / 2f + new Vector2(0f, 250f), new Vector2(200, 120), "Back"));
            _settingsSliderManager.Sliders.Add(new Slider(ScreenSize / 2f + new Vector2(-275f, -200f), 400f, SliderType.Master, MasterVolume));
            _settingsSliderManager.Sliders.Add(new Slider(ScreenSize / 2f + new Vector2(-275f, -50f), 400f, SliderType.Sfx, SoundEffectVolume));
            _settingsSliderManager.Sliders.Add(new Slider(ScreenSize / 2f + new Vector2(-275f, 100f), 400f, SliderType.Music, MusicVolume));
            /* Create lists of small/big stars with random positions */
            for (var i = 0; i < 150; i++)
                _smallStars.Add(new Vector2(_rand.Next(0, (int)ScreenSize.X), _rand.Next(0, (int)ScreenSize.Y)));
            for (var i = 0; i < 5; i++)
                _bigStars.Add(new Vector2(_rand.Next(0, (int)ScreenSize.X), _rand.Next(0, (int)ScreenSize.Y)));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_titleMusic);
        }
        public override void Suspend() { }
        public override void Resume() {
            MediaPlayer.Play(_titleMusic);
        }
        public override void LoadContent(ContentManager content) {
            _smallStar = content.Load<Texture2D>("Textures/TitleScreen/SmallStar");
            _bigStar = content.Load<Texture2D>("Textures/TitleScreen/BigStar");
            _bg = content.Load<Texture2D>("Textures/TitleScreen/Bg");
            _mainBar = content.Load<Texture2D>("Textures/TitleScreen/MainBar");
            _subBar = content.Load<Texture2D>("Textures/TitleScreen/SubBar");
            _byChris = content.Load<Texture2D>("Textures/TitleScreen/ByChris");
            _etherText = content.Load<Texture2D>("Textures/TitleScreen/EtherText");
            _etherOutline = content.Load<Texture2D>("Textures/TitleScreen/EtherOutline");
            _mono = content.Load<Texture2D>("Textures/TitleScreen/Mono");
            _triangle = content.Load<Texture2D>("Textures/TitleScreen/Triangle");
            _treeBg = content.Load<Texture2D>("Textures/TitleScreen/TreeBg");
            _treeLeaf = content.Load<Texture2D>("Textures/TitleScreen/TreeLeaf");
            _titleMusic = content.Load<Song>("Songs/TitleScreen");
        }

        public override void UnloadContent() {
            _smallStar = null;
            _bigStar = null;
            _bg = null;
            _mainBar = null;
            _subBar = null;
            _byChris = null;
            _etherText = null;
            _etherOutline = null;
            _mono = null;
            _triangle = null;
            _treeBg = null;
            _treeLeaf = null;
            _titleMusic = null;
        }

        public override void Update(GameTime gameTime) {
            /* Reset scene if user pressed R */
            if (Input.WasKeyJustUp(Keys.R))
                Initialize();
            _framesSinceStart++;
            _framesSinceTransition++;
            switch (_state) {
                case "Title press any key":
                    if (Input.Keyboard.GetPressedKeyCount() > 0 || Input.WasLeftButtonJustDown) {
                        SetState("Title press any key -> Title");
                        GlobalAssets.Click.Play(SoundEffectVolume, 0f, 0f);
                    }
                    break;
                case "Title press any key -> Title":
                    if (_framesSinceTransition > TransitionFrames) SetState("Title");
                    HandleTitleButtons();
                    break;
                case "Title":
                    if (Input.WasRightButtonJustDown) {
                        SetState("Title press any key");
                    }
                    HandleTitleButtons();
                    break;
                case "Title -> Level selection":
                    if (_framesSinceTransition > TransitionFrames) SetState("Level selection");
                    HandleLevelButtons();
                    break;
                case "Title -> Settings":
                    if (_framesSinceTransition > TransitionFrames) SetState("Settings");
                    HandleSettingsWindow();
                    break;
                case "Settings":
                    HandleSettingsWindow();
                    break;
                case "Settings -> Title":
                    if (_framesSinceTransition > TransitionFrames) SetState("Title");
                    HandleTitleButtons();
                    break;
                case "Level selection":
                    HandleLevelButtons();
                    break;
                case "Level selection -> Title":
                    if (_framesSinceTransition > TransitionFrames) SetState("Title");
                    HandleTitleButtons();
                    break;
                case "Level selection -> level":
                    break;
            }
        }
        public override void Draw(SpriteBatch batch) {
            switch (_state) {
                case "Title press any key":
                    DrawBg(batch);
                    DrawLogo(batch, new Vector2(0, 20f));
                    /* Press any key to start */
                    if (_framesSinceTransition % 60 < 40)
                        batch.DrawStringCentered(GlobalAssets.NovaSquare48, "PRESS ANY KEY", new Vector2(ScreenSize.X / 2f, ScreenSize.Y - 100f), Color.White);
                    /* Screen intro flash */
                    if (_framesSinceStart <= 60)
                        batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(0, 0, ScreenSize.X, ScreenSize.Y), GetTransparentColor((60 - _framesSinceStart) / 60f));
                    break;
                case "Title press any key -> Title":
                    DrawBg(batch);
                    DrawLogo(batch, MyUtils.EInterpolate(Vector2.Zero, new Vector2(0f, -20f), _framesSinceTransition));
                    _titleButtonManager.Draw(batch, MyUtils.EInterpolate(new Vector2(0f, 200f), Vector2.Zero, _framesSinceTransition));
                    break;
                case "Title":
                    DrawBg(batch);
                    DrawLogo(batch, new Vector2(0, -20f));
                    _titleButtonManager.Draw(batch);
                    break;
                case "Title -> Level selection":
                    DrawBg(batch);
                    DrawLogo(batch, MyUtils.EInterpolate(new Vector2(0f, -20f), new Vector2(0f, -ScreenSize.Y), _framesSinceTransition));
                    _titleButtonManager.Draw(batch, MyUtils.EInterpolate(Vector2.Zero, new Vector2(0f, 200f), _framesSinceTransition));
                    _levelButtonManager.Draw(batch, MyUtils.EInterpolate(new Vector2(-400f, 0f), Vector2.Zero, _framesSinceTransition));
                    _carouselButtonManager.Draw(batch, MyUtils.EInterpolate(new Vector2(600f, 0f), Vector2.Zero, _framesSinceTransition));
                    break;
                case "Title -> Settings":
                    DrawBg(batch);
                    DrawLogo(batch, new Vector2(0, -20f));
                    _titleButtonManager.Draw(batch);
                    batch.Draw(GlobalAssets.Pixel, MyUtils.EInterpolate(new Vector2(ScreenSize.X / 2f, ScreenSize.Y * 1.5f), ScreenSize / 2f, _framesSinceTransition), null, new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    _settingsSliderManager.Draw(batch, MyUtils.EInterpolate(new Vector2(0f, ScreenSize.Y), Vector2.Zero, _framesSinceTransition));
                    _settingsButtonManager.Draw(batch, MyUtils.EInterpolate(new Vector2(0f, ScreenSize.Y), Vector2.Zero, _framesSinceTransition));
                    break;
                case "Settings -> Title":
                    DrawBg(batch);
                    DrawLogo(batch, new Vector2(0, -20f));
                    _titleButtonManager.Draw(batch);
                    batch.Draw(GlobalAssets.Pixel, MyUtils.EInterpolate(ScreenSize / 2f, new Vector2(ScreenSize.X / 2f, ScreenSize.Y * 1.5f), _framesSinceTransition), null, new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    _settingsSliderManager.Draw(batch, MyUtils.EInterpolate(Vector2.Zero, new Vector2(0f, ScreenSize.Y), _framesSinceTransition));
                    _settingsButtonManager.Draw(batch, MyUtils.EInterpolate(Vector2.Zero, new Vector2(0f, ScreenSize.Y), _framesSinceTransition));
                    break;
                case "Settings":
                    DrawBg(batch);
                    DrawLogo(batch, new Vector2(0, -20f));
                    _titleButtonManager.Draw(batch);
                    batch.Draw(GlobalAssets.Pixel, ScreenSize / 2f, null, new Color(0.25f, 0.25f, 0.25f, 0.95f), 0f, new Vector2(0.5f), new Vector2(1190f, 670f), 0, 0);
                    _settingsSliderManager.Draw(batch);
                    _settingsButtonManager.Draw(batch);
                    break;
                case "Level selection":
                    DrawBg(batch);
                    _levelButtonManager.Draw(batch, Vector2.Zero);
                    _carouselButtonManager.Draw(batch, Vector2.Zero);
                    break;
                case "Level selection -> Title":
                    DrawBg(batch);
                    DrawLogo(batch, MyUtils.EInterpolate(new Vector2(0f, -ScreenSize.Y), new Vector2(0f, -20f), _framesSinceTransition));
                    _titleButtonManager.Draw(batch, MyUtils.EInterpolate(new Vector2(0f, 200f), Vector2.Zero, _framesSinceTransition));
                    _levelButtonManager.Draw(batch, MyUtils.EInterpolate(Vector2.Zero, new Vector2(-400f, 0f), _framesSinceTransition));
                    _carouselButtonManager.Draw(batch, MyUtils.EInterpolate(Vector2.Zero, new Vector2(600f, 0f), _framesSinceTransition));
                    break;
                case "Level selection -> level":
                    DrawBg(batch);
                    break;
            }
        }
        private void SetState(string state) {
            _state = state;
            _framesSinceTransition = 0;
        }
        private void HandleTitleButtons() {
            _titleButtonManager.Buttons.ForEach(b => b.Update());
            switch (_titleButtonManager.PressedButton) {
                case "Start":
                    _state = "Title -> Level selection";
                    _framesSinceTransition = 0;
                    GlobalAssets.Click.Play(SoundEffectVolume, 0f, 0f);
                    break;
                case "Settings":
                    _state = "Title -> Settings";
                    _framesSinceTransition = 0;
                    GlobalAssets.Click.Play(SoundEffectVolume, 0f, 0f);
                    LoadSettings();
                    //ScreenManager.AddScreen(new SettingsScreen(GraphicsDevice));
                    break;
                case "Exit":
                    ScreenManager.RemoveScreen();
                    break;
            }
        }
        private void HandleSettingsWindow() {
            _settingsButtonManager.Buttons.ForEach(b => b.Update());
            if (_settingsButtonManager.PressedButton == "Back") {
                SaveSettings();
                SetState("Settings -> Title");
            }
            foreach (var slider in _settingsSliderManager.Sliders)
                slider.Update();
            foreach (var slider in _settingsSliderManager.Sliders.Where(slider => slider.IsBeingDragged)) {
                switch (slider.Type) {
                    case SliderType.Master:
                        MasterVolume = slider.Value;
                        break;
                    case SliderType.Sfx:
                        SoundEffectVolume = slider.Value;
                        break;
                    case SliderType.Music:
                        MusicVolume = slider.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                ApplyChanges();
            }
        }

        private void HandleLevelButtons()
        {
            /* Handle level button presses */
            _levelButtonManager.Buttons.ForEach(b => b.Update());
            switch (_levelButtonManager.PressedButton)
            {
                case "Back":
                    _state = "Level selection -> Title";
                    _framesSinceTransition = 0;
                    GlobalAssets.Click.Play(SoundEffectVolume, 0f, 0f);
                    break;
            }

            /* Handle Carousel Button Presses */
            _carouselButtonManager.Buttons.ForEach(b => b.Update());
            switch (_carouselButtonManager.PressedButton)
            {
                case "Level One":
                    GlobalAssets.Click.Play(SoundEffectVolume, 0f, 0f);

                    break;
                // TODO other levels
            }

            /* Update carousel offset */
            var numButtons = _carouselButtonManager.Buttons.Count;
            _carouselOffsetVelocity -= Input.DeltaScrollWheelValue / 1300f; // Scroll wheel
            if (Input.WasKeyJustDown(Keys.Up)) _carouselOffsetVelocity += 0.1f;
            if (Input.WasKeyJustDown(Keys.Down)) _carouselOffsetVelocity -= 0.1f;
            _carouselOffsetVelocity *= 0.9f;
            _carouselOffset += _carouselOffsetVelocity;
            /* Ensure buttons don't go completely off-screen */
            if (_carouselOffset > -1.5f)
                _carouselOffset = MathHelper.Lerp(_carouselOffset, -1.5f, 0.2f);
            if (_carouselOffset < 1.5f - numButtons)
                _carouselOffset = MathHelper.Lerp(_carouselOffset, 1.5f - numButtons, 0.2f);
            /* Update button positions */
            for (int i = 0; i < numButtons; i++)
            {
                var j = i + _carouselOffset;
                _carouselButtonManager.Buttons[i].Pos = new Vector2(
                    ScreenSize.X - 300f * MathF.Exp(-j * j / 25f),
                    ScreenSize.Y / 2f + j * 120f);
            }
        }
        private void DrawBg(SpriteBatch batch) {
            Vector2 offset = new Vector2(-Input.Mouse.X / ScreenSize.X - 0.5f,
                -Input.Mouse.Y / ScreenSize.Y - 0.5f) * 10f;
            /* Bg */
            batch.Draw(_bg, MyUtils.RectangleF(0, 0, ScreenSize.X, ScreenSize.Y), Color.White);
            /* SmallStars */
            foreach (var starPos in _smallStars)
                batch.Draw(_smallStar, starPos + offset, null, GetTransparentColor(0.5f + MathF.Sin(starPos.X + _framesSinceStart / 60f) * 0.5f), 0f, _smallStar.Size() / 2f, 1f + MathF.Sin(starPos.X) / 3f, 0, 0);
            /* BigStars */
            foreach (var starPos in _bigStars)
                batch.Draw(_bigStar, starPos + offset, null, GetTransparentColor(0.5f + MathF.Sin(starPos.X + _framesSinceStart / 60f) * 0.5f), starPos.X + _framesSinceStart / 60f, _bigStar.Size() / 2f, 0.5f + MathF.Sin(starPos.X) / 3f, 0, 0);
            /* Trees */
            batch.Draw(_treeBg, new Vector2(ScreenSize.X / 2f - 550f, ScreenSize.Y / 2f) + offset, null, Color.White, 0f, _treeBg.Size() / 2f, 1f, 0, 0);
            batch.Draw(_treeBg, new Vector2(ScreenSize.X / 2f + 550f, ScreenSize.Y / 2f) + offset, null, Color.White, 0f, _treeBg.Size() / 2f, 1f, SpriteEffects.FlipHorizontally, 0);
            /* Tree Leaves */
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 484f, ScreenSize.Y / 2f - 120f) + offset, null, Color.White, 0.1f + MathF.Sin(_framesSinceStart / 60f) / 3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 484f, ScreenSize.Y / 2f - 115f) + offset, null, Color.White, -0.4f + MathF.Sin(_framesSinceStart / 50f + 1) / 2.9f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 484f, ScreenSize.Y / 2f - 110f) + offset, null, Color.White, 0.3f + MathF.Sin(_framesSinceStart / 70f) / 2.5f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 484f, ScreenSize.Y / 2f - 120f) + offset, null, Color.White, 3.5f + MathF.Sin(_framesSinceStart / 65f + 1) / 3.5f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 484f, ScreenSize.Y / 2f - 115f) + offset, null, Color.White, 2.9f + MathF.Sin(_framesSinceStart / 55f) / 4f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 680f, ScreenSize.Y / 2f - 20f) + offset, null, Color.White, 0.1f + MathF.Sin(_framesSinceStart / 75f + 1) / 2.7f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f - 680f, ScreenSize.Y / 2f - 20f) + offset, null, Color.White, 3.1f + MathF.Sin(_framesSinceStart / 85f) / 3.3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 484f, ScreenSize.Y / 2f - 120f) + offset, null, Color.White, 3.1f + MathF.Sin(_framesSinceStart / 50f) / 2.9f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 484f, ScreenSize.Y / 2f - 115f) + offset, null, Color.White, -3.4f + MathF.Sin(_framesSinceStart / 60f + 1) / 3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 484f, ScreenSize.Y / 2f - 110f) + offset, null, Color.White, 3.3f + MathF.Sin(_framesSinceStart / 65f) / 2.9f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 484f, ScreenSize.Y / 2f - 120f) + offset, null, Color.White, 0.5f + MathF.Sin(_framesSinceStart / 70f + 1) / 4f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 484f, ScreenSize.Y / 2f - 115f) + offset, null, Color.White, 0.9f + MathF.Sin(_framesSinceStart / 85f) / 3.5f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 680f, ScreenSize.Y / 2f - 20f) + offset, null, Color.White, 3.1f + MathF.Sin(_framesSinceStart / 55f + 1) / 3.3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(ScreenSize.X / 2f + 680f, ScreenSize.Y / 2f - 20f) + offset, null, Color.White, 0.1f + MathF.Sin(_framesSinceStart / 75f) / 2.7f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            /* SubBars */
            for (int i = 0; i < 6; i++)
                batch.Draw(_subBar, new Vector2(ScreenSize.X / 2f, ScreenSize.Y - 203f + MathF.Exp((_framesSinceStart + i * 20) % 120 / 16f) + offset.Y), null, Color.White, 0f, _subBar.Size() / 2f, 1f, 0, 0);
            /* MainBar */
            batch.Draw(_mainBar, new Vector2(ScreenSize.X / 2f, ScreenSize.Y - 203f + offset.Y), null, Color.White, 0f, _mainBar.Size() / 2f, 1f, 0, 0);
        }
        private void DrawLogo(SpriteBatch batch, Vector2 offset) {
            offset += new Vector2(-Input.Mouse.X / ScreenSize.X - 0.5f,
                -Input.Mouse.Y / ScreenSize.Y - 0.5f) * 20f;
            /* Triangles */
            for (int i = 0; i < 5; i++)
                batch.Draw(_triangle, ScreenSize / 2f + new Vector2(0f, 161f) + offset, null, GetTransparentColor(255 / (i / 2 + 1)), MathF.Sin(_framesSinceStart / 250f + i * 10) / 5f, new Vector2(_triangle.Width / 2f, _triangle.Height), 1f + MathF.Sin(_framesSinceStart / 260f + i * 17) / 10f, 0, 0);
            /* EtherOut (Blue -> Pink -> White) */
            batch.Draw(_etherOutline, ScreenSize / 2f + new Vector2(-6f, -48f) + offset, null, Color.CornflowerBlue, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            batch.Draw(_etherOutline, ScreenSize / 2f + new Vector2(6f, -56f) + offset, null, Color.Violet, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            batch.Draw(_etherOutline, ScreenSize / 2f + new Vector2(0f, -52f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            /* Ether */
            batch.Draw(_etherText, ScreenSize / 2f + new Vector2(0f, -52f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherText.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            /* Mono */
            batch.Draw(_mono, ScreenSize / 2f + new Vector2(0f, -231f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 120f) / 20f, _mono.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 130f) / 20f, 0, 0);
            /* ByChris */
            batch.Draw(_byChris, ScreenSize / 2f + new Vector2(0f, 125f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 90f) / 30f, _byChris.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 80f) / 20f, 0, 0);
        }
        private static Color GetTransparentColor(float brightness) => new Color(brightness, brightness, brightness, brightness);
        private static Color GetTransparentColor(int brightness) => new Color(brightness, brightness, brightness, brightness);
    }
}
