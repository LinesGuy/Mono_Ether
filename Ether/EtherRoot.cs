using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Mono_Ether.States;

namespace Mono_Ether.Ether {
    public class EtherRoot : GameState {
        public static EtherRoot Instance { get; private set; }
        public bool paused = false;
        public bool editorMode = false;
        private readonly string MapFileName;
        private Vector2 MapSize;
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        public static GameTime CurrentGameTime;
        public static Hud hud;
        public EtherRoot(GraphicsDevice graphicsDevice, string mapFileName, Vector2 mapSize, string tutorialState = "none") : base(graphicsDevice) {
            MapFileName = mapFileName;
            MapSize = mapSize;
            Tutorial.state = tutorialState;
        }

        public override void Initialize() {
            Instance = this;
            EntityManager.Add(new PlayerShip());
            //EntityManager.Add(new PlayerShip());
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
            Microsoft.Xna.Framework.Audio.SoundEffect.MasterVolume = GameSettings.MasterVolume;
            PauseMenu.Initialize();
            EnemySpawner.enabled = true;
            PowerPackSpawner.enabled = true;
            hud = new Hud();
            Map.LoadFromFile(MapFileName, MapSize);
            BackgroundParticleManager.Populate(Map.WorldSize, 128);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Sounds.Music);
        }

        public override void LoadContent(ContentManager content) {
        }
        public override void UnloadContent() {
            EntityManager.Killall();
            Tutorial.state = "none";
            BackgroundParticleManager.Clear();
            MediaPlayer.Stop();
        }

        public override void Update(GameTime gameTime) {
            if (!GameRoot.Instance.IsActive)
                return;
            CurrentGameTime = gameTime;
            // P to toggle Editor Mode
            if (Input.WasKeyJustDown(Keys.P)) {
                if (editorMode) {
                    EnemySpawner.enabled = true;
                    PowerPackSpawner.enabled = true;
                    editorMode = false;
                } else {
                    EntityManager.Enemies.ForEach(x => x.IsExpired = true);
                    EnemySpawner.enabled = false;
                    EntityManager.PowerPacks.ForEach(x => x.IsExpired = true);
                    PowerPackSpawner.enabled = false;
                    editorMode = true;
                }
            }
            // Esc to toggle pause
            if (Input.WasKeyJustDown(Keys.Escape))
                paused = !paused;

            Map.Update();

            if (!paused) {
                Camera.Update();
                EntityManager.Update();
                EnemySpawner.Update();
                PowerPackSpawner.Update();
                ParticleManager.Update();
                BackgroundParticleManager.Update();
                if (Tutorial.state != "none")
                    Tutorial.Update();
                FloatingTextManager.Update();
                hud.Update();
            } else PauseMenu.Update();
            
        }
        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, samplerState: SamplerState.PointClamp);

            Map.Draw(spriteBatch);
            BackgroundParticleManager.Draw(spriteBatch);
            EntityManager.Draw(spriteBatch);
            ParticleManager.Draw(spriteBatch);

            Vector2 mousePos = Camera.WorldToScreen(Camera.MouseWorldCoords());
            spriteBatch.Draw(Art.Pointer, mousePos - new Vector2(16, 16), Color.White);

            spriteBatch.End();
            // No BlendState.Additive from here
            spriteBatch.Begin();
            if (paused)
                PauseMenu.Draw(spriteBatch);
            if (Tutorial.state != "none")
                Tutorial.Draw(spriteBatch);
            FloatingTextManager.Draw(spriteBatch);
            hud.Draw(spriteBatch);
            if (!GameRoot.Instance.IsActive)
                spriteBatch.DrawString(Fonts.NovaSquare24, "GAME IS UNFOCUSED, CLICK ANYWHERE TO FOCUS WINDOW", GameRoot.ScreenSize / 4f, Color.White);
            spriteBatch.End();
        }
    }
}
