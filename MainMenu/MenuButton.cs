using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.MainMenu
{
    public class MenuButton
    {
        private Vector2 position;
        private string text;
        private Texture2D image;
        public bool Visible;
        public Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }
        public MenuButton(Vector2 position, string text)
        {
            this.position = position;
            this.text = text;
            this.Visible = false;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, position, null, Color.White, 0f, Size / 2f, 1f, 0, 0);
        }
    }
}
