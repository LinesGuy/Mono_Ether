using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using Mono_Ether.States;
using MonoGame.Extended.Input;

namespace Mono_Ether.MainMenu
{
    public class MainMenu : GameState
    {
        private int frame;

        private const int FadeInFrames = 60;
        private float fadeTransparency = 1f;
        private readonly Cookie cookie = new Cookie();
        private readonly Tweener tweener = new Tweener();
        private readonly MenuButton playButton = new MenuButton(GameRoot.ScreenSize / 2f, Art.playButton);
        private readonly MenuButton settingsButton = new MenuButton(GameRoot.ScreenSize / 2f, Art.settingsButton);
        private readonly MenuButton creditsButton = new MenuButton(GameRoot.ScreenSize / 2f, Art.creditsButton);
        private readonly MenuButton exitButton = new MenuButton(GameRoot.ScreenSize / 2f, Art.exitButton);

        private Song welcomeTrack;
        public MainMenu(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(ContentManager content)
        {

            welcomeTrack = content.Load<Song>("Tracks/welcome_track");
            cookie.image = content.Load<Texture2D>("Textures/Menu/cookie");
            MediaPlayer.Volume = 1f;
            MediaPlayer.Play(welcomeTrack);
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            // White fade-in
            if (frame < FadeInFrames)
                fadeTransparency = MathUtil.Interpolate(1f, 0f, (float)frame / FadeInFrames);

            cookie.Update(gameTime);

            if (cookie.state == 1 && Input.mouseState.WasButtonJustDown(MouseButton.Left))
            {
                // Check if any menu buttons were pressed
                if (playButton.CursorInButton())
                {
                    Debug.WriteLine("change screen to game");
                    GameStateManager.Instance.ChangeScreen(new Ether.EtherRoot(_graphicsDevice));
                }
                    
                else if (settingsButton.CursorInButton())
                {
                    Debug.WriteLine("change screen to settings");
                }
                else if (creditsButton.CursorInButton())
                {
                    Debug.WriteLine("change screen to credits");
                }
                else if (exitButton.CursorInButton())
                {
                    Debug.WriteLine("change screen to exit");
                }
            }
            
            if (Input.mouseState.WasButtonJustDown(MouseButton.Left) && cookie.state == 0)
            {
                // Move cookie slightly to left and shrink
                cookie.state = 1;
                tweener.TweenTo(cookie, a => a.position, new Vector2(GameRoot.ScreenSize.X * 0.3f, GameRoot.ScreenSize.Y * 0.5f), duration: 2)
                    .Easing(EasingFunctions.ExponentialOut);
                tweener.TweenTo(cookie, a => a.baseScalar, 0.6f, duration: 2)
                    .Easing(EasingFunctions.ExponentialOut);

                // Move buttons to right
                tweener.TweenTo(playButton, a => a.Position, new Vector2(GameRoot.ScreenSize.X * 0.55f, GameRoot.ScreenSize.Y * 0.2f), duration: 2)
                    .Easing(EasingFunctions.ExponentialOut);
                tweener.TweenTo(settingsButton, a => a.Position, new Vector2(GameRoot.ScreenSize.X * 0.65f, GameRoot.ScreenSize.Y * 0.4f), duration: 2)
                    .Easing(EasingFunctions.ExponentialOut);
                tweener.TweenTo(creditsButton, a => a.Position, new Vector2(GameRoot.ScreenSize.X * 0.65f, GameRoot.ScreenSize.Y * 0.6f), duration: 2)
                    .Easing(EasingFunctions.ExponentialOut);
                tweener.TweenTo(exitButton, a => a.Position, new Vector2(GameRoot.ScreenSize.X * 0.55f, GameRoot.ScreenSize.Y * 0.8f), duration: 2)
                    .Easing(EasingFunctions.ExponentialOut);
            }
            
            playButton.Update(gameTime);

            tweener.Update(gameTime.GetElapsedSeconds());

            frame += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            playButton.Draw(spriteBatch);
            settingsButton.Draw(spriteBatch);
            creditsButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
            
            cookie.Draw(spriteBatch);
            // Fade in
            spriteBatch.Draw(Art.Pixel, new RectangleF(0, 0, GameRoot.ScreenSize.X, GameRoot.ScreenSize.Y).ToRectangle(), Color.White * fadeTransparency);
            spriteBatch.End();
        }
    }
}
