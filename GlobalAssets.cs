using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mono_Ether {
    public static class GlobalAssets {
        // Assets that will be used across multiple parts of the program (as opposed to just one part of the program)
        public static Texture2D Default;
        public static Texture2D ButtonCorner1;
        public static Texture2D ButtonCorner2;
        public static Texture2D ButtonTop;
        public static Texture2D ButtonSide;
        public static Texture2D SliderBall;
        public static Texture2D SwitchOff;
        public static Texture2D SwitchOn;
        public static Texture2D Pixel;
        
        public static SpriteFont NovaSquare24;
        public static SpriteFont NovaSquare48;

        public static SoundEffect Click;
        public static void LoadContent(ContentManager content) {
            Default = content.Load<Texture2D>("Textures/GlobalAssets/Default");
            ButtonCorner1 = content.Load<Texture2D>("Textures/GlobalAssets/ButtonCorner1");
            ButtonCorner2 = content.Load<Texture2D>("Textures/GlobalAssets/ButtonCorner2");
            ButtonTop = content.Load<Texture2D>("Textures/GlobalAssets/ButtonTop");
            ButtonSide = content.Load<Texture2D>("Textures/GlobalAssets/ButtonSide");
            SliderBall = content.Load<Texture2D>("Textures/GlobalAssets/SliderBall");
            SwitchOff = content.Load<Texture2D>("Textures/GlobalAssets/SwitchOff");
            SwitchOn = content.Load<Texture2D>("Textures/GlobalAssets/SwitchOn");
            Pixel = content.Load<Texture2D>("Textures/GlobalAssets/Pixel");

            NovaSquare24 = content.Load<SpriteFont>("Fonts/NovaSquare24");
            NovaSquare48 = content.Load<SpriteFont>("Fonts/NovaSquare48");

            Click = content.Load<SoundEffect>("SoundEffects/Click");
        }
    }
}