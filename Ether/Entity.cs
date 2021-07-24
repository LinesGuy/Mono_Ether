using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether
{
    internal abstract class Entity
    {
        protected Texture2D Image = Art.Default;  // Tint of image
        protected Color Color = Color.White;

        public Vector2 Position;
        protected Vector2 Velocity;
        protected float Orientation;
        public float Radius = 20;  // Used for circular collision detection
        public bool IsExpired;  // If true, entity will be removed on next update

        private Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var screenPos = Camera.world_to_screen_pos(Position);
            //spriteBatch.Draw(image, screen_pos, null, color, Orientation + Camera.orientation, Size / 2f, Camera.zoom, 0, 0);
            spriteBatch.Draw(Image, screenPos, null, Color, Orientation, Size / 2f, Camera.Zoom, 0, 0);
        }

        public void HandleTileCollisions()
        {
            
        }
    }
}
