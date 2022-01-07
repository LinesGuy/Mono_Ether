using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Mono_Ether {
    public class GameScreen : GameState {
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly ParticleManager _particleManager = new ParticleManager();
        private readonly EnemySpawner _enemySpawner = new EnemySpawner();
        private readonly TileMap _tileMap;
        private readonly string _mode = "Playing";
        public GameScreen(GraphicsDevice graphicsDevice, string mapFileName) : base(graphicsDevice) {
            /* Load tile map data from filename */
            _tileMap = new TileMap(mapFileName);
        }
        public override void Initialize() {
            ParticleManager.Instance = _particleManager;
            /* Add one player */
            _entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2, MyUtils.ViewportF(0, 0, GameSettings.ScreenSize.X, GameSettings.ScreenSize.Y)));
            /* Add two players */
            //_entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2, MyUtils.ViewportF(0, 0, GameSettings.ScreenSize.X * 2f / 3f, GameSettings.ScreenSize.Y)));
            //_entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2 + new Vector2(100f, 0f), MyUtils.ViewportF(GameSettings.ScreenSize.X * 2f / 3f, 0, GameSettings.ScreenSize.X / 3f, GameSettings.ScreenSize.Y)));
            /* Add four players */
            //_entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2, MyUtils.ViewportF(0, 0, GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y / 2f)));
            //_entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2 + new Vector2(100f, 0f), MyUtils.ViewportF(GameSettings.ScreenSize.X / 2f, 0, GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y / 2f)));
            //_entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2 + new Vector2(100f, 100f), MyUtils.ViewportF(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y / 2f, GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y / 2f)));
            //_entityManager.Add(new PlayerShip(GraphicsDevice, _tileMap.WorldSize / 2 + new Vector2(0f, 100f), MyUtils.ViewportF(0, GameSettings.ScreenSize.Y / 2f, GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y / 2f)));
        }
        public override void Suspend() {

        }
        public override void Resume() {

        }
        public override void LoadContent(ContentManager content) {
            PlayerShip.LoadContent(content);
            Particle.PointParticle = content.Load<Texture2D>("Textures/GameScreen/Particles/Point");
            Tile.LoadContent(content);
            Enemy.LoadContent(content);
        }
        public override void UnloadContent() {
            PlayerShip.UnloadContent();
            Particle.PointParticle = null;
            Tile.UnloadContent();
            Enemy.UnloadContent();
        }
        public override void Update(GameTime gameTime) {
            // TODO remove esc to return
            if (Input.WasKeyJustDown(Keys.Escape)) {
                ScreenManager.RemoveScreen();
                return;
            }
            /* Update all entity positions and handle collisions */
            _entityManager.Update(gameTime);
            _particleManager.Update(gameTime);
            _enemySpawner.Update(_entityManager, _tileMap);
        }
        public override void Draw(SpriteBatch batch) {
            //GraphicsDevice.Clear(Color.Black); // TODO remove
            //foreach (PlayerShip player in _entityManager.Players) {
            var player = _entityManager.Players.First();
            //GraphicsDevice.SetRenderTarget(player.PlayerCamera.Screen);
            batch.Begin(samplerState: SamplerState.PointClamp);
            Color backgroundColor = player.Index switch {
                PlayerIndex.One => new Color(16, 0, 0),
                PlayerIndex.Two => new Color(0, 16, 0),
                PlayerIndex.Three => new Color(0, 0, 16),
                PlayerIndex.Four => new Color(16, 0, 16),
                _ => throw new ArgumentOutOfRangeException()
            };
            batch.Draw(GlobalAssets.Pixel, MyUtils.RectangleF(0, 0, player.PlayerCamera.ScreenSize.X, player.PlayerCamera.ScreenSize.Y), backgroundColor);
            _entityManager.Draw(batch, player.PlayerCamera); /* Draw all entities (inc players, bullets, powerpacks etc) */
            _tileMap.Draw(batch, player.PlayerCamera, _mode == "Editor"); /* Draw tilemap with tile boundaries if in editor mode */
            _particleManager.Draw(batch, player.PlayerCamera);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Player pos: {player.Position}", Vector2.Zero, Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Mouse world pos: {player.PlayerCamera.MouseWorldCoords()}", new Vector2(0f, 32f), Color.White);
            batch.End();
            //}
            /*GraphicsDevice.SetRenderTarget(null);
            batch.Begin();
            foreach (PlayerShip player in _entityManager.Players) {
                batch.Draw(player.PlayerCamera.Screen, player.PlayerCamera.CameraViewport.Bounds, Color.White);
            }
            batch.End();*/
            // TODO enable splitscreen?
        }
    }
}
