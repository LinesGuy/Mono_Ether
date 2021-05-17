using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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
        public static SpriteFont DebugFont { get; private set; }
        // IntroWelcome
        public static Texture2D welcomeText { get; private set; }

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
            DebugFont = content.Load<SpriteFont>("Fonts/DebugFont");
            welcomeText = content.Load<Texture2D>("Textures/Intro/welcome_text");
        }
    }
}
