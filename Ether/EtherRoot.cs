using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mono_Ether.Ether
{
    public class EtherRoot : GameState
    {
        //private SpriteBatch spriteBatch;
        public EtherRoot(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
            EntityManager.Add(PlayerShip.Instance);
            //for (int i = 0; i < 10; i++)
            //EntityManager.Add(new Dummy());
        }

        public override void LoadContent(ContentManager content)
        {
            //Art.Load(content);
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            EntityManager.Update();
            Camera.Update();
            EnemySpawner.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(spriteBatch);
            Vector2 mouse_pos = Camera.world_to_screen_pos(Camera.mouse_world_coords());
            //spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
            spriteBatch.Draw(Art.Pointer, mouse_pos - new Vector2(16, 16), Color.White);

            // Debug texts
            spriteBatch.DrawString(Art.DebugFont, "Player pos: " + PlayerShip.Instance.Position.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Camera pos: " + Camera.cameraPosition.ToString(), new Vector2(0, 30), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor pos: " + Camera.mouse_world_coords().ToString(), new Vector2(0, 60), Color.White);
            spriteBatch.End();
        }
    }
}
