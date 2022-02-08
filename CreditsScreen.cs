using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether {
    public class CreditsScreen : GameState {
        public CreditsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {

        }
        public override void Suspend() {

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
            if (Input.WasKeyJustDown(Keys.Escape)) ScreenManager.RemoveScreen();
        }
        public override void Draw(SpriteBatch batch) {
            batch.Begin();
            GraphicsDevice.Clear(Color.Black); // TODO remove
            batch.DrawString(GlobalAssets.NovaSquare24, "everything by me", Vector2.Zero, Color.White);
            batch.End();
        }
    }
}