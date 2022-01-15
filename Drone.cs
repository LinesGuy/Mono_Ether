using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public enum DroneType { Shooter, Collector, Defender }

    public class Drone : Entity
    {
        private static Texture2D _shooterTexture;
        private static Texture2D _collectorTexture2D;
        private static Texture2D _defenderTexture;
        public DroneType Type;
        private readonly List<IEnumerator<int>> _behaviours = new List<IEnumerator<int>>();
        public PlayerIndex OwnerPlayerIndex;
        private static readonly Random Random = new Random();
        public Drone(PlayerIndex ownerPlayerIndex, DroneType type) {
            OwnerPlayerIndex = ownerPlayerIndex;
            Type = type;
            Position = EntityManager.Instance.Players[(int)ownerPlayerIndex].Position;
            switch (type) {
                case DroneType.Collector:
                    Image = _collectorTexture2D;
                    break;
                case DroneType.Shooter:
                    Image = _shooterTexture;
                    break;
                case DroneType.Defender:
                    Image = _defenderTexture;
                    break;
            }
            Radius = Image.Width / 2f;
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
