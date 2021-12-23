using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether {
    public class GameScreen : GameState {
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly TileMap _tileMap;
        private readonly Camera _camera = new Camera();
        private string _mode = "Playing";
        public GameScreen(GraphicsDevice graphicsDevice, string mapFileName) : base(graphicsDevice) {
            /* Load tile map data from filename */
            _tileMap = new TileMap(mapFileName);
        }
        public override void Initialize() {
            /* Add player one */
            _entityManager.Add(new PlayerShip() { Position = _tileMap.WorldSize / 2 });
            /* Set camera initial position to player position */
            _camera.Position = _entityManager.Players.First().Position;
        }
        public override void Suspend() {

        }
        public override void Resume() {

        }
        public override void LoadContent(ContentManager content) {
            PlayerShip.Texture = content.Load<Texture2D>("Textures/GameScreen/PlayerShip");
            Tile.LoadContent(content);
        }
        public override void UnloadContent() {
            PlayerShip.Texture = null;
            Tile.UnloadContent();
        }
        public override void Update(GameTime gameTime) {
            // TODO remove esc to return
            if (Input.WasKeyJustDown(Keys.Escape)) ScreenManager.RemoveScreen();
            /* Handle camera position based on user input */
            _camera.Update();
            /* Update all entity positions and handle collisions */
            _entityManager.Update(gameTime);
            /* Lerp camera if enabled */
            if (_camera.IsLerping) {
                // TODO lerp to average of all player pos', not just player1
                _camera.LerpTo(_entityManager.Players.First().Position);
            }
        }
        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black); // TODO remove
            batch.Begin(samplerState: SamplerState.PointClamp);
            /* Draw all entities (inc players, bullets, powerpacks etc) */
            _entityManager.Draw(batch, _camera);
            /* Draw tilemap with tile boundaries if in editor mode */
            _tileMap.Draw(batch, _camera, _mode == "Editor");
            batch.End();
            batch.Begin();

            batch.End();
        }
    }
}
