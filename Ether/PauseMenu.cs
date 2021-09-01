using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether
{
    public static class PauseMenu
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Art.pauseBg, GameRoot.ScreenSize / 2f, null, Color.White, 0f, Art.pauseBg.Size() / 2f, 1f, SpriteEffects.None, 0);
        }
    }
}