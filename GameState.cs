using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public abstract class GameState {
        // Abstract GameState class that all other gamestates (e.g main menu, game screen) will use.
        protected readonly GraphicsDevice GraphicsDevice;
        protected GameState(GraphicsDevice graphicsDevice) {
            GraphicsDevice = graphicsDevice;
        }
        public abstract void Initialize();
        public abstract void Suspend();
        public abstract void Resume();
        public abstract void LoadContent(ContentManager content);
        public abstract void UnloadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
