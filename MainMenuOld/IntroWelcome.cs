using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using Mono_Ether.States;

namespace Mono_Ether.MainMenuOld
{
    public class IntroWelcome : GameState
    {
        //private SpriteBatch spriteBatch;
        private int frame;
        
        private const int FadeInFrames = 120;
        private const int FastScaleFrames = 30;
        private const int FadeOutFrames = 8;

        private float welcomeTransparency;
        private RectangleF welcomeRect = new RectangleF(GameRoot.ScreenSize.X / 2 - Art.WelcomeText.Width / 4,
            GameRoot.ScreenSize.Y / 2 - Art.WelcomeText.Height / 4, Art.WelcomeText.Width / 2, 0);

        Song welcomePiano;
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
            if (frame <= FastScaleFrames)
                // Fast scale
                welcomeRect.Inflate(0, 1.3f);

            if (frame < FadeInFrames)
            {
                // Fade in + slow scale
                welcomeTransparency = MathUtil.Interpolate(0, 1, (float)frame / FadeInFrames);
                welcomeRect.Inflate(0.12f, 0.025f);
            }
            else if (FadeInFrames <= frame && frame <= FadeInFrames + FadeOutFrames)
            {
                // Fade out
                welcomeTransparency = MathUtil.Interpolate(1, 0, (float)(frame - FadeInFrames) / FadeOutFrames);
            }
            else if (frame > FadeInFrames + FadeOutFrames)
            {
                // Change screen
                // To game:
                //GameStateManager.Instance.ChangeScreen(new Ether.EtherRoot(_graphicsDevice));
                // To menu:
                GameStateManager.Instance.ChangeScreen(new MainMenu(GraphicsDevice));
            }
            frame += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(Art.WelcomeText, welcomeRect.ToRectangle(), null, Color.White * welcomeTransparency);
            spriteBatch.End();
        }
    }
}
