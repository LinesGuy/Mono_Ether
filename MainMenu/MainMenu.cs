using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using Mono_Ether.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mono_Ether.MainMenu
{
    public class MainMenu : GameState
    {
        private int frame = 0;

        private const int fadeInFrames = 60;
        private float fadeTransparency = 1f;
        private Cookie cookie = new Cookie();
        private readonly Tweener _tweener = new Tweener();

        Song welcomeTrack;
        public MainMenu(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(ContentManager content)
        {

            welcomeTrack = content.Load<Song>("Tracks/welcome_track");
            cookie.image = content.Load<Texture2D>("Textures/Menu/cookie");
            MediaPlayer.Volume = 1f;
            MediaPlayer.Play(welcomeTrack);
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            // White fade-in
            if (frame < fadeInFrames)
                fadeTransparency = MathUtil.Interpolate(1f, 0f, (float)frame / fadeInFrames);

            cookie.Update(gameTime);

            frame += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            cookie.Draw(spriteBatch);
            
            // Fade in
            spriteBatch.Draw(Art.Pixel, new RectangleF(0, 0, GameRoot.ScreenSize.X, GameRoot.ScreenSize.Y).ToRectangle(), Color.White * fadeTransparency);
            spriteBatch.End();
        }
    }
}
