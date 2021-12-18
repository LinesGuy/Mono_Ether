using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public abstract class GameState {
        protected readonly GraphicsDevice GraphicsDevice;
        protected GameState(GraphicsDevice graphicsDevice) {
            GraphicsDevice = graphicsDevice;
        }
        public abstract void Initialize();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void LoadContent(ContentManager content);
        public abstract void UnloadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
