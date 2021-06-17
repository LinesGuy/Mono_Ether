using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Sprite = MonoGame.Extended.Sprites.Sprite;

namespace Mono_Ether.Ether
{
    public class ParticleManager<T>
    {
        // This delegate will be called for each particle.
        private Action<Particle> updateParticle;
        private CircularParticleArray particleList;

        public ParticleManager(int capacity, Action<Particle> updateParticle)
        {
            this.updateParticle = updateParticle;
            particleList = new CircularParticleArray(capacity);
            
            // Populate the list with empty particle objects, for reuse.
            for (int i = 0; i < capacity; i++)
                particleList[i] = new Particle();
        }
        
        public class Particle
        {
            public Texture2D Texture;
            public Vector2 Position;
            public float Orientation;

            public Vector2 Scale = Vector2.One;

            public Color Tint;
            public float Duration;
            public float PercentLife = 1f;
            public T State;
        }

        public class CircularParticleArray
        {
            private int start;

            public int Start
            {
                get { return start; }
                set { start = value % list.Length; }
            }

            public int Count { get; set; }

            public int Capacity  {  get  { return list.Length; }  }
            private Particle[] list;

            public CircularParticleArray(int capacity)
            {
                list = new Particle[capacity];
            }

            public Particle this[int i]
            {
                get { return list[(start + i) % list.Length]; }
                set { list[(start + i) % list.Length] = value; }
            }
        }

        public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale,
            T state, float theta = 0)
        {
            CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
        }
        public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale,
            T state, float theta = 0)
        {
            Particle particle;
            if (particleList.Count == particleList.Capacity)
            {
                // If the list is full, overwrite the oldest particle, and rotate the cirular list
                particle = particleList[0];
                particleList.Start++;
            }
            else
            {
                particle = particleList[particleList.Count];
                particleList.Count++;
            }
            
            // Create the particle
            particle.Texture = texture;
            particle.Position = position;
            particle.Tint = tint;

            particle.Duration = duration;
            particle.PercentLife = 1f;
            particle.Scale = scale;
            particle.Orientation = theta;
            particle.State = state;
        }

        public void Update()
        {
            int removalCount = 0;
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];
                updateParticle(particle);
                particle.PercentLife -= 1f / particle.Duration;
                
                // Sift deleted particles to the end of the list
                Swap(particleList, i - removalCount, i);
                
                // If the particle has expired, delete this praticle
                if (particle.PercentLife < 0)
                    removalCount++;
            }

            particleList.Count -= removalCount;
        }

        private static void Swap(CircularParticleArray list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];

                Vector2 origin = new Vector2(particle.Texture.Width / 2, particle.Texture.Height / 2);
                var screenPos = Camera.world_to_screen_pos(particle.Position);
                var scale = Camera.zoom * particle.Scale;
                spriteBatch.Draw(particle.Texture, screenPos, null, particle.Tint, particle.Orientation, origin, scale, 0, 0);
            }
        }
    }
}