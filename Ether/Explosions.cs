using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether.Ether {
    /*
    public static class ExplosionManager
    {
        private static List<Explosion> Explosions;

        public static void Add(Explosion explosion)
        {
            Explosions.Add(explosion);
        }
        public static void Initialize()
        {
            Explosions = new List<Explosion>();
        }
        public static void Update()
        {
            foreach (Explosion e in Explosions)
            {
                e.Update();
            }
            Explosions = Explosions.Where(e => !e.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Explosion e in Explosions)
            {
                e.Draw(spriteBatch);
            }
        }
    } 
    public class Explosion
    {
        protected Texture2D[] Images;
        private int _age = 0;
        private int _lifespan = 8;
        private Color _color;
        private Vector2 _pos;
        private float _radius;
        public bool IsExpired;

        public Explosion(Vector2 pos)
        {
            Images = GlobalAssets.ExplosionSmall;
            _radius = Images[0].Width;
            _color = new Color(128, 128, 128);
            _pos = pos;
        }
        public void Update()
        {
            _age++;
            if (_age == _lifespan)
                IsExpired = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 screenPos = Camera.WorldToScreen(_pos);
            spriteBatch.Draw(Images[_age / 2], screenPos, null, _color, Camera.Orientation, new Vector2(_radius) / 2f, Camera.Zoom * 2f, 0, 0);
        }
    }
    */
}