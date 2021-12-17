﻿using Microsoft.Xna.Framework;
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
        private List<Vector2> _smallStars;
        private List<Vector2> _bigStars;
        private Random _rand;
        private ButtonManager buttonManager;
        private int _frame;
        public TitleScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {
            _frame = 0;
            _rand = new Random();
            /* Create buttonManager and add Start/Exit buttons */
            buttonManager = new ButtonManager();
            buttonManager.Buttons.Add(new Button(new Vector2(GameSettings.ScreenSize.X / 2f - 300f, GameSettings.ScreenSize.Y - 80f), "Start"));
            buttonManager.Buttons.Add(new Button(new Vector2(GameSettings.ScreenSize.X / 2f + 300f, GameSettings.ScreenSize.Y - 80f), "Exit"));
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
        }

        public override void Update(GameTime gameTime) {
            _frame++;
            /* Reset scene if user pressed R */
            if (Input.WasKeyJustDown(Keys.R))
                Initialize();
            buttonManager.Update();
            switch (buttonManager.PressedButton) {
                case "Start":
                    ScreenManager.AddScreen(new GameScreen(GraphicsDevice));
                    break;
                case "Exit":
                    ScreenManager.RemoveScreen();
                    break;
            }
    }

    public override void Draw(SpriteBatch batch) {
        /* Bg */
        batch.Draw(_bg, MyUtils.RectangleF(0, 0, GameSettings.ScreenSize.X, GameSettings.ScreenSize.Y), Color.White);
        /* Triangles */
        for (int i = 0; i < 5; i++)
            batch.Draw(_triangle, GameSettings.ScreenSize / 2f + new Vector2(0f, 161f), null, GetTransparentColor(255 / (i / 2 + 1)), MathF.Sin(_frame / 250f + i * 10) / 5f, new Vector2(_triangle.Width / 2f, _triangle.Height), 1f + MathF.Sin(_frame / 260f + i * 17) / 10f, 0, 0);
        /* SubBars */
        for (int i = 0; i < 6; i++)
            batch.Draw(_subBar, new Vector2(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y - 203f + MathF.Exp((_frame + i * 20) % 120 / 16f)), null, Color.White, 0f, _subBar.Size() / 2f, 1f, 0, 0);
        /* MainBar */
        batch.Draw(_mainBar, new Vector2(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y - 203f), null, Color.White, 0f, _mainBar.Size() / 2f, 1f, 0, 0);
        // TODO Orange/yellow tree thingies y'know
        //batch.Draw()
        /* SmallStars */
        foreach (var starPos in _smallStars)
            batch.Draw(_smallStar, starPos, null, GetTransparentColor(0.5f + MathF.Sin(starPos.X + _frame / 60f) * 0.5f), 0f, _smallStar.Size() / 2f, 1f + MathF.Sin(starPos.X) / 3f, 0, 0);
        /* BigStars */
        foreach (var starPos in _bigStars)
            batch.Draw(_bigStar, starPos, null, GetTransparentColor(0.5f + MathF.Sin(starPos.X + _frame / 60f) * 0.5f), starPos.X + _frame / 60f, _bigStar.Size() / 2f, 0.5f + MathF.Sin(starPos.X) / 3f, 0, 0);
        /* EtherOut (Blue -> Pink -> White) */
        batch.Draw(_etherOutline, GameSettings.ScreenSize / 2f + new Vector2(-6f, -48f), null, Color.CornflowerBlue, MathF.Sin(_frame / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_frame / 110f) / 20f, 0, 0);
        batch.Draw(_etherOutline, GameSettings.ScreenSize / 2f + new Vector2(6f, -56f), null, Color.Violet, MathF.Sin(_frame / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_frame / 110f) / 20f, 0, 0);
        batch.Draw(_etherOutline, GameSettings.ScreenSize / 2f + new Vector2(0f, -52f), null, Color.White, MathF.Sin(_frame / 150f) / 16f, _etherOutline.Size() / 2f, 1.5f + MathF.Sin(_frame / 110f) / 20f, 0, 0);
        /* Ether */
        batch.Draw(_etherText, GameSettings.ScreenSize / 2f + new Vector2(0f, -52f), null, Color.White, MathF.Sin(_frame / 150f) / 16f, _etherText.Size() / 2f, 1.5f + MathF.Sin(_frame / 110f) / 20f, 0, 0);
        /* Mono */
        batch.Draw(_mono, GameSettings.ScreenSize / 2f + new Vector2(0f, -231f), null, Color.White, MathF.Sin(_frame / 120f) / 20f, _mono.Size() / 2f, 1.5f + MathF.Sin(_frame / 130f) / 20f, 0, 0);
        /* ByChris */
        batch.Draw(_byChris, GameSettings.ScreenSize / 2f + new Vector2(0f, 125f), null, Color.White, MathF.Sin(_frame / 90f) / 30f, _byChris.Size() / 2f, 1.5f + MathF.Sin(_frame / 80f) / 20f, 0, 0);
        /* Buttons */
        buttonManager.Draw(batch);
        /* Flash screen on start, keep this below everything else */
        if (_frame <= 60)
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(0, 0, GameSettings.ScreenSize.X, GameSettings.ScreenSize.Y), GetTransparentColor((60 - _frame) / 60f));
    }

    private Color GetTransparentColor(float brightness) {
        return new Color(brightness, brightness, brightness, brightness);
    }
    private Color GetTransparentColor(int brightness) {
        return new Color(brightness, brightness, brightness, brightness);
    }
}
}