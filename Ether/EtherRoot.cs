using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether.Ether
{
    public class EtherRoot : GameState
    {
        private bool paused;
        //public static Map MyMap;
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        public static GameTime CurrentGameTime;
        public EtherRoot(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
            //MyMap = new Map();
            EntityManager.Add(PlayerShip.Instance);
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
        }

        public override void LoadContent(ContentManager content)
        {
            //Art.Load(content);
            //Tiles.Content = content;

            //Map.LoadFromFile("susMap.txt", new Vector2(12, 12));
            Map.LoadFromFile("susMap2.txt", new Vector2(3, 2));
            //MyMap.LoadFromFile("bigMap.txt", new Vector2(256, 256));
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            CurrentGameTime = gameTime;
            // pause menu.update thingy here instead of this
            if (Input.Keyboard.WasKeyJustDown(Keys.P))
            {
                paused = !paused;
            }
            
            Camera.Update();
            
            if (!paused)
            {
                EntityManager.Update();
                
                EnemySpawner.Update();
                ParticleManager.Update();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, samplerState: SamplerState.PointClamp);
            
            Map.Draw(spriteBatch);

            EntityManager.Draw(spriteBatch);
            
            ParticleManager.Draw(spriteBatch);
            
            Vector2 mousePos = Camera.world_to_screen_pos(Camera.mouse_world_coords());
            //spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
            spriteBatch.Draw(Art.Pointer, mousePos - new Vector2(16, 16), Color.White);

            // Debug texts
            spriteBatch.DrawString(Art.DebugFont, "Player pos: " + PlayerShip.Instance.Position.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Camera pos: " + Camera.CameraPosition.ToString(), new Vector2(0, 30), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor world pos: " + Camera.mouse_world_coords().ToString(), new Vector2(0, 60), Color.White);
            if (paused) 
                spriteBatch.DrawString(Art.DebugFont, "GAME PAUSED", new Vector2(0, 90), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "TILE: " + Map.GetTileFromMap(Map.WorldtoMap(Camera.mouse_world_coords())), new Vector2(0, 120), Color.White);
            spriteBatch.End();
        }
    }
}
