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
        private float timeOffset = 0f;
        private const float bounceInTime = 0.417f;
        private const float bounceOutTime = 0.083f;
        private const float totalBounce = bounceInTime + bounceOutTime;

        public Vector2 position = GameRoot.ScreenSize / 2;
        public float baseScalar = 0.65f;
        private float bounceScalar;

        public Texture2D image;

        public int state = 0;
        public void Update(GameTime gameTime)
        {
            // Cookie bounce
            timeOffset += 1 / 60f;

            if (timeOffset % totalBounce < bounceInTime)
                // Grow
                bounceScalar = MathUtil.Interpolate(0.94f, 1f, (timeOffset % totalBounce) / bounceInTime);
            else
                // Shrink
                bounceScalar = MathUtil.Interpolate(1f, 0.94f, (timeOffset % totalBounce - bounceInTime) / bounceOutTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, position, null, Color.White, 0f, image.Size() / 2f, baseScalar * bounceScalar, SpriteEffects.None, 0);
        }
    }
}
