using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mono_Ether
{
    public static class ScreenManager
    {
        public static Stack<GameState> screenStack = new Stack<GameState>();
        public static GameState pendingScreen;
        public static int framesUntilTransition = 0;
        public static int transitionState = 0; // 0 = none, 1 = load screen, -1 = unload screen
        private const int transitionLength = 30; // frames
        public static void Update()
        {
            if (framesUntilTransition > 0)
            {
                framesUntilTransition -= 1;
                if (framesUntilTransition == 0)
                {
                    if (transitionState == 1)
                    {
                        AddScreen(pendingScreen);
                    }
                    else if (transitionState == -1)
                    {
                        RemoveScreen();
                    }
                    transitionState = 0;
                }
            }
            else if (framesUntilTransition < 0)
                framesUntilTransition += 1;
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (framesUntilTransition != 0)
            {
                float transparency;
                if (framesUntilTransition > 0)
                    transparency = 1 - framesUntilTransition / (float)transitionLength;
                else
                    transparency = Math.Abs(framesUntilTransition / (float)transitionLength);

                spriteBatch.Draw(Art.Pixel, new Rectangle(0, 0, (int)GameRoot.ScreenSize.X, (int)GameRoot.ScreenSize.Y), Color.Black * transparency);
            }
        }
        public static void AddScreen(GameState screen)
        {
            screenStack.Push(screen);
            screenStack.Peek().Initialize();
            screenStack.Peek().LoadContent(GameRoot.Instance.Content);
            framesUntilTransition = -transitionLength;
        }
        public static void TransitionScreen(GameState screen)
        {
            pendingScreen = screen;
            transitionState = 1;
            framesUntilTransition = transitionLength;
        }
        private static void RemoveScreen()
        {
            screenStack.Peek().UnloadContent();
            screenStack.Pop();
            if (screenStack.Count == 0)
            {
                GameRoot.Instance.Exit();
            }
            framesUntilTransition = -transitionLength;
        }
        public static void RemoveScreenTransition()
        {
            transitionState = -1;
            framesUntilTransition = transitionLength;
        }
    }
}
