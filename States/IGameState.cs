using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.States
{
    interface IGameState
    {
        // Initialize game settings
        void Initialize();

        // Load all content
        void LoadContent(ContentManager content);

        // Unload content
        void UnloadContent();

        // Update game
        void Update(GameTime gameTime);

        // Draw game
        void Draw(SpriteBatch spriteBatch);
    }
}
