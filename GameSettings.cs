using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Mono_Ether {
    public static class GameSettings {
        public const string settingsFilename = "settings.txt";
        public static float MasterVolume;
        public static float MusicVolume;
        public static float SoundEffectVolume;
        public static void ApplyChanges() {
            SoundEffect.MasterVolume = GameSettings.MasterVolume * GameSettings.SoundEffectVolume;
            MediaPlayer.Volume = GameSettings.MasterVolume * GameSettings.MusicVolume;
        }
        public static void LoadSettings() {
            if (File.Exists(settingsFilename)) {
                string[] lines = File.ReadAllText(settingsFilename).Split("\n");
                float.TryParse(lines[0], out MasterVolume);
                float.TryParse(lines[1], out MusicVolume);
                float.TryParse(lines[2], out SoundEffectVolume);
            } else {
                // Default settings
                File.WriteAllText(settingsFilename, "0.05\n0.05\n0.8");
                LoadSettings();
            }
            GameSettings.ApplyChanges();
        }
        public static void SaveSettings() {
            File.WriteAllText(settingsFilename, $"{MasterVolume}\n{MusicVolume}\n{SoundEffectVolume}");
        }
    }
}
