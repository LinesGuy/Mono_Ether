using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class GameScreen : GameState {
        public GameScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }

        public override void Initialize() {

        }

        public override void Suspend()
        {

        }
        public override void Resume() {

        }

        public override void LoadContent(ContentManager content) {
            /* Load textures */

        }

        public override void UnloadContent() {
            /* Unload textures */

        }

        public override void Update(GameTime gameTime) {
        }

        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black); // TODO remove
            batch.DrawString(GlobalAssets.NovaSquare24, "asdf", Vector2.Zero, Color.White);
        }
    }
}
