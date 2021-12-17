using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    static class GlobalAssets {
        public static Texture2D Button;
        public static Texture2D Pixel;
        public static SpriteFont NovaSquare24;
        public static SpriteFont NovaSquare48;
        public static void LoadContent(ContentManager content) {
            Button = content.Load<Texture2D>("Textures/GlobalAssets/Button");
            Pixel = content.Load<Texture2D>("Textures/GlobalAssets/Pixel");
            NovaSquare24 = content.Load<SpriteFont>("Fonts/NovaSquare24");
            NovaSquare48 = content.Load<SpriteFont>("Fonts/NovaSquare48");
        }
    }
}