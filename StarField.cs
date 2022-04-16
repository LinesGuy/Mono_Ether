using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public class StarField {
        private static readonly Random Random = new Random();
        private class Star {
            private readonly Vector2 _position;
            private readonly float _distance;
            public Star(Vector2 position, float distance) {
                _position = position;
                _distance = distance;
            }
            public void Draw(SpriteBatch batch, Camera camera) {
                // Parallax
                var offsetPos = (camera.WorldToScreen(_position) - camera.ScreenSize / 2f) * _distance + camera.ScreenSize / 2f;
                MyUtils.DrawLine(batch, offsetPos, offsetPos + (camera.LastPosition - camera.Position).Rotate(camera.Orientation) * _distance * camera.Zoom, Color.LightGray, 1f);
            }
        }
        private readonly List<Star> Stars = new List<Star>();
        public void Populate(Vector2 size, int amount) {
            // Create the stars based on the size of the map and the number of stars to add
            for (var i = 0; i < amount; i++)
                Stars.Add(new Star(new Vector2(Random.NextFloat(0f, size.X), Random.NextFloat(0f, size.Y)), Random.NextFloat(0.5f, 1.5f)));
        }
        public void Draw(SpriteBatch batch, Camera camera) {
            foreach (var star in Stars)
                star.Draw(batch, camera);
        }
    }
}
