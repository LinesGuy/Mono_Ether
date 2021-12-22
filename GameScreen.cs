using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class GameScreen : GameState {
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly Camera _camera = new Camera();
        public GameScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {
            _entityManager.Add(new PlayerShip());
        }
        public override void Suspend() {

        }
        public override void Resume() {

        }
        public override void LoadContent(ContentManager content) {
            /* Load textures */
            PlayerShip.Texture = content.Load<Texture2D>("Textures/GameScreen/PlayerShip");
        }
        public override void UnloadContent() {
            /* Unload textures */
            PlayerShip.Texture = null;
        }
        public override void Update(GameTime gameTime) {
            /* Handle user inputs */
            _camera.Update();

            _entityManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black); // TODO remove
            batch.DrawString(GlobalAssets.NovaSquare24, "asdf", Vector2.Zero, Color.White); // TODO remove
            _entityManager.Draw(batch, _camera);
        }
    }
}
