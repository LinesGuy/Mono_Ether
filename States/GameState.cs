using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.States
{
    public abstract class GameState : IGameState
    {
        protected GraphicsDevice _graphicsDevice;
        public GameState(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }
        public abstract void Initialize();
        public abstract void LoadContent(ContentManager content);
        public abstract void UnloadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
