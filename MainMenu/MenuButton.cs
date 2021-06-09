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

        public MenuButton(Vector2 _position, string _text)
        {
            this.position = _position;
            this.text = _text;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
