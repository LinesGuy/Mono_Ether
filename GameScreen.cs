using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Mono_Ether {
    public enum GameMode { Playing, Paused, Editor }
    public enum Level { Debug, Level1, Level2, Level3, Secret, Tutorial }
    public class GameScreen : GameState {
        private static Texture2D _gameCursor;
        private float _cursorRotation;
        public static GameScreen Instance;
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly ParticleManager _particleManager = new ParticleManager();
        private readonly EnemySpawner _enemySpawner = new EnemySpawner();
        private readonly PauseWindow _pauseWindow = new PauseWindow();
        private readonly StarField _starField = new StarField();
        private readonly TileMap _tileMap;
        public GameMode Mode = GameMode.Playing;
        public GameScreen(GraphicsDevice graphicsDevice, Level level) : base(graphicsDevice) {
            /* Load tile map data from filename */
            _tileMap = new TileMap(level);
        }
        public override void Initialize() {
            Instance = this;
            ParticleManager.Instance = _particleManager;
            _starField.Populate(_tileMap.WorldSize, 500);
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
            /* Give each player a shooter drone */
            foreach (var player in EntityManager.Instance.Players)
            {
                EntityManager.Instance.Add(Drone.CreateShooter(player.Index));
                EntityManager.Instance.Add(Drone.CreateCollector(player.Index));
            }
                
        }
        public override void Suspend() {

        }
        public override void Resume() {

        }
        public override void LoadContent(ContentManager content) {
            _gameCursor = content.Load<Texture2D>("Textures/GameScreen/GameCursor");
            PlayerShip.LoadContent(content);
            Bullet.LoadContent(content);
            Starburst.LoadContent(content);
            Particle.LoadContent(content);
            Tile.LoadContent(content);
            Enemy.LoadContent(content);
            Drone.LoadContent(content);
            Geom.LoadContent(content);
        }
        public override void UnloadContent() {
            _gameCursor = null;
            PlayerShip.UnloadContent();
            Bullet.UnloadContent();
            Starburst.UnloadContent();
            Particle.UnloadContent();
            Tile.UnloadContent();
            Enemy.UnloadContent();
            Drone.UnloadContent();
            Geom.UnloadContent();
        }
        public override void Update(GameTime gameTime) {
            /* Update cursor rotation */
            _cursorRotation += 0.05f;
            if (Input.WasKeyJustDown(Keys.Escape)) {
                switch (Mode) {
                    case GameMode.Playing:
                        Mode = GameMode.Paused;
                        _pauseWindow.Pause();
                        break;
                    case GameMode.Paused:
                        Mode = GameMode.Playing;
                        _pauseWindow.UnPause();
                        break;
                }
            }

            if (Input.WasKeyJustDown(Keys.P))
            {
                switch (Mode)
                {
                    case GameMode.Playing:
                        Mode = GameMode.Editor;
                        EnemySpawner.Enabled = false;
                        // disable power pack spawner TODO
                        _entityManager.Enemies.ForEach(e => e.IsExpired = true);
                        // Clear powerpacks TODO
                        break;
                    case GameMode.Editor:
                        Mode = GameMode.Playing;
                        EnemySpawner.Enabled = true;
                        // enable power pack spanwer TODO
                        break;
                }
            }
            _pauseWindow.Update(gameTime);
            if (Mode == GameMode.Paused)
                return;
            /* Update all entity positions and handle collisions */
            _entityManager.Update(gameTime);
            _particleManager.Update(gameTime);
            _enemySpawner.Update(_entityManager, _tileMap);
            _tileMap.Update(Mode == GameMode.Editor);
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
            _starField.Draw(batch, player.PlayerCamera);
            _tileMap.Draw(batch, player.PlayerCamera, Mode == GameMode.Editor); /* Draw tilemap with tile boundaries if in editor mode */
            _particleManager.Draw(batch, player.PlayerCamera);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Player pos: {player.Position}", Vector2.Zero, Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Mouse world pos: {player.PlayerCamera.MouseWorldCoords()}", new Vector2(0f, 32f), Color.White);
            if (_pauseWindow.Visible)
                _pauseWindow.Draw(batch);
            /* Draw cursor */
            batch.Draw(_gameCursor, Input.Mouse.Position.ToVector2(), null, Color.White, _cursorRotation,
                _gameCursor.Size() / 2f, 1f, 0, 0);
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
