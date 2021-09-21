using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether.Ether {
    public class EtherRoot : GameState {
        public static EtherRoot Instance { get; private set; }
        public bool paused = false;
        public bool editorMode = false;
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        public static GameTime CurrentGameTime;
        public EtherRoot(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }

        public override void Initialize() {
            Instance = this;
            EntityManager.Add(PlayerShip.Instance);
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
            Microsoft.Xna.Framework.Audio.SoundEffect.MasterVolume = 0.1f;
            PauseMenu.Initialize();
        }

        public override void LoadContent(ContentManager content) {
            Map.LoadFromFile("debugMap.txt", new Vector2(64, 64));
            BackgroundParticleManager.Populate(Map._size * Map.cellSize, 256);
        }

        public override void UnloadContent() {
        }

        public override void Update(GameTime gameTime) {
            CurrentGameTime = gameTime;
            // P to toggle Editor Mode
            if (Input.Keyboard.WasKeyJustDown(Keys.P)) {
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
            if (Input.Keyboard.WasKeyJustDown(Keys.Escape))
                paused = !paused;

            
            Map.Update();
            

            if (paused) {
                PauseMenu.Update();
            } else {
                Camera.Update();
                EntityManager.Update();
                EnemySpawner.Update();
                PowerPackSpawner.Update();
                ParticleManager.Update();
                BackgroundParticleManager.Update();
            }
        }
        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, samplerState: SamplerState.PointClamp);

            Map.Draw(spriteBatch);
            BackgroundParticleManager.Draw(spriteBatch);
            EntityManager.Draw(spriteBatch);
            ParticleManager.Draw(spriteBatch);

            Vector2 mousePos = Camera.world_to_screen_pos(Camera.mouse_world_coords());
            spriteBatch.Draw(Art.Pointer, mousePos - new Vector2(16, 16), Color.White);

            spriteBatch.End();
            // No BlendState.Additive from here
            spriteBatch.Begin();
            if (paused)
                PauseMenu.Draw(spriteBatch);
            Hud.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
