using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using Mono_Ether.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mono_Ether.MainMenu
{
    public class MainMenu : GameState
    {
        //private SpriteBatch spriteBatch;
        private int frame = 0;
        private const int bounceOffset = 0;

        private const int fadeInFrames = 60;
        private float fadeTransparency = 1f;

        private const int bounceInFrames = 25;
        private const int bounceOutFrames = 5;
        private const int totalFrames = bounceInFrames + bounceOutFrames;
        private Texture2D cookie;
        private Vector2 cookiePos = GameRoot.ScreenSize / 2;
        private Vector2 destPos = GameRoot.ScreenSize / 2;
        private float bounceScalar;
        private float cookieScalar = 0.65f;
        private float destScalar = 0.65f;
        private bool cursorInCookie = false;

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
            cookie = content.Load<Texture2D>("Textures/Menu/cookie");
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

            // Cookie bounce
            float offsetFrames = (float)frame + bounceOffset; // Float to skip casting to float later on
            if (offsetFrames % totalFrames < bounceInFrames)
            {
                // Grow
                bounceScalar = MathUtil.Interpolate(0.94f, 1f, (offsetFrames % totalFrames) / bounceInFrames);
            }
            else
            {
                // Shrink
                bounceScalar = MathUtil.Interpolate(1f, 0.94f, (offsetFrames % totalFrames - bounceInFrames) / bounceOutFrames);
            }

            // If cursor is within cookie...
            float cookieWidth = cookie.Width * cookieScalar * bounceScalar;
            if (Vector2.DistanceSquared(cookiePos, Input.MousePosition) < cookieWidth * cookieWidth)
            {
                // Expand cookie slightly
                if (!cursorInCookie)
                {
                    cursorInCookie = true;
                    destScalar *= 1.1f;
                }

                // If left clicked, advance menu
                if (Input.WasMouseClicked("left"))
                {
                    destPos.X = GameRoot.ScreenSize.X / 3f;
                    destScalar *= 0.75f;
                }
            }
            else
            {
                if (cursorInCookie)
                {
                    cursorInCookie = false;
                    destScalar /= 1.1f;
                }
            }

            // Lerp cookie and button pos'
            cookiePos = MathUtil.InterpolateV(cookiePos, destPos, 0.05f);
            cookieScalar = MathUtil.Interpolate(cookieScalar, destScalar, 0.05f);

            frame += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(cookie, cookiePos, null, Color.White, 0f, cookie.Size() / 2f, cookieScalar * bounceScalar, SpriteEffects.None, 0);
            // Fade in
            spriteBatch.Draw(Art.Pixel, new RectangleF(0, 0, GameRoot.ScreenSize.X, GameRoot.ScreenSize.Y).ToRectangle(), Color.White * fadeTransparency);
            spriteBatch.End();
        }
    }
}
