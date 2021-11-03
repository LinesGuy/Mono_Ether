using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Mono_Ether {
    public static class GameSettings {
        public const string settingsFilename = "settings.txt";
        public static float MasterVolume;
        public static float MusicVolume;
        public static float SoundEffectVolume;
        public static string Difficulty;
        public static void ApplyChanges() {
            SoundEffect.MasterVolume = MasterVolume * SoundEffectVolume;
            MediaPlayer.Volume = MasterVolume * MusicVolume;
            if (Difficulty == "Easy")
                Ether.Enemy.baseHealthMultiplier = 1;
            else if (Difficulty == "Normal")
                Ether.Enemy.baseHealthMultiplier = 2;
            else if (Difficulty == "Hard")
                Ether.Enemy.baseHealthMultiplier = 3;
        }
        public static void LoadSettings() {
            if (File.Exists(settingsFilename)) {
                string[] lines = File.ReadAllText(settingsFilename).Split("\n");
                float.TryParse(lines[0], out MasterVolume);
                float.TryParse(lines[1], out MusicVolume);
                float.TryParse(lines[2], out SoundEffectVolume);
                Difficulty = lines[3];
            } else {
                // Default settings
                File.WriteAllText(settingsFilename, "0.05\n0.05\n0.8\nEasy");
                LoadSettings();
            }
            ApplyChanges();
        }
        public static void SaveSettings() {
            File.WriteAllText(settingsFilename, $"{MasterVolume}\n{MusicVolume}\n{SoundEffectVolume}\n{Difficulty}");
        }
    }
}
