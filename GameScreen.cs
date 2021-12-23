using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class GameScreen : GameState {
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly TileMap _tileMap = new TileMap("debugMap.txt");
        private readonly Camera _camera = new Camera();
        private string _mode = "Playing";
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
            Tile.LoadContent(content);
        }
        public override void UnloadContent() {
            /* Unload textures */
            PlayerShip.Texture = null;
            Tile.UnloadContent();
        }
        public override void Update(GameTime gameTime) {
            /* Handle user inputs */
            _camera.Update();

            _entityManager.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black); // TODO remove
            batch.Begin(samplerState: SamplerState.PointClamp);
            _entityManager.Draw(batch, _camera);
            _tileMap.Draw(batch, _camera, _mode == "Editor");
            batch.End();
            batch.Begin();

            batch.End();
        }
    }
}
