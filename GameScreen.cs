using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Media;

namespace Mono_Ether {
    public enum GameMode { Playing, Paused, Editor }
    public enum Level { Debug, Level1, Level2, Level3, Secret, Tutorial }
    public class GameScreen : GameState {
        public static Texture2D GameCursor;
        private float _cursorRotation;
        public static GameScreen Instance;
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly ParticleManager _particleManager = new ParticleManager();
        private readonly EnemySpawner _enemySpawner = new EnemySpawner();
        private readonly PowerPackSpawner _powerPackSpawner = new PowerPackSpawner(); 
        private readonly PauseWindow _pauseWindow = new PauseWindow();
        private readonly StarField _starField = new StarField();
        private readonly TileMap _tileMap;
        private TimeSpan _timeSinceTransition;
        private string _state = "FadeIn";
        private Hud _hud;
        public Level CurrentLevel;
        public GameMode Mode = GameMode.Playing;
        private Song _gameMusic;
        public GameScreen(GraphicsDevice graphicsDevice, Level level) : base(graphicsDevice) {
            CurrentLevel = level;
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
            foreach (var player in EntityManager.Instance.Players) {
                EntityManager.Instance.Add(Drone.CreateShooter(player.Index));
                EntityManager.Instance.Add(Drone.CreateCollector(player.Index));
            }
            /* If level is level one, two or three, summon a boss and enable the boss bar, and move player to top left */
            if (CurrentLevel == Level.Level1 || CurrentLevel == Level.Level2 || CurrentLevel == Level.Level3) {
                _hud = new Hud(true);
                if (CurrentLevel == Level.Level1) _entityManager.Add(new BossOne(_tileMap.WorldSize / 2f));
                if (CurrentLevel == Level.Level2) _entityManager.Add(new BossTwo(_tileMap.WorldSize / 2f));
                if (CurrentLevel == Level.Level3) _entityManager.Add(new BossThree(_tileMap.WorldSize / 2f));
                foreach (var player in _entityManager.Players)
                    player.Position = new Vector2(128f, 128f);
            } else
                _hud = new Hud();
            /* Move all player cameras to player */
            foreach (var player in _entityManager.Players)
            {
                player.PlayerCamera.Position = player.Position;
            }
            /* Play music */
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_gameMusic);
        }
        public override void Suspend() {

        }
        public override void Resume() {

        }
        public override void LoadContent(ContentManager content) {
            GameCursor = content.Load<Texture2D>("Textures/GameScreen/GameCursor");
            _gameMusic = content.Load<Song>("Songs/GameScreen");
            PlayerShip.LoadContent(content);
            Bullet.LoadContent(content);
            Starburst.LoadContent(content);
            Particle.LoadContent(content);
            Tile.LoadContent(content);
            Enemy.LoadContent(content);
            Drone.LoadContent(content);
            Geom.LoadContent(content);
            PowerPack.LoadContent(content);
        }
        public override void UnloadContent() {
            GameCursor = null;
            _gameMusic = null;
            PlayerShip.UnloadContent();
            Bullet.UnloadContent();
            Starburst.UnloadContent();
            Particle.UnloadContent();
            Tile.UnloadContent();
            Enemy.UnloadContent();
            Drone.UnloadContent();
            Geom.UnloadContent();
            PowerPack.UnloadContent();
        }
        private void SetState(string state) {
            _state = state;
            _timeSinceTransition = TimeSpan.Zero;
        }
        public override void Update(GameTime gameTime) {
            if (!GameRoot.Instance.IsActive)
                return;
            _timeSinceTransition += gameTime.ElapsedGameTime;
            switch (_state) {
                case "FadeIn":
                    if (_timeSinceTransition > TimeSpan.FromSeconds(0.5)) SetState("normal");
                    // TODO camera zoom out
                    // TODO player invincibility ring remove
                    break;
                case "Normal":
                    break;
                case "Win":
                    if (_timeSinceTransition > TimeSpan.FromSeconds(3)) ScreenManager.RemoveScreen();
                    break;
                case "Lose":
                    if (_timeSinceTransition > TimeSpan.FromSeconds(3)) ScreenManager.RemoveScreen();
                    break;
            }
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

            if (Input.WasKeyJustDown(Keys.P)) {
                switch (Mode) {
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
            _powerPackSpawner.Update(gameTime);
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
            _hud.Draw(batch);
            player.DrawHud(batch);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Player pos: {player.Position}", Vector2.Zero, Color.White);
            batch.DrawString(GlobalAssets.NovaSquare24, $"Mouse world pos: {player.PlayerCamera.MouseWorldCoords()}", new Vector2(0f, 32f), Color.White);
            if (_pauseWindow.Visible)
                _pauseWindow.Draw(batch);
            /* Draw cursor */
            batch.Draw(GameCursor, Input.Mouse.Position.ToVector2(), null, Color.White, _cursorRotation,
                GameCursor.Size() / 2f, 1f, 0, 0);
            if (!GameRoot.Instance.IsActive)
                batch.DrawString(GlobalAssets.NovaSquare24, "GAME IS UNFOCUSED, CLICK ANYWHERE TO FOCUS WINDOW", GameSettings.ScreenSize / 4f, Color.White);
            batch.End();
            //}
            /*GraphicsDevice.SetRenderTarget(null);
            batch.Begin();
            foreach (PlayerShip player in _entityManager.Players) {
                batch.Draw(player.PlayerCamera.Screen, player.PlayerCamera.CameraViewport.Bounds, Color.White);
            }
            batch.End();*/
            // TODO enable splitscreen?
            // TODO fade in on load
            // TODO win/lose screen
        }
    }
}
