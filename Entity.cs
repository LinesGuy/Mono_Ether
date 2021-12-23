using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public abstract class Entity
    {
        protected Texture2D Image = GlobalAssets.Default;
        private readonly Color _color = Color.White;
        public Vector2 Position = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;
        public float Orientation = 0f;
        public readonly float Radius;
        public bool IsExpired = false;
        private Vector2 Size => Image.Size();
        public abstract void Update(GameTime gameTime);
        public virtual void Draw(SpriteBatch batch, Camera camera) {
            Vector2 screenPos = camera.WorldToScreen(Position);
            batch.Draw(Image, screenPos, null, _color, Orientation + camera.Orientation, Size / 2f, camera.Zoom, 0, 0);
        }
    }
}
