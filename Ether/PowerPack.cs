using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Mono_Ether.Ether
{
    class PowerPack : Entity
    {
        private int timeUntilStart = 60;
        public bool IsActive => timeUntilStart <= 0;
        public string powerType;
        private readonly Random rand = new Random();
        private PowerPack(Texture2D image, Vector2 position)
        {
            this.Image = image;
            Position = position;
            Radius = image.Width / 2f;
            Color = Color.Transparent;
        }

        public override void Update()
        {
            if (timeUntilStart > 0)
            {
                timeUntilStart--;
                Color = Color.White * (1 - timeUntilStart / 60f);
            }
        }

        public void WasPickedUp()
        {
            // TODO: probably some particle animations or something idk lmao
        }
    }
    static class PowerPackSpawner
    {
        static Random _rand = new Random();
        static float _inverseSpawnChance = 60;
        public static bool enabled = true;
        public static void Update()
        {
            if (!enabled)
                return;

            //if (!PlayerShip.Instance.IsDead && )
        }
    }
}
