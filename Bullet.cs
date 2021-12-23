using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class Bullet : Entity
    {
        public static Texture2D BulletTexture;
        public static Texture2D BulletGlowTexture;
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        public void LoadContent(ContentManager content)
        {
            BulletTexture = content.Load<Texture2D>("Textures/GameScreen/Bullet");
            BulletGlowTexture = content.Load<Texture2D>("Textures/GameScreen/BulletGlow");
        }
        public void UnloadContent()
        {
            BulletTexture = null;
            BulletGlowTexture = null;
        }
    }
}
