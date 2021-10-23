using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Mono_Ether.States;
using System;
using System.Collections.Generic;

namespace Mono_Ether {
    public static class GameSettings {
        public static float MasterVolume = 0.05f;
        public static float MusicVolume = 0.5f;
        public static float SoundEffectVolume = 0.8f;
        public static void ApplyChanges() {
            SoundEffect.MasterVolume = GameSettings.MasterVolume * GameSettings.SoundEffectVolume;
            MediaPlayer.Volume = GameSettings.MasterVolume * GameSettings.MusicVolume;
        }
    }
}
