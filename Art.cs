using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public static Texture2D MenuButtonBlank { get; private set; }
        public static Texture2D TileGrass { get; private set; }
        public static Texture2D TileDirt { get; private set; }
        public static Texture2D TileStone { get; private set; }
        public static Texture2D TileSus { get; private set; }
        public static Texture2D CollisionLeft { get; private set; }
        public static Texture2D CollisionUp { get; private set; }
        public static Texture2D CollisionRight { get; private set; }
        public static Texture2D CollisionDown { get; private set; }
        public static Texture2D CollisionTopLeft { get; private set; }
        public static Texture2D CollisionTopRight { get; private set; }
        public static Texture2D CollisionBottomRight { get; private set; }
        public static Texture2D CollisionBottomLeft { get; private set; }
        public static Texture2D PauseBg { get; private set; }
        public static Texture2D PauseExit { get; private set; }
        public static Texture2D PauseResume { get; private set; }
        public static Texture2D PowerShootSpeedIncrease { get; private set; }
        public static Texture2D PowerShootSpeedDecrease { get; private set; }
        public static Texture2D PowerMoveSpeedIncrease { get; private set; }
        public static Texture2D PowerMoveSpeedDecrease { get; private set; }
        public static SoundEffect PowerPackPickup { get; private set; }
        public static SoundEffect PowerPackPickupBad { get; private set; }
        public static Texture2D BackgroundParticle { get; private set; }
        public static Texture2D Heart { get; private set; }
        public static Texture2D SnakeHead { get; private set; }
        public static Texture2D SnakeBody { get; private set; }
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
            MenuButtonBlank = content.Load<Texture2D>("Textures/Menu/menu_button_blank");
            TileGrass = content.Load<Texture2D>("Textures/tiles/tile1");
            TileDirt = content.Load<Texture2D>("Textures/tiles/tile2");
            TileStone = content.Load<Texture2D>("Textures/tiles/tile3");
            TileSus = content.Load<Texture2D>("Textures/tiles/tile4");
            CollisionLeft = content.Load<Texture2D>("Textures/tiles/collisionLeft");
            CollisionUp = content.Load<Texture2D>("Textures/tiles/collisionUp");
            CollisionRight = content.Load<Texture2D>("Textures/tiles/collisionRight");
            CollisionDown = content.Load<Texture2D>("Textures/tiles/collisionDown");
            CollisionTopLeft = content.Load<Texture2D>("Textures/tiles/collisionTopLeft");
            CollisionTopRight = content.Load<Texture2D>("Textures/tiles/collisionTopRight");
            CollisionBottomRight = content.Load<Texture2D>("Textures/tiles/collisionBottomRight");
            CollisionBottomLeft = content.Load<Texture2D>("Textures/tiles/collisionBottomLeft");
            PauseBg = content.Load<Texture2D>("Textures/GamePause/pause_bg");
            PauseExit = content.Load<Texture2D>("Textures/GamePause/exit_button");
            PauseResume = content.Load<Texture2D>("Textures/GamePause/resume_button");
            PowerShootSpeedIncrease = content.Load<Texture2D>("Textures/GamePlay/PowerShootSpeedIncrease");
            PowerShootSpeedDecrease = content.Load<Texture2D>("Textures/GamePlay/PowerShootSpeedDecrease");
            PowerMoveSpeedIncrease = content.Load<Texture2D>("Textures/GamePlay/PowerMoveSpeedIncrease");
            PowerMoveSpeedDecrease = content.Load<Texture2D>("Textures/GamePlay/PowerMoveSpeedDecrease");
            PowerPackPickup = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickup");
            PowerPackPickupBad = content.Load<SoundEffect>("Samples/Gameplay/PowerPackPickupBad");
            BackgroundParticle = content.Load<Texture2D>("Textures/GamePlay/BackgroundParticle");
            Heart = content.Load<Texture2D>("Textures/GamePlay/Heart");
            SnakeHead = content.Load<Texture2D>("Textures/GamePlay/SnakeHead");
            SnakeBody = content.Load<Texture2D>("Textures/GamePlay/SnakeBody");
        }
    }
}