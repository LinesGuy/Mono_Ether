using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        private List<Vector2> _smallStars;
        private List<Vector2> _bigStars;
        private Random _rand;
        private ButtonManager _titleButtonManager;
        private ButtonManager _levelButtonManager;
        private ButtonManager _carouselButtonManager;
        private float _carouselOffset;
        private int _framesSinceStart;
        private int _framesSinceTransition;
        private string state;
        public TitleScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {
            _framesSinceStart = 0;
            _framesSinceTransition = 0;
            state = "Title press any key";
            _rand = new Random();
            /* Create button managers */
            _titleButtonManager = new ButtonManager();
            _levelButtonManager = new ButtonManager();
            _carouselButtonManager = new ButtonManager();
            /* Add Title screen buttons */
            _titleButtonManager.Buttons.Add(new Button(new Vector2(GameSettings.ScreenSize.X / 2f - 300f, GameSettings.ScreenSize.Y - 100f), "Start"));
            _titleButtonManager.Buttons.Add(new Button(new Vector2(GameSettings.ScreenSize.X / 2f + 300f, GameSettings.ScreenSize.Y - 100f), "Exit"));
            /* Add Level selection screen button */
            _levelButtonManager.Buttons.Add(new Button(new Vector2(200f, GameSettings.ScreenSize.Y - 100f), "Back"));
            /* Add carousel buttons */
            List<string> levels = new List<string> {"Level One", "Level Two", "Level Three", "Level Four", "Level Five", "Level Six", "Level Seven ", "Level Eight", "Level Nine", "Level Ten" };
            for (int i = 0; i < levels.Count; i++)
                _carouselButtonManager.Buttons.Add(new Button(new Vector2(GameSettings.ScreenSize.X  - 300f * MathF.Exp(-i * i / 25f), GameSettings.ScreenSize.Y / 2f + i * 120f), levels[i]));
            _carouselOffset = 0f;
            /* Create lists of small/big stars with random positions */
            _smallStars = new List<Vector2>();
            _bigStars = new List<Vector2>();
            for (var i = 0; i < 150; i++)
                _smallStars.Add(new Vector2(_rand.Next(0, (int)GameSettings.ScreenSize.X), _rand.Next(0, (int)(GameSettings.ScreenSize.Y))));
            for (var i = 0; i < 5; i++)
                _bigStars.Add(new Vector2(_rand.Next(0, (int)GameSettings.ScreenSize.X), _rand.Next(0, (int)GameSettings.ScreenSize.Y)));
        }

        public override void LoadContent(ContentManager content) {
            /* Load textures */
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
        }

        public override void UnloadContent() {
            /* Unload textures */
            _smallStar.Dispose();
            _bigStar.Dispose();
            _bg.Dispose();
            _mainBar.Dispose();
            _subBar.Dispose();
            _byChris.Dispose();
            _etherText.Dispose();
            _etherOutline.Dispose();
            _mono.Dispose();
            _triangle.Dispose();
            _treeBg.Dispose();
            _treeLeaf.Dispose();
        }

        public override void Update(GameTime gameTime) {
            /* Reset scene if user pressed R */
            if (Input.WasKeyJustUp(Keys.R))
                Initialize();
            _framesSinceStart++;
            _framesSinceTransition++;
            switch (state) {
                case "Title press any key":
                    if (Input.Keyboard.GetPressedKeyCount() > 0 || Input.WasLeftButtonJustDown || Input.WasRightButtonJustDown) {
                        state = "Title press any key -> Title";
                        _framesSinceTransition = 0;
                    }
                    break;
                case "Title press any key -> Title":
                    if (_framesSinceTransition > 30f) {
                        state = "Title";
                        _framesSinceTransition = 0;
                    }
                    _titleButtonManager.Update();
                    HandleTitleButtonPresses();
                    break;
                case "Title":
                    _titleButtonManager.Update();
                    HandleTitleButtonPresses();
                    break;
                case "Title -> Level selection":
                    if (_framesSinceTransition > 30f) {
                        state = "Level selection";
                        _framesSinceTransition = 0;
                    }
                    _titleButtonManager.Update();
                    _levelButtonManager.Update();
                    _carouselButtonManager.Update();
                    HandleLevelButtonPresses();
                    break;
                case "Level selection":
                    _levelButtonManager.Update();
                    _carouselOffset += Input.DeltaScrollWheelValue / 500f;
                    Debug.WriteLine(_carouselOffset);
                    for (int i = 0; i < _carouselButtonManager.Buttons.Count; i++)
                    {
                        float j = i + _carouselOffset;
                        _carouselButtonManager.Buttons[i].Pos = new Vector2(
                            GameSettings.ScreenSize.X - 300f * MathF.Exp(-j * j / 25f),
                            GameSettings.ScreenSize.Y / 2f + j * 120f);
                    }
                    _carouselButtonManager.Update();
                    HandleLevelButtonPresses();
                    break;
                case "Level selection -> Title":
                    if (_framesSinceTransition > 30f) {
                        state = "Title";
                        _framesSinceTransition = 0;
                    }
                    _titleButtonManager.Update();
                    _levelButtonManager.Update();
                    _carouselButtonManager.Update();
                    HandleTitleButtonPresses();
                    break;
                case "Level selection -> level":
                    break;
            }
        }
        public override void Draw(SpriteBatch batch) {
            switch (state) {
                case "Title press any key":
                    DrawBG(batch);
                    Draw_Logo(batch, new Vector2(0, 20f));
                    /* Press any key to start */
                    if (_framesSinceTransition % 60 < 40)
                        batch.DrawStringCentered(GlobalAssets.NovaSquare48, "PRESS ANY KEY", new Vector2(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y - 100f), Color.White);
                    /* Screen flash */
                    if (_framesSinceStart <= 60)
                        batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(0, 0, GameSettings.ScreenSize.X, GameSettings.ScreenSize.Y), GetTransparentColor((60 - _framesSinceStart) / 60f));
                    break;
                case "Title press any key -> Title":
                    DrawBG(batch);
                    Draw_Logo(batch, new Vector2(0, -20f + 40f / MathF.Exp(_framesSinceTransition / 5f)));
                    _titleButtonManager.Draw(batch, new Vector2(0f, 200f / MathF.Exp(_framesSinceTransition / 5f)));
                    break;
                case "Title":
                    DrawBG(batch);
                    Draw_Logo(batch, new Vector2(0, -20f));
                    _titleButtonManager.Draw(batch, Vector2.Zero);
                    break;
                case "Title -> Level selection":
                    DrawBG(batch);
                    Draw_Logo(batch, new Vector2(0f, - 20f - GameSettings.ScreenSize.Y * (1 - MathF.Exp(-_framesSinceTransition / 5f))));
                    _titleButtonManager.Draw(batch, new Vector2(0f, 200f * (1 - MathF.Exp(-_framesSinceTransition / 5f))));
                    _levelButtonManager.Draw(batch, new Vector2(-GameSettings.ScreenSize.X * MathF.Exp(-_framesSinceTransition / 5f), 0f));
                    _carouselButtonManager.Draw(batch, new Vector2(GameSettings.ScreenSize.X * MathF.Exp(-_framesSinceTransition / 5f), 0f));
                    break;
                case "Level selection":
                    DrawBG(batch);
                    _levelButtonManager.Draw(batch, Vector2.Zero);
                    _carouselButtonManager.Draw(batch, Vector2.Zero);
                    break;
                case "Level selection -> Title":
                    DrawBG(batch);
                    Draw_Logo(batch, new Vector2(0f, -20f - GameSettings.ScreenSize.Y * MathF.Exp(-_framesSinceTransition / 5f)));
                    _titleButtonManager.Draw(batch, new Vector2(0f, 200f * MathF.Exp(-_framesSinceTransition / 5f)));
                    _levelButtonManager.Draw(batch, new Vector2(GameSettings.ScreenSize.X * (1f - MathF.Exp(_framesSinceTransition / 10f)), 0f));
                    _carouselButtonManager.Draw(batch, new Vector2(-GameSettings.ScreenSize.X * (1f - MathF.Exp(_framesSinceTransition / 10f)), 0f));
                    break;
                case "Level selection -> level":
                    DrawBG(batch);
                    break;
            }
        }

        private void HandleTitleButtonPresses() {
            switch (_titleButtonManager.PressedButton) {
                case "Start":
                    state = "Title -> Level selection";
                    _framesSinceTransition = 0;
                    break;
                case "Exit":
                    ScreenManager.RemoveScreen();
                    break;
            }
        }
        private void HandleLevelButtonPresses()
        {
            switch (_levelButtonManager.PressedButton) {
                case "Back":
                    state = "Level selection -> Title";
                    _framesSinceTransition = 0;
                    break;
            }
        }

        private void HandleCarouselButtonPresses()
        {
            switch (_carouselButtonManager.PressedButton)
            {
                case "Level One":
                    break;
                // TODO other levels
            }
        }
        private void DrawBG(SpriteBatch batch) {
            /* Bg */
            batch.Draw(_bg, MyUtils.RectangleF(0, 0, GameSettings.ScreenSize.X, GameSettings.ScreenSize.Y), Color.White);
            /* SmallStars */
            foreach (var starPos in _smallStars)
                batch.Draw(_smallStar, starPos, null, GetTransparentColor(0.5f + MathF.Sin(starPos.X + _framesSinceStart / 60f) * 0.5f), 0f, _smallStar.Size() / 2f, 1f + MathF.Sin(starPos.X) / 3f, 0, 0);
            /* BigStars */
            foreach (var starPos in _bigStars)
                batch.Draw(_bigStar, starPos, null, GetTransparentColor(0.5f + MathF.Sin(starPos.X + _framesSinceStart / 60f) * 0.5f), starPos.X + _framesSinceStart / 60f, _bigStar.Size() / 2f, 0.5f + MathF.Sin(starPos.X) / 3f, 0, 0);
            /* Trees */
            batch.Draw(_treeBg, new Vector2(GameSettings.ScreenSize.X / 2f - 550f, GameSettings.ScreenSize.Y / 2f), null, Color.White, 0f, _treeBg.Size() / 2f, 1f, 0, 0);
            batch.Draw(_treeBg, new Vector2(GameSettings.ScreenSize.X / 2f + 550f, GameSettings.ScreenSize.Y / 2f), null, Color.White, 0f, _treeBg.Size() / 2f, 1f, SpriteEffects.FlipHorizontally, 0);
            /* Tree Leaves */
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 484f, GameSettings.ScreenSize.Y / 2f - 120f), null, Color.White, 0.1f + MathF.Sin(_framesSinceStart / 60f) / 3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 484f, GameSettings.ScreenSize.Y / 2f - 115f), null, Color.White, -0.4f + MathF.Sin(_framesSinceStart / 50f + 1) / 2.9f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 484f, GameSettings.ScreenSize.Y / 2f - 110f), null, Color.White, 0.3f + MathF.Sin(_framesSinceStart / 70f) / 2.5f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 484f, GameSettings.ScreenSize.Y / 2f - 120f), null, Color.White, 3.5f + MathF.Sin(_framesSinceStart / 65f + 1) / 3.5f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 484f, GameSettings.ScreenSize.Y / 2f - 115f), null, Color.White, 2.9f + MathF.Sin(_framesSinceStart / 55f) / 4f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 680f, GameSettings.ScreenSize.Y / 2f - 20f), null, Color.White, 0.1f + MathF.Sin(_framesSinceStart / 75f + 1) / 2.7f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f - 680f, GameSettings.ScreenSize.Y / 2f - 20f), null, Color.White, 3.1f + MathF.Sin(_framesSinceStart / 85f) / 3.3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 484f, GameSettings.ScreenSize.Y / 2f - 120f), null, Color.White, 3.1f + MathF.Sin(_framesSinceStart / 50f) / 2.9f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 484f, GameSettings.ScreenSize.Y / 2f - 115f), null, Color.White, -3.4f + MathF.Sin(_framesSinceStart / 60f + 1) / 3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 484f, GameSettings.ScreenSize.Y / 2f - 110f), null, Color.White, 3.3f + MathF.Sin(_framesSinceStart / 65f) / 2.9f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 484f, GameSettings.ScreenSize.Y / 2f - 120f), null, Color.White, 0.5f + MathF.Sin(_framesSinceStart / 70f + 1) / 4f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 484f, GameSettings.ScreenSize.Y / 2f - 115f), null, Color.White, 0.9f + MathF.Sin(_framesSinceStart / 85f) / 3.5f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 680f, GameSettings.ScreenSize.Y / 2f - 20f), null, Color.White, 3.1f + MathF.Sin(_framesSinceStart / 55f + 1) / 3.3f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            batch.Draw(_treeLeaf, new Vector2(GameSettings.ScreenSize.X / 2f + 680f, GameSettings.ScreenSize.Y / 2f - 20f), null, Color.White, 0.1f + MathF.Sin(_framesSinceStart / 75f) / 2.7f, new Vector2(0, _treeLeaf.Height), 1f, 0, 0);
            /* SubBars */
            for (int i = 0; i < 6; i++)
                batch.Draw(_subBar, new Vector2(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y - 203f + MathF.Exp((_framesSinceStart + i * 20) % 120 / 16f)), null, Color.White, 0f, _subBar.Size() / 2f, 1f, 0, 0);
            /* MainBar */
            batch.Draw(_mainBar, new Vector2(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y - 203f), null, Color.White, 0f, _mainBar.Size() / 2f, 1f, 0, 0);
        }

        private void Draw_Logo(SpriteBatch batch, Vector2 offset) {
            /* Triangles */
            for (int i = 0; i < 5; i++)
                batch.Draw(_triangle, GameSettings.ScreenSize / 2f + new Vector2(0f, 161f) + offset, null, GetTransparentColor(255 / (i / 2 + 1)), MathF.Sin(_framesSinceStart / 250f + i * 10) / 5f, new Vector2(_triangle.Width / 2f, _triangle.Height), 1f + MathF.Sin(_framesSinceStart / 260f + i * 17) / 10f, 0, 0);
            /* EtherOut (Blue -> Pink -> White) */
            batch.Draw(_etherOutline, GameSettings.ScreenSize / 2f + new Vector2(-6f, -48f) + offset, null, Color.CornflowerBlue, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            batch.Draw(_etherOutline, GameSettings.ScreenSize / 2f + new Vector2(6f, -56f) + offset, null, Color.Violet, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            batch.Draw(_etherOutline, GameSettings.ScreenSize / 2f + new Vector2(0f, -52f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            /* Ether */
            batch.Draw(_etherText, GameSettings.ScreenSize / 2f + new Vector2(0f, -52f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 150f) / 16f, _etherText.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 110f) / 20f, 0, 0);
            /* Mono */
            batch.Draw(_mono, GameSettings.ScreenSize / 2f + new Vector2(0f, -231f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 120f) / 20f, _mono.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 130f) / 20f, 0, 0);
            /* ByChris */
            batch.Draw(_byChris, GameSettings.ScreenSize / 2f + new Vector2(0f, 125f) + offset, null, Color.White, MathF.Sin(_framesSinceStart / 90f) / 30f, _byChris.Size() / 2f, 1.5f + MathF.Sin(_framesSinceStart / 80f) / 20f, 0, 0);
        }
        private Color GetTransparentColor(float brightness) {
            return new Color(brightness, brightness, brightness, brightness);
        }
        private Color GetTransparentColor(int brightness) {
            return new Color(brightness, brightness, brightness, brightness);
        }
    }
}
