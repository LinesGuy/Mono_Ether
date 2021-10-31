using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    static class Fonts {
        public static SpriteFont Arial24;
        public static SpriteFont NovaSquare24;
        public static SpriteFont NovaSquare48;
        public static void Load(ContentManager content) {
            Arial24 = content.Load<SpriteFont>("Fonts/Arial24");
            NovaSquare24 = content.Load<SpriteFont>("Fonts/NovaSquare24");
            NovaSquare48 = content.Load<SpriteFont>("Fonts/NovaSquare48");
        }
    }
}