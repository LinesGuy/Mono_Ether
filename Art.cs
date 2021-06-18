using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Mono_Ether
{
    static class Art
    {
        public static Texture2D Pixel { get; private set; }
        // EtherRoot
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
        // IntroWelcome
        public static Texture2D WelcomeText { get; private set; }
        // Main Menu
        public static Texture2D MenuPlayButton { get; private set; }
        public static Texture2D MenuSettingsButton { get; private set; }
        public static Texture2D MenuCreditsButton { get; private set; }
        public static Texture2D MenuExitButton { get; private set; }
        // Pause menu
        //public static Texture2D PauseExitButton { get; private set; }
        

        public static void Load(ContentManager content)
        {
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
            MenuPlayButton = content.Load<Texture2D>("Textures/Menu/play_button");
            MenuSettingsButton = content.Load<Texture2D>("Textures/Menu/settings_button");
            MenuCreditsButton = content.Load<Texture2D>("Textures/Menu/credits_button");
            MenuExitButton = content.Load<Texture2D>("Textures/Menu/exit_button");
        }
    }
}
