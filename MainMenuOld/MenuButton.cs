using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Mono_Ether.MainMenuOld
{
    public class MenuButton
    {
        public Vector2 Position;
        private Texture2D image;
        private Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        public bool CursorInButton()
        {
            return new RectangleF(Position.X - Size.X / 2, Position.Y - Size.Y / 2, Size.X, Size.Y).Contains(
                Input.Mouse.Position);
        }
        public MenuButton(Vector2 position, Texture2D image)
        {
            this.Position = position;
            this.image = image;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Position, null, Color.White, 0f, Size / 2f, 1f, 0, 0);
        }
    }
}
