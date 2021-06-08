using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using Mono_Ether.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.MainMenu
{
    public class IntroWelcome : GameState
    {
        //private SpriteBatch spriteBatch;
        private int frame = 0;
        
        private const int fadeInFrames = 120;
        private const int fastScaleFrames = 30;
        private const int fadeOutFrames = 8;
        
        private float welcomeTransparency = 0f;
        private RectangleF welcomeRect = new RectangleF(GameRoot.ScreenSize.X / 2 - Art.welcomeText.Width / 4, GameRoot.ScreenSize.Y / 2 - Art.welcomeText.Height / 4, Art.welcomeText.Width / 2, 0);

        Song welcomePiano;
        Song welcomeTrack;
        public IntroWelcome(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            welcomePiano = content.Load<Song>("Samples/Intro/welcome_piano");
            
            //welcomePiano = content.Load<Song>("Tracks/Deceit");
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(welcomePiano);
        }

        public override void UnloadContent()
        {
            MediaPlayer.Stop();
            welcomePiano.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            if (frame <= fastScaleFrames)
                // Fast scale
                welcomeRect.Inflate(0, 1.3f);

            if (frame < fadeInFrames)
            {
                // Fade in + slow scale
                welcomeTransparency = MathUtil.Interpolate(0, 1, (float)frame / fadeInFrames);
                welcomeRect.Inflate(0.12f, 0.025f);
            }
            else if (fadeInFrames <= frame && frame <= fadeInFrames + fadeOutFrames)
            {
                // Fade out
                welcomeTransparency = MathUtil.Interpolate(1, 0, (float)(frame - fadeInFrames) / fadeOutFrames);
            }
            else if (frame > fadeInFrames + fadeOutFrames)
            {
                // Change screen
                // To game:
                //GameStateManager.Instance.ChangeScreen(new Ether.EtherRoot(_graphicsDevice));
                // To menu:
                GameStateManager.Instance.ChangeScreen(new MainMenu(_graphicsDevice));
            }
            frame += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(Art.welcomeText, welcomeRect.ToRectangle(), null, Color.White * welcomeTransparency);
            spriteBatch.End();
        }
    }
}
