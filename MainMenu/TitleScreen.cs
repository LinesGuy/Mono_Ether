using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.MainMenu
{
    public class TitleScreen : States.GameState
    {
        private int foobar = 0;
        public TitleScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }
        public override void Initialize()
        {

        }
        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }
        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime)
        {
            foobar++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(Art.DebugFont, $"xd + {foobar}", Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
