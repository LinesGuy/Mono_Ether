using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Mono_Ether.Ether {
    /*
    public class DoomRoot : GameState {
        public static DoomRoot Instance { get; private set; }
        public bool paused = false;
        private readonly string MapFileName;
        private Vector2 MapSize;
        public static GameTime CurrentGameTime;
        public DoomRoot(GraphicsDevice graphicsDevice, string mapFileName) : base(graphicsDevice) {
            MapFileName = mapFileName;
            switch (mapFileName) {
                case "Secret.txt":
                    MapSize = new Vector2(32, 32);
                    break;
            }
        }
        public override void Initialize() {
            Instance = this;
            // ADD PLAYER ONE
            EntityManager.Add(new PlayerShip());
            // LOAD MAP, SET PLAYER POS
            Hud.Reset();
            Hud.bossBarEnabled = true; // TODO
            Microsoft.Xna.Framework.Audio.SoundEffect.MasterVolume = GameSettings.MasterVolume;
            PauseMenu.Initialize();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Sounds.Music);
            EntityManager.Player1.DoomMovement = true;
        }
        public override void LoadContent(ContentManager content) {
        }
        public override void UnloadContent() {
            Hud.Reset();
            MediaPlayer.Stop();
            PauseMenu.state = "hidden";
            EntityManager.Player1.DoomMovement = false;
        }
        public override void Update(GameTime gameTime) {
            if (!GameRoot.Instance.IsActive)
                return;
            CurrentGameTime = gameTime;
            // Esc to toggle pause
            if (Input.WasKeyJustDown(Keys.Escape)) {
                paused = !paused;
                if (paused)
                    PauseMenu.SlideIn();
                else
                    PauseMenu.SlideOut();
            }

            Map.Update();
            if (!paused) {
                EntityManager.Player1.Update();
            }
            PauseMenu.Update();

        }
        public override void Draw(SpriteBatch spriteBatch) {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            Doom.Draw(spriteBatch);
            spriteBatch.DrawString(Fonts.NovaSquare24, "FIND THE SUSSY IMPOSTOR TO CONTINUE", GameRoot.ScreenSize / 6f, Color.White);

            Vector2 mousePos = Camera.WorldToScreen(Camera.MouseWorldCoords());
            spriteBatch.Draw(GlobalAssets.Pointer, mousePos - new Vector2(16, 16), Color.White);
            if (PauseMenu.state != "hidden")
                PauseMenu.Draw(spriteBatch);
            if (!GameRoot.Instance.IsActive)
                spriteBatch.DrawString(Fonts.NovaSquare24, "GAME IS UNFOCUSED, CLICK ANYWHERE TO FOCUS WINDOW", GameRoot.ScreenSize / 4f, Color.White);
            spriteBatch.End();
        }
    }
    */
}
