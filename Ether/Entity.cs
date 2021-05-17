using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether.Ether
{
    abstract class Entity
    {
        protected Texture2D image = Art.Default;  // Tint of image
        protected Color color = Color.White;

        public Vector2 Position, Velocity;
        public float Orientation;
        public float Radius = 20;  // Used for circular collision detection
        public bool IsExpired;  // If true, entity will be removed on next update

        public Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Vector2 screen_pos = Camera.world_to_screen_pos(Position);
            //spriteBatch.Draw(image, screen_pos, null, color, Orientation + Camera.orientation, Size / 2f, Camera.zoom, 0, 0);
            spriteBatch.Draw(image, screen_pos, null, color, Orientation, Size / 2f, Camera.zoom, 0, 0);
        }
    }
}
