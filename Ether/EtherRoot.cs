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
        private Map map;
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        public static GameTime CurrentGameTime;
        public EtherRoot(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
            map = new Map();
            EntityManager.Add(PlayerShip.Instance);
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
        }

        public override void LoadContent(ContentManager content)
        {
            //Art.Load(content);
            Tiles.Content = content;
            
            map.Generate(new[,]
            {
                {0,0,0,0,1,1,1,1,1,0,0,0},
                {0,0,0,1,3,3,3,3,3,1,0,0},
                {0,1,1,2,3,3,3,2,2,2,1,0},
                {1,3,3,2,3,3,2,3,3,3,3,1},
                {2,3,3,2,3,3,2,3,3,3,3,2},
                {2,3,3,2,3,3,3,2,2,2,2,0},
                {2,3,3,2,3,3,3,3,3,2,0,0},
                {2,3,3,2,3,3,3,3,3,2,0,0},
                {0,2,2,2,3,3,3,3,3,2,0,0},
                {0,0,0,2,3,2,2,2,3,2,0,0},
                {0,0,0,2,3,2,0,2,3,2,0,0},
                {0,0,0,2,2,2,0,2,2,2,0,0},
            },64);
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

            if (!paused)
            {
                EntityManager.Update();
                Camera.Update();
                EnemySpawner.Update();
                ParticleManager.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, samplerState: SamplerState.PointClamp);
            
            map.Draw(spriteBatch);
            
            EntityManager.Draw(spriteBatch);
            
            ParticleManager.Draw(spriteBatch);
            
            Vector2 mousePos = Camera.world_to_screen_pos(Camera.mouse_world_coords());
            //spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
            spriteBatch.Draw(Art.Pointer, mousePos - new Vector2(16, 16), Color.White);

            // Debug texts
            spriteBatch.DrawString(Art.DebugFont, "Player pos: " + PlayerShip.Instance.Position.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Camera pos: " + Camera.CameraPosition.ToString(), new Vector2(0, 30), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor pos: " + Camera.mouse_world_coords().ToString(), new Vector2(0, 60), Color.White);
            spriteBatch.End();
        }
    }
}
