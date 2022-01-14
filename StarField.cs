using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public class StarField {
        private static readonly Random Random = new Random();
        private class Star
        {
            private readonly Vector2 _position;
            private readonly float _distance;
            private readonly Color _color;
            public Star(Vector2 position, float distance)
            {
                _position = position;
                _distance = distance;
            }
            public void Update() {
                
            }
            public void Draw(SpriteBatch batch, Camera camera)
            {
                var offsetPos = (camera.WorldToScreen(_position) - camera.ScreenSize / 2f) * _distance + camera.ScreenSize / 2f;
                MyUtils.DrawLine(batch, offsetPos, offsetPos + (camera.LastPosition - camera.Position) * _distance, Color.Gray, 2f);
                // camera.WorldToScreen(_position + (camera.LastPosition - camera.Position))
            }
        }
        private readonly List<Star> Stars = new List<Star>();
        public void Populate(Vector2 size, int amount) {
            for (var i = 0; i < amount; i++)
                Stars.Add(new Star(new Vector2(Random.NextFloat(0f, size.X), Random.NextFloat(0f, size.Y)), Random.NextFloat(0.5f, 1.5f)));
        }
        public void Update() {
            foreach (var star in Stars)
                star.Update();
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (var star in Stars)
                star.Draw(batch, camera);
        }
    }
}
