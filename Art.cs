﻿using System.Net.Mime;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Mono_Ether {
    static class Art {
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Default { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Seeker { get; private set; }
        public static Texture2D Wanderer { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D StarBurst { get; private set; }
        public static Texture2D Pointer { get; private set; }
        public static Texture2D LineParticle { get; private set; }
        public static Texture2D Glow { get; private set; }
        public static SoundEffect PlayerShoot { get; private set; }
        public static SoundEffect PlayerDeath { get; private set; }
        public static SoundEffect EnemyExplosion { get; private set; }
        public static SpriteFont DebugFont { get; private set; }
        public static Texture2D WelcomeText { get; private set; }
        /*public static Texture2D MenuPlayButton { get; private set; }
        public static Texture2D MenuSettingsButton { get; private set; }
        public static Texture2D MenuCreditsButton { get; private set; }
        public static Texture2D MenuExitButton { get; private set; }
        public static Texture2D MenuBackButton { get; private set; }*/
        public static Texture2D MenuButtonBlank { get; private set; }
        public static Texture2D tileGrass { get; private set; }
        public static Texture2D tileDirt { get; private set; }
        public static Texture2D tileStone { get; private set; }
        public static Texture2D tileSus { get; private set; }
        public static Texture2D collisionLeft { get; private set; }
        public static Texture2D collisionUp { get; private set; }
        public static Texture2D collisionRight { get; private set; }
        public static Texture2D collisionDown { get; private set; }
        public static Texture2D collisionTopLeft { get; private set; }
        public static Texture2D collisionTopRight { get; private set; }
        public static Texture2D collisionBottomRight { get; private set; }
        public static Texture2D collisionBottomLeft { get; private set; }
        public static Texture2D pauseBg { get; private set; }
        public static Texture2D pauseExit { get; private set; }
        public static Texture2D pauseResume { get; private set; }
        public static Texture2D PowerShootSpeedIncrease { get; private set; }
        public static Texture2D PowerShootSpeedDecrease { get; private set; }
        public static Texture2D PowerMoveSpeedIncrease { get; private set; }
        public static Texture2D PowerMoveSpeedDecrease { get; private set; }
        public static SoundEffect PowerPackPickup { get; private set; }
        public static SoundEffect PowerPackPickupBad { get; private set; }
        public static Texture2D BackgroundParticle { get; private set; }
        public static void Load(ContentManager content) {
            Pixel = content.Load<Texture2D>("Textures/Menu/pixel");
            Default = content.Load<Texture2D>("Textures/Gameplay/Default");
            Player = content.Load<Texture2D>("Textures/Gameplay/Player");
            Seeker = content.Load<Texture2D>("Textures/Gameplay/Seeker");
            Wanderer = content.Load<Texture2D>("Textures/Gameplay/Wanderer");
            Bullet = content.Load<Texture2D>("Textures/Gameplay/Bullet");
            StarBurst = content.Load<Texture2D>("Textures/Gameplay/StarBurst");
            Pointer = content.Load<Texture2D>("Textures/Gameplay/Pointer");
            LineParticle = content.Load<Texture2D>("Textures/Gameplay/Laser");
            Glow = content.Load<Texture2D>("Textures/Gameplay/Glow");
            PlayerShoot = content.Load<SoundEffect>("Samples/Gameplay/player_shoot");
            PlayerDeath = content.Load<SoundEffect>("Samples/Gameplay/player_death");
            EnemyExplosion = content.Load<SoundEffect>("Samples/Gameplay/enemy_explosion");
            DebugFont = content.Load<SpriteFont>("Fonts/DebugFont");
            WelcomeText = content.Load<Texture2D>("Textures/Intro/welcome_text");
            /*MenuPlayButton = content.Load<Texture2D>("Textures/Menu/play_button");
            MenuSettingsButton = content.Load<Texture2D>("Textures/Menu/settings_button");
            MenuCreditsButton = content.Load<Texture2D>("Textures/Menu/credits_button");
            MenuExitButton = content.Load<Texture2D>("Textures/Menu/exit_button");
            MenuBackButton = content.Load<Texture2D>("Textures/Menu/back_button");*/
            MenuButtonBlank = content.Load<Texture2D>("Textures/Menu/menu_button_blank");
            tileGrass = content.Load<Texture2D>("Textures/tiles/tile1");
            tileDirt = content.Load<Texture2D>("Textures/tiles/tile2");
            tileStone = content.Load<Texture2D>("Textures/tiles/tile3");
            tileSus = content.Load<Texture2D>("Textures/tiles/tile4");
            collisionLeft = content.Load<Texture2D>("Textures/tiles/collisionLeft");
            collisionUp = content.Load<Texture2D>("Textures/tiles/collisionUp");
            collisionRight = content.Load<Texture2D>("Textures/tiles/collisionRight");
            collisionDown = content.Load<Texture2D>("Textures/tiles/collisionDown");
            collisionTopLeft = content.Load<Texture2D>("Textures/tiles/collisionTopLeft");
            collisionTopRight = content.Load<Texture2D>("Textures/tiles/collisionTopRight");
            collisionBottomRight = content.Load<Texture2D>("Textures/tiles/collisionBottomRight");
            collisionBottomLeft = content.Load<Texture2D>("Textures/tiles/collisionBottomLeft");
            pauseBg = content.Load<Texture2D>("Textures/GamePause/pause_bg");
            pauseExit = content.Load<Texture2D>("Textures/GamePause/exit_button");
            pauseResume = content.Load<Texture2D>("Textures/GamePause/resume_button");
            PowerShootSpeedIncrease = content.Load<Texture2D>("Textures/GamePlay/PowerShootSpeedIncrease");
            PowerShootSpeedDecrease = content.Load<Texture2D>("Textures/GamePlay/PowerShootSpeedDecrease");
            PowerMoveSpeedIncrease = content.Load<Texture2D>("Textures/GamePlay/PowerMoveSpeedIncrease");
            PowerMoveSpeedDecrease = content.Load<Texture2D>("Textures/GamePlay/PowerMoveSpeedDecrease");
            PowerPackPickup = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickup");
            PowerPackPickupBad = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickupBad");
            BackgroundParticle = content.Load<Texture2D>("Textures/GamePlay/BackgroundParticle");
        }
    }
}

