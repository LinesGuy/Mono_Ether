using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.MainMenuOld {
    public class Cookie {
        private float timeOffset;
        private const float BounceInTime = 0.417f;
        private const float BounceOutTime = 0.083f;
        private const float TotalBounce = BounceInTime + BounceOutTime;

        public Vector2 Position = GameRoot.ScreenSize / 2;
        public const float BaseScalar = 0.65f;
        private float bounceScalar;

        public Texture2D Image;

        public int State = 0;
        public void Update(GameTime gameTime) {
            // Cookie bounce
            timeOffset += 1 / 60f;

            bounceScalar = timeOffset % TotalBounce < BounceInTime ? MathUtil.Interpolate(0.94f, 1f, (timeOffset % TotalBounce) / BounceInTime) : MathUtil.Interpolate(1f, 0.94f, (timeOffset % TotalBounce - BounceInTime) / BounceOutTime);
        }
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Image, Position, null, Color.White, 0f, Image.Size() / 2f, BaseScalar * bounceScalar, SpriteEffects.None, 0);
        }
    }
}
