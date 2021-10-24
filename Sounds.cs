using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    static class Sounds {
        private static readonly Random rand = new Random();

        public static SoundEffect GeomPickup;
        public static SoundEffect ButtonClick;
        public static SoundEffect PlayerDeath;
        public static SoundEffect PowerPackPickup;
        public static SoundEffect PowerPackPickupBad;

        private static SoundEffect[] explosions;
        public static SoundEffect EnemyExplosion { get { return explosions[rand.Next(explosions.Length)]; } }
        private static SoundEffect[] playerShoots;
        public static SoundEffect PlayerShoot { get { return playerShoots[rand.Next(playerShoots.Length)]; } }
        private static SoundEffect[] enemySpawns;
        public static SoundEffect EnemySpawn { get { return enemySpawns[rand.Next(enemySpawns.Length)]; } }

        public static Song Music;
        public static void Load(ContentManager content) {
            ButtonClick = content.Load<SoundEffect>("Samples/click");
            GeomPickup = content.Load<SoundEffect>("Samples/Gameplay/geomPickup");
            PlayerDeath = content.Load<SoundEffect>("Samples/Gameplay/player_death");
            PowerPackPickup = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickup");
            PowerPackPickupBad = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickupBad");

            explosions = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Samples/Gameplay/explosions/explosion-0" + x)).ToArray();
            playerShoots = Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Samples/Gameplay/shoot/shoot-0" + x)).ToArray();
            enemySpawns = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Samples/Gameplay/spawn/spawn-0" + x)).ToArray();

            Music = content.Load<Song>("Tracks/Music");
        }
    }
}