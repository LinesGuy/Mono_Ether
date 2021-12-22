using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public class EntityManager
    {
        public List<Entity> Entities = new List<Entity>();
        public List<PlayerShip> Players = new List<PlayerShip>();
        public static List<Enemy> Enemies = new List<Enemy>();
        public EntityManager()
        {

        }
        public void Update(GameTime gameTime)
        {
            foreach (Entity entity in Entities)
                entity.Update(gameTime);
        }
        public void Draw(SpriteBatch batch, Camera camera)
        {
            foreach (Entity entity in Entities)
                entity.Draw(batch, camera);
        }
    }
}
