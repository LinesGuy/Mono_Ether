using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono_Ether {
    static class Art {
        private static readonly Random rand = new Random();

        public static SpriteFont Arial24;
        public static SpriteFont NovaSquare24;
        public static SpriteFont NovaSquare48;

        public static Texture2D Glow;
        public static Texture2D Geom;
        public static Texture2D Heart;
        public static Texture2D Pixel;
        public static Texture2D Player;
        public static Texture2D Bullet;
        public static Texture2D PauseBg;
        public static Texture2D Default;
        public static Texture2D TileSus;
        public static Texture2D Pointer;
        public static Texture2D TileDirt;
        public static Texture2D SnakeBody;
        public static Texture2D PauseExit;
        public static Texture2D SnakeHead;
        public static Texture2D StarBurst;
        public static Texture2D TileGrass;
        public static Texture2D TileStone;
        public static Texture2D BlueSeeker;
        public static Texture2D PinkSeeker;
        public static Texture2D GreenSeeker;
        public static Texture2D PauseResume;
        public static Texture2D CollisionUp;
        public static Texture2D WelcomeText;
        public static Texture2D PinkWanderer;
        public static Texture2D LineParticle;
        public static Texture2D CollisionLeft;
        public static Texture2D CollisionDown;
        public static Texture2D BackAndForther;
        public static Texture2D PurpleWanderer;
        public static Texture2D CollisionRight;
        public static Texture2D PinkSeekerChild;
        public static Texture2D MenuButtonBlank;
        public static Texture2D CollisionTopLeft;
        public static Texture2D CollisionTopRight;
        public static Texture2D BackgroundParticle;        
        public static Texture2D CollisionBottomLeft;
        public static Texture2D CollisionBottomRight;
        public static Texture2D PowerMoveSpeedIncrease;
        public static Texture2D PowerMoveSpeedDecrease;
        public static Texture2D PowerShootSpeedIncrease;
        public static Texture2D PowerShootSpeedDecrease;

        public static SoundEffect GeomPickup;
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
            Arial24 = content.Load<SpriteFont>("Fonts/Arial24");
            NovaSquare24 = content.Load<SpriteFont>("Fonts/NovaSquare24");
            NovaSquare48 = content.Load<SpriteFont>("Fonts/NovaSquare48");

            Pixel = content.Load<Texture2D>("Textures/Menu/pixel");
            Glow = content.Load<Texture2D>("Textures/Gameplay/Glow");
            Geom = content.Load<Texture2D>("Textures/GamePlay/Geom");
            TileSus = content.Load<Texture2D>("Textures/tiles/tile4");
            Heart = content.Load<Texture2D>("Textures/GamePlay/Heart");
            TileDirt = content.Load<Texture2D>("Textures/tiles/tile2");
            TileGrass = content.Load<Texture2D>("Textures/tiles/tile1");
            TileStone = content.Load<Texture2D>("Textures/tiles/tile3");
            Player = content.Load<Texture2D>("Textures/Gameplay/Player");
            Bullet = content.Load<Texture2D>("Textures/Gameplay/Bullet");
            Default = content.Load<Texture2D>("Textures/Gameplay/Default");
            Pointer = content.Load<Texture2D>("Textures/Gameplay/Pointer");
            PauseBg = content.Load<Texture2D>("Textures/GamePause/pause_bg");
            LineParticle = content.Load<Texture2D>("Textures/Gameplay/Laser");
            SnakeHead = content.Load<Texture2D>("Textures/GamePlay/SnakeHead");
            SnakeBody = content.Load<Texture2D>("Textures/GamePlay/SnakeBody");
            StarBurst = content.Load<Texture2D>("Textures/Gameplay/StarBurst");
            CollisionUp = content.Load<Texture2D>("Textures/tiles/collisionUp");
            BlueSeeker = content.Load<Texture2D>("Textures/Gameplay/BlueSeeker");
            PinkSeeker = content.Load<Texture2D>("Textures/Gameplay/PinkSeeker");
            WelcomeText = content.Load<Texture2D>("Textures/Intro/welcome_text");
            PauseExit = content.Load<Texture2D>("Textures/GamePause/exit_button");
            GreenSeeker = content.Load<Texture2D>("Textures/Gameplay/GreenSeeker");
            CollisionDown = content.Load<Texture2D>("Textures/tiles/collisionDown");
            CollisionLeft = content.Load<Texture2D>("Textures/tiles/collisionLeft");
            PinkWanderer = content.Load<Texture2D>("Textures/Gameplay/PinkWanderer");
            CollisionRight = content.Load<Texture2D>("Textures/tiles/collisionRight");
            PauseResume = content.Load<Texture2D>("Textures/GamePause/resume_button");
            BackAndForther = content.Load<Texture2D>("Textures/GamePlay/BackAndForther");
            MenuButtonBlank = content.Load<Texture2D>("Textures/Menu/menu_button_blank");
            PurpleWanderer = content.Load<Texture2D>("Textures/Gameplay/PurpleWanderer");
            CollisionTopLeft = content.Load<Texture2D>("Textures/tiles/collisionTopLeft");
            PinkSeekerChild = content.Load<Texture2D>("Textures/Gameplay/PinkSeekerChild");
            CollisionTopRight = content.Load<Texture2D>("Textures/tiles/collisionTopRight");
            CollisionBottomLeft = content.Load<Texture2D>("Textures/tiles/collisionBottomLeft");
            BackgroundParticle = content.Load<Texture2D>("Textures/GamePlay/BackgroundParticle");
            CollisionBottomRight = content.Load<Texture2D>("Textures/tiles/collisionBottomRight");
            PowerMoveSpeedIncrease = content.Load<Texture2D>("Textures/GamePlay/PowerMoveSpeedIncrease");
            PowerMoveSpeedDecrease = content.Load<Texture2D>("Textures/GamePlay/PowerMoveSpeedDecrease");
            PowerShootSpeedIncrease = content.Load<Texture2D>("Textures/GamePlay/PowerShootSpeedIncrease");
            PowerShootSpeedDecrease = content.Load<Texture2D>("Textures/GamePlay/PowerShootSpeedDecrease");

            GeomPickup = content.Load<SoundEffect>("Samples/Gameplay/geomPickup");
            PlayerDeath = content.Load<SoundEffect>("Samples/Gameplay/player_death");
            PowerPackPickup = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickup");
            PowerPackPickupBad = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickupBad");

            explosions = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Samples/Gameplay/explosions/explosion-0" + x)).ToArray();
            playerShoots = Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Samples/Gameplay/shoot/shoot-0" + x)).ToArray();
            enemySpawns = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Samples/Gameplay/spawn/spawn-0" + x)).ToArray();
        }
    }
}