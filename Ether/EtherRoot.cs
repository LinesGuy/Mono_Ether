using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Mono_Ether.Ether
{
    public class EtherRoot : GameState
    {
        private bool paused = false;

        private Map map;
        public EtherRoot(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
            map = new Map();
            EntityManager.Add(PlayerShip.Instance);
        }

        public override void LoadContent(ContentManager content)
        {
            //Art.Load(content);
            Tiles.Content = content;
            
            map.Generate(new int[,]
            {
                {0,0,0,1,1,1,1,1,1},
                {0,0,1,2,2,2,2,2,2},
                {0,1,2,2,3,3,3,3,3},
                {1,2,2,3,3,3,3,3,3},
                {2,2,3,3,3,3,3,3,3},
            },64);
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            // pause menu.update thingy here instead of this
            if (Input.keyboardState.WasKeyJustDown(Keys.P))
            {
                paused = !paused;
            }

            if (!paused)
            {
                EntityManager.Update();
                Camera.Update();
                EnemySpawner.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, samplerState: SamplerState.PointClamp);
            
            map.Draw(spriteBatch);
            
            EntityManager.Draw(spriteBatch);
            Vector2 mousePos = Camera.world_to_screen_pos(Camera.mouse_world_coords());
            //spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
            spriteBatch.Draw(Art.Pointer, mousePos - new Vector2(16, 16), Color.White);

            // Debug texts
            spriteBatch.DrawString(Art.DebugFont, "Player pos: " + PlayerShip.Instance.Position.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Camera pos: " + Camera.cameraPosition.ToString(), new Vector2(0, 30), Color.White);
            spriteBatch.DrawString(Art.DebugFont, "Cursor pos: " + Camera.mouse_world_coords().ToString(), new Vector2(0, 60), Color.White);
            spriteBatch.End();
        }
    }
}
