using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class PlayerShip : Entity {
        public static Texture2D Texture;
        public PlayerShip() {
            Image = Texture;
        }
        public override void Update(GameTime gameTime) {
        }
    }
}
