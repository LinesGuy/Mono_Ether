using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mono_Ether.MainMenu
{
    public class Cookie
    {
        private int frame = 0;
        private const int bounceOffset = 0;
        private const int bounceInFrames = 25;
        private const int bounceOutFrames = 5;
        private const int totalBounceFrames = bounceInFrames + bounceOutFrames;

        public Vector2 position = GameRoot.ScreenSize / 2;
        private float bounceScalar;
        public float baseScalar = 0.65f;

        public Texture2D image;

        public int state = 0;
        public bool justClicked;
        public bool cursorInCookie = false;

        private readonly Tweener tweener = new Tweener();

        public void Update(GameTime gameTime)
        {
            // Cookie bounce
            float offsetFrames = (float)frame + bounceOffset; // Float to skip casting to float later on
            if (offsetFrames % totalBounceFrames < bounceInFrames)
            {
                // Grow
                bounceScalar = MathUtil.Interpolate(0.94f, 1f, (offsetFrames % totalBounceFrames) / bounceInFrames);
            }
            else
            {
                // Shrink
                bounceScalar = MathUtil.Interpolate(1f, 0.94f, (offsetFrames % totalBounceFrames - bounceInFrames) / bounceOutFrames);
            }

            // If cursor is within cookie...
            float cookieWidth = image.Width * baseScalar * bounceScalar;
            if (Vector2.DistanceSquared(position, Input.MousePosition) < cookieWidth * cookieWidth)
            {
                if (Input.WasMouseClicked("left"))
                {
                    tweener.TweenTo(this, a => a.position, new Vector2(GameRoot.ScreenSize.X / 3f, GameRoot.ScreenSize.Y / 2f), duration: 2)
                        .Easing(EasingFunctions.ExponentialOut);
                    tweener.TweenTo(this, a => a.baseScalar, 0.48f, duration: 2)
                        .Easing(EasingFunctions.ExponentialOut);
                }
            }
            tweener.Update(gameTime.GetElapsedSeconds());

            frame += 1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, position, null, Color.White, 0f, image.Size() / 2f, baseScalar * bounceScalar, SpriteEffects.None, 0);
        }
    }
}
