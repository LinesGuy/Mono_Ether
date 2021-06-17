using Microsoft.Xna.Framework;
using System;

namespace Mono_Ether.Ether
{
    class Dummy : Entity
    {
        private Random rand = new Random();
        private float rotationVelocity;

        public Dummy()
        {
            Image = Art.Default;
            Position = new Vector2(rand.NextFloat(-640, 640), rand.NextFloat(-360, 360));
            Orientation = rand.NextFloat(0, 2 * (float)Math.PI);
            Radius = 8;
            rotationVelocity = 0f;
        }

        public override void Update()
        {
            rotationVelocity = Math.Min(Math.Max(rotationVelocity + rand.NextFloat(-0.001f, 0.001f), -0.1f), 0.1f);
            Orientation += rotationVelocity;
        }
    }
}
